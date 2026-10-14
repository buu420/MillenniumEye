// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.Scenario;

namespace MillenniumEye;

[HarmonyPatch(typeof(ScenarioBehaviour_MovieCreate), "ProgressFinish")]
internal class PatchMovieFinish
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		PatchScenarioText.Reset();
		PatchScenarioTitle.Reset();
		PatchMovieMarkerCallback.Reset();
	}
}
