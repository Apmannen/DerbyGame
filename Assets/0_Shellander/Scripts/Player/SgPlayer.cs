using ShellanderGames.WeaponWheel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum SgPlayerStance { Normal, Sitting, Hidden }
public enum SgSkinType { Illegal, Normal, Black, Aik }
public class SgPlayer : SgBehavior
{
	public SgCamera mainCam;
	public SgSkinSettings[] skins;
	public SpriteRenderer mainRenderer;
	public SpriteRenderer sitSprite;
	public SgSpawnPosition[] spawnPositions;
	public SgCharacter character;
	public SgInteractGroup roomEnterInteraction;
	public float textOffsetFlippedLeft;

	private enum SgPlayerState 
	{ 
		None, 
		Walking, 
		InteractWalking, 
		Interacting, 
		AwaitingDialogueReply,
	}
	
	private SgPlayerState m_State = SgPlayerState.None;
	private Vector3 m_WalkTarget;
	private Vector3 m_LookTarget;
	private NavMeshAgent m_Agent;
	private Vector3 m_PrevPos;
	private readonly SgInteraction m_CurrentInteraction = new();
	private float m_StateActivatedTime;
	private static SgPlayer s_Player;
	private string? m_HighlightedActionTranslation;
	private bool m_ScheduledMoveToSpawnPos;
	private SgWheelSliceMapping m_LastHighlightedSlice;
	private SgCursorController CursorController => InputManager.cursorController;
	private SgSkinType m_CurrentSkinType;
	public SgSkinSettings CurrentSkin => GetSkinByType(m_CurrentSkinType);

	private void Awake()
	{
		s_Player = this;
		ChangeSkin(SaveDataManager.CurrentSaveFile.currentSkin.Get());
	}

	private void Start()
	{
		MoveToSpawnPos();

		CursorController.Init();

		ClearInteraction();
		CursorController.ClearText();		

		m_PrevPos = this.transform.position;

		m_WalkTarget = this.transform.position;
		m_LookTarget = this.transform.position;

		m_Agent = GetComponent<NavMeshAgent>();
		m_Agent.updateRotation = false;
		m_Agent.updateUpAxis = false;
		m_Agent.enabled = true;
		m_Agent.isStopped = false;

		mainCam.AttachPlayer(this);

		HudManager.AddWheelListener(OnItemWheelChange);

		//if the navmesh is rebuilt, the positioning must
		//be performed in the update loop for whatever reason
		m_ScheduledMoveToSpawnPos = true;

		if(roomEnterInteraction != null)
		{
			RunRoomEnterInteraction(roomEnterInteraction);
		}		
	}

	private void OnDestroy()
	{
		HudManager.RemoveWheelListener(OnItemWheelChange);
	}

	private SgSkinSettings GetSkinByType(SgSkinType type)
	{
		return skins.Single(s => s.skinType == type);
	}

	private void ChangeSkin(SgSkinType type)
	{
		m_CurrentSkinType = type;
		this.mainRenderer.sprite = CurrentSkin.walkAnimation.sprites[0];
		this.sitSprite.sprite = CurrentSkin.sitSprite;
		SaveDataManager.CurrentSaveFile.currentSkin.Set(type);
		ItemManager.RefreshTshirts(type);
	}

	private void MoveToSpawnPos()
	{
		foreach (SgSpawnPosition spawnPos in spawnPositions)
		{
			if (SceneManager.PrevRoomName == spawnPos.connectedRoom)
			{
				this.transform.position = spawnPos.transform.position;
				m_LookTarget = this.transform.position;
				SetFlipX(spawnPos.sprite.flipX);
				break;
			}
		}
	}

	//Has Weapon Wheel Generator dependency, could be handled by HudManager
	private void OnItemWheelChange(SgWeaponWheelEvent wheelEvent)
	{
		SgWheelSliceMapping sliceMapping = null;
		if (wheelEvent.slice != null)
		{
			sliceMapping = HudManager.GetWheelSliceMapping(wheelEvent.slice.sliceName);
		}

		switch(wheelEvent.type)
		{
			case SgWeaponWheelEventType.Select:
				CursorController.SetSelectedInteract(sliceMapping.interactType);
				break;
			case SgWeaponWheelEventType.Highlight:
				m_HighlightedActionTranslation = InteractManager.Get(sliceMapping.translationId);
				m_LastHighlightedSlice = sliceMapping;
				break;
			case SgWeaponWheelEventType.Dehighlight:
				m_HighlightedActionTranslation = null;
				break;
			case SgWeaponWheelEventType.WheelVisible:
				break;
			case SgWeaponWheelEventType.WheelInvisible:
				m_HighlightedActionTranslation = null;
				break;
		}
	}

