using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	public class MonsterStaticInfoMgr
	{
		public static XElement AllMonstersXml { get; private set; }

		public static void Initialize()
		{
			if (MonsterStaticInfoMgr.AllMonstersXml == null)
			{
				XElement xelement = null;
				try
				{
					xelement = XElement.Load(Global.GameResPath("Config/Monsters.xml"));
				}
				catch (Exception ex)
				{
				}
				if (xelement == null)
				{
					throw new Exception(string.Format("加载系统怪物配置文件:{0}, 失败。没有找到相关XML配置文件!", "Config / Monsters.xml"));
				}
				MonsterStaticInfoMgr.AllMonstersXml = xelement;
			}
			MonsterStaticInfoMgr.AllInfos.Clear();
		}

		public static MonsterStaticInfo GetInfo(int MonsterID)
		{
			MonsterStaticInfo monsterStaticInfo = null;
			MonsterStaticInfo result;
			if (!MonsterStaticInfoMgr.AllInfos.TryGetValue(MonsterID, out monsterStaticInfo))
			{
				result = null;
			}
			else
			{
				result = monsterStaticInfo;
			}
			return result;
		}

		public static void SetInfo(int code, MonsterStaticInfo info)
		{
			MonsterStaticInfoMgr.AllInfos[code] = info;
		}

		private static Dictionary<int, MonsterStaticInfo> AllInfos = new Dictionary<int, MonsterStaticInfo>();
	}
}
