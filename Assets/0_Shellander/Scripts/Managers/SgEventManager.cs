using System;
using System.Collections.Generic;
using UnityEngine;

public enum SgEventName { NamedSaveBoolUpdated }

public class SgEventManager : MonoBehaviour
{
	private Dictionary<SgEventName, List<Action>> m_Listener = new();

	private void Awake()
	{
		var eventNames = Enum.GetValues(typeof(SgEventName));
		foreach (SgEventName eventName in eventNames)
		{
			m_Listener[eventName] = new List<Action>();
		}
	}

	public void Register(SgEventName eventName, Action action)
	{
		m_Listener[eventName].Add(action);
	}
	public void Unregister(SgEventName eventName, Action action)
	{
		m_Listener[eventName].Remove(action);
	}

	public void Execute(SgEventName eventName)
	{
		foreach(Action action in m_Listener[eventName])
		{
			action.Invoke();
		}
	}
}
