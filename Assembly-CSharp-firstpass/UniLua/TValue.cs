using System;

namespace UniLua
{
	public struct TValue
	{
		public override int GetHashCode()
		{
			return this.Tt.GetHashCode() ^ this.NValue.GetHashCode() ^ this.UInt64Value.GetHashCode() ^ ((this.OValue == null) ? 305419896 : this.OValue.GetHashCode());
		}

		public override bool Equals(object o)
		{
			return o is TValue && this.Equals((TValue)o);
		}

		public bool Equals(TValue o)
		{
			if (this.Tt != o.Tt || this.NValue != o.NValue || this.UInt64Value != o.UInt64Value)
			{
				return false;
			}
			switch (this.Tt)
			{
			case 0:
				return true;
			case 1:
				return this.BValue() == o.BValue();
			case 3:
				return this.NValue == o.NValue;
			case 4:
				return this.SValue() == o.SValue();
			case 9:
				return this.UInt64Value == o.UInt64Value;
			}
			return object.ReferenceEquals(this.OValue, o.OValue);
		}

		internal bool TtIsNil()
		{
			return this.Tt == 0;
		}

		internal bool TtIsBoolean()
		{
			return this.Tt == 1;
		}

		internal bool TtIsNumber()
		{
			return this.Tt == 3;
		}

		internal bool TtIsUInt64()
		{
			return this.Tt == 9;
		}

		internal bool TtIsString()
		{
			return this.Tt == 4;
		}

		internal bool TtIsTable()
		{
			return this.Tt == 5;
		}

		internal bool TtIsFunction()
		{
			return this.Tt == 6;
		}

		internal bool TtIsThread()
		{
			return this.Tt == 8;
		}

		internal bool ClIsLuaClosure()
		{
			return this.UInt64Value == 0UL;
		}

		internal bool ClIsCsClosure()
		{
			return this.UInt64Value == 1UL;
		}

		internal bool ClIsLcsClosure()
		{
			return this.UInt64Value == 2UL;
		}

		internal bool BValue()
		{
			return this.UInt64Value != 0UL;
		}

		internal string SValue()
		{
			return (string)this.OValue;
		}

		internal LuaTable HValue()
		{
			return this.OValue as LuaTable;
		}

		internal LuaLClosureValue ClLValue()
		{
			return (LuaLClosureValue)this.OValue;
		}

		internal LuaCsClosureValue ClCsValue()
		{
			return (LuaCsClosureValue)this.OValue;
		}

		internal LuaUserDataValue RawUValue()
		{
			return this.OValue as LuaUserDataValue;
		}

		internal void SetNilValue()
		{
			this.Tt = 0;
			this.NValue = 0.0;
			this.UInt64Value = 0UL;
			this.OValue = null;
		}

		internal void SetBValue(bool v)
		{
			this.Tt = 1;
			this.NValue = 0.0;
			this.UInt64Value = ((!v) ? 0UL : 1UL);
			this.OValue = null;
		}

		internal void SetObj(ref TValue v)
		{
			this.Tt = v.Tt;
			this.NValue = v.NValue;
			this.UInt64Value = v.UInt64Value;
			this.OValue = v.OValue;
		}

		internal void SetNValue(double v)
		{
			this.Tt = 3;
			this.NValue = v;
			this.UInt64Value = 0UL;
			this.OValue = null;
		}

		internal void SetUInt64Value(ulong v)
		{
			this.Tt = 9;
			this.NValue = 0.0;
			this.UInt64Value = v;
			this.OValue = null;
		}

		internal void SetSValue(string v)
		{
			this.Tt = 4;
			this.NValue = 0.0;
			this.UInt64Value = 0UL;
			this.OValue = v;
		}

		internal void SetHValue(LuaTable v)
		{
			this.Tt = 5;
			this.NValue = 0.0;
			this.UInt64Value = 0UL;
			this.OValue = v;
		}

		internal void SetThValue(LuaState v)
		{
			this.Tt = 8;
			this.NValue = 0.0;
			this.UInt64Value = 0UL;
			this.OValue = v;
		}

		internal void SetPValue(object v)
		{
			this.Tt = 2;
			this.NValue = 0.0;
			this.UInt64Value = 0UL;
			this.OValue = v;
		}

		internal void SetClLValue(LuaLClosureValue v)
		{
			this.Tt = 6;
			this.NValue = 0.0;
			this.UInt64Value = 0UL;
			this.OValue = v;
		}

		internal void SetClCsValue(LuaCsClosureValue v)
		{
			this.Tt = 6;
			this.NValue = 0.0;
			this.UInt64Value = 1UL;
			this.OValue = v;
		}

		internal void SetClLcsValue(CSharpFunctionDelegate v)
		{
			this.Tt = 6;
			this.NValue = 0.0;
			this.UInt64Value = 2UL;
			this.OValue = v;
		}

		public override string ToString()
		{
			if (this.TtIsString())
			{
				return string.Format("(string, {0})", this.SValue());
			}
			if (this.TtIsNumber())
			{
				return string.Format("(number, {0})", this.NValue);
			}
			if (this.TtIsNil())
			{
				return "(nil)";
			}
			return string.Format("(type:{0})", this.Tt);
		}

		public static bool operator ==(TValue lhs, TValue rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(TValue lhs, TValue rhs)
		{
			return !lhs.Equals(rhs);
		}

		private const ulong CLOSURE_LUA = 0UL;

		private const ulong CLOSURE_CS = 1UL;

		private const ulong CLOSURE_LCS = 2UL;

		private const ulong BOOLEAN_FALSE = 0UL;

		private const ulong BOOLEAN_TRUE = 1UL;

		public int Tt;

		public double NValue;

		public ulong UInt64Value;

		public object OValue;
	}
}
