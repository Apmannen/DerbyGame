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
		return GetInteractTranslation(defaultTranslations, interactType);
	}

	public static int[] GetInteractTranslationIds(SgInteractTranslation[] interactTranslations, SgInteractType interactType)
	{
		return GetInteractTranslation(interactTranslations, interactType).translationIds;
	}
	public static SgInteractTranslation GetInteractTranslation(SgInteractTranslation[] interactTranslations, SgInteractType interactType)
	{
		foreach (SgInteractTranslation interactTranslation in interactTranslations)
		{
			if (interactTranslation.interactType == interactType)
			{
				return interactTranslation;
			}
		}
		return null;
	}
}

[System.Serializable]
public class SgInteractTranslation
{
	public SgInteractType interactType;
	public int[] translationIds;
	public bool walkToItFirst;
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SgTranslationManager : SgBehavior
//{
//	public TextAsset interactsCsv;

//	//private readonly List<SgInteractTexts> m_InteractTexts = new();
//	//private readonly Dictionary<string, Dictionary<SgInteractType, SgInteractText>> m_InteractTexts = new();
//	private readonly Dictionary<string, SgInteractTexts> m_InteractTexts = new();
//	private int m_CurrentLanguageId;
//	public int CurrentLanguageId => m_CurrentLanguageId;

//	private void Awake()
//	{
//		m_CurrentLanguageId = 0;
//		string content = interactsCsv.text;
//		string[] lines = content.Split("\n"[0]);

//		for (int i = 0; i < lines.Length; i++)
//		{
//			string[] split = lines[i].Split(';');
//			if (split.Length < 3)
//			{
//				continue;
//			}

//			string objectNameId = split[0];
//			string translationType = split[1];
//			string translation0 = split[2];

//			SgInteractTexts interactTexts = GetOrCreateInteractTexts(objectNameId);
//			if (translationType == "Object")
//			{
//				interactTexts.objectTranslation = new SgTranslation(translation0);
//			}
//			else
//			{
//				SgInteractType interactType = (SgInteractType)System.Enum.Parse(typeof(SgInteractType), translationType);
//				interactTexts.texts[interactType] = new SgInteractText(translation0);
//			}
//		}
//	}

//	private SgInteractTexts GetOrCreateInteractTexts(string objectNameId)
//	{
//		if (!m_InteractTexts.ContainsKey(objectNameId))
//		{
//			m_InteractTexts[objectNameId] = new SgInteractTexts();
//		}
//		return m_InteractTexts[objectNameId];
//	}

//	public string GetInteractText(string objectName, SgInteractType interactType)
//	{
//		if (m_InteractTexts.ContainsKey(objectName) && m_InteractTexts[objectName].texts.ContainsKey(interactType))
//		{
//			return m_InteractTexts[objectName].texts[interactType].translation.Translations[CurrentLanguageId];
//		}
//		return m_InteractTexts["Default"].texts[interactType].translation.Translations[CurrentLanguageId];
//	}
//	public SgTranslation GetObjectTranslation(string objectName)
//	{
//		return m_InteractTexts[objectName].objectTranslation;
//	}

//	private class SgInteractTexts
//	{
//		public SgTranslation objectTranslation;
//		public Dictionary<SgInteractType, SgInteractText> texts = new();
//	}

//	private struct SgInteractText
//	{
//		public SgTranslation translation;

//		public SgInteractText(string swedishText)
//		{
//			this.translation = new SgTranslation(swedishText);
//		}
//	}


//}

//public struct SgTranslation
//{
//	private readonly string[] m_Translations;
//	public string[] Translations => m_Translations;

//	public SgTranslation(string swedishText)
//	{
//		this.m_Translations = new string[] { swedishText };
//	}
//}


