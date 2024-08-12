using UnityEngine;

public class SgCursorController : SgBehavior
{
	public SgCursorTypeDefinition[] cursors;

	private SgCursorTypeDefinition CurrentCursor => cursors[m_CurrentCursorIndex];
	private SgUiCursor UiCursor => HudManager.cursor;
	//private SgPlayer Player => SgUtil.LazyComponent(this, ref m_Player);

	private int m_CurrentCursorIndex = 0;
	private SgItemType m_SelectedItem = SgItemType.Illegal;
	private SgCursorTypeDefinition m_PrevCursor;
	//private SgPlayer m_Player;

	private void Start()
	{
		m_PrevCursor = GetCursorByInteractType(SgInteractType.Walk);
	}

	private void Update()
	{
		if(HudManager.IsWheelVisible)
		{
			SetCursorByInteractType(SgInteractType.Generic);
			return;
		}

		if(m_SelectedItem != SgItemType.Illegal)
		{
			SetCursorByItemType(m_SelectedItem);
			return;
		}
	}

	public void SetSelectedItem(SgItemType itemType)
	{
		m_SelectedItem = itemType;
	}

	private void SetCursorByItemType(SgItemType itemType)
	{
		SgCursorTypeDefinition cursor = GetCursorByInteractType(SgInteractType.Item);
		cursor.sprite = ItemManager.Get(itemType).sprite;
		SetCursorByInteractType(SgInteractType.Item);
	}
	private void SetCursorByInteractType(SgInteractType type)
	{
		for (int i = 0; i < cursors.Length; i++)
		{
			if (cursors[i].interactType == type)
			{
				SetCursorByIndex(i);
				SetSelectedItem(SgItemType.Illegal);
				return;
			}
		}
	}
	private void SetCursorByIndex(int index)
	{
		int prevIndex = m_CurrentCursorIndex;
		m_CurrentCursorIndex = index;
		Cursor.visible = false;
		UiCursor.image.sprite = CurrentCursor.sprite;

		if (prevIndex != m_CurrentCursorIndex)
		{
			m_PrevCursor = cursors[prevIndex];
		}
	}
	private SgCursorTypeDefinition GetCursorByInteractType(SgInteractType type)
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


}
