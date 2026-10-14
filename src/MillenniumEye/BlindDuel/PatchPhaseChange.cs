// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(EffectTaskPhaseChange), "PlayPhaseChangeEffect")]
internal class PatchPhaseChange
{
	private static Engine.Phase _lastPhase = Engine.Phase.Null;

	[HarmonyPostfix]
	private static void Postfix(EffectTaskPhaseChange __instance)
	{
		try
		{
			DuelState.HasPhaseStarted = true;
			Engine.Phase phase = __instance.phase;
			if (phase != _lastPhase)
			{
				_lastPhase = phase;
				if (1 == 0)
				{
				}
				string text = phase switch
				{
					Engine.Phase.Draw => "Draw Phase", 
					Engine.Phase.Standby => "Standby Phase", 
					Engine.Phase.Main1 => "Main Phase 1", 
					Engine.Phase.Battle => "Battle Phase", 
					Engine.Phase.Main2 => "Main Phase 2", 
					Engine.Phase.End => "End Phase", 
					_ => "", 
				};
				if (1 == 0)
				{
				}
				string text2 = text;
				if (!string.IsNullOrEmpty(text2))
				{
					Log.Write("[PhaseChange] " + text2);
					Speech.SayQueued(text2);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchPhaseChange] " + ex.Message);
		}
	}
}
