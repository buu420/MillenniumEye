// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.CardBrowser;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public class CardBrowserHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		if (viewControllerName == "CardBrowser" || viewControllerName == "CardListBrowser")
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
			if (text.Contains("Related") || text.Contains("related"))
			{
				return true;
			}
			Speech.AnnounceScreen(text);
			return true;
		}
		return false;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		if (button.name.Contains("Related") || button.name.Contains("related"))
		{
			return "";
		}
		string text = TextExtractor.ExtractFirst(button.gameObject);
		if (text != null && text.Contains("Related"))
		{
			return "";
		}
		try
		{
			ViewController focusVC = ScreenDetector.GetFocusVC();
			if (focusVC == null)
			{
				return null;
			}
			CardListBrowserViewController cardListBrowserViewController = focusVC.TryCast<CardListBrowserViewController>();
			if (cardListBrowserViewController != null)
			{
				return HandleCardListBrowser(button, cardListBrowserViewController);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[CardBrowserHandler] " + ex.Message);
		}
		return null;
	}

	private static string HandleCardListBrowser(SelectionButton button, CardListBrowserViewController vc)
	{
		Dictionary<GameObject, CardListBrowserViewController.CardWidget> cardWidgetDic = vc.m_CardWidgetDic;
		if (cardWidgetDic == null)
		{
			return null;
		}
		CardListBrowserViewController.CardWidget cardWidget = null;
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
		int num = vc.m_CardMrks?.Count ?? 0;
		int value2 = cardWidget.m_Idx + 1;
		if (num > 1)
		{
			text2 += $"\n{value2} of {num}";
		}
		return text2;
	}
}
