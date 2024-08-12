using UnityEngine;

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
			if(lines[i] == "")
			{
				//break;
			}

			int lineId = i + 1;

			m_TranslationsSe[lineId] = lines[i];
		}
	}

	public string Get(int translationId)
	{
		return m_TranslationsSe[translationId];
	}

	
	public SgInteractTranslation GetDefaultTranslation(SgInteractType interactType)
	{
		return GetInteractTranslation(defaultTranslations, interactType, false, SgItemType.Illegal);
	}

	public static int[] GetInteractTranslationIds(SgInteractTranslation[] interactTranslations, SgInteractType interactType, bool isCollected)
	{
		return GetInteractTranslation(interactTranslations, interactType, isCollected, SgItemType.Illegal).translationIds;
	}
	public static SgInteractTranslation GetInteractTranslation(SgInteractTranslation[] interactConfigs, SgInteractType interactType, bool isCollected, 
		SgItemType itemType)
	{
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

			if (interactType == SgInteractType.Item)
			{
				if(interactConfig.interactType == interactType && interactConfig.itemType == itemType)
				{
					return interactConfig;
				}
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
	public SgItemType itemType;
	public int[] translationIds;
	public bool walkToItFirst;
	public bool toggleSprite = false;
	public bool onlyWhenCollected;
	public bool onlyWhenNotCollected;
	public SgRoomName transitionToRoom = SgRoomName.Illegal;
	public string setNamedBool;
	public SgDialogue startDialogue;
}
