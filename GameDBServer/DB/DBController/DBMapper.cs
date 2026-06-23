using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameDBServer.DB.DBController
{
	internal class DBMapper
	{
		public DBMapper(Type type)
		{
			MemberInfo[] members = type.GetMembers();
			foreach (MemberInfo memberInfo in members)
			{
				if (memberInfo.MemberType == MemberTypes.Field || memberInfo.MemberType == MemberTypes.Property)
				{
					object[] customAttributes = memberInfo.GetCustomAttributes(typeof(DBMappingAttribute), false);
					if (null != customAttributes)
					{
						DBMappingAttribute[] array2 = (DBMappingAttribute[])customAttributes;
						foreach (DBMappingAttribute dbmappingAttribute in array2)
						{
							if (dbmappingAttribute.ColumnName != null && !"".Equals(dbmappingAttribute.ColumnName))
							{
								this.memberMappings.Add(dbmappingAttribute.ColumnName, memberInfo);
							}
						}
					}
				}
			}
		}

		public MemberInfo getMemberInfo(string columnName)
		{
			MemberInfo result = null;
			this.memberMappings.TryGetValue(columnName, out result);
			return result;
		}

		private Dictionary<string, MemberInfo> memberMappings = new Dictionary<string, MemberInfo>();
	}
}
