using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class RoleDamage : IComparable<RoleDamage>
	{
		public RoleDamage()
		{
		}

		public RoleDamage(int roleID, long damage, string roleName = null, params int[] param)
		{
			this.RoleID = roleID;
			this.Damage = damage;
			this.RoleName = roleName;
			if (param != null && param.Length > 0)
			{
				this.FlagList = Enumerable.ToList<int>(param);
			}
		}

		public int CompareTo(RoleDamage right)
		{
			long num = right.Damage - this.Damage;
			if (num > 0L)
			{
				return 1;
			}
			if (num == 0L)
			{
				return 0;
			}
			return -1;
		}

		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public long Damage;

		[ProtoMember(3)]
		public string RoleName;

		[ProtoMember(4)]
		public List<int> FlagList;
	}
}
