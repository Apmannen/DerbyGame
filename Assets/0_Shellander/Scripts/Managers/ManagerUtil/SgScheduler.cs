using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SgScheduler : MonoBehaviour
{
	public int eventCount;

	private readonly List<ISgScheduledEvent> m_Events = new List<ISgScheduledEvent>(200);
	private readonly List<ISgScheduledEvent> m_CancelList = new List<ISgScheduledEvent>();

	public void DoReset()
	{
		m_CancelList.Clear();
		m_Events.Clear();
		eventCount = 0;
	}

	public ISgScheduledEvent ScheduleFixed(float delay, System.Action action)
	{
		return Add(new SgScheduledEvent(Time.time, delay, true, action));
	}

	public ISgScheduledEvent ScheduleFixed<T>(float delay, Action<T> action, T param)
	{
		return Add(new SgScheduledEvent<T>(Time.time, delay, true, action, param));
	}
	public ISgScheduledEvent ScheduleFixed<T, U>(float delay, Action<T, U> action, T param, U param2)
	{
		return Add(new SgScheduledEvent<T, U>(Time.time, delay, true, action, param, param2));
	}

	public ISgScheduledEvent Schedule(float delay, System.Action action)
	{
		return Add(new SgScheduledEvent(Time.time, delay, false, action));
	}

	private ISgScheduledEvent Add(ISgScheduledEvent anEvent)
	{
		this.m_Events.Add(anEvent);
		return anEvent;
	}

	public void Cancel(ISgScheduledEvent anEvent)
	{
		if(anEvent == null)
		{
			return;
		}
		m_CancelList.Add(anEvent);
	}

	private void Update()
	{
		Check(false);
	}
	private void FixedUpdate()
	{
		Check(true);
	}

	private void Check(bool fixedLoop)
	{
		foreach (ISgScheduledEvent anEvent in m_CancelList)
		{
			m_Events.Remove(anEvent);
		}
		m_CancelList.Clear();

		float time = Time.time;
		eventCount = m_Events.Count;
		if (eventCount > 200)
		{
			Debug.LogError("Unreasonable number of scheduled events, c=" + eventCount);
		}
		for (int i = eventCount - 1; i >= 0; i--)
		{
			ISgScheduledEvent anEvent = m_Events[i];
			if (anEvent.IsInFixedLoop() == fixedLoop && time >= (anEvent.StartTime() + anEvent.Delay()))
			{
				anEvent.Execute();
				m_Events.RemoveAt(i);
			}
		}
	}
}

public interface ISgScheduledEvent
{
	public void Execute();
	public bool IsInFixedLoop();
	public float StartTime();
	public float Delay();
}


public struct SgScheduledEvent : ISgScheduledEvent
{
	public readonly float startTime;
	public readonly float delay;
	public readonly Action action;
	public readonly bool inFixedLoop;

	public SgScheduledEvent(float startTime, float delay, bool inFixedLoop, System.Action action)
	{
		this.startTime = startTime;
		this.delay = delay;
		this.inFixedLoop = inFixedLoop;
		this.action = action;
	}

	public float Delay()
	{
		return this.delay;
	}

	public void Execute()
	{
		action.Invoke();
	}
	public bool IsInFixedLoop()
	{
		return this.inFixedLoop;
	}

	public float StartTime()
	{
		return this.startTime;
	}
}

public struct SgScheduledEvent<T> : ISgScheduledEvent
{
	public readonly float startTime;
	public readonly float delay;
	public readonly Action<T> action;
	public readonly bool inFixedLoop;
	public readonly T param;

	public SgScheduledEvent(float startTime, float delay, bool inFixedLoop, System.Action<T> action, T param)
	{
		this.startTime = startTime;
		this.delay = delay;
		this.inFixedLoop = inFixedLoop;
		this.action = action;
		this.param = param;
	}

	public float Delay()
	{
		return this.delay;
	}

	public void Execute()
	{
		action.Invoke(param);
	}
	public bool IsInFixedLoop()
	{
		return this.inFixedLoop;
	}

	public float StartTime()
	{
		return this.startTime;
	}
}

public struct SgScheduledEvent<T, U> : ISgScheduledEvent
{
	public readonly float startTime;
	public readonly float delay;
	public readonly Action<T, U> action;
	public readonly bool inFixedLoop;
	public readonly T param;
	public readonly U param2;

	public SgScheduledEvent(float startTime, float delay, bool inFixedLoop, System.Action<T, U> action, T param, U param2)
	{
		this.startTime = startTime;
		this.delay = delay;
		this.inFixedLoop = inFixedLoop;
		this.action = action;
		this.param = param;
		this.param2 = param2;
	}

	public float Delay()
	{
		return this.delay;
	}

	public void Execute()
	{
		action.Invoke(param, param2);
	}
	public bool IsInFixedLoop()
	{
		return this.inFixedLoop;
	}

	public float StartTime()
	{
		return this.startTime;
	}
}

