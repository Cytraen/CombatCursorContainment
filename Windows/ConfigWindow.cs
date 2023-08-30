using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace CombatCursorContainment.Windows;

internal class ConfigWindow : Window, IDisposable
{
	private readonly Configuration _configuration;
	private readonly MouseLock _lock;

	internal ConfigWindow(MouseLock mouseLock, Configuration configuration) : base(
		"Combat Cursor Containment Settings",
		ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
	{
		Size = new Vector2(0, 0);
		_lock = mouseLock;
		_configuration = configuration;
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	public override void Draw()
	{
		var changed = false;
		var enableLocking = _configuration.EnableLocking;
		var disableWhenDead = _configuration.DoNotLockIfDead;
		var disableOutsideDuty = _configuration.DoNotLockIfOutsideDuty;
		var disableDuringCutscene = _configuration.DoNotLockDuringCutscene;
		var disableWeaponSheathed = _configuration.DoNotLockIfWeaponSheathed;
		var disableWhenMounted = _configuration.DoNotLockIfMounted;
		var disableWhenHandLand = _configuration.DoNotLockIfGathererCrafter;

		ImGui.AlignTextToFramePadding();
		ImGui.Text("Mouse cursor is currently: ");

		if (enableLocking)
		{
			ImGui.SameLine(0, ImGui.GetStyle().FramePadding.X);
			ImGui.Text(_lock.GetMouseLimit() ? "locked" : "unlocked");
			ImGui.SameLine(0, ImGui.GetStyle().FramePadding.X);
			ImGui.Spacing();
		}
		else
		{
			ImGui.SameLine(0, 0);
			if (ImGui.Button(_lock.GetMouseLimit() ? "locked" : "unlocked"))
				_lock.SetMouseLimit(!_lock.GetMouseLimit());
		}
		ImGuiComponents.HelpMarker(
			"The game's cursor lock will not do anything if the game window is not focused (i.e. if you are 'alt-tabbed' out of the game.)\n" +
			"Conversely, if the game window is focused and your cursor is off-screen when the lock activates, it will be 'stolen' back into the game window.");

		if (changed |= ImGui.Checkbox("Automatically lock mouse in combat", ref enableLocking))
		{
			_configuration.EnableLocking = enableLocking;

			if (enableLocking)
				_lock.EnableLock();
			else
				_lock.DisableLock();
		}
		ImGuiComponents.HelpMarker(
			"This will automatically lock/unlock the cursor when the conditions for doing so are met.\n" +
			"You will be unable to toggle the cursor lock manually using the in-game setting while this is enabled.");

		if (!enableLocking) return;

		ImGui.Indent();
		ImGui.Text("Except while:");
		ImGui.Indent();

		if (changed |= ImGui.Checkbox("Dead", ref disableWhenDead))
			_configuration.DoNotLockIfDead = disableWhenDead;

		if (changed |= ImGui.Checkbox("Not in a duty", ref disableOutsideDuty))
			_configuration.DoNotLockIfOutsideDuty = disableOutsideDuty;
		ImGuiComponents.HelpMarker("In other words, checking this makes your cursor lock only during duties.");

		if (changed |= ImGui.Checkbox("In a cutscene", ref disableDuringCutscene))
			_configuration.DoNotLockDuringCutscene = disableDuringCutscene;
		ImGuiComponents.HelpMarker(
			"This is for solo duties that have cutscenes, as the game considers you to be 'in combat' during them.\n" +
			"This does not work for mid-fight cutscenes in group duties, like in some raids and trials.");

		if (changed |= ImGui.Checkbox("Weapon is sheathed", ref disableWeaponSheathed))
			_configuration.DoNotLockIfWeaponSheathed = disableWeaponSheathed;

		if (changed |= ImGui.Checkbox("On a mount", ref disableWhenMounted))
			_configuration.DoNotLockIfMounted = disableWhenMounted;

		if (changed |= ImGui.Checkbox("On a DoH/DoL class", ref disableWhenHandLand))
			_configuration.DoNotLockIfGathererCrafter = disableWhenHandLand;

		if (changed)
			_configuration.Save();
	}
}
