using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace CombatCursorContainment.Windows;

public class ConfigWindow : Window, IDisposable
{
	private Plugin Plugin;
	private Configuration Configuration;

	public ConfigWindow(Plugin plugin) : base(
		"Combat Cursor Containment Settings",
		ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
	{
		Size = new Vector2(280, 220);
		SizeCondition = ImGuiCond.Always;

		Plugin = plugin;
		Configuration = plugin.Configuration;
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	public override void Draw()
	{
		var enableLocking = Configuration.EnableLocking;
		var disableWhenDead = Configuration.DoNotLockIfDead;
		var disableOutsideDuty = Configuration.DoNotLockIfOutsideDuty;
		var disableWeaponSheathed = Configuration.DoNotLockIfWeaponSheathed;
		var disableWhenMounted = Configuration.DoNotLockIfMounted;
		var disableWhenHandLand = Configuration.DoNotLockIfGathererCrafter;

		if (ImGui.Checkbox("Automatically lock mouse in combat", ref enableLocking))
		{
			Configuration.EnableLocking = enableLocking;
			Configuration.Save();
			Plugin.UpdateSetting();
		}
		ImGui.Indent();
		ImGui.Text("Except while:");
		ImGui.Indent();
		if (ImGui.Checkbox("Dead", ref disableWhenDead))
		{
			Configuration.DoNotLockIfDead = disableWhenDead;
			Configuration.Save();
			Plugin.UpdateSetting();
		}
		if (ImGui.Checkbox("Not in a duty", ref disableOutsideDuty))
		{
			Configuration.DoNotLockIfOutsideDuty = disableOutsideDuty;
			Configuration.Save();
			Plugin.UpdateSetting();
		}
		if (ImGui.Checkbox("Weapon is sheathed", ref disableWeaponSheathed))
		{
			Configuration.DoNotLockIfWeaponSheathed = disableWeaponSheathed;
			Configuration.Save();
			Plugin.UpdateSetting();
		}
		if (ImGui.Checkbox("On a mount", ref disableWhenMounted))
		{
			Configuration.DoNotLockIfMounted = disableWhenMounted;
			Configuration.Save();
			Plugin.UpdateSetting();
		}
		if (ImGui.Checkbox("On a DoH/DoL class", ref disableWhenHandLand))
		{
			Configuration.DoNotLockIfGathererCrafter = disableWhenHandLand;
			Configuration.Save();
			Plugin.UpdateSetting();
		}
	}
}
