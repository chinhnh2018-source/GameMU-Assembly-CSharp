using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Interface;
using HSGameEngine.GameEngine.Sprite;

namespace HSGameEngine.GameEngine.Logic
{
	public static class ObjectsManager
	{
		public static Dictionary<string, IObject>.Enumerator GetEnumerator()
		{
			return ObjectsManager.ObjectDict.GetEnumerator();
		}

		public static List<IObject> GetObjectsList()
		{
			return Enumerable.ToList<IObject>(ObjectsManager.ObjectDict.Values);
		}

		public static void Clear()
		{
			string[] array = new string[ObjectsManager.ObjectDict.Keys.Count];
			ObjectsManager.ObjectDict.Keys.CopyTo(array, 0);
			foreach (string text in array)
			{
				if (ObjectsManager.ObjectDict.ContainsKey(text))
				{
					ObjectsManager.ObjectDict[text].Destroy();
					ObjectsManager.ObjectDict.Remove(text);
				}
			}
			ObjectsManager.SpriteList.Clear();
		}

		public static IObject FindSprite(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			IObject result = null;
			ObjectsManager.ObjectDict.TryGetValue(name, ref result);
			return result;
		}

		public static GSprite FindSprite(int id)
		{
			GSprite result = null;
			ObjectsManager.SpriteList.TryGetValue(id, ref result);
			return result;
		}

		public static void Add(IObject theObject)
		{
			try
			{
				if (!ObjectsManager.ObjectDict.ContainsKey(theObject.Name))
				{
					ObjectsManager.ObjectDict.Add(theObject.Name, theObject);
					GSprite gsprite = theObject as GSprite;
					if (gsprite != null)
					{
						if (!ObjectsManager.SpriteList.ContainsKey(gsprite.RoleID))
						{
							ObjectsManager.SpriteList.Add(gsprite.RoleID, gsprite);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public static void Remove(IObject theObject)
		{
			try
			{
				ObjectsManager.ObjectDict.Remove(theObject.Name);
				GSprite gsprite = theObject as GSprite;
				if (gsprite != null)
				{
					ObjectsManager.SpriteList.Remove(gsprite.RoleID);
				}
			}
			catch (Exception ex)
			{
				GError.AddErrMsg(new Exception(string.Format(Global.GetLang("对象名称不存在: {0}, 类型:{1}"), theObject.Name, theObject.GetType())));
				MUDebug.LogException(ex);
			}
		}

		public static string GetAutoObjectName(string preName)
		{
			return string.Format("{0}_ObjName_{1}", preName, ObjectsManager.ObjectNameIndex++);
		}

		private static int ObjectNameIndex = 1;

		private static Dictionary<string, IObject> ObjectDict = new Dictionary<string, IObject>();

		private static Dictionary<int, GSprite> SpriteList = new Dictionary<int, GSprite>();
	}
}
