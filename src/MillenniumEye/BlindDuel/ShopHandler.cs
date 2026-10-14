// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using Il2CppYgomGame.Shop;
using Il2CppYgomGame.Utility;
using Il2CppYgomSystem.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MillenniumEye;

public class ShopHandler : IMenuHandler
{
	private int _lastCategoryId = -1;

	private int _lastSubCategoryId = -1;

	private int _lastSectionId = -1;

	private int _lastSubTabIdx = -1;

	public bool CanHandle(string viewControllerName)
	{
		if (viewControllerName == "Shop" || viewControllerName == "ShopMenu")
		{
			return true;
		}
		return false;
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		_lastCategoryId = -1;
		_lastSubCategoryId = -1;
		_lastSectionId = -1;
		_lastSubTabIdx = -1;
		string text = ScreenDetector.ReadGameHeaderText();
		string text2 = text ?? "Shop";
		try
		{
			ShopViewController.ShowcaseData showcaseData = GetShopVC()?.m_ShowcaseData;
			if (showcaseData != null)
			{
				int currentCategoryId = showcaseData.currentCategoryId;
				string text3 = showcaseData.GetCategoryData(currentCategoryId)?.labelText;
				if (!string.IsNullOrEmpty(text3))
				{
					text2 = text2 + ", " + text3;
				}
			}
		}
		catch
		{
		}
		Speech.AnnounceScreen(text2);
		return true;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		try
		{
			ShopViewController shopVC = GetShopVC();
			if (IsMainTab(button, shopVC))
			{
				_lastSubTabIdx = -1;
				return HandleCategoryTab(button, shopVC);
			}
			if (IsSubTab(button, shopVC))
			{
				return HandleSubcategoryTab(button, shopVC);
			}
			_lastSubTabIdx = -1;
			var (productWidget, index, total) = FindProduct(button, shopVC);
			if (productWidget != null)
			{
				return HandleProduct(productWidget, index, total, shopVC);
			}
			if (button.name == "CardPict")
			{
				return HandleCardInPack(button);
			}
			return null;
		}
		catch (Exception ex)
		{
			Log.Write("[ShopHandler] " + ex.Message);
			return null;
		}
	}

	private string HandleCategoryTab(SelectionButton button, ShopViewController shopVC)
	{
		string text = TextExtractor.ExtractFirst(button.gameObject);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		string text2 = text + ", tab";
		try
		{
			ShopViewController.ShowcaseData showcaseData = shopVC?.m_ShowcaseData;
			if (showcaseData != null)
			{
				int value = showcaseData.currentCategoryIdx + 1;
				int categoriesLength = showcaseData.GetCategoriesLength();
				if (categoriesLength > 1)
				{
					text2 += $", {value} of {categoriesLength}";
				}
			}
		}
		catch
		{
		}
		return text2;
	}

