using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgCharacter : SgBehavior
{
	public TMPro.TextMeshPro speechText;
	public SgAnimation speechAnimation;
	public Transform speechAnimationPosWhenFlipped;
	public SpriteRenderer mainRenderer;

	private Vector3 m_DefaultSpeechAnimationPos;
	private Vector3 m_FlippedSpeechAnimationPos;
	private bool m_SpeechAborted = false;

	private void Start()
	{
		m_DefaultSpeechAnimationPos = speechAnimation.transform.localPosition;
		m_FlippedSpeechAnimationPos = speechAnimationPosWhenFlipped != null ? speechAnimationPosWhenFlipped.localPosition : m_DefaultSpeechAnimationPos;
		speechAnimation.gameObject.SetActive(true);
		if(speechAnimationPosWhenFlipped != null)
		{
			speechAnimationPosWhenFlipped.gameObject.SetActive(false);
		}		
		ClearSpeech();
	}

	private void Update()
	{
		Vector3 speechAnimationPos = mainRenderer != null && mainRenderer.flipX ? m_FlippedSpeechAnimationPos : m_DefaultSpeechAnimationPos;
		speechAnimation.transform.localPosition = speechAnimationPos;
	}

	public void ClearSpeech()
	{
		speechAnimation.Stop();
		speechAnimation.spriteRenderer.sprite = null;
		speechText.text = "";
		StopAllCoroutines();
	}
	public void SkipSpeech()
	{
		m_SpeechAborted = true;
	}

	public IEnumerator Talk(int translationId)
	{
		return Talk(new int[] { translationId });
	}
	public IEnumerator Talk(IList<int> translationIds)
	{
		speechAnimation.Play();
		foreach (int id in translationIds)
		{
			string translation = InteractManager.Get(id);
			speechText.text = translation;
			m_SpeechAborted = false;
			yield return Wait(3f);
		}
		ClearSpeech();
	}

	private IEnumerator Wait(float maxDuration)
	{
		float time = 0;
		while (time < maxDuration && !m_SpeechAborted)
		{
			time += Time.deltaTime;
			yield return null;
		}
	}
}
