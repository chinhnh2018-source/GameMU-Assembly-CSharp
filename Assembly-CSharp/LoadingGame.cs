using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class LoadingGame : UserControl
{
	private void InitTextInPrefabs()
	{
		if (Context.IsHaiwai)
		{
			this.TextHint.Text = Global.GetLang("载入游戏中...");
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.HideLogo();
		GameConfigManager.NextStep = delegate(object s, NextStepEventArgs e)
		{
			GameConfigManager.NextStep = null;
			base.StartCoroutine<bool>(this.InitMainScene());
		};
		base.StartCoroutine<bool>(GameConfigManager.InitVersionConfig());
	}

	private void HideLogo()
	{
		if (Global.IsTuiGuangFenBao)
		{
			Transform transform = base.transform.FindChild("Logo");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private IEnumerator InitMainScene()
	{
		if (Global.IsOpenABLoadAnim && Context.IsHaiwai)
		{
			for (int i = 0; i < 6; i++)
			{
				if (i != 4)
				{
					string skeletonName = Global.GetSkeletonNameByOccupation(i);
					yield return base.StartCoroutine<bool>(U3DUtils.LoadSekletonPrefab(skeletonName));
				}
			}
		}
		Super.InitRootParams();
		Super.InitVersions();
		Super.InitParams();
		yield return base.StartCoroutine(MainGame._current.LoadShareBundle());
		yield return base.StartCoroutine(MainGame._current.LoadEffectShareBundle());
		yield return base.StartCoroutine(BackStageDownloadManager.instance.InitDownLoadInfo());
		if (this.NextStep == null)
		{
			yield return null;
		}
		if (null == Global.Login3DBakMapLoader)
		{
			WWW www = null;
			if (Global.IsTuiGuangFenBao)
			{
				www = new WWW(PathUtils.WebPath("Map/denglu_TG.unity3d"));
			}
			else
			{
				www = new WWW(PathUtils.WebPath("Map/Denglu.unity3d"));
			}
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				yield break;
			}
			Global.Login3DBakMapLoader = www.assetBundle;
			www.Dispose();
			www = null;
		}
		Global.AudioListener43D.enabled = false;
		Global.MainCamera.transform.localPosition = new Vector3(8f, 2f, 98f);
		Global.MainCamera.transform.localRotation = Quaternion.Euler(0.597f, 181.89f, -0.6f);
		Global.MainCamera.far = 2000f;
		if (Global.IsTuiGuangFenBao)
		{
			Global.MainCamera.fieldOfView = 21f;
		}
		else
		{
			Global.MainCamera.fieldOfView = 35f;
		}
		LayerCullDistanceslMgr.SetCameraLayerDistance(Global.MainCamera, 1000f);
		string levelName = null;
		if (Global.IsTuiGuangFenBao)
		{
			levelName = "denglu_TG";
		}
		else
		{
			levelName = "denglu";
		}
		AsyncOperation asyncOperation = Application.LoadLevelAsync(levelName);
		Global.DirectLight.enabled = false;
		Global.IsInGameScene = false;
		yield return asyncOperation;
		PerformanceCtrl.CopyCameraParmasForDengLu();
		if (this.NextStep != null)
		{
			this.NextStep(this, new NextStepEventArgs
			{
				StepType = 0
			});
		}
		yield break;
	}

	public NextStepEventHandler NextStep;

	public TextBlock TextHint;
}
