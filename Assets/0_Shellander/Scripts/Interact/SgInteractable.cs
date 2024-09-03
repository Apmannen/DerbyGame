using UnityEngine;

public class SgInteractable : SgBehavior
{
	public SgObjectDependency[] dependencies;
	public int priority;

	private SgInteractGroup m_InteractGroup;
	public SgInteractGroup InteractGroup => SgUtil.LazyParentComponent(this, ref m_InteractGroup);
	private BoxCollider2D m_Collider;
	private BoxCollider2D Collider => SgUtil.LazyComponent(this, ref m_Collider);

	private void Update()
	{
		foreach(SgObjectDependency dependency in dependencies)
		{
			Collider.enabled = dependency.gameObject.activeSelf;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		SgInteractTranslation collisionInteractConfig = InteractGroup.GetInteractConfig(SgInteractType.Collision, SgItemType.Illegal);
		if (collisionInteractConfig == null)
		{
			return;
		}
		if(!collision.gameObject.TryGetComponent<SgPlayer>(out SgPlayer player))
		{
			return;
		}

		player.OnTriggerCollision(this.InteractGroup);
	}
}

[System.Serializable]
public class SgObjectDependency
{
	public GameObject gameObject;
}
