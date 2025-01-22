using ReactiveUI;
using System;
using System.Diagnostics;
namespace Whiteboard.ModuleManagement
{
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
