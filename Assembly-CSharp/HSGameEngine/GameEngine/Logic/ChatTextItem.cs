using System;
using System.Collections.Generic;
using Server.Data;

namespace HSGameEngine.GameEngine.Logic
{
	public class ChatTextItem
	{
		public ChatTextItem()
		{
			this.mReceiveID = (long)GChat.ChatReceiveID;
			this.IsRead = 0;
		}

		public long ReceiveID
		{
			get
			{
				return this.mReceiveID;
			}
		}

		public int IsRead { get; set; }

		public int FromRoleID { get; set; }

		public int PTID { get; set; }

		public string FromRoleName { get; set; }

		public int Status { get; set; }

		public string ToRoleName { get; set; }

		public ChatTypeIndexes ChatIndex { get; set; }

		public string TextMsg { get; set; }

		public long Ticks { get; set; }

		public ChatType ChatType { get; set; }

		public int occupation { get; set; }

		public int roleSex { get; set; }

		public byte[] EncodeVoiceBytes { get; set; }

		public int SpeakZoneID { get; set; }

		public int ListenZoneID { get; set; }

		public string VioceToWordStr { get; set; }

		public bool IsTeamSystemInfo { get; set; }

		private long mReceiveID;

		public float ClipLength = -1f;

		public List<GoodsData> ShowGoods;
	}
}
