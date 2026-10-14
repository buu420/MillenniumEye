// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader;

namespace MillenniumEye;

public static class HandlerRegistry
{
	private static readonly List<IMenuHandler> _handlers = new List<IMenuHandler>();

	private static IMenuHandler _current;

	public static IMenuHandler Current => _current;

	public static void Init()
	{
		_handlers.Clear();
		IEnumerable<Type> enumerable = from t in Assembly.GetExecutingAssembly().GetTypes()
			where !t.IsInterface && !t.IsAbstract && typeof(IMenuHandler).IsAssignableFrom(t)
			select t;
		foreach (Type item2 in enumerable)
		{
			try
			{
				IMenuHandler item = (IMenuHandler)Activator.CreateInstance(item2);
				_handlers.Add(item);
				Log.Write("[Registry] Registered handler: " + item2.Name);
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[Registry] Failed to create " + item2.Name + ": " + ex.Message);
			}
		}
		MelonLogger.Msg($"[Registry] {_handlers.Count} menu handlers registered");
	}

	public static IMenuHandler SetCurrentFromVC(string viewControllerName)
	{
		_current = _handlers.FirstOrDefault((IMenuHandler h) => h.CanHandle(viewControllerName));
		return _current;
	}

	public static IMenuHandler GetHandler(string viewControllerName)
	{
		return _handlers.FirstOrDefault((IMenuHandler h) => h.CanHandle(viewControllerName));
	}
}
