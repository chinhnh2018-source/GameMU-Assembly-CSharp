using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class QiRiKuangHuanMergedActivityIcons : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		int num = 0;
		this.XinfuIcon.gameObject.SetActive(false);
		this.HefuIcon.gameObject.SetActive(false);
		this.QirikuanghuanIcon.gameObject.SetActive(false);
		this.GrowFundIcon.gameObject.SetActive(false);
		this.IdBingingIcon.gameObject.SetActive(false);
		if (!Global.IsXinFuActivityEnd())
		{
			this.XinfuIcon.gameObject.SetActive(true);
			int num2 = -100 + num * 65;
			this.XinfuIcon.transform.localPosition = new Vector3((float)num2, 35f, -0.5f);
			if (this.LoadXinFuObj && this.OpenDoubleXinFuTime())
			{
				string path = "UITeXiao/Perfabs/Jiemianhuodongtishi/Xinfuhuodong_UI";
				GameObject gameObject = this.LoadTeXiaoObj(path, this.XinfuIcon.transform);
				if (gameObject)
				{
					gameObject.transform.localPosition = new Vector3(-77.5f, -163f, -10f);
					this.LoadXinFuObj = false;
				}
			}
			num++;
		}
		if (!Global.IsHefuActivityEnd())
		{
			int num3 = -100 + num * 65;
			this.HefuIcon.gameObject.SetActive(true);
			this.HefuIcon.transform.localPosition = new Vector3((float)num3, 35f, -0.5f);
			num++;
		}
		if (!Global.IsQiRiKuangHuanActivityEnd())
		{
			int num4 = -100 + num * 65;
			this.QirikuanghuanIcon.gameObject.SetActive(true);
			this.QirikuanghuanIcon.transform.localPosition = new Vector3((float)num4, 35f, -0.5f);
			num++;
		}
		if (Global.IsOpenGrowFund())
		{
			int num5 = -100 + num * 65;
			this.GrowFundIcon.gameObject.SetActive(true);
			this.GrowFundIcon.transform.localPosition = new Vector3((float)num5, 35f, -0.5f);
			num++;
		}
		this.IdBingingIcon.gameObject.SetActive(false);
		UIEventListener.Get(this.XinfuIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		UIEventListener.Get(this.HefuIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
		};
		UIEventListener.Get(this.QirikuanghuanIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2
			});
		};
		UIEventListener.Get(this.GrowFundIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 3
			});
		};
		UIEventListener.Get(this.IdBingingIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 4
			});
		};
		ActivityTipManager.RegActivityTipItem(6000, new ActivityTipEventHandler(this.XinfuIconActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(12000, new ActivityTipEventHandler(this.HefuIconActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(17000, new ActivityTipEventHandler(this.QiRiKuangHuanTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14109, new ActivityTipEventHandler(this.GrowFundTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14111, new ActivityTipEventHandler(this.IdBingingTipEventHandler));
		base.transform.localPosition = Vector3.zero;
	}

	private new void OnDestroy()
	{
		ActivityTipManager.UnRegActivityTipItem(6000, new ActivityTipEventHandler(this.XinfuIconActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(12000, new ActivityTipEventHandler(this.HefuIconActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(17000, new ActivityTipEventHandler(this.QiRiKuangHuanTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(14109, new ActivityTipEventHandler(this.GrowFundTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(14111, new ActivityTipEventHandler(this.IdBingingTipEventHandler));
	}

	private void XinfuIconActivityTipEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.XinfuTipIcon)
		{
			this.XinfuTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void HefuIconActivityTipEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.HefuTipIcon)
		{
			this.HefuTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void QiRiKuangHuanTipEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.QirikuanghuanTipIcon)
		{
			this.QirikuanghuanTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void GrowFundTipEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.GrowFundTipIcon)
		{
			this.GrowFundTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void IdBingingTipEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.IdBingingIcon)
		{
		}
	}

	private bool OpenDoubleXinFuTime()
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		DateTime dateTime3 = default(DateTime);
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("DoubleXinFu", '|');
		if (systemParamStringArrayByName.Length == 0)
		{
			return false;
		}
		DateTime.TryParse(string.Format("{0}", Global.Data.roleData.KaiFuStartDay), ref dateTime3);
		DateTime.TryParse(systemParamStringArrayByName[0], ref dateTime);
		DateTime.TryParse(systemParamStringArrayByName[1], ref dateTime2);
		return dateTime3.Ticks >= dateTime.Ticks && dateTime3.Ticks <= dateTime2.Ticks && dateTime3.AddDays(9.0).Ticks >= correctDateTime.Ticks;
	}

	private GameObject LoadTeXiaoObj(string Path, Transform parent)
	{
		Object @object = Resources.Load(Path);
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.SetParent(parent, false);
			U3DUtils.ResetLayer(gameObject, "MUUI");
			return gameObject;
		}
		return null;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UIButton XinfuIcon;

	public UIButton HefuIcon;

	public UIButton QirikuanghuanIcon;

	public UIButton GrowFundIcon;

	public UIButton IdBingingIcon;

	public Transform XinfuTipIcon;

	public Transform HefuTipIcon;

	public Transform QirikuanghuanTipIcon;

	public Transform GrowFundTipIcon;

	public Transform IdBinging;

	private bool LoadXinFuObj = true;
}
