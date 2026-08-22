using CommunityToolkit.Mvvm.ComponentModel;
using Kyanite.Database;

namespace Kyanite.Modules;

public abstract class Module : ObservableObject
{
     public Guid ModuleId { get; }
     public string Name { get; }

     protected Module(ModuleInformation moduleInformation)
     {
          ModuleId = moduleInformation.Id;
          Name = moduleInformation.Name;
     }

     public void Refresh()
     {
          OnPropertyChanged(string.Empty);
     }
}
