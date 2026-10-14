// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

internal static class DuelLogHelper
{
	private static int _prevActionCount;

	public static void CapturePrevActionCount(DuelLogController instance)
	{
		try
		{
			_prevActionCount = instance.m_DataList_ShowAction?.Count ?? 0;
		}
		catch
		{
			_prevActionCount = -1;
		}
	}

	public static void AnnounceSummon(DuelLogController instance, int prevCardNameCount, string defaultLabel)
	{
		try
		{
			if (!NavigationState.IsInDuel || prevCardNameCount < 0)
			{
				return;
			}
			string text = defaultLabel;
			int? value = null;
			try
			{
				List<ShowActionData> dataList_ShowAction = instance.m_DataList_ShowAction;
				if (dataList_ShowAction != null && _prevActionCount >= 0 && dataList_ShowAction.Count > _prevActionCount)
				{
					ShowActionData showActionData = dataList_ShowAction[_prevActionCount];
					LogDataCenter datac = showActionData.datac;
					LOGACTIONTYPE acttype = datac.acttype;
					string summonTypeName = GetSummonTypeName(acttype);
					if (summonTypeName != null)
					{
						text = summonTypeName;
					}
					try
					{
						value = showActionData.datal.owner;
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
			List<ShowCardNameData> dataList_ShowCardName = instance.m_DataList_ShowCardName;
			if (dataList_ShowCardName == null || dataList_ShowCardName.Count <= prevCardNameCount)
			{
				return;
			}
			for (int i = prevCardNameCount; i < dataList_ShowCardName.Count; i++)
			{
				ShowCardNameData showCardNameData = dataList_ShowCardName[i];
				if (!showCardNameData.isCost)
				{
					int cardid = showCardNameData.cardid;
					bool flag = (value.HasValue ? (!DuelState.IsMyPlayer(value.Value)) : DuelState.IsOpponentTeam(showCardNameData.team));
					string text2 = ResolveCardName(cardid);
					Log.Write($"[DuelLog] {text}: {text2} (owner={value}, cardTeam={showCardNameData.team}, opponent={flag}, cardid={cardid})");
					DuelState.MessageJustAnnounced = true;
					string text3 = (flag ? "Opponent " : "");
					Speech.SayImmediate(text3 + text + ": " + text2);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[DuelLogHelper] " + ex.Message);
		}
	}

	public static void AnnounceActivation(DuelLogController instance, int prevCardNameCount)
	{
		try
		{
			if (!NavigationState.IsInDuel || prevCardNameCount < 0)
			{
				return;
			}
			int? value = null;
			try
			{
				List<ShowActionData> dataList_ShowAction = instance.m_DataList_ShowAction;
				if (dataList_ShowAction != null && _prevActionCount >= 0 && dataList_ShowAction.Count > _prevActionCount)
				{
					try
					{
						value = dataList_ShowAction[_prevActionCount].datal.owner;
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
			List<ShowCardNameData> dataList_ShowCardName = instance.m_DataList_ShowCardName;
			if (dataList_ShowCardName == null || dataList_ShowCardName.Count <= prevCardNameCount)
			{
				return;
			}
			for (int i = prevCardNameCount; i < dataList_ShowCardName.Count; i++)
			{
				ShowCardNameData showCardNameData = dataList_ShowCardName[i];
				if (!showCardNameData.isCost)
				{
					int cardid = showCardNameData.cardid;
					bool flag = (value.HasValue ? (!DuelState.IsMyPlayer(value.Value)) : DuelState.IsOpponentTeam(showCardNameData.team));
					string text = ResolveCardName(cardid);
					// The native log has publicly named the activation. Its card,
					// not the previously inspected field card, owns detail reading.
					CardReader.SetPublicDuelDetails(cardid);
					FieldFocusHandler.ClearLastFocus();
					DuelState.LastQueuedButtonText = null;
					Log.Write($"[DuelLog] Activates: {text} (owner={value}, cardTeam={showCardNameData.team}, opponent={flag}, cardid={cardid})");
					DuelState.MessageJustAnnounced = true;
					string text2 = (flag ? "Opponent activates" : "Activate");
					Speech.SayImmediate(text2 + ": " + text);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[DuelLogHelper] Activation: " + ex.Message);
		}
	}

	public static string ResolveCardName(int cardId)
	{
		try
		{
			Content s_instance = Content.s_instance;
			if (s_instance != null && cardId > 0)
			{
				return s_instance.GetName(cardId);
			}
		}
		catch
		{
		}
		return "unknown";
	}

	private static string GetSummonTypeName(LOGACTIONTYPE type)
	{
		if (1 == 0)
		{
		}
		string result = type switch
		{
			LOGACTIONTYPE.ACTION_SUMMON => "Summon", 
			LOGACTIONTYPE.ACTION_SPSUMMON => "Special Summon", 
			LOGACTIONTYPE.ACTION_SPSUMMON_RITUAL => "Ritual Summon", 
			LOGACTIONTYPE.ACTION_SPSUMMON_FUSION => "Fusion Summon", 
			LOGACTIONTYPE.ACTION_SPSUMMON_SYNC => "Synchro Summon", 
			LOGACTIONTYPE.ACTION_SPSUMMON_XYZ => "Xyz Summon", 
			LOGACTIONTYPE.ACTION_SPSUMMON_PENDULUM => "Pendulum Summon", 
			LOGACTIONTYPE.ACTION_SPSUMMON_LINK => "Link Summon", 
			LOGACTIONTYPE.ACTION_REVERSESUMMON => "Flip Summon", 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
