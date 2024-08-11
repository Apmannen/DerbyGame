using NavMeshPlus.Components;
using UnityEngine;

public class SgSavedActivation : SgBehavior
{
    public string namedBool;
	public bool activateWhenNamedTrue;
	public NavMeshSurface navMeshToRebuild;

	private void Start()
	{
		EventManager.Register(SgEventName.NamedSaveBoolUpdated, RefreshActivation);
		RefreshActivation(true);
	}
	private void OnDestroy()
	{
		EventManager.Unregister(SgEventName.NamedSaveBoolUpdated, RefreshActivation);
	}

	private void RefreshActivation()
	{
		RefreshActivation(false);
	}

	private void RefreshActivation(bool forceChange)
	{
		bool value = SaveDataManager.CurrentSaveFile.GetNamedBoolValue(namedBool);

		bool oldActive = this.gameObject.activeSelf;
		bool newActive = (activateWhenNamedTrue && value) || (!activateWhenNamedTrue && !value);
		if(oldActive != newActive || forceChange)
		{
			this.gameObject.SetActive(newActive);
			if(navMeshToRebuild != null)
			{
				navMeshToRebuild.BuildNavMesh();
			}
		}
	}
}
