// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelClient), "Awake")]
internal class PatchDuelClientAwake
{
	[HarmonyPostfix]
	private static void Postfix(DuelClient __instance)
	{
		NavigationState.CurrentMenu = Menu.Duel;
		NavigationState.IsInDuel = true;
		DuelLogReader.Reset();
		DuelFieldNav.Reset();
		ActivationConfirmHandler.Reset();
		DuelEventDelivery.Reset();
		DuelHandReveals.Reset();
		try
		{
			EngineInitializer engineInitializer = __instance.engineInitializer;
			if (engineInitializer != null)
			{
				Log.Write($"[DuelClientAwake] myPlayerNum={engineInitializer.myPlayerNum}, rivalPlayerNum={engineInitializer.rivalPlayerNum}");
			}
		}
		catch
		{
		}
		try
		{
			FieldFocusHandler.Subscribe(__instance);
		}
		catch (Exception ex)
		{
			Log.Write("[DuelClientAwake] Focus subscribe failed: " + ex.Message);
		}
	}
}
