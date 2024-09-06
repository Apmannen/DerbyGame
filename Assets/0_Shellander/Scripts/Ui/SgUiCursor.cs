using UnityEngine;
using UnityEngine.UI;

public class SgUiCursor : SgBehavior
{
	public Image image;
	public TMPro.TextMeshProUGUI text;

    private Vector2 m_DefaultTextLocalPos;

    private void Start()
    {
        m_DefaultTextLocalPos = text.transform.localPosition;    
    }

    private void Update()
    {
        text.transform.localPosition = m_DefaultTextLocalPos;
        HudManager.KeepInFrame(text.transform, text.transform.position, HudManager.cursorTextLimitTopLeft, HudManager.cursorTextLimitTopRight);
    }
}
