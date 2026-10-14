// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System.Collections.Generic;
using HarmonyLib;
using Il2CppYgomSystem;
using UnityEngine;

namespace MillenniumEye;

[HarmonyPatch(typeof(GamePad_PC), "GetKeyDown")]
internal class PatchGamePadGetKeyDown
{
	private static Dictionary<int, KeyCode> _map;

	private static void Postfix(int Type, ref bool __result)
	{
		if (__result && InputMap.IsRightStickDuelSuppress(Type))
		{
			__result = false;
		}
		else if (!__result && Application.isFocused)
		{
			Dictionary<int, KeyCode> map = GetMap();
			if (map != null && map.TryGetValue(Type, out var value) && !IsCtrlDuelSuppress(Type) && !InputMap.IsDuelOnlySuppress(Type) && !InputMap.IsInputEditingSuppress(Type))
			{
				__result = Input.GetKeyDown(value);
			}
		}
	}

	private static Dictionary<int, KeyCode> GetMap()
	{
		return _map ?? (_map = InputMap.Build());
	}

	private static bool IsCtrlDuelSuppress(int type)
	{
		return InputMap.IsCtrlDuelSuppress(type);
	}
}
