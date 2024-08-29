using System.Collections.Generic;
using UnityEngine;

public class SgSavedActivation : SgBehavior
{
	public bool auto = true;
	public SgActivationCondition[] conditions;

	//Legacy name conversion
	public bool activateWhenNamedTrue;
	private bool SetActiveWhenAllSuccess => activateWhenNamedTrue;

	[Header("Legacy stuff")]
	public SgItemType collectedItem = SgItemType.Illegal;
    public string namedBool;	

	private List<SgActivationCondition> m_Conditions = null;

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

		m_Conditions = new List<SgActivationCondition>();
		m_Conditions.AddRange(conditions);

		//Legacy conversion
		if (!string.IsNullOrEmpty(namedBool))
		{
			m_Conditions.Add(new SgActivationCondition { successOnTrue = true, namedBool = namedBool, collectedItem = SgItemType.Illegal });
		}
		if (collectedItem != SgItemType.Illegal)
		{
			m_Conditions.Add(new SgActivationCondition { successOnTrue = true, namedBool = "", collectedItem = collectedItem });
		}
	}

	public bool RefreshActivation()
	{
		Init();

		bool allSuccess = true;
		foreach (SgActivationCondition condition in m_Conditions)
        {
			bool value = false;
			if(condition.collectedItem != SgItemType.Illegal)
            {
				value = ItemManager.IsCollected(condition.collectedItem);
            }
			else if(condition.collectedItemEver != SgItemType.Illegal)
			{
				value = ItemManager.HasEverBeenCollected(condition.collectedItemEver);
			}
			else if(!string.IsNullOrEmpty(condition.namedBool))
            {
				value = SaveDataManager.CurrentSaveFile.GetNamedBoolValue(condition.namedBool);
			}
			else if(condition.skinType != SgSkinType.Illegal)
            {
				value = SgPlayer.Get().CurrentSkin.skinType == condition.skinType;
			}

			bool conditionSuccess = (condition.successOnTrue && value) || (!condition.successOnTrue && !value);
			if(!conditionSuccess)
            {
				allSuccess = false;
				break;
			}
		}

		bool oldActive = this.gameObject.activeSelf;
		bool newActive;
		if(allSuccess)
        {
			newActive = SetActiveWhenAllSuccess;
        }
		else
        {
			newActive = !SetActiveWhenAllSuccess;
        }
		this.gameObject.SetActive(newActive);

		//Debug.Log("*** allSuccess="+ allSuccess+ ", newActive="+ newActive+", SetActiveWhenAllSuccess="+ SetActiveWhenAllSuccess+", this="+this, this.gameObject);

		return newActive != oldActive;
	}
}

[System.Serializable]
public class SgActivationCondition
{
	public bool successOnTrue;
	public string namedBool;
	public SgItemType collectedItem = SgItemType.Illegal;
	public SgItemType collectedItemEver = SgItemType.Illegal;
	public SgSkinType skinType = SgSkinType.Illegal;
}
