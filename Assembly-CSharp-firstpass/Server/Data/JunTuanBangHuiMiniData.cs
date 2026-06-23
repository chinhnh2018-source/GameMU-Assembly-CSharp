using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class JunTuanBangHuiMiniData
	{
		public JunTuanBangHuiMiniData()
		{
		}

		public JunTuanBangHuiMiniData(int bhid, int junTuanId, string junTuanName, int junTuanZhiWu, int lingDi)
		{
			this.BhId = bhid;
			this.JunTuanId = junTuanId;
			this.JunTuanName = junTuanName;
			this.JunTuanZhiWu = junTuanZhiWu;
			this.LingDi = lingDi;
		}

		[ProtoMember(1)]
		public int BhId;

		[ProtoMember(2)]
		public int JunTuanId;

		[ProtoMember(3)]
		public string JunTuanName;

		[ProtoMember(4)]
		public int JunTuanZhiWu;

		[ProtoMember(5)]
		public int LingDi;

		[ProtoMember(6)]
		public int RoleId;
	}
}
