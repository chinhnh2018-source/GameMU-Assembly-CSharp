using System;

namespace ProtoBuf.Meta
{
	public sealed class LockContentedEventArgs : EventArgs
	{
		internal LockContentedEventArgs(string ownerStackTrace)
		{
			this.x3f18a5aa1f261013 = ownerStackTrace;
		}

		public string OwnerStackTrace
		{
			get
			{
				return this.x3f18a5aa1f261013;
			}
		}

		private readonly string x3f18a5aa1f261013;
	}
}
