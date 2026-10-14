// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelSelectDialog), "Open")]
internal class PatchDuelSelectDialogOpen
{
	[HarmonyPostfix]
	private static void Postfix(string message)
	{
		try
		{
			PatchDuelSelectDialogUpdateMessage.ResetDedup();
			if (!string.IsNullOrWhiteSpace(message))
			{
				string text = TextUtil.StripTags(message);
				if (!string.IsNullOrWhiteSpace(text))
				{
					Log.Write("[DuelSelectDialog] " + text);
					Speech.SayImmediate(text);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchDuelSelectDialogOpen] " + ex.Message);
		}
	}
}
