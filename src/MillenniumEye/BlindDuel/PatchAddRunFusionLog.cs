// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelLogController), "AddRunFusionLog")]
internal class PatchAddRunFusionLog
{
	[HarmonyPrefix]
	private static void Prefix(DuelLogController __instance, ref int __state)
	{
		try
		{
			__state = __instance.m_DataList_ShowCardName?.Count ?? 0;
		}
		catch
		{
			__state = -1;
		}
		DuelLogHelper.CapturePrevActionCount(__instance);
	}

	[HarmonyPostfix]
	private static void Postfix(DuelLogController __instance, int __state)
	{
		DuelLogHelper.AnnounceSummon(__instance, __state, "Fusion Summon");
	}
}
