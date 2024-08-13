using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class SgCursorController : SgBehavior
{
	public SgCursorTypeDefinition[] cursors;

	private SgUiCursor UiCursor => HudManager.cursor;
	private SgCursorTypeDefinition m_CurrentCursor;
	private SgItemType m_SelectedItem = SgItemType.Illegal;
	private SgInteractType m_SelectedInteract = SgInteractType.Walk;
	private bool m_WaitMode;
	private bool m_IsInitialized = false;

	public void Init()
	{
		for (int i = 0; i < cursors.Length; i++)
		{
			cursors[i].index = i;
		}
		SetCursor(GetCursorByInteractType(SgInteractType.Walk));
		m_IsInitialized = true;
	}
	public SgInteractType SelectedInteractType => m_SelectedInteract;
	private SgCursorTypeDefinition ItemCursor => GetCursorByInteractType(SgInteractType.Item);
	public SgItemType SelectedItem => SelectedInteractType == SgInteractType.Item && ItemCursor.sprite != null 
		? m_SelectedItem : SgItemType.Illegal;
	public void ClearText()
	{
		SetText("");
	}
	public void SetText(string text)
	{
		UiCursor.text.text = text;
	}
	public Vector3 UpdateCursorPos(InputAction pointerAction, Camera mainCamera)
	{
		Vector3 cursorScreenPos = pointerAction.ReadValue<Vector2>();
		Vector3 cursorWorldPos = mainCamera.ScreenToWorldPoint(cursorScreenPos);
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

		if (HudManager.IsWheelVisible || HudManager.IsReplyBarVisible)
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
		m_SelectedInteract = SgInteractType.Item;
		UpdateCurrentCursor();
	}
	public void SetSelectedInteract(SgInteractType interactType)
	{
		m_SelectedInteract = interactType;
		UpdateCurrentCursor();
	}


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
}

[System.Serializable]
public class SgCursorTypeDefinition
{
	public SgInteractType interactType;
	public Sprite sprite;
	public int index = -1;
}
