using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgPlayerMirror : MonoBehaviour
{
	public SgPlayer player;
	public float yAdd;
	public SpriteRenderer spriteRenderer;
	public SpriteRenderer fadeOverMirrorSprite;
	public Transform fullyVisiblePos;
	public Transform fullyInvisiblePos;

	private float FullyVisiblePos => fullyVisiblePos.position.y;
	private float FullyInvisiblePos => fullyInvisiblePos.position.y;

	private void Update()
	{
		float playerX = player.transform.position.x;
		float playerY = player.transform.position.y;

		Vector3 pos = this.transform.position;
		pos.x = playerX;
		pos.y = -(playerY + yAdd);
		this.transform.position = pos;

		spriteRenderer.sprite = player.mainRenderer.sprite;
		spriteRenderer.flipX = player.mainRenderer.flipX;


		float alpha;
		if (playerY < FullyInvisiblePos)
		{
			alpha = 1;
		}
		else if (playerY > FullyVisiblePos)
		{
			alpha = 0;
		}
		else
		{
			float span = FullyVisiblePos - FullyInvisiblePos;
			float offset = Mathf.Abs(FullyInvisiblePos - playerY);
			alpha = ((float)offset) / ((float)span);
			alpha = 1 - alpha;
			//Debug.Log("*** a="+alpha+", span="+span+", y="+playerY+", offset="+offset);
			alpha = Mathf.Clamp01(alpha);
		}

		Color color = fadeOverMirrorSprite.color;
		color.a = alpha;
		fadeOverMirrorSprite.color = color;
	}
}
