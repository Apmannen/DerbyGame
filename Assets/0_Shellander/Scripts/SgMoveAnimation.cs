using System.Collections;
using UnityEngine;

public class SgMoveAnimation : MonoBehaviour
{
	public bool autoStart;
	public SgMoveAnimationStep[] steps;
	private Vector3 m_DefaultPosition;

	private void Start()
	{
		m_DefaultPosition = this.transform.position;
		foreach(SgMoveAnimationStep step in steps)
		{
			step.transform.SetParent(this.transform.parent);
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
		foreach (SgMoveAnimationStep step in steps)
		{
			float timeCounter = 0;
			Vector3 startPos = this.transform.position;

			while (timeCounter < step.time)
			{
				timeCounter += Time.deltaTime;
				this.transform.position = Vector3.Lerp(startPos, step.transform.position, timeCounter / step.time);
				yield return null;
			}
		}
	}
}

[System.Serializable]
public class SgMoveAnimationStep
{
	public float time;
	public Transform transform;
}

