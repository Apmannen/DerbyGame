using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgAnimation : SgBehavior
{
	public SpriteRenderer spriteRenderer;
	public Sprite[] sprites;
	public float changeInterval = 0.4f;

	private int m_CurrentIndex = 0;
	private bool m_IsPlaying = false;
	private float m_TimeCounter;

	public void Play()
	{
		m_IsPlaying = true;
		m_CurrentIndex = 0;
		spriteRenderer.sprite = sprites[m_CurrentIndex];
	}
	public void Stop()
	{
		m_IsPlaying = false;
		m_CurrentIndex = 0;
		spriteRenderer.sprite = sprites[m_CurrentIndex];
	}

	private void FixedUpdate()
	{
		if(!m_IsPlaying)
		{
			return;
		}

		m_TimeCounter += Time.deltaTime;
		if(m_TimeCounter >= changeInterval)
		{
			m_TimeCounter -= changeInterval;
			m_CurrentIndex++;
			if(m_CurrentIndex >= sprites.Length)
			{
				m_CurrentIndex = 0;
			}
			spriteRenderer.sprite = sprites[m_CurrentIndex];
		}
	}
}
