using System;

namespace ProtoBuf
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class ProtoPartialIgnoreAttribute : ProtoIgnoreAttribute
	{
		public ProtoPartialIgnoreAttribute(string memberName)
		{
			if (!x479f2661aae93792.x1c140bd1078ddda1(memberName))
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
