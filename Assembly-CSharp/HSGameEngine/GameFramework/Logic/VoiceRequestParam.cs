using System;
using UnityEngine;

namespace HSGameEngine.GameFramework.Logic
{
	public class VoiceRequestParam
	{
		public string url;

		public Action<WWW, object> callback;

		public byte[] postData;

		public object param;

		public int timeout;

		public float Length;
	}
}
