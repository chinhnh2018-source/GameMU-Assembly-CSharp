using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class PlayerJingJiSkillData
	{
		public string getStringValue()
		{
			return string.Format("{0},{1}", this.skillID, this.skillLevel);
		}

		public static PlayerJingJiSkillData createPlayerJingJiSkillData(string value)
		{
			PlayerJingJiSkillData result;
			if (value == null || value.Equals(""))
			{
				result = null;
			}
			else
			{
				string[] array = value.Split(new char[]
				{
					','
				});
				if (array.Length != 2)
				{
					result = null;
				}
				else
				{
					result = new PlayerJingJiSkillData
					{
						skillID = Convert.ToInt32(array[0]),
						skillLevel = Convert.ToInt32(array[1])
					};
				}
			}
			return result;
		}

		[ProtoMember(1)]
		public int skillID;

		[ProtoMember(2)]
		public int skillLevel;
	}
}
