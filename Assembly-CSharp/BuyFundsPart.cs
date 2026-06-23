using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class BuyFundsPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public int FundType
	{
		get
		{
			return this.fundType;
		}
		set
		{
			this.fundType = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.diamondList.ItemsSource;
		this.Buy.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.FundType != 1 && Global.GetVIPLeve() < this.minVIP)
			{
				PlayZone.GlobalPlayZone.ShowChongZhiWindow();
			}
			else
			{
				GameInstance.Game.BuyFund(this.FundType);
			}
		};
	}

	private void RePosition()
	{
		this.PnlGoodItem.transform.localPosition = new Vector3(66f, 72f, -0.5f);
		this.PnlGoodItem.transform.GetComponent<UIPanel>().clipRange = new Vector4(158f, -122f, 495f, 412f);
	}

	public void InitPage(int page, int mainID)
	{
		this.InitLeftPage(page, mainID);
		this.InitList(mainID, page);
	}

	private void InitLeftPage(int page, int mainID)
	{
		string tips_ = Global.GetFundSetXmlDic()[page][mainID].Tips_1;
		this.beishu.spriteName = string.Format("{0}", tips_);
		this.miaoshu1.text = string.Format("{0}", Global.GetFundSetXmlDic()[page][mainID].Tips_2);
		this.miaoshu2.text = string.Format("{0}", Global.GetFundSetXmlDic()[page][mainID].Tips_3);
		this.minVIP = Global.GetFundSetXmlDic()[page][mainID].MinVip;
		this.beishu.pivot = 5;
		this.beishu.transform.localPosition = new Vector3(-240f, this.beishu.transform.localPosition.y, this.beishu.transform.localPosition.z);
		if (page == 1)
		{
			this.leftbak.URL = "NetImages/GameRes/Images/Plate/zhuanshengjijin.jpg";
			if (Global.fundData.FundDic[page].BuyType == 2)
			{
				this.diamondBottom.SetActive(true);
				this.diamondSprite.SetActive(true);
				this.diamondNum.gameObject.SetActive(true);
				this.Buy.gameObject.SetActive(true);
				this.zhuanshengLevel.gameObject.SetActive(false);
				this.Buy.Label.text = Global.GetLang("购买");
				this.diamondNum.text = string.Format("{0}", Global.GetFundSetXmlDic()[page][mainID].Price);
			}
			else
			{
				this.diamondBottom.SetActive(false);
				this.diamondSprite.SetActive(false);
				this.diamondNum.gameObject.SetActive(false);
				this.Buy.gameObject.SetActive(false);
				this.zhuanshengLevel.gameObject.SetActive(true);
				this.zhuanshengLevel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					string.Format(Global.GetLang("角色转生等级：{0}转{1}级"), Global.fundData.FundDic[page].Value1, Global.fundData.FundDic[page].Value2)
				});
			}
			this.buyLevel.gameObject.SetActive(false);
			this.leijiLogin.gameObject.SetActive(false);
			for (int i = 0; i < this.hideGame.Length; i++)
			{
				this.hideGame[i].SetActive(false);
			}
			this.yichongzhi.gameObject.SetActive(false);
			this.yixiaofei.gameObject.SetActive(false);
			this.diamondTextXiaofei.gameObject.SetActive(false);
			this.diamondTextChongzhi.gameObject.SetActive(false);
		}
		else if (page == 2)
		{
			this.leftbak.URL = "NetImages/GameRes/Images/Plate/shiguangjijin.jpg";
			if (Global.fundData.FundDic[page].BuyType == 1)
			{
				this.diamondBottom.SetActive(false);
				this.diamondSprite.SetActive(false);
				this.diamondNum.gameObject.SetActive(false);
				this.Buy.gameObject.SetActive(false);
				this.buyLevel.gameObject.SetActive(false);
				this.leijiLogin.gameObject.SetActive(true);
				this.leijiLogin.text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					string.Format(Global.GetLang("角色累计登录：{0}天"), Global.fundData.FundDic[page].Value1)
				});
			}
			else if (Global.fundData.FundDic[page].BuyType == 2)
			{
				this.diamondBottom.SetActive(true);
				this.diamondSprite.SetActive(true);
				this.diamondNum.gameObject.SetActive(true);
				this.Buy.gameObject.SetActive(true);
				this.buyLevel.gameObject.SetActive(true);
				this.leijiLogin.gameObject.SetActive(false);
				this.Buy.Label.text = Global.GetLang("购买");
				this.diamondNum.text = string.Format("{0}", Global.GetFundSetXmlDic()[page][mainID].Price);
				this.buyLevel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("VIP{0}可购买"), Global.GetFundSetXmlDic()[page][mainID].MinVip)
				});
			}
			else if (Global.fundData.FundDic[page].BuyType == 3)
			{
				this.diamondBottom.SetActive(false);
				this.diamondSprite.SetActive(false);
				this.diamondNum.gameObject.SetActive(false);
				this.Buy.gameObject.SetActive(true);
				this.buyLevel.gameObject.SetActive(true);
				this.leijiLogin.gameObject.SetActive(false);
				this.Buy.Label.text = Global.GetLang("充值VIP");
				this.buyLevel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"d02929",
					string.Format(Global.GetLang("VIP{0}可购买"), Global.GetFundSetXmlDic()[page][mainID].MinVip)
				});
			}
			this.zhuanshengLevel.gameObject.SetActive(false);
			for (int j = 0; j < this.hideGame.Length; j++)
			{
				this.hideGame[j].SetActive(false);
			}
			this.yichongzhi.gameObject.SetActive(false);
			this.yixiaofei.gameObject.SetActive(false);
			this.diamondTextXiaofei.gameObject.SetActive(false);
			this.diamondTextChongzhi.gameObject.SetActive(false);
		}
		else if (page == 3)
		{
			this.leftbak.URL = "NetImages/GameRes/Images/Plate/haoqijijin.jpg";
			if (Global.fundData.FundDic[page].BuyType == 2)
			{
				for (int k = 0; k < this.hideGame.Length; k++)
				{
					this.hideGame[k].SetActive(false);
				}
				this.yichongzhi.gameObject.SetActive(false);
				this.yixiaofei.gameObject.SetActive(false);
				this.diamondTextXiaofei.gameObject.SetActive(false);
				this.diamondTextChongzhi.gameObject.SetActive(false);
				this.diamondBottom.SetActive(true);
				this.diamondSprite.SetActive(true);
				this.diamondNum.gameObject.SetActive(true);
				this.Buy.gameObject.SetActive(true);
				this.Buy.Label.text = Global.GetLang("购买");
				this.diamondNum.text = string.Format("{0}", Global.GetFundSetXmlDic()[page][mainID].Price);
			}
			else
			{
				for (int l = 0; l < this.hideGame.Length; l++)
				{
					this.hideGame[l].SetActive(true);
				}
				this.yichongzhi.gameObject.SetActive(true);
				this.yixiaofei.gameObject.SetActive(true);
				this.diamondTextXiaofei.gameObject.SetActive(true);
				this.diamondTextChongzhi.gameObject.SetActive(true);
				this.diamondBottom.SetActive(false);
				this.diamondSprite.SetActive(false);
				this.diamondNum.gameObject.SetActive(false);
				this.Buy.gameObject.SetActive(false);
				this.yichongzhi.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("已充值：")
				});
				this.yixiaofei.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("已消费：")
				});
				this.diamondTextChongzhi.text = string.Format("{0}", Global.fundData.FundDic[page].Value1);
				this.diamondTextXiaofei.text = string.Format("{0}", Global.fundData.FundDic[page].Value2);
			}
			this.zhuanshengLevel.gameObject.SetActive(false);
			this.buyLevel.gameObject.SetActive(false);
			this.leijiLogin.gameObject.SetActive(false);
		}
	}

	private void InitList(int MainID, int page)
	{
		this.ItemCollection.Clear();
		this.RePosition();
		this.IsMax = false;
		foreach (Global.FundXml fundXml in Global.GetFundxmlDic()[MainID].Values)
		{
			BuyFundsPartGoodItem buyFundsPartGoodItem = U3DUtils.NEW<BuyFundsPartGoodItem>();
			buyFundsPartGoodItem.ID = fundXml.ID;
			buyFundsPartGoodItem.MainID = MainID;
			buyFundsPartGoodItem.FundType = page;
			buyFundsPartGoodItem.RewardCount = fundXml.RewardCount;
			buyFundsPartGoodItem.RewardType = fundXml.RewardType;
			buyFundsPartGoodItem.Attribute = fundXml.Tips;
			buyFundsPartGoodItem.State = 3;
			buyFundsPartGoodItem.GoalNum = fundXml.GoalNum;
			buyFundsPartGoodItem.DPSelectItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.type = e.IDType;
				this.mainID = e.ID;
				this.id = e.Index;
				this.IsMax = this.IsMaxID(this.mainID, this.id);
			};
			UIPanel component = buyFundsPartGoodItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			this.ItemCollection.AddNoUpdate(buyFundsPartGoodItem);
		}
		if (this.ItemCollection.Count <= 4)
		{
			this.PnlGoodItem.GetComponent<UIDraggablePanel>().scale = new Vector3(0f, 0f, 0f);
		}
		else
		{
			this.PnlGoodItem.GetComponent<UIDraggablePanel>().scale = new Vector3(0f, 1f, 0f);
		}
		if (Global.fundData.FundDic[page].BuyType != 1)
		{
			return;
		}
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			this.item = U3DUtils.AS<BuyFundsPartGoodItem>(this.ItemCollection[i]);
			if (this.item.ID < Global.fundData.FundDic[page].AwardID)
			{
				this.item.State = 1;
			}
			else if (this.item.ID == Global.fundData.FundDic[page].AwardID)
			{
				this.item.State = Global.fundData.FundDic[page].AwardType;
			}
			else if (this.item.ID > Global.fundData.FundDic[page].AwardID)
			{
				string[] array = this.item.GoalNum.Split(new char[]
				{
					','
				});
				if (page == 1)
				{
					if (Global.fundData.FundDic[page].Value1 >= int.Parse(array[0]))
					{
						this.item.State = 4;
					}
					else
					{
						this.item.State = 3;
					}
				}
				else if (page == 2)
				{
					if (Global.fundData.FundDic[page].Value1 >= int.Parse(array[0]))
					{
						this.item.State = 4;
					}
					else
					{
						this.item.State = 3;
					}
				}
				else if (page == 3)
				{
					if (Global.fundData.FundDic[page].Value1 >= int.Parse(array[0]) && Global.fundData.FundDic[page].Value2 >= int.Parse(array[1]))
					{
						this.item.State = 4;
					}
					else
					{
						this.item.State = 3;
					}
				}
			}
		}
	}

	public void SetListBtnState(FundData fundData)
	{
		if (fundData == null)
		{
			return;
		}
		if (!fundData.IsOpen)
		{
			return;
		}
		if (fundData.State < 0)
		{
			int state = fundData.State;
			switch (state + 7)
			{
			case 0:
				Super.HintMainText(Global.GetLang("已经领取"), 10, 3);
				break;
			case 1:
				Super.HintMainText(Global.GetLang("未达到领奖条件"), 10, 3);
				break;
			case 2:
				Super.HintMainText(Global.GetLang("vip限制购买"), 10, 3);
				break;
			case 3:
				Super.HintMainText(Global.GetLang("已购买"), 10, 3);
				break;
			case 4:
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
				break;
			case 5:
				Super.HintMainText(Global.GetLang("未开放"), 10, 3);
				break;
			case 6:
				Super.HintMainText(Global.GetLang("操作失败"), 10, 3);
				break;
			}
			return;
		}
		if (fundData.State == 0)
		{
			Super.HintMainText(Global.GetLang("领取成功！"), 10, 3);
		}
		if (this.IsMax && fundData.FundType == 2)
		{
			DPSelectedItemEventHandler handler = delegate(object sender, DPSelectedItemEventArgs args)
			{
				if (args.ID == 0)
				{
				}
			};
			string message = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				string.Format("       VIP{0}", Global.GetFundSetXmlDic()[this.type][this.mainID].MinVip)
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"9d8667",
				Global.GetLang("的时光基金已全部领取 \r\n \r\n          新的时光基金开放购买")
			});
			Super.ShowMessageBoxEx(Global.GetLang("恭喜"), message, handler, new string[]
			{
				Global.GetLang("确定")
			});
		}
		if (this.IsMax)
		{
			this.InitPage(fundData.FundType, fundData.FundDic[fundData.FundType].FundID);
		}
		else
		{
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				this.item = U3DUtils.AS<BuyFundsPartGoodItem>(this.ItemCollection[i]);
				if (this.item.ID < fundData.FundDic[fundData.FundType].AwardID)
				{
					this.item.State = 1;
				}
				else if (this.item.ID == fundData.FundDic[fundData.FundType].AwardID)
				{
					this.item.State = fundData.FundDic[fundData.FundType].AwardType;
				}
				else if (this.item.ID > fundData.FundDic[fundData.FundType].AwardID)
				{
					string[] array = this.item.GoalNum.Split(new char[]
					{
						','
					});
					if (fundData.FundType == 1)
					{
						if (Global.fundData.FundDic[fundData.FundType].Value1 >= int.Parse(array[0]))
						{
							this.item.State = 4;
						}
						else
						{
							this.item.State = 3;
						}
					}
					else if (fundData.FundType == 2)
					{
						if (Global.fundData.FundDic[fundData.FundType].Value1 >= int.Parse(array[0]))
						{
							this.item.State = 4;
						}
						else
						{
							this.item.State = 3;
						}
					}
					else if (fundData.FundType == 3)
					{
						if (Global.fundData.FundDic[fundData.FundType].Value1 >= int.Parse(array[0]) && Global.fundData.FundDic[fundData.FundType].Value2 >= int.Parse(array[1]))
						{
							this.item.State = 4;
						}
						else
						{
							this.item.State = 3;
						}
					}
				}
			}
		}
	}

	private bool IsMaxID(int mainID, int id)
	{
		foreach (Global.FundXml fundXml in Global.GetFundxmlDic()[mainID].Values)
		{
			if (fundXml.ID > id)
			{
				return false;
			}
		}
		return true;
	}

	private void OnDestory()
	{
		Object.Destroy(base.gameObject);
	}

	public GButton Buy;

	public ListBox diamondList;

	public ShowNetImage leftbak;

	public UILabel diamondNum;

	public GameObject diamondSprite;

	public GameObject diamondBottom;

	public UILabel miaoshu1;

	public UILabel miaoshu2;

	public UISprite beishu;

	public GameObject PnlGoodItem;

	public UILabel zhuanshengLevel;

	public UILabel buyLevel;

	public UILabel leijiLogin;

	public GameObject[] hideGame;

	public UILabel yichongzhi;

	public UILabel yixiaofei;

	public UILabel diamondTextXiaofei;

	public UILabel diamondTextChongzhi;

	public BuyFundsPartGoodItem item;

	private ObservableCollection _ItemCollection;

	private int fundType = 1;

	private int minVIP;

	private int mainID;

	private int type;

	private int id;

	private bool IsMax;
}
