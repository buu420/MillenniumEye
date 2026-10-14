// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.SubMenu;
using Il2CppYgomSystem.UI;
using Il2CppYgomSystem.UI.InfinityScroll;
using UnityEngine;

namespace MillenniumEye;

public class HomeSubMenuHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		if (viewControllerName == "HomeSubMenu" || viewControllerName == "HomeSubMenuViewController")
		{
			return true;
		}
		return false;
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		return true;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		try
		{
			HomeSubMenuViewController currentInstance = HomeSubMenuState.CurrentInstance;
			if (currentInstance == null)
			{
				return null;
			}
			InfinityScrollView infinityScrollView = currentInstance.m_InfinityScrollView;
			List<int> templates = currentInstance.m_Templates;
			List<string> texts = currentInstance.m_Texts;
			if (infinityScrollView == null || templates == null || texts == null)
			{
				return null;
			}
			int k_TitleTNo = currentInstance.k_TitleTNo;
			int k_ItemTNo = currentInstance.k_ItemTNo;
			int num = Math.Min(templates.Count, texts.Count);
			int num2 = -1;
			Transform transform = button.transform;
			for (int i = 0; i < 6; i++)
			{
				if (!(transform != null))
				{
					break;
				}
				int dataIndexByEntity = infinityScrollView.GetDataIndexByEntity(transform.gameObject);
				if (dataIndexByEntity >= 0 && dataIndexByEntity < num)
				{
					num2 = dataIndexByEntity;
					break;
				}
				transform = transform.parent;
			}
			string text = TextExtractor.ExtractFirst(button.gameObject);
			if (string.IsNullOrWhiteSpace(text) && num2 >= 0)
			{
				text = texts[num2];
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return null;
			}
			string text2 = null;
			int num3 = 0;
			int num4 = 0;
			if (num2 >= 0)
			{
				for (int num5 = num2; num5 >= 0; num5--)
				{
					if (templates[num5] == k_TitleTNo)
					{
						text2 = texts[num5];
						break;
					}
				}
			}
			for (int j = 0; j < num; j++)
			{
				if (templates[j] == k_ItemTNo)
				{
					num4++;
					if (j == num2)
					{
						num3 = num4;
					}
				}
			}
			string text3 = text;
			if (num3 > 0 && num4 > 1)
			{
				text3 += $", {num3} of {num4}";
			}
			if (!string.IsNullOrWhiteSpace(text2) && text2 != HomeSubMenuState.LastSpokenSection)
			{
				HomeSubMenuState.LastSpokenSection = text2;
				text3 = text2 + ",\n" + text3;
			}
			return text3;
		}
		catch (Exception ex)
		{
			Log.Write("[HomeSubMenu] " + ex.Message);
			return null;
		}
	}
}
