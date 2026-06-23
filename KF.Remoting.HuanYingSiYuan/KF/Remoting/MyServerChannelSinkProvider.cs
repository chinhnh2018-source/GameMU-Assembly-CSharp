using System;
using System.Collections;
using System.Runtime.Remoting.Channels;

namespace KF.Remoting
{
	public class MyServerChannelSinkProvider : IServerChannelSinkProvider
	{
		public MyServerChannelSinkProvider(IDictionary properties, ICollection providerData)
		{
		}

		public IServerChannelSink CreateSink(IChannelReceiver channel)
		{
			IServerChannelSink nextSink = null;
			if (this.NextSink != null)
			{
				nextSink = (this.NextSink as IServerChannelSinkProvider).CreateSink(channel);
			}
			return new MyServerChannelSink(nextSink);
		}

		public void GetChannelData(IChannelDataStore channelData)
		{
		}

		public IServerChannelSinkProvider Next
		{
			get
			{
				return this.NextSink as IServerChannelSinkProvider;
			}
			set
			{
				this.NextSink = value;
			}
		}

		private object NextSink;
	}
}
