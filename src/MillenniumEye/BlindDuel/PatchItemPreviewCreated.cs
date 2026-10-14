// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Menu;

namespace MillenniumEye;

[HarmonyPatch(typeof(ItemPreviewViewController), "OnCreatedView")]
internal class PatchItemPreviewCreated
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		try
		{
			MillenniumEyeCore.Instance?.CancelInvoke("ReadItemPreviewDelayed");
			MillenniumEyeCore.Instance?.Invoke("ReadItemPreviewDelayed", 0.3f);
		}
		catch (Exception ex)
		{
			Log.Write("[ItemPreview] " + ex.Message);
		}
	}
}
