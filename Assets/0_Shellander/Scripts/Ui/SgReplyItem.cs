using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Should be called SgUiReplyItem or something similar to differentiate from SgDialogueReply
public class SgReplyItem : SgBehavior
{
	public TMPro.TextMeshProUGUI text;
	public Image hoverImage;
	public int index;
	public SgDialogueReply dialogueReply;
	public Color hoverColor;

	private Color m_DefaultColor;
	private void Start()
	{
		m_DefaultColor = text.color;
		hoverImage.enabled = false;
	}

	public void OnClick()
	{
		HudManager.NotifyReplyItemClicked(this);
	}

	public void OnHoverEnter()
	{
		text.color = hoverColor;
		hoverImage.enabled = true;
	}
	public void OnHoverExit()
	{
		hoverImage.enabled = false;
		text.color = m_DefaultColor;
	}
}
