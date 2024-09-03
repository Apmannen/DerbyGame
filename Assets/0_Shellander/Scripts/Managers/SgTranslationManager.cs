using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;

public class SgTranslationManager : SgBehavior
{
	public TextAsset interactsSe;

	private string[] m_TranslationsSe;
	private int m_CurrentLanguageId;
	public int CurrentLanguageId => m_CurrentLanguageId;
	public SgInteractTranslation[] defaultTranslations;

	private void Awake()
	{
		m_CurrentLanguageId = 0;
		string content = interactsSe.text;
		string[] lines = content.Split("\n"[0]);

		m_TranslationsSe = new string[lines.Length+1];

		//Skip first line to start at 1, easier when manually reading text file from external editor (Visual Studio Code)
		for (int i = 0; i < lines.Length; i++)
		{
			int lineId = i + 1;

			m_TranslationsSe[lineId] = lines[i];
		}
	}

	public string Get(int translationId)
	{
		return m_TranslationsSe[translationId];
	}

	public int[] GetInteractTranslationIds(SgInteractTranslation[] interactTranslations, SgInteractType interactType, bool isCollected)
	{
		return GetInteractConfig(interactTranslations, interactType, isCollected, SgItemType.Illegal).translationIds;
	}
	//TODO: move away from here, is not (only) translations anymore... or rather convert this class to InteractManager.
	public SgInteractTranslation GetInteractConfig(SgInteractTranslation[] interactConfigs, SgInteractType interactType, bool isCollected, 
		SgItemType itemType)
	{
		if(interactType == SgInteractType.Item)
        {
			SgInteractTranslation specificItemConfig = interactConfigs.SingleOrDefault(c => c.interactType == SgInteractType.Item && c.ItemTypes.Contains(itemType));
			if(specificItemConfig != null)
            {
				return specificItemConfig;
            }
			SgInteractTranslation genericItemConfig = interactConfigs.SingleOrDefault(c => c.interactType == SgInteractType.Item && c.ItemTypes.Count == 0);
			if(genericItemConfig != null)
            {
				return genericItemConfig;
            }
			return null;
		}

		foreach (SgInteractTranslation interactConfig in interactConfigs)
		{
			if(interactConfig.onlyWhenCollected && !isCollected)
			{
				continue;
			}
			if(interactConfig.onlyWhenNotCollected && isCollected)
			{
				continue;
			}
			
			if(interactConfig.onlyWhenCollectedItemTypes.SingleOrDefault(aItemType => ItemManager.HasEverBeenCollected(aItemType)) != SgItemType.Illegal)
			{
				continue;
			}
			if (interactConfig.onlyWhenNotCollectedItemTypes.SingleOrDefault(aItemType => !ItemManager.HasEverBeenCollected(aItemType)) != SgItemType.Illegal)
			{
				continue;
			}

			if (interactConfig.interactType == interactType)
			{
				return interactConfig;
			}
		}
		return null;
	}
}

[System.Serializable]
public class SgInteractTranslation
{
	public SgInteractType interactType;
	[System.Obsolete]
	public SgItemType itemType;
	public SgItemType[] itemTypes;
	public SgItemType triggerCollect;
	public SgItemType[] triggerRemove;
	public int[] translationIds;
	public bool walkToItFirst;
	public bool toggleSprite = false;
	[System.Obsolete("I don't really map items directly to room objects anymore")]
	public bool onlyWhenCollected;
	[System.Obsolete("I don't really map items directly to room objects anymore")]
	public bool onlyWhenNotCollected;
	public SgItemType[] onlyWhenCollectedItemTypes;
	public SgItemType[] onlyWhenNotCollectedItemTypes;
	public SgRoomName transitionToRoom = SgRoomName.Illegal;
	public string setNamedBool;
	public bool setNamedBoolToFalse;
	public SgDialogue startDialogue;
	public UnityEvent method;
	public Sprite fullscreenSprite;
	public SgInteractGroup redirect;
	public string debugString;

	private HashSet<SgItemType> m_ItemTypes;

	public ICollection<SgItemType> ItemTypes
    {
		get
        {
			if(m_ItemTypes == null)
            {
				m_ItemTypes = new();
				m_ItemTypes.UnionWith(itemTypes);
				m_ItemTypes.Add(itemType);
				m_ItemTypes.Remove(SgItemType.Illegal);
            }
			return m_ItemTypes;
        }
    }
}
