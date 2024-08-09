using ShellanderGames.WeaponWheel;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class SgHudManager : SgBehavior
{
	public SgUiCursor cursor;
	[System.Obsolete]
	public SgItembar itembar;
	public SgWeaponWheel weaponWheel;
	public CanvasGroup wheelBgGroup;
	public float wheelBgAlphaSmoothTime = 0.1f;
	public SgWheelSliceMapping[] sliceMappings;
	public SgSliceController itemSliceTemplate;

	private float m_BgAlphaVel = 0;

	private void Start()
	{
		RefreshWheel();
	}

	public void AddWheelListener(System.Action<SgWeaponWheelEvent> action)
	{
		weaponWheel.AddEventCallback(action);
	}
	public void RemoveWheelListener(System.Action<SgWeaponWheelEvent> action)
	{
		weaponWheel.RemoveEventCallback(action);
	}

	public void RefreshWheel()
	{
		for(int i = weaponWheel.sliceContents.Count-1; i >= 0; i--)
		{
			SgSliceController slice = weaponWheel.sliceContents[i];
			if(slice.sliceName.StartsWith("Item"))
			{
				weaponWheel.sliceContents.Remove(slice);
				Destroy(slice.gameObject);
			}
		}
		List<SgItemDefinition> availableItems = ItemManager.GetAvailableItems();
		foreach(SgItemDefinition item in availableItems)
		{
			SgSliceController newSlice = Instantiate(itemSliceTemplate);
			newSlice.sliceName = "Item" + item.itemType;
			newSlice.graphicSelectables[0].GetComponent<Image>().sprite = item.sprite;
			weaponWheel.sliceContents.Add(newSlice);
		}

		weaponWheel.Generate(false, true);
	}

	public SgWheelSliceMapping GetWheelSliceMapping(string sliceName)
	{
		if(sliceName.StartsWith("Item"))
		{
			SgItemType itemType = (SgItemType) System.Enum.Parse(typeof(SgItemType), sliceName.Replace("Item", ""));
			return new SgWheelSliceMapping { sliceName = sliceName, interactType = SgInteractType.Item, translationId = ItemManager.Get(itemType).translationId };
		}
		return sliceMappings.Single(m => m.sliceName == sliceName);
	}

	private void Update()
	{
		wheelBgGroup.alpha = Mathf.SmoothDamp(wheelBgGroup.alpha, IsWheelVisible ? 1 : 0, ref m_BgAlphaVel, wheelBgAlphaSmoothTime);
	}

	public bool IsWheelVisible => weaponWheel.IsVisible;
}

[System.Serializable]
public class SgWheelSliceMapping
{
	public string sliceName;
	public SgInteractType interactType;
	public int translationId;
}
