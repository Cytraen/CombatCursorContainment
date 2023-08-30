using CombatCursorContainment.Windows;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Config;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace CombatCursorContainment;

internal sealed class Plugin : IDalamudPlugin
{
	private const string ConfigWindowCommandName = "/ccc";
	private readonly ChatGui _chatGui;
	private readonly CommandManager _commandManager;
	private readonly Configuration _config;
	private readonly ConfigWindow _configWindow;

	private readonly MouseLock _lock;
	private readonly DalamudPluginInterface _pluginInterface;
	private readonly WindowSystem _windowSystem;

	public Plugin(
		[RequiredVersion("1.0")] Framework framework,
		[RequiredVersion("1.0")] ClientState clientState,
		[RequiredVersion("1.0")] Condition condition,
		[RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
		[RequiredVersion("1.0")] CommandManager commandManager,
		[RequiredVersion("1.0")] GameConfig gameConfig,
		[RequiredVersion("1.0")] ChatGui chatGui)
	{
		_pluginInterface = pluginInterface;
		_commandManager = commandManager;
		_chatGui = chatGui;

		_config = _pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
		_config.Initialize(_pluginInterface);

		_lock = new MouseLock(framework, clientState, condition, gameConfig, _config);

		_configWindow = new ConfigWindow(_lock, _config);

		_windowSystem = new WindowSystem("CombatCursorContainment");
		_windowSystem.AddWindow(_configWindow);

		_commandManager.AddHandler(ConfigWindowCommandName, new CommandInfo(OnConfigWindowCommand)
		{
			HelpMessage = "Opens the Combat Cursor Containment config window." +
						  "\n/ccc <enable|on|1> → Enables locking cursor during combat." +
						  "\n/ccc <disable|off|0> → Disables locking cursor during combat."
		});

		_pluginInterface.UiBuilder.Draw += DrawUi;
		_pluginInterface.UiBuilder.OpenConfigUi += DrawConfigUi;

		if (_config.EnableLocking) _lock.EnableLock();
	}

	public string Name => "Combat Cursor Containment";

	public void Dispose()
	{
		_lock.DisableLock();

		_windowSystem.RemoveAllWindows();
		_configWindow.Dispose();
		_commandManager.RemoveHandler(ConfigWindowCommandName);

		_pluginInterface.UiBuilder.Draw -= DrawUi;
		_pluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUi;
	}

	private void OnConfigWindowCommand(string command, string args)
	{
		switch (args)
		{
			case "":
				DrawConfigUi();
				break;

			case "enable" or "on" or "1":
				if (_config.EnableLocking)
				{
					_chatGui.Print("Combat Cursor Containment was already enabled.");
				}
				else
				{
					_chatGui.Print("Combat Cursor Containment now enabled.");
					_config.EnableLocking = true;
					_config.Save();
					_lock.EnableLock();
				}

				break;

			case "disable" or "off" or "0":
				if (_config.EnableLocking)
				{
					_chatGui.Print("Combat Cursor Containment now disabled.");
					_config.EnableLocking = false;
					_config.Save();
					_lock.DisableLock();
				}
				else
				{
					_chatGui.Print("Combat Cursor Containment was already disabled.");
				}

				break;

			default:
				_chatGui.PrintError($"Unknown command: '/{command} {args}'");
				break;
		}
	}

	private void DrawUi()
	{
		_windowSystem.Draw();
	}

	private void DrawConfigUi()
	{
		_configWindow.IsOpen = true;
	}
}
