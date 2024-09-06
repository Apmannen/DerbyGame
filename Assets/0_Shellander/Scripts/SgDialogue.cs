using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SgDialogue : MonoBehaviour
{
	public int[] mainTranslationIds;
	public SgMainDialogueSpeak[] mainDialogueSpeaks;
	public SgMainDialogueSpeak[] autoReplies;
	public SgDialogueReply[] replies;
	public SgDialogue redirectAfterDialogue;
	public SgCharacter character;

	private void Awake()
	{
		if(autoReplies == null)
		{
			autoReplies = new SgMainDialogueSpeak[] { };
		}
	}

	private void Start()
	{
		if(character == null)
		{
			character = GetComponentInParent<SgCharacter>();
		}
		foreach(SgDialogueReply reply in replies)
		{
			reply.connectedToDialogue = this;
		}
	}

	public IList<int> MainTranslationIds
	{
		get
		{
			List<int> translationIds = new List<int>();
			translationIds.AddRange(mainTranslationIds);
			if(mainDialogueSpeaks == null)
			{
				mainDialogueSpeaks = new SgMainDialogueSpeak[0];
			}
			foreach (SgMainDialogueSpeak dialogueItem in mainDialogueSpeaks)
			{
				if (SgCondition.TestConditions(dialogueItem.conditions, false))
				{
					translationIds.AddRange(dialogueItem.translationIds);
				}
			}
			return translationIds;
		}
	}

	public IList<SgDialogueReply> ValidReplies
	{
		get
		{
			return replies.Where(r => SgCondition.TestConditions(r.conditions, false) && (!r.autoRemove || !r.hasBeenUsed)).ToList();
		}
	}
}

[System.Serializable]
//TODO: crap name
public class SgMainDialogueSpeak
{
	public int[] translationIds;
	public SgCondition[] conditions;
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
	public SgDialogue connectedToDialogue;
	public bool autoRemove;
	public bool hasBeenUsed;
}
