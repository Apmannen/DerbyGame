using UnityEngine;

public class SgAutoInFrameUi : MonoBehaviour
{
	public bool checkScreen = true;
	public float maxY;

	private Vector3 m_DefaultLocalPos;

	void Start()
	{
		m_DefaultLocalPos = transform.localPosition;
	}

	private void Update()
	{
		transform.localPosition = m_DefaultLocalPos;

		float compareMaxY = checkScreen ? Screen.height : maxY;
		if (transform.position.y > compareMaxY)
		{
			SgUtil.SetPos(transform, compareMaxY, 1);
		}
	}
}
