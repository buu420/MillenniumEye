// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppYgomGame.Scenario;

namespace MillenniumEye;

[HarmonyPatch(typeof(ScenarioBehaviour_Title), "ProgressInit")]
internal class PatchScenarioTitle
{
	private static string _lastTitle = "";

	[HarmonyPostfix]
	private static void Postfix(ScenarioBehaviour_Title __instance)
	{
		try
		{
			string titleText = __instance.m_TitleText;
			if (!string.IsNullOrWhiteSpace(titleText))
			{
				titleText = TextUtil.StripTags(titleText).Trim();
				if (!string.IsNullOrWhiteSpace(titleText) && !(titleText == _lastTitle))
				{
					_lastTitle = titleText;
					Log.Write("[Scenario] Title: " + titleText);
					Speech.SayQueued(titleText);
				}
			}
		}
		catch
		{
		}
	}

	public static void Reset()
	{
		_lastTitle = "";
	}
}
