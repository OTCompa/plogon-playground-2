using Dalamud.Game.Command;
using Dalamud.Game.Gui.NamePlate;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using SamplePlugin.Windows;
using System;
using System.Collections.Generic;
using System.IO;

namespace SamplePlugin;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IPlayerState PlayerState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] internal static INamePlateGui NamePlateGui { get; private set; } = null!;

    private const string CommandName = "/pmycommand";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("SamplePlugin");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    public record struct IconInfo(byte BattleIconId, int NamePlateIconId);
    private Dictionary<ulong, IconInfo> originalIcon = [];


    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // You might normally want to embed resources and load them from the manifest stream
        var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, goatImagePath);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        // Tell the UI system that we want our windows to be drawn through the window system
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        // This adds a button to the plugin installer entry of this plugin which allows
        // toggling the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        // Adds another button doing the same but for the main ui of the plugin
        ClientState.TerritoryChanged += TerritoryChanged;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

        NamePlateGui.OnDataUpdate += NamePlateGuiOnOnDataUpdate;
        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        // Example Output: 00:57:54.959 | INF | [SamplePlugin] ===A cool log message from Sample Plugin===
        Log.Information($"===A cool log message from {PluginInterface.Manifest.Name}===");
    }

    private void TerritoryChanged(ushort obj)
    {
        originalIcon.Clear();
    }

    private unsafe void NamePlateGuiOnOnDataUpdate(INamePlateUpdateContext context, IReadOnlyList<INamePlateUpdateHandler> handlers)
    {
        foreach (var handler in handlers)
        {
            if (handler.BattleChara != null)
            {
                var bChara = (BattleChara*)handler.BattleChara.Address;
                if (handler.BattleChara.IsCasting)
                {
                    if (bChara->Icon != 2)
                    {
                        originalIcon[bChara->GetGameObjectId()] = new IconInfo(bChara->Icon, handler.NameIconId);
                        bChara->Icon = 2;
                    }
                    if (originalIcon.TryGetValue(bChara->GetGameObjectId(), out var icon))
                    {
                        handler.NameIconId = icon.NamePlateIconId;
                    }
                } else
                {
                    if (originalIcon.TryGetValue(bChara->GetGameObjectId(), out var icon))
                    {
                        bChara->Icon = icon.BattleIconId;
                        originalIcon.Remove(bChara->GetGameObjectId());
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        // Unregister all actions to not leak anything during disposal of plugin
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        NamePlateGui.OnDataUpdate -= NamePlateGuiOnOnDataUpdate;
        UndoChanges();
        ClientState.TerritoryChanged -= TerritoryChanged;
        WindowSystem.RemoveAllWindows();


        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private unsafe void UndoChanges()
    {
        foreach (var kvp in originalIcon)
        {
            var obj = ObjectTable.SearchById(kvp.Key);
            if (obj == null) continue;
            var bChara = (BattleChara*)obj.Address;
            bChara->Icon = kvp.Value.BattleIconId;
        }
    }

    private unsafe void OnCommand(string command, string args)
    {

    }
    
    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();
}
