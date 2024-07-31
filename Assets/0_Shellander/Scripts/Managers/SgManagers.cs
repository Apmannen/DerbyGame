using UnityEngine;
using UnityEngine.EventSystems;


public class SgManagers : MonoBehaviour
{
	public SgHudManager hudManager;
	public SgItemManager itemManager;
	public SgLayerManager layerManager;
	public SgSaveDataManager saveDataManager;
	public SgTranslationManager translationManager;

	private static SgManagers s_Instance;

	public static SgManagers _
	{
		get
		{
			if (s_Instance == null && EventSystem.current != null)
			{
				s_Instance = EventSystem.current.GetComponent<SgManagers>();
			}
			return s_Instance;
		}
	}

}
