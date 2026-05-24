// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/ReceiptForm.Designer.cs
// =================================================================

using System.Drawing;
using System.Windows.Forms;
using VelvetBeans.Helpers;

namespace VelvetBeans.Forms
{
    partial class ReceiptForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private Panel       panelHeader = null!;
        private Label       lblHeader   = null!;
        private RichTextBox txtReceipt  = null!;
        private Panel       panelFooter = null!;
        private Button      btnClose    = null!;

        private void InitializeComponent()
        {
            this.Text            = "Receipt";
            this.ClientSize      = new Size(420, 580);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.BackColor       = System.Drawing.Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.Font            = Theme.FontBody;

            // Header — shows "Order #X placed successfully!"
            panelHeader           = new Panel();
            panelHeader.Dock      = DockStyle.Top;
            panelHeader.Height    = 48;
            panelHeader.BackColor = Theme.Primary;

            lblHeader           = new Label();
            lblHeader.Text      = string.Empty;  // Set in ReceiptForm.cs constructor
            lblHeader.Font      = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblHeader.ForeColor = Theme.Accent;
            lblHeader.AutoSize  = true;
            lblHeader.Location  = new Point(12, 12);
            panelHeader.Controls.Add(lblHeader);

            // Receipt text (Courier New = monospaced = receipt look)
            txtReceipt            = new RichTextBox();
            txtReceipt.Dock       = DockStyle.Fill;
            txtReceipt.Font       = new Font("Courier New", 10f);
            txtReceipt.BackColor  = System.Drawing.Color.White;
            txtReceipt.ForeColor  = System.Drawing.Color.Black;
            txtReceipt.ReadOnly   = true;
            txtReceipt.BorderStyle= BorderStyle.None;
            txtReceipt.ScrollBars = RichTextBoxScrollBars.Vertical;

            // Footer with Close button
            panelFooter           = new Panel();
            panelFooter.Dock      = DockStyle.Bottom;
            panelFooter.Height    = 48;
            panelFooter.BackColor = Theme.Background;

            btnClose                           = new Button();
            btnClose.Text                      = "Close";
            btnClose.Font                      = Theme.FontBody;
            btnClose.BackColor                 = Theme.Primary;
            btnClose.ForeColor                 = System.Drawing.Color.White;
            btnClose.FlatStyle                 = FlatStyle.Flat;
            btnClose.Cursor                    = Cursors.Hand;
            btnClose.Bounds                    = new Rectangle(290, 8, 110, 32);
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += new System.EventHandler(this.btnClose_Click);
            panelFooter.Controls.Add(btnClose);

            this.Controls.Add(panelHeader);
            this.Controls.Add(txtReceipt);
            this.Controls.Add(panelFooter);
        }
    }
}
