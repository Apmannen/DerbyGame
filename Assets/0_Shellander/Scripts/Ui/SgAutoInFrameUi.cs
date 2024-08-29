using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SgAutoInFrameUi : MonoBehaviour
{
	private Vector3 m_DefaultLocalPos;

	private RectTransform m_RectTransform;
	private RectTransform RectTransform => SgUtil.LazyComponent(this, ref m_RectTransform);
	//private Canvas m_Canvas;
	//private Canvas Canvas => SgUtil.LazyParentComponent(this, ref m_Canvas);
	//private CanvasScaler m_CanvasScaler;
	//private CanvasScaler CanvasScaler => SgUtil.LazyParentComponent(this, ref m_CanvasScaler);

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

		//Debug.Log("**** POS:"+RectTransform.position.y+", screen="+Screen.currentResolution.height+", s2="+Screen.height+"; "+Canvas.GetComponent<RectTransform>().sizeDelta.y+"; //"+CanvasScaler.referenceResolution.y+"; ");
	}
}
