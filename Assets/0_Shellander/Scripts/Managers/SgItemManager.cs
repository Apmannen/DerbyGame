using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SgItemManager : SgBehavior
{
	public SgItemDefinition[] itemDefinitions;

	private void Start()
	{
		RefreshMoney();
		EventManager.Register(SgEventName.NamedSaveBoolUpdated, OnNamedBoolUpdated);
	}
	private void OnDestroy()
	{
		EventManager.Unregister(SgEventName.NamedSaveBoolUpdated, OnNamedBoolUpdated);
	}
	private void OnNamedBoolUpdated()
	{
		RefreshMoney();
	}

	public SgItemDefinition Get(SgItemType itemType)
	{
		foreach(SgItemDefinition definition in itemDefinitions)
		{
			if(definition.itemType == itemType)
			{
				return definition;
			}
		}
		return null;
	}

	public bool IsCollected(SgItemType itemType)
	{
		SgItemDefinition definition = Get(itemType);
		return definition != null && definition.IsColleted;
	}
	public bool HasEverBeenCollected(SgItemType itemType)
	{
		SgItemDefinition definition = Get(itemType);
		return definition != null && definition.HasEverBeenCollected;
	}

	public void Collect(SgItemType itemType)
	{
		SgItemDefinition definition = Get(itemType);
		if(!definition.Savable.isCollected.Value)
		{
			definition.Savable.collectTime.Set(SgUtil.CurrentTimeMs());
		}
		definition.Savable.isCollected.Set(true);
		definition.Savable.hasEverBeenCollected.Set(true);
		

		HudManager.itembar.Refresh();

		if(definition.moneyValue <= 0)
		{
			RefreshMoney();
		}

		EventManager.Execute(SgEventName.ItemCollected, itemType);
	}
	public void RemoveItem(SgItemType itemType)
	{
		SgItemDefinition definition = Get(itemType);
		definition.Savable.isCollected.Set(false);

		HudManager.itembar.Refresh();
		if (definition.moneyValue <= 0)
		{
			RefreshMoney();
		}
	}

	public int GetCurrentMoney()
	{
		foreach (SgItemDefinition definition in itemDefinitions)
		{
			if (definition.IsColleted && definition.IsMoney)
			{
				return definition.moneyValue;
			}
		}
		return 0;
	}

	public void RefreshMoney()
	{
		int money = 100;

		if(SaveDataManager.CurrentSaveFile.GetNamedBoolValue("BathroomBillColl"))
		{
			money += 100;
		}

		if(HasEverBeenCollected(SgItemType.TshirtBlack))
		{
			money -= 100;
		}

		if (money > 0)
		{
			foreach (SgItemDefinition definition in itemDefinitions)
			{
				if (definition.moneyValue == money)
				{
					Collect(definition.itemType);
				}
				else if (definition.moneyValue > 0)
				{
					RemoveItem(definition.itemType);
				}
			}
		}
		else
		{
			foreach (SgItemDefinition definition in itemDefinitions)
			{
				if (definition.IsColleted && definition.IsMoney)
				{
					RemoveItem(definition.itemType);
				}
			}
		}
	}

	//private void SetMoney(int newMoney)
	//{
	//	Debug.Log("*** SETM:"+newMoney);
	//	if(newMoney > 0)
	//	{
	//		foreach (SgItemDefinition definition in itemDefinitions)
	//		{
	//			if (definition.moneyValue == newMoney)
	//			{
	//				Collect(definition.itemType);
	//			}
	//			else if(definition.moneyValue > 0)
	//			{
	//				RemoveItem(definition.itemType);
	//			}
	//		}
	//	}
	//	else
	//	{
	//		foreach (SgItemDefinition definition in itemDefinitions)
	//		{
	//			if (definition.IsColleted && definition.IsMoney)
	//			{
	//				RemoveItem(definition.itemType);
	//			}
	//		}
	//	}		
	//}

	//public void ChangeMoney(int change)
	//{
	//	int newMoney = GetCurrentMoney() + change;
	//	SetMoney(newMoney);
	//}

	public void RefreshTshirts(SgSkinType wornSkin)
	{
		if(wornSkin == SgSkinType.Black)
		{
			RemoveItem(SgItemType.TshirtBlack);
			Collect(SgItemType.TshirtBlue);
		}
		else if(wornSkin == SgSkinType.Normal)
		{
			RemoveItem(SgItemType.TshirtBlue);
			if(HasEverBeenCollected(SgItemType.TshirtBlack))
			{
				Collect(SgItemType.TshirtBlack);
			}			
		}
	}

	public List<SgItemDefinition> GetAvailableItems()
	{
		List<SgItemDefinition> availableItems = new();
		foreach (SgItemDefinition definition in ItemManager.itemDefinitions)
		{
			if (definition.IsColleted)
			{
				availableItems.Add(definition);
			}
		}
		availableItems = availableItems.OrderBy(definition => definition.Savable.collectTime.Value).ToList();
		return availableItems;
	}
}
