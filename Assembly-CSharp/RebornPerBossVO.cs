using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using Server.Data;

public class RebornPerBossVO
{
	public int PaychallgeNum(int Num)
	{
		if (Num > this.FreeChallengeNum)
		{
			int num = Num - this.FreeChallengeNum - 1;
			string[] array = this.PayChallengeNum.Split(new char[]
			{
				','
			});
			int[] array2 = new int[array.Length];
			byte b = 0;
			while ((int)b < array.Length)
			{
				array2[(int)b] = array[(int)b].SafeToInt32(0);
				b += 1;
			}
			if (0 < array2.Length)
			{
				if (num < array2.Length)
				{
					return array2[num];
				}
				return array2[array2.Length - 1];
			}
		}
		return 0;
	}

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

	internal BetterList<GoodsData> GetAwardGoodsDatas()
	{
		BetterList<GoodsData> betterList = new BetterList<GoodsData>();
		if (0 >= this.AwardGoodsList.Count)
		{
			if (!string.IsNullOrEmpty(this.PerfectGoodsOne))
			{
				string[] array = this.PerfectGoodsOne.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					GoodsData goodsDataByStr = Global.GetGoodsDataByStr(array[i], 0);
					this.AwardGoodsList.Add(goodsDataByStr);
				}
			}
			if (!string.IsNullOrEmpty(this.GoodsTwo))
			{
				string[] array2 = this.GoodsTwo.Split(new char[]
				{
					'|'
				});
				for (int j = 0; j < array2.Length; j++)
				{
					GoodsData goodsDataByStr2 = Global.GetGoodsDataByStr(array2[j], 0);
					if (!MUJieripartChongzhiKingItem.IsTongGuo(goodsDataByStr2.GoodsID.ToString(), Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation)))
					{
						this.AwardGoodsList.Add(goodsDataByStr2);
					}
				}
			}
		}
		for (int k = 0; k < this.AwardGoodsList.Count; k++)
		{
			betterList.Add(this.AwardGoodsList[k]);
		}
		return betterList;
	}

	public int DayMaxEnterNum
	{
		get
		{
			return this.FreeChallengeNum + this.PayChallengeNum.Split(new char[]
			{
				','
			}).Length;
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

	public int FreeChallengeNum;

	public string PayChallengeNum;

	public string PerfectGoodsOne;

	public string UnPerfectGoodsOne;

	public string FailGoodsOne;

	public string GoodsTwo;

	public int MoppingConditions;

	public string ChallengeReward;

	private List<GoodsData> AwardGoodsList = new List<GoodsData>();
}
