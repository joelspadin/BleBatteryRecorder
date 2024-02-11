using Microsoft.Extensions.Logging.EventLog;

namespace BleBatteryRecorder;

public class Program
{
	public static void Main(string[] args)
	{
		CreateHostBuilder(args).Build().Run();
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureLogging(logging =>
			{
				logging.ClearProviders();
				logging.AddConsole();

				logging.AddEventLog(new EventLogSettings()
				{
					SourceName = "BLE Battery Recorder",
					LogName = "BLE Battery Recorder",
				});
			})
			.ConfigureServices((hostContext, services) =>
			{
				services.AddHostedService<WindowsBackgroundService>();
			})
			.UseWindowsService(options =>
			{
				options.ServiceName = "BLE Battery Recorder";
			});
}

