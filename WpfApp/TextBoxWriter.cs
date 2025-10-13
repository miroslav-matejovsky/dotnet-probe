using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace dotnet_probe;

public class TextBoxWriter(TextBox textBox) : TextWriter
{
    private readonly Dispatcher _dispatcher = textBox.Dispatcher;

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        _dispatcher.Invoke(() =>
        {
            textBox.AppendText(value.ToString());
            textBox.ScrollToEnd();
        });
    }

    public override void Write(string? value)
    {
        if (value == null) return;
        _dispatcher.Invoke(() =>
        {
            textBox.AppendText(value);
            textBox.ScrollToEnd();
        });
    }

    public override void WriteLine(string? value)
    {
        Write(value + "\r\n");
    }
}