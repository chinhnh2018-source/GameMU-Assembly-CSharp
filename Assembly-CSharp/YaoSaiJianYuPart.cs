using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class YaoSaiJianYuPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BG.URL = "NetImages/GameRes/Images/YaoSaiJianYuTex/tongyongdi.png.qj";
		this.LaoFangTitle.URL = "NetImages/GameRes/Images/YaoSaiJianYuTex/laofangtitle.png.qj";
		this.Fankang.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("反抗冷却 ：")
		});
		this.ZhengFu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("征服次数 ：")
		});
		this.JieJiu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("解救次数 ：")
		});
		this.LaoDong.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("派遣劳动 ：")
		});
		this.BtnFanKang.Label.text = Global.GetLang("反抗");
		this.Fankang.pivot = 5;
		this.Fankang.transform.localPosition = new Vector3(15f, 140f, -1f);
		this.ZhengFu.pivot = 5;
		this.ZhengFu.transform.localPosition = new Vector3(15f, 92f, -1f);
	}

	private string SetName
	{
		set
		{
			this.Name.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				value
			});
		}
	}

	private string SetLevel
	{
		set
		{
			this.Level.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				value
			});
		}
	}

	private long SetZhanLi
	{
		set
		{
			this.ZhanLi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(Global.GetLang("战斗力：{0}"), value)
			});
		}
	}

	public long FanKangCDCount
	{
		get
		{
			return this.fankangCD;
		}
		set
		{
			this.fankangCD = value;
			if (value == 0L)
			{
				this.FanKangLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("无")
				});
			}
			else
			{
				this.countdowntimes = (double)value;
				this.StopTimer();
				this.StartUITimer();
			}
		}
	}

	private int ZhengFuLeftCount
	{
		get
		{
			return this.zhengfuleftCount;
		}
		set
		{
			this.zhengfuleftCount = value;
			this.ZhengFuLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				value
			});
		}
	}

	private int ZhengFuCount
	{
		get
		{
			return this.zhengfuCount;
		}
		set
		{
			this.zhengfuCount = value;
		}
	}

	private int JieJiuCount
	{
		set
		{
			this.JieJiuLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				value
			});
		}
	}

	public int LaoDongCout
	{
		set
		{
			this.LaoDongLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				value
			});
		}
	}

	private string SetTouXiang
	{
		set
		{
			this.TouXiang.URL = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.OBCItem = this.Items.ItemsSource;
		this.AddItem();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnFanKang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.GetMapSceneUIClass() != SceneUIClasses.Normal)
			{
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang("反抗将会离开当前场景，是否确认前往"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						if (this.FanKangCDCount > 0L)
						{
							int zuanshiNum = ConfigSystemParam.GetSystemParamIntArrayByName("ManorCommandAgainst", ',')[1];
							string message2 = string.Format(Global.GetLang("清除冷却消耗{0}钻石，您确定要清除吗？"), zuanshiNum);
							string[] buttons2 = new string[]
							{
								Global.GetLang("确定"),
								Global.GetLang("取消")
							};
							Super.ShowMessageBoxEx(Global.GetLang("提示"), message2, delegate(object s2, DPSelectedItemEventArgs e2)
							{
								if (e2.ID == 0)
								{
									if (Global.Data.roleData.UserMoney < zuanshiNum)
									{
										Super.HintMainText(Global.GetLang("钻石不足！"), 10, 3);
										return;
									}
									GameInstance.Game.SendPrisonRevoltData(1);
									Super.ShowNetWaiting(null);
								}
							}, buttons2);
						}
						else
						{
							GameInstance.Game.SendPrisonRevoltData(0);
							Super.ShowNetWaiting(null);
						}
					}
					return true;
				};
			}
			else
			{
				if (this.FanKangCDCount > 0L)
				{
					int zuanshiNum = ConfigSystemParam.GetSystemParamIntArrayByName("ManorCommandAgainst", ',')[1];
					string message = string.Format(Global.GetLang("清除冷却消耗{0}钻石，您确定要清除吗？"), zuanshiNum);
					string[] buttons = new string[]
					{
						Global.GetLang("确定"),
						Global.GetLang("取消")
					};
					Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s2, DPSelectedItemEventArgs e2)
					{
						if (e2.ID == 0)
						{
							if (Global.Data.roleData.UserMoney < zuanshiNum)
							{
								Super.HintMainText(Global.GetLang("钻石不足！"), 10, 3);
								return;
							}
							GameInstance.Game.SendPrisonRevoltData(1);
							Super.ShowNetWaiting(null);
						}
					}, buttons);
					return;
				}
				GameInstance.Game.SendPrisonRevoltData(0);
				Super.ShowNetWaiting(null);
			}
		};
		this.BtnJiLu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowYaoSaiJianYuPartJiLuWindow();
		};
		GameInstance.Game.SendPrisonMainData();
		Super.ShowNetWaiting(null);
	}

	public void InitServerData(PrisonMainData data)
	{
		if (data == null)
		{
			return;
		}
		this.SetName = data.roleData.Name;
		this.SetLevel = string.Format(Global.GetLang("LV{0}【{1}转】"), data.roleData.Level, data.roleData.ChangeLevel);
		this.SetZhanLi = (long)data.roleData.CombatForce;
		if (data.MineFuLuState == 1)
		{
			this.BtnFanKang.gameObject.SetActive(true);
			this.Master.gameObject.SetActive(true);
		}
		else
		{
			this.BtnFanKang.gameObject.SetActive(false);
			this.Master.gameObject.SetActive(false);
		}
		string setTouXiang = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
		{
			Global.CalcOriginalOccupationID(data.roleData.Occupation),
			data.roleData.RoleSex
		});
		this.SetTouXiang = setTouXiang;
		this.ZhengFuLeftCount = data.ZhengFuLeftCount;
		this.ZhengFuCount = data.ZhengFuCount;
		int num = ConfigSystemParam.GetSystemParamIntArrayByName("ManorHelp", ',')[0];
		if (num - data.JieJiuCount > 0)
		{
			this.JieJiuCount = num - data.JieJiuCount;
		}
		else
		{
			this.JieJiuCount = 0;
		}
		int num2 = ConfigSystemParam.GetSystemParamIntArrayByName("ManorCommandAward", ',')[0];
		this.LaoDongCout = num2 - data.LaoDongCount;
		this.FanKangCDCount = data.RevoltCD / 1000L;
		this.ReItems(data.FuLuData);
	}

	public void ReZhengFuCount(int count)
	{
		this.ZhengFuLeftCount = count;
	}

	private void AddItem()
	{
		int[] paramvalue = ConfigSystemParam.GetSystemParamIntArrayByName("ManorCatch", ',');
		int freeCount = paramvalue[0];
		for (int i = 0; i < 4; i++)
		{
			YaoSaiJianYuPartItem yaoSaiJianYuPartItem = U3DUtils.NEW<YaoSaiJianYuPartItem>();
			yaoSaiJianYuPartItem.DPSelectItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.ZhengFuLeftCount == 0 && this.ZhengFuCount >= freeCount)
				{
					int num;
					if (this.ZhengFuCount - freeCount + 1 >= paramvalue.Length)
					{
						num = paramvalue[paramvalue.Length - 1];
					}
					else
					{
						num = paramvalue[this.ZhengFuCount - freeCount + 1];
					}
					string message = string.Format(Global.GetLang("你确定花费{0}钻石，增加一次征服次数吗？"), num);
					string[] buttons = new string[]
					{
						Global.GetLang("确定"),
						Global.GetLang("取消")
					};
					Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s2, DPSelectedItemEventArgs e2)
					{
						if (e2.ID == 0)
						{
							GameInstance.Game.SendPrisonBuyData();
							Super.ShowNetWaiting(null);
						}
					}, buttons);
				}
				else
				{
					int num2 = ConfigSystemParam.GetSystemParamByName("ManorSearchCost", true).SafeToInt32(0);
					int num3 = Global.Data.roleData.Money1 + Global.Data.roleData.YinLiang;
					if (num3 < num2)
					{
						Super.HintMainText(Global.GetLang("金币不足"), 10, 3);
						return;
					}
					if (Global.GetMapSceneUIClass() != SceneUIClasses.Normal)
					{
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang("寻找俘虏将会离开当前场景，是否确认前往"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								GameInstance.Game.SendYaoSaiData(0);
								GameInstance.Game.SpriteGetFriends();
							}
							return true;
						};
					}
					else
					{
						GameInstance.Game.SendYaoSaiData(0);
						GameInstance.Game.SpriteGetFriends();
					}
				}
			};
			this.OBCItem.AddNoUpdate(yaoSaiJianYuPartItem);
		}
	}

	public void ReItems(List<FuLuState> FuLuData)
	{
		for (int i = 0; i < this.OBCItem.Count; i++)
		{
			YaoSaiJianYuPartItem yaoSaiJianYuPartItem = U3DUtils.AS<YaoSaiJianYuPartItem>(this.OBCItem[i]);
			yaoSaiJianYuPartItem.InitData(2, 0L);
			yaoSaiJianYuPartItem.Level = 0;
			yaoSaiJianYuPartItem.ChangeLevel = 0;
			yaoSaiJianYuPartItem.RoleID = 0;
		}
		if (FuLuData == null || FuLuData.Count <= 0 || FuLuData.Count > this.OBCItem.Count)
		{
			return;
		}
		for (int j = 0; j < FuLuData.Count; j++)
		{
			YaoSaiJianYuPartItem yaoSaiJianYuPartItem2 = U3DUtils.AS<YaoSaiJianYuPartItem>(this.OBCItem[j]);
			string setTouXiang = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(FuLuData[j].Occupation),
				FuLuData[j].RoleSex
			});
			yaoSaiJianYuPartItem2.Level = FuLuData[j].Level;
			yaoSaiJianYuPartItem2.ChangeLevel = FuLuData[j].ChangeLevel;
			yaoSaiJianYuPartItem2.RoleID = FuLuData[j].RoleID;
			yaoSaiJianYuPartItem2.SetTouXiang = setTouXiang;
			yaoSaiJianYuPartItem2.SetName = FuLuData[j].Name;
			yaoSaiJianYuPartItem2.SetLevel = string.Format(Global.GetLang("LV{0}【{1}转】"), FuLuData[j].Level, FuLuData[j].ChangeLevel);
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(FuLuData[j].ZoneID, out ztBuffServerInfo))
			{
				yaoSaiJianYuPartItem2.SetServer = Global.GetLang("服务器：") + ztBuffServerInfo.strServerName;
			}
			else
			{
				yaoSaiJianYuPartItem2.SetServer = string.Format(Global.GetLang("服务器：S.{0}"), FuLuData[j].ZoneID);
			}
			if (FuLuData[j].LaoDongState == 0)
			{
				yaoSaiJianYuPartItem2.InitData(1, 0L);
			}
			else if (FuLuData[j].LaoDongState == 1)
			{
				yaoSaiJianYuPartItem2.InitData(0, FuLuData[j].LaoDongTime / 1000L);
			}
		}
	}

	public void ServerDataReItem(int targetID)
	{
		for (int i = 0; i < this.OBCItem.Count; i++)
		{
			YaoSaiJianYuPartItem yaoSaiJianYuPartItem = U3DUtils.AS<YaoSaiJianYuPartItem>(this.OBCItem[i]);
			if (targetID == yaoSaiJianYuPartItem.RoleID)
			{
				int num = ConfigSystemParam.GetSystemParamIntArrayByName("ManorCommandAward", ',')[2];
				long timelen = (long)num * 60L;
				yaoSaiJianYuPartItem.InitData(0, timelen);
				return;
			}
		}
	}

	public void ServerDataShiFangReItem(int targetID)
	{
		for (int i = 0; i < this.OBCItem.Count; i++)
		{
			YaoSaiJianYuPartItem yaoSaiJianYuPartItem = U3DUtils.AS<YaoSaiJianYuPartItem>(this.OBCItem[i]);
			if (targetID == yaoSaiJianYuPartItem.RoleID)
			{
				yaoSaiJianYuPartItem.InitData(2, 0L);
				return;
			}
		}
	}

	public void ShowYaoSaiJianYuPartJiLuWindow()
	{
		if (this.YaoSaiJianYuPartJiLuWindow)
		{
			this.CloseYaoSaiJianYuPartJiLuWindow();
		}
		if (this.YaoSaiJianYuPartJiLuWindow == null)
		{
			this.YaoSaiJianYuPartJiLuWindow = U3DUtils.NEW<GChildWindow>();
			this.YaoSaiJianYuPartJiLuWindow.IsShowModal = true;
			this.YaoSaiJianYuPartJiLuWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.YaoSaiJianYuPartJiLuWindow, Global.GetLang("牢房记录界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.YaoSaiJianYuPartJiLuWindow);
		}
		if (this.yaosaiJianYuPartJiLu == null)
		{
			this.yaosaiJianYuPartJiLu = U3DUtils.NEW<YaoSaiJianYuPartJiLu>();
			this.yaosaiJianYuPartJiLu.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseYaoSaiJianYuPartJiLuWindow();
			};
		}
		this.YaoSaiJianYuPartJiLuWindow.SetContent(this.YaoSaiJianYuPartJiLuWindow.BodyPresenter, this.yaosaiJianYuPartJiLu, 0.0, 0.0, true);
	}

	public void CloseYaoSaiJianYuPartJiLuWindow()
	{
		if (null != this.yaosaiJianYuPartJiLu)
		{
			this.yaosaiJianYuPartJiLu.transform.parent = null;
			Object.Destroy(this.yaosaiJianYuPartJiLu.gameObject);
			this.yaosaiJianYuPartJiLu = null;
		}
		if (null != this.YaoSaiJianYuPartJiLuWindow)
		{
			Super.CloseChildWindow(base.Children, this.YaoSaiJianYuPartJiLuWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.YaoSaiJianYuPartJiLuWindow, true);
			this.YaoSaiJianYuPartJiLuWindow = null;
		}
	}

	protected void StartUITimer()
	{
		this.UITimer = new DispatcherTimer("YaoSaiJianYuPart_Timer");
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
			this.FanKangLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				YaoSaiJianYuPart.GetTimeStrBySecEx(this.countdowntimes)
			});
			this.countdowntimes -= 1.0;
		}
		else
		{
			this.FanKangCDCount = 0L;
			this.StopTimer();
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

	public static Dictionary<int, ManorCommandXml> GetDicManorCommand()
	{
		if (YaoSaiJianYuPart.dicManorCommand.Count > 0)
		{
			return YaoSaiJianYuPart.dicManorCommand;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ManorCommand.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ManorCommand");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ManorCommandXml manorCommandXml = new ManorCommandXml();
			manorCommandXml.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			manorCommandXml.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			manorCommandXml.Award = Global.GetXElementAttributeStr(xelementList[i], "Award");
			if (!YaoSaiJianYuPart.dicManorCommand.ContainsKey(manorCommandXml.ID))
			{
				YaoSaiJianYuPart.dicManorCommand.Add(manorCommandXml.ID, manorCommandXml);
			}
			i++;
		}
		return YaoSaiJianYuPart.dicManorCommand;
	}

	private new void OnDestroy()
	{
		YaoSaiJianYuPart.dicManorCommand.Clear();
	}

	public ShowNetImage BG;

	public ShowNetImage LaoFangTitle;

	public ShowNetImage TouXiang;

	public new UILabel Name;

	public UILabel Level;

	public UILabel ZhanLi;

	public UILabel Fankang;

	public UILabel ZhengFu;

	public UILabel JieJiu;

	public UILabel LaoDong;

	public UILabel FanKangLab;

	public UILabel ZhengFuLab;

	public UILabel JieJiuLab;

	public UILabel LaoDongLab;

	public GButton BtnClose;

	public GButton BtnFanKang;

	public GButton BtnJiLu;

	public UISprite Master;

	public DPSelectedItemEventHandler CloseHandler;

	public ListBox Items;

	private ObservableCollection OBCItem;

	private long fankangCD;

	private int zhengfuleftCount;

	private int zhengfuCount;

	protected GChildWindow YaoSaiJianYuPartJiLuWindow;

	public YaoSaiJianYuPartJiLu yaosaiJianYuPartJiLu;

	private DispatcherTimer UITimer;

	private double countdowntimes;

	private static Dictionary<int, ManorCommandXml> dicManorCommand = new Dictionary<int, ManorCommandXml>();
}
