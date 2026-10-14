// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System.Collections.Generic;
using Il2CppYgomSystem.UI;
using MelonLoader;

namespace MillenniumEye;

public static class Speech
{
	private static readonly List<string> _history = new List<string>();

	private static string _lastSpoken = "";

	private static string _lastHeader = "";

	private static SelectionButton _lastButton;

	public static IReadOnlyList<string> History => _history;

	public static void AnnounceScreen(string text)
	{
		if (!string.IsNullOrWhiteSpace(text))
		{
			text = TextUtil.StripTags(text).Trim();
			if (!string.IsNullOrWhiteSpace(text) && !(text == _lastHeader))
			{
				_lastHeader = text;
				_lastSpoken = "";
				Log.Write("[Screen] " + text);
				MelonLogger.Msg("screen: " + text);
				ScreenReader.Say(text);
				RecordHistory(text);
			}
		}
	}

	public static void SayItem(string text)
	{
		if (!string.IsNullOrWhiteSpace(text))
		{
			text = TextUtil.StripTags(text).Trim();
			if (!string.IsNullOrWhiteSpace(text) && !(text == _lastSpoken))
			{
				_lastSpoken = text;
				Log.Write("[Item] " + text);
				MelonLogger.Msg("item: " + text);
				ScreenReader.Say(text);
				RecordHistory(text);
			}
		}
	}

	public static void SayIndex(int current, int total)
	{
		if (total > 1)
		{
			string text = $"{current} of {total}";
			Log.Write("[Index] " + text);
			ScreenReader.SayQueued(text);
		}
	}

	public static void SayDescription(string text)
	{
		if (!string.IsNullOrWhiteSpace(text))
		{
			text = TextUtil.StripTags(text).Trim();
			if (!string.IsNullOrWhiteSpace(text))
			{
				Log.Write("[Desc] " + text);
				ScreenReader.SayQueued(text);
			}
		}
	}

	public static void SayQueued(string text)
	{
		if (!string.IsNullOrWhiteSpace(text))
		{
			text = TextUtil.StripTags(text).Trim();
			if (!string.IsNullOrWhiteSpace(text))
			{
				Log.Write("[Queued] " + text);
				MelonLogger.Msg("queued: " + text);
				ScreenReader.SayQueued(text);
				RecordHistory(text);
			}
		}
	}

	public static void SayImmediate(string text)
	{
		if (!string.IsNullOrWhiteSpace(text))
		{
			text = TextUtil.StripTags(text).Trim();
			if (!string.IsNullOrWhiteSpace(text))
			{
				_lastSpoken = text;
				Log.Write("[Immediate] " + text);
				MelonLogger.Msg("immediate: " + text);
				ScreenReader.Say(text);
				RecordHistory(text);
			}
		}
	}

	public static void SayTyping(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			ScreenReader.Say(text);
		}
	}

	public static bool IsSameButton(SelectionButton button, bool peek = false)
	{
		if (button == _lastButton)
		{
			return true;
		}
		if (!peek)
		{
			_lastButton = button;
		}
		return false;
	}

	public static void ResetDedup()
	{
		_lastSpoken = "";
	}

	public static void ResetButtonDedup()
	{
		_lastButton = null;
	}

	public static void Silence()
	{
		ScreenReader.Silence();
	}

	public static void RepeatLast()
	{
		ScreenReader.RepeatLast();
	}

	private static void RecordHistory(string text)
	{
		_history.Add(text);
	}
}
