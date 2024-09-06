using System.Collections.Generic;
using UnityEngine;

public class SgSavedActivation : SgBehavior
{
	public bool auto = true;
	public SgCondition[] conditions;
	public bool orCheckAll = false;
	public SgSavedActivation inverseFor;

	//Legacy name conversion
	public bool activateWhenNamedTrue;
	private bool SetActiveWhenAllSuccess => activateWhenNamedTrue;

	[Header("Legacy stuff")]
	public SgItemType collectedItem = SgItemType.Illegal;
    public string namedBool;	

	private List<SgCondition> m_Conditions = null;
	public IList<SgCondition> Conditions
	{
		get
        {
			Init();
			return m_Conditions;
		}
	}

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

	private void Init()
    {
		if(m_Conditions != null)
        {
			return;
		}

		m_Conditions = new List<SgCondition>();
		m_Conditions.AddRange(conditions);

		//Legacy conversion
		if (!string.IsNullOrEmpty(namedBool))
		{
			m_Conditions.Add(new SgCondition { successOnTrue = true, namedBool = namedBool, collectedItem = SgItemType.Illegal });
		}
		if (collectedItem != SgItemType.Illegal)
		{
			m_Conditions.Add(new SgCondition { successOnTrue = true, namedBool = "", collectedItem = collectedItem });
		}
	}

	public bool RefreshActivation()
	{
		Init();

		bool conditionsSuccess;
		if(inverseFor != null)
        {
			inverseFor.Init();
			conditionsSuccess = !SgCondition.TestConditions(inverseFor.Conditions, inverseFor.orCheckAll);
		}
		else
        {
			conditionsSuccess = SgCondition.TestConditions(m_Conditions, orCheckAll);
		}
		

		bool oldActive = this.gameObject.activeSelf;
		bool newActive;
		if(conditionsSuccess)
        {
			newActive = SetActiveWhenAllSuccess;
        }
		else
        {
			newActive = !SetActiveWhenAllSuccess;
        }
		this.gameObject.SetActive(newActive);

		return newActive != oldActive;
	}
}

