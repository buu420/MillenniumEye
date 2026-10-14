// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.Menu;

namespace MillenniumEye;

[HarmonyPatch(typeof(ActionSheetViewController), "OnCreatedView")]
internal class PatchActionSheetCreated
{
	[HarmonyPostfix]
	private static void Postfix(ActionSheetViewController __instance)
	{
		DialogDetector.AnnounceDialog(__instance);
	}
}
