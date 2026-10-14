// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.Deck;

namespace MillenniumEye;

[HarmonyPatch(typeof(CardActionMenu), "Close")]
internal class PatchCardActionMenuClose
{
	[HarmonyPrefix]
	private static void Prefix(CardActionMenu __instance)
	{
		try
		{
			int m_CurrentCardID_k__BackingField = __instance._m_CurrentCardID_k__BackingField;
			if (m_CurrentCardID_k__BackingField <= 0)
			{
				return;
			}
			int cardRarityID = CardCollectionInfo.GetCardRarityID(m_CurrentCardID_k__BackingField);
			int currentPremium = (int)__instance.m_CurrentPremium;
			int index = -1;
			int total = 0;
			try
			{
				DeckEditViewController2 deckEditViewController = ScreenDetector.GetFocusVC()?.TryCast<DeckEditViewController2>();
				if (deckEditViewController != null)
				{
					if (__instance.fromDeck)
					{
						int num = deckEditViewController.m_MainDeckCards?.Count ?? 0;
						int num2 = deckEditViewController.m_ExtraDeckCards?.Count ?? 0;
						total = num + num2;
						index = __instance.m_CurrentIdx;
					}
					else
					{
						List<CardBaseData> cardCollection = deckEditViewController.m_CardCollection;
						total = cardCollection?.Count ?? 0;
						index = DeckHandler.FindCollectionIndex(cardCollection, m_CurrentCardID_k__BackingField, currentPremium);
					}
				}
			}
			catch
			{
			}
			bool canDismantle = false;
			try
			{
				canDismantle = CardCollectionInfo.IsDismantleable(new CardBaseData
				{
					_CardID_k__BackingField = m_CurrentCardID_k__BackingField,
					_PremiumID_k__BackingField = currentPremium,
					_Rarity_k__BackingField = cardRarityID
				});
			}
			catch
			{
			}
			string text = DeckHandler.BuildCardSpeech(m_CurrentCardID_k__BackingField, cardRarityID, currentPremium, -1, index, total, canDismantle);
			if (!string.IsNullOrEmpty(text))
			{
				Speech.ResetButtonDedup();
				Speech.SayQueued(text);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[CardActionMenuClose] " + ex.Message);
		}
	}
}
