using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace KF.Remoting
{
	public class ServiceCompressBehavior : BehaviorExtensionElement, IServiceBehavior
	{
		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
			{
				ChannelDispatcher channelDispatcher = (ChannelDispatcher)channelDispatcherBase;
				foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
				{
					endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new MessageInspector());
				}
			}
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		public override Type BehaviorType
		{
			get
			{
				return typeof(ServiceCompressBehavior);
			}
		}

		protected override object CreateBehavior()
		{
			return new ServiceCompressBehavior();
		}
	}
}
