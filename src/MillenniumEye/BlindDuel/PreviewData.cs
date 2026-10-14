// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
namespace MillenniumEye;

public class PreviewData
{
	public CardData Card { get; set; } = new CardData();

	public string Name { get; set; } = "";

	public string Description { get; set; } = "";

	public string TimeLeft { get; set; } = "";

	public string Price { get; set; } = "";

	public void Clear()
	{
		Card.Clear();
		string text = (Price = "");
		string text3 = (TimeLeft = text);
		string name = (Description = text3);
		Name = name;
	}
}
