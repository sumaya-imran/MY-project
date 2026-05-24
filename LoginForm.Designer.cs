// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/LoginForm.Designer.cs
//  Purpose: Auto-generated layout code for LoginForm.
//           Defines all controls (buttons, textboxes, labels) and
//           their positions. This file is read by the VS Designer.
//           DO NOT put business logic here — use LoginForm.cs.
// =================================================================

using System.Drawing;
using System.Windows.Forms;
using VelvetBeans.Helpers;

namespace VelvetBeans.Forms
{
    partial class LoginForm
    {
        // Required by Windows Forms Designer
        private System.ComponentModel.IContainer components = null;

        // Clean up resources when the form is disposed
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        // -----------------------------------------------------------------
        //  Control declarations — each control is a field so that
        //  LoginForm.cs (the logic file) can access them by name.
        // -----------------------------------------------------------------
        private Panel   panelLeft   = null!;  // Left branding panel (brown)
        private Panel   panelRight  = null!;  // Right login panel (white)
        private Label   lblAppName  = null!;  // "VELVET BEANS" heading
        private Label   lblTagline  = null!;  // "Cafe Management System"
        private Label   lblSlogan   = null!;  // Italic slogan
        private Label   lblWelcome  = null!;  // "Welcome Back" heading
        private Label   lblSubHead  = null!;  // "Sign in to continue"
        private Label   lblUser     = null!;  // "Username" field label
        private Label   lblPass     = null!;  // "Password" field label
        private TextBox txtUsername = null!;  // Username input field
        private TextBox txtPassword = null!;  // Password input field (masked)
        private Label   lblError    = null!;  // Shows login error messages
        private Button  btnLogin    = null!;  // Sign In button

