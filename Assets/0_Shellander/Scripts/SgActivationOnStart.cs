using UnityEngine;

public class SgActivationOnStart : MonoBehaviour
{
    public bool deactivate;
    public bool onAwake;
	public bool onStart;

	private void Awake()
	{
		if(onAwake)
		{
			Execute();
		}
	}
	private void Start()
	{
		if (onStart)
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
