using System;

namespace ProtoBuf.Serializers
{
	internal sealed class x9caf06b8b9c3a409 : x66ec8c25e4c7547d
	{
		public x9caf06b8b9c3a409(Type enumType, x9caf06b8b9c3a409.EnumPair[] map)
		{
			if (false)
			{
				goto IL_EC;
			}
			if (enumType != null)
			{
				goto IL_1C4;
			}
			throw new ArgumentNullException("enumType");
			IL_11:
			int num;
			int num2;
			if (num < map.Length)
			{
				num2 = 0;
				goto IL_1BC;
			}
			if ((uint)num >= 0U)
			{
				return;
			}
			goto IL_1C4;
			IL_49:
			num2++;
			IL_4D:
			if (num2 >= num)
			{
				if ((uint)num2 + (uint)num2 >= 0U)
				{
					num++;
					goto IL_11;
				}
				goto IL_1BC;
			}
			else if (map[num].WireValue == map[num2].WireValue)
			{
				if (!object.Equals(map[num].RawValue, map[num2].RawValue))
				{
					throw new ProtoException("Multiple enums with wire-value " + map[num].WireValue);
				}
			}
			IL_78:
			if (!object.Equals(map[num].RawValue, map[num2].RawValue))
			{
				goto IL_49;
			}
			IL_A1:
			if (map[num].WireValue == map[num2].WireValue)
			{
				goto IL_49;
			}
			goto IL_108;
			IL_EC:
			bool flag = (uint)num > uint.MaxValue;
			if (!flag)
			{
				goto IL_A1;
			}
			if (-2147483648 == 0)
			{
				goto IL_78;
			}
			IL_108:
			throw new ProtoException("Multiple enums with deserialized-value " + map[num].RawValue);
			IL_1BC:
			goto IL_4D;
			IL_1C4:
			this.x7cbaa9e36b617ca7 = enumType;
			this.x12fedb3de1c57ea7 = map;
			flag = ((uint)num2 - (uint)num2 > uint.MaxValue);
			if (flag || map != null)
			{
				num = 1;
				goto IL_11;
			}
			if (-2 == 0)
			{
				goto IL_EC;
			}
		}

		private xd669244d58bc09c0 xf70eec89828a813c()
		{
			Type type = x479f2661aae93792.xe5e08d1dc9f521de(this.x7cbaa9e36b617ca7);
			if (-1 == 0 || type == null)
			{
				type = this.x7cbaa9e36b617ca7;
			}
			return x479f2661aae93792.xf70eec89828a813c(type);
		}

		public Type x00c54479c3a7c440
		{
			get
			{
				return this.x7cbaa9e36b617ca7;
			}
		}

		bool x66ec8c25e4c7547d.x84790f641eeacdd2
		{
			get
			{
				return false;
			}
		}

		bool x66ec8c25e4c7547d.x60089ae1c8e627b2
		{
			get
			{
				return true;
			}
		}

		private int x15cae3472b818a14(object xbcea506a33cf9111)
		{
			xd669244d58bc09c0 xd669244d58bc09c = this.xf70eec89828a813c();
			if (-1 != 0)
			{
				switch (xd669244d58bc09c)
				{
				case xd669244d58bc09c0.x2025ae83be2038f0:
					return (int)((sbyte)xbcea506a33cf9111);
				case xd669244d58bc09c0.xc0f9b651d77da240:
					return (int)((byte)xbcea506a33cf9111);
				case xd669244d58bc09c0.x697a219ddc6427a9:
					return (int)((short)xbcea506a33cf9111);
				case xd669244d58bc09c0.xf12cc4804eec7b89:
					return (int)((ushort)xbcea506a33cf9111);
				case xd669244d58bc09c0.x85254000935bfc25:
					return (int)xbcea506a33cf9111;
				case xd669244d58bc09c0.x6cedabde5251b1e5:
					return (int)((uint)xbcea506a33cf9111);
				case xd669244d58bc09c0.x0b2292ab52b25d76:
					return (int)((long)xbcea506a33cf9111);
				case xd669244d58bc09c0.x394150f1be471c3c:
					break;
				default:
					throw new InvalidOperationException();
				}
			}
			return (int)((ulong)xbcea506a33cf9111);
		}

