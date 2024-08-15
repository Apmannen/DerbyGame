using System.Collections;
using UnityEngine;

public enum SgInteractType { Illegal, Walk, Look, Use, Pickup, Talk, Wait, Collision, Item, Generic }

public class SgInteractGroup : SgBehavior
{
	public string nameId;
	public int nameTranslationId = -1;
	public SgItemType itemType = SgItemType.Illegal;
	public SgInteractTranslation[] interactTranslations; //interact translation is a obsolete name, more of an interact config. Shouldn't be handled by translation manager anymore.
	public SpriteRenderer[] spriteRenderers;
	public int defaultRendererIndex = 0;
	public int collectedRendererIndex = 1;
	public Transform walkTarget;
	public bool interactableAfterPickup;
	public bool redirectToItem = true;
	//public TMPro.TextMeshPro speechText;
	public SgCharacter character;

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
			if(spriteRenderers[i] == null)
			{
				continue;
			}
			spriteRenderers[i].gameObject.SetActive(i == renderIndex);
		}
	}

	private void RefreshPickedUpVisibility()
	{
		SetVisibleSprite(IsConnectedToItem && ItemDefinition.IsColleted ? collectedRendererIndex : defaultRendererIndex);
		foreach (SgInteractable childInteractable in Interactables)
		{
			childInteractable.gameObject.SetActive(interactableAfterPickup || !IsConnectedToItem || !ItemDefinition.IsColleted);
		}
	}

	private bool IsItemCollected => ItemManager.IsCollected(this.itemType);

	public SgInteractTranslation GetInteractConfig(SgInteractType interactType, SgItemType itemType)
	{
		SgInteractTranslation interactConfig = null;
		if (IsConnectedToItem && redirectToItem)
		{
			interactConfig = SgTranslationManager.GetInteractTranslation(ItemDefinition.interactTranslations, interactType, IsItemCollected, itemType);
		}
		if (interactConfig == null)
		{
			interactConfig = SgTranslationManager.GetInteractTranslation(interactTranslations, interactType, IsItemCollected, itemType);
		}
		if (interactConfig == null)
		{
			interactConfig = TranslationManager.GetDefaultTranslation(interactType);
		}

		return interactConfig;
	}

	public int[] GetInteractTranslationIds(SgInteractType interactType, SgItemType itemType)
	{
		return GetInteractConfig(interactType, itemType).translationIds;
	}

	//Should use more customizable predefined actions like toggle and pick up. In other words, pick up should be configurable in the same way as toggle. 
	public virtual void OnBeforeInteract(SgInteractType interactType, SgItemType itemType)
	{
		SgInteractTranslation interactConfig = GetInteractConfig(interactType, itemType);

		if (interactConfig == null)
		{
			return;
		}

		if(!string.IsNullOrEmpty(interactConfig.setNamedBool))
		{
			SaveDataManager.CurrentSaveFile.SetNamedBoolValue(interactConfig.setNamedBool, true);
		}

		if(interactConfig.triggerCollect != SgItemType.Illegal)
		{
			ItemManager.Collect(interactConfig.triggerCollect);
		}
		else if (IsConnectedToItem && interactType == SgInteractType.Pickup)
		{
			ItemDefinition.Collect();
			RefreshPickedUpVisibility();
			return;
		}		

		if(interactConfig.toggleSprite)
		{
			SetVisibleSprite(m_RenderIndex == 0 ? 1 : 0);
		}
		
		if(interactConfig.transitionToRoom != SgRoomName.Illegal)
		{
			SceneManager.SetNewRoom(interactConfig.transitionToRoom);
		}
	}
	public virtual IEnumerator InteractRoutine(SgPlayer player, SgInteractType interactType)
	{
		yield break;
	}
}
