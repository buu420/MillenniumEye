// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Menu;
using Il2CppYgomGame.TextIDs;
using Il2CppYgomSystem.Utility;

namespace MillenniumEye;

[HarmonyPatch(typeof(TitleViewController), "OnClickStart")]
internal class PatchTitleOnClickStart
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		try
		{
			SystemProgress instance = SystemProgress.Instance;
			if (instance?.connectingProgress != null)
			{
				string text = TextExtractor.ExtractFirst(instance.connectingProgress);
				if (!string.IsNullOrWhiteSpace(text))
				{
					Speech.SayImmediate(text);
					return;
				}
			}
			string text2 = TextData.GetText(IDS_SYS.CHECK);
			if (!string.IsNullOrWhiteSpace(text2))
			{
				Speech.SayImmediate(text2);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[TitleOnClickStart] " + ex.Message);
		}
	}
}
