// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.MDMarkup;

namespace MillenniumEye;

[HarmonyPatch(typeof(MDMarkupPageWidgetBase), "BindContentData")]
internal class PatchMDMarkupBindContent
{
	[HarmonyPostfix]
	private static void Postfix(MDMarkupPageWidgetBase __instance)
	{
		try
		{
			if (NavigationState.IsInDuel)
			{
				string text = __instance.m_CaptionText?.text;
				string text2 = __instance.m_Text?.text;
				text = TextUtil.StripTags(text);
				text2 = TextUtil.StripTags(text2);
				if (!string.IsNullOrWhiteSpace(text) || !string.IsNullOrWhiteSpace(text2))
				{
					string text3 = ((!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(text2)) ? (text + ". " + text2) : (text ?? text2));
					Log.Write("[MatchTips] " + text3);
					Speech.SayQueued(text3);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchMDMarkupBindContent] " + ex.Message);
		}
	}
}
