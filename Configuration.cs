using Dalamud.Configuration;

namespace CombatCursorContainment;

[Serializable]
public class Configuration : IPluginConfiguration
{
	public int Version { get; set; } = 0;

	public bool EnableLocking { get; set; } = true;

	public bool DoNotLockIfDead { get; set; } = false;

	public bool DoNotLockIfOutsideDuty { get; set; } = false;

	public bool DoNotLockDuringCutscene { get; set; } = false;

	public bool DoNotLockIfWeaponSheathed { get; set; } = false;

	public bool DoNotLockIfMounted { get; set; } = false;

	public bool DoNotLockIfGathererCrafter { get; set; } = false;

	public void Save()
	{
		Services.PluginInterface.SavePluginConfig(this);
	}
}
