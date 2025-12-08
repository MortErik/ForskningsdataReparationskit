using System;
using System.Windows.Forms;

namespace TestvaerkstedetToolkit
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Sæt DPI awareness mode (nødvendigt for .NET 8)
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start med dashboard i stedet for Form1
            Application.Run(new WelcomeForm());
        }
    }
}