	public static SgPlayer Get()
	{
		return s_Player;
	}
	public static SgPlayer GetFromCollider(Collider2D collider)
    {
		bool success = collider.gameObject.TryGetComponent(out SgPlayer player);
		return success ? player : null;
	}

	public void SetStance(SgPlayerStance stance)
	{
		switch(stance)
		{
			case SgPlayerStance.Sitting:
				sitSprite.enabled = true;
				mainRenderer.enabled = false;
				break;
			case SgPlayerStance.Normal:
				sitSprite.enabled = false;
				mainRenderer.enabled = true;
				break;
			case SgPlayerStance.Hidden:
				sitSprite.enabled = false;
				mainRenderer.enabled = false;
				break;
		}
	}

	private void StartInteraction()
	{
		SetState(SgPlayerState.Interacting);
		Interact(m_CurrentInteraction);
	}

	private void SetState(SgPlayerState newState)
	{
		if(newState == m_State)
		{
			return;
		}

		m_State = newState;
		m_StateActivatedTime = Time.time;

		Debug.Log("*** NEW STATE:"+m_State);
		

		if (IsStateAnyInteract())
		{
			CursorController.SetWaitMode(true);

			//Gives unwanted side effect if want to continue interaction.
			//Probably should mostly have mostly "passive" effects here.

			//if(newState == SgPlayerState.Interacting)
			//{
			//	Interact(m_CurrentInteraction);
			//}
		}
		else
		{
			CursorController.SetWaitMode(false);
		}
	}

	

	private SgInteraction SetInteraction(SgInteractGroup interactGroup, SgItemType itembarItem, SgItemType useItemType, SgInteractType type)
	{
		m_CurrentInteraction.interactGroup = interactGroup;
		m_CurrentInteraction.itembarItem = itembarItem;
		m_CurrentInteraction.useItem = useItemType;
		m_CurrentInteraction.type = type;
		m_CurrentInteraction.cachedInteractConfig = null;
		return m_CurrentInteraction;
	}

	private void ClearInteraction()
	{
		SetInteraction(null, SgItemType.Illegal, SgItemType.Illegal, SgInteractType.Illegal);
	}

	private bool IsStateAnyWalking()
	{
		return m_State == SgPlayerState.Walking || m_State == SgPlayerState.InteractWalking;
	}
	public bool IsStateAnyInteract()
	{
		return m_State is SgPlayerState.Interacting or SgPlayerState.InteractWalking;
	}
	public bool IsActionsAllowed()
	{
		return !IsStateAnyInteract() && !HudManager.IsWheelVisible && !HudManager.IsFullscreenImageVisible && m_State != SgPlayerState.AwaitingDialogueReply;
	}
	private bool IsCursorAnyInteract()
	{
		return CursorController.IsAnyInteract();
	}


