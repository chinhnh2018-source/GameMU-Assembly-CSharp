using System;
using System.Collections.Generic;

namespace Server.TCP
{
	public class FullBufferManager
	{
		public static string GetErrorStr(int errorCode)
		{
			string result = "未知";
			switch (errorCode)
			{
			case 0:
				result = "发送数据超时";
				break;
			case 1:
				result = "发送缓冲区已经满";
				break;
			case 2:
				result = "缓冲区过半，大数据包被丢弃";
				break;
			}
			return result;
		}

		public void Remove(TMSKSocket s)
		{
			if (this.ErrorDict.Count > 0)
			{
				lock (this.ErrorDict)
				{
					this.ErrorDict.Remove(s);
				}
			}
		}

		public void Add(TMSKSocket s, int iError)
		{
			lock (this.ErrorDict)
			{
				if (!this.ErrorDict.ContainsKey(s))
				{
					this.ErrorDict.Add(s, iError);
				}
				else
				{
					this.ErrorDict[s] = iError;
				}
			}
		}

		public string GetFullBufferInfoStr()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (this.ErrorDict.Count > 0)
			{
				this.ListError.Clear();
				lock (this.ErrorDict)
				{
					this.ListError.AddRange(this.ErrorDict.Values);
				}
				using (List<int>.Enumerator enumerator = this.ListError.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						switch (enumerator.Current)
						{
						case 0:
							num++;
							break;
						case 1:
							num2++;
							break;
						case 2:
							num3++;
							break;
						default:
							num4++;
							break;
						}
					}
				}
			}
			return string.Format("发送超时{0}个, 缓冲区满{1}个, 丢弃大数据包{2}个, 未知{3}个", new object[]
			{
				num,
				num2,
				num3,
				num4
			});
		}

		public const int Error_SendTimeOut = 0;

		public const int Error_BufferFull = 1;

		public const int Error_DiscardBigPacket = 2;

		private Dictionary<TMSKSocket, int> ErrorDict = new Dictionary<TMSKSocket, int>();

		private List<int> ListError = new List<int>();
	}
}
