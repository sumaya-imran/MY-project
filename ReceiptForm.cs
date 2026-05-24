// =================================================================
//  VelvetBeans Cafe Management System
//  File: Forms/ReceiptForm.cs
//  Purpose: Modal dialog shown after a successful order placement.
//           Displays a plain-text receipt in a monospaced font.
//           Uses ReceiptBuilder from Helpers.cs to generate the text.
// =================================================================

using System.Windows.Forms;
using VelvetBeans.Helpers;
using VelvetBeans.Models;

namespace VelvetBeans.Forms
{
    public partial class ReceiptForm : Form
    {
        // Constructor receives the completed Order model from NewOrderForm
        public ReceiptForm(Order order)
        {
            InitializeComponent();

            // Update the header label with the order number
            lblHeader.Text = "Order #" + order.OrderId + " placed successfully!";

            // Generate the full receipt text and display it
            // ReceiptBuilder.Build() formats the order like a thermal receipt
            txtReceipt.Text = ReceiptBuilder.Build(order);
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
