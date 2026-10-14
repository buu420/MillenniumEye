// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomSystem.UI.ElementWidget;
using UnityEngine.UI;

namespace MillenniumEye;

[HarmonyPatch(typeof(InputFieldWidget), "ActivateInputField")]
internal class PatchInputFieldActivate
{
	[HarmonyPrefix]
	private static void Prefix(InputFieldWidget __instance, out bool __state)
	{
		__state = __instance.m_inputFieldActivated;
	}

	[HarmonyPostfix]
	private static void Postfix(InputFieldWidget __instance, bool __state)
	{
		try
		{
			if (__state || !__instance.m_inputFieldActivated)
			{
				return;
			}
			PatchInputFieldValueChanged.IsEditing = true;
			PatchInputFieldValueChanged.PreviousText = __instance.text ?? "";
			string text = null;
			try
			{
				Graphic graphic = __instance.inputField?.placeholder;
				if (graphic != null)
				{
					text = TextExtractor.ExtractFirst(graphic.gameObject);
				}
			}
			catch
			{
			}
			string text2 = ((!string.IsNullOrEmpty(text)) ? text : "Text input");
			string text3 = __instance.text;
			if (!string.IsNullOrEmpty(text3))
			{
				text2 = text2 + ": " + text3;
			}
			text2 += ", editing";
			Speech.SayImmediate(text2);
		}
		catch (Exception ex)
		{
			Log.Write("[InputField] ActivateInputField: " + ex.Message);
		}
	}
}
