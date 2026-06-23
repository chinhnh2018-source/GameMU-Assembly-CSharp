using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAwakenSuitDetail
	{
		public MUAwakenSuitDetail()
		{
		}

		public MUAwakenSuitDetail(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Type = XMLHelper.GetIntArrtibute(xe, "Type");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_Icon = XMLHelper.GetStringArrtibute(xe, "Icon");
			this.m_SpecialEffect = XMLHelper.GetStringArrtibute(xe, "SpecialEffect");
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, "AwakenID");
			this.m_AwakenIDs = new List<int>();
			string[] array = stringArrtibute.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				this.m_AwakenIDs.Add(Global.SafeConvertToInt32(array[i]));
			}
			this.m_TaoZhuangProps1Num = XMLHelper.GetIntArrtibute(xe, "TaoZhuangProps1Num");
			string stringArrtibute2 = XMLHelper.GetStringArrtibute(xe, "TaoZhuangProps1");
			this.m_TaoZhuangProps1 = MUPropInfoHelper.DeserializeToPropList(stringArrtibute2);
			this.m_TaoZhuangProps2Num = XMLHelper.GetIntArrtibute(xe, "TaoZhuangProps2Num");
			string stringArrtibute3 = XMLHelper.GetStringArrtibute(xe, "TaoZhuangProps2");
			this.m_TaoZhuangProps2 = MUPropInfoHelper.DeserializeToPropList(stringArrtibute3);
			this.m_TaoZhuangProps3Num = XMLHelper.GetIntArrtibute(xe, "TaoZhuangProps3Num");
			string stringArrtibute4 = XMLHelper.GetStringArrtibute(xe, "TaoZhuangProps3");
			this.m_TaoZhuangProps3 = MUPropInfoHelper.DeserializeToPropList(stringArrtibute4);
			string stringArrtibute5 = XMLHelper.GetStringArrtibute(xe, "WeaponMaster");
			string[] array2 = stringArrtibute5.Split(new char[]
			{
				','
			});
			if (array2.Length < 2)
			{
				this.m_WeaponMasterIndex = 0;
				this.m_WeaponMasterType = 0;
			}
			else
			{
				this.m_WeaponMasterIndex = array2[0].SafeToInt32(0);
				this.m_WeaponMasterType = array2[1].SafeToInt32(0);
			}
			this.m_MagicIds = new List<int>();
			string stringArrtibute6 = XMLHelper.GetStringArrtibute(xe, "Magic");
			if (stringArrtibute6 != string.Empty)
			{
				string[] array3 = stringArrtibute6.Split(new char[]
				{
					','
				});
				if (array3.Length < 2)
				{
					this.m_MagicEffectNum = 0;
				}
				else
				{
					this.m_MagicEffectNum = array3[0].SafeToInt32(0);
					for (int j = 1; j < array3.Length; j++)
					{
						this.m_MagicIds.Add(array3[1].SafeToInt32(0));
					}
				}
			}
			this.m_PassiveEffectIds = new List<int>();
			string stringArrtibute7 = XMLHelper.GetStringArrtibute(xe, "PassiveEffect");
			string[] array4 = stringArrtibute7.Split(new char[]
			{
				','
			});
			if (array4.Length < 2)
			{
				this.m_PassiveEffectNum = 0;
			}
			else
			{
				this.m_PassiveEffectNum = array4[0].SafeToInt32(0);
				for (int k = 1; k < array4.Length; k++)
				{
					this.m_PassiveEffectIds.Add(array4[1].SafeToInt32(0));
				}
			}
		}

		public int ID
		{
			get
			{
				return this.m_ID;
			}
			set
			{
				this.m_ID = value;
			}
		}

		public int Type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
			}
		}

		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public string Icon
		{
			get
			{
				return this.m_Icon;
			}
			set
			{
				this.m_Icon = value;
			}
		}

		public string SpecialEffect
		{
			get
			{
				return this.m_SpecialEffect;
			}
			set
			{
				this.m_SpecialEffect = value;
			}
		}

		public List<int> AwakenIDs
		{
			get
			{
				return this.m_AwakenIDs;
			}
			set
			{
				this.m_AwakenIDs = value;
			}
		}

		public int TaoZhuangProps1Num
		{
			get
			{
				return this.m_TaoZhuangProps1Num;
			}
			set
			{
				this.m_TaoZhuangProps1Num = value;
			}
		}

		public List<MUPropInfo> TaoZhuangProps1
		{
			get
			{
				return this.m_TaoZhuangProps1;
			}
			set
			{
				this.m_TaoZhuangProps1 = value;
			}
		}

		public int TaoZhuangProps2Num
		{
			get
			{
				return this.m_TaoZhuangProps2Num;
			}
			set
			{
				this.m_TaoZhuangProps2Num = value;
			}
		}

		public List<MUPropInfo> TaoZhuangProps2
		{
			get
			{
				return this.m_TaoZhuangProps2;
			}
			set
			{
				this.m_TaoZhuangProps2 = value;
			}
		}

		public int TaoZhuangProps3Num
		{
			get
			{
				return this.m_TaoZhuangProps3Num;
			}
			set
			{
				this.m_TaoZhuangProps3Num = value;
			}
		}

		public List<MUPropInfo> TaoZhuangProps3
		{
			get
			{
				return this.m_TaoZhuangProps3;
			}
			set
			{
				this.m_TaoZhuangProps3 = value;
			}
		}

		public int WeaponMasterIndex
		{
			get
			{
				return this.m_WeaponMasterIndex;
			}
			set
			{
				this.m_WeaponMasterIndex = value;
			}
		}

		public int WeaponMasterType
		{
			get
			{
				return this.m_WeaponMasterType;
			}
			set
			{
				this.m_WeaponMasterType = value;
			}
		}

		public int MagicEffectNum
		{
			get
			{
				return this.m_MagicEffectNum;
			}
			set
			{
				this.m_MagicEffectNum = value;
			}
		}

		public List<int> MagicId
		{
			get
			{
				return this.m_MagicIds;
			}
			set
			{
				this.m_MagicIds = value;
			}
		}

		public int PassiveEffectNum
		{
			get
			{
				return this.m_PassiveEffectNum;
			}
			set
			{
				this.m_PassiveEffectNum = value;
			}
		}

		public List<int> PassiveEffectIds
		{
			get
			{
				return this.m_PassiveEffectIds;
			}
			set
			{
				this.m_PassiveEffectIds = value;
			}
		}

		public int GetWeaponMasterNeedJiHuoNum()
		{
			return this.m_WeaponMasterIndex;
		}

		private int m_ID;

		private int m_Type;

		private string m_Name;

		private string m_Icon;

		private string m_SpecialEffect;

		private List<int> m_AwakenIDs;

		private int m_TaoZhuangProps1Num;

		private List<MUPropInfo> m_TaoZhuangProps1;

		private int m_TaoZhuangProps2Num;

		private List<MUPropInfo> m_TaoZhuangProps2;

		private int m_TaoZhuangProps3Num;

		private List<MUPropInfo> m_TaoZhuangProps3;

		private int m_WeaponMasterIndex;

		private int m_WeaponMasterType;

		private int m_MagicEffectNum;

		private List<int> m_MagicIds;

		private int m_PassiveEffectNum;

		private List<int> m_PassiveEffectIds;
	}
}
