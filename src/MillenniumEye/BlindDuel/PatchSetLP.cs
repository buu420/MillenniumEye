// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelLP), "SetLP")]
internal class PatchSetLP
{
	[HarmonyPostfix]
	private static void Postfix(DuelLP __instance, int lp, bool initialSet)
	{
		try
		{
			if (initialSet)
			{
				string value = (__instance.m_IsNear ? "Your" : "Opponent's");
				Speech.SayQueued($"{value} starting life points: {lp}");
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchSetLP] " + ex.Message);
		}
	}
}
