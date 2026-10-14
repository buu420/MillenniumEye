// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTMPro;
using Il2CppYgomGame.Duelpass;
using Il2CppYgomGame.Utility;
using Il2CppYgomSystem.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MillenniumEye;

public class DuelPassHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		switch (viewControllerName)
		{
		case "DuelPassMenu":
		case "DuelPass":
		case "DuelPassRewardList":
			return true;
		default:
			return false;
		}
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		return false;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		DuelpassRewardButtonWidget duelpassRewardButtonWidget = FindRewardWidget(button);
		if (duelpassRewardButtonWidget == null)
		{
			try
			{
				string text = null;
				string text2 = null;
				Transform transform = button.transform;
				for (int i = 0; i < 5; i++)
				{
					if (!(transform != null))
					{
						break;
					}
					if (string.IsNullOrEmpty(text))
					{
						Il2CppArrayBase<TMP_Text> componentsInChildren = transform.GetComponentsInChildren<TMP_Text>(includeInactive: true);
						if (componentsInChildren != null)
						{
							foreach (TMP_Text item in componentsInChildren)
							{
								try
								{
									string text3 = item?.text?.Trim();
									if (!string.IsNullOrEmpty(text3) && text3.StartsWith("×"))
									{
										text = text3;
									}
								}
								catch
								{
								}
							}
						}
					}
					if (string.IsNullOrEmpty(text2))
					{
						Il2CppArrayBase<Image> componentsInChildren2 = transform.GetComponentsInChildren<Image>(includeInactive: true);
						if (componentsInChildren2 != null)
						{
							foreach (Image item2 in componentsInChildren2)
							{
								try
								{
									if (item2?.sprite == null)
									{
										continue;
									}
									string text4 = item2.sprite.name ?? "";
									if (!text4.StartsWith("ConsumeIcon"))
									{
										continue;
									}
									string s = text4.Substring("ConsumeIcon".Length);
									if (int.TryParse(s, out var result) && result > 0)
									{
										try
										{
											text2 = ItemUtil.GetItemName(isPerild: false, 1, result);
										}
										catch
										{
										}
									}
									if (string.IsNullOrEmpty(text2))
									{
										text2 = text4;
									}
									break;
								}
								catch
								{
								}
							}
						}
					}
					if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
					{
						break;
					}
					transform = transform.parent;
				}
				string text5 = FindPassType(button);
				string text6 = FindGrade(button);
				string text7 = "";
				if (!string.IsNullOrEmpty(text6))
				{
					text7 = "Grade " + text6;
				}
				if (!string.IsNullOrEmpty(text2))
				{
					text7 = text7 + ((text7.Length > 0) ? ", " : "") + text2;
				}
				if (!string.IsNullOrEmpty(text))
				{
					text7 = text7 + ((text7.Length > 0) ? " " : "") + text;
				}
				if (!string.IsNullOrEmpty(text5))
				{
					text7 = text7 + ", " + text5 + " Pass";
				}
				if (!string.IsNullOrEmpty(text7))
				{
					return text7;
				}
			}
			catch (Exception ex)
			{
				Log.Write("[DuelPass] fallback: " + ex.Message);
			}
			return null;
		}
		try
		{
			string text8 = ReadText(duelpassRewardButtonWidget.countText) ?? ReadText(duelpassRewardButtonWidget.rewardNumText);
			string text9 = null;
			try
			{
				GameObject rewardThumbHolder = duelpassRewardButtonWidget.rewardThumbHolder;
				if (rewardThumbHolder != null)
				{
					text9 = ReadItemFromSprites(rewardThumbHolder);
				}
			}
			catch
			{
			}
			if (string.IsNullOrEmpty(text9))
			{
				try
				{
					GameObject rewardThumbHolder2 = duelpassRewardButtonWidget.rewardThumbHolder;
					if (rewardThumbHolder2 != null)
					{
						text9 = TextExtractor.ExtractFirst(rewardThumbHolder2);
					}
				}
				catch
				{
				}
			}
			string text10 = FindPassType(button);
			string text11 = FindGrade(button);
			string text12 = "";
			if (!string.IsNullOrEmpty(text11))
			{
				text12 = "Grade " + text11;
			}
			if (!string.IsNullOrEmpty(text9))
			{
				text12 = text12 + ((text12.Length > 0) ? ", " : "") + text9;
			}
			if (!string.IsNullOrEmpty(text8))
			{
				text12 = text12 + ((text12.Length > 0) ? " " : "") + text8;
			}
			if (!string.IsNullOrEmpty(text10))
			{
				text12 = text12 + ", " + text10 + " Pass";
			}
			return (!string.IsNullOrEmpty(text12)) ? text12 : null;
		}
		catch (Exception ex2)
		{
			Log.Write("[DuelPass] " + ex2.Message);
			return null;
		}
	}

	private static string ReadText(TMP_Text tmp)
	{
		try
		{
			if (tmp == null)
			{
				return null;
			}
			string text = tmp.text?.Trim();
			return string.IsNullOrEmpty(text) ? null : text;
		}
		catch
		{
			return null;
		}
	}

	private static string ReadItemFromSprites(GameObject holder)
	{
		Il2CppArrayBase<Image> componentsInChildren = holder.GetComponentsInChildren<Image>(includeInactive: true);
		if (componentsInChildren == null)
		{
			return null;
		}
		foreach (Image item in componentsInChildren)
		{
			try
			{
				if (!(item == null) && !(item.sprite == null))
				{
					string text = item.sprite.name?.ToLower() ?? "";
					if (text.Contains("cp_ur") || text.Contains("craft_ur"))
					{
						return "CP UR";
					}
					if (text.Contains("cp_sr") || text.Contains("craft_sr"))
					{
						return "CP SR";
					}
					if (text.Contains("cp_r") || text.Contains("craft_r"))
					{
						return "CP R";
					}
					if (text.Contains("cp_n") || text.Contains("craft_n"))
					{
						return "CP N";
					}
					if (text.Contains("gem"))
					{
						return "Gems";
					}
				}
			}
			catch
			{
			}
		}
		return null;
	}

	private static string FindPassType(SelectionButton button)
	{
		try
		{
			Transform transform = button.transform;
			for (int i = 0; i < 6; i++)
			{
				if (!(transform != null))
				{
					break;
				}
				string text = transform.name?.ToLower() ?? "";
				if (text.Contains("normal"))
				{
					return "Normal";
				}
				if (text.Contains("gold"))
				{
					return "Gold";
				}
				transform = transform.parent;
			}
		}
		catch
		{
		}
		return "";
	}

	private static string FindGrade(SelectionButton button)
	{
		try
		{
			Transform parent = button.transform.parent;
			for (int i = 0; i < 4; i++)
			{
				if (!(parent != null))
				{
					break;
				}
				Transform transform = parent.Find("GradeText") ?? parent.Find("gradeText") ?? parent.Find("Grade");
				if (transform != null)
				{
					string text = TextExtractor.ExtractFirst(transform.gameObject);
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
				parent = parent.parent;
			}
		}
		catch
		{
		}
		return null;
	}

	private static DuelpassRewardButtonWidget FindRewardWidget(SelectionButton button)
	{
		Transform transform = button.transform;
		for (int i = 0; i < 4; i++)
		{
			if (!(transform != null))
			{
				break;
			}
			try
			{
				DuelpassRewardButtonWidget component = transform.GetComponent<DuelpassRewardButtonWidget>();
				if (component != null)
				{
					return component;
				}
			}
			catch
			{
			}
			transform = transform.parent;
		}
		return null;
	}
}
