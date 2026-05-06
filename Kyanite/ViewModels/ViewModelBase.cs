using ReactiveUI;
using Splat;

namespace Kyanite.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        protected T GetService<T>()
        {
            var service = Locator.Current.GetService<T>();

            if (service is null)
                throw new System.Exception("Unknown service type: " + typeof(T).FullName);

            return service;
        }
    }
}
