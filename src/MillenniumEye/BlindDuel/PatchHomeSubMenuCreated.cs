// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.SubMenu;

namespace MillenniumEye;

[HarmonyPatch(typeof(HomeSubMenuViewController), "OnCreatedView")]
internal class PatchHomeSubMenuCreated
{
	[HarmonyPostfix]
	private static void Postfix(HomeSubMenuViewController __instance)
	{
		HomeSubMenuState.CurrentInstance = __instance;
		HomeSubMenuState.LastSpokenSection = null;
		HandlerRegistry.SetCurrentFromVC("HomeSubMenu");
		Log.Write("[HomeSubMenu] Opened - handler installed");
	}
}
