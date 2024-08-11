public class SgSavedActivation : SgBehavior
{
    public string namedBool;
	public bool activateWhenNamedBoolIsTrue;

	private void Start()
	{
		EventManager.Register(SgEventName.NamedSaveBoolUpdated, RefreshActivation);
		RefreshActivation();
	}
	private void OnDestroy()
	{
		EventManager.Unregister(SgEventName.NamedSaveBoolUpdated, RefreshActivation);
	}

	private void RefreshActivation()
	{
		bool value = SaveDataManager.CurrentSaveFile.GetNamedBoolValue(namedBool);

		this.gameObject.SetActive(activateWhenNamedBoolIsTrue && value);
	}
}
