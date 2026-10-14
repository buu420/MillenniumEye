// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppYgomGame.Card;
using UnityEngine;
using UnityEngine.UI;

namespace MillenniumEye;

public static class CardReader
{
	private static readonly (string path, Func<bool> condition, string attributePath)[] UiFallbackPaths = new(string, Func<bool>, string)[3]
	{
		("UI/ContentCanvas/ContentManager/DeckEdit/DeckEditUI(Clone)/CardDetail/Root/Window", () => GameObject.Find("UI/ContentCanvas/ContentManager/DeckEdit/") != null, "TitleArea/PlateTitle/IconAttribute"),
		("UI/ContentCanvas/ContentManager/DeckBrowser/DeckBrowserUI(Clone)/Root/CardDetail/Root/Window", () => GameObject.Find("UI/ContentCanvas/ContentManager/DeckBrowser/") != null, "TitleArea/AttributeRoot/IconAttribute"),
		("UI/ContentCanvas/ContentManager/DuelClient/CardInfo/CardInfo(Clone)/Root/Window", () => true, "TitleArea/AttributeRoot/IconAttribute")
	};

	private const string ItemPreviewPath = "UI/OverlayCanvas/DialogManager/ItemPreview/ItemPreviewUI(Clone)/Root/RootMainArea/DescArea/RootDesc/";

	private static readonly TextSearchOptions CardTextOptions = new TextSearchOptions
	{
		ActiveOnly = true,
		FilterBanned = false
	};

	public static CardData ReadCardFromData(int mrk)
	{
		CardData card = new CardData();
		MillenniumEyeCore.Preview.Card = card;
		Content s_instance = Content.s_instance;
		if (s_instance == null || mrk <= 0)
		{
			return card;
		}
		card.Name = s_instance.GetName(mrk);
		Content.Attribute attr = s_instance.GetAttr(mrk);
		card.Element = s_instance.GetAttributeText(attr);
		if (attr == Content.Attribute.Magic || attr == Content.Attribute.Trap)
		{
			card.SpellType = s_instance.GetIconFullText(mrk);
		}
		else
		{
			int atk = s_instance.GetAtk(mrk);
			card.Atk = ((atk >= 0) ? atk.ToString() : "?");
			Content.Frame frame = s_instance.GetFrame(mrk);
			if (frame == Content.Frame.Link)
			{
				card.Link = s_instance.GetLinkNum(mrk).ToString();
				card.LinkArrows = DeckHandler.FormatLinkArrows(s_instance.GetLinkMask(mrk));
			}
			else
			{
				int def = s_instance.GetDef(mrk);
				card.Def = ((def >= 0) ? def.ToString() : "?");
			}
			if (frame != Content.Frame.Link)
			{
				int rank = s_instance.GetRank(mrk);
				int star = s_instance.GetStar(mrk);
				if (rank > 0)
				{
					card.Rank = rank.ToString();
				}
				else if (star > 0)
				{
					card.Level = star.ToString();
				}
			}
			int scaleL = s_instance.GetScaleL(mrk);
			if (scaleL > 0)
			{
				card.PendulumScale = scaleL.ToString();
			}
			Content.Type type = s_instance.GetType(mrk);
			Content.Kind kind = s_instance.GetKind(mrk);
			string typeText = s_instance.GetTypeText(type);
			string kindText = s_instance.GetKindText(kind);
			if (!string.IsNullOrEmpty(typeText) && !string.IsNullOrEmpty(kindText))
			{
				card.Attributes = $"[{typeText}/{kindText}]";
			}
			else if (!string.IsNullOrEmpty(typeText))
			{
				card.Attributes = "[" + typeText + "]";
			}
			else if (!string.IsNullOrEmpty(kindText))
			{
				card.Attributes = "[" + kindText + "]";
			}
		}
		card.Description = s_instance.GetDesc(mrk);
		Log.Write($"[CardReader] Read from data: {card.Name} (mrk={mrk})");
		return card;
	}

