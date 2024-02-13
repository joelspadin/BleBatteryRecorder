# BLE Battery Recorder

This is a Windows background service which periodically records the battery levels of Bluetooth devices. It runs in the background and logs battery levels to `C:\ProgramData\BLE Battery Recorder\battery.csv` every 10 minutes.

## Setup

Install the following:

1. [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2. [PowerShell](https://github.com/PowerShell/PowerShell/releases/latest)
3. [Python](https://www.python.org/downloads/)

Clone this repo:

```bash
git clone https://github.com/joelspadin/BleBatteryRecorder.git
```

To install the Windows service, open a terminal as administrator and navigate to the directory containing the `.csproj` file, then run the `Install.ps1` script:

```bash
cd BleBatteryRecorder/BleBatteryRecorder
./Install.ps1
```

To remove the service, run `Uninstall.ps1`.

Alternatively, this program can be run as a standard console application. The log file is then written to the current directory instead of to `C:\ProgramData`.

```bash
cd BleBatteryRecorder/BleBatteryRecorder
dotnet run
```

## Graphing Results

Results are written to a CSV file. To display them as a graph, open a terminal and navigate to the directory containing the `.csproj` file.

Optionally, create a virtual environment to avoid installing dependencies globally:

```bash
python -m venv .venv
.venv\Scripts\Activate.ps1
```

Next, install the dependencies:

```bash
pip install -r requirements.txt
```

Then run the `graph.py` script. You can provide a path to a CSV file as the first argument if you want to graph a different file.

```bash
./graph.py
```
