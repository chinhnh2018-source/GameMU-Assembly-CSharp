using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUTransfigurationFashionEffectAll
	{
		public MUTransfigurationFashionEffectAll()
		{
		}

		public MUTransfigurationFashionEffectAll(XElement xe)
		{
			this.m_TransfigurationFashionEffects = new List<MUTransfigurationFashionEffect>();
			IEnumerable<XElement> enumerable = xe.Elements("TransfigurationFashionEffect");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUTransfigurationFashionEffect mutransfigurationFashionEffect = new MUTransfigurationFashionEffect(xe2);
					this.m_TransfigurationFashionEffects.Add(mutransfigurationFashionEffect);
				}
			}
		}

		public List<MUTransfigurationFashionEffect> TransfigurationFashionEffects
		{
			get
			{
				return this.m_TransfigurationFashionEffects;
			}
			set
			{
				this.m_TransfigurationFashionEffects = value;
			}
		}

		public MUTransfigurationFashionEffect GetTransfigurationFashionEffectByID(int id)
		{
			if (this.m_TransfigurationFashionEffects == null)
			{
				return null;
			}
			return this.m_TransfigurationFashionEffects.Find((MUTransfigurationFashionEffect info) => info.ID == id);
		}

		private List<MUTransfigurationFashionEffect> m_TransfigurationFashionEffects;
	}
}
