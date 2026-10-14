// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.Duel;
using UnityEngine;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelLogController), "OnUpdateShowChain")]
internal class PatchDuelLogShowChain
{
	[HarmonyPostfix]
	private static void Postfix(GameObject eom, int dataindex)
	{
		DuelLogReader.CaptureItem(eom, "Chain", dataindex);
	}
}
