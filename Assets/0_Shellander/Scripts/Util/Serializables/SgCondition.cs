using System.Collections.Generic;

[System.Serializable]
public class SgCondition
{
	public bool successOnTrue;
	public string namedBool;
	public SgItemType collectedItem = SgItemType.Illegal;
	public SgItemType collectedItemEver = SgItemType.Illegal;
	public SgItemType discoveredItem = SgItemType.Illegal;
	public SgSkinType skinType = SgSkinType.Illegal;
	public int minMoney = -1;

	private static SgItemManager ItemManager => SgManagers._.itemManager;
	private static SgSaveDataManager SaveDataManager => SgManagers._.saveDataManager;
	public static bool TestConditions(IList<SgCondition> conditions)
	{
		foreach (SgCondition c in conditions)
		{
			bool value = false;
			if (c.collectedItem != SgItemType.Illegal)
			{
				value = ItemManager.IsCollected(c.collectedItem);
			}
			else if (c.collectedItemEver != SgItemType.Illegal)
			{
				value = ItemManager.HasEverBeenCollected(c.collectedItemEver);
			}
			else if(c.discoveredItem != SgItemType.Illegal)
			{
				value = ItemManager.IsDiscovered(c.discoveredItem);
			}
			else if (!string.IsNullOrEmpty(c.namedBool))
			{
				value = SaveDataManager.CurrentSaveFile.GetNamedBoolValue(c.namedBool);
			}
			else if (c.skinType != SgSkinType.Illegal)
			{
				value = SgPlayer.Get().CurrentSkin.skinType == c.skinType;
			}
			else if(c.minMoney >= 0)
			{
				value = ItemManager.GetCurrentMoney() >= c.minMoney;
			}

			bool conditionSuccess = (c.successOnTrue && value) || (!c.successOnTrue && !value);
			if (!conditionSuccess)
			{
				return false;
			}
		}
		return true;
	}
}
