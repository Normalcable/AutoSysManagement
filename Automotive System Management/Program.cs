using System;
using System.Windows.Forms;

namespace Automotive_System_Management
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create an instance of the main form (form1 form)
            Login mainForm = new Login();

            // Run the application and start the message loop with the main form
            Application.Run(mainForm);
        }
    }
}
