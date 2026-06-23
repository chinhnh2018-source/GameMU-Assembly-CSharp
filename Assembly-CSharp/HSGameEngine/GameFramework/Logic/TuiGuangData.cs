using System;
using System.Collections.Generic;
using ProtoBuf;

namespace HSGameEngine.GameFramework.Logic
{
	public class TuiGuangData
	{
		public string TuiGuangYuanID
		{
			get
			{
				return this.UpdateData.SpreadCode;
			}
			set
			{
				this.UpdateData.SpreadCode = value;
			}
		}

		public string TuiJianRenID
		{
			get
			{
				return this.UpdateData.VerifyCode;
			}
		}

		public int Amount
		{
			get
			{
				return this.UpdateData.CountRole;
			}
		}

		public int AwardVipAmount
		{
			get
			{
				int num = this.UpdateData.CountVip - this.AwardDicKeyValue(TuiGuangData.ESpreadAward.Vip);
				return (num <= 0) ? 0 : num;
			}
		}

		public int AwardLevelAmount
		{
			get
			{
				int num = this.UpdateData.CountLevel - this.AwardDicKeyValue(TuiGuangData.ESpreadAward.Level);
				return (num <= 0) ? 0 : num;
			}
		}

		public int BoxOpenedAmount
		{
			get
			{
				int num = 0;
				foreach (KeyValuePair<int, int> keyValuePair in this.UpdateData.AwardCountDic)
				{
					int value = keyValuePair.Value;
					if (value > 0)
					{
						num++;
					}
				}
				return num;
			}
		}

		public bool IsPhoneNumChecked
		{
			get
			{
				return this._IsPhoneNumChecked || !string.IsNullOrEmpty(this.UpdateData.VerifyCode);
			}
			set
			{
				this._IsPhoneNumChecked = value;
			}
		}

		public bool IsNewAwardReceived
		{
			get
			{
				return this.AwardDicKeyValue(TuiGuangData.ESpreadAward.Verify) != 0;
			}
		}

		public bool IsRegist
		{
			get
			{
				return !string.IsNullOrEmpty(this.TuiGuangYuanID);
			}
		}

		public int AwardDicKeyValue(TuiGuangData.ESpreadAward key)
		{
			int result = 0;
			if (this.UpdateData.AwardDic.ContainsKey((int)key))
			{
				int.TryParse(this.UpdateData.AwardDic[(int)key], ref result);
				return result;
			}
			return 0;
		}

		public TuiGuangData.SpreadData UpdateData = new TuiGuangData.SpreadData();

		public bool _IsPhoneNumChecked;

		[ProtoContract]
		public class SpreadData
		{
			[ProtoMember(1)]
			public bool IsOpen;

			[ProtoMember(2)]
			public string SpreadCode = string.Empty;

			[ProtoMember(3)]
			public string VerifyCode = string.Empty;

			[ProtoMember(4)]
			public int CountRole;

			[ProtoMember(5)]
			public int CountVip;

			[ProtoMember(6)]
			public int CountLevel;

			[ProtoMember(7)]
			public int State;

			[ProtoMember(8)]
			public Dictionary<int, string> AwardDic = new Dictionary<int, string>();

			[ProtoMember(9)]
			public Dictionary<int, int> AwardCountDic = new Dictionary<int, int>();
		}

		public enum ESpreadAward
		{
			Vip = 1,
			Level,
			Count,
			Verify
		}

		public enum ESpreadState
		{
			ETelMore = -36,
			ETelCodeOutTime,
			ETelCodeWrong,
			ETelCodeGet,
			ETelBind,
			ETelWrong,
			ETelNull,
			ESpreadIsSign = -21,
			ESpreadNo,
			EVerifyMore = -16,
			EVerifySelf,
			EVerifyCodeWrong,
			EVerifyOutTime,
			EVerifyCodeHave,
			EVerifyCodeNull,
			EVerifyNo,
			EServer = -5,
			ENoAward,
			ENoBag,
			ENoOpen,
			Fail,
			Default,
			Success
		}
	}
}
