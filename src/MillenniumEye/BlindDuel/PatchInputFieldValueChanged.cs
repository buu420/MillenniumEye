// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomSystem.UI.ElementWidget;

namespace MillenniumEye;

[HarmonyPatch(typeof(InputFieldWidget), "OnValueChanged")]
internal class PatchInputFieldValueChanged
{
	internal static string PreviousText = "";

	internal static bool IsEditing;

	[HarmonyPostfix]
	private static void Postfix(InputFieldWidget __instance, string input)
	{
		try
		{
			if (!__instance.m_inputFieldActivated || !IsEditing)
			{
				return;
			}
			string previousText = PreviousText;
			PreviousText = input ?? "";
			if (string.IsNullOrEmpty(input))
			{
				Speech.SayTyping("no text");
			}
			else if (string.IsNullOrEmpty(previousText))
			{
				Speech.SayTyping(input[input.Length - 1].ToString());
			}
			else if (input.Length == previousText.Length + 1)
			{
				for (int i = 0; i < previousText.Length; i++)
				{
					if (input[i] != previousText[i])
					{
						Speech.SayTyping(input[i].ToString());
						return;
					}
				}
				Speech.SayTyping(input[input.Length - 1].ToString());
			}
			else if (input.Length == previousText.Length - 1)
			{
				for (int j = 0; j < input.Length; j++)
				{
					if (input[j] != previousText[j])
					{
						Speech.SayTyping(previousText[j].ToString());
						return;
					}
				}
				Speech.SayTyping(previousText[previousText.Length - 1].ToString());
			}
			else if (input.Length > previousText.Length)
			{
				Speech.SayTyping(input);
			}
			else if (input.Length < previousText.Length)
			{
				Speech.SayTyping("no text");
			}
		}
		catch (Exception ex)
		{
			Log.Write("[InputField] OnValueChanged: " + ex.Message);
		}
	}
}
