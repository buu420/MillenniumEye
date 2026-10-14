// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Solo;

namespace MillenniumEye;

[HarmonyPatch(typeof(SoloSelectChapterViewController.AccessDialogManager), "CloseAccessDialog")]
internal class PatchSoloAccessDialogClose
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		try
		{
			ScreenDetector.QueueFocusedItem();
		}
		catch (Exception ex)
		{
			Log.Write("[SoloDialogClose] " + ex.Message);
		}
	}
}
