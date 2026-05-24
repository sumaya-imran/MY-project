// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/OrderHistoryForm.cs
//  Purpose: Browse all orders with date range and status filters.
//           Left grid   — list of orders
//           Right panel — line items for the selected order
//           Admins can mark orders as Completed or Cancelled.
// =================================================================

using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using VelvetBeans.Database;
using VelvetBeans.Helpers;
using VelvetBeans.Models;

namespace VelvetBeans.Forms
{
    public partial class OrderHistoryForm : Form
    {
        // Guard flag — prevents SelectionChanged from running while
        // LoadOrders() is rebinding the DataSource of the orders grid.
        private bool _loading = false;

        public OrderHistoryForm()
        {
            InitializeComponent();
            LoadOrders();  // Populate grid with orders on open
        }

        // =================================================================
        //  LoadOrders — queries orders within the selected date range
        //  and status filter, then binds the result to the grid.
        // =================================================================
        private void LoadOrders()
        {
            _loading = true;

            // Reset the detail panel while loading new data
            lblDetail.Text        = "Select an order to view its items.";
            gridItems.DataSource  = null;

            // Build optional status filter clause.
            // Starts with " AND " so it safely appends after the WHERE clause.
            string sc = (cmbStatus.SelectedItem?.ToString() == "All")
                ? string.Empty
                : " AND o.Status = '" + cmbStatus.SelectedItem + "'";

            var p = new[]
            {
                new SqliteParameter("@from", dtpFrom.Value.ToString("yyyy-MM-dd")),
                new SqliteParameter("@to",   dtpTo.Value.ToString("yyyy-MM-dd"))
            };

            // All aliases are plain single words — spaces/# in aliases break cell lookup
            var dt = DatabaseHelper.Query(
                "SELECT o.OrderId                       AS OrderId,   " +
                "       u.Username                      AS Cashier,   " +
                "       o.TableNumber                   AS TableNo,   " +
                "       o.Status                        AS Status,    " +
                "       o.TotalAmount                   AS Subtotal,  " +
                "       o.Discount                      AS Discount,  " +
                "       (o.TotalAmount - o.Discount)    AS GrandTotal," +
                "       o.CreatedAt                     AS OrderDate  " +
                "FROM   Orders o                                       " +
                "JOIN   Users  u ON u.UserId = o.UserId               " +
                "WHERE  date(o.CreatedAt) BETWEEN date(@from) AND date(@to) " +
                sc +
                " ORDER BY o.CreatedAt DESC;", p);

            gridOrders.DataSource = dt;
            _loading = false;
        }

        // =================================================================
        //  Event: A row is selected in the orders grid.
        //  Loads and shows the line items for that order.
        // =================================================================
        private void gridOrders_SelectionChanged(object sender, EventArgs e)
        {
            // Guard: skip while grid is being loaded to avoid null ref errors
            if (_loading) return;
            if (gridOrders.CurrentRow == null) return;

            var row = gridOrders.CurrentRow;

            // All cell values are strings (DatabaseHelper.Query returns strings)
            int    orderId   = DatabaseHelper.ParseInt(row.Cells["OrderId"].Value);
            string cashier   = row.Cells["Cashier"].Value?.ToString()    ?? string.Empty;
            string tableNo   = row.Cells["TableNo"].Value?.ToString()    ?? "0";
            string status    = row.Cells["Status"].Value?.ToString()     ?? string.Empty;
            string grandTot  = row.Cells["GrandTotal"].Value?.ToString() ?? "0";
            string orderDate = row.Cells["OrderDate"].Value?.ToString()  ?? string.Empty;

            // Update the detail summary label
            lblDetail.Text =
                "Order #" + orderId + "   Cashier: " + cashier + "\n" +
                "Table: " + (tableNo == "0" ? "Takeaway" : tableNo) +
                "   Status: " + status + "\n" +
                "Grand Total: £" + grandTot + "   Date: " + orderDate;

            // Load the line items for this order into the right-side grid
            gridItems.DataSource = DatabaseHelper.Query(
                "SELECT m.Name                         AS Item,     " +
                "       oi.Quantity                    AS Qty,      " +
                "       oi.UnitPrice                   AS UnitPrice," +
                "       (oi.Quantity * oi.UnitPrice)   AS Subtotal  " +
                "FROM   OrderItems oi                               " +
                "JOIN   MenuItems  m ON m.ItemId = oi.ItemId        " +
                "WHERE  oi.OrderId = @id;",
                new[] { new SqliteParameter("@id", orderId) });
        }

        // =================================================================
        //  SetStatus — updates an order's status in the database.
        //  Only called from the Completed and Cancelled buttons.
        //  Only enabled for Admin users (set in Designer.cs).
        // =================================================================
        private void SetStatus(string newStatus)
        {
            if (gridOrders.CurrentRow == null)
            { Msg.Error("Please select an order first."); return; }

            int    orderId = DatabaseHelper.ParseInt(gridOrders.CurrentRow.Cells["OrderId"].Value);
            string current = gridOrders.CurrentRow.Cells["Status"].Value?.ToString() ?? string.Empty;

            // Can't change status of already-finished orders
            if (current == "Completed" || current == "Cancelled")
            { Msg.Info("This order is already " + current + "."); return; }

            if (!Msg.Confirm("Mark order #" + orderId + " as " + newStatus + "?")) return;

            // Set CompletedAt timestamp when completing (not for cancellations)
            string extra = (newStatus == "Completed")
                ? ", CompletedAt = datetime('now')" : string.Empty;

            DatabaseHelper.Execute(
                "UPDATE Orders SET Status = @s" + extra + " WHERE OrderId = @id;",
                new[] { new SqliteParameter("@s", newStatus), new SqliteParameter("@id", orderId) });

            Msg.Info("Order #" + orderId + " marked as " + newStatus + ".");
            LoadOrders();  // Refresh the grid to show the new status
        }

        // Event handlers — wired in Designer.cs
        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e) => LoadOrders();
        private void dtpFrom_ValueChanged(object sender, EventArgs e)           => LoadOrders();
        private void dtpTo_ValueChanged(object sender, EventArgs e)             => LoadOrders();
        private void btnRefresh_Click(object sender, EventArgs e)               => LoadOrders();
        private void btnComplete_Click(object sender, EventArgs e)              => SetStatus("Completed");
        private void btnCancel_Click(object sender, EventArgs e)                => SetStatus("Cancelled");
        private void btnClose_Click(object sender, EventArgs e)                 => Close();
    }
}
