using Fenrus.Models;
using Fenrus.Models.UiModels;
using Fenrus.Services;
using Microsoft.AspNetCore.Components;

namespace Fenrus.Components.SideEditors;

/// <summary>
/// Group Item Editor
/// </summary>
public partial class GroupItemEditor
{
    [Parameter] public GroupItem Item { get; set; }
    
    [Parameter] public EventCallback<GroupItem> OnSaved { get; set; }
    [Parameter] public EventCallback OnCanceled { get; set; }

    private GroupItemEditorModel Model;
    private string Title;

    private Dictionary<string, FenrusApp> Apps;
    private List<ListOption> SmartApps, BasicApps;

    private FenrusApp SelectedApp;

    private string SelectedAppName
    {
        get => SelectedApp?.Name;
        set
        {
            if (value != null && Apps.ContainsKey(value))
                SelectedApp = Apps[value];
            else
                SelectedApp = null;
            Model.AppName = value;
        }
    }

    protected override void OnInitialized()
    {
        Apps = AppService.Apps;
        SmartApps = Apps.Where(x => x.Value.IsSmart == true).OrderBy(x => x.Key)
            .Select(x => new ListOption() { Label = x.Key, Value = x.Key }).ToList();
        
        BasicApps = Apps.Where(x => x.Value.IsSmart == false).OrderBy(x => x.Key)
            .Select(x => new ListOption() { Label = x.Key, Value = x.Key }).ToList();
        
        this.Model = new();
        Title = "Edit Item";
        if (Item is AppItem app)
        {
            Model.ItemType = app.Type;
            Model.Target = app.Target;
            Model.Url = app.Url;
            Model.AppName = app.AppName;
            Model.DockerContainer = app.DockerContainer;
            Model.DockerUid = app.DockerUid;
            Model.DockerCommand = app.DockerCommand;
            Model.SshPassword = app.SshPassword;
            Model.SshServer = app.SshServer;
            Model.SshUserName = app.SshUserName;
            Model.Enabled = app.Enabled;
            Model.Icon = app.Icon;
            Model.Name = app.Name;
            Model.Size = app.Size;
            Model.Uid = app.Uid;
        }
        else if (Item is LinkItem link)
        {
            Model.ItemType = link.Type;
            Model.Target = link.Target;
            Model.Url = link.Url;
            Model.Enabled = link.Enabled;
            Model.Icon = link.Icon;
            Model.Name = link.Name;
            Model.Size = link.Size;
            Model.Uid = link.Uid;
        }
        else
        {
            Title = "New Item";
            Model.ItemType = "DashboardApp";
            Model.AppName = "FileFlows";
            Model.Target = string.Empty;
            Model.Url = "https://";
            Model.Enabled = true;
            Model.Icon = string.Empty;
            Model.Name = string.Empty;
            Model.Size = ItemSize.Medium;
            Model.Uid = Guid.Empty;
        }

        if (string.IsNullOrEmpty(Model.TerminalType))
            Model.TerminalType = "SSH";

        SelectedAppName = Model.AppName;
    }

    async Task Save()
    {
        // validate
        GroupItem result;
        switch (Model.ItemType)
        {
            case "DashboardApp":
                {
                    var app  = new AppItem();
                    app.Target = Model.Target;
                    app.Url = Model.Url;
                    app.AppName = Model.AppName;
                    app.DockerContainer = Model.DockerContainer;
                    app.DockerUid = Model.DockerUid;
                    app.DockerCommand = Model.DockerCommand;
                    app.SshPassword = Model.SshPassword;
                    app.SshServer = Model.SshServer;
                    app.SshUserName = Model.SshUserName;
                    app.Enabled = Model.Enabled;
                    app.Icon = Model.Icon;
                    app.Name = Model.Name;
                    app.Size = Model.Size;
                    app.Uid = Model.Uid;
                    result = app;
                }
                break;
            case "DashboardLink":
                {
                    var link = new LinkItem();
                    link.Target = Model.Target;
                    link.Url = Model.Url;
                    link.Enabled = Model.Enabled;
                    link.Icon = Model.Icon;
                    link.Name = Model.Name;
                    link.Size = Model.Size;
                    link.Uid = Model.Uid;
                    result = link;
                }
                break;
            default:
                throw new Exception("Unknown type: " + Model.ItemType);
        }

        await OnSaved.InvokeAsync(result);
    }

    Task Cancel()
        => OnCanceled.InvokeAsync();
}

class GroupItemEditorModel : GroupItem
{
    public override string Type => ItemType;
    public string ItemType { get; set; }
    public string Url { get; set; }
    
    public string TerminalType { get; set; }

    public string ApiUrl { get; set; }

    public string SizeString
    {
        get => Size.ToString();
        set => Size = Enum.Parse<ItemSize>(value);
    }

    public string AppName { get; set; }
    public string Target { get; set; }

    public string SshServer { get; set; }
    public string SshUserName { get; set; }
    public string SshPassword { get; set; }
    public Guid? DockerUid { get; set; }
    public string DockerContainer { get; set; }
    public string DockerCommand { get; set; }

    public Dictionary<string, object> Properties { get; set; } = new();

    public object GetValue(FenrusAppProperty prop)
    {
        if (Properties.ContainsKey(prop.Id) == false)
        {
            if (prop.DefaultValue != null)
            {
                if (prop.DefaultValue is JsonElement je)
                {
                    // in case its a json element value, we need to convert it
                    prop.DefaultValue = je.ValueKind == JsonValueKind.String ? je.GetString() :
                        je.ValueKind == JsonValueKind.False ? false :
                        je.ValueKind == JsonValueKind.True ? true :
                        je.ValueKind == JsonValueKind.Number ? je.GetInt32() :
                        je.ValueKind == JsonValueKind.Null ? null :
                        null;
                }
                Properties.Add(prop.Id, prop.DefaultValue);
            }

            return prop.DefaultValue;
        }

        return Properties[prop.Id];
    }

    public void SetValue(FenrusAppProperty prop, object value)
    {
        if (Properties.ContainsKey(prop.Id))
            Properties[prop.Id] = value;
        else
            Properties.Add(prop.Id, value);
    }
}