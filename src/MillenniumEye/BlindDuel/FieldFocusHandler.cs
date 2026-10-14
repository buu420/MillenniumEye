// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.Duel;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

internal static class FieldFocusHandler
{
	private static int _lastPlayer;

	private static int _lastPosition;

	private static int _lastViewIndex;

	private static bool _hasLastFocus;

	private static DuelClient.onFocusFieldDelegate _handler;

	public static void Subscribe(DuelClient client)
	{
		_handler = (Action<int, int, int>)OnFieldFocused;
		client.add_onFocusFieldHandler(_handler);
		Log.Write("[FieldFocus] Subscribed to onFocusFieldHandler");
	}

	public static void FocusPosition(int player, int position, int viewIndex = 0)
	{
		try
		{
			string anchorName = GetAnchorName(player, position, viewIndex);
			if (anchorName != null)
			{
				GameObject gameObject = GameObject.Find(anchorName);
				if (gameObject == null)
				{
					Log.Write("[FieldNav] GO not found: " + anchorName);
				}
				else
				{
					SelectionButton component = gameObject.GetComponent<SelectionButton>();
					if (component == null)
					{
						Log.Write("[FieldNav] No SelectionButton on " + anchorName);
					}
					else
					{
						try
						{
							Il2CppArrayBase<SelectionButton> il2CppArrayBase = UnityEngine.Object.FindObjectsOfType<SelectionButton>();
							foreach (SelectionButton item in il2CppArrayBase)
							{
								try
								{
									if (item.isSelected)
									{
										item.OnDeselected();
									}
								}
								catch
								{
								}
							}
						}
						catch
						{
						}
						SelectionItem selectionItem = component.TryCast<SelectionItem>();
						bool flag = selectionItem.Select(initializeSelection: false, force: true);
						Log.Write($"[FieldNav] {anchorName} Select={flag}");
						if (flag)
						{
							return;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[FieldNav] " + ex.Message);
		}
		OnFieldFocused(player, position, viewIndex);
	}

	private static string GetAnchorName(int player, int position, int viewIndex)
	{
		string text = (DuelState.IsMyPlayer(player) ? "Near" : "Far");
		if (position == Engine.PosHand)
		{
			return $"{text}HandCard/HandCard{viewIndex}/HandCardButton{viewIndex}";
		}
		string text2 = null;
		if (position == Engine.PosMonsterLL)
		{
			text2 = "Monster0";
		}
		else if (position == Engine.PosMonsterL)
		{
			text2 = "Monster1";
		}
		else if (position == Engine.PosMonsterC)
		{
			text2 = "Monster2";
		}
		else if (position == Engine.PosMonsterR)
		{
			text2 = "Monster3";
		}
		else if (position == Engine.PosMonsterRR)
		{
			text2 = "Monster4";
		}
		else if (position == Engine.PosMagicLL)
		{
			text2 = "Magic0";
		}
		else if (position == Engine.PosMagicL)
		{
			text2 = "Magic1";
		}
		else if (position == Engine.PosMagicC)
		{
			text2 = "Magic2";
		}
		else if (position == Engine.PosMagicR)
		{
			text2 = "Magic3";
		}
		else if (position == Engine.PosMagicRR)
		{
			text2 = "Magic4";
		}
		else if (position == Engine.PosExLMonster)
		{
			text2 = "ExMonsterL";
		}
		else if (position == Engine.PosExRMonster)
		{
			text2 = "ExMonsterR";
		}
		else if (position == Engine.PosField)
		{
			text2 = "FieldMagic";
		}
		else if (position == Engine.PosGrave)
		{
			text2 = "Grave";
		}
		else if (position == Engine.PosExtra)
		{
			text2 = "Extra";
		}
		else if (position == Engine.PosDeck)
		{
			text2 = "MainDeck";
		}
		else if (position == Engine.PosExclude)
		{
			text2 = "Exclude";
		}
		if (text2 == null)
		{
			return null;
		}
		return "Anchor_" + text + "_" + text2;
	}

	private static void OnFieldFocused(int player, int position, int viewIndex)
	{
		try
		{
			if (!NavigationState.IsInDuel || !DuelState.HasPhaseStarted || DuelState.IsShowingResult)
			{
				return;
			}
			PatchCardInfoSetDescription.ResetHandDedup();
			DuelHandler.ResetSelectionDedup();
			if (DuelState.HasPendingSelection)
			{
				DuelState.DeferredFieldFocus = (player, position, viewIndex);
				return;
			}
			try
			{
				DuelClient instance = DuelClient.instance;
				if (instance != null && instance.fieldInputBlockCounter > 0)
				{
					return;
				}
			}
			catch
			{
			}
			if (DuelState.SuppressNextFieldFocus)
			{
				DuelState.SuppressNextFieldFocus = false;
				if (position != Engine.PosExtra && position != Engine.PosDeck)
				{
					_lastPlayer = player;
					_lastPosition = position;
					_lastViewIndex = viewIndex;
					_hasLastFocus = true;
				}
				return;
			}
			bool messageJustAnnounced = DuelState.MessageJustAnnounced;
			DuelState.MessageJustAnnounced = false;
			NavigationState.ScreenJustAnnounced = false;
			NavigationState.DialogJustAnnounced = false;
			DuelState.InSelectionList = false;
			_lastPlayer = player;
			_lastPosition = position;
			_lastViewIndex = viewIndex;
			_hasLastFocus = true;
			string zoneName = GetZoneName(player, position, viewIndex);
			DuelState.CardDetailLines = null;
			if (position == Engine.PosGrave || position == Engine.PosExtra || position == Engine.PosExclude)
			{
				DuelState.LastBrowsePlayer = player;
				DuelState.LastBrowsePosition = position;
				DuelState.BrowseIndex = -1;
				DuelState.BrowseDirection = 1;
				DuelState.LastBrowseLogicalIdx = -1;
			}
			else
			{
				DuelState.LastBrowsePlayer = -1;
				DuelState.LastBrowsePosition = -1;
				DuelState.BrowseIndex = -1;
				DuelState.BrowseDirection = 1;
				DuelState.LastBrowseLogicalIdx = -1;
			}
			if (position == Engine.PosExtra || position == Engine.PosDeck)
			{
				DuelState.CardDetailLines = null;
				DuelState.CardDetailIndex = 0;
				if (!string.IsNullOrEmpty(zoneName))
				{
					SpeakField(zoneName, messageJustAnnounced);
				}
				_hasLastFocus = false;
				return;
			}
			int num = 0;
			int num2 = 0;
			try
			{
				num = Engine.GetCardID(player, position, viewIndex);
				num2 = Engine.GetCardUniqueID(player, position, viewIndex);
			}
			catch (Exception ex)
			{
				Log.Write("[FocusField] Engine query failed: " + ex.Message);
			}
			if (num <= 0 && !DuelState.IsMyPlayer(player) && (IsMonsterZone(position) || IsSpellTrapZone(position) || position == Engine.PosHand))
			{
				try
				{
					int cardNum = Engine.GetCardNum(player, position);
					if (cardNum > 0)
					{
						string text = ((!string.IsNullOrEmpty(zoneName)) ? ("Face-down card, " + zoneName) : "Face-down card");
						Log.Write("[FocusField] " + text);
						SpeakField(text, messageJustAnnounced);
						DuelState.CardDetailLines = null;
						DuelState.CardDetailIndex = 0;
						return;
					}
				}
				catch (Exception ex2)
				{
					Log.Write("[FocusField] Face-down check: " + ex2.Message);
				}
			}
			if (num <= 0)
			{
				if (!string.IsNullOrEmpty(zoneName))
				{
					Log.Write("[FocusField] Empty: " + zoneName);
					SpeakField(zoneName, messageJustAnnounced);
				}
				DuelState.CardDetailLines = null;
				DuelState.CardDetailIndex = 0;
				return;
			}
			if (!DuelState.IsMyPlayer(player))
			{
				try
				{
					if (!EngineApiUtil.IsCardKnown(player, position, viewIndex))
					{
						string text2 = ((!string.IsNullOrEmpty(zoneName)) ? ("Face-down card, " + zoneName) : "Face-down card");
						Log.Write("[FocusField] " + text2);
						SpeakField(text2, messageJustAnnounced);
						DuelState.CardDetailLines = null;
						DuelState.CardDetailIndex = 0;
						return;
					}
				}
				catch
				{
					DuelState.CardDetailLines = null;
					SpeakField("Card details unavailable, " + zoneName, messageJustAnnounced);
					return;
				}
			}
			bool flag = false;
			try
			{
				Content s_instance = Content.s_instance;
				if (s_instance != null)
				{
					flag = s_instance.GetFrame(num) == Content.Frame.Link;
				}
			}
			catch
			{
			}
			string battlePosition = null;
			int? num3 = null;
			int? num4 = null;
			if (IsMonsterZone(position))
			{
				num3 = -1;
				if (!flag) num4 = -1;
				if (flag)
				{
					battlePosition = "Attack Mode";
				}
				else
				{
					try
					{
						bool cardFace = Engine.GetCardFace(player, position, viewIndex);
						bool cardTurn = Engine.GetCardTurn(player, position, viewIndex);
						battlePosition = ((!cardFace) ? "Set" : (cardTurn ? "Defense Mode" : "Attack Mode"));
					}
					catch (Exception ex3)
					{
						Log.Write("[FocusField] Battle mode check: " + ex3.Message);
					}
				}
			}
			if (num2 > 0 && IsMonsterZone(position))
			{
				try
				{
					Engine.BasicVal basicValByUniqueId = Engine.GetBasicValByUniqueId(num2);
					num3 = basicValByUniqueId.Atk;
					if (!flag)
					{
						num4 = basicValByUniqueId.Def;
					}
				}
				catch (Exception ex4)
				{
					Log.Write("[FocusField] BasicVal: " + ex4.Message);
					if (IsMonsterZone(position)) { num3 = -1; if (!flag) num4 = -1; }
				}
			}
			CardData cardData = CardReader.ReadCardFromData(num);
			if (num3.HasValue)
			{
				cardData.Atk = ((num3.Value >= 0) ? num3.Value.ToString() : "?");
			}
			if (num4.HasValue && !cardData.IsLink)
			{
				cardData.Def = ((num4.Value >= 0) ? num4.Value.ToString() : "?");
			}
			List<string> detailLines = cardData.GetDetailLines(out var summary, battlePosition, zoneName);
			DuelState.CardDetailLines = detailLines;
			DuelState.CardDetailIndex = 0;
			if (!string.IsNullOrEmpty(summary))
			{
				SpeakField(summary, messageJustAnnounced);
			}
		}
		catch (Exception ex5)
		{
			DuelState.CardDetailLines = null;
			Log.Write("[PatchInvokeFocusField] " + ex5.Message);
		}
	}

	public static void ResetDedup()
	{
		Speech.ResetDedup();
	}

	public static void ClearLastFocus()
	{
		_hasLastFocus = false;
		ResetDedup();
	}

	public static void SpeakDeferredFocus(int player, int position, int viewIndex)
	{
		DuelState.CardDetailLines = null;
		string zoneName = GetZoneName(player, position, viewIndex);
		if (position == Engine.PosExtra || position == Engine.PosDeck)
		{
			if (!string.IsNullOrEmpty(zoneName)) Speech.SayQueued(zoneName);
			return;
		}
		if (string.IsNullOrEmpty(zoneName))
		{
			return;
		}
		int num = 0;
		try
		{
			num = Engine.GetCardID(player, position, viewIndex);
		}
		catch
		{
		}
		if (num <= 0 && !DuelState.IsMyPlayer(player) && (IsMonsterZone(position) || IsSpellTrapZone(position)))
		{
			try
			{
				int cardNum = Engine.GetCardNum(player, position);
				if (cardNum > 0)
				{
					Speech.SayQueued("Face-down card, " + zoneName);
					return;
				}
			}
			catch
			{
			}
		}
		if (num <= 0)
		{
			if (!string.IsNullOrEmpty(zoneName))
			{
				Speech.SayQueued(zoneName);
			}
			return;
		}
		if (!DuelState.IsMyPlayer(player))
		{
			try
			{
				if (!EngineApiUtil.IsCardKnown(player, position, viewIndex))
				{
					Speech.SayQueued("Face-down card, " + zoneName);
					return;
				}
			}
			catch
			{
				DuelState.CardDetailLines = null;
				Speech.SayQueued("Card details unavailable, " + zoneName);
				return;
			}
		}
		int? atk = null, def = null;
		string battlePosition = null;
		if (IsMonsterZone(position))
		{
			CardData card = CardReader.ReadCardFromData(num);
			try
			{
				int uid = Engine.GetCardUniqueID(player, position, viewIndex);
				if (uid <= 0) throw new InvalidOperationException("No current card instance");
				Engine.BasicVal value = Engine.GetBasicValByUniqueId(uid);
				atk = value.Atk;
				if (!card.IsLink) def = value.Def;
				battlePosition = !Engine.GetCardFace(player, position, viewIndex) ? "Set" :
					Engine.GetCardTurn(player, position, viewIndex) ? "Defense Mode" : "Attack Mode";
			}
			catch (Exception ex)
			{
				Log.Write("[DeferredFocus] Current stats: " + ex.Message);
				atk = -1; if (!card.IsLink) def = -1;
			}
		}
		CardReader.SpeakCardFromData(num, zoneName, atk, def, queued: true, battlePosition: battlePosition);
	}

	public static void RequeueLastFocus()
	{
		if (_hasLastFocus)
		{
			_hasLastFocus = false;
			SpeakDeferredFocus(_lastPlayer, _lastPosition, _lastViewIndex);
		}
	}

	private static void SpeakField(string text, bool queued)
	{
		if (queued)
		{
			Speech.SayQueued(text);
		}
		else
		{
			Speech.SayItem(text);
		}
	}

	private static bool IsSpellTrapZone(int position)
	{
		return position == Engine.PosMagicLL || position == Engine.PosMagicL || position == Engine.PosMagicC || position == Engine.PosMagicR || position == Engine.PosMagicRR;
	}

	private static string GetZoneName(int player, int position, int viewIndex)
	{
		string text = ((!DuelState.IsMyPlayer(player)) ? "Opponent's " : "");
		if (position == Engine.PosMonsterLL)
		{
			return text + "Monster Zone 1";
		}
		if (position == Engine.PosMonsterL)
		{
			return text + "Monster Zone 2";
		}
		if (position == Engine.PosMonsterC)
		{
			return text + "Monster Zone 3";
		}
		if (position == Engine.PosMonsterR)
		{
			return text + "Monster Zone 4";
		}
		if (position == Engine.PosMonsterRR)
		{
			return text + "Monster Zone 5";
		}
		if (position == Engine.PosMagicLL)
		{
			return text + "Spell Trap Zone 1";
		}
		if (position == Engine.PosMagicL)
		{
			return text + "Spell Trap Zone 2";
		}
		if (position == Engine.PosMagicC)
		{
			return text + "Spell Trap Zone 3";
		}
		if (position == Engine.PosMagicR)
		{
			return text + "Spell Trap Zone 4";
		}
		if (position == Engine.PosMagicRR)
		{
			return text + "Spell Trap Zone 5";
		}
		if (position == Engine.PosField)
		{
			return text + "Field Spell Zone";
		}
		if (position == Engine.PosPendulumLeft)
		{
			return text + "Left Pendulum Zone";
		}
		if (position == Engine.PosPendulumRight)
		{
			return text + "Right Pendulum Zone";
		}
		if (position == Engine.PosExLMonster)
		{
			return text + "Extra Monster Zone Left";
		}
		if (position == Engine.PosExRMonster)
		{
			return text + "Extra Monster Zone Right";
		}
		if (position == Engine.PosHand)
		{
			try
			{
				int cardNum = Engine.GetCardNum(player, Engine.PosHand);
				if (!DuelState.IsMyPlayer(player))
				{
					return DuelReadingPolicy.ZoneCount(text + "Hand", cardNum);
				}
				if (cardNum > 0 && viewIndex >= 0)
				{
					return $"{text}Hand, {viewIndex + 1} of {cardNum}";
				}
			}
			catch
			{
			}
			return DuelState.IsMyPlayer(player) ? text + "Hand" : DuelReadingPolicy.ZoneCount(text + "Hand", null);
		}
		if (position == Engine.PosExtra)
		{
			return ReadZoneCount(text + "Extra Deck", player, position);
		}
		if (position == Engine.PosDeck)
		{
			return ReadZoneCount(text + "Deck", player, position);
		}
		if (position == Engine.PosGrave)
		{
			try
			{
				int cardNum2 = Engine.GetCardNum(player, Engine.PosGrave);
				if (cardNum2 > 0)
				{
					return $"{text}Graveyard, {cardNum2} cards";
				}
			}
			catch
			{
			}
			return text + "Graveyard";
		}
		if (position == Engine.PosExclude)
		{
			try
			{
				int cardNum3 = Engine.GetCardNum(player, Engine.PosExclude);
				if (cardNum3 > 0)
				{
					return $"{text}Banished, {cardNum3} cards";
				}
			}
			catch
			{
			}
			return text + "Banished";
		}
		Log.Write($"[FocusField] Unknown position: player={player}, pos={position}, mapped to nothing");
		return null;
	}

	internal static string ReadZoneCount(string label, int player, int position)
	{
		try { return DuelReadingPolicy.ZoneCount(label, Engine.GetCardNum(player, position)); }
		catch (Exception ex) { Log.Write("[ZoneCount] " + ex.Message); return DuelReadingPolicy.ZoneCount(label, null); }
	}

	internal static bool IsMonsterZone(int position)
	{
		return position == Engine.PosMonsterLL || position == Engine.PosMonsterL || position == Engine.PosMonsterC || position == Engine.PosMonsterR || position == Engine.PosMonsterRR || position == Engine.PosExLMonster || position == Engine.PosExRMonster;
	}
}
