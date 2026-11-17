using System.Collections.Generic;

namespace Kyanite.Models;

internal record AppSettings
{
    public bool DatabaseinformationInitialized { get; set; } = false;
    public bool KeepRunningInBackground { get; set; } = true;
    public bool EnableNotifications { get; set; } = true;
    public bool NotifyOnOverdue { get; set; } = true;
    public int DueSoonThresholdMinutes { get; set; } = 30;
    public string? DatabaseStackType { get; set; }
    public Dictionary<string, string> DatabaseInformation { get; set; } = [];
}
