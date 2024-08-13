using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgDialogue : MonoBehaviour
{
	public int[] mainTranslationIds;
	public SgDialogueReply[] replies;
	public SgDialogue redirectAfterDialogue;
}

[System.Serializable]
public class SgDialogueReply
{
	public int translationId;
	public bool isSilent = false;
	public SgDialogue nextDialogue;
}
