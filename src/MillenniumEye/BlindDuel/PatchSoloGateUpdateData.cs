// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.Solo;

namespace MillenniumEye;

[HarmonyPatch(typeof(SoloGateUtil), "UpdateData")]
internal class PatchSoloGateUpdateData
{
	[HarmonyPostfix]
	private static void Postfix(SoloGateUtil.GateManager gateManager)
	{
		SoloState.CaptureGateData(gateManager, "UpdateData");
	}
}
