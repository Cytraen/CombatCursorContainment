using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace CombatCursorContainment.Windows;

internal class ConfigWindow : Window, IDisposable
{
	internal ConfigWindow() : base(
		"Combat Cursor Containment Settings",
		ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
	{
		Size = new Vector2(0, 0);
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	public override void Draw()
	{
		var changed = false;
		var enableLocking = Services.Config.EnableLocking;
		var disableWhenDead = Services.Config.DoNotLockIfDead;
		var disableOutsideDuty = Services.Config.DoNotLockIfOutsideDuty;
		var disableDuringCutscene = Services.Config.DoNotLockDuringCutscene;
		var disableWeaponSheathed = Services.Config.DoNotLockIfWeaponSheathed;
		var disableWhenMounted = Services.Config.DoNotLockIfMounted;
		var disableWhenHandLand = Services.Config.DoNotLockIfGathererCrafter;

		ImGui.AlignTextToFramePadding();
		ImGui.Text("Mouse cursor is currently: ");

		if (enableLocking)
		{
			ImGui.SameLine(0, ImGui.GetStyle().FramePadding.X);
			ImGui.Text(MouseLock.GetMouseLimit() ? "locked" : "unlocked");
			ImGui.SameLine(0, ImGui.GetStyle().FramePadding.X);
			ImGui.Spacing();
		}
		else
		{
			ImGui.SameLine(0, 0);
			if (ImGui.Button(MouseLock.GetMouseLimit() ? "locked" : "unlocked"))
				MouseLock.SetMouseLimit(!MouseLock.GetMouseLimit());
		}
		ImGuiComponents.HelpMarker(
			"The game's cursor lock will not do anything if the game window is not focused (i.e. if you are 'alt-tabbed' out of the game.)\n" +
			"Conversely, if the game window is focused and your cursor is off-screen when the lock activates, it will be 'stolen' back into the game window.");

		if (changed |= ImGui.Checkbox("Automatically lock mouse in combat", ref enableLocking))
		{
			Services.Config.EnableLocking = enableLocking;

			if (enableLocking)
				MouseLock.EnableMouseAutoLock();
			else
				MouseLock.DisableMouseAutoLock();
		}
		ImGuiComponents.HelpMarker(
			"This will automatically lock/unlock the cursor when the conditions for doing so are met.\n" +
			"You will be unable to toggle the cursor lock manually using the in-game setting while this is enabled.");

		if (!enableLocking) return;

		ImGui.Indent();
		ImGui.Text("Except while:");
		ImGui.Indent();

		if (changed |= ImGui.Checkbox("Dead", ref disableWhenDead))
			Services.Config.DoNotLockIfDead = disableWhenDead;

		if (changed |= ImGui.Checkbox("Not in a duty", ref disableOutsideDuty))
			Services.Config.DoNotLockIfOutsideDuty = disableOutsideDuty;
		ImGuiComponents.HelpMarker("In other words, checking this makes your cursor lock only during duties.");

		if (changed |= ImGui.Checkbox("In a cutscene", ref disableDuringCutscene))
			Services.Config.DoNotLockDuringCutscene = disableDuringCutscene;
		ImGuiComponents.HelpMarker(
			"This is for solo duties that have cutscenes, as the game considers you to be 'in combat' during them.\n" +
			"This does not work for mid-fight cutscenes in group duties, like in some raids and trials.");

		if (changed |= ImGui.Checkbox("Weapon is sheathed", ref disableWeaponSheathed))
			Services.Config.DoNotLockIfWeaponSheathed = disableWeaponSheathed;

		if (changed |= ImGui.Checkbox("On a mount", ref disableWhenMounted))
			Services.Config.DoNotLockIfMounted = disableWhenMounted;

		if (changed |= ImGui.Checkbox("On a DoH/DoL class", ref disableWhenHandLand))
			Services.Config.DoNotLockIfGathererCrafter = disableWhenHandLand;

		if (changed)
			Services.Config.Save();
	}
}
