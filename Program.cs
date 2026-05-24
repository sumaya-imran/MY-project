// =================================================================
//  VelvetBeans Cafe Management System
//  File: Program.cs
//  Purpose: This is where the application starts (Main method).
//           It sets up Windows visual styles, creates the database
//           file if it doesn't exist, then opens the Login screen.
// =================================================================

using System;
using System.Windows.Forms;
using VelvetBeans.Database;

namespace VelvetBeans
{
    internal static class Program
    {
        // [STAThread] is required for all Windows Forms apps.
        // It sets the COM threading model to Single-Threaded Apartment.
        [STAThread]
        static void Main()
        {
            // Enable modern Windows button/control styles (rounded corners etc.)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Initialise the SQLite database.
                // This creates VelvetBeans.db next to the .exe on first run.
                // On subsequent runs it just checks the tables exist — safe to call every time.
                DatabaseHelper.InitialiseDatabase();
            }
            catch (Exception ex)
            {
                // If the database can't be created, show an error and exit.
                // Common cause: the folder is read-only.
                MessageBox.Show(
                    "Database could not be initialised:\n\n" + ex.Message +
                    "\n\nMake sure the application folder is writable.",
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Open the Login form and start the Windows message loop.
            // The app runs until the Login form's message loop ends.
            Application.Run(new Forms.LoginForm());
        }
    }
}
