// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.Regulation;
using Il2CppYgomSystem.ElementSystem;
using Il2CppYgomSystem.UI;
using Il2CppYgomSystem.UI.ElementWidget;
using UnityEngine;

namespace MillenniumEye;

public class RegulationHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		if (viewControllerName == "CardListBrowserRegulationFilter" || viewControllerName == "CardListBrowserRegulationFilterViewController")
		{
			return true;
		}
		return false;
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		string text = ScreenDetector.ReadGameHeaderText();
		if (!string.IsNullOrEmpty(text))
		{
			Speech.AnnounceScreen(text);
			return true;
		}
		return false;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		try
		{
			ViewController focusVC = ScreenDetector.GetFocusVC();
			if (focusVC == null)
			{
				return null;
			}
			CardListBrowserRegulationFilterViewController cardListBrowserRegulationFilterViewController = focusVC.TryCast<CardListBrowserRegulationFilterViewController>();
			if (cardListBrowserRegulationFilterViewController == null)
			{
				return null;
			}
			string text = HandleCardWidget(button, cardListBrowserRegulationFilterViewController);
			if (text != null)
			{
				return text;
			}
			string text2 = HandleFilterTab(button, cardListBrowserRegulationFilterViewController);
			if (text2 != null)
			{
				return text2;
			}
		}
		catch (Exception ex)
		{
			Log.Write("[RegulationHandler] " + ex.Message);
		}
		return null;
	}

	private static string HandleCardWidget(SelectionButton button, CardListBrowserRegulationFilterViewController regVC)
	{
		Il2CppSystem.Collections.Generic.Dictionary<GameObject, CardListBrowserRegulationFilterViewController.CardWidget> cardWidgetDic = regVC.m_CardWidgetDic;
		if (cardWidgetDic == null)
		{
			return null;
		}
		CardListBrowserRegulationFilterViewController.CardWidget cardWidget = null;
		Transform transform = button.transform;
		for (int i = 0; i < 5; i++)
		{
			if (!(transform != null))
			{
				break;
			}
			if (cardWidgetDic.TryGetValue(transform.gameObject, out var value))
			{
				cardWidget = value;
				break;
			}
			transform = transform.parent;
		}
		if (cardWidget == null)
		{
			return null;
		}
		int mrk = cardWidget.m_Mrk;
		if (mrk <= 0)
		{
			return null;
		}
		string text = Content.s_instance?.GetName(mrk);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		string text2 = text;
		int num = regVC.m_DisplayCardMrks?.Count ?? 0;
		int value2 = cardWidget.m_Idx + 1;
		if (num > 1)
		{
			text2 += $"\n{value2} of {num}";
		}
		return text2;
	}

	private static string HandleFilterTab(SelectionButton button, CardListBrowserRegulationFilterViewController regVC)
	{
		CardListBrowserRegulationFilterViewController.FilterFormWidget filterFormWidget = regVC.m_FilterFormWidget;
		if (filterFormWidget == null)
		{
			return null;
		}
		DirectionalTabGroup tabGroup = filterFormWidget.tabGroup;
		if (tabGroup == null)
		{
			return null;
		}
		Il2CppSystem.Collections.Generic.List<ElementObjectManager> tabEoms = tabGroup.m_TabEoms;
		Il2CppSystem.Collections.Generic.Dictionary<ElementObjectManager, ToggleWidget> tabWidgetMap = tabGroup.m_TabWidgetMap;
		if (tabEoms == null || tabWidgetMap == null)
		{
			return null;
		}
		int num = -1;
		for (int i = 0; i < tabEoms.Count; i++)
		{
			ElementObjectManager elementObjectManager = tabEoms[i];
			if (!(elementObjectManager == null) && tabWidgetMap.TryGetValue(elementObjectManager, out var value) && value?.button?.Pointer == button.Pointer)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return null;
		}
		System.Collections.Generic.List<TextResult> list = TextExtractor.ExtractAll(tabEoms[num].gameObject, new TextSearchOptions
		{
			ActiveOnly = false,
			FilterBanned = false
		});
		string text = null;
		foreach (TextResult item in list)
		{
			if (!string.IsNullOrEmpty(item.Text) && !item.Text.Contains("仮"))
			{
				text = item.Text;
				break;
			}
		}
		if (string.IsNullOrEmpty(text) && list.Count > 0)
		{
			text = list[0].Text;
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		int count = tabEoms.Count;
		if (count > 1)
		{
			text += $"\n{num + 1} of {count}";
		}
		return text;
	}
}
