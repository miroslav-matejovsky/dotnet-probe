namespace WinFormsApp;

public partial class ExternalWinLoginForm : Form
{
    public ExternalWinLoginForm()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        var dialog = new CredentialsDialog("Main Program");
        dialog.Show();
    }
}