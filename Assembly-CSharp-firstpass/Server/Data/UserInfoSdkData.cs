using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class UserInfoSdkData
	{
		public UserInfoSdkData()
		{
			this._RoleID = 0;
			this.username = string.Empty;
			this.password = string.Empty;
			this.uid = string.Empty;
			this.otherInfo = new List<UserInfoSdkData>();
			this.bindPhoneNo = string.Empty;
			this.bindEmail = string.Empty;
		}

		[ProtoMember(1)]
		public int _RoleID;

		[ProtoMember(2)]
		public string username;

		[ProtoMember(3)]
		public string password;

		[ProtoMember(4)]
		public string uid;

		[ProtoMember(5)]
		public List<UserInfoSdkData> otherInfo;

		[ProtoMember(6)]
		public string bindPhoneNo;

		[ProtoMember(7)]
		public string bindEmail;
	}
}
