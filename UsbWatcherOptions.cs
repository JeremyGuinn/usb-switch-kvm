public class UsbWatcherOptions
{
    public string VendorId { get; set; }
    public string ProductId { get; set; }

    public MonitorBehavior[] ConnectedBehaviors { get; set; }
    public MonitorBehavior[] DisconnectedBehaviors { get; set; }
}

public class MonitorBehavior
{
    public string MonitorID { get; set; }
    public string Input { get; set; }
}