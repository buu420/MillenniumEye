// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.Tutorial;
using UnityEngine.Events;

namespace MillenniumEye;

[HarmonyPatch(typeof(TutorialNavigator), "PlayCenterMsg", new Type[]
{
	typeof(Il2CppSystem.Collections.Generic.IList<string>),
	typeof(UnityAction),
	typeof(float)
})]
internal class PatchTutorialCenterMsg
{
	private static string _lastMessage = "";

	[HarmonyPostfix]
	private static void Postfix(Il2CppSystem.Collections.Generic.IList<string> messages)
	{
		try
		{
			if (messages == null)
			{
				return;
			}
			System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
			int num = 0;
			while (true)
			{
				string text;
				try
				{
					text = messages[num];
				}
				catch
				{
					break;
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					list.Add(TextUtil.StripTags(text));
				}
				num++;
			}
			string text2 = string.Join(". ", list);
			if (!string.IsNullOrWhiteSpace(text2) && !(text2 == _lastMessage))
			{
				_lastMessage = text2;
				Log.Write("[TutorialCenter] " + text2);
				if (NavigationState.IsInDuel)
				{
					DuelState.MessageJustAnnounced = true;
					Speech.SayImmediate(text2);
				}
				else
				{
					Speech.SayQueued(text2);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchTutorialCenterMsg] " + ex.Message);
		}
	}
}
