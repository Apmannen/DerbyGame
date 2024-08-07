using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SgItembarItem : MonoBehaviour
{
	private Image m_Image;
	private Image Image => SgUtil.LazyParentComponent(this, ref m_Image);
	private bool m_IsHovered;
	public bool IsHovered => m_IsHovered && Definition != null;
	private SgItemDefinition m_Definition;
	public SgItemDefinition Definition => m_Definition;

	public void Set(SgItemDefinition itemDefinition)
	{
		m_Definition = itemDefinition;
		if (itemDefinition == null)
		{
			Image.sprite = null;
		}
		else
		{
			Image.sprite = itemDefinition.sprite;
		}
	}

	public void OnPointerEnter()
	{
		//Debug.Log("*** PENTER:"+this);
		m_IsHovered = true;
	}
	public void OnPointerExit()
	{
		m_IsHovered = false;
	}
	public void OnClick()
	{
		
	}
}
