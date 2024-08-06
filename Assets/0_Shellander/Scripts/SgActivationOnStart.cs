using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgActivationOnStart : MonoBehaviour
{
    public bool deactivate;
    public bool onAwake;

	private void Awake()
	{
		if(onAwake)
		{
			Execute();
		}
	}

	private void Execute()
	{
		if(deactivate)
		{
			this.gameObject.SetActive(false);
		}
	}
}
