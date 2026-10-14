// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System.Collections.Generic;
using Il2CppYgomGame.Duel;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public static class DuelState
{
	public static List<CardRoot> Cards { get; } = new List<CardRoot>();

	public static bool IsShowingResult { get; set; }

	public static bool IsDuelLogOpen { get; set; }

	public static DuelLogScrollView LogScrollView { get; set; }

	public static DuelLogController LogController { get; set; }

	public static bool HasPhaseStarted { get; set; }

	public static bool InSelectionList { get; set; }

	public static bool MessageJustAnnounced { get; set; }

	public static bool HasPendingSelection { get; set; }

	public static string LastQueuedButtonText { get; set; }

	public static int LastQueuedButtonFrame { get; set; }

	public static bool LastQueuedButtonInterrupt { get; set; }

	public static bool SuppressNextFieldFocus { get; set; }

	private static readonly DuelDetailBuffer Details = new DuelDetailBuffer();

	public static List<string> CardDetailLines { get => Details.Lines; set => Details.Replace(value); }

	public static int CardDetailIndex { get => Details.Index; set => Details.Index = value; }

	public static SelectionButton DeferredSelectionButton { get; set; }

	public static (int player, int position, int viewIndex)? DeferredFieldFocus { get; set; }

	public static int LastBrowsePlayer { get; set; } = -1;

	public static int LastBrowsePosition { get; set; } = -1;

	public static int BrowseIndex { get; set; } = -1;

	public static int BrowseDirection { get; set; } = 1;

	public static int LastBrowseLogicalIdx { get; set; } = -1;

	public static void Clear()
	{
		Cards.Clear();
		IsShowingResult = false;
		IsDuelLogOpen = false;
		LogScrollView = null;
		LogController = null;
		HasPhaseStarted = false;
		InSelectionList = false;
		MessageJustAnnounced = false;
		HasPendingSelection = false;
		SuppressNextFieldFocus = false;
		CardDetailLines = null;
		CardDetailIndex = 0;
		LastQueuedButtonText = null;
		DeferredSelectionButton = null;
		DeferredFieldFocus = null;
		LastBrowsePlayer = -1;
		LastBrowsePosition = -1;
		BrowseIndex = -1;
		BrowseDirection = 1;
		LastBrowseLogicalIdx = -1;
	}

	public static CardRoot FindCardAtPosition(Vector3 position)
	{
		return Cards.Find((CardRoot c) => c.cardLocator.pos == position);
	}

	public static int GetMyPlayerNum()
	{
		try
		{
			DuelClient instance = DuelClient.instance;
			if (instance != null)
			{
				EngineInitializer engineInitializer = instance.engineInitializer;
				if (engineInitializer != null)
				{
					return engineInitializer.myPlayerNum;
				}
			}
		}
		catch
		{
		}
		return 0;
	}

	public static bool IsMyPlayer(int player)
	{
		return player == GetMyPlayerNum();
	}

	public static bool IsOpponentTeam(bool team)
	{
		int myPlayerNum = GetMyPlayerNum();
		return team ? (myPlayerNum != 0) : (myPlayerNum == 0);
	}
}
