// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/OrderHistoryForm.Designer.cs
// =================================================================

using System.Drawing;
using System.Windows.Forms;
using VelvetBeans.Helpers;

namespace VelvetBeans.Forms
{
    partial class OrderHistoryForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private Panel          panelHeader  = null!;
        private Panel          panelFilter  = null!;
        private ComboBox       cmbStatus    = null!;
        private DateTimePicker dtpFrom      = null!;
        private DateTimePicker dtpTo        = null!;
        private Button         btnRefresh   = null!;
        private DataGridView   gridOrders   = null!;
        private Panel          panelDetail  = null!;
        private Label          lblDetail    = null!;
        private DataGridView   gridItems    = null!;
        private Button         btnComplete  = null!;
        private Button         btnCancel    = null!;
        private Button         btnClose     = null!;

        private void InitializeComponent()
        {
            this.Text          = "Order History";
            this.ClientSize    = new Size(1060, 660);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor     = Theme.Background;
            this.MinimizeBox   = false;
            this.Font          = Theme.FontBody;

            // Header
            panelHeader           = new Panel();
            panelHeader.Dock      = DockStyle.Top;
            panelHeader.Height    = 54;
            panelHeader.BackColor = Theme.Primary;
            panelHeader.Controls.Add(new Label
            {
                Text = "Order History", Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Theme.Accent, AutoSize = true, Location = new Point(16, 13)
            });

            // Filter bar
            panelFilter          = new Panel();
            panelFilter.Location = new Point(10, 62);
            panelFilter.Size     = new Size(1040, 40);
            panelFilter.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            panelFilter.Controls.Add(new Label
            {
                Text = "Status:", Font = Theme.FontSmall, ForeColor = Theme.Primary,
                AutoSize = true, Location = new Point(0, 12)
            });
            cmbStatus               = new ComboBox();
            cmbStatus.Bounds        = new Rectangle(52, 8, 120, 26);
            cmbStatus.Font          = Theme.FontBody;
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new object[] { "All","Pending","Preparing","Completed","Cancelled" });
            cmbStatus.SelectedIndex = 0;
            cmbStatus.SelectedIndexChanged += new System.EventHandler(this.cmbStatus_SelectedIndexChanged);
            panelFilter.Controls.Add(cmbStatus);

            panelFilter.Controls.Add(new Label
            {
                Text = "From:", Font = Theme.FontSmall, ForeColor = Theme.Primary,
                AutoSize = true, Location = new Point(188, 12)
            });
            dtpFrom          = new DateTimePicker();
            dtpFrom.Bounds   = new Rectangle(226, 8, 130, 26);
            dtpFrom.Font     = Theme.FontBody;
            dtpFrom.Format   = DateTimePickerFormat.Short;
            dtpFrom.Value    = System.DateTime.Today.AddDays(-30);
            dtpFrom.ValueChanged += new System.EventHandler(this.dtpFrom_ValueChanged);
            panelFilter.Controls.Add(dtpFrom);

            panelFilter.Controls.Add(new Label
            {
                Text = "To:", Font = Theme.FontSmall, ForeColor = Theme.Primary,
                AutoSize = true, Location = new Point(368, 12)
            });
            dtpTo          = new DateTimePicker();
            dtpTo.Bounds   = new Rectangle(392, 8, 130, 26);
            dtpTo.Font     = Theme.FontBody;
            dtpTo.Format   = DateTimePickerFormat.Short;
            dtpTo.Value    = System.DateTime.Today;
            dtpTo.ValueChanged += new System.EventHandler(this.dtpTo_ValueChanged);
            panelFilter.Controls.Add(dtpTo);

