// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppYgomGame.Download;
using Il2CppYgomGame.Enquete;
using Il2CppYgomGame.Menu;
using Il2CppYgomGame.SubMenu;
using Il2CppYgomSystem.ElementSystem;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public static class ScreenDetector
{
	private static readonly Dictionary<string, Menu> VCToMenu = new Dictionary<string, Menu>(StringComparer.OrdinalIgnoreCase)
	{
		{
			"SoloMode",
			Menu.Solo
		},
		{
			"SoloGate",
			Menu.Solo
		},
		{
			"SoloSelectChapter",
			Menu.Solo
		},
		{
			"DuelClient",
			Menu.Duel
		},
		{
			"DuelLive",
			Menu.Duel
		},
		{
			"DeckEdit",
			Menu.Deck
		},
		{
			"DeckBrowser",
			Menu.Deck
		},
		{
			"DeckSelect",
			Menu.Deck
		},
		{
			"Shop",
			Menu.Shop
		},
		{
			"SettingMenuViewController",
			Menu.Settings
		}
	};

	private static bool _pendingEnqueteCheck;

	private static string _lastEnquetePage = "";

	private static DownloadViewController _activeDownloadVC;

	private static int _lastDownloadPercent = -1;

	private static ViewController _pendingVC;

	private static string _pendingCleanName;

	private static SelectionButton _deferredButton;

	internal static bool HasPendingScreen => _pendingVC != null;

	public static void Poll()
	{
		CheckScreenChange();
		CheckPendingAnnouncement();
		CheckEnqueteScreen();
		CheckDownloadProgress();
	}

	public static void TrackDownload(DownloadViewController vc)
	{
		_activeDownloadVC = vc;
		_lastDownloadPercent = -1;
	}

	internal static bool IsScreenReady()
	{
		try
		{
			GameObject gameObject = GameObject.Find("UI/ContentCanvas/ContentManager");
			if (gameObject == null)
			{
				return true;
			}
			ViewControllerManager component = gameObject.GetComponent<ViewControllerManager>();
			return component == null || component.IsReadyTransition();
		}
		catch
		{
			return true;
		}
	}

	internal static ViewController GetFocusVC()
	{
		GameObject gameObject = GameObject.Find("UI/ContentCanvas/ContentManager");
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<ViewControllerManager>()?.GetFocusViewController();
	}

	internal static ElementObjectManager GetView(ViewController vc)
	{
		try
		{
			BaseMenuViewController baseMenuViewController = vc.TryCast<BaseMenuViewController>();
			if (baseMenuViewController == null)
			{
				Log.Write($"[GetView] TryCast failed for: {vc.name} ({vc.GetIl2CppType().Name})");
				return null;
			}
			return baseMenuViewController.m_View;
		}
		catch (Exception ex)
		{
			Log.Write("[GetView] Error for " + vc.name + ": " + ex.Message);
			return null;
		}
	}

	internal static string FindScreenTitle(ViewController vc)
	{
		if (vc == null)
		{
			return null;
		}
		ElementObjectManager view = GetView(vc);
		List<(string, string)> list = null;
		if (view != null)
		{
			list = ElementReader.ReadElements(view, "[FindTitle] " + vc.name + " |");
			if (list.Count == 0)
			{
				List<TextResult> list2 = TextExtractor.ExtractAll(view.gameObject, new TextSearchOptions
				{
					ActiveOnly = false,
					FilterBanned = false,
					LogPrefix = "[FindTitle-fallback] " + vc.name + " |"
				});
				list = new List<(string, string)>();
				foreach (TextResult item in list2)
				{
					list.Add((item.Path, item.Text));
				}
			}
		}
		if (list == null || list.Count == 0)
		{
			List<TextResult> list3 = TextExtractor.ExtractAll(vc.gameObject, new TextSearchOptions
			{
				ActiveOnly = false,
				FilterBanned = false,
				LogPrefix = "[FindTitle-vc] " + vc.name + " |"
			});
			list = new List<(string, string)>();
			foreach (TextResult item2 in list3)
			{
				list.Add((item2.Path, item2.Text));
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		var (text, text2) = ElementReader.ExtractTitleAndBody(list);
		if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
		{
			return text + ". " + text2;
		}
		return text ?? text2;
	}

	internal static string ReadGameHeaderText()
	{
		try
		{
			HeaderViewController instance = HeaderViewController.instance;
			if (instance == null || !instance.gameObject.activeInHierarchy)
			{
				return null;
			}
			ElementObjectManager ui = instance.ui;
			if (ui == null)
			{
				return null;
			}
			string tXT_LABEL = instance.TXT_LABEL;
			if (string.IsNullOrEmpty(tXT_LABEL))
			{
				return null;
			}
			string elementText = ElementReader.GetElementText(ui, tXT_LABEL);
			if (!string.IsNullOrWhiteSpace(elementText) && !IsNumericOnly(elementText))
			{
				return elementText;
			}
			List<TextResult> list = TextExtractor.ExtractAll(ui.gameObject, new TextSearchOptions
			{
				ActiveOnly = true,
				FilterBanned = false
			});
			foreach (TextResult item in list)
			{
				if (!string.IsNullOrWhiteSpace(item.Text) && !IsNumericOnly(item.Text))
				{
					return item.Text;
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			Log.Write("[HeaderVC] Error: " + ex.Message);
			return null;
		}
	}

	private static void UpdateMenuContext(string cleanName)
	{
		if (VCToMenu.TryGetValue(cleanName, out var value))
		{
			NavigationState.CurrentMenu = value;
			Log.Write($"[MenuContext] Set to {value} from VC: {cleanName}");
		}
		else if (NavigationState.CurrentMenu != Menu.None)
		{
			Log.Write($"[MenuContext] Reset from {NavigationState.CurrentMenu} to None for VC: {cleanName}");
			NavigationState.CurrentMenu = Menu.None;
		}
	}

	private static void CheckScreenChange()
	{
		try
		{
			ViewController focusVC = GetFocusVC();
			if (focusVC == null)
			{
				return;
			}
			string name = focusVC.name;
			if (name == NavigationState.LastFocusVCName)
			{
				return;
			}
			NavigationState.LastFocusVCName = name;
			string text;
			if (!name.EndsWith("(Clone)"))
			{
				text = name;
			}
			else
			{
				string text2 = name;
				text = text2.Substring(0, text2.Length - 7);
			}
			string text3 = text;
			UpdateMenuContext(text3);
			HandlerRegistry.SetCurrentFromVC(text3);
			HomeSubMenuViewController currentInstance = HomeSubMenuState.CurrentInstance;
			if (HandlerRegistry.Current is HomeHandler && currentInstance != null && !currentInstance.WasCollected)
			{
				HandlerRegistry.SetCurrentFromVC("HomeSubMenu");
			}
			bool flag;
			switch (text3)
			{
			case "GameEntryV1":
			case "GameEntrySequenceV2":
			case "Scenario":
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
			if (!flag && !(text3 == "MDMarkupAsset"))
			{
				if (text3 == "Enquete")
				{
					_pendingEnqueteCheck = true;
					_lastEnquetePage = "";
				}
				else
				{
					_pendingVC = focusVC;
					_pendingCleanName = text3;
					_deferredButton = null;
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[CheckScreenChange] " + ex.Message);
		}
	}

	private static void CheckPendingAnnouncement()
	{
		if (_pendingVC == null)
		{
			return;
		}
		try
		{
			GameObject gameObject = GameObject.Find("UI/ContentCanvas/ContentManager");
			if (gameObject == null)
			{
				return;
			}
			ViewControllerManager component = gameObject.GetComponent<ViewControllerManager>();
			if (component != null && !component.IsReadyTransition())
			{
				return;
			}
			BaseMenuViewController baseMenuViewController = _pendingVC.TryCast<BaseMenuViewController>();
			if (baseMenuViewController != null && baseMenuViewController.isLoading)
			{
				return;
			}
			ViewController pendingVC = _pendingVC;
			string pendingCleanName = _pendingCleanName;
			_pendingVC = null;
			_pendingCleanName = null;
			IMenuHandler current = HandlerRegistry.Current;
			if (current != null && current.OnScreenEntered(pendingCleanName))
			{
				NavigationState.ScreenJustAnnounced = true;
				QueueFocusedItem();
				return;
			}
			string text = ReadGameHeaderText();
			string text2 = FindScreenTitle(pendingVC);
			if (1 == 0)
			{
			}
			string text3 = ((text == null) ? ((text2 == null) ? null : text2) : ((text2 == null) ? text : (text + ". " + text2)));
			if (1 == 0)
			{
			}
			string text4 = text3;
			if (text4 == null)
			{
				Log.Write("[ScreenChange] No text found for: " + pendingCleanName);
				return;
			}
			Log.Write($"[ScreenChange] {pendingCleanName} | header='{text}', title='{text2}'");
			Speech.AnnounceScreen(text4);
			NavigationState.ScreenJustAnnounced = true;
			QueueFocusedItem();
		}
		catch (Exception ex)
		{
			Log.Write("[CheckPendingAnnouncement] " + ex.Message);
		}
	}

	private static void CheckEnqueteScreen()
	{
		if (!_pendingEnqueteCheck)
		{
			return;
		}
		try
		{
			ViewController focusVC = GetFocusVC();
			if (focusVC == null)
			{
				return;
			}
			string text;
			if (!focusVC.name.EndsWith("(Clone)"))
			{
				text = focusVC.name;
			}
			else
			{
				string name = focusVC.name;
				text = name.Substring(0, name.Length - 7);
			}
			string text2 = text;
			if (text2 != "Enquete")
			{
				_pendingEnqueteCheck = false;
				_lastEnquetePage = "";
				return;
			}
			EnqueteViewController enqueteViewController = focusVC.TryCast<EnqueteViewController>();
			if (enqueteViewController == null)
			{
				return;
			}
			string text3 = enqueteViewController.m_PageText?.text?.Trim();
			if (string.IsNullOrEmpty(text3) || text3 == _lastEnquetePage)
			{
				return;
			}
			_lastEnquetePage = text3;
			List<TextResult> list = TextExtractor.ExtractAll(focusVC.gameObject, new TextSearchOptions
			{
				ActiveOnly = true,
				FilterBanned = false,
				ExcludePathContaining = new List<string> { "button", "shortcut", "toggle", "entity", "checkbox" }
			});
			List<string> list2 = new List<string>();
			foreach (TextResult item in list)
			{
				if (item.Text != text3)
				{
					list2.Add(item.Text);
				}
			}
			if (list2.Count != 0)
			{
				string text4 = string.Join(". ", list2) + ". " + text3;
				Speech.AnnounceScreen(text4);
				QueueFocusedItem();
			}
		}
		catch (Exception ex)
		{
			Log.Write("[Enquete] Error: " + ex.Message);
		}
	}

	private static void CheckDownloadProgress()
	{
		try
		{
			if (_activeDownloadVC == null)
			{
				return;
			}
			if (_activeDownloadVC.WasCollected || !_activeDownloadVC.gameObject.activeInHierarchy)
			{
				_activeDownloadVC = null;
				_lastDownloadPercent = -1;
				return;
			}
			DownloadController downloadController = _activeDownloadVC.downloadController;
			if (downloadController == null)
			{
				return;
			}
			int num = (int)(downloadController.TotalProgress * 100f);
			if (num == _lastDownloadPercent)
			{
				return;
			}
			_lastDownloadPercent = num;
			if (num >= 100)
			{
				string text = _activeDownloadVC.DownloadingStateText?.text?.Trim();
				if (string.IsNullOrEmpty(text))
				{
					text = _activeDownloadVC.DownloadingText?.text?.Trim();
				}
				Speech.SayQueued(text ?? $"{num} percent");
				_activeDownloadVC = null;
				_lastDownloadPercent = -1;
			}
			else
			{
				Speech.SayQueued($"{num} percent");
			}
		}
		catch (Exception ex)
		{
			Log.Write("[Download] " + ex.Message);
		}
	}

	internal static void DeferFocusedButton(SelectionButton btn)
	{
		_deferredButton = btn;
	}

	internal static void QueueFocusedItem()
	{
		try
		{
			SelectionButton selectionButton = null;
			SelectionItem currentItem = SelectorManager.currentItem;
			if (currentItem != null)
			{
				selectionButton = currentItem.TryCast<SelectionButton>();
			}
			if (selectionButton == null)
			{
				selectionButton = _deferredButton;
			}
			_deferredButton = null;
			if (selectionButton == null || selectionButton.WasCollected || !selectionButton.gameObject.activeInHierarchy)
			{
				return;
			}
			string text = null;
			IMenuHandler current = HandlerRegistry.Current;
			if (current != null)
			{
				text = current.OnButtonFocused(selectionButton);
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				text = TextExtractor.ExtractFirst(selectionButton.gameObject);
				if (!string.IsNullOrWhiteSpace(text))
				{
					var (value, num) = TransformSearch.GetButtonIndex(selectionButton);
					if (num > 1)
					{
						text += $"\n{value} of {num}";
					}
				}
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				Speech.SayQueued(text);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[QueueFocusedItem] " + ex.Message);
		}
	}

	private static bool IsNumericOnly(string text)
	{
		string text2 = text.Trim();
		foreach (char c in text2)
		{
			if (c < '0' || c > '9')
			{
				return false;
			}
		}
		return text.Trim().Length > 0;
	}
}
