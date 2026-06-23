using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	internal class MonsterContainer
	{
		public void initialize(IEnumerable<XElement> mapItems)
		{
			foreach (XElement xml in mapItems)
			{
				int num = (int)Global.GetSafeAttributeLong(xml, "Code");
				List<object> value = new List<object>(100);
				this._MapObjectDict.Add(num, value);
				Dictionary<int, object> value2 = new Dictionary<int, object>(100);
				this._ObjectDict.Add(num, value2);
				if (num == 6090)
				{
					for (int i = 0; i < 25; i++)
					{
						Dictionary<int, object> value3 = new Dictionary<int, object>(2000);
						this._FreshPlayerObjectDict.Add(i, value3);
						List<object> value4 = new List<object>(100);
						this._FreshPlayerMapObjectDict.Add(i, value4);
					}
				}
			}
		}

		public List<object> ObjectList
		{
			get
			{
				return this._ObjectList;
			}
		}

		public Dictionary<int, Dictionary<int, object>> ObjectDict
		{
			get
			{
				return this._ObjectDict;
			}
		}

		public Dictionary<int, List<object>> MapObjectDict
		{
			get
			{
				return this._MapObjectDict;
			}
		}

		public Dictionary<int, List<object>> CopyMapIDObjectDict
		{
			get
			{
				return this._CopyMapIDObjectDict;
			}
		}

		public void AddObject(int id, int mapCode, int copyMapID, Monster obj)
		{
			lock (this._ObjectList)
			{
				this._ObjectList.Add(obj);
			}
			Dictionary<int, object> dictionary = null;
			if (this._ObjectDict.TryGetValue(mapCode, out dictionary))
			{
				lock (dictionary)
				{
					dictionary.Add(id, obj);
				}
			}
			List<object> list = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out list))
			{
				lock (list)
				{
					list.Add(obj);
				}
			}
			if (mapCode == 6090)
			{
				int randomNumber = Global.GetRandomNumber(0, 24);
				obj.SubMapCode = randomNumber;
				List<object> list2 = null;
				if (this._FreshPlayerMapObjectDict.TryGetValue(randomNumber, out list2))
				{
					lock (list2)
					{
						list2.Add(obj);
					}
				}
				Dictionary<int, object> dictionary2 = null;
				if (this._FreshPlayerObjectDict.TryGetValue(randomNumber, out dictionary2))
				{
					lock (dictionary2)
					{
						dictionary2.Add(id, obj);
					}
				}
			}
			lock (this._CopyMapIDObjectDict)
			{
				List<object> list3 = null;
				if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out list3))
				{
					list3.Add(obj);
				}
				else
				{
					list3 = new List<object>(100);
					list3.Add(obj);
					this._CopyMapIDObjectDict.Add(copyMapID, list3);
				}
			}
		}

		public void RemoveObject(int id, int mapCode, int copyMapID, Monster obj)
		{
			lock (this._ObjectList)
			{
				this._ObjectList.Remove(obj);
			}
			Dictionary<int, object> dictionary = null;
			if (this._ObjectDict.TryGetValue(mapCode, out dictionary))
			{
				lock (dictionary)
				{
					dictionary.Remove(id);
				}
			}
			List<object> list = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out list))
			{
				try
				{
					lock (list)
					{
						list.Remove(obj);
					}
				}
				catch (Exception)
				{
				}
			}
			if (mapCode == 6090)
			{
				int subMapCode = obj.SubMapCode;
				List<object> list2 = null;
				if (this._FreshPlayerMapObjectDict.TryGetValue(subMapCode, out list2))
				{
					try
					{
						lock (list2)
						{
							list2.Remove(obj);
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
				Dictionary<int, object> dictionary2 = null;
				if (this._FreshPlayerObjectDict.TryGetValue(subMapCode, out dictionary2))
				{
					try
					{
						lock (dictionary2)
						{
							dictionary2.Remove(id);
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
			}
			List<object> list3 = null;
			if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out list3))
			{
				try
				{
					lock (list3)
					{
						list3.Remove(obj);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public List<object> GetObjectsByMap(int mapCode, int subMapCode = -1)
		{
			List<object> result = null;
			List<object> list = null;
			if (mapCode == 6090 && subMapCode != -1)
			{
				if (this._FreshPlayerMapObjectDict.TryGetValue(subMapCode, out list))
				{
					lock (list)
					{
						result = list.GetRange(0, list.Count);
					}
				}
			}
			else if (this._MapObjectDict.TryGetValue(mapCode, out list))
			{
				lock (list)
				{
					result = list.GetRange(0, list.Count);
				}
			}
			return result;
		}

		public int GetObjectsCountByMap(int mapCode)
		{
			int result = 0;
			List<object> list = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out list))
			{
				lock (list)
				{
					result = list.Count;
				}
			}
			return result;
		}

		public List<object> GetObjectsByCopyMapID(int copyMapID)
		{
			List<object> result = null;
			List<object> list = null;
			if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out list))
			{
				lock (list)
				{
					result = list.GetRange(0, list.Count);
				}
			}
			return result;
		}

		public int GetObjectsCountByCopyMapID(int copyMapID, int aliveType = -1)
		{
			int num = 0;
			List<object> list = null;
			if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out list))
			{
				if (null != list)
				{
					if (-1 == aliveType)
					{
						lock (list)
						{
							num = list.Count;
						}
					}
					else if (0 == aliveType)
					{
						lock (list)
						{
							for (int i = 0; i < list.Count; i++)
							{
								if ((list[i] as Monster).VLife > 0.0 && (list[i] as Monster).Alive && (list[i] as Monster).MonsterType != 1001)
								{
									num++;
								}
							}
						}
					}
					else
					{
						lock (list)
						{
							for (int i = 0; i < list.Count; i++)
							{
								if (!(list[i] as Monster).Alive)
								{
									num++;
								}
							}
						}
					}
				}
			}
			return num;
		}

		public bool IsAnyMonsterAliveByCopyMapID(int copyMapID)
		{
			List<object> list = null;
			if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out list))
			{
				if (null != list)
				{
					lock (list)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if ((list[i] as Monster).Alive && (list[i] as Monster).MonsterType != 1001)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public object FindObject(int id, int mapCode)
		{
			object result = null;
			Dictionary<int, object> dictionary = null;
			if (this._ObjectDict.TryGetValue(mapCode, out dictionary))
			{
				lock (dictionary)
				{
					dictionary.TryGetValue(id, out result);
				}
			}
			return result;
		}

		public List<object> FindObjectAll(int mapCode)
		{
			List<object> list = new List<object>();
			Dictionary<int, object> dictionary = null;
			if (this._ObjectDict.TryGetValue(mapCode, out dictionary))
			{
				lock (dictionary)
				{
					foreach (KeyValuePair<int, object> keyValuePair in dictionary)
					{
						list.Add(keyValuePair.Value);
					}
				}
			}
			return list;
		}

		public List<object> FindObjectsByExtensionID(int extensionID, int copyMapID)
		{
			List<object> list = new List<object>();
			List<object> list2 = null;
			if (this._CopyMapIDObjectDict.TryGetValue(copyMapID, out list2))
			{
				if (null != list2)
				{
					lock (list2)
					{
						for (int i = 0; i < list2.Count; i++)
						{
							if ((list2[i] as Monster).VLife > 0.0 && (list2[i] as Monster).Alive && (list2[i] as Monster).MonsterInfo.ExtensionID == extensionID)
							{
								list.Add(list2[i]);
							}
						}
					}
				}
			}
			return list;
		}

		public List<object> _ObjectList = new List<object>(20000);

		private Dictionary<int, Dictionary<int, object>> _ObjectDict = new Dictionary<int, Dictionary<int, object>>(10000);

		private Dictionary<int, Dictionary<int, object>> _FreshPlayerObjectDict = new Dictionary<int, Dictionary<int, object>>(50);

		private Dictionary<int, List<object>> _MapObjectDict = new Dictionary<int, List<object>>(10000);

		private Dictionary<int, List<object>> _FreshPlayerMapObjectDict = new Dictionary<int, List<object>>(50);

		private Dictionary<int, List<object>> _CopyMapIDObjectDict = new Dictionary<int, List<object>>(10000);
	}
}
