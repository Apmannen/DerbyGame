using System.Collections;
using UnityEngine;

public class SgMoveAnimation : MonoBehaviour
{
    public bool autoStart;
    public SgMoveAnimationStep[] steps;
	//private int m_CurrentStepIndex = -1;
	//private float m_TimeCounter = 0;
	//private Vector3 m_StartPosition;

	private void Start()
	{
		if(autoStart)
		{
			StartAnimation();
		}
	}

	private void StartAnimation()
	{
		//m_StartPosition = this.transform.position;
		//m_TimeCounter = 0;
		//m_CurrentStepIndex = 0;
		//Debug.Log("**** STARTANIM");
		StartCoroutine(AnimationRoutine());
	}

	public IEnumerator AnimationRoutine()
	{
		foreach(SgMoveAnimationStep step in steps)
		{
			float timeCounter = 0;
			Vector3 startPos = this.transform.position;

			while(timeCounter < step.time)
			{
				timeCounter += Time.deltaTime;
				this.transform.position = Vector3.Lerp(startPos, step.transform.position, timeCounter / step.time);
				yield return null;
			}
		}
	}

	//private void Update()
	//{
	//	if(m_CurrentStepIndex < 0 || m_CurrentStepIndex >= steps.Length)
	//	{
	//		return;
	//	}

	//	m_TimeCounter += Time.deltaTime;
	//	SgMoveAnimationStep step = steps[m_CurrentStepIndex];
	//	this.transform.position = Vector3.Lerp(m_StartPosition, step.transform.position, m_TimeCounter / step.time);
	//}
}

[System.Serializable]
public class SgMoveAnimationStep
{
	public float time;
    public Transform transform;
}

