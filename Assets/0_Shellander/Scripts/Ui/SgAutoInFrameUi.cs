using UnityEngine;

public class SgAutoInFrameUi : MonoBehaviour
{
	private Vector3 m_DefaultLocalPos;

	private RectTransform m_RectTransform;
	private RectTransform RectTransform => SgUtil.LazyComponent(this, ref m_RectTransform);

	void Start()
	{
		m_DefaultLocalPos = RectTransform.localPosition;
	}

	private void Update()
	{
		RectTransform.localPosition = m_DefaultLocalPos;

		if(RectTransform.position.y > Screen.height)
		{
			SgUtil.SetPos(RectTransform, Screen.height, 1);
		}
	}
}
