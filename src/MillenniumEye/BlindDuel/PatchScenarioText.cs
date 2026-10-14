// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.Scenario;

namespace MillenniumEye;

[HarmonyPatch(typeof(ScenarioBehaviour_Text), "ProgressInit")]
internal class PatchScenarioText
{
	private static string _lastText = "";

	[HarmonyPostfix]
	private static void Postfix(ScenarioBehaviour_Text __instance)
	{
		try
		{
			string text = __instance.m_Text;
			if (!string.IsNullOrWhiteSpace(text))
			{
				text = TextUtil.StripTags(text).Trim();
				if (!string.IsNullOrWhiteSpace(text) && !(text == _lastText))
				{
					_lastText = text;
					Log.Write("[Scenario] Text: " + text);
					Speech.SayQueued(text);
				}
			}
		}
		catch
		{
		}
	}

	public static void Reset()
	{
		_lastText = "";
	}
}
