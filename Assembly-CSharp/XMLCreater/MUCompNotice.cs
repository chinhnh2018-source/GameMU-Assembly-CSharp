using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUCompNotice
	{
		public MUCompNotice()
		{
		}

		public MUCompNotice(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Intro = XMLHelper.GetStringArrtibute(xe, "Intro");
			this.m_Type = XMLHelper.GetIntArrtibute(xe, "Type");
			this.m_Goal = XMLHelper.GetIntArrtibute(xe, "Goal");
			this.m_CDTime = XMLHelper.GetIntArrtibute(xe, "CDTime");
			this.m_Range = XMLHelper.GetIntArrtibute(xe, "Range");
			this.m_CompMapOpen = XMLHelper.GetStringArrtibute(xe, "CompMapOpen");
			this.m_beMapOpen = new List<bool>();
			string[] array = this.m_CompMapOpen.Split(new char[]
			{
				'|'
			});
			if (array.Length >= 3)
			{
				for (int i = 0; i < array.Length; i++)
				{
					this.m_beMapOpen.Add(array[i].SafeToInt32(0) == 1);
				}
			}
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, "OriginalMapOpen");
			this.m_OriginalMapOpen = new List<bool>();
			string[] array2 = stringArrtibute.Split(new char[]
			{
				'|'
			});
			if (array2.Length >= 2)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					this.m_OriginalMapOpen.Add(array2[j].SafeToInt32(0) == 1);
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

		public string Intro
		{
			get
			{
				return this.m_Intro;
			}
			set
			{
				this.m_Intro = value;
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

		public int Goal
		{
			get
			{
				return this.m_Goal;
			}
			set
			{
				this.m_Goal = value;
			}
		}

		public int CDTime
		{
			get
			{
				return this.m_CDTime;
			}
			set
			{
				this.m_CDTime = value;
			}
		}

		public int Range
		{
			get
			{
				return this.m_Range;
			}
			set
			{
				this.m_Range = value;
			}
		}

		public string CompMapOpen
		{
			get
			{
				return this.m_CompMapOpen;
			}
			set
			{
				this.m_CompMapOpen = value;
			}
		}

		private List<bool> BeMapOpen
		{
			get
			{
				return this.m_beMapOpen;
			}
			set
			{
				this.m_beMapOpen = value;
			}
		}

		public List<bool> OriginalMapOpen
		{
			get
			{
				return this.m_OriginalMapOpen;
			}
			set
			{
				this.m_OriginalMapOpen = value;
			}
		}

		public bool IsCompOpen(NoticeShowType type)
		{
			return this.BeMapOpen[(int)type];
		}

		public bool IsOriginalMapOpen(NoticeShowType type)
		{
			if (type == NoticeShowType.TopMessage)
			{
				return this.OriginalMapOpen[0];
			}
			return type == NoticeShowType.ChatMessage && this.OriginalMapOpen[1];
		}

		private int m_ID;

		private string m_Intro;

		private int m_Type;

		private int m_Goal;

		private int m_CDTime;

		private int m_Range;

		private string m_CompMapOpen;

		private List<bool> m_beMapOpen;

		private List<bool> m_OriginalMapOpen;
	}
}
