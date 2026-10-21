// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Runtime.InteropServices;
using MelonLoader;
using UnityEngine;

namespace MillenniumEye;

public static class ScreenReader
{
	private static bool _initialized;

	private static string _lastMessage = "";

	public static bool IsAvailable => _initialized;

	[DllImport("Tolk", CallingConvention = CallingConvention.Cdecl)]
	private static extern void Tolk_Load();

	[DllImport("Tolk", CallingConvention = CallingConvention.Cdecl)]
	private static extern void Tolk_Unload();

	[DllImport("Tolk", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool Tolk_IsLoaded();

	[DllImport("Tolk", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool Tolk_HasSpeech();

	[DllImport("Tolk", CallingConvention = CallingConvention.Cdecl)]
	private static extern void Tolk_TrySAPI(bool trySAPI);

	[DllImport("Tolk", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
	private static extern IntPtr Tolk_DetectScreenReader();

	[DllImport("Tolk", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
	private static extern bool Tolk_Output([MarshalAs(UnmanagedType.LPWStr)] string text, bool interrupt);

	[DllImport("Tolk", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool Tolk_Silence();

	public static bool Initialize()
	{
		if (_initialized)
		{
			return true;
		}
		try
		{
			Tolk_TrySAPI(trySAPI: true);
			Tolk_Load();
			_initialized = Tolk_IsLoaded() && Tolk_HasSpeech();
			if (_initialized)
			{
				IntPtr intPtr = Tolk_DetectScreenReader();
				string text = ((intPtr != IntPtr.Zero) ? Marshal.PtrToStringUni(intPtr) : "SAPI fallback");
				MelonLogger.Msg("Screen reader detected: " + text);
				Log.Write("[Init] Screen reader: " + text);
			}
			else
			{
				MelonLogger.Warning("No screen reader detected and SAPI unavailable");
				Log.Write("[Init] No screen reader available");
			}
			return _initialized;
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Failed to initialize Tolk: " + ex.Message);
			return false;
		}
	}

	public static void Shutdown()
	{
		if (_initialized)
		{
			try
			{
				Tolk_Unload();
			}
			catch (Exception ex)
			{
				Log.Write("[ScreenReader] Shutdown error: " + ex.Message);
			}
			_initialized = false;
		}
	}

	public static void Say(string text)
	{
		if (string.IsNullOrEmpty(text) || !_initialized)
		{
			return;
		}
		text = TextUtil.FixPlurals(text);
		_lastMessage = text;
		if (!Application.isFocused)
		{
			return;
		}
		try
		{
			Tolk_Output(text, interrupt: !NavigationState.IsInDuel || DuelEventDelivery.ShouldInterrupt(true));
		}
		catch (Exception ex)
		{
			Log.Write("[ScreenReader] Say error: " + ex.Message);
		}
	}

	public static void SayQueued(string text)
	{
		if (string.IsNullOrEmpty(text) || !_initialized)
		{
			return;
		}
		text = TextUtil.FixPlurals(text);
		_lastMessage = text;
		if (!Application.isFocused)
		{
			return;
		}
		try
		{
			Tolk_Output(text, interrupt: false);
		}
		catch (Exception ex)
		{
			Log.Write("[ScreenReader] SayQueued error: " + ex.Message);
		}
	}

	public static void SayDuelEvent(string text)
	{
		if (string.IsNullOrEmpty(text) || !_initialized) return;
		text = TextUtil.FixPlurals(text);
		_lastMessage = text;
		if (!Application.isFocused) return;
		try { Tolk_Output(text, DuelEventDelivery.BeginEvent(Time.frameCount)); }
		catch (Exception ex) { Log.Write("[ScreenReader] Event speech error: " + ex.Message); }
	}

	public static void Silence()
	{
		DuelEventDelivery.Reset();
		if (!_initialized)
		{
			return;
		}
		try
		{
			Tolk_Silence();
		}
		catch (Exception ex)
		{
			Log.Write("[ScreenReader] Silence error: " + ex.Message);
		}
	}

	public static void RepeatLast()
	{
		if (!string.IsNullOrEmpty(_lastMessage))
		{
			Say(_lastMessage);
		}
	}
}
