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

	private ISgScheduledEvent m_ScheduledEvent;

	protected override void Start()
	{
		bottle.gameObject.SetActive(false);
		Schedule(5, AnimateUp);
	}
	private void Schedule(float delay, Action action)
	{
		Scheduler.Cancel(m_ScheduledEvent);
		m_ScheduledEvent = Scheduler.Schedule(delay, action);
	}

	private void OnDestroy()
	{
		Scheduler.Cancel(m_ScheduledEvent);
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
