// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTMPro;
using UnityEngine;

namespace MillenniumEye;

public static class TextExtractor
{
	public static List<TextResult> ExtractAll(GameObject root, TextSearchOptions options = null)
	{
		if (options == null)
		{
			options = TextSearchOptions.Default;
		}
		List<TextResult> list = new List<TextResult>();
		if (root == null)
		{
			return list;
		}
		Il2CppArrayBase<TMP_Text> componentsInChildren = root.GetComponentsInChildren<TMP_Text>(!options.ActiveOnly);
		if (componentsInChildren == null)
		{
			return list;
		}
		foreach (TMP_Text item in componentsInChildren)
		{
			if (item == null || (options.ActiveOnly && !item.gameObject.activeInHierarchy))
			{
				continue;
			}
			string text = item.text;
			if (string.IsNullOrWhiteSpace(text))
			{
				continue;
			}
			string text2 = TextUtil.StripTags(text).Trim();
			if (string.IsNullOrWhiteSpace(text2))
			{
				continue;
			}
			string text3 = (item.transform.parent?.name ?? "root") + "/" + item.name;
			if (PassesPathFilter(text3, options) && (!options.FilterBanned || !TextUtil.IsBannedText(item.gameObject, text2, options.HasMenuContext)))
			{
				list.Add(new TextResult(text3, text2));
				if (options.LogPrefix != null)
				{
					Log.Write($"{options.LogPrefix} {text3} = {text2}");
				}
			}
		}
		return list;
	}

	public static string ExtractFirst(GameObject root, TextSearchOptions options = null)
	{
		List<TextResult> list = ExtractAll(root, options);
		return (list.Count > 0) ? list[0].Text : null;
	}

	public static string ExtractFirst(string gameObjectPath, TextSearchOptions options = null)
	{
		GameObject gameObject = GameObject.Find(gameObjectPath);
		return (gameObject != null) ? ExtractFirst(gameObject, options) : null;
	}

	public static List<TextResult> ExtractAll(string gameObjectPath, TextSearchOptions options = null)
	{
		GameObject gameObject = GameObject.Find(gameObjectPath);
		return (gameObject != null) ? ExtractAll(gameObject, options) : new List<TextResult>();
	}

	private static bool PassesPathFilter(string path, TextSearchOptions options)
	{
		if (options.ExcludePathContaining != null)
		{
			foreach (string item in options.ExcludePathContaining)
			{
				if (path.Contains(item, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
		}
		if (options.IncludePathContaining != null)
		{
			foreach (string item2 in options.IncludePathContaining)
			{
				if (path.Contains(item2, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}
}
