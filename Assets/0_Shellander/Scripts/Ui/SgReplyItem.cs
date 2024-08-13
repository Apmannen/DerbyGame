using UnityEngine;
using UnityEngine.EventSystems;

//Should be called SgUiReplyItem or something similar to differentiate from SgDialogueReply
public class SgReplyItem : SgBehavior
{
	public TMPro.TextMeshProUGUI text;
	public int index;
	public SgDialogueReply dialogueReply;

	public void OnClick()
	{
		Debug.Log("***** ONCLICK!!!!!!!!!");
		HudManager.NotifyReplyItemClicked(this);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		print("I was clicked");
	}
}
