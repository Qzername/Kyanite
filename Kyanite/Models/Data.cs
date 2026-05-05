using System;

namespace Kyanite.Modules;

public struct Data
{
    public Guid Id { get; set; }
    public Guid ModuleId { get; set; }
    public string Name { get; set; }
    public string TypeName { get; set; }
    public object Information { get; set; }
}
