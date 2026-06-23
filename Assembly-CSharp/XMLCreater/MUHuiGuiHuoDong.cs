using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUHuiGuiHuoDong
	{
		public MUHuiGuiHuoDong()
		{
		}

		public MUHuiGuiHuoDong(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_HuoDongLevel = XMLHelper.GetIntArrtibute(xe, "HuoDongLevel");
			this.m_BeginTime = XMLHelper.GetStringArrtibute(xe, "BeginTime");
			this.m_FinishTime = XMLHelper.GetStringArrtibute(xe, "FinishTime");
			this.m_RegisterBegin = XMLHelper.GetStringArrtibute(xe, "RegisterBegin");
			this.m_RegisterFinish = XMLHelper.GetStringArrtibute(xe, "RegisterFinish");
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

		public int HuoDongLevel
		{
			get
			{
				return this.m_HuoDongLevel;
			}
			set
			{
				this.m_HuoDongLevel = value;
			}
		}

		public string BeginTime
		{
			get
			{
				return this.m_BeginTime;
			}
			set
			{
				this.m_BeginTime = value;
			}
		}

		public string FinishTime
		{
			get
			{
				return this.m_FinishTime;
			}
			set
			{
				this.m_FinishTime = value;
			}
		}

		public string RegisterBegin
		{
			get
			{
				return this.m_RegisterBegin;
			}
			set
			{
				this.m_RegisterBegin = value;
			}
		}

		public string RegisterFinish
		{
			get
			{
				return this.m_RegisterFinish;
			}
			set
			{
				this.m_RegisterFinish = value;
			}
		}

		private int m_ID;

		private int m_HuoDongLevel;

		private string m_BeginTime;

		private string m_FinishTime;

		private string m_RegisterBegin;

		private string m_RegisterFinish;
	}
}
