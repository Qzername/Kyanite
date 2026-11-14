namespace Kyanite.Services;

public interface IAppSettingsService
{
    bool KeepRunningInBackground { get; }
    void SetKeepRunningInBackground(bool value);
}
