// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelLogController), "Open")]
internal class PatchDuelLogOpen
{
	[HarmonyPostfix]
	private static void Postfix(DuelLogController __instance)
	{
		try
		{
			if (NavigationState.IsInDuel)
			{
				DuelState.IsDuelLogOpen = true;
				DuelState.LogController = __instance;
				DuelLogScrollView sv = (DuelState.LogScrollView = __instance.m_ScrollView);
				Log.Write("[DuelLog] Opened");
				Speech.SayImmediate("Duel Log");
				DuelLogReader.InitScroll(sv);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchDuelLogOpen] " + ex.Message);
		}
	}
}
