using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SgItembar : SgBehavior
{
	public SgItembarItem[] itembarItems;
	public RectTransform container;

	private void Start()
	{
		Refresh();
	}

	public void SetMaxWidth(int maxWidth)
	{
		SgUtil.SetSizeDeltaX(container, maxWidth);
	}

	public void Refresh()
	{
		List<SgItemDefinition> collectedItems = ItemManager.GetAvailableItems();
		for (int i = 0; i < itembarItems.Length; i++)
		{
			SgItembarItem itembarItem = itembarItems[i];

			if (i < collectedItems.Count)
			{
				itembarItem.Set(collectedItems[i]);
				itembarItem.gameObject.SetActive(true);
			}
			else
			{
				itembarItem.Set(null);
				itembarItem.gameObject.SetActive(false);
			}
		}
	}

	//public SgItembarItem[] items;

	//private readonly List<SgItemDefinition> m_AvailableItems = new List<SgItemDefinition>();

	//public void Refresh()
	//{
	//	m_AvailableItems.Clear();
	//	foreach(SgItemDefinition definition in ItemManager.itemDefinitions)
	//	{
	//		if(definition.IsColleted)
	//		{
	//			m_AvailableItems.Add(definition);
	//		}
	//	}
	//	m_AvailableItems.OrderBy(definition => definition.Savable.collectTime.Value);

	//	for(int i = 0; i < items.Length; i++)
	//	{
	//		SgItembarItem itembarItem = items[i];

	//		if(i < m_AvailableItems.Count)
	//		{
	//			itembarItem.Set(m_AvailableItems[i]);
	//		}
	//		else
	//		{
	//			itembarItem.Set(null);
	//		}
	//	}

	//}
}
