// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.Solo;
using Il2CppYgomSystem.ElementSystem;
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

public class SoloHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		switch (viewControllerName)
		{
		case "SoloMode":
		case "SoloGate":
		case "SoloSelectChapter":
			return true;
		default:
			return false;
		}
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		try
		{
			if (viewControllerName == "SoloSelectChapter")
			{
				SoloSelectChapterViewController soloSelectChapterViewController = ScreenDetector.GetFocusVC()?.TryCast<SoloSelectChapterViewController>();
				if (soloSelectChapterViewController != null)
				{
					int gateId = soloSelectChapterViewController.m_GateId;
					if (SoloState.Gates.TryGetValue(gateId, out var value) && !string.IsNullOrEmpty(value.Name))
					{
						Speech.AnnounceScreen(value.Name);
						return true;
					}
				}
			}
			string text = ScreenDetector.ReadGameHeaderText();
			if (!string.IsNullOrEmpty(text))
			{
				Speech.AnnounceScreen(text);
				return true;
			}
			ViewController focusVC = ScreenDetector.GetFocusVC();
			if (focusVC != null)
			{
				string text2 = ScreenDetector.FindScreenTitle(focusVC);
				if (!string.IsNullOrEmpty(text2))
				{
					Speech.AnnounceScreen(text2);
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[SoloHandler] OnScreenEntered: " + ex.Message);
		}
		return false;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		ViewController focusVC = ScreenDetector.GetFocusVC();
		if (focusVC == null)
		{
			return null;
		}
		SoloGateViewController soloGateViewController = focusVC.TryCast<SoloGateViewController>();
		if (soloGateViewController != null)
		{
			return ReadGateButton(button, soloGateViewController);
		}
		SoloSelectChapterViewController soloSelectChapterViewController = focusVC.TryCast<SoloSelectChapterViewController>();
		if (soloSelectChapterViewController != null)
		{
			return ReadChapterButton(button, soloSelectChapterViewController);
		}
		SoloModeViewController soloModeViewController = focusVC.TryCast<SoloModeViewController>();
		if (soloModeViewController != null)
		{
			return ReadPortalButton(button);
		}
		return null;
	}

	private static string ReadGateButton(SelectionButton button, SoloGateViewController gateVC)
	{
		try
		{
			SoloGateUtil.GateManager gateManager = gateVC.m_GateManager;
			if (gateManager?.masterDataDic == null)
			{
				return null;
			}
			string text = ReadGateNameFromEom(button);
			if (string.IsNullOrEmpty(text))
			{
				text = ReadGateNameFromText(button);
			}
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			SoloGateUtil.Data data = FindGateDataByName(gateManager, text);
			string text2 = text;
			if (data != null)
			{
				if (data.IsComplete)
				{
					text2 += ", Complete";
				}
				else if (data.IsClear)
				{
					text2 += ", Clear";
				}
				else if (!data.isUnlocked)
				{
					text2 += ", Locked";
				}
			}
			string text3 = data?.strOverview;
			if (string.IsNullOrEmpty(text3))
			{
				SoloState.GateInfo? gateInfo = SoloState.FindGateByName(text);
				if (gateInfo.HasValue)
				{
					text3 = gateInfo.Value.Overview;
				}
			}
			if (!string.IsNullOrEmpty(text3))
			{
				text2 = text2 + "\n" + TextUtil.StripTags(text3).Trim();
			}
			var (value, num) = FindGateIndex(data?.gateID ?? (-1), gateVC);
			if (num > 0)
			{
				text2 += $"\n{value} of {num}";
			}
			return text2;
		}
		catch (Exception ex)
		{
			Log.Write("[SoloHandler] Gate button: " + ex.Message);
			return null;
		}
	}

	private static string ReadPortalButton(SelectionButton button)
	{
		try
		{
			string text = ReadGateNameFromEom(button);
			if (string.IsNullOrEmpty(text))
			{
				text = ReadGateNameFromText(button);
			}
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			SoloState.GateInfo? gateInfo = SoloState.FindGateByName(text);
			if (!gateInfo.HasValue)
			{
				return null;
			}
			SoloState.GateInfo value = gateInfo.Value;
			string text2 = text;
			if (value.IsComplete)
			{
				text2 += ", Complete";
			}
			else if (value.IsClear)
			{
				text2 += ", Clear";
			}
			if (!string.IsNullOrEmpty(value.Overview))
			{
				text2 = text2 + "\n" + TextUtil.StripTags(value.Overview).Trim();
			}
			return text2;
		}
		catch (Exception ex)
		{
			Log.Write("[SoloHandler] Portal button: " + ex.Message);
			return null;
		}
	}

	private static string ReadGateNameFromEom(SelectionButton button)
	{
		try
		{
			ElementObjectManager elementObjectManager = button.GetComponentInParent<ElementObjectManager>();
			if (elementObjectManager == null)
			{
				elementObjectManager = button.GetComponent<ElementObjectManager>();
			}
			if (elementObjectManager == null)
			{
				return null;
			}
			string tXT_GATENAME_LABEL = SoloGateUtil.TXT_GATENAME_LABEL;
			if (string.IsNullOrEmpty(tXT_GATENAME_LABEL))
			{
				return null;
			}
			return ElementReader.GetElementText(elementObjectManager, tXT_GATENAME_LABEL);
		}
		catch (Exception ex)
		{
			Log.Write("[SoloHandler] EOM read: " + ex.Message);
			return null;
		}
	}

	private static string ReadGateNameFromText(SelectionButton button)
	{
		try
		{
			System.Collections.Generic.List<TextResult> list = TextExtractor.ExtractAll(button.gameObject, new TextSearchOptions
			{
				FilterBanned = false
			});
			if (list.Count == 0)
			{
				return null;
			}
			return list[list.Count - 1].Text;
		}
		catch
		{
			return null;
		}
	}

	private static SoloGateUtil.Data FindGateDataByName(SoloGateUtil.GateManager gateManager, string name)
	{
		if (gateManager?.masterDataDic == null || string.IsNullOrEmpty(name))
		{
			return null;
		}
		foreach (Il2CppSystem.Collections.Generic.KeyValuePair<int, SoloGateUtil.Data> item in gateManager.masterDataDic)
		{
			if (item.Value.StrName == name)
			{
				return item.Value;
			}
		}
		return null;
	}

	private static (int index, int total) FindGateIndex(int gateId, SoloGateViewController gateVC)
	{
		if (gateId < 0)
		{
			return (index: 0, total: 0);
		}
		Il2CppSystem.Collections.Generic.List<int> subDataList = gateVC.subDataList;
		if (subDataList != null)
		{
			for (int i = 0; i < subDataList.Count; i++)
			{
				if (subDataList[i] == gateId)
				{
					return (index: i + 1, total: subDataList.Count);
				}
			}
		}
		Il2CppSystem.Collections.Generic.List<int> mainDataList = gateVC.mainDataList;
		if (mainDataList != null)
		{
			for (int j = 0; j < mainDataList.Count; j++)
			{
				if (mainDataList[j] == gateId)
				{
					return (index: j + 1, total: mainDataList.Count);
				}
			}
		}
		return (index: 0, total: 0);
	}

	private static string ReadChapterButton(SelectionButton button, SoloSelectChapterViewController chapterVC)
	{
		try
		{
			SoloSelectChapterViewController.ChapterMap chapterMap = chapterVC.chapterMap;
			if (chapterMap?.chapterDataDic == null)
			{
				return null;
			}
			SoloSelectChapterViewController.Chapter chapter = null;
			int value = 0;
			int count = chapterMap.chapterDataDic.Count;
			int num = 0;
			foreach (Il2CppSystem.Collections.Generic.KeyValuePair<int, SoloSelectChapterViewController.Chapter> item in chapterMap.chapterDataDic)
			{
				num++;
				SoloSelectChapterViewController.Chapter value2 = item.Value;
				if (value2?.go == null || !button.transform.IsChildOf(value2.go.transform))
				{
					continue;
				}
				chapter = value2;
				value = num;
				break;
			}
			if (chapter == null)
			{
				return null;
			}
			string text = chapter.strChapter;
			if (string.IsNullOrEmpty(text))
			{
				text = "Chapter";
			}
			SoloModeUtil.DialogType dType = chapter.dType;
			if (1 == 0)
			{
			}
			string text2 = dType switch
			{
				SoloModeUtil.DialogType.DUEL => "Duel", 
				SoloModeUtil.DialogType.SCENARIO => "Scenario", 
				SoloModeUtil.DialogType.LOCK => "Locked", 
				SoloModeUtil.DialogType.REWARD => "Reward", 
				SoloModeUtil.DialogType.TUTORIAL => "Tutorial", 
				_ => null, 
			};
			if (1 == 0)
			{
			}
			string text3 = text2;
			if (!string.IsNullOrEmpty(text3))
			{
				text = text + ", " + text3;
			}
			SoloSelectChapterViewController.ChapterDuel chapterDuel = chapter.TryCast<SoloSelectChapterViewController.ChapterDuel>();
			if (chapterDuel != null && chapterDuel.level > 0)
			{
				text += $", Level {chapterDuel.level}";
			}
			SoloModeUtil.ChapterStatus status = chapter.status;
			if (1 == 0)
			{
			}
			text2 = status switch
			{
				SoloModeUtil.ChapterStatus.UNOPEN => "Locked", 
				SoloModeUtil.ChapterStatus.OPEN => "Available", 
				SoloModeUtil.ChapterStatus.RENTAL_CLEAR => "Loaner Clear", 
				SoloModeUtil.ChapterStatus.MYDECK_CLEAR => "My Deck Clear", 
				SoloModeUtil.ChapterStatus.COMPLETE => "Complete", 
				_ => null, 
			};
			if (1 == 0)
			{
			}
			string text4 = text2;
			if (!string.IsNullOrEmpty(text4))
			{
				text = text + ", " + text4;
			}
			string strExplanation = chapter.strExplanation;
			if (!string.IsNullOrEmpty(strExplanation))
			{
				text = text + "\n" + TextUtil.StripTags(strExplanation).Trim();
			}
			if (count > 0)
			{
				text += $"\n{value} of {count}";
			}
			return text;
		}
		catch (Exception ex)
		{
			Log.Write("[SoloHandler] Chapter button: " + ex.Message);
			return null;
		}
	}
}
