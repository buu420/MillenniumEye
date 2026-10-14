// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.Menu;
using Il2CppYgomGame.Utility;
using Il2CppYgomSystem.UI;
using Il2CppYgomSystem.UI.InfinityScroll;
using UnityEngine;

namespace MillenniumEye;

public class EditProfileHandler : IMenuHandler
{
	private struct GridQuery
	{
		public int id;

		public int index;

		public int total;

		public bool currentlySet;
	}

	public bool CanHandle(string viewControllerName)
	{
		switch (viewControllerName)
		{
		case "ProfileEditView":
		case "ProfileEditViewController":
		case "ProfileEdit":
			return true;
		default:
			return false;
		}
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		string text = ScreenDetector.ReadGameHeaderText();
		Speech.AnnounceScreen((!string.IsNullOrWhiteSpace(text)) ? text : "Edit Profile");
		return true;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		try
		{
			ProfileEditViewController profileEditViewController = ScreenDetector.GetFocusVC()?.TryCast<ProfileEditViewController>();
			if (profileEditViewController == null)
			{
				return null;
			}
			if (button.transform.GetComponentInParent<ProfileEditViewController.SideMenu>() != null)
			{
				return FormatCategory(button);
			}
			ProfileEditViewController.ProfileEdit currentEditing = profileEditViewController.currentEditing;
			if (currentEditing != null)
			{
				string text = FormatGridItem(button, currentEditing);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			return null;
		}
		catch (System.Exception ex)
		{
			Log.Write("[EditProfile] " + ex.Message);
			return null;
		}
	}

	private static string FormatCategory(SelectionButton button)
	{
		string text = TextExtractor.ExtractFirst(button.gameObject);
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}
		var (num, num2) = CountCategorySiblings(button);
		if (num2 > 1 && num > 0)
		{
			return $"{text}, {num} of {num2}";
		}
		return text;
	}

