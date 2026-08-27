using System.Collections.Generic;

namespace Kyanite.Models;

internal record AppSettings
{
    public bool DatabaseinformationInitialized { get; set; } = false;
    public Dictionary<string, string> DatabaseInformation { get; set; } = [];
}
