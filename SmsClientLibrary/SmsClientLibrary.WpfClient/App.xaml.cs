using Microsoft.Extensions.Configuration;

using Serilog;

using System.IO;
using System.Windows;

namespace SmsClientLibrary.WpfClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IConfiguration Configuration { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var logPath = Path.Combine(AppContext.BaseDirectory, $"test-sms-wpf-wpf-app-{DateTime.Now:yyyyMMdd}.log");
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(logPath)
            .CreateLogger();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}

