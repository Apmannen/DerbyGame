using ShellanderGames.WeaponWheel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public enum SgPlayerStance { Normal, Sitting, Hidden }
public class SgPlayer : SgBehavior
{
	public SgCamera mainCam;
	public SgAnimation walkAnimation;
	public SpriteRenderer mainRenderer;
	public SpriteRenderer sitSprite;
	[System.Obsolete]
	public SgCursorTypeDefinition[] cursors;
	//public SgCursorTypeDefinition waitCursor;
	public TMPro.TextMeshPro speechText;
	public SgSpawnPosition[] spawnPositions;

	private enum SgPlayerState { None, Walking, InteractWalking, Interacting }
	
	private SgPlayerState m_State = SgPlayerState.None;
	private Vector3 m_WalkTarget;
	private NavMeshAgent m_Agent;
	private Vector3 m_PrevPos;
	[System.Obsolete]
	private int m_CurrentCursorIndex = 0;
	private readonly SgInteraction m_CurrentInteraction = new();
	private float m_StateActivatedTime;
	private bool m_SpeechAborted;
	private static SgPlayer s_Player;
	private string? m_HighlightedActionTranslation;
	private bool m_ScheduledMoveToSpawnPos;
	private SgWheelSliceMapping m_LastHighlightedSlice;
	private SgCursorController m_CursorController;
	private SgCursorController CursorController => SgUtil.LazyComponent(this, ref m_CursorController);

	//Cursor
	[System.Obsolete]
	private SgItemType m_CursorItem = SgItemType.Illegal;
	[System.Obsolete]
	private SgItemType CursorItem => CurrentCursor.interactType == SgInteractType.Item ? m_CursorItem : SgItemType.Illegal;
	//[System.Obsolete]
	//private SgUiCursor UiCursor => HudManager.cursor;
	[System.Obsolete]
	private SgCursorTypeDefinition CurrentCursor => cursors[m_CurrentCursorIndex];
	[System.Obsolete]
	private SgCursorTypeDefinition m_PrevCursor;	

	//Input
	private InputActionMap m_CurrentActionMap;
	private PlayerInput m_PlayerInput;
	private InputAction m_ClickAction;
	private InputAction m_PointerAction;
	private InputAction m_ShiftCursorRight; //TODO: rename since ince multi-purpose

	private void Awake()
	{
		s_Player = this;
	}

	private void Start()
	{
		ResetInput();

		MoveToSpawnPos();

		CursorController.Init();

		//m_PrevCursor = GetCursor(SgInteractType.Walk);
		ClearInteraction();
		UiCursor.text.text = "";
		speechText.text = "";

		m_PrevPos = this.transform.position;

		m_WalkTarget = this.transform.position;

		m_Agent = GetComponent<NavMeshAgent>();
		m_Agent.updateRotation = false;
		m_Agent.updateUpAxis = false;
		m_Agent.enabled = true;
		m_Agent.isStopped = false;

		//SetCursor(0);
		mainCam.AttachPlayer(this);

		HudManager.AddWheelListener(OnItemWheelChange);

		//if the navmesh is rebuilt, the positioning must
		//be performed in the update loop for whatever reason
		m_ScheduledMoveToSpawnPos = true; 
	}

	private void OnDestroy()
	{
		HudManager.RemoveWheelListener(OnItemWheelChange);
	}

