using System.Collections;
using UnityEngine;

public class SgMoveAnimation : SgBehavior
{
	public bool autoStart;
	public SgMoveAnimationStep[] steps;
	
	private Vector3 m_DefaultPosition;
	private bool m_IsStepAborted;


	private void Awake()
	{
		m_DefaultPosition = this.transform.position;
		foreach(SgMoveAnimationStep step in steps)
		{
			step.transform.SetParent(this.transform.parent, true);
		}
		if (autoStart)
		{
			StartAnimation();
		}
	}

	public Coroutine StartAnimation()
	{
		return StartCoroutine(AnimationRoutine());
	}

	public IEnumerator AnimationRoutine()
	{
		this.transform.position = m_DefaultPosition;
		for(int i = 0; i < steps.Length; i++)
		{
			yield return AnimateStep(i);
		}
	}

	public IEnumerator AnimateStep(int stepIndex)
	{
		SgMoveAnimationStep step = steps[stepIndex];
		m_IsStepAborted = false;

		if (stepIndex == 0)
		{
			this.transform.position = m_DefaultPosition;
		}
		else
		{
			this.transform.position = steps[stepIndex - 1].transform.position;
		}		

		float timeCounter = 0;
		Vector3 startPos = this.transform.position;

		while (timeCounter < step.time)
		{
			timeCounter += Time.deltaTime;
			this.transform.position = Vector3.Lerp(startPos, step.transform.position, timeCounter / step.time);
			if(m_IsStepAborted)
			{
				break;
			}
			yield return null;
		}

		this.transform.position = step.transform.position;
	}

	private void AbortStep()
	{
		m_IsStepAborted = true;
	}

	private void Update()
	{
		if (InputManager.ClickAction.WasPressedThisFrame())
		{
			AbortStep();
		}
	}
}

[System.Serializable]
public class SgMoveAnimationStep
{
	public float time;
	public Transform transform;
}

