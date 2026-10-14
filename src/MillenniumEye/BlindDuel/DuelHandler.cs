// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppYgomGame.Duel;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public class DuelHandler : IMenuHandler
{
	private static string _pendingSelectionIndex;

	public static void ResetSelectionDedup()
	{
		Speech.ResetButtonDedup();
	}

	public bool CanHandle(string viewControllerName)
	{
		if (viewControllerName == "DuelClient" || viewControllerName == "DuelLive")
		{
			return true;
		}
		return false;
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		Speech.AnnounceScreen("Duel");
		return true;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		if (DuelState.IsShowingResult)
		{
			return "";
		}
		if (!NavigationState.IsInDuel)
		{
			return null;
		}
		try
		{
			ActivateConfirmToggle toggle = button.GetComponentInParent<ActivateConfirmToggle>();
			if (toggle != null) return ActivationConfirmHandler.Label(toggle);
		}
		catch (Exception ex) { Log.Write("[ActivationToggle] Focus read failed: " + ex.Message); }
		try
		{
			CardSelectionList componentInParent = button.GetComponentInParent<CardSelectionList>();
			if (componentInParent != null)
			{
				DuelState.CardDetailLines = null;
				ListCard listCard = button.GetComponent<ListCard>();
				if (listCard == null)
				{
					listCard = button.GetComponentInParent<ListCard>();
				}
				if (!(listCard != null))
				{
					return null;
				}
				try
				{
					ListCardData cardData = listCard.m_CardData;
					if (cardData != null && cardData.cardid > 0)
					{
						// List cells can be reused for another card. Always refresh
						// its data; Speech deduplicates the resulting visible text.
						DuelState.CardDetailLines = null;
						try
						{
							if (!cardData.isknown)
							{
								DuelState.CardDetailLines = null;
								string selectionIndex = GetSelectionIndex(button);
								string text = ((selectionIndex != null) ? ("Face-down card, " + selectionIndex) : "Face-down card");
								if (PatchCardSelectionListSetTitle.ConsumeQueuedFlag())
								{
									Speech.SayQueued(text);
								}
								else
								{
									Speech.SayItem(text);
								}
								return "";
							}
						}
						catch
						{
							DuelState.CardDetailLines = null;
							return "";
						}
						string selectionIndex2 = GetSelectionIndex(button);
						bool flag = PatchCardSelectionListSetTitle.ConsumeQueuedFlag();
						int cardid = cardData.cardid;
						bool queued = flag;
						int? atk = null, def = null;
						string battlePosition = null;
						if (cardData.hasInstance && FieldFocusHandler.IsMonsterZone(cardData.cardstatus.Position))
						{
							atk = cardData.basicval.Atk; def = cardData.basicval.Def;
							battlePosition = !cardData.cardstatus.Face ? "Set" :
								cardData.cardstatus.Turn ? "Defense Mode" : "Attack Mode";
						}
						CardReader.SpeakCardFromData(cardid, selectionIndex2, atk, def, queued, battlePosition);
						return "";
					}
				}
				catch (Exception ex)
				{
						DuelState.CardDetailLines = null;
					Log.Write("[DuelHandler] ListCard read: " + ex.Message);
						return "Card details unavailable";
				}
			}
		}
		catch (Exception ex2)
		{
			Log.Write("[DuelHandler] CardSelection: " + ex2.Message);
		}
		if (DuelState.LastBrowsePosition >= 0)
		{
			try
			{
				if (IsBrowseListButton(button))
				{
					int lastBrowsePlayer = DuelState.LastBrowsePlayer;
					int lastBrowsePosition = DuelState.LastBrowsePosition;
					int num = 0;
					try
					{
						num = Engine.GetCardNum(lastBrowsePlayer, lastBrowsePosition);
					}
					catch
					{
					}
					int browseDirection = DuelState.BrowseDirection;
					int num2 = DuelState.BrowseIndex + browseDirection;
					if (num2 < 0)
					{
						num2 = 0;
					}
					if (num > 0 && num2 >= num)
					{
						num2 = num - 1;
					}
					DuelState.BrowseIndex = num2;
					if (num2 == DuelState.LastBrowseLogicalIdx)
					{
						return "";
					}
					DuelState.LastBrowseLogicalIdx = num2;
					string index = ((num > 0) ? $"{num2 + 1} of {num}" : null);
					string text2 = BuildPileZoneLabel(lastBrowsePlayer, lastBrowsePosition, index);
					if (!DuelState.IsMyPlayer(lastBrowsePlayer))
					{
						try
						{
							if (!EngineApiUtil.IsCardKnown(lastBrowsePlayer, lastBrowsePosition, num2))
							{
								string text3 = ((text2 != null) ? ("Face-down card, " + text2) : "Face-down card");
								DuelState.CardDetailLines = null;
								DuelState.CardDetailIndex = 0;
								Speech.SayItem(text3);
								return "";
							}
						}
						catch
						{
							DuelState.CardDetailLines = null;
							Speech.SayItem("Card details unavailable");
							return "";
						}
					}
					int cardID = Engine.GetCardID(lastBrowsePlayer, lastBrowsePosition, num2);
					if (cardID > 0)
					{
						SpeakCardSummaryFromData(cardID, text2);
						return "";
					}
					string text4 = ((text2 != null) ? ("Face-down card, " + text2) : "Face-down card");
					DuelState.CardDetailLines = null;
					DuelState.CardDetailIndex = 0;
					Speech.SayItem(text4);
					return "";
				}
			}
			catch (Exception ex3)
			{
				Log.Write("[DuelHandler] Zone browse fallback: " + ex3.Message);
			}
		}
		string name = button.name;
		if (name.Contains("Anchor_"))
		{
			return "";
		}
		try
		{
			if (!button.interactable)
			{
				return "";
			}
		}
		catch
		{
		}
		return null;
	}

	private static void SpeakCardSummaryFromData(int mrk, string zoneLabel, bool queued = false)
	{
		MillenniumEyeCore.Preview.Clear();
		CardData cardData = CardReader.ReadCardFromData(mrk);
		List<string> detailLines = cardData.GetDetailLines(out var summary, null, zoneLabel);
		DuelState.CardDetailLines = detailLines;
		DuelState.CardDetailIndex = 0;
		if (string.IsNullOrWhiteSpace(summary))
		{
			summary = ((!string.IsNullOrWhiteSpace(zoneLabel)) ? zoneLabel : cardData.Name);
		}
		if (!string.IsNullOrWhiteSpace(summary))
		{
			if (queued)
			{
				Speech.SayQueued(summary);
			}
			else
			{
				Speech.SayItem(summary);
			}
		}
	}

	private static bool IsPileBrowsePosition(int position)
	{
		return position == Engine.PosGrave || position == Engine.PosExclude || position == Engine.PosExtra;
	}

	private static string BuildPileZoneLabel(int player, int position, string index)
	{
		string text = (DuelState.IsMyPlayer(player) ? "" : "Opponent's ");
		string text2 = null;
		if (position == Engine.PosGrave)
		{
			text2 = text + "Graveyard";
		}
		else if (position == Engine.PosExclude)
		{
			text2 = text + "Banished";
		}
		else if (position == Engine.PosExtra)
		{
			text2 = text + "Extra Deck";
		}
		if (string.IsNullOrEmpty(text2))
		{
			return index;
		}
		return (!string.IsNullOrEmpty(index)) ? (text2 + ", " + index) : text2;
	}

	public static string ConsumeSelectionIndex()
	{
		string pendingSelectionIndex = _pendingSelectionIndex;
		_pendingSelectionIndex = null;
		return pendingSelectionIndex;
	}

	private static string GetSelectionIndex(SelectionButton button)
	{
		try
		{
			Transform parent = button.transform.parent;
			if (parent == null)
			{
				return null;
			}
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < parent.childCount; i++)
			{
				Transform child = parent.GetChild(i);
				if (child.gameObject.activeInHierarchy && !(child.GetComponent<SelectionButton>() == null))
				{
					if (child == button.transform)
					{
						num = num2;
					}
					num2++;
				}
			}
			if (num >= 0 && num2 > 0)
			{
				return $"{num + 1} of {num2}";
			}
		}
		catch (Exception ex)
		{
			Log.Write("[DuelHandler] SelectionIndex: " + ex.Message);
		}
		return null;
	}

	private static bool IsBrowseListButton(SelectionButton button)
	{
		try
		{
			Transform transform = button.transform;
			for (int i = 0; i < 6; i++)
			{
				if (!(transform != null))
				{
					break;
				}
				if (transform.name.Contains("DuelListCard"))
				{
					return true;
				}
				if (transform.name == "template(Clone)" && transform.parent != null && transform.parent.name == "Content")
				{
					return true;
				}
				transform = transform.parent;
			}
		}
		catch
		{
		}
		return false;
	}

	private static string FindCardIndex(int player, int position, int totalCards, int cardid)
	{
		try
		{
			for (int i = 0; i < totalCards; i++)
			{
				if (Engine.GetCardID(player, position, i) == cardid)
				{
					return $"{i + 1} of {totalCards}";
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private static (int index, int total) GetDuelListCardIndex(SelectionButton button)
	{
		try
		{
			Transform transform = button.transform;
			for (int i = 0; i < 6; i++)
			{
				if (!(transform != null))
				{
					break;
				}
				if (transform.name.Contains("DuelListCard") || (transform.name == "template(Clone)" && transform.parent != null && transform.parent.name == "Content"))
				{
					Transform parent = transform.parent;
					if (parent == null)
					{
						break;
					}
					int item = -1;
					int num = 0;
					for (int j = 0; j < parent.childCount; j++)
					{
						Transform child = parent.GetChild(j);
						if (child.gameObject.activeInHierarchy)
						{
							if (child == transform)
							{
								item = num;
							}
							num++;
						}
					}
					return (index: item, total: num);
				}
				transform = transform.parent;
			}
		}
		catch (Exception ex)
		{
			Log.Write("[DuelHandler] ListCardIndex: " + ex.Message);
		}
		return (index: -1, total: 0);
	}
}
