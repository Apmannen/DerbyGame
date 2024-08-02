using UnityEngine;

public class SgCamera : MonoBehaviour
{
    public Camera cam;

    private SgPlayer m_Player;

    public void AttachPlayer(SgPlayer player)
    {
        m_Player = player;    
	}

	private void Update()
	{
		if(m_Player == null)
		{
			return;
		}

		Vector3 position = cam.transform.position;
		position.x = m_Player.transform.position.x;
		cam.transform.position = position;
	}
}
