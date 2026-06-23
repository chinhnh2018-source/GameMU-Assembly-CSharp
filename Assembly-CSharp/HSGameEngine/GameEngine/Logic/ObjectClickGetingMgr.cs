using System;
using System.Collections.Generic;

namespace HSGameEngine.GameEngine.Logic
{
	public class ObjectClickGetingMgr
	{
		public static event ClickGetThingNotifyEventHandler ClickGetThingNotify
		{
			add
			{
				if (ObjectClickGetingMgr.ClickGetThingEvents.IndexOf(value) == -1)
				{
					ObjectClickGetingMgr.ClickGetThingEvents.Add(value);
				}
			}
			remove
			{
				ObjectClickGetingMgr.ClickGetThingEvents.Remove(value);
			}
		}

		public static bool IsType(int type)
		{
			return type == ObjectClickGetingMgr.ClickGetThingType;
		}

		public static void StartClickGetThing(int clickGetThingType, object e)
		{
			ObjectClickGetingMgr.ClickGetThingType = clickGetThingType;
			string obj = "Images/Cursor/101.png";
			int arg = 101;
			Global.SetGlobalGameCursor(obj, arg);
		}

		public static void CancelClickGetThing(int clickGetThingType = -1)
		{
			if (clickGetThingType >= 0 && clickGetThingType != ObjectClickGetingMgr.ClickGetThingType)
			{
				return;
			}
			ObjectClickGetingMgr.ClickGetThingType = -1;
			Global.SetGlobalGameCursor(null, 0);
		}

		public static bool ProcessClickGetThing(object sender, int goodsDbID, object e)
		{
			if (ObjectClickGetingMgr.ClickGetThingType < 0)
			{
				return false;
			}
			Global.SetGlobalGameCursor(null, 0);
			ClickGetThingEventArgs clickGetThingEventArgs = new ClickGetThingEventArgs
			{
				ClickGetThingDbID = goodsDbID,
				ClickGetThingType = ObjectClickGetingMgr.ClickGetThingType,
				e = e,
				Handled = false,
				NextClick = false
			};
			for (int i = 0; i < ObjectClickGetingMgr.ClickGetThingEvents.Count; i++)
			{
				ObjectClickGetingMgr.ClickGetThingEvents[i](sender, clickGetThingEventArgs);
				if (clickGetThingEventArgs.Handled)
				{
					break;
				}
			}
			if (clickGetThingEventArgs.NextClick)
			{
				ObjectClickGetingMgr.StartClickGetThing(ObjectClickGetingMgr.ClickGetThingType, e);
			}
			else
			{
				ObjectClickGetingMgr.ClickGetThingType = -1;
			}
			return clickGetThingEventArgs.Handled;
		}

		public static bool IsInClickGetThing()
		{
			return false;
		}

		private static int ClickGetThingType = -1;

		private static List<ClickGetThingNotifyEventHandler> ClickGetThingEvents = new List<ClickGetThingNotifyEventHandler>();
	}
}
