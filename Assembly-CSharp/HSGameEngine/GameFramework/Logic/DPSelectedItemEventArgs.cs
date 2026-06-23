using System;
using Server.Data;

namespace HSGameEngine.GameFramework.Logic
{
	public class DPSelectedItemEventArgs : EventArgs
	{
		public int ID { get; set; }

		public int IDType { get; set; }

		public bool ShowFlagUpdate { get; set; }

		public object Tag { get; set; }

		public object Data { get; set; }

		public int Index { get; set; }

		public int MagicType { get; set; }

		public int[] EquipIDs { get; set; }

		public int Flag { get; set; }

		public int FilterType { get; set; }

		public GoodsData ZhuZhuangBei { get; set; }

		public GoodsData FuZhuangBei { get; set; }

		public bool AutoUseGold { get; set; }

		public int NextMainTaskID { get; set; }

		public int HandleType { get; set; }

		public int NeedYuanBao { get; set; }

		public int MyID { get; set; }

		public string Title { get; set; }

		public int Quality { get; set; }

		public int Level { get; set; }

		public bool AllowAutoBuy { get; set; }

		public string CountdownInfo { get; set; }

		public int buyFrom { get; set; }

		public int GetThingOpt
		{
			get
			{
				return this.m_GetThingOpt;
			}
			set
			{
				this.m_GetThingOpt = value;
			}
		}

		public BufferData MyBufferData { get; set; }

		public int Type { get; set; }

		public int ItemCode
		{
			get
			{
				return this.IDType;
			}
		}

		public int CanUse
		{
			get
			{
				return this.MagicType;
			}
			set
			{
				this.MagicType = value;
			}
		}

		public static readonly DPSelectedItemEventArgs Empty;

		private int m_GetThingOpt = 1;
	}
}
