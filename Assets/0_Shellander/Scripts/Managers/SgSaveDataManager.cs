using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: save to file
public class SgSaveDataManager : SgBehavior
{
	public int saveInterval = 120;

	private readonly HashSet<string> m_SaveKeys = new HashSet<string>();
	public HashSet<string> SaveKeys => m_SaveKeys;
	private float m_TimeElapsed = 0;
	private SgSaveFile m_CurrentSaveFile;
	public SgSaveFile CurrentSaveFile => m_CurrentSaveFile;

	private void Awake()
	{
		m_CurrentSaveFile = new SgSaveFile(0);
	}

	public void HandleSaveAction(SgPropertySaveAction saveAction)
	{
		switch (saveAction)
		{
			case SgPropertySaveAction.Immediate:
				m_TimeElapsed = saveInterval;
				break;
			case SgPropertySaveAction.Delayed:
				ScheduleSaveData();
				break;
		}
	}

	public void ScheduleSaveData()
	{
		m_TimeElapsed = saveInterval - 10;
	}

	public class SgSaveFile
	{
		public readonly Dictionary<SgItemType, SgItemSavable> items = new();
		private readonly Dictionary<string, SgSavableBool> m_NamedBools = new();

		private readonly int m_SaveFileId;
		public int SaveFileId => m_SaveFileId;

		public SgSaveFile(int saveFileId)
		{
			m_SaveFileId = saveFileId;

			foreach (SgItemType itemType in Enum.GetValues(typeof(SgItemType)))
			{
				items[itemType] = new SgItemSavable(saveFileId, itemType);
			}
		}

		public bool GetNamedBoolValue(string name)
		{
			return m_NamedBools.ContainsKey(name) && m_NamedBools[name].Value == true;
		}
		public void SetNamedBoolValue(string name, bool value)
		{
			if(!m_NamedBools.ContainsKey(name))
			{
				m_NamedBools[name] = new SgSavableBool(SaveFileId, "NamedBool_"+name, false, SgPropertySaveAction.Delayed);
			}
			m_NamedBools[name].Set(value);
		}
	}

	public abstract class SgSavable
	{
		private readonly List<ISgSavableProperty> m_Properties = new List<ISgSavableProperty>();

		public void AddProperty(ISgSavableProperty property)
		{
			m_Properties.Add(property);
		}

		public void DebugProperties()
		{
			foreach (var property in m_Properties)
			{
				property.DebugProperty();
			}
		}
	}

	public class SgItemSavable : SgSavable
	{
		public readonly SgSavableBool isCollected;
		public readonly SgSavableLong collectTime;

		public SgItemSavable(long saveFileId, SgItemType itemType)
		{
			isCollected = new SgSavableBool(saveFileId, "Item" + itemType + "Collected", false, SgPropertySaveAction.Delayed);
			collectTime = new SgSavableLong(saveFileId, "Item" + itemType + "Time", 0, SgPropertySaveAction.Delayed);

			base.AddProperty(isCollected);
		}
	}

	public class SgSavableBool : SgSavableProperty<bool>
	{
		public SgSavableBool(long saveFileId, string key, bool defaultValue, SgPropertySaveAction saveAction) :
			base(saveFileId, key, defaultValue, saveAction)
		{ }

		public override void Set(bool value)
		{
			PlayerPrefs.SetInt(FullKey, value ? 1 : 0);
			HandleSave();
		}
		public override bool Get()
		{
			return PlayerPrefs.GetInt(FullKey, DefaultValue ? 1 : 0) == 1;
		}
	}

	public class SgSavableLong : SgSavableProperty<long>
	{
		public SgSavableLong(long saveFileId, string key, long defaultValue, SgPropertySaveAction saveAction) :
			base(saveFileId, key, defaultValue, saveAction)
		{ }

		public override void Set(long value)
		{
			if (value == DefaultValue)
			{
				PlayerPrefs.DeleteKey(FullKey);
			}
			else
			{
				PlayerPrefs.SetString(FullKey, value.ToString());
			}
			HandleSave();
		}
		public override long Get()
		{
			return long.Parse(PlayerPrefs.GetString(FullKey, DefaultValue.ToString()));
		}
	}
	public enum SgPropertySaveAction { None, Immediate, Delayed }

	public interface ISgSavableProperty
	{
		public void DebugProperty();
	}
	public abstract class SgSavableProperty<E> : ISgSavableProperty
	{
		private long m_SaveFileId;
		private readonly string m_Key;
		private readonly E m_DefaultValue;
		private string m_FullKey;
		public string FullKey => m_FullKey;

		public E DefaultValue => m_DefaultValue;
		private E m_InitialValue;
		private SgPropertySaveAction m_SaveAction;

		public SgSavableProperty(long saveFileId, string key, E defaultValue, SgPropertySaveAction saveAction)
		{
			this.m_SaveFileId = saveFileId;
			this.m_Key = key;
			this.m_DefaultValue = defaultValue;
			this.m_SaveAction = saveAction;
			RefreshFullKey();
			m_InitialValue = Get();
		}

		private void RefreshFullKey()
		{
			this.m_FullKey = GetFullKey(m_SaveFileId, m_Key);
			SgManagers._.saveDataManager.SaveKeys.Add(this.m_FullKey);
		}

		public static string GetFullKey(long saveFileId, string key)
		{
			string newFullKey = "Sg" + saveFileId + "_" + key;
			return newFullKey;
		}

		protected void HandleSave()
		{
			//No, sometimes wants to batch it?
			SgManagers._.saveDataManager.HandleSaveAction(m_SaveAction);
		}

		public abstract void Set(E value);
		public abstract E Get();

		public E Value
		{
			get
			{
				return Get();
			}
			set
			{
				Set(value);
			}
		}

		public void DebugProperty()
		{
			Debug.Log("Pref property " + FullKey + "=" + Value);
		}
	}
}
