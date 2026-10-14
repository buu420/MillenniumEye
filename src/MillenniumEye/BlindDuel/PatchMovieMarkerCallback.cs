// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppTMPro;
using Il2CppYgomGame.Scenario;

namespace MillenniumEye;

[HarmonyPatch(typeof(ScenarioBehaviour_MovieCreate.__c__DisplayClass10_1), "_ProgressInit_b__7")]
internal class PatchMovieMarkerCallback
{
	private static string _lastText = "";

	[HarmonyPostfix]
	private static void Postfix(ScenarioBehaviour_MovieCreate.__c__DisplayClass10_1 __instance)
	{
		try
		{
			TextMeshProUGUI subtitle = __instance.subtitle;
			if (subtitle == null)
			{
				Log.Write("[Scenario] Marker fired but subtitle TMP is null");
				return;
			}
			string text = subtitle.text;
			if (!string.IsNullOrWhiteSpace(text))
			{
				text = TextUtil.StripTags(text).Trim();
				if (!string.IsNullOrWhiteSpace(text) && !(text == _lastText))
				{
					_lastText = text;
					Log.Write("[Scenario] Movie subtitle: " + text);
					Speech.SayQueued(text);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[Scenario] Marker callback error: " + ex.Message);
		}
	}

	public static void Reset()
	{
		_lastText = "";
	}
}
