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
		ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
		ImGuiWindowFlags.NoScrollWithMouse)
	{
		Size = new Vector2(300, 75);
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
		var configValue = Configuration.EnablePlugin;
		if (ImGui.Checkbox("Enable CCC", ref configValue))
		{
			Configuration.EnablePlugin = configValue;
			Configuration.Save();
			Plugin.UpdateSetting();
		}
	}
}
