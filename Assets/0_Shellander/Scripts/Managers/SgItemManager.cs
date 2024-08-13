using System.Collections.Generic;
using System.Linq;

public class SgItemManager : SgBehavior
{
	public SgItemDefinition[] itemDefinitions;

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

	public void Collect(SgItemType itemType)
	{
		SgItemDefinition definition = Get(itemType);
		definition.Savable.isCollected.Set(true);
		definition.Savable.collectTime.Set(SgUtil.CurrentTimeMs());

		HudManager.RefreshWheel();
	}
	public void RemoveItem(SgItemType itemType)
	{
		SgItemDefinition definition = Get(itemType);
		definition.Savable.isCollected.Set(false);

		HudManager.RefreshWheel();
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

	private void SetMoney(int newMoney)
	{
		if(newMoney > 0)
		{
			foreach (SgItemDefinition definition in itemDefinitions)
			{
				if (definition.moneyValue == newMoney)
				{
					Collect(definition.itemType);
				}
				else if(definition.moneyValue > 0)
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

	public void ChangeMoney(int change)
	{
		int newMoney = GetCurrentMoney() + change;
		SetMoney(newMoney);
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
