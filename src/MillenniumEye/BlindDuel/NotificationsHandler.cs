// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

public class NotificationsHandler : IMenuHandler
{
	public bool CanHandle(string viewControllerName)
	{
		if (viewControllerName == "NotificationMenu" || viewControllerName == "Notifications")
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
		if (button.transform.Find("BaseCategory") != null)
		{
			return HomeHandler.ReadNotificationText(button);
		}
		return null;
	}
}
