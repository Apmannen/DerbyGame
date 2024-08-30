using System.Collections.Generic;

[System.Serializable]
public class SgCondition
{
	public bool successOnTrue;
	public string namedBool;
	public SgItemType collectedItem = SgItemType.Illegal;
	public SgItemType collectedItemEver = SgItemType.Illegal;
	public SgSkinType skinType = SgSkinType.Illegal;

	private static SgItemManager ItemManager => SgManagers._.itemManager;
	private static SgSaveDataManager SaveDataManager => SgManagers._.saveDataManager;
	public static bool TestConditions(IList<SgCondition> conditions)
	{
		foreach (SgCondition condition in conditions)
		{
			bool value = false;
			if (condition.collectedItem != SgItemType.Illegal)
			{
				value = ItemManager.IsCollected(condition.collectedItem);
			}
			else if (condition.collectedItemEver != SgItemType.Illegal)
			{
				value = ItemManager.HasEverBeenCollected(condition.collectedItemEver);
			}
			else if (!string.IsNullOrEmpty(condition.namedBool))
			{
				value = SaveDataManager.CurrentSaveFile.GetNamedBoolValue(condition.namedBool);
			}
			else if (condition.skinType != SgSkinType.Illegal)
			{
				value = SgPlayer.Get().CurrentSkin.skinType == condition.skinType;
			}

			bool conditionSuccess = (condition.successOnTrue && value) || (!condition.successOnTrue && !value);
			if (!conditionSuccess)
			{
				return false;
			}
		}
		return true;
	}
}
