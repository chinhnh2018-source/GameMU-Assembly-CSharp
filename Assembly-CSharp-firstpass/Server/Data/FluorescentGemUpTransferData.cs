using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class FluorescentGemUpTransferData
	{
		public FluorescentGemUpTransferData(int roleID, int levelupType, int bagIndex, int position, int gemType, Dictionary<int, int> decGoodsDic)
		{
			this._RoleID = roleID;
			this._UpType = levelupType;
			this._BagIndex = bagIndex;
			this._Position = position;
			this._GemType = gemType;
			this._DecGoodsDict = decGoodsDic;
		}

		[ProtoMember(1)]
		public int _RoleID;

		[ProtoMember(2)]
		public int _UpType;

		[ProtoMember(3)]
		public int _BagIndex;

		[ProtoMember(4)]
		public int _Position;

		[ProtoMember(5)]
		public int _GemType;

		[ProtoMember(6)]
		public Dictionary<int, int> _DecGoodsDict;
	}
}
