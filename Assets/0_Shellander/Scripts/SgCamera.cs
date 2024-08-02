using UnityEngine;

public class SgCamera : MonoBehaviour
{
    public Camera cam;

    private SgPlayer m_Player;

    public void AttachPlayer(SgPlayer player)
    {
        m_Player = player;    
	}
}
