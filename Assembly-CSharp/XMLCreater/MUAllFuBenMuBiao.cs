using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAllFuBenMuBiao
	{
		public MUAllFuBenMuBiao()
		{
		}

		public MUAllFuBenMuBiao(XElement xe)
		{
			this.m_FuBenMuBiaos = new List<MUFuBenMuBiao>();
			IEnumerable<XElement> enumerable = xe.Elements("FuBenMuBiao");
			if (enumerable != null)
			{
				foreach (XElement xelement in enumerable)
				{
					IEnumerable<XElement> enumerable2 = xelement.Elements("MuBiao");
					if (enumerable2 != null)
					{
						foreach (XElement xe2 in enumerable2)
						{
							MUFuBenMuBiao mufuBenMuBiao = new MUFuBenMuBiao(xe2);
							this.m_FuBenMuBiaos.Add(mufuBenMuBiao);
						}
					}
				}
			}
		}

		public List<MUFuBenMuBiao> FuBenMuBiaos
		{
			get
			{
				return this.m_FuBenMuBiaos;
			}
			set
			{
				this.m_FuBenMuBiaos = value;
			}
		}

		public bool BeExistFuBen(int fubenId)
		{
			if (this.m_FuBenMuBiaos == null)
			{
				return false;
			}
			MUFuBenMuBiao mufuBenMuBiao = this.m_FuBenMuBiaos.Find((MUFuBenMuBiao info) => info.FuBenID == fubenId);
			return mufuBenMuBiao != null;
		}

		public MUFuBenMuBiao GetFuBenMuBiao(int fubenId, int muBiaoID)
		{
			if (this.m_FuBenMuBiaos == null)
			{
				return null;
			}
			return this.m_FuBenMuBiaos.Find((MUFuBenMuBiao info) => info.FuBenID == fubenId && info.MuBiaoID == muBiaoID);
		}

		private List<MUFuBenMuBiao> m_FuBenMuBiaos;
	}
}
