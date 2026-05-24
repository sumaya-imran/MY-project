// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/UserManagerForm.cs
//  Purpose: Admin-only screen for managing staff accounts.
//           Full CRUD: Add / Edit / Delete users.
//           Cannot delete your own account (safety check).
//           Roles: "Admin" (full access) or "Cashier" (limited).
//
//  Layout (built in Designer.cs):
//    Left  — DataGridView listing all users
//    Right — Add/Edit panel with Username, Password, Role fields
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
    public partial class UserManagerForm : Form
    {
        // -----------------------------------------------------------------
        //  _editId tracks which user is currently selected in the grid.
        //  -1 means "no user selected — we are in ADD NEW mode".
        //  Any positive value means "we are EDITING the user with this ID".
        // -----------------------------------------------------------------
        private int _editId = -1;

        // Guard flag: prevents SelectionChanged from firing while
        // we are programmatically rebinding the grid's DataSource.
        private bool _gridLoading = false;

        public UserManagerForm()
        {
            InitializeComponent();  // Builds all controls (in Designer.cs)
            LoadUsers();            // Fill the grid with existing users
        }

        // =================================================================
        //  READ: Load all users from the database into the grid.
        //  Uses alias "UID" instead of "UserId" to avoid any conflict
        //  with DataTable's internal column naming.
        // =================================================================
        private void LoadUsers()
        {
            _gridLoading = true;

            var dt = DatabaseHelper.Query(
                "SELECT UserId    AS UID,       " +
                "       Username  AS Username,  " +
                "       Role      AS Role,      " +
                "       CreatedAt AS CreatedAt  " +
                "FROM   Users                   " +
                "ORDER  BY Role, Username;");   // Admins first, then cashiers

            gridUsers.DataSource = dt;

            // Hide the raw UserId — it's internal, shown only in _editId
            if (gridUsers.Columns.Contains("UID"))
                gridUsers.Columns["UID"]!.Visible = false;

            _gridLoading = false;
            ClearForm();
        }

        // =================================================================
        //  Event: Row selected in the grid.
        //  Populates the edit panel with that user's data.
        // =================================================================
        private void gridUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (_gridLoading) return;
            if (gridUsers.CurrentRow == null) return;

            var row = gridUsers.CurrentRow;

            // Store the selected user's ID so Save knows to UPDATE not INSERT
            _editId = DatabaseHelper.ParseInt(row.Cells["UID"].Value);

            // Populate the text fields
            txtUsername.Text = row.Cells["Username"].Value?.ToString() ?? string.Empty;

            // NEVER pre-fill the password field — security best practice.
            // If user leaves it blank on save, we keep the existing password.
            txtPassword.Clear();

            // Select the matching role in the dropdown
            string role = row.Cells["Role"].Value?.ToString() ?? "Cashier";
            cmbRole.SelectedItem = role;
            if (cmbRole.SelectedIndex < 0)
                cmbRole.SelectedIndex = 1; // Fallback to "Cashier"

            // Change button text to show we are in Edit mode
            btnSave.Text = "Update User";
        }

        // =================================================================
        //  CREATE / UPDATE: Save button clicked.
        //  _editId == -1  → INSERT a new user
        //  _editId > 0    → UPDATE the existing user
        // =================================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate: username is always required
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            { Msg.Error("Username is required."); return; }

            // Validate: password required only when creating a NEW user
            if (_editId == -1 && string.IsNullOrWhiteSpace(txtPassword.Text))
            { Msg.Error("Password is required when creating a new user."); return; }

            string role = cmbRole.SelectedItem?.ToString() ?? "Cashier";

            try
            {
                if (_editId == -1)
                {
                    // ---- INSERT: Create a new staff account ---------
                    DatabaseHelper.Execute(
                        "INSERT INTO Users (Username, Password, Role) " +
                        "VALUES (@u, @p, @r);",
                        new[]
                        {
                            new SqliteParameter("@u", txtUsername.Text.Trim()),
                            new SqliteParameter("@p", txtPassword.Text),
                            new SqliteParameter("@r", role)
                        });
                    Msg.Info("User '" + txtUsername.Text.Trim() + "' created successfully.");
                }
                else if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    // ---- UPDATE: Change username/role but keep old password --
                    // Password field was left blank — do NOT overwrite it
                    DatabaseHelper.Execute(
                        "UPDATE Users SET Username = @u, Role = @r " +
                        "WHERE UserId = @id;",
                        new[]
                        {
                            new SqliteParameter("@u",  txtUsername.Text.Trim()),
                            new SqliteParameter("@r",  role),
                            new SqliteParameter("@id", _editId)
                        });
                    Msg.Info("User updated (password unchanged).");
                }
                else
                {
                    // ---- UPDATE: Change everything including password --
                    DatabaseHelper.Execute(
                        "UPDATE Users SET Username = @u, Password = @p, Role = @r " +
                        "WHERE UserId = @id;",
                        new[]
                        {
                            new SqliteParameter("@u",  txtUsername.Text.Trim()),
                            new SqliteParameter("@p",  txtPassword.Text),
                            new SqliteParameter("@r",  role),
                            new SqliteParameter("@id", _editId)
                        });
                    Msg.Info("User updated successfully.");
                }
            }
            catch
            {
                // SQLite throws if the UNIQUE constraint on Username is violated
                Msg.Error("That username already exists. Please choose a different one.");
                return;
            }

            // Reload the grid and reset the edit panel after saving
            LoadUsers();
        }

        // =================================================================
        //  DELETE: Remove the selected user from the database.
        //  Safety checks:
        //    1. Cannot delete yourself (you'd be locked out)
        //    2. Asks for confirmation before deleting
        // =================================================================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editId == -1)
            { Msg.Error("Please select a user from the list to delete."); return; }

            // Safety: prevent self-deletion — would lock the admin out
            if (_editId == Session.CurrentUser?.UserId)
            { Msg.Error("You cannot delete your own account while logged in."); return; }

            if (!Msg.Confirm("Delete user '" + txtUsername.Text + "'?\nThis cannot be undone."))
                return;

            DatabaseHelper.Execute(
                "DELETE FROM Users WHERE UserId = @id;",
                new[] { new SqliteParameter("@id", _editId) });

            Msg.Info("User '" + txtUsername.Text + "' deleted.");
            LoadUsers();
        }

        // =================================================================
        //  CLEAR: Reset the edit panel back to "Add new user" state.
        // =================================================================
        private void btnClear_Click(object sender, EventArgs e) => ClearForm();

        private void ClearForm()
        {
            _editId = -1;               // Switch back to ADD mode
            txtUsername.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = 1;  // Default to "Cashier" for new accounts
            btnSave.Text = "Save User";
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
