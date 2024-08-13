using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SgDocumentationText : MonoBehaviour
{
	[TextArea]
	public string documentationString;
}

#if UNITY_EDITOR
[CustomEditor(typeof(SgDocumentationText))]
public class SgDocumentationTextEditor : Editor
{
	private SerializedProperty documentationString;
	private bool isEditEnabled = false;

	protected void OnEnable()
	{
		documentationString = serializedObject.FindProperty("documentationString");
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.HelpBox(documentationString.stringValue, MessageType.Info);

		if (GUILayout.Button("(Edit documentation)"))
		{
			isEditEnabled = true;
		}

		if (isEditEnabled)
		{
			EditorGUILayout.PropertyField(documentationString);

			if (GUILayout.Button("Close"))
			{
				isEditEnabled = false;
			}
		}
		serializedObject.ApplyModifiedProperties();

	}
}
#endif
