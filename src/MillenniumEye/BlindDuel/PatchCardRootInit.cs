// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(CardRoot), "Initialize", MethodType.Normal)]
internal class PatchCardRootInit
{
	[HarmonyPostfix]
	private static void Postfix(CardRoot __instance)
	{
		DuelState.Cards.Add(__instance);
	}
}
