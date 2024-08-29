using UnityEngine;

public class SgAutoInFrameUi : MonoBehaviour
{
	private Vector3 m_DefaultLocalPos;

	//private RectTransform m_RectTransform;
	//private RectTransform RectTransform => SgUtil.LazyComponent(this, ref m_RectTransform);

	void Start()
	{
		m_DefaultLocalPos = transform.localPosition;
	}

	private void Update()
	{
		transform.localPosition = m_DefaultLocalPos;

		if(transform.position.y > Screen.height)
		{
			SgUtil.SetPos(transform, Screen.height, 1);
		}
	}
}
