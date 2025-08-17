using ReactiveUI;
using System;
namespace Whiteboard.ViewLocators
{
    //viewlocator used for modules
    public class ModuleViewLocator : IViewLocator
    {
        IViewFor IViewLocator.ResolveView<T>(T viewModel, string contract)
        {
            var name = viewModel!.GetType().FullName + "View";
            var type = Type.GetType(name);

            return (IViewFor)Activator.CreateInstance(type!)!;
        }
    }
}
