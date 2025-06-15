using Avalonia.Collections;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Diagnostics;
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
    public AvaloniaList<ModuleInfo> modules { get; set; }
    public RoutingState Router { get; }

    GoogleDriveConnection driveConnection;
    LiteDbConnection liteDBConnection;

    IDataItemDatabase dataDatabase;
    IModuleDatabase moduleDatabase;

    PopupService popupService;

    ModuleManager moduleMananger;

    [Reactive] int width { get; set; }= 240;

    public MainViewModel()
    {
        Router = new RoutingState();

        liteDBConnection = GetService<LiteDbConnection>();
        driveConnection = GetService<GoogleDriveConnection>();

        driveConnection.OnInitialized += () =>
        {
            liteDBConnection.OpenConnection();
            GetModules();
        };

        dataDatabase = GetService<IDataItemDatabase>();
        moduleDatabase = GetService<IModuleDatabase>();

        popupService = GetService<PopupService>();
        popupService.OnPopupClosed += GetModules;

        moduleMananger = new(dataDatabase);
        modules = new AvaloniaList<ModuleInfo>();
    }

    void GetModules()
    {
        modules.Clear();
        modules.AddRange(moduleDatabase.GetModules());
    }

    public async Task SwitchModule(object moduleInfoObj)
    {
        ModuleInfo moduleInfo = (ModuleInfo)moduleInfoObj;

        Module module = null;

        if (moduleInfo.Type == nameof(TextModule))
        {
            module = new TextModule();
            Router.Navigate.Execute(module);
        }
        else if(moduleInfo.Type == nameof(ReminderModule))
        {
            module = new ReminderModule();
            Router.Navigate.Execute(module);
        }

        if (module is null)
            throw new System.Exception("Wrong module info type has been read from database");

        await moduleMananger.LoadModule(moduleInfo.ID!.Value, module);
    }

    public async void AddModule()
    {
        ModuleInfo moduleInfo = new ModuleInfo() 
        {
            Type = nameof(TextModule),
            Name = nameof(TextModule) 
        };

        moduleDatabase.AddModule(moduleInfo);

        GetModules();
    }

    public async void DeleteModule(object moduleInfoObj)
    {
        ModuleInfo moduleInfo = (ModuleInfo)moduleInfoObj;
        moduleDatabase.DeleteModule(moduleInfo.ID.Value);

        GetModules();
    }

    public void OpenRename(object moduleInfoObj)
    {
        ModuleInfo moduleInfo = (ModuleInfo)moduleInfoObj;
        popupService.ShowPopup(new RenameViewModel(moduleInfo));
    }

    public void ChangeWidth()
    {
        if (width == 240)
            width = 40;
        else
            width = 240;
    }

    public void UploadChanges() => _ = SaveDatabase();
    public override void OnClose() => _ = SaveDatabase();
    async Task SaveDatabase() => await driveConnection.SaveDatabase();
}