            btnRefresh                           = new Button();
            btnRefresh.Text                      = "Refresh";
            btnRefresh.Font                      = Theme.FontSmall;
            btnRefresh.BackColor                 = Theme.PrimaryLight;
            btnRefresh.ForeColor                 = System.Drawing.Color.White;
            btnRefresh.FlatStyle                 = FlatStyle.Flat;
            btnRefresh.Cursor                    = Cursors.Hand;
            btnRefresh.Bounds                    = new Rectangle(534, 8, 80, 26);
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            panelFilter.Controls.Add(btnRefresh);

            // Orders grid (left)
            gridOrders          = new DataGridView();
            gridOrders.Location = new Point(10, 110);
            gridOrders.Size     = new Size(610, 510);
            gridOrders.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            GridStyler.Apply(gridOrders);
            gridOrders.SelectionChanged += new System.EventHandler(this.gridOrders_SelectionChanged);

            // Detail panel (right)
            panelDetail           = new Panel();
            panelDetail.Location  = new Point(630, 110);
            panelDetail.Size      = new Size(420, 510);
            panelDetail.BackColor = System.Drawing.Color.White;
            panelDetail.Anchor    = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            lblDetail           = new Label();
            lblDetail.Text      = "Select an order to view its items.";
            lblDetail.Font      = Theme.FontBody;
            lblDetail.ForeColor = System.Drawing.Color.Gray;
            lblDetail.AutoSize  = false;
            lblDetail.Bounds    = new Rectangle(8, 8, 404, 54);
            lblDetail.TextAlign = ContentAlignment.TopLeft;

            gridItems          = new DataGridView();
            gridItems.Location = new Point(0, 68);
            gridItems.Size     = new Size(420, 280);
            GridStyler.Apply(gridItems);

            // Admin-only status buttons (Enabled=Session.IsAdmin set here)
            btnComplete                           = new Button();
            btnComplete.Text                      = "Mark Completed";
            btnComplete.Font                      = Theme.FontSmall;
            btnComplete.BackColor                 = Theme.Success;
            btnComplete.ForeColor                 = System.Drawing.Color.White;
            btnComplete.FlatStyle                 = FlatStyle.Flat;
            btnComplete.Cursor                    = Cursors.Hand;
            btnComplete.Bounds                    = new Rectangle(0, 358, 196, 30);
            btnComplete.FlatAppearance.BorderSize = 0;
            btnComplete.Enabled                   = VelvetBeans.Models.Session.IsAdmin;
            btnComplete.Click += new System.EventHandler(this.btnComplete_Click);

            btnCancel                           = new Button();
            btnCancel.Text                      = "Cancel Order";
            btnCancel.Font                      = Theme.FontSmall;
            btnCancel.BackColor                 = Theme.Danger;
            btnCancel.ForeColor                 = System.Drawing.Color.White;
            btnCancel.FlatStyle                 = FlatStyle.Flat;
            btnCancel.Cursor                    = Cursors.Hand;
            btnCancel.Bounds                    = new Rectangle(210, 358, 196, 30);
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Enabled                   = VelvetBeans.Models.Session.IsAdmin;
            btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            panelDetail.Controls.Add(lblDetail);
            panelDetail.Controls.Add(gridItems);
            panelDetail.Controls.Add(btnComplete);
            panelDetail.Controls.Add(btnCancel);

            // Close button
            btnClose                           = new Button();
            btnClose.Text                      = "Close";
            btnClose.Font                      = Theme.FontBody;
            btnClose.BackColor                 = Theme.Primary;
            btnClose.ForeColor                 = System.Drawing.Color.White;
            btnClose.FlatStyle                 = FlatStyle.Flat;
            btnClose.Cursor                    = Cursors.Hand;
            btnClose.Bounds                    = new Rectangle(940, 628, 110, 32);
            btnClose.Anchor                    = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += new System.EventHandler(this.btnClose_Click);

            this.Controls.Add(panelHeader);
            this.Controls.Add(panelFilter);
            this.Controls.Add(gridOrders);
            this.Controls.Add(panelDetail);
            this.Controls.Add(btnClose);
        }
    }
}
