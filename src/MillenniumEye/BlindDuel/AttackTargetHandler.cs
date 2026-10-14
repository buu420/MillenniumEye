// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

internal static class AttackTargetHandler
{
	private static DuelClient.onDecideAttackTargetDelegate _handler;

	public static void Subscribe(DuelClient client)
	{
		_handler = (Action<int, int, int, int, int, int>)OnAttackDeclared;
		client.add_onDecideAttackTargetHandler(_handler);
		Log.Write("[AttackTarget] Subscribed to onDecideAttackTargetHandler");
	}

	private static void OnAttackDeclared(int attackerPlayer, int attackerPosition, int attackerIndex, int targetPlayer, int targetPosition, int targetIndex)
	{
		try
		{
			if (DuelState.IsMyPlayer(attackerPlayer))
			{
				return;
			}
			Content s_instance = Content.s_instance;
			if (s_instance != null)
			{
				int num = 0;
				try
				{
					num = Engine.GetCardID(attackerPlayer, attackerPosition, attackerIndex);
				}
				catch
				{
				}
				string text = ((num > 0) ? s_instance.GetName(num) : "Unknown monster");
				int num2 = 0;
				try
				{
					num2 = Engine.GetCardID(targetPlayer, targetPosition, targetIndex);
				}
				catch
				{
				}
				string text2;
				if (num2 > 0)
				{
					string name = s_instance.GetName(num2);
					text2 = "Opponent attacks " + name + " with " + text;
				}
				else
				{
					text2 = "Opponent attacks directly with " + text;
				}
				Log.Write("[AttackTarget] " + text2);
				Speech.SayQueued(text2);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[AttackTarget] " + ex.Message);
		}
	}
}
