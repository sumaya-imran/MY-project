// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/MenuManagerForm.Designer.cs
//  Purpose: Designer layout for MenuManagerForm.
//           Items-only CRUD — no categories tab.
//           Layout: grid left, edit panel right, action buttons below.
// =================================================================

using System.Drawing;
using System.Windows.Forms;
using VelvetBeans.Helpers;

namespace VelvetBeans.Forms
{
    partial class MenuManagerForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        // -----------------------------------------------------------------
        //  Control declarations — accessible from MenuManagerForm.cs
        // -----------------------------------------------------------------
        private Panel         panelHeader  = null!;  // Dark top bar
        private Panel         panelLeft    = null!;  // Grid + filter
        private Panel         panelRight   = null!;  // Edit fields
        private Label         lblPageTitle = null!;  // "Menu Manager"
        private Label         lblFilter    = null!;  // "Filter Category:" label
        private ComboBox      cmbFilter    = null!;  // Category filter dropdown
        private DataGridView  gridItems    = null!;  // Items list
        private Label         lblEditTitle = null!;  // "Add / Edit Item"
        private Label         lblName      = null!;
        private TextBox       txtName      = null!;  // Item name field
        private Label         lblDesc      = null!;
        private TextBox       txtDesc      = null!;  // Description multiline field
        private Label         lblPrice     = null!;
        private NumericUpDown numPrice     = null!;  // Price spinner
        private Label         lblCategory  = null!;
        private ComboBox      cmbCategory  = null!;  // Category picker for item
        private CheckBox      chkAvailable = null!;  // Available toggle
        private Button        btnSave      = null!;  // Add/Update button
        private Button        btnClear     = null!;  // Reset form button
        private Button        btnDelete    = null!;  // Delete selected item
        private Button        btnClose     = null!;  // Close form

