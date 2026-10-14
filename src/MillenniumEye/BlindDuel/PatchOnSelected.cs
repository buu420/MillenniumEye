// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTMPro;
using Il2CppYgomGame.Duel;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

[HarmonyPatch(typeof(SelectionButton), "OnSelected", MethodType.Normal)]
internal class PatchOnSelected
{
	private static string TryReadDialogItem(Transform buttonTransform)
	{
		try
		{
			Transform transform = buttonTransform;
			for (int i = 0; i < 5; i++)
			{
				if (!(transform != null))
				{
					break;
				}
				Il2CppArrayBase<TextMeshProUGUI> componentsInChildren = transform.GetComponentsInChildren<TextMeshProUGUI>();
				string text = null;
				string text2 = null;
				foreach (TextMeshProUGUI item in componentsInChildren)
				{
					if (item.gameObject.name == "ItemNameText")
					{
						text = item.text?.Trim();
					}
					else if (item.gameObject.name == "ItemNumText")
					{
						text2 = item.text?.Trim();
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					if (!string.IsNullOrEmpty(text2))
					{
						text2 = text2.TrimStart('×', 'x', 'X', ' ');
						return text + " x" + text2;
					}
					return text;
				}
				transform = transform.parent;
			}
		}
		catch
		{
		}
		return null;
	}

	private static string FindSiblingText(Transform start)
	{
		Transform transform = start;
		for (int i = 0; i < 3; i++)
		{
			if (!(transform.parent != null))
			{
				break;
			}
			Transform parent = transform.parent;
			for (int j = 0; j < parent.childCount; j++)
			{
				Transform child = parent.GetChild(j);
				if (child == transform)
				{
					continue;
				}
				TMP_Text component = child.GetComponent<TMP_Text>();
				if (component != null && component.gameObject.activeInHierarchy)
				{
					string text = TextUtil.StripTags(component.text ?? "").Trim();
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
			}
			transform = transform.parent;
		}
		return null;
	}

	[HarmonyPostfix]
	private static void Postfix(SelectionButton __instance, bool __result)
	{
		if (!__result || DuelState.IsDuelLogOpen)
		{
			return;
		}
		if (Speech.IsSameButton(__instance))
		{
			try
			{
				if (__instance.GetComponentInParent<CardSelectionList>() == null)
				{
					return;
				}
			}
			catch
			{
				return;
			}
		}
		if (ScreenDetector.HasPendingScreen)
		{
			ScreenDetector.DeferFocusedButton(__instance);
		}
		else if (DuelState.HasPendingSelection)
		{
			DuelState.DeferredSelectionButton = __instance;
		}
		else if (DuelState.SuppressNextFieldFocus && NavigationState.IsInDuel)
		{
			DuelState.SuppressNextFieldFocus = false;
			DuelState.DeferredSelectionButton = __instance;
		}
		else
		{
			if (!ScreenDetector.IsScreenReady())
			{
				return;
			}
			string text = TextExtractor.ExtractFirst(__instance.gameObject);
			if (string.IsNullOrWhiteSpace(text))
			{
				text = FindSiblingText(__instance.transform) ?? text;
			}
			string text2 = null;
			IMenuHandler current = HandlerRegistry.Current;
			if (current != null)
			{
				text2 = current.OnButtonFocused(__instance);
				if (text2 != null)
				{
					text = text2;
				}
			}
			if (text2 == null)
			{
				string text3 = TryReadDialogItem(__instance.transform);
				if (text3 != null)
				{
					text = text3;
					text2 = text3;
				}
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			if (text2 == null)
			{
				var (value, num) = TransformSearch.GetButtonIndex(__instance);
				if (num > 1)
				{
					text += $"\n{value} of {num}";
				}
			}
			bool flag = NavigationState.DialogJustAnnounced || NavigationState.ScreenJustAnnounced || DuelState.MessageJustAnnounced;
			if (flag)
			{
				NavigationState.DialogJustAnnounced = false;
				NavigationState.ScreenJustAnnounced = false;
				DuelState.MessageJustAnnounced = false;
			}
			if (NavigationState.IsInDuel)
			{
				DuelState.LastQueuedButtonText = text;
				DuelState.LastQueuedButtonFrame = Time.frameCount;
				DuelState.LastQueuedButtonInterrupt = !flag;
			}
			else if (flag)
			{
				Speech.SayQueued(text);
			}
			else
			{
				Speech.SayItem(text);
			}
		}
	}
}
