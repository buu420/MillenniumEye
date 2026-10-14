// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

[HarmonyPatch(typeof(SelectionButton), "OnDeselected", MethodType.Normal)]
internal class PatchOnDeselected
{
	[HarmonyPostfix]
	private static void Postfix(SelectionButton __instance)
	{
		Speech.ResetDedup();
	}
}
