using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SgUtil
{
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
}
