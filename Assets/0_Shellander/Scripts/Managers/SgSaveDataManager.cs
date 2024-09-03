using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;

public class SgSaveDataManager : SgBehavior
{
	public int saveInterval = 120;

	private readonly HashSet<string> m_SaveKeys = new HashSet<string>();
	public HashSet<string> SaveKeys => m_SaveKeys;
	private SgSaveFile m_CurrentSaveFile;
	public SgSaveFile CurrentSaveFile => m_CurrentSaveFile;
	private SgPlayerPrefs m_Prefs = new SgPlayerPrefs();

	private void Awake()
	{
		m_CurrentSaveFile = new SgSaveFile(0);
	}

	public void Save()
	{
		m_Prefs.Save(0);
	}

	public class SgSaveFile
	{
		public readonly Dictionary<SgItemType, SgItemSavable> items = new();
		private readonly Dictionary<string, SgSavableBool> m_NamedBools = new();
		public SgSavableEnum<SgSkinType> currentSkin;

		private readonly int m_SaveFileId;
		public int SaveFileId => m_SaveFileId;

		public SgSaveFile(int saveFileId)
		{
			m_SaveFileId = saveFileId;

			currentSkin = new SgSavableEnum<SgSkinType>(saveFileId, "CurrentSkin", SgSkinType.Normal);

			foreach (SgItemType itemType in Enum.GetValues(typeof(SgItemType)))
			{
				items[itemType] = new SgItemSavable(saveFileId, itemType);
			}
		}

		private SgSavableBool GetNamedBool(string name)
		{
			if (!m_NamedBools.ContainsKey(name))
			{
				m_NamedBools[name] = new SgSavableBool(SaveFileId, "NamedBool_" + name, false);
			}
			return m_NamedBools[name];
		}
		public bool GetNamedBoolValue(string name)
		{
			return GetNamedBool(name).Value == true;
		}
		public void SetNamedBoolValue(string name, bool value)
		{
			GetNamedBool(name).Set(value);
			SgManagers._.eventManager.Execute(SgEventName.NamedSaveBoolUpdated);
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
		public readonly SgSavableBool isDiscovered;
		public readonly SgSavableBool hasEverBeenCollected;
		public readonly SgSavableLong collectTime;		

		public SgItemSavable(long saveFileId, SgItemType itemType)
		{
			isCollected = new SgSavableBool(saveFileId, "Item" + itemType + "Collected", false);
			isDiscovered = new SgSavableBool(saveFileId, "Item" + itemType + "Discovered", false);
			hasEverBeenCollected = new SgSavableBool(saveFileId, "Item" + itemType + "EverCollected", false);
			collectTime = new SgSavableLong(saveFileId, "Item" + itemType + "Time", 0);

			base.AddProperty(isCollected);
		}
	}

	public class SgSavableBool : SgSavableProperty<bool>
	{
		public SgSavableBool(long saveFileId, string key, bool defaultValue) :
			base(saveFileId, key, defaultValue)
		{ }

		public override void Set(bool value)
		{
			PlayerPrefs.SetInt(FullKey, value ? 1 : 0);
		}
		public override bool Get()
		{
			return PlayerPrefs.GetInt(FullKey, DefaultValue ? 1 : 0) == 1;
		}
	}

	public class SgSavableEnum<E> : SgSavableProperty<E> where E : struct, Enum
	{
		public SgSavableEnum(long saveFileId, string key, E defaultValue) :
			base(saveFileId, key, defaultValue)
		{ }

		public override void Set(E value)
		{
			PlayerPrefs.SetInt(FullKey, Convert.ToInt32(value));
		}
		public override E Get()
		{
			if(!PlayerPrefs.HasKey(FullKey))
			{
				return this.DefaultValue;
			}

			int intValue = PlayerPrefs.GetInt(FullKey);
			var enumValues = Enum.GetValues(typeof(E));

			return (E) enumValues.GetValue(intValue);
		}
	}

