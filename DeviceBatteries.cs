using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BleBatteryRecorder;

public class BatteryLevel(GattCommunicationStatus status, byte value, string description)
{
	public GattCommunicationStatus Status { get; } = status;
	public byte Value { get; } = value;
	public string Description { get; } = description;
}

public class DeviceBatteries(string name, IEnumerable<BatteryLevel> levels)
{
	public string Name { get; } = name;
	public IEnumerable<BatteryLevel> Levels { get; } = levels;

	static public async IAsyncEnumerable<DeviceBatteries> FindAllAsync()
	{
		var selector = BluetoothLEDevice.GetDeviceSelectorFromConnectionStatus(BluetoothConnectionStatus.Connected);
		var devices = await DeviceInformation.FindAllAsync(selector);

		foreach (var device in devices)
		{
			using var bleDevice = await GetBluetoothLEDeviceAsync(device);
			if (bleDevice == null)
			{
				continue;
			}

			await foreach (var service in GetBatteryServicesAsync(bleDevice))
			{
				var characteristics = await GetBatteryCharacteristicsAsync(service);
				var levels = await Task.WhenAll(characteristics.Select(GetBatteryLevelAsync));
				yield return new DeviceBatteries(bleDevice.Name, levels);
			}
		}
	}

	static async Task<BluetoothLEDevice?> GetBluetoothLEDeviceAsync(DeviceInformation device)
	{
		try
		{
			return await BluetoothLEDevice.FromIdAsync(device.Id);
		}
		catch (ArgumentException)
		{
			return null;
		}
	}

	static async IAsyncEnumerable<GattDeviceService> GetBatteryServicesAsync(BluetoothLEDevice device)
	{
		var result = await device.GetGattServicesForUuidAsync(GattServiceUuids.Battery);
		if (result == null)
		{
			yield break;
		}

		foreach (var service in result.Services)
		{
			yield return service;
		}
	}

	static async Task<IEnumerable<GattCharacteristic>> GetBatteryCharacteristicsAsync(GattDeviceService service)
	{
		var result = await service.GetCharacteristicsForUuidAsync(GattCharacteristicUuids.BatteryLevel);
		if (result == null)
		{
			return Enumerable.Empty<GattCharacteristic>();
		}

		return result.Characteristics.Where(c => c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read));
	}

	static async Task<BatteryLevel> GetBatteryLevelAsync(GattCharacteristic characteristic)
	{
		byte value = 0;
		var result = await characteristic.ReadValueAsync();
        if (result.Status == GattCommunicationStatus.Success)
        {
            var reader = DataReader.FromBuffer(result.Value);
			value = reader.ReadByte();
        }

        return new BatteryLevel(
			status: result.Status,
			value: value,
			description: characteristic.UserDescription);
	}
}
