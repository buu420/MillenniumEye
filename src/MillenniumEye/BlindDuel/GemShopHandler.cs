// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.GemShop;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public class GemShopHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		return viewControllerName == "GemShop";
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		string text = ScreenDetector.ReadGameHeaderText();
		Speech.AnnounceScreen(text ?? "Gem Shop");
		return true;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		try
		{
			ProductWidget productWidget = FindProductWidget(button);
			if (productWidget == null)
			{
				return null;
			}
			string text = productWidget.productName?.text?.Trim();
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			string text2 = text;
			try
			{
				Dictionary<int, ProductWidget.ItemWidget> itemWidgetMap = productWidget.m_ItemWidgetMap;
				if (itemWidgetMap != null)
				{
					ProductWidget.ItemWidget value = null;
					ProductWidget.ItemWidget value2 = null;
					itemWidgetMap.TryGetValue(0, out value);
					itemWidgetMap.TryGetValue(1, out value2);
					string text3 = value?.itemNumText?.text?.Trim();
					string text4 = value2?.itemNumText?.text?.Trim();
					if (!string.IsNullOrEmpty(text3))
					{
						text2 = text2 + "\n" + text3 + " purchased";
					}
					if (!string.IsNullOrEmpty(text4))
					{
						text2 = text2 + "\n" + text4 + " free";
					}
				}
			}
			catch
			{
			}
			string text5 = TextUtil.StripCJK(productWidget.limitCountText?.text?.Trim());
			if (!string.IsNullOrEmpty(text5))
			{
				text2 = text2 + "\n" + text5;
			}
			try
			{
				GameObject popIconRoot = productWidget.popIconRoot;
				if (popIconRoot != null && popIconRoot.activeInHierarchy)
				{
					string text6 = TextUtil.StripCJK(productWidget.popIconLabel?.text?.Trim());
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
				GameObject limitDateRoot = productWidget.limitDateRoot;
				if (limitDateRoot != null && limitDateRoot.activeInHierarchy)
				{
					string text7 = productWidget.limitDateText?.text?.Trim();
					if (!string.IsNullOrEmpty(text7))
					{
						text2 = text2 + "\n" + text7;
					}
				}
			}
			catch
			{
			}
			string text8 = TextUtil.StripCJK(productWidget.priceLabel?.text?.Trim());
			if (!string.IsNullOrEmpty(text8))
			{
				text2 = text2 + "\nPrice: " + text8;
			}
			int value3 = productWidget.idx + 1;
			int productCount = GetProductCount();
			if (productCount > 1)
			{
				text2 += $"\n{value3} of {productCount}";
			}
			return text2;
		}
		catch (Exception ex)
		{
			Log.Write("[GemShopHandler] " + ex.Message);
			return null;
		}
	}

	private static ProductWidget FindProductWidget(SelectionButton button)
	{
		try
		{
			ViewController focusVC = ScreenDetector.GetFocusVC();
			if (focusVC == null)
			{
				return null;
			}
			GemShopViewController gemShopViewController = focusVC.TryCast<GemShopViewController>();
			if (gemShopViewController == null)
			{
				return null;
			}
			Dictionary<GameObject, ProductWidget> entityWidgetMap = gemShopViewController.m_EntityWidgetMap;
			if (entityWidgetMap == null)
			{
				return null;
			}
			Transform transform = button.transform;
			for (int i = 0; i < 5; i++)
			{
				if (!(transform != null))
				{
					break;
				}
				if (entityWidgetMap.TryGetValue(transform.gameObject, out var value))
				{
					return value;
				}
				transform = transform.parent;
			}
		}
		catch
		{
		}
		return null;
	}

	private static int GetProductCount()
	{
		try
		{
			return ((ScreenDetector.GetFocusVC()?.TryCast<GemShopViewController>())?.m_ProductContexts?.Count).GetValueOrDefault();
		}
		catch
		{
			return 0;
		}
	}
}
