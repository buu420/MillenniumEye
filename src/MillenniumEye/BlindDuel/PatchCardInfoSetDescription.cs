// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(CardInfo), "SetDescriptionArea")]
internal class PatchCardInfoSetDescription
{
	private const float ReadDelay = 0.15f;

	private static int _lastUniqueId;

	private static int _pendingMrk;

	private static int _pendingUniqueId;

	public static int PendingMrk => _pendingMrk;

	public static int PendingUniqueId => _pendingUniqueId;

	[HarmonyPostfix]
	private static void Postfix(CardInfo __instance)
	{
		try
		{
			if (!__instance.gameObject.activeInHierarchy) return;
			CardInfoData data = __instance.m_CardInfoData;
			if (NavigationState.IsInDuel)
			{
				if (!DuelState.HasPhaseStarted || DuelState.IsShowingResult) return;
				// Invalidate first: failures and hidden selections must never retain
				// the previous card's effect description.
				DuelState.CardDetailLines = null;
				if (data.cardid <= 0) return;
				if (data.hasinstance && !DuelState.IsMyPlayer(data.player))
				{
					if (data.position == Engine.PosDeck || data.position == Engine.PosExtra ||
						!EngineApiUtil.IsCardKnown(data.player, data.position, data.index)) return;
				}
				CardData card = CardReader.ReadCardFromData(data.cardid);
				string battlePosition = null;
				string zone = null;
				if (data.hasinstance && FieldFocusHandler.IsMonsterZone(data.position))
				{
					card.Atk = data.basicval.Atk >= 0 ? data.basicval.Atk.ToString() : "?";
					if (!card.IsLink) card.Def = data.basicval.Def >= 0 ? data.basicval.Def.ToString() : "?";
					battlePosition = !Engine.GetCardFace(data.player, data.position, data.index) ? "Set" :
						Engine.GetCardTurn(data.player, data.position, data.index) ? "Defense Mode" : "Attack Mode";
				}
				if (data.hasinstance && data.position == Engine.PosHand && DuelState.IsMyPlayer(data.player))
				{
					int count = Engine.GetCardNum(data.player, Engine.PosHand);
					zone = count > 0 ? $"Hand, {data.index + 1} of {count}" : "Hand";
				}
				DuelState.CardDetailLines = card.GetDetailLines(out string summary, battlePosition, zone);
				Log.Write($"[CardDetails] Visible panel mrk={data.cardid}, uid={data.uniqueid}, player={data.player}, pos={data.position}");
				if (data.hasinstance && data.position == Engine.PosHand && DuelState.IsMyPlayer(data.player) && !string.IsNullOrWhiteSpace(summary))
				{
					if (DuelState.MessageJustAnnounced) Speech.SayQueued(summary);
					else Speech.SayItem(summary);
				}
				return;
			}
			_pendingMrk = data.cardid;
			_pendingUniqueId = data.uniqueid;
			MillenniumEyeCore.Instance.CancelInvoke("ReadCardDelayed");
			MillenniumEyeCore.Instance.Invoke("ReadCardDelayed", ReadDelay);
		}
		catch (Exception ex)
		{
			if (NavigationState.IsInDuel) DuelState.CardDetailLines = null;
			_pendingMrk = 0;
			_pendingUniqueId = 0;
			Log.Write("[SetDescription] " + ex.Message);
		}
	}
	public static bool CheckAndUpdateDedup(int uniqueId)
	{
		if (uniqueId <= 0)
		{
			return false;
		}
		if (uniqueId == _lastUniqueId)
		{
			return true;
		}
		_lastUniqueId = uniqueId;
		return false;
	}

	public static void ResetDedup()
	{
		_lastUniqueId = 0;
	}

	public static void ResetHandDedup()
	{
		ResetDedup();
	}
}
