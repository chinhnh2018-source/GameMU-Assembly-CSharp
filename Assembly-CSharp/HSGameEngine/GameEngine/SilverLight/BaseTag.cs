using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class BaseTag
	{
		public BaseTag(int id)
		{
			this.ID = id;
		}

		public BaseTag(string tag, int id = 0, int type = 0)
		{
			this.Tag = tag;
			this.ID = id;
			this.Type = type;
		}

		public string Tag { get; set; }

		public int Type { get; set; }

		public int ID { get; set; }
	}
}
