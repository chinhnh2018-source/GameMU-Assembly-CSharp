using System;
using HSGameEngine.GameEngine.Logic;

namespace XMLCreater
{
	public class MUMaterialInfo
	{
		public MUMaterialInfo(string content)
		{
			string[] array = content.Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				this.m_materialId = Global.SafeConvertToInt32(array[0]);
				this.m_num = Global.SafeConvertToInt32(array[1]);
			}
			else
			{
				this.m_materialId = 0;
				this.m_num = 0;
			}
		}

		public MUMaterialInfo(int meterialId, int num)
		{
			this.m_materialId = meterialId;
			this.m_num = num;
		}

		public int MaterialId
		{
			get
			{
				return this.m_materialId;
			}
			set
			{
				this.m_materialId = value;
			}
		}

		public int Num
		{
			get
			{
				return this.m_num;
			}
			set
			{
				this.m_num = value;
			}
		}

		private int m_materialId;

		private int m_num;
	}
}
