using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAllAwakenRecoverys
	{
		public MUAllAwakenRecoverys()
		{
		}

		public MUAllAwakenRecoverys(XElement xe)
		{
			this.m_AwakenRecoverys = new List<MUAwakenRecovery>();
			IEnumerable<XElement> enumerable = xe.Elements("AwakenRecovery");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUAwakenRecovery muawakenRecovery = new MUAwakenRecovery(xe2);
					this.m_AwakenRecoverys.Add(muawakenRecovery);
				}
			}
		}

		public List<MUAwakenRecovery> AwakenRecoverys
		{
			get
			{
				return this.m_AwakenRecoverys;
			}
			set
			{
				this.m_AwakenRecoverys = value;
			}
		}

		public MUAwakenRecovery GetAwakenRecoveryByGoodsID(int goodsId)
		{
			if (this.m_AwakenRecoverys == null)
			{
				return null;
			}
			return this.m_AwakenRecoverys.Find((MUAwakenRecovery info) => info.GoodsID == goodsId);
		}

		public int GetAwakenNumByGoodsID(int goodsId)
		{
			MUAwakenRecovery awakenRecoveryByGoodsID = this.GetAwakenRecoveryByGoodsID(goodsId);
			if (awakenRecoveryByGoodsID == null)
			{
				return 0;
			}
			return awakenRecoveryByGoodsID.AwakenNum;
		}

		private List<MUAwakenRecovery> m_AwakenRecoverys;
	}
}
