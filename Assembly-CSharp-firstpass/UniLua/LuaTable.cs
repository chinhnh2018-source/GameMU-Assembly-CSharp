using System;
using UniLua.Tools;

namespace UniLua
{
	public class LuaTable
	{
		public LuaTable(LuaState l)
		{
			this.InitLuaTable(l);
		}

		static LuaTable()
		{
			LuaTable.TheNilValue = new StkId();
			LuaTable.TheNilValue.V.SetNilValue();
			LuaTable.DummyArrayPart = new StkId[0];
			LuaTable.DummyNode = new LuaTable.HNode();
			LuaTable.DummyNode.Key = LuaTable.TheNilValue;
			LuaTable.DummyNode.Val = LuaTable.TheNilValue;
			LuaTable.DummyNode.Next = null;
			LuaTable.DummyHashPart = new LuaTable.HNode[1];
			LuaTable.DummyHashPart[0] = LuaTable.DummyNode;
			LuaTable.DummyHashPart[0].Index = 0;
		}

		~LuaTable()
		{
			this.Recycle();
		}

		public StkId Get(ref TValue key)
		{
			if (key.Tt == 0)
			{
				return LuaTable.TheNilValue;
			}
			if (this.IsPositiveInteger(ref key))
			{
				return this.GetInt((int)key.NValue);
			}
			if (key.Tt == 4)
			{
				return this.GetStr(key.SValue());
			}
			int hashCode = key.GetHashCode();
			for (LuaTable.HNode hnode = this.GetHashNode(hashCode); hnode != null; hnode = hnode.Next)
			{
				if (hnode.Key.V == key)
				{
					return hnode.Val;
				}
			}
			return LuaTable.TheNilValue;
		}

		public StkId GetInt(int key)
		{
			if (0 < key && key - 1 < this.ArrayPart.Length)
			{
				return this.ArrayPart[key - 1];
			}
			TValue tvalue = default(TValue);
			tvalue.SetNValue((double)key);
			for (LuaTable.HNode hnode = this.GetHashNode(ref tvalue); hnode != null; hnode = hnode.Next)
			{
				if (hnode.Key.V.TtIsNumber() && hnode.Key.V.NValue == (double)key)
				{
					return hnode.Val;
				}
			}
			return LuaTable.TheNilValue;
		}

		public StkId GetStr(string key)
		{
			int hashCode = key.GetHashCode();
			for (LuaTable.HNode hnode = this.GetHashNode(hashCode); hnode != null; hnode = hnode.Next)
			{
				if (hnode.Key.V.TtIsString() && hnode.Key.V.SValue() == key)
				{
					return hnode.Val;
				}
			}
			return LuaTable.TheNilValue;
		}

		public void Set(ref TValue key, ref TValue val)
		{
			StkId stkId = this.Get(ref key);
			if (stkId == LuaTable.TheNilValue)
			{
				stkId = this.NewTableKey(ref key);
			}
			stkId.V.SetObj(ref val);
		}

		public void SetInt(int key, ref TValue val)
		{
			StkId stkId = this.GetInt(key);
			if (stkId == LuaTable.TheNilValue)
			{
				TValue tvalue = default(TValue);
				tvalue.SetNValue((double)key);
				stkId = this.NewTableKey(ref tvalue);
			}
			stkId.V.SetObj(ref val);
		}

		private int FindIndex(StkId key)
		{
			if (key.V.TtIsNil())
			{
				return -1;
			}
			int num = this.ArrayIndex(ref key.V);
			if (0 < num && num <= this.ArrayPart.Length)
			{
				return num - 1;
			}
			LuaTable.HNode hnode = this.GetHashNode(ref key.V);
			while (!this.L.V_RawEqualObj(ref hnode.Key.V, ref key.V))
			{
				hnode = hnode.Next;
				if (hnode == null)
				{
					this.L.G_RunError("invalid key to 'next'", new object[0]);
				}
			}
			return this.ArrayPart.Length + hnode.Index;
		}

		public bool Next(StkId key, StkId val)
		{
			int i = this.FindIndex(key);
			for (i++; i < this.ArrayPart.Length; i++)
			{
				if (!this.ArrayPart[i].V.TtIsNil())
				{
					key.V.SetNValue((double)(i + 1));
					val.V.SetObj(ref this.ArrayPart[i].V);
					return true;
				}
			}
			for (i -= this.ArrayPart.Length; i < this.HashPart.Length; i++)
			{
				if (!this.HashPart[i].Val.V.TtIsNil())
				{
					key.V.SetObj(ref this.HashPart[i].Key.V);
					val.V.SetObj(ref this.HashPart[i].Val.V);
					return true;
				}
			}
			return false;
		}

