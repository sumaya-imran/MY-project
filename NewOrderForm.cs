// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/NewOrderForm.cs
//  Purpose: Point-of-Sale (POS) screen for taking customer orders.
//           Left panel  — browse menu by category/search, double-click to add
//           Right panel — current basket, table number, discount, totals
//           Place Order button saves to database and shows receipt.
// =================================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using VelvetBeans.Database;
using VelvetBeans.Helpers;
using VelvetBeans.Models;

namespace VelvetBeans.Forms
{
    public partial class NewOrderForm : Form
    {
        // In-memory basket — holds all items added before placing the order.
        // Using List<OrderItem> instead of a DataTable means we can easily
        // find, modify, and sum items using LINQ.
        private readonly List<OrderItem> _basket = new List<OrderItem>();

        public NewOrderForm()
        {
            InitializeComponent();   // Builds controls (in Designer.cs)
            LoadCategories();        // Fill category filter dropdown
            LoadMenu();              // Load available menu items into grid
        }

        // =================================================================
        //  LoadCategories — fills the category filter dropdown
        // =================================================================
        private void LoadCategories()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("All Categories");  // Show everything by default

            var dt = DatabaseHelper.Query(
                "SELECT CategoryId, Name FROM Categories ORDER BY Name;");

            // Store as "CategoryId|Name" so we can extract the ID later for filtering
            foreach (DataRow r in dt.Rows)
                cmbCategory.Items.Add(r["CategoryId"] + "|" + r["Name"]);

            cmbCategory.SelectedIndex = 0;
        }

        // =================================================================
        //  LoadMenu — queries available items and binds to the left grid.
        //  Applies optional category filter and search text.
        // =================================================================
        private void LoadMenu()
        {
            // Build optional WHERE clause for category filter
            string catWhere = string.Empty;
            if (cmbCategory.SelectedIndex > 0)
            {
                string cid = (cmbCategory.SelectedItem?.ToString() ?? "").Split('|')[0];
                catWhere = " AND m.CategoryId = " + cid;
            }

            string search = txtSearch.Text.Trim();

            // Trailing space on each fragment prevents token run-ons in SQL
            var dt = DatabaseHelper.Query(
                "SELECT m.ItemId       AS ItemId,      " +
                "       c.Name         AS Category,    " +
                "       m.Name         AS Item,        " +
                "       m.Description  AS Description, " +
                "       m.Price        AS Price        " +
                "FROM   MenuItems m                    " +
                "JOIN   Categories c ON c.CategoryId = m.CategoryId " +
                "WHERE  m.IsAvailable = 1 " +    // Only show items currently available
                catWhere +
                " AND (m.Name LIKE @s OR c.Name LIKE @s) " +
                "ORDER BY c.Name, m.Name;",
                new[] { new SqliteParameter("@s", "%" + search + "%") });

            gridMenu.DataSource = dt;

            // Hide ItemId — it's internal, the cashier doesn't need to see it
            if (gridMenu.Columns.Contains("ItemId"))
                gridMenu.Columns["ItemId"]!.Visible = false;
        }

        // =================================================================
        //  AddToBasket — adds the currently selected menu item to the basket.
        //  If the item is already in the basket, increments its quantity.
        // =================================================================
        private void AddToBasket()
        {
            if (gridMenu.CurrentRow == null) return;

            var row    = gridMenu.CurrentRow;
            int    id  = DatabaseHelper.ParseInt(row.Cells["ItemId"].Value);
            string nm  = row.Cells["Item"].Value?.ToString() ?? string.Empty;
            decimal pr = DatabaseHelper.ParseDecimal(row.Cells["Price"].Value);

            // Check if this item is already in the basket
            var existing = _basket.Find(x => x.ItemId == id);
            if (existing != null)
            {
                // Already there — just increase quantity by 1
                existing.Quantity++;
            }
            else
            {
                // Not in basket yet — add a new OrderItem
                _basket.Add(new OrderItem
                {
                    ItemId    = id,
                    ItemName  = nm,
                    UnitPrice = pr,
                    Quantity  = 1
                });
            }

            RefreshBasket();
        }

        // =================================================================
        //  RefreshBasket — rebuilds the basket DataGridView from the
        //  in-memory _basket list and updates the totals labels.
        // =================================================================
        private void RefreshBasket()
        {
            // Build a fresh DataTable from the basket list
            var dt = new DataTable();
            dt.Columns.Add("Item");
            dt.Columns.Add("Qty",      typeof(int));
            dt.Columns.Add("Price",    typeof(decimal));
            dt.Columns.Add("Subtotal", typeof(decimal));

            foreach (var it in _basket)
                dt.Rows.Add(it.ItemName, it.Quantity, it.UnitPrice, it.Subtotal);

            gridBasket.DataSource = dt;
            RefreshTotals();
        }

        // =================================================================
        //  RefreshTotals — recalculates and displays Subtotal, Discount, Total
        // =================================================================
        private void RefreshTotals()
        {
            // LINQ Sum over all basket items
            decimal sub  = _basket.Sum(x => x.Subtotal);
            decimal disc = numDiscount.Value;
            decimal tot  = Math.Max(0m, sub - disc);  // Total can't go below 0

            lblSubtotal.Text = string.Format("Subtotal :   {0,10:F2}", sub);
            lblDiscount.Text = string.Format("Discount :  -{0,10:F2}", disc);
            lblTotal.Text    = string.Format("TOTAL    :   {0,10:F2}", tot);
        }

        // =================================================================
        //  Event handlers — wired up in Designer.cs
        // =================================================================

        // Category dropdown changed — reload the menu with the new filter
        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
            => LoadMenu();

        // Search text changed — reload the menu filtered by search term
        private void txtSearch_TextChanged(object sender, EventArgs e)
            => LoadMenu();