	private string HandleSubcategoryTab(SelectionButton button, ShopViewController shopVC)
	{
		try
		{
			ShopViewController.ShowcaseData showcaseData = shopVC?.m_ShowcaseData;
			if (showcaseData == null)
			{
				return null;
			}
			int currentCategoryId = showcaseData.currentCategoryId;
			int currentSubCategoryId = showcaseData.currentSubCategoryId;
			int sectionsLength = showcaseData.GetSectionsLength(currentCategoryId, currentSubCategoryId);
			int num = ((sectionsLength > 0) ? showcaseData.currentSectionIdx : 0);
			List<int> subcategories = showcaseData.GetSubcategories(currentCategoryId);
			if (subcategories == null)
			{
				return null;
			}
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < subcategories.Count; i++)
			{
				int num4 = subcategories[i];
				int sectionsLength2 = showcaseData.GetSectionsLength(currentCategoryId, num4);
				int num5 = ((sectionsLength2 <= 0) ? 1 : sectionsLength2);
				if (num4 == currentSubCategoryId)
				{
					num2 = num3 + ((sectionsLength2 <= 0) ? 1 : (num + 1));
				}
				num3 += num5;
			}
			if (num2 == _lastSubTabIdx)
			{
				return "";
			}
			_lastSubTabIdx = num2;
			string text = null;
			if (sectionsLength > 0)
			{
				int currentSectionId = showcaseData.currentSectionId;
				text = showcaseData.GetSectionData(currentCategoryId, currentSubCategoryId, currentSectionId)?.labelText;
			}
			else
			{
				text = showcaseData.GetSubCategoryData(currentCategoryId, currentSubCategoryId)?.labelText;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = TextExtractor.ExtractFirst(button.gameObject);
			}
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			_lastCategoryId = currentCategoryId;
			_lastSubCategoryId = currentSubCategoryId;
			return $"{text}, subtab, {num2} of {num3}";
		}
		catch
		{
		}
		string text2 = TextExtractor.ExtractFirst(button.gameObject);
		return (!string.IsNullOrEmpty(text2)) ? (text2 + ", subtab") : null;
	}

	private string HandleProduct(ProductWidget widget, int index, int total, ShopViewController shopVC)
	{
		ProductContext productContext = widget.productContext;
		string text = productContext?.productName;
		if (string.IsNullOrEmpty(text))
		{
			text = widget.nameText?.text?.Trim();
		}
		if (string.IsNullOrEmpty(text))
		{
			text = TextExtractor.ExtractFirst(widget.button?.gameObject);
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		string text2 = null;
		try
		{
			ShopViewController.ShowcaseData showcaseData = shopVC?.m_ShowcaseData;
			if (showcaseData != null)
			{
				int currentCategoryId = showcaseData.currentCategoryId;
				int currentSubCategoryId = showcaseData.currentSubCategoryId;
				int currentSectionId = showcaseData.currentSectionId;
				if (_lastCategoryId >= 0 && currentCategoryId != _lastCategoryId)
				{
					string text3 = showcaseData.GetCategoryData(currentCategoryId)?.labelText;
					int sectionsLength = showcaseData.GetSectionsLength(currentCategoryId, currentSubCategoryId);
					string text4 = ((sectionsLength <= 0) ? showcaseData.GetSubCategoryData(currentCategoryId, currentSubCategoryId)?.labelText : showcaseData.GetSectionData(currentCategoryId, currentSubCategoryId, currentSectionId)?.labelText);
					if (!string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text4))
					{
						text2 = text3 + ", " + text4;
					}
					else if (!string.IsNullOrEmpty(text3))
					{
						text2 = text3;
					}
					else if (!string.IsNullOrEmpty(text4))
					{
						text2 = text4;
					}
				}
				else if (_lastSubCategoryId >= 0 && currentSubCategoryId != _lastSubCategoryId)
				{
					int sectionsLength2 = showcaseData.GetSectionsLength(currentCategoryId, currentSubCategoryId);
					text2 = ((sectionsLength2 <= 0) ? showcaseData.GetSubCategoryData(currentCategoryId, currentSubCategoryId)?.labelText : showcaseData.GetSectionData(currentCategoryId, currentSubCategoryId, currentSectionId)?.labelText);
				}
				else if (_lastSectionId >= 0 && currentSectionId != _lastSectionId)
				{
					int sectionsLength3 = showcaseData.GetSectionsLength(currentCategoryId, currentSubCategoryId);
					if (sectionsLength3 > 0)
					{
						text2 = showcaseData.GetSectionData(currentCategoryId, currentSubCategoryId, currentSectionId)?.labelText;
					}
				}
				_lastCategoryId = currentCategoryId;
				_lastSubCategoryId = currentSubCategoryId;
				_lastSectionId = currentSectionId;
			}
		}
		catch
		{
		}
		string text5 = ((!string.IsNullOrEmpty(text2)) ? (text2 + ", " + text) : text);
		string text6 = productContext?.headLabelText;
		if (!string.IsNullOrEmpty(text6))
		{
			text5 = text5 + "\n" + text6;
		}
		if (productContext != null && productContext.isSoldOut)
		{
			text5 += "\nSold out";
		}
		try
		{
			GameObject packPickupMessageGroup = widget.packPickupMessageGroup;
			if (packPickupMessageGroup != null && packPickupMessageGroup.activeInHierarchy)
			{
				string text7 = widget.packPickupMessage?.text?.Trim();
				if (!string.IsNullOrEmpty(text7))
				{
					text5 = text5 + "\n" + text7;
				}
			}
		}
		catch
		{
		}
		string text8 = TextUtil.StripCJK(widget.limitRemainText?.text?.Trim());
		if (!string.IsNullOrEmpty(text8) && text8 != "00:00")
		{
			text5 = text5 + "\n" + text8;
		}
		TMP_Text priceFreeText = widget.priceFreeText;
		if (priceFreeText != null && priceFreeText.gameObject.activeInHierarchy)
		{
			string text9 = TextUtil.StripCJK(priceFreeText.text?.Trim());
			if (!string.IsNullOrEmpty(text9))
			{
				text5 = text5 + "\nFree: " + text9;
			}
		}
		try
		{
			PriceContext priceContext = productContext?.listButtonPrice;
			if (priceContext != null && priceContext.priceAmount > 0)
			{
				int priceAmount = priceContext.priceAmount;
				int payItemCategory = priceContext.payItemCategory;
				if (payItemCategory == 1)
				{
					text5 += $"\nPrice: {priceAmount} Gems";
				}
				else
				{
					string text10 = null;
					try
					{
						text10 = TextUtil.StripCJK(ItemUtil.GetItemName(priceContext.payItemIsPeriod, payItemCategory, priceContext.payItemId));
					}
					catch
					{
					}
					if (!string.IsNullOrEmpty(text10))
					{
						if (priceAmount == 1 && text10.EndsWith("s"))
						{
							text10 = text10.Substring(0, text10.Length - 1);
						}
						else if (priceAmount > 1 && !text10.EndsWith("s"))
						{
							text10 += "s";
						}
					}
					text5 += ((!string.IsNullOrEmpty(text10)) ? $"\nPrice: {priceAmount} {text10}" : $"\nPrice: {priceAmount} Tickets");
				}
			}
		}
		catch
		{
			string text11 = widget.priceText?.text?.Trim();
			if (!string.IsNullOrEmpty(text11))
			{
				text5 = text5 + "\nPrice: " + text11;
			}
		}
		if (total > 1 && index > 0)
		{
			text5 += $"\n{index} of {total}";
		}
		return text5;
	}

	private string HandleCardInPack(SelectionButton button)
	{
		Transform transform = button.transform.parent?.Find("NumTextArea");
		string text = ((transform != null) ? TextExtractor.ExtractFirst(transform.gameObject) : "");
		if (!string.IsNullOrEmpty(text))
		{
			string text2 = text;
			text = "x" + text2.Substring(1, text2.Length - 1);
		}
		Transform transform2 = button.transform.parent?.Find("IconRarity");
		string value = "";
		if (transform2 != null)
		{
			value = EnumUtil.ParseRarity(transform2.GetComponent<Image>()?.sprite?.name);
		}
		Transform transform3 = button.transform.parent?.Find("NewIcon");
		bool flag = transform3 != null && transform3.gameObject.activeInHierarchy;
		return $"Rarity: {value}, New: {(flag ? "Yes" : "No")}, Owned: {text}";
	}

	private static ShopViewController GetShopVC()
	{
		try
		{
			return ScreenDetector.GetFocusVC()?.TryCast<ShopViewController>();
		}
		catch
		{
			return null;
		}
	}

	private static bool IsMainTab(SelectionButton button, ShopViewController shopVC)
	{
		try
		{
			Dictionary<GameObject, ShopTabWidget> dictionary = shopVC?.m_MainTabList?.m_TabWidgetMap;
			if (dictionary == null)
			{
				return false;
			}
			foreach (KeyValuePair<GameObject, ShopTabWidget> item in dictionary)
			{
				if (item.Value?.button == button)
				{
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static bool IsSubTab(SelectionButton button, ShopViewController shopVC)
	{
		try
		{
			Dictionary<GameObject, ISubTabWidget> dictionary = shopVC?.m_SubTabList?.m_TabWidgetMap;
			if (dictionary == null)
			{
				return false;
			}
			Transform transform = null;
			Dictionary<GameObject, ISubTabWidget>.Enumerator enumerator = dictionary.GetEnumerator();
			if (enumerator.MoveNext())
			{
				transform = enumerator.Current.Key?.transform?.parent;
			}
			if (transform == null)
			{
				return false;
			}
			Transform parent = button.transform.parent;
			while (parent != null)
			{
				if (parent == transform)
				{
					return true;
				}
				parent = parent.parent;
			}
		}
		catch
		{
		}
		return false;
	}

	private static (ProductWidget widget, int index, int total) FindProduct(SelectionButton button, ShopViewController shopVC)
	{
		try
		{
			Dictionary<GameObject, ProductContainerWidget> dictionary = shopVC?.m_ProductList?.m_ContainerWidgetMap;
			if (dictionary == null)
			{
				return (widget: null, index: 0, total: 0);
			}
			ProductWidget productWidget = null;
			foreach (KeyValuePair<GameObject, ProductContainerWidget> item in dictionary)
			{
				List<ProductWidget> list = item.Value?.productWidgets;
				if (list == null)
				{
					continue;
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i]?.button == button)
					{
						productWidget = list[i];
						break;
					}
				}
				if (productWidget == null)
				{
					continue;
				}
				break;
			}
			if (productWidget == null)
			{
				return (widget: null, index: 0, total: 0);
			}
			int num = productWidget.productContext?.shopId ?? (-1);
			if (num < 0)
			{
				return (widget: productWidget, index: 0, total: 0);
			}
			List<ProductContainerWidget.Context> list2 = shopVC?.m_ProductList?.m_ProductContainerCtxs;
			if (list2 == null)
			{
				return (widget: productWidget, index: 0, total: 0);
			}
			int num2 = 0;
			int num3 = -1;
			for (int j = 0; j < list2.Count; j++)
			{
				List<ProductContext> list3 = list2[j]?.productCtxs;
				if (list3 == null)
				{
					continue;
				}
				for (int k = 0; k < list3.Count; k++)
				{
					num2++;
					if (num3 < 0)
					{
						ProductContext productContext = list3[k];
						if (productContext != null && productContext.shopId == num)
						{
							num3 = num2;
						}
					}
				}
			}
			return (widget: productWidget, index: (num3 > 0) ? num3 : 0, total: num2);
		}
		catch
		{
			return (widget: null, index: 0, total: 0);
		}
	}
}
