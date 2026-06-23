using System;
using System.Runtime.Remoting.Channels;

namespace KF.Remoting
{
	public class MyClientChannelSinkProvider : IClientChannelSinkProvider
	{
		public void GetChannelData(IChannelDataStore channelData)
		{
		}

		public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
		{
			IClientChannelSink clientChannelSink = null;
			if (this.NextSink != null)
			{
				clientChannelSink = (this.NextSink as IClientChannelSinkProvider).CreateSink(channel, url, remoteChannelData);
				if (clientChannelSink == null)
				{
					return null;
				}
			}
			return new MyClientChannelSink(clientChannelSink);
		}

		IClientChannelSinkProvider IClientChannelSinkProvider.Next
		{
			get
			{
				return this.NextSink as IClientChannelSinkProvider;
			}
			set
			{
				this.NextSink = value;
			}
		}

		private object NextSink;
	}
}
