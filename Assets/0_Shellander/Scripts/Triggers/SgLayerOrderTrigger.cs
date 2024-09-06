using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgLayerOrderTrigger : MonoBehaviour
{
	public SpriteRenderer[] affectedSpriteRenderers;
	public int newOrder;
	public bool resetOrder;

	private int[] m_DefaultOrders;

	private void Start()
	{
		GetComponent<SpriteRenderer>().enabled = false;

		m_DefaultOrders = new int[affectedSpriteRenderers.Length];
		for (int i = 0; i < affectedSpriteRenderers.Length; i++)
		{
			m_DefaultOrders[i] = affectedSpriteRenderers[i].sortingOrder;
		}
	}

	private int GetSpriteIndex(SpriteRenderer sprite)
    {
		for(int i = 0; i < affectedSpriteRenderers.Length; i++)
        {
			//Debug.Log("**** i="+i+", iid="+ affectedSpriteRenderers[i].gameObject.GetInstanceID()+", iid="+ sprite.GetInstanceID());
			if(affectedSpriteRenderers[i].GetInstanceID() == sprite.GetInstanceID())
            {
				return i;
            }
        }
		throw new KeyNotFoundException("Couldn't find index for: "+ sprite.gameObject);
    }

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (SgPlayer.GetFromCollider(other) == null)
		{
			return;
		}

		int changeOrderTo = newOrder;		
		foreach(SpriteRenderer sprite in affectedSpriteRenderers)
		{
			if (resetOrder)
			{
				changeOrderTo = m_DefaultOrders[GetSpriteIndex(sprite)];
			}
			sprite.sortingOrder = changeOrderTo;
		}
	}
}
