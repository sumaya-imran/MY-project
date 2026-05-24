// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/ReportsForm.Designer.cs
//  Purpose: Designer layout for the Reports screen.
//           Header + date filter bar + 3-tab control.
// =================================================================

using System.Drawing;
using System.Windows.Forms;
using VelvetBeans.Helpers;

namespace VelvetBeans.Forms
{
    partial class ReportsForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        // Controls accessible from ReportsForm.cs
        private Panel          panelHeader  = null!;
        private Panel          panelFilter  = null!;
        private DateTimePicker dtpFrom      = null!;
        private DateTimePicker dtpTo        = null!;
        private Button         btnGenerate  = null!;
        private Button         btnToday     = null!;
        private Button         btnWeek      = null!;
        private Button         btnMonth     = null!;
        private TabControl     tabMain      = null!;
        private Button         btnClose     = null!;

        // Summary tab stat labels — updated by LoadSummary() in logic file
        private Label lblRevenue   = null!;
        private Label lblOrders    = null!;
        private Label lblAvg       = null!;
        private Label lblCompleted = null!;
        private Label lblCancelled = null!;
        private Label lblDiscount  = null!;

        // Grids for tabs 2 and 3
        private DataGridView gridTop   = null!;
        private DataGridView gridDaily = null!;

