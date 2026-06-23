using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using GameServer.Core.Executor;
using Server.Tools;

namespace KF.Remoting
{
	public class MyServerChannelSink : IServerChannelSink, IChannelSinkBase
	{
		public IDictionary Properties
		{
			get
			{
				return null;
			}
		}

		public IServerChannelSink NextChannelSink
		{
			get
			{
				return this.nextSink;
			}
		}

		public MyServerChannelSink(object nextSink)
		{
			this.nextSink = (nextSink as IServerChannelSink);
		}

		public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			this.nextSink.AsyncProcessResponse(sinkStack, state, msg, headers, stream);
		}

		public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
		{
			return this.nextSink.GetResponseStream(sinkStack, state, msg, headers);
		}

		ServerProcessing IServerChannelSink.ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			long num = TimeUtil.NOW();
			string text = null;
			long dataSize = 0L;
			if (requestStream != null && requestHeaders != null)
			{
				text = requestHeaders["__RequestUri"].ToString();
			}
			ServerProcessing result = this.nextSink.ProcessMessage(sinkStack, requestMsg, requestHeaders, requestStream, out responseMsg, out responseHeaders, out responseStream);
			if (null != responseMsg)
			{
				object obj = responseMsg.Properties["__MethodName"];
				if (text != null && obj != null)
				{
					string cmdName = text.ToString() + obj.ToString();
					CmdMonitor.RecordCmdDetail(cmdName, TimeUtil.NOW() - num, dataSize, responseStream.Length, TCPProcessCmdResults.RESULT_OK);
				}
				else
				{
					LogManager.WriteExceptionUseCache(string.Format("IServerChannelSink.ProcessMessage#uri={0},methodName={1}", text, obj));
				}
			}
			else
			{
				LogManager.WriteExceptionUseCache(string.Format("IServerChannelSink.ProcessMessage#uri={0},responseMsg=null", text));
			}
			return result;
		}

		private IServerChannelSink nextSink;
	}
}
