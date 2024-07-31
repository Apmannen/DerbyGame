using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


public class SgPlayer : SgBehavior
{
	public Camera mainCam;
	public SgAnimation walkAnimation;
	public SpriteRenderer mainRenderer;
	public SgCursorTypeDefinition[] cursors;
	public SgCursorTypeDefinition waitCursor;
	public TMPro.TextMeshPro speechText;

	private enum SgPlayerState { None, Walking, InteractWalking, Interacting }
	private SgCursorTypeDefinition CurrentCursor => cursors[m_CurrentCursorIndex];
	private SgCursorTypeDefinition m_PrevCursor;
	private SgPlayerState m_State = SgPlayerState.None;
	private Vector3 m_WalkTarget;
	private NavMeshAgent m_Agent;
	private Vector3 m_PrevPos;
	private int m_CurrentCursorIndex = 0;
	private SgUiCursor UiCursor => HudManager.cursor;
	private readonly SgInteraction m_CurrentInteraction = new();
	private float m_StateActivatedTime;
	private bool m_SpeechAborted;

	//Input
	private InputActionMap m_CurrentActionMap;
	private PlayerInput m_PlayerInput;
	private InputAction m_ClickAction;
	private InputAction m_PointerAction;
	private InputAction m_ShiftCursorRight;

	private void Start()
	{
		ResetInput();

		m_PrevCursor = GetCursor(SgInteractType.Walk);
		ClearInteraction();
		UiCursor.text.text = "";
		speechText.text = "";

		m_PrevPos = this.transform.position;
		if (mainCam == null)
		{
			mainCam = Camera.main;
		}

		m_WalkTarget = this.transform.position;

		m_Agent = GetComponent<NavMeshAgent>();
		m_Agent.updateRotation = false;
		m_Agent.updateUpAxis = false;
		m_Agent.enabled = true;
		m_Agent.isStopped = false;

		SetCursor(0);
	}

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

