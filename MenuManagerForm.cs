// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/MenuManagerForm.cs
//  Purpose: Admin-only screen for managing menu items.
//           Full CRUD on Items: Add / Edit / Delete / Toggle Available.
//           Categories are shown in a filter dropdown but are NOT
//           editable here (they are managed directly in DB Browser).
//
//  Layout:
//    Left  — DataGridView showing all menu items (filterable by category)
//    Right — Add/Edit panel with fields: Name, Description, Price,
//             Category, Available checkbox
//    Bottom buttons — Save Item, Clear Form, Delete Selected
// =================================================================

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using VelvetBeans.Database;
using VelvetBeans.Helpers;

namespace VelvetBeans.Forms
{
    public partial class MenuManagerForm : Form
    {
        // -----------------------------------------------------------------
        //  State fields
        // -----------------------------------------------------------------

        // _editId tracks which item is selected for editing.
        // Value = -1 means "no item selected, we are in ADD mode".
        // Value > 0 means "editing the item with this ItemId".
        private int _editId = -1;

        // _gridLoading prevents the SelectionChanged event from firing
        // while we are programmatically binding data to the grid.
        // Without this guard, changing DataSource triggers SelectionChanged,
        // which tries to read cells that don't exist yet — causing crashes.
        private bool _gridLoading = false;

        public MenuManagerForm()
        {
            InitializeComponent();  // Builds all controls (in Designer.cs)
            LoadCategoryFilter();   // Fill the category dropdown
            LoadItems();            // Load all menu items into the grid
        }

        // =================================================================
        //  LOAD: Fill category filter dropdown
        //  Called once on load and after any category changes.
        // =================================================================
        private void LoadCategoryFilter()
        {
            cmbFilter.Items.Clear();
            cmbFilter.Items.Add("All Categories");  // Default "show everything" option

            // Get all categories from the database ordered by name
            var dt = DatabaseHelper.Query(
                "SELECT CategoryId, Name FROM Categories ORDER BY Name;");

            // Each item in the dropdown stores "CategoryId|Name"
            // We split on '|' later to extract the ID for filtering
            foreach (DataRow r in dt.Rows)
                cmbFilter.Items.Add(r["CategoryId"] + "|" + r["Name"]);

            cmbFilter.SelectedIndex = 0;  // Select "All Categories" by default

            // Also reload the category dropdown in the edit panel
            LoadCategoryCombo();
        }

        // =================================================================
        //  LOAD: Fill the category dropdown in the edit panel
        //  Used when saving an item — user must pick which category it belongs to.
        // =================================================================
        private void LoadCategoryCombo()
        {
            cmbCategory.Items.Clear();

            var dt = DatabaseHelper.Query(
                "SELECT CategoryId, Name FROM Categories ORDER BY Name;");

            foreach (DataRow r in dt.Rows)
                cmbCategory.Items.Add(r["CategoryId"] + "|" + r["Name"]);

            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
        }

        // =================================================================
        //  READ: Load menu items into the grid.
        //  Applies optional category filter from the filter dropdown.
        //
        //  SQL note: All column aliases use simple single words.
        //  DataGridView uses the alias as the column.Name for cell lookup.
        //  Special characters (#, spaces) in aliases cause KeyNotFoundException.
        // =================================================================
        private void LoadItems()
        {
            // Raise the guard flag so SelectionChanged ignores events
            // while we are rebinding the DataSource
            _gridLoading = true;

            // Build the optional WHERE clause for category filtering.
            // catWhere starts with " AND " which safely appends after "WHERE 1=1 "
            string catWhere = string.Empty;
            if (cmbFilter.SelectedIndex > 0)
            {
                // Extract the CategoryId from the "123|Hot Drinks" format
                string cid = (cmbFilter.SelectedItem?.ToString() ?? "").Split('|')[0];
                catWhere = " AND m.CategoryId = " + cid;
            }

            // Fetch items with their category name joined in.
            // Every SQL fragment ends with a trailing space to prevent
            // accidental token run-ons like "= 1ORDER BY" which crash SQLite.
            var dt = DatabaseHelper.Query(
                "SELECT m.ItemId          AS ItemId,   " +
                "       c.Name            AS Category, " +
                "       m.Name            AS ItemName, " +
                "       m.Description     AS ItemDesc, " +
                "       m.Price           AS Price,    " +
                "       CASE m.IsAvailable " +
                "           WHEN 1 THEN 'Yes' ELSE 'No' " +
                "       END               AS Available " +
                "FROM   MenuItems m " +
                "JOIN   Categories c ON c.CategoryId = m.CategoryId " +
                "WHERE  1 = 1 " +           // Always-true base condition (makes AND appending easy)
                catWhere +
                " ORDER BY c.Name, m.Name;"); // Space before ORDER is critical

            gridItems.DataSource = dt;

            // Hide the raw ItemId column — it's internal, users don't need to see it
            if (gridItems.Columns.Contains("ItemId"))
                gridItems.Columns["ItemId"]!.Visible = false;

            // Lower the guard and reset the edit panel
            _gridLoading = false;
            ClearForm();
        }

        // =================================================================
        //  Event: Category filter changed — reload the grid
        // =================================================================
        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Don't reload if we are currently in the middle of loading
            if (!_gridLoading) LoadItems();
        }