        // -----------------------------------------------------------------
        //  InitializeComponent — called by the constructor in LoginForm.cs.
        //  Creates every control, sets its properties, and adds it to the form.
        // -----------------------------------------------------------------
        private void InitializeComponent()
        {
            // ---- Form properties ----------------------------------------
            this.Text            = "VelvetBeans — Login";
            this.ClientSize      = new Size(780, 500);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;  // Fixed size — no resize
            this.MaximizeBox     = false;
            this.BackColor       = Theme.Background;
            this.Font            = Theme.FontBody;

            // ---- Left branding panel (espresso brown background) ---------
            panelLeft           = new Panel();
            panelLeft.Location  = new Point(0, 0);
            panelLeft.Size      = new Size(360, 500);
            panelLeft.BackColor = Theme.Primary;

            lblAppName           = new Label();
            lblAppName.Text      = "VELVET BEANS";
            lblAppName.Font      = new Font("Segoe UI", 22f, FontStyle.Bold);
            lblAppName.ForeColor = Theme.Accent;      // Caramel gold colour
            lblAppName.AutoSize  = false;
            lblAppName.TextAlign = ContentAlignment.MiddleCenter;
            lblAppName.Bounds    = new Rectangle(0, 150, 360, 52);

            lblTagline           = new Label();
            lblTagline.Text      = "Cafe Management System";
            lblTagline.Font      = new Font("Segoe UI", 11f);
            lblTagline.ForeColor = Color.FromArgb(200, 180, 160);
            lblTagline.AutoSize  = false;
            lblTagline.TextAlign = ContentAlignment.MiddleCenter;
            lblTagline.Bounds    = new Rectangle(0, 208, 360, 30);

            lblSlogan           = new Label();
            lblSlogan.Text      = "Good coffee, great vibes.";
            lblSlogan.Font      = new Font("Segoe UI", 9f, FontStyle.Italic);
            lblSlogan.ForeColor = Color.FromArgb(160, 140, 120);
            lblSlogan.AutoSize  = false;
            lblSlogan.TextAlign = ContentAlignment.MiddleCenter;
            lblSlogan.Bounds    = new Rectangle(0, 250, 360, 24);

            panelLeft.Controls.Add(lblAppName);
            panelLeft.Controls.Add(lblTagline);
            panelLeft.Controls.Add(lblSlogan);

            // ---- Right login panel (white background) -------------------
            panelRight           = new Panel();
            panelRight.Location  = new Point(360, 0);
            panelRight.Size      = new Size(420, 500);
            panelRight.BackColor = Theme.Background;

            lblWelcome           = new Label();
            lblWelcome.Text      = "Welcome Back";
            lblWelcome.Font      = new Font("Segoe UI", 18f, FontStyle.Bold);
            lblWelcome.ForeColor = Theme.Primary;
            lblWelcome.AutoSize  = false;
            lblWelcome.Bounds    = new Rectangle(40, 80, 340, 44);

            lblSubHead           = new Label();
            lblSubHead.Text      = "Sign in to continue";
            lblSubHead.Font      = Theme.FontBody;
            lblSubHead.ForeColor = Color.Gray;
            lblSubHead.AutoSize  = false;
            lblSubHead.Bounds    = new Rectangle(40, 126, 340, 22);

            // Username label and textbox
            lblUser           = new Label();
            lblUser.Text      = "Username";
            lblUser.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblUser.ForeColor = Theme.Primary;
            lblUser.AutoSize  = false;
            lblUser.Bounds    = new Rectangle(40, 174, 340, 20);

            txtUsername             = new TextBox();
            txtUsername.Bounds      = new Rectangle(40, 196, 340, 30);
            txtUsername.Font        = Theme.FontBody;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.BackColor   = Color.White;
            txtUsername.Text        = "admin";  // Pre-filled for convenience

            // Password label and textbox
            lblPass           = new Label();
            lblPass.Text      = "Password";
            lblPass.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblPass.ForeColor = Theme.Primary;
            lblPass.AutoSize  = false;
            lblPass.Bounds    = new Rectangle(40, 238, 340, 20);

            txtPassword                       = new TextBox();
            txtPassword.Bounds                = new Rectangle(40, 260, 340, 30);
            txtPassword.Font                  = Theme.FontBody;
            txtPassword.BorderStyle           = BorderStyle.FixedSingle;
            txtPassword.BackColor             = Color.White;
            txtPassword.UseSystemPasswordChar = true;   // Shows ●●●● instead of text
            txtPassword.Text                  = "admin123";
            // Wire up the Enter key handler (defined in LoginForm.cs)
            txtPassword.KeyDown += new KeyEventHandler(this.txtPassword_KeyDown);

            // Error message label (hidden until a login fails)
            lblError           = new Label();
            lblError.Text      = string.Empty;
            lblError.Font      = Theme.FontSmall;
            lblError.ForeColor = Theme.Danger;   // Red text for errors
            lblError.AutoSize  = false;
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Bounds    = new Rectangle(40, 300, 340, 22);

            // Sign In button
            btnLogin                           = new Button();
            btnLogin.Text                      = "Sign In";
            btnLogin.Font                      = Theme.FontHeading;
            btnLogin.BackColor                 = Theme.Primary;
            btnLogin.ForeColor                 = Color.White;
            btnLogin.FlatStyle                 = FlatStyle.Flat;
            btnLogin.Cursor                    = Cursors.Hand;
            btnLogin.Bounds                    = new Rectangle(40, 330, 340, 44);
            btnLogin.FlatAppearance.BorderSize = 0;
            // Wire up the click handler (defined in LoginForm.cs)
            btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            // Add all controls to the right panel
            panelRight.Controls.Add(lblWelcome);
            panelRight.Controls.Add(lblSubHead);
            panelRight.Controls.Add(lblUser);
            panelRight.Controls.Add(txtUsername);
            panelRight.Controls.Add(lblPass);
            panelRight.Controls.Add(txtPassword);
            panelRight.Controls.Add(lblError);
            panelRight.Controls.Add(btnLogin);

            // Add both panels to the form
            this.Controls.Add(panelLeft);
            this.Controls.Add(panelRight);
        }
    }
}
