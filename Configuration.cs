using Dalamud.Configuration;
using Dalamud.Plugin;

namespace CombatCursorContainment
{
	[Serializable]
	public class Configuration : IPluginConfiguration
	{
		public int Version { get; set; } = 0;

		public bool EnableLocking { get; set; } = true;

		public bool DoNotLockIfOutsideDuty { get; set; } = false;

		public bool DoNotLockIfWeaponSheathed { get; set; } = false;

		public bool DoNotLockIfMounted { get; set; } = false;

		public bool DoNotLockIfGathererCrafter { get; set; } = false;

		[NonSerialized]
		private DalamudPluginInterface? PluginInterface;

		public void Initialize(DalamudPluginInterface pluginInterface)
		{
			PluginInterface = pluginInterface;
		}

		public void Save()
		{
			PluginInterface!.SavePluginConfig(this);
		}
	}
}
