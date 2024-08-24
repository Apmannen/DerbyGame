using UnityEngine;
using static SgSaveDataManager;

//Don't change order (or add index values), inspectors depend on it
public enum SgItemType
{
	Illegal, 
	BussCard, //TODO: refactor name
	Scissors, 
	Ticket,
	Crowbar,
	Money100,
	TshirtBlack,
	MembershipCard,
	TshirtBlue,
	Money200,
	Glue,
	Money190,
	Money90,
	Patch,
	GluedTshirtBlack,
	GluedPatch,
	TshirtAik,
	Id,
	SleepingPills,
	BottlesBag,
}

public class SgItemDefinition : SgBehavior
{
	public SgItemType itemType;
	public int translationId = -1;
	public Sprite sprite;
	public SgInteractTranslation[] interactTranslations;
	public int moneyValue;
	public SgSkinType skinType;

	public bool IsMoney => moneyValue > 0;

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

	public bool HasEverBeenCollected => Savable.hasEverBeenCollected.Get();
}
