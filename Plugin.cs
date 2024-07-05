using CombatCursorContainment.Windows;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace CombatCursorContainment;

internal sealed class Plugin : IDalamudPlugin
{
	private const string ConfigWindowCommandName = "/ccc";
	private readonly ConfigWindow _configWindow;

	private readonly WindowSystem _windowSystem;

	public Plugin(IDalamudPluginInterface pluginInterface)
	{
		pluginInterface.Create<Services>();

		Services.Config = Services.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

		_configWindow = new ConfigWindow();
		_windowSystem = new WindowSystem("CombatCursorContainment");
		_windowSystem.AddWindow(_configWindow);

		Services.CommandManager.AddHandler(ConfigWindowCommandName, new CommandInfo(OnConfigWindowCommand)
		{
			HelpMessage = "Opens the Combat Cursor Containment config window." +
						  "\n/ccc <enable|on|1> → Enables locking cursor during combat." +
						  "\n/ccc <disable|off|0> → Disables locking cursor during combat."
		});

		Services.PluginInterface.UiBuilder.Draw += DrawUi;
		Services.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUi;

		if (Services.Config.EnableLocking) MouseLock.EnableMouseAutoLock();
	}

	public void Dispose()
	{
		MouseLock.DisableMouseAutoLock();

		_windowSystem.RemoveAllWindows();
		_configWindow.Dispose();
		Services.CommandManager.RemoveHandler(ConfigWindowCommandName);

		Services.PluginInterface.UiBuilder.Draw -= DrawUi;
		Services.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUi;
	}

	private void OnConfigWindowCommand(string command, string args)
	{
		switch (args)
		{
			case "":
				DrawConfigUi();
				break;

			case "enable" or "on" or "1":
				if (Services.Config.EnableLocking)
				{
					Services.ChatGui.Print("Combat Cursor Containment was already enabled.");
				}
				else
				{
					Services.ChatGui.Print("Combat Cursor Containment now enabled.");
					Services.Config.EnableLocking = true;
					Services.Config.Save();
					MouseLock.EnableMouseAutoLock();
				}

				break;

			case "disable" or "off" or "0":
				if (Services.Config.EnableLocking)
				{
					Services.ChatGui.Print("Combat Cursor Containment now disabled.");
					Services.Config.EnableLocking = false;
					Services.Config.Save();
					MouseLock.DisableMouseAutoLock();
				}
				else
				{
					Services.ChatGui.Print("Combat Cursor Containment was already disabled.");
				}

				break;

			default:
				Services.ChatGui.PrintError($"Unknown command: '/{command} {args}'");
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
