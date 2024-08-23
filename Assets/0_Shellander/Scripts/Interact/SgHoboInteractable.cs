using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgHoboInteractable : SgInteractGroup
{
	//public SpriteRenderer spriteRenderer;
	public SgAnimation animationDown;
	public SgAnimation animationUp;

	private ISgScheduledEvent m_ScheduledEvent;

	protected override void Start()
	{
		Schedule(AnimateUp);
	}
	private void Schedule(Action action)
	{
		Scheduler.Cancel(m_ScheduledEvent);
		m_ScheduledEvent = Scheduler.Schedule(5, action);
	}

	private void OnDestroy()
	{
		Scheduler.Cancel(m_ScheduledEvent);
	}

	private void AnimateUp()
	{
		animationUp.Play();
		Schedule(AnimateDown);
	}
	private void AnimateDown()
	{
		animationDown.Play();
		Schedule(AnimateUp);
	}
}
