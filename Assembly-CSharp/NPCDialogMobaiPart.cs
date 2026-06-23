using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;
using XMLCreater;

public class NPCDialogMobaiPart : UserControl
{
	private MoBai CurMoBai
	{
		get
		{
			return this.MoBaiOf(this.eMoBaiType);
		}
	}

	private void InitTextInPrefabs()
	{
		this.btnDetail.Text = Global.GetLang("查看");
		this.btnMobai.Text = Global.GetLang("膜拜");
		this.lblMobaiNum.transform.localPosition = new Vector3(-370f, -239f, 0f);
		this.Bak.URL = "NetImages/GameRes/Images/Plate/mobai_bak.jpg";
		this.btnDuanWeiBang.gameObject.SetActive(false);
		this.chengzhanJoin.Text = Global.GetLang("参加城战");
		this.chengzhanRole.Text = Global.GetLang("城战规则");
		this.chengzhanJoin.Label.lineWidth = 97;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.RefreshTimes();
		this.chkModeDiamond.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.chkModeDiamond.Check)
			{
				this.ResetPromptInfo(1);
			}
		};
		this.chkModeGold.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.chkModeGold.Check)
			{
				this.ResetPromptInfo(0);
			}
		};
		this.btnDetail.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.pkKingRoleId > 0)
			{
				GameInstance.Game.SpriteGetOtherAttrib(this.pkKingRoleId);
			}
		};
		this.btnLiDaiChengZhu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.langHunLingYuKingShowDataHist == null)
			{
				GameInstance.Game.GetShengYuChengZhuRoleListInfo();
			}
			if (this.lidaichengzhu == null)
			{
				this.lidaichengzhu = U3DUtils.NEW<LiDaiChengZhuPart>();
				this.lidaichengzhu.transform.parent = base.transform;
				this.lidaichengzhu.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.lidaichengzhu.transform.localScale = new Vector3(1f, 1f, 1f);
				WindowManage.AddWindows(this.lidaichengzhu, false, null);
				this.lidaichengzhu.DPSelectedItem = delegate(object ss, DPSelectedItemEventArgs ee)
				{
					WindowManage.RemoveWindows(this.lidaichengzhu);
					Object.Destroy(this.lidaichengzhu.gameObject);
					this.lidaichengzhu = null;
				};
			}
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			}
		};
		GameInstance.Game.GetJieriFanbeiInfo();
	}

	public void InitFanbei()
	{
		if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShiLiZhengBa)
		{
			return;
		}
		if (Global.isFanbei(12) && Global.GetMapSceneUIClass() != SceneUIClasses.LingDiCaiJi)
		{
			FanbeiPrefab fanbeiPrefab = U3DUtils.NEW<FanbeiPrefab>();
			fanbeiPrefab.tetUrl.URL = "NetImages/GameRes/Images/JieriFanbei/TimesDouble.png";
			this.obj.Add(fanbeiPrefab);
			this.obj.gameObject.SetActive(true);
		}
	}

	private void RefreshTimes()
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (correctDateTime.Hour == 0 && correctDateTime.Minute == 0 && correctDateTime.Second == 0)
		{
			if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShengYu)
			{
				GameInstance.Game.GetShengYuChengZhuRoleInfo();
			}
		}
		else
		{
			base.Invoke("RefreshTimes", 1f);
		}
	}

	private void ResetPromptInfo(int mobaiType)
	{
		int num = 1;
		double[] systemParamDoubleArrayByName = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuanShengExpXiShu");
		if (systemParamDoubleArrayByName.Length > 0 && Global.Data.roleData.ChangeLifeCount < systemParamDoubleArrayByName.Length)
		{
			num = (int)systemParamDoubleArrayByName[Global.Data.roleData.ChangeLifeCount];
		}
		if (mobaiType != 0)
		{
			if (mobaiType == 1)
			{
				if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.FuQi)
				{
					if (this.CurMoBai != null)
					{
						if (this.CurMoBai.NeedJinBi != 0)
						{
							int num2 = this.CurMoBai.ZuanShiExpAward * num;
							this.lblPrompt.text = string.Format(Global.GetLang("花费{0}钻石膜拜，可获得{1}经验,{2}灵晶"), Global.GetColorStringForNGUIText(new object[]
							{
								"00FF00",
								this.CurMoBai.NeedZuanShi
							}), Global.GetColorStringForNGUIText(new object[]
							{
								"00FF00",
								num2
							}), Global.GetColorStringForNGUIText(new object[]
							{
								"00FF00",
								this.CurMoBai.ZuanShiLingJing
							}));
						}
						else
						{
							this.lblPrompt.text = string.Empty;
						}
					}
				}
				else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ZhongShen)
				{
					if (this.CurMoBai != null)
					{
						if (this.CurMoBai.NeedZuanShi != 0)
						{
							int num3 = this.CurMoBai.ZuanShiExpAward * num;
							this.lblPrompt.text = string.Format(Global.GetLang("花费{0}钻石膜拜，可获得{1}经验,{2}战功"), Global.GetColorStringForNGUIText(new object[]
							{
								"00FF00",
								this.CurMoBai.NeedZuanShi
							}), Global.GetColorStringForNGUIText(new object[]
							{
								"00FF00",
								num3
							}), Global.GetColorStringForNGUIText(new object[]
							{
								"00FF00",
								this.CurMoBai.ZuanShiZhanGongAward
							}));
						}
						else
						{
							this.lblPrompt.text = string.Empty;
						}
					}
				}
				else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShengYu)
				{
					if (this.CurMoBai != null)
					{
						if (this.CurMoBai.NeedZuanShi != 0)
						{
							int num4 = this.CurMoBai.ZuanShiExpAward * num;
							this.lblPrompt.text = string.Format(Global.GetLang("花费{0}钻石膜拜，可获得{1}经验,{2}战功"), Global.GetColorStringForNGUIText(new object[]
							{
								"00FF00",
								this.CurMoBai.NeedZuanShi
							}), Global.GetColorStringForNGUIText(new object[]
							{
								"00FF00",
								num4
							}), Global.GetColorStringForNGUIText(new object[]
							{
								"00FF00",
								this.CurMoBai.ZuanShiZhanGongAward
							}));
						}
						else
						{
							this.lblPrompt.text = string.Empty;
						}
					}
				}
				else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.PK)
				{
					int num5 = int.Parse(Global.GetMobaiZuanshiJiangliJingyan()) * num;
					this.lblPrompt.text = string.Format(Global.GetLang("花费{0}钻石膜拜，可获得{1}经验,{2}声望"), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						Global.GetMobaiZuanshiXiaohao()
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						num5.ToString()
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						Global.GetMobaiZuanshiJiangliShengwang()
					}));
				}
				else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.LuoLan)
				{
					string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("LuoLanZuanShiMoBai", ',');
					if (systemParamStringArrayByName.Length >= 3)
					{
						this.lblPrompt.text = string.Format(Global.GetLang("花费{0}钻石膜拜，可获得{1}经验,{2}声望"), Global.GetColorStringForNGUIText(new object[]
						{
							"00FF00",
							systemParamStringArrayByName[0]
						}), Global.GetColorStringForNGUIText(new object[]
						{
							"00FF00",
							Convert.ToInt32(systemParamStringArrayByName[1]) * num
						}), Global.GetColorStringForNGUIText(new object[]
						{
							"00FF00",
							systemParamStringArrayByName[2]
						}));
					}
					else
					{
						this.lblPrompt.text = string.Empty;
					}
				}
				else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.JunTuanDiGong)
				{
					this.lblPrompt.text = string.Format(Global.GetLang("花费{0}钻石膜拜，可获得{1}经验,{2}战功"), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						this.CurMoBai.NeedZuanShi
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						Convert.ToInt32(this.CurMoBai.ZuanShiExpAward) * num
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						this.CurMoBai.ZuanShiZhanGongAward
					}));
				}
				else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ZhanMengLianSai)
				{
					this.lblPrompt.text = string.Format(Global.GetLang("花费{0}钻石膜拜，可获得{1}"), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						this.CurMoBai.NeedZuanShi
					}), this.GetAwardString(this.CurMoBai, 1, num));
				}
				else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShiLiZhengBa)
				{
					this.lblPrompt.text = string.Format(Global.GetLang("花费{0}钻石膜拜，可获得{1}经验,{2}战功"), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						this.CurMoBai.NeedZuanShi
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						Convert.ToInt32(this.CurMoBai.ZuanShiExpAward) * num
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						this.CurMoBai.ZuanShiZhanGongAward
					}));
				}
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.FuQi)
		{
			if (this.CurMoBai != null)
			{
				if (this.CurMoBai.NeedJinBi != 0)
				{
					int num6 = this.CurMoBai.JinBiExpAward * num;
					this.lblPrompt.text = string.Format(Global.GetLang("花费{0}金币膜拜，可获得{1}经验,{2}灵晶"), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						this.CurMoBai.NeedJinBi
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						num6
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						this.CurMoBai.JinBiLingJing
					}));
				}
				else
				{
					this.lblPrompt.text = string.Empty;
				}
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ZhongShen)
		{
			if (this.CurMoBai != null)
			{
				if (this.CurMoBai.NeedJinBi != 0)
				{
					int num7 = this.CurMoBai.JinBiExpAward * num;
					this.lblPrompt.text = string.Format(Global.GetLang("花费{0}金币膜拜，可获得{1}经验,{2}战功"), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						this.CurMoBai.NeedJinBi
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						num7
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						this.CurMoBai.JinBiZhanGongAward
					}));
				}
				else
				{
					this.lblPrompt.text = string.Empty;
				}
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShengYu)
		{
			if (this.CurMoBai != null)
			{
				if (this.CurMoBai.NeedJinBi != 0)
				{
					int num8 = this.CurMoBai.JinBiExpAward * num;
					this.lblPrompt.text = string.Format(Global.GetLang("花费{0}金币膜拜，可获得{1}经验,{2}战功"), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						this.CurMoBai.NeedJinBi
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						num8
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00FF00",
						this.CurMoBai.JinBiZhanGongAward
					}));
				}
				else
				{
					this.lblPrompt.text = string.Empty;
				}
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.PK)
		{
			int num9 = int.Parse(Global.GetMobaiJinbiJiangliJingyan()) * num;
			this.lblPrompt.text = string.Format(Global.GetLang("花费{0}金币膜拜，可获得{1}经验,{2}声望"), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				Global.GetMobaiJinbiXiaohao()
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				num9.ToString()
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				Global.GetMobaiJinbiJiangliShengwang()
			}));
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.LuoLan)
		{
			string[] systemParamStringArrayByName2 = ConfigSystemParam.GetSystemParamStringArrayByName("LuoLanJiBiMoBai", ',');
			if (systemParamStringArrayByName2.Length >= 3)
			{
				this.lblPrompt.text = string.Format(Global.GetLang("花费{0}金币膜拜，可获得{1}经验,{2}声望"), Global.GetColorStringForNGUIText(new object[]
				{
					"00FF00",
					systemParamStringArrayByName2[0]
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"00FF00",
					Convert.ToInt32(systemParamStringArrayByName2[1]) * num
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"00FF00",
					systemParamStringArrayByName2[2]
				}));
			}
			else
			{
				this.lblPrompt.text = string.Empty;
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.JunTuanDiGong)
		{
			this.lblPrompt.text = string.Format(Global.GetLang("花费{0}金币膜拜，可获得{1}经验,{2}战功"), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				this.CurMoBai.NeedJinBi
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				Convert.ToInt32(this.CurMoBai.JinBiExpAward) * num
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				this.CurMoBai.JinBiZhanGongAward
			}));
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.JunTuanHuangMo)
		{
			this.lblPrompt.text = string.Format(Global.GetLang("花费{0}金币膜拜，可获得{1}经验,{2}战功"), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				this.CurMoBai.NeedJinBi
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				Convert.ToInt32(this.CurMoBai.JinBiExpAward) * num
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				this.CurMoBai.JinBiZhanGongAward
			}));
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ZhanMengLianSai)
		{
			this.lblPrompt.text = string.Format(Global.GetLang("花费{0}金币膜拜，可获得{1}"), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				this.CurMoBai.NeedJinBi
			}), this.GetAwardString(this.CurMoBai, 0, num));
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShiLiZhengBa)
		{
			this.lblPrompt.text = string.Format(Global.GetLang("花费{0}金币膜拜，可获得{1}经验,{2}战功"), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				this.CurMoBai.NeedJinBi
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				Convert.ToInt32(this.CurMoBai.JinBiExpAward) * num
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"00FF00",
				this.CurMoBai.JinBiZhanGongAward
			}));
		}
	}

	private string GetAwardString(MoBai CurMoBai, byte Type, int ExpProportion)
	{
		string text = string.Empty;
		if (Type == 0)
		{
			if (0 < CurMoBai.JinBiZhanGongAward)
			{
				text = text + Global.GetColorStringForNGUIText(new object[]
				{
					"00FF00",
					CurMoBai.JinBiZhanGongAward
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"e2b36d",
					Global.GetLang("战功")
				});
			}
			if (0 < CurMoBai.JinBiExpAward)
			{
				text = text + Global.GetColorStringForNGUIText(new object[]
				{
					"00FF00",
					CurMoBai.JinBiExpAward * ExpProportion
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"e2b36d",
					Global.GetLang("经验")
				});
			}
			if (0 < CurMoBai.JinBiLingJing)
			{
				text = text + Global.GetColorStringForNGUIText(new object[]
				{
					"00FF00",
					CurMoBai.JinBiLingJing
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"e2b36d",
					Global.GetLang("灵晶")
				});
			}
			if (0 < CurMoBai.JinBishenlijinghua)
			{
				text = text + Global.GetColorStringForNGUIText(new object[]
				{
					"00FF00",
					CurMoBai.JinBishenlijinghua
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"e2b36d",
					Global.GetLang("神力精华")
				});
			}
		}
		else
		{
			if (0 < CurMoBai.ZuanShiZhanGongAward)
			{
				text = text + Global.GetColorStringForNGUIText(new object[]
				{
					"00FF00",
					CurMoBai.ZuanShiZhanGongAward
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"e2b36d",
					Global.GetLang("战功")
				});
			}
			if (0 < CurMoBai.ZuanShiExpAward)
			{
				text = text + Global.GetColorStringForNGUIText(new object[]
				{
					"00FF00",
					CurMoBai.ZuanShiExpAward * ExpProportion
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"e2b36d",
					Global.GetLang("经验")
				});
			}
			if (0 < CurMoBai.ZuanShiLingJing)
			{
				text = text + Global.GetColorStringForNGUIText(new object[]
				{
					"00FF00",
					CurMoBai.ZuanShiLingJing
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"e2b36d",
					Global.GetLang("灵晶")
				});
			}
			if (0 < CurMoBai.ZuanShishenlijinghua)
			{
				text = text + Global.GetColorStringForNGUIText(new object[]
				{
					"00FF00",
					CurMoBai.ZuanShishenlijinghua
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"e2b36d",
					Global.GetLang("神力精华")
				});
			}
		}
		return text;
	}

	public void InitPartData()
	{
		this.ResetPromptInfo(0);
		this.mGoToGlorgHallBtn.gameObject.SetActive(false);
		if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.JunTuanDiGong)
		{
			this.btnDetail.gameObject.SetActive(false);
			this.lblMobaiNum.text = "0/1";
			GameInstance.Game.SendGetJunTuanRoleInfo();
			this.chengzhanJoin.gameObject.SetActive(false);
			this.chengzhanRole.gameObject.SetActive(false);
			this.btnDuanWeiBang.gameObject.SetActive(false);
			this.btnLiDaiChengZhu.gameObject.SetActive(false);
			this.texNameBak.gameObject.SetActive(false);
			this.btnMobai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int type = (!this.chkModeGold.Check) ? 2 : 1;
				GameInstance.Game.SendJunTuanMoBaiInfo(type);
			};
		}
		if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.JunTuanHuangMo)
		{
			this.btnDetail.gameObject.SetActive(false);
			this.lblMobaiNum.text = "0/1";
			GameInstance.Game.SendGetJunTuanRoleInfo();
			this.chengzhanJoin.gameObject.SetActive(false);
			this.chengzhanRole.gameObject.SetActive(false);
			this.btnDuanWeiBang.gameObject.SetActive(false);
			this.btnLiDaiChengZhu.gameObject.SetActive(false);
			this.texNameBak.gameObject.SetActive(false);
			this.btnMobai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int type = (!this.chkModeGold.Check) ? 2 : 1;
				GameInstance.Game.SendJunTuanMoBaiInfo(type);
			};
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShengYu)
		{
			this.btnDetail.gameObject.SetActive(false);
			this.lblMobaiNum.text = "0/1";
			GameInstance.Game.GetShengYuChengZhuRoleInfo();
			GameInstance.Game.SpriteQueryBangHuiDetail(Global.Data.roleData.Faction);
			this.chengzhanJoin.gameObject.SetActive(true);
			this.chengzhanRole.gameObject.SetActive(true);
			GameInstance.Game.GetShengYuChengZhuRoleListInfo();
			this.btnMobai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int nType = (!this.chkModeGold.Check) ? 2 : 1;
				GameInstance.Game.ShengYuChengZhuAdrationCmd(nType);
			};
			this.chengzhanJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhamMeng))
				{
					if (string.IsNullOrEmpty(Global.Data.roleData.BHName))
					{
						Super.HintMainText(Global.GetLang("需要加入任意战盟后才可参与"), 10, 3);
					}
					else if (this.ZhanMengLev != 0 && this.ZhanMengLev < this.GetZhanmengLevelLimit())
					{
						Super.HintMainText(Global.GetLang("需战盟等级达到5级"), 10, 3);
					}
					else
					{
						PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
						{
							ID = 209
						});
						if (this.DPSelectedItem != null)
						{
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								IDType = -10
							});
						}
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("需要开启战盟功能并加入任意战盟后才可参与"), 10, 3);
				}
			};
			this.chengzhanRole.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (null == this.WolfSoulField)
				{
					this.WolfSoulField = U3DUtils.NEW<WolfSoulField_Rule>();
					this.WolfSoulField.transform.parent = base.transform;
					this.WolfSoulField.transform.localPosition = new Vector3(0f, 0f, 0f);
					this.WolfSoulField.transform.localScale = new Vector3(1f, 1f, 1f);
					WindowManage.AddWindows(this.WolfSoulField, false, null);
					this.WolfSoulField.DPSelectItem = delegate(object ss, DPSelectedItemEventArgs ee)
					{
						WindowManage.RemoveWindows(this.WolfSoulField);
						Object.Destroy(this.WolfSoulField.gameObject);
						this.WolfSoulField = null;
					};
				}
			};
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.PK)
		{
			this.chengzhanJoin.gameObject.SetActive(false);
			this.chengzhanRole.gameObject.SetActive(false);
			this.btnLiDaiChengZhu.gameObject.SetActive(false);
			GameInstance.Game.SpriteGetPKKingAdrationInfoCmd();
			this.btnMobai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int nType = (!this.chkModeGold.Check) ? 2 : 1;
				GameInstance.Game.SpritePKKingAdrationCmd(nType);
			};
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.LuoLan)
		{
			this.chengzhanJoin.gameObject.SetActive(true);
			this.chengzhanRole.gameObject.SetActive(true);
			GameInstance.Game.GetLuoLanChengZhuRoleInfo();
			this.btnMobai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int nType = (!this.chkModeGold.Check) ? 2 : 1;
				GameInstance.Game.LuoLanChengZhuAdrationCmd(nType);
			};
			this.chengzhanJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhamMeng))
				{
					if (string.IsNullOrEmpty(Global.Data.roleData.BHName))
					{
						Super.HintMainText(Global.GetLang("需要加入任意战盟后才可参与"), 10, 3);
					}
					else
					{
						PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
						{
							ID = 208
						});
						if (this.DPSelectedItem != null)
						{
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								IDType = -10
							});
						}
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("需要开启战盟功能并加入任意战盟后才可参与"), 10, 3);
				}
			};
			this.chengzhanRole.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (null == this.luoLanRolePart)
				{
					this.luoLanRolePart = U3DUtils.NEW<LuoLanRolePart>();
					this.luoLanRolePart.transform.parent = base.transform;
					this.luoLanRolePart.transform.localPosition = new Vector3(0f, 0f, 0f);
					this.luoLanRolePart.transform.localScale = new Vector3(1f, 1f, 1f);
					WindowManage.AddWindows(this.luoLanRolePart, false, null);
					this.luoLanRolePart.DPSelectedItem = delegate(object ss, DPSelectedItemEventArgs ee)
					{
						WindowManage.RemoveWindows(this.luoLanRolePart);
						Object.Destroy(this.luoLanRolePart.gameObject);
						this.luoLanRolePart = null;
					};
				}
			};
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.FuQi)
		{
			this.Bak.URL = "NetImages/GameRes/Images/Wish/MoBaiBG.jpg.qj";
			this.chengzhanJoin.gameObject.SetActive(false);
			this.chengzhanRole.gameObject.SetActive(false);
			this.btnLiDaiChengZhu.gameObject.SetActive(false);
			this.btnDetail.gameObject.SetActive(false);
			this.btnDuanWeiBang.gameObject.SetActive(true);
			GameInstance.Game.GetAdmireDataForCoupleWish();
			this.btnMobai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int type = (!this.chkModeGold.Check) ? 2 : 1;
				GameInstance.Game.GetAdmireStateForCoupleWish(this.DbCoupleId, type);
			};
			this.btnDuanWeiBang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				PlayZone.GlobalPlayZone.OpenLoversWishPartWindow();
			};
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ZhongShen)
		{
			this.chengzhanJoin.gameObject.SetActive(true);
			this.chengzhanRole.gameObject.SetActive(false);
			this.btnLiDaiChengZhu.gameObject.SetActive(false);
			this.btnDetail.gameObject.SetActive(false);
			this.btnDuanWeiBang.gameObject.SetActive(false);
			GameInstance.Game.GetZhongShenChengZhuRoleInfo();
			GameInstance.Game.GetDaoJiShiTime();
			this.chengzhanJoin.Text = Global.GetLang("争霸详情");
			this.btnMobai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int nType = (!this.chkModeGold.Check) ? 2 : 1;
				GameInstance.Game.ZhongShenChengZhuAdrationCmd(nType);
			};
			this.chengzhanJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.miniData != null && this.miniData.IsThisMonthInActivity && this.miniData.IsZhengBaOpened)
				{
					PlayZone.GlobalPlayZone.ShowActivityWindow(false, 1200, -1);
					PlayZone.GlobalPlayZone.ShowZhongShenZhengBaWindow();
				}
				else
				{
					Super.HintMainText(Global.GetLang("功能未开启"), 10, 3);
				}
			};
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ZhanMengLianSai)
		{
			this.chengzhanJoin.gameObject.SetActive(false);
			this.chengzhanRole.gameObject.SetActive(false);
			this.btnLiDaiChengZhu.gameObject.SetActive(false);
			this.btnDetail.gameObject.SetActive(false);
			this.btnDuanWeiBang.gameObject.SetActive(false);
			GameInstance.Game.SendZhanMengLianSaiGetMoBaiData();
			this.btnMobai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int type = (!this.chkModeGold.Check) ? 2 : 1;
				GameInstance.Game.SendZhanMengLianSaiMoBai(type);
			};
			this.mGoToGlorgHallBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (Global.IsHavingBangHui(Global.Data.roleData))
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = 1499
					});
				}
				else
				{
					Super.HintMainText(Global.GetLang("请先加入或创建战盟"), 10, 3);
				}
			};
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShiLiZhengBa)
		{
			this.chengzhanJoin.gameObject.SetActive(false);
			this.chengzhanRole.gameObject.SetActive(false);
			this.btnLiDaiChengZhu.gameObject.SetActive(false);
			this.btnDetail.gameObject.SetActive(false);
			GameInstance.Game.SpriteGetShiLiAdrationInfoCmd();
			this.btnMobai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int nType = (!this.chkModeGold.Check) ? 2 : 1;
				GameInstance.Game.SpriteShiLiAdrationCmd(nType);
			};
		}
	}

	public void NotifyLoversWishInfo(int LeftRoleId, int RightRoleId, string mobaiNumber)
	{
		if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.FuQi)
		{
			this.mobaiLimit = this.CurMoBai.mobaiLimit;
			if (Global.Data.roleData.RoleID == LeftRoleId || Global.Data.roleData.RoleID == RightRoleId)
			{
				this.mobaiLimit += this.CurMoBai.extraNumber;
			}
			if (Global.Data.roleData.VIPLevel != 0)
			{
				this.mobaiLimit += int.Parse(Global.GetMobaiExteraVIPNumber(Global.Data.roleData.VIPLevel));
			}
			this.texNameBak.gameObject.SetActive(false);
		}
		this.mobaiLimit = this.FanBei(this.mobaiLimit);
		this.mobaiUsed = int.Parse(mobaiNumber);
		this.lblMobaiNum.text = this.mobaiUsed + "/" + this.mobaiLimit;
	}

	private int FanBei(int NowNum)
	{
		int num = 0;
		if (Global.isFanbei(12) && this.eMoBaiType != NPCDialogMobaiPart.UIMoBaiType.ShiLiZhengBa)
		{
			num = 2;
		}
		if (Global.isFanbei(211))
		{
			double num2 = 0.0;
			if (double.TryParse(Global.JieriFanbeiInfo[211].ExtArg1, ref num2))
			{
				num += (int)num2;
			}
		}
		if (num == 0)
		{
			num = 1;
		}
		return NowNum * num;
	}

	public void NotifyPKInfo(string roleId, string mobaiNumber)
	{
		this.pkKingRoleId = int.Parse(roleId);
		if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShengYu)
		{
			if (this.pkKingRoleId > 0)
			{
				this.chengzhanRole.gameObject.SetActive(true);
				this.chengzhanJoin.gameObject.SetActive(true);
				this.ShengYuKing.gameObject.SetActive(true);
			}
			else
			{
				this.HideTitle();
			}
		}
		if (this.pkKingRoleId > 0)
		{
			if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.PK)
			{
				GameInstance.Game.GetPKKingRoleLooks(this.pkKingRoleId);
			}
			if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.LuoLan)
			{
				GameInstance.Game.GetLuoLanKingRoleLooks(this.pkKingRoleId);
			}
			this.texNameBak.gameObject.SetActive(true);
		}
		else
		{
			GameInstance.Game.SpriteGetUsingGoodsDataList(this.pkKingRoleId, false);
			this.texNameBak.gameObject.SetActive(false);
		}
		if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.PK)
		{
			this.mobaiLimit = int.Parse(Global.GetMobaiNumber());
			if (Global.Data.roleData.RoleID == int.Parse(roleId))
			{
				this.mobaiLimit += int.Parse(Global.GetMobaiPKKingNumber());
			}
			if (Global.Data.roleData.VIPLevel != 0)
			{
				this.mobaiLimit += int.Parse(Global.GetMobaiExteraVIPNumber(Global.Data.roleData.VIPLevel));
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.LuoLan)
		{
			this.mobaiLimit = Convert.ToInt32(ConfigSystemParam.GetSystemParamByName("LuoLanMoBaiNumber", true));
			if (Global.Data.roleData.RoleCommonUseIntPamams[26] > 0)
			{
				this.mobaiLimit += Convert.ToInt32(ConfigSystemParam.GetSystemParamByName("LuoLanKingMoBaiNum", true));
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShengYu)
		{
			this.mobaiLimit = this.CurMoBai.mobaiLimit;
			if (Global.Data.roleData.RoleID == int.Parse(roleId))
			{
				this.mobaiLimit += this.CurMoBai.extraNumber;
			}
			if (Global.Data.roleData.VIPLevel != 0)
			{
				this.mobaiLimit += int.Parse(Global.GetMobaiExteraVIPNumber(Global.Data.roleData.VIPLevel));
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ZhongShen)
		{
			this.mobaiLimit = this.CurMoBai.mobaiLimit;
			if (Global.Data.roleData.RoleID == int.Parse(roleId))
			{
				this.mobaiLimit += this.CurMoBai.extraNumber;
			}
			if (Global.Data.roleData.VIPLevel != 0)
			{
				this.mobaiLimit += int.Parse(Global.GetMobaiExteraVIPNumber(Global.Data.roleData.VIPLevel));
			}
		}
		else if (this.eMoBaiType != NPCDialogMobaiPart.UIMoBaiType.ZhanMengLianSai)
		{
			if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShiLiZhengBa)
			{
				this.mobaiLimit = this.CurMoBai.mobaiLimit;
				if (Global.Data.roleData.RoleID == int.Parse(roleId))
				{
					this.mobaiLimit += this.CurMoBai.extraNumber;
				}
			}
		}
		this.mobaiLimit = this.FanBei(this.mobaiLimit);
		this.mobaiUsed = int.Parse(mobaiNumber);
		this.lblMobaiNum.text = this.mobaiUsed + "/" + this.mobaiLimit;
	}

	public void NotifyJunTuanInfo(string roleId, string mobaiNumber)
	{
		if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.JunTuanDiGong || this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.JunTuanHuangMo)
		{
			if (ConvertExt.SafeConvertToInt32(roleId) > 0)
			{
				this.texNameBak.gameObject.SetActive(true);
			}
			else
			{
				this.texNameBak.gameObject.SetActive(false);
			}
			this.mobaiLimit = this.CurMoBai.mobaiLimit;
			if (Global.Data.roleData.RoleID == ConvertExt.SafeConvertToInt32(roleId))
			{
				this.mobaiLimit += this.CurMoBai.extraNumber;
			}
			this.mobaiUsed = int.Parse(mobaiNumber);
			this.lblMobaiNum.text = this.mobaiUsed + "/" + this.mobaiLimit;
		}
	}

	public void NotifyMobaiResult(string result)
	{
		int num = int.Parse(result);
		int num2 = num;
		switch (num2 + 5)
		{
		case 0:
			if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShiLiZhengBa)
			{
				Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
			}
			else
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			}
			break;
		case 1:
			if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShiLiZhengBa)
			{
				Super.HintMainText(Global.GetLang("金币不足"), 10, 3);
			}
			else
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			}
			break;
		case 2:
			Super.HintMainText(Global.GetLang("玩家今天的膜拜次数已经用完！"), 10, 3);
			break;
		case 3:
			Super.HintMainText(Global.GetLang("配置文件出错！"), 10, 3);
			break;
		case 4:
			if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.PK)
			{
				Super.HintMainText(Global.GetLang("当前沒有PK之王！"), 10, 3);
			}
			else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.LuoLan)
			{
				Super.HintMainText(Global.GetLang("当前沒有罗兰城主！"), 10, 3);
			}
			else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShengYu)
			{
				Super.HintMainText(Global.GetLang("当前沒有圣域城主！"), 10, 3);
			}
			else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.JunTuanDiGong)
			{
				Super.HintMainText(Global.GetLang("没有地宫军团长"), 10, 3);
			}
			else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.JunTuanHuangMo)
			{
				Super.HintMainText(Global.GetLang("没有荒漠军团长"), 10, 3);
			}
			else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ZhanMengLianSai)
			{
				Super.HintMainText(Global.GetLang("没有联赛霸主"), 10, 3);
			}
			break;
		default:
			if (num2 != -16)
			{
				string errMsg = StdErrorCode.GetErrMsg(num, true, true);
				Super.HintMainText(errMsg, 10, 3);
			}
			else
			{
				Super.HintMainText(Global.GetLang("玩家今天的膜拜次数已经用完！"), 10, 3);
			}
			break;
		case 6:
			Super.HintMainText(Global.GetLang("膜拜成功！"), 10, 3);
			this.mobaiUsed++;
			this.lblMobaiNum.text = this.mobaiUsed + "/" + this.mobaiLimit;
			break;
		}
	}

	public void NotifyMobaiResultZhongShen(string result)
	{
		int num = int.Parse(result);
		int num2 = num;
		switch (num2 + 16)
		{
		case 0:
			Super.HintMainText(Global.GetLang("玩家今天的膜拜次数已经用完！"), 10, 3);
			return;
		case 6:
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		case 7:
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			return;
		case 11:
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		case 12:
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			return;
		case 13:
			Super.HintMainText(Global.GetLang("玩家今天的膜拜次数已经用完！"), 10, 3);
			return;
		case 14:
			Super.HintMainText(Global.GetLang("配置文件出错！"), 10, 3);
			return;
		case 15:
			if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.PK)
			{
				Super.HintMainText(Global.GetLang("当前沒有PK之王！"), 10, 3);
			}
			else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.LuoLan)
			{
				Super.HintMainText(Global.GetLang("当前沒有罗兰城主！"), 10, 3);
			}
			else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShengYu)
			{
				Super.HintMainText(Global.GetLang("当前沒有圣域城主！"), 10, 3);
			}
			return;
		case 16:
		case 17:
		case 18:
			Super.HintMainText(Global.GetLang("膜拜成功！"), 10, 3);
			this.mobaiUsed++;
			this.lblMobaiNum.text = this.mobaiUsed + "/" + this.mobaiLimit;
			return;
		}
		string errMsg = StdErrorCode.GetErrMsg(num, true, true);
		Super.HintMainText(errMsg, 10, 3);
	}

	public void GetRoleUsingGoodsDataList(RoleData4Selector roleData4Selector)
	{
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		if (roleData4Selector == null || roleData4Selector.RoleID < 0)
		{
			return;
		}
		this.lblForce.text = string.Empty + roleData4Selector.CombatForce;
		this.lblPkName.text = roleData4Selector.RoleName;
		this.LoadRoleRes(roleData4Selector);
	}

	public void GetRoleUsingGoodsDataList(RoleData4Selector roleData4Selector, int AdmireCount)
	{
		this.mobaiLimit = this.CurMoBai.extraNumber;
		if (roleData4Selector != null)
		{
			this.lblForce.text = string.Empty + roleData4Selector.CombatForce;
			this.lblPkName.text = roleData4Selector.RoleName;
			if (null != this.oldRole)
			{
				Object.Destroy(this.oldRole);
				this.oldRole = null;
			}
			if (roleData4Selector.RoleID < 0)
			{
				return;
			}
			this.LoadRoleRes(roleData4Selector);
			if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ZhanMengLianSai && Global.Data.roleData.RoleID == roleData4Selector.RoleID)
			{
				this.mobaiLimit += this.CurMoBai.extraNumber;
			}
		}
		this.mobaiLimit = this.FanBei(this.mobaiLimit);
		this.mobaiUsed = AdmireCount;
		this.lblMobaiNum.text = AdmireCount + "/" + this.mobaiLimit;
	}

	public void GetRoleUsingGoodsDataList(RoleData4Selector roleData4SelectorLeft, RoleData4Selector roleData4SelectorRight)
	{
		this.lblForce.text = string.Empty;
		this.lblPkName.text = string.Empty;
		this.LoversManName.text = ((roleData4SelectorLeft != null) ? roleData4SelectorLeft.RoleName : string.Empty);
		this.LoversWifeName.text = ((roleData4SelectorRight != null) ? roleData4SelectorRight.RoleName : string.Empty);
		if (string.IsNullOrEmpty(this.LoversManName.text) && string.IsNullOrEmpty(this.LoversWifeName.text))
		{
			this.LoversObj.SetActive(false);
		}
		else
		{
			this.LoversObj.SetActive(true);
		}
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		if (roleData4SelectorLeft != null && roleData4SelectorLeft.RoleID > 0)
		{
			if (this.leftResLoader != null)
			{
				this.leftResLoader.Stop();
			}
			this.leftResLoader = this.LoadRoleRes(roleData4SelectorLeft, this.LeftRoleModal);
		}
		if (roleData4SelectorRight != null && roleData4SelectorRight.RoleID > 0)
		{
			if (this.rightResLoader != null)
			{
				this.rightResLoader.Stop();
			}
			this.rightResLoader = this.LoadRoleRes(roleData4SelectorRight, this.RightRoleModal);
		}
	}

	private RoleResLoader LoadRoleRes(RoleData4Selector roleData4Selector, Modal3DShow RoleModal)
	{
		RoleResLoader roleResLoader = null;
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		if (null != RoleModal)
		{
			RoleModal.Clear();
		}
		if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.FuQi)
		{
			int fashionGoodsID = Global.GetFashionGoodsID(roleData4Selector.FashionWingsID);
			roleResLoader = UIHelper.LoadRoleRes(RoleModal, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1f, fashionGoodsID, null, false);
			if (roleResLoader != null)
			{
				this.HideTitle();
			}
		}
		return roleResLoader;
	}

	public override void Destroy()
	{
		if (this.bossResLoader != null)
		{
			this.bossResLoader.Stop();
			this.bossResLoader = null;
		}
		if (this.rightResLoader != null)
		{
			this.rightResLoader.Stop();
			this.rightResLoader = null;
		}
		if (this.leftResLoader != null)
		{
			this.leftResLoader.Stop();
			this.leftResLoader = null;
		}
		base.Destroy();
	}

	private void LoadRoleRes(RoleData4Selector roleData4Selector)
	{
		this.mChengHaoImage.gameObject.SetActive(false);
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		if (null != this.m_BossModal)
		{
			this.m_BossModal.Clear();
		}
		if (this.bossResLoader != null)
		{
			this.bossResLoader.Stop();
		}
		if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.LuoLan)
		{
			int luolanFashionWingID = Global.GetLuolanFashionWingID(1);
			this.bossResLoader = UIHelper.LoadRoleRes(this.m_BossModal, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1f, luolanFashionWingID, null, false);
			if (this.bossResLoader != null)
			{
				this.HideTitle();
				this.luoLanKing.gameObject.SetActive(true);
			}
			else
			{
				this.HideTitle();
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.PK)
		{
			int fashionGoodsID = Global.GetFashionGoodsID(roleData4Selector.FashionWingsID);
			this.bossResLoader = UIHelper.LoadRoleRes(this.m_BossModal, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1f, fashionGoodsID, null, false);
			if (this.bossResLoader != null)
			{
				this.HideTitle();
				this.PKKing.gameObject.SetActive(true);
			}
			else
			{
				this.HideTitle();
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShengYu)
		{
			int fashionGoodsID2 = Global.GetFashionGoodsID(roleData4Selector.FashionWingsID);
			this.bossResLoader = UIHelper.LoadRoleRes(this.m_BossModal, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1f, fashionGoodsID2, null, false);
			if (this.bossResLoader != null)
			{
				this.HideTitle();
				this.ShengYuKing.gameObject.SetActive(true);
			}
			else
			{
				this.HideTitle();
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ZhongShen)
		{
			int fashionGoodsID3 = Global.GetFashionGoodsID(roleData4Selector.FashionWingsID);
			this.bossResLoader = UIHelper.LoadRoleRes(this.m_BossModal, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1f, fashionGoodsID3, null, false);
			if (this.bossResLoader != null)
			{
				this.HideTitle();
				this.zhongShenZhiWang.gameObject.SetActive(true);
			}
			else
			{
				this.HideTitle();
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.JunTuanDiGong)
		{
			int fashionGoodsID4 = Global.GetFashionGoodsID(roleData4Selector.FashionWingsID);
			this.bossResLoader = UIHelper.LoadRoleRes(this.m_BossModal, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1f, fashionGoodsID4, null, false);
			if (this.bossResLoader != null)
			{
				this.HideTitle();
				this.junTuanCaiJiChengHao.gameObject.SetActive(true);
				this.junTuanCaiJiChengHao.spriteName = "diGongLingZhu";
			}
			else
			{
				this.HideTitle();
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.JunTuanHuangMo)
		{
			int fashionGoodsID5 = Global.GetFashionGoodsID(roleData4Selector.FashionWingsID);
			this.bossResLoader = UIHelper.LoadRoleRes(this.m_BossModal, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1f, fashionGoodsID5, null, false);
			if (this.bossResLoader != null)
			{
				this.HideTitle();
				this.junTuanCaiJiChengHao.gameObject.SetActive(true);
				this.junTuanCaiJiChengHao.spriteName = "huangMoLingZhu";
			}
			else
			{
				this.HideTitle();
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ZhanMengLianSai)
		{
			int fashionWingGoodsId = (int)ConfigSystemParam.GetSystemParamIntByName("LeagueWing");
			this.bossResLoader = UIHelper.LoadRoleRes(this.m_BossModal, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1f, fashionWingGoodsId, null, false);
			if (null != this.obj)
			{
				this.HideTitle();
				this.junTuanCaiJiChengHao.gameObject.SetActive(false);
				this.mChengHaoImage.gameObject.SetActive(true);
				this.mChengHaoImage.URL = "NetImages/GameRes/Images/ChengHaoTeShu/SpecialTitle_11.png";
				this.mChengHaoImage.ImageDownloaded = delegate(object g)
				{
					this.mChengHaoImage.transform.localScale = new Vector3((float)this.mChengHaoImage.ItsSizeWidth, (float)this.mChengHaoImage.ItsSizeHeight, 0f);
				};
			}
			else
			{
				this.HideTitle();
			}
		}
		else if (this.eMoBaiType == NPCDialogMobaiPart.UIMoBaiType.ShiLiZhengBa)
		{
			int fashionGoodsID6 = Global.GetFashionGoodsID(roleData4Selector.FashionWingsID);
			this.mChengHaoImage.gameObject.SetActive(true);
			this.zhongShenZhiWang.gameObject.SetActive(false);
			MUComp compByMapCode = ShiLiData.GetCompByMapCode(Global.Data.roleData.MapCode);
			MUCompLevel compLevelByCompIDAndLevel = ShiLiData.GetCompLevelByCompIDAndLevel(compByMapCode.CompID, 1);
			if (compLevelByCompIDAndLevel == null)
			{
				this.mChengHaoImage.URL = string.Empty;
			}
			else
			{
				this.mChengHaoImage.URL = ShiLiData.GetSpecialTitleURL(compLevelByCompIDAndLevel);
			}
			this.bossResLoader = UIHelper.LoadRoleRes(this.m_BossModal, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1f, fashionGoodsID6, null, false);
			if (this.bossResLoader != null)
			{
				this.HideTitle();
				this.zhongShenZhiWang.gameObject.SetActive(true);
			}
			else
			{
				this.HideTitle();
			}
		}
	}

	public bool IsModalVisable
	{
		set
		{
			if (null != this.m_BossModal)
			{
				this.m_BossModal.transform.localScale = ((!value) ? Vector3.zero : new Vector3(1.2f, 1.2f, 1f));
			}
		}
	}

	public void NotifyLiDaiChengZhuResult(bool isShow)
	{
		this.btnLiDaiChengZhu.gameObject.SetActive(isShow);
	}

	public void NotifyZhanMengLev(int lev)
	{
		this.ZhanMengLev = lev;
		Global.zhanmengLevel = lev;
	}

	private int GetZhanmengLevelLimit()
	{
		SystemOpenVO systemOpenVOByID = ConfigSystemOpen.GetSystemOpenVOByID(71);
		if (systemOpenVOByID != null)
		{
			return systemOpenVOByID.TimeParameters;
		}
		return 5;
	}

	public void GetFuQiZhuFuMoBaiData(CoupleWishTop1AdmireData Data)
	{
		this.DbCoupleId = Data.DbCoupleId;
	}

	public void RefreshMoBai()
	{
		this.mobaiUsed++;
		this.lblMobaiNum.text = this.mobaiUsed + "/" + this.mobaiLimit;
	}

	private void HideTitle()
	{
		this.luoLanKing.gameObject.SetActive(false);
		this.PKKing.gameObject.SetActive(false);
		this.ShengYuKing.gameObject.SetActive(false);
		this.zhongShenZhiWang.gameObject.SetActive(false);
		this.junTuanCaiJiChengHao.gameObject.SetActive(false);
	}

	private Dictionary<int, MoBai> MoBaiDic
	{
		get
		{
			if (this._MoBaiDic.Count > 0)
			{
				return this._MoBaiDic;
			}
			XElement gameResXml = Global.GetGameResXml("Config/MoBai.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"圣域城主膜拜数据配置错误"
				});
				return null;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Mobai");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				MoBai moBai = new MoBai();
				moBai.ID = Global.GetXElementAttributeInt(xelement, "ID");
				moBai.NeedZuanShi = Global.GetXElementAttributeInt(xelement, "NeedZuanShi");
				moBai.ZuanShiZhanGongAward = Global.GetXElementAttributeInt(xelement, "ZuanShiZhanGongAward");
				moBai.NeedJinBi = Global.GetXElementAttributeInt(xelement, "NeedJinBi");
				moBai.JinBiZhanGongAward = Global.GetXElementAttributeInt(xelement, "JinBiZhanGongAward");
				moBai.mobaiLimit = Global.GetXElementAttributeInt(xelement, "Number");
				moBai.extraNumber = Global.GetXElementAttributeInt(xelement, "ExtraNumber");
				moBai.JinBiExpAward = Global.GetXElementAttributeInt(xelement, "JinBiExpAward");
				moBai.ZuanShiExpAward = Global.GetXElementAttributeInt(xelement, "ZuanShiExpAward");
				moBai.JinBiLingJing = Global.GetXElementAttributeInt(xelement, "JinBiLingJing");
				moBai.ZuanShiLingJing = Global.GetXElementAttributeInt(xelement, "ZuanShiLingJing");
				moBai.JinBishenlijinghua = Global.GetXElementAttributeInt(xelement, "JinBishenlijinghua");
				moBai.ZuanShishenlijinghua = Global.GetXElementAttributeInt(xelement, "ZuanShishenlijinghua");
				if (!this._MoBaiDic.ContainsKey(moBai.ID))
				{
					this._MoBaiDic.Add(moBai.ID, moBai);
				}
			}
			return this._MoBaiDic;
		}
	}

	private MoBai MoBaiOf(NPCDialogMobaiPart.UIMoBaiType eType)
	{
		switch (eType)
		{
		case NPCDialogMobaiPart.UIMoBaiType.ShengYu:
			return this.MoBaiDic[1];
		case NPCDialogMobaiPart.UIMoBaiType.FuQi:
			return this.MoBaiDic[2];
		case NPCDialogMobaiPart.UIMoBaiType.ZhongShen:
			return this.MoBaiDic[3];
		case NPCDialogMobaiPart.UIMoBaiType.JunTuanDiGong:
		case NPCDialogMobaiPart.UIMoBaiType.JunTuanHuangMo:
			return this.MoBaiDic[4];
		case NPCDialogMobaiPart.UIMoBaiType.ZhanMengLianSai:
			return this.MoBaiDic[6];
		case NPCDialogMobaiPart.UIMoBaiType.ShiLiZhengBa:
			return this.MoBaiDic[7];
		default:
			return null;
		}
	}

	public void InitZhengBaTime(ZhengBaMiniStateData time)
	{
		this.miniData = time;
		if (!this.miniData.IsThisMonthInActivity || this.miniData.IsZhengBaOpened)
		{
		}
	}

	public NPCDialogMobaiPart.UIMoBaiType eMoBaiType;

	public GCheckBox chkModeGold;

	public GCheckBox chkModeDiamond;

	public GButton btnDetail;

	public GButton btnLiDaiChengZhu;

	public GButton chengzhanJoin;

	public GButton chengzhanRole;

	public GButton btnDuanWeiBang;

	public GButton btnClose;

	public GButton btnMobai;

	public UILabel lblForce;

	public UILabel lblPrompt;

	public UILabel lblMobaiNum;

	public UILabel lblPkName;

	public DPSelectedItemEventHandler DPSelectedItem;

	public Modal3DShow m_BossModal;

	public Modal3DShow LeftRoleModal;

	public Modal3DShow RightRoleModal;

	protected FamilyPart familyPart;

	public ShowNetImage Bak;

	public UISprite texNameBak;

	public UISprite luoLanKing;

	public UISprite PKKing;

	public UISprite zhongShenZhiWang;

	public UISprite ShengYuKing;

	public UISprite junTuanCaiJiChengHao;

	private LuoLanRolePart luoLanRolePart;

	private WolfSoulField_Rule WolfSoulField;

	[SerializeField]
	private ShowNetImage mChengHaoImage;

	public UILabel LoversManName;

	public UILabel LoversWifeName;

	public GameObject LoversObj;

	[SerializeField]
	private GButton mGoToGlorgHallBtn;

	public SpriteSL obj;

	private LiDaiChengZhuPart lidaichengzhu;

	private int pkKingRoleId = -1;

	private int mobaiUsed;

	private int mobaiLimit = 1;

	private int DbCoupleId;

	private GameObject oldRole;

	private RoleResLoader bossResLoader;

	private RoleResLoader rightResLoader;

	private RoleResLoader leftResLoader;

	private int ZhanMengLev;

	private Dictionary<int, MoBai> _MoBaiDic = new Dictionary<int, MoBai>();

	private ZhengBaMiniStateData miniData;

	public enum UIMoBaiType
	{
		PK,
		LuoLan,
		ShengYu,
		FuQi,
		ZhongShen,
		JunTuanDiGong,
		JunTuanHuangMo,
		ZhanMengLianSai,
		ShiLiZhengBa
	}
}
