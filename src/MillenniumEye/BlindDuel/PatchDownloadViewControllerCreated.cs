// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.Menu;

namespace MillenniumEye;

[HarmonyPatch(typeof(DownloadViewController), "OnCreatedView")]
internal class PatchDownloadViewControllerCreated
{
	[HarmonyPostfix]
	private static void Postfix(DownloadViewController __instance)
	{
		ScreenDetector.TrackDownload(__instance);
		string text = ScreenDetector.FindScreenTitle(__instance);
		if (!string.IsNullOrEmpty(text))
		{
			Speech.AnnounceScreen(text);
		}
		Log.Write("[Download] DownloadViewController created, tracking progress");
	}
}
