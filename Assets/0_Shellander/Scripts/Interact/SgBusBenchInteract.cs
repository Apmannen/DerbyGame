using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgBusBenchInteract : SgInteractGroup
{
	public SgMoveAnimation bus;
	public int busCardMissingTranslationId;

	public override IEnumerator InteractRoutine(SgPlayer player, SgInteractType interactType)
	{
		if (interactType == SgInteractType.Use)
		{
			player.SetStance(SgPlayerStance.Sitting);
			bus.gameObject.SetActive(true);
			Coroutine busRoutine = bus.StartAnimation();
			yield return new WaitForSeconds(5);
			player.SetStance(SgPlayerStance.Normal);
			yield return busRoutine;

			yield return player.Talk(new int[] { busCardMissingTranslationId });
		}
		yield return base.InteractRoutine(player, interactType);
	}
}
