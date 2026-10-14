// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.MDMarkup;

namespace MillenniumEye;

[HarmonyPatch(typeof(MDMarkupBoardContainerWidget), "OnStart")]
internal class PatchMDMarkupBoardOnStart
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		MillenniumEyeCore.Instance.CancelInvoke("ReadMDMarkupContent");
		MillenniumEyeCore.Instance.Invoke("ReadMDMarkupContent", 0.3f);
	}
}
