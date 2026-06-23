using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Interface;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Sprite;

namespace HSGameEngine.GameEngine.Render
{
	public static class RenderManager
	{
		public static void AddObject(IObject obj)
		{
			RenderManager._IObjectActionList[obj] = Global.GetCorrectLocalTime();
		}

		public static void RemoveObject(IObject obj)
		{
			RenderManager._IObjectActionList[obj] = 0L;
		}

		private static bool JugeIsLeader(IObject obj)
		{
			return obj is GSprite && (obj as GSprite).SpriteType == GSpriteTypes.Leader;
		}

		public static void ProcessRenderObject(bool isLeaderMoving, bool shiftIsPressed)
		{
			try
			{
				foreach (KeyValuePair<IObject, long> keyValuePair in RenderManager._IObjectsDict)
				{
					IObject key = keyValuePair.Key;
					if (key != null)
					{
						key.OnFrameRender();
					}
				}
				if (RenderManager._IObjectActionList.Count != 0)
				{
					foreach (KeyValuePair<IObject, long> keyValuePair2 in RenderManager._IObjectActionList)
					{
						if (keyValuePair2.Value == 0L)
						{
							Dictionary<IObject, long> iobjectsDict = RenderManager._IObjectsDict;
							Dictionary<IObject, long>.Enumerator enumerator;
							KeyValuePair<IObject, long> keyValuePair3 = enumerator.Current;
							iobjectsDict.Remove(keyValuePair3.Key);
						}
						else
						{
							Dictionary<IObject, long> iobjectsDict2 = RenderManager._IObjectsDict;
							Dictionary<IObject, long>.Enumerator enumerator;
							KeyValuePair<IObject, long> keyValuePair4 = enumerator.Current;
							IObject key2 = keyValuePair4.Key;
							KeyValuePair<IObject, long> keyValuePair5 = enumerator.Current;
							iobjectsDict2[key2] = keyValuePair5.Value;
						}
					}
					RenderManager._IObjectActionList.Clear();
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		private static Dictionary<IObject, long> _IObjectsDict = new Dictionary<IObject, long>();

		private static Dictionary<IObject, long> _IObjectActionList = new Dictionary<IObject, long>();

		public static string[] SpriteTypeNames = new string[]
		{
			"Leader",
			"Other",
			"Monster",
			"NPC",
			"Pet",
			"BiaoChe",
			"JunQi"
		};
	}
}
