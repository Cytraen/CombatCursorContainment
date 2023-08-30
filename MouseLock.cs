using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Config;
using Dalamud.Logging;

namespace CombatCursorContainment;

internal class MouseLock
{
	private readonly ClientState _clientState;
	private readonly Condition _condition;
	private readonly Configuration _config;
	private readonly Framework _framework;
	private readonly GameConfig _gameConfig;

	internal MouseLock(Framework framework, ClientState clientState, Condition condition, GameConfig gameConfig,
		Configuration config)
	{
		_framework = framework;
		_clientState = clientState;
		_condition = condition;
		_gameConfig = gameConfig;
		_config = config;
	}

	internal bool GetMouseLimit()
	{
		return _gameConfig.System.GetBool(SystemConfigOption.MouseOpeLimit.ToString());
	}

	internal void SetMouseLimit(bool value)
	{
		PluginLog.Debug($"Toggled mouse lock {(value ? "on" : "off")}");
		_gameConfig.System.Set(SystemConfigOption.MouseOpeLimit.ToString(), value ? 1u : 0u);
	}

	internal void EnableLock()
	{
		_condition.ConditionChange += OnConditionChange;
		OnConditionChange(ConditionFlag.InCombat, _condition[ConditionFlag.InCombat]);
	}

	internal void DisableLock()
	{
		_condition.ConditionChange -= OnConditionChange;
		_framework.Update -= MouseLockTick;
		if (GetMouseLimit()) SetMouseLimit(false);
	}

	private void OnConditionChange(ConditionFlag flag, bool value)
	{
		if (flag != ConditionFlag.InCombat) return;
		if (value)
		{
			_framework.Update += MouseLockTick;
		}
		else
		{
			_framework.Update -= MouseLockTick;
			_framework.Update += MouseUnlockTick;
		}
	}

	private void MouseLockTick(Framework framework)
	{
		var shouldLock = ShouldLockMouse();
		if (shouldLock == GetMouseLimit()) return;
		SetMouseLimit(shouldLock);
	}

	private void MouseUnlockTick(Framework framework)
	{
		_framework.Update -= MouseUnlockTick;
		if (GetMouseLimit()) SetMouseLimit(false);
	}

	private bool ShouldLockMouse()
	{
		if (_config.DoNotLockDuringCutscene && IsInCutscene()) return false;
		if (_config.DoNotLockIfDead && IsDead()) return false;
		if (_config.DoNotLockIfOutsideDuty && IsInDuty()) return false;
		if (_config.DoNotLockIfWeaponSheathed && !IsWeaponOut()) return false;
		if (_config.DoNotLockIfMounted && IsMounted()) return false;
		if (_config.DoNotLockIfGathererCrafter && IsCraftingJob()) return false;
		return true;
	}

	private bool IsInCutscene()
	{
		return _condition[ConditionFlag.OccupiedInCutSceneEvent]
			   || _condition[ConditionFlag.WatchingCutscene];
	}

	private bool IsDead()
	{
		return _clientState.LocalPlayer?.IsDead == true;
	}

	private bool IsInDuty()
	{
		return _condition[ConditionFlag.BoundByDuty]
			   || _condition[ConditionFlag.BoundByDuty56]
			   || _condition[ConditionFlag.BoundByDuty95]
			   || _condition[ConditionFlag.BoundToDuty97];
	}

	private bool IsWeaponOut()
	{
		return _clientState.LocalPlayer?.StatusFlags.HasFlag(StatusFlags.WeaponOut) == true;
	}

	private bool IsMounted()
	{
		return _condition[ConditionFlag.Mounted]
			   || _condition[ConditionFlag.Mounted2]
			   || _condition[ConditionFlag.Mounting]
			   || _condition[ConditionFlag.Mounting71];
	}

	private bool IsCraftingJob()
	{
		return _clientState.LocalPlayer?.ClassJob.GameData?.Role == 0;
	}
}
