using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUWeaponMasterInfo
	{
		public MUWeaponMasterInfo()
		{
		}

		public MUWeaponMasterInfo(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Type = XMLHelper.GetIntArrtibute(xe, "Type");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, "WeaponType1");
			this.m_WeaponType1 = this.StrToList(stringArrtibute);
			string stringArrtibute2 = XMLHelper.GetStringArrtibute(xe, "WeaponType2");
			this.m_WeaponType2 = this.StrToList(stringArrtibute2);
			string stringArrtibute3 = XMLHelper.GetStringArrtibute(xe, "WeaponMasterProps");
			this.m_WeaponMasterProps = MUPropInfoHelper.DeserializeToPropList(stringArrtibute3);
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

		public List<int> WeaponType1
		{
			get
			{
				return this.m_WeaponType1;
			}
			set
			{
				this.m_WeaponType1 = value;
			}
		}

		public List<int> WeaponType2
		{
			get
			{
				return this.m_WeaponType2;
			}
			set
			{
				this.m_WeaponType2 = value;
			}
		}

		public List<MUPropInfo> WeaponMasterProps
		{
			get
			{
				return this.m_WeaponMasterProps;
			}
			set
			{
				this.m_WeaponMasterProps = value;
			}
		}

		private List<int> StrToList(string str)
		{
			List<int> list = new List<int>();
			string[] array = str.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				int num = array[i].SafeToInt32(0);
				if (num != -1)
				{
					list.Add(num);
				}
			}
			return list;
		}

		private int m_ID;

		private int m_Type;

		private string m_Name;

		private List<int> m_WeaponType1;

		private List<int> m_WeaponType2;

		private List<MUPropInfo> m_WeaponMasterProps;
	}
}
