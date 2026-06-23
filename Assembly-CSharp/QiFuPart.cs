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

public class QiFuPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnOne.Text = Global.GetLang("抽取一次");
		this.btnTen.Text = Global.GetLang("抽取十次");
		this.TimeText.pivot = 5;
		this.TimeLabel.transform.localPosition = new Vector3(-295f, -223f, 0f);
		this.TimeText.transform.localPosition = new Vector3(-300f, -223f, 0f);
		this.m_TextMiaoshu.lineWidth = 240;
	}

	protected override void InitializeComponent()
	{
		GameInstance.Game.GetJieriFanbeiInfo();
		this.InitTextInPrefabs();
		this.InitUI();
		this.iconDic.Add(0, this.icon1);
		this.iconDic.Add(1, this.icon2);
		this.iconDic.Add(2, this.icon3);
		this.iconDic.Add(3, this.icon4);
		this.iconDic.Add(4, this.icon5);
		this.iconDic.Add(5, this.icon6);
		this.iconDic.Add(6, this.icon7);
		this.iconDic.Add(7, this.icon8);
		this.iconDic.Add(8, this.icon9);
		this.iconDic.Add(9, this.icon10);
		this.iconDic.Add(10, this.icon11);
		this.iconDic.Add(11, this.icon12);
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
			SystemHelpMgr.OnAction(UIObjIDs.QiFuPart, HelpStateEvents.Inactived, 1);
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
			if (200 > Global.GetRoleOwnNumByMoneyType(163) && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("QiFuXiTong", 200, true) && this.OneTimeType == QiFuPart.QiFuOneTimeType.Diamond)
			{
				IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = 200 - Global.GetRoleOwnNumByMoneyType(163);
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
			Global.Data.IsDoingZaJinDan = true;
			if (this.OneTimeType != QiFuPart.QiFuOneTimeType.Good && this.OneTimeType != QiFuPart.QiFuOneTimeType.Diamond)
			{
				Global.SendEvent("1500", Global.GetLang("免费祈福次数"));
			}
			else
			{
				if (this.OneTimeType == QiFuPart.QiFuOneTimeType.Diamond && Global.GetZuanShi(ZuanShiPartClass.QiFu))
				{
					string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "QiFuXiTong", 200);
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), 200, text)
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
					{
						messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
					}
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
						{
							Global.SetZuanShi(ZuanShiPartClass.QiFu, !messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
						}
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							Global.SendEvent("1501", Global.GetLang("单次祈福次数"));
							Super.ShowNetWaiting(null);
							GameInstance.Game.SpriteExecuteImpetrateCmd(1);
						}
						return true;
					};
					return;
				}
				Global.SendEvent("1501", Global.GetLang("单次祈福次数"));
			}
			Super.ShowNetWaiting(null);
			GameInstance.Game.SpriteExecuteImpetrateCmd(1);
		};
		this.btnTen.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (1800 > Global.GetRoleOwnNumByMoneyType(163) && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("QiFuXiTong", 1800, true))
			{
				IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = 1800 - Global.GetRoleOwnNumByMoneyType(163);
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
			if (Global.GetZuanShi(ZuanShiPartClass.QiFu) && this.OneTimeType != QiFuPart.QiFuOneTimeType.FirstChouQu)
			{
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "QiFuXiTong", 1800);
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang(string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), 1800, text))
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
				{
					messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
				}
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
					{
						Global.SetZuanShi(ZuanShiPartClass.QiFu, !messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
					}
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						Global.Data.IsDoingZaJinDan = true;
						Super.ShowNetWaiting(null);
						Global.SendEvent("1502", Global.GetLang("10次祈福次数"));
						GameInstance.Game.SpriteExecuteImpetrateCmd(2);
					}
					return true;
				};
				return;
			}
			Global.Data.IsDoingZaJinDan = true;
			Super.ShowNetWaiting(null);
			Global.SendEvent("1502", Global.GetLang("10次祈福次数"));
			GameInstance.Game.SpriteExecuteImpetrateCmd(2);
		};
		base.StartCoroutine<bool>(this.InitGoodsGicon());
		GameInstance.Game.SpriteQueryImpetrateInfoCmd();
	}

	public void RefreshGoodIcons()
	{
		foreach (KeyValuePair<int, GGoodIcon> keyValuePair in this.iconDic)
		{
			GGoodIcon value = keyValuePair.Value;
			value.ResetUI();
		}
		base.StartCoroutine<bool>(this.InitGoodsGicon());
	}

	public void InitFanBei()
	{
		if (Global.isFanbei(103))
		{
			this.m_TextMiaoshu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"9d8667",
				Global.GetLang(ConfigSystemParam.GetSystemParamByName("JieRiQiFuIntro", true))
			});
		}
	}

	private void InitUI()
	{
		string text = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("可以使用")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"b266ff",
			Global.GetLang("【祈福代劵】")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("抽取道具")
		}));
		GameInstance.Game.GetJieriFanbeiInfo();
		this.m_TextMiaoshu.text = text;
		this.zhuangshiText.text = "0";
		this.JifenText.text = "0";
		this.goodsCountText.text = "0";
		this.TimeLabel.text = string.Empty;
		this.TimeText.text = string.Empty;
		this.TextOne.text = "200";
		this.TextTen.text = "1800";
		this.ButtonOneLabel.text = Global.GetLang("抽取一次");
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void RefreshByServerData(bool isFirst, int remainTime)
	{
		if (isFirst)
		{
			this.TimeLabel.text = string.Empty;
			this.TimeText.text = string.Empty;
			this.ButtonOneLabel.text = Global.GetLang("免费首抽");
			this.OneTimeType = QiFuPart.QiFuOneTimeType.FirstChouQu;
			this.TextureOne.gameObject.SetActive(false);
			this.TextOne.text = Global.GetLang("免费");
			this.IconFirst.gameObject.SetActive(true);
			this.iconTen.gameObject.SetActive(false);
		}
		else
		{
			this.IconFirst.gameObject.SetActive(false);
			this.iconTen.gameObject.SetActive(true);
			this.RemainTime = remainTime;
			this.ButtonOneLabel.text = Global.GetLang("抽取一次");
			if (remainTime > 0)
			{
				this.TextureOne.gameObject.SetActive(true);
				this.TimeLabel.text = Global.GetLang("后免费");
				this.TimeText.text = UIHelper.FormatSecs2((long)remainTime, "00:00:00");
				base.CancelInvoke("TickProc");
				base.InvokeRepeating("TickProc", 1f, 1f);
				int num = int.Parse(ConfigSystemParam.GetSystemParamByName("JinDan", true));
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(num);
				if (totalGoodsCountByID > 0)
				{
					this.OneTimeType = QiFuPart.QiFuOneTimeType.Good;
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
					this.OneTimeType = QiFuPart.QiFuOneTimeType.Diamond;
					this.TextOne.text = "200";
					this.TextureOne.URL = "NetImages/GameRes/Images/Unit/xingyunzhixing.png";
				}
			}
			else
			{
				this.TimeLabel.text = string.Empty;
				this.TimeText.text = string.Empty;
				this.TextOne.text = Global.GetLang("免费");
				this.OneTimeType = QiFuPart.QiFuOneTimeType.Free;
				this.TextureOne.gameObject.SetActive(false);
			}
		}
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.TextureOne, "QiFuXiTong", 200, "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.TextureTen, "QiFuXiTong", 1800, "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
	}

	protected void TickProc()
	{
		this.RemainTime--;
		if (this.RemainTime >= 0)
		{
			this.TimeText.text = UIHelper.FormatSecs2((long)this.RemainTime, "00:00:00");
		}
		else
		{
			base.CancelInvoke("TickProc");
			this.TimeLabel.text = string.Empty;
			this.TimeText.text = string.Empty;
			this.TextOne.text = Global.GetLang("免费");
			this.OneTimeType = QiFuPart.QiFuOneTimeType.Free;
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
		if (Global.Data.JinDanGoodsDataList == null || Global.Data.JinDanGoodsDataList.Count == 0)
		{
			GameInstance.Game.SpriteGetJinDanGoodsDataList(2000);
		}
		else
		{
			this.SetGoodsCountText();
		}
		SystemHelpMgr.OnAction(UIObjIDs.QiFuPart, HelpStateEvents.Actived, 1);
	}

	public void GetNewData()
	{
		this.RefreshMoney();
		GameInstance.Game.SpriteGetZaJinDanJiFen();
	}

	private string GetGoodsString()
	{
		XElement xelement = null;
		LinkedList<string> linkedList = HuoDongCommonFlag.TopIconTreeActivityActivedLst();
		if (linkedList.Contains("JieRi") && Global.isFanbei(103))
		{
			xelement = Global.GetGameResXml("Config/HuoDongDigType.xml");
		}
		if (xelement == null)
		{
			xelement = Global.GetGameResXml("Config/DigType.xml");
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
		yield break;
	}

	public void UpdateGoodsCountText(int count)
	{
		this.goodsCountText.Text = count.ToString();
	}

	public void SetGoodsCountText()
	{
		if (Global.Data.JinDanGoodsDataList != null)
		{
			this.goodsCountText.Text = Global.Data.JinDanGoodsDataList.Count.ToString();
		}
		else
		{
			this.goodsCountText.Text = "0";
		}
	}

	public void OnQiFuCompleted(int qiFuType, string goodsStr, int remainTime)
	{
		this.RefreshMoney();
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.TextureOne, "QiFuXiTong", 200, "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.TextureTen, "QiFuXiTong", 1800, "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 2
			});
		}
		if (this.QiFuGiftsPart == null)
		{
			this.OpenQiFuGiftsPart();
		}
		if (qiFuType == 1)
		{
			this.QiFuGiftsPart.RefreshUIUnit(true);
		}
		else
		{
			this.QiFuGiftsPart.RefreshUIUnit(false);
		}
		this.QiFuGiftsPart.RefreshAddGoodIcons(goodsStr);
		this.SetGoodsCountText();
	}

	public void OnGetXinyundianCompleted(int lucky, int bits)
	{
		if (lucky < 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("获取幸运点失败"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			this.JifenText.Text = lucky.ToString();
		}
	}

	public void RefreshMoney()
	{
		this.zhuangshiText.Text = StringUtil.substitute("{0}", new object[]
		{
			(!IConfigbase<ConfigXingYunXingShiYong>.Instance.XingYunXingKaiGuan("QiFuXiTong")) ? Global.GetRoleOwnNumByMoneyType(40) : Global.GetRoleOwnNumByMoneyType(163)
		});
	}

	public void ChangeMap()
	{
		if (null != this.QiFuGiftsWindow)
		{
			this.CloseQiFuGiftsPart();
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this.btnOne, default(Vector4));
			}
			else if (id == 1)
			{
				SystemHelpPart.SetMask(this.changKuBtn, default(Vector4));
			}
			else if (id == 10000)
			{
				SystemHelpPart.SetMask(this.closeBtn, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private void OpenQiFuGiftsPart()
	{
		this.QiFuGiftsWindow = U3DUtils.NEW<GChildWindow>();
		this.QiFuGiftsWindow.ModalType = ChildWindowModalType.Translucent;
		Super.InitChildWindow(this.QiFuGiftsWindow, Global.GetLang(string.Empty));
		if (null == this.QiFuGiftsPart)
		{
			this.QiFuGiftsPart = U3DUtils.NEW<QiFuGetGiftsPart>();
			this.QiFuGiftsPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.CloseQiFuGiftsPart();
					SystemHelpMgr.OnAction(UIObjIDs.QiFuGiftsPartOK, HelpStateEvents.Clicked, 1);
				}
			};
			this.QiFuGiftsWindow.SetContent(this.QiFuGiftsWindow.BodyPresenter, this.QiFuGiftsPart, 0.0, 0.0, true);
			Super.GData.PlayZoneRoot.Children.Add(this.QiFuGiftsWindow);
		}
		this.ObjContainer.SetActive(false);
	}

	private void CloseQiFuGiftsPart()
	{
		if (null != this.QiFuGiftsWindow)
		{
			GameInstance.Game.SpriteQueryImpetrateInfoCmd();
			this.SetGoodsCountText();
			Super.GData.PlayZoneRoot.Children.Remove(this.QiFuGiftsWindow, true);
			Object.Destroy(this.QiFuGiftsPart.gameObject);
			this.QiFuGiftsPart = null;
			this.QiFuGiftsWindow = null;
		}
		this.ObjContainer.SetActive(true);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private QiFuPart.QiFuOneTimeType OneTimeType = QiFuPart.QiFuOneTimeType.Diamond;

	public GGoodIcon icon1;

	public GGoodIcon icon2;

	public GGoodIcon icon3;

	public GGoodIcon icon4;

	public GGoodIcon icon5;

	public GGoodIcon icon6;

	public GGoodIcon icon7;

	public GGoodIcon icon8;

	public GGoodIcon icon9;

	public GGoodIcon icon10;

	public GGoodIcon icon11;

	public GGoodIcon icon12;

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

	public UILabel m_TextMiaoshu;

	public GameObject BakContainer;

	public GameObject ObjContainer;

	public UISprite IconFirst;

	public UISprite iconTen;

	public ShowNetImage TextureOne;

	public ShowNetImage TextureTen;

	public UISprite iconXingYunXing;

	private int RemainTime;

	public Dictionary<int, GGoodIcon> iconDic = new Dictionary<int, GGoodIcon>();

	private GChildWindow QiFuGiftsWindow;

	public QiFuGetGiftsPart QiFuGiftsPart;

	public enum QiFuOneTimeType
	{
		FirstChouQu = 1,
		Free,
		Good,
		Diamond
	}
}
