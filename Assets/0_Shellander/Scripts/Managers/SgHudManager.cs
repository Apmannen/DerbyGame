using ShellanderGames.WeaponWheel;
using UnityEngine;

public class SgHudManager : MonoBehaviour
{
	public SgUiCursor cursor;
	public SgItembar itembar;
	public SgWeaponWheel weaponWheel;
	public CanvasGroup wheelBgGroup;
	public float wheelBgAlphaSmoothTime = 0.1f;

	private float m_BgAlphaVel = 0;

	private void Awake()
	{
		weaponWheel.AddEventCallback(OnWheelEvent);
	}

	private void Update()
	{
		wheelBgGroup.alpha = Mathf.SmoothDamp(wheelBgGroup.alpha, IsWheelVisible ? 1 : 0, ref m_BgAlphaVel, wheelBgAlphaSmoothTime);
	}

	private void OnWheelEvent(SgWeaponWheelEvent wheelEvent)
	{
		
	}

	public bool IsWheelVisible => weaponWheel.IsVisible;
}
