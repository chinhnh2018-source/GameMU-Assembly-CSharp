using System;
using System.Text;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public static class SystemInformation
	{
		private static string GetSystemInfoItem(string itemName, object o)
		{
			return string.Format("{0}={1}\n", itemName, o.ToString());
		}

		public static string GetSystemInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("deviceModel", SystemInfo.deviceModel));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("deviceName", SystemInfo.deviceName));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("deviceType", SystemInfo.deviceType));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("deviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("graphicsDeviceID", SystemInfo.graphicsDeviceID));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("graphicsDeviceName", SystemInfo.graphicsDeviceName));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("graphicsDeviceVendorID", SystemInfo.graphicsDeviceVendorID));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("graphicsMemorySize", SystemInfo.graphicsMemorySize));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("graphicsPixelFillrate", SystemInfo.graphicsPixelFillrate));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("graphicsShaderLevel", SystemInfo.graphicsShaderLevel));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("npotSupport", SystemInfo.npotSupport));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("operatingSystem", SystemInfo.operatingSystem));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("processorCount", SystemInfo.processorCount));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("processorType", SystemInfo.processorType));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("supports3DTextures", SystemInfo.supports3DTextures));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("supportsAccelerometer", SystemInfo.supportsAccelerometer));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("supportsComputeShaders", SystemInfo.supportsComputeShaders));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("supportsGyroscope", SystemInfo.supportsGyroscope));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("supportsImageEffects", SystemInfo.supportsImageEffects));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("supportsInstancing", SystemInfo.supportsInstancing));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("supportsLocationService", SystemInfo.supportsLocationService));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("supportsRenderTextures", SystemInfo.supportsRenderTextures));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("supportsShadows", SystemInfo.supportsShadows));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("supportsVibration", SystemInfo.supportsVibration));
			stringBuilder.Append(SystemInformation.GetSystemInfoItem("systemMemorySize", SystemInfo.systemMemorySize));
			return stringBuilder.ToString();
		}

		public static string GetSupportTextureFormats()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 20; i++)
			{
				if (i != 3 && i != 8 && i != 10)
				{
					if (SystemInfo.SupportsRenderTextureFormat(i))
					{
						stringBuilder.AppendFormat("{0}\n", i);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static MobileGPUs GetMobileGPU()
		{
			if (SystemInformation._FirstGetMobileGPU)
			{
				SystemInformation._FirstGetMobileGPU = false;
				string text = SystemInfo.graphicsDeviceName;
				text = text.ToLower();
				for (int i = 0; i < 4; i++)
				{
					string text2 = ((MobileGPUs)i).ToString();
					text2 = text2.ToLower();
					if (text.IndexOf(text2) >= 0)
					{
						SystemInformation._MoblieGPU = (MobileGPUs)i;
					}
				}
			}
			return SystemInformation._MoblieGPU;
		}

		private static bool _FirstGetMobileGPU = true;

		private static MobileGPUs _MoblieGPU = MobileGPUs.Unknown;
	}
}
