using System;
using System.Collections.Generic;

namespace UniLua
{
	public struct Pointer<T>
	{
		public Pointer(List<T> list, int index)
		{
			this.List = list;
			this.Index = index;
		}

		public Pointer(Pointer<T> other)
		{
			this.List = other.List;
			this.Index = other.Index;
		}

		public int Index { get; set; }

		public T Value
		{
			get
			{
				return this.List[this.Index];
			}
			set
			{
				this.List[this.Index] = value;
			}
		}

		public T ValueInc
		{
			get
			{
				return this.List[this.Index++];
			}
			set
			{
				this.List[this.Index++] = value;
			}
		}

		public static Pointer<T>operator +(Pointer<T> lhs, int rhs)
		{
			return new Pointer<T>(lhs.List, lhs.Index + rhs);
		}

		public static Pointer<T>operator -(Pointer<T> lhs, int rhs)
		{
			return new Pointer<T>(lhs.List, lhs.Index - rhs);
		}

		private List<T> List;
	}
}
