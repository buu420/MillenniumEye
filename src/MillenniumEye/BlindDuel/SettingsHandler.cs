// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using Il2CppYgomGame.Settings;
using Il2CppYgomSystem.UI;
using Il2CppYgomSystem.YGomTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MillenniumEye;

public class SettingsHandler : IMenuHandler
{
	private static SelectionButton _lastValueButton;

	private static float _lastSliderValue;

	private static string _lastModeText;

	public bool CanHandle(string viewControllerName)
	{
		if (viewControllerName == "SettingMenuViewController" || viewControllerName == "GameSettingMenu")
		{
			return true;
		}
		return false;
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		string text = ScreenDetector.ReadGameHeaderText();
		Speech.AnnounceScreen((!string.IsNullOrWhiteSpace(text)) ? text : "Game Settings");
		return true;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		if (button.name == "CancelButton")
		{
			return null;
		}
		SettingMenuViewController.PageInfo pageInfo = TryGetPageInfo(button, out var index, out var total);
		if (pageInfo != null)
		{
			return FormatCategory(pageInfo, index, total);
		}
		return FormatSetting(button);
	}

	public static void PollSettingValue()
	{
		try
		{
			if (!(HandlerRegistry.Current is SettingsHandler))
			{
				_lastValueButton = null;
				return;
			}
			SelectionButton selectionButton = SelectorManager.currentItem?.TryCast<SelectionButton>();
			if (selectionButton == null || selectionButton.WasCollected)
			{
				_lastValueButton = null;
				return;
			}
			Slider componentInChildren = selectionButton.GetComponentInChildren<Slider>();
			ExtendedTextMeshProUGUI extendedTextMeshProUGUI = FindModeText(selectionButton);
			if (componentInChildren == null && extendedTextMeshProUGUI == null)
			{
				_lastValueButton = null;
				return;
			}
			if (selectionButton != _lastValueButton)
			{
				_lastValueButton = selectionButton;
				_lastSliderValue = componentInChildren?.value ?? 0f;
				_lastModeText = extendedTextMeshProUGUI?.text?.Trim() ?? "";
				return;
			}
			if (componentInChildren != null)
			{
				int num = (int)Math.Round(componentInChildren.value);
				int num2 = (int)Math.Round(_lastSliderValue);
				if (num != num2)
				{
					_lastSliderValue = componentInChildren.value;
					int value = (int)Math.Round(componentInChildren.maxValue);
					Speech.SayImmediate($"{num} of {value}");
				}
			}
			if (extendedTextMeshProUGUI != null)
			{
				string text = extendedTextMeshProUGUI.text?.Trim() ?? "";
				if (!string.IsNullOrEmpty(text) && text != _lastModeText)
				{
					_lastModeText = text;
					Speech.SayImmediate(text);
				}
			}
		}
		catch
		{
		}
	}

	private static ExtendedTextMeshProUGUI FindModeText(SelectionButton btn)
	{
		Il2CppArrayBase<ExtendedTextMeshProUGUI> componentsInChildren = btn.GetComponentsInChildren<ExtendedTextMeshProUGUI>();
		if (componentsInChildren == null)
		{
			return null;
		}
		foreach (ExtendedTextMeshProUGUI item in componentsInChildren)
		{
			if (item != null && item.name == "ModeText" && item.gameObject.activeInHierarchy)
			{
				return item;
			}
		}
		return null;
	}

	private static SettingMenuViewController.PageInfo TryGetPageInfo(SelectionButton button, out int index, out int total)
	{
		index = 0;
		total = 0;
		SettingMenuViewController settingMenuViewController = ScreenDetector.GetFocusVC()?.TryCast<SettingMenuViewController>();
		if (settingMenuViewController?.pageInfo == null)
		{
			return null;
		}
		int instanceID = button.gameObject.GetInstanceID();
		System.Collections.Generic.List<(int, SettingMenuViewController.PageInfo)> list = new System.Collections.Generic.List<(int, SettingMenuViewController.PageInfo)>();
		foreach (Il2CppSystem.Collections.Generic.KeyValuePair<SettingMenuViewController.Page, SettingMenuViewController.PageInfo> item in settingMenuViewController.pageInfo)
		{
			SettingMenuViewController.PageInfo value = item.Value;
			if (!(value?.button == null) && value.button.gameObject.activeInHierarchy)
			{
				list.Add(((int)item.Key, value));
			}
		}
		list.Sort(((int order, SettingMenuViewController.PageInfo info) a, (int order, SettingMenuViewController.PageInfo info) b) => a.order.CompareTo(b.order));
		total = list.Count;
		for (int num = 0; num < list.Count; num++)
		{
			if (list[num].Item2.button.gameObject.GetInstanceID() == instanceID)
			{
				index = num + 1;
				return list[num].Item2;
			}
		}
		return null;
	}

