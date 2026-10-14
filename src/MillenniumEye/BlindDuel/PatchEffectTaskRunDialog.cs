// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(EffectTaskRunDialog), "RunDialog")]
internal class PatchEffectTaskRunDialog
{
	[HarmonyPostfix]
	private static void Postfix(EffectTaskRunDialog __instance)
	{
		try
		{
			if (!NavigationState.IsInDuel)
			{
				return;
			}
			string text = __instance.text;
			if (!string.IsNullOrWhiteSpace(text))
			{
				string text2 = TextUtil.StripTags(text);
				if (!string.IsNullOrWhiteSpace(text2))
				{
					Log.Write("[EffectTaskRunDialog] " + text2);
					PatchCardSelectionListSetTitle.HandleTitle(text2);
					return;
				}
			}
			string activateCardSelectionText = EffectTaskRunDialog.activateCardSelectionText;
			if (!string.IsNullOrWhiteSpace(activateCardSelectionText))
			{
				string text3 = TextUtil.StripTags(activateCardSelectionText);
				if (!string.IsNullOrWhiteSpace(text3))
				{
					Log.Write("[EffectTaskRunDialog.activate] " + text3);
					PatchCardSelectionListSetTitle.HandleTitle(text3);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchEffectTaskRunDialog] " + ex.Message);
		}
	}
}
