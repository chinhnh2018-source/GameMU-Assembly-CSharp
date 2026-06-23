using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MURebornCombatForceAll
	{
		public MURebornCombatForceAll()
		{
		}

		public MURebornCombatForceAll(XElement xe)
		{
			IEnumerable<XElement> enumerable = xe.Elements("Rebornzhanli");
			if (enumerable != null)
			{
				IEnumerator<XElement> enumerator = enumerable.GetEnumerator();
				if (enumerator.MoveNext())
				{
					this.m_Rebornzhanli = new MURebornCombatForce(enumerator.Current);
				}
			}
		}

		public MURebornCombatForce Rebornzhanli
		{
			get
			{
				return this.m_Rebornzhanli;
			}
			set
			{
				this.m_Rebornzhanli = value;
			}
		}

		private MURebornCombatForce m_Rebornzhanli;
	}
}
