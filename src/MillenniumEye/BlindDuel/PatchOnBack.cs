// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

[HarmonyPatch(typeof(ViewController), "OnBack")]
internal class PatchOnBack
{
	[HarmonyPostfix]
	public static void Postfix(ViewController __instance)
	{
		try
		{
			if (!(__instance.manager == null) && __instance.manager.GetFocusViewController()?.name == "Home")
			{
				NavigationState.CurrentMenu = Menu.None;
			}
		}
		catch (Exception ex)
		{
			Log.Write("[OnBack] " + ex.Message);
		}
	}
}
