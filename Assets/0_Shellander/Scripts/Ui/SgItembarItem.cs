using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SgItembarItem : MonoBehaviour
{
	public Image image;	

	private SgItemDefinition m_Definition;
	public SgItemDefinition Definition => m_Definition;
	private bool m_IsHovered;
	public bool IsHovered => m_IsHovered && Definition != null;

	public void Set(SgItemDefinition itemDefinition)
	{
		m_Definition = itemDefinition;
		if (itemDefinition == null)
		{
			image.sprite = null;
		}
		else
		{
			image.sprite = itemDefinition.sprite;
		}
	}

	public void OnPointerEnter()
	{
		m_IsHovered = true;
	}
	public void OnPointerExit()
	{
		m_IsHovered = false;
	}
}
