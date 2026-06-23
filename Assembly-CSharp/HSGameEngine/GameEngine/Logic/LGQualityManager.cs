using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Tmsk.Xml;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public class LGQualityManager
	{
		public static TMDeviceLevel DeviceLevel
		{
			get
			{
				return LGQualityManager.mDeviceLevel;
			}
		}

		public static TMDeviceLevel GPULevel
		{
			get
			{
				return LGQualityManager.mGPULevel;
			}
		}

		public static TMQualitySetting QualitySetting
		{
			get
			{
				return LGQualityManager.mQualitySetting;
			}
			set
			{
				LGQualityManager.mQualitySetting = value;
				if (LGQualityManager.mQualitySetting == TMQualitySetting.Auto)
				{
					if (LGQualityManager.DeviceLevel == TMDeviceLevel.High)
					{
						LGQualityManager.CurrentQuality = TMRenderQuality.High;
					}
					else if (LGQualityManager.DeviceLevel == TMDeviceLevel.Middle)
					{
						LGQualityManager.CurrentQuality = TMRenderQuality.Middle;
					}
					else
					{
						LGQualityManager.CurrentQuality = TMRenderQuality.Low;
					}
				}
				else
				{
					LGQualityManager.CurrentQuality = (TMRenderQuality)LGQualityManager.mQualitySetting;
				}
			}
		}

		public static TMRenderQuality CurrentQuality
		{
			get
			{
				return LGQualityManager.mCurrentQuality;
			}
			set
			{
				if (LGQualityManager.mCurrentQuality == value)
				{
					return;
				}
				LGQualityManager.mCurrentQuality = value;
				LGQualityManager.ApplyCurrentSetting();
			}
		}

		public static int MaxSkillEffectCount
		{
			get
			{
				return LGQualityManager.mMaxSkillEffectCount;
			}
			set
			{
				LGQualityManager.mMaxSkillEffectCount = value;
			}
		}

		public static bool PostEffectEnabled
		{
			get
			{
				return LGQualityManager.mPostEffectEnabled;
			}
			set
			{
				if (value == LGQualityManager.mPostEffectEnabled)
				{
					return;
				}
				LGQualityManager.mPostEffectEnabled = value;
			}
		}

		public static bool WaterDistortEnabled
		{
			get
			{
				return LGQualityManager.mWaterDistortEnabled;
			}
			set
			{
				if (value == LGQualityManager.mWaterDistortEnabled)
				{
					return;
				}
				LGQualityManager.mWaterDistortEnabled = (value && LGQualityManager.mWaterDistrotSupport);
			}
		}

		public static bool ParticleEnabled
		{
			get
			{
				return LGQualityManager.mParticleEnabled;
			}
			set
			{
				if (value == LGQualityManager.mParticleEnabled)
				{
					return;
				}
				LGQualityManager.mParticleEnabled = value;
			}
		}

		public static int MaxVisiblePlayer
		{
			get
			{
				return LGQualityManager.mMaxVisiblePlayer;
			}
			set
			{
				if (value == LGQualityManager.mMaxVisiblePlayer)
				{
					return;
				}
				LGQualityManager.mMaxVisiblePlayer = value;
				if (LGQualityManager.OnMaxVisiblePlayerChanged != null)
				{
					LGQualityManager.OnMaxVisiblePlayerChanged.Invoke();
				}
			}
		}

		public static int MaxVisiblePet
		{
			get
			{
				return LGQualityManager.mMaxVisiblePet;
			}
			set
			{
				if (value == LGQualityManager.mMaxVisiblePet)
				{
					return;
				}
				LGQualityManager.mMaxVisiblePet = value;
			}
		}

		public static int MaxVisibleWings
		{
			get
			{
				return LGQualityManager.mMaxVisibleWings;
			}
			set
			{
				if (value == LGQualityManager.mMaxVisibleWings)
				{
					return;
				}
				LGQualityManager.mMaxVisibleWings = value;
			}
		}

		public static int MaxVisibleHunQi
		{
			get
			{
				return LGQualityManager.mMaxVisibleHunQi;
			}
			set
			{
				if (value == LGQualityManager.mMaxVisibleHunQi)
				{
					return;
				}
				LGQualityManager.mMaxVisibleHunQi = value;
			}
		}

		public static int MaxVisibleWeaponEff
		{
			get
			{
				return LGQualityManager.mMaxVisibleWeaponEff;
			}
			set
			{
				if (value == LGQualityManager.mMaxVisibleWeaponEff)
				{
					return;
				}
				LGQualityManager.mMaxVisibleWeaponEff = value;
			}
		}

		public static bool ShadowEnabled
		{
			get
			{
				return LGQualityManager.mShadowEnabled;
			}
			set
			{
				if (value == LGQualityManager.mShadowEnabled)
				{
					return;
				}
				LGQualityManager.mShadowEnabled = value;
				if (LGQualityManager.OnShadowSettingChanged != null)
				{
					LGQualityManager.OnShadowSettingChanged.Invoke();
				}
			}
		}

		public static bool CameraBloomEnabled
		{
			get
			{
				return LGQualityManager.mCameraBloomEnabled;
			}
			set
			{
				if (value == LGQualityManager.mCameraBloomEnabled)
				{
					return;
				}
				LGQualityManager.mCameraBloomEnabled = value;
			}
		}

		public static int TargetFPS
		{
			get
			{
				return LGQualityManager.mTargetFPS;
			}
		}

		public static void InitializeWithConfig()
		{
			LGQualityManager.mDefaultHeight = Screen.height;
			LGQualityManager.mCurrentHeight = LGQualityManager.mDefaultHeight;
			LGQualityManager.mCurrentWidth = Screen.width;
			LGQualityManager.mScreenRatio = (float)Screen.width / (float)Screen.height;
			byte[] array = null;
			if (File.Exists(Application.persistentDataPath + "/quality_config.xml"))
			{
				array = File.ReadAllBytes(Application.persistentDataPath + "/quality_config.xml");
			}
			else
			{
				WWW www = new WWW(Application.streamingAssetsPath + "/quality_config.xml");
				while (!www.isDone)
				{
					Thread.Sleep(300);
				}
				if (!string.IsNullOrEmpty(www.error))
				{
					MUDebug.LogError<string>(new string[]
					{
						www.error
					});
				}
				else
				{
					array = www.bytes;
				}
			}
			if (array != null)
			{
				string utf8StringFromBytes = Global.GetUTF8StringFromBytes(array);
				XElement ele = XElement.Parse(utf8StringFromBytes);
				LGQualityManager.mRenderSettings.LoadFrom(ele);
			}
			LGQualityManager.mDeviceLevel = TMDeviceLevel.High;
			int num = 2000;
			int num2 = 0;
			if (num < 1500)
			{
				num2 = 2;
			}
			else if (num < 2000)
			{
				num2 = 1;
			}
			int num3 = 2000;
			int num4 = 0;
			if (num3 < 1000)
			{
				num4 = 2;
			}
			else if (num3 < 2000)
			{
				num4 = 1;
			}
			LGQualityManager.mDeviceLevel = (TMDeviceLevel)((num2 >= num4) ? num2 : num4);
			WWW www2 = new WWW(PathUtils.WebPath("android_dev.xml"));
			while (!www2.isDone)
			{
				Thread.Sleep(300);
			}
			LGQualityManager.mGPULevel = TMDeviceLevel.Low;
			string graphicsDeviceName = SystemInfo.graphicsDeviceName;
			if (string.IsNullOrEmpty(www2.error))
			{
				XElement xelement = XElement.Parse(Global.GetUTF8StringFromBytes(www2.bytes));
				XElement xelement2 = xelement.Element("HighLevel");
				IEnumerator<XElement> enumerator = xelement2.Elements("GPU").GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (graphicsDeviceName.Contains(Global.GetXElementAttributeStr(enumerator.Current, "Name")))
					{
						LGQualityManager.mGPULevel = TMDeviceLevel.High;
						break;
					}
				}
				XElement xelement3 = xelement.Element("MiddleLevel");
				enumerator = xelement3.Elements("GPU").GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (graphicsDeviceName.Contains(Global.GetXElementAttributeStr(enumerator.Current, "Name")))
					{
						LGQualityManager.mGPULevel = TMDeviceLevel.Middle;
						break;
					}
				}
			}
			LGQualityManager.mTargetFPS = 40;
			if (SystemInfo.graphicsDeviceType == 8)
			{
				LGQualityManager.mWaterDistrotSupport = false;
			}
			Application.targetFrameRate = LGQualityManager.mTargetFPS;
			if (LGQualityManager.mGPULevel == TMDeviceLevel.High && SystemInfo.graphicsDeviceType == 8)
			{
				LGQualityManager.mGPULevel = TMDeviceLevel.Middle;
			}
			if (Screen.height > 960 && LGQualityManager.mGPULevel == TMDeviceLevel.Middle)
			{
				LGQualityManager.mDeviceLevel = TMDeviceLevel.Low;
			}
			MUDebug.Log<string>(new string[]
			{
				string.Concat(new object[]
				{
					"Device Level ",
					LGQualityManager.mDeviceLevel,
					" | GPU Level ",
					LGQualityManager.mGPULevel
				})
			});
			if (LGQualityManager.mGPULevel == TMDeviceLevel.High)
			{
				LGQualityManager.mMaxScreenRes = 1080;
				LGQualityManager.mMaxShaderLOD = 300;
			}
			else if (LGQualityManager.mGPULevel == TMDeviceLevel.Middle)
			{
				LGQualityManager.mMaxScreenRes = 960;
				LGQualityManager.mMaxShaderLOD = 200;
			}
			else if (LGQualityManager.mGPULevel == TMDeviceLevel.Low)
			{
				LGQualityManager.mMaxScreenRes = 720;
				LGQualityManager.mMaxShaderLOD = 150;
			}
			LGQualityManager.mCurrentQuality = (TMRenderQuality)LGQualityManager.DeviceLevel;
		}

		public static void ApplyCurrentSetting()
		{
			Shader.globalMaximumLOD = Mathf.Min(LGQualityManager.mRenderSettings.ShaderLOD[(int)LGQualityManager.CurrentQuality], LGQualityManager.mMaxShaderLOD);
			LGQualityManager.ParticleEnabled = LGQualityManager.mRenderSettings.ParticleEnable[(int)LGQualityManager.CurrentQuality];
			LGQualityManager.PostEffectEnabled = (LGQualityManager.mRenderSettings.PostEffectEnable[(int)LGQualityManager.CurrentQuality] && LGQualityManager.GPULevel != TMDeviceLevel.Low);
			LGQualityManager.WaterDistortEnabled = (LGQualityManager.mRenderSettings.WaterDistortEnable[(int)LGQualityManager.CurrentQuality] && LGQualityManager.GPULevel < TMDeviceLevel.Middle);
			LGQualityManager.MaxVisiblePlayer = LGQualityManager.mRenderSettings.MaxVisiblePlayer[(int)LGQualityManager.CurrentQuality];
			LGQualityManager.MaxVisiblePet = LGQualityManager.mRenderSettings.MaxVisiblePet[(int)LGQualityManager.CurrentQuality];
			LGQualityManager.MaxVisibleWings = LGQualityManager.mRenderSettings.MaxVisibleWing[(int)LGQualityManager.CurrentQuality];
			LGQualityManager.MaxVisibleHunQi = LGQualityManager.mRenderSettings.MaxVisibleHunQi[(int)LGQualityManager.CurrentQuality];
			LGQualityManager.MaxVisibleWeaponEff = LGQualityManager.mRenderSettings.MaxVisibleWeaponEff[(int)LGQualityManager.CurrentQuality];
			LGQualityManager.MaxSkillEffectCount = LGQualityManager.mRenderSettings.MaxVisibleSkillEffect[(int)LGQualityManager.CurrentQuality];
			LGQualityManager.CameraBloomEnabled = (LGQualityManager.mRenderSettings.CameraBloomEnable[(int)LGQualityManager.CurrentQuality] && LGQualityManager.GPULevel < TMDeviceLevel.Middle);
			LGQualityManager.ShadowEnabled = LGQualityManager.mRenderSettings.ShadowEnable[(int)LGQualityManager.CurrentQuality];
			if (LGQualityManager.OnPostEffectSettingChanged != null)
			{
				LGQualityManager.OnPostEffectSettingChanged.Invoke();
			}
			LGQualityManager.ResetResolution();
		}

		public static void ApplyLoaderSceneSettings()
		{
			Shader.globalMaximumLOD = LGQualityManager.mRenderSettings.ShaderLODOnLoading[(int)LGQualityManager.CurrentQuality];
			LGQualityManager.ParticleEnabled = LGQualityManager.mRenderSettings.ParticleEnabledOnLoading[(int)LGQualityManager.CurrentQuality];
			LGQualityManager.PostEffectEnabled = (LGQualityManager.GPULevel < TMDeviceLevel.Low);
			LGQualityManager.WaterDistortEnabled = false;
			if (LGQualityManager.OnPostEffectSettingChanged != null)
			{
				LGQualityManager.OnPostEffectSettingChanged.Invoke();
			}
			int num = Mathf.Min(LGQualityManager.mDefaultHeight, LGQualityManager.mMaxScreenRes);
			if (num != LGQualityManager.mCurrentHeight)
			{
				int num2 = Mathf.RoundToInt((float)num * LGQualityManager.mScreenRatio);
				Screen.SetResolution(num2, num, true);
				LGQualityManager.mCurrentHeight = num;
				LGQualityManager.mCurrentWidth = num2;
			}
		}

		private static void ResetResolution()
		{
		}

		public static Action OnShadowSettingChanged;

		public static Action OnPostEffectSettingChanged;

		public static Action OnMaxVisiblePlayerChanged;

		public static TMRenderSettings mRenderSettings = new TMRenderSettings();

		private static TMDeviceLevel mDeviceLevel = TMDeviceLevel.High;

		private static TMDeviceLevel mGPULevel = TMDeviceLevel.High;

		private static TMQualitySetting mQualitySetting = TMQualitySetting.Auto;

		private static TMRenderQuality mCurrentQuality = TMRenderQuality.High;

		private static int mDefaultHeight;

		private static int mCurrentWidth;

		private static int mCurrentHeight;

		private static float mScreenRatio;

		private static int mMaxSkillEffectCount = 1000;

		private static bool mPostEffectEnabled = true;

		private static bool mWaterDistrotSupport = true;

		private static bool mWaterDistortEnabled = true;

		private static bool mParticleEnabled = true;

		private static int mMaxVisiblePlayer = 1000;

		private static int mMaxVisiblePet = 1000;

		private static int mMaxVisibleWings = 1000;

		private static int mMaxVisibleHunQi = 1000;

		private static int mMaxVisibleWeaponEff = 1000;

		private static bool mShadowEnabled = true;

		private static bool mCameraBloomEnabled = true;

		private static int mTargetFPS = 40;

		private static int mMaxShaderLOD = 300;

		private static int mMaxScreenRes = 1080;
	}
}