        private void InitializeComponent()
        {
            // ---- Form -----------------------------------------------
            this.Text          = "Menu Manager — Items";
            this.ClientSize    = new Size(1060, 640);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor     = Theme.Background;
            this.MinimizeBox   = false;
            this.Font          = Theme.FontBody;

            // ---- Header bar -----------------------------------------
            panelHeader           = new Panel();
            panelHeader.Dock      = DockStyle.Top;
            panelHeader.Height    = 54;
            panelHeader.BackColor = Theme.Primary;

            lblPageTitle           = new Label();
            lblPageTitle.Text      = "Menu Manager";
            lblPageTitle.Font      = new Font("Segoe UI", 16f, FontStyle.Bold);
            lblPageTitle.ForeColor = Theme.Accent;
            lblPageTitle.AutoSize  = true;
            lblPageTitle.Location  = new Point(16, 13);
            panelHeader.Controls.Add(lblPageTitle);

            // ---- Left panel: grid with filter -----------------------
            panelLeft        = new Panel();
            panelLeft.Location = new Point(10, 62);
            panelLeft.Size   = new Size(630, 556);
            panelLeft.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;

            // Category filter label and dropdown
            lblFilter           = new Label();
            lblFilter.Text      = "Filter by Category:";
            lblFilter.Font      = Theme.FontSmall;
            lblFilter.ForeColor = Theme.Primary;
            lblFilter.AutoSize  = true;
            lblFilter.Location  = new Point(0, 7);

            cmbFilter               = new ComboBox();
            cmbFilter.Location      = new Point(130, 3);
            cmbFilter.Size          = new Size(200, 26);
            cmbFilter.Font          = Theme.FontBody;
            cmbFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            // Wire up event (defined in MenuManagerForm.cs)
            cmbFilter.SelectedIndexChanged += new System.EventHandler(this.cmbFilter_SelectedIndexChanged);

            // Items DataGridView
            gridItems          = new DataGridView();
            gridItems.Location = new Point(0, 36);
            gridItems.Size     = new Size(630, 520);
            gridItems.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            GridStyler.Apply(gridItems);
            // Wire up row selection event (defined in MenuManagerForm.cs)
            gridItems.SelectionChanged += new System.EventHandler(this.gridItems_SelectionChanged);

            panelLeft.Controls.Add(lblFilter);
            panelLeft.Controls.Add(cmbFilter);
            panelLeft.Controls.Add(gridItems);

            // ---- Right panel: edit fields ---------------------------
            panelRight           = new Panel();
            panelRight.Location  = new Point(652, 62);
            panelRight.Size      = new Size(398, 556);
            panelRight.BackColor = Color.White;
            panelRight.Anchor    = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            lblEditTitle           = new Label();
            lblEditTitle.Text      = "Add / Edit Menu Item";
            lblEditTitle.Font      = Theme.FontHeading;
            lblEditTitle.ForeColor = Theme.Primary;
            lblEditTitle.AutoSize  = true;
            lblEditTitle.Location  = new Point(12, 14);

            // -- Name field --
            lblName           = new Label();
            lblName.Text      = "Item Name:";
            lblName.Font      = Theme.FontSmall;
            lblName.ForeColor = Theme.Primary;
            lblName.AutoSize  = true;
            lblName.Location  = new Point(12, 52);

            txtName             = new TextBox();
            txtName.Bounds      = new Rectangle(12, 70, 374, 28);
            txtName.Font        = Theme.FontBody;
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.BackColor   = Color.White;

            // -- Description field (multi-line) --
            lblDesc           = new Label();
            lblDesc.Text      = "Description:";
            lblDesc.Font      = Theme.FontSmall;
            lblDesc.ForeColor = Theme.Primary;
            lblDesc.AutoSize  = true;
            lblDesc.Location  = new Point(12, 108);

            txtDesc             = new TextBox();
            txtDesc.Bounds      = new Rectangle(12, 126, 374, 60);
            txtDesc.Font        = Theme.FontBody;
            txtDesc.Multiline   = true;  // Allows multiple lines of text
            txtDesc.BorderStyle = BorderStyle.FixedSingle;
            txtDesc.BackColor   = Color.White;

            // -- Price field (NumericUpDown spinner) --
            lblPrice           = new Label();
            lblPrice.Text      = "Price (GBP):";
            lblPrice.Font      = Theme.FontSmall;
            lblPrice.ForeColor = Theme.Primary;
            lblPrice.AutoSize  = true;
            lblPrice.Location  = new Point(12, 196);

            numPrice               = new NumericUpDown();
            numPrice.Bounds        = new Rectangle(12, 214, 160, 28);
            numPrice.Font          = Theme.FontBody;
            numPrice.DecimalPlaces = 2;       // Show pence (e.g. 3.50)
            numPrice.Minimum       = 0m;
            numPrice.Maximum       = 9999m;
            numPrice.Increment     = 0.25m;   // Up/down arrows move by 25p

            // -- Category dropdown --
            lblCategory           = new Label();
            lblCategory.Text      = "Category:";
            lblCategory.Font      = Theme.FontSmall;
            lblCategory.ForeColor = Theme.Primary;
            lblCategory.AutoSize  = true;
            lblCategory.Location  = new Point(12, 252);

            cmbCategory               = new ComboBox();
            cmbCategory.Bounds        = new Rectangle(12, 270, 374, 28);
            cmbCategory.Font          = Theme.FontBody;
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            // -- Available checkbox --
            chkAvailable           = new CheckBox();
            chkAvailable.Text      = "Available for ordering  (untick to hide from POS)";
            chkAvailable.Font      = Theme.FontBody;
            chkAvailable.ForeColor = Theme.TextDark;
            chkAvailable.Bounds    = new Rectangle(12, 312, 374, 26);
            chkAvailable.Checked   = true;  // Default: new items are available

            // -- Action buttons --
            btnSave                           = new Button();
            btnSave.Text                      = "Save Item";
            btnSave.Font                      = Theme.FontBody;
            btnSave.BackColor                 = Theme.Primary;
            btnSave.ForeColor                 = Color.White;
            btnSave.FlatStyle                 = FlatStyle.Flat;
            btnSave.Cursor                    = Cursors.Hand;
            btnSave.Bounds                    = new Rectangle(12, 354, 160, 36);
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += new System.EventHandler(this.btnSave_Click);

            btnClear                           = new Button();
            btnClear.Text                      = "Clear Form";
            btnClear.Font                      = Theme.FontBody;
            btnClear.BackColor                 = Color.LightGray;
            btnClear.ForeColor                 = Theme.TextDark;
            btnClear.FlatStyle                 = FlatStyle.Flat;
            btnClear.Cursor                    = Cursors.Hand;
            btnClear.Bounds                    = new Rectangle(182, 354, 120, 36);
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += new System.EventHandler(this.btnClear_Click);

            btnDelete                           = new Button();
            btnDelete.Text                      = "Delete Selected Item";
            btnDelete.Font                      = Theme.FontSmall;
            btnDelete.BackColor                 = Theme.Danger;
            btnDelete.ForeColor                 = Color.White;
            btnDelete.FlatStyle                 = FlatStyle.Flat;
            btnDelete.Cursor                    = Cursors.Hand;
            btnDelete.Bounds                    = new Rectangle(12, 404, 200, 32);
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            // Close button
            btnClose                           = new Button();
            btnClose.Text                      = "Close";
            btnClose.Font                      = Theme.FontBody;
            btnClose.BackColor                 = Theme.PrimaryLight;
            btnClose.ForeColor                 = Color.White;
            btnClose.FlatStyle                 = FlatStyle.Flat;
            btnClose.Cursor                    = Cursors.Hand;
            btnClose.Bounds                    = new Rectangle(12, 506, 120, 32);
            btnClose.Anchor                    = AnchorStyles.Bottom | AnchorStyles.Left;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // Add all right panel controls
            panelRight.Controls.Add(lblEditTitle);
            panelRight.Controls.Add(lblName);
            panelRight.Controls.Add(txtName);
            panelRight.Controls.Add(lblDesc);
            panelRight.Controls.Add(txtDesc);
            panelRight.Controls.Add(lblPrice);
            panelRight.Controls.Add(numPrice);
            panelRight.Controls.Add(lblCategory);
            panelRight.Controls.Add(cmbCategory);
            panelRight.Controls.Add(chkAvailable);
            panelRight.Controls.Add(btnSave);
            panelRight.Controls.Add(btnClear);
            panelRight.Controls.Add(btnDelete);
            panelRight.Controls.Add(btnClose);

            // Add panels to form
            this.Controls.Add(panelHeader);
            this.Controls.Add(panelLeft);
            this.Controls.Add(panelRight);
        }
    }
}
