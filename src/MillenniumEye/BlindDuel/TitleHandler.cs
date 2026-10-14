// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppYgomSystem.ElementSystem;
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

public class TitleHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		return viewControllerName == "Title";
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		ViewController focusVC = ScreenDetector.GetFocusVC();
		ElementObjectManager elementObjectManager = ((focusVC != null) ? ScreenDetector.GetView(focusVC) : null);
		string text = "Title screen";
		if (elementObjectManager != null)
		{
			string text2 = ElementReader.GetElementText(elementObjectManager, "CodeVer");
			if (!string.IsNullOrWhiteSpace(text2))
			{
				if (text2.StartsWith("Ver:", StringComparison.OrdinalIgnoreCase))
				{
					string text3 = text2;
					text2 = text3.Substring(4, text3.Length - 4).Trim();
				}
				text = text + ". Version " + text2;
			}
			string elementText = ElementReader.GetElementText(elementObjectManager, "PlayerID");
			if (!string.IsNullOrWhiteSpace(elementText))
			{
				text = text + ". Player ID: " + elementText;
			}
		}
		Speech.AnnounceScreen(text);
		if (elementObjectManager != null)
		{
			string text4 = ElementReader.GetElementText(elementObjectManager, "TextGameStart") ?? ElementReader.GetElementText(elementObjectManager, "PressMsgText");
			if (!string.IsNullOrWhiteSpace(text4))
			{
				Speech.SayQueued(text4);
			}
		}
		return true;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		string name = button.name;
		if (name.Contains("Start", StringComparison.OrdinalIgnoreCase))
		{
			return "";
		}
		return null;
	}
}
