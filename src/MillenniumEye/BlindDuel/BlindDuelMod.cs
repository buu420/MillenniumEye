// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using UnityEngine;

namespace MillenniumEye;

public class MillenniumEyeMod : MelonMod
{
	public override void OnInitializeMelon()
	{
		try
		{
			ClassInjector.RegisterTypeInIl2Cpp<MillenniumEyeCore>();
			GameObject gameObject = new GameObject("MillenniumEyeCore");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<MillenniumEyeCore>();
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			MelonLogger.Msg("MillenniumEye loaded!");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error loading MillenniumEye!");
			MelonLogger.Error(ex.ToString());
		}
	}
}
