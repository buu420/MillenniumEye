// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Il2CppYgomGame.Menu;
using Il2CppYgomSystem.UI.InfinityScroll;
using Il2CppYgomSystem.Utility;
using UnityEngine;

namespace MillenniumEye;

[HarmonyPatch(typeof(ProfileDataViewController), "UpdateProfileData")]
internal class PatchProfileDataReady
{
	[HarmonyPostfix]
	private static void Postfix(ProfileDataViewController __instance)
	{
		try
		{
			InfinityScrollView isvData = __instance.isvData;
			if (isvData == null)
			{
				return;
			}
			int dataCount = isvData.dataCount;
			if (dataCount == 0)
			{
				return;
			}
			int dataInfoHeadIndex = __instance.dataInfoHeadIndex;
			Il2CppSystem.Collections.Generic.List<ProfileDataViewController.ProfileDataInfo> dataInfos = __instance.dataInfos;
			System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i < dataCount; i++)
			{
				GameObject entityByDataIndex = isvData.GetEntityByDataIndex(i);
				if (!(entityByDataIndex == null))
				{
					System.Collections.Generic.List<TextResult> list2 = TextExtractor.ExtractAll(entityByDataIndex, new TextSearchOptions
					{
						ActiveOnly = true,
						FilterBanned = false
					});
					if (list2.Count >= 2)
					{
						list.Add(list2[0].Text + ": " + list2[1].Text);
					}
					else if (list2.Count == 1)
					{
						list.Add(list2[0].Text);
					}
					if (i >= dataInfoHeadIndex)
					{
						hashSet.Add(i - dataInfoHeadIndex);
					}
				}
			}
			if (dataInfos != null)
			{
				for (int j = 0; j < dataInfos.Count; j++)
				{
					if (!hashSet.Contains(j))
					{
						try
						{
							ProfileDataViewController.ProfileDataInfo profileDataInfo = dataInfos[j];
							string text = TextData.GetText(profileDataInfo.record);
							list.Add($"{text}: {profileDataInfo.value}");
						}
						catch (Exception ex)
						{
							Log.Write($"[ProfileData] TextData error at {j}: {ex.Message}");
							break;
						}
					}
				}
			}
			if (list.Count > 0)
			{
				Speech.SayQueued(string.Join(". ", list));
			}
			Log.Write($"[ProfileData] Read {list.Count} items (dataInfos={dataInfos?.Count ?? 0})");
		}
		catch (Exception ex2)
		{
			Log.Write("[ProfileData] error: " + ex2.Message);
		}
	}
}
