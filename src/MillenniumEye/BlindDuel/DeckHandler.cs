// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.Deck;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public class DeckHandler : IMenuHandler
{
	private DeckEditViewController2.ViewType _lastViewType = DeckEditViewController2.ViewType.None;

	private DeckCard.LocationInDeck _lastDeckLocation = DeckCard.LocationInDeck.NA;

	public bool CanHandle(string viewControllerName)
	{
		switch (viewControllerName)
		{
		case "DeckSelect":
		case "DeckEdit":
		case "DeckBrowser":
			return true;
		default:
			return false;
		}
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		_lastViewType = DeckEditViewController2.ViewType.None;
		_lastDeckLocation = DeckCard.LocationInDeck.NA;
		if (viewControllerName == "DeckEdit")
		{
			return AnnounceDeckEdit();
		}
		if (viewControllerName == "DeckSelect")
		{
			return AnnounceDeckSelect();
		}
		return false;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		try
		{
			DeckBox componentInParent = button.GetComponentInParent<DeckBox>();
			if (componentInParent != null)
			{
				return HandleDeckBox(componentInParent);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[DeckHandler] DeckBox: " + ex.Message);
		}
		try
		{
			CardActionMenu componentInParent2 = button.GetComponentInParent<CardActionMenu>();
			if (componentInParent2 != null)
			{
				return HandleActionMenu(button, componentInParent2);
			}
		}
		catch (Exception ex2)
		{
			Log.Write("[DeckHandler] ActionMenu: " + ex2.Message);
		}
		try
		{
			CardBase componentInParent3 = button.GetComponentInParent<CardBase>();
			if (componentInParent3 != null)
			{
				return HandleCard(componentInParent3);
			}
		}
		catch (Exception ex3)
		{
			Log.Write("[DeckHandler] Card: " + ex3.Message);
		}
		try
		{
			switch (button.transform.parent?.parent?.parent?.name)
			{
			case "Category":
			{
				string text = TextExtractor.ExtractFirst(button.transform.parent?.parent?.gameObject);
				string text2 = TextExtractor.ExtractFirst(button.gameObject);
				return text2 + ", category: " + text;
			}
			case "InputButton":
				return "Rename deck button";
			case "AutoBuildButton":
				return "Auto-build button";
			}
		}
		catch (Exception ex4)
		{
			Log.Write("[DeckHandler] Filter: " + ex4.Message);
		}
		return null;
	}

	private bool AnnounceDeckSelect()
	{
		string text = ScreenDetector.ReadGameHeaderText();
		string text2 = text ?? "Deck";
		try
		{
			DeckSelectViewController2 deckSelectVC = GetDeckSelectVC();
			if (deckSelectVC != null)
			{
				string text3 = deckSelectVC.m_TextDeckNum?.text?.Trim();
				if (!string.IsNullOrEmpty(text3))
				{
					text2 = text2 + ", " + text3;
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[DeckHandler] DeckSelect announce: " + ex.Message);
		}
		Speech.AnnounceScreen(text2);
		return true;
	}

	private bool AnnounceDeckEdit()
	{
		try
		{
			DeckEditViewController2 deckEditVC = GetDeckEditVC();
			if (deckEditVC != null)
			{
				string deckName = deckEditVC.m_DeckName;
				int value = deckEditVC.m_MainDeckCards?.Count ?? 0;
				int value2 = deckEditVC.m_ExtraDeckCards?.Count ?? 0;
				string text = ((!string.IsNullOrEmpty(deckName)) ? deckName : "Deck Editor");
				text += $", main {value}, extra {value2}";
				Speech.AnnounceScreen(text);
				return true;
			}
		}
		catch (Exception ex)
		{
			Log.Write("[DeckHandler] DeckEdit announce: " + ex.Message);
		}
		Speech.AnnounceScreen("Deck Editor");
		return true;
	}

	private string HandleDeckBox(DeckBox deckBox)
	{
		if (deckBox.m_Condition == DeckSelectViewController2.DeckCondition.New)
		{
			return "New deck";
		}
		string text = deckBox.deckName;
		if (string.IsNullOrEmpty(text))
		{
			text = deckBox.m_DeckNameText?.text?.Trim();
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		string text2 = text;
		try
		{
			if (deckBox.m_CurrentDeckIcon != null && deckBox.m_CurrentDeckIcon.activeInHierarchy)
			{
				text2 += ", current";
			}
		}
		catch
		{
		}
		try
		{
			if (deckBox.m_DisabledIcon != null && deckBox.m_DisabledIcon.activeInHierarchy)
			{
				text2 += ", disabled";
			}
		}
		catch
		{
		}
		try
		{
			DeckSelectViewController2 deckSelectVC = GetDeckSelectVC();
			if (deckSelectVC?.m_Decks != null)
			{
				int deckID = deckBox.deckID;
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < deckSelectVC.m_Decks.Count; i++)
				{
					DeckSelectViewController2.DeckReference deckReference = deckSelectVC.m_Decks[i];
					if (deckReference.deckType == DeckSelectViewController2.DeckEventType.MyDeck)
					{
						num2++;
						if (deckReference.deckID == deckID)
						{
							num = num2;
						}
					}
				}
				if (num > 0)
				{
					text2 += $"\n{num} of {num2}";
				}
			}
		}
		catch
		{
		}
		return text2;
	}

	private string HandleActionMenu(SelectionButton button, CardActionMenu actionMenu)
	{
		string text = null;
		CardActionMenu.MenuArea menuArea = actionMenu.m_MenuArea;
		if (menuArea != null)
		{
			if (button == menuArea.m_BookmarkButton)
			{
				text = ((menuArea.m_BookmarkOn != null && menuArea.m_BookmarkOn.gameObject.activeInHierarchy) ? "Bookmark, on" : "Bookmark, off");
			}
			else if (button == menuArea.m_HowToGetButton)
			{
				text = "How to obtain";
			}
			else if (button == menuArea.m_RelatedCardButton)
			{
				text = "Related cards";
			}
			else if (button == menuArea.m_AddCardButton)
			{
				text = "Add to deck";
			}
			else if (button == menuArea.m_RemoveCardButton)
			{
				text = "Remove from deck";
			}
		}
		CardActionMenu.CraftArea craftArea = actionMenu.m_CraftArea;
		if (text == null && craftArea != null)
		{
			if (button == craftArea.CreateButton)
			{
				text = ReadCraftButton(craftArea.CraftCreateButton, "Generate");
			}
			else if (button == craftArea.DismantleButton)
			{
				text = ReadCraftButton(craftArea.CraftDismantleButton, "Dismantle");
			}
		}
		CardActionMenu.CardArea cardArea = actionMenu.m_CardArea;
		if (text == null && cardArea != null && button == cardArea.m_CardImageButton)
		{
			text = "Card image";
		}
		if (text == null)
		{
			return null;
		}
		try
		{
			Il2CppSystem.Collections.Generic.List<SelectionItem> list = (menuArea?.m_Selector)?.items;
			if (list != null && list.Count > 1)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i]?.TryCast<SelectionButton>() == button)
					{
						text += $"\n{i + 1} of {list.Count}";
						break;
					}
				}
			}
		}
		catch
		{
		}
		return text;
	}

	private static string ReadCraftButton(CardActionMenu.CraftArea.CraftButtonWidget widget, string label)
	{
		if (widget == null)
		{
			return label;
		}
		string text = widget.m_ButtonText?.text?.Trim();
		if (!string.IsNullOrEmpty(text))
		{
			label = text;
		}
		string text2 = widget.m_TextCP?.text?.Trim();
		if (!string.IsNullOrEmpty(text2))
		{
			string value = "";
			try
			{
				Sprite sprite = widget.m_IconCP?.sprite;
				if (sprite != null)
				{
					value = EnumUtil.ParseRarity(sprite.name);
				}
			}
			catch
			{
			}
			label += ((!string.IsNullOrEmpty(value)) ? $", {text2} {value} CP" : (", " + text2 + " CP"));
		}
		try
		{
			if (widget.m_IconDisabled != null && widget.m_IconDisabled.gameObject.activeInHierarchy)
			{
				label += ", unavailable";
			}
			else if (widget.m_IconLocked != null && widget.m_IconLocked.gameObject.activeInHierarchy)
			{
				label += ", locked";
			}
		}
		catch
		{
		}
		return label;
	}

	private string HandleCard(CardBase cardBase)
	{
		CardBaseData baseData = cardBase.m_BaseData;
		int cardID_k__BackingField = baseData._CardID_k__BackingField;
		Content s_instance = Content.s_instance;
		if (s_instance == null || cardID_k__BackingField <= 0)
		{
			return null;
		}
		string name = s_instance.GetName(cardID_k__BackingField);
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		string text = null;
		int index = -1;
		int total = 0;
		try
		{
			DeckEditViewController2 deckEditVC = GetDeckEditVC();
			if (deckEditVC != null)
			{
				DeckEditViewController2.ViewType currentView = deckEditVC.currentView;
				switch (currentView)
				{
				case DeckEditViewController2.ViewType.Deck:
				{
					DeckCard.LocationInDeck locationInDeck = (cardBase.TryCast<DeckCard>() ?? cardBase.GetComponentInParent<DeckCard>())?.m_Location ?? DeckCard.LocationInDeck.NA;
					if (_lastViewType != currentView || _lastDeckLocation != locationInDeck)
					{
						if (1 == 0)
						{
						}
						string text2 = locationInDeck switch
						{
							DeckCard.LocationInDeck.M => "Main Deck", 
							DeckCard.LocationInDeck.E => "Extra Deck", 
							_ => "Deck", 
						};
						if (1 == 0)
						{
						}
						text = text2;
					}
					Il2CppSystem.Collections.Generic.List<CardBaseData> list = ((locationInDeck == DeckCard.LocationInDeck.E) ? deckEditVC.m_ExtraDeckCards : deckEditVC.m_MainDeckCards);
					total = list?.Count ?? 0;
					index = GetDeckCardIndex(cardBase, list, cardID_k__BackingField, baseData._PremiumID_k__BackingField);
					_lastDeckLocation = locationInDeck;
					break;
				}
				case DeckEditViewController2.ViewType.CardCollection:
				{
					if (_lastViewType != currentView)
					{
						text = "Card Collection";
					}
					Il2CppSystem.Collections.Generic.List<CardBaseData> cardCollection = deckEditVC.m_CardCollection;
					total = cardCollection?.Count ?? 0;
					index = FindCollectionIndex(cardCollection, cardID_k__BackingField, baseData._PremiumID_k__BackingField);
					_lastDeckLocation = DeckCard.LocationInDeck.NA;
					break;
				}
				}
				_lastViewType = currentView;
			}
		}
		catch (Exception ex)
		{
			Log.Write("[DeckHandler] Section: " + ex.Message);
		}
		string text3 = "";
		if (text != null)
		{
			text3 = text + ", ";
		}
		bool canDismantle = false;
		try
		{
			canDismantle = CardCollectionInfo.IsDismantleable(baseData);
		}
		catch
		{
		}
		return text3 + BuildCardSpeech(cardID_k__BackingField, baseData._Rarity_k__BackingField, baseData._PremiumID_k__BackingField, baseData._Inventory_k__BackingField, index, total, canDismantle);
	}

	public static string BuildCardSpeech(int cardId, int rarityId, int premiumId, int owned = -1, int index = -1, int total = 0, bool canDismantle = false)
	{
		Content s_instance = Content.s_instance;
		if (s_instance == null || cardId <= 0)
		{
			return null;
		}
		string name = s_instance.GetName(cardId);
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		string text = name;
		string cardRarityText = CardCollectionInfo.GetCardRarityText(rarityId);
		if (!string.IsNullOrEmpty(cardRarityText))
		{
			text = text + ", " + cardRarityText;
		}
		string cardStyleText = CardCollectionInfo.GetCardStyleText(premiumId);
		if (!string.IsNullOrEmpty(cardStyleText))
		{
			text = text + ", " + cardStyleText;
		}
		Content.Attribute attr = s_instance.GetAttr(cardId);
		if (attr == Content.Attribute.Magic || attr == Content.Attribute.Trap)
		{
			string iconFullText = s_instance.GetIconFullText(cardId);
			if (!string.IsNullOrEmpty(iconFullText))
			{
				text = text + ", " + iconFullText;
			}
		}
		else
		{
			Content.Frame frame = s_instance.GetFrame(cardId);
			if (frame != Content.Frame.Link)
			{
				int rank = s_instance.GetRank(cardId);
				int star = s_instance.GetStar(cardId);
				if (rank > 0)
				{
					text += $", Rank {rank}";
				}
				else if (star > 0)
				{
					text += $", Level {star}";
				}
			}
			int atk = s_instance.GetAtk(cardId);
			text = text + ", Attack " + ((atk >= 0) ? atk.ToString() : "?");
			if (frame == Content.Frame.Link)
			{
				text += $", Link {s_instance.GetLinkNum(cardId)}";
				string text2 = FormatLinkArrows(s_instance.GetLinkMask(cardId));
				if (!string.IsNullOrEmpty(text2))
				{
					text = text + ", " + text2;
				}
			}
			else
			{
				int def = s_instance.GetDef(cardId);
				text = text + ", Defense " + ((def >= 0) ? def.ToString() : "?");
			}
			int scaleL = s_instance.GetScaleL(cardId);
			if (scaleL > 0)
			{
				text += $", Pendulum scale {scaleL}";
			}
		}
		string attributeText = s_instance.GetAttributeText(attr);
		if (!string.IsNullOrEmpty(attributeText))
		{
			text = text + ", " + attributeText;
		}
		Content.Type type = s_instance.GetType(cardId);
		Content.Kind kind = s_instance.GetKind(cardId);
		string typeText = s_instance.GetTypeText(type);
		string kindText = s_instance.GetKindText(kind);
		if (!string.IsNullOrEmpty(typeText) && !string.IsNullOrEmpty(kindText))
		{
			text = text + ", " + typeText + "/" + kindText;
		}
		else if (!string.IsNullOrEmpty(typeText))
		{
			text = text + ", " + typeText;
		}
		if (owned >= 0)
		{
			text += $", owned {owned}";
		}
		try
		{
			DeckEditViewController2 deckEditVC = GetDeckEditVC();
			if (deckEditVC != null)
			{
				int value = CountCardInDeck(deckEditVC, cardId);
				text += $", in deck {value}";
			}
		}
		catch
		{
		}
		if (total > 0 && index >= 0)
		{
			text += $"\n{index + 1} of {total},";
		}
		try
		{
			string desc = s_instance.GetDesc(cardId);
			if (!string.IsNullOrEmpty(desc))
			{
				text = text + "\n" + desc;
			}
		}
		catch
		{
		}
		return text + (canDismantle ? ", can dismantle" : ", cannot dismantle");
	}

	private static int GetDeckCardIndex(CardBase cardBase, Il2CppSystem.Collections.Generic.List<CardBaseData> deckList, int cardId, int premiumId)
	{
		if (deckList == null)
		{
			return -1;
		}
		int num = 0;
		try
		{
			Transform parent = cardBase.transform.parent;
			if (parent != null)
			{
				for (int i = 0; i < parent.childCount; i++)
				{
					Transform child = parent.GetChild(i);
					if (child.gameObject.activeInHierarchy)
					{
						if (child == cardBase.transform)
						{
							break;
						}
						CardBase component = child.GetComponent<CardBase>();
						if (component != null && component.m_BaseData._CardID_k__BackingField == cardId && component.m_BaseData._PremiumID_k__BackingField == premiumId)
						{
							num++;
						}
					}
				}
			}
		}
		catch
		{
		}
		int num2 = 0;
		for (int j = 0; j < deckList.Count; j++)
		{
			CardBaseData cardBaseData = deckList[j];
			if (cardBaseData._CardID_k__BackingField == cardId && cardBaseData._PremiumID_k__BackingField == premiumId)
			{
				if (num2 == num)
				{
					return j;
				}
				num2++;
			}
		}
		return -1;
	}

	public static int FindCollectionIndex(Il2CppSystem.Collections.Generic.List<CardBaseData> collection, int cardId, int premiumId)
	{
		if (collection == null)
		{
			return -1;
		}
		for (int i = 0; i < collection.Count; i++)
		{
			CardBaseData cardBaseData = collection[i];
			if (cardBaseData._CardID_k__BackingField == cardId && cardBaseData._PremiumID_k__BackingField == premiumId)
			{
				return i;
			}
		}
		return -1;
	}

	private static int CountCardInDeck(DeckEditViewController2 vc, int cardId)
	{
		int num = 0;
		Il2CppSystem.Collections.Generic.List<CardBaseData> mainDeckCards = vc.m_MainDeckCards;
		if (mainDeckCards != null)
		{
			for (int i = 0; i < mainDeckCards.Count; i++)
			{
				if (mainDeckCards[i]._CardID_k__BackingField == cardId)
				{
					num++;
				}
			}
		}
		Il2CppSystem.Collections.Generic.List<CardBaseData> extraDeckCards = vc.m_ExtraDeckCards;
		if (extraDeckCards != null)
		{
			for (int j = 0; j < extraDeckCards.Count; j++)
			{
				if (extraDeckCards[j]._CardID_k__BackingField == cardId)
				{
					num++;
				}
			}
		}
		return num;
	}

	public static string FormatLinkArrows(int mask)
	{
		if (mask == 0)
		{
			return null;
		}
		System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
		if ((mask & 1) != 0)
		{
			list.Add("up left");
		}
		if ((mask & 2) != 0)
		{
			list.Add("up");
		}
		if ((mask & 4) != 0)
		{
			list.Add("up right");
		}
		if ((mask & 8) != 0)
		{
			list.Add("left");
		}
		if ((mask & 0x10) != 0)
		{
			list.Add("right");
		}
		if ((mask & 0x20) != 0)
		{
			list.Add("down left");
		}
		if ((mask & 0x40) != 0)
		{
			list.Add("down");
		}
		if ((mask & 0x80) != 0)
		{
			list.Add("down right");
		}
		return (list.Count > 0) ? string.Join(", ", list) : null;
	}

	private static DeckSelectViewController2 GetDeckSelectVC()
	{
		return ScreenDetector.GetFocusVC()?.TryCast<DeckSelectViewController2>();
	}

	private static DeckEditViewController2 GetDeckEditVC()
	{
		return ScreenDetector.GetFocusVC()?.TryCast<DeckEditViewController2>();
	}
}
