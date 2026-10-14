// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

public class SetupHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		switch (viewControllerName)
		{
		case "GameEntryV1":
		case "GameEntrySequenceV2":
		case "Enquete":
			return true;
		default:
			return false;
		}
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		return false;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		return null;
	}
}
