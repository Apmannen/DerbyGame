using UnityEngine;
using UnityEngine.EventSystems;

//Should be called SgUiReplyItem or something similar to differentiate from SgDialogueReply
public class SgReplyItem : SgBehavior
{
	public TMPro.TextMeshProUGUI text;
	public int index;
	public SgDialogueReply dialogueReply;
	public Color hoverColor;

	private Color m_DefaultColor;
	private void Start()
	{
		m_DefaultColor = text.color;
	}

	public void OnClick()
	{
		HudManager.NotifyReplyItemClicked(this);
	}

	public void OnHoverEnter()
	{
		text.color = hoverColor;
	}
	public void OnHoverExit()
	{
		text.color = m_DefaultColor;
	}
}
