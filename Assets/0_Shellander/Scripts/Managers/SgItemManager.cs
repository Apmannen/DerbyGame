using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public void Collect(SgItemType itemType)
	{
		SgItemDefinition definition = Get(itemType);
		definition.Savable.isCollected.Set(true);
		definition.Savable.collectTime.Set(SgUtil.CurrentTimeMs());

		HudManager.itembar.Refresh();
	}
}
