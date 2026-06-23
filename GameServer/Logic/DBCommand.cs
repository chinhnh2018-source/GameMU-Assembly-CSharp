using System;

namespace GameServer.Logic
{
	public class DBCommand
	{
		public int DBCommandID { get; set; }

		public string DBCommandText { get; set; }

		public event DBCommandEventHandler DBCommandEvent;

		public void DoDBCommandEvent(DBCommandEventArgs e)
		{
			if (null != this.DBCommandEvent)
			{
				this.DBCommandEvent(this, e);
			}
		}

		public int ServerId;
	}
}