	public class SgSavableLong : SgSavableProperty<long>
	{
		public SgSavableLong(long saveFileId, string key, long defaultValue) :
			base(saveFileId, key, defaultValue)
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
		}
		public override long Get()
		{
			return long.Parse(PlayerPrefs.GetString(FullKey, DefaultValue.ToString()));
		}
	}

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

		public SgSavableProperty(long saveFileId, string key, E defaultValue)
		{
			this.m_SaveFileId = saveFileId;
			this.m_Key = key;
			this.m_DefaultValue = defaultValue;
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

	private class SgPlayerPrefs
	{
		private readonly Dictionary<string, object> m_Map = new Dictionary<string, object>();
		private readonly string m_LocalFilename = "settings_local.sav"; //graphics settings etc.
																		//private readonly string m_GlobalFilename = "savefile_global.sav"; //save data, share with cloud
		private readonly string m_Path;

		private SgGameManager GameManager => SgManagers._.gameManager;

		public SgPlayerPrefs()
		{
			string innerDirectory;

			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				innerDirectory = "editor";
			}
			else
			{
				innerDirectory = "generic";
			}

			m_Path = Application.persistentDataPath + "/" + innerDirectory;

			if (!System.IO.Directory.Exists(m_Path))
			{
				System.IO.Directory.CreateDirectory(m_Path);
			}

			LoadData(GetFullPath(m_LocalFilename));

			string[] fileNames = System.IO.Directory.GetFiles(m_Path);
			string pattern = @"^savefile[0-9]+_global.sav$";
			foreach (string fileNameFull in fileNames)
			{
				string fileName = Path.GetFileName(fileNameFull);
				bool match = Regex.IsMatch(fileName, pattern);
				if (match)
				{
					LoadData(fileNameFull);
				}
			}
		}

		private string GetFullSaveFilePath(int saveFileId)
		{
			string fileName = "savefile" + saveFileId + "_global.sav";
			return GetFullPath(fileName);
		}

		private string GetFullPath(string filename)
		{
			return m_Path + "/" + filename;
		}


		private void LoadData(string fullPath)
		{
			if (!System.IO.File.Exists(fullPath))
			{
				Debug.LogError("Couldn't find file: " + fullPath);
				return;
			}

			string[] lines = System.IO.File.ReadAllLines(fullPath);

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				try
				{
					ParseLine(line, m_Map);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
					continue;
				}
			}
		}

		private void ParseLine(string line, Dictionary<string, object> map)
		{
			string[] split = line.Split(';');
			string key = split[0];
			string typeString = split[1];
			string valueString = "";
			for (int i2 = 2; i2 < split.Length; i2++)
			{
				if (i2 > 2)
				{
					valueString += ";";
				}
				valueString += split[i2];
			}

			switch (typeString)
			{
				case "long":
					SetLong(key, long.Parse(valueString, CultureInfo.InvariantCulture), map);
					break;
				case "float":
					SetFloat(key, float.Parse(valueString, CultureInfo.InvariantCulture), map);
					break;
				case "int":
					SetInt(key, int.Parse(valueString, CultureInfo.InvariantCulture), map);
					break;
				case "string":
					SetString(key, valueString, map);
					break;
			}
		}

		public HashSet<string> GetAllKeysWithPattern(string pattern)
		{
			HashSet<string> filteredSet = new HashSet<string>();
			foreach (KeyValuePair<string, object> keyValue in m_Map)
			{
				if (Regex.IsMatch(keyValue.Key, pattern))
				{
					filteredSet.Add(keyValue.Key);
				}
			}
			return filteredSet;
		}

		public void DeleteSaveFile(int id)
		{
			DeleteAllWithPrefix("Sg" + id + "_");
			string fullPath = GetFullSaveFilePath(id);
			if (System.IO.File.Exists(fullPath))
			{
				System.IO.File.Delete(fullPath);
			}
		}

		public void DeleteAllWithPrefix(string prefix)
		{
			string[] keys = m_Map.Keys.ToArray();
			foreach (string key in keys)
			{
				if (key.StartsWith(prefix))
				{
					DeleteKey(key);
				}
			}
		}

