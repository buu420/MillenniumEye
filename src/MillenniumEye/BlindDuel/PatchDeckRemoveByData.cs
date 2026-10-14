// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame;
using Il2CppYgomGame.Deck;

namespace MillenniumEye;

[HarmonyPatch(typeof(DeckEditViewController2), "RemoveFromDeck", new Type[]
{
	typeof(CardBaseData),
	typeof(bool)
})]
internal class PatchDeckRemoveByData
{
	[HarmonyPostfix]
	private static void Postfix(DeckCard __result, CardBaseData baseData)
	{
		try
		{
			if (!(__result == null))
			{
				int cardID_k__BackingField = baseData._CardID_k__BackingField;
				DeckPatchHelper.AnnounceRemoved(cardID_k__BackingField);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[DeckRemove] " + ex.Message);
		}
	}
}
