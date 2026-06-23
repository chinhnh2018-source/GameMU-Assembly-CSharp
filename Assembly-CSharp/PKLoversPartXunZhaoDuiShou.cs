using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class PKLoversPartXunZhaoDuiShou : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.Init();
		try
		{
			if (this.Close.transform.GetChild(1).name.Equals("Label"))
			{
				this.Close.transform.GetChild(1).gameObject.GetComponent<UILabel>().text = Global.GetLang("取消");
				this.Time.transform.localPosition = new Vector3(70f, 60f, this.Time.transform.localPosition.z);
			}
		}
		catch
		{
			MUDebug.Log<string>(new string[]
			{
				"越南东南亚英文，可能报空"
			});
		}
		UIEventListener.Get(this.Close.gameObject).onClick = delegate(GameObject s)
		{
			GameInstance.Game.SendEnterGameInfoForPKLovers(this.gameID, 0);
			GameInstance.Game.SendQuitJionInfoForPKLovers();
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.StartUITimer();
	}

	public void EnterInfo(int gameID)
	{
		this.gameID = gameID;
		this.count = 3;
		this.canenter = true;
	}

	private void Init()
	{
		this.DaoJiShi.gameObject.SetActive(true);
		this.Sorry.gameObject.SetActive(false);
		this.ZhaoDaoDuiShou.gameObject.SetActive(false);
	}

	protected void StartUITimer()
	{
		this.UITimer = new DispatcherTimer("zhongshenzhengbaPart_Timer");
		this.UITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		this.UITimer.Start();
	}

	private void StopTimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Tick = null;
			this.UITimer.Stop();
			this.UITimer = null;
		}
	}

	protected void UITimer_Tick(object sender, object e)
	{
		if (!this.canenter)
		{
			if (this.count >= 0)
			{
				this.Time.text = this.count.ToString();
				this.count--;
			}
			else
			{
				this.StopTimer();
				this.Time.text = null;
				this.DaoJiShi.gameObject.SetActive(false);
				this.Sorry.gameObject.SetActive(true);
				this.ZhaoDaoDuiShou.gameObject.SetActive(false);
				Super.HintMainText(Global.GetLang("没有匹配到对手，请稍后再试!"), 10, 3);
			}
		}
		else
		{
			if (this.count >= 0)
			{
				this.Time.text = this.count.ToString();
				this.count--;
			}
			else
			{
				this.StopTimer();
				GameInstance.Game.SendEnterGameInfoForPKLovers(this.gameID, 1);
			}
			this.DaoJiShi.gameObject.SetActive(false);
			this.Sorry.gameObject.SetActive(false);
			this.ZhaoDaoDuiShou.gameObject.SetActive(true);
		}
	}

	public override void Destroy()
	{
		this.StopTimer();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public UIButton Close;

	public UILabel Time;

	public UISprite DaoJiShi;

	public UISprite Sorry;

	public UISprite ZhaoDaoDuiShou;

	private int count = 60;

	private bool canenter;

	private int gameID;

	private DispatcherTimer UITimer;
}
