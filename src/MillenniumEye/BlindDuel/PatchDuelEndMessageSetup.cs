// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelEndMessage), "Setup")]
internal class PatchDuelEndMessageSetup
{
	[HarmonyPostfix]
	private static void Postfix(string message, bool winMyself, bool winRival)
	{
		try
		{
			string text = (winMyself ? "Victory" : (winRival ? "Defeat" : "Draw"));
			string text2 = ((!string.IsNullOrWhiteSpace(message)) ? TextUtil.StripTags(message) : "");
			string text3 = ((!string.IsNullOrEmpty(text2)) ? (text + ". " + text2) : text);
			Log.Write("[DuelEnd] " + text3);
			Speech.SayImmediate(text3);
			NavigationState.IsInDuel = false;
			DuelState.Clear();
		}
		catch (Exception ex)
		{
			Log.Write("[PatchDuelEndMessageSetup] " + ex.Message);
		}
	}
}
