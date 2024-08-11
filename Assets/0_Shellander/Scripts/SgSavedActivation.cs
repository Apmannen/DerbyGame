using NavMeshPlus.Components;
using UnityEngine;

public class SgSavedActivation : SgBehavior
{
	public bool auto = true;
    public string namedBool;
	public bool activateWhenNamedTrue;

	private void Start()
	{
		if(auto)
		{
			EventManager.Register(SgEventName.NamedSaveBoolUpdated, RefreshActivationVoid);
			RefreshActivation();
		}
	}
	private void OnDestroy()
	{
		if (auto)
		{
			EventManager.Unregister(SgEventName.NamedSaveBoolUpdated, RefreshActivationVoid);
		}
	}

	private void RefreshActivationVoid()
	{
		RefreshActivation();
	}

	public bool RefreshActivation()
	{
		bool value = SaveDataManager.CurrentSaveFile.GetNamedBoolValue(namedBool);

		bool oldActive = this.gameObject.activeSelf;
		bool newActive = (activateWhenNamedTrue && value) || (!activateWhenNamedTrue && !value);
		this.gameObject.SetActive(newActive);

		return newActive != oldActive;
	}
}
