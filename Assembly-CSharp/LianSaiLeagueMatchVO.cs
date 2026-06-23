using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class LianSaiLeagueMatchVO
{
	public LianSaiLeagueMatchVO(XElement xml)
	{
		if (xml != null)
		{
			this.ID = Global.GetXElementAttributeInt(xml, "ID");
			this.Name = Global.GetXElementAttributeStr(xml, "Name");
			this.Random = Global.GetXElementAttributeInt(xml, "Random");
			this.Round = Global.GetXElementAttributeInt(xml, "Round");
			this.mMapCode = Global.GetXElementAttributeStr(xml, "MapCode");
			this.mTimePoints = Global.GetXElementAttributeStr(xml, "TimePoints");
			this.ApplyStartTime = Global.GetXElementAttributeLong(xml, "ApplyStartTime");
			this.ApplyOverTime = Global.GetXElementAttributeLong(xml, "ApplyOverTime");
			this.PrepareSecs = Global.GetXElementAttributeLong(xml, "PrepareSecs");
			this.FightingSecs = Global.GetXElementAttributeLong(xml, "FightingSecs");
			this.ClearRolesSecs = Global.GetXElementAttributeLong(xml, "ClearRolesSecs");
			this.MaxEnterNum = Global.GetXElementAttributeInt(xml, "MaxEnterNum");
			this.Exp = Global.GetXElementAttributeInt(xml, "Exp");
			this.BandJInBi = Global.GetXElementAttributeInt(xml, "BandJInBi");
			this.mWinGoods = Global.GetXElementAttributeStr(xml, "WinGoods");
			this.mLoseGoods = Global.GetXElementAttributeStr(xml, "LoseGoods");
		}
	}

	public int[] MapCode
	{
		get
		{
			int[] array = new int[2];
			if (!string.IsNullOrEmpty(this.mMapCode))
			{
				string[] array2 = this.mMapCode.Split(new char[]
				{
					'|'
				});
				if (array2 != null && array2.Length == 2)
				{
					array[0] = Global.SafeConvertToInt32(array2[0]);
					array[1] = Global.SafeConvertToInt32(array2[1]);
				}
			}
			return array;
		}
	}

	public List<LianSaiLeagueMatchVO.TimePoint> TimePoints
	{
		get
		{
			if (this.mTimePointList.Count == 0 && !string.IsNullOrEmpty(this.mTimePoints))
			{
				string[] array = this.mTimePoints.Split(new char[]
				{
					'|'
				});
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (!string.IsNullOrEmpty(array[i]))
						{
							string[] array2 = array[i].Split(new char[]
							{
								','
							});
							if (array2.Length == 2)
							{
								LianSaiLeagueMatchVO.TimePoint timePoint = default(LianSaiLeagueMatchVO.TimePoint);
								timePoint.Week = array2[0].SafeToInt32(0);
								if (timePoint.Week == 7)
								{
									timePoint.Week = 0;
								}
								string[] array3 = array2[1].Split(new char[]
								{
									'-'
								});
								try
								{
									DateTime correctDateTime = Global.GetCorrectDateTime();
									timePoint.Time1 = new DateTime(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, array3[0].Split(new char[]
									{
										':'
									})[0].SafeToInt32(0), array3[0].Split(new char[]
									{
										':'
									})[1].SafeToInt32(0), 0);
									timePoint.Time2 = new DateTime(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, array3[1].Split(new char[]
									{
										':'
									})[0].SafeToInt32(0), array3[1].Split(new char[]
									{
										':'
									})[1].SafeToInt32(0), 0);
								}
								catch (Exception ex)
								{
									MUDebug.LogError<string>(new string[]
									{
										ex.Message
									});
								}
								this.mTimePointList.Add(timePoint);
							}
						}
					}
				}
			}
			return this.mTimePointList;
		}
	}

	public List<GoodsData> WinGoods
	{
		get
		{
			return this.GetGoods(this.mWinGoods);
		}
	}

	public List<GoodsData> LoseGoods
	{
		get
		{
			return this.GetGoods(this.mLoseGoods);
		}
	}

	private List<GoodsData> GetGoods(string str)
	{
		List<GoodsData> list = new List<GoodsData>();
		if (!string.IsNullOrEmpty(str))
		{
			string[] array = str.Split(new char[]
			{
				'|'
			});
			if (array != null && 0 < array.Length)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						if (array2.Length == 7)
						{
							GoodsData emptyGoodsData = Global.GetEmptyGoodsData(array2[0].SafeToInt32(0), array2[1].SafeToInt32(0), array2[2].SafeToInt32(0), array2[3].SafeToInt32(0), array2[4].SafeToInt32(0), array2[5].SafeToInt32(0), array2[6].SafeToInt32(0), 0, 0);
							if (emptyGoodsData != null)
							{
								list.Add(emptyGoodsData);
							}
						}
					}
				}
			}
		}
		return list;
	}

	public int ID;

	public string Name;

	public int Random;

	public int Round;

	private string mMapCode;

	private string mTimePoints;

	public long ApplyStartTime;

	public long ApplyOverTime;

	public long PrepareSecs;

	public long FightingSecs;

	public long ClearRolesSecs;

	public int MaxEnterNum;

	public int Exp;

	public int BandJInBi;

	private string mWinGoods;

	private string mLoseGoods;

	private List<LianSaiLeagueMatchVO.TimePoint> mTimePointList = new List<LianSaiLeagueMatchVO.TimePoint>();

	public struct TimePoint
	{
		public int Week;

		public DateTime Time1;

		public DateTime Time2;
	}
}
