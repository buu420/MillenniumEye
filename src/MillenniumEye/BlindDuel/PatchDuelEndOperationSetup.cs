// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelEndOperation), "Setup")]
internal class PatchDuelEndOperationSetup
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		try
		{
			DuelState.IsShowingResult = true;
			Log.Write("[DuelEndOp] Duel end sequence started, suppressing buttons");
		}
		catch (Exception ex)
		{
			Log.Write("[PatchDuelEndOperationSetup] " + ex.Message);
		}
	}
}
