// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

public interface IMenuHandler
{
	bool CanHandle(string viewControllerName);

	bool OnScreenEntered(string viewControllerName);

	string OnButtonFocused(SelectionButton button);
}
