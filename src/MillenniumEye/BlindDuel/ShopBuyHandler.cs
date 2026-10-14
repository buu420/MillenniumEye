// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.Shop;
using Il2CppYgomSystem.ElementSystem;
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

public class ShopBuyHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		return viewControllerName == "ShopBuy";
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		try
		{
			ViewController focusVC = ScreenDetector.GetFocusVC();
			ShopBuyViewController shopBuyViewController = focusVC?.TryCast<ShopBuyViewController>();
			if (shopBuyViewController == null)
			{
				return false;
			}
			ProductContext productContext = shopBuyViewController.m_ProductContext;
			string text = productContext?.productName;
			if (string.IsNullOrEmpty(text))
			{
				ElementObjectManager view = ScreenDetector.GetView(focusVC);
				if (view != null)
				{
					text = ElementReader.GetElementText(view, shopBuyViewController.k_ELabelProductNameText ?? "ProductNameText");
				}
			}
			string text2 = text ?? "Product Details";
			string text3 = productContext?.descTextShort;
			if (string.IsNullOrEmpty(text3))
			{
				text3 = productContext?.listDescText;
			}
			if (!string.IsNullOrEmpty(text3))
			{
				text2 = text2 + "\n" + text3;
			}
			Speech.AnnounceScreen(text2);
			return true;
		}
		catch (Exception ex)
		{
			Log.Write("[ShopBuyHandler] " + ex.Message);
			return false;
		}
	}

	public string OnButtonFocused(SelectionButton button)
	{
		try
		{
			List<BuyButtonGroupWidget.IButtonWidget> list = ((ScreenDetector.GetFocusVC()?.TryCast<ShopBuyViewController>())?.m_BuyButtonGroupWidget)?.m_ButtonWidgets;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					BuyButtonGroupWidget.IButtonWidget buttonWidget = list[i];
					if (!(buttonWidget?.button == button))
					{
						continue;
					}
					BuyButtonGroupWidget.BuyButtonWidget buyButtonWidget = buttonWidget.TryCast<BuyButtonGroupWidget.BuyButtonWidget>();
					if (buyButtonWidget != null)
					{
						string text = buyButtonWidget.numText?.text?.Trim();
						string text2 = buyButtonWidget.priceText?.text?.Trim();
						string text3 = buyButtonWidget.infoText?.text?.Trim();
						string text4 = "";
						if (!string.IsNullOrEmpty(text))
						{
							text4 += text;
						}
						if (!string.IsNullOrEmpty(text3))
						{
							text4 += (string.IsNullOrEmpty(text4) ? text3 : (" " + text3));
						}
						if (!string.IsNullOrEmpty(text2))
						{
							text4 = text4 + "\nPrice: " + text2;
						}
						text4 += $"\n{i + 1} of {list.Count}";
						return string.IsNullOrEmpty(text4) ? null : text4;
					}
					BuyButtonGroupWidget.ActionButtonWidget actionButtonWidget = buttonWidget.TryCast<BuyButtonGroupWidget.ActionButtonWidget>();
					if (actionButtonWidget != null)
					{
						string text5 = actionButtonWidget.actionText?.text?.Trim();
						if (!string.IsNullOrEmpty(text5))
						{
							return text5;
						}
					}
				}
			}
		}
		catch
		{
		}
		try
		{
			ShopBuyViewController shopBuyViewController = ScreenDetector.GetFocusVC()?.TryCast<ShopBuyViewController>();
			List<HighlightWidget.IThumbWidget> list2 = shopBuyViewController?.m_HighlightWidget?.m_ThumbWidgets;
			List<int> list3 = shopBuyViewController?.m_CardBrowseMrks;
			List<int> list4 = shopBuyViewController?.m_CardBrowseRareList;
			if (list2 != null)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					if (!(list2[j]?.button == button))
					{
						continue;
					}
					string text6 = "";
					int num = ((list3 != null && j < list3.Count) ? list3[j] : 0);
					if (num > 0)
					{
						string text7 = Content.s_instance?.GetName(num);
						if (!string.IsNullOrEmpty(text7))
						{
							text6 = text7;
						}
					}
					int num2 = ((list4 != null && j < list4.Count) ? list4[j] : (-1));
					if (1 == 0)
					{
					}
					string text8 = num2 switch
					{
						1 => "Rare", 
						2 => "Super Rare", 
						3 => "Ultra Rare", 
						_ => null, 
					};
					if (1 == 0)
					{
					}
					string text9 = text8;
					if (!string.IsNullOrEmpty(text9))
					{
						text6 += (string.IsNullOrEmpty(text6) ? text9 : (", " + text9));
					}
					text6 += $"\n{j + 1} of {list2.Count}";
					return string.IsNullOrEmpty(text6.Trim()) ? null : text6;
				}
			}
		}
		catch
		{
		}
		return TextExtractor.ExtractFirst(button.gameObject);
	}
}