	private static string FormatCategory(SettingMenuViewController.PageInfo info, int index, int total)
	{
		string text = ReadCategoryText(info);
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}
		if (total > 1 && index > 0)
		{
			return $"{text}, {index} of {total}";
		}
		return text;
	}

	private static string ReadCategoryText(SettingMenuViewController.PageInfo info)
	{
		string a = ReadTMPTextInclusive(info.on);
		string b = ReadTMPTextInclusive(info.off);
		return PickLocalized(a, b);
	}

	private static string ReadTMPTextInclusive(GameObject root)
	{
		if (root == null)
		{
			return null;
		}
		return root.GetComponentInChildren<TMP_Text>(includeInactive: true)?.text?.Trim();
	}

	private static string PickLocalized(string a, string b)
	{
		bool flag = IsPlaceholder(a);
		bool flag2 = IsPlaceholder(b);
		if (!flag & flag2)
		{
			return a;
		}
		if (flag && !flag2)
		{
			return b;
		}
		return string.IsNullOrEmpty(a) ? b : a;
	}

	private static bool IsPlaceholder(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return true;
		}
		return text.Contains("(仮)");
	}

	private static string FormatSetting(SelectionButton button)
	{
		string text = null;
		string text2 = null;
		System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
		Il2CppArrayBase<ExtendedTextMeshProUGUI> componentsInChildren = button.GetComponentsInChildren<ExtendedTextMeshProUGUI>();
		if (componentsInChildren != null)
		{
			foreach (ExtendedTextMeshProUGUI item in componentsInChildren)
			{
				if (item == null || !item.gameObject.activeInHierarchy)
				{
					continue;
				}
				string text3 = item.text?.Trim();
				if (!string.IsNullOrEmpty(text3))
				{
					if (item.name == "ModeText")
					{
						text2 = text3;
					}
					else if (text == null)
					{
						text = text3;
					}
					else
					{
						list.Add(text3);
					}
				}
			}
		}
		string text4 = null;
		Slider componentInChildren = button.GetComponentInChildren<Slider>();
		if (componentInChildren != null)
		{
			int value = (int)Math.Round(componentInChildren.value);
			int value2 = (int)Math.Round(componentInChildren.maxValue);
			text4 = $"{value} of {value2}";
		}
		else if (!string.IsNullOrEmpty(text2))
		{
			text4 = text2;
		}
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}
		string text5 = text;
		if (!string.IsNullOrEmpty(text4))
		{
			text5 = text5 + ", " + text4;
		}
		foreach (string item2 in list)
		{
			text5 = text5 + ", " + item2;
		}
		var (num, num2) = CountSettingIndex(button);
		if (num2 > 1 && num > 0)
		{
			text5 += $", {num} of {num2}";
		}
		return text5;
	}

	private static (int index, int total) CountSettingIndex(SelectionButton button)
	{
		Transform transform = button.transform;
		Transform transform2 = button.transform.parent;
		if (transform2 == null)
		{
			return (index: 0, total: 0);
		}
		Transform transform3 = transform2;
		int num = -1;
		for (int i = 0; i < 3; i++)
		{
			if (!(transform3 != null))
			{
				break;
			}
			int num2 = 0;
			for (int j = 0; j < transform3.childCount; j++)
			{
				Transform child = transform3.GetChild(j);
				if (child.gameObject.activeInHierarchy && child.GetComponentInChildren<SelectionButton>() != null)
				{
					num2++;
				}
			}
			if (num2 > num)
			{
				num = num2;
				transform2 = transform3;
				Transform transform4 = button.transform;
				for (int k = 0; k < i; k++)
				{
					transform4 = transform4.parent;
				}
				transform = transform4;
			}
			transform3 = transform3.parent;
		}
		if (num < 2)
		{
			return (index: 0, total: 0);
		}
		int item = 0;
		int num3 = 0;
		for (int l = 0; l < transform2.childCount; l++)
		{
			Transform child2 = transform2.GetChild(l);
			if (child2.gameObject.activeInHierarchy && !(child2.GetComponentInChildren<SelectionButton>() == null))
			{
				num3++;
				if (child2 == transform)
				{
					item = num3;
				}
			}
		}
		return (index: item, total: num3);
	}
}
