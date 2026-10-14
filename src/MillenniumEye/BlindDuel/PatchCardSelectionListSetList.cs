// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(CardSelectionList), "SetList")]
internal class PatchCardSelectionListSetList
{
	[HarmonyPrefix]
	private static void Prefix()
	{
		if (NavigationState.IsInDuel)
		{
			DuelState.HasPendingSelection = true;
			DuelState.DeferredSelectionButton = null;
			DuelState.DeferredFieldFocus = null;
			DuelHandler.ResetSelectionDedup();
			PatchCardSelectionListSetTitle.ResetDedup();
		}
	}

	[HarmonyPostfix]
	private static void Postfix(string title)
	{
		try
		{
			if (NavigationState.IsInDuel)
			{
				DuelState.HasPendingSelection = false;
			}
			if (!string.IsNullOrWhiteSpace(title))
			{
				string text = TextUtil.StripTags(title);
				if (!string.IsNullOrWhiteSpace(text))
				{
					PatchCardSelectionListSetTitle.HandleTitle(text);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchCardSelectionListSetList] " + ex.Message);
		}
	}
}
