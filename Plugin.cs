using CombatCursorContainment.Windows;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Config;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace CombatCursorContainment
{
	public sealed class Plugin : IDalamudPlugin
	{
		public string Name => "Combat Cursor Containment";
		private const string ConfigWindowCommandName = "/ccc";

		private readonly WindowSystem WindowSystem = new("CombatCursorContainment");
		private DalamudPluginInterface PluginInterface { get; }
		private CommandManager CommandManager { get; }
		private Condition Condition { get; }
		private GameConfig GameConfig { get; }
		private ConfigWindow ConfigWindow { get; }
		public Configuration Configuration { get; }

		public Plugin(
			[RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
			[RequiredVersion("1.0")] CommandManager commandManager,
			[RequiredVersion("1.0")] Condition condition,
			[RequiredVersion("1.0")] GameConfig gameConfig)
		{
			PluginInterface = pluginInterface;
			CommandManager = commandManager;
			Condition = condition;
			GameConfig = gameConfig;

			Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
			Configuration.Initialize(PluginInterface);

			ConfigWindow = new ConfigWindow(this);

			WindowSystem.AddWindow(ConfigWindow);

			CommandManager.AddHandler(ConfigWindowCommandName, new CommandInfo(OnConfigWindowCommand)
			{
				HelpMessage = "Opens the Combat Cursor Containment config window."
			});

			PluginInterface.UiBuilder.Draw += DrawUI;
			PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

			if (Configuration.EnablePlugin)
			{
				Condition.ConditionChange += ChangedCondition;
			}
		}

		private void ChangedCondition(ConditionFlag flag, bool value)
		{
			if (flag == ConditionFlag.InCombat)
			{
				GameConfig.System.Set(SystemConfigOption.MouseOpeLimit.ToString(), value ? 1u : 0u);
			}
		}

		public void UpdateSetting()
		{
			if (Configuration.EnablePlugin)
			{
				Condition.ConditionChange += ChangedCondition;
			}
			else
			{
				Condition.ConditionChange -= ChangedCondition;
			}
		}

		public void Dispose()
		{
			WindowSystem.RemoveAllWindows();
			ConfigWindow.Dispose();
			CommandManager.RemoveHandler(ConfigWindowCommandName);

			if (Configuration.EnablePlugin)
			{
				Condition.ConditionChange -= ChangedCondition;
			}
		}

		private void OnConfigWindowCommand(string command, string args)
		{
			ConfigWindow.IsOpen = true;
		}

		private void DrawUI()
		{
			WindowSystem.Draw();
		}

		public void DrawConfigUI()
		{
			ConfigWindow.IsOpen = true;
		}
	}
}