	private static (int idx, int total) CountCategorySiblings(SelectionButton button)
	{
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
				return (idx: num, total: num2);
			}
			parent = parent.parent;
		}
		return (idx: 0, total: 0);
	}

	private static string FormatGridItem(SelectionButton button, ProfileEditViewController.ProfileEdit editing)
	{
		GridQuery? gridQuery = ResolveGridItem(button, editing);
		if (!gridQuery.HasValue)
		{
			return null;
		}
		int id = gridQuery.Value.id;
		ItemUtil.Category category = MapCategory(editing.sideMenuType);
		string text = null;
		try
		{
			if (category != ItemUtil.Category.NONE)
			{
				text = ItemUtil.GetItemName(isPerild: false, (int)category, id);
			}
		}
		catch
		{
		}
		if (string.IsNullOrWhiteSpace(text))
		{
			text = TextExtractor.ExtractFirst(button.gameObject);
		}
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}
		string text2 = text;
		if (gridQuery.Value.currentlySet)
		{
			text2 += ", currently set";
		}
		if (gridQuery.Value.total > 0 && gridQuery.Value.index > 0)
		{
			text2 += $", {gridQuery.Value.index} of {gridQuery.Value.total}";
		}
		return text2;
	}

	private static GridQuery? ResolveGridItem(SelectionButton button, ProfileEditViewController.ProfileEdit editing)
	{
		ProfileEditViewController.ProfileAvatarEdit avatar = editing.TryCast<ProfileEditViewController.ProfileAvatarEdit>();
		if (avatar != null)
		{
			return Query(button, avatar.m_InfinityScroll, avatar.itemIdList?.Count ?? 0, (int i) => UnboxInt(avatar.itemIdList, i), avatar.selectedItemId);
		}
		ProfileEditViewController.ProfileEditImage image = editing.TryCast<ProfileEditViewController.ProfileEditImage>();
		if (image != null)
		{
			return Query(button, image.m_InfinityScroll, image.itemList?.Count ?? 0, (int i) => UnboxInt(image.itemList, i), image.selectedItemId);
		}
		ProfileEditViewController.ProfileCardFileEdit cardfile = editing.TryCast<ProfileEditViewController.ProfileCardFileEdit>();
		if (cardfile != null)
		{
			return Query(button, cardfile.m_InfinityScroll, cardfile.itemIdList?.Count ?? 0, (int i) => UnboxInt(cardfile.itemIdList, i), cardfile.selectedItemId);
		}
		ProfileEditViewController.ProfileHomeWallPaperEdit wallpaper = editing.TryCast<ProfileEditViewController.ProfileHomeWallPaperEdit>();
		if (wallpaper != null)
		{
			return Query(button, wallpaper.m_InfinityScroll, wallpaper.itemIdList?.Count ?? 0, (int i) => UnboxInt(wallpaper.itemIdList, i), wallpaper.selectedItemId);
		}
		ProfileEditViewController.ProfileEditTag tag = editing.TryCast<ProfileEditViewController.ProfileEditTag>();
		if (tag != null)
		{
			return Query(button, tag.m_InfinityScroll, tag.tagList?.Count ?? 0, (int i) => UnboxInt(tag.tagList, i), tag.selectedItemId);
		}
		return null;
	}

	private static GridQuery? Query(SelectionButton button, InfinityScrollView scroll, int total, System.Func<int, int> idAt, int selectedItemId)
	{
		if (scroll == null || total == 0)
		{
			return null;
		}
		int num = -1;
		Transform transform = button.transform;
		for (int i = 0; i < 6; i++)
		{
			if (!(transform != null))
			{
				break;
			}
			int dataIndexByEntity = scroll.GetDataIndexByEntity(transform.gameObject);
			if (dataIndexByEntity >= 0 && dataIndexByEntity < total)
			{
				num = dataIndexByEntity;
				break;
			}
			transform = transform.parent;
		}
		if (num < 0)
		{
			return null;
		}
		int num2 = idAt(num);
		return new GridQuery
		{
			id = num2,
			index = num + 1,
			total = total,
			currentlySet = (num2 == selectedItemId)
		};
	}

	private static int UnboxInt(List<Il2CppSystem.Object> list, int index)
	{
		try
		{
			if (list == null || index < 0 || index >= list.Count)
			{
				return -1;
			}
			Il2CppSystem.Object obj = list[index];
			if (obj == null)
			{
				return -1;
			}
			string s = obj.ToString();
			int result;
			return int.TryParse(s, out result) ? result : (-1);
		}
		catch
		{
			return -1;
		}
	}

	private static ItemUtil.Category MapCategory(ProfileEditViewController.SideMenuType type)
	{
		if (1 == 0)
		{
		}
		ItemUtil.Category result = type switch
		{
			ProfileEditViewController.SideMenuType.ICON => ItemUtil.Category.ICON, 
			ProfileEditViewController.SideMenuType.ICONFRAME => ItemUtil.Category.ICON_FRAME, 
			ProfileEditViewController.SideMenuType.TAG => ItemUtil.Category.PROFILE_TAG, 
			ProfileEditViewController.SideMenuType.MATE => ItemUtil.Category.AVATAR, 
			ProfileEditViewController.SideMenuType.MATEBASE => ItemUtil.Category.AVATAR, 
			ProfileEditViewController.SideMenuType.MATE_DECK => ItemUtil.Category.AVATAR, 
			ProfileEditViewController.SideMenuType.WALLPAPER_PROFILE => ItemUtil.Category.WALLPAPER, 
			ProfileEditViewController.SideMenuType.WALLPAPER_HOME => ItemUtil.Category.AVATAR_HOME, 
			ProfileEditViewController.SideMenuType.CARDFILE => ItemUtil.Category.CARD_FILE, 
			ProfileEditViewController.SideMenuType.DECKCASE => ItemUtil.Category.DECK_CASE, 
			ProfileEditViewController.SideMenuType.PROTECTOR => ItemUtil.Category.PROTECTOR, 
			ProfileEditViewController.SideMenuType.FIELD => ItemUtil.Category.FIELD, 
			ProfileEditViewController.SideMenuType.FIELDPARTS => ItemUtil.Category.FIELD_OBJ, 
			_ => ItemUtil.Category.NONE, 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
