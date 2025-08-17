using Avalonia.Collections;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;
using Whiteboard.Models;
using Whiteboard.Modules;
using Whiteboard.Modules.Reminder;
using Whiteboard.Modules.Text;
using Whiteboard.Services;
using Whiteboard.Services.DataItems;
using Whiteboard.Services.Modules;
using Whiteboard.ViewModels.Popups;

namespace Whiteboard.ViewModels;

public class MainViewModel : ViewModelBase
{
    public RoutingState Router { get; }

    GoogleDriveConnection driveConnection;
    LiteDbConnection liteDBConnection;

    IDataItemDatabase dataDatabase;
    IModuleDatabase moduleDatabase;

    PopupService popupService;

    AvaloniaList<ModuleInfo> modules { get; set; }
    ModuleManager moduleMananger;

    [Reactive] int width { get; set; } = 240;

    public MainViewModel()
    {
        Router = new RoutingState();

        //database connection initialization
        liteDBConnection = GetService<LiteDbConnection>();
        driveConnection = GetService<GoogleDriveConnection>();

        driveConnection.OnInitialized += () =>
        {
            liteDBConnection.OpenConnection();
            GetModules();
        };

        //database management initialization
        dataDatabase = GetService<IDataItemDatabase>();
        moduleDatabase = GetService<IModuleDatabase>();

        //popups
        popupService = GetService<PopupService>();
        popupService.OnPopupClosed += GetModules;

        //module manager initialization
        modules = new AvaloniaList<ModuleInfo>();
        moduleMananger = new(dataDatabase);

        //this is for after user closes app and reopens it
        if (driveConnection.IsInitialized)
            GetModules();
    }

    //button
    public void SwitchModule(object moduleInfoObj)
    {
        ModuleInfo moduleInfo = (ModuleInfo)moduleInfoObj;

        Module module = null;

        //TODO: this should be automated later on 
        if (moduleInfo.Type == nameof(TextModule))
        {
            module = new TextModule();
            Router.Navigate.Execute(module);
        }
        else if (moduleInfo.Type == nameof(ReminderModule))
        {
            module = new ReminderModule();
            Router.Navigate.Execute(module);
        }

        if (module is null)
            throw new System.Exception("Wrong module info type has been read from database");

        moduleMananger.LoadModule(moduleInfo.ID!.Value, module);
    }

    //button
    public void AddModule() => popupService.ShowPopup(new ModuleCreationViewModel());

    //button
    public void DeleteModule(object moduleInfoObj)
    {
        ModuleInfo moduleInfo = (ModuleInfo)moduleInfoObj;
        moduleDatabase.DeleteModule(moduleInfo.ID.Value);

        GetModules();
    }

    //button
    public void OpenRename(object moduleInfoObj)
    {
        ModuleInfo moduleInfo = (ModuleInfo)moduleInfoObj;
        popupService.ShowPopup(new RenameViewModel(moduleInfo));
    }

    //button
    public void ChangeWidth()
    {
        if (width == 240)
            width = 40;
        else
            width = 240;
    }

    //button
    public void UploadChanges() => _ = SaveDatabase();

    //module list management
    void GetModules()
    {
        modules.Clear();
        modules.AddRange(moduleDatabase.GetModules());
    }

    async Task SaveDatabase()
    {
        await driveConnection.SaveDatabase();
        GetService<INotificationService>().ShowNotification("Whiteboard", "Synchronization sucessful");
    }

    //from ViewModelBase
    public override void OnClose() => _ = SaveDatabase();
}
