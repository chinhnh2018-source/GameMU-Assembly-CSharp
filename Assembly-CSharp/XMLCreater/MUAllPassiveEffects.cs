using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAllPassiveEffects
	{
		public MUAllPassiveEffects()
		{
		}

		public MUAllPassiveEffects(XElement xe)
		{
			this.m_PassiveEffects = new List<MUPassiveEffect>();
			IEnumerable<XElement> enumerable = xe.Elements("PassiveEffect");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUPassiveEffect mupassiveEffect = new MUPassiveEffect(xe2);
					this.m_PassiveEffects.Add(mupassiveEffect);
				}
			}
		}

		public List<MUPassiveEffect> PassiveEffects
		{
			get
			{
				return this.m_PassiveEffects;
			}
			set
			{
				this.m_PassiveEffects = value;
			}
		}

		public MUPassiveEffect GetPassiveEffectByID(int id)
		{
			if (this.m_PassiveEffects == null)
			{
				return null;
			}
			return this.m_PassiveEffects.Find((MUPassiveEffect info) => info.ID == id);
		}

		private List<MUPassiveEffect> m_PassiveEffects;
	}
}
