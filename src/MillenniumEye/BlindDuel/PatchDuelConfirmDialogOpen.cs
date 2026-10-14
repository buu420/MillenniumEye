// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch]
internal class PatchDuelConfirmDialogOpen
{
	private static IEnumerable<MethodBase> TargetMethods()
	{
		List<MethodBase> list = new List<MethodBase>();
		MethodInfo[] methods = typeof(DuelConfirmDialog).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		foreach (MethodInfo methodInfo in methods)
		{
			// The four-parameter convenience overload calls this canonical one.
			// Hook once so the visible prompt is not announced twice.
			if (methodInfo.Name == "Open" && methodInfo.GetParameters().Length == 6 && methodInfo.GetParameters()[0].ParameterType == typeof(string))
			{
				list.Add(methodInfo);
			}
		}
		return list;
	}

	[HarmonyPostfix]
	private static void Postfix(string message)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(message))
			{
				string text = TextUtil.StripTags(message);
				if (!string.IsNullOrWhiteSpace(text))
				{
					Log.Write("[DuelConfirmDialog] " + text);
					Speech.SayImmediate(text);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchDuelConfirmDialogOpen] " + ex.Message);
		}
	}
}
