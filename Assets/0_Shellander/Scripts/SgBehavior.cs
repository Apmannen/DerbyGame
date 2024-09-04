using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgBehavior : MonoBehaviour
{
	protected SgEventManager EventManager => SgManagers._.eventManager;
	protected SgHudManager HudManager => SgManagers._.hudManager;
	protected SgInputManager InputManager => SgManagers._.inputManager;
	protected SgTranslationManager InteractManager => SgManagers._.translationManager;
	protected SgItemManager ItemManager => SgManagers._.itemManager;
	protected SgLayerManager LayerManager => SgManagers._.layerManager;
	protected SgSaveDataManager SaveDataManager => SgManagers._.saveDataManager;
	protected SgSceneManager SceneManager => SgManagers._.sceneManager;
	protected SgScheduler Scheduler => SgManagers._.gameManager.scheduler;
}
