using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ChongZhiPart : UserControl
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

	private void InitTextInPrefabs()
	{
		this.LevProgBar.Percent = 0.0;
		this.TequanBtn.Text = Global.GetLang("查看特权");
		this.infoText.text = string.Empty;
		if (this.ConstTexts != null && this.ConstTexts.Length == 2)
		{
			this.ConstTexts[0].Text = Global.GetLang("请选择要充值的钻石：");
		}
		this.ConstTexts[1].gameObject.SetActive(false);
		this.Btn_ThirdCharge.Text = Global.GetLang("充值卡");
		this.Btn_ThirdCharge.gameObject.SetActive(false);
		if (PlatSDKMgr.PlatName == "YNGoogle")
		{
			if (Global.Data.roleData.ChangeLifeCount >= 1)
			{
				this.Btn_ThirdCharge.transform.localPosition = new Vector3(388f, 120f, -1f);
				this.TequanBtn.transform.localPosition = new Vector3(388f, 175f, -1f);
				this.Btn_ThirdCharge.gameObject.SetActive(true);
			}
			else
			{
				this.Btn_ThirdCharge.gameObject.SetActive(false);
			}
		}
		else if (PlatSDKMgr.PlatName == "YNGW")
		{
			this.Btn_ThirdCharge.gameObject.SetActive(false);
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.listBox.ItemsSource;
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		this.TequanBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		};
		this.Btn_ThirdCharge.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ChongZhi2();
		};
		this.LoadChongZhiItemConf();
		this.LoadChongZhiFanLiCfg();
		this.InitRechargeItems();
		this.GetFanLiDatas();
		this.RefreshFanLiItemByTime();
		this.CountDownWhenFanLiIsOpen();
	}

	public void GetNewData()
	{
		GameInstance.Game.GetFirstChargeState();
		GameInstance.Game.SpriteGetVipInfo();
	}

	public void SetVipInfo(int exp)
	{
		int num = Global.GetVIPLeve() + 1;
		if (num >= MUVipPart.MaxVipLevel)
		{
			num = MUVipPart.MaxVipLevel;
		}
		this.vipLeveText.Text = Global.GetVIPLeve().ToString();
		XElement gameResXml = Global.GetGameResXml("Config/MuVip.xml");
		XElement xelement = Global.GetXElement(gameResXml, "Item", "VIPLevel", Convert.ToString(num));
		if (xelement != null)
		{
			long systemParamIntByName = ConfigSystemParam.GetSystemParamIntByName("ZuanshiVIPExp");
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "NeedExp");
			int num2 = (int)((long)xelementAttributeInt * systemParamIntByName);
			this.progBarText.Text = string.Format("{0}/{1}", (exp <= xelementAttributeInt) ? exp : xelementAttributeInt, xelementAttributeInt);
			this.infoText.Text = string.Format(Global.GetLang("累计充值{{00FF00}}{0}钻石{{-}}，即可升级到{{00FF00}}VIP{1}{{-}}"), num2, num);
			this.LevProgBar.Percent = ((exp <= xelementAttributeInt) ? ((double)exp / (double)xelementAttributeInt) : 1.0);
		}
	}

	public void LoadChongZhiItemConf()
	{
		string text = string.Empty;
		string rechargeItemCfgTypeByPlatform = Global.GetRechargeItemCfgTypeByPlatform();
		XElement gameResXml = Global.GetGameResXml("Config/MU_ChongZhi.xml");
		List<XElement> list = new List<XElement>();
		foreach (XElement xelement in gameResXml.Elements())
		{
			if (xelement.Attribute("TypeID").Value.ToString() == rechargeItemCfgTypeByPlatform)
			{
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					list.Add(xelement2);
				}
				break;
			}
		}
		if (list.Count <= 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"LoadChongZhiItemConf 充值配置为空！"
			});
		}
		for (int i = 0; i < list.Count; i++)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(list[i], "Show");
			if (xelementAttributeInt == 1)
			{
				text = Global.GetXElementAttributeStr(list[i], "ID");
				ChongZhiPart.ChongzhiInfo chongzhiInfo;
				if (this.chongzhiInfoDict.ContainsKey(text))
				{
					chongzhiInfo = this.chongzhiInfoDict[text];
				}
				else
				{
					chongzhiInfo = new ChongZhiPart.ChongzhiInfo();
				}
				chongzhiInfo.Icon = Global.GetXElementAttributeStr(list[i], "Icon");
				chongzhiInfo.money = Global.GetXElementAttributeStr(list[i], "RMB");
				chongzhiInfo.zuanshiCount = Global.GetXElementAttributeStr(list[i], "ZuanShi");
				chongzhiInfo.freeDiamond = Global.GetXElementAttributeStr(list[i], "FirstBindZuanShi");
				chongzhiInfo.meiYuan = Global.GetXElementAttributeStr(list[i], "MeiYuan");
				chongzhiInfo.productId = string.Empty + text;
				if (text == "10000" && Context.IsAPPVerify)
				{
					chongzhiInfo.Icon = "1";
				}
				if (text == "10000")
				{
					chongzhiInfo.Type = ChongZhiPart.ChongzhiInfo.ChongZhiType.Normal;
				}
				else
				{
					chongzhiInfo.Type = ChongZhiPart.ChongzhiInfo.ChongZhiType.Normal;
				}
				chongzhiInfo.productId = Global.GetXElementAttributeStr(list[i], "productIdAn");
				this.chongzhiInfoDict.Add(text, chongzhiInfo);
			}
		}
	}

	private void InitChargeBtns()
	{
		ChongZhiPart.ChongzhiInfo chongzhiInfoItem = null;
		for (int i = 0; i < this.btnGroup.transform.childCount; i++)
		{
			Transform child = this.btnGroup.transform.GetChild(i);
			GButton component = child.GetComponent<GButton>();
			chongzhiInfoItem = this.chongzhiInfoDict[component.BtnTag];
			component.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GButton gbutton = U3DUtils.AS<GButton>(s as GameObject);
				chongzhiInfoItem = this.chongzhiInfoDict[gbutton.BtnTag];
				this.ChongZhi(int.Parse(chongzhiInfoItem.money), chongzhiInfoItem.productId);
			};
			Transform transform = child.Find("icon");
			UISprite component2 = transform.GetComponent<UISprite>();
			component2.spriteName = chongzhiInfoItem.Icon;
			transform = child.Find("lblMoney");
			UILabel component3 = transform.GetComponent<UILabel>();
			component3.text = Global.GetLang("￥") + chongzhiInfoItem.money;
			transform = child.Find("lblZuanshi");
			component3 = transform.GetComponent<UILabel>();
			component3.text = chongzhiInfoItem.zuanshiCount + Global.GetLang("钻石");
		}
	}

	private void ChongZhi2()
	{
		MUDebug.Log<string>(new string[]
		{
			"越南安卓包充值"
		});
		PlatSDKMgr.Pay(8, "1", 0);
	}

	private void ChongZhi(int money, string productId = "")
	{
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"越南测试用productId=",
				productId,
				"; money=",
				money
			})
		});
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"YNChongzhi_else_PlatSDKMgr.Pay(money, productId)：",
				money,
				",",
				productId,
				")"
			})
		});
		PlatSDKMgr.Pay(money, productId, 0);
	}

	private int Money { get; set; }

	private string ProductId { get; set; }

	public void InitRechargeItems()
	{
		foreach (KeyValuePair<string, ChongZhiPart.ChongzhiInfo> keyValuePair in this.chongzhiInfoDict)
		{
			ChongZhiItem chongZhiItem = U3DUtils.NEW<ChongZhiItem>();
			this.ItemCollection.AddNoUpdate(chongZhiItem);
			chongZhiItem.chongZhiType = keyValuePair.Value.Type;
			if (keyValuePair.Value.Type == ChongZhiPart.ChongzhiInfo.ChongZhiType.YueKa)
			{
				chongZhiItem.SetYueKaUI();
				chongZhiItem.texDiamondLevel.MakePixelPerfect();
			}
			else if (keyValuePair.Value.Type == ChongZhiPart.ChongzhiInfo.ChongZhiType.Normal)
			{
				chongZhiItem.DiamondNum = keyValuePair.Value.zuanshiCount;
				chongZhiItem.texDiamondLevel.MakePixelPerfect();
				chongZhiItem.FreeBingDiamond = keyValuePair.Value.freeDiamond;
			}
			if (PlatSDKMgr.PlatName == "GATAn" || PlatSDKMgr.PlatName == "GATGoogle")
			{
				chongZhiItem.Money = keyValuePair.Value.meiYuan + " USD";
			}
			chongZhiItem.Money = keyValuePair.Value.meiYuan + " USD";
			chongZhiItem.RMB = keyValuePair.Value.money;
			chongZhiItem.DiamondLevel = keyValuePair.Value.Icon;
			chongZhiItem.ItemTag = keyValuePair.Key;
			chongZhiItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.Index <= 0)
				{
					Super.HintMainText(Global.GetLang("个人限购次数不足"), 10, 3);
					return;
				}
				ChongZhiPart.ChongzhiInfo chongzhiInfo = this.chongzhiInfoDict[string.Empty + e.ID];
				this.Money = int.Parse(chongzhiInfo.money);
				this.ProductId = chongzhiInfo.productId;
				if (!HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_SuperInputFanLi))
				{
					this.ChongZhi(int.Parse(chongzhiInfo.money), chongzhiInfo.productId);
				}
				else if ((this.mChongZhiFanLiDatas.Count == 1 && this.IsTodayLastFanLiHuoDongOver()) || this.CurState != ChongZhiFanLiState.Selling || ChongChiFanLiUtils.IsXianGouCountOver(this.boughtNum, this.mChongZhiFanLiDatas[0]))
				{
					this.ChongZhi(int.Parse(chongzhiInfo.money), chongzhiInfo.productId);
				}
				else if (e.Flag == 1)
				{
					this.ChongZhi(int.Parse(chongzhiInfo.money), chongzhiInfo.productId);
				}
				else
				{
					GameInstance.Game.RequestHuoDongFanLi();
				}
			};
			UIPanel component = chongZhiItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
	}

	private bool IsTodayLastFanLiHuoDongOver()
	{
		if (this.mChongZhiFanLiDatas.Count != 1)
		{
			return false;
		}
		long num = ChongChiFanLiUtils.GetEndTime(this.mChongZhiFanLiDatas[0].Data, this.mChongZhiFanLiDatas[0].EndTime).Ticks - Global.GetCorrectDateTime().Ticks;
		double num2 = (double)(num / 10000000L);
		return num2 < 0.0;
	}

	public void HasFreeDiamond(string firstRechargeInfo)
	{
		string[] array = firstRechargeInfo.Split(new char[]
		{
			','
		});
		if (array.Length == this.listBox.Count())
		{
			for (int i = 0; i < this.listBox.Count(); i++)
			{
				ChongZhiItem chongZhiItem = U3DUtils.AS<ChongZhiItem>(this.listBox[i]);
				chongZhiItem.HasFreeDiamond = (array[i] == "1");
			}
		}
	}

	public void NotifyFirstChargeInfo(string chargeInfo)
	{
		Debug.Log("NotifyFirstChargeInfo......[" + chargeInfo + "]");
		int num = 1;
		if (!string.IsNullOrEmpty(chargeInfo))
		{
			string[] array = chargeInfo.Split(new char[]
			{
				','
			});
			for (int i = 0; i < this.listBox.Count(); i++)
			{
				ChongZhiItem chongZhiItem = U3DUtils.AS<ChongZhiItem>(this.listBox[i]);
				if (chongZhiItem.chongZhiType == ChongZhiPart.ChongzhiInfo.ChongZhiType.YueKa && Global.Data.roleData.RoleCommonUseIntPamams.Count > 28 && Global.Data.roleData.RoleCommonUseIntPamams[28] > 0)
				{
					chongZhiItem.SetYueKaUI();
				}
				else
				{
					string text = chongZhiItem.Money.Replace(Global.GetLang("售价："), string.Empty).Replace(".00", string.Empty);
					if (PlatSDKMgr.PlatName == "GATAn" || PlatSDKMgr.PlatName == "GATGoogle")
					{
						chongZhiItem.HasFreeDiamond = (array.IndexOf((chongZhiItem.RMB.SafeToInt32(0) / num).ToString()) < 0);
					}
					chongZhiItem.HasFreeDiamond = (array.IndexOf((chongZhiItem.RMB.SafeToInt32(0) / num).ToString()) < 0);
				}
			}
		}
	}

	public void RefreshYueKaItem()
	{
		for (int i = 0; i < this.listBox.Count(); i++)
		{
			ChongZhiItem chongZhiItem = U3DUtils.AS<ChongZhiItem>(this.listBox[i]);
			if (chongZhiItem.chongZhiType == ChongZhiPart.ChongzhiInfo.ChongZhiType.YueKa && Global.Data.roleData.RoleCommonUseIntPamams.Count > 28 && Global.Data.roleData.RoleCommonUseIntPamams[28] > 0)
			{
				chongZhiItem.SetYueKaUI();
				break;
			}
		}
	}

	public void RespondFanLiBuyInfo(string field)
	{
		if (string.IsNullOrEmpty(field))
		{
			return;
		}
		string[] array = field.Split(new char[]
		{
			','
		});
		int num = Global.SafeConvertToInt32(array[0]);
		int num2 = Global.SafeConvertToInt32(array[1]);
		int num3 = Global.SafeConvertToInt32(array[3]);
		if (num >= 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(array[1]);
			stringBuilder.Append(",");
			stringBuilder.Append(array[2]);
			stringBuilder.Append(",");
			stringBuilder.Append(array[3]);
			this.RespondChongZhiFanLiData(stringBuilder.ToString(), true);
			this.ChongZhi(this.Money, this.ProductId);
		}
		else if (num == -16)
		{
			Super.HintMainText(Global.GetLang("抢购份数不足"), 10, 3);
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(num, false, false)), 10, 3);
		}
	}

	private void NoFanLiData()
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			ChongZhiItem component = this.ItemCollection.GetAt(i).GetComponent<ChongZhiItem>();
			component.ChangeToPuTongItem();
		}
	}

	public void RespondChongZhiFanLiData(string str, bool isChongZhiRefresh = false)
	{
		if (!HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_SuperInputFanLi))
		{
			return;
		}
		base.CancelInvoke("BeginCountDown");
		base.CancelInvoke("EndCountDown");
		if (!string.IsNullOrEmpty(str))
		{
			string[] array = str.Split(new char[]
			{
				','
			});
			this.boughtNum = Global.SafeConvertToInt32(array[0]);
			this.fullNum = Global.SafeConvertToInt32(array[1]);
			this.boughtFullNum = Global.SafeConvertToInt32(array[2]);
		}
		if (this.fullNum - this.boughtFullNum <= 0)
		{
			this.isNotSumNum = true;
		}
		if (this.boughtNum == 0 && this.fullNum == 0 && this.boughtFullNum == 0)
		{
			this.NoFanLiData();
			return;
		}
		if (this.OneOclickBool)
		{
			base.InvokeRepeating("RefreshFanLiItemByTime", 10f, 10f);
			this.OneOclickBool = false;
			if (this.ItemCollection == null)
			{
				return;
			}
			if (this.mChongZhiFanLiDatas.Count <= 0)
			{
				return;
			}
			NGUITools.SetActive(this.m_HuoDongFanLiLab.gameObject, true);
			bool isXianGouCountOver = false;
			this.ItemConfigData = this.GetChongZhiFanLiData(this.boughtNum, this.mChongZhiFanLiDatas[0], this.isNotSumNum, out isXianGouCountOver);
			this.isBuyCountOver = isXianGouCountOver;
			this.m_HuoDongFanLiLab.text = this.GetTimeDescribe(isXianGouCountOver);
			NGUITools.SetActive(this.m_tishi.gameObject, false);
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				ChongZhiItem component = this.ItemCollection.GetAt(i).GetComponent<ChongZhiItem>();
				component.SetJieRiFanLi(this.ItemConfigData, this.boughtNum, this.CurState);
			}
		}
		else
		{
			if (isChongZhiRefresh)
			{
				base.CancelInvoke("RefreshFanLiItemByTime");
				base.InvokeRepeating("RefreshFanLiItemByTime", 10f, 10f);
			}
			if (this.mChongZhiFanLiDatas.Count <= 0)
			{
				return;
			}
			NGUITools.SetActive(this.m_HuoDongFanLiLab.gameObject, true);
			if (this.isBuyCountOver)
			{
				this.ItemConfigData = this.mChongZhiFanLiDatas[0];
				this.m_HuoDongFanLiLab.text = this.GetTimeDescribe(this.isBuyCountOver);
			}
			else
			{
				bool isXianGouCountOver2 = false;
				this.ItemConfigData = this.GetChongZhiFanLiData(this.boughtNum, this.mChongZhiFanLiDatas[0], this.isNotSumNum, out isXianGouCountOver2);
				this.isBuyCountOver = isXianGouCountOver2;
				this.m_HuoDongFanLiLab.text = this.GetTimeDescribe(isXianGouCountOver2);
			}
			NGUITools.SetActive(this.m_tishi.gameObject, false);
			for (int j = 0; j < this.ItemCollection.Count; j++)
			{
				ChongZhiItem component2 = this.ItemCollection.GetAt(j).GetComponent<ChongZhiItem>();
				component2.SetJieRiFanLi(this.ItemConfigData, this.boughtNum, this.CurState);
			}
		}
	}

	private ChongZhiFanLiData GetChongZhiFanLiData(int boughtNum, ChongZhiFanLiData tData, bool _isNotSumNum, out bool isOver)
	{
		isOver = false;
		bool flag = ChongChiFanLiUtils.IsXianGouCountOver(boughtNum, tData);
		isOver = (flag || _isNotSumNum);
		ChongZhiFanLiData result;
		if (flag || _isNotSumNum)
		{
			if (this.mChongZhiFanLiDatas.Count == 1)
			{
				result = this.mChongZhiFanLiDatas[0];
			}
			else if (this.mChongZhiFanLiDatas.Count > 1 && this.mChongZhiFanLiDatas.Contains(tData))
			{
				this.mChongZhiFanLiDatas.Remove(tData);
			}
			result = this.mChongZhiFanLiDatas[0];
		}
		else
		{
			result = tData;
		}
		return result;
	}

	private void RefreshFanLiItemByTime()
	{
		bool flag = HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_SuperInputFanLi);
		if (flag)
		{
			GameInstance.Game.SendDataHuoDongFanLi();
		}
	}

	private void GetFanLiDatas()
	{
		if (!HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_SuperInputFanLi))
		{
			return;
		}
		if (this.mChongZhiFanLiDataXml.Count <= 0)
		{
			return;
		}
		Dictionary<int, ChongZhiFanLiData>.Enumerator enumerator = this.mChongZhiFanLiDataXml.GetEnumerator();
		ChongZhiFanLiData chongZhiFanLiData = null;
		while (enumerator.MoveNext())
		{
			DateTime dateTime;
			dateTime..ctor(Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month, Global.GetCorrectDateTime().Day, Global.GetCorrectDateTime().Hour, Global.GetCorrectDateTime().Minute, Global.GetCorrectDateTime().Second);
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair = enumerator.Current;
			int year = keyValuePair.Value.Data.Year;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair2 = enumerator.Current;
			int month = keyValuePair2.Value.Data.Month;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair3 = enumerator.Current;
			int day = keyValuePair3.Value.Data.Day;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair4 = enumerator.Current;
			int hour = keyValuePair4.Value.BeginTime.Hour;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair5 = enumerator.Current;
			int minute = keyValuePair5.Value.BeginTime.Minute;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair6 = enumerator.Current;
			DateTime dateTime2;
			dateTime2..ctor(year, month, day, hour, minute, keyValuePair6.Value.BeginTime.Second);
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair7 = enumerator.Current;
			int year2 = keyValuePair7.Value.Data.Year;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair8 = enumerator.Current;
			int month2 = keyValuePair8.Value.Data.Month;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair9 = enumerator.Current;
			int day2 = keyValuePair9.Value.Data.Day;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair10 = enumerator.Current;
			int hour2 = keyValuePair10.Value.EndTime.Hour;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair11 = enumerator.Current;
			int minute2 = keyValuePair11.Value.EndTime.Minute;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair12 = enumerator.Current;
			DateTime dateTime3;
			dateTime3..ctor(year2, month2, day2, hour2, minute2, keyValuePair12.Value.EndTime.Second);
			if (dateTime2 < dateTime)
			{
				KeyValuePair<int, ChongZhiFanLiData> keyValuePair13 = enumerator.Current;
				chongZhiFanLiData = keyValuePair13.Value;
			}
			else if (dateTime2 > dateTime)
			{
				if (dateTime2.Day == dateTime.Day)
				{
					List<ChongZhiFanLiData> today = this.Today;
					KeyValuePair<int, ChongZhiFanLiData> keyValuePair14 = enumerator.Current;
					today.Add(keyValuePair14.Value);
				}
			}
			else
			{
				List<ChongZhiFanLiData> today2 = this.Today;
				KeyValuePair<int, ChongZhiFanLiData> keyValuePair15 = enumerator.Current;
				today2.Add(keyValuePair15.Value);
			}
			if (dateTime2 <= dateTime && dateTime < dateTime3)
			{
				List<ChongZhiFanLiData> today3 = this.Today;
				KeyValuePair<int, ChongZhiFanLiData> keyValuePair16 = enumerator.Current;
				today3.Add(keyValuePair16.Value);
			}
		}
		if (this.Today.Count <= 0 && chongZhiFanLiData != null)
		{
			this.Today.Add(chongZhiFanLiData);
		}
		this.mChongZhiFanLiDatas.AddRange(this.Today);
	}

	private void CountDownWhenFanLiIsOpen()
	{
		bool flag = HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_SuperInputFanLi);
		if (flag && this.mChongZhiFanLiDatas != null && this.mChongZhiFanLiDatas.Count == 1)
		{
			base.InvokeRepeating("CountDownTomorrow", 0f, 1f);
		}
	}

	private void CountDownTomorrow()
	{
		if (this.mChongZhiFanLiDatas != null && this.mChongZhiFanLiDatas.Count == 1)
		{
			bool flag = ChongChiFanLiUtils.IsNewDay(this.mChongZhiFanLiDatas[0].Data.Day);
			if (flag)
			{
				base.CancelInvoke("CountDownTomorrow");
				this.GetFanLiDatas();
				this.RefreshFanLiItemByTime();
			}
		}
	}

	private string GetTimeDescribe(bool isXianGouCountOver = false)
	{
		this.tBegin = ChongChiFanLiUtils.GetBeginTime(this.ItemConfigData.Data, this.ItemConfigData.BeginTime);
		this.tEnd = ChongChiFanLiUtils.GetEndTime(this.ItemConfigData.Data, this.ItemConfigData.EndTime);
		this.CurState = ChongChiFanLiUtils.GetCurState(this.tBegin, this.tEnd, this.boughtNum, this.ItemConfigData, isXianGouCountOver, this.isForceSelling);
		return this.mTimeDescribe;
	}

	public ChongZhiFanLiState CurState
	{
		get
		{
			return this.mCurState;
		}
		set
		{
			this.mCurState = value;
			switch (this.mCurState)
			{
			case ChongZhiFanLiState.WillBegin:
				this.mTimeDescribe = string.Format(Global.GetLang("距离下轮开始{0}"), this.ItemConfigData.BeginTime.Hour.ToString("00") + ":" + this.ItemConfigData.BeginTime.Second.ToString("00"));
				this.CountDown();
				break;
			case ChongZhiFanLiState.Selling:
				this.isForceSelling = false;
				this.mTimeDescribe = string.Format(Global.GetLang("剩余{0}份多倍，距离本轮结束 {1}"), this.fullNum - this.boughtFullNum, this.ItemConfigData.BeginTime.Hour.ToString("00") + ":" + this.ItemConfigData.BeginTime.Second.ToString("00"));
				this.CountDown();
				break;
			case ChongZhiFanLiState.End:
			{
				this.isBuyCountOver = false;
				this.mTimeDescribe = Global.GetLang("已结束");
				bool flag = this.NextFanLiData();
				if (!flag || this.IsBuyCountOver)
				{
					this.CancelAllInvokes();
					this.mTimeDescribe = Global.GetLang("请选择要充值的钻石：");
					this.NoFanLiData();
				}
				break;
			}
			}
		}
	}

	private void CountDown()
	{
		switch (this.CurState)
		{
		case ChongZhiFanLiState.WillBegin:
			base.CancelInvoke("BeginCountDown");
			base.InvokeRepeating("BeginCountDown", 0f, 1f);
			break;
		case ChongZhiFanLiState.Selling:
			base.CancelInvoke("EndCountDown");
			base.InvokeRepeating("EndCountDown", 0f, 1f);
			break;
		case ChongZhiFanLiState.End:
		{
			bool flag = this.NextFanLiData();
			if (!flag || this.IsBuyCountOver)
			{
				this.CancelAllInvokes();
				this.mTimeDescribe = Global.GetLang("请选择要充值的钻石：");
				this.NoFanLiData();
			}
			break;
		}
		}
	}

	private bool IsBuyCountOver
	{
		get
		{
			return this.fullNum - this.boughtFullNum <= 0;
		}
	}

	private bool NextFanLiData()
	{
		if (this.mChongZhiFanLiDatas.Count == 1)
		{
			return false;
		}
		if (this.mChongZhiFanLiDatas.Count > 1)
		{
			if (this.mChongZhiFanLiDatas.Contains(this.ItemConfigData))
			{
				this.mChongZhiFanLiDatas.Remove(this.ItemConfigData);
			}
			bool flag = HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_SuperInputFanLi);
			if (flag)
			{
				GameInstance.Game.SendDataHuoDongFanLi();
			}
			return true;
		}
		return false;
	}

	private void CancelAllInvokes()
	{
		base.CancelInvoke("EndCountDown");
		base.CancelInvoke("BeginCountDown");
	}

	public void BeginCountDown()
	{
		long num = this.tBegin.Ticks - Global.GetCorrectDateTime().Ticks;
		double num2 = (double)(num / 10000000L);
		num2 -= 1.0;
		if (num2 < 0.0)
		{
			base.CancelInvoke("BeginCountDown");
			this.CurState = ChongZhiFanLiState.Selling;
			this.isForceSelling = true;
			this.RefreshFanLiItemByTime();
		}
		else if (num2 <= 0.0)
		{
			this.m_HuoDongFanLiLab.text = Global.GetLang(string.Empty);
		}
		else
		{
			this.m_HuoDongFanLiLab.text = Global.GetLang("距离下轮开始：") + ChongChiFanLiUtils.GetStrTime(num2);
		}
	}

	public void EndCountDown()
	{
		long num = this.tEnd.Ticks - Global.GetCorrectDateTime().Ticks;
		double num2 = (double)(num / 10000000L);
		num2 -= 1.0;
		if (num2 < 0.0)
		{
			base.CancelInvoke("EndCountDown");
			this.CurState = ChongZhiFanLiState.End;
		}
		else
		{
			this.m_HuoDongFanLiLab.text = string.Concat(new object[]
			{
				Global.GetLang("剩余"),
				this.fullNum - this.boughtFullNum,
				Global.GetLang("份多倍，距离本轮结束 "),
				ChongChiFanLiUtils.GetStrTime(num2)
			});
		}
	}

	private void XmlTime()
	{
		XElement gameResXml = Global.GetGameResXml("Config/JieRiGifts/JieRiChongZhiFanLi.xml");
		if (gameResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(gameResXml, "Activities");
		DateTime dateTime = DateTime.Parse(Global.GetXElementAttributeStr(xelement, "AwardStartDate"));
		DateTime dateTime2 = DateTime.Parse(Global.GetXElementAttributeStr(xelement, "AwardEndDate"));
		string text = string.Concat(new object[]
		{
			dateTime.Year,
			"-",
			dateTime.Month,
			"-",
			dateTime.Day
		});
		string text2 = string.Concat(new object[]
		{
			dateTime2.Year,
			"-",
			dateTime2.Month,
			"-",
			dateTime2.Day
		});
		NGUITools.SetActive(this.m_HuoDongFanLiLab.gameObject, true);
		this.m_HuoDongFanLiLab.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("充值双倍活动时间：")
		})) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			text
		})) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("至")
		})) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			text2
		}));
		NGUITools.SetActive(this.m_tishi.gameObject, false);
	}

	private void LoadChongZhiFanLiCfg()
	{
		this.mChongZhiFanLiDataXml.Clear();
		string text = "dl_android";
		XElement gameResXml = Global.GetGameResXml("Config/JieRiGifts/Mu_ChongZhiFanLi.xml");
		List<XElement> list = new List<XElement>();
		foreach (XElement xelement in gameResXml.Elements())
		{
			if (xelement.Attribute("TypeID").Value.ToString() == text)
			{
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					list.Add(xelement2);
				}
				break;
			}
		}
		if (gameResXml == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			ChongZhiFanLiData chongZhiFanLiData = new ChongZhiFanLiData(list[i]);
			if (this.mChongZhiFanLiDataXml.ContainsKey(chongZhiFanLiData.ID))
			{
				this.mChongZhiFanLiDataXml[chongZhiFanLiData.ID] = chongZhiFanLiData;
			}
			else
			{
				this.mChongZhiFanLiDataXml.Add(chongZhiFanLiData.ID, chongZhiFanLiData);
			}
		}
	}

	public override void Destroy()
	{
		base.CancelInvoke("CountDownTomorrow");
		base.CancelInvoke("RefreshFanLiItemByTime");
		base.CancelInvoke("BeginCountDown");
		base.CancelInvoke("EndCountDown");
		base.Destroy();
	}

	private const string FLAG_FENGQIANG = "fengqiang_bg";

	private Dictionary<string, ChongZhiPart.ChongzhiInfo> chongzhiInfoDict = new Dictionary<string, ChongZhiPart.ChongzhiInfo>();

	public TextBlock vipLeveText;

	public TextBlock infoText;

	public TextBlock progBarText;

	public GImgProgressBar LevProgBar;

	public GButton TequanBtn;

	public GButton CloseBtn;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	public GameObject btnGroup;

	public GButton Btn1;

	public GButton Btn2;

	public GButton Btn3;

	public GButton Btn4;

	public GButton Btn5;

	public GButton Btn6;

	public GButton Btn7;

	public GButton Btn8;

	public GButton Btn9;

	public TextBlock[] ConstTexts;

	public ListBox listBox;

	private ObservableCollection _ItemCollection;

	public UILabel m_HuoDongFanLiLab;

	public UILabel m_tishi;

	private Dictionary<int, ChongZhiFanLiData> mChongZhiFanLiDataXml = new Dictionary<int, ChongZhiFanLiData>();

	private bool OneOclickBool = true;

	public GButton Btn_ThirdCharge;

	private XElement YNXml;

	private bool isNotSumNum;

	private bool isBuyCountOver;

	private int boughtNum;

	private int fullNum;

	private int boughtFullNum;

	private List<ChongZhiFanLiData> Today = new List<ChongZhiFanLiData>();

	private List<ChongZhiFanLiData> mChongZhiFanLiDatas = new List<ChongZhiFanLiData>();

	public ChongZhiFanLiData ItemConfigData;

	private string mTimeDescribe;

	private DateTime tBegin;

	private DateTime tEnd;

	private ChongZhiFanLiState mCurState;

	private bool isForceSelling;

	public class ChongzhiInfo
	{
		public string Icon = string.Empty;

		public string money = string.Empty;

		public string zuanshiCount = string.Empty;

		public string productId = string.Empty;

		public string freeDiamond = string.Empty;

		public string meiYuan = string.Empty;

		public ChongZhiPart.ChongzhiInfo.ChongZhiType Type;

		public enum ChongZhiType
		{
			Normal,
			YueKa
		}
	}
}
