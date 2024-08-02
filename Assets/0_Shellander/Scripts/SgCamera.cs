using UnityEngine;

public class SgCamera : MonoBehaviour
{
    public Camera cam;
	public bool isStatic = true;
	public Transform boundsLeft;
	public Transform boundsRight;	

    private SgPlayer m_Player;

	public void AttachPlayer(SgPlayer player)
    {
        m_Player = player;    
	}

	private void Update()
	{
		if(isStatic || m_Player == null)
		{
			return;
		}

		Vector3 position = cam.transform.position;
		position.x = m_Player.transform.position.x;
		if(position.x < boundsRight.position.x && position.x > boundsLeft.position.x)
		{
			cam.transform.position = position;
		}		
	}
}
