using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class JingLingZhaoHuanPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.TimeLabel.text = Global.GetLang("后免费");
		this.btnOne.Text = Global.GetLang("抽取一次");
		this.btnTen.Text = Global.GetLang("召唤十次");
		this.TimeLabel.transform.localPosition = new Vector3(-310f, -223f, 0f);
		this.TimeText.pivot = 5;
		this.TimeText.transform.localPosition = new Vector3(-314f, -223f, 0f);
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.iconXingYunXing, "JingLingLieQu", 200, "xingyunzhixing");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.TextureOne, "JingLingLieQu", JingLingZhaoHuanPart.OneTimeNeedZuanShi, "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.TextureTen, "JingLingLieQu", JingLingZhaoHuanPart.TenTimeNeedZuanShi, "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
		GameInstance.Game.GetJieriFanbeiInfo();
		this.InitUI();
		this.iconDic.Add(0, this.icon1);
		this.iconDic.Add(1, this.icon2);
		this.iconDic.Add(2, this.icon3);
		this.iconDic.Add(3, this.icon4);
		this.iconDic.Add(4, this.icon5);
		this.iconDic.Add(5, this.icon6);
		this.iconDic.Add(6, this.icon7);
		this.iconDic.Add(7, this.icon8);
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		if (null != this.duiHuanBtn)
		{
			UIEventListener.Get(this.duiHuanBtn.gameObject).onClick = delegate(GameObject s)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 1
					});
				}
			};
		}
		if (null != this.changKuBtn)
		{
			UIEventListener.Get(this.changKuBtn.gameObject).onClick = delegate(GameObject s)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 2
					});
				}
			};
		}
		if (this.chongZhiBtn != null)
		{
			this.chongZhiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 3
					});
				}
			};
		}
		if (this.shuoMingBtn != null)
		{
			UIEventListener.Get(this.shuoMingBtn.gameObject).onClick = delegate(GameObject s)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 4
					});
				}
			};
		}
		this.btnOne.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (JingLingZhaoHuanPart.OneTimeNeedZuanShi > Global.GetRoleOwnNumByMoneyType(163) && this.OneTimeType == JingLingZhaoHuanPart.JingLingOneTimeType.Diamond && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("JingLingLieQu", JingLingZhaoHuanPart.OneTimeNeedZuanShi, true))
			{
				IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = JingLingZhaoHuanPart.OneTimeNeedZuanShi - Global.GetRoleOwnNumByMoneyType(163);
				string lang = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang(string.Format(lang, IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi))
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SendBoCaiDaiBi(IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi, 2);
					}
					return true;
				};
				return;
			}
			if (this.OneTimeType == JingLingZhaoHuanPart.JingLingOneTimeType.Diamond && Global.GetZuanShi(ZuanShiPartClass.JingLingZhaoHuan))
			{
				if (this.messageBoxWindow != null)
				{
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
				}
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "JingLingLieQu", JingLingZhaoHuanPart.OneTimeNeedZuanShi);
				this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), JingLingZhaoHuanPart.OneTimeNeedZuanShi, text)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
				{
					this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
				}
				this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
					if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
					{
						Global.SetZuanShi(ZuanShiPartClass.JingLingZhaoHuan, !this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
					}
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						Global.Data.IsDoingJingLingZhaoHuan = true;
						Super.ShowNetWaiting(null);
						GameInstance.Game.SpriteExecuteJingLingZhaoHuanCmd(1);
					}
					return true;
				};
				return;
			}
			Global.Data.IsDoingJingLingZhaoHuan = true;
			Super.ShowNetWaiting(null);
			GameInstance.Game.SpriteExecuteJingLingZhaoHuanCmd(1);
		};
		this.btnTen.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (JingLingZhaoHuanPart.TenTimeNeedZuanShi > Global.GetRoleOwnNumByMoneyType(163) && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("JingLingLieQu", JingLingZhaoHuanPart.TenTimeNeedZuanShi, true))
			{
				IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = JingLingZhaoHuanPart.TenTimeNeedZuanShi - Global.GetRoleOwnNumByMoneyType(163);
				string lang = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang(string.Format(lang, IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi))
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SendBoCaiDaiBi(IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi, 2);
					}
					return true;
				};
				return;
			}
			if (Global.GetZuanShi(ZuanShiPartClass.JingLingZhaoHuan))
			{
				if (this.messageBoxWindow != null)
				{
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
				}
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "JingLingLieQu", JingLingZhaoHuanPart.TenTimeNeedZuanShi);
				this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), JingLingZhaoHuanPart.TenTimeNeedZuanShi, text)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
				{
					this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
				}
				this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
					if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
					{
						Global.SetZuanShi(ZuanShiPartClass.JingLingZhaoHuan, !this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
					}
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						Global.Data.IsDoingJingLingZhaoHuan = true;
						Super.ShowNetWaiting(null);
						GameInstance.Game.SpriteExecuteJingLingZhaoHuanCmd(10);
					}
					return true;
				};
				return;
			}
			Global.Data.IsDoingJingLingZhaoHuan = true;
			Super.ShowNetWaiting(null);
			GameInstance.Game.SpriteExecuteJingLingZhaoHuanCmd(10);
		};
		this.ButtonTenLabel.text = Global.GetLang("召唤十次");
		base.StartCoroutine<bool>(this.InitGoodsGicon());
		GameInstance.Game.SpriteQueryJingLingZhaoHuanInfoCmd();
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("CallPet", ',');
		if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length == 2)
		{
			JingLingZhaoHuanPart.OneTimeNeedZuanShi = systemParamIntArrayByName[0];
			JingLingZhaoHuanPart.TenTimeNeedZuanShi = systemParamIntArrayByName[1];
		}
		SystemHelpMgr.OnAction(UIObjIDs.jingLingZhaoHuanPart, HelpStateEvents.Actived, -1);
	}

	public void RefreshGoodIcons()
	{
		foreach (GGoodIcon ggoodIcon in this.iconDic.Values)
		{
			ggoodIcon.ResetUI();
		}
		base.StartCoroutine<bool>(this.InitGoodsGicon());
	}

	private void InitUI()
	{
		this.zhuangshiText.text = "0";
		this.JifenText.text = "0";
		this.goodsCountText.text = "0";
		this.TimeLabel.text = string.Empty;
		this.TimeText.text = string.Empty;
		this.TextOne.text = JingLingZhaoHuanPart.OneTimeNeedZuanShi.ToString();
		this.TextTen.text = JingLingZhaoHuanPart.TenTimeNeedZuanShi.ToString();
		this.ButtonOneLabel.text = Global.GetLang("召唤一次");
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void RefreshByServerData(bool isFirst, long remainTime)
	{
		if (isFirst)
		{
			this.TimeLabel.text = string.Empty;
			this.TimeText.text = string.Empty;
			this.ButtonOneLabel.text = Global.GetLang("免费召唤");
			this.OneTimeType = JingLingZhaoHuanPart.JingLingOneTimeType.FirstChouQu;
			this.TextureOne.gameObject.SetActive(false);
			this.TextOne.text = Global.GetLang("免费");
		}
		else
		{
			this.RemainTime = remainTime;
			this.ButtonOneLabel.text = Global.GetLang("召唤一次");
			if (remainTime > 0L)
			{
				this.TextureOne.gameObject.SetActive(true);
				this.TimeLabel.text = Global.GetLang("后免费");
				this.TimeText.text = UIHelper.FormatSecs2(remainTime, "00:00:00");
				base.CancelInvoke("TickProc");
				base.InvokeRepeating("TickProc", 1f, 1f);
				int num = int.Parse(ConfigSystemParam.GetSystemParamByName("ZhaoHuan", true));
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(num);
				if (totalGoodsCountByID > 0)
				{
					this.OneTimeType = JingLingZhaoHuanPart.JingLingOneTimeType.Good;
					this.TextOne.text = Global.GetGoodsNameByID(num, false);
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
					if (goodsXmlNodeByID != null)
					{
						string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
						this.TextureOne.URL = StringUtil.substitute("NetImages/GameRes/{0}", new object[]
						{
							goodsImageURLFromIconCode
						});
					}
				}
				else
				{
					this.OneTimeType = JingLingZhaoHuanPart.JingLingOneTimeType.Diamond;
					this.TextOne.text = JingLingZhaoHuanPart.OneTimeNeedZuanShi.ToString();
				}
			}
			else
			{
				this.TimeLabel.text = string.Empty;
				this.TimeText.text = string.Empty;
				this.TextOne.text = Global.GetLang("免费");
				this.OneTimeType = JingLingZhaoHuanPart.JingLingOneTimeType.Free;
				this.TextureOne.gameObject.SetActive(false);
			}
		}
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.TextureOne, "JingLingLieQu", JingLingZhaoHuanPart.OneTimeNeedZuanShi, "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.TextureTen, "JingLingLieQu", JingLingZhaoHuanPart.TenTimeNeedZuanShi, "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
	}

	protected void TickProc()
	{
		this.RemainTime -= 1L;
		if (this.RemainTime >= 0L)
		{
			this.TimeText.text = UIHelper.FormatSecs2(this.RemainTime, "00:00:00");
		}
		else
		{
			base.CancelInvoke("TickProc");
			this.TimeLabel.text = string.Empty;
			this.TimeText.text = string.Empty;
			this.TextOne.text = Global.GetLang("免费");
			this.OneTimeType = JingLingZhaoHuanPart.JingLingOneTimeType.Free;
			this.TextureOne.gameObject.SetActive(false);
		}
	}

	public override void Destroy()
	{
		base.CancelInvoke("TickProc");
		base.Destroy();
	}

	public void InitPartData()
	{
		if (Global.Data.JingLingGoodsDataList == null || Global.Data.JingLingGoodsDataList.Count == 0)
		{
			GameInstance.Game.SpriteGetJingLingGoodsDataList();
		}
		else
		{
			this.SetGoodsCountText();
		}
	}

	public void GetNewData()
	{
		this.RefreshMoney();
	}

	private string GetGoodsString()
	{
		XElement xelement = null;
		LinkedList<string> linkedList = HuoDongCommonFlag.TopIconTreeActivityActivedLst();
		if (linkedList.Contains("JieRi"))
		{
			if (Global.isFanbei(101))
			{
				xelement = Global.GetGameResXml("Config/HuoDongCallPetType.xml");
			}
			if (Global.isFanbei(253))
			{
				xelement = Global.GetGameResXml("Config/TeQuanCallPetType.xml");
			}
		}
		if (xelement == null)
		{
			xelement = Global.GetGameResXml("Config/CallPetType.xml");
		}
		if (xelement == null)
		{
			return string.Empty;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement2 = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "MaxLevel");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "MinLevel");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "MaxZhuanSheng");
			string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "MinZhuanSheng");
			if (Global.Data.roleData.ChangeLifeCount >= Convert.ToInt32(xelementAttributeStr4) && Global.Data.roleData.Level >= Convert.ToInt32(xelementAttributeStr2) && Global.Data.roleData.ChangeLifeCount <= Convert.ToInt32(xelementAttributeStr3) && Global.Data.roleData.Level <= Convert.ToInt32(xelementAttributeStr))
			{
				string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement2, "ID");
				string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement2, "Name");
				return Global.GetXElementAttributeStr(xelement2, "Goods");
			}
		}
		return string.Empty;
	}

	public IEnumerator InitGoodsGicon()
	{
		yield return null;
		string goodsIds = this.GetGoodsString();
		string[] goodsArr = goodsIds.Split(new char[]
		{
			'|'
		});
		if (goodsArr.Length <= 0)
		{
			yield break;
		}
		for (int i = 0; i < goodsArr.Length; i++)
		{
			string[] fields = goodsArr[i].Split(new char[]
			{
				','
			});
			if (fields.Length > 0)
			{
				int goodsid = Convert.ToInt32(fields[0]);
				int forgeLevel = Convert.ToInt32(fields[2]);
				int zhuijiaLevel = Convert.ToInt32(fields[3]);
				int zhuoyueIndex = Convert.ToInt32(fields[5]);
				int lucky = Convert.ToInt32(fields[4]);
				if (this.iconDic.ContainsKey(i))
				{
					GGoodIcon icon = this.iconDic[i];
					GoodsData gd = Global.GetDummyGoodsDataMu(goodsid, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
					icon.Width = 78.0;
					icon.Height = 78.0;
					GoodVO goodVO = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
					string bitmapImageURL = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodVO), string.Empty);
					icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
					{
						bitmapImageURL
					}), false, 0);
					icon.TipType = 1;
					icon.ItemCode = gd.GoodsID;
					icon.ItemObject = gd;
					icon.BoxTypes = 0;
					icon.TextSize = 16;
					icon.TextShadowColor = 4278190080U;
					icon.Tag = gd.ExcellenceInfo;
					icon.BackSpriteName0 = "bagGrid4_bak";
					Super.InitGoodsGIcon(icon, gd, true, IconTextTypes.Qianghua);
					icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
						GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
						GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, goodData);
					};
					if (Convert.ToInt32(fields[1]) == -1 || string.Empty == fields[1])
					{
					}
				}
			}
		}
		yield break;
	}

	public void UpdateGoodsCountText(int count)
	{
		this.goodsCountText.Text = count.ToString();
	}

	public void SetGoodsCountText()
	{
		if (Global.Data.JingLingGoodsDataList != null)
		{
			this.goodsCountText.Text = Global.Data.JingLingGoodsDataList.Count.ToString();
		}
		else
		{
			this.goodsCountText.Text = "0";
		}
	}

	public void OnZhaoHuanCompleted(int qiFuType, string goodsStr, int remainTime)
	{
		this.RefreshMoney();
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.TextureOne, "JingLingLieQu", JingLingZhaoHuanPart.OneTimeNeedZuanShi, "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.TextureTen, "JingLingLieQu", JingLingZhaoHuanPart.TenTimeNeedZuanShi, "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 2
			});
		}
		if (this.jingLingZhaoHuanGiftsPart == null)
		{
			this.OpenZhaoHuanGiftsPart();
		}
		if (qiFuType == 1)
		{
			this.jingLingZhaoHuanGiftsPart.RefreshUIUnit(true);
		}
		else
		{
			this.jingLingZhaoHuanGiftsPart.RefreshUIUnit(false);
		}
		this.jingLingZhaoHuanGiftsPart.RefreshAddGoodIcons(goodsStr);
		this.SetGoodsCountText();
	}

	public void RefreshMoney()
	{
		this.zhuangshiText.Text = StringUtil.substitute("{0}", new object[]
		{
			(!IConfigbase<ConfigXingYunXingShiYong>.Instance.XingYunXingKaiGuan("JingLingLieQu")) ? Global.GetRoleOwnNumByMoneyType(40) : Global.GetRoleOwnNumByMoneyType(163)
		});
		this.JifenText.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.PetJiFen).ToString();
	}

	private void OpenZhaoHuanGiftsPart()
	{
		this.JingLingZhaoHuanGiftsWindow = U3DUtils.NEW<GChildWindow>();
		this.JingLingZhaoHuanGiftsWindow.ModalType = ChildWindowModalType.Translucent;
		Super.InitChildWindow(this.JingLingZhaoHuanGiftsWindow, Global.GetLang(string.Empty));
		if (null == this.jingLingZhaoHuanGiftsPart)
		{
			this.jingLingZhaoHuanGiftsPart = U3DUtils.NEW<JingLingZhaoHuanGetGiftsPart>();
			this.jingLingZhaoHuanGiftsPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.CloseJingLingZhaoHuanGiftsPart();
					SystemHelpMgr.OnAction(UIObjIDs.jingLingZhaoHuanDetailPart, HelpStateEvents.Clicked, -1);
				}
			};
			this.JingLingZhaoHuanGiftsWindow.SetContent(this.JingLingZhaoHuanGiftsWindow.BodyPresenter, this.jingLingZhaoHuanGiftsPart, 0.0, 0.0, true);
			Super.GData.PlayZoneRoot.Children.Add(this.JingLingZhaoHuanGiftsWindow);
		}
		this.ObjContainer.SetActive(false);
	}

	public void ChangeMap()
	{
		if (null != this.JingLingZhaoHuanGiftsWindow)
		{
			this.CloseJingLingZhaoHuanGiftsPart();
		}
	}

	private void CloseJingLingZhaoHuanGiftsPart()
	{
		if (null != this.JingLingZhaoHuanGiftsWindow)
		{
			GameInstance.Game.SpriteQueryJingLingZhaoHuanInfoCmd();
			this.SetGoodsCountText();
			Super.GData.PlayZoneRoot.Children.Remove(this.JingLingZhaoHuanGiftsWindow, true);
			Object.Destroy(this.jingLingZhaoHuanGiftsPart.gameObject);
			this.jingLingZhaoHuanGiftsPart = null;
			this.JingLingZhaoHuanGiftsWindow = null;
		}
		this.ObjContainer.SetActive(true);
	}

	internal void ShowHelpAnim(int p, int param)
	{
		if (p == 604)
		{
			SystemHelpPart.SetMask(this.btnOne, default(Vector4));
		}
		else if (p == 605)
		{
			if (null != this.jingLingZhaoHuanGiftsPart)
			{
				SystemHelpPart.SetMask(this.jingLingZhaoHuanGiftsPart.btnOK, default(Vector4));
			}
		}
		else if (p == 606)
		{
			SystemHelpPart.SetMask(this.changKuBtn, default(Vector4));
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private JingLingZhaoHuanPart.JingLingOneTimeType OneTimeType = JingLingZhaoHuanPart.JingLingOneTimeType.Diamond;

	public GGoodIcon icon1;

	public GGoodIcon icon2;

	public GGoodIcon icon3;

	public GGoodIcon icon4;

	public GGoodIcon icon5;

	public GGoodIcon icon6;

	public GGoodIcon icon7;

	public GGoodIcon icon8;

	public GButton btnOne;

	public GButton btnTen;

	public GButton chongZhiBtn;

	public GButton closeBtn;

	public UIButton changKuBtn;

	public UIButton duiHuanBtn;

	public UIButton shuoMingBtn;

	public TextBlock zhuangshiText;

	public TextBlock JifenText;

	public TextBlock goodsCountText;

	public UILabel TimeLabel;

	public UILabel TimeText;

	public UILabel TextOne;

	public UILabel TextTen;

	public UILabel ButtonOneLabel;

	public UILabel ButtonTenLabel;

	public GameObject BakContainer;

	public GameObject ObjContainer;

	public ShowNetImage TextureOne;

	public ShowNetImage TextureTen;

	private GChildWindow messageBoxWindow;

	private long RemainTime;

	public Dictionary<int, GGoodIcon> iconDic = new Dictionary<int, GGoodIcon>();

	public static int OneTimeNeedZuanShi = 200;

	public static int TenTimeNeedZuanShi = 1800;

	public UISprite iconXingYunXing;

	private GChildWindow JingLingZhaoHuanGiftsWindow;

	public JingLingZhaoHuanGetGiftsPart jingLingZhaoHuanGiftsPart;

	public enum JingLingOneTimeType
	{
		FirstChouQu = 1,
		Free,
		Good,
		Diamond
	}
}
