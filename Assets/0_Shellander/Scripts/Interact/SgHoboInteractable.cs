using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgHoboInteractable : SgInteractGroup
{
	//public SpriteRenderer spriteRenderer;
	public SgAnimation animationDown;
	public SgAnimation animationUp;
	public SgInteractGroup bottle;

	private Action m_ScheduledAction;
	private float m_ScheduledDelay;
	private float m_ScheduledTimeCounter;

	protected override void Start()
	{
		Schedule(5, AnimateUp);
	}

	private void Schedule(float delay, Action action)
	{
		m_ScheduledTimeCounter = 0;
		m_ScheduledDelay = delay;
		m_ScheduledAction = action;
	}
	private void Update()
	{
		SgPlayer player = SgPlayer.Get();
		if(m_ScheduledAction == null || player == null || !player.IsActionsAllowed())
		{
			return;
		}

		m_ScheduledTimeCounter += Time.deltaTime;
		if(m_ScheduledTimeCounter >= m_ScheduledDelay)
		{
			m_ScheduledAction();
		}
	}

	private void AnimateUp()
	{
		bottle.gameObject.SetActive(false);
		animationUp.Play();
		Schedule(3, AnimateDown);
	}
	private void AnimateDown()
	{
		StartCoroutine(AnimateDownRoutine());
	}
	private IEnumerator AnimateDownRoutine()
	{
		yield return animationDown.PlayRoutine();
		bottle.gameObject.SetActive(true);
		Schedule(5, AnimateUp);
	}
}

//private ISgScheduledEvent m_ScheduledEvent;
//private void Schedule(float delay, Action action)
//{
//	Scheduler.Cancel(m_ScheduledEvent);
//	m_ScheduledEvent = Scheduler.Schedule(delay, action);
//}

//private void OnDestroy()
//{
//	Scheduler.Cancel(m_ScheduledEvent);
//}
