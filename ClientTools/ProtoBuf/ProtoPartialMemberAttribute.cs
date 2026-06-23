using System;

namespace ProtoBuf
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class ProtoPartialMemberAttribute : ProtoMemberAttribute
	{
		public ProtoPartialMemberAttribute(int tag, string memberName) : base(tag)
		{
			if ((uint)tag < 0U || (!false && !x479f2661aae93792.x1c140bd1078ddda1(memberName)))
			{
				this.memberName = memberName;
				return;
			}
			throw new ArgumentNullException("memberName");
		}

		public string MemberName
		{
			get
			{
				return this.memberName;
			}
		}

		private readonly string memberName;
	}
}
