#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SgNavGizmos : MonoBehaviour
{
	public void SetVisible(bool active)
	{
		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
		foreach(SpriteRenderer aRenderer in renderers)
		{
			aRenderer.enabled = active;
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(SgNavGizmos))]
public class SgWeaponWheelInputInspector : Editor
{
	private SgNavGizmos m_MainObject;


	protected void OnEnable()
	{
		m_MainObject = (SgNavGizmos)serializedObject.targetObject;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		bool activate = GUILayout.Button("Activate");
		bool deactivate = GUILayout.Button("Deactivate");
		if (activate || deactivate)
		{
			m_MainObject.SetVisible(activate);
			EditorUtility.SetDirty(m_MainObject);
		}
	}
}
#endif
