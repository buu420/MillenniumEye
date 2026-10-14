// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.SubMenu;
using Il2CppYgomSystem.UI;
using System.Reflection;

namespace MillenniumEye;

[HarmonyPatch]
internal class PatchHomeSubMenuBack
{
	private static MethodBase TargetMethod() => AccessTools.Method(typeof(HomeSubMenuViewController), "OnBack");

	[HarmonyPostfix]
	private static void Postfix(ViewController __instance, bool __result)
	{
		if (__result && __instance.TryCast<HomeSubMenuViewController>() != null)
		{
			HomeSubMenuState.CurrentInstance = null;
			HandlerRegistry.SetCurrentFromVC("Home");
			Log.Write("[HomeSubMenu] Closed - reverted to Home");
		}
	}
}
