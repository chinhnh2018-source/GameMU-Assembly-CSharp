using System;
using System.Text;

namespace HTMLEngine
{
	internal class PStringBuilder : PoolableObject
	{
		internal override void OnAcquire()
		{
		}

		internal override void OnRelease()
		{
			this.sb.Length = 0;
		}

		public void Append(char c)
		{
			this.sb.Append(c);
		}

		public void Append(string s)
		{
			this.sb.Append(s);
		}

		public override string ToString()
		{
			return this.sb.ToString();
		}

		public static implicit operator StringBuilder(PStringBuilder psb)
		{
			return psb.sb;
		}

		private readonly StringBuilder sb = new StringBuilder();
	}
}
