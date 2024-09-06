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
	public Image fullscreenImage;
	public TMPro.TextMeshProUGUI speechTextOverlayTemplate;
	public RectTransform textLimitTopLeft;
	public RectTransform textLimitTopRight;
	public RectTransform cursorTextLimitTopLeft;
	public RectTransform cursorTextLimitTopRight;

	private float m_BgAlphaVel = 0;
	private List<SgReplyItem> m_ReplyItems = new();
	private SgReplyItem m_SelectedReplyItem;

	private void Start()
	{
		speechTextOverlayTemplate.gameObject.SetActive(false);
		replyBarContainer.gameObject.SetActive(false);
		replyItemTemplate.gameObject.SetActive(false);
		SetFullscreenImage(null);
	}
	private void Update()
	{
		wheelBgGroup.alpha = Mathf.SmoothDamp(wheelBgGroup.alpha, IsWheelVisible ? 1 : 0, ref m_BgAlphaVel, wheelBgAlphaSmoothTime);
		wheelRaycaster.enabled = IsWheelVisible;
	}

	public void SetItembarVisible(bool visible)
	{
		itembar.gameObject.SetActive(visible);
	}

	public bool IsReplyBarVisible => replyBarContainer.gameObject.activeSelf;

	public void NotifyReplyItemClicked(SgReplyItem replyItem)
	{
		HideReplyBar();
		m_SelectedReplyItem = replyItem;
	}
	public SgDialogueReply SelectedDialogueReply => m_SelectedReplyItem != null ? m_SelectedReplyItem.dialogueReply : null;

	public void HideReplyBar()
	{
		replyBarContainer.gameObject.SetActive(false);
	}
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

	//Could be generalized to only one set of limits, but since there's only two use cases it's not worth it
	public void KeepInFrame(Transform aTransform, Vector2 desiredPos, Transform topLeftLimit, Transform topRightLimit)
    {
		desiredPos.x = Mathf.Clamp(desiredPos.x, topLeftLimit.position.x, topRightLimit.position.x);
		aTransform.transform.position = desiredPos;
	}

	public void ShowReplyBar(IList<SgDialogueReply> replies)
	{
		ClearReplyBar();

		SetItembarVisible(false);
		int i = 0;
		foreach (SgDialogueReply reply in replies)
		{
			SgReplyItem replyItem = Instantiate(replyItemTemplate, replyItemTemplate.transform.parent);
			replyItem.text.text = InteractManager.Get(reply.translationId);
			replyItem.index = i;
			replyItem.dialogueReply = reply;
			replyItem.gameObject.SetActive(true);

			m_ReplyItems.Add(replyItem);
			i++;
		}

		SgUtil.SetSizeDeltaY(replyBarContainer, 100 + (100 * replies.Count));

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

	public void SetFullscreenImage(Sprite sprite)
	{
		fullscreenImage.sprite = sprite;
		fullscreenImage.gameObject.SetActive(sprite != null);
	}

	public bool IsFullscreenImageVisible => fullscreenImage.sprite != null;
	public bool IsWheelVisible => weaponWheel.IsVisible;
}

[System.Serializable]
public class SgWheelSliceMapping
{
	public string sliceName;
	public SgInteractType interactType;
	public int translationId;
}
