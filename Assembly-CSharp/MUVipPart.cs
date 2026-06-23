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

public class MUVipPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.LevProgBar.Percent = 0.0;
		this.tab1Btn.Text = Global.GetLang("VIP总览");
		this.tab2Btn.Text = Global.GetLang("福利对比");
		this.tab3Btn.Text = Global.GetLang("特权操作");
		this.tab4Btn.Text = Global.GetLang("特权BUFF");
		this.tab5Btn.Text = Global.GetLang("专属客服");
		this.chongzhiBtn.Text = Global.GetLang("充值");
		this.lingquBtn.Text = Global.GetLang("领取");
		this.levelUpinfoText.Text = string.Empty;
		this.levelUpinfoText.Z = -1.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.tab1Btn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.tab2Btn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.tab3Btn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.tab4Btn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.tab5Btn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.vipLeveText.Text = Global.GetVIPLeve().ToString();
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
		this.chongzhiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 10
				});
			}
		};
		this.ItemCollection = this.listBox.ItemsSource;
		this.ItemCollection2 = this.VipListBox.ItemsSource;
		this.VipListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.buffItemCollection = this.buffListBox.ItemsSource;
		this.tab1Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(1);
		};
		this.tab2Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(2);
		};
		this.tab3Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(3);
		};
		this.tab4Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(4);
		};
		this.tab5Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsVIPPrivilegeOpen())
			{
				return;
			}
			this.SetPart(5);
		};
		this.tab5Btn.gameObject.SetActive(false);
		this.lingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.SendEvent("1700", Global.GetLang("领取VIP礼包次数"));
			GameInstance.Game.SpriteGetVipLeveLaward(this.CurrentSelectedPage);
		};
		this.xml = Global.GetGameResXml("Config/MuVip.xml");
		for (int i = 1; i <= MUVipPart.MaxVipLevel; i++)
		{
			MuVipLeveItem muVipLeveItem = U3DUtils.NEW<MuVipLeveItem>();
			this.ItemCollection2.Add(muVipLeveItem);
			muVipLeveItem.Btn.Label.text = string.Format("VIP {0}", i);
			muVipLeveItem.vipLevel = i;
			UIPanel component = muVipLeveItem.gameObject.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
		ActivityTipManager.RegActivityTipItem(10001, delegate(int s, ActivityTipItem e)
		{
			this._VIPGiftsTipIcon.gameObject.SetActive(e.IsActive);
		});
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		ActivityTipManager.RegActivityTipItem(10001, null);
	}

	public void GetNewData()
	{
		GameInstance.Game.SpriteGetVipInfo();
	}

	public void SetnVipExp(int exp, int flag)
	{
		this.nVipExp = exp;
		this.nVipAwardFlag = flag;
		int num = Global.GetVIPLeve() + 1;
		if (num > MUVipPart.MaxVipLevel)
		{
			num = MUVipPart.MaxVipLevel;
		}
		long systemParamIntByName = ConfigSystemParam.GetSystemParamIntByName("ZuanshiVIPExp");
		XElement xelement = Global.GetXElement(this.xml, "Item", "VIPLevel", Convert.ToString(num));
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "NeedExp");
		int num2 = (int)((long)xelementAttributeInt * systemParamIntByName);
		this.jinduText.Text = string.Format("{0}/{1}", (this.nVipExp <= xelementAttributeInt) ? this.nVipExp : xelementAttributeInt, xelementAttributeInt);
		this.levelUpinfoText.Text = string.Format(Global.GetLang("累计充值{{00FF00}}{0}钻石{{-}}，即可升级到{{00FF00}}VIP{1}{{-}}"), num2, num);
		this.LevProgBar.Percent = ((this.nVipExp <= xelementAttributeInt) ? ((double)this.nVipExp / (double)xelementAttributeInt) : 1.0);
		if (Global.GetVIPLeve() == 0)
		{
			this.InitVIPZongLanPartData(0);
		}
		else
		{
			this.InitVIPZongLanPartData(Global.GetVIPLeve());
		}
		this.tab5Btn.gameObject.SetActive(this.IsVIPPrivilegeOpen());
	}

	private void InitVIPZongLanPartData(int leve)
	{
		this.CurrentSelectedPage = leve;
		int num = leve;
		if (num == 0)
		{
			num = 1;
		}
		else if (num >= MUVipPart.MaxVipLevel)
		{
			num = MUVipPart.MaxVipLevel;
		}
		this.vipLeveText2.Text = num.ToString();
		this.image1.URL = StringUtil.substitute("NetImages/GameRes/Images/Vip/{0}_{1}.jpg", new object[]
		{
			num,
			1
		});
		this.image2.URL = StringUtil.substitute("NetImages/GameRes/Images/Vip/{0}_{1}.jpg", new object[]
		{
			num,
			2
		});
		this.image3.URL = StringUtil.substitute("NetImages/GameRes/Images/Vip/{0}_{1}.jpg", new object[]
		{
			num,
			3
		});
		long systemParamIntByName = ConfigSystemParam.GetSystemParamIntByName("ZuanshiVIPExp");
		this.biliText.Text = string.Format("(1:{0})", systemParamIntByName);
		if (this.xml != null)
		{
			XElement xelement = Global.GetXElement(this.xml, "Item", "VIPLevel", Convert.ToString(num));
			if (xelement != null)
			{
				this.ItemCollection.Clear();
				string[] array = Global.GetXElementAttributeStr(xelement, "LiBaoAward").Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					int goodsID = Convert.ToInt32(array2[0]);
					int gcount = Convert.ToInt32(array2[1]);
					int binding = Convert.ToInt32(array2[2]);
					int forgeLevel = Convert.ToInt32(array2[3]);
					int zhuijiaLevel = Convert.ToInt32(array2[4]);
					int lucky = Convert.ToInt32(array2[5]);
					int zhuoyueIndex = Convert.ToInt32(array2[6]);
					GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(goodsID, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, binding, gcount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
					GGoodIcon icon = this.GetIcon(dummyGoodsDataMu);
					this.ItemCollection.Add(icon);
					Super.InitGoodsGIcon(icon, dummyGoodsDataMu, true, IconTextTypes.Qianghua);
					icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
				}
			}
			if ((this.nVipAwardFlag & Global.GetBitValue(num + 1)) > 0)
			{
				this.SwitchLingquBtnState(true);
				this.lingquBtn.isEnabled = false;
			}
			else
			{
				this.SwitchLingquBtnState(false);
				if (Global.GetVIPLeve() < num)
				{
					this.lingquBtn.isEnabled = false;
				}
				else
				{
					this.lingquBtn.isEnabled = true;
				}
			}
		}
	}

	private void InitVIPBuffDataList(int viplev)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/VipDailyAwards.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(isolateResXml, "Config"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "AwardID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "VIPlev");
				if (xelementAttributeInt <= 1012)
				{
					if (viplev == xelementAttributeInt2)
					{
						dictionary.Add(xelementAttributeInt, xelementAttributeInt2);
					}
					if (xelementAttributeInt == 1012 && dictionary.Count <= 0)
					{
						dictionary.Add(1001, 1);
					}
				}
				else
				{
					dictionary.Add(xelementAttributeInt, xelementAttributeInt2);
				}
			}
		}
		this.InitBuffListBox(dictionary.Count);
		int num = 0;
		Dictionary<int, int>.Enumerator enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			MUVipItem item = U3DUtils.NEW<MUVipItem>();
			this.buffListBox.Replace(this.getindex(num), item.gameObject);
			item.awardID = keyValuePair.Key;
			item.needLev = keyValuePair.Value.ToString();
			if (viplev >= keyValuePair.Value || keyValuePair.Value == -1)
			{
				item.blstat = true;
			}
			else
			{
				item.blstat = false;
			}
			if (item.awardID == 4001)
			{
				item.lingquBtn.Label.text = Global.GetLang("一键修理");
			}
			else if (item.awardID == 5001)
			{
				item.lingquBtn.Label.text = Global.GetLang("打开仓库");
			}
			else if (item.awardID == 6001)
			{
				item.lingquBtn.Label.text = Global.GetLang("打开药店");
			}
			UIPanel component = item.transform.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
			num++;
			item.lingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (item.awardID == 4001 && !Global.IsAnyEquipNeedMend())
				{
					Super.HintMainText(Global.GetLang("您当前身上佩戴的装备不需要修理"), 10, 3);
					return;
				}
				if (item.awardID == 5001)
				{
					if (this.DPSelectedItem != null)
					{
						Global.SendEvent("1701", Global.GetLang("随身仓库使用次数"));
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = 1
						});
					}
				}
				else if (item.awardID == 6001)
				{
					if (this.DPSelectedItem != null)
					{
						Global.SendEvent("1703", Global.GetLang("随身药店使用次数"));
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = 2
						});
					}
				}
				else
				{
					Global.SendEvent("1702", Global.GetLang("随身修理使用次数"));
					GameInstance.Game.SpriteGetVipAwardCmd(item.awardID);
				}
			};
		}
	}

	private void InitBuffListBox(int count)
	{
		if (count < 6)
		{
			count = 6;
		}
		else
		{
			count = 12;
		}
		this.countItem = count;
		this.buffItemCollection.Clear();
		for (int i = 0; i < count; i++)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			this.buffItemCollection.Add(ggoodIcon);
			ggoodIcon.name = StringUtil.substitute("item{0}", new object[]
			{
				i
			});
		}
	}

	private int getindex(int dex)
	{
		int num = this.countItem / 2;
		this.buffListBox.maxPerLine = num;
		int num2 = dex / 3 / 2;
		int num3 = dex % 6;
		int num4 = num3 % 3;
		int num5 = num3 / 3 % 2;
		return num4 + num5 * num + num2 * 3;
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		MuVipLeveItem muVipLeveItem = U3DUtils.AS<MuVipLeveItem>(this.VipListBox.SelectedItem);
		if (null == muVipLeveItem)
		{
			return;
		}
		if (this.tempItem != null && this.tempItem != muVipLeveItem)
		{
			this.tempItem.Sate.gameObject.SetActive(false);
		}
		this.tempItem = muVipLeveItem;
		muVipLeveItem.Sate.gameObject.SetActive(true);
		this.InitVIPZongLanPartData(muVipLeveItem.vipLevel);
	}

	private GGoodIcon GetIcon(GoodsData gd)
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			Super.GetIconCode(gd.GoodsID)
		}), false, 0);
		ggoodIcon.TipType = 1;
		ggoodIcon.ItemCode = gd.GoodsID;
		ggoodIcon.ItemObject = gd;
		ggoodIcon.BoxTypes = -1;
		return ggoodIcon;
	}

	public void SetPart(int type)
	{
		switch (type)
		{
		case 1:
			this.Canver2.gameObject.SetActive(false);
			this.Canver3.gameObject.SetActive(false);
			this.Canver1.gameObject.SetActive(true);
			this.Canver4.gameObject.SetActive(false);
			this.Canver5.gameObject.SetActive(false);
			this.buffItemCollection.Clear();
			this.SetBtnStat(this.tab1Btn);
			if (Global.GetVIPLeve() == 0)
			{
				this.VipListBox.SelectedIndex = 0;
			}
			else
			{
				this.VipListBox.SelectedIndex = Global.GetVIPLeve() - 1;
				if (Global.GetVIPLeve() > 11)
				{
					this.UIDragPl.target.x = -400f;
					this.UIDragPl.target.y = 163f;
					this.UIDragPl.enabled = true;
				}
				else if (Global.GetVIPLeve() > 7)
				{
					this.UIDragPl.target.x = -400f;
					this.UIDragPl.target.y = 55f;
					this.UIDragPl.enabled = true;
				}
			}
			break;
		case 2:
			this.Canver1.gameObject.SetActive(false);
			this.Canver3.gameObject.SetActive(false);
			this.Canver2.gameObject.SetActive(true);
			this.Canver4.gameObject.SetActive(false);
			this.Canver5.gameObject.SetActive(false);
			this.buffItemCollection.Clear();
			this.SetBtnStat(this.tab2Btn);
			break;
		case 3:
			this.Canver1.gameObject.SetActive(false);
			this.Canver2.gameObject.SetActive(false);
			this.Canver4.gameObject.SetActive(false);
			this.Canver3.gameObject.SetActive(true);
			this.Canver5.gameObject.SetActive(false);
			this.SetBtnStat(this.tab3Btn);
			this.InitVIPBuffDataList(Global.GetVIPLeve());
			break;
		case 4:
			this.Canver1.gameObject.SetActive(false);
			this.Canver2.gameObject.SetActive(false);
			this.Canver3.gameObject.SetActive(false);
			this.Canver4.gameObject.SetActive(true);
			this.Canver5.gameObject.SetActive(false);
			this.buffItemCollection.Clear();
			this.SetBtnStat(this.tab4Btn);
			if (this.m_MUVipBuffPart == null)
			{
				this.m_MUVipBuffPart = U3DUtils.NEW<MUVipBuffPart>();
				this.m_MUVipBuffPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = 10
						});
					}
				};
				this.m_MUVipBuffPart.xml = this.xml;
				U3DUtils.AddChild(this.Canver4, this.m_MUVipBuffPart.gameObject, true);
			}
			this.m_MUVipBuffPart.SetnVipExp(this.nVipExp, this.nVipAwardFlag);
			break;
		case 5:
			this.Canver1.gameObject.SetActive(false);
			this.Canver2.gameObject.SetActive(false);
			this.Canver3.gameObject.SetActive(false);
			this.Canver4.gameObject.SetActive(false);
			this.Canver5.gameObject.SetActive(true);
			this.buffItemCollection.Clear();
			this.SetBtnStat(this.tab5Btn);
			if (null == this.vipPrivileges)
			{
				this.vipPrivileges = U3DUtils.NEW<VIPPrivileges>();
				U3DUtils.AddChild(this.Canver5, this.vipPrivileges.gameObject, true);
			}
			break;
		}
	}

	private void SetBtnStat(GButton btn)
	{
		if (this.tempBtn != null)
		{
			if (this.tempBtn == btn)
			{
				btn.Label.color = NGUIMath.HexToColorEx(16766048U);
				return;
			}
			btn.Pressed = true;
			btn.Label.color = NGUIMath.HexToColorEx(16766048U);
			this.tempBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
			this.tempBtn.Pressed = false;
			this.tempBtn = btn;
		}
		else
		{
			btn.Label.color = NGUIMath.HexToColorEx(16766048U);
			btn.Pressed = true;
			this.tempBtn = btn;
		}
	}

	public void SwitchLingquBtnState(bool isLingqu)
	{
		this.lingquBtn.gameObject.SetActive(!isLingqu);
		this.lingquOkSprite.gameObject.SetActive(isLingqu);
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.None, ggoodIcon.ItemObject as GoodsData);
	}

	public void OnUseVipDailyPriorityCompleted(int result, int type = 0)
	{
		if (result >= 0)
		{
			if (type != 4001)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("领取完成！"), new object[0]), 10, 3);
			}
			else
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("修理完成！"), new object[0]), 10, 3);
			}
		}
		else if (result == -1)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("领取特权奖励失败，需要vip权限"), new object[0]), 10, 3);
		}
		else if (result == -2)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("VIP等级不够当前领取的奖励要求！"), new object[0]), 10, 3);
		}
		else if (result == -3)
		{
			this.lingquBtn.isEnabled = false;
			Super.HintMainText(StringUtil.substitute(Global.GetLang("此奖励已经领取过！"), new object[0]), 10, 3);
		}
		else if (result == -100)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("领取失败！"), new object[0]), 10, 3);
		}
		else if (result == -50)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("没有要修理的装备！"), new object[0]), 10, 3);
		}
	}

	private bool IsVIPPrivilegeOpen()
	{
		string name = string.Empty;
		switch (Global.DevicePlatform())
		{
		case AppPlatform.Default:
		case AppPlatform.Android:
			name = "VIPKeFu_Android";
			break;
		case AppPlatform.IOS:
			name = "VIPKeFu_APP";
			break;
		case AppPlatform.IOS_Jailbreak:
			name = "VIPKeFu_YueYu";
			break;
		}
		string systemParamByName = ConfigSystemParam.GetSystemParamByName(name, true);
		if (string.IsNullOrEmpty(systemParamByName))
		{
			return false;
		}
		string[] array = systemParamByName.Split(new char[]
		{
			','
		});
		if (array == null || array.Length < 2)
		{
			return false;
		}
		string text = array[0];
		if (string.IsNullOrEmpty(text) || !text.Equals("1"))
		{
			return false;
		}
		string str = array[1];
		return this.nVipExp >= Global.SafeConvertToInt32(str);
	}

	public GButton tab1Btn;

	public GButton tab2Btn;

	public GButton tab3Btn;

	public GButton tab4Btn;

	public GButton tab5Btn;

	public GButton closeBtn;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	public int iVipLeve;

	public int nVipExp;

	public int nVipAwardFlag;

	private XElement xml;

	public Transform _VIPGiftsTipIcon;

	public GameObject Canver1;

	public TextBlock vipLeveText;

	public TextBlock vipLeveText2;

	public TextBlock levelUpinfoText;

	public TextBlock biliText;

	public TextBlock jinduText;

	public GImgProgressBar LevProgBar;

	public GButton lingquBtn;

	public UISprite lingquOkSprite;

	public ListBox listBox;

	public ListBox VipListBox;

	public ShowNetImage image1;

	public ShowNetImage image2;

	public ShowNetImage image3;

	public GButton chongzhiBtn;

	public SpringPanel UIDragPl;

	private ObservableCollection ItemCollection;

	private ObservableCollection ItemCollection2;

	private int CurrentSelectedPage = 1;

	public GameObject Canver2;

	public GameObject Canver3;

	public GameObject Canver4;

	public GameObject Canver5;

	public ListBox buffListBox;

	private ObservableCollection buffItemCollection;

	public MUVipBuffPart m_MUVipBuffPart;

	[HideInInspector]
	public VIPPrivileges vipPrivileges;

	public static int MaxVipLevel = 15;

	private int countItem;

	private MuVipLeveItem tempItem;

	private GButton tempBtn;
}
