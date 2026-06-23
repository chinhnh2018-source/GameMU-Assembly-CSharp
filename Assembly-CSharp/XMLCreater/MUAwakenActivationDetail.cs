using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAwakenActivationDetail
	{
		public MUAwakenActivationDetail()
		{
		}

		public MUAwakenActivationDetail(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_Position = XMLHelper.GetIntArrtibute(xe, "Position");
			this.m_Icon = XMLHelper.GetStringArrtibute(xe, "Icon");
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, "Material");
			this.m_Material = new MUMaterialInfo(stringArrtibute);
			string stringArrtibute2 = XMLHelper.GetStringArrtibute(xe, "BaseProps");
			this.m_BaseProps = MUPropInfoHelper.DeserializeToPropList(stringArrtibute2);
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

		public int Position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
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

		public MUMaterialInfo Material
		{
			get
			{
				return this.m_Material;
			}
			set
			{
				this.m_Material = value;
			}
		}

		public List<MUPropInfo> BaseProps
		{
			get
			{
				return this.m_BaseProps;
			}
			set
			{
				this.m_BaseProps = value;
			}
		}

		private int m_ID;

		private string m_Name;

		private int m_Position;

		private string m_Icon;

		private MUMaterialInfo m_Material;

		private List<MUPropInfo> m_BaseProps;
	}
}
