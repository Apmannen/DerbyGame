using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SgRoomName { Illegal, Home, Stockholm, Solna, ApartmentBuilding, Sewers, Shop, Apartment }

public class SgSceneManager : SgBehavior
{
	public SgRoom[] rooms;

	private bool m_IsTransitioning = false;
	private SgRoomName m_PrevRoom = SgRoomName.Illegal;
	private SgRoom m_CurrentRoom;
	private SgRoomName[] m_RoomNames;
	private SgRoomName[] RoomNames
	{
		get
		{
			if(m_RoomNames == null)
			{
				m_RoomNames = SgUtil.EnumValues<SgRoomName>();
			}
			return m_RoomNames;
		}
	}
	public SgRoomName PrevRoom => m_PrevRoom;

	private void Start()
	{
		bool isAnyLoaded = false;
		foreach (SgRoomName roomName in RoomNames)
		{
			Scene aScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(roomName.ToString());
			if (aScene.isLoaded)
			{
				isAnyLoaded = true;
				break;
			}
		}
		if(!isAnyLoaded)
		{
			SetRoom(SgRoomName.Home);
		}
	}

	private SgRoom GetRoom(SgRoomName roomName)
	{
		return rooms.Single(r => r.name == roomName.ToString());
	}

	public void SetRoom(SgRoomName roomName)
	{
		if(m_IsTransitioning)
		{
			return;
		}
		m_IsTransitioning = true;
		StartCoroutine(RoomTransition(roomName));
	}

	private IEnumerator RoomTransition(SgRoomName roomName)
	{
		foreach(SgRoomName otherRoomName in RoomNames)
		{
			if(otherRoomName == roomName)
			{
				continue;
			}
			Scene aScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(otherRoomName.ToString());
			if(aScene.isLoaded)
			{
				yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(aScene);
				m_PrevRoom = otherRoomName;
			}
		}

		yield return AsyncLoadScene(roomName.ToString());
		m_CurrentRoom = GetRoom(roomName);

		HudManager.RefreshSizes(m_CurrentRoom.uiWidth);

		m_IsTransitioning = false;
	}

	private IEnumerator AsyncLoadScene(string sceneName)
	{
		Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
		if (scene.isLoaded)
		{
			yield break;
		}

		LoadSceneParameters parameters = new LoadSceneParameters();
		parameters.loadSceneMode = LoadSceneMode.Additive;
		long t0 = SgUtil.CurrentTimeMs();
		AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, parameters);

		while (true)
		{
			yield return new WaitForSeconds(0.1f);
			if (op.isDone)
			{
				break;
			}
		}	
	}
}

