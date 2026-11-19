using Kyanite.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kyanite.Services;

/// <summary>
/// the goal of this class is making it able to dispose shell after app is closed
/// </summary>
internal class ShellViewModelProvider
{
    readonly IServiceProvider _serviceProvider;

    ShellViewModel? shellViewModel;

    public ShellViewModelProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ShellViewModel CreateOrGetShell()
    {
        shellViewModel ??= ActivatorUtilities.CreateInstance<ShellViewModel>(_serviceProvider);
        return shellViewModel;
    }

    public bool TryRetriveShellCurrentViewModel<T>(out T viewModel) where T : ViewModelBase
    {
        viewModel = default!;

        if (shellViewModel is null)
            return false;

        if (shellViewModel.CurrentViewModel is not T)
            return false;

        viewModel = (T)shellViewModel.CurrentViewModel;
        return true;
    }

    public void DisposeShell()
    {
        shellViewModel = null;
    }
}
