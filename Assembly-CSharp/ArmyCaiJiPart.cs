using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ArmyCaiJiPart : UserControl
{
	protected override void InitializeComponent()
	{
		GameInstance.Game.SendOpenArmyCaiJiWinodw();
		this.InitTextInPrefabs();
		this.InitBtnCallBack();
		this.SetActive(this.m_CheckedKuang, false);
		this.caiJiType = -1;
	}

	private void InitTextInPrefabs()
	{
		this.m_DiGongTitle.Text = string.Empty;
		this.m_DiGongTime.Text = string.Empty;
		this.m_DiGongName.Text = string.Empty;
		this.m_HuangMoTitle.Text = string.Empty;
		this.m_HuangMoTime.Text = string.Empty;
		this.m_HuangMoName.Text = string.Empty;
		this.m_Describe.Text = string.Empty;
		this.m_LeftTimes.Text = string.Empty;
		this.m_BtnEnter.Text = string.Empty;
		this.m_DiGongTitle.Text = Global.GetLang("阿卡伦地宫");
		this.m_DiGongTime.Text = Global.GetLang("双倍采集剩余时间：01分02秒");
		this.m_DiGongName.Text = Global.GetLang("占领军团：一二三四五六七");
		this.m_BtnEnter.Text = Global.GetLang("立即进入");
		this.m_HuangMoTitle.Text = Global.GetLang("阿卡伦荒漠");
		this.m_HuangMoTime.Text = Global.GetLang("双倍采集剩余时间：01分02秒");
		this.m_HuangMoName.Text = Global.GetLang("占领军团：一二三四五六七");
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ManorCollectDoubleAwardDefault", '|');
		if (systemParamStringArrayByName.Length >= 2)
		{
			string[] array = systemParamStringArrayByName[0].Split(new char[]
			{
				','
			});
			string[] array2 = systemParamStringArrayByName[1].Split(new char[]
			{
				','
			});
			string chineseText = string.Format(Global.GetLang("每周{0}、周{1} {2}—{3}开启双倍采集，双倍期间采集收益翻倍，并会出现超级采集点。领地所属军\n团玩家采集奖励翻倍，系统双倍时间内，采集奖励三倍。领地内PK模式只能是军团模式。"), new object[]
			{
				array[2],
				array2[2],
				array[0],
				array[1]
			});
			this.m_Describe.Text = Global.GetLang(chineseText);
			this.m_Describe.MaxWidth = 775.0;
		}
		this.m_LeftTimes.Text = Global.GetLang("今日采集剩余次数：29");
	}

	private void InitBtnCallBack()
	{
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.m_BtnEnter.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.caiJiType == -1)
			{
				Super.HintMainText(Global.GetLang("请选择采集场景类型"), 10, 3);
				return;
			}
			if (Global.Data.CurrentCopyTeamData != null)
			{
				Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
				{
					if (e2.ID == 0)
					{
						GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
						Super.ShowNetWaiting(null);
						Global.Data.CurrentJunTaiCaiJiType = this.caiJiType;
						GameInstance.Game.SendEnterArmyCaiJiScene(this.caiJiType);
					}
				}, -1);
			}
			else
			{
				Super.ShowNetWaiting(null);
				Global.Data.CurrentJunTaiCaiJiType = this.caiJiType;
				GameInstance.Game.SendEnterArmyCaiJiScene(this.caiJiType);
			}
		};
		UIEventListener.Get(this.m_BtnDiGong).onClick = delegate(GameObject s)
		{
			this.SetActive(this.m_CheckedKuang, true);
			this.m_CheckedKuang.transform.localPosition = this.leftLocalPosition;
			this.caiJiType = 0;
		};
		UIEventListener.Get(this.m_BtnHuangMo).onClick = delegate(GameObject s)
		{
			this.SetActive(this.m_CheckedKuang, true);
			this.m_CheckedKuang.transform.localPosition = this.rightLocaltPosition;
			this.caiJiType = 1;
		};
	}

	private void ErrorTips()
	{
		Super.HintMainText(Global.GetLang("未找到跨服服务器"), 10, 3);
		this.m_DiGongTime.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("未开启双倍采集")
		});
		this.m_DiGongName.Text = Global.GetLang("未被占领");
		this.m_HuangMoTime.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("未开启双倍采集")
		});
		this.m_HuangMoName.Text = Global.GetLang("未被占领");
		this.m_LeftTimes.Text = string.Format("{0}{1}", Global.GetLang("本周采集剩余次数："), 0);
	}

	public void InitValue(LingDiCaiJiMainData data)
	{
		if (data == null)
		{
			this.ErrorTips();
			return;
		}
		if (data.LingDiCaiJiDataList == null || data.LingDiCaiJiDataList.Count <= 0)
		{
			this.ErrorTips();
			return;
		}
		this.m_LeftTimes.Text = string.Format("{0}{1}", Global.GetLang("本周采集剩余次数："), data.LingDiCaiJiLeftCount);
		int count = data.LingDiCaiJiDataList.Count;
		for (int i = 0; i < count; i++)
		{
			LingDiCaiJiData lingDiCaiJiData = data.LingDiCaiJiDataList[i];
			if (lingDiCaiJiData.LingDiType == 0)
			{
				this.m_DiGongData = lingDiCaiJiData;
			}
			if (lingDiCaiJiData.LingDiType == 1)
			{
				this.m_HuangMoData = lingDiCaiJiData;
			}
		}
		if (!this.m_DiGongData.HaveJunTuan)
		{
			this.m_DiGongName.Text = Global.GetLang("未被占领");
		}
		else
		{
			this.m_DiGongName.Text = string.Format("{0}{1}", Global.GetLang("占领军团："), this.m_DiGongData.ZhanLingName);
		}
		if (!this.m_HuangMoData.HaveJunTuan)
		{
			this.m_HuangMoName.Text = Global.GetLang("未被占领");
		}
		else
		{
			this.m_HuangMoName.Text = string.Format("{0}{1}", Global.GetLang("占领军团："), this.m_HuangMoData.ZhanLingName);
		}
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (correctDateTime >= this.m_DiGongData.BeginTime && correctDateTime <= this.m_DiGongData.EndTime)
		{
			long num = this.m_DiGongData.EndTime.Ticks - correctDateTime.Ticks;
			this.secondsDiGong = (int)(num / 10000000L);
			base.InvokeRepeating("TickProcsDiGong", 0f, 1f);
		}
		else
		{
			this.m_DiGongTime.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("未开启双倍采集")
			});
		}
		if (correctDateTime >= this.m_HuangMoData.BeginTime && correctDateTime <= this.m_HuangMoData.EndTime)
		{
			long num2 = this.m_HuangMoData.EndTime.Ticks - correctDateTime.Ticks;
			this.secondsHuangMo = (int)(num2 / 10000000L);
			base.InvokeRepeating("TickProcsHuangMo", 0f, 1f);
		}
		else
		{
			this.m_HuangMoTime.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("未开启双倍采集")
			});
		}
	}

	protected void TickProcsDiGong()
	{
		this.m_DiGongTime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			string.Format(Global.GetLang("双倍采集剩余时间： {0}"), Global.GetTimeStrBySecEx((double)this.secondsDiGong, true, -1))
		});
		this.secondsDiGong--;
		if (this.secondsDiGong < 0)
		{
			base.CancelInvoke("TickProcsDiGong");
			this.m_DiGongTime.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("未开启双倍采集")
			});
		}
	}

	protected void TickProcsHuangMo()
	{
		this.m_HuangMoTime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			string.Format(Global.GetLang("双倍采集剩余时间： {0}"), Global.GetTimeStrBySecEx((double)this.secondsHuangMo, true, -1))
		});
		this.secondsHuangMo--;
		if (this.secondsHuangMo < 0)
		{
			base.CancelInvoke("TickProcsHuangMo");
			this.m_HuangMoTime.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("未开启双倍采集")
			});
		}
	}

	private void SetActive(GameObject obj, bool isShow)
	{
		obj.SetActive(isShow);
	}

	protected override void OnDestroy()
	{
		this.m_BtnClose = null;
		this.CloseHandler = null;
		this.caiJiType = -1;
		this.m_CheckedKuang = null;
		this.m_BtnHuangMo = null;
		this.m_BtnEnter = null;
		this.m_DiGongTitle = null;
		this.m_DiGongTime = null;
		this.m_DiGongName = null;
		this.m_HuangMoTitle = null;
		this.m_HuangMoTime = null;
		this.m_HuangMoName = null;
		this.m_Describe = null;
		this.m_LeftTimes = null;
		this.CloseHandler = null;
		base.CancelInvoke("TickProcsDiGong");
		base.CancelInvoke("TickProcsHuangMo");
	}

	public GButton m_BtnClose;

	public GameObject m_BtnDiGong;

	public GameObject m_BtnHuangMo;

	public GButton m_BtnEnter;

	public GameObject m_CheckedKuang;

	private Vector3 leftLocalPosition = new Vector3(-236f, 22f, -0.5f);

	private Vector3 rightLocaltPosition = new Vector3(236f, 22f, -0.5f);

	private int caiJiType = -1;

	public TextBlock m_DiGongTitle;

	public TextBlock m_DiGongTime;

	public TextBlock m_DiGongName;

	private LingDiCaiJiData m_DiGongData;

	public TextBlock m_HuangMoTitle;

	public TextBlock m_HuangMoTime;

	public TextBlock m_HuangMoName;

	private LingDiCaiJiData m_HuangMoData;

	public TextBlock m_Describe;

	public TextBlock m_LeftTimes;

	public DPSelectedItemEventHandler CloseHandler;

	private int secondsDiGong;

	private int secondsHuangMo;
}
