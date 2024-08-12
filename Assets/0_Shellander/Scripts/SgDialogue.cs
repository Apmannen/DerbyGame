using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgDialogue : MonoBehaviour
{
	public int[] mainTranslationIds;
	public SgDialogueReply[] replies;
}

[System.Serializable]
public class SgDialogueReply
{
	public int translationId;
	public SgDialogue nextDialogue;
}
