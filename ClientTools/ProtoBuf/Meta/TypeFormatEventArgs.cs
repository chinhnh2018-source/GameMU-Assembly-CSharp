using System;

namespace ProtoBuf.Meta
{
	public class TypeFormatEventArgs : EventArgs
	{
		public Type Type
		{
			get
			{
				return this.x43163d22e8cd5a71;
			}
			set
			{
				if (this.x43163d22e8cd5a71 != value)
				{
					if (this.xa5dd5d97e988cf10)
					{
						throw new InvalidOperationException("The type is fixed and cannot be changed");
					}
					this.x43163d22e8cd5a71 = value;
				}
			}
		}

		public string FormattedName
		{
			get
			{
				return this.xe65670886e2b460f;
			}
			set
			{
				if (this.xe65670886e2b460f != value)
				{
					while (this.xa5dd5d97e988cf10)
					{
						if (8 != 0)
						{
							this.xe65670886e2b460f = value;
							return;
						}
					}
					throw new InvalidOperationException("The formatted-name is fixed and cannot be changed");
				}
			}
		}

		internal TypeFormatEventArgs(string formattedName)
		{
			if (true)
			{
				goto IL_17;
			}
			IL_0D:
			this.xa5dd5d97e988cf10 = false;
			if (!false)
			{
				return;
			}
			IL_17:
			if (false || x479f2661aae93792.x1c140bd1078ddda1(formattedName))
			{
				throw new ArgumentNullException("formattedName");
			}
			this.xe65670886e2b460f = formattedName;
			if (3 != 0)
			{
				goto IL_0D;
			}
		}

		internal TypeFormatEventArgs(Type type)
		{
			if (255 != 0)
			{
				if (type == null)
				{
					throw new ArgumentNullException("type");
				}
				this.x43163d22e8cd5a71 = type;
			}
			this.xa5dd5d97e988cf10 = true;
		}

		private Type x43163d22e8cd5a71;

		private string xe65670886e2b460f;

		private readonly bool xa5dd5d97e988cf10;
	}
}
