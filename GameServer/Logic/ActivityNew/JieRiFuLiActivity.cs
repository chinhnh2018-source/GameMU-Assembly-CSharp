using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class JieRiFuLiActivity : Activity
	{
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(this.FuLiCfgFile));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(this.FuLiCfgFile));
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("加载{0}时出错!!!文件不存在", this.FuLiCfgFile), null, true);
					return false;
				}
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					this.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					this.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
				}
				xelement2 = xelement.Element("GiftList");
				foreach (XElement xml in xelement2.Elements())
				{
					JieRiFuLiItem jieRiFuLiItem = new JieRiFuLiItem();
					jieRiFuLiItem.Type = (EJieRiFuLiType)Global.GetSafeAttributeLong(xml, "TypeID");
					jieRiFuLiItem.Open = (int)Global.GetSafeAttributeLong(xml, "Button");
					jieRiFuLiItem.StartDate = Global.GetSafeAttributeStr(xml, "AwardStartDate");
					jieRiFuLiItem.EndDate = Global.GetSafeAttributeStr(xml, "AwardEndDate");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Function");
					if (jieRiFuLiItem.Type == EJieRiFuLiType.CallPetReplace)
					{
						jieRiFuLiItem.Arg = Convert.ToInt32(safeAttributeStr);
					}
					else if (jieRiFuLiItem.Type == EJieRiFuLiType.SoulStoneExtFunc)
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						List<Tuple<int, int>> list = new List<Tuple<int, int>>();
						for (int i = 0; i < array.Length; i++)
						{
							string[] array2 = array[i].Split(new char[]
							{
								','
							});
							list.Add(new Tuple<int, int>(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1])));
						}
						jieRiFuLiItem.Arg = list;
					}
					else if (jieRiFuLiItem.Type == EJieRiFuLiType.OneDiscountDiamond)
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						List<double> list2 = new List<double>();
						for (int i = 0; i < array.Length; i++)
						{
							list2.Add(Convert.ToDouble(array[i]));
						}
						jieRiFuLiItem.Arg = list2;
					}
					else
					{
						jieRiFuLiItem.Arg = safeAttributeStr;
					}
					this.fuliDict.Add(jieRiFuLiItem.Type, jieRiFuLiItem);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", this.FuLiCfgFile, ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool IsOpened(EJieRiFuLiType type, out object arg)
		{
			arg = null;
			bool result;
			if (!this.InActivityTime())
			{
				result = false;
			}
			else
			{
				JieRiFuLiItem jieRiFuLiItem = null;
				if (!this.fuliDict.TryGetValue(type, out jieRiFuLiItem))
				{
					result = false;
				}
				else
				{
					DateTime t = DateTime.Parse(jieRiFuLiItem.StartDate);
					DateTime t2 = DateTime.Parse(jieRiFuLiItem.EndDate);
					if (TimeUtil.NowDateTime() < t || TimeUtil.NowDateTime() > t2)
					{
						result = false;
					}
					else if (jieRiFuLiItem.Open != 1)
					{
						result = false;
					}
					else
					{
						arg = jieRiFuLiItem.Arg;
						result = true;
					}
				}
			}
			return result;
		}

		public bool IsOpened(EJieRiFuLiType type)
		{
			bool result;
			if (!this.InActivityTime())
			{
				result = false;
			}
			else
			{
				JieRiFuLiItem jieRiFuLiItem = null;
				if (!this.fuliDict.TryGetValue(type, out jieRiFuLiItem))
				{
					result = false;
				}
				else
				{
					DateTime t = DateTime.Parse(jieRiFuLiItem.StartDate);
					DateTime t2 = DateTime.Parse(jieRiFuLiItem.EndDate);
					result = (!(TimeUtil.NowDateTime() < t) && !(TimeUtil.NowDateTime() > t2) && jieRiFuLiItem.Open == 1);
				}
			}
			return result;
		}

		private readonly string FuLiCfgFile = "Config/JieRiGifts/JieRiFuLi.xml";

		private Dictionary<EJieRiFuLiType, JieRiFuLiItem> fuliDict = new Dictionary<EJieRiFuLiType, JieRiFuLiItem>();
	}
}
