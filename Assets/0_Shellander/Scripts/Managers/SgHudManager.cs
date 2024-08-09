using ShellanderGames.WeaponWheel;
using UnityEngine;
using System.Linq;

public class SgHudManager : MonoBehaviour
{
	public SgUiCursor cursor;
	public SgItembar itembar;
	public SgWeaponWheel weaponWheel;
	public CanvasGroup wheelBgGroup;
	public float wheelBgAlphaSmoothTime = 0.1f;
	public SgWheelSliceMapping[] sliceMappings;

	private float m_BgAlphaVel = 0;

	public void AddWheelListener(System.Action<SgWeaponWheelEvent> action)
	{
		weaponWheel.AddEventCallback(action);
	}
	public void RemoveWheelListener(System.Action<SgWeaponWheelEvent> action)
	{
		weaponWheel.RemoveEventCallback(action);
	}

	public SgWheelSliceMapping GetWheelSliceMapping(string sliceName)
	{
		return sliceMappings.Single(m => m.sliceName == sliceName);
	}

	private void Update()
	{
		wheelBgGroup.alpha = Mathf.SmoothDamp(wheelBgGroup.alpha, IsWheelVisible ? 1 : 0, ref m_BgAlphaVel, wheelBgAlphaSmoothTime);
	}

	public bool IsWheelVisible => weaponWheel.IsVisible;
}

[System.Serializable]
public class SgWheelSliceMapping
{
	public string sliceName;
	public SgInteractType interactType;
	public int translationId;
}
