using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using GameServer.Core.Executor;

namespace KF.Remoting
{
	public class MessageInspector : IDispatchMessageInspector
	{
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			string action = request.Headers.Action;
			object result;
			if (action.Length > 20)
			{
				MessageInspector.cmdInfo cmdInfo = new MessageInspector.cmdInfo
				{
					cmdName = action.Substring(19),
					cmdSize = 0,
					receiveTicks = TimeUtil.NOW()
				};
				result = cmdInfo;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			MessageInspector.cmdInfo cmdInfo = (MessageInspector.cmdInfo)correlationState;
			if (null != cmdInfo)
			{
				CmdMonitor.RecordCmdDetail(cmdInfo.cmdName, TimeUtil.NOW() - cmdInfo.receiveTicks, (long)cmdInfo.cmdSize, TCPProcessCmdResults.RESULT_OK);
			}
		}

		public static string GetHeader(string headerName)
		{
			string result;
			if (OperationContext.Current.IncomingMessageHeaders.FindHeader(headerName, "http://tempuri.org") >= 0)
			{
				result = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(headerName, "http://tempuri.org");
			}
			else
			{
				result = null;
			}
			return result;
		}

		private class cmdInfo
		{
			public long receiveTicks;

			public int cmdSize;

			public string cmdName;
		}
	}
}