        private void InitializeComponent()
        {
            this.Text          = "Reports & Analytics";
            this.ClientSize    = new Size(1060, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor     = Theme.Background;
            this.MinimizeBox   = false;
            this.Font          = Theme.FontBody;

            // ---- Header bar -----------------------------------------
            panelHeader           = new Panel();
            panelHeader.Dock      = DockStyle.Top;
            panelHeader.Height    = 54;
            panelHeader.BackColor = Theme.Primary;
            panelHeader.Controls.Add(new Label
            {
                Text      = "Reports & Analytics",
                Font      = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Theme.Accent,
                AutoSize  = true,
                Location  = new Point(16, 13)
            });

            // ---- Date range filter bar ------------------------------
            panelFilter          = new Panel();
            panelFilter.Location = new Point(10, 62);
            panelFilter.Size     = new Size(1040, 42);
            panelFilter.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            panelFilter.Controls.Add(new Label
            {
                Text = "From:", Font = Theme.FontSmall, ForeColor = Theme.Primary,
                AutoSize = true, Location = new Point(0, 13)
            });

            dtpFrom          = new DateTimePicker();
            dtpFrom.Bounds   = new Rectangle(44, 8, 130, 28);
            dtpFrom.Font     = Theme.FontBody;
            dtpFrom.Format   = DateTimePickerFormat.Short;
            dtpFrom.Value    = System.DateTime.Today.AddDays(-30);
            panelFilter.Controls.Add(dtpFrom);

            panelFilter.Controls.Add(new Label
            {
                Text = "To:", Font = Theme.FontSmall, ForeColor = Theme.Primary,
                AutoSize = true, Location = new Point(186, 13)
            });

            dtpTo          = new DateTimePicker();
            dtpTo.Bounds   = new Rectangle(210, 8, 130, 28);
            dtpTo.Font     = Theme.FontBody;
            dtpTo.Format   = DateTimePickerFormat.Short;
            dtpTo.Value    = System.DateTime.Today;
            panelFilter.Controls.Add(dtpTo);

            // Generate button — applies the selected date range
            btnGenerate = MkBtn("Generate", new Rectangle(352, 8, 90, 28),
                Theme.Primary, this.btnGenerate_Click);
            panelFilter.Controls.Add(btnGenerate);

            // Quick range buttons — one click to set common date ranges
            btnToday = MkBtn("Today",     new Rectangle(454, 8, 70, 28), Theme.PrimaryLight, this.btnToday_Click);
            btnWeek  = MkBtn("This Week", new Rectangle(532, 8, 82, 28), Theme.PrimaryLight, this.btnWeek_Click);
            btnMonth = MkBtn("This Month",new Rectangle(622, 8, 90, 28), Theme.PrimaryLight, this.btnMonth_Click);
            panelFilter.Controls.Add(btnToday);
            panelFilter.Controls.Add(btnWeek);
            panelFilter.Controls.Add(btnMonth);

            // ---- Tab control with 3 tabs ----------------------------
            tabMain          = new TabControl();
            tabMain.Location = new Point(10, 112);
            tabMain.Size     = new Size(1040, 528);
            tabMain.Anchor   = AnchorStyles.Top | AnchorStyles.Left
                             | AnchorStyles.Right | AnchorStyles.Bottom;
            tabMain.Font     = Theme.FontBody;

            // Tab 1: Summary stat cards
            var tabSummary = BuildSummaryTab();
            tabMain.TabPages.Add(tabSummary);

            // Tab 2: Top selling items grid
            var tabTop           = new TabPage("Top Selling Items");
            tabTop.BackColor     = Theme.Background;
            gridTop              = new DataGridView();
            gridTop.Location     = new Point(8, 8);
            gridTop.Size         = new Size(1016, 468);
            gridTop.Anchor       = AnchorStyles.Top | AnchorStyles.Left
                                 | AnchorStyles.Right | AnchorStyles.Bottom;
            GridStyler.Apply(gridTop);
            tabTop.Controls.Add(gridTop);
            tabMain.TabPages.Add(tabTop);

            // Tab 3: Daily revenue breakdown grid
            var tabDaily         = new TabPage("Daily Revenue");
            tabDaily.BackColor   = Theme.Background;
            gridDaily            = new DataGridView();
            gridDaily.Location   = new Point(8, 8);
            gridDaily.Size       = new Size(1016, 468);
            gridDaily.Anchor     = AnchorStyles.Top | AnchorStyles.Left
                                 | AnchorStyles.Right | AnchorStyles.Bottom;
            GridStyler.Apply(gridDaily);
            tabDaily.Controls.Add(gridDaily);
            tabMain.TabPages.Add(tabDaily);

            // ---- Close button ---------------------------------------
            btnClose = MkBtn("Close", new Rectangle(940, 648, 110, 32),
                Theme.Primary, this.btnClose_Click);
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            this.Controls.Add(panelHeader);
            this.Controls.Add(panelFilter);
            this.Controls.Add(tabMain);
            this.Controls.Add(btnClose);
        }

        // =================================================================
        //  BuildSummaryTab — creates the first tab with 6 coloured stat cards.
        //  Each card has a title label and a value label.
        //  The value labels are stored as fields so ReportsForm.cs can
        //  update them with live data from the database.
        // =================================================================
        private TabPage BuildSummaryTab()
        {
            var tab           = new TabPage("Summary");
            tab.BackColor     = Theme.Background;

            // FlowLayoutPanel arranges the cards automatically left-to-right,
            // wrapping to the next row when there's no more horizontal space.
            var flow           = new FlowLayoutPanel();
            flow.Location      = new Point(10, 10);
            flow.Size          = new Size(1016, 480);
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.WrapContents  = true;

            // Create each stat card and store the value label for updates
            lblRevenue   = StatCard(flow, "Total Revenue (GBP)",   Theme.Primary);
            lblOrders    = StatCard(flow, "Total Orders",           Theme.PrimaryLight);
            lblAvg       = StatCard(flow, "Avg Order Value (GBP)",  Theme.Accent);
            lblCompleted = StatCard(flow, "Completed Orders",       Color.FromArgb(70, 148, 70));
            lblCancelled = StatCard(flow, "Cancelled Orders",       Theme.Danger);
            lblDiscount  = StatCard(flow, "Total Discounts (GBP)",  Color.FromArgb(100, 100, 165));

            tab.Controls.Add(flow);
            return tab;
        }

        // Helper: creates one coloured stat card in the flow panel
        private static Label StatCard(FlowLayoutPanel parent, string title, Color colour)
        {
            var card      = new Panel();
            card.Size     = new Size(316, 130);
            card.BackColor= colour;
            card.Margin   = new Padding(0, 0, 14, 14);

            var lTitle      = new Label();
            lTitle.Text     = title;
            lTitle.Font     = Theme.FontSmall;
            lTitle.ForeColor= Color.FromArgb(210, 200, 185);
            lTitle.AutoSize = false;
            lTitle.Bounds   = new Rectangle(14, 14, 288, 22);

            var lValue      = new Label();
            lValue.Text     = "...";   // Placeholder — replaced by Reload()
            lValue.Font     = new Font("Segoe UI", 24f, FontStyle.Bold);
            lValue.ForeColor= Color.White;
            lValue.AutoSize = false;
            lValue.Bounds   = new Rectangle(14, 44, 288, 60);
            lValue.TextAlign= ContentAlignment.MiddleLeft;

            card.Controls.Add(lTitle);
            card.Controls.Add(lValue);
            parent.Controls.Add(card);

            // Return the value label so the logic file can update its text
            return lValue;
        }

        // Helper: creates a styled flat button quickly
        private static Button MkBtn(string text, Rectangle bounds,
            Color backColor, System.EventHandler handler)
        {
            var btn = new Button();
            btn.Text                      = text;
            btn.Bounds                    = bounds;
            btn.Font                      = Theme.FontSmall;
            btn.BackColor                 = backColor;
            btn.ForeColor                 = Color.White;
            btn.FlatStyle                 = FlatStyle.Flat;
            btn.Cursor                    = Cursors.Hand;
            btn.FlatAppearance.BorderSize = 0;
            btn.Click                    += handler;
            return btn;
        }
    }
}
