using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;

using System;

namespace WebAppLab
{
	public class Program
	{
		public static int Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Information)
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.WriteTo.Debug()
				.CreateBootstrapLogger();
			try
			{
				Log.Information("Starting web host");
				CreateHostBuilder(args).Build().Run();
				return 0;
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Host terminated unexpecredly");
				return 1;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration(b => b.AddJsonFile("appsettings.Serilog.json"))
				.UseSerilog((context, configuration) => configuration
					.ReadFrom.Configuration(context.Configuration)
					// Unable to add this setting in appSettings.json: https://github.com/serilog/serilog-sinks-map#configuration-with-appsettings-and-appsettingsjson.
					.WriteTo.Map(evt => evt.Level, (level, wt) => wt.File($"Logs\\{level}-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)))
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
