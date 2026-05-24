// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/LoginForm.cs
//  Purpose: The first screen shown when the app starts.
//           Checks the username and password against the database.
//           On success: stores the user in Session and opens Dashboard.
//
//  Default accounts (created automatically on first run):
//    Username: admin     Password: admin123     Role: Admin
//    Username: cashier   Password: cashier123   Role: Cashier
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
    // partial means this class is split across two files:
    // LoginForm.cs (this file — logic/events)
    // LoginForm.Designer.cs (controls layout, InitializeComponent)
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            // InitializeComponent() is in LoginForm.Designer.cs
            // It creates all the buttons, textboxes, labels etc.
            InitializeComponent();
        }

        // -----------------------------------------------------------------
        //  btnLogin_Click — fires when user clicks "Sign In"
        // -----------------------------------------------------------------
        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty; // Clear previous error

            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;

            // Validate inputs before hitting the database
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                lblError.Text = "Please enter both username and password.";
                return;
            }

            // Query the database for a matching username + password combination.
            // Using parameterised query (@u, @p) prevents SQL injection.
            var dt = DatabaseHelper.Query(
                "SELECT UserId, Username, Password, Role, CreatedAt " +
                "FROM Users WHERE Username = @u AND Password = @p;",
                new[]
                {
                    new SqliteParameter("@u", user),
                    new SqliteParameter("@p", pass)
                });

            // If no rows came back, credentials are wrong
            if (dt.Rows.Count == 0)
            {
                lblError.Text = "Invalid username or password.";
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            // Credentials matched — populate the Session with this user's info.
            // Session is a static class, so any form in the app can read it.
            DataRow row = dt.Rows[0];
            Session.CurrentUser = new User
            {
                UserId    = DatabaseHelper.ParseInt(row["UserId"]),
                Username  = row["Username"]?.ToString()  ?? string.Empty,
                Password  = row["Password"]?.ToString()  ?? string.Empty,
                Role      = row["Role"]?.ToString()      ?? "Cashier",
                CreatedAt = row["CreatedAt"]?.ToString() ?? string.Empty
            };

            // Open the Dashboard. When Dashboard closes (logout), show Login again.
            var dash = new DashboardForm();
            dash.FormClosed += (s, ev) => Show();  // bring Login back on logout
            Hide();     // hide Login (don't close it — we need it back on logout)
            dash.Show();
        }

        // -----------------------------------------------------------------
        //  txtPassword_KeyDown — allows pressing Enter to submit the form
        // -----------------------------------------------------------------
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(sender, e);
        }
    }
}
