using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgBehavior : MonoBehaviour
{
	protected SgEventManager EventManager => SgManagers._.eventManager;
	protected SgHudManager HudManager => SgManagers._.hudManager;
	protected SgItemManager ItemManager => SgManagers._.itemManager;
	protected SgLayerManager LayerManager => SgManagers._.layerManager;
	protected SgSaveDataManager SaveDataManager => SgManagers._.saveDataManager;
	protected SgSceneManager SceneManager => SgManagers._.sceneManager;
	protected SgTranslationManager TranslationManager => SgManagers._.translationManager;
}
