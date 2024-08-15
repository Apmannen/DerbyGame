using NavMeshPlus.Components;
using UnityEngine;

public class SgSavedActivation : SgBehavior
{
	public bool auto = true;
	public SgItemType collectedItem = SgItemType.Illegal;
    public string namedBool;
	public bool activateWhenNamedTrue;

	private void Start()
	{
		if(auto)
		{
			EventManager.Register(SgEventName.NamedSaveBoolUpdated, RefreshActivationVoid);
			EventManager.Register<SgItemType>(SgEventName.ItemCollected, OnItemCollected);
			RefreshActivation();
		}
	}
	private void OnDestroy()
	{
		if (auto)
		{
			EventManager.Unregister(SgEventName.NamedSaveBoolUpdated, RefreshActivationVoid);
			EventManager.Unregister<SgItemType>(SgEventName.ItemCollected, OnItemCollected);
		}
	}
	private void OnItemCollected(SgItemType itemType)
	{
		RefreshActivation();
	}

	private void RefreshActivationVoid()
	{
		RefreshActivation();
	}

	public bool RefreshActivation()
	{
		bool value;
		if(collectedItem != SgItemType.Illegal)
		{
			value = ItemManager.IsCollected(collectedItem);
		}
		else
		{
			value = SaveDataManager.CurrentSaveFile.GetNamedBoolValue(namedBool);
		}		

		bool oldActive = this.gameObject.activeSelf;
		bool newActive = (activateWhenNamedTrue && value) || (!activateWhenNamedTrue && !value);
		this.gameObject.SetActive(newActive);

		return newActive != oldActive;
	}
}
