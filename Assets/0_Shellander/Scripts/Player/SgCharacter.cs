using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgCharacter : SgBehavior
{
	public TMPro.TextMeshPro speechText;

	private bool m_SpeechAborted = false;

	private void Start()
	{
		speechText.text = "";
	}

	public void ClearSpeech()
	{
		speechText.text = "";
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
		foreach (int id in translationIds)
		{
			string translation = TranslationManager.Get(id);
			speechText.text = translation;
			m_SpeechAborted = false;
			yield return Wait(3f);
		}
		speechText.text = "";
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
