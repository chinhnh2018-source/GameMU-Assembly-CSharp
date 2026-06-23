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

public class MarryLoveTockenPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.PkloverText.text = Global.GetLang("情侣竞技场");
		this.ChatBtn.Text = Global.GetLang("私聊");
		this.jieLevText.text = string.Empty;
		this.shuomingText.text = Global.GetLang("可获得伴侣信物的30%属性加成");
		this.Name1.text = string.Empty;
		this.Name2.text = string.Empty;
		this.Name1.textColor = Global.ParseStringColorToUint("#3681f3");
		this.Name2.textColor = Global.ParseStringColorToUint("#dd36bc");
		this.BuyRingTxt.text = Global.GetLang("购买钻戒");
		this.HunLiTxt.text = Global.GetLang("婚礼宴席");
		this.FuBenTxt.text = Global.GetLang("情侣副本");
		this.DivorceTxt.text = Global.GetLang("离婚");
		this.Title1.text = Global.GetLang("详细属性");
		this.Title2.text = Global.GetLang("升星消耗");
		this.ClickChange.text = Global.GetLang("点击更换");
		this.ReduceGold.text = Global.GetLang("消耗钻石：");
		this.BtnTSdengji.Text = Global.GetLang("献花");
		this.LookOtherRoleBtn.Text = Global.GetLang("查看装备");
		try
		{
			this.ZhuFuLovers.GetComponentInChildren<UILabel>().text = Global.GetLang("情侣祝福榜");
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"越南用，可能预制报空"
			});
		}
		this.BtnTSdengji.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Super.MessageBoxIsHint[6] == 0)
			{
				DPSelectedItemEventHandler handler = delegate(object sender, DPSelectedItemEventArgs args)
				{
					if (args.ID == 0)
					{
						if (this.CurrStep == 10 && this.CurrStar == 10)
						{
							Super.HintMainText(Global.GetLang("已达到最高等级，无法继续献花！"), 10, 3);
							return;
						}
						if (this.CurrHaveRoseNum <= 0)
						{
							Super.HintMainText(Global.GetLang("背包中没有选中的鲜花"), 10, 3);
							return;
						}
						if (this.CostGold > Global.GetRoleOwnNumByMoneyType(40))
						{
							Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, new DPSelectedItemEventHandler(this.CloseThisWindow), string.Empty, string.Empty);
							return;
						}
						this.itsType = 0;
						Super.ShowNetWaiting(string.Empty);
						GameInstance.Game.SpriteXianHua(this.RoseId);
					}
					else
					{
						Super.MessageBoxIsHint[6] = 0;
					}
				};
				if (!this.ReduceGoldNum.text.Equals("0"))
				{
					Super.ZuanShiShowMessageBox(Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("本次操作需要花费钻石       {0} ,  确认要执行该操作吗？"), this.ReduceGoldNum.text)
					}), 2, handler, MessBoxIsHintTypes.JieHunHuaFeiZuanShiNeedHint, 0f, string.Empty, 0);
				}
				else
				{
					if (this.CurrStep == 10 && this.CurrStar == 10)
					{
						Super.HintMainText(Global.GetLang("已达到最高等级，无法继续献花！"), 10, 3);
						return;
					}
					if (this.CurrHaveRoseNum <= 0)
					{
						Super.HintMainText(Global.GetLang("背包中没有选中的鲜花"), 10, 3);
						return;
					}
					if (this.CostGold > Global.GetRoleOwnNumByMoneyType(40))
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, new DPSelectedItemEventHandler(this.CloseThisWindow), string.Empty, string.Empty);
						return;
					}
					this.itsType = 0;
					Super.ShowNetWaiting(string.Empty);
					GameInstance.Game.SpriteXianHua(this.RoseId);
				}
			}
			else
			{
				if (this.CurrStep == 10 && this.CurrStar == 10)
				{
					Super.HintMainText(Global.GetLang("已达到最高等级，无法继续献花！"), 10, 3);
					return;
				}
				if (this.CurrHaveRoseNum <= 0)
				{
					Super.HintMainText(Global.GetLang("背包中没有选中的鲜花"), 10, 3);
					return;
				}
				if (this.CostGold > Global.GetRoleOwnNumByMoneyType(40))
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, new DPSelectedItemEventHandler(this.CloseThisWindow), string.Empty, string.Empty);
					return;
				}
				this.itsType = 0;
				Super.ShowNetWaiting(string.Empty);
				GameInstance.Game.SpriteXianHua(this.RoseId);
			}
		};
		this.ReturnBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.CloseMarryTockenWindow();
		};
		this.LookOtherRoleBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SpriteGetOtherAttrib(Global.Data.MarryData.nSpouseID);
		};
		this.ChatBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.callback != null)
			{
				this.callback(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					Title = Global.Data.MarryOtherData.roleName
				});
			}
		};
		UIEventListener.Get(this.BuyRingBtn.gameObject).onClick = delegate(GameObject s)
		{
			if (this.callback != null)
			{
				this.callback(this, new DPSelectedItemEventArgs
				{
					ID = 2
				});
			}
		};
		UIEventListener.Get(this.HunYanBtn.gameObject).onClick = delegate(GameObject s)
		{
			Super.ShowNetWaiting(string.Empty);
			GameInstance.Game.GetHunYanListInfo(true);
		};
		UIEventListener.Get(this.FuBenBtn.gameObject).onClick = delegate(GameObject s)
		{
			SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
			if (mapSceneUIClass == SceneUIClasses.LoveFuBen)
			{
				Super.HintMainText(Global.GetLang("情侣副本中不能打开此功能"), 10, 3);
				return;
			}
			if (Global.Data.roleData.OccupationList != null && Global.Data.roleData.Occupation != Global.Data.roleData.OccupationList[0])
			{
				string lang = Global.GetLang("此功能必须使用主职业");
				Super.HintMainText(lang, 10, 3);
				return;
			}
			if (this.callback != null)
			{
				this.callback(this, new DPSelectedItemEventArgs
				{
					ID = 4
				});
			}
		};
		UIEventListener.Get(this.DivorceBtn.gameObject).onClick = delegate(GameObject s)
		{
			SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
			if (mapSceneUIClass == SceneUIClasses.LoveFuBen)
			{
				Super.HintMainText(Global.GetLang("情侣副本中不能打开此功能"), 10, 3);
				return;
			}
			PlayZone.GlobalPlayZone.CloseMarryTockenWindow();
			PlayZone.GlobalPlayZone.OpenRequestDivorcePart();
		};
		UIEventListener.Get(this.TouXiang2.gameObject).onClick = delegate(GameObject s)
		{
			if (!this.IsMan)
			{
				return;
			}
			if (!this.OpenMenu)
			{
				this.PosTween.Play(true);
				this.TouXiang2.GetComponent<BoxCollider>().size = new Vector3(87f, 87f, 1f);
			}
			else
			{
				this.PosTween.Play(false);
				this.TouXiang2.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, 1f);
			}
			this.OpenMenu = !this.OpenMenu;
		};
		UIEventListener.Get(this.TouXiang1.gameObject).onClick = delegate(GameObject s)
		{
			if (this.IsMan)
			{
				return;
			}
			if (!this.OpenMenu)
			{
				this.PosTween.Play(true);
				this.TouXiang1.GetComponent<BoxCollider>().size = new Vector3(87f, 87f, 1f);
			}
			else
			{
				this.PosTween.Play(false);
				this.TouXiang1.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, 1f);
			}
			this.OpenMenu = !this.OpenMenu;
		};
		this.HuaIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenSelectRosePart(new DPSelectedItemEventHandler(this.SetSelectRose), 1);
		};
		UIEventListener.Get(this.PKLovers.gameObject).onClick = delegate(GameObject s)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.PKLovers, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.PKLovers, trigger, param, param2, true);
				return;
			}
			if (Global.Data.roleData.OccupationList != null && Global.Data.roleData.Occupation != Global.Data.roleData.OccupationList[0])
			{
				string lang = Global.GetLang("此功能必须使用主职业");
				Super.HintMainText(lang, 10, 3);
				return;
			}
			PlayZone.GlobalPlayZone.OpenPKLoversWindow();
		};
		UIEventListener.Get(this.ZhuFuLovers.gameObject).onClick = delegate(GameObject s)
		{
			PlayZone.GlobalPlayZone.OpenLoversWishPartWindow();
		};
		this.InitPropsDic();
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		if (this.callback != null)
		{
			this.callback(this, new DPSelectedItemEventArgs
			{
				ID = -1
			});
		}
	}

	public void NotifyShowOtherRoleInfo(RoleDataEx otherRoleData)
	{
		PlayZone.GlobalPlayZone.CloseMarryTockenWindow();
	}

	public void NotifyShowHunYanWindow(MarryPartyData partyData)
	{
		if (partyData != null)
		{
			PlayZone.GlobalPlayZone.OpenHunYanPart(1, 1, partyData, new DPSelectedItemEventHandler(this.CloseCallBack));
		}
	}

	private void SetHuaIcon(int roseId)
	{
		this.SetGoodsData(Global.GetFakeEquipGoodsData(roseId, 0, 0));
		XElement gameResXml = Global.GetGameResXml("Config/GiveRose.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Rose");
		for (int i = 0; i < xelementList.Count; i++)
		{
			if (Global.GetXElementAttributeInt(xelementList[i], "GoodsID") == roseId)
			{
				this.AddExp = Global.GetXElementAttributeInt(xelementList[i], "GoodWill");
				break;
			}
		}
	}

	public void SetGoodsData(GoodsData data)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		int categoriy = goodsXmlNodeByID.Categoriy;
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		bool canUse = Global.CanUseGoods(data.GoodsID, false, true);
		this.HuaIcon.Width = 78.0;
		this.HuaIcon.Height = 78.0;
		this.HuaIcon.BackSpriteName0 = "bagGrid4_bak";
		NGUITools.SetActive(this.HuaIcon.BackgroundSprite0, true);
		this.HuaIcon.BackgroundSprite0.MakePixelPerfect();
		this.HuaIcon.ItemCategory = categoriy;
		this.HuaIcon.ItemCode = data.GoodsID;
		this.HuaIcon.ItemObject = data;
		this.HuaIcon.isAutoSize = false;
		this.HuaIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		this.HuaIcon.TeXiao.gameObject.SetActive(false);
		this.HuaIcon.BackgroundSprite1.gameObject.SetActive(false);
		Super.InitGoodsGIcon(this.HuaIcon, data, canUse, IconTextTypes.Qianghua);
		this.HuaIcon.transform.localScale = new Vector3(0.72f, 0.72f, 0.72f);
		this.RefreshHuaNum();
	}

	private void RefreshHuaNum()
	{
		this.CurrHaveRoseNum = Global.GetTotalGoodsCountByID(this.RoseId);
		this.HuaIcon.SecondText.text = this.CurrHaveRoseNum + "/1";
	}

	public void InitPartData()
	{
		this.MarryXML = Global.GetGameResXml("Config/GoodWill.xml");
		if (this.MarryXML == null)
		{
			return;
		}
		this.XianHuaCost = ConfigSystemParam.GetSystemParamIntArrayByName("XianHuaCost", ',');
		XElement gameResXml = Global.GetGameResXml("Config/GiveRose.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Rose");
		for (int i = 0; i < xelementList.Count; i++)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "GoodsID");
			if (i == 0 || Global.GetTotalGoodsCountByID(xelementAttributeInt) > 0)
			{
				this.RoseId = xelementAttributeInt;
			}
		}
		this.SetHuaIcon(this.RoseId);
		this.Refresh(false);
	}

	public void Refresh(bool BaoJi = false)
	{
		this.IsBaoJi = BaoJi;
		this.RefreshData();
		this.RefreshUI();
	}

	public void CloseCallBack(object target, DPSelectedItemEventArgs args)
	{
		this.Refresh(false);
	}

	private void RefreshData()
	{
		if (Global.Data.MarryData == null)
		{
			return;
		}
		this.CurrExp = Global.Data.MarryData.nGoodwillexp;
		this.CurrStar = (int)Global.Data.MarryData.byGoodwillstar;
		this.CurrStep = (int)Global.Data.MarryData.byGoodwilllevel;
		this.XianHuaCout = Global.Data.MarryData.nGivenrose;
		XElement marryXmlNode = this.GetMarryXmlNode(this.CurrStep.ToString(), this.CurrStar.ToString());
		this.MaxExp = Global.GetXElementAttributeInt(marryXmlNode, "NeedGoodWill");
	}

	private void RefreshUI()
	{
		this.ShowRing3DModal();
		if (this.itsType != -1)
		{
			if (this.hintPart == null)
			{
				Vector3 localPosition;
				localPosition..ctor(-220f, -78f, -3f);
				this.hintPart = U3DUtils.NEW<GetGoodsHintPart>();
				this.hintPart.transform.localPosition = localPosition;
				base.Children.Add(this.hintPart);
			}
			if (this.IsBaoJi && (this.CurrExp - this.PreExp != this.AddExp || this.CurrStar != this.PreStar))
			{
				int num;
				if (this.CurrExp <= this.PreExp || this.CurrStar != this.PreStar)
				{
					num = this.PreMaxExp - this.PreExp + this.CurrExp;
				}
				else
				{
					num = this.CurrExp - this.PreExp;
				}
				this.hintPart.AddTextItem(1, string.Format(Global.GetLang("{{00ff00}}爆增 +{0}{{-}}"), num));
			}
			bool flag = false;
			if (this.PreStar < this.CurrStar)
			{
				flag = true;
				this.statAnim.gameObject.SetActive(false);
				this.statAnim.gameObject.SetActive(true);
				this.statAnim.transform.localPosition = new Vector3((float)(-125 + this.tsLevProgBar.Level * 32), -106f, -1f);
			}
			if (this.PreStep < this.CurrStep)
			{
				this.JinJieEff.SetActive(false);
				this.JinJieEff.SetActive(true);
			}
			if (this.PreExp < this.CurrExp)
			{
				if (flag)
				{
					this.ExpFullTex.SetActive(false);
					this.ExpFullTex.SetActive(true);
				}
				else
				{
					this.ExpEmptyTex.SetActive(false);
					this.ExpEmptyTex.SetActive(true);
				}
			}
		}
		this.PreExp = this.CurrExp;
		this.PreStar = this.CurrStar;
		this.PreStep = this.CurrStep;
		this.PreMaxExp = this.MaxExp;
		this.jieLevText.text = this.CurrStep.ToString();
		this.tsLevProgBar.ItemWidth = 32f;
		this.tsLevProgBar.MaxLevel = 10;
		this.tsLevProgBar.Level = this.CurrStar;
		this.tsJingduProgressBar.ProgessText = this.CurrExp + "/" + this.MaxExp;
		this.tsJingduProgressBar.Percent = (double)this.CurrExp * 1.0 / (double)this.MaxExp;
		if (this.XianHuaCout < this.XianHuaCost.Length)
		{
			this.XianHuaCout = ((this.XianHuaCout <= 0) ? 0 : this.XianHuaCout);
			int costGold = this.XianHuaCost[this.XianHuaCout];
			this.CostGold = costGold;
		}
		else
		{
			int costGold2 = this.XianHuaCost[this.XianHuaCost.Length - 1];
			this.CostGold = costGold2;
		}
		this.ReduceGoldNum.text = this.CostGold.ToString();
		this.IsMan = false;
		if (Global.Data.roleData.Occupation != 2)
		{
			this.IsMan = true;
		}
		if (this.IsMan)
		{
			this.TouXiang1.URL = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(Global.Data.roleData.OccupationList[0]),
				Global.Data.roleData.RoleSex
			});
			this.TouXiang2.URL = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(Global.Data.MarryOtherData.occupationId),
				1 - Global.Data.roleData.RoleSex
			});
			this.Name1.text = Global.Data.roleData.RoleName;
			this.Name2.text = Global.Data.MarryOtherData.roleName;
		}
		else
		{
			this.TouXiang1.URL = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(Global.Data.MarryOtherData.occupationId),
				1 - Global.Data.roleData.RoleSex
			});
			this.TouXiang2.URL = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(Global.Data.roleData.OccupationList[0]),
				Global.Data.roleData.RoleSex
			});
			this.Name2.text = Global.Data.roleData.RoleName;
			this.Name1.text = Global.Data.MarryOtherData.roleName;
		}
		this.RefreshHuaNum();
		this.InitPropsVal();
	}

	private void PlayStart(Animation anim, ActiveAnimation.OnFinished onFinished)
	{
		ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, 1);
		if (activeAnimation == null)
		{
			return;
		}
		activeAnimation.onFinished = onFinished;
	}

	private void PlayFinished(ActiveAnimation anim)
	{
		anim.gameObject.SetActive(false);
	}

	private void SetSelectRose(object sender, DPSelectedItemEventArgs args)
	{
		if (args != null)
		{
			this.RoseId = args.ID;
			this.SetHuaIcon(this.RoseId);
			MUDebug.LogWarning<string>(new string[]
			{
				"-----------:" + this.RoseId
			});
		}
	}

	public override void Destroy()
	{
		if (this.wingsResLoader != null)
		{
			this.wingsResLoader.Stop();
			this.wingsResLoader = null;
		}
		base.Destroy();
	}

	private void ShowRing3DModal()
	{
		if ((this.CurrRingId != 0 && Global.Data.MarryData != null && Global.Data.MarryData.nRingID == this.CurrRingId) || (Global.Data.MarryData == null && this.Show3DModal != null))
		{
			return;
		}
		if (this.Show3DModal != null)
		{
			Object.Destroy(this.Show3DModal.gameObject);
		}
		XElement gameResXml = Global.GetGameResXml("Config/WeddingRing.xml");
		if (gameResXml == null)
		{
			MUDebug.LogWarning<string>(new string[]
			{
				"Card XML IS NULL"
			});
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Ring");
		int num = 0;
		if (Global.Data.MarryData != null)
		{
			num = Global.Data.MarryData.nRingID;
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "GoodsID");
				if (xelementAttributeInt == num)
				{
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "RingIntro");
					this.RingName.spriteName = xelementAttributeStr;
					this.RingName.MakePixelPerfect();
					this.RingName.transform.localPosition = new Vector3(this.RingName.transform.localPosition.x, this.RingName.transform.localPosition.y, -0.7f);
					break;
				}
			}
		}
		else
		{
			for (int j = 0; j < xelementList.Count; j++)
			{
				XElement xelement = xelementList[j];
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "ID");
				if (xelementAttributeInt2 == 1)
				{
					num = Global.GetXElementAttributeInt(xelement, "GoodsID");
					string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "RingIntro");
					this.RingName.spriteName = xelementAttributeStr2;
					this.RingName.MakePixelPerfect();
					this.RingName.transform.localPosition = new Vector3(this.RingName.transform.localPosition.x, this.RingName.transform.localPosition.y, -0.7f);
					break;
				}
			}
		}
		this.CurrRingId = num;
		this.Show3DModal = U3DUtils.NEW<Modal3DShow>();
		U3DUtils.AddChild(base.gameObject, this.Show3DModal.gameObject, false);
		Transform transform = this.Show3DModal.transform;
		transform.localPosition = new Vector3(27f, -8f, -0.8f);
		transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		UIHelper.SetModalPosZ(this.Show3DModal.transform);
		if (this.wingsResLoader != null)
		{
			this.wingsResLoader.Stop();
		}
		this.wingsResLoader = UIHelper.LoadGoodsRes(this.Show3DModal, num, 1f, 0.005f, 0, "UIModel", false);
	}

	private void InitPropsDic()
	{
		for (int i = 0; i < this.pos.Length; i++)
		{
			this.PropsDic.Add(this.pos[i], string.Empty);
			this.AddPropsDic.Add(this.pos[i], string.Empty);
		}
	}

	private void CloseThisWindow(object sender, DPSelectedItemEventArgs args)
	{
		PlayZone.GlobalPlayZone.CloseMarryTockenWindow();
	}

	private void InitPropsVal()
	{
		this.SetAttribList();
		this.wgongText.Text = string.Format("{{c39550}}" + this.names[0] + "{{-}} {0}", this.PropsDic[this.pos[0]]);
		this.mgongText.Text = string.Format("{{c39550}}" + this.names[1] + "{{-}} {0}", this.PropsDic[this.pos[1]]);
		this.wfangText.Text = string.Format("{{c39550}}" + this.names[2] + "{{-}} {0}", this.PropsDic[this.pos[2]]);
		this.mfangText.Text = string.Format("{{c39550}}" + this.names[3] + "{{-}} {0}", this.PropsDic[this.pos[3]]);
		this.hitvText.Text = string.Format("{{c39550}}" + this.names[4] + "{{-}} {0}", this.PropsDic[this.pos[4]]);
		this.dodgeText.Text = string.Format("{{c39550}}" + this.names[5] + "{{-}} {0}", this.PropsDic[this.pos[5]]);
		this.shengmingText.Text = string.Format("{{c39550}}" + this.names[6] + "{{-}} {0}", this.PropsDic[this.pos[6]]);
		if (this.CurrStep != 10 || this.CurrStar != 10)
		{
			this.duibiPanel.SetActive(true);
			this.SetDuiBiInfo();
		}
		else
		{
			this.duibiPanel.SetActive(false);
		}
	}

	private void SetDuiBiInfo()
	{
		this.bwgongText.Text = this.AddPropsDic[this.pos[0]];
		this.bmgongText.Text = this.AddPropsDic[this.pos[1]];
		this.bwfangText.Text = this.AddPropsDic[this.pos[2]];
		this.bmfangText.Text = this.AddPropsDic[this.pos[3]];
		this.bhitvText.Text = this.AddPropsDic[this.pos[4]];
		this.bdodgeText.Text = this.AddPropsDic[this.pos[5]];
		this.bshengmingText.Text = this.AddPropsDic[this.pos[6]];
	}

	private void SetAttribList()
	{
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(Global.Data.MarryData.nRingID);
		for (int i = 0; i < goodsEquipPropsDoubleList.Length; i++)
		{
			if (goodsEquipPropsDoubleList[i] > 0.0)
			{
				string text = ExtPropIndexes.ExtPropIndexChineseNames[i];
				double num = Global.SafeConvertToDouble(ConfigSystemParam.GetSystemParamByName("GoodWillXiShu", true));
				double num2 = goodsEquipPropsDoubleList[i] * ((double)(1 + (this.CurrStep - 1) * 2) + (double)this.CurrStar * num);
				double num3 = goodsEquipPropsDoubleList[i] * ((double)(1 + ((this.CurrStar != 10) ? (this.CurrStep - 1) : this.CurrStep) * 2) + (double)((this.CurrStar != 10) ? (this.CurrStar + 1) : 1) * num) - num2;
				string text2 = num2.ToString();
				string text3 = ((int)Math.Ceiling(num3)).ToString();
				if (ExtPropIndexes.ExtPropIndexPercents[i] > 0)
				{
					text2 = string.Format("+{0}%", (int)(num2 * 100.0));
					text3 = string.Format("+{0}%", (int)(num3 * 100.0));
				}
				if (this.PropsDic.ContainsKey(i))
				{
					this.PropsDic[i] = text2;
				}
				if (this.AddPropsDic.ContainsKey(i))
				{
					this.AddPropsDic[i] = text3;
				}
			}
		}
	}

	private XElement GetMarryXmlNode(string type, string starid)
	{
		XElement gameResXml = Global.GetGameResXml("Config/GoodWill.xml");
		if (gameResXml == null)
		{
			return null;
		}
		int num = Global.SafeConvertToInt32(starid);
		int num2 = Global.SafeConvertToInt32(type);
		if (num2 != 10 || num != 10)
		{
			if (num == 10)
			{
				num = 1;
				num2++;
			}
			else
			{
				num++;
			}
		}
		XElement xelement = Global.GetXElement(gameResXml, "GoodWill", "Type", num2.ToString());
		if (xelement == null)
		{
			return null;
		}
		xelement = Global.GetXElement(xelement, "GoodWill", "ID", num.ToString());
		if (xelement == null)
		{
			return null;
		}
		return xelement;
	}

	public void JingJieChengGong()
	{
		Transform deco = U3DUtils.NEW<Transform>("Wing_JinJieChengGong");
		if (null != deco)
		{
			U3DUtils.AddChild(base.gameObject, deco.gameObject, false);
			deco.transform.localPosition = Vector3.back * 2f;
			UIHelper.DelayInvoke(1.5f, delegate(object s, EventArgs e)
			{
				Object.Destroy(deco.gameObject);
			});
		}
	}

	private void InitChildWindow(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow(childWindow, title);
	}

	public UILabel PkloverText;

	public TextBlock jieLevText;

	public TextBlock shuomingText;

	public TextBlock Name1;

	public TextBlock Name2;

	public TextBlock Title1;

	public TextBlock Title2;

	public TextBlock ClickChange;

	public TextBlock ReduceGold;

	public TextBlock ReduceGoldNum;

	public UILabel BuyRingTxt;

	public UILabel HunLiTxt;

	public UILabel FuBenTxt;

	public UILabel DivorceTxt;

	public GButton BtnTSdengji;

	public UIButton BuyRingBtn;

	public UIButton HunYanBtn;

	public UIButton FuBenBtn;

	public UIButton DivorceBtn;

	public GButton ReturnBtn;

	public GButton LookOtherRoleBtn;

	public GButton ChatBtn;

	public UIButton PKLovers;

	public UIButton ZhuFuLovers;

	public TweenPosition PosTween;

	public TextBlock wgongText;

	public TextBlock mgongText;

	public TextBlock wfangText;

	public TextBlock mfangText;

	public TextBlock hitvText;

	public TextBlock dodgeText;

	public TextBlock shengmingText;

	public TextBlock xishouText;

	public TextBlock jiachengText;

	public GameObject duibiPanel;

	public TextBlock bwgongText;

	public TextBlock bmgongText;

	public TextBlock bwfangText;

	public TextBlock bmfangText;

	public TextBlock bhitvText;

	public TextBlock bdodgeText;

	public TextBlock bshengmingText;

	public TextBlock bxishouText;

	public TextBlock bjiachengText;

	public ShowNetImage TouXiang1;

	public ShowNetImage TouXiang2;

	public GGoodIcon HuaIcon;

	public GImgProgressBar tsLevProgBar;

	public GImgProgressBar tsJingduProgressBar;

	public TextBlock tsjinduText;

	public Animator statAnim;

	public GameObject JinJieEff;

	public GameObject ExpEmptyTex;

	public GameObject ExpFullTex;

	private GetGoodsHintPart hintPart;

	private int itsType = -1;

	private Modal3DShow Show3DModal;

	public UISprite RingName;

	private XElement MarryXML;

	private int CurrExp;

	private int CurrStar;

	private int CurrStep;

	private int MaxExp;

	private int XianHuaCout;

	private int CostGold;

	private int PreExp;

	private int PreMaxExp;

	private int PreStar;

	private int PreStep;

	private int AddExp;

	private int CurrHaveRoseNum;

	private int CurrRingId;

	public DPSelectedItemEventHandler callback;

	private int[] XianHuaCost;

	private bool OpenMenu;

	private bool IsMan;

	private string[] names = new string[]
	{
		Global.GetLang("物理攻击"),
		Global.GetLang("魔法攻击"),
		Global.GetLang("物理防御"),
		Global.GetLang("魔法防御"),
		Global.GetLang("命        中"),
		Global.GetLang("闪        避"),
		Global.GetLang("生命上限")
	};

	private int[] pos = new int[]
	{
		7,
		9,
		3,
		5,
		18,
		19,
		13
	};

	private Dictionary<int, string> PropsDic = new Dictionary<int, string>();

	private Dictionary<int, string> AddPropsDic = new Dictionary<int, string>();

	private bool IsBaoJi;

	private int RoseId;

	private WingsResLoader wingsResLoader;
}
