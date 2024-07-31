using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgInteractable : SgBehavior
{
	private SgInteractGroup m_InteractGroup;
	public SgInteractGroup InteractGroup => SgUtil.LazyParentComponent(this, ref m_InteractGroup);
}
