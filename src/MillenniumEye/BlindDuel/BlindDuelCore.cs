// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppYgomGame.Duel;
using Il2CppYgomGame.Utility;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public class MillenniumEyeCore : MonoBehaviour
{
	public static MillenniumEyeCore Instance { get; private set; }

	public static PreviewData Preview { get; } = new PreviewData();

	public void Awake()
	{
		Instance = this;
		Log.Init();
		ScreenReader.Initialize();
		SDLController.Initialize();
		HandlerRegistry.Init();
	}

	public void OnApplicationQuit()
	{
		SDLController.Shutdown();
		ScreenReader.Shutdown();
	}

	public void Update()
	{
		SDLController.Update();
		if (NavigationState.IsInDuel && Application.isFocused)
			DuelEventDelivery.ObservePlayerInput(Input.anyKeyDown || SDLController.HasNewInput, Time.frameCount);
		if (NavigationState.IsInDuel)
		{
			ActivationConfirmHandler.HandleInput();
			bool flag = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			bool flag2 = flag && Input.GetKeyDown(KeyCode.DownArrow);
			bool flag3 = flag && Input.GetKeyDown(KeyCode.UpArrow);
			if (!flag2 && !flag3)
			{
				if (SDLController.RightStickDownTriggered)
				{
					flag2 = true;
				}
				else if (SDLController.RightStickUpTriggered)
				{
					flag3 = true;
				}
			}
			if (flag2)
			{
				List<string> cardDetailLines = DuelState.CardDetailLines;
				if (cardDetailLines != null)
				{
					int num = DuelState.CardDetailIndex + 1;
					if (num < cardDetailLines.Count)
					{
						DuelState.CardDetailIndex = num;
						Speech.SayImmediate(cardDetailLines[num]);
					}
				}
			}
			else if (flag3)
			{
				List<string> cardDetailLines2 = DuelState.CardDetailLines;
				if (cardDetailLines2 != null)
				{
					int num2 = DuelState.CardDetailIndex - 1;
					if (num2 >= 0)
					{
						DuelState.CardDetailIndex = num2;
						Speech.SayImmediate(cardDetailLines2[num2]);
					}
				}
			}
			if (Input.GetKeyDown(KeyCode.LeftAlt))
			{
				Preview.Clear();
			}
			bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			DuelFieldNav.HandleInput(shift);
		}
		if (Input.GetKeyDown(KeyCode.P) && (NavigationState.CurrentMenu == Menu.Deck || NavigationState.CurrentMenu == Menu.Shop))
		{
			try
			{
				if (NavigationState.CurrentMenu == Menu.Shop)
				{
					int hasTotalGem = ItemUtil.GetHasTotalGem();
					Speech.SayImmediate($"Gems: {hasTotalGem}");
				}
				else
				{
					int hasItemQuantity = ItemUtil.GetHasItemQuantity(3);
					int hasItemQuantity2 = ItemUtil.GetHasItemQuantity(4);
					int hasItemQuantity3 = ItemUtil.GetHasItemQuantity(5);
					int hasItemQuantity4 = ItemUtil.GetHasItemQuantity(6);
					Speech.SayImmediate($"Normal: {hasItemQuantity}, Rare: {hasItemQuantity2}, Super Rare: {hasItemQuantity3}, Ultra Rare: {hasItemQuantity4}");
				}
			}
			catch
			{
				Speech.SayImmediate("Points unavailable");
			}
		}
		if (DuelState.IsDuelLogOpen)
		{
			DuelLogReader.PollSelection();
		}
		if (!string.IsNullOrEmpty(DuelState.LastQueuedButtonText) && Time.frameCount > DuelState.LastQueuedButtonFrame)
		{
			if (DuelState.LastQueuedButtonInterrupt)
			{
				Speech.SayItem(DuelState.LastQueuedButtonText);
			}
			else
			{
				Speech.SayQueued(DuelState.LastQueuedButtonText);
			}
			DuelState.LastQueuedButtonText = null;
		}
		DialogDetector.Poll();
		ScreenDetector.Poll();
		SettingsHandler.PollSettingValue();
	}

	public void ReadCardDelayed()
	{
		string text = DuelHandler.ConsumeSelectionIndex();
		bool queued = PatchCardSelectionListSetTitle.ConsumeQueuedFlag();
		string suffix = ((!string.IsNullOrEmpty(text)) ? ("\n" + text) : null);
		CardReader.ReadAndSpeak(suffix, queued);
	}

	public void ReadItemPreviewDelayed()
	{
		CardReader.ReadPreviewAndSpeak();
	}

	public void ReadMDMarkupContent()
	{
		try
		{
			ViewController focusVC = ScreenDetector.GetFocusVC();
			if (focusVC == null)
			{
				Log.Write("[MDMarkup] No focused VC");
				return;
			}
			List<TextResult> list = TextExtractor.ExtractAll(focusVC.gameObject, new TextSearchOptions
			{
				ActiveOnly = true,
				FilterBanned = false,
				ExcludePathContaining = new List<string> { "SnapContent" },
				LogPrefix = "[MDMarkup]"
			});
			if (list.Count == 0)
			{
				Log.Write("[MDMarkup] No text found in focused VC");
				return;
			}
			List<string> list2 = new List<string>();
			foreach (TextResult item in list)
			{
				list2.Add(item.Text);
			}
			string text = string.Join(". ", list2);
			Log.Write("[MDMarkup] " + text);
			Speech.AnnounceScreen(text);
		}
		catch (Exception ex)
		{
			Log.Write("[MDMarkup] Error: " + ex.Message);
		}
	}
}
