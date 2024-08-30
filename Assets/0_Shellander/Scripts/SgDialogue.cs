using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

	public IList<SgDialogueReply> ValidReplies
	{
		get
		{
			return replies.Where(r => SgCondition.TestConditions(r.conditions)).ToList();
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
	public SgCondition[] conditions;
}
