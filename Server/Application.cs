// LFInteractive LLC. - All Rights Reserved
using Chase.WebDeploy.Core;
using Chase.WebDeploy.Core.Controllers;
using Serilog;
using Serilog.Events;

namespace Chase.WebDeploy.Server;

public static class Application
{
    private static readonly int PORT = ConfigurationController.Instance.Settings.Port;

    private static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(LogEventLevel.Information)
            .WriteTo.File(Path.Combine(Values.Directories.Logs, "debug.log"), LogEventLevel.Verbose, buffered: true, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 5_000_000)
            .WriteTo.File(Path.Combine(Values.Directories.Logs, "latest.log"), LogEventLevel.Information, buffered: true, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 5_000_000)
            .CreateLogger();
        Log.Information("Starting Kestral Template");
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