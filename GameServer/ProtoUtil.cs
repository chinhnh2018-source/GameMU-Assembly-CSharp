using System;
using System.Collections.Generic;
using System.Text;
using Tmsk.Contract;

internal class ProtoUtil
{
	public static int CalcIntSize(int val)
	{
		int result;
		if (val < 0)
		{
			result = 10;
		}
		else
		{
			int num = 0;
			do
			{
				num++;
			}
			while ((val >>= 7) != 0);
			result = num;
		}
		return result;
	}

	public static int GetIntSize(int val, bool calcMember = false, int protoMember = 0, bool useDef = true, int defval = 0)
	{
		int result;
		if (useDef && val == defval)
		{
			result = 0;
		}
		else
		{
			int num = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					num = 1;
				}
				else
				{
					num = 2;
				}
			}
			if (val < 0)
			{
				result = num + 10;
			}
			else
			{
				int num2 = 0;
				do
				{
					num2++;
				}
				while ((val >>= 7) != 0);
				result = num + num2;
			}
		}
		return result;
	}

	public static int IntToBytes(byte[] data, int offset, int val)
	{
		int num = 0;
		if (val >= 0)
		{
			do
			{
				num++;
				data[offset++] = (byte)((val & 127) | 128);
			}
			while ((val >>= 7) != 0);
			int num2 = offset - 1;
			data[num2] &= 127;
		}
		else
		{
			data[offset] = (byte)(val | 128);
			data[offset + 1] = (byte)(val >> 7 | 128);
			data[offset + 2] = (byte)(val >> 14 | 128);
			data[offset + 3] = (byte)(val >> 21 | 128);
			data[offset + 4] = (byte)(val >> 28 | 128);
			data[offset + 5] = (data[offset + 6] = (data[offset + 7] = (data[offset + 8] = byte.MaxValue)));
			data[offset + 9] = 1;
			num = 10;
		}
		return num;
	}

	public static int IntFromBytes(byte[] data, ref int offset, ref int ncount)
	{
		int num = offset;
		int num2 = 0;
		uint num3 = (uint)data[num++];
		if ((num3 & 128U) == 0U)
		{
			num2 = 1;
		}
		else
		{
			num3 &= 127U;
			uint num4 = (uint)data[num++];
			num3 |= (num4 & 127U) << 7;
			if ((num4 & 128U) == 0U)
			{
				num2 = 2;
			}
			else
			{
				num4 = (uint)data[num++];
				num3 |= (num4 & 127U) << 14;
				if ((num4 & 128U) == 0U)
				{
					num2 = 3;
				}
				else
				{
					num4 = (uint)data[num++];
					num3 |= (num4 & 127U) << 21;
					if ((num4 & 128U) == 0U)
					{
						num2 = 4;
					}
					else
					{
						num4 = (uint)data[num];
						num3 |= num4 << 28;
						if ((num4 & 240U) == 0U)
						{
							num2 = 5;
						}
						else if ((num4 & 240U) == 240U && data[++num] == 255 && data[++num] == 255 && data[++num] == 255 && data[++num] == 255 && data[num + 1] == 1)
						{
							num2 = 10;
						}
					}
				}
			}
		}
		offset += num2;
		ncount += num2;
		if (ProtoUtil.CheckInvalidUnpack && num2 <= 0)
		{
			throw new Exception("unexcepted field Length: " + num2);
		}
		return (int)num3;
	}

	public static void GetTag(byte[] data, ref int offset, ref int fieldnumber, ref int wt, ref int ncount)
	{
		int num = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
		fieldnumber = num >> 3;
		wt = (num & 7);
	}

	public static void IntMemberToBytes(byte[] data, int fieldnumber, ref int offset, int val, bool useDef = true, int defval = 0)
	{
		if (!useDef || val != defval)
		{
			int val2 = fieldnumber << 3;
			offset += ProtoUtil.IntToBytes(data, offset, val2);
			offset += ProtoUtil.IntToBytes(data, offset, val);
		}
	}

	public static int IntMemberFromBytes(byte[] data, int wt, ref int offset, ref int ncount)
	{
		if (wt != 0)
		{
			throw new ArgumentException("int member from bytes error, type error!!!");
		}
		return ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
	}

	public static int GetLongSize(long val, bool calcMember = false, int protoMember = 0, bool useDef = true, long defval = 0L)
	{
		int result;
		if (useDef && val == defval)
		{
			result = 0;
		}
		else
		{
			int num = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					num = 1;
				}
				else
				{
					num = 2;
				}
			}
			if (val < 0L)
			{
				result = 10 + num;
			}
			else
			{
				int num2 = 0;
				do
				{
					num2++;
				}
				while ((val >>= 7) != 0L);
				result = num + num2;
			}
		}
		return result;
	}

	private static int LongToBytes(byte[] data, int offset, long val)
	{
		int num = 0;
		if (val >= 0L)
		{
			do
			{
				data[offset++] = (byte)((val & 127L) | 128L);
				num++;
			}
			while ((val >>= 7) != 0L);
			int num2 = offset - 1;
			data[num2] &= 127;
		}
		else
		{
			num = 10;
			data[offset] = (byte)(val | 128L);
			data[offset + 1] = (byte)((int)(val >> 7) | 128);
			data[offset + 2] = (byte)((int)(val >> 14) | 128);
			data[offset + 3] = (byte)((int)(val >> 21) | 128);
			data[offset + 4] = (byte)((int)(val >> 28) | 128);
			data[offset + 5] = (byte)((int)(val >> 35) | 128);
			data[offset + 6] = (byte)((int)(val >> 42) | 128);
			data[offset + 7] = (byte)((int)(val >> 49) | 128);
			data[offset + 8] = (byte)((int)(val >> 56) | 128);
			data[offset + 9] = 1;
		}
		return num;
	}

	private static long LongFromBytes(byte[] data, ref int offset, ref int ncount)
	{
		int num = offset;
		ulong num2 = (ulong)data[num++];
		int num3;
		if ((num2 & 128UL) == 0UL)
		{
			num3 = 1;
		}
		else
		{
			num2 &= 127UL;
			ulong num4 = (ulong)data[num++];
			num2 |= (num4 & 127UL) << 7;
			if ((num4 & 128UL) == 0UL)
			{
				num3 = 2;
			}
			else
			{
				num4 = (ulong)data[num++];
				num2 |= (num4 & 127UL) << 14;
				if ((num4 & 128UL) == 0UL)
				{
					num3 = 3;
				}
				else
				{
					num4 = (ulong)data[num++];
					num2 |= (num4 & 127UL) << 21;
					if ((num4 & 128UL) == 0UL)
					{
						num3 = 4;
					}
					else
					{
						num4 = (ulong)data[num++];
						num2 |= (num4 & 127UL) << 28;
						if ((num4 & 128UL) == 0UL)
						{
							num3 = 5;
						}
						else
						{
							num4 = (ulong)data[num++];
							num2 |= (num4 & 127UL) << 35;
							if ((num4 & 128UL) == 0UL)
							{
								num3 = 6;
							}
							else
							{
								num4 = (ulong)data[num++];
								num2 |= (num4 & 127UL) << 42;
								if ((num4 & 128UL) == 0UL)
								{
									num3 = 7;
								}
								else
								{
									num4 = (ulong)data[num++];
									num2 |= (num4 & 127UL) << 49;
									if ((num4 & 128UL) == 0UL)
									{
										num3 = 8;
									}
									else
									{
										num4 = (ulong)data[num++];
										num2 |= (num4 & 127UL) << 56;
										if ((num4 & 128UL) == 0UL)
										{
											num3 = 9;
										}
										else
										{
											num4 = (ulong)data[num];
											num2 |= num4 << 63;
											num3 = 10;
											if ((num4 & 18446744073709551614UL) != 0UL)
											{
												throw new OverflowException("long parse over flow, sign bit error!");
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		offset += num3;
		ncount += num3;
		if (ProtoUtil.CheckInvalidUnpack && num3 <= 0)
		{
			throw new Exception("unexcepted field Length: " + num3);
		}
		return (long)num2;
	}

	public static void LongMemberToBytes(byte[] data, int fieldnumber, ref int offset, long val, bool useDef = true, long defval = 0L)
	{
		if (!useDef || val != defval)
		{
			int val2 = fieldnumber << 3;
			offset += ProtoUtil.IntToBytes(data, offset, val2);
			offset += ProtoUtil.LongToBytes(data, offset, val);
		}
	}

	public static long LongMemberFromBytes(byte[] data, int wt, ref int offset, ref int ncount)
	{
		if (wt != 0)
		{
			throw new ArgumentException("long member from bytes error, type error!!!");
		}
		return ProtoUtil.LongFromBytes(data, ref offset, ref ncount);
	}

	public static int GetDoubleSize(double val, bool calcMember = false, int protoMember = 0, bool useDef = true, double defval = 0.0)
	{
		int result;
		if (useDef && val == defval)
		{
			result = 0;
		}
		else
		{
			int num = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					num = 1;
				}
				else
				{
					num = 2;
				}
			}
			result = num + 8;
		}
		return result;
	}

	public static void DoubleMemberToBytes(byte[] data, int fieldnumber, ref int offset, double val, bool useDef = true, double valdef = 0.0)
	{
		if (!useDef || val != valdef)
		{
			int val2 = fieldnumber << 3 | 1;
			offset += ProtoUtil.IntToBytes(data, offset, val2);
			long num = BitConverter.ToInt64(BitConverter.GetBytes(val), 0);
			data[offset++] = (byte)num;
			data[offset++] = (byte)(num >> 8);
			data[offset++] = (byte)(num >> 16);
			data[offset++] = (byte)(num >> 24);
			data[offset++] = (byte)(num >> 32);
			data[offset++] = (byte)(num >> 40);
			data[offset++] = (byte)(num >> 48);
			data[offset++] = (byte)(num >> 56);
		}
	}

	public static double DoubleMemberFromBytes(byte[] data, int wt, ref int offset, ref int ncount)
	{
		if (wt != 1)
		{
			throw new ArgumentException("double from bytes error, type error!!!");
		}
		long num = (long)((ulong)data[offset++]);
		num |= (long)((long)((ulong)data[offset++]) << 8);
		num |= (long)((long)((ulong)data[offset++]) << 16);
		num |= (long)((long)((ulong)data[offset++]) << 24);
		num |= (long)((long)((ulong)data[offset++]) << 32);
		num |= (long)((long)((ulong)data[offset++]) << 40);
		num |= (long)((long)((ulong)data[offset++]) << 48);
		num |= (long)((long)((ulong)data[offset++]) << 56);
		ncount += 8;
		return BitConverter.ToDouble(BitConverter.GetBytes(num), 0);
	}

	public static int GetStringSize(string val, bool calcMember = false, int protoMember = 0)
	{
		int result;
		if (val == null)
		{
			result = 0;
		}
		else
		{
			int num = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					num = 1;
				}
				else
				{
					num = 2;
				}
			}
			int length = val.Length;
			if (length == 0)
			{
				result = 1 + num;
			}
			else
			{
				int byteCount = new UTF8Encoding().GetByteCount(val);
				result = num + ProtoUtil.CalcIntSize(byteCount) + byteCount;
			}
		}
		return result;
	}

	public static void StringMemberToBytes(byte[] data, int fieldnumber, ref int offset, string val)
	{
		if (val != null)
		{
			int val2 = fieldnumber << 3 | 2;
			offset += ProtoUtil.IntToBytes(data, offset, val2);
			int length = val.Length;
			if (length == 0)
			{
				data[offset++] = 0;
			}
			else
			{
				int byteCount = new UTF8Encoding().GetByteCount(val);
				offset += ProtoUtil.IntToBytes(data, offset, byteCount);
				int bytes = new UTF8Encoding().GetBytes(val, 0, val.Length, data, offset);
				offset += byteCount;
			}
		}
	}

	public static string StringMemberFromBytes(byte[] data, int wt, ref int offset, ref int ncount)
	{
		if (wt != 2)
		{
			throw new ArgumentException("string from bytes error, type error!!!");
		}
		int num = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
		string result;
		if (num == 0)
		{
			result = "";
		}
		else
		{
			string @string = new UTF8Encoding().GetString(data, offset, num);
			offset += num;
			ncount += num;
			result = @string;
		}
		return result;
	}

	public static int GetListBytesSize<T>(List<T> lst, bool calcMember = false, int protoMember = 0) where T : IProtoBuffDataEx
	{
		int result;
		if (lst == null || lst.Count <= 0)
		{
			result = 0;
		}
		else
		{
			int num = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					num = 1;
				}
				else
				{
					num = 2;
				}
			}
			int num2 = 0;
			num2 += ProtoUtil.CalcIntSize(lst.Count);
			int count = lst.Count;
			for (int i = 0; i < count; i++)
			{
				T t = lst[i];
				int bytesSize = t.getBytesSize();
				num2 += ProtoUtil.CalcIntSize(bytesSize);
				num2 += bytesSize;
			}
			result = num + num2;
		}
		return result;
	}

	public static byte[] ListToBytes<T>(List<T> lst, int member, ref int offset, byte[] data) where T : IProtoBuffData
	{
		byte[] result;
		if (lst == null || lst.Count <= 0)
		{
			result = data;
		}
		else
		{
			int val = member << 3 | 2;
			offset += ProtoUtil.IntToBytes(data, offset, val);
			offset += ProtoUtil.IntToBytes(data, offset, lst.Count);
			for (int i = 0; i < lst.Count; i++)
			{
				T t = lst[i];
				byte[] array = t.toBytes();
				int num = ProtoUtil.CalcIntSize(array.Length);
				if (array.Length + offset + num > data.Length)
				{
					byte[] array2 = new byte[data.Length * 2];
					Array.Copy(data, array2, data.Length);
					data = array2;
				}
				offset += ProtoUtil.IntToBytes(data, offset, array.Length);
				if (array.Length > 0)
				{
					Array.Copy(array, 0, data, offset, array.Length);
					offset += array.Length;
				}
			}
			if (offset < data.Length)
			{
				Array.Resize<byte>(ref data, offset);
			}
			result = data;
		}
		return result;
	}

	public static void ListMemberFromBytes<T>(List<T> lst, byte[] data, int wt, ref int offset, ref int ncount) where T : class, IProtoBuffData, new()
	{
		if (lst != null && data != null)
		{
			if (wt != 2)
			{
				throw new ArgumentException("list member from bytes error, type error!!!");
			}
			int num = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
			for (int i = 0; i < num; i++)
			{
				int num2 = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
				T item = Activator.CreateInstance<T>();
				if (num2 <= 0)
				{
					lst.Add(item);
				}
				else
				{
					int num3 = item.fromBytes(data, offset, num2);
					ncount += num3 - offset;
					offset = num3;
					lst.Add(item);
				}
			}
		}
	}

	public static int GetListIntBytesSize(List<int> lst, bool calcMember = false, int protoMember = 0)
	{
		int result;
		if (lst == null || lst.Count <= 0)
		{
			result = 0;
		}
		else
		{
			int num = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					num = 1;
				}
				else
				{
					num = 2;
				}
			}
			int num2 = 0;
			num2 += ProtoUtil.CalcIntSize(lst.Count);
			int count = lst.Count;
			for (int i = 0; i < count; i++)
			{
				num2 += ProtoUtil.CalcIntSize(lst[i]);
			}
			result = num + num2;
		}
		return result;
	}

	public static void ListIntToBytes(byte[] data, int member, ref int offset, List<int> lst)
	{
		if (lst != null && lst.Count > 0)
		{
			int val = member << 3 | 2;
			offset += ProtoUtil.IntToBytes(data, offset, val);
			offset += ProtoUtil.IntToBytes(data, offset, lst.Count);
			for (int i = 0; i < lst.Count; i++)
			{
				offset += ProtoUtil.IntToBytes(data, offset, lst[i]);
			}
		}
	}

	public static List<int> ListIntFromBytes(byte[] data, int wt, ref int offset, ref int ncount, List<int> lst)
	{
		List<int> result;
		if (data == null)
		{
			result = lst;
		}
		else
		{
			if (wt != 2)
			{
				throw new ArgumentException("list member from bytes error, type error!!!");
			}
			int num = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
			if (num <= 0)
			{
				result = lst;
			}
			else
			{
				if (lst == null)
				{
					lst = new List<int>(num);
				}
				for (int i = 0; i < num; i++)
				{
					int item = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
					lst.Add(item);
				}
				result = lst;
			}
		}
		return result;
	}

	public static int GetDictionaryMemberHeader(int fieldnumber)
	{
		return fieldnumber << 3 | 2;
	}

	private static bool CheckInvalidUnpack = true;
}
