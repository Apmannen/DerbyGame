using UnityEngine;

//Should be called SgUiReplyItem or something similar to differentiate from SgDialogueReply
public class SgReplyItem : SgBehavior
{
	public TMPro.TextMeshProUGUI text;
	public int index;
	public SgDialogueReply dialogueReply;

	public void OnClick()
	{
		HudManager.NotifyReplyItemClicked(this);
	}
}
