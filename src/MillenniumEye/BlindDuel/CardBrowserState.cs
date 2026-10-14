// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.CardBrowser;
using Il2CppYgomSystem.UI;

namespace MillenniumEye;

public static class CardBrowserState
{
	public static CardBrowserViewController ViewController { get; set; }

	public static SnapContentManager SnapContentManager { get; set; }

	public static bool IsOpen => SnapContentManager != null;

	public static int CurrentPage => SnapContentManager?.currentPage ?? 0;

	public static int GetCurrentMrk()
	{
		List<CardBrowserViewController.CardContext> list = ViewController?.m_CardContexts;
		int currentPage = CurrentPage;
		if (list != null && currentPage >= 0 && currentPage < list.Count)
		{
			return list[currentPage].mrk;
		}
		return 0;
	}
}
