// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomSystem.UI.ElementWidget;

namespace MillenniumEye;

[HarmonyPatch(typeof(InputFieldWidget), "OnEndEdit")]
internal class PatchInputFieldEndEdit
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		if (PatchInputFieldValueChanged.IsEditing)
		{
			PatchInputFieldValueChanged.IsEditing = false;
			PatchInputFieldValueChanged.PreviousText = "";
			Speech.SayImmediate("confirmed");
		}
	}
}