	private void MoveToSpawnPos()
	{
		foreach (SgSpawnPosition spawnPos in spawnPositions)
		{
			if (SceneManager.PrevRoom == spawnPos.connectedRoom)
			{
				this.transform.position = spawnPos.transform.position;
				Debug.Log("**** #21; POS SET");
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
				if(sliceMapping.interactType == SgInteractType.Item)
				{
					CursorController.SetSelectedItem(sliceMapping.ItemType);
				}
				else
				{
					CursorController.SetSelectedInteract(sliceMapping.interactType);
				}
				break;
			case SgWeaponWheelEventType.Highlight:
				m_HighlightedActionTranslation = TranslationManager.Get(sliceMapping.translationId);
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

	public InputAction ClickAction => m_ClickAction;

	private void ResetInput()
	{
		m_CurrentActionMap = null;
		if (m_PlayerInput == null)
		{
			m_PlayerInput = GetComponent<PlayerInput>();
		}
		foreach (UnityEngine.InputSystem.InputActionMap map in m_PlayerInput.actions.actionMaps)
		{
			if (map.name == "Player")
			{
				m_CurrentActionMap = map.Clone();
				break;
			}
		}
		m_ClickAction = FindInputAction("Click");
		m_PointerAction = FindInputAction("PointerPos");
		m_ShiftCursorRight = FindInputAction("ShiftCursorRight");
		m_CurrentActionMap.Enable();
	}

	private InputAction FindInputAction(string name)
	{
		InputAction action = m_CurrentActionMap.FindAction(name, false);
		if (action == null)
		{
			Debug.Log("Ignoring input action: " + name);
		}
		return action;
	}

	private void SetState(SgPlayerState newState)
	{
		m_State = newState;
		m_StateActivatedTime = Time.time;

		Debug.Log("*** NEW STATE:"+m_State);

		if (IsStateAnyInteract())
		{
			CursorController.SetWaitMode(true);

			if(newState == SgPlayerState.Interacting)
			{
				Interact(m_CurrentInteraction);
			}
		}
		else
		{
			CursorController.SetWaitMode(false);
		}
	}

	

	private void SetInteraction(SgInteractGroup interactGroup, SgItemType itemWheelItem, SgItemType useItemType, SgInteractType type)
	{
		m_CurrentInteraction.interactGroup = interactGroup;
		m_CurrentInteraction.itemWheelItem = itemWheelItem;
		m_CurrentInteraction.useItem = useItemType;
		m_CurrentInteraction.type = type;
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
		return m_State == SgPlayerState.Interacting || m_State == SgPlayerState.InteractWalking;
	}
	private bool IsActionsAllowed()
	{
		return !IsStateAnyInteract() && !HudManager.IsWheelVisible;
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
		//Vector3 cursorScreenPos = m_PointerAction.ReadValue<Vector2>();
		//Vector3 cursorWorldPos = mainCam.cam.ScreenToWorldPoint(cursorScreenPos);
		//UiCursor.transform.position = cursorScreenPos;
		Vector3 cursorWorldPos = CursorController.UpdateCursorPos();
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
			if (interactable.priority < bestPrio)
			{
				continue;
			}
			selectedInteractable = interactable;
		}

		//UI
		if (IsActionsAllowed() && selectedInteractable != null)
		{
			hoveredInteractGroup = selectedInteractable.InteractGroup;
			UiCursor.text.text = hoveredInteractGroup.TranslatedName;
		}
		else if(m_HighlightedActionTranslation != null)
		{
			UiCursor.text.text = m_HighlightedActionTranslation;
		}
		else
		{
			UiCursor.text.text = "";
			if (IsActionsAllowed())
			{
				ClearInteraction();
			}
		}

		//State handling 1
		if (m_State == SgPlayerState.InteractWalking && HasReachedDestination())
		{
			walkAnimation.Stop();
			SetState(SgPlayerState.Interacting);
		}
		else if (m_State == SgPlayerState.Walking && HasReachedDestination())
		{
			walkAnimation.Stop();
			SetState(SgPlayerState.None);
		}

		//Input handling
		if(IsActionsAllowed())
		{
			if (m_ClickAction.WasPressedThisFrame())
			{
				Vector3 walkTarget = cursorWorldPos;
				walkTarget.z = this.transform.position.z;

				SetState(SgPlayerState.Walking);
				walkAnimation.Play();

				SetDestination(walkTarget);

				if(IsCursorAnyInteract())
				{
					HandleInteractClick(hoveredInteractGroup);
				}
				
			}
			else if (m_ShiftCursorRight.WasPressedThisFrame())
			{
				CursorController.CycleCursor(1);
			}
		}
		else if(m_State == SgPlayerState.Interacting && Time.time > (m_StateActivatedTime+0.25f) && 
			(m_ClickAction.WasPressedThisFrame() || m_ShiftCursorRight.WasPressedThisFrame()))
		{
			SkipSpeech();
		}
		else if(HudManager.IsWheelVisible && m_ShiftCursorRight.WasPressedThisFrame() && 
			m_LastHighlightedSlice.ItemType != SgItemType.Illegal)
		{
			SetInteraction(null, m_LastHighlightedSlice.ItemType, SgItemType.Illegal, SgInteractType.Look);
			SetDestination(this.transform.position);
			walkAnimation.Stop();
			SetState(SgPlayerState.Interacting);
		}

		//State handling 2
		if (IsStateAnyWalking())
		{
			Vector3 curMove = transform.position - m_PrevPos;
			float currentSpeed = curMove.magnitude / Time.deltaTime;
			float animationInterval = Mathf.Lerp(0.2f, 0.1f, currentSpeed / 4f);
			walkAnimation.changeInterval = animationInterval;

			float diff = this.transform.position.x - m_WalkTarget.x;
			bool isRight = diff < 0;

			mainRenderer.flipX = isRight;
		}

		//Prevs
		m_PrevPos = this.transform.position;
	}

	private void SetDestination(Vector3 targetPosition)
	{
		m_WalkTarget = targetPosition;
		m_Agent.SetDestination(targetPosition);
	}

	private void HandleInteractClick(SgInteractGroup hoveredInteractGroup)
	{
		SgInteractTranslation interactConfig = hoveredInteractGroup.GetInteractConfig(CurrentCursor.interactType, CursorItem);

		if (interactConfig != null)
		{
			SetInteraction(hoveredInteractGroup, SgItemType.Illegal, CursorItem, CurrentCursor.interactType);
			
			if (interactConfig != null && interactConfig.walkToItFirst)
			{
				bool overrideDestination = m_CurrentInteraction != null && m_CurrentInteraction.interactGroup != null &&
					m_CurrentInteraction.interactGroup.walkTarget != null; //null propagation shouldn't be used on Unity objects
				if (overrideDestination) 
				{
					SetDestination(m_CurrentInteraction.interactGroup.walkTarget.position);
				}
				SetState(SgPlayerState.InteractWalking);
			}
			else
			{
				SetDestination(this.transform.position);
				walkAnimation.Stop();
				SetState(SgPlayerState.Interacting);
			}

		}
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
		int[] interactTranslationIds = new int[] { };
		if (interaction.IsRoomInteraction)
		{
			interactTranslationIds = interaction.interactGroup.GetInteractTranslationIds(interaction.type, interaction.useItem);
			interaction.interactGroup.OnBeforeInteract(interaction.type, interaction.useItem);
		}
		else if(interaction.IsItemInteraction)
		{
			switch(interaction.type)
			{
				case SgInteractType.Look:
					interactTranslationIds = ItemManager.Get(interaction.itemWheelItem).GetInteractTranslationIds(interaction.type);
					break;
				default: 
					break;
			}			
		}

		yield return Talk(interactTranslationIds);

		if(interaction.IsRoomInteraction)
		{
			yield return interaction.interactGroup.InteractRoutine(this, interaction.type);
		}

		ClearInteraction();
		speechText.text = "";
		//if(CurrentCursor.interactType == SgInteractType.Wait)
		//{
		//	SetCursor(m_PrevCursor.interactType);
		//}
		SetState(SgPlayerState.None);
	}

	//Should be moved to a generic talk component
	public IEnumerator Talk(IList<int> translationIds)
	{
		return Talk(translationIds, this.speechText);
	}
	public IEnumerator Talk(IList<int> translationIds, TMPro.TextMeshPro aSpeechText)
	{
		foreach (int id in translationIds)
		{
			string translation = TranslationManager.Get(id);
			aSpeechText.text = translation;
			m_SpeechAborted = false;
			yield return Wait(3f);
		}
		aSpeechText.text = "";
	}

	private IEnumerator Wait(float maxDuration)
	{
		float time = 0;
		while(time < maxDuration && !m_SpeechAborted)
		{
			time += Time.deltaTime;
			yield return null;
		}
	}

	private void SkipSpeech()
	{
		m_SpeechAborted = true;
	}

	private class SgInteraction
	{
		public SgInteractGroup interactGroup;
		public SgItemType itemWheelItem; //interaction on the item
		public SgItemType useItem; //an item is used as cursor
		public SgInteractType type;
		public bool IsRoomInteraction => interactGroup != null;
		public bool IsItemInteraction => !IsRoomInteraction && type != SgInteractType.Illegal;
	}
}
