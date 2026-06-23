using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KuaFuLueDuoMainInfo
	{
		public List<KuaFuLueDuoServerInfo> CloneServerList
		{
			get
			{
				List<KuaFuLueDuoServerInfo> list = new List<KuaFuLueDuoServerInfo>();
				try
				{
					int i = 0;
					int count = this.ServerList.Count;
					while (i < count)
					{
						list.Add(this.ServerList[i].Clone());
						i++;
					}
				}
				catch (Exception ex)
				{
					MUDebug.Log<string>(new string[]
					{
						ex.Message
					});
				}
				return list;
			}
		}

		public KuaFuLueDuoServerInfo GetKuaFuLueDuoServerInfoDataByID(int serverID)
		{
			return this.ServerList.Find((KuaFuLueDuoServerInfo e) => e.ServerId == serverID);
		}

		public List<KuaFuLueDuoServerJingJiaState> CloneStateList
		{
			get
			{
				List<KuaFuLueDuoServerJingJiaState> list = new List<KuaFuLueDuoServerJingJiaState>();
				int i = 0;
				int count = this.StateList.Count;
				while (i < count)
				{
					list.Add(this.StateList[i].Clone());
					i++;
				}
				return list;
			}
		}

		public KuaFuLueDuoServerJingJiaState GetKuaFuLueDuoServerJingJiaStateDataByID(int serverID)
		{
			return this.StateList.Find((KuaFuLueDuoServerJingJiaState e) => e.ServerId == serverID);
		}

		[ProtoMember(1)]
		public long ServerListAge;

		[ProtoMember(2)]
		public List<KuaFuLueDuoServerInfo> ServerList = new List<KuaFuLueDuoServerInfo>();

		[ProtoMember(3)]
		public long StateListAge;

		[ProtoMember(4)]
		public List<KuaFuLueDuoServerJingJiaState> StateList = new List<KuaFuLueDuoServerJingJiaState>();

		[ProtoMember(5)]
		public KuaFuLueDuoBangHuiJingJiaData JingJiaData;
	}
}