		if (IsStateAnyInteract())
		{
			SetCursor(SgInteractType.Wait);

			if(newState == SgPlayerState.Interacting)
			{
				Interact(m_CurrentInteraction);
			}
		}
	}

	private void SetCursor(int index)
	{
		int prevIndex = m_CurrentCursorIndex;
		m_CurrentCursorIndex = index;
		Cursor.visible = false;
		UiCursor.image.sprite = CurrentCursor.sprite;

		if(prevIndex != m_CurrentCursorIndex)
		{
			m_PrevCursor = cursors[prevIndex];
		}

		//Cursor.SetCursor(cursors[index].texture, Vector2.zero, CursorMode.ForceSoftware);
		
	}

	private void SetCursor(SgInteractType type)
	{
		for(int i = 0; i < cursors.Length; i++)
		{
			if(cursors[i].interactType == type)
			{
				SetCursor(i);
				return;
			}
		}
	}
	private SgCursorTypeDefinition GetCursor(SgInteractType type)
	{
		for (int i = 0; i < cursors.Length; i++)
		{
			if (cursors[i].interactType == type)
			{
				return cursors[i];
			}
		}
		return null;
	}

	private void CycleCursor(int direction)
	{
		int newIndex = m_CurrentCursorIndex + direction;
		if(newIndex >= cursors.Length)
		{
			newIndex = 0;
		}
		else if(newIndex < 0)
		{
			newIndex = cursors.Length - 1;
		}
		if(cursors[newIndex].interactType == SgInteractType.Wait)
		{
			newIndex = 0;
		}

		SetCursor(newIndex);
	}

	private void SetInteraction(SgInteractGroup interactGroup, SgItembarItem itembarItem, SgInteractType type)
	{
		m_CurrentInteraction.interactGroup = interactGroup;
		m_CurrentInteraction.itembarItem = itembarItem;
		m_CurrentInteraction.type = type;
	}

	private void ClearInteraction()
	{
		SetInteraction(null, null, SgInteractType.Illegal);
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
		return !IsStateAnyInteract();
	}
	private bool IsCursorAnyInteract()
	{
		switch(CurrentCursor.interactType)
		{
			case SgInteractType.Look:
			case SgInteractType.Pickup:
			case SgInteractType.Talk:
			case SgInteractType.Use:
				return true;
			default:
				return false;
		}
	}

	private void Update()
	{
		//Variables
		Vector3 cursorScreenPos = m_PointerAction.ReadValue<Vector2>();
		Vector3 cursorWorldPos = mainCam.ScreenToWorldPoint(cursorScreenPos);
		UiCursor.transform.position = cursorScreenPos;
		RaycastHit2D hit = Physics2D.Raycast(cursorWorldPos, Vector2.zero);
		SgInteractGroup hoveredInteractGroup = null;
		SgItembarItem hoveredItembarItem = null;

		//UI
		if(IsActionsAllowed() && hit.collider != null && hit.collider.gameObject.layer == SgLayerManager.layerInteractable)
		{
			hoveredInteractGroup = hit.collider.gameObject.GetComponent<SgInteractable>().InteractGroup;
			UiCursor.text.text = hoveredInteractGroup.TranslatedName;
		}
		else
		{
			foreach(SgItembarItem itembarItem in HudManager.itembar.items)
			{
				if(itembarItem.IsHovered)
				{
					hoveredItembarItem = itembarItem;
					break;
				}
			}

			if(hoveredItembarItem != null)
			{
				UiCursor.text.text = hoveredItembarItem.Definition.TranslatedName;
			}
			else
			{
				UiCursor.text.text = "";
				if (IsActionsAllowed())
				{
					ClearInteraction();
				}
			}
		}

		//State handling 1
		if(m_State == SgPlayerState.InteractWalking && HasReachedDestination())
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
				m_WalkTarget = cursorWorldPos;
				m_WalkTarget.z = this.transform.position.z;

				SetState(SgPlayerState.Walking);
				walkAnimation.Play();

				m_Agent.SetDestination(m_WalkTarget);

				if(IsCursorAnyInteract())
				{
					HandleInteractClick(hoveredInteractGroup, hoveredItembarItem);
				}
				
			}
			else if (m_ShiftCursorRight.WasPressedThisFrame())
			{
				CycleCursor(1);
			}
		}
		else if(m_State == SgPlayerState.Interacting && Time.time > (m_StateActivatedTime+0.25f) && 
			(m_ClickAction.WasPressedThisFrame() || m_ShiftCursorRight.WasPressedThisFrame()))
		{
			SkipSpeech();
		}
		

		//State handling 2
		if(IsStateAnyWalking())
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

	private void HandleInteractClick(SgInteractGroup hoveredInteractGroup, SgItembarItem hoveredItembarItem)
	{
		if (hoveredInteractGroup != null)
		{
			SetInteraction(hoveredInteractGroup, null, CurrentCursor.interactType);
			SgInteractTranslation interactConfig = hoveredInteractGroup.GetInteractConfig(CurrentCursor.interactType);
			if (interactConfig.walkToItFirst)
			{
				SetState(SgPlayerState.InteractWalking);
			}
			else
			{
				m_WalkTarget = this.transform.position;
				m_Agent.SetDestination(m_WalkTarget);
				walkAnimation.Stop();
				SetState(SgPlayerState.Interacting);
			}

		}
		else if (hoveredItembarItem != null)
		{
			SetInteraction(null, hoveredItembarItem, CurrentCursor.interactType);
			m_WalkTarget = this.transform.position;
			m_Agent.SetDestination(m_WalkTarget);
			walkAnimation.Stop();
			SetState(SgPlayerState.Interacting);
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
		int[] interactTranslationIds;
		if (interaction.interactGroup != null)
		{
			interactTranslationIds = interaction.interactGroup.GetInteractTranslationIds(interaction.type);
			interaction.interactGroup.OnBeforeInteract(interaction.type);
		}
		else
		{
			interactTranslationIds = interaction.itembarItem.Definition.GetInteractTranslationIds(interaction.type);
		}		

		foreach (int id in interactTranslationIds)
		{
			string translation = TranslationManager.Get(id);
			this.speechText.text = translation;
			m_SpeechAborted = false;
			yield return Wait(3f);
		}		

		ClearInteraction();
		speechText.text = "";
		SetCursor(m_PrevCursor.interactType);
		SetState(SgPlayerState.None);
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
		public SgItembarItem itembarItem;
		public SgInteractType type;
	}
}


[System.Serializable]
public class SgCursorTypeDefinition
{
	public SgInteractType interactType;
	public Sprite sprite;
}