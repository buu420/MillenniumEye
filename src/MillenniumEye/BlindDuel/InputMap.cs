// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppYgomSystem;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

internal static class InputMap
{
	private static int _btnUp;

	private static int _btnDown;

	private static int _btnLeft;

	private static int _btnRight;

	private static int _btnDecision;

	private static int _btnCancel;

	private static int _btnRUp;

	private static int _btnRDown;

	private static int _btnSub1;

	private static int _btnSub2;

	private static int _btnL2;

	private static int _btnR2;

	private static int _btnOption1;

	private static int _btnOption2;

	private static int _btnL1;

	private static int _btnR1;

	private static int _btnL3;

	public static Dictionary<int, KeyCode> Build()
	{
		try
		{
			_btnUp = GamePad.BUTTON_UP;
			_btnDown = GamePad.BUTTON_DOWN;
			_btnLeft = GamePad.BUTTON_LEFT;
			_btnRight = GamePad.BUTTON_RIGHT;
			_btnDecision = GamePad.BUTTON_FUNC_DECISION;
			_btnCancel = GamePad.BUTTON_FUNC_CANCEL;
			_btnRUp = GamePad.BUTTON_RUP;
			_btnRDown = GamePad.BUTTON_RDOWN;
			_btnSub1 = SelectorManager.GetGamePadKeyConfig(SelectorManager.KeyType.Sub1);
			_btnSub2 = SelectorManager.GetGamePadKeyConfig(SelectorManager.KeyType.Sub2);
			_btnL2 = GamePad.BUTTON_L2;
			_btnR2 = GamePad.BUTTON_R2;
			_btnOption1 = SelectorManager.GetGamePadKeyConfig(SelectorManager.KeyType.Option1);
			_btnOption2 = SelectorManager.GetGamePadKeyConfig(SelectorManager.KeyType.Option2);
			_btnL1 = GamePad.BUTTON_L1;
			_btnR1 = GamePad.BUTTON_R1;
			_btnL3 = GamePad.BUTTON_L3;
			return new Dictionary<int, KeyCode>
			{
				{
					_btnUp,
					KeyCode.UpArrow
				},
				{
					_btnDown,
					KeyCode.DownArrow
				},
				{
					_btnLeft,
					KeyCode.LeftArrow
				},
				{
					_btnRight,
					KeyCode.RightArrow
				},
				{
					_btnDecision,
					KeyCode.Return
				},
				{
					_btnCancel,
					KeyCode.Backspace
				},
				{
					_btnSub2,
					KeyCode.E
				},
				{
					_btnSub1,
					KeyCode.F
				},
				{
					_btnL2,
					KeyCode.Z
				},
				{
					_btnR2,
					KeyCode.X
				},
				{
					_btnOption1,
					KeyCode.Tab
				},
				{
					_btnOption2,
					KeyCode.Escape
				},
				{
					_btnL1,
					KeyCode.I
				},
				{
					_btnR1,
					KeyCode.O
				},
				{
					_btnL3,
					KeyCode.D
				}
			};
		}
		catch (Exception ex)
		{
			Log.Write("[InputMap] Failed to read GamePad button constants: " + ex.Message);
			return null;
		}
	}

	public static bool IsCtrlDuelSuppress(int type)
	{
		if (!NavigationState.IsInDuel)
		{
			return false;
		}
		if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
		{
			return false;
		}
		return type == _btnUp || type == _btnDown;
	}

	public static bool IsRightStickDuelSuppress(int type)
	{
		if (!NavigationState.IsInDuel)
		{
			return false;
		}
		return type == _btnRUp || type == _btnRDown;
	}

	public static bool IsDuelOnlySuppress(int type)
	{
		if (type == _btnOption2)
		{
			return !NavigationState.IsInDuel;
		}
		if (type == _btnL3)
		{
			return NavigationState.IsInDuel;
		}
		return false;
	}

	public static bool IsInputEditingSuppress(int type)
	{
		if (type != _btnCancel)
		{
			return false;
		}
		return PatchInputFieldValueChanged.IsEditing;
	}
}
