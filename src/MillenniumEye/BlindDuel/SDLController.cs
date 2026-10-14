// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Runtime.InteropServices;

namespace MillenniumEye;

public static class SDLController
{
	public enum Button
	{
		South,
		East,
		West,
		North,
		Back,
		Guide,
		Start,
		LeftStick,
		RightStick,
		LeftShoulder,
		RightShoulder,
		DPadUp,
		DPadDown,
		DPadLeft,
		DPadRight
	}

	public enum Axis
	{
		LeftX,
		LeftY,
		RightX,
		RightY,
		LeftTrigger,
		RightTrigger
	}

	private const uint SDL_INIT_GAMEPAD = 8192u;

	private const uint SDL_INIT_JOYSTICK = 512u;

	private const string DLL = "SDL3.dll";

	private static bool _initialized;

	private static bool _available;

	private static IntPtr _gamepad = IntPtr.Zero;

	private static string _controllerName = "";

	private static short _lastRightY;

	private static short _lastRightX;

	private static short _rightY;

	private static short _rightX;

	private const short StickThreshold = 16000;

	public static bool IsAvailable => _available && _gamepad != IntPtr.Zero;

	public static string ControllerName => _controllerName;

	public static bool RightStickUpTriggered => _available && _gamepad != IntPtr.Zero && _rightY < -16000 && _lastRightY >= -16000;

	public static bool RightStickDownTriggered => _available && _gamepad != IntPtr.Zero && _rightY > 16000 && _lastRightY <= 16000;

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool SDL_Init(uint flags);

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern void SDL_Quit();

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern void SDL_PumpEvents();

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr SDL_GetGamepads(out int count);

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr SDL_OpenGamepad(uint instance_id);

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern void SDL_CloseGamepad(IntPtr gamepad);

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool SDL_GamepadConnected(IntPtr gamepad);

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool SDL_GetGamepadButton(IntPtr gamepad, Button button);

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern short SDL_GetGamepadAxis(IntPtr gamepad, Axis axis);

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr SDL_GetGamepadName(IntPtr gamepad);

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr SDL_GetError();

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool SDL_SetHint([MarshalAs(UnmanagedType.LPUTF8Str)] string name, [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

	[DllImport("SDL3.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern void SDL_free(IntPtr mem);

	public static bool Initialize()
	{
		if (_initialized)
		{
			return _available;
		}
		_initialized = true;
		try
		{
			SDL_SetHint("SDL_WINDOWS_DISABLE_THREAD_NAMING", "1");
			if (!SDL_Init(8704u))
			{
				IntPtr intPtr = SDL_GetError();
				string text = ((intPtr != IntPtr.Zero) ? Marshal.PtrToStringUTF8(intPtr) : "Unknown");
				Log.Write("[SDL3] Init failed: " + text);
				return false;
			}
			_available = true;
			Log.Write("[SDL3] Initialized");
			OpenFirstGamepad();
			return true;
		}
		catch (DllNotFoundException)
		{
			Log.Write("[SDL3] SDL3.dll not found — controller detail reading unavailable");
			return false;
		}
		catch (Exception ex2)
		{
			Log.Write("[SDL3] Init error: " + ex2.Message);
			return false;
		}
	}

	public static void Update()
	{
		if (!_available)
		{
			return;
		}
		try
		{
			SDL_PumpEvents();
			if (_gamepad != IntPtr.Zero && !SDL_GamepadConnected(_gamepad))
			{
				Log.Write("[SDL3] Disconnected: " + _controllerName);
				SDL_CloseGamepad(_gamepad);
				_gamepad = IntPtr.Zero;
				_controllerName = "";
			}
			if (_gamepad == IntPtr.Zero)
			{
				OpenFirstGamepad();
			}
			if (!(_gamepad == IntPtr.Zero))
			{
				_lastRightY = _rightY;
				_lastRightX = _rightX;
				_rightY = SDL_GetGamepadAxis(_gamepad, Axis.RightY);
				_rightX = SDL_GetGamepadAxis(_gamepad, Axis.RightX);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[SDL3] Update error: " + ex.Message);
		}
	}

	public static void Shutdown()
	{
		if (!_available)
		{
			return;
		}
		try
		{
			if (_gamepad != IntPtr.Zero)
			{
				SDL_CloseGamepad(_gamepad);
				_gamepad = IntPtr.Zero;
			}
			SDL_Quit();
			_available = false;
			Log.Write("[SDL3] Shutdown");
		}
		catch (Exception ex)
		{
			Log.Write("[SDL3] Shutdown error: " + ex.Message);
		}
	}

	private static void OpenFirstGamepad()
	{
		try
		{
			IntPtr intPtr = SDL_GetGamepads(out var count);
			if (!(intPtr == IntPtr.Zero) && count != 0)
			{
				uint instance_id = (uint)Marshal.ReadInt32(intPtr);
				SDL_free(intPtr);
				_gamepad = SDL_OpenGamepad(instance_id);
				if (_gamepad != IntPtr.Zero)
				{
					IntPtr intPtr2 = SDL_GetGamepadName(_gamepad);
					_controllerName = ((intPtr2 != IntPtr.Zero) ? Marshal.PtrToStringUTF8(intPtr2) : "Unknown");
					Log.Write("[SDL3] Connected: " + _controllerName);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[SDL3] OpenGamepad error: " + ex.Message);
		}
	}
}
