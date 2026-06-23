using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAwakenActivationInfos
	{
		public MUAwakenActivationInfos()
		{
		}

		public MUAwakenActivationInfos(XElement xe)
		{
			this.m_EnchantmentSuits = new List<MUAwakenActivationDetail>();
			IEnumerable<XElement> enumerable = xe.Elements("AwakenActivation");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUAwakenActivationDetail muawakenActivationDetail = new MUAwakenActivationDetail(xe2);
					this.m_EnchantmentSuits.Add(muawakenActivationDetail);
				}
			}
		}

		public List<MUAwakenActivationDetail> EnchantmentSuits
		{
			get
			{
				return this.m_EnchantmentSuits;
			}
			set
			{
				this.m_EnchantmentSuits = value;
			}
		}

		public MUAwakenActivationDetail GetAwakenActivationDetailByID(int id)
		{
			if (this.m_EnchantmentSuits == null)
			{
				return null;
			}
			return this.m_EnchantmentSuits.Find((MUAwakenActivationDetail info) => info.ID == id);
		}

		private List<MUAwakenActivationDetail> m_EnchantmentSuits;
	}
}