		private void DeleteKey(string fullKey)
		{
			m_Map.Remove(fullKey);
		}

		public bool HasKey(string fullKey)
		{
			if (fullKey == null)
			{
				return false;
			}
			return m_Map.ContainsKey(fullKey);
		}
		public void SetInt(string fullKey, int value)
		{
			m_Map[fullKey] = value;
		}
		public void SetFloat(string fullKey, float value)
		{
			m_Map[fullKey] = value;
		}
		public void SetLong(string fullKey, long value)
		{
			m_Map[fullKey] = value;
		}
		public void SetString(string fullKey, string value)
		{
			m_Map[fullKey] = value;
		}
		public void SetInt(string fullKey, int value, Dictionary<string, object> map)
		{
			map[fullKey] = value;
		}
		public void SetFloat(string fullKey, float value, Dictionary<string, object> map)
		{
			map[fullKey] = value;
		}
		public void SetLong(string fullKey, long value, Dictionary<string, object> map)
		{
			map[fullKey] = value;
		}
		public void SetString(string fullKey, string value, Dictionary<string, object> map)
		{
			map[fullKey] = value;
		}

		public int GetInt(string fullKey, int defaultValue)
		{
			return Get<int>(fullKey, defaultValue);
		}
		public float GetFloat(string fullKey, float defaultValue)
		{
			return Get<float>(fullKey, defaultValue);
		}
		public long GetLong(string fullKey, long defaultValue)
		{
			return Get<long>(fullKey, defaultValue);
		}
		public string GetString(string fullKey, string defaultValue)
		{
			return Get<string>(fullKey, defaultValue);
		}
		private T Get<T>(string fullKey, object defaultValue)
		{
			try
			{
				if (!m_Map.ContainsKey(fullKey))
				{
					return (T)defaultValue;
				}
				return (T)m_Map[fullKey];
			}
			catch
			{
				//exceptions creates garbage, prevent it
				return (T)defaultValue;
			}
		}

		public void Save(int saveFileId)
		{
			List<string> keysList = m_Map.Keys.ToList();
			if (GameManager.isBeta)
			{
				keysList.Sort((a, b) =>
				{
					int num1 = GetSaveFileIdNumber(a);
					int num2 = GetSaveFileIdNumber(b);

					int cmp = num1.CompareTo(num2);

					if (cmp != 0)
					{
						return cmp;
					}

					return a.CompareTo(b);
				});
			}

			string saveStartPattern = "Sg" + saveFileId + "_";

			StringBuilder sbLocal = new StringBuilder();
			StringBuilder sbGlobal = new StringBuilder(m_Map.Count * 20);
			foreach (string key in keysList)
			{
				object value = m_Map[key];

				bool isSettings = key.StartsWith("Sg-1_");
				bool isThisSave = !isSettings && key.StartsWith(saveStartPattern);

				if (!isSettings && !isThisSave)
				{
					continue;
				}

				string typeString;
				string valueString;
				if (value is int intValue)
				{
					typeString = "int";
					valueString = intValue.ToString();
				}
				else if (value is long longValue)
				{
					typeString = "long";
					valueString = longValue.ToString();
				}
				else if (value is float floatValue)
				{
					typeString = "float";
					valueString = floatValue.ToString("0.000", CultureInfo.InvariantCulture);
				}
				else if (value is string stringValue)
				{
					typeString = "string";
					valueString = stringValue;
				}
				else
				{
					continue;
				}

				StringBuilder sb = isSettings ? sbLocal : sbGlobal;

				sb.Append(key);
				sb.Append(";");
				sb.Append(typeString);
				sb.Append(";");
				sb.Append(valueString);
				sb.AppendLine();
			}
			System.IO.File.WriteAllText(GetFullPath(m_LocalFilename), sbLocal.ToString());
			System.IO.File.WriteAllText(GetFullSaveFilePath(saveFileId), sbGlobal.ToString());
		}

		private static int GetSaveFileIdNumber(string line)
		{
			string numString = line.Substring(2, line.IndexOf('_') - 2);
			return int.Parse(numString);
		}
	}
}
