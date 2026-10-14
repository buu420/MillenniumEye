// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.Deck;

namespace MillenniumEye;

internal static class DeckPatchHelper
{
	public static void AnnounceRemoved(int cardId)
	{
		string value = Content.s_instance?.GetName(cardId);
		if (string.IsNullOrEmpty(value))
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
					if (mainDeckCards[i]._CardID_k__BackingField == cardId)
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
					if (extraDeckCards[j]._CardID_k__BackingField == cardId)
					{
						num++;
					}
				}
			}
		}
		string text = $"Removed {value}, {num} in deck";
		Speech.SayImmediate(text);
	}
}
