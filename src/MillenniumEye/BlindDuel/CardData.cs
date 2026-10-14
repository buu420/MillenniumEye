// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System.Collections.Generic;
using UnityEngine;

namespace MillenniumEye;

public class CardData
{
	public string Name { get; set; } = "";

	public string Description { get; set; } = "";

	public string Level { get; set; } = "";

	public string Rank { get; set; } = "";

	public string Atk { get; set; } = "";

	public string Def { get; set; } = "";

	public string PendulumScale { get; set; } = "";

	public string Link { get; set; } = "";

	public string LinkArrows { get; set; } = "";

	public string Element { get; set; } = "";

	public string Attributes { get; set; } = "";

	public string SpellType { get; set; } = "";

	public string Owned { get; set; } = "";

	public bool IsInHand { get; set; } = true;

	public GameObject CardObject { get; set; }

	public bool IsMonster => !string.IsNullOrEmpty(Atk);

	public bool IsLink => !string.IsNullOrEmpty(Link);

	public void Clear()
	{
		string text = (Owned = "");
		string text3 = (SpellType = text);
		string text5 = (Attributes = text3);
		string text7 = (Element = text5);
		string text9 = (LinkArrows = text7);
		string text11 = (Link = text9);
		string text13 = (PendulumScale = text11);
		string text15 = (Def = text13);
		string text17 = (Atk = text15);
		string text19 = (Rank = text17);
		string text21 = (Level = text19);
		string name = (Description = text21);
		Name = name;
		IsInHand = true;
		CardObject = null;
	}

	public string Format(bool isDuel = false, bool trimAttributes = false, string battlePosition = null)
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			return "";
		}
		List<string> list = new List<string> { Name };
		if (!string.IsNullOrEmpty(Atk))
		{
			list.Add("Attack: " + Atk);
		}
		if (!string.IsNullOrEmpty(Link))
		{
			string text = "Link rating: " + Link;
			if (!string.IsNullOrEmpty(LinkArrows))
			{
				text = text + ", " + LinkArrows;
			}
			list.Add(text);
		}
		if (!IsLink && !string.IsNullOrEmpty(Def))
		{
			list.Add("Defense: " + Def);
		}
		if (!string.IsNullOrEmpty(battlePosition))
		{
			list.Add(battlePosition);
		}
		if (!string.IsNullOrEmpty(Rank))
		{
			list.Add("Rank: " + Rank);
		}
		if (!string.IsNullOrEmpty(Level))
		{
			list.Add("Level: " + Level);
		}
		if (!string.IsNullOrEmpty(Element))
		{
			list.Add("Element: " + Element);
		}
		if (!string.IsNullOrEmpty(PendulumScale))
		{
			list.Add("Pendulum scale: " + PendulumScale);
		}
		if (!string.IsNullOrEmpty(Attributes))
		{
			string text2;
			if (!trimAttributes || Attributes.Length <= 2)
			{
				text2 = Attributes;
			}
			else
			{
				string attributes = Attributes;
				text2 = attributes.Substring(1, attributes.Length - 1 - 1);
			}
			string text3 = text2;
			list.Add("Attributes: " + text3);
		}
		if (!IsMonster && !string.IsNullOrEmpty(SpellType))
		{
			list.Add("Spell type: " + SpellType);
		}
		if (!string.IsNullOrEmpty(Owned))
		{
			list.Add("Owned: " + Owned);
		}
		if (!string.IsNullOrEmpty(Description))
		{
			list.Add("Description: " + Description);
		}
		return string.Join("\n", list);
	}

	public List<string> GetDetailLines(out string summary, string battlePosition = null, string zone = null)
	{
		List<string> list = new List<string>();
		if (!string.IsNullOrEmpty(Name))
		{
			list.Add(Name);
		}
		if (!string.IsNullOrEmpty(battlePosition))
		{
			list.Add(battlePosition);
		}
		if (!string.IsNullOrEmpty(battlePosition) && !string.IsNullOrEmpty(Atk))
		{
			list.Add("Attack: " + Atk);
		}
		if (!string.IsNullOrEmpty(battlePosition) && !IsLink && !string.IsNullOrEmpty(Def))
		{
			list.Add("Defense: " + Def);
		}
		if (!string.IsNullOrEmpty(zone))
		{
			list.Add(zone);
		}
		summary = string.Join(", ", list);
		List<string> list2 = new List<string>();
		if (!string.IsNullOrEmpty(Name))
		{
			list2.Add(Name);
		}
		if (!string.IsNullOrEmpty(Atk) && !IsLink && !string.IsNullOrEmpty(Def))
		{
			list2.Add("Attack: " + Atk + ", Defense: " + Def);
		}
		else if (!string.IsNullOrEmpty(Atk))
		{
			list2.Add("Attack: " + Atk);
		}
		if (!string.IsNullOrEmpty(Link))
		{
			string text = "Link rating: " + Link;
			if (!string.IsNullOrEmpty(LinkArrows))
			{
				text = text + ", " + LinkArrows;
			}
			list2.Add(text);
		}
		if (!string.IsNullOrEmpty(Rank))
		{
			list2.Add("Rank: " + Rank);
		}
		if (!string.IsNullOrEmpty(Level))
		{
			list2.Add("Level: " + Level);
		}
		if (!string.IsNullOrEmpty(Element))
		{
			list2.Add("Element: " + Element);
		}
		if (!string.IsNullOrEmpty(PendulumScale))
		{
			list2.Add("Pendulum scale: " + PendulumScale);
		}
		if (!string.IsNullOrEmpty(Attributes))
		{
			list2.Add("Attributes: " + Attributes);
		}
		if (!IsMonster && !string.IsNullOrEmpty(SpellType))
		{
			list2.Add("Spell type: " + SpellType);
		}
		if (!string.IsNullOrEmpty(Description))
		{
			list2.Add("Description: " + Description);
		}
		return list2;
	}
}
