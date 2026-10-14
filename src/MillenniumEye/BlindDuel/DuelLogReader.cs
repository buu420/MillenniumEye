// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppYgomGame.Duel;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

internal static class DuelLogReader
{
	private static readonly List<string> _entries = new List<string>();

	private static string _lastSpoken = "";

	private static SelectionItem _lastSelectedItem;

	public static void CaptureItem(GameObject eom, string type, int dataindex)
	{
		try
		{
			if (eom == null)
			{
				return;
			}
			List<TextResult> list = TextExtractor.ExtractAll(eom);
			if (list.Count != 0)
			{
				string text = string.Join(", ", list.ConvertAll((TextResult r) => r.Text));
				if (!string.IsNullOrWhiteSpace(text))
				{
					_entries.Add(text);
					Log.Write($"[DuelLog-Item] [{type}:{dataindex}] {text}");
				}
			}
		}
		catch
		{
		}
	}

	public static void ReadRecentEntries()
	{
		try
		{
			if (_entries.Count == 0)
			{
				Speech.SayQueued("No entries");
				Log.Write("[DuelLog-Read] No entries");
				return;
			}
			int num = Math.Max(0, _entries.Count - 8);
			List<string> list = new List<string>();
			for (int i = num; i < _entries.Count; i++)
			{
				list.Add(_entries[i]);
			}
			string text = string.Join(". ", list);
			Log.Write("[DuelLog-Read] " + text);
			Speech.SayQueued(text);
		}
		catch (Exception ex)
		{
			Log.Write("[DuelLog-Read] " + ex.Message);
		}
	}

	public static void InitScroll(DuelLogScrollView sv)
	{
		_lastSelectedItem = null;
		_lastSpoken = "";
		Log.Write("[DuelLog] Selection tracking initialized");
	}

	public static void PollSelection()
	{
		try
		{
			DuelLogScrollView logScrollView = DuelState.LogScrollView;
			if (logScrollView == null)
			{
				return;
			}
			Selector selector = logScrollView.selector;
			if (selector == null)
			{
				return;
			}
			SelectionItem selectedItem = selector.GetSelectedItem();
			if (selectedItem == null || selectedItem == _lastSelectedItem)
			{
				return;
			}
			_lastSelectedItem = selectedItem;
			_lastSpoken = "";
			GameObject gameObject = selectedItem.gameObject;
			if (gameObject == null)
			{
				return;
			}
			string text = null;
			string name = gameObject.name;
			if (name == "FaceIconL" || name == "FaceIconR")
			{
				int player = ((!(name == "FaceIconL")) ? 1 : 0);
				int lP = DuelClient.GetLP(player);
				string value = (DuelState.IsMyPlayer(player) ? "Your" : "Opponent");
				text = $"{value} LP: {lP}";
			}
			if (text == null && name.StartsWith("Card") && gameObject.transform.parent?.name == "CardRoot")
			{
				return;
			}
			if (text == null)
			{
				Transform transform = gameObject.transform;
				List<TextResult> list = TextExtractor.ExtractAll(gameObject);
				if (list.Count > 0)
				{
					text = string.Join(", ", list.ConvertAll((TextResult r) => r.Text));
				}
				if (string.IsNullOrWhiteSpace(text) && transform.parent != null)
				{
					list = TextExtractor.ExtractAll(transform.parent.gameObject);
					if (list.Count > 0)
					{
						text = string.Join(", ", list.ConvertAll((TextResult r) => r.Text));
					}
				}
				if (string.IsNullOrWhiteSpace(text) && transform.parent?.parent != null)
				{
					list = TextExtractor.ExtractAll(transform.parent.parent.gameObject);
					if (list.Count > 0)
					{
						text = string.Join(", ", list.ConvertAll((TextResult r) => r.Text));
					}
				}
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				Log.Write("[DuelLog-Poll] No text: " + name);
				return;
			}
			_lastSpoken = text;
			Log.Write("[DuelLog-Nav] " + text);
			Speech.SayItem(text);
		}
		catch (Exception ex)
		{
			Log.Write("[DuelLog-Poll] " + ex.Message);
		}
	}

	public static void Reset()
	{
		_entries.Clear();
		_lastSpoken = "";
		_lastSelectedItem = null;
	}
}
