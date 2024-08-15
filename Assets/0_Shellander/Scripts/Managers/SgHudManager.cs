using ShellanderGames.WeaponWheel;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class SgHudManager : SgBehavior
{
	public SgUiCursor cursor;
	public SgItembar itembar;
	public SgWeaponWheel weaponWheel;
	public CanvasGroup wheelBgGroup;
	public GraphicRaycaster wheelRaycaster;
	public float wheelBgAlphaSmoothTime = 0.1f;
	public SgWheelSliceMapping[] sliceMappings;
	public RectTransform replyBarContainer;
	public SgReplyItem replyItemTemplate;

	private float m_BgAlphaVel = 0;
	private List<SgReplyItem> m_ReplyItems = new();
	private SgReplyItem m_SelectedReplyItem;

	private void Awake()
	{
		EventManager.Register<SgRoom>(SgEventName.RoomChanged, OnRoomChanged);
	}
	private void OnDestroy()
	{
		EventManager.Unregister<SgRoom>(SgEventName.RoomChanged, OnRoomChanged);
	}

	private void Start()
	{
		replyBarContainer.gameObject.SetActive(false);
		replyItemTemplate.gameObject.SetActive(false);
	}
	private void Update()
	{
		wheelBgGroup.alpha = Mathf.SmoothDamp(wheelBgGroup.alpha, IsWheelVisible ? 1 : 0, ref m_BgAlphaVel, wheelBgAlphaSmoothTime);
		wheelRaycaster.enabled = IsWheelVisible;
	}

	private void OnRoomChanged(SgRoom newRoom)
	{
		ScreenRefrefresh(newRoom);
	}
	public void ScreenRefrefresh(SgRoom room)
	{
		int maxWidth = room != null && room.uiWidth >= 0 ? room.uiWidth : Screen.currentResolution.width;
		itembar.SetMaxWidth(maxWidth);
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

		//Vector2 size = replyBarContainer.sizeDelta;
		//size.y = 100 + (100 * replies.Length);
		//replyBarContainer.sizeDelta = size;

		SgUtil.SetSizeDeltaY(replyBarContainer, 100 + (100 * replies.Length));

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

	public SgWheelSliceMapping GetWheelSliceMapping(string sliceName)
	{
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
}