        // Double-click on a menu row — add that item to the basket
        private void gridMenu_DoubleClick(object sender, EventArgs e)
            => AddToBasket();

        // Add button clicked — same as double-clicking a row
        private void btnAdd_Click(object sender, EventArgs e)
            => AddToBasket();

        // Discount value changed — refresh totals only
        private void numDiscount_ValueChanged(object sender, EventArgs e)
            => RefreshTotals();

        // Remove button — removes the currently selected basket line
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (gridBasket.CurrentRow == null) return;
            int idx = gridBasket.CurrentRow.Index;
            if (idx >= 0 && idx < _basket.Count)
            {
                _basket.RemoveAt(idx);
                RefreshBasket();
            }
        }

        // =================================================================
        //  PlaceOrder — saves the order to the database and shows a receipt.
        //  Uses a database TRANSACTION so that if anything fails midway,
        //  no partial data is saved (all-or-nothing).
        // =================================================================
        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (_basket.Count == 0)
            { Msg.Error("Please add at least one item before placing the order."); return; }

            decimal subtotal = _basket.Sum(x => x.Subtotal);
            decimal discount = numDiscount.Value;

            // Open ONE connection and begin a transaction.
            // All database writes share this connection and transaction.
            // If any step fails, tx.Rollback() undoes everything.
            using (var con = DatabaseHelper.OpenConnection())
            using (var tx  = con.BeginTransaction())
            {
                try
                {
                    // Step 1: Insert the Order header row
                    DatabaseHelper.Run(con, tx,
                        "INSERT INTO Orders " +
                        "(UserId, TableNumber, Status, TotalAmount, Discount) " +
                        "VALUES (@uid, @tbl, 'Pending', @amt, @disc);",
                        new[]
                        {
                            // Session.CurrentUser! — we know it's not null because login succeeded
                            new SqliteParameter("@uid",  Session.CurrentUser!.UserId),
                            new SqliteParameter("@tbl",  (int)numTable.Value),
                            new SqliteParameter("@amt",  (double)subtotal),   // Cast to double for SQLite REAL
                            new SqliteParameter("@disc", (double)discount)
                        });

                    // Get the auto-generated OrderId of the row we just inserted
                    long orderId = DatabaseHelper.LastId(con, tx);

                    // Step 2: Insert one OrderItem row for every basket line
                    foreach (var item in _basket)
                    {
                        DatabaseHelper.Run(con, tx,
                            "INSERT INTO OrderItems (OrderId, ItemId, Quantity, UnitPrice) " +
                            "VALUES (@oid, @iid, @qty, @up);",
                            new[]
                            {
                                new SqliteParameter("@oid", orderId),
                                new SqliteParameter("@iid", item.ItemId),
                                new SqliteParameter("@qty", item.Quantity),
                                new SqliteParameter("@up",  (double)item.UnitPrice)
                            });
                    }

                    // Step 3: Commit — makes all changes permanent
                    tx.Commit();

                    // Step 4: Build receipt model by re-reading the saved order
                    var order = BuildReceiptOrder((int)orderId);

                    // Step 5: Show the receipt dialog
                    new ReceiptForm(order).ShowDialog(this);

                    // Step 6: Clear everything for the next order
                    _basket.Clear();
                    numDiscount.Value = 0m;
                    numTable.Value    = 0m;
                    RefreshBasket();
                }
                catch (Exception ex)
                {
                    // Something went wrong — roll back all changes
                    tx.Rollback();
                    Msg.Error("Failed to place order:\n" + ex.Message);
                }
            }
        }

        // =================================================================
        //  BuildReceiptOrder — re-reads the saved order from the database
        //  and constructs an Order model object for the ReceiptForm.
        //  Uses explicit column aliases (no SELECT *) to avoid ConstraintException.
        // =================================================================
        private static Order BuildReceiptOrder(int orderId)
        {
            // Get order header + cashier name (joined from Users)
            var odt = DatabaseHelper.Query(
                "SELECT o.OrderId, o.TableNumber, o.TotalAmount, " +
                "       o.Discount, o.CreatedAt, u.Username " +
                "FROM   Orders o " +
                "JOIN   Users  u ON u.UserId = o.UserId " +
                "WHERE  o.OrderId = @id;",
                new[] { new SqliteParameter("@id", orderId) });

            // Get all line items with item names (joined from MenuItems)
            var idt = DatabaseHelper.Query(
                "SELECT m.Name AS ItemName, oi.Quantity, oi.UnitPrice " +
                "FROM   OrderItems oi " +
                "JOIN   MenuItems  m ON m.ItemId = oi.ItemId " +
                "WHERE  oi.OrderId = @id;",
                new[] { new SqliteParameter("@id", orderId) });

            var row = odt.Rows[0];
            var order = new Order
            {
                OrderId     = DatabaseHelper.ParseInt(row["OrderId"]),
                CashierName = row["Username"]?.ToString()    ?? string.Empty,
                TableNumber = DatabaseHelper.ParseInt(row["TableNumber"]),
                TotalAmount = DatabaseHelper.ParseDecimal(row["TotalAmount"]),
                Discount    = DatabaseHelper.ParseDecimal(row["Discount"]),
                CreatedAt   = row["CreatedAt"]?.ToString()   ?? string.Empty
            };

            foreach (DataRow ir in idt.Rows)
                order.Items.Add(new OrderItem
                {
                    ItemName  = ir["ItemName"]?.ToString()  ?? string.Empty,
                    Quantity  = DatabaseHelper.ParseInt(ir["Quantity"]),
                    UnitPrice = DatabaseHelper.ParseDecimal(ir["UnitPrice"])
                });

            return order;
        }
    }
}
