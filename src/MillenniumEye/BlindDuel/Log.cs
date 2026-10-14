// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.IO;
using System.Reflection;
using MelonLoader;

namespace MillenniumEye;

public static class Log
{
	private static string _logPath;

	private static readonly object _lock = new object();

	public static void Init()
	{
		try
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			_logPath = Path.Combine(directoryName, "MillenniumEye_debug.log");
			lock (_lock)
			{
				File.WriteAllText(_logPath, $"[{DateTime.Now:HH:mm:ss.fff}] === MillenniumEye Debug Log Started ===\n");
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Log.Init] " + ex.Message);
		}
	}

	public static void Write(string message)
	{
		if (string.IsNullOrEmpty(_logPath))
		{
			return;
		}
		try
		{
			lock (_lock)
			{
				File.AppendAllText(_logPath, $"[{DateTime.Now:HH:mm:ss.fff}] {message}\n");
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Warning("[Log.Write] " + ex.Message);
		}
	}
}
