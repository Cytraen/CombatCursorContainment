using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Config;
using Dalamud.Game.DutyState;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace CombatCursorContainment;

internal sealed class Services
{
	[PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;

	[PluginService] public static ClientState ClientState { get; private set; } = null!;

	[PluginService] public static Condition Condition { get; private set; } = null!;

	[PluginService] public static Framework Framework { get; private set; } = null!;

	[PluginService] public static GameConfig GameConfig { get; private set; } = null!;

	[PluginService] public static CommandManager CommandManager { get; private set; } = null!;

	[PluginService] public static ChatGui ChatGui { get; private set; } = null!;

	[PluginService] public static DutyState DutyState { get; private set; } = null!;

	public static Configuration Config { get; internal set; } = null!;
}
