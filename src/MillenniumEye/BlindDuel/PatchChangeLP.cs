// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelLP), "ChangeLP", MethodType.Normal)]
internal class PatchChangeLP
{
	[HarmonyPostfix]
	private static void Postfix(DuelLP __instance, int afterLP, int damage, Engine.DamageType type)
	{
		try
		{
			string value = (__instance.m_IsNear ? "Your" : "Opponent's");
			if (1 == 0)
			{
			}
			string text = type switch
			{
				Engine.DamageType.ByBattle => "battle damage", 
				Engine.DamageType.ByEffect => "effect damage", 
				Engine.DamageType.ByCost => "cost", 
				Engine.DamageType.ByPay => "payment", 
				Engine.DamageType.ByLost => "lost", 
				Engine.DamageType.Recover => "recovery", 
				_ => "", 
			};
			if (1 == 0)
			{
			}
			string text2 = text;
			if (type == Engine.DamageType.Recover)
			{
				Speech.SayImmediate($"{value} life points: {afterLP}, gained {damage} from {text2}");
			}
			else if (damage > 0 && text2.Length > 0)
			{
				Speech.SayImmediate($"{value} life points: {afterLP}, took {damage} {text2}");
			}
			else
			{
				Speech.SayImmediate($"{value} life points: {afterLP}");
			}
			if (afterLP < 1)
			{
				NavigationState.IsInDuel = false;
				DuelState.Clear();
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchChangeLP] " + ex.Message);
		}
	}
}
