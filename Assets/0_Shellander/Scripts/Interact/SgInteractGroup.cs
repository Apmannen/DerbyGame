using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SgInteractType { Illegal, Walk, Look, Use, Pickup, Talk, Wait }

public class SgInteractGroup : SgBehavior
{
	public string nameId;
	public int nameTranslationId = -1;
	public SgItemType itemType = SgItemType.Illegal;
	public SgInteractTranslation[] interactTranslations; //interact translation is a obsolete name, more of an interact config. Shouldn't be handled by translation manager anymore.
	public SpriteRenderer[] spriteRenderers;
	public int defaultRendererIndex = 0;
	public int collectedRendererIndex = 1;

	private int m_RenderIndex = 0;
	private SgInteractable[] m_Interactables;
	private SgInteractable[] Interactables => SgUtil.LazyChildComponents(this, ref m_Interactables);
	public bool IsConnectedToItem => itemType != SgItemType.Illegal;
	private SgItemDefinition m_ItemDefinition;
	public SgItemDefinition ItemDefinition
	{
		get
		{
			if (IsConnectedToItem && m_ItemDefinition == null)
			{
				m_ItemDefinition = ItemManager.Get(itemType);
			}
			return m_ItemDefinition;
		}
	}

	protected virtual void Start()
	{
		RefreshPickedUpVisibility();
	}

	public string TranslatedName
	{
		get
		{
			if (nameTranslationId < 0 && IsConnectedToItem)
			{
				return ItemDefinition.TranslatedName;
			}
			return TranslationManager.Get(nameTranslationId);
		}
	}

	protected void SetVisibleSprite(int renderIndex)
	{
		m_RenderIndex = renderIndex;
		for (int i = 0; i < spriteRenderers.Length; i++)
		{
			spriteRenderers[i].gameObject.SetActive(i == renderIndex);
		}
	}

	private void RefreshPickedUpVisibility()
	{
		SetVisibleSprite(IsConnectedToItem && ItemDefinition.IsColleted ? collectedRendererIndex : defaultRendererIndex);
		foreach (SgInteractable childInteractable in Interactables)
		{
			childInteractable.gameObject.SetActive(!IsConnectedToItem || !ItemDefinition.IsColleted);
		}
	}

	public SgInteractTranslation GetInteractConfig(SgInteractType interactType)
	{
		SgInteractTranslation translation = null;
		if (IsConnectedToItem)
		{
			translation = SgTranslationManager.GetInteractTranslation(ItemDefinition.interactTranslations, interactType);
		}
		if (translation == null)
		{
			translation = SgTranslationManager.GetInteractTranslation(interactTranslations, interactType);
		}
		if (translation == null)
		{
			translation = TranslationManager.GetDefaultTranslation(interactType);
		}
		return translation;
	}

	public int[] GetInteractTranslationIds(SgInteractType interactType)
	{
		return GetInteractConfig(interactType).translationIds;
	}

	//Should use more customizable predefined actions like toggle and pick up. In other words, pick up should be configurable in the same way as toggle. 
	public virtual void OnBeforeInteract(SgInteractType interactType)
	{
		SgInteractTranslation interactConfig = GetInteractConfig(interactType);
		if (IsConnectedToItem && interactType == SgInteractType.Pickup)
		{
			ItemDefinition.Collect();
			RefreshPickedUpVisibility();
		}
		else if(interactConfig != null && interactConfig.toggleSprite)
		{
			SetVisibleSprite(m_RenderIndex == 0 ? 1 : 0);
		}
	}
}