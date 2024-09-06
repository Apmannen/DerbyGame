using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgCharacter : SgBehavior
{
	public TMPro.TextMeshPro speechText;
	public SgAnimation speechAnimation;
	public Transform speechAnimationPosWhenFlipped;
	public SpriteRenderer mainRenderer;
	public Color vertexColor = Color.white;

	private Vector3 m_DefaultSpeechAnimationPos;
	private Vector3 m_FlippedSpeechAnimationPos;
	private bool m_SpeechAborted = false;
	private TMPro.TextMeshProUGUI m_OverlayText;
	private float m_XOffset;

	private void Start()
	{
		//speechText.gameObject.SetActive(false);
		m_OverlayText = GameObject.Instantiate(HudManager.speechTextOverlayTemplate, HudManager.speechTextOverlayTemplate.transform.parent);
		m_OverlayText.gameObject.SetActive(true);		
		m_OverlayText.color = vertexColor;

		if (speechAnimation != null)
		{
			m_DefaultSpeechAnimationPos = speechAnimation.transform.localPosition;
			m_FlippedSpeechAnimationPos = speechAnimationPosWhenFlipped != null ? speechAnimationPosWhenFlipped.localPosition : m_DefaultSpeechAnimationPos;
			speechAnimation.gameObject.SetActive(true);
			if (speechAnimationPosWhenFlipped != null)
			{
				speechAnimationPosWhenFlipped.gameObject.SetActive(false);
			}
		}
		ClearSpeech();
	}

	public void SetXOffset(float xOffset)
    {
		m_XOffset = xOffset;
	}

	private void Update()
	{
		if(speechAnimation != null)
		{
			Vector3 speechAnimationPos = mainRenderer != null && mainRenderer.flipX ? m_FlippedSpeechAnimationPos : m_DefaultSpeechAnimationPos;
			speechAnimation.transform.localPosition = speechAnimationPos;
		}

		//TODO: keep in frame and then generalize this so that it can be used by cursor text as well
		//Don't wanna get component each frame
		Vector3 worldPos = speechText.transform.position;
		worldPos.x += m_XOffset;
		m_OverlayText.transform.position = Camera.main.WorldToScreenPoint(worldPos);

		RectTransform rectTransform = m_OverlayText.GetComponent<RectTransform>();
		//rectTransform.anchoredPosition
		//TestTopLeft(rectTransform);
	}

	private void TestTopLeft(RectTransform rectTransform)
    {
		TestPreset(rectTransform, new Vector2(0,1), new Vector2(0, 1), new Vector2(0, 1));
    }

	private static void TestPreset(RectTransform rectTransform, Vector2 min, Vector2 max, Vector2 pivot)
    {
		rectTransform.anchorMin = min;
		rectTransform.anchorMax = max;
		rectTransform.pivot = pivot;
	}

	/*
	Pivot presets

	min x y
	max x y
	pivot x y

	top left
	0 1
	0 1
	0 1

	top right
	1 1
	1 1
	1 1

	bottom left
	0 0
	0 0
	0 y

	bottom right
	1 0
	1 0
	1 y
	*/

	public void ClearSpeech()
	{
		if(speechAnimation != null)
		{
			speechAnimation.Stop();
			speechAnimation.spriteRenderer.sprite = null;
		}
		m_OverlayText.text = "";
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
		if (speechAnimation != null)
		{
			speechAnimation.Play();
		}
		foreach (int id in translationIds)
		{
			string translation = InteractManager.Get(id);
			m_OverlayText.text = translation;
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
