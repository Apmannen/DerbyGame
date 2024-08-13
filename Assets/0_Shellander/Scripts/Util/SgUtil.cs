using System;
using System.Collections.Generic;
using UnityEngine;

public static class SgUtil
{
	private static readonly System.Random s_Random = new System.Random();

	public static T LazyParentComponent<T>(Component component, ref T localReference) where T : Component
	{
		if (localReference == null)
		{
			localReference = component.GetComponentInParent<T>();
		}
		return localReference;
	}
	public static T LazyComponent<T>(Component component, ref T localReference) where T : Component
	{
		if (localReference == null)
		{
			localReference = component.GetComponent<T>();
		}
		return localReference;
	}
	public static T[] LazyChildComponents<T>(Component component, ref T[] localReference) where T : Component
	{
		if (localReference == null)
		{
			localReference = component.GetComponentsInChildren<T>();
		}
		return localReference;
	}

	public static long CurrentTimeMs()
	{
		long value = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		return value;
	}

	public static T[] EnumValues<T>()
	{
		return (T[])Enum.GetValues(typeof(T));
	}

	public static IList<T> Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = s_Random.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
		return list;
	}
	public static int RandomInt(int min, int max)
	{
		return s_Random.Next(min, max + 1);
	}
	public static E RandomElement<E>(List<E> list)
	{
		return list[RandomInt(0, list.Count - 1)];
	}
}
