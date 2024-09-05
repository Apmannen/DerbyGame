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

	public SgItembarItem GetHoveredItem()
	{
		return itembarItems.SingleOrDefault(i => i.IsHovered);
	}
}
