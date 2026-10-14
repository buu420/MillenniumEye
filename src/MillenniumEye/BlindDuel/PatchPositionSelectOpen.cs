// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(CardCommandEx), "SetDefaultPosition")]
internal class PatchPositionSelectOpen
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		try
		{
			if (NavigationState.IsInDuel)
			{
				Log.Write("[PositionSelect] Battle position selection opened");
				NavigationState.DialogJustAnnounced = true;
				DuelState.HasPendingSelection = true;
				DuelState.DeferredSelectionButton = null;
				DuelState.DeferredFieldFocus = null;
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchPositionSelectOpen] " + ex.Message);
		}
	}
}
