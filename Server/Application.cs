/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Vesta.Core;
using Chase.Vesta.Core.Controllers;
using Serilog;
using Serilog.Events;

namespace Chase.Vesta.Server;

public static class Application
{
    private static readonly int PORT = ConfigurationController.Instance.Settings.Port;

    private static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(LogEventLevel.Debug)
            .WriteTo.File(Path.Combine(Values.Directories.Logs, "debug.log"), LogEventLevel.Verbose, buffered: true, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 5_000_000)
            .WriteTo.File(Path.Combine(Values.Directories.Logs, "latest.log"), LogEventLevel.Information, buffered: true, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 5_000_000)
            .CreateLogger();
        Log.Information("Starting {NAME}", Values.ApplicationName);
        if (OperatingSystem.IsWindows())
        {
            if (!Environment.Is64BitOperatingSystem)
            {
                Log.Error("Windows ARM is not currently supported!");
                return;
            }
        }

        Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseIISIntegration();
                builder.UseContentRoot(Directory.GetCurrentDirectory());
                builder.UseKestrel(options =>
                {
                    options.ListenAnyIP(PORT);
                });
                builder.UseStartup<Startup>();
                Log.Information("Server running at {SERVER}", $"http://127.0.0.1:{PORT}");
            }).Build().Run();
    }
}

internal class Startup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment evn)
    {
        app.UseForwardedHeaders();
        app.UseStaticFiles();
        app.UseDefaultFiles();
        app.UseRouting();
        app.UseSession();
        app.UseMvc();
        app.UseSerilogRequestLogging();
    }

    public void ConfigureServices(IServiceCollection service)
    {
        service.AddMvc(action =>
        {
            action.EnableEndpointRouting = false;
        });
        service.AddSession(action =>
        {
            action.IOTimeout = TimeSpan.FromSeconds(10);
            action.IdleTimeout = TimeSpan.FromSeconds(10);
        });
    }
}