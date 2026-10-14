// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using Il2CppYgomGame.Menu;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public class HomeHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		return viewControllerName == "Home";
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		Speech.AnnounceScreen("Home");
		return true;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		try
		{
			HeaderViewController instance = HeaderViewController.instance;
			if (instance != null && instance.BtnGem == button)
			{
				string text = TextExtractor.ExtractFirst(button.gameObject);
				if (!string.IsNullOrEmpty(text))
				{
					return text + " gems";
				}
			}
		}
		catch
		{
		}
		if (button.name == "ButtonPlayer")
		{
			Transform transform = TransformSearch.FindByName(button.transform, "TextLevel");
			if (transform != null)
			{
				string text2 = TextExtractor.ExtractFirst(transform.gameObject, TextSearchOptions.AllInclusive);
				if (!string.IsNullOrEmpty(text2))
				{
					string text3 = TextExtractor.ExtractFirst(button.gameObject);
					return text3 + ", level " + text2;
				}
			}
		}
		if (button.name == "SearchButton")
		{
			return "Add friend button";
		}
		if (button.name == "OpenToggle")
		{
			return TextExtractor.ExtractFirst(button.transform.parent?.gameObject);
		}
		if (button.name == "DuelShortcut")
		{
			return "Event banner";
		}
		if (button.name == "ButtonBanner")
		{
			ScrollRectPageSnap scrollRectPageSnap = button.transform.parent?.GetComponent<ScrollRectPageSnap>();
			if (scrollRectPageSnap != null)
			{
				return $"Topic banner, page {scrollRectPageSnap.hpage}";
			}
		}
		try
		{
			Transform transform2 = button.transform.parent?.parent?.parent?.parent?.parent?.parent;
			if (transform2 != null && transform2.name == "NotificationWidget")
			{
				return ReadNotificationText(button);
			}
		}
		catch
		{
		}
		return null;
	}

	internal static string ReadNotificationText(SelectionButton button)
	{
		Transform transform = button.transform.Find("TextBody");
		if (transform == null)
		{
			return null;
		}
		string text = TextExtractor.ExtractFirst(transform.gameObject, new TextSearchOptions
		{
			FilterBanned = false
		});
		Transform transform2 = button.transform.Find("BaseCategory");
		if (transform2 != null && transform2.gameObject.activeInHierarchy)
		{
			string text2 = TextExtractor.ExtractFirst(transform2.gameObject);
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + "\nStatus: " + text2;
			}
		}
		return text;
	}
}
