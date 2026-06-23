using System;
using MySQLDriverCS;

namespace GameDBServer.DB
{
	public class CommandInfo
	{
		private event EventHandler _solicitationEvent;

		public event EventHandler SolicitationEvent
		{
			add
			{
				this._solicitationEvent += value;
			}
			remove
			{
				this._solicitationEvent -= value;
			}
		}

		public void OnSolicitationEvent()
		{
			if (this._solicitationEvent != null)
			{
				this._solicitationEvent(this, new EventArgs());
			}
		}

		public CommandInfo()
		{
		}

		public CommandInfo(string sqlText, MySQLParameter[] para)
		{
			this.CommandText = sqlText;
			this.Parameters = para;
		}

		public CommandInfo(string sqlText, MySQLParameter[] para, EffentNextType type)
		{
			this.CommandText = sqlText;
			this.Parameters = para;
			this.EffentNextType = type;
		}

		public object ShareObject = null;

		public object OriginalData = null;

		public string CommandText;

		public MySQLParameter[] Parameters;

		public EffentNextType EffentNextType = EffentNextType.None;
	}
}
