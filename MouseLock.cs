using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Config;
using Dalamud.Plugin.Services;

namespace CombatCursorContainment;

internal static class MouseLock
{
	internal static bool GetMouseLimit()
	{
		return Services.GameConfig.System.GetBool(SystemConfigOption.MouseOpeLimit.ToString());
	}

	internal static void SetMouseLimit(bool value)
	{
		if (GetMouseLimit() == value) return;
		Services.PluginLog.Debug($"Toggled mouse lock {(value ? "on" : "off")}");
		Services.GameConfig.System.Set(SystemConfigOption.MouseOpeLimit.ToString(), value);
	}

	internal static void EnableMouseAutoLock()
	{
		Services.Condition.ConditionChange += OnConditionChange;
		OnConditionChange(ConditionFlag.InCombat, Services.Condition[ConditionFlag.InCombat]);
	}

	internal static void DisableMouseAutoLock()
	{
		Services.Condition.ConditionChange -= OnConditionChange;
		Services.Framework.Update -= CombatFrameworkTick;
		SetMouseLimit(false);
	}

	private static void OnConditionChange(ConditionFlag flag, bool value)
	{
		if (flag != ConditionFlag.InCombat) return;
		Services.Framework.Update -= CombatFrameworkTick;
		if (value)
		{
			Services.Framework.Update += CombatFrameworkTick;
		}
		else
		{
			Services.Framework.RunOnTick(() => { SetMouseLimit(false); });
		}
	}

	private static void CombatFrameworkTick(IFramework _)
	{
		SetMouseLimit(ShouldLockMouse());
	}

	private static bool ShouldLockMouse()
	{
		if (Services.Config.DoNotLockDuringCutscene && IsInCutscene()) return false;
		if (Services.Config.DoNotLockIfDead && IsDead()) return false;
		if (Services.Config.DoNotLockIfOutsideDuty && !IsInDuty()) return false;
		if (Services.Config.DoNotLockIfWeaponSheathed && !IsWeaponOut()) return false;
		if (Services.Config.DoNotLockIfMounted && IsMounted()) return false;
		if (Services.Config.DoNotLockIfGathererCrafter && IsCraftingJob()) return false;
		return true;
	}

	private static bool IsInCutscene()
	{
		return Services.Condition[ConditionFlag.OccupiedInCutSceneEvent]
			   || Services.Condition[ConditionFlag.WatchingCutscene];
	}

	private static bool IsDead()
	{
		return Services.ClientState.LocalPlayer?.IsDead == true;
	}

	private static bool IsInDuty()
	{
		return Services.DutyState.IsDutyStarted;
	}

	private static bool IsWeaponOut()
	{
		return Services.ClientState.LocalPlayer?.StatusFlags.HasFlag(StatusFlags.WeaponOut) == true;
	}

	private static bool IsMounted()
	{
		return Services.Condition[ConditionFlag.Mounted]
			   || Services.Condition[ConditionFlag.Mounted2]
			   || Services.Condition[ConditionFlag.Mounting]
			   || Services.Condition[ConditionFlag.Mounting71];
	}

	private static bool IsCraftingJob()
	{
		return Services.ClientState.LocalPlayer?.ClassJob.Value.Role == 0;
	}
}
