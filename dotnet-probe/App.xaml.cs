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
    
    public static IConfiguration Config { get; private set; }

    // Configure Serilog to write to the console
    public App()
    {
        // Configure Serilog to write to the console
        const string template = "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: template)
            .CreateLogger();
        
        Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }
}