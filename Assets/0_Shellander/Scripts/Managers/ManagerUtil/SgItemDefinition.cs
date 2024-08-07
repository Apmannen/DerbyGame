using UnityEngine;
using static SgSaveDataManager;

public enum SgItemType
{
	Illegal, 
	BussCard, //TODO: refactor name
	Scissors, 
	Ticket,
	Crowbar,
}

public class SgItemDefinition : SgBehavior
{
	public SgItemType itemType;
	public int translationId = -1;
	public Sprite sprite;
	public SgInteractTranslation[] interactTranslations;

	private SgItemSavable m_Savable;
	public SgItemSavable Savable
	{
		get
		{
			if (m_Savable == null)
			{
				m_Savable = SaveDataManager.CurrentSaveFile.items[itemType];
			}
			return m_Savable;
		}
	}

	public string TranslatedName
	{
		get
		{
			return TranslationManager.Get(translationId);
		}
	}

	public void Collect()
	{
		ItemManager.Collect(itemType);
	}

	public int[] GetInteractTranslationIds(SgInteractType interactType)
	{
		return SgTranslationManager.GetInteractTranslationIds(interactTranslations, interactType, IsColleted);
	}

	public bool IsColleted => Savable.isCollected.Get();
}
