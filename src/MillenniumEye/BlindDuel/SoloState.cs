// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.Solo;

namespace MillenniumEye;

public static class SoloState
{
	public struct GateInfo
	{
		public int GateID;

		public string Name;

		public string Overview;

		public bool IsClear;

		public bool IsComplete;

		public bool IsUnlocked;
	}

	public static System.Collections.Generic.Dictionary<int, GateInfo> Gates { get; } = new System.Collections.Generic.Dictionary<int, GateInfo>();

	public static System.Collections.Generic.Dictionary<string, int> GateNameToId { get; } = new System.Collections.Generic.Dictionary<string, int>();

	public static GateInfo? FindGateByName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (GateNameToId.TryGetValue(name, out var value) && Gates.TryGetValue(value, out var value2))
		{
			return value2;
		}
		return null;
	}

	public static void CaptureGateData(SoloGateUtil.GateManager gateManager, string source)
	{
		try
		{
			if (gateManager?.masterDataDic == null)
			{
				Log.Write("[SoloGate] " + source + ": masterDataDic is null");
				return;
			}
			int num = 0;
			foreach (Il2CppSystem.Collections.Generic.KeyValuePair<int, SoloGateUtil.Data> item in gateManager.masterDataDic)
			{
				SoloGateUtil.Data value = item.Value;
				string strName = value.StrName;
				if (!string.IsNullOrEmpty(strName))
				{
					GateInfo value2 = new GateInfo
					{
						GateID = value.gateID,
						Name = strName,
						Overview = value.strOverview,
						IsClear = value.IsClear,
						IsComplete = value.IsComplete,
						IsUnlocked = value.isUnlocked
					};
					Gates[value2.GateID] = value2;
					GateNameToId[strName] = value2.GateID;
					num++;
				}
			}
			Log.Write($"[SoloGate] {source}: captured {num} gate overviews");
		}
		catch (Exception ex)
		{
			Log.Write("[SoloGate] " + source + " error: " + ex.Message);
		}
	}
}
