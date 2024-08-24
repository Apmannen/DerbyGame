using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgHoboInteractable : SgInteractGroup
{
	public SgAnimation animationDown;
	public SgAnimation animationUp;
	public SgInteractGroup bottle;
	public Sprite dropSprite;
	public Sprite sleepingSprite;

	private Action m_ScheduledAction;
	private float m_ScheduledDelay;
	private float m_ScheduledTimeCounter;
	private SpriteRenderer SpriteRenderer => spriteRenderers[0];

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
	private void ClearSchedule()
	{
		m_ScheduledTimeCounter = 0;
		m_ScheduledDelay = float.MaxValue;
		m_ScheduledAction = null;
	}
	private const string m_BottleSmashedString = "BottleSmashed";
	private bool IsSleeping => SaveDataManager.CurrentSaveFile.GetNamedBoolValue(m_BottleSmashedString);

	private void Update()
	{
		if(IsSleeping)
		{
			SpriteRenderer.sprite = sleepingSprite;
			return;
		}

		//Could actually do this in SgScheduler too, in one way or another (there are ways around coupling).
		SgPlayer player = SgPlayer.Get();
		Action scheduledAction = m_ScheduledAction;
		if(m_IsBlocked || scheduledAction == null || player == null || !player.IsActionsAllowed())
		{
			return;
		}

		m_ScheduledTimeCounter += Time.deltaTime;
		if(m_ScheduledTimeCounter >= m_ScheduledDelay)
		{			
			ClearSchedule();
			scheduledAction();
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

	public void TriggerSleepingPillDrop()
	{
		m_IsBlocked = true;
		StartCoroutine(TriggerSleepingPillDropEnumerator());
	}
	private IEnumerator TriggerSleepingPillDropEnumerator()
	{
		bottle.gameObject.SetActive(false);
		yield return animationUp.PlayRoutine();
		yield return new WaitForSeconds(1);
		yield return animationDown.PlayRoutine();
		SpriteRenderer.sprite = dropSprite;

		SaveDataManager.CurrentSaveFile.SetNamedBoolValue(m_BottleSmashedString, true);

		m_IsBlocked = false;
	}
}
