using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgGameManager : SgBehavior
{
	public SgScheduler scheduler;
	public bool isBeta;

	void Start()
	{
		string initialized = "IsGameInitialized";
		if (!SaveDataManager.CurrentSaveFile.GetNamedBoolValue(initialized))
		{
			//ItemManager.ChangeMoney(100);
			SaveDataManager.CurrentSaveFile.SetNamedBoolValue(initialized, true);
		}
	}
}
