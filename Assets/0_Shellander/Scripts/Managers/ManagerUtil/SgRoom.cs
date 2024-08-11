using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class SgRoom : MonoBehaviour
{

}

#if UNITY_EDITOR
[CustomEditor(typeof(SgRoom))]
[CanEditMultipleObjects]
public class SgSceneLoaderEditor : Editor
{
	private SgRoom m_MainObject;

	void OnEnable()
	{
		m_MainObject = (SgRoom)serializedObject.targetObject;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

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
}
#endif
