using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class KuaFuLueDuoServerInfo
	{
		public KuaFuLueDuoServerInfo Clone()
		{
			return new KuaFuLueDuoServerInfo
			{
				LastZiYuan = this.LastZiYuan,
				ServerId = this.ServerId,
				ShiChouList = this.ShiChouList,
				ZhengFuList = this.ZhengFuList,
				ZoneIdRangeList = this.ZoneIdRangeList
			};
		}

		public static List<KuaFuLueDuoRankInfo> MingXingStr2RankList(string mingxing)
		{
			List<KuaFuLueDuoRankInfo> list = new List<KuaFuLueDuoRankInfo>();
			if (!string.IsNullOrEmpty(mingxing))
			{
				string[] array = mingxing.Split(new char[]
				{
					'|'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					int key;
					int value;
					if (array3.Length == 3 && int.TryParse(array3[0], ref key) && int.TryParse(array3[1], ref value))
					{
						KuaFuLueDuoRankInfo kuaFuLueDuoRankInfo = new KuaFuLueDuoRankInfo
						{
							Key = key,
							Value = value,
							Param1 = KuaFuLueDuoServerInfo.UnZipStringToBase64EX(array3[2])
						};
						list.Add(kuaFuLueDuoRankInfo);
					}
				}
			}
			return list;
		}

		public static string UnZipStringToBase64EX(string base64)
		{
			try
			{
				if (string.IsNullOrEmpty(base64))
				{
					return string.Empty;
				}
				byte[] array = Convert.FromBase64String(base64);
				return new UTF8Encoding().GetString(array, 0, array.Length);
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return string.Empty;
		}

		[ProtoMember(1)]
		public int ServerId;

		[ProtoMember(2)]
		public List<int> ZoneIdRangeList;

		[ProtoMember(3)]
		public List<int> ZhengFuList;

		[ProtoMember(4)]
		public List<int> ShiChouList;

		[ProtoMember(5)]
		public int ZiYuan;

		[ProtoMember(6)]
		public int LastZiYuan;

		[ProtoMember(7)]
		public string MingXingZhanMengList;
	}
}
