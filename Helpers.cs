// =================================================================
//  VelvetBeans Cafe Management System
//  File: Helpers/Helpers.cs
//  Purpose: Shared utilities used by every form in the project.
//           Includes the colour/font theme, DataGridView styler,
//           message box shortcuts, and receipt text generator.
// =================================================================

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VelvetBeans.Models;

namespace VelvetBeans.Helpers
{
    // =================================================================
    //  Theme — Central colour palette and fonts.
    //  Every form uses these instead of hard-coded colours so you
    //  can change the look of the whole app by editing just this class.
    // =================================================================
    public static class Theme
    {
        // Main brown/espresso colour — used for headers, primary buttons
        public static readonly Color Background   = Color.FromArgb(250, 245, 240); // Warm off-white
        public static readonly Color Primary      = Color.FromArgb( 93,  64,  55); // Dark espresso brown
        public static readonly Color PrimaryLight = Color.FromArgb(141,  97,  85); // Medium brown
        public static readonly Color Accent       = Color.FromArgb(215, 165, 100); // Caramel gold
        public static readonly Color Success      = Color.FromArgb( 70, 148,  70); // Green (Place Order)
        public static readonly Color Danger       = Color.FromArgb(200,  55,  50); // Red (Delete/Cancel)
        public static readonly Color TextDark     = Color.FromArgb( 40,  30,  20); // Near black
        public static readonly Color TextLight    = Color.White;
        public static readonly Color GridAlt      = Color.FromArgb(250, 240, 230); // Alternating row tint

        // Fonts — Segoe UI is installed on all modern Windows PCs
        public static readonly Font FontTitle   = new Font("Segoe UI", 18f, FontStyle.Bold);
        public static readonly Font FontHeading = new Font("Segoe UI", 12f, FontStyle.Bold);
        public static readonly Font FontBody    = new Font("Segoe UI", 10f);
        public static readonly Font FontSmall   = new Font("Segoe UI",  9f);
    }

    // =================================================================
    //  Msg — Consistent dialog helpers.
    //  Instead of typing MessageBox.Show(...) with all its parameters
    //  everywhere, just call Msg.Info("Done!") or Msg.Confirm("Sure?")
    // =================================================================
    public static class Msg
    {
        // Shows an information dialog with an OK button
        public static void Info(string text, string title = "VelvetBeans")
            => MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Shows an error dialog with an OK button
        public static void Error(string text, string title = "Error")
            => MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

        // Shows a Yes/No confirm dialog and returns true if user clicked Yes
        public static bool Confirm(string text, string title = "Confirm")
            => MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
               == DialogResult.Yes;
    }

    // =================================================================
    //  GridStyler — Applies the cafe colour theme to a DataGridView.
    //  Call GridStyler.Apply(myGrid) after creating any DataGridView
    //  to instantly apply consistent styling across all grids.
    // =================================================================
    public static class GridStyler
    {
        public static void Apply(DataGridView g)
        {
            g.BackgroundColor                           = Theme.Background;
            g.BorderStyle                               = BorderStyle.None;
            g.CellBorderStyle                           = DataGridViewCellBorderStyle.SingleHorizontal;
            g.GridColor                                 = Color.FromArgb(220, 210, 200);
            g.RowHeadersVisible                         = false;   // Hide the left row-number column
            g.SelectionMode                             = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect                               = false;   // Only one row selected at a time
            g.AutoSizeColumnsMode                       = DataGridViewAutoSizeColumnsMode.Fill;
            g.AllowUserToAddRows                        = false;   // No blank row at the bottom
            g.AllowUserToDeleteRows                     = false;   // Delete only via our buttons
            g.ReadOnly                                  = true;    // Data shown but not editable in grid
            g.Font                                      = Theme.FontBody;
            g.DefaultCellStyle.BackColor                = Theme.Background;
            g.DefaultCellStyle.ForeColor                = Theme.TextDark;
            g.DefaultCellStyle.SelectionBackColor       = Theme.PrimaryLight;
            g.DefaultCellStyle.SelectionForeColor       = Theme.TextLight;
            g.AlternatingRowsDefaultCellStyle.BackColor = Theme.GridAlt; // Zebra striping
            g.EnableHeadersVisualStyles                 = false;   // Use our custom header colours
            g.ColumnHeadersDefaultCellStyle.BackColor   = Theme.Primary;
            g.ColumnHeadersDefaultCellStyle.ForeColor   = Theme.TextLight;
            g.ColumnHeadersDefaultCellStyle.Font        = Theme.FontHeading;
            g.ColumnHeadersHeight                       = 36;
            g.RowTemplate.Height                        = 30;
        }
    }

    // =================================================================
    //  ReceiptBuilder — Generates a plain-text receipt string.
    //  Takes a completed Order object and formats it like a
    //  thermal printer receipt (monospaced, fixed width).
    // =================================================================
    public static class ReceiptBuilder
    {
        // Total width of the receipt in characters
        private const int W = 44;

        public static string Build(Order order)
        {
            var sb = new StringBuilder();

            // --- Header ---
            sb.AppendLine(Centre("*** VELVET BEANS CAFE ***"));
            sb.AppendLine(Centre("Good coffee, great vibes."));
            sb.AppendLine(new string('-', W));
            sb.AppendLine("Order #  : " + order.OrderId);
            sb.AppendLine("Table    : " + (order.TableNumber == 0 ? "Takeaway" : order.TableNumber.ToString()));
            sb.AppendLine("Cashier  : " + order.CashierName);
            sb.AppendLine("Date     : " + order.CreatedAt);
            sb.AppendLine(new string('-', W));

            // --- Column headers ---
            sb.AppendLine(string.Format("{0,-22} {1,3} {2,8} {3,8}", "Item", "Qty", "Price", "Sub"));
            sb.AppendLine(new string('-', W));

            // --- Line items ---
            foreach (var it in order.Items)
            {
                // Trim item name if too long for the receipt width
                string n = it.ItemName.Length > 20
                    ? it.ItemName.Substring(0, 20)
                    : it.ItemName;
                sb.AppendLine(string.Format("{0,-22} {1,3} {2,8:F2} {3,8:F2}",
                    n, it.Quantity, it.UnitPrice, it.Subtotal));
            }

            // --- Totals ---
            sb.AppendLine(new string('-', W));
            sb.AppendLine(string.Format("{0,-34} {1,8:F2}", "Subtotal:", order.TotalAmount));
            if (order.Discount > 0)
                sb.AppendLine(string.Format("{0,-34} {1,8:F2}", "Discount:", -order.Discount));
            sb.AppendLine(new string('-', W));
            sb.AppendLine(string.Format("{0,-34} {1,8:F2}", "TOTAL:", order.GrandTotal));
            sb.AppendLine(new string('-', W));
            sb.AppendLine();
            sb.AppendLine(Centre("Thank you for visiting!"));
            sb.AppendLine(Centre("www.velvetbeans.com"));

            return sb.ToString();
        }

        // Helper: centres a string within the receipt width by padding spaces
        private static string Centre(string s)
        {
            if (s.Length >= W) return s;
            int pad = (W - s.Length) / 2;
            return s.PadLeft(s.Length + pad).PadRight(W);
        }
    }
}
