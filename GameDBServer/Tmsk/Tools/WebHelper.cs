using System;
using System.IO;
using System.Net;
using Server.Tools;

namespace Tmsk.Tools
{
	public static class WebHelper
	{
		public static byte[] RequestByPost(string uri, byte[] bytes, int timeOut = 5000, int readWriteTimeout = 100000)
		{
			int num;
			return WebHelper.RequestByPost(uri, bytes, out num, timeOut, readWriteTimeout);
		}

		public static byte[] RequestByPost(string uri, byte[] bytes, out int httpStatusCode, int timeOut = 5000, int readWriteTimeout = 100000)
		{
			httpStatusCode = 404;
			byte[] result = null;
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
				httpWebRequest.ContentLength = (long)bytes.Length;
				httpWebRequest.KeepAlive = false;
				httpWebRequest.Timeout = timeOut;
				httpWebRequest.ReadWriteTimeout = readWriteTimeout;
				using (Stream requestStream = httpWebRequest.GetRequestStream())
				{
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
					HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					if (null == httpWebResponse)
					{
						return null;
					}
					httpStatusCode = (int)httpWebResponse.StatusCode;
					if (httpStatusCode == 200)
					{
						result = WebHelper.GetBytes(httpWebResponse);
					}
					requestStream.Close();
					httpWebResponse.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = null;
			}
			finally
			{
			}
			return result;
		}

		private static byte[] GetBytes(HttpWebResponse res)
		{
			byte[] array = null;
			try
			{
				Stream responseStream = res.GetResponseStream();
				try
				{
					MemoryStream memoryStream = new MemoryStream();
					array = new byte[256];
					for (int i = responseStream.Read(array, 0, array.Length); i > 0; i = responseStream.Read(array, 0, array.Length))
					{
						memoryStream.Write(array, 0, i);
					}
					array = memoryStream.ToArray();
				}
				finally
				{
					responseStream.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return array;
		}
	}
}
