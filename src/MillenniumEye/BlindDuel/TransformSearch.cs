// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using Il2CppYgomSystem.UI;
using UnityEngine;

namespace MillenniumEye;

public static class TransformSearch
{
	public static Transform GetChild(Transform parent, int index, string context = null)
	{
		if (parent == null || index < 0 || index >= parent.childCount)
		{
			Log.Write($"[GetChild] {context ?? "unknown"}: index {index}, parent {parent?.name ?? "null"} has {parent?.childCount ?? 0} children");
			return null;
		}
		return parent.GetChild(index);
	}

	public static Transform FindByName(Transform root, string name)
	{
		if (root == null || string.IsNullOrEmpty(name))
		{
			return null;
		}
		Transform transform = root.Find(name);
		if (transform != null)
		{
			return transform;
		}
		for (int i = 0; i < root.childCount; i++)
		{
			Transform child = root.GetChild(i);
			if (child.name == name)
			{
				return child;
			}
			Transform transform2 = FindByName(child, name);
			if (transform2 != null)
			{
				return transform2;
			}
		}
		return null;
	}

	public static T FindComponent<T>(Transform root) where T : Component
	{
		if (root == null)
		{
			return null;
		}
		return root.GetComponentInChildren<T>();
	}

	public static (int index, int total) GetButtonIndex(SelectionButton button)
	{
		try
		{
			Transform parent = button.transform.parent;
			if (parent == null)
			{
				return (index: 0, total: 0);
			}
			int item = 0;
			int num = 0;
			bool flag = false;
			for (int i = 0; i < parent.childCount; i++)
			{
				Transform child = parent.GetChild(i);
				if (child.gameObject.activeInHierarchy && !(child.GetComponent<SelectionButton>() == null))
				{
					num++;
					if (child.gameObject == button.gameObject)
					{
						item = num;
						flag = true;
					}
				}
			}
			if (flag)
			{
				return (index: item, total: num);
			}
		}
		catch (Exception ex)
		{
			Log.Write("[GetButtonIndex] " + ex.Message);
		}
		return (index: 0, total: 0);
	}
}