	private void Update()
	{
		CursorController.UpdateCurrentCursor();

		if(m_ScheduledMoveToSpawnPos)
		{
			MoveToSpawnPos();
			m_ScheduledMoveToSpawnPos = false;
		}

		//Variables
		Vector3 cursorWorldPos = CursorController.UpdateCursorPos(InputManager.PointerAction, mainCam.cam);
		SgInteractGroup hoveredInteractGroup = null;

		//Detect interactable hover
		SgInteractable selectedInteractable = null;
		RaycastHit2D[] hits = Physics2D.RaycastAll(cursorWorldPos, Vector2.zero);
		int bestPrio = -1;

		foreach (RaycastHit2D hit in hits)
		{
			if (hit.collider == null || hit.collider.gameObject.layer != SgLayerManager.layerInteractable)
			{
				continue;
			}
			SgInteractable interactable = hit.collider.gameObject.GetComponent<SgInteractable>();
			if (interactable.priority < bestPrio || interactable.InteractGroup.AlwaysWalkTo)
			{
				continue;
			}
			bestPrio = interactable.priority;
			selectedInteractable = interactable;
		}

		//UI
		SgItembarItem hoveredItembarItem = HudManager.itembar.GetHoveredItem();
		if (IsActionsAllowed() && selectedInteractable != null)
		{
			hoveredInteractGroup = selectedInteractable.InteractGroup;
			CursorController.SetText(hoveredInteractGroup.TranslatedName);
		}
		else if(IsActionsAllowed() && hoveredItembarItem != null)
		{
			CursorController.SetText(hoveredItembarItem.Definition.TranslatedName);
		}
		else if(m_HighlightedActionTranslation != null)
		{
			CursorController.SetText(m_HighlightedActionTranslation);
		}
		else
		{
			CursorController.ClearText();
			if (IsActionsAllowed())
			{
				ClearInteraction();
			}
		}

		//State handling 1
		if (m_State == SgPlayerState.InteractWalking && HasReachedDestination())
		{
			CurrentSkin.walkAnimation.Stop();
			StartInteraction();
		}
		else if (m_State == SgPlayerState.Walking && HasReachedDestination())
		{
			CurrentSkin.walkAnimation.Stop();
			SetState(SgPlayerState.None);
		}

		//Input handling
		if (m_State == SgPlayerState.AwaitingDialogueReply && HudManager.SelectedDialogueReply != null)
		{
			StartDialogueReply(HudManager.SelectedDialogueReply);
			return;
		}
		else if (IsActionsAllowed())
		{
			if (InputManager.ClickAction.WasPressedThisFrame())
			{
				Vector3 walkTarget = cursorWorldPos;
				walkTarget.z = this.transform.position.z;

				SetState(SgPlayerState.Walking);
				CurrentSkin.walkAnimation.Play();

				SetDestination(walkTarget, cursorWorldPos);

				if (IsCursorAnyInteract() && (hoveredInteractGroup != null || hoveredItembarItem != null))
				{
					HandleInteractClick(hoveredInteractGroup, hoveredItembarItem);
				}

			}
			else if (InputManager.ShiftCursorRightAction.WasPressedThisFrame())
			{
				CursorController.CycleCursor(1);
			}
		}
		else if (m_State == SgPlayerState.Interacting && Time.time > (m_StateActivatedTime + 0.25f) &&
			(InputManager.ClickAction.WasPressedThisFrame() || InputManager.ShiftCursorRightAction.WasPressedThisFrame()))
		{
			SkipAnySpeech();
		}
		else if (HudManager.IsFullscreenImageVisible)
		{
			if (InputManager.ClickAction.WasPressedThisFrame())
			{
				HudManager.SetFullscreenImage(null);
			}
		}

		//State handling 2
		if (IsStateAnyWalking())
		{
			Vector3 curMove = transform.position - m_PrevPos;
			float currentSpeed = curMove.magnitude / Time.deltaTime;
			float animationInterval = Mathf.Lerp(0.2f, 0.1f, currentSpeed / 4f);
			CurrentSkin.walkAnimation.changeInterval = animationInterval;
		}

		RefreshFlip();

		//Prevs
		m_PrevPos = this.transform.position;
	}

	private void RefreshFlip()
	{
		float diff = this.transform.position.x - m_LookTarget.x;
		if(diff == 0)
		{
			return;
		}
		bool isRight = diff < 0;
		SetFlipX(isRight);
	}

	private void SetFlipX(bool isRight)
    {
		mainRenderer.flipX = isRight;
		character.SetXOffset(isRight ? 0 : textOffsetFlippedLeft);
	}

	private void SetDestination(Vector3 targetPosition, Vector3? overrideLookTarget)
	{
		m_WalkTarget = targetPosition;
		if(overrideLookTarget != null)
		{
			m_LookTarget = (Vector3)overrideLookTarget;
		}
		m_Agent.SetDestination(targetPosition);
	}

	private void HandleInteractClick(SgInteractGroup hoveredInteractGroup, SgItembarItem itembarItem)
	{
		SgItemType itembarItemType = itembarItem != null ? itembarItem.Definition.itemType : SgItemType.Illegal;
		SgInteraction interaction = SetInteraction(hoveredInteractGroup, itembarItemType, CursorController.SelectedItem, CursorController.SelectedInteractType);
		SgInteractTranslation interactConfig = interaction?.InteractConfig;
		if (interactConfig != null && interactConfig.redirect)
		{
			interaction.interactGroup = interactConfig.redirect;
			interactConfig = interaction.InteractConfig;
		}
			
		if (interactConfig != null && interactConfig.walkToItFirst)
		{
			bool overrideDestination = m_CurrentInteraction != null && m_CurrentInteraction.interactGroup != null &&
				m_CurrentInteraction.interactGroup.walkTarget != null; //null propagation shouldn't be used on Unity objects
			if (overrideDestination) 
			{
				SetDestination(m_CurrentInteraction.interactGroup.walkTarget.position, null);
			}
			SetState(SgPlayerState.InteractWalking);
		}
		else
		{
			StopMoving();
			StartInteraction();
		}
	}
	private void StopMoving()
	{
		SetDestination(this.transform.position, null);
		CurrentSkin.walkAnimation.Stop();
	}
	public void OnTriggerCollision(SgInteractGroup interactGroup)
	{
		if(m_State != SgPlayerState.Walking)
		{
			return;
		}

		SetInteraction(interactGroup, SgItemType.Illegal, SgItemType.Illegal, SgInteractType.Collision);
		StopMoving();
		StartInteraction();
	}
	private void RunRoomEnterInteraction(SgInteractGroup interactGroup)
	{
		SetInteraction(interactGroup, SgItemType.Illegal, SgItemType.Illegal, SgInteractType.Generic);
		StartInteraction();
	}

