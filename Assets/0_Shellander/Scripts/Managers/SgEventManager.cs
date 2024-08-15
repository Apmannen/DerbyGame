using System;
using System.Collections.Generic;
using UnityEngine;

public enum SgEventName { NamedSaveBoolUpdated, NavMeshRebuild, RoomChanged }

public class SgEventManager : MonoBehaviour
{
	private readonly Dictionary<SgEventName, List<SgEvent>> m_Listeners = new();

	//private void Awake()
	//{
	//	var eventNames = Enum.GetValues(typeof(SgEventName));
	//	foreach (SgEventName eventName in eventNames)
	//	{
	//		m_Listeners[eventName] = new List<SgEvent>();
	//	}
	//}

	private void AddIfMissing(SgEventName eventName)
	{
		if(!m_Listeners.ContainsKey(eventName))
		{
			m_Listeners[eventName] = new();
		}
	}

	public void Register(SgEventName eventName, Action action)
	{
		AddIfMissing(eventName);
		m_Listeners[eventName].Add(new SgEvent { action0 = action });
	}
	public void Register<T>(SgEventName eventName, Action<T> action)
	{
		AddIfMissing(eventName);
		m_Listeners[eventName].Add(new SgEvent { action1 = action });
	}
	public void Unregister(SgEventName eventName, Action action)
	{
		AddIfMissing(eventName);
		m_Listeners[eventName].RemoveAll(e => action.Equals(e.action0));
	}
	public void Unregister<T>(SgEventName eventName, Action<T> action)
	{
		AddIfMissing(eventName);
		m_Listeners[eventName].RemoveAll(e => action.Equals(e.action1));
	}

	public void Execute(SgEventName eventName)
	{
		AddIfMissing(eventName);
		foreach (SgEvent anEvent in m_Listeners[eventName])
		{
			anEvent.action0?.Invoke();
		}
	}
	public void Execute<T>(SgEventName eventName, T param)
	{
		AddIfMissing(eventName);
		foreach (SgEvent anEvent in m_Listeners[eventName])
		{
			if(anEvent.action1 != null)
			{
				((Action<T>)anEvent.action1).Invoke(param);
			}
		}
	}

	private class SgEvent
	{
		public Action action0;
		public object action1;
	}
}


