using System;
using HSGameEngine.Drawing;

namespace HSGameEngine.GameEngine.Logic
{
	public class GoodsDragingMgr
	{
		public static void StartGoodsDraging(SpriteSL obj, int stageX, int stageY)
		{
		}

		public static bool GoodsMove(Point pt)
		{
			return GoodsDragingMgr.GoodsMoving && null != GoodsDragingMgr.MovingObject;
		}

		public static void CancelGoodsDraging()
		{
		}

		public static bool IsGoodsMoving()
		{
			return GoodsDragingMgr.GoodsMoving;
		}

		private static bool GoodsMoving;

		private static SpriteSL MovingObject;
	}
}
