using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;

namespace HSGameEngine.GameEngine.Logic
{
	public class GoodsMovingMgr
	{
		public static event GoodsMovingEndNotifyEventHandler GoodsMovingEndNotify
		{
			add
			{
				if (GoodsMovingMgr.GoodsMovingEndEvents.IndexOf(value) == -1)
				{
					GoodsMovingMgr.GoodsMovingEndEvents.Add(value);
				}
			}
			remove
			{
				GoodsMovingMgr.GoodsMovingEndEvents.Remove(value);
			}
		}

		public static BaseEventDispatcher GetEventDispatcher()
		{
			return GoodsMovingMgr.MyEventDispatcher;
		}

		public static int GetListBoxType()
		{
			return 0;
		}

		public static bool IsGoodsMoving()
		{
			return GoodsMovingMgr.GoodsMoving;
		}

		private static SpriteSL GetMovingObject()
		{
			return GoodsMovingMgr.MovingObject;
		}

		public static void StartGoodsMoving(int listBoxType, GoodsData gd, int x, int y)
		{
			GoodsMovingMgr.GoodsMoving = true;
		}

		public static bool GoodsMove(Point pt)
		{
			return GoodsMovingMgr.GoodsMoving && null != GoodsMovingMgr.MovingObject;
		}

		public static void CancelGoodsMoving()
		{
			GoodsMovingMgr.GoodsMoving = false;
			GoodsMovingMgr.MovingObject = null;
		}

		public static bool ProcessGoodsMovingEnd(object e)
		{
			GoodsMovingEndEventArgs goodsMovingEndEventArgs = new GoodsMovingEndEventArgs
			{
				ClickMouseButtonEventArgs = e,
				Handled = false
			};
			for (int i = 0; i < GoodsMovingMgr.GoodsMovingEndEvents.Count; i++)
			{
				GoodsMovingMgr.GoodsMovingEndEvents[i](null, goodsMovingEndEventArgs);
				if (goodsMovingEndEventArgs.Handled)
				{
					break;
				}
			}
			GoodsMovingMgr.CancelGoodsMoving();
			return goodsMovingEndEventArgs.Handled;
		}

		private static BaseEventDispatcher MyEventDispatcher;

		private static bool GoodsMoving = false;

		private static List<GoodsMovingEndNotifyEventHandler> GoodsMovingEndEvents = new List<GoodsMovingEndNotifyEventHandler>();

		private static SpriteSL MovingObject = null;

		public static bool GoodsMovingMgrHandled = false;
	}
}