	public static CardData ReadCurrentCard()
	{
		CardData card = MillenniumEyeCore.Preview.Card;
		if (CardBrowserState.IsOpen)
		{
			try
			{
				int currentMrk = CardBrowserState.GetCurrentMrk();
				if (currentMrk > 0)
				{
					return ReadCardFromData(currentMrk);
				}
			}
			catch (Exception ex)
			{
				Log.Write("[CardReader] CardBrowser data read failed: " + ex.Message);
			}
		}
		(string, Func<bool>, string)[] uiFallbackPaths = UiFallbackPaths;
		for (int i = 0; i < uiFallbackPaths.Length; i++)
		{
			var (text, func, text2) = uiFallbackPaths[i];
			try
			{
				if (!func())
				{
					continue;
				}
				GameObject gameObject = GameObject.Find(text);
				if (!(gameObject == null))
				{
					List<TextResult> list = TextExtractor.ExtractAll(gameObject, CardTextOptions);
					if (list.Count != 0)
					{
						MapTextsToCard(card, list);
						string gameObjectPath = text + "/" + text2;
						card.Element = ReadAttributeFromSprite(gameObjectPath);
						Log.Write("[CardReader] Read card: " + card.Name + " from " + text);
						return card;
					}
				}
			}
			catch (Exception ex2)
			{
				Log.Write("[CardReader] Error trying path: " + ex2.Message);
			}
		}
		Log.Write("[CardReader] No card info panel found");
		return card;
	}

	public static PreviewData ReadItemPreview()
	{
		PreviewData preview = MillenniumEyeCore.Preview;
		try
		{
			GameObject gameObject = GameObject.Find("UI/OverlayCanvas/DialogManager/ItemPreview/ItemPreviewUI(Clone)/Root/RootMainArea/DescArea/RootDesc/");
			if (gameObject == null)
			{
				return preview;
			}
			List<TextResult> list = TextExtractor.ExtractAll(gameObject, CardTextOptions);
			if (list.Count == 0)
			{
				return preview;
			}
			if (list.Count > 2)
			{
				preview.Name = list[0].Text + " - " + list[list.Count - 2].Text;
			}
			else if (list.Count >= 1)
			{
				preview.Name = list[list.Count - 2].Text;
			}
			if (list.Count >= 2)
			{
				preview.Description = list[list.Count - 1].Text;
			}
		}
		catch (Exception ex)
		{
			Log.Write("[CardReader] ItemPreview error: " + ex.Message);
		}
		return preview;
	}

	private static void MapTextsToCard(CardData card, List<TextResult> texts)
	{
		if (texts.Count > 0)
		{
			card.Name = texts[0].Text;
		}
		foreach (var (text3, text4) in texts)
		{
			string text5 = text3;
			if (text5.Contains("DescriptionValue", StringComparison.OrdinalIgnoreCase))
			{
				card.Description = text4;
			}
			else if (text5.Contains("Rank", StringComparison.OrdinalIgnoreCase))
			{
				card.Rank = text4;
			}
			else if (text5.Contains("Level", StringComparison.OrdinalIgnoreCase))
			{
				card.Level = text4;
			}
			else if (text5.Contains("Atk", StringComparison.OrdinalIgnoreCase))
			{
				card.Atk = text4;
			}
			else if (text5.Contains("Def", StringComparison.OrdinalIgnoreCase))
			{
				card.Def = text4;
			}
			else if (text5.Contains("Pendulum", StringComparison.OrdinalIgnoreCase))
			{
				card.PendulumScale = text4;
			}
			else if (text5.Contains("Link", StringComparison.OrdinalIgnoreCase))
			{
				card.Link = text4;
			}
			else if (text5.Contains("DescriptionItem", StringComparison.OrdinalIgnoreCase))
			{
				card.Attributes = text4;
			}
			else if (text5.Contains("SpellTrap", StringComparison.OrdinalIgnoreCase))
			{
				card.SpellType = text4;
			}
			else if (text5.Contains("CardNum", StringComparison.OrdinalIgnoreCase))
			{
				card.Owned = text4;
			}
		}
	}

