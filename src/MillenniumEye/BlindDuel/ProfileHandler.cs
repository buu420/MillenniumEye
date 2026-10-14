// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Il2CppYgomGame.Menu;
using Il2CppYgomSystem.ElementSystem;
using Il2CppYgomSystem.Extension;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public class ProfileHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		switch (viewControllerName)
		{
		case "Profile":
		case "PlayerProfile":
		case "ProfileData":
			return true;
		default:
			return false;
		}
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		if (viewControllerName == "ProfileData")
		{
			return HandleDataScreen();
		}
		return HandleProfileScreen();
	}

	private bool HandleProfileScreen()
	{
		string text = ScreenDetector.ReadGameHeaderText();
		string text2 = text ?? "Profile";
		try
		{
			ProfileViewController profileVC = GetProfileVC();
			if (profileVC == null)
			{
				Speech.AnnounceScreen(text2);
				return true;
			}
			ProfileCard profileCard = profileVC.profileCard;
			string text3 = null;
			string text4 = null;
			string text5 = null;
			string text6 = null;
			if (profileCard?.eom != null)
			{
				text4 = TryGetElement(profileCard.eom, ProfileCard.TXT_LEVEL_LABEL);
				text5 = TryGetElement(profileCard.eom, ProfileCard.TXT_FOLLOWER_LABEL);
				text6 = TryGetElement(profileCard.eom, ProfileCard.TXT_FOLLOW_LABEL);
				text3 = TryGetPlayerName(profileCard.eom);
			}
			if (!string.IsNullOrEmpty(text3))
			{
				text2 = text2 + ", " + text3;
			}
			if (!string.IsNullOrEmpty(text4))
			{
				text2 = ((!text4.StartsWith("LV", StringComparison.OrdinalIgnoreCase) && !text4.StartsWith("Level", StringComparison.OrdinalIgnoreCase)) ? (text2 + ", Level " + text4) : (text2 + ", " + text4));
			}
			long pcode = profileVC.pcode;
			if (pcode > 0)
			{
				text2 = text2 + ", ID: " + FormatPcode(pcode);
			}
			else if (profileCard?.eom != null)
			{
				string text7 = TryGetElement(profileCard.eom, ProfileCard.TXT_ID_LABEL);
				if (!string.IsNullOrEmpty(text7))
				{
					text2 = text2 + ", " + text7;
				}
			}
			if (!string.IsNullOrEmpty(text5))
			{
				text2 = text2 + ", " + text5;
			}
			if (!string.IsNullOrEmpty(text6))
			{
				text2 = text2 + ", " + text6;
			}
			if (profileCard?.eom != null)
			{
				List<string> list = new List<string>();
				for (int i = 1; i <= 4; i++)
				{
					string text8 = TryGetElement(profileCard.eom, $"TextTag{i}");
					if (!string.IsNullOrEmpty(text8) && text8 != "-")
					{
						list.Add(text8);
					}
				}
				if (list.Count > 0)
				{
					text2 = text2 + ", " + string.Join(", ", list);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[ProfileHandler] Screen error: " + ex.Message);
		}
		Speech.AnnounceScreen(text2);
		return true;
	}

	private bool HandleDataScreen()
	{
		string text = ScreenDetector.ReadGameHeaderText();
		Speech.AnnounceScreen(text ?? "Data");
		return true;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		try
		{
			if (ScreenDetector.GetFocusVC()?.TryCast<ProfileDataViewController>() != null)
			{
				return null;
			}
			string text = TextExtractor.ExtractFirst(button.gameObject);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			Transform parent = button.transform.parent;
			while (parent != null)
			{
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < parent.childCount; i++)
				{
					Transform child = parent.GetChild(i);
					if (child == null || !child.gameObject.activeInHierarchy)
					{
						continue;
					}
					SelectionButton componentInChildren = child.GetComponentInChildren<SelectionButton>(includeInactive: true);
					if (!(componentInChildren == null))
					{
						num2++;
						if (componentInChildren == button)
						{
							num = num2;
						}
					}
				}
				if (num2 > 1 && num > 0)
				{
					return $"{text}, {num} of {num2}";
				}
				parent = parent.parent;
			}
			return text;
		}
		catch (Exception ex)
		{
			Log.Write("[ProfileHandler] Button error: " + ex.Message);
			return null;
		}
	}

	private static string TryGetElement(ElementObjectManager eom, string label)
	{
		if (string.IsNullOrEmpty(label))
		{
			return null;
		}
		try
		{
			return ElementReader.GetElementText(eom, label);
		}
		catch
		{
			return null;
		}
	}

	private static string TryGetPlayerName(ElementObjectManager eom)
	{
		try
		{
			GameObject gameObject = eom?.gameObject;
			if (gameObject == null)
			{
				return null;
			}
			PlatformPlayerNameGroup componentInChildren = gameObject.GetComponentInChildren<PlatformPlayerNameGroup>(includeInactive: true);
			if (componentInChildren != null)
			{
				return componentInChildren.GetYmdPlayerName();
			}
		}
		catch (Exception ex)
		{
			Log.Write("[Profile] Name lookup error: " + ex.Message);
		}
		return null;
	}

	private static string FormatPcode(long pcode)
	{
		try
		{
			return pcode.ToPcodeFormatString();
		}
		catch
		{
			string text = pcode.ToString();
			if (text.Length == 9)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
				defaultInterpolatedStringHandler.AppendFormatted(text.Substring(0, 3));
				defaultInterpolatedStringHandler.AppendLiteral("-");
				defaultInterpolatedStringHandler.AppendFormatted(text.Substring(3, 3));
				defaultInterpolatedStringHandler.AppendLiteral("-");
				string text2 = text;
				defaultInterpolatedStringHandler.AppendFormatted(text2.Substring(6, text2.Length - 6));
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			return text;
		}
	}

	private static ProfileViewController GetProfileVC()
	{
		try
		{
			return ScreenDetector.GetFocusVC()?.TryCast<ProfileViewController>();
		}
		catch
		{
			return null;
		}
	}
}
