using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgPlayerMirror : MonoBehaviour
{
	public SgPlayer player;
	public float yAdd;
	public SpriteRenderer spriteRenderer;

	private void Update()
	{
		Vector3 pos = this.transform.position;
		pos.x = player.transform.position.x;
		pos.y = -(player.transform.position.y+yAdd);
		this.transform.position = pos;

		spriteRenderer.sprite = player.mainRenderer.sprite;
		spriteRenderer.flipX = player.mainRenderer.flipX;

		Debug.Log("*** Y:"+player.transform.position.y);
	}
}
