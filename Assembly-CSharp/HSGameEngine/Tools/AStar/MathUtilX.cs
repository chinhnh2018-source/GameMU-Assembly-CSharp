using System;
using HSGameEngine.Drawing;

namespace HSGameEngine.Tools.AStar
{
	public static class MathUtilX
	{
		public static CalcLineHandler getLineFunc(PointF ponit1, PointF point2, int type = 0)
		{
			CalcLineHandler result = null;
			if (ponit1.X == point2.X)
			{
				if (type == 0)
				{
					throw new Exception("两点所确定直线垂直于y轴，不能根据x值得到y值");
				}
				if (type == 1)
				{
					result = ((float y) => ponit1.X);
				}
				return result;
			}
			else
			{
				if (ponit1.Y == point2.Y)
				{
					if (type == 0)
					{
						result = ((float y) => ponit1.Y);
					}
					else if (type == 1)
					{
						throw new Exception("两点所确定直线垂直于y轴，不能根据x值得到y值");
					}
					return result;
				}
				float a = (ponit1.Y - point2.Y) / (ponit1.X - point2.X);
				float b = ponit1.Y - a * ponit1.X;
				if (type == 0)
				{
					result = ((float x) => a * x + b);
				}
				else if (type == 1)
				{
					result = delegate(float y)
					{
						if (a != 0f)
						{
							return (y - b) / a;
						}
						return 0f;
					};
				}
				return result;
			}
		}

		public static float getSlope(PointF ponit1, PointF point2)
		{
			return (point2.Y - ponit1.Y) / (point2.X - ponit1.X);
		}
	}
}
