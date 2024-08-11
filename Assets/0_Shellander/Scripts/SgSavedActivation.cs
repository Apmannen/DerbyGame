using NavMeshPlus.Components;

public class SgSavedActivation : SgBehavior
{
    public string namedBool;
	public bool activateWhenNamedBoolIsTrue;
	public NavMeshSurface navMeshToRebuild;

	//private AsyncOperation m_BuildOperation;

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
		bool newActive = activateWhenNamedBoolIsTrue && value;
		if(oldActive != newActive || forceChange)
		{
			this.gameObject.SetActive(newActive);
			if(navMeshToRebuild != null)
			{
				navMeshToRebuild.BuildNavMesh();
			}
			//if(m_BuildOperation == null || m_BuildOperation.isDone)
			//{
			//	m_BuildOperation = navMeshRebake.BuildNavMeshAsync();
			//}
		}
	}
}
