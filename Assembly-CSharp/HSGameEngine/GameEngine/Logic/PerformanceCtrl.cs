using System;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public static class PerformanceCtrl
	{
		public static PerformanceTypes PerformanceType
		{
			get
			{
				return PerformanceCtrl._PerformanceType;
			}
			set
			{
				PerformanceCtrl.ChangePerformanceType(value);
			}
		}

		public static bool Fog { get; set; }

		public static void CopyCameraParmasForDengLu()
		{
			GameObject gameObject = GameObject.Find("CameraParams");
			if (gameObject != null && Global.MainCamera != null)
			{
				Component component = Global.MainCamera.GetComponent("ColorCorrectionCurves");
				if (null != component)
				{
					Component component2 = gameObject.GetComponent("ColorCorrectionCurves");
					if (null != component2)
					{
						string[] fieldNames = new string[]
						{
							"saturation",
							"mode",
							"redChannel",
							"greenChannel",
							"blueChannel",
							"depthRedChannel",
							"depthGreenChannel",
							"depthBlueChannel",
							"zCurve",
							"selectiveCc"
						};
						U3DUtils.CopyComponent(component2, component, fieldNames);
						(component as MonoBehaviour).enabled = true;
					}
				}
				Component component3 = Global.MainCamera.GetComponent("BloomOptimized");
				if (component3 != null)
				{
					Component component4 = gameObject.GetComponent("BloomOptimized");
					if (null != component4)
					{
						string[] fieldNames2 = new string[]
						{
							"threshold",
							"intensity",
							"blurSize",
							"blurIterations",
							"blurType"
						};
						U3DUtils.CopyComponent(component4, component3, fieldNames2);
						(component3 as MonoBehaviour).enabled = true;
					}
				}
			}
		}

		public static void ResetNormalSceneSettings()
		{
			Component component = Global.MainCamera.GetComponent("ColorCorrectionCurves");
			if (null != component)
			{
				(component as MonoBehaviour).enabled = false;
			}
			Component component2 = Global.MainCamera.GetComponent("Bloom");
			if (null != component2)
			{
				(component2 as MonoBehaviour).enabled = false;
			}
			Component component3 = Global.MainCamera.GetComponent("BloomAndLensFlares");
			if (null != component3)
			{
				(component3 as MonoBehaviour).enabled = false;
			}
			Component component4 = Global.MainCamera.GetComponent("Vignetting");
			if (null != component4)
			{
				(component4 as MonoBehaviour).enabled = false;
			}
			Component component5 = Global.MainCamera.GetComponent("BloomOptimized");
			if (component5 != null)
			{
				(component5 as MonoBehaviour).enabled = false;
			}
		}

		private static void ChangePerformanceType(PerformanceTypes performanceType)
		{
			PerformanceCtrl._PerformanceType = performanceType;
			U3DUtils.ChangePerformanceTypeShader(null, performanceType);
			Component component = Global.MainCamera.GetComponent("ColorCorrectionCurves");
			if (null != component)
			{
				(component as MonoBehaviour).enabled = false;
			}
			Component component2 = Global.MainCamera.GetComponent("Vignetting");
			if (null != component2)
			{
				(component2 as MonoBehaviour).enabled = false;
			}
			Component component3 = Global.MainCamera.GetComponent("Bloom");
			if (null != component3)
			{
				(component3 as MonoBehaviour).enabled = false;
			}
			Component component4 = Global.MainCamera.GetComponent("BloomOptimized");
			if (null != component4)
			{
				(component4 as MonoBehaviour).enabled = false;
			}
			Component component5 = Global.MainCamera.GetComponent("BloomAndLensFlares");
			if (null != component5)
			{
				(component5 as MonoBehaviour).enabled = false;
			}
			Component component6 = Global.MainCamera.GetComponent("Animator");
			if (null != component6)
			{
				(component6 as Animator).enabled = false;
			}
			Global.MainCamera.backgroundColor = Color.black;
			if (PerformanceCtrl._PerformanceType == PerformanceTypes.HiUsage)
			{
				UILabel.enableEffect = true;
				RenderSettings.fog = PerformanceCtrl.Fog;
				GameObject gameObject = GameObject.Find("/CameraParams");
				if (null != gameObject)
				{
					Camera camera = gameObject.GetComponent("Camera") as Camera;
					if (null != camera)
					{
						Global.MainCamera.backgroundColor = camera.backgroundColor;
						Global.MainCamera.farClipPlane = Math.Min(45f, camera.farClipPlane);
					}
					Component component7 = gameObject.GetComponent("ColorCorrectionCurves");
					if (null != component7)
					{
						string[] fieldNames = new string[]
						{
							"saturation",
							"mode",
							"redChannel",
							"greenChannel",
							"blueChannel",
							"depthRedChannel",
							"depthGreenChannel",
							"depthBlueChannel",
							"zCurve",
							"selectiveCc"
						};
						U3DUtils.CopyComponent(component7, component, fieldNames);
						(component as MonoBehaviour).enabled = true;
					}
					Component component8 = gameObject.GetComponent("Bloom");
					if (null != component8)
					{
						string[] fieldNames2 = new string[]
						{
							"quality",
							"tweakMode",
							"screenBlendMode",
							"hdr",
							"bloomIntensity",
							"bloomThreshold",
							"bloomThresholdColor",
							"bloomBlurIterations",
							"sepBlurSpread",
							"lensflareMode",
							"lensflareIntensity",
							"lensflareThreshold"
						};
						U3DUtils.CopyComponent(component8, component3, fieldNames2);
						(component3 as MonoBehaviour).enabled = true;
					}
					Component component9 = gameObject.GetComponent("BloomOptimized");
					if (null != component9)
					{
						string[] fieldNames3 = new string[]
						{
							"threshold",
							"intensity",
							"blurSize",
							"blurIterations",
							"blurType"
						};
						U3DUtils.CopyComponent(component9, component4, fieldNames3);
						(component4 as MonoBehaviour).enabled = true;
					}
					Component component10 = gameObject.GetComponent("BloomAndLensFlares");
					if (null != component10)
					{
						string[] fieldNames4 = new string[]
						{
							"tweakMode",
							"screenBlendMode",
							"hdr",
							"bloomIntensity",
							"bloomThreshhold",
							"sepBlurSpread"
						};
						U3DUtils.CopyComponent(component10, component5, fieldNames4);
						(component5 as MonoBehaviour).enabled = true;
					}
					Component component11 = gameObject.GetComponent("Vignetting");
					if (null != component11)
					{
						string[] fieldNames5 = new string[]
						{
							"intensity",
							"blur",
							"mode",
							"chromaticAberration"
						};
						U3DUtils.CopyComponent(component11, component2, fieldNames5);
						(component2 as MonoBehaviour).enabled = true;
					}
					Component component12 = gameObject.GetComponent("Animator");
					if (null != component12)
					{
						string[] fieldNames6 = new string[]
						{
							"runtimeAnimatorController"
						};
						U3DUtils.CopyComponent(component12, component6, fieldNames6);
						(component6 as Animator).enabled = true;
					}
				}
			}
			else if (PerformanceCtrl._PerformanceType == PerformanceTypes.LowUsage)
			{
				RenderSettings.fog = false;
				UILabel.enableEffect = false;
			}
		}

		private static PerformanceTypes _PerformanceType;
	}
}
