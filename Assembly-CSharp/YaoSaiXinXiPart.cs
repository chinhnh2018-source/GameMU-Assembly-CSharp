using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class YaoSaiXinXiPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BtnXinXi.Label.text = Global.GetLang("信息");
		this.BtnFuLu.Label.text = Global.GetLang("俘虏");
		this.BtnXinXi.Label.color = NGUIMath.HexToColorEx(14599836U);
		this.BtnFuLu.Label.color = NGUIMath.HexToColorEx(14599836U);
		this.BtnZhengFu.Label.text = Global.GetLang("征服");
		this.BtnQiangDuo_m.Label.text = Global.GetLang("抢夺");
		this.BtnJieJiu.Label.text = Global.GetLang("解救");
		this.BtnZhengFu.gameObject.SetActive(false);
		this.BtnQiangDuo_m.gameObject.SetActive(false);
		this.BtnJieJiu.gameObject.SetActive(false);
		this.TabMine.SetActive(false);
		this.TabMine_master.SetActive(false);
		this.TabFuLu.gameObject.SetActive(false);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.data = Global.Data.yaosaiData;
		this.OBCItem = this.FuLuItem.ItemsSource;
		this.SetBtnStat(this.BtnXinXi);
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnXinXi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetBtnStat(this.BtnXinXi);
			if (this.data != null && this.data.state == 1)
			{
				this.BtnZhengFu.gameObject.SetActive(false);
				this.BtnQiangDuo_m.gameObject.SetActive(true);
				this.TabMine.SetActive(false);
				this.TabMine_master.SetActive(true);
				this.SetBtnJieJiuState();
			}
			else
			{
				if (Global.Data.roleData.RoleID == this.data.Mine.RoleID)
				{
					this.BtnZhengFu.gameObject.SetActive(false);
				}
				else
				{
					this.BtnZhengFu.gameObject.SetActive(true);
				}
				this.BtnQiangDuo_m.gameObject.SetActive(false);
				this.BtnJieJiu.gameObject.SetActive(false);
				this.TabMine.SetActive(true);
				this.TabMine_master.SetActive(false);
			}
			this.TabFuLu.SetActive(false);
		};
		this.BtnFuLu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetBtnStat(this.BtnFuLu);
			this.BtnZhengFu.gameObject.SetActive(false);
			this.BtnQiangDuo_m.gameObject.SetActive(false);
			this.BtnJieJiu.gameObject.SetActive(false);
			this.TabMine.SetActive(false);
			this.TabMine_master.SetActive(false);
			this.TabFuLu.gameObject.SetActive(true);
		};
		this.BtnZhengFu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.data.Master.RoleID == Global.Data.roleData.RoleID)
			{
				Super.HintMainText(Global.GetLang("对方已经是你的俘虏了"), 10, 3);
				return;
			}
			if (this.data.FuLuList != null)
			{
				for (int i = 0; i < this.data.FuLuList.Count; i++)
				{
					if (this.data.FuLuList[i].RoleID == Global.Data.roleData.RoleID)
					{
						Super.HintMainText(Global.GetLang("无法征服主人"), 10, 3);
						return;
					}
				}
			}
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ManorCatch", ',');
			int num = systemParamIntArrayByName[0];
			if (this.data.zhenfuLeftCount == 0 && this.data.zhenfuCount >= num)
			{
				int num2;
				if (this.data.zhenfuCount - num + 1 >= systemParamIntArrayByName.Length)
				{
					num2 = systemParamIntArrayByName[systemParamIntArrayByName.Length - 1];
				}
				else
				{
					num2 = systemParamIntArrayByName[this.data.zhenfuCount - num + 1];
				}
				string message = string.Format(Global.GetLang("你确定花费{0}钻石，增加一次征服次数吗？"), num2);
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
				GameInstance.Game.SendPrisonFireData(this.data.Mine.RoleID, 0);
				Super.ShowNetWaiting(null);
			}
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnQiangDuo_m.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.data.Master.RoleID == Global.Data.roleData.RoleID)
			{
				Super.HintMainText(Global.GetLang("不能抢夺自己的俘虏"), 10, 3);
				return;
			}
			if (this.data.FuLuList != null)
			{
				for (int i = 0; i < this.data.FuLuList.Count; i++)
				{
					if (this.data.FuLuList[i].RoleID == Global.Data.roleData.RoleID)
					{
						Super.HintMainText(Global.GetLang("无法抢夺主人"), 10, 3);
						return;
					}
				}
			}
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ManorCatch", ',');
			int num = systemParamIntArrayByName[0];
			if (this.data.zhenfuLeftCount == 0 && this.data.zhenfuCount >= num)
			{
				int num2;
				if (this.data.zhenfuCount - num + 1 >= systemParamIntArrayByName.Length)
				{
					num2 = systemParamIntArrayByName[systemParamIntArrayByName.Length - 1];
				}
				else
				{
					num2 = systemParamIntArrayByName[this.data.zhenfuCount - num + 1];
				}
				string message = string.Format(Global.GetLang("你确定花费{0}钻石，增加一次征服次数吗？"), num2);
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
				GameInstance.Game.SendPrisonFireData(this.data.Mine.RoleID, 1);
				Super.ShowNetWaiting(null);
			}
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnJieJiu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.data.Master.RoleID == Global.Data.roleData.RoleID)
			{
				Super.HintMainText(Global.GetLang("不能解救自己的俘虏"), 10, 3);
				return;
			}
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ManorHelp", ',');
			int num = systemParamIntArrayByName[0];
			if (this.data.jiejiuCount >= num)
			{
				int num2;
				if (this.data.jiejiuCount - num + 1 >= systemParamIntArrayByName.Length)
				{
					num2 = systemParamIntArrayByName[systemParamIntArrayByName.Length - 1];
				}
				else
				{
					num2 = systemParamIntArrayByName[this.data.jiejiuCount - num + 1];
				}
				string message = string.Format(Global.GetLang("你确定花费{0}钻石，增加一次解救次数吗？"), num2);
				string[] buttons = new string[]
				{
					Global.GetLang("确定"),
					Global.GetLang("取消")
				};
				Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s2, DPSelectedItemEventArgs e2)
				{
					if (e2.ID == 0)
					{
						GameInstance.Game.SendPrisonFireData(this.data.Mine.RoleID, 2);
						Super.ShowNetWaiting(null);
					}
				}, buttons);
			}
			else
			{
				GameInstance.Game.SendPrisonFireData(this.data.Mine.RoleID, 2);
				Super.ShowNetWaiting(null);
			}
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public void InitData()
	{
		if (this.data == null || this.data.Mine.Name == null)
		{
			PlayZone.GlobalPlayZone.CloseYaoSaiXinXiPartWindow();
			Super.HintMainText(Global.GetLang("没有要塞信息"), 10, 3);
			return;
		}
		if (Global.Data.RoleID == this.data.Mine.RoleID)
		{
			PlayZone.GlobalPlayZone.CloseYaoSaiXinXiPartWindow();
			return;
		}
		if (this.data.state == 1)
		{
			this.BtnQiangDuo_m.gameObject.SetActive(true);
			this.TabMine.SetActive(false);
			this.TabMine_master.SetActive(true);
			this.TabFuLu.SetActive(false);
			string url = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(this.data.Mine.Occupation),
				this.data.Mine.RoleSex
			});
			this.MineTex_m.URL = url;
			this.MineName_m.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.data.Mine.Name
			});
			this.MineLevel_m.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("LV{0}【{1}转】"), this.data.Mine.Level, this.data.Mine.ChangeLevel)
			});
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(this.data.Mine.ZoneID, out ztBuffServerInfo))
			{
				this.MineServer_m.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("服务器：{0}"), ztBuffServerInfo.strServerName)
				});
			}
			else
			{
				this.MineServer_m.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("服务器：S.{0}"), this.data.Mine.ZoneID)
				});
			}
			this.MineZhanLi_m.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Format(Global.GetLang("战力：{0}"), this.data.Mine.CombatForce)
			});
			string url2 = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(this.data.Master.Occupation),
				this.data.Master.RoleSex
			});
			this.MasterTex.URL = url2;
			this.MasterName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.data.Master.Name
			});
			this.MasterLevel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("LV{0}【{1}转】"), this.data.Master.Level, this.data.Master.ChangeLevel)
			});
			ZtBuffServerInfo ztBuffServerInfo2 = null;
			if (Global.GetNowServerIsZhuTiFu(this.data.Master.ZoneID, out ztBuffServerInfo2))
			{
				this.MasterServer.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("服务器：{0}"), ztBuffServerInfo2.strServerName)
				});
			}
			else
			{
				this.MasterServer.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("服务器：S.{0}"), this.data.Master.ZoneID)
				});
			}
			this.MasterZhanLi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Format(Global.GetLang("战力：{0}"), this.data.Master.CombatForce)
			});
		}
		else
		{
			if (Global.Data.roleData.RoleID == this.data.Mine.RoleID)
			{
				this.BtnZhengFu.gameObject.SetActive(false);
			}
			else
			{
				this.BtnZhengFu.gameObject.SetActive(true);
			}
			this.TabMine.SetActive(true);
			this.TabMine_master.SetActive(false);
			this.TabFuLu.SetActive(false);
			string url3 = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(this.data.Mine.Occupation),
				this.data.Mine.RoleSex
			});
			this.MineTex.URL = url3;
			this.MineName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.data.Mine.Name
			});
			this.MineLevel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("LV{0}【{1}转】"), this.data.Mine.Level, this.data.Mine.ChangeLevel)
			});
			ZtBuffServerInfo ztBuffServerInfo3 = null;
			string text = null;
			if (Global.GetNowServerIsZhuTiFu(this.data.Mine.ZoneID, out ztBuffServerInfo3) && !string.IsNullOrEmpty(ztBuffServerInfo3.strServerName))
			{
				text = ztBuffServerInfo3.strServerName;
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.MineServer.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("服务器：{0}"), text)
				});
			}
			else
			{
				this.MineServer.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("服务器：S.{0}"), this.data.Mine.ZoneID)
				});
			}
			this.MineZhanLi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Format(Global.GetLang("战力：{0}"), this.data.Mine.CombatForce)
			});
		}
		if (this.data.FuLuList != null && this.data.FuLuList.Count > 0)
		{
			this.AddItem();
		}
		else
		{
			this.BtnFuLu.gameObject.SetActive(false);
		}
		this.SetBtnJieJiuState();
	}

	private void SetBtnJieJiuState()
	{
		if (Global.Data.FriendDataList != null)
		{
			for (int i = 0; i < Global.Data.FriendDataList.Count; i++)
			{
				if (this.data.Mine.RoleID == Global.Data.FriendDataList[i].OtherRoleID)
				{
					this.BtnJieJiu.gameObject.SetActive(true);
					return;
				}
			}
		}
		if (Global.Data.bangHuiMemberDataList != null)
		{
			for (int j = 0; j < Global.Data.bangHuiMemberDataList.Count; j++)
			{
				if (this.data.Mine.RoleID == Global.Data.bangHuiMemberDataList[j].RoleID)
				{
					this.BtnJieJiu.gameObject.SetActive(true);
					return;
				}
			}
		}
		this.BtnJieJiu.gameObject.SetActive(false);
	}

	private void AddItem()
	{
		for (int i = 0; i < this.data.FuLuList.Count; i++)
		{
			YaoSaiXinXiPartFuLuItem yaoSaiXinXiPartFuLuItem = U3DUtils.NEW<YaoSaiXinXiPartFuLuItem>();
			string setTouXiang = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(this.data.FuLuList[i].Occupation),
				this.data.FuLuList[i].RoleSex
			});
			yaoSaiXinXiPartFuLuItem.SetTouXiang = setTouXiang;
			yaoSaiXinXiPartFuLuItem.SetName = this.data.FuLuList[i].Name;
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(this.data.FuLuList[i].ZoneID, out ztBuffServerInfo))
			{
				yaoSaiXinXiPartFuLuItem.SetSever = string.Format(Global.GetLang(Global.GetLang("服务器：{0}")), ztBuffServerInfo.strServerName);
			}
			else
			{
				yaoSaiXinXiPartFuLuItem.SetSever = string.Format(Global.GetLang(Global.GetLang("服务器：S.{0}")), this.data.FuLuList[i].ZoneID);
			}
			this.OBCItem.AddNoUpdate(yaoSaiXinXiPartFuLuItem);
		}
	}

	private void SetBtnStat(GButton btn)
	{
		if (this.tempBtn != null)
		{
			if (this.tempBtn == btn)
			{
				btn.Label.color = NGUIMath.HexToColorEx(16777215U);
				return;
			}
			btn.Pressed = true;
			btn.Label.color = NGUIMath.HexToColorEx(16777215U);
			this.tempBtn.Label.color = NGUIMath.HexToColorEx(14599836U);
			this.tempBtn.Pressed = false;
			this.tempBtn = btn;
		}
		else
		{
			btn.Label.color = NGUIMath.HexToColorEx(16777215U);
			btn.Pressed = true;
			this.tempBtn = btn;
		}
	}

	public GButton BtnXinXi;

	public GButton BtnFuLu;

	public GButton BtnClose;

	public GameObject TabMine;

	public ShowNetImage MineTex;

	public UILabel MineName;

	public UILabel MineLevel;

	public UILabel MineServer;

	public UILabel MineZhanLi;

	public GButton BtnZhengFu;

	public GameObject TabMine_master;

	public ShowNetImage MineTex_m;

	public UILabel MineName_m;

	public UILabel MineLevel_m;

	public UILabel MineServer_m;

	public UILabel MineZhanLi_m;

	public GButton BtnQiangDuo_m;

	public ShowNetImage MasterTex;

	public UILabel MasterName;

	public UILabel MasterLevel;

	public UILabel MasterServer;

	public UILabel MasterZhanLi;

	public GButton BtnJieJiu;

	public GameObject TabFuLu;

	public ListBox FuLuItem;

	public DPSelectedItemEventHandler CloseHandler;

	private ObservableCollection OBCItem;

	private YaoSaiData data;

	private GButton tempBtn;
}
