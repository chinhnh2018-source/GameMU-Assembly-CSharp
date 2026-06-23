using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class DJPointsData
	{
		[ProtoMember(1)]
		public DJPointData SelfDJPointData;

		[ProtoMember(2)]
		public List<DJPointData> HotDJPointDataList;
	}
}
