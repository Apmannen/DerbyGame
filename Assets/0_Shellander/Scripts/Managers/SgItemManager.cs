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

		HudManager.itembar.Refresh();
	}
}
