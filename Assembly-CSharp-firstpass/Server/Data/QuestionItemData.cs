using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class QuestionItemData
	{
		[ProtoMember(1)]
		public int QuestionId;

		[ProtoMember(2)]
		public string Question;

		[ProtoMember(3)]
		public string[] AnswerContentArray;

		[ProtoMember(4)]
		public bool UseTianShi;

		[ProtoMember(5)]
		public bool UseEMo;

		[ProtoMember(6)]
		public int RoleAnswer;

		[ProtoMember(7)]
		public DateTime EndTime;

		[ProtoMember(8)]
		public bool[] QuestionState;

		[ProtoMember(9)]
		public int RolePoint;
	}
}
