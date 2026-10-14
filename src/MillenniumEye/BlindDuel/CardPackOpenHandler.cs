// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppYgomGame.Card;
using Il2CppYgomGame.CardPack.Open.Actor;
using Il2CppYgomGame.Menu.Common;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public class CardPackOpenHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		if (viewControllerName == "CardPackOpen" || viewControllerName == "CardPackOpenResult")
		{
			return true;
		}
		return false;
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		if (viewControllerName == "CardPackOpenResult")
		{
			Speech.AnnounceScreen("Pack Results");
		}
		else
		{
			PackFlipTracker.Reset();
			Speech.AnnounceScreen("Pack Opening");
		}
		return true;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		try
		{
			string text = TextExtractor.ExtractFirst(button.gameObject);
			if (text != null && text.Contains("Related"))
			{
				return "";
			}
		}
		catch
		{
		}
		try
		{
			CardPackCardActor cardPackCardActor = FindActor(button);
			if (cardPackCardActor != null)
			{
				int mrk_k__BackingField = cardPackCardActor._mrk_k__BackingField;
				if (PackFlipTracker.IsFlipped(mrk_k__BackingField) && mrk_k__BackingField > 0)
				{
					string text2 = CardReader.ReadCardFromData(mrk_k__BackingField)?.Name;
					if (string.IsNullOrEmpty(text2))
					{
						return null;
					}
					string text3 = text2;
					try
					{
						string cardRarityText = CardCollectionInfo.GetCardRarityText((int)CardCollectionInfo.GetCardRarity(mrk_k__BackingField));
						if (!string.IsNullOrEmpty(cardRarityText))
						{
							text3 = text3 + ", " + cardRarityText;
						}
					}
					catch
					{
					}
					return text3;
				}
				return "Face-down card";
			}
			int num = FindResultMrk(button, out var isNew);
			if (num > 0)
			{
				string text4 = CardReader.ReadCardFromData(num)?.Name;
				if (!string.IsNullOrEmpty(text4))
				{
					string text5 = text4;
					try
					{
						string cardRarityText2 = CardCollectionInfo.GetCardRarityText((int)CardCollectionInfo.GetCardRarity(num));
						if (!string.IsNullOrEmpty(cardRarityText2))
						{
							text5 = text5 + ", " + cardRarityText2;
						}
					}
					catch
					{
					}
					return text5 + (isNew ? ", New" : ", Owned");
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[CardPackOpen] " + ex.Message);
		}
		return null;
	}

	private static CardPackCardActor FindActor(SelectionButton button)
	{
		Transform transform = button.transform;
		for (int i = 0; i < 5; i++)
		{
			if (!(transform != null))
			{
				break;
			}
			try
			{
				CardPackCardActor component = transform.GetComponent<CardPackCardActor>();
				if (component != null)
				{
					return component;
				}
			}
			catch
			{
			}
			transform = transform.parent;
		}
		return null;
	}

	private static int FindResultMrk(SelectionButton button, out bool isNew)
	{
		isNew = false;
		try
		{
			BindingCardMaterial component = button.GetComponent<BindingCardMaterial>();
			if (component != null)
			{
				int cardId = component.m_CardId;
				if (cardId > 0)
				{
					Transform parent = button.transform.parent;
					if (parent != null)
					{
						Transform transform = parent.Find("NewIcon");
						if (transform != null)
						{
							isNew = transform.gameObject.activeInHierarchy;
						}
					}
					return cardId;
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[CardPackResult] " + ex.Message);
		}
		return 0;
	}
}
