// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System.Collections.Generic;

namespace MillenniumEye;

public static class NavigationState
{
	public static readonly Dictionary<string, Menu> VCNameToMenu = new Dictionary<string, Menu>
	{
		{
			"DuelMenu",
			Menu.Duel
		},
		{
			"DeckMenu",
			Menu.Deck
		},
		{
			"SoloMenu",
			Menu.Solo
		},
		{
			"ShopMenu",
			Menu.Shop
		},
		{
			"MissionMenu",
			Menu.Missions
		},
		{
			"NotificationMenu",
			Menu.Notifications
		},
		{
			"GameSettingMenu",
			Menu.Settings
		},
		{
			"DuelPassMenu",
			Menu.DuelPass
		}
	};

	public static readonly Dictionary<string, string> ScreenHeaderToMenu = new Dictionary<string, string>
	{
		{ "DUEL", "Duel" },
		{ "DECK", "Deck" },
		{ "SOLO", "Solo" },
		{ "SHOP", "Shop" },
		{ "MISSION", "Missions" },
		{ "Notifications", "Notifications" },
		{ "Game Settings", "Settings" },
		{ "Duel Pass", "Duel Pass" }
	};

	public static Menu CurrentMenu { get; set; } = Menu.None;

	public static string LastFocusVCName { get; set; } = "";

	public static string LastDialogTitle { get; set; } = "";

	public static bool IsInDuel { get; set; }

	public static bool DialogJustAnnounced { get; set; }

	public static bool ScreenJustAnnounced { get; set; }
}
