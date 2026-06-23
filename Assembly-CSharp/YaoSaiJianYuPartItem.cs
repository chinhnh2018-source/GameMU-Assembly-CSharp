using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class YaoSaiJianYuPartItem : UserControl
{
	public int RoleID
	{
		get
		{
			return this.roleid;
		}
		set
		{
			this.roleid = value;
		}
	}

	public string SetTouXiang
	{
		set
		{
			this.Touxiang.URL = value;
		}
	}

	public string SetName
	{
		get
		{
			return this.setname;
		}
		set
		{
			this.setname = value;
			if (value == null)
			{
				this.name.text = Global.GetColorStringForNGUIText(new object[]
				{
					"808081",
					Global.GetLang("暂无俘虏")
				});
			}
			else
			{
				this.name.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					value
				});
			}
		}
	}

	public string SetLevel
	{
		set
		{
			this.level.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				value
			});
		}
	}

	public string SetServer
	{
		set
		{
			this.server.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				value
			});
		}
	}

	private void InitTextInPrefabs()
	{
		this.BtnLaoDong.Label.text = Global.GetLang("劳动");
		this.BtnShiFang.Label.text = Global.GetLang("释放");
		this.BtnFind.Label.text = Global.GetLang("寻找俘虏");
		this.xiuxi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			Global.GetLang("休息中...")
		});
		this.xiaohao.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			Global.GetLang("消耗:")
		});
		this.xiaohaonum.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			ConfigSystemParam.GetSystemParamByName("ManorSearchCost", true).SafeToInt32(0)
		});
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitState();
		this.SetState(2);
		this.BtnLaoDong.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowYaoSaiJianYuPartHuDongWindow();
		};
		this.BtnShiFang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string message = string.Format(Global.GetLang("你确定要释放你的俘虏{0}吗？"), this.SetName);
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					GameInstance.Game.SendPrisonFreeData(this.RoleID);
					Super.ShowNetWaiting(null);
				}
			}, buttons);
		};
		this.BtnFind.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.CurrentCopyTeamData != null)
			{
				Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
				{
					if (e2.ID == 0)
					{
						GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
						this.DPSelectItem(this, new DPSelectedItemEventArgs());
					}
				}, -1);
			}
			else
			{
				this.DPSelectItem(this, new DPSelectedItemEventArgs());
			}
		};
	}

	public void InitData(int state, long timelen = 0L)
	{
		this.InitState();
		this.SetState(state);
		this.StopTimer();
		if (state == 0)
		{
			this.countdowntimes = (double)timelen;
			this.StartUITimer();
		}
	}

	private void InitState()
	{
		try
		{
			if (this.name)
			{
				this.name.gameObject.SetActive(false);
			}
			if (this.level)
			{
				this.level.gameObject.SetActive(false);
			}
			if (this.server)
			{
				this.server.gameObject.SetActive(false);
			}
			if (this.xiuxi)
			{
				this.xiuxi.gameObject.SetActive(false);
			}
			if (this.time)
			{
				this.time.gameObject.SetActive(false);
			}
			if (this.xiaohao)
			{
				this.xiaohao.gameObject.SetActive(false);
			}
			if (this.xiaohaonum)
			{
				this.xiaohaonum.gameObject.SetActive(false);
			}
			if (this.shuijiao)
			{
				this.shuijiao.SetActive(false);
			}
			if (this.jinbi)
			{
				this.jinbi.gameObject.SetActive(false);
			}
			if (this.timeBG)
			{
				this.timeBG.gameObject.SetActive(false);
			}
			if (this.jinbiBG)
			{
				this.jinbiBG.gameObject.SetActive(false);
			}
			if (this.BtnLaoDong)
			{
				this.BtnLaoDong.gameObject.SetActive(false);
			}
			if (this.BtnShiFang)
			{
				this.BtnShiFang.gameObject.SetActive(false);
			}
			if (this.BtnFind)
			{
				this.BtnFind.gameObject.SetActive(false);
			}
			if (this.Touxiang)
			{
				this.Touxiang.gameObject.SetActive(false);
			}
		}
		catch (Exception ex)
		{
		}
	}

	private void SetState(int state)
	{
		switch (state)
		{
		case 0:
		{
			if (this.name)
			{
				this.name.gameObject.SetActive(true);
			}
			if (this.level)
			{
				this.level.gameObject.SetActive(true);
			}
			if (this.server)
			{
				this.server.gameObject.SetActive(true);
			}
			if (this.xiuxi)
			{
				this.xiuxi.gameObject.SetActive(true);
			}
			if (this.time)
			{
				this.time.gameObject.SetActive(true);
			}
			if (this.timeBG)
			{
				this.timeBG.gameObject.SetActive(true);
			}
			if (this.shuijiao)
			{
				this.shuijiao.SetActive(true);
			}
			if (this.Touxiang)
			{
				this.Touxiang.gameObject.SetActive(true);
			}
			UITexture uitexture = null;
			if (this.Touxiang)
			{
				this.Touxiang.GetComponent<UITexture>();
			}
			if (uitexture)
			{
				uitexture.shader = Shader.Find("Unlit/Transparent Colored");
			}
			break;
		}
		case 1:
		{
			if (this.Touxiang)
			{
				this.Touxiang.gameObject.SetActive(true);
			}
			if (this.name)
			{
				this.name.gameObject.SetActive(true);
			}
			if (this.level)
			{
				this.level.gameObject.SetActive(true);
			}
			if (this.server)
			{
				this.server.gameObject.SetActive(true);
			}
			if (this.BtnLaoDong)
			{
				this.BtnLaoDong.gameObject.SetActive(true);
			}
			if (this.BtnShiFang)
			{
				this.BtnShiFang.gameObject.SetActive(true);
			}
			UITexture uitexture2 = null;
			if (this.Touxiang)
			{
				this.Touxiang.GetComponent<UITexture>();
			}
			if (uitexture2)
			{
				uitexture2.shader = Shader.Find("Unlit/Transparent Colored");
			}
			break;
		}
		case 2:
			if (this.wenhao)
			{
				this.wenhao.gameObject.SetActive(true);
			}
			if (this.name)
			{
				this.name.gameObject.SetActive(true);
			}
			if (this.xiaohao)
			{
				this.xiaohao.gameObject.SetActive(true);
			}
			if (this.jinbi)
			{
				this.jinbi.gameObject.SetActive(true);
			}
			if (this.jinbiBG)
			{
				this.jinbiBG.gameObject.SetActive(true);
			}
			if (this.xiaohaonum)
			{
				this.xiaohaonum.gameObject.SetActive(true);
			}
			if (this.BtnFind)
			{
				this.BtnFind.gameObject.SetActive(true);
			}
			this.SetName = null;
			break;
		}
	}

	protected void StartUITimer()
	{
		this.UITimer = new DispatcherTimer("YaoSaiJianYuPartItem_Timer");
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
		if (this.countdowntimes > 0.0)
		{
			this.time.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				YaoSaiJianYuPartItem.GetTimeStrBySecEx(this.countdowntimes)
			});
			this.countdowntimes -= 1.0;
		}
		else
		{
			this.time.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				"00:00:00"
			});
			this.StopTimer();
			this.InitData(1, 0L);
		}
	}

	private static string GetTimeStrBySecEx(double sec)
	{
		int num = 86400;
		int num2 = 3600;
		int num3 = 60;
		return StringUtil.substitute("{0}:{1}:{2}", new object[]
		{
			((int)(sec / (double)num2) <= 9) ? ("0" + ((int)(sec / (double)num2)).ToString()) : ((int)(sec / (double)num2)).ToString(),
			((int)(sec % (double)num % (double)num2 / (double)num3) <= 9) ? ("0" + ((int)(sec % (double)num % (double)num2 / (double)num3)).ToString()) : ((int)(sec % (double)num % (double)num2 / (double)num3)).ToString(),
			((int)(sec % (double)num % (double)num2 % (double)num3) <= 9) ? ("0" + ((int)(sec % (double)num % (double)num2 % (double)num3)).ToString()) : ((int)(sec % (double)num % (double)num2 % (double)num3)).ToString()
		});
	}

	public void ShowYaoSaiJianYuPartHuDongWindow()
	{
		if (this.YaoSaiJianYuPartHuDongWindow == null)
		{
			this.YaoSaiJianYuPartHuDongWindow = U3DUtils.NEW<GChildWindow>();
			this.YaoSaiJianYuPartHuDongWindow.IsShowModal = true;
			this.YaoSaiJianYuPartHuDongWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.YaoSaiJianYuPartHuDongWindow, Global.GetLang("互动界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.YaoSaiJianYuPartHuDongWindow);
		}
		if (this.yaosaiJianYuPartHuDong == null)
		{
			this.yaosaiJianYuPartHuDong = U3DUtils.NEW<YaoSaiJianYuPartHuDong>();
			this.yaosaiJianYuPartHuDong.Level = this.Level;
			this.yaosaiJianYuPartHuDong.ChangeLevel = this.ChangeLevel;
			this.yaosaiJianYuPartHuDong.roleID = this.RoleID;
			this.yaosaiJianYuPartHuDong.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseYaoSaiJianYuPartHuDongWindow();
			};
		}
		this.YaoSaiJianYuPartHuDongWindow.SetContent(this.YaoSaiJianYuPartHuDongWindow.BodyPresenter, this.yaosaiJianYuPartHuDong, 0.0, 0.0, true);
	}

	private void CloseYaoSaiJianYuPartHuDongWindow()
	{
		if (null != this.yaosaiJianYuPartHuDong)
		{
			this.yaosaiJianYuPartHuDong.transform.parent = null;
			Object.Destroy(this.yaosaiJianYuPartHuDong.gameObject);
			this.yaosaiJianYuPartHuDong = null;
		}
		if (null != this.YaoSaiJianYuPartHuDongWindow)
		{
			Super.CloseChildWindow(base.Children, this.YaoSaiJianYuPartHuDongWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.YaoSaiJianYuPartHuDongWindow, true);
			this.YaoSaiJianYuPartHuDongWindow = null;
		}
	}

	public UILabel name;

	public UILabel level;

	public UILabel server;

	public UILabel xiuxi;

	public UILabel time;

	public UILabel xiaohao;

	public UILabel xiaohaonum;

	public GameObject shuijiao;

	public UISprite jinbi;

	public UISprite timeBG;

	public UISprite jinbiBG;

	public UISprite wenhao;

	public GButton BtnLaoDong;

	public GButton BtnShiFang;

	public GButton BtnFind;

	public ShowNetImage Touxiang;

	public DPSelectedItemEventHandler DPSelectItem;

	public int Level;

	public int ChangeLevel;

	private int roleid;

	private string setname;

	private DispatcherTimer UITimer;

	private double countdowntimes;

	protected GChildWindow YaoSaiJianYuPartHuDongWindow;

	public YaoSaiJianYuPartHuDong yaosaiJianYuPartHuDong;
}
