using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.Goods
{
	public class GoodsReplaceManager : SingletonTemplate<GoodsReplaceManager>
	{
		private GoodsReplaceManager()
		{
		}

		public bool NeedCheckSuit(int categoriy)
		{
			return (categoriy >= 0 && categoriy <= 6) || (categoriy >= 11 && categoriy <= 21);
		}

		public void Init()
		{
			this.replaceJudgerDict.Clear();
			this.replaceJudgerDict["WingSuit".ToLower()] = new CondJudger_WingSuit();
			this.replaceJudgerDict["QiangHuaLevel".ToLower()] = new CondJudger_EquipForgeLvl();
			this.replaceJudgerDict["ZhuiJiaLevel".ToLower()] = new CondJudger_EquipAppendLvl();
			this.replaceJudgerDict["EquipSuit".ToLower()] = new CondJudger_EquipSuit();
			this.replaceJudgerDict["JuHun".ToLower()] = new CondJudger_JuHun();
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ReplaceGoods.xml"));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ReplaceGoods.xml"));
			if (xelement == null)
			{
				LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件不存在", "Config/ReplaceGoods.xml"), null, true);
			}
			else
			{
				try
				{
					this.replaceDict.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							GoodsReplaceManager.ReplaceRecord replaceRecord = new GoodsReplaceManager.ReplaceRecord();
							replaceRecord.seq = (int)Global.GetSafeAttributeLong(xelement2, "ID");
							replaceRecord.condIdx = Global.GetSafeAttributeStr(xelement2, "ToType").ToLower();
							replaceRecord.condArg = Global.GetSafeAttributeStr(xelement2, "ToTypeProperty");
							replaceRecord.oldGoods = (int)Global.GetSafeAttributeLong(xelement2, "OldGoods");
							replaceRecord.newGoods = (int)Global.GetSafeAttributeLong(xelement2, "NewGoods");
							List<GoodsReplaceManager.ReplaceRecord> list = null;
							if (!this.replaceDict.TryGetValue(replaceRecord.oldGoods, out list))
							{
								list = new List<GoodsReplaceManager.ReplaceRecord>();
								this.replaceDict[replaceRecord.oldGoods] = list;
							}
							list.Add(replaceRecord);
						}
					}
					foreach (KeyValuePair<int, List<GoodsReplaceManager.ReplaceRecord>> keyValuePair in this.replaceDict)
					{
						keyValuePair.Value.Sort(delegate(GoodsReplaceManager.ReplaceRecord left, GoodsReplaceManager.ReplaceRecord right)
						{
							int result;
							if (left.seq > right.seq)
							{
								result = 1;
							}
							else if (left.seq == right.seq)
							{
								result = 0;
							}
							else
							{
								result = -1;
							}
							return result;
						});
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("加载{0}时出错!!! {1}", "Config/ReplaceGoods.xml", ex.Message), null, true);
				}
			}
		}

		public GoodsReplaceResult GetReplaceResult(GameClient client, int OriginGoods)
		{
			GoodsReplaceResult result;
			if (client == null)
			{
				result = null;
			}
			else
			{
				GoodsReplaceResult goodsReplaceResult = new GoodsReplaceResult();
				goodsReplaceResult.OriginBindGoods.IsBind = true;
				goodsReplaceResult.OriginBindGoods.GoodsID = OriginGoods;
				goodsReplaceResult.OriginBindGoods.GoodsCnt = Global.GetTotalBindGoodsCountByID(client, OriginGoods);
				goodsReplaceResult.OriginUnBindGoods.IsBind = false;
				goodsReplaceResult.OriginUnBindGoods.GoodsID = OriginGoods;
				goodsReplaceResult.OriginUnBindGoods.GoodsCnt = Global.GetTotalNotBindGoodsCountByID(client, OriginGoods);
				List<GoodsReplaceManager.ReplaceRecord> list = null;
				if (this.replaceDict.TryGetValue(OriginGoods, out list))
				{
					foreach (GoodsReplaceManager.ReplaceRecord replaceRecord in list)
					{
						ICondJudger condJudger = null;
						if (this.replaceJudgerDict.TryGetValue(replaceRecord.condIdx, out condJudger))
						{
							string empty = string.Empty;
							if (condJudger.Judge(client, replaceRecord.condArg, out empty))
							{
								int newGoods = replaceRecord.newGoods;
								int totalBindGoodsCountByID = Global.GetTotalBindGoodsCountByID(client, newGoods);
								int totalNotBindGoodsCountByID = Global.GetTotalNotBindGoodsCountByID(client, newGoods);
								if (totalBindGoodsCountByID > 0)
								{
									GoodsReplaceResult.ReplaceItem replaceItem = new GoodsReplaceResult.ReplaceItem();
									replaceItem.IsBind = true;
									replaceItem.GoodsID = newGoods;
									replaceItem.GoodsCnt = totalBindGoodsCountByID;
									goodsReplaceResult.TotalBindCnt += totalBindGoodsCountByID;
									goodsReplaceResult.BindList.Add(replaceItem);
								}
								if (totalNotBindGoodsCountByID > 0)
								{
									GoodsReplaceResult.ReplaceItem replaceItem = new GoodsReplaceResult.ReplaceItem();
									replaceItem.IsBind = false;
									replaceItem.GoodsID = newGoods;
									replaceItem.GoodsCnt = totalNotBindGoodsCountByID;
									goodsReplaceResult.TotalUnBindCnt += totalNotBindGoodsCountByID;
									goodsReplaceResult.UnBindList.Add(replaceItem);
								}
							}
						}
					}
				}
				result = goodsReplaceResult;
			}
			return result;
		}

		private const string ReplaceCfgFile = "Config/ReplaceGoods.xml";

		private Dictionary<int, List<GoodsReplaceManager.ReplaceRecord>> replaceDict = new Dictionary<int, List<GoodsReplaceManager.ReplaceRecord>>();

		private Dictionary<string, ICondJudger> replaceJudgerDict = new Dictionary<string, ICondJudger>();

		private class ReplaceRecord
		{
			public int seq;

			public string condIdx;

			public string condArg;

			public int oldGoods;

			public int newGoods;
		}
	}
}
