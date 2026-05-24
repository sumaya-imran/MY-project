// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/DashboardForm.Designer.cs
//  Purpose: Designer layout for DashboardForm.
//           Header bar, 4 stat cards, 5 navigation cards.
// =================================================================

using System.Drawing;
using System.Windows.Forms;
using VelvetBeans.Helpers;

namespace VelvetBeans.Forms
{
    partial class DashboardForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        // Controls accessible from DashboardForm.cs
        private Panel  panelHeader      = null!;
        private Label  lblAppTitle      = null!;
        private Label  lblUserInfo      = null!;
        private Button btnLogout        = null!;
        private Label  lblDate          = null!;

        // Stat card value labels (text updated by LoadStats() in logic file)
        private Label  lblSales         = null!;
        private Label  lblOrders        = null!;
        private Label  lblPending       = null!;
        private Label  lblItems         = null!;

        // Navigation cards (Panel + child labels, all clicks forwarded)
        private Panel  cardNewOrder     = null!;
        private Panel  cardOrderHistory = null!;
        private Panel  cardMenuManager  = null!;
        private Panel  cardUserManager  = null!;
        private Panel  cardReports      = null!;

        private void InitializeComponent()
        {
            this.Text          = "VelvetBeans — Dashboard";
            this.ClientSize    = new Size(1060, 660);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor     = Theme.Background;
            this.MinimumSize   = new Size(900, 600);
            this.Font          = Theme.FontBody;

            // ---- Header bar -----------------------------------------
            panelHeader           = new Panel();
            panelHeader.Dock      = DockStyle.Top;
            panelHeader.Height    = 64;
            panelHeader.BackColor = Theme.Primary;

            lblAppTitle           = new Label();
            lblAppTitle.Text      = "VELVET BEANS CAFE";
            lblAppTitle.Font      = new Font("Segoe UI", 17f, FontStyle.Bold);
            lblAppTitle.ForeColor = Theme.Accent;
            lblAppTitle.AutoSize  = true;
            lblAppTitle.Location  = new Point(18, 17);

            // Shows "Logged in: admin [Admin]" — filled in DashboardForm.cs
            lblUserInfo           = new Label();
            lblUserInfo.Text      = string.Empty;
            lblUserInfo.Font      = Theme.FontBody;
            lblUserInfo.ForeColor = Color.FromArgb(210, 195, 180);
            lblUserInfo.AutoSize  = true;
            lblUserInfo.Location  = new Point(500, 22);

            btnLogout                           = new Button();
            btnLogout.Text                      = "Logout";
            btnLogout.Font                      = Theme.FontSmall;
            btnLogout.BackColor                 = Theme.Danger;
            btnLogout.ForeColor                 = Color.White;
            btnLogout.FlatStyle                 = FlatStyle.Flat;
            btnLogout.Cursor                    = Cursors.Hand;
            btnLogout.Size                      = new Size(82, 32);
            btnLogout.Location                  = new Point(958, 16);
            btnLogout.Anchor                    = AnchorStyles.Top | AnchorStyles.Right;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            panelHeader.Controls.Add(lblAppTitle);
            panelHeader.Controls.Add(lblUserInfo);
            panelHeader.Controls.Add(btnLogout);

            // ---- Date label -----------------------------------------
            lblDate           = new Label();
            lblDate.Text      = string.Empty;  // Set in DashboardForm.cs constructor
            lblDate.Font      = Theme.FontBody;
            lblDate.ForeColor = Color.Gray;
            lblDate.AutoSize  = true;
            lblDate.Location  = new Point(20, 74);

            // ---- Stat cards (4 coloured cards with live numbers) ----
            // BuildStatCard creates the panel, wires up the value label,
            // and returns the label so the logic file can update its text
            Panel s1, s2, s3, s4;
            lblSales   = BuildStatCard(out s1, "Today's Revenue (GBP)", Theme.Primary,      new Point(16,  100));
            lblOrders  = BuildStatCard(out s2, "Orders Today",           Theme.PrimaryLight, new Point(278, 100));
            lblPending = BuildStatCard(out s3, "Pending Orders",          Theme.Accent,       new Point(540, 100));
            lblItems   = BuildStatCard(out s4, "Available Menu Items",    Color.FromArgb(70, 120, 90), new Point(802, 100));

            // ---- Navigation section header --------------------------
            var lblNavTitle = new Label();
            lblNavTitle.Text      = "Quick Navigation";
            lblNavTitle.Font      = Theme.FontHeading;
            lblNavTitle.ForeColor = Theme.Primary;
            lblNavTitle.AutoSize  = true;
            lblNavTitle.Location  = new Point(20, 228);

            // ---- Navigation cards -----------------------------------
            // Each card is a clickable panel. Visible for all users
            // unless restricted to Admin (set in DashboardForm.cs).
            cardNewOrder     = BuildNavCard("New Order",     "Take a customer order",       Theme.Primary,                new Point(16,  256), this.cardNewOrder_Click);
            cardOrderHistory = BuildNavCard("Order History", "View and manage all orders",  Theme.PrimaryLight,           new Point(222, 256), this.cardOrderHistory_Click);
            cardMenuManager  = BuildNavCard("Menu Manager",  "Add and edit menu items",     Theme.Accent,                 new Point(428, 256), this.cardMenuManager_Click);
            cardUserManager  = BuildNavCard("User Manager",  "Manage staff accounts",       Color.FromArgb(70, 120, 90),  new Point(634, 256), this.cardUserManager_Click);
            cardReports      = BuildNavCard("Reports",       "Sales reports & analytics",   Color.FromArgb(100, 100, 165),new Point(840, 256), this.cardReports_Click);

            // ---- Assemble form --------------------------------------
            this.Controls.Add(panelHeader);
            this.Controls.Add(lblDate);
            this.Controls.Add(s1);
            this.Controls.Add(s2);
            this.Controls.Add(s3);
            this.Controls.Add(s4);
            this.Controls.Add(lblNavTitle);
            this.Controls.Add(cardNewOrder);
            this.Controls.Add(cardOrderHistory);
            this.Controls.Add(cardMenuManager);
            this.Controls.Add(cardUserManager);
            this.Controls.Add(cardReports);
        }

