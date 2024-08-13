using System.Collections;
using UnityEngine;

public class SgBusBenchInteract : SgInteractGroup
{
	public SgMoveAnimation bus;
	public int busCardMissingTranslationId;
	public SgRoomName goToRoom = SgRoomName.Illegal;

	private bool CutsceneAborted => m_AbortClicks >= 2;
	private int m_AbortClicks = 0;

	public override IEnumerator InteractRoutine(SgPlayer player, SgInteractType interactType)
	{
		if (interactType == SgInteractType.Use)
		{
			m_AbortClicks = 0;

			player.SetStance(SgPlayerStance.Sitting);
			bus.gameObject.SetActive(true);
			yield return bus.AnimateStep(0);
			//bus.StartAnimation();
			//yield return Wait(5);
			//CheckHandleAborted();
			//float busLeaveTime = 3;

			if (ItemManager.IsCollected(SgItemType.BussCard))
			{
				player.SetStance(SgPlayerStance.Hidden);
				yield return bus.AnimateStep(1);
				yield return bus.AnimateStep(2);
				//yield return Wait(busLeaveTime);
				SceneManager.SetRoom(goToRoom);
			}
			else
			{
				player.SetStance(SgPlayerStance.Normal);
				yield return bus.AnimateStep(1);
				yield return bus.AnimateStep(2);
				//yield return Wait(busLeaveTime);
				//CheckHandleAborted();
				yield return player.character.Talk(busCardMissingTranslationId);
			}			
		}
		else
		{
			yield return base.InteractRoutine(player, interactType);
		}
	}

	

	//private void CheckHandleAborted()
	//{
	//	if(!CutsceneAborted)
	//	{
	//		return;
	//	}
	//	bus.gameObject.SetActive(false);
	//}

	//private IEnumerator Wait(float maxDuration)
	//{
	//	float time = 0;
	//	while (time < maxDuration && !CutsceneAborted)
	//	{
	//		time += Time.deltaTime;
	//		yield return null;
	//	}
	//}
}
