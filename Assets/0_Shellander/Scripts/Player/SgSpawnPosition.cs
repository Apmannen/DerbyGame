using UnityEngine;

public class SgSpawnPosition : MonoBehaviour
{
    public SgRoomName connectedRoom;
    public SpriteRenderer sprite;

	private void Start()
	{
		sprite.enabled = false;
	}
}
