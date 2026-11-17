using Kyanite.Core.Models;

namespace Kyanite.Core.Services;

public interface IAppSettingsService
{
    AppSettings? CurrentAppSettings { get; }
    public void Save() => Save(CurrentAppSettings ?? new());
    void Save(AppSettings appSettings);
}