        // Helper: Creates a coloured stat card and returns its value label
        private static Label BuildStatCard(out Panel card, string title, Color colour, Point loc)
        {
            card           = new Panel();
            card.Location  = loc;
            card.Size      = new Size(246, 100);
            card.BackColor = colour;

            var t = new Label();
            t.Text = title; t.Font = Theme.FontSmall;
            t.ForeColor = Color.FromArgb(210, 200, 185);
            t.AutoSize = false; t.Bounds = new Rectangle(14, 12, 218, 20);

            var v = new Label();
            v.Text = "..."; v.Font = new Font("Segoe UI", 22f, FontStyle.Bold);
            v.ForeColor = Color.White; v.AutoSize = false;
            v.Bounds = new Rectangle(14, 38, 218, 48);
            v.TextAlign = ContentAlignment.MiddleLeft;

            card.Controls.Add(t);
            card.Controls.Add(v);
            return v;  // Caller stores this label to update the displayed number
        }

        // Helper: Creates a clickable white navigation card
        private static Panel BuildNavCard(string title, string subtitle,
            Color accentColour, Point loc, System.EventHandler clickHandler)
        {
            var card = new Panel();
            card.Location  = loc;
            card.Size      = new Size(190, 120);
            card.BackColor = Color.White;
            card.Cursor    = Cursors.Hand;

            // Thin coloured strip at the top — brand identity
            var strip = new Panel();
            strip.Dock      = DockStyle.Top;
            strip.Height    = 5;
            strip.BackColor = accentColour;

            var lt = new Label();
            lt.Text      = title;
            lt.Font      = Theme.FontHeading;
            lt.ForeColor = Theme.Primary;
            lt.AutoSize  = false;
            lt.Bounds    = new Rectangle(0, 20, 190, 30);
            lt.TextAlign = ContentAlignment.MiddleCenter;

            var ls = new Label();
            ls.Text      = subtitle;
            ls.Font      = Theme.FontSmall;
            ls.ForeColor = Color.Gray;
            ls.AutoSize  = false;
            ls.Bounds    = new Rectangle(8, 54, 174, 44);
            ls.TextAlign = ContentAlignment.TopCenter;

            card.Controls.Add(strip);
            card.Controls.Add(lt);
            card.Controls.Add(ls);

            // Forward clicks from all child controls to the same handler
            // so clicking the text OR the panel background both work
            card.Click   += clickHandler;
            strip.Click  += clickHandler;
            lt.Click     += clickHandler;
            ls.Click     += clickHandler;

            return card;
        }
    }
}
