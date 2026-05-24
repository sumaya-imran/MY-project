// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/DashboardForm.cs
//  Purpose: Main hub screen shown after successful login.
//           Displays 4 live stat cards (revenue, orders, pending,
//           available items) and navigation cards to every module.
//           Admin users see extra management cards.
// =================================================================

using System;
using System.Windows.Forms;
using VelvetBeans.Database;
using VelvetBeans.Helpers;
using VelvetBeans.Models;

namespace VelvetBeans.Forms
{
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            // InitializeComponent() builds all controls (in Designer.cs)
            InitializeComponent();

            // Show the logged-in user's name and role in the header
            lblUserInfo.Text = "Logged in: " + Session.CurrentUser?.Username +
                               "   [" + Session.CurrentUser?.Role + "]";

            // Show today's date below the header
            lblDate.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");

            // Admin-only cards are hidden for Cashier role.
            // Session.IsAdmin is a shortcut for CurrentUser.Role == "Admin"
            cardMenuManager.Visible  = Session.IsAdmin;
            cardUserManager.Visible  = Session.IsAdmin;
            cardReports.Visible      = Session.IsAdmin;

            // Load the live statistics into the stat cards
            LoadStats();
        }

        // =================================================================
        //  LoadStats — queries the database for today's summary figures
        //  and updates the four coloured stat card labels.
        //  Called on load and again after each module closes (so numbers
        //  stay current without needing a manual refresh).
        // =================================================================
        private void LoadStats()
        {
            try
            {
                // Today's revenue — sum of completed orders only, today only
                var rev = DatabaseHelper.Scalar(
                    "SELECT IFNULL(SUM(TotalAmount - Discount), 0) " +
                    "FROM   Orders " +
                    "WHERE  Status = 'Completed' " +
                    "AND    date(CreatedAt) = date('now');");
                // ParseDecimal handles InvariantCulture so "3.50" works on all Windows locales
                lblSales.Text = string.Format("{0:F2}", DatabaseHelper.ParseDecimal(rev));

                // Total orders placed today (all statuses)
                var tod = DatabaseHelper.Scalar(
                    "SELECT COUNT(*) FROM Orders WHERE date(CreatedAt) = date('now');");
                lblOrders.Text = tod?.ToString() ?? "0";

                // Pending/Preparing orders — ones that still need action
                var pen = DatabaseHelper.Scalar(
                    "SELECT COUNT(*) FROM Orders WHERE Status IN ('Pending', 'Preparing');");
                lblPending.Text = pen?.ToString() ?? "0";

                // Available menu items — items the kitchen can currently serve
                var itm = DatabaseHelper.Scalar(
                    "SELECT COUNT(*) FROM MenuItems WHERE IsAvailable = 1;");
                lblItems.Text = itm?.ToString() ?? "0";
            }
            catch (Exception ex)
            {
                Msg.Error("Could not load dashboard stats:\n" + ex.Message);
            }
        }

        // =================================================================
        //  Navigation card click handlers — each opens the relevant form
        //  as a modal dialog (ShowDialog blocks until the form closes).
        //  LoadStats() is called afterward so numbers stay up to date.
        // =================================================================

        private void cardNewOrder_Click(object sender, EventArgs e)
        {
            // Open the POS screen. ShowDialog = waits until cashier closes it.
            new NewOrderForm().ShowDialog(this);
            LoadStats();  // Refresh stats — a new order may have been placed
        }

        private void cardOrderHistory_Click(object sender, EventArgs e)
        {
            new OrderHistoryForm().ShowDialog(this);
            LoadStats();  // Order statuses may have changed
        }

        private void cardMenuManager_Click(object sender, EventArgs e)
        {
            // Admin only — menu items may have been added/removed
            new MenuManagerForm().ShowDialog(this);
            LoadStats();  // Available item count may have changed
        }

        private void cardUserManager_Click(object sender, EventArgs e)
        {
            // Admin only — staff accounts management
            new UserManagerForm().ShowDialog(this);
        }

        private void cardReports_Click(object sender, EventArgs e)
        {
            // Admin only — sales reports and analytics
            new ReportsForm().ShowDialog(this);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (!Msg.Confirm("Are you sure you want to log out?")) return;

            // Clear the session so no user data lingers in memory
            Session.Clear();

            // Close the Dashboard — the Login form's FormClosed handler
            // (set in LoginForm.cs) will show Login again automatically
            Close();
        }
    }
}
