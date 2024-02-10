using System.Management;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace Development
{

    public class UsbWatcher
    {
        private readonly MonitorController _monitorController;
        private readonly ILogger _logger;
        private readonly UsbWatcherOptions _options;

        private ManagementEventWatcher connectWatcher;
        private ManagementEventWatcher disconnectWatcher;
        private bool isRunning = false;

        public UsbWatcher(
            ILogger<UsbWatcher> logger,
            MonitorController monitorController,
            IOptions<UsbWatcherOptions> options)
        {
            _monitorController = monitorController;
            _logger = logger;
            _options = options.Value;

            var connectQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBControllerDevice'");
            var disconnectQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBControllerDevice'");

            connectWatcher = new ManagementEventWatcher(connectQuery);
            disconnectWatcher = new ManagementEventWatcher(disconnectQuery);

            connectWatcher.EventArrived += (sender, eventArgs) => OnUsbChanged(eventArgs, true);
            disconnectWatcher.EventArrived += (sender, eventArgs) => OnUsbChanged(eventArgs, false);

        }

        public void Start()
        {
            if (isRunning) return;

            _logger.LogInformation("Starting USBWatcher");
            isRunning = true;
            connectWatcher.Start();
            disconnectWatcher.Start();
        }

        public void Stop()
        {
            if (!isRunning) return;

            _logger.LogInformation("Stoping USBWatcher");
            isRunning = false;
            connectWatcher.Stop();
            disconnectWatcher.Stop();
        }

        private void OnUsbChanged(EventArrivedEventArgs e, bool connected)
        {
            var targetInstance = e.NewEvent["TargetInstance"] as ManagementBaseObject;
            if (targetInstance != null)
            {
                string dependent = targetInstance["Dependent"].ToString();

                var match = Regex.Match(dependent, @"DeviceID=""(.+?)""");
                if (match.Success)
                {
                    string deviceId = match.Groups[1].Value;
                    _logger.LogDebug($"Device {(connected ? "Connected" : "Disconnected")}: {deviceId}");

                    string targetDeviceId = $"VID_{_options.VendorId}&PID_{_options.ProductId}";
                    if (deviceId.Contains(targetDeviceId))
                    {
                        RunScript(connected);
                    }
                }
            }
        }


        private void RunScript(bool connected)
        {
            var behaviors = connected ? _options.ConnectedBehaviors : _options.DisconnectedBehaviors;

            foreach (var behavior in behaviors)
            {
                _monitorController.SetMonitorInput(behavior.Input, behavior.MonitorID);
            }
        }
    }

}
