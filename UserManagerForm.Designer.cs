// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/UserManagerForm.Designer.cs
//  Purpose: Designer layout for UserManagerForm.
//           Left: user list grid. Right: add/edit panel.
// =================================================================

using System.Drawing;
using System.Windows.Forms;
using VelvetBeans.Helpers;

namespace VelvetBeans.Forms
{
    partial class UserManagerForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        // Controls accessible from UserManagerForm.cs
        private Panel        panelHeader  = null!;
        private DataGridView gridUsers    = null!;
        private Panel        panelEdit    = null!;
        private Label        lblEditTitle = null!;
        private Label        lblUsername  = null!;
        private TextBox      txtUsername  = null!;
        private Label        lblPassword  = null!;
        private TextBox      txtPassword  = null!;
        private Label        lblRole      = null!;
        private ComboBox     cmbRole      = null!;
        private Button       btnSave      = null!;
        private Button       btnClear     = null!;
        private Button       btnDelete    = null!;
        private Label        lblNote      = null!;
        private Button       btnClose     = null!;

        private void InitializeComponent()
        {
            this.Text          = "User Manager";
            this.ClientSize    = new Size(900, 580);
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
                Text      = "User Manager",
                Font      = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Theme.Accent,
                AutoSize  = true,
                Location  = new Point(16, 13)
            });

            // ---- Users grid (left) ----------------------------------
            gridUsers          = new DataGridView();
            gridUsers.Location = new Point(10, 64);
            gridUsers.Size     = new Size(550, 464);
            gridUsers.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            GridStyler.Apply(gridUsers);
            // Wire up row selection (defined in UserManagerForm.cs)
            gridUsers.SelectionChanged += new System.EventHandler(this.gridUsers_SelectionChanged);

            // ---- Edit panel (right) ---------------------------------
            panelEdit           = new Panel();
            panelEdit.Location  = new Point(572, 64);
            panelEdit.Size      = new Size(318, 464);
            panelEdit.BackColor = Color.White;
            panelEdit.Anchor    = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            lblEditTitle           = new Label();
            lblEditTitle.Text      = "Add / Edit User";
            lblEditTitle.Font      = Theme.FontHeading;
            lblEditTitle.ForeColor = Theme.Primary;
            lblEditTitle.AutoSize  = true;
            lblEditTitle.Location  = new Point(12, 14);

            // -- Username --
            lblUsername           = new Label();
            lblUsername.Text      = "Username:";
            lblUsername.Font      = Theme.FontSmall;
            lblUsername.ForeColor = Theme.Primary;
            lblUsername.AutoSize  = true;
            lblUsername.Location  = new Point(12, 54);

            txtUsername             = new TextBox();
            txtUsername.Bounds      = new Rectangle(12, 72, 294, 28);
            txtUsername.Font        = Theme.FontBody;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.BackColor   = Color.White;

            // -- Password --
            lblPassword           = new Label();
            lblPassword.Text      = "Password:";
            lblPassword.Font      = Theme.FontSmall;
            lblPassword.ForeColor = Theme.Primary;
            lblPassword.AutoSize  = true;
            lblPassword.Location  = new Point(12, 112);

            txtPassword             = new TextBox();
            txtPassword.Bounds      = new Rectangle(12, 130, 294, 28);
            txtPassword.Font        = Theme.FontBody;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.BackColor   = Color.White;
            // Do NOT use UseSystemPasswordChar here so admin can see what they type
            // when setting someone else's password

            // -- Role --
            lblRole           = new Label();
            lblRole.Text      = "Role:";
            lblRole.Font      = Theme.FontSmall;
            lblRole.ForeColor = Theme.Primary;
            lblRole.AutoSize  = true;
            lblRole.Location  = new Point(12, 170);

            cmbRole               = new ComboBox();
            cmbRole.Bounds        = new Rectangle(12, 188, 294, 28);
            cmbRole.Font          = Theme.FontBody;
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.Items.AddRange(new object[] { "Admin", "Cashier" });
            cmbRole.SelectedIndex = 1;  // Default: Cashier

            // -- Action buttons --
            btnSave                           = new Button();
            btnSave.Text                      = "Save User";
            btnSave.Font                      = Theme.FontBody;
            btnSave.BackColor                 = Theme.Primary;
            btnSave.ForeColor                 = Color.White;
            btnSave.FlatStyle                 = FlatStyle.Flat;
            btnSave.Cursor                    = Cursors.Hand;
            btnSave.Bounds                    = new Rectangle(12, 234, 140, 34);
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += new System.EventHandler(this.btnSave_Click);

            btnClear                           = new Button();
            btnClear.Text                      = "Clear Form";
            btnClear.Font                      = Theme.FontBody;
            btnClear.BackColor                 = Color.LightGray;
            btnClear.ForeColor                 = Theme.TextDark;
            btnClear.FlatStyle                 = FlatStyle.Flat;
            btnClear.Cursor                    = Cursors.Hand;
            btnClear.Bounds                    = new Rectangle(162, 234, 110, 34);
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += new System.EventHandler(this.btnClear_Click);

            btnDelete                           = new Button();
            btnDelete.Text                      = "Delete Selected User";
            btnDelete.Font                      = Theme.FontSmall;
            btnDelete.BackColor                 = Theme.Danger;
            btnDelete.ForeColor                 = Color.White;
            btnDelete.FlatStyle                 = FlatStyle.Flat;
            btnDelete.Cursor                    = Cursors.Hand;
            btnDelete.Bounds                    = new Rectangle(12, 284, 190, 30);
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            // Informational note about the password field behaviour
            lblNote           = new Label();
            lblNote.Text      = "Tip: Leave Password blank when editing\n" +
                                "to keep the user's existing password.";
            lblNote.Font      = Theme.FontSmall;
            lblNote.ForeColor = Color.Gray;
            lblNote.AutoSize  = false;
            lblNote.Bounds    = new Rectangle(12, 328, 294, 40);

            panelEdit.Controls.Add(lblEditTitle);
            panelEdit.Controls.Add(lblUsername);
            panelEdit.Controls.Add(txtUsername);
            panelEdit.Controls.Add(lblPassword);
            panelEdit.Controls.Add(txtPassword);
            panelEdit.Controls.Add(lblRole);
            panelEdit.Controls.Add(cmbRole);
            panelEdit.Controls.Add(btnSave);
            panelEdit.Controls.Add(btnClear);
            panelEdit.Controls.Add(btnDelete);
            panelEdit.Controls.Add(lblNote);

            // ---- Close button ---------------------------------------
            btnClose                           = new Button();
            btnClose.Text                      = "Close";
            btnClose.Font                      = Theme.FontBody;
            btnClose.BackColor                 = Theme.Primary;
            btnClose.ForeColor                 = Color.White;
            btnClose.FlatStyle                 = FlatStyle.Flat;
            btnClose.Cursor                    = Cursors.Hand;
            btnClose.Bounds                    = new Rectangle(780, 548, 110, 32);
            btnClose.Anchor                    = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += new System.EventHandler(this.btnClose_Click);

            this.Controls.Add(panelHeader);
            this.Controls.Add(gridUsers);
            this.Controls.Add(panelEdit);
            this.Controls.Add(btnClose);
        }
    }
}
