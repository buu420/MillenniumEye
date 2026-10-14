// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.Deck;

namespace MillenniumEye;

[HarmonyPatch(typeof(DeckEditViewController2), "AddToMainOrExtraDeck", new Type[]
{
	typeof(CardBaseData),
	typeof(bool)
})]
internal class PatchDeckAdd
{
	[HarmonyPostfix]
	private static void Postfix(DeckCard __result, CardBaseData baseData)
	{
		try
		{
			if (__result == null)
			{
				return;
			}
			int cardID_k__BackingField = baseData._CardID_k__BackingField;
			string text = Content.s_instance?.GetName(cardID_k__BackingField);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			DeckEditViewController2 deckEditViewController = ScreenDetector.GetFocusVC()?.TryCast<DeckEditViewController2>();
			int num = 0;
			if (deckEditViewController != null)
			{
				List<CardBaseData> mainDeckCards = deckEditViewController.m_MainDeckCards;
				if (mainDeckCards != null)
				{
					for (int i = 0; i < mainDeckCards.Count; i++)
					{
						if (mainDeckCards[i]._CardID_k__BackingField == cardID_k__BackingField)
						{
							num++;
						}
					}
				}
				List<CardBaseData> extraDeckCards = deckEditViewController.m_ExtraDeckCards;
				if (extraDeckCards != null)
				{
					for (int j = 0; j < extraDeckCards.Count; j++)
					{
						if (extraDeckCards[j]._CardID_k__BackingField == cardID_k__BackingField)
						{
							num++;
						}
					}
				}
			}
			string text2 = ((num > 0) ? $"Added {text}, now {num} in deck" : ("Added " + text));
			Speech.SayImmediate(text2);
		}
		catch (Exception ex)
		{
			Log.Write("[DeckAdd] " + ex.Message);
		}
	}
}
