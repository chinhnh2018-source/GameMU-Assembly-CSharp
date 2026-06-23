using System;

namespace ProtoBuf
{
	public sealed class SerializationContext
	{
		internal void x69322180ae719ea6()
		{
			this.x04c5d583b9ff8998 = true;
		}

		private void x2eda32a9e793a6b9()
		{
			if (this.x04c5d583b9ff8998)
			{
				throw new InvalidOperationException("The serialization-context cannot be changed once it is in use");
			}
		}

		public object Context
		{
			get
			{
				return this.x0f7b23d1c393aed9;
			}
			set
			{
				if (this.x0f7b23d1c393aed9 != value)
				{
					this.x2eda32a9e793a6b9();
					this.x0f7b23d1c393aed9 = value;
				}
			}
		}

		static SerializationContext()
		{
			SerializationContext.x35a828ec58c7269b.x69322180ae719ea6();
		}

		internal static SerializationContext xb9715d2f06b63cf0
		{
			get
			{
				return SerializationContext.x35a828ec58c7269b;
			}
		}

		private bool x04c5d583b9ff8998;

		private object x0f7b23d1c393aed9;

		private static readonly SerializationContext x35a828ec58c7269b = new SerializationContext();
	}
}
