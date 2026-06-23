using System;
using Tmsk.Contract;

namespace Server.Data
{
	public static class ProtoHelper
	{
		public static object Unmarshal<T>(byte[] bytes, int offset, int count) where T : IProtoBuffData, new()
		{
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			try
			{
				if (bytes == null)
				{
					return t;
				}
				t.fromBytes(bytes, offset, count);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return t;
		}
	}
}
