using System;
using System.IO;

namespace ProtoBuf
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				byte[] array = null;
				if (!false)
				{
					goto IL_27;
				}
				IL_05:
				MemoryStream memoryStream;
				memoryStream.Read(array, 0, array.Length);
				memoryStream.Dispose();
				if (2 != 0)
				{
					memoryStream = null;
					if (-2 != 0)
					{
						goto IL_5D;
					}
				}
				IL_27:
				FriendData friendData = new FriendData();
				friendData.DbID = 11111;
				memoryStream = new MemoryStream();
				Serializer.Serialize<FriendData>(memoryStream, friendData);
				array = new byte[memoryStream.Length];
				memoryStream.Position = 0L;
				if (!false)
				{
					goto IL_05;
				}
				IL_5D:;
			}
			catch (Exception)
			{
			}
		}
	}
}
