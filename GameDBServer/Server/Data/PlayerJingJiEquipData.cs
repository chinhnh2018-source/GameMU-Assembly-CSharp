using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class PlayerJingJiEquipData
	{
		public string getStringValue()
		{
			return string.Format("{0},{1},{2},{3}", new object[]
			{
				this.EquipId,
				this.Forge_level,
				this.ExcellenceInfo,
				this.BagIndex
			});
		}

		public static PlayerJingJiEquipData createPlayerJingJiEquipData(string value)
		{
			PlayerJingJiEquipData result;
			if (value == null || value.Equals(""))
			{
				result = null;
			}
			else
			{
				PlayerJingJiEquipData playerJingJiEquipData = new PlayerJingJiEquipData();
				string[] array = value.Split(new char[]
				{
					','
				});
				if (array.Length != 3 && array.Length != 4)
				{
					result = null;
				}
				else
				{
					playerJingJiEquipData.EquipId = Convert.ToInt32(array[0]);
					playerJingJiEquipData.Forge_level = Convert.ToInt32(array[1]);
					playerJingJiEquipData.ExcellenceInfo = Convert.ToInt32(array[2]);
					if (array.Length == 4)
					{
						playerJingJiEquipData.BagIndex = Convert.ToInt32(array[3]);
					}
					result = playerJingJiEquipData;
				}
			}
			return result;
		}

		[ProtoMember(1)]
		public int EquipId;

		[ProtoMember(2)]
		public int Forge_level;

		[ProtoMember(3)]
		public int ExcellenceInfo;

		[ProtoMember(4)]
		public int BagIndex;
	}
}
