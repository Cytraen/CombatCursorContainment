using Dalamud.Configuration;
using Dalamud.Plugin;

namespace CombatCursorContainment;

[Serializable]
public class Configuration : IPluginConfiguration
{
	[NonSerialized] private DalamudPluginInterface? _pluginInterface;

	public int Version { get; set; } = 0;

	public bool EnableLocking { get; set; } = true;

	public bool DoNotLockIfDead { get; set; } = false;

	public bool DoNotLockIfOutsideDuty { get; set; } = false;

	public bool DoNotLockDuringCutscene { get; set; } = false;

	public bool DoNotLockIfWeaponSheathed { get; set; } = false;

	public bool DoNotLockIfMounted { get; set; } = false;

	public bool DoNotLockIfGathererCrafter { get; set; } = false;

	public void Initialize(DalamudPluginInterface pluginInterface)
	{
		_pluginInterface = pluginInterface;
	}

	public void Save()
	{
		_pluginInterface!.SavePluginConfig(this);
	}
}