	private static string ReadAttributeFromSprite(string gameObjectPath)
	{
		try
		{
			GameObject gameObject = GameObject.Find(gameObjectPath);
			if (gameObject == null)
			{
				return "";
			}
			Image component = gameObject.GetComponent<Image>();
			if (component == null || component.sprite == null)
			{
				return "";
			}
			return EnumUtil.ParseAttribute(component.sprite.name);
		}
		catch (Exception ex)
		{
			Log.Write("[CardReader] Sprite read error: " + ex.Message);
			return "";
		}
	}

	public static bool ReadAndSpeak(string suffix = null, bool queued = false)
	{
		MillenniumEyeCore.Preview.Clear();
		CardData cardData;
		if (NavigationState.IsInDuel)
		{
			int pendingMrk = PatchCardInfoSetDescription.PendingMrk;
			cardData = ((pendingMrk <= 0) ? ReadCurrentCard() : ReadCardFromData(pendingMrk));
			int pendingUniqueId = PatchCardInfoSetDescription.PendingUniqueId;
			if (pendingUniqueId > 0 && PatchCardInfoSetDescription.CheckAndUpdateDedup(pendingUniqueId))
			{
				Log.Write($"[CardReader] Suppressed duplicate read: {cardData.Name} (uid={pendingUniqueId})");
				return false;
			}
		}
		else
		{
			cardData = ReadCurrentCard();
		}
		string text = cardData.Format(NavigationState.IsInDuel, NavigationState.CurrentMenu == Menu.Deck);
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(suffix))
		{
			text += suffix;
		}
		if (queued)
		{
			Speech.SayQueued(text);
		}
		else
		{
			Speech.SayItem(text);
		}
		return true;
	}

	public static void SetPublicDuelDetails(int mrk)
	{
		DuelHandler.ResetSelectionDedup();
		DuelState.CardDetailLines = null;
		if (mrk <= 0) return;
		try
		{
			CardData card = ReadCardFromData(mrk);
			if (!string.IsNullOrWhiteSpace(card.Name))
				DuelState.CardDetailLines = card.GetDetailLines(out _);
		}
		catch (Exception ex) { Log.Write("[CardDetails] " + ex.Message); }
	}

	public static void SpeakCardFromData(int mrk, string zone, int? liveAtk = null, int? liveDef = null, bool queued = false, string battlePosition = null)
	{
		MillenniumEyeCore.Preview.Clear();
		CardData cardData = ReadCardFromData(mrk);
		if (liveAtk.HasValue)
		{
			cardData.Atk = ((liveAtk.Value >= 0) ? liveAtk.Value.ToString() : "?");
		}
		if (liveDef.HasValue && !cardData.IsLink)
		{
			cardData.Def = ((liveDef.Value >= 0) ? liveDef.Value.ToString() : "?");
		}
		DuelState.CardDetailLines = cardData.GetDetailLines(out _, battlePosition, zone);
		DuelState.CardDetailIndex = 0;
		string text = cardData.Format(isDuel: true, trimAttributes: false, battlePosition);
		if (string.IsNullOrEmpty(text))
		{
			if (!string.IsNullOrEmpty(zone))
			{
				if (queued)
				{
					Speech.SayQueued(zone);
				}
				else
				{
					Speech.SayItem(zone);
				}
			}
			return;
		}
		if (!string.IsNullOrEmpty(zone))
		{
			text = text + ", " + zone;
		}
		if (queued)
		{
			Speech.SayQueued(text);
		}
		else
		{
			Speech.SayItem(text);
		}
	}

	public static void ReadPreviewAndSpeak()
	{
		MillenniumEyeCore.Preview.Clear();
		PreviewData previewData = ReadItemPreview();
		if (!string.IsNullOrEmpty(previewData.Name))
		{
			string text = "Name: " + previewData.Name;
			if (!string.IsNullOrEmpty(previewData.Description))
			{
				text = text + "\nDescription: " + previewData.Description;
			}
			Speech.SayItem(text);
		}
	}
}
