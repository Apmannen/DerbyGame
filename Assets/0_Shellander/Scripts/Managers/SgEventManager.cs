using System;
using System.Collections.Generic;
using UnityEngine;

public enum SgEventName { NamedSaveBoolUpdated, NavMeshRebuild, RoomChanged, ItemCollected }

public class SgEventManager : MonoBehaviour
{
	private readonly Dictionary<SgEventName, List<SgEvent>> m_Listeners = new();

	private List<SgEvent> GetEventList(SgEventName eventName)
	{
		if (!m_Listeners.ContainsKey(eventName))
		{
			m_Listeners[eventName] = new();
		}
		return m_Listeners[eventName];
	}

	public void Register(SgEventName eventName, Action action)
	{
		GetEventList(eventName).Add(new SgEvent { action0 = action });
	}
	public void Register<T>(SgEventName eventName, Action<T> action)
	{
		GetEventList(eventName).Add(new SgEvent { action1 = action });
	}
	public void Unregister(SgEventName eventName, Action action)
	{
		GetEventList(eventName).RemoveAll(e => action.Equals(e.action0));
	}
	public void Unregister<T>(SgEventName eventName, Action<T> action)
	{
		GetEventList(eventName).RemoveAll(e => action.Equals(e.action1));
	}

	public void Execute(SgEventName eventName)
	{
		foreach (SgEvent anEvent in GetEventList(eventName))
		{
			anEvent.action0?.Invoke();
		}
	}
	public void Execute<T>(SgEventName eventName, T param)
	{
		foreach (SgEvent anEvent in GetEventList(eventName))
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


