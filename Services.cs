using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace CombatCursorContainment;

internal sealed class Services
{
	[PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

	[PluginService] public static IClientState ClientState { get; private set; } = null!;

	[PluginService] public static ICondition Condition { get; private set; } = null!;

	[PluginService] public static IFramework Framework { get; private set; } = null!;

	[PluginService] public static IGameConfig GameConfig { get; private set; } = null!;

	[PluginService] public static ICommandManager CommandManager { get; private set; } = null!;

	[PluginService] public static IChatGui ChatGui { get; private set; } = null!;

	[PluginService] public static IDutyState DutyState { get; private set; } = null!;

	[PluginService] public static IPluginLog PluginLog { get; private set; } = null!;

	public static Configuration Config { get; internal set; } = null!;
}