		public int Length
		{
			get
			{
				uint num = (uint)this.ArrayPart.Length;
				if (num > 0U && this.ArrayPart[(int)((UIntPtr)(num - 1U))].V.TtIsNil())
				{
					uint num2 = 0U;
					while (num - num2 > 1U)
					{
						uint num3 = (num2 + num) / 2U;
						if (this.ArrayPart[(int)((UIntPtr)(num3 - 1U))].V.TtIsNil())
						{
							num = num3;
						}
						else
						{
							num2 = num3;
						}
					}
					return (int)num2;
				}
				if (this.HashPart == LuaTable.DummyHashPart)
				{
					return (int)num;
				}
				return this.UnboundSearch(num);
			}
		}

		public void Resize(int nasize, int nhsize)
		{
			int num = this.ArrayPart.Length;
			LuaTable.HNode[] hashPart = this.HashPart;
			if (nasize > num)
			{
				this.SetArraryVector(nasize);
			}
			this.SetNodeVector(nhsize);
			if (nasize < num)
			{
				StkId[] arrayPart = this.ArrayPart;
				this.ArrayPart = LuaTable.DummyArrayPart;
				for (int i = nasize; i < num; i++)
				{
					if (!arrayPart[i].V.TtIsNil())
					{
						this.SetInt(i + 1, ref arrayPart[i].V);
					}
				}
				StkId[] array = new StkId[nasize];
				for (int j = 0; j < nasize; j++)
				{
					array[j] = arrayPart[j];
				}
				this.ArrayPart = array;
			}
			foreach (LuaTable.HNode hnode in hashPart)
			{
				if (!hnode.Val.V.TtIsNil())
				{
					this.Set(ref hnode.Key.V, ref hnode.Val.V);
				}
			}
			if (hashPart != LuaTable.DummyHashPart)
			{
				this.RecycleHNode(hashPart);
			}
		}

		private void Recycle()
		{
			if (this.HashPart != null && this.HashPart != LuaTable.DummyHashPart)
			{
				this.RecycleHNode(this.HashPart);
				this.HashPart = null;
			}
		}

		private void RecycleHNode(LuaTable.HNode[] garbage)
		{
			if (garbage == null || garbage.Length == 0)
			{
				return;
			}
			for (int i = 0; i < garbage.Length - 1; i++)
			{
				garbage[i].Next = garbage[i + 1];
			}
			object cacheHeadLock = LuaTable.CacheHeadLock;
			lock (cacheHeadLock)
			{
				garbage[garbage.Length - 1].Next = LuaTable.CacheHead;
				LuaTable.CacheHead = garbage[0];
			}
		}

		private LuaTable.HNode NewHNode()
		{
			LuaTable.HNode hnode;
			if (LuaTable.CacheHead == null)
			{
				hnode = new LuaTable.HNode();
				hnode.Key = new StkId();
				hnode.Val = new StkId();
			}
			else
			{
				object cacheHeadLock = LuaTable.CacheHeadLock;
				lock (cacheHeadLock)
				{
					hnode = LuaTable.CacheHead;
					LuaTable.CacheHead = LuaTable.CacheHead.Next;
				}
				hnode.Next = null;
				hnode.Index = 0;
				hnode.Key.V.SetNilValue();
				hnode.Val.V.SetNilValue();
			}
			return hnode;
		}

		private void InitLuaTable(LuaState lua)
		{
			this.L = lua;
			this.ArrayPart = LuaTable.DummyArrayPart;
			this.SetNodeVector(0);
		}

		private bool IsPositiveInteger(ref TValue v)
		{
			return v.TtIsNumber() && v.NValue > 0.0 && v.NValue % 1.0 == 0.0 && v.NValue <= 2147483647.0;
		}

		private LuaTable.HNode GetHashNode(int hashcode)
		{
			return this.HashPart[(int)(checked((IntPtr)(unchecked((ulong)hashcode % (ulong)((long)this.HashPart.Length)))))];
		}

		private LuaTable.HNode GetHashNode(ref TValue v)
		{
			if (this.IsPositiveInteger(ref v))
			{
				return this.GetHashNode((int)v.NValue);
			}
			if (v.TtIsString())
			{
				return this.GetHashNode(v.SValue().GetHashCode());
			}
			return this.GetHashNode(v.GetHashCode());
		}

