using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SgBusBenchInteract : SgInteractGroup
{
	public SgMoveAnimation bus;

	public override IEnumerator InteractRoutine(SgPlayer player, SgInteractType interactType)
	{
		if (interactType == SgInteractType.Use)
		{
			player.SetStance(SgPlayerStance.Sitting);
			yield return new WaitForSeconds(5f);
			player.SetStance(SgPlayerStance.Normal);
		}
		yield return base.InteractRoutine(player, interactType);
	}
}
