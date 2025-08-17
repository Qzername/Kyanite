using ReactiveUI;
using Splat;

namespace Whiteboard.ViewModels;

public class ViewModelBase : ReactiveObject
{
    /// <summary>
    /// On window close 
    /// </summary>
    public virtual void OnClose()
    {

    }

    protected T GetService<T>()
    {
        var service = Locator.Current.GetService<T>();

        if (service is null)
            throw new System.Exception("Unknown service type: " + typeof(T).FullName);

        return service;
    }
}
