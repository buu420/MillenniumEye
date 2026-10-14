// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

[HarmonyPatch(typeof(CardSelectionList), "SetTitle")]
internal class PatchCardSelectionListSetTitle
{
	private static bool _nextReadQueued;

	private static string _lastTitle = "";

	public static void HandleTitle(string cleaned)
	{
		if (!(cleaned == _lastTitle))
		{
			_lastTitle = cleaned;
			Log.Write("[CardSelectionList] " + cleaned);
			DuelState.MessageJustAnnounced = true;
			Speech.SayImmediate(cleaned);
			_nextReadQueued = true;
			DuelState.HasPendingSelection = false;
			QueueDeferredItem();
			FieldFocusHandler.ClearLastFocus();
			if (!string.IsNullOrEmpty(DuelState.LastQueuedButtonText))
			{
				Speech.SayQueued(DuelState.LastQueuedButtonText);
				DuelState.LastQueuedButtonText = null;
			}
		}
	}

	private static void QueueDeferredItem()
	{
		SelectionButton deferredSelectionButton = DuelState.DeferredSelectionButton;
		if (deferredSelectionButton != null)
		{
			DuelState.DeferredSelectionButton = null;
			try
			{
				IMenuHandler current = HandlerRegistry.Current;
				if (current != null)
				{
					string text = current.OnButtonFocused(deferredSelectionButton);
					if (text == null)
					{
						text = TextExtractor.ExtractFirst(deferredSelectionButton.gameObject);
					}
					if (!string.IsNullOrWhiteSpace(text) && text != "")
					{
						Speech.SayQueued(text);
					}
				}
				return;
			}
			catch (Exception ex)
			{
				Log.Write("[QueueDeferred] Button: " + ex.Message);
				return;
			}
		}
		(int, int, int)? deferredFieldFocus = DuelState.DeferredFieldFocus;
		if (deferredFieldFocus.HasValue)
		{
			DuelState.DeferredFieldFocus = null;
			try
			{
				var (player, position, viewIndex) = deferredFieldFocus.Value;
				FieldFocusHandler.SpeakDeferredFocus(player, position, viewIndex);
			}
			catch (Exception ex2)
			{
				Log.Write("[QueueDeferred] Field: " + ex2.Message);
			}
		}
	}

	[HarmonyPostfix]
	private static void Postfix(string title)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(title))
			{
				string text = TextUtil.StripTags(title);
				if (!string.IsNullOrWhiteSpace(text))
				{
					HandleTitle(text);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchCardSelectionListSetTitle] " + ex.Message);
		}
	}

	public static bool ConsumeQueuedFlag()
	{
		bool nextReadQueued = _nextReadQueued;
		_nextReadQueued = false;
		return nextReadQueued;
	}

	public static void ResetDedup()
	{
		_lastTitle = "";
	}
}
