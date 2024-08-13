using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SgDebugClick : MonoBehaviour
{
	public void OnClick()
	{
		print("I was clicked1");
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		print("I was clicked2");
	}
}
