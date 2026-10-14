// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppYgomGame.Dialog.CommonDialog;
using Il2CppYgomGame.Utility;
using Il2CppYgomSystem.ElementSystem;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public static class DialogDetector
{
	public static void Poll()
	{
		try
		{
			GameObject gameObject = GameObject.Find("UI/OverlayCanvas/DialogManager");
			if (gameObject == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				Transform child = gameObject.transform.GetChild(i);
				if (!child.gameObject.activeInHierarchy)
				{
					continue;
				}
				for (int j = 0; j < child.childCount; j++)
				{
					Transform child2 = child.GetChild(j);
					if (!child2.gameObject.activeInHierarchy || !child2.name.Contains("(Clone)"))
					{
						continue;
					}
					flag = true;
					string text;
					if (!child2.name.EndsWith("(Clone)"))
					{
						text = child2.name;
					}
					else
					{
						string name = child2.name;
						text = name.Substring(0, name.Length - 7);
					}
					string text2 = text;
					if (text2 == NavigationState.LastDialogTitle)
					{
						return;
					}
					if (text2 == "CardBrowserUI")
					{
						continue;
					}
					List<TextResult> list = TextExtractor.ExtractAll(child2.gameObject, new TextSearchOptions
					{
						ActiveOnly = true,
						FilterBanned = false,
						LogPrefix = "[Dialog-scan] " + child2.name + " |"
					});
					List<(string, string)> list2 = new List<(string, string)>();
					foreach (TextResult item in list)
					{
						list2.Add((item.Path, item.Text));
					}
					var (text3, text4) = ElementReader.ExtractTitleAndBody(list2);
					if (ElementReader.IsPlaceholder(text3))
					{
						Log.Write("[Dialog] Skipping placeholder for " + child2.name);
					}
					else if (!string.IsNullOrEmpty(text3) || !string.IsNullOrEmpty(text4))
					{
						string text5 = BuildItemListFromWidget(child2.gameObject) ?? BuildItemListBody(list);
						if (!string.IsNullOrEmpty(text5))
						{
							text4 = text5;
						}
						NavigationState.LastDialogTitle = text2;
						string text6 = ElementReader.FormatAnnouncement(text3, text4);
						Log.Write("[Dialog] Detected: " + child.name + "/" + child2.name);
						NavigationState.DialogJustAnnounced = true;
						Speech.AnnounceScreen(text6);
					}
					return;
				}
			}
			if (!flag)
			{
				NavigationState.LastDialogTitle = "";
			}
		}
		catch (Exception ex)
		{
			Log.Write("[DialogDetector] " + ex.Message);
		}
	}

	private static string BuildItemListFromWidget(GameObject root)
	{
		try
		{
			CommonDialogItemListWidget componentInChildren = root.GetComponentInChildren<CommonDialogItemListWidget>(includeInactive: true);
			if (componentInChildren == null)
			{
				return null;
			}
			EntryItemListData entryData = componentInChildren.m_EntryData;
			if (entryData?.contexts == null || entryData.contexts.Count == 0)
			{
				return null;
			}
			List<string> list = new List<string>();
			for (int i = 0; i < entryData.contexts.Count; i++)
			{
				EntryItemListData.Context context = entryData.contexts[i];
				if (context != null)
				{
					string text = ItemUtil.GetItemName(context.isPeriod, context.itemCategory, context.itemId);
					if (string.IsNullOrWhiteSpace(text))
					{
						text = ItemUtil.GetItemCategoryName(context.isPeriod, context.itemCategory, context.itemId);
					}
					if (!string.IsNullOrWhiteSpace(text))
					{
						list.Add((context.num > 1) ? $"{text} x{context.num}" : text);
					}
				}
			}
			return (list.Count > 0) ? string.Join(", ", list) : null;
		}
		catch (Exception ex)
		{
			Log.Write("[Dialog] ItemListWidget read error: " + ex.Message);
			return null;
		}
	}

	private static string BuildItemListBody(List<TextResult> texts)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (TextResult text3 in texts)
		{
			if (text3.Path.EndsWith("ItemNameText"))
			{
				list.Add(text3.Text);
			}
			else if (text3.Path.EndsWith("ItemNumText"))
			{
				list2.Add(text3.Text?.TrimStart('×', 'x', 'X', ' ') ?? "");
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		List<string> list3 = new List<string>();
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i];
			string text2 = ((i < list2.Count) ? list2[i] : "");
			list3.Add((!string.IsNullOrEmpty(text2)) ? (text + " x" + text2) : text);
		}
		return string.Join(", ", list3);
	}

	public static void AnnounceDialog(ViewController vc)
	{
		try
		{
			ElementObjectManager view = ScreenDetector.GetView(vc);
			if (view == null)
			{
				return;
			}
			List<TextResult> list = TextExtractor.ExtractAll(view.gameObject, TextSearchOptions.ActiveFiltered);
			if (list.Count == 0)
			{
				return;
			}
			List<(string, string)> list2 = new List<(string, string)>();
			foreach (TextResult item in list)
			{
				list2.Add((item.Path, item.Text));
			}
			var (text, text2) = ElementReader.ExtractTitleAndBody(list2);
			if (!string.IsNullOrEmpty(text))
			{
				string text3 = view.name;
				if (text3.EndsWith("(Clone)"))
				{
					string text4 = text3;
					text3 = text4.Substring(0, text4.Length - 7);
				}
				NavigationState.LastDialogTitle = text3;
				string text5 = ElementReader.FormatAnnouncement(text, text2);
				Log.Write($"[Dialog-VC] {vc.name}: title='{text}', body='{text2}'");
				NavigationState.DialogJustAnnounced = true;
				Speech.AnnounceScreen(text5);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[Dialog-VC] Error: " + ex.Message);
		}
	}
}
