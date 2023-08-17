using CombatCursorContainment.Windows;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
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
		private Framework Framework { get; }
		private DalamudPluginInterface PluginInterface { get; }
		private CommandManager CommandManager { get; }
		private ClientState ClientState { get; }
		private Condition Condition { get; }
		private GameConfig GameConfig { get; }
		private ConfigWindow ConfigWindow { get; }
		public Configuration Configuration { get; }

		public Plugin(
			[RequiredVersion("1.0")] Framework framework,
			[RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
			[RequiredVersion("1.0")] CommandManager commandManager,
			[RequiredVersion("1.0")] ClientState clientState,
			[RequiredVersion("1.0")] Condition condition,
			[RequiredVersion("1.0")] GameConfig gameConfig)
		{
			Framework = framework;
			PluginInterface = pluginInterface;
			CommandManager = commandManager;
			ClientState = clientState;
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

			if (Configuration.EnableLocking)
			{
				EnablePlugin();
			}
		}

		private void EnablePlugin()
		{
			Framework.Update += MouseTick;
		}

		private void DisablePlugin()
		{
			Framework.Update -= MouseTick;
		}

		private void MouseTick(Framework framework)
		{
			SetMouseLimit(ShouldLockMouse());
		}

		private bool ShouldLockMouse()
		{
			if (!Condition[ConditionFlag.InCombat])
			{
				return false;
			}
			if (Configuration.DoNotLockIfOutsideDuty && !(Condition[ConditionFlag.BoundByDuty] || Condition[ConditionFlag.BoundByDuty56] || Condition[ConditionFlag.BoundByDuty95] || Condition[ConditionFlag.BoundToDuty97]))
			{
				return false;
			}
			if (Configuration.DoNotLockIfWeaponSheathed && ClientState.LocalPlayer?.StatusFlags.HasFlag(StatusFlags.WeaponOut) != true)
			{
				return false;
			}
			if (Configuration.DoNotLockIfMounted && (Condition[ConditionFlag.Mounted] || Condition[ConditionFlag.Mounted2] || Condition[ConditionFlag.Mounting] || Condition[ConditionFlag.Mounting71]))
			{
				return false;
			}
			if (Configuration.DoNotLockIfGathererCrafter && ClientState.LocalPlayer?.ClassJob?.GameData?.Role == 0)
			{
				return false;
			}
			return true;
		}

		private void SetMouseLimit(bool value)
		{
			GameConfig.System.Set(SystemConfigOption.MouseOpeLimit.ToString(), value ? 1u : 0u);
		}

		public void UpdateSetting()
		{
			if (Configuration.EnableLocking)
			{
				EnablePlugin();
			}
			else
			{
				DisablePlugin();
			}
		}

		public void Dispose()
		{
			WindowSystem.RemoveAllWindows();
			ConfigWindow.Dispose();
			CommandManager.RemoveHandler(ConfigWindowCommandName);

			if (Configuration.EnableLocking)
			{
				DisablePlugin();
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
