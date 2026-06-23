using System;
using System.Collections;
using System.Reflection;

namespace ProtoBuf.Meta
{
	[DefaultMember("Item")]
	internal class x826e0336b5da6af5 : IEnumerable
	{
		public void x0fe4f26e70030075(Array x9d5750eb2d6373bc, int x374ea4fe62468d0f)
		{
			this.x4cbb0cc714782977.x0fe4f26e70030075(x9d5750eb2d6373bc, x374ea4fe62468d0f);
		}

		public int xd6b6ed77479ef68c(object xbcea506a33cf9111)
		{
			return (this.x4cbb0cc714782977 = this.x4cbb0cc714782977.Append(xbcea506a33cf9111)).Length - 1;
		}

		public object xe6d4b1b411ed94b5
		{
			get
			{
				return this.x4cbb0cc714782977[xc0c4c459c6ccbd00];
			}
		}

		public object x034113016eff150a(int xc0c4c459c6ccbd00)
		{
			return this.x4cbb0cc714782977.TryGet(xc0c4c459c6ccbd00);
		}

		public void x5749d92c9865c1ba()
		{
			this.x4cbb0cc714782977 = this.x4cbb0cc714782977.Trim();
		}

		public int xd44988f225497f3a
		{
			get
			{
				return this.x4cbb0cc714782977.Length;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return new x826e0336b5da6af5.xb56968f92e308c8a(this.x4cbb0cc714782977);
		}

		internal int x2ee5ad3d826ed0fe(x826e0336b5da6af5.xd9fd150330ba5bb7 x0a7c6450dfaa2f9a)
		{
			return this.x4cbb0cc714782977.x2ee5ad3d826ed0fe(x0a7c6450dfaa2f9a);
		}

		internal int x13c79e2766d7a14b(object x6ed4ed9ed59eb694)
		{
			return this.x4cbb0cc714782977.x13c79e2766d7a14b(x6ed4ed9ed59eb694);
		}

		internal bool x263d579af1d0d43f(object xbcea506a33cf9111)
		{
			using (IEnumerator enumerator = this.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					bool result;
					for (;;)
					{
						object objA = enumerator.Current;
						if (!object.Equals(objA, xbcea506a33cf9111))
						{
							break;
						}
						result = true;
						if (2147483647 != 0)
						{
							goto Block_4;
						}
					}
					continue;
					Block_4:
					return result;
				}
			}
			return false;
		}

		internal static x826e0336b5da6af5 x1a26f70f68cf5f10(int[] x83f3ea1d0a03c7e1, object[] x0788cd5a9865fc16)
		{
			if (x83f3ea1d0a03c7e1 != null)
			{
				if (x0788cd5a9865fc16 == null)
				{
					throw new ArgumentNullException("values");
				}
				IL_C4:
				while (x0788cd5a9865fc16.Length >= x83f3ea1d0a03c7e1.Length)
				{
					x826e0336b5da6af5 x826e0336b5da6af = new x826e0336b5da6af5();
					x826e0336b5da6af5.x20422d9e11f237e1 x20422d9e11f237e = null;
					int i;
					do
					{
						i = 0;
						if (false)
						{
							goto IL_C4;
						}
						IL_0F:
						while (i < x83f3ea1d0a03c7e1.Length)
						{
							for (;;)
							{
								if (i != 0)
								{
									goto IL_6D;
								}
								goto IL_80;
								IL_77:
								if (x20422d9e11f237e == null)
								{
									x20422d9e11f237e = new x826e0336b5da6af5.x20422d9e11f237e1(x83f3ea1d0a03c7e1[i]);
									x826e0336b5da6af.xd6b6ed77479ef68c(x20422d9e11f237e);
									if (4 == 0)
									{
										goto IL_0F;
									}
									if (2147483647 == 0)
									{
										continue;
									}
								}
								x20422d9e11f237e.xe0d5f9fb50308841.xd6b6ed77479ef68c(x0788cd5a9865fc16[i]);
								if (true)
								{
									break;
								}
								IL_6D:
								if (x83f3ea1d0a03c7e1[i] == x83f3ea1d0a03c7e1[i - 1])
								{
									goto IL_77;
								}
								IL_80:
								x20422d9e11f237e = null;
								goto IL_77;
							}
							bool flag = (uint)i + (uint)i < 0U;
							if (flag)
							{
								goto IL_A4;
							}
							i++;
						}
					}
					while ((uint)i - (uint)i > 4294967295U);
					IL_A4:
					if (!true)
					{
						goto IL_E8;
					}
					return x826e0336b5da6af;
				}
				throw new ArgumentException("Not all keys are covered by values", "values");
			}
			IL_E8:
			throw new ArgumentNullException("keys");
		}

		private static readonly x826e0336b5da6af5.Node x46115cc68b86d3d8 = new x826e0336b5da6af5.Node(null, 0);

		protected x826e0336b5da6af5.Node x4cbb0cc714782977 = x826e0336b5da6af5.x46115cc68b86d3d8;