		private void SetArraryVector(int size)
		{
			Utl.Assert(size >= this.ArrayPart.Length);
			StkId[] array = new StkId[size];
			int i;
			for (i = 0; i < this.ArrayPart.Length; i++)
			{
				array[i] = this.ArrayPart[i];
			}
			while (i < size)
			{
				array[i] = new StkId();
				array[i].V.SetNilValue();
				i++;
			}
			this.ArrayPart = array;
		}

		private void SetNodeVector(int size)
		{
			if (size == 0)
			{
				this.HashPart = LuaTable.DummyHashPart;
				this.LastFree = size;
				return;
			}
			int num = this.CeilLog2(size);
			if (num > 30)
			{
				this.L.G_RunError("table overflow", new object[0]);
			}
			size = 1 << num;
			this.HashPart = new LuaTable.HNode[size];
			for (int i = 0; i < size; i++)
			{
				this.HashPart[i] = this.NewHNode();
				this.HashPart[i].Index = i;
			}
			this.LastFree = size;
		}

		private LuaTable.HNode GetFreePos()
		{
			while (this.LastFree > 0)
			{
				LuaTable.HNode hnode = this.HashPart[--this.LastFree];
				if (hnode.Key.V.TtIsNil())
				{
					return hnode;
				}
			}
			return null;
		}

		private int ArrayIndex(ref TValue k)
		{
			if (this.IsPositiveInteger(ref k))
			{
				return (int)k.NValue;
			}
			return -1;
		}

		private int CeilLog2(int x)
		{
			Utl.Assert(x > 0);
			int num = 0;
			for (x--; x >= 256; x >>= 8)
			{
				num += 8;
			}
			return num + (int)LuaTable.Log2_[x];
		}

		private int CountInt(ref TValue key, ref int[] nums)
		{
			int num = this.ArrayIndex(ref key);
			if (0 < num && num <= 1073741824)
			{
				nums[this.CeilLog2(num)]++;
				return 1;
			}
			return 0;
		}

		private int NumUseArray(ref int[] nums)
		{
			int num = 0;
			int i = 1;
			int j = 0;
			int num2 = 1;
			while (j <= 30)
			{
				int num3 = 0;
				int num4 = num2;
				if (num4 > this.ArrayPart.Length)
				{
					num4 = this.ArrayPart.Length;
					if (i > num4)
					{
						break;
					}
				}
				while (i <= num4)
				{
					if (!this.ArrayPart[i - 1].V.TtIsNil())
					{
						num3++;
					}
					i++;
				}
				nums[j] += num3;
				num += num3;
				j++;
				num2 *= 2;
			}
			return num;
		}

		private int NumUseHash(ref int[] nums, ref int nasize)
		{
			int num = 0;
			int num2 = 0;
			int num3 = this.HashPart.Length;
			while (num3-- > 0)
			{
				LuaTable.HNode hnode = this.HashPart[num3];
				if (!hnode.Val.V.TtIsNil())
				{
					num2 += this.CountInt(ref hnode.Key.V, ref nums);
					num++;
				}
			}
			nasize += num2;
			return num;
		}

		private int ComputeSizes(ref int[] nums, ref int nasize)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 1;
			while (num5 / 2 < nasize)
			{
				if (nums[num4] > 0)
				{
					num += nums[num4];
					if (num > num5 / 2)
					{
						num3 = num5;
						num2 = num;
					}
				}
				if (num == nasize)
				{
					break;
				}
				num4++;
				num5 *= 2;
			}
			nasize = num3;
			Utl.Assert(nasize / 2 <= num2 && num2 <= nasize);
			return num2;
		}

		private void Rehash(ref TValue k)
		{
			for (int i = 0; i <= 30; i++)
			{
				LuaTable.Nums[i] = 0;
			}
			int num = this.NumUseArray(ref LuaTable.Nums);
			int num2 = num;
			num2 += this.NumUseHash(ref LuaTable.Nums, ref num);
			num += this.CountInt(ref k, ref LuaTable.Nums);
			num2++;
			int num3 = this.ComputeSizes(ref LuaTable.Nums, ref num);
			this.Resize(num, num2 - num3);
		}

