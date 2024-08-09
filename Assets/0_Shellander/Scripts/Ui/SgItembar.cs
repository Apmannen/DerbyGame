using System.Collections.Generic;
using System.Linq;

[System.Obsolete("Replaced by item wheel")]//
public class SgItembar : SgBehavior
{
	public SgItembarItem[] items;

	private readonly List<SgItemDefinition> m_AvailableItems = new List<SgItemDefinition>();

	private void Start()
	{
		Refresh();
	}

	public void Refresh()
	{
		m_AvailableItems.Clear();
		foreach(SgItemDefinition definition in ItemManager.itemDefinitions)
		{
			if(definition.IsColleted)
			{
				m_AvailableItems.Add(definition);
			}
		}
		m_AvailableItems.OrderBy(definition => definition.Savable.collectTime.Value);

		for(int i = 0; i < items.Length; i++)
		{
			SgItembarItem itembarItem = items[i];

			if(i < m_AvailableItems.Count)
			{
				itembarItem.Set(m_AvailableItems[i]);
			}
			else
			{
				itembarItem.Set(null);
			}
		}

	}
}
