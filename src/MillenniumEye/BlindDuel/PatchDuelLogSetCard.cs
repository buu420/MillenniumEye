// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(CardInfo), "SetCardByCardId")]
internal class PatchDuelLogSetCard
{
	[HarmonyPostfix]
	private static void Postfix(int cardid)
	{
		try
		{
			if (!DuelState.IsDuelLogOpen || DuelState.InSelectionList || cardid <= 0)
			{
				return;
			}
			bool flag = false;
			try
			{
				DuelLogController logController = DuelState.LogController;
				if (logController != null)
				{
					List<ShowCardNameData> dataList_ShowCardName = logController.m_DataList_ShowCardName;
					if (dataList_ShowCardName != null)
					{
						for (int num = dataList_ShowCardName.Count - 1; num >= 0; num--)
						{
							if (dataList_ShowCardName[num].cardid == cardid)
							{
								flag = DuelState.IsOpponentTeam(dataList_ShowCardName[num].team);
								break;
							}
						}
					}
				}
			}
			catch
			{
			}
			string text = DuelLogHelper.ResolveCardName(cardid);
			string text2 = (flag ? ("Opponent: " + text) : text);
			Log.Write("[DuelLog-Card] " + text2);
			Speech.SayItem(text2);
		}
		catch (Exception ex)
		{
			Log.Write("[PatchDuelLogSetCard] " + ex.Message);
		}
	}
}
