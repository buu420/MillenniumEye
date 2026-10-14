// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Tutorial;

namespace MillenniumEye;

[HarmonyPatch(typeof(TutorialNavigator), "PlayTopMsg", new Type[]
{
	typeof(string),
	typeof(float)
})]
internal class PatchTutorialTopMsg
{
	private static string _lastMessage = "";

	[HarmonyPostfix]
	private static void Postfix(string message)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}
			string text = TextUtil.StripTags(message);
			if (!string.IsNullOrWhiteSpace(text) && !(text == _lastMessage))
			{
				_lastMessage = text;
				Log.Write("[TutorialTop] " + text);
				if (NavigationState.IsInDuel)
				{
					DuelState.MessageJustAnnounced = true;
					Speech.SayImmediate(text);
				}
				else
				{
					Speech.SayQueued(text);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchTutorialTopMsg] " + ex.Message);
		}
	}
}
