// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(InstantMessage), "ReqOpen")]
internal class PatchInstantMessageReqOpen
{
	[HarmonyPostfix]
	private static void Postfix(string message)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}
			string text = TextUtil.StripTags(message);
			if (string.IsNullOrWhiteSpace(text) || text == InstantMessageDedup.LastMessage)
			{
				return;
			}
			InstantMessageDedup.LastMessage = text;
			Log.Write("[InstantMessageReq] " + text);
			if (NavigationState.IsInDuel)
			{
				if (DuelState.MessageJustAnnounced)
				{
					Speech.SayQueued(text);
					return;
				}
				DuelState.MessageJustAnnounced = true;
				Speech.SayImmediate(text);
			}
			else
			{
				Speech.SayQueued(text);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchInstantMessageReqOpen] " + ex.Message);
		}
	}
}
