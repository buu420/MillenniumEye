// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

public class FriendsHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		if (viewControllerName == "FriendsMenu" || viewControllerName == "Friends")
		{
			return true;
		}
		return false;
	}

	public bool OnScreenEntered(string viewControllerName)
	{
		return false;
	}

	public string OnButtonFocused(SelectionButton button)
	{
		if (button.name == "SearchButton")
		{
			return "Add friend button";
		}
		if (button.name == "OpenToggle")
		{
			return TextExtractor.ExtractFirst(button.transform.parent?.gameObject);
		}
		return null;
	}
}
