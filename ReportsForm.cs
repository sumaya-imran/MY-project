// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/ReportsForm.cs
//  Purpose: Admin-only sales reports and analytics screen.
//           Three tabs:
//             1. Summary    — 6 stat cards (revenue, orders, avg, etc.)
//             2. Top Items  — best-selling menu items ranked by quantity
//             3. Daily      — day-by-day revenue breakdown
//           Date range picker + quick buttons (Today / Week / Month).
// =================================================================

using System;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using VelvetBeans.Database;
using VelvetBeans.Helpers;

namespace VelvetBeans.Forms
{
    public partial class ReportsForm : Form
    {
        public ReportsForm()
        {
            InitializeComponent();  // Builds all controls (in Designer.cs)
            Reload();               // Run all three report queries on open
        }

        // =================================================================
        //  Reload — runs all three reports for the currently selected
        //  date range. Called on open and whenever a date/filter changes.
        // =================================================================
        private void Reload()
        {
            string from = dtpFrom.Value.ToString("yyyy-MM-dd");
            string to   = dtpTo.Value.ToString("yyyy-MM-dd");

            LoadSummary(from, to);
            LoadTopItems(from, to);
            LoadDaily(from, to);
        }

        // =================================================================
        //  Tab 1 — Summary: populates the 6 stat card labels.
        // =================================================================
        private void LoadSummary(string from, string to)
        {
            // Reusable parameters for all summary queries
            var p = new[]
            {
                new SqliteParameter("@from", from),
                new SqliteParameter("@to",   to)
            };

            // -- Total revenue from completed orders --
            object? rev = DatabaseHelper.Scalar(
                "SELECT IFNULL(SUM(TotalAmount - Discount), 0) " +
                "FROM   Orders " +
                "WHERE  Status = 'Completed' " +
                "AND    date(CreatedAt) BETWEEN date(@from) AND date(@to);", p);
            decimal revenue = DatabaseHelper.ParseDecimal(rev);
            lblRevenue.Text = string.Format("{0:F2}", revenue);

            // -- Total number of orders in the date range (all statuses) --
            object? tot = DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM Orders " +
                "WHERE  date(CreatedAt) BETWEEN date(@from) AND date(@to);", p);
            int total = DatabaseHelper.ParseInt(tot);
            lblOrders.Text = total.ToString();

            // -- Average order value (revenue / total orders) --
            lblAvg.Text = total > 0
                ? string.Format("{0:F2}", revenue / total)
                : "0.00";

            // -- Count of completed orders --
            object? comp = DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM Orders WHERE Status = 'Completed' " +
                "AND date(CreatedAt) BETWEEN date(@from) AND date(@to);", p);
            lblCompleted.Text = comp?.ToString() ?? "0";

            // -- Count of cancelled orders --
            object? canc = DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM Orders WHERE Status = 'Cancelled' " +
                "AND date(CreatedAt) BETWEEN date(@from) AND date(@to);", p);
            lblCancelled.Text = canc?.ToString() ?? "0";

            // -- Total discount value given away --
            object? disc = DatabaseHelper.Scalar(
                "SELECT IFNULL(SUM(Discount), 0) FROM Orders " +
                "WHERE  date(CreatedAt) BETWEEN date(@from) AND date(@to);", p);
            lblDiscount.Text = string.Format("{0:F2}", DatabaseHelper.ParseDecimal(disc));
        }

        // =================================================================
        //  Tab 2 — Top Selling Items: ranked by total quantity sold.
        //  Only counts items from Completed orders.
        // =================================================================
        private void LoadTopItems(string from, string to)
        {
            var p = new[]
            {
                new SqliteParameter("@from", from),
                new SqliteParameter("@to",   to)
            };

            // Single-word aliases only — spaces in aliases break DataGridView cell lookup
            gridTop.DataSource = DatabaseHelper.Query(
                "SELECT m.Name                                 AS MenuItem,  " +
                "       c.Name                                 AS Category,  " +
                "       SUM(oi.Quantity)                       AS TotalSold, " +
                "       ROUND(SUM(oi.Quantity * oi.UnitPrice), 2) AS Revenue " +
                "FROM   OrderItems oi                                         " +
                "JOIN   MenuItems  m ON m.ItemId      = oi.ItemId            " +
                "JOIN   Categories c ON c.CategoryId  = m.CategoryId        " +
                "JOIN   Orders     o ON o.OrderId     = oi.OrderId           " +
                "WHERE  o.Status = 'Completed'                               " +
                "AND    date(o.CreatedAt) BETWEEN date(@from) AND date(@to)  " +
                "GROUP  BY m.ItemId                                          " +
                "ORDER  BY SUM(oi.Quantity) DESC                             " +
                "LIMIT  20;", p);
        }

        // =================================================================
        //  Tab 3 — Daily Revenue: one row per day showing order counts
        //  and revenue, ordered newest first.
        // =================================================================
        private void LoadDaily(string from, string to)
        {
            var p = new[]
            {
                new SqliteParameter("@from", from),
                new SqliteParameter("@to",   to)
            };

            gridDaily.DataSource = DatabaseHelper.Query(
                "SELECT date(CreatedAt)  AS OrderDate,    " +
                "       COUNT(*)         AS TotalOrders,  " +
                "       SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END) AS Completed, " +
                "       SUM(CASE WHEN Status = 'Cancelled' THEN 1 ELSE 0 END) AS Cancelled, " +
                "       ROUND(IFNULL(SUM(                                     " +
                "           CASE WHEN Status = 'Completed'                    " +
                "           THEN TotalAmount - Discount ELSE 0 END), 0), 2)   " +
                "                        AS Revenue       " +
                "FROM   Orders                            " +
                "WHERE  date(CreatedAt) BETWEEN date(@from) AND date(@to)     " +
                "GROUP  BY date(CreatedAt)                                    " +
                "ORDER  BY date(CreatedAt) DESC;", p);
        }

        // =================================================================
        //  Event handlers — wired up in Designer.cs
        // =================================================================

        // Generate button — reloads with current date range
        private void btnGenerate_Click(object sender, EventArgs e) => Reload();

        // Quick range buttons — set date pickers then reload
        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Today;
            dtpTo.Value   = DateTime.Today;
            Reload();
        }

        private void btnWeek_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Today.AddDays(-7);
            dtpTo.Value   = DateTime.Today;
            Reload();
        }

        private void btnMonth_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value   = DateTime.Today;
            Reload();
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