		private void DumpParts()
		{
			ULDebug.Log.Invoke("------------------ [DumpParts] enter -----------------------");
			ULDebug.Log.Invoke("<< Array Part >>");
			for (int i = 0; i < this.ArrayPart.Length; i++)
			{
				StkId stkId = this.ArrayPart[i];
				ULDebug.Log.Invoke(string.Format("i:{0} val:{1}", i, stkId.V));
			}
			ULDebug.Log.Invoke("<< Hash Part >>");
			for (int j = 0; j < this.HashPart.Length; j++)
			{
				LuaTable.HNode hnode = this.HashPart[j];
				int num = (hnode.Next != null) ? hnode.Next.Index : -1;
				ULDebug.Log.Invoke(string.Format("i:{0} index:{1} key:{2} val:{3} next:{4}", new object[]
				{
					j,
					hnode.Index,
					hnode.Key.V,
					hnode.Val.V,
					num
				}));
			}
			ULDebug.Log.Invoke("++++++++++++++++++ [DumpParts] leave +++++++++++++++++++++++");
		}

		private StkId NewTableKey(ref TValue k)
		{
			if (k.TtIsNil())
			{
				this.L.G_RunError("table index is nil", new object[0]);
			}
			if (k.TtIsNumber() && double.IsNaN(k.NValue))
			{
				this.L.G_RunError("table index is NaN", new object[0]);
			}
			LuaTable.HNode hnode = this.GetHashNode(ref k);
			if (!hnode.Val.V.TtIsNil() || hnode == LuaTable.DummyNode)
			{
				LuaTable.HNode freePos = this.GetFreePos();
				if (freePos == null)
				{
					this.Rehash(ref k);
					StkId stkId = this.Get(ref k);
					if (stkId != LuaTable.TheNilValue)
					{
						return stkId;
					}
					return this.NewTableKey(ref k);
				}
				else
				{
					Utl.Assert(freePos != LuaTable.DummyNode);
					LuaTable.HNode hnode2 = this.GetHashNode(ref hnode.Key.V);
					if (hnode2 != hnode)
					{
						while (hnode2.Next != hnode)
						{
							hnode2 = hnode2.Next;
						}
						hnode2.Next = freePos;
						freePos.CopyFrom(hnode);
						hnode.Next = null;
						hnode.Val.V.SetNilValue();
					}
					else
					{
						freePos.Next = hnode.Next;
						hnode.Next = freePos;
						hnode = freePos;
					}
				}
			}
			hnode.Key.V.SetObj(ref k);
			Utl.Assert(hnode.Val.V.TtIsNil());
			return hnode.Val;
		}

		private int UnboundSearch(uint j)
		{
			uint num = j;
			j += 1U;
			while (!this.GetInt((int)j).V.TtIsNil())
			{
				num = j;
				j *= 2U;
				if (j > 2147483645U)
				{
					num = 1U;
					while (!this.GetInt((int)num).V.TtIsNil())
					{
						num += 1U;
					}
					return (int)(num - 1U);
				}
			}
			while (j - num > 1U)
			{
				uint num2 = (num + j) / 2U;
				if (this.GetInt((int)num2).V.TtIsNil())
				{
					j = num2;
				}
				else
				{
					num = num2;
				}
			}
			return (int)num;
		}

		private const int MAXBITS = 30;

		private const int MAXASIZE = 1073741824;

		public LuaTable MetaTable;

		public uint NoTagMethodFlags;

		private LuaState L;

		private StkId[] ArrayPart;

		private LuaTable.HNode[] HashPart;

		private int LastFree;

		private static StkId TheNilValue;

		private static StkId[] DummyArrayPart;

		private static LuaTable.HNode DummyNode;

		private static LuaTable.HNode[] DummyHashPart;

		private static LuaTable.HNode CacheHead = null;

		private static object CacheHeadLock = new object();

		private static readonly byte[] Log2_ = new byte[]
		{
			0,
			1,
			2,
			2,
			3,
			3,
			3,
			3,
			4,
			4,
			4,
			4,
			4,
			4,
			4,
			4,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8,
			8
		};

		private static int[] Nums = new int[31];

		private class HNode
		{
			public void CopyFrom(LuaTable.HNode o)
			{
				this.Key.V.SetObj(ref o.Key.V);
				this.Val.V.SetObj(ref o.Val.V);
				this.Next = o.Next;
			}

			public int Index;

			public StkId Key;

			public StkId Val;

			public LuaTable.HNode Next;
		}
	}
}
