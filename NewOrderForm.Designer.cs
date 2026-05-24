// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/NewOrderForm.Designer.cs
//  Purpose: Designer layout for the POS / New Order screen.
// =================================================================

using System.Drawing;
using System.Windows.Forms;
using VelvetBeans.Helpers;

namespace VelvetBeans.Forms
{
    partial class NewOrderForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private Panel         panelHeader   = null!;
        private Panel         panelLeft     = null!;
        private Panel         panelRight    = null!;
        private ComboBox      cmbCategory   = null!;
        private TextBox       txtSearch     = null!;
        private DataGridView  gridMenu      = null!;
        private Button        btnAdd        = null!;
        private DataGridView  gridBasket    = null!;
        private Button        btnRemove     = null!;
        private NumericUpDown numTable      = null!;
        private NumericUpDown numDiscount   = null!;
        private Label         lblSubtotal   = null!;
        private Label         lblDiscount   = null!;
        private Panel         panelSep      = null!;
        private Label         lblTotal      = null!;
        private Button        btnPlaceOrder = null!;

        private void InitializeComponent()
        {
            this.Text          = "New Order";
            this.ClientSize    = new Size(1060, 650);
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
                Text      = "New Order — POS",
                Font      = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Theme.Accent,
                AutoSize  = true,
                Location  = new Point(16, 13)
            });

            // ---- Left panel: menu browser ---------------------------
            panelLeft        = new Panel();
            panelLeft.Location = new Point(10, 62);
            panelLeft.Size   = new Size(600, 568);
            panelLeft.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;

            panelLeft.Controls.Add(new Label
            {
                Text = "Category:", Font = Theme.FontSmall, ForeColor = Theme.Primary,
                AutoSize = true, Location = new Point(0, 6)
            });
            cmbCategory               = new ComboBox();
            cmbCategory.Location      = new Point(72, 2);
            cmbCategory.Size          = new Size(180, 26);
            cmbCategory.Font          = Theme.FontBody;
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.SelectedIndexChanged += new System.EventHandler(this.cmbCategory_SelectedIndexChanged);
            panelLeft.Controls.Add(cmbCategory);

            panelLeft.Controls.Add(new Label
            {
                Text = "Search:", Font = Theme.FontSmall, ForeColor = Theme.Primary,
                AutoSize = true, Location = new Point(264, 6)
            });
            txtSearch             = new TextBox();
            txtSearch.Location    = new Point(316, 2);
            txtSearch.Size        = new Size(190, 26);
            txtSearch.Font        = Theme.FontBody;
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.BackColor   = System.Drawing.Color.White;
            txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            panelLeft.Controls.Add(txtSearch);

            gridMenu          = new DataGridView();
            gridMenu.Location = new Point(0, 36);
            gridMenu.Size     = new Size(600, 494);
            gridMenu.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            GridStyler.Apply(gridMenu);
            gridMenu.DoubleClick += new System.EventHandler(this.gridMenu_DoubleClick);
            panelLeft.Controls.Add(gridMenu);

            btnAdd                           = new Button();
            btnAdd.Text                      = "+ Add to Order  (or double-click a row)";
            btnAdd.Font                      = Theme.FontSmall;
            btnAdd.BackColor                 = Theme.Accent;
            btnAdd.ForeColor                 = System.Drawing.Color.White;
            btnAdd.FlatStyle                 = FlatStyle.Flat;
            btnAdd.Cursor                    = Cursors.Hand;
            btnAdd.Bounds                    = new Rectangle(0, 536, 600, 32);
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            panelLeft.Controls.Add(btnAdd);

            // ---- Right panel: basket & order options ----------------
            panelRight        = new Panel();
            panelRight.Location = new Point(620, 62);
            panelRight.Size   = new Size(430, 568);
            panelRight.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            panelRight.Controls.Add(new Label
            {
                Text = "Current Order", Font = Theme.FontHeading,
                ForeColor = Theme.Primary, AutoSize = true, Location = new Point(0, 0)
            });

            gridBasket        = new DataGridView();
            gridBasket.Location = new Point(0, 28);
            gridBasket.Size   = new Size(430, 270);
            gridBasket.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GridStyler.Apply(gridBasket);
            panelRight.Controls.Add(gridBasket);

            btnRemove                           = new Button();
            btnRemove.Text                      = "Remove Selected Line";
            btnRemove.Font                      = Theme.FontSmall;
            btnRemove.BackColor                 = Theme.Danger;
            btnRemove.ForeColor                 = System.Drawing.Color.White;
            btnRemove.FlatStyle                 = FlatStyle.Flat;
            btnRemove.Cursor                    = Cursors.Hand;
            btnRemove.Bounds                    = new Rectangle(0, 304, 210, 28);
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            panelRight.Controls.Add(btnRemove);

            panelRight.Controls.Add(new Label
            {
                Text = "Table Number  (0 = Takeaway):", Font = Theme.FontSmall,
                ForeColor = Theme.Primary, AutoSize = true, Location = new Point(0, 346)
            });
            numTable        = new NumericUpDown();
            numTable.Bounds = new Rectangle(0, 364, 160, 28);
            numTable.Font   = Theme.FontBody;
            numTable.Minimum = 0; numTable.Maximum = 99;
            panelRight.Controls.Add(numTable);

            panelRight.Controls.Add(new Label
            {
                Text = "Discount (GBP):", Font = Theme.FontSmall,
                ForeColor = Theme.Primary, AutoSize = true, Location = new Point(230, 346)
            });
            numDiscount               = new NumericUpDown();
            numDiscount.Bounds        = new Rectangle(230, 364, 160, 28);
            numDiscount.Font          = Theme.FontBody;
            numDiscount.DecimalPlaces = 2;
            numDiscount.Minimum = 0m; numDiscount.Maximum = 9999m; numDiscount.Increment = 0.50m;
            numDiscount.ValueChanged += new System.EventHandler(this.numDiscount_ValueChanged);
            panelRight.Controls.Add(numDiscount);

            // Totals labels
            lblSubtotal           = new Label();
            lblSubtotal.Text      = string.Empty;
            lblSubtotal.Font      = Theme.FontBody;
            lblSubtotal.ForeColor = Theme.TextDark;
            lblSubtotal.AutoSize  = false;
            lblSubtotal.Bounds    = new Rectangle(0, 406, 430, 22);
            panelRight.Controls.Add(lblSubtotal);

            lblDiscount           = new Label();
            lblDiscount.Text      = string.Empty;
            lblDiscount.Font      = Theme.FontBody;
            lblDiscount.ForeColor = Theme.Danger;
            lblDiscount.AutoSize  = false;
            lblDiscount.Bounds    = new Rectangle(0, 430, 430, 22);
            panelRight.Controls.Add(lblDiscount);

            panelSep           = new Panel();
            panelSep.Bounds    = new Rectangle(0, 456, 430, 2);
            panelSep.BackColor = Theme.Primary;
            panelRight.Controls.Add(panelSep);

            lblTotal           = new Label();
            lblTotal.Text      = string.Empty;
            lblTotal.Font      = new Font("Segoe UI", 13f, FontStyle.Bold);
            lblTotal.ForeColor = Theme.Primary;
            lblTotal.AutoSize  = false;
            lblTotal.Bounds    = new Rectangle(0, 462, 430, 24);
            panelRight.Controls.Add(lblTotal);

            btnPlaceOrder                           = new Button();
            btnPlaceOrder.Text                      = "PLACE ORDER";
            btnPlaceOrder.Font                      = Theme.FontHeading;
            btnPlaceOrder.BackColor                 = Theme.Success;
            btnPlaceOrder.ForeColor                 = System.Drawing.Color.White;
            btnPlaceOrder.FlatStyle                 = FlatStyle.Flat;
            btnPlaceOrder.Cursor                    = Cursors.Hand;
            btnPlaceOrder.Bounds                    = new Rectangle(0, 496, 430, 46);
            btnPlaceOrder.FlatAppearance.BorderSize = 0;
            btnPlaceOrder.Click += new System.EventHandler(this.btnPlaceOrder_Click);
            panelRight.Controls.Add(btnPlaceOrder);

            this.Controls.Add(panelHeader);
            this.Controls.Add(panelLeft);
            this.Controls.Add(panelRight);
        }
    }
}
