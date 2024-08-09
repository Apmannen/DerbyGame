using ShellanderGames.WeaponWheel;
using UnityEngine;

public class SgHudManager : MonoBehaviour
{
	public SgUiCursor cursor;
	public SgItembar itembar;
	public SgWeaponWheel weaponWheel;

	private void Awake()
	{
		weaponWheel.AddEventCallback(OnWheelEvent);
	}

	private void OnWheelEvent(SgWeaponWheelEvent wheelEvent)
	{
		
	}

	public bool IsWheelVisible => weaponWheel.IsVisible;
}
