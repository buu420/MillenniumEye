// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTMPro;
using Il2CppYgomSystem.ElementSystem;
using UnityEngine;

namespace MillenniumEye;

public static class ElementReader
{
	private static readonly HashSet<string> ContainerLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Root", "RootContent", "RootBottom" };

	public static List<(string label, string text)> ReadElements(ElementObjectManager eom, string logPrefix = null)
	{
		List<(string, string)> list = new List<(string, string)>();
		if (eom == null)
		{
			return list;
		}
		try
		{
			Il2CppReferenceArray<ElementObject> serializedElements = eom.serializedElements;
			if (serializedElements == null || serializedElements.Length == 0)
			{
				return list;
			}
			foreach (ElementObject item in serializedElements)
			{
				if (item == null)
				{
					continue;
				}
				string label = item.label;
				if (string.IsNullOrEmpty(label) || ContainerLabels.Contains(label))
				{
					continue;
				}
				GameObject gameObject = item.gameObject;
				if (gameObject == null)
				{
					continue;
				}
				Il2CppArrayBase<TMP_Text> componentsInChildren = gameObject.GetComponentsInChildren<TMP_Text>(includeInactive: true);
				if (componentsInChildren == null)
				{
					continue;
				}
				foreach (TMP_Text item2 in componentsInChildren)
				{
					if (item2 == null)
					{
						continue;
					}
					string text = item2.text;
					if (string.IsNullOrWhiteSpace(text))
					{
						continue;
					}
					string text2 = TextUtil.StripTags(text).Trim();
					if (!string.IsNullOrWhiteSpace(text2))
					{
						list.Add((label, text2));
						if (logPrefix != null)
						{
							Log.Write($"{logPrefix} [{label}] {item2.transform.parent?.name}/{item2.name} = {text2}");
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[ElementReader] Error: " + ex.Message);
		}
		return list;
	}

	public static string GetElementText(ElementObjectManager eom, string elementLabel)
	{
		if (eom == null || string.IsNullOrEmpty(elementLabel))
		{
			return null;
		}
		try
		{
			GameObject element = eom.GetElement(elementLabel);
			if (element == null)
			{
				return null;
			}
			TMP_Text componentInChildren = element.GetComponentInChildren<TMP_Text>(includeInactive: true);
			if (componentInChildren == null)
			{
				return null;
			}
			string text = componentInChildren.text;
			return string.IsNullOrWhiteSpace(text) ? null : TextUtil.StripTags(text).Trim();
		}
		catch (Exception ex)
		{
			Log.Write("[ElementReader] GetElementText(" + elementLabel + "): " + ex.Message);
			return null;
		}
	}

	public static (string title, string body) ExtractTitleAndBody(List<(string label, string text)> elements)
	{
		string text = null;
		string text2 = null;
		foreach (var element in elements)
		{
			string item = element.label;
			string item2 = element.text;
			string text3 = item.ToLower();
			if (!text3.StartsWith("button") && !text3.Contains("cancel"))
			{
				if (text == null && (text3.Contains("title") || text3.Contains("header") || text3.Contains("start")))
				{
					text = item2;
				}
				else if (text2 == null && (text3.Contains("message") || text3.Contains("desc") || text3.Contains("info") || text3.Contains("body")))
				{
					text2 = item2;
				}
			}
		}
		if (text != null && text2 == null)
		{
			bool flag = false;
			foreach (var (text4, text5) in elements)
			{
				if (text5 == text)
				{
					flag = true;
				}
				else if (flag && text5.Length >= 30)
				{
					string text6 = text4.ToLower();
					if (!text6.StartsWith("button") && !text6.Contains("cancel"))
					{
						text2 = text5;
						break;
					}
				}
			}
		}
		if (text == null && text2 == null)
		{
			foreach (var element2 in elements)
			{
				string item3 = element2.label;
				string item4 = element2.text;
				string text7 = item3.ToLower();
				if (!text7.StartsWith("button") && !text7.Contains("cancel") && !IsPlaceholder(item4))
				{
					if (text != null)
					{
						text2 = item4;
						break;
					}
					text = item4;
				}
			}
		}
		return (title: text, body: text2);
	}

	public static bool IsPlaceholder(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return true;
		}
		return text.Length > 30 && !text.Contains(' ') && !text.Contains('\n');
	}

	public static string FormatAnnouncement(string title, string body)
	{
		if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(body))
		{
			return "Dialog. " + title + ". " + body;
		}
		return "Dialog. " + (string.IsNullOrEmpty(title) ? body : title);
	}
}
