using System.Collections;
using UnityEngine;

public class SgBusBenchInteract : SgInteractGroup
{
	public SgMoveAnimation bus;
	public int busCardMissingTranslationId;
	public SgRoomName goToRoom = SgRoomName.Illegal;

	public override IEnumerator InteractRoutine(SgPlayer player, SgInteractType interactType)
	{
		if (interactType == SgInteractType.Use)
		{
			player.SetStance(SgPlayerStance.Sitting);
			bus.gameObject.SetActive(true);
			Coroutine busRoutine = bus.StartAnimation();
			yield return new WaitForSeconds(5);

			if(ItemManager.IsCollected(SgItemType.BussCard))
			{
				player.SetStance(SgPlayerStance.Hidden);
				yield return busRoutine;
				SceneManager.SetRoom(goToRoom);
			}
			else
			{
				player.SetStance(SgPlayerStance.Normal);
				yield return busRoutine;
				yield return player.Talk(new int[] { busCardMissingTranslationId });
			}			
		}
		else
		{
			yield return base.InteractRoutine(player, interactType);
		}
	}
}
