using WinFormsApp;

namespace WinFormsApp1;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        if (Login(null)) Application.Run(new Form());
    }
    
    private static bool Login(string name)
    {
        bool value = false;
        try
        {
            CredentialsDialog dialog = new CredentialsDialog("Secure Application");
            if (name != null) dialog.AlwaysDisplay = true; // prevent an infinite loop
            if (dialog.Show(name) == DialogResult.OK)
            {
                if (Authenticate(dialog.Name, dialog.Password))
                {
                    value = true;
                    if (dialog.SaveChecked) dialog.Confirm(true);
                }
                else
                {
                    try
                    {
                        dialog.Confirm(false);
                    }
                    catch (ApplicationException applicationException)
                    {
                        // exception handling ...
                    }
                    value = Login(dialog.Name); // need to find a way to display 'Logon unsuccessful'
                }
            }
        }
        catch (ApplicationException applicationException)
        {
            // exception handling ...
        }
        return value;
    }
       
    private static bool Authenticate(string name, string password)
    {
        Console.WriteLine("Authenticating {0}/{1}", name, password);
        return true;
    }
}