using System.Configuration;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Windows;
using Serilog;

namespace dotnet_probe;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    
    // Configure Serilog to write to the console
    public App()
    {
        // Configure Serilog to write to the console
        const string template = "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("System.Net.Http", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Extensions.Http", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: template)
            .CreateLogger();
    }
}