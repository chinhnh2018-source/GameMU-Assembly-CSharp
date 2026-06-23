using System;
using HSGameEngine.Drawing;

public class RebornBossVO
{
	public Point monsetRefreshPos
	{
		get
		{
			Point result = default(Point);
			if (!string.IsNullOrEmpty(this.Site))
			{
				string[] array = this.Site.Split(new char[]
				{
					'|'
				});
				result.X = array[0].SafeToInt32(0);
				result.Y = array[1].SafeToInt32(0);
			}
			return result;
		}
	}

	public int ID;

	public int MapID;

	public int MonstersID;

	public int RebornLevel;

	public int ZhanLi;

	public double Scale;

	public string Site;

	public int Radius;

	public int Num;

	public int PursuitRadius;

	public int Time;

	public int EffectiveTime;
}
