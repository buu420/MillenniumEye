// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomSystem;

namespace MillenniumEye;

[HarmonyPatch(typeof(GamePad_PC), "GetKeyDown")]
internal class PatchBrowseDirectionGetKeyDown
{
	private static void Postfix(int Type, ref bool __result)
	{
		BrowseDirectionTracker.TrackKeyDown(Type, __result);
	}
}