        // =================================================================
        //  Event: User clicked a row in the grid
        //  Populate the edit panel fields with that item's data.
        // =================================================================
        private void gridItems_SelectionChanged(object sender, EventArgs e)
        {
            // Ignore if we are loading data (grid is being re-bound)
            if (_gridLoading) return;
            if (gridItems.CurrentRow == null) return;

            var row = gridItems.CurrentRow;

            // Remember which item we are editing (used in Save to decide INSERT vs UPDATE)
            _editId = DatabaseHelper.ParseInt(row.Cells["ItemId"].Value);

            // Fill text fields — use null-coalescing in case the cell is empty
            txtName.Text = row.Cells["ItemName"].Value?.ToString() ?? string.Empty;
            txtDesc.Text = row.Cells["ItemDesc"].Value?.ToString() ?? string.Empty;

            // ParseDecimal uses InvariantCulture so "3.50" always parses correctly
            // even on European Windows systems that use a comma as decimal separator
            decimal price = DatabaseHelper.ParseDecimal(row.Cells["Price"].Value);
            // Clamp price to the NumericUpDown's valid range before assigning
            if (price < numPrice.Minimum) price = numPrice.Minimum;
            if (price > numPrice.Maximum) price = numPrice.Maximum;
            numPrice.Value = price;

            // Tick the checkbox based on the "Yes"/"No" string in the grid
            chkAvailable.Checked = (row.Cells["Available"].Value?.ToString() == "Yes");

            // Select the correct category in the dropdown
            string cat = row.Cells["Category"].Value?.ToString() ?? string.Empty;
            for (int i = 0; i < cmbCategory.Items.Count; i++)
            {
                // Each item is stored as "CategoryId|CategoryName"
                string[] parts = (cmbCategory.Items[i]?.ToString() ?? "").Split('|');
                if (parts.Length > 1 && parts[1] == cat)
                {
                    cmbCategory.SelectedIndex = i;
                    break;
                }
            }

            // Change the Save button text to show we are in edit mode
            btnSave.Text = "Update Item";
        }

        // =================================================================
        //  CREATE / UPDATE: Save button clicked
        //  If _editId == -1  → INSERT new item
        //  If _editId > 0    → UPDATE existing item
        // =================================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate required fields before touching the database
            if (string.IsNullOrWhiteSpace(txtName.Text))
            { Msg.Error("Item name is required."); return; }

            if (cmbCategory.SelectedIndex < 0)
            { Msg.Error("Please select a category."); return; }

            // Extract CategoryId from the "123|Hot Drinks" format
            int catId = 0;
            int.TryParse((cmbCategory.SelectedItem?.ToString() ?? "0|").Split('|')[0], out catId);
            if (catId == 0) { Msg.Error("Invalid category selected."); return; }

            // Convert checkbox state to SQLite integer (1 = available, 0 = not)
            int avail = chkAvailable.Checked ? 1 : 0;

            // numPrice.Value is already a decimal — cast to double for SQLite REAL type
            double price = (double)numPrice.Value;

            if (_editId == -1)
            {
                // ---- INSERT: Adding a brand new menu item ----------------
                DatabaseHelper.Execute(
                    "INSERT INTO MenuItems " +
                    "(CategoryId, Name, Description, Price, IsAvailable) " +
                    "VALUES (@c, @n, @d, @p, @a);",
                    new[]
                    {
                        new SqliteParameter("@c", catId),
                        new SqliteParameter("@n", txtName.Text.Trim()),
                        new SqliteParameter("@d", txtDesc.Text.Trim()),
                        new SqliteParameter("@p", price),
                        new SqliteParameter("@a", avail)
                    });
                Msg.Info("Item added successfully.");
            }
            else
            {
                // ---- UPDATE: Saving changes to an existing item ----------
                DatabaseHelper.Execute(
                    "UPDATE MenuItems " +
                    "SET CategoryId = @c, Name = @n, Description = @d, " +
                    "    Price = @p, IsAvailable = @a " +
                    "WHERE ItemId = @id;",
                    new[]
                    {
                        new SqliteParameter("@c",  catId),
                        new SqliteParameter("@n",  txtName.Text.Trim()),
                        new SqliteParameter("@d",  txtDesc.Text.Trim()),
                        new SqliteParameter("@p",  price),
                        new SqliteParameter("@a",  avail),
                        new SqliteParameter("@id", _editId)   // Target the specific item
                    });
                Msg.Info("Item updated successfully.");
            }

            // Refresh the grid and reset the form after saving
            LoadCategoryFilter();
            LoadItems();
        }

        // =================================================================
        //  DELETE: Remove the currently selected item from the database.
        // =================================================================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editId == -1)
            { Msg.Error("Please select an item from the list to delete."); return; }

            // Confirm before deleting — this cannot be undone
            if (!Msg.Confirm("Delete '" + txtName.Text + "'?\nThis action cannot be undone."))
                return;

            DatabaseHelper.Execute(
                "DELETE FROM MenuItems WHERE ItemId = @id;",
                new[] { new SqliteParameter("@id", _editId) });

            Msg.Info("Item deleted successfully.");
            LoadCategoryFilter();
            LoadItems();
        }

        // =================================================================
        //  CLEAR: Reset the edit panel back to "Add new item" state.
        //  Called after Save, Delete, or when user clicks Clear button.
        // =================================================================
        private void btnClear_Click(object sender, EventArgs e) => ClearForm();

        private void ClearForm()
        {
            _editId           = -1;             // Reset to "add new" mode
            txtName.Text      = string.Empty;
            txtDesc.Text      = string.Empty;
            numPrice.Value    = 0m;
            chkAvailable.Checked = true;        // New items default to available
            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
            btnSave.Text = "Save Item";         // Reset button text
        }

        // =================================================================
        //  Close button
        // =================================================================
        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
