// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using Il2CppYgomGame.Mission;
using Il2CppYgomGame.Utility;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public class MissionsHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		if (viewControllerName == "Mission" || viewControllerName == "MissionMenu")
		{
			return true;
		}
		return false;
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		string text = ScreenDetector.ReadGameHeaderText() ?? "Missions";
		try
		{
			string text2 = (GetRootContext()?.currentTab)?._tabNameText_k__BackingField;
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + ", " + text2;
			}
		}
		catch
		{
		}
		Speech.AnnounceScreen(text);
		return true;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		try
		{
			if (button.name == "Button" && IsTabButton(button))
			{
				return HandleTab(button);
			}
			if (button.name == "Locator" && IsGoalButton(button))
			{
				return HandleGoal(button);
			}
			if (IsReceiveAllButton(button))
			{
				return "Receive All";
			}
			return null;
		}
		catch (Exception ex)
		{
			Log.Write("[Missions] ERROR: " + ex.Message);
			return null;
		}
	}

	private static bool IsTabButton(SelectionButton button)
	{
		Transform parent = button.transform.parent;
		return parent != null && parent.name.StartsWith("TabTemplate");
	}

	private static bool IsGoalButton(SelectionButton button)
	{
		Transform parent = button.transform.parent;
		return parent != null && parent.name.StartsWith("GoalHolder");
	}

	private static bool IsReceiveAllButton(SelectionButton button)
	{
		Transform transform = button.transform;
		for (int i = 0; i < 4; i++)
		{
			if (!(transform != null))
			{
				break;
			}
			if (transform.name.Contains("BulkRecieve"))
			{
				return true;
			}
			transform = transform.parent;
		}
		return false;
	}

	private string HandleTab(SelectionButton button)
	{
		try
		{
			string text = ReadTabText(button.transform.parent);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			Transform parent = button.transform.parent;
			Transform transform = parent?.parent;
			if (transform == null)
			{
				return text;
			}
			int value = 0;
			int num = 0;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.gameObject.activeInHierarchy && child.name.StartsWith("TabTemplate"))
				{
					num++;
					if (child == parent)
					{
						value = num;
					}
				}
			}
			string text2 = "";
			try
			{
				List<TabContext> list = GetRootContext()?.tabs;
				if (list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						TabContext tabContext = list[j];
						if (tabContext?._tabNameText_k__BackingField == text)
						{
							List<MissionContext> missions = tabContext.m_Missions;
							if (missions != null && missions.Count > 0)
							{
								text2 = $", {missions.Count} missions";
							}
							break;
						}
					}
				}
			}
			catch
			{
			}
			string text3 = text + text2;
			if (num > 1)
			{
				text3 += $"\n{value} of {num}";
			}
			return text3;
		}
		catch (Exception ex)
		{
			Log.Write("[Missions] Tab ERROR: " + ex.Message);
			return null;
		}
	}

	private static string ReadTabText(Transform tabTemplate)
	{
		if (tabTemplate == null)
		{
			return null;
		}
		string[] array = new string[2] { "ImageOff", "ImageOn" };
		foreach (string n in array)
		{
			Transform transform = tabTemplate.Find(n);
			if (transform == null)
			{
				continue;
			}
			Transform transform2 = transform.Find("TextTMP");
			if (!(transform2 == null))
			{
				string text = transform2.GetComponent<TextMeshProUGUI>()?.text?.Trim();
				if (!string.IsNullOrEmpty(text) && text != "OK")
				{
					return text;
				}
			}
		}
		return null;
	}

	private string HandleGoal(SelectionButton button)
	{
		TabContext tabContext = GetRootContext()?.currentTab;
		if (tabContext == null)
		{
			return null;
		}
		Dictionary<GameObject, MissionPanelWidget> dictionary = GetResidentContent()?.m_PanelWidgetsMap;
		if (dictionary == null || dictionary.Count == 0)
		{
			return null;
		}
		MissionPanelWidget missionPanelWidget = null;
		GameObject gameObject = null;
		foreach (KeyValuePair<GameObject, MissionPanelWidget> item in dictionary)
		{
			if (item.Key != null && button.transform.IsChildOf(item.Key.transform))
			{
				missionPanelWidget = item.Value;
				gameObject = item.Key;
				break;
			}
		}
		if (missionPanelWidget == null)
		{
			return null;
		}
		int missionId = missionPanelWidget.missionId;
		Dictionary<int, MissionContext> missionMap = tabContext.m_MissionMap;
		MissionContext value = null;
		missionMap?.TryGetValue(missionId, out value);
		if (value == null)
		{
			return null;
		}
		string text = value.m_MissionNameCache;
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		try
		{
			Il2CppReferenceArray<GoalContext> goals_k__BackingField = value._goals_k__BackingField;
			if (goals_k__BackingField != null && goals_k__BackingField.Length > 0)
			{
				int requirement = goals_k__BackingField[0].requirement;
				if (requirement > 0)
				{
					text += $" ({value.progress}/{requirement})";
				}
			}
		}
		catch
		{
		}
		string text2 = text;
		try
		{
			string text3 = button.transform.parent?.name;
			Il2CppReferenceArray<GoalContext> goals_k__BackingField2 = value._goals_k__BackingField;
			if (goals_k__BackingField2 != null && text3 != null && text3.StartsWith("GoalHolder") && int.TryParse(text3.Substring("GoalHolder".Length), out var result) && result < goals_k__BackingField2.Length)
			{
				GoalContext goalContext = goals_k__BackingField2[result];
				if (goalContext != null)
				{
					MissionGoalWidget.GoalType goalType = goalContext.GetGoalType(value);
					if (1 == 0)
					{
					}
					string text4 = goalType switch
					{
						MissionGoalWidget.GoalType.Complete => "Received", 
						MissionGoalWidget.GoalType.Recievable => "Reward available", 
						MissionGoalWidget.GoalType.InProgress => "In progress", 
						_ => "", 
					};
					if (1 == 0)
					{
					}
					string text5 = text4;
					if (!string.IsNullOrEmpty(text5))
					{
						text2 = text2 + "\n" + text5;
					}
					if (goalContext.itemCount > 0)
					{
						string rewardName = GetRewardName(goalContext);
						text2 += $"\nReward: {goalContext.itemCount} {rewardName}".TrimEnd();
					}
				}
			}
		}
		catch
		{
		}
		try
		{
			if (gameObject != null)
			{
				string text6 = FindTMPText(gameObject.transform, "LimitDateTextTMP");
				if (!string.IsNullOrEmpty(text6))
				{
					text2 = text2 + "\n" + text6;
				}
			}
		}
		catch
		{
		}
		try
		{
			List<MissionContext> missions = tabContext.m_Missions;
			if (missions != null)
			{
				int count = missions.Count;
				for (int i = 0; i < count; i++)
				{
					MissionContext missionContext = missions[i];
					if (missionContext != null && missionContext.missionId == missionId)
					{
						text2 += $"\n{i + 1} of {count}";
						break;
					}
				}
			}
		}
		catch
		{
		}
		return text2;
	}

	private static string GetRewardName(GoalContext goal)
	{
		try
		{
			string itemName = ItemUtil.GetItemName(goal.isPeriod, goal.itemCategory, goal.itemId);
			if (!string.IsNullOrEmpty(itemName))
			{
				return itemName;
			}
			string itemCategoryName = ItemUtil.GetItemCategoryName(goal.isPeriod, goal.itemCategory, goal.itemId);
			if (!string.IsNullOrEmpty(itemCategoryName))
			{
				return itemCategoryName;
			}
		}
		catch
		{
		}
		return "";
	}

	private static string FindTMPText(Transform root, string targetName)
	{
		Il2CppArrayBase<TextMeshProUGUI> componentsInChildren = root.GetComponentsInChildren<TextMeshProUGUI>();
		foreach (TextMeshProUGUI item in componentsInChildren)
		{
			if (item.gameObject.name == targetName)
			{
				string text = item.text?.Trim();
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
		}
		return null;
	}

	private static MissionViewController GetMissionVC()
	{
		try
		{
			return ScreenDetector.GetFocusVC()?.TryCast<MissionViewController>();
		}
		catch
		{
			return null;
		}
	}

	private static ResidentMissionContent GetResidentContent()
	{
		try
		{
			MissionViewController missionVC = GetMissionVC();
			if (missionVC == null)
			{
				return null;
			}
			Dictionary<MissionViewController.MissionContentType, IMissionContent> contentMap = missionVC.m_ContentMap;
			if (contentMap == null)
			{
				return null;
			}
			if (contentMap.TryGetValue(MissionViewController.MissionContentType.Mission, out var value))
			{
				return value?.TryCast<ResidentMissionContent>();
			}
		}
		catch
		{
		}
		return null;
	}

	private static MissionRootContext GetRootContext()
	{
		return GetResidentContent()?.m_RootContext;
	}
}
