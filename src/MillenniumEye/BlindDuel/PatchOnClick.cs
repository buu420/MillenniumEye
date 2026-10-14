// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

[HarmonyPatch(typeof(SelectionButton), "OnClick", MethodType.Normal)]
internal class PatchOnClick
{
	private static readonly HashSet<string> PreviewElements = new HashSet<string>
	{
		"CardPict", "CardClone", "CreateButton", "ImageCard", "NextButton", "PrevButton", "Related Cards", "ThumbButton", "SlotTemplate(Clone)", "Locator",
		"GoldpassRewardButton", "NormalpassRewardButton", "ButtonDuelPass"
	};

	private static readonly Dictionary<string, Menu> TextToMenu = new Dictionary<string, Menu>
	{
		{
			"DUEL",
			Menu.Duel
		},
		{
			"DECK",
			Menu.Deck
		},
		{
			"SOLO",
			Menu.Solo
		},
		{
			"SHOP",
			Menu.Shop
		},
		{
			"MISSION",
			Menu.Missions
		},
		{
			"Notifications",
			Menu.Notifications
		},
		{
			"Game Settings",
			Menu.Settings
		},
		{
			"Duel Pass",
			Menu.DuelPass
		}
	};

	[HarmonyPostfix]
	private static void Postfix(SelectionButton __instance)
	{
		try
		{
			string text = TextExtractor.ExtractFirst(__instance.gameObject);
			if (text != null && TextToMenu.TryGetValue(text, out var value))
			{
				NavigationState.CurrentMenu = value;
			}
		}
		catch (Exception ex)
		{
			Log.Write("[OnClick] " + ex.Message);
		}
		if (__instance.name == "ButtonDecidePositive(Clone)" && NavigationState.IsInDuel)
		{
			NavigationState.IsInDuel = false;
			DuelState.Clear();
		}
		if (PreviewElements.Contains(__instance.name))
		{
			float time = ((NavigationState.CurrentMenu == Menu.DuelPass) ? 1.5f : 0.5f);
			MillenniumEyeCore.Instance.Invoke("ReadCardDelayed", time);
		}
	}
}
