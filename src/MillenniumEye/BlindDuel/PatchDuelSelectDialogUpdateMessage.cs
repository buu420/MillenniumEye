// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(DuelSelectDialog), "UpdateMessage")]
internal class PatchDuelSelectDialogUpdateMessage
{
	private static int _lastIndex = -1;

	[HarmonyPostfix]
	private static void Postfix(DuelSelectDialog __instance, int effectIndex)
	{
		try
		{
			if (effectIndex == _lastIndex)
			{
				return;
			}
			_lastIndex = effectIndex;
			List<DuelSelectDialog.Info> infoList = __instance.infoList;
			if (infoList == null || effectIndex < 0 || effectIndex >= infoList.Count)
			{
				return;
			}
			string message = infoList[effectIndex].message;
			if (!string.IsNullOrWhiteSpace(message))
			{
				string text = TextUtil.StripTags(message);
				if (!string.IsNullOrWhiteSpace(text))
				{
					Log.Write($"[DuelSelectTab] Tab {effectIndex}: {text}");
					Speech.SayItem(text);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchDuelSelectDialogUpdateMessage] " + ex.Message);
		}
	}

	public static void ResetDedup()
	{
		_lastIndex = -1;
	}
}
