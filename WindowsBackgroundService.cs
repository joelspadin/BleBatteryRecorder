using System.Globalization;

namespace BleBatteryRecorder;

public class WindowsBackgroundService(IConfiguration config, ILogger<WindowsBackgroundService> logger) : BackgroundService
{
	private const int DefaultPeriodMinutes = 10;
	private readonly string DefaultLogFilePath = Path.Join(
		Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
		"BLE Battery Recorder",
		"battery.csv");
	private readonly string FallbackLogFilePath = "battery.csv";

	private readonly IConfiguration _config = config;
	private readonly ILogger<WindowsBackgroundService> _logger = logger;

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		var timer = new PeriodicTimer(period: GetPeriod());

		while (!cancellationToken.IsCancellationRequested)
		{
			await WriteSample();
			await timer.WaitForNextTickAsync(cancellationToken);
		}
	}

	private async Task WriteSample()
	{
		var timestamp = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);

		using var csv = OpenLogFile();
		if (csv.BaseStream.Position == 0)
		{
			csv.WriteLine("Time,Name,Battery");
		}

		await foreach (var device in DeviceBatteries.FindAllAsync())
		{
			foreach (var value in device.Levels)
			{
				var name = GetName(device.Name, value.Description);

				_logger.LogDebug("{Name} {Level}", name, value.Value);
				csv.WriteLine($"{timestamp},{Escape(name)},{value.Value}");
			}
		}
	}

	private StreamWriter OpenLogFile()
	{
		try
		{
			var logfile = GetLogFilePath();
			return OpenLogFile(logfile);
		}
		catch (UnauthorizedAccessException)
		{
			return OpenLogFile(FallbackLogFilePath);
		}
	}

	private static StreamWriter OpenLogFile(string logfile)
	{
		var directory = Path.GetDirectoryName(logfile);
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}

		return File.AppendText(logfile);
	}

	private TimeSpan GetPeriod()
	{
		var period = _config["Period"];
		return TimeSpan.FromMinutes(period != null ? int.Parse(period) : DefaultPeriodMinutes);
	}

	private string GetLogFilePath()
	{
		return _config["LogFile"] ?? DefaultLogFilePath;
	}

	static string Escape(string s)
	{
		if (s.Contains(','))
		{
			return $"\"{s.Replace("\"", "\"\"")}\"";
		}

		return s;
	}

	static string GetName(string name, string description)
	{
		if (string.IsNullOrEmpty(description))
		{
			return name;
		}

		return $"{name} ({description})";
	}
}
