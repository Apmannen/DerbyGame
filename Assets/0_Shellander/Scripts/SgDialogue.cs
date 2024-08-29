using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgDialogue : MonoBehaviour
{
	public int[] mainTranslationIds;
	public SgDialogueReply[] replies;
	public SgDialogue redirectAfterDialogue;
	public SgCharacter character;

	private void Start()
	{
		if(character == null)
		{
			character = GetComponentInParent<SgCharacter>();
		}
	}
}

[System.Serializable]
public class SgDialogueReply
{
	public int translationId;
	public bool isSilent = false;
	public SgDialogue nextDialogue;
	public SgItemType addItem;
	public int reduceMoney;
}
