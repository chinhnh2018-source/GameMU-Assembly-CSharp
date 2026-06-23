using System;

namespace HSGameEngine.GameEngine.Network
{
	public class SocketConnectEventArgs : EventArgs
	{
		public string RemoteEndPoint
		{
			get
			{
				return this._x2db763e3d9143c7c;
			}
			set
			{
				this._x2db763e3d9143c7c = value;
				SocketConnectEventArgs.x8f8de168ba7486ae = value;
			}
		}

		public string Error
		{
			get
			{
				return this._xdc1af3a17717bf0a;
			}
			set
			{
				this._xdc1af3a17717bf0a = value;
				this._x1416709a7bc0f471 = value;
			}
		}

		public string ErrorStr
		{
			get
			{
				return this._xd29d1f8924057ce0;
			}
			set
			{
				this._xd29d1f8924057ce0 = value;
				this._xe704395988e14adf = value;
			}
		}

		private string _x2db763e3d9143c7c;

		internal static string x8f8de168ba7486ae;

		private string _xdc1af3a17717bf0a;

		internal string _x1416709a7bc0f471;

		private string _xd29d1f8924057ce0;

		internal string _xe704395988e14adf;

		public string ErrorMsg;

		public bool ReturnStartPage;

		public bool ShowMsgBox;

		public int NetSocketType;

		public int CmdID = -1;

		public string[] fields;

		public byte[] bytesData;
	}
}
