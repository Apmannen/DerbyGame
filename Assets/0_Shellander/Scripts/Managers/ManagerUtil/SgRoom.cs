using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class SgRoom : MonoBehaviour
{
	public int uiWidth = -1;
}

#if UNITY_EDITOR
[CustomEditor(typeof(SgRoom))]
[CanEditMultipleObjects]
public class SgSceneLoaderEditor : Editor
{
	private SgRoom m_MainObject;
	private SerializedProperty[] m_Properties;

	void OnEnable()
	{
		m_MainObject = (SgRoom)serializedObject.targetObject;
		m_Properties = FindProperties(serializedObject, "uiWidth");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		foreach (SerializedProperty property in m_Properties)
		{
			EditorGUILayout.PropertyField(property);
		}

		string sceneName = m_MainObject.name;
		string path = "Assets/0_Shellander/Scenes/" + sceneName + ".unity";
		if (GUILayout.Button("Load"))
		{
			EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
		}
		else if (GUILayout.Button("LoadReplace"))
		{
			SgRoomName[] roomNames = SgUtil.EnumValues<SgRoomName>();
			foreach(SgRoomName room in roomNames)
			{
				EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByName(room.ToString()), true);
			}
			EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
		}
		else if (GUILayout.Button("Unload"))
		{
			EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByName(sceneName), true);
		}

		serializedObject.ApplyModifiedProperties();
	}

	private static SerializedProperty[] FindProperties(SerializedObject serializedObject, params string[] propertyNames)
	{
		SerializedProperty[] properties = new SerializedProperty[propertyNames.Length];
		for (int i = 0; i < propertyNames.Length; i++)
		{
			properties[i] = serializedObject.FindProperty(propertyNames[i]);
		}
		return properties;
	}
}
#endif
