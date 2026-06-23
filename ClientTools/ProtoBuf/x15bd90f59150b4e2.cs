using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	internal sealed class x15bd90f59150b4e2
	{
		private xd4dab911626dd004 x06ca69422bbb7502
		{
			get
			{
				if (this.x8b36ba1bbfb9de86 == null)
				{
					this.x8b36ba1bbfb9de86 = new xd4dab911626dd004();
				}
				return this.x8b36ba1bbfb9de86;
			}
		}

		internal object x7c483588b4d2e948(int xba08ce632055a1d9)
		{
			if (xba08ce632055a1d9-- != 0)
			{
				x826e0336b5da6af5 x06ca69422bbb = this.x06ca69422bbb7502;
				if ((uint)xba08ce632055a1d9 <= 4294967295U)
				{
					if (!false)
					{
						if (xba08ce632055a1d9 >= 0 && xba08ce632055a1d9 < x06ca69422bbb.xd44988f225497f3a)
						{
							object obj = x06ca69422bbb.get_xe6d4b1b411ed94b5(xba08ce632055a1d9);
							if (obj == null)
							{
								goto IL_47;
							}
							if (!false)
							{
								return obj;
							}
						}
						throw new ProtoException("Internal error; a missing key occurred");
					}
					goto IL_82;
				}
				IL_47:
				throw new ProtoException("A deferred key does not have a value yet");
			}
			if (this.x0abec3718b036f41 != null)
			{
				return this.x0abec3718b036f41;
			}
			IL_82:
			throw new ProtoException("No root object assigned");
		}

		internal void x24ab034b13a4cf87(int xba08ce632055a1d9, object xbcea506a33cf9111)
		{
			if (xba08ce632055a1d9-- == 0)
			{
				if (4 != 0)
				{
					if (xbcea506a33cf9111 == null)
					{
						goto IL_100;
					}
				}
				if (this.x0abec3718b036f41 == null)
				{
					if (false)
					{
						goto IL_100;
					}
				}
				else
				{
					if (this.x0abec3718b036f41 != xbcea506a33cf9111)
					{
						throw new ProtoException("The root object cannot be reassigned");
					}
					if (-2147483648 == 0)
					{
						goto IL_17;
					}
					if (-1 != 0 && (uint)xba08ce632055a1d9 - (uint)xba08ce632055a1d9 < 0U)
					{
						goto IL_60;
					}
				}
				this.x0abec3718b036f41 = xbcea506a33cf9111;
				if (4 == 0)
				{
					return;
				}
				return;
				IL_100:
				throw new ArgumentNullException("value");
			}
			xd4dab911626dd004 x06ca69422bbb = this.x06ca69422bbb7502;
			goto IL_60;
			IL_17:
			throw new ProtoException("Internal error; a key mismatch occurred");
			IL_60:
			if (xba08ce632055a1d9 >= x06ca69422bbb.xd44988f225497f3a)
			{
				if (xba08ce632055a1d9 != x06ca69422bbb.xd6b6ed77479ef68c(xbcea506a33cf9111))
				{
					goto IL_17;
				}
			}
			else
			{
				object obj = x06ca69422bbb.get_xe6d4b1b411ed94b5(xba08ce632055a1d9);
				if (obj == null)
				{
					x06ca69422bbb.set_xe6d4b1b411ed94b5(xba08ce632055a1d9, xbcea506a33cf9111);
					return;
				}
				if (!object.ReferenceEquals(obj, xbcea506a33cf9111))
				{
					throw new ProtoException("Reference-tracked objects cannot change reference");
				}
			}
		}

		internal int xa711886cf1a666de(object xbcea506a33cf9111, out bool x24aa3dd0a7e54d61)
		{
			if (xbcea506a33cf9111 == null)
			{
				throw new ArgumentNullException("value");
			}
			if (xbcea506a33cf9111 != this.x0abec3718b036f41)
			{
				string text = xbcea506a33cf9111 as string;
				x826e0336b5da6af5 x06ca69422bbb = this.x06ca69422bbb7502;
				bool flag2;
				int num;
				if (text != null)
				{
					bool flag = ((flag2 ? 1U : 0U) & 0U) == 0U;
					if (!flag)
					{
						goto IL_B8;
					}
					flag = ((uint)num + (flag2 ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						goto IL_172;
					}
					if (!false)
					{
						if ((flag2 ? 1U : 0U) + (uint)num <= 4294967295U)
						{
							goto IL_F8;
						}
						goto IL_184;
					}
				}
				else
				{
					if (this.xf113a91eac3b2cd1 == null)
					{
						goto IL_172;
					}
					if (!this.xf113a91eac3b2cd1.TryGetValue(xbcea506a33cf9111, out num))
					{
						num = -1;
						goto IL_139;
					}
					if (!false)
					{
						goto IL_7F;
					}
					goto IL_A6;
				}
				IL_43:
				num = x06ca69422bbb.xd6b6ed77479ef68c(xbcea506a33cf9111);
				if (text == null)
				{
					this.xf113a91eac3b2cd1.Add(xbcea506a33cf9111, num);
					bool flag = (flag2 ? 1U : 0U) + (flag2 ? 1U : 0U) > uint.MaxValue;
					if (!flag)
					{
						if (2147483647 == 0)
						{
							goto IL_F8;
						}
					}
				}
				else
				{
					this.xcf88094436c91137.Add(text, num);
				}
				IL_73:
				goto IL_1D1;
				IL_7F:
				flag2 = (x24aa3dd0a7e54d61 = (num >= 0));
				if (!false)
				{
					bool flag = (flag2 ? 1U : 0U) + (flag2 ? 1U : 0U) < 0U;
					if (flag)
					{
						goto IL_1A4;
					}
					if (!flag2 || ((uint)num & 0U) != 0U)
					{
						goto IL_43;
					}
					goto IL_73;
				}
				IL_A6:
				if (this.xcf88094436c91137.TryGetValue(text, out num))
				{
					goto IL_7F;
				}
				num = -1;
				if ((uint)num + (uint)num <= 4294967295U)
				{
					goto IL_7F;
				}
				goto IL_139;
				IL_B8:
				goto IL_A6;
				IL_F8:
				if (this.xcf88094436c91137 != null)
				{
					goto IL_A6;
				}
				this.xcf88094436c91137 = new Dictionary<string, int>();
				num = -1;
				IL_139:
				goto IL_7F;
				IL_172:
				this.xf113a91eac3b2cd1 = new Dictionary<object, int>(x15bd90f59150b4e2.x0bc52a69d578b1b2.xb9715d2f06b63cf0);
				num = -1;
				IL_184:
				goto IL_7F;
				IL_1A4:
				goto IL_43;
				IL_1D1:
				return num + 1;
			}
			x24aa3dd0a7e54d61 = true;
			return 0;
		}

		internal void xa335b28bb552e966(object xbcea506a33cf9111)
		{
			if (this.x0abec3718b036f41 != null)
			{
				int num;
				for (;;)
				{
					if (this.x8b36ba1bbfb9de86 == null)
					{
						if (false)
						{
							goto IL_24;
						}
						break;
					}
					else
					{
						num = this.x55302249462a830e;
					}
					IL_4B:
					if (num >= this.x8b36ba1bbfb9de86.xd44988f225497f3a)
					{
						if (!false)
						{
							break;
						}
						continue;
					}
					else
					{
						this.x55302249462a830e = num + 1;
						if (false)
						{
							continue;
						}
						if (3 == 0)
						{
							break;
						}
					}
					IL_24:
					if (-1 == 0)
					{
						return;
					}
					if (this.x8b36ba1bbfb9de86.get_xe6d4b1b411ed94b5(num) == null)
					{
						goto Block_5;
					}
					num++;
					goto IL_4B;
				}
				return;
				Block_5:
				this.x8b36ba1bbfb9de86.set_xe6d4b1b411ed94b5(num, xbcea506a33cf9111);
				return;
			}
			this.x0abec3718b036f41 = xbcea506a33cf9111;
		}

		internal const int x29e7ace4c90f74cd = 0;

		private xd4dab911626dd004 x8b36ba1bbfb9de86;

		private object x0abec3718b036f41;

		private int x55302249462a830e;

		private Dictionary<string, int> xcf88094436c91137;

		private Dictionary<object, int> xf113a91eac3b2cd1;

		private sealed class x0bc52a69d578b1b2 : IEqualityComparer<object>
		{
			private x0bc52a69d578b1b2()
			{
			}

			bool IEqualityComparer<object>.xc44858ebde896d74(object x08db3aeabb253cb1, object x1e218ceaee1bb583)
			{
				return x08db3aeabb253cb1 == x1e218ceaee1bb583;
			}

			int IEqualityComparer<object>.x25d0fe846d727308(object xa59bff7708de3a18)
			{
				return RuntimeHelpers.GetHashCode(xa59bff7708de3a18);
			}

			public static readonly x15bd90f59150b4e2.x0bc52a69d578b1b2 xb9715d2f06b63cf0 = new x15bd90f59150b4e2.x0bc52a69d578b1b2();
		}
	}
}
