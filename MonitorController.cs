using System.Diagnostics;

namespace Development
{
    public class MonitorController
    {
        public void SetMonitorInput(string input, string monitorId)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "monitorcontrol",
                    Arguments = $"--set-input-source {input} --monitor {monitorId}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
        }
    }
}
