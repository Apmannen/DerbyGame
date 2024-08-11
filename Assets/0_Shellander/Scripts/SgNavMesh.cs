using NavMeshPlus.Components;
using UnityEngine;

public class SgNavMesh : SgBehavior
{
	public NavMeshSurface navMesh;

	private SgSavedActivation[] m_ChildSavedActivations;
	private SgSavedActivation[] ChildSavedActivations => SgUtil.LazyChildComponents<SgSavedActivation>(this, ref m_ChildSavedActivations);

	private void Start()
	{
		EventManager.Register(SgEventName.NamedSaveBoolUpdated, Refresh);
		Refresh(true);
	}

	private void OnDestroy()
	{
		EventManager.Unregister(SgEventName.NamedSaveBoolUpdated, Refresh);
	}

	private void Refresh()
	{
		Refresh(false);
	}

	private void Refresh(bool initial)
	{
		bool anyChange = (initial && ChildSavedActivations.Length > 0);
		foreach (SgSavedActivation childSavedActivation in ChildSavedActivations)
		{
			anyChange = childSavedActivation.RefreshActivation() || anyChange;
		}
		if(anyChange)
		{
			navMesh.BuildNavMesh();
			Debug.Log("*** ONREBUILD execute");
			EventManager.Execute<bool>(SgEventName.NavMeshRebuild, initial);
		}
	}
}
