using Avalonia;
using LabelWhisper.Logic;
using Microsoft.Extensions.Logging;
using neXn.Lib.ConfigurationHandler;
using QuestPDF.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using SixLabors.ImageSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LabelWhisper
{
    internal static class Program
    {
        private readonly static LogEventLevel minimumLevel = LogEventLevel.Verbose;

        public static string AppLocalBasePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "neXn-Systems", "LabelWhisper");

        [STAThread]
        public static void Main(string[] args)
        {
            // Setup logger
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console(restrictedToMinimumLevel: minimumLevel)
            .WriteTo.Debug()
            .Enrich.WithProperty("Application", typeof(Program).Assembly.GetName().Name)
            .CreateLogger();

            Microsoft.Extensions.Logging.ILogger logger = new SerilogLoggerProvider().CreateLogger("app");

            logger.LogInformation("Starting up");

            QuestPDF.Settings.License = LicenseType.Community;
            logger.LogTrace("QuestPDF Community license set");

            string userConfigPath = Path.Combine(AppLocalBasePath, "config", "config.json");

            if (string.IsNullOrEmpty(userConfigPath))
            {
                userConfigPath = Path.Combine(AppContext.BaseDirectory, "config", "config.json");
            }

            Globals.UserConfig = new ConfigurationHandler<Models.Configuration>(new(userConfigPath));
            Globals.UserConfig.Load().Wait();
            logger.LogInformation("Loaded user config");

            logger.LogTrace("Deploying resources...");
            Stopwatch sw = Stopwatch.StartNew();
            // Deploy extern resources
            foreach (string f in Globals.Assembly.GetManifestResourceNames().Where(x => x.Contains(".Extern.")))
            {
                if (f.EndsWith("exe") && OperatingSystem.IsWindows())
                {
                    string filename = string.Join('.', f.Split('.').Skip(2));

                    string filepath = Path.Combine(AppLocalBasePath, filename);

                    if (!File.Exists(filepath))
                    {
                        using (Stream s = Globals.Assembly.GetManifestResourceStream($"{Globals.Assembly.GetName().Name}.Extern.{filename}"))
                        {
                            using (FileStream fs = File.Create(filepath))
                            {
                                s.CopyTo(fs);
                            }
                        }
                    }
                }
            }
            sw.Stop();
            logger.LogTrace("Deployed resources in {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);

            //Discover Printers
            Task.Run(() =>
            {
                Globals.AppStatus.Change("Reading printers...", true);

                foreach (string p in neXn.Lib.PrinterManagement.Query.GetInstalledPrinters(neXn.Lib.PrinterManagement.Query.PrinterFilter.OnlyLabelPrinters))
                {
                    Globals.IntalledPrinters.Add(p);
                }

                logger.LogInformation("Found ({Printercount}) printers on system", Globals.IntalledPrinters.Count);

                Globals.AppStatus.Change($"Found ({Globals.IntalledPrinters.Count}) printers on system", false, true);
            });

            logger.LogTrace("Loading/building app...");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                        .UsePlatformDetect()
                        .WithInterFont()
                        .LogToTrace();
        }
    }
}
