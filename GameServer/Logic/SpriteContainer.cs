using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	public class SpriteContainer
	{
		public void initialize(IEnumerable<XElement> mapItems)
		{
			foreach (XElement xml in mapItems)
			{
				int key = (int)Global.GetSafeAttributeLong(xml, "Code");
				List<object> value = new List<object>(100);
				this._MapObjectDict.Add(key, value);
			}
		}

		public Dictionary<int, object> ObjectDict
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

		public void AddObject(int id, int mapCode, object obj)
		{
			lock (this._ObjectDict)
			{
				this._ObjectDict.Add(id, obj);
			}
			List<object> list = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out list))
			{
				lock (list)
				{
					list.Add(obj);
				}
			}
		}

		public bool RemoveObject(int id, int mapCode, object obj)
		{
			bool result = false;
			lock (this._ObjectDict)
			{
				try
				{
					if (this._ObjectDict.ContainsKey(id))
					{
						this._ObjectDict.Remove(id);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			List<object> list = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out list))
			{
				try
				{
					lock (list)
					{
						result = list.Remove(obj);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return result;
		}

		public List<object> GetObjectsByMap(int mapCode)
		{
			List<object> result = null;
			List<object> list = null;
			if (this._MapObjectDict.TryGetValue(mapCode, out list))
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

		public object FindObject(int id)
		{
			object result = null;
			lock (this._ObjectDict)
			{
				this._ObjectDict.TryGetValue(id, out result);
			}
			return result;
		}

		public string GetAllMapRoleNumStr()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<int, List<object>> keyValuePair in this._MapObjectDict)
			{
				lock (keyValuePair.Value)
				{
					if (keyValuePair.Value.Count > 0)
					{
						stringBuilder.AppendFormat("{0}:{1}\n", keyValuePair.Key, keyValuePair.Value.Count);
					}
				}
			}
			return stringBuilder.ToString();
		}

		private Dictionary<int, object> _ObjectDict = new Dictionary<int, object>(1000);

		private Dictionary<int, List<object>> _MapObjectDict = new Dictionary<int, List<object>>(1000);
	}
}
