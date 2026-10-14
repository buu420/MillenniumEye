// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using Il2CppYgomGame.Duel;
using UnityEngine;

namespace MillenniumEye;

internal static class DuelFieldNav
{
	public static void HandleInput(bool shift)
	{
		if (shift && Input.GetKeyDown(KeyCode.P))
		{
			if (!DuelState.HasPhaseStarted || DuelState.IsShowingResult)
			{ Speech.SayImmediate("Opponent card counts unavailable"); return; }
			try
			{
				int opponent = GetOpponent();
				string hand = FieldFocusHandler.ReadZoneCount("Opponent's Hand", opponent, Engine.PosHand);
				string deck = FieldFocusHandler.ReadZoneCount("Deck", opponent, Engine.PosDeck);
				string extra = FieldFocusHandler.ReadZoneCount("Extra Deck", opponent, Engine.PosExtra);
				Speech.SayImmediate(hand + ". " + deck + ". " + extra);
			}
			catch (System.Exception ex) { Log.Write("[ZoneCount] " + ex.Message); Speech.SayImmediate("Opponent card counts unavailable"); }
			return;
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			int player = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			string value = (shift ? "Opponent's" : "Your");
			int lP = DuelClient.GetLP(player);
			Speech.SayImmediate($"{value} life points: {lP}");
			return;
		}
		int? num = null;
		int position = 0;
		if (Input.GetKeyDown(KeyCode.C))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosHand;
		}
		else if (Input.GetKeyDown(KeyCode.M))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosMonsterLL;
		}
		else if (Input.GetKeyDown(KeyCode.S))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosMagicLL;
		}
		else if (Input.GetKeyDown(KeyCode.T))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosField;
		}
		else if (Input.GetKeyDown(KeyCode.G))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosGrave;
		}
		else if (Input.GetKeyDown(KeyCode.B))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosExclude;
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosExtra;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosMonsterLL;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosMonsterL;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosMonsterC;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosMonsterR;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			num = (shift ? GetOpponent() : DuelState.GetMyPlayerNum());
			position = Engine.PosMonsterRR;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			num = DuelState.GetMyPlayerNum();
			position = Engine.PosExLMonster;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			num = DuelState.GetMyPlayerNum();
			position = Engine.PosExRMonster;
		}
		if (num.HasValue)
		{
			FieldFocusHandler.FocusPosition(num.Value, position);
		}
	}

	private static int GetOpponent()
	{
		return (DuelState.GetMyPlayerNum() == 0) ? 1 : 0;
	}

	public static void Reset()
	{
	}
}
