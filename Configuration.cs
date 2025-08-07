using System.Text.Json;

namespace CombatCursorContainment;

public sealed class Configuration
{
	public bool EnableLocking { get; set; } = true;

	public bool DoNotLockIfDead { get; set; } = false;

	public bool DoNotLockIfOutsideDuty { get; set; } = false;

	public bool DoNotLockDuringCutscene { get; set; } = false;

	public bool DoNotLockDuringTransition { get; set; } = false;

	public bool DoNotLockIfWeaponSheathed { get; set; } = false;

	public bool DoNotLockIfMounted { get; set; } = false;

	public bool DoNotLockIfGathererCrafter { get; set; } = false;

	public static Configuration Load()
	{
		if (!File.Exists(Services.PluginInterface.ConfigFile.FullName))
		{
			return new Configuration();
		}

		var bytes = File.ReadAllBytes(Services.PluginInterface.ConfigFile.FullName);
		return JsonSerializer.Deserialize<Configuration>(bytes) ?? new Configuration();
	}

	public static void Save(Configuration config)
	{
		config.Save();
	}

	public void Save()
	{
		var str = JsonSerializer.Serialize(this);
		File.WriteAllText(Services.PluginInterface.ConfigFile.FullName, str);
	}
}
