// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.CardPack.Open.Actor;

namespace MillenniumEye;

[HarmonyPatch(typeof(CardPackCardActor), "OnEndPlayable")]
internal class PatchCardFlipComplete
{
	[HarmonyPostfix]
	private static void Postfix(CardPackCardActor __instance)
	{
		try
		{
			int mrk_k__BackingField = __instance._mrk_k__BackingField;
			if (mrk_k__BackingField <= 0)
			{
				return;
			}
			PackFlipTracker.MarkFlipped(mrk_k__BackingField);
			string text = CardReader.ReadCardFromData(mrk_k__BackingField)?.Name;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string text2 = text;
			try
			{
				string cardRarityText = CardCollectionInfo.GetCardRarityText((int)CardCollectionInfo.GetCardRarity(mrk_k__BackingField));
				if (!string.IsNullOrEmpty(cardRarityText))
				{
					text2 = text2 + ", " + cardRarityText;
				}
			}
			catch
			{
			}
			Speech.SayImmediate(text2);
		}
		catch (Exception ex)
		{
			Log.Write("[PackFlip] " + ex.Message);
		}
	}
}
