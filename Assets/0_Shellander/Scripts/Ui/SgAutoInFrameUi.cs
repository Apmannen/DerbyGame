using UnityEngine;

public class SgAutoInFrameUi : MonoBehaviour
{
	public bool checkScreen;
	public bool useMaxY;
	public bool useMinX;

	public float maxY;
	public float minX;

	private Vector3 m_DefaultLocalPos;

	void Start()
	{
		m_DefaultLocalPos = transform.localPosition;
	}

	private void Update()
	{
		transform.localPosition = m_DefaultLocalPos;

		AdjustMinX();
		AdjustY();
	}

	private void AdjustMinX()
	{
		float compareMinX = float.MinValue;
		if(useMinX)
		{
			compareMinX = minX;
		}

		if (transform.position.x < compareMinX)
		{
			SgUtil.SetPos(transform, compareMinX, 0);
		}
	}

	private void AdjustY()
	{
		float compareMaxY = float.MaxValue; 
		if(useMaxY)
		{
			compareMaxY = maxY;
		}
		else if(checkScreen)
		{
			compareMaxY = Screen.height;
		}

		if (transform.position.y > compareMaxY)
		{
			SgUtil.SetPos(transform, compareMaxY, 1);
		}
	}
}
