using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class HuobiPart : UserControl
{
	protected override void InitializeComponent()
	{
		this._ItemCollection = this.listBox.ItemsSource;
	}

	private void OnEnable()
	{
		GameInstance.Game.SpriteQueryChengJiuData();
	}

	public void InitPartData()
	{
		this.listBox.Clear();
		this._ItemCollection.Clear();
		this.SetListContent();
		if (null != this.background)
		{
			this.background.transform.localPosition = Vector3.zero;
		}
	}

	private void SetListContent()
	{
		HuobiPartItem huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = false;
		huobiPartItem.Icon = "gold";
		huobiPartItem.Count = Global.GetRoleOwnNumByMoneyType(8).ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang("【金币】:"),
			"9456C6",
			Global.GetLang("各个系统均有消耗")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 0
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "bindmoney";
		huobiPartItem.Count = Global.Data.roleData.Money1.ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang("【绑金】:"),
			"9456C6",
			Global.GetLang("NPC商店")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 2
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "diamond";
		huobiPartItem.Count = string.Empty + Global.Data.roleData.UserMoney;
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang("【钻石】:"),
			"9456C6",
			Global.GetLang("钻石商城")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.QiFu, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.QiFu, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 1
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "bindDiamond";
		huobiPartItem.Count = string.Empty + Global.Data.roleData.Gold;
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang("【绑钻】:"),
			"9456C6",
			Global.GetLang("绑钻商城")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 3
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "xingyunzhixing";
		huobiPartItem.Count = string.Empty + Global.GetRoleOwnNumByMoneyType(163);
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang("【幸运之星】:"),
			"9456C6",
			Global.GetLang("祈福")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!UIHelper.IsGongNengOpenedOrHint(GongNengIDs.QiFu, true))
			{
				return;
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 17
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "mojing";
		huobiPartItem.Count = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang("【魔晶】:"),
			"9456C6",
			Global.GetLang("魔晶兑换")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 4
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "chengjiu";
		huobiPartItem.Count = Global.Data.ChengJiuData.ChengJiuPoints.ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang("【成就】:"),
			"9456C6",
			Global.GetLang("提升称号")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 5
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "shengwang";
		huobiPartItem.Count = string.Empty + Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWang);
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang("【声望】:"),
			"9456C6",
			Global.GetLang("提升军衔")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.JingJiChang, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.JingJiChang, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 6
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "xinghun";
		huobiPartItem.Count = string.Empty + Global.Data.roleData.StarSoulValue;
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang("【星魂】:"),
			"9456C6",
			Global.GetLang("激活星座")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GamePayerRolePartXingZuo, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.GamePayerRolePartXingZuo, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 7
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "mohe";
		huobiPartItem.Count = string.Empty + Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MoHeValue).ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang(Global.GetLang("【灵晶】:")),
			"9456C6",
			Global.GetLang(Global.GetLang("精灵升级"))
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.JingLingSystem, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.JingLingSystem, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 8
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "yuansu";
		huobiPartItem.Count = string.Empty + Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.YuansuFenmo).ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang(Global.GetLang("【粉末】:")),
			"9456C6",
			Global.GetLang(Global.GetLang("抽取元素之心"))
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.YuanSuHeart, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.YuanSuHeart, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 9
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "zaizao";
		huobiPartItem.Count = string.Empty + Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZaizaoDian).ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang(Global.GetLang("【再造点数】:")),
			"9456C6",
			Global.GetLang("神器再造")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZaiZao, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.ZaiZao, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 10
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "shouhu";
		huobiPartItem.Count = string.Empty + Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.GuardStatue).ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang(Global.GetLang("【守护点数】:")),
			"9456C6",
			Global.GetLang("提升守护雕像")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GuardStatue, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.GuardStatue, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 11
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "RongYao";
		huobiPartItem.Count = string.Empty + Global.Data.roleData.TianTiRongYao;
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang(Global.GetLang("【荣耀点数】:")),
			"9456C6",
			Global.GetLang("兑换天梯奖励")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.TianTiJingSai, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.TianTiJingSai, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 12
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "yingguangfenmo";
		huobiPartItem.Count = string.Empty + Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.FluorescentPoint).ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang(Global.GetLang("【荧光粉末】:")),
			"9456C6",
			Global.GetLang("挖掘荧光宝石")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.FluorescentDiamond, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.FluorescentDiamond, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 13
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "langhunfenmo";
		huobiPartItem.Count = string.Empty + Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.LangHunFenMo).ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang(Global.GetLang("【狼魂粉末】:")),
			"9456C6",
			Global.GetLang("获取魂石")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.SoulCometStonePowder, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.SoulCometStonePowder, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 14
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "shenlijinghua";
		huobiPartItem.Count = string.Empty + Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShenLiJingHua).ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang("【神力精华】:"),
			"9456C6",
			Global.GetLang("提升神器等级")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ShenQiSystem, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.ShenQiSystem, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 15
				});
			}
		};
		huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
		this._ItemCollection.AddNoUpdate(huobiPartItem);
		huobiPartItem.EnableBtn = true;
		huobiPartItem.Icon = "teamCompeteHuoBi";
		huobiPartItem.Count = string.Empty + Global.Data.roleData.MoneyData[160].ToString();
		huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
		{
			"E3B36C",
			Global.GetLang("【战队荣耀】:"),
			"9456C6",
			Global.GetLang("获取战队荣耀点")
		});
		huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuTeamCompete, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.KuaFuTeamCompete, trigger, param, param2, true);
			}
			else if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -2,
					IDType = 16
				});
			}
		};
		if (ConfigVersionSystemOpen.IsVersionSystemOpen(100113) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuTeamCompeteZhengBa))
		{
			huobiPartItem = U3DUtils.NEW<HuobiPartItem>();
			this._ItemCollection.AddNoUpdate(huobiPartItem);
			huobiPartItem.EnableBtn = true;
			huobiPartItem.Icon = "zhanduijingcaidian";
			huobiPartItem.Count = string.Empty + Global.Data.roleData.MoneyData[162].ToString();
			huobiPartItem.Title = Global.GetColorStringForNGUIText(new object[]
			{
				"E3B36C",
				Global.GetLang("【战队争霸】:"),
				"9456C6",
				Global.GetLang("获取战队竞猜点")
			});
			huobiPartItem.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int trigger = 0;
				int param = 0;
				int param2 = 0;
				if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuTeamCompeteZhengBa, ref trigger, ref param, ref param2))
				{
					UIHelper.HintGongNengOpenCondition(GongNengIDs.KuaFuTeamCompeteZhengBa, trigger, param, param2, true);
				}
				else if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -2,
						IDType = 18
					});
				}
			};
		}
	}

	public ListBox listBox;

	private ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject background;
}
