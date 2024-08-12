using UnityEngine;
using System.Linq;

public class SgCursorController : SgBehavior
{
	public SgCursorTypeDefinition[] cursors;

	private SgCursorTypeDefinition CurrentCursor => cursors[m_CurrentCursorIndex];
	private SgUiCursor UiCursor => HudManager.cursor;
	//private SgPlayer Player => SgUtil.LazyComponent(this, ref m_Player);

	//private int m_CurrentCursorIndex = 0;
	private SgCursorTypeDefinition m_CurrentCursor;
	private SgItemType m_SelectedItem = SgItemType.Illegal;
	private SgInteractType m_SelectedInteract = SgInteractType.Walk;
	//private SgPlayer m_Player;

	//Don't think we will need the cached prev cursor anymore? Go by selected interact instead.
	//private SgCursorTypeDefinition m_PrevCursor;

	private void Awake()
	{
		for(int i = 0; i < cursors.Length; i++)
		{
			cursors[i].index = i;
		}
		SetCursor(GetCursorByInteractType(SgInteractType.Walk));
	}

	private void Start()
	{
		//m_PrevCursor = GetCursorByInteractType(SgInteractType.Walk);
	}

	private void Update()
	{
		UpdateCursor();
	}

	private void UpdateCursor()
	{
		if (HudManager.IsWheelVisible)
		{
			SetCursor(GetCursorByInteractType(SgInteractType.Generic));
			return;
		}

		SetCursor(GetCursorByInteractType(m_SelectedInteract));
	}

	public void SetSelectedItem(SgItemType itemType)
	{
		SgCursorTypeDefinition itemCursor = GetCursorByInteractType(SgInteractType.Item);
		itemCursor.sprite = itemType != SgItemType.Illegal ? ItemManager.Get(itemType).sprite : null;
		m_SelectedItem = itemType;
		m_SelectedInteract = SgInteractType.Item;
		UpdateCursor();
	}
	public void SetSelectedInteract(SgInteractType interactType)
	{
		m_SelectedInteract = interactType;
		UpdateCursor();
	}

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

	private void SetCursor(SgCursorTypeDefinition cursor)
	{
		Cursor.visible = false;
		UiCursor.image.sprite = CurrentCursor.sprite;
		m_CurrentCursor = cursor;
	}

	private SgCursorTypeDefinition GetCursorByInteractType(SgInteractType type)
	{
		return cursors.SingleOrDefault(c => c.interactType == type);
		//for (int i = 0; i < cursors.Length; i++)
		//{
		//	if (cursors[i].interactType == type)
		//	{
		//		return cursors[i];
		//	}
		//}
		//return null;
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

[System.Serializable]
public class SgCursorTypeDefinition
{
	public SgInteractType interactType;
	public Sprite sprite;
	public int index = -1;
}
