using System;
using System.Reflection;

namespace ProtoBuf
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ProtoMemberAttribute : Attribute, IComparable<ProtoMemberAttribute>, IComparable
	{
		public int CompareTo(object other)
		{
			return this.CompareTo(other as ProtoMemberAttribute);
		}

		public int CompareTo(ProtoMemberAttribute other)
		{
			int num;
			if (other != null)
			{
				while (this != other)
				{
					if (!false)
					{
						goto IL_0C;
					}
					bool flag = ((uint)num & 0U) == 0U;
					if (!flag)
					{
						goto IL_24;
					}
					if ((uint)num - (uint)num >= 0U)
					{
						flag = ((uint)num - (uint)num < 0U);
						if (!flag)
						{
							if (-1 == 0)
							{
								return num;
							}
							goto IL_0C;
						}
					}
				}
				return 0;
			}
			if (!false)
			{
				return -1;
			}
			IL_0C:
			num = this.tag.CompareTo(other.tag);
			if (num != 0)
			{
				return num;
			}
			IL_24:
			num = string.CompareOrdinal(this.name, other.name);
			return num;
		}

		public ProtoMemberAttribute(int tag) : this(tag, false)
		{
		}

		internal ProtoMemberAttribute(int tag, bool forced)
		{
			if (tag <= 0)
			{
				if (!forced)
				{
					throw new ArgumentOutOfRangeException("tag");
				}
			}
			this.tag = tag;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public DataFormat DataFormat
		{
			get
			{
				return this.dataFormat;
			}
			set
			{
				this.dataFormat = value;
			}
		}

		public int Tag
		{
			get
			{
				return this.tag;
			}
		}

		internal void Rebase(int tag)
		{
			this.tag = tag;
		}

		public bool IsRequired
		{
			get
			{
				return (this.options & MemberSerializationOptions.Required) == MemberSerializationOptions.Required;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.Required;
					return;
				}
				this.options &= ~MemberSerializationOptions.Required;
			}
		}

		public bool IsPacked
		{
			get
			{
				return (this.options & MemberSerializationOptions.Packed) == MemberSerializationOptions.Packed;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.Packed;
					return;
				}
				this.options &= ~MemberSerializationOptions.Packed;
			}
		}

		public bool OverwriteList
		{
			get
			{
				return (this.options & MemberSerializationOptions.OverwriteList) == MemberSerializationOptions.OverwriteList;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.OverwriteList;
					return;
				}
				this.options &= ~MemberSerializationOptions.OverwriteList;
			}
		}

		public bool AsReference
		{
			get
			{
				return (this.options & MemberSerializationOptions.AsReference) == MemberSerializationOptions.AsReference;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.AsReference;
					return;
				}
				this.options &= ~MemberSerializationOptions.AsReference;
			}
		}

		public bool DynamicType
		{
			get
			{
				return (this.options & MemberSerializationOptions.DynamicType) == MemberSerializationOptions.DynamicType;
			}
			set
			{
				if (value)
				{
					this.options |= MemberSerializationOptions.DynamicType;
					return;
				}
				this.options &= ~MemberSerializationOptions.DynamicType;
			}
		}

		public MemberSerializationOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		internal MemberInfo Member;

		internal bool TagIsPinned;

		private string name;

		private DataFormat dataFormat;

		private int tag;

		private MemberSerializationOptions options;
	}
}
