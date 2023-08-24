using Dalamud.Interface.Components;
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
		ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
	{
		Size = new Vector2(0, 0);
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
		var disableDuringCutscene = Configuration.DoNotLockDuringCutscene;
		var disableWeaponSheathed = Configuration.DoNotLockIfWeaponSheathed;
		var disableWhenMounted = Configuration.DoNotLockIfMounted;
		var disableWhenHandLand = Configuration.DoNotLockIfGathererCrafter;
		ImGui.Text($"Mouse cursor is currently: {(Plugin.MouseLimit ? "locked" : "unlocked")}");

		if (ImGui.Checkbox("Automatically lock mouse in combat", ref enableLocking))
		{
			Configuration.EnableLocking = enableLocking;
			if (enableLocking)
			{
				Plugin.EnablePlugin();
			}
			else
			{
				Plugin.DisablePlugin();
			}
			Configuration.Save();
		}
		ImGuiComponents.HelpMarker($"This will automatically lock/unlock the cursor when the conditions for doing so are met.\n" +
			$"You will be unable to toggle the cursor lock manually using the in-game setting while this is enabled.");
		if (enableLocking)
		{
			ImGui.Indent();
			ImGui.Text("Except while:");
			ImGui.Indent();
			if (ImGui.Checkbox("Dead", ref disableWhenDead))
			{
				Configuration.DoNotLockIfDead = disableWhenDead;
				Configuration.Save();
			}
			if (ImGui.Checkbox("Not in a duty", ref disableOutsideDuty))
			{
				Configuration.DoNotLockIfOutsideDuty = disableOutsideDuty;
				Configuration.Save();
			}
			ImGuiComponents.HelpMarker($"In other words, checking this makes your cursor only lock during duties.");
			if (ImGui.Checkbox("In a cutscene", ref disableDuringCutscene))
			{
				Configuration.DoNotLockDuringCutscene = disableDuringCutscene;
				Configuration.Save();
			}
			ImGuiComponents.HelpMarker($"There are some solo duties that have cutscenes between fights,\n" +
				$"and will consider you in combat during those cutscenes.\n" +
				$"This does not work for mid-fight cutscenes like in some raids and trials.");
			if (ImGui.Checkbox("Weapon is sheathed", ref disableWeaponSheathed))
			{
				Configuration.DoNotLockIfWeaponSheathed = disableWeaponSheathed;
				Configuration.Save();
			}
			if (ImGui.Checkbox("On a mount", ref disableWhenMounted))
			{
				Configuration.DoNotLockIfMounted = disableWhenMounted;
				Configuration.Save();
			}
			if (ImGui.Checkbox("On a DoH/DoL class", ref disableWhenHandLand))
			{
				Configuration.DoNotLockIfGathererCrafter = disableWhenHandLand;
				Configuration.Save();
			}
		}
	}
}
