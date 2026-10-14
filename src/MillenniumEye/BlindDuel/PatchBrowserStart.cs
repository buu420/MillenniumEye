// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.CardBrowser;
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

[HarmonyPatch(typeof(CardBrowserViewController), "Start")]
internal class PatchBrowserStart
{
	[HarmonyPostfix]
	private static void Postfix(CardBrowserViewController __instance)
	{
		SnapContentManager componentInChildren = __instance.GetComponentInChildren<SnapContentManager>();
		CardBrowserState.SnapContentManager = componentInChildren;
		CardBrowserState.ViewController = __instance;
	}
}