	private bool HasReachedDestination()
	{
		if (!m_Agent.pathPending)
		{
			if (m_Agent.remainingDistance <= m_Agent.stoppingDistance)
			{
				if (!m_Agent.hasPath || m_Agent.velocity.sqrMagnitude == 0f)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void Interact(SgInteraction interaction)
	{
		StartCoroutine(InteractRoutine(interaction));
	}

	private IEnumerator InteractRoutine(SgInteraction interaction)
	{
		SgInteractTranslation interactConfig = interaction.InteractConfig;
		interaction.cachedInteractConfig = interactConfig;

		int[] interactTranslationIds = new int[] { };
		if (interaction.IsRoomInteraction)
		{
			interactTranslationIds = interaction.interactGroup.GetInteractTranslationIds(interaction.type, interaction.useItem);
			interaction.interactGroup.OnBeforeInteract(interaction.type, interaction.useItem);
		}
		//Some duplicated code
		else if (interaction.IsItemInteraction)
		{
			bool ignoreUse = false;
			if (interactConfig != null)
			{
				if(interactConfig.fullscreenSprite != null)
				{
					HudManager.SetFullscreenImage(interactConfig.fullscreenSprite);
					ignoreUse = true;
				}
			}

			switch (interaction.type)
			{
				case SgInteractType.Look:
					interactTranslationIds = ItemManager.Get(interaction.itembarItem).GetInteractTranslationIds(interaction.type);
					break;
				case SgInteractType.Use:
					if(interaction.ItembarItemDefinition.skinType != SgSkinType.Illegal)
					{
						if(interactConfig != null && !interactConfig.isFallback)
						{
							interactTranslationIds = interactConfig.translationIds;
						}
						if(SceneManager.CurrentRoom.RoomName != SgRoomName.Sewers)
						{
							ChangeSkin(interaction.ItembarItemDefinition.skinType);
						}
					}
					else if(!ignoreUse)
					{
						CursorController.SetSelectedItem(interaction.itembarItem);
					}
					break;
				case SgInteractType.Item:
					
					if(interactConfig != null)
					{
						interactTranslationIds = interactConfig.translationIds;
						foreach (SgItemType removeItemType in interactConfig.triggerRemove)
						{
							ItemManager.RemoveItem(removeItemType);
						}
						if (interactConfig.triggerCollect != SgItemType.Illegal)
						{
							ItemManager.Collect(interactConfig.triggerCollect);
						}
					}
					else
					{
						Debug.LogError("Why didn't we find any in interaction.InteractConfig?");
					}
					break;
				default: 
					break;
			}			
		}

		yield return character.Talk(interactTranslationIds);

		if(interaction.IsRoomInteraction)
		{
			yield return interaction.interactGroup.InteractRoutine(this, interaction.type);

			if (interactConfig != null && interactConfig.startDialogue)
			{
				yield return DialogueRoutine(interactConfig.startDialogue);
			}
		}
		

		character.ClearSpeech();

		if(m_State == SgPlayerState.AwaitingDialogueReply)
		{
			//
		}
		else
		{
			AfterInteraction();
		}
	}
	private void AfterInteraction()
	{
		HudManager.SetItembarVisible(true);

		SgInteraction interaction = m_CurrentInteraction;
		SgInteractTranslation interactConfig = interaction.cachedInteractConfig;

        if (interactConfig != null && interaction.IsRoomInteraction)
        {
            interaction.interactGroup.OnAfterInteract(interactConfig);
        }

        bool shouldWalkToATarget = interaction.type == SgInteractType.Collision 
			&& interactConfig != null && interactConfig.walkToItFirst; //could be generalized to "walkToItAfter"
		if (shouldWalkToATarget)
		{
			Vector3 pos = m_CurrentInteraction.interactGroup.walkTarget.position;
			SetDestination(pos, pos);
			CurrentSkin.walkAnimation.Play();
			SetInteraction(m_CurrentInteraction.interactGroup, SgItemType.Illegal, SgItemType.Illegal, SgInteractType.Generic);
			SetState(SgPlayerState.InteractWalking);
		}
		else
		{
			SetState(SgPlayerState.None);
			ClearInteraction();
		}		
	}

	private void StartDialogueReply(SgDialogueReply reply)
	{
		//Do BEFORE routine
		SgCharacter otherCharacter = reply.connectedToDialogue.character;
		SetState(SgPlayerState.Interacting);
		character.ClearSpeech();
		otherCharacter.ClearSpeech();
		reply.hasBeenUsed = true;
		foreach(SgItemType itemType in reply.discoverItems)
		{
			ItemManager.Discover(itemType);
		}

		StartCoroutine(DialogueReplyRoutine(reply, otherCharacter));
	}
	private IEnumerator DialogueReplyRoutine(SgDialogueReply reply, SgCharacter otherCharacter)
	{
		if(reply.addItem != SgItemType.Illegal)
		{
			ItemManager.Collect(reply.addItem);
		}

		if(!reply.isSilent)
		{
			yield return character.Talk(reply.translationId);
		}

		if (reply.nextDialogue != null)
		{
			yield return DialogueRoutine(reply.nextDialogue);
		}
		else
		{
			AfterInteraction();
		}
	}
	private IEnumerator DialogueRoutine(SgDialogue dialogue)
	{
		SgCharacter otherCharacter = dialogue.character;

		HudManager.ClearReplyBar();
		SetState(SgPlayerState.Interacting);
		yield return otherCharacter.Talk(dialogue.MainTranslationIds);

		foreach(SgMainDialogueSpeak speak in dialogue.autoReplies)
		{
			if(!SgCondition.TestConditions(speak.conditions, false))
			{
				continue;
			}
			yield return character.Talk(speak.translationIds);
		}

		bool finish = false;

		IList<SgDialogueReply> validReplies = dialogue.ValidReplies;
		if(validReplies.Count == 1)
		{
			yield return character.Talk(validReplies[0].translationId);
			finish = true;
		}
		else if(validReplies.Count > 1)
		{
			HudManager.ShowReplyBar(validReplies);
			SetState(SgPlayerState.AwaitingDialogueReply);
		}
		else if(dialogue.redirectAfterDialogue != null)
		{
			yield return DialogueRoutine(dialogue.redirectAfterDialogue);
		}
		else
		{
			finish = true;
		}

		if(finish)
		{
			SetState(SgPlayerState.None);
			HudManager.SetItembarVisible(true);
		}
	}

	private void SkipAnySpeech()
	{
		character.SkipSpeech();

		if(m_CurrentInteraction != null && m_CurrentInteraction.cachedInteractConfig != null 
			&& m_CurrentInteraction.cachedInteractConfig.startDialogue != null)
		{
			m_CurrentInteraction.cachedInteractConfig.startDialogue.character.SkipSpeech();
		}
	}

	private class SgInteraction
	{
		public SgInteractGroup interactGroup;
		public SgItemType itembarItem; //interaction on the item
		public SgItemType useItem; //an item is used as cursor
		public SgInteractType type;
		public bool IsRoomInteraction => interactGroup != null;
		public bool IsItemInteraction => itembarItem != SgItemType.Illegal;
		public SgItemDefinition ItembarItemDefinition => SgManagers._.itemManager.Get(itembarItem);
		//conditions can be changed when a interaction is started, so don't always get the config by using function call
		public SgInteractTranslation cachedInteractConfig;

		private static SgTranslationManager InteractManager => SgManagers._.translationManager;

		public SgInteractTranslation InteractConfig
		{
			get
			{
				if (IsRoomInteraction)
				{
					return interactGroup.GetInteractConfig(this.type, this.useItem);
				}
				else if (IsItemInteraction)
				{
					return InteractManager.GetItembarInteractConfig(type, ItembarItemDefinition, useItem);
				}
				return null;
			}
		}
	}
}

[System.Serializable]
public class SgSkinSettings
{
	public SgSkinType skinType;
	public SgAnimation walkAnimation;
	public Sprite sitSprite;
}
