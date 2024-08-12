using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class SgCursorController : SgBehavior
{
	public SgCursorTypeDefinition[] cursors;

	private SgUiCursor UiCursor => HudManager.cursor;
	private SgCursorTypeDefinition m_CurrentCursor;
	//private SgItemType m_SelectedItem = SgItemType.Illegal;
	private SgInteractType m_SelectedInteract = SgInteractType.Walk;
	private bool m_WaitMode;
	private bool m_IsInitialized = false;
	private SgCamera m_MainCam;
	private InputAction m_PointerAction;

	public void Init(InputAction pointerAction)
	{
		for (int i = 0; i < cursors.Length; i++)
		{
			cursors[i].index = i;
		}
		SetCursor(GetCursorByInteractType(SgInteractType.Walk));
		m_PointerAction = pointerAction;
		m_IsInitialized = true;
	}

	//private void Update()
	//{
	//	UpdateCursor();
	//}

	//Publics
	public Vector3 UpdateCursorPos()
	{
		Vector3 cursorScreenPos = m_PointerAction.ReadValue<Vector2>();
		Vector3 cursorWorldPos = m_MainCam.cam.ScreenToWorldPoint(cursorScreenPos);
		UiCursor.transform.position = cursorScreenPos;
		return cursorWorldPos;
	}

	public void UpdateCurrentCursor()
	{
		if (!m_IsInitialized)
		{
			return;
		}

		if (m_WaitMode)
		{
			SetCursor(GetCursorByInteractType(SgInteractType.Wait));
			return;
		}

		if (HudManager.IsWheelVisible)
		{
			SetCursor(GetCursorByInteractType(SgInteractType.Generic));
			return;
		}

		SetCursor(GetCursorByInteractType(m_SelectedInteract));
	}

	public bool IsAnyInteract()
	{
		switch (m_CurrentCursor.interactType)
		{
			case SgInteractType.Look:
			case SgInteractType.Pickup:
			case SgInteractType.Talk:
			case SgInteractType.Use:
			case SgInteractType.Item:
				return true;
			default:
				return false;
		}
	}
	public void CycleCursor(int direction)
	{
		int newIndex = m_CurrentCursor.index + direction;
		if (newIndex >= cursors.Length)
		{
			newIndex = 0;
		}
		else if (newIndex < 0)
		{
			newIndex = cursors.Length - 1;
		}
		if (cursors[newIndex].interactType == SgInteractType.Wait || cursors[newIndex].sprite == null)
		{
			newIndex = 0;
		}

		m_SelectedInteract = cursors[newIndex].interactType;
		UpdateCurrentCursor();
	}
	public void SetWaitMode(bool waitMode)
	{
		m_WaitMode = waitMode;
	}
	public void SetSelectedItem(SgItemType itemType)
	{
		SgCursorTypeDefinition itemCursor = GetCursorByInteractType(SgInteractType.Item);
		itemCursor.sprite = itemType != SgItemType.Illegal ? ItemManager.Get(itemType).sprite : null;
		//m_SelectedItem = itemType;
		m_SelectedInteract = SgInteractType.Item;
		UpdateCurrentCursor();
	}
	public void SetSelectedInteract(SgInteractType interactType)
	{
		m_SelectedInteract = interactType;
		UpdateCurrentCursor();
	}


	//Privates
	private void SetCursor(SgCursorTypeDefinition cursor)
	{
		Cursor.visible = false;
		UiCursor.image.sprite = cursor.sprite;
		m_CurrentCursor = cursor;
	}

	private SgCursorTypeDefinition GetCursorByInteractType(SgInteractType type)
	{
		return cursors.SingleOrDefault(c => c.interactType == type);
	}

	//private void CycleCursor(int direction)
	//{
	//	int newIndex = m_CurrentCursorIndex + direction;
	//	if (newIndex >= cursors.Length)
	//	{
	//		newIndex = 0;
	//	}
	//	else if (newIndex < 0)
	//	{
	//		newIndex = cursors.Length - 1;
	//	}
	//	if (cursors[newIndex].interactType == SgInteractType.Wait || cursors[newIndex].sprite == null)
	//	{
	//		newIndex = 0;
	//	}

	//	SetCursor(newIndex);
	//}


	/*
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
		
	}

	private void SetItemCursor(SgItemType itemType)
	{
		SgCursorTypeDefinition cursor = GetCursor(SgInteractType.Item);
		cursor.sprite = ItemManager.Get(itemType).sprite;
		SetCursor(SgInteractType.Item);
		m_CursorItem = itemType;
	}
	private void SetCursor(SgInteractType type)
	{
		for(int i = 0; i < cursors.Length; i++)
		{
			if(cursors[i].interactType == type)
			{
				SetCursor(i);
				m_CursorItem = SgItemType.Illegal;
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
	*/

	//private void SetCursorByItemType(SgItemType itemType)
	//{
	//	SgCursorTypeDefinition cursor = GetCursorByInteractType(SgInteractType.Item);
	//	cursor.sprite = ItemManager.Get(itemType).sprite;
	//	SetCursorByInteractType(SgInteractType.Item);
	//}
	//private void SetCursorByInteractType(SgInteractType type)
	//{
	//	for (int i = 0; i < cursors.Length; i++)
	//	{
	//		if (cursors[i].interactType == type)
	//		{
	//			SetCursorByIndex(i);
	//			SetSelectedItem(SgItemType.Illegal);
	//			return;
	//		}
	//	}
	//}

	//private void SetCursorByIndex(int index)
	//{
	//	int prevIndex = m_CurrentCursorIndex;
	//	m_CurrentCursorIndex = index;
	//	Cursor.visible = false;
	//	UiCursor.image.sprite = CurrentCursor.sprite;

	//	if (prevIndex != m_CurrentCursorIndex)
	//	{
	//		//m_PrevCursor = cursors[prevIndex];
	//	}
	//}
}

[System.Serializable]
public class SgCursorTypeDefinition
{
	public SgInteractType interactType;
	public Sprite sprite;
	public int index = -1;
}