		protected sealed class Node
		{
			public object this[int index]
			{
				get
				{
					if (index < 0 || index >= this.x961016a387451f05)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return this.x4a3f0a05c02f235f[index];
				}
				set
				{
					if (index < 0 || index >= this.x961016a387451f05)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					this.x4a3f0a05c02f235f[index] = value;
				}
			}

			public object TryGet(int index)
			{
				if (index >= 0 && index < this.x961016a387451f05)
				{
					return this.x4a3f0a05c02f235f[index];
				}
				return null;
			}

			public int Length
			{
				get
				{
					return this.x961016a387451f05;
				}
			}

			internal Node(object[] data, int length)
			{
				this.x4a3f0a05c02f235f = data;
				this.x961016a387451f05 = length;
			}

			public void RemoveLastWithMutate()
			{
				if (this.x961016a387451f05 == 0)
				{
					throw new InvalidOperationException();
				}
				this.x961016a387451f05--;
			}

			public x826e0336b5da6af5.Node Append(object value)
			{
				int num = this.x961016a387451f05 + 1;
				object[] array;
				if (this.x4a3f0a05c02f235f == null)
				{
					array = new object[10];
					if ((uint)num + (uint)num >= 0U)
					{
						goto IL_22;
					}
				}
				else if (this.x961016a387451f05 == this.x4a3f0a05c02f235f.Length)
				{
					array = new object[this.x4a3f0a05c02f235f.Length * 2];
					Array.Copy(this.x4a3f0a05c02f235f, array, this.x961016a387451f05);
					goto IL_12;
				}
				array = this.x4a3f0a05c02f235f;
				IL_12:
				array[this.x961016a387451f05] = value;
				if (2147483647 != 0)
				{
					return new x826e0336b5da6af5.Node(array, num);
				}
				IL_22:
				goto IL_12;
			}

			public x826e0336b5da6af5.Node Trim()
			{
				if (this.x961016a387451f05 == 0 || this.x961016a387451f05 == this.x4a3f0a05c02f235f.Length)
				{
					return this;
				}
				object[] array = new object[this.x961016a387451f05];
				Array.Copy(this.x4a3f0a05c02f235f, array, this.x961016a387451f05);
				return new x826e0336b5da6af5.Node(array, this.x961016a387451f05);
			}

			internal int x13c79e2766d7a14b(object x6ed4ed9ed59eb694)
			{
				for (int i = 0; i < this.x961016a387451f05; i++)
				{
					if (x6ed4ed9ed59eb694 == this.x4a3f0a05c02f235f[i])
					{
						return i;
					}
				}
				return -1;
			}

			internal int x2ee5ad3d826ed0fe(x826e0336b5da6af5.xd9fd150330ba5bb7 x0a7c6450dfaa2f9a)
			{
				int i = 0;
				for (;;)
				{
					while (i < this.x961016a387451f05)
					{
						if (x0a7c6450dfaa2f9a.xc313ef0c9ca8870d(this.x4a3f0a05c02f235f[i]))
						{
							return i;
						}
						i++;
					}
					bool flag = (uint)i - (uint)i > uint.MaxValue;
					if (!flag)
					{
						return -1;
					}
				}
				return i;
			}

			internal void x0fe4f26e70030075(Array x9d5750eb2d6373bc, int x374ea4fe62468d0f)
			{
				if (this.x961016a387451f05 > 0)
				{
					Array.Copy(this.x4a3f0a05c02f235f, 0, x9d5750eb2d6373bc, x374ea4fe62468d0f, this.x961016a387451f05);
				}
			}

			private readonly object[] x4a3f0a05c02f235f;

			private int x961016a387451f05;
		}

		internal interface xd9fd150330ba5bb7
		{
			bool xc313ef0c9ca8870d(object xa59bff7708de3a18);
		}

		private sealed class xb56968f92e308c8a : IEnumerator
		{
			public xb56968f92e308c8a(x826e0336b5da6af5.Node node)
			{
				this.xda5bf54deb817e37 = node;
			}

			void IEnumerator.xf80999337bada71a()
			{
				this.x13d4cb8d1bd20347 = -1;
			}

			public object Current
			{
				get
				{
					return this.xda5bf54deb817e37[this.x13d4cb8d1bd20347];
				}
			}

			public bool MoveNext()
			{
				int length = this.xda5bf54deb817e37.Length;
				return this.x13d4cb8d1bd20347 <= length && ++this.x13d4cb8d1bd20347 < length;
			}

			private int x13d4cb8d1bd20347 = -1;

			private readonly x826e0336b5da6af5.Node xda5bf54deb817e37;
		}

		internal class x20422d9e11f237e1
		{
			public x20422d9e11f237e1(int first)
			{
				this.x38ced5a01a389303 = first;
				this.xe0d5f9fb50308841 = new x826e0336b5da6af5();
			}

			public readonly int x38ced5a01a389303;

			public readonly x826e0336b5da6af5 xe0d5f9fb50308841;
		}
	}
}
