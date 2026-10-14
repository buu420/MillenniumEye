// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelInfoDialogBase), "Open")]
internal class PatchDuelInfoDialogOpen
{
	[HarmonyPostfix]
	private static void Postfix(string message)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}
			string text = TextUtil.StripTags(message);
			if (!string.IsNullOrWhiteSpace(text))
			{
				Log.Write("[DuelInfoDialog] " + text);
				if (NavigationState.IsInDuel)
				{
					DuelState.MessageJustAnnounced = true;
					Speech.SayImmediate(text);
					FieldFocusHandler.RequeueLastFocus();
				}
				else
				{
					Speech.SayQueued(text);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchDuelInfoDialog] " + ex.Message);
		}
	}
}
