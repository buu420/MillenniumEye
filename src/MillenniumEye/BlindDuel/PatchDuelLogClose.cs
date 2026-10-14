// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelLogController), "Close")]
internal class PatchDuelLogClose
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		try
		{
			if (DuelState.IsDuelLogOpen)
			{
				DuelState.IsDuelLogOpen = false;
				Log.Write("[DuelLog] Closed");
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchDuelLogClose] " + ex.Message);
		}
	}
}
