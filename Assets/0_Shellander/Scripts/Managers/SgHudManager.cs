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
	public RectTransform replyBarContainer;
	public SgReplyItem replyItemTemplate;

	private float m_BgAlphaVel = 0;
	private List<SgReplyItem> m_ReplyItems = new();
	private SgReplyItem m_SelectedReplyItem;

	private void Start()
	{
		RefreshWheel();
		replyBarContainer.gameObject.SetActive(false);
		replyItemTemplate.gameObject.SetActive(false);
	}
	private void Update()
	{
		wheelBgGroup.alpha = Mathf.SmoothDamp(wheelBgGroup.alpha, IsWheelVisible ? 1 : 0, ref m_BgAlphaVel, wheelBgAlphaSmoothTime);
	}

	public bool IsReplyBarVisible => replyBarContainer.gameObject.activeSelf;

	public void NotifyReplyItemClicked(SgReplyItem replyItem)
	{
		m_SelectedReplyItem = replyItem;
	}
	public SgDialogueReply SelectedDialogueReply => m_SelectedReplyItem != null ? m_SelectedReplyItem.dialogueReply : null;

	public void ClearReplyBar()
	{
		foreach (SgReplyItem oldReplyItem in m_ReplyItems)
		{
			Destroy(oldReplyItem.gameObject);
		}
		m_ReplyItems.Clear();
		m_SelectedReplyItem = null;
		replyBarContainer.gameObject.SetActive(false);
	}

	public void ShowReplyBar(SgDialogueReply[] replies)
	{
		ClearReplyBar();

		int i = 0;
		foreach (SgDialogueReply reply in replies)
		{
			SgReplyItem replyItem = Instantiate(replyItemTemplate, replyItemTemplate.transform.parent);
			replyItem.text.text = TranslationManager.Get(reply.translationId);
			replyItem.index = i;
			replyItem.dialogueReply = reply;
			replyItem.gameObject.SetActive(true);

			m_ReplyItems.Add(replyItem);
			i++;
		}

		Vector2 size = replyBarContainer.sizeDelta;
		size.y = 100 + (100 * replies.Length);
		replyBarContainer.sizeDelta = size;
		replyBarContainer.gameObject.SetActive(true);
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
			SgItemType itemType = SgWheelSliceMapping.GetItemType(sliceName);
			return new SgWheelSliceMapping { sliceName = sliceName, interactType = SgInteractType.Item, 
				translationId = ItemManager.Get(itemType).translationId };
		}
		return sliceMappings.Single(m => m.sliceName == sliceName);
	}

	

	public bool IsWheelVisible => weaponWheel.IsVisible;
}

[System.Serializable]
public class SgWheelSliceMapping
{
	public string sliceName;
	public SgInteractType interactType;
	public int translationId;

	public static SgItemType GetItemType(string aSliceName)
	{
		if(!aSliceName.StartsWith("Item"))
		{
			return SgItemType.Illegal;
		}
		return (SgItemType)System.Enum.Parse(typeof(SgItemType), aSliceName.Replace("Item", ""));
	}
	public SgItemType ItemType => GetItemType(sliceName);
}
