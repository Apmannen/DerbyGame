using System.Collections;
using UnityEngine;

public class SgBusBenchInteract : SgInteractGroup
{
	public SgMoveAnimation bus;
	public int busCardMissingTranslationId;
	public SgRoomName goToRoom = SgRoomName.Illegal;

	protected override void Start()
	{
		base.Start();
		bus.gameObject.SetActive(false);
	}

	public override IEnumerator InteractRoutine(SgPlayer player, SgInteractType interactType)
	{
		if (interactType == SgInteractType.Use)
		{
			player.SetStance(SgPlayerStance.Sitting);
			bus.gameObject.SetActive(true);
			yield return bus.AnimateStep(0);

			if (ItemManager.IsCollected(SgItemType.BussCard))
			{
				player.SetStance(SgPlayerStance.Hidden);
				yield return bus.AnimateStep(1);
				yield return bus.AnimateStep(2);
				SceneManager.SetRoom(goToRoom);
			}
			else
			{
				player.SetStance(SgPlayerStance.Normal);
				yield return bus.AnimateStep(1);
				yield return bus.AnimateStep(2);
				yield return player.character.Talk(busCardMissingTranslationId);
			}			
		}
		else
		{
			yield return base.InteractRoutine(player, interactType);
		}
	}
}
