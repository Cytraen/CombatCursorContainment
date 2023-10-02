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
		Services.PluginLog.Debug($"Toggled mouse lock {(value ? "on" : "off")}");
		Services.GameConfig.System.Set(SystemConfigOption.MouseOpeLimit.ToString(), value ? 1u : 0u);
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
		if (GetMouseLimit()) SetMouseLimit(false);
	}

	private static void OnConditionChange(ConditionFlag flag, bool value)
	{
		if (flag != ConditionFlag.InCombat) return;
		if (value)
		{
			Services.Framework.Update += CombatFrameworkTick;
		}
		else
		{
			Services.Framework.Update -= CombatFrameworkTick;
			Services.Framework.Update += PostCombatCleanupTick;
		}
	}

	private static void CombatFrameworkTick(IFramework framework)
	{
		var shouldLock = ShouldLockMouse();
		if (shouldLock == GetMouseLimit()) return;
		SetMouseLimit(shouldLock);
	}

	private static void PostCombatCleanupTick(IFramework framework)
	{
		if (GetMouseLimit()) SetMouseLimit(false);
		framework.Update -= PostCombatCleanupTick;
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
		return Services.ClientState.LocalPlayer?.ClassJob.GameData?.Role == 0;
	}
}
