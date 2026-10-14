// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppYgomSystem.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MillenniumEye;

[HarmonyPatch(typeof(ColorContainerGraphic), "SetColor")]
internal class PatchColorContainerGraphic
{
	private static readonly Dictionary<string, string> ParentLabels = new Dictionary<string, string>
	{
		{ "ButtonMaintenance", "Maintenance" },
		{ "ButtonBug", "Issues" },
		{ "ButtonNotification", "Notification" },
		{ "AutoBuildButton", "Auto-build button" },
		{ "ButtonBookmark", "Add card to bookmark button" },
		{ "BookmarkButton", "Bookmarked cards button" },
		{ "HowToGetButton", "How to get button" },
		{ "RelatedCard", "Related cards button" },
		{ "AddButton", "Add +1" },
		{ "RemoveButton", "Remove -1" },
		{ "CardListButton", "Card list button" },
		{ "HistoryButton", "Card history button" },
		{ "ButtonRegulation", "Regulation button" },
		{ "ButtonSecretPack", "Secret pack button" },
		{ "ButtonInfoSwitching", "Switch display mode button" },
		{ "ButtonSave", "Save button" },
		{ "ButtonMenu", "Menu button" },
		{ "ButtonPickupCard", "Show cards on decks preview" },
		{ "BulkDecksDeletionButton", "Bulk deck deletion button" },
		{ "ButtonOpenNeuronDecks", "Link with Yu Gi Oh Database" },
		{ "FilterButton", "Filters button" },
		{ "SortButton", "Sort button" },
		{ "ClearButton", "Clear filters button" },
		{ "ButtonDismantleIncrement", "Increment dismantle amount" },
		{ "ButtonDismantleDecrement", "Decrement dismantle amount" },
		{ "ButtonEnter", "Play" },
		{ "CopyButton", "Copy deck button" },
		{ "OKButton", "Ok" },
		{ "ShowOwnedNumToggle", "Show owned button" }
	};

	private static readonly Dictionary<string, string> GrandparentLabels = new Dictionary<string, string>
	{
		{ "TabMyDeck", "My Deck" },
		{ "TabRental", "Loaner" },
		{ "DuelMenuButton", "Menu button" }
	};

	[HarmonyPostfix]
	private static void Postfix(ColorContainerGraphic __instance)
	{
		try
		{
			if (__instance.currentStatusMode != ColorContainer.StatusMode.Enter)
			{
				return;
			}
			Transform parent = __instance.transform.parent.parent;
			Transform parent2 = parent.parent;
			string name = parent.name;
			string name2 = parent2.name;
			if (NavigationState.IsInDuel && name.Contains("DuelListCard"))
			{
				return;
			}
			string text = null;
			if ((name == "DismantleButton" || name == "CreateButton") ? true : false)
			{
				Transform child = TransformSearch.GetChild(parent, 6, "Dismantle/Create");
				if (child != null)
				{
					string value = TextExtractor.ExtractFirst(child.gameObject);
					string value2 = EnumUtil.ParseRarity(child.GetComponentInChildren<Image>().sprite.name);
					text = ((!(name == "DismantleButton") || !string.IsNullOrEmpty(value)) ? $"{((name == "DismantleButton") ? "Dismantle" : "Create")} card for: {value} {value2} cp" : "Cant be dismantled");
				}
			}
			else
			{
				switch (name)
				{
				case "InputButton":
					text = ((NavigationState.CurrentMenu == Menu.None) ? "Rename button/input" : "Search card input");
					break;
				case "Button0":
					text = TextExtractor.ExtractFirst(parent2.gameObject) + ", lower to higher";
					break;
				case "Button1":
					text = TextExtractor.ExtractFirst(parent2.gameObject) + ", higher to lower";
					break;
				default:
				{
					if (ParentLabels.TryGetValue(name, out var value3))
					{
						text = value3;
					}
					break;
				}
				}
			}
			string value4;
			if (name2 == "ChapterDuel(Clone)")
			{
				Transform child2 = TransformSearch.GetChild(parent, 4, "ChapterDuel/stars");
				if (child2 != null)
				{
					text = "Duel, " + TextExtractor.ExtractFirst(child2.gameObject) + " stars";
				}
			}
			else if (GrandparentLabels.TryGetValue(name2, out value4))
			{
				text = value4;
			}
			if (!string.IsNullOrEmpty(text))
			{
				Speech.SayItem(text);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[ColorContainer] " + ex.Message);
		}
	}
}
