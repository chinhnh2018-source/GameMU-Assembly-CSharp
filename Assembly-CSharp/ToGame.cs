using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ToGame : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.GongGaoBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_GonggaoWindow = U3DUtils.NEW<GChildWindow>();
			this.m_GonggaoWindow.transform.parent = this.Root.parent;
			this.m_GonggaoWindow.transform.localPosition = new Vector3(0f, 0f, -4f);
			this.m_GonggaoWindow.transform.localScale = new Vector3(1f, 1f, 1f);
			this.m_GonggaoWindow.ModalType = ChildWindowModalType.TransBak;
			this.m_GonggaoPart = U3DUtils.NEW<WebGongGao>();
			this.m_GonggaoPart.transform.parent = this.m_GonggaoWindow.Body.transform;
			this.m_GonggaoPart.transform.localPosition = new Vector3(0f, 0f, 0f);
			this.m_GonggaoPart.transform.localScale = new Vector3(1f, 1f, 1f);
			this.m_GonggaoPart.DPSelectedItem = delegate(object a, DPSelectedItemEventArgs d)
			{
				if (d.IDType == -10)
				{
					Object.Destroy(this.m_GonggaoPart.gameObject);
					Object.Destroy(this.m_GonggaoWindow.gameObject);
					this.m_GonggaoWindow = null;
					this.m_GonggaoPart = null;
				}
			};
		};
		this.RegisterUserNameBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.RegisterUserNameBtn_MouseLeftButtonUp);
		this.EnterGameBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.EnterGameBtn_MouseLeftButtonUp);
		if (this.Is3DBackground)
		{
			base.StartCoroutine<bool>(this.Init3DMap());
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		if (null == Global.RoleSel3DBakMapLoader && Global.RoleSel3DBakMapWWW != null)
		{
			Global.RoleSel3DBakMapWWW = null;
		}
		if (null == Global.RoleCreate3DBakMapLoader && Global.RoleCreate3DBakMapWWW != null)
		{
			Global.RoleCreate3DBakMapWWW = null;
		}
	}

	private void RegisterUserNameBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.RegAccount != null)
		{
			this.RegAccount(this, new NextStepEventArgs
			{
				StepType = 0
			});
		}
	}

	private void EnterGameBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.NextStep != null)
		{
			this.NextStep(this, new NextStepEventArgs
			{
				StepType = 0
			});
		}
	}

	private IEnumerator Init3DMap()
	{
		bool initScene = false;
		AssetBundle CurrentMapLoader = null;
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
			CurrentMapLoader = www.assetBundle;
			Global.Login3DBakMapLoader = CurrentMapLoader;
			initScene = true;
			www.Dispose();
			www = null;
		}
		else
		{
			CurrentMapLoader = Global.Login3DBakMapLoader;
		}
		if (initScene)
		{
			Global.AudioListener43D.enabled = false;
			Global.MainCamera.transform.localPosition = new Vector3(8f, 2f, 98f);
			Global.MainCamera.transform.localRotation = Quaternion.Euler(0.597f, 181.89f, -0.6f);
			Global.MainCamera.farClipPlane = 2000f;
			if (Global.IsTuiGuangFenBao)
			{
				Global.MainCamera.fieldOfView = 21f;
			}
			else
			{
				Global.MainCamera.fieldOfView = 35f;
			}
			LoadingMap.ClearSpeicalMapEffect();
			PerformanceCtrl.PerformanceType = PerformanceTypes.HiUsage;
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
		}
		string skeletonName = null;
		skeletonName = Global.GetSkeletonNameByOccupation(0);
		U3DUtils.LoadSkeletonByName(skeletonName, true);
		skeletonName = Global.GetSkeletonNameByOccupation(1);
		U3DUtils.LoadSkeletonByName(skeletonName, true);
		skeletonName = Global.GetSkeletonNameByOccupation(2);
		U3DUtils.LoadSkeletonByName(skeletonName, true);
		skeletonName = Global.GetSkeletonNameByOccupation(3);
		U3DUtils.LoadSkeletonByName(skeletonName, true);
		skeletonName = Global.GetSkeletonNameByOccupation(5);
		U3DUtils.LoadSkeletonByName(skeletonName, true);
		WWW www2 = new WWW(PathUtils.WebPath("Map/chuangjue.unity3d"));
		Global.RoleCreate3DBakMapWWW = www2;
		yield return www2;
		if (!string.IsNullOrEmpty(www2.error))
		{
			yield break;
		}
		Global.RoleCreate3DBakMapLoader = www2.assetBundle;
		www2.Dispose();
		www2 = null;
		WWW www3 = new WWW(PathUtils.WebPath("Map/xuanjue.unity3d"));
		Global.RoleSel3DBakMapWWW = www3;
		yield return www3;
		if (!string.IsNullOrEmpty(www3.error))
		{
			yield break;
		}
		Global.RoleSel3DBakMapLoader = www3.assetBundle;
		www3.Dispose();
		www3 = null;
		yield break;
	}

	public GButton RegisterUserNameBtn;

	public GButton EnterGameBtn;

	public GButton GongGaoBtn;

	public Transform Root;

	public new ShowNetImage Background;

	private GChildWindow m_GonggaoWindow;

	private WebGongGao m_GonggaoPart;

	public NextStepEventHandler NextStep;

	public NextStepEventHandler RegAccount;

	private bool Is3DBackground = true;
}