		private object x231070506ace0521(int xbcea506a33cf9111)
		{
			xd669244d58bc09c0 xd669244d58bc09c = this.xf70eec89828a813c();
			if (!false)
			{
				switch (xd669244d58bc09c)
				{
				case xd669244d58bc09c0.x2025ae83be2038f0:
					return Enum.ToObject(this.x7cbaa9e36b617ca7, (sbyte)xbcea506a33cf9111);
				case xd669244d58bc09c0.xc0f9b651d77da240:
					return Enum.ToObject(this.x7cbaa9e36b617ca7, (byte)xbcea506a33cf9111);
				case xd669244d58bc09c0.x697a219ddc6427a9:
					return Enum.ToObject(this.x7cbaa9e36b617ca7, (short)xbcea506a33cf9111);
				case xd669244d58bc09c0.xf12cc4804eec7b89:
					break;
				case xd669244d58bc09c0.x85254000935bfc25:
					return Enum.ToObject(this.x7cbaa9e36b617ca7, xbcea506a33cf9111);
				case xd669244d58bc09c0.x6cedabde5251b1e5:
					return Enum.ToObject(this.x7cbaa9e36b617ca7, (uint)xbcea506a33cf9111);
				case xd669244d58bc09c0.x0b2292ab52b25d76:
					return Enum.ToObject(this.x7cbaa9e36b617ca7, (long)xbcea506a33cf9111);
				case xd669244d58bc09c0.x394150f1be471c3c:
					return Enum.ToObject(this.x7cbaa9e36b617ca7, (ulong)((long)xbcea506a33cf9111));
				default:
					throw new InvalidOperationException();
				}
			}
			return Enum.ToObject(this.x7cbaa9e36b617ca7, (ushort)xbcea506a33cf9111);
		}

		public object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			int num = x337e217cb3ba0627.ReadInt32();
			while (this.x12fedb3de1c57ea7 != null)
			{
				int num2 = 0;
				for (;;)
				{
					if (num2 >= this.x12fedb3de1c57ea7.Length)
					{
						if ((uint)num2 + (uint)num2 >= 0U)
						{
							break;
						}
					}
					if (this.x12fedb3de1c57ea7[num2].WireValue == num)
					{
						goto IL_36;
					}
					num2++;
				}
				x337e217cb3ba0627.ThrowEnumException(this.x00c54479c3a7c440, num);
				bool flag = ((uint)num2 | 2U) == 0U;
				if (flag)
				{
					continue;
				}
				return null;
				IL_36:
				return this.x12fedb3de1c57ea7[num2].TypedValue;
			}
			return this.x231070506ace0521(num);
		}

		public void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			if (this.x12fedb3de1c57ea7 != null)
			{
				for (;;)
				{
					int num = 0;
					for (;;)
					{
						if (num >= this.x12fedb3de1c57ea7.Length)
						{
							ProtoWriter.ThrowEnumException(x6b8e154b42d5c1e3, xbcea506a33cf9111);
							bool flag = (uint)num < 0U;
							if (!flag)
							{
								return;
							}
							if (3 != 0)
							{
								goto IL_9A;
							}
							if ((uint)num - (uint)num <= 4294967295U)
							{
								goto IL_5A;
							}
							break;
						}
						IL_7A:
						if (!object.Equals(this.x12fedb3de1c57ea7[num].TypedValue, xbcea506a33cf9111))
						{
							num++;
							continue;
						}
						IL_5A:
						ProtoWriter.WriteInt32(this.x12fedb3de1c57ea7[num].WireValue, x6b8e154b42d5c1e3);
						if (!false)
						{
							return;
						}
						goto IL_7A;
					}
				}
				return;
			}
			IL_9A:
			ProtoWriter.WriteInt32(this.x15cae3472b818a14(xbcea506a33cf9111), x6b8e154b42d5c1e3);
		}

		private readonly Type x7cbaa9e36b617ca7;

		private readonly x9caf06b8b9c3a409.EnumPair[] x12fedb3de1c57ea7;

		public struct EnumPair
		{
			public EnumPair(int wireValue, object raw, Type type)
			{
				this.WireValue = wireValue;
				this.RawValue = raw;
				this.TypedValue = (Enum)Enum.ToObject(type, raw);
			}

			public readonly object RawValue;

			public readonly Enum TypedValue;

			public readonly int WireValue;
		}
	}
}
