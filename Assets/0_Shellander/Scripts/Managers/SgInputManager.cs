using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SgInputManager : MonoBehaviour
{
	public PlayerInput playerInput;

	private InputActionMap m_CurrentActionMap;
	private InputAction m_ClickAction;
	private InputAction m_PointerAction;
	private InputAction m_ShiftCursorRight;

	public InputAction ClickAction => m_ClickAction;
	public InputAction PointerAction => m_PointerAction;
	public InputAction ShiftCursorRightAction => m_ShiftCursorRight;


	private void Awake()
	{
		ResetInput();
	}

	private void ResetInput()
	{
		m_CurrentActionMap = null;
		foreach (UnityEngine.InputSystem.InputActionMap map in playerInput.actions.actionMaps)
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
}
