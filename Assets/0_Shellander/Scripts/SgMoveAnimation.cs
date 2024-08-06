using System.Collections;
using UnityEngine;

public class SgMoveAnimation : MonoBehaviour
{
    public bool autoStart;
    public SgMoveAnimationStep[] steps;

	private void Start()
	{
		if(autoStart)
		{
			StartAnimation();
		}
	}

	private void StartAnimation()
	{
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
}

[System.Serializable]
public class SgMoveAnimationStep
{
	public float time;
    public Transform transform;
}

