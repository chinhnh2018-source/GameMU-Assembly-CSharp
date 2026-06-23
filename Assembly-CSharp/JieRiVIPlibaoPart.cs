using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class JieRiVIPlibaoPart : UserControl
{
	private void Init()
	{
		this.m_Title1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("花费")
		});
		this.m_XiangouNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("限购：")
		}) + Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			string.Format("{0}", string.Empty)
		});
		this.m_Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("花费{0}元宝获得价值{1}元宝大礼包"), string.Empty, string.Empty)
		});
		this.m_Button.Text = Global.GetLang("兑换");
		this.m_VIPTishi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format(Global.GetLang("VIP{0}级可购买"), "10")
		});
	}

	public void InitXml(string XmlData)
	{
		XElement xelement = XElement.Parse(XmlData);
		if (xelement == null)
		{
			return;
		}
		XElement xelement2 = Global.GetXElement(xelement, "Activities");
		this.m_Activities.FromDate = Global.GetXElementAttributeStr(xelement2, "FromDate");
		this.m_Activities.ToDate = Global.GetXElementAttributeStr(xelement2, "ToDate");
		this.m_Activities.AwardStartDate = Global.GetXElementAttributeStr(xelement2, "AwardStartDate");
		this.m_Activities.AwardEndDate = Global.GetXElementAttributeStr(xelement2, "AwardEndDate");
		List<XElement> xelementList = Global.GetXElementList(xelement, "Award");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement3 = xelementList[i];
			if (xelement3 == null)
			{
				return;
			}
			JieRiVIPlibaoPart.Award award = default(JieRiVIPlibaoPart.Award);
			award.ID = Global.GetXElementAttributeInt(xelement3, "ID");
			award.VIPLevel = Global.GetXElementAttributeInt(xelement3, "VIPLevel");
			award.GoodsOne = Global.GetXElementAttributeStr(xelement3, "GoodsOne");
			award.GoodsTwo = Global.GetXElementAttributeStr(xelement3, "GoodsTwo");
			award.GoodsThr = Global.GetXElementAttributeStr(xelement3, "GoodsThr");
			award.EffectiveTime = Global.GetXElementAttributeStr(xelement3, "EffectiveTime");
			award.Image = Global.GetXElementAttributeStr(xelement3, "Image");
			award.Name = Global.GetXElementAttributeStr(xelement3, "Name");
			award.ItemImage = Global.GetXElementAttributeStr(xelement3, "ItemImage");
			award.Description = Global.GetXElementAttributeStr(xelement3, "Description");
			award.OrigPrice = Global.GetXElementAttributeInt(xelement3, "OrigPrice");
			award.Price = Global.GetXElementAttributeInt(xelement3, "Price");
			award.SinglePurchase = Global.GetXElementAttributeInt(xelement3, "SinglePurchase");
			award.FullPurchase = Global.GetXElementAttributeInt(xelement3, "FullPurchase");
			if (!this.m_MapAward.ContainsKey(award.ID))
			{
				this.m_MapAward.Add(award.ID, award);
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"配置表错误"
				});
			}
		}
	}

	protected override void InitializeComponent()
	{
		this.Init();
		this.GetDaTa();
		this.jiangliListBoxOBC = this.m_ListBox.ItemsSource;
	}

	public void GetLingJiang(string[] str)
	{
		string errMsg = StdErrorCode.GetErrMsg(int.Parse(str[0]), true, true);
		if (int.Parse(str[0]) != 0)
		{
			Super.HintMainText(Global.GetLang(errMsg), 10, 3);
			return;
		}
		Super.HintMainText(Global.GetLang("购买成功"), 10, 3);
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new string[]
			{
				errMsg,
				"全部购买数量：",
				str[2],
				"---------个人购买的数量:",
				str[3]
			})
		});
		int num = int.Parse(str[1]);
		if (this.m_MapData.ContainsKey(num))
		{
			JieRiVIPlibaoPart.Data data = default(JieRiVIPlibaoPart.Data);
			data.AllNum = int.Parse(str[2]);
			data.SelfNum = int.Parse(str[3]);
			this.m_MapData[num] = data;
			if (this.m_MapItem.ContainsKey(num))
			{
				UILabel component = this.m_MapItem[num].transform.Find("Number").GetComponent<UILabel>();
				component.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("剩余：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					this.m_MapAward[num].FullPurchase - this.m_MapData[num].AllNum
				});
				this.m_XiangouNumber.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("限购：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					string.Format("{0}", this.m_MapAward[num].SinglePurchase - this.m_MapData[num].SelfNum)
				});
				this.DuiHuanBtnRefresh(num);
			}
			return;
		}
	}

	public void ReturnData(string str = "1,100,2,100,3,100|1,1,2,2,3,3,4,4")
	{
		string[] array = str.Split(new char[]
		{
			'|'
		});
		if (array.Length != 2)
		{
			MUDebug.Log<string>(new string[]
			{
				"活动数据回调错误"
			});
			return;
		}
		string[] array2 = array[0].Split(new char[]
		{
			','
		});
		string[] array3 = array[1].Split(new char[]
		{
			','
		});
		Dictionary<int, JieRiVIPlibaoPart.Award>.Enumerator enumerator = this.m_MapAward.GetEnumerator();
		while (enumerator.MoveNext())
		{
			bool flag = true;
			KeyValuePair<int, JieRiVIPlibaoPart.Award> keyValuePair = enumerator.Current;
			int key = keyValuePair.Key;
			JieRiVIPlibaoPart.Data data = default(JieRiVIPlibaoPart.Data);
			if (array2.Length < 2)
			{
				data.AllNum = 0;
				data.SelfNum = 0;
			}
			else
			{
				for (int i = 0; i < array2.Length; i += 2)
				{
					if (key.ToString().Equals(array2[i]))
					{
						for (int j = 0; j < array3.Length; j += 2)
						{
							if (array3[j].Equals(array2[i]) && flag)
							{
								data.AllNum = int.Parse(array2[i + 1]);
								data.SelfNum = int.Parse(array3[j + 1]);
								flag = false;
								break;
							}
						}
						if (flag)
						{
							data.AllNum = int.Parse(array2[i + 1]);
							data.SelfNum = 0;
							flag = false;
						}
					}
				}
				if (flag)
				{
					data.AllNum = 0;
					data.SelfNum = 0;
				}
			}
			if (!this.m_MapData.ContainsKey(key))
			{
				this.m_MapData.Add(key, data);
			}
		}
		bool flag2 = true;
		Dictionary<int, JieRiVIPlibaoPart.Award>.Enumerator enumerator2 = this.m_MapAward.GetEnumerator();
		int num = 0;
		while (enumerator2.MoveNext())
		{
			JieRiVIPlibaoPart.<ReturnData>c__AnonStorey26E <ReturnData>c__AnonStorey26E = new JieRiVIPlibaoPart.<ReturnData>c__AnonStorey26E();
			<ReturnData>c__AnonStorey26E.<>f__this = this;
			JieRiVIPlibaoPart.<ReturnData>c__AnonStorey26E <ReturnData>c__AnonStorey26E2 = <ReturnData>c__AnonStorey26E;
			KeyValuePair<int, JieRiVIPlibaoPart.Award> keyValuePair2 = enumerator2.Current;
			<ReturnData>c__AnonStorey26E2.ID = keyValuePair2.Key;
			JieRiVIPlibaoPart.<ReturnData>c__AnonStorey26E <ReturnData>c__AnonStorey26E3 = <ReturnData>c__AnonStorey26E;
			KeyValuePair<int, JieRiVIPlibaoPart.Award> keyValuePair3 = enumerator2.Current;
			<ReturnData>c__AnonStorey26E3.award = keyValuePair3.Value;
			UILabel component = this.m_List[num].transform.Find("Number").GetComponent<UILabel>();
			if (<ReturnData>c__AnonStorey26E.award.FullPurchase - this.m_MapData[<ReturnData>c__AnonStorey26E.ID].AllNum >= 0)
			{
				component.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("剩余：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					<ReturnData>c__AnonStorey26E.award.FullPurchase - this.m_MapData[<ReturnData>c__AnonStorey26E.ID].AllNum
				});
			}
			else
			{
				component.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("剩余：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					0
				});
			}
			ShowNetImage component2 = this.m_List[num].transform.Find("back").GetComponent<ShowNetImage>();
			if (null != component2)
			{
				component2.URL = this.m_VIPLiBaoImagePath + "/VIPLiBaoDiTu.jpg";
			}
			UILabel component3 = this.m_List[num].transform.Find("Name").GetComponent<UILabel>();
			if (null != component3)
			{
				KeyValuePair<int, JieRiVIPlibaoPart.Award> keyValuePair4 = enumerator2.Current;
				if (!keyValuePair4.Value.Name.Equals(string.Empty))
				{
					UILabel uilabel = component3;
					object[] array4 = new object[2];
					array4[0] = "fac60d";
					int num2 = 1;
					KeyValuePair<int, JieRiVIPlibaoPart.Award> keyValuePair5 = enumerator2.Current;
					array4[num2] = Global.GetLang(keyValuePair5.Value.Name);
					uilabel.text = Global.GetColorStringForNGUIText(array4);
				}
				else
				{
					UILabel uilabel2 = component3;
					object[] array5 = new object[2];
					array5[0] = "fac60d";
					int num3 = 1;
					object obj = "礼包";
					KeyValuePair<int, JieRiVIPlibaoPart.Award> keyValuePair6 = enumerator2.Current;
					array5[num3] = Global.GetLang(obj + keyValuePair6.Value.ID);
					uilabel2.text = Global.GetColorStringForNGUIText(array5);
				}
			}
			ShowNetImage component4 = this.m_List[num].transform.Find("Img").GetComponent<ShowNetImage>();
			if (null != component4)
			{
				KeyValuePair<int, JieRiVIPlibaoPart.Award> keyValuePair7 = enumerator2.Current;
				if (!keyValuePair7.Value.Image.Equals(string.Empty))
				{
					URLImage urlimage = component4;
					string vipliBaoImagePath = this.m_VIPLiBaoImagePath;
					string text = "/";
					KeyValuePair<int, JieRiVIPlibaoPart.Award> keyValuePair8 = enumerator2.Current;
					urlimage.URL = vipliBaoImagePath + text + keyValuePair8.Value.Image;
				}
			}
			GButton component5 = this.m_List[num].transform.Find("back").GetComponent<GButton>();
			if (null != component5)
			{
				component5.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					<ReturnData>c__AnonStorey26E.<>f__this.ItemOnclick(<ReturnData>c__AnonStorey26E.ID, <ReturnData>c__AnonStorey26E.award);
					<ReturnData>c__AnonStorey26E.<>f__this.OnItemBack(<ReturnData>c__AnonStorey26E.ID);
				};
			}
			if (!this.m_MapItem.ContainsKey(<ReturnData>c__AnonStorey26E.ID))
			{
				this.m_MapItem.Add(<ReturnData>c__AnonStorey26E.ID, this.m_List[num]);
				if (flag2)
				{
					this.ItemOnclick(<ReturnData>c__AnonStorey26E.ID, <ReturnData>c__AnonStorey26E.award);
					this.OnItemBack(<ReturnData>c__AnonStorey26E.ID);
					flag2 = false;
				}
			}
			num++;
		}
	}

	private string GuoLv(string goodsId)
	{
		string text = string.Empty;
		string[] array = goodsId.Split(new char[]
		{
			'|'
		});
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		bool flag = true;
		for (int i = 0; i < array.Length; i++)
		{
			string goodStr = array[i].Split(new char[]
			{
				','
			})[0];
			if (!MUJieripartChongzhiKingItem.IsTongGuo(goodStr, roleOcc))
			{
				if (flag)
				{
					text += array[i];
					flag = false;
				}
				else
				{
					text = text + "|" + array[i];
				}
			}
		}
		return text;
	}

	private void OnItemBack(int ID)
	{
		if (!this.m_MapItem.ContainsKey(ID))
		{
			this.m_MapItem[ID].transform.Find("xuanzhongkuang").GetComponent<UISprite>();
		}
		if (null != this.m_MapItem[ID].transform.Find("xuanzhongkuang").GetComponent<UISprite>())
		{
			NGUITools.SetActive(this.m_MapItem[ID].transform.Find("xuanzhongkuang").GetComponent<UISprite>(), true);
		}
		if (this.m_OnitemBack != ID)
		{
			NGUITools.SetActive(this.m_MapItem[this.m_OnitemBack].transform.Find("xuanzhongkuang").GetComponent<UISprite>(), false);
		}
		this.m_OnitemBack = ID;
	}

	private void GetDaTa()
	{
		GameInstance.Game.SetDataVIPLiBao();
	}

	private void ItemOnclick(int ID, JieRiVIPlibaoPart.Award award)
	{
		this.m_ListBox.Clear();
		int num = 0;
		string text = string.Empty;
		string text2 = string.Empty;
		string effect = string.Empty;
		if (!string.IsNullOrEmpty(award.GoodsTwo))
		{
			text = award.GoodsOne + "@" + award.GoodsTwo;
			string[] array = StringUtil.trim(award.GoodsOne).Split(new char[]
			{
				'|'
			});
			string[] array2 = this.GuoLv(StringUtil.trim(award.GoodsTwo)).Split(new char[]
			{
				'|'
			});
			if (!string.IsNullOrEmpty(StringUtil.trim(award.GoodsOne)))
			{
				num += array.Length;
			}
			if (!string.IsNullOrEmpty(StringUtil.trim(award.GoodsTwo)))
			{
				num += array2.Length;
			}
		}
		else
		{
			text = award.GoodsOne;
			string[] array3 = StringUtil.trim(text).Split(new char[]
			{
				'|'
			});
			if (!string.IsNullOrEmpty(StringUtil.trim(text)))
			{
				num += array3.Length;
			}
		}
		this.m_ListBox.transform.localPosition = new Vector3(15f, this.m_ListBox.transform.localPosition.y, -1f);
		text2 = award.GoodsThr;
		if (!string.IsNullOrEmpty(StringUtil.trim(text2)))
		{
			num += text2.Split(new char[]
			{
				'|'
			}).Length;
		}
		effect = award.EffectiveTime;
		this.jiangliListBoxOBC.Clear();
		if (!string.IsNullOrEmpty(text))
		{
			Super.LoadGoodsList(text, this.jiangliListBoxOBC);
		}
		if (!string.IsNullOrEmpty(text2))
		{
			Super.LoadOtherGoodsList(text2, this.jiangliListBoxOBC, effect);
		}
		this.SetListBox(num);
		string text3 = award.Description.ToString().Split(new char[]
		{
			'{'
		})[0];
		string text4 = award.Description.ToString().Split(new char[]
		{
			'{'
		})[1].Split(new char[]
		{
			'}'
		})[1];
		string text5 = award.Description.ToString().Split(new char[]
		{
			'}'
		})[2];
		this.m_ZuanShi1.transform.localPosition = new Vector3((float)(text3.Length * 18), 3f, 0f);
		this.m_Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("{0}{1}"), Global.GetColorStringForNGUIText(new object[]
			{
				"14e43e",
				award.Price
			}), text4)
		});
		this.m_Title.transform.localPosition = new Vector3((float)(text3.Length * 18 + 27), 0f, 0f);
		this.m_ZuanShi2.transform.localPosition = new Vector3((float)(text3.Length * 18 + 27 + award.Price.ToString().Length * 10 + text4.Length * 18), 3f, 0f);
		this.m_Title3.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("{0}{1}"), award.OrigPrice, text5)
		});
		this.m_Title3.transform.localPosition = new Vector3((float)(text3.Length * 18 + 54 + award.Price.ToString().Length * 10 + text4.Length * 18), 0f, 0f);
		this.m_VIPTishi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format(Global.GetLang("VIP{0}级可购买"), award.VIPLevel)
		});
		this.m_Titles.transform.localPosition = new Vector3((float)(211 - (216 + award.Price.ToString().Length * 10 + award.VIPLevel.ToString().Length * 10) / 2), -40f, -1f);
		this.m_XiangouNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("限购：")
		}) + Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			string.Format("{0}", award.SinglePurchase - this.m_MapData[ID].SelfNum)
		});
		if (!award.ItemImage.Equals(string.Empty))
		{
			this.m_VIPLiBaoImage.URL = this.m_VIPLiBaoImagePath + "/" + award.ItemImage;
		}
		this.DuiHuanBtnRefresh(ID);
	}

	public void SetListBox(int inCount = 5)
	{
		this.m_ListBox.transform.localPosition = new Vector3(this.m_ListBox.transform.localPosition.x + (float)((5 - inCount) * 50), this.m_ListBox.transform.localPosition.y, -1f);
	}

	public void RefreshVipBnt()
	{
		this.DuiHuanBtnRefresh(this.m_OnitemBack);
	}

	private void DuiHuanBtnRefresh(int ID)
	{
		if (!this.m_MapAward.ContainsKey(ID))
		{
			return;
		}
		if (!this.m_MapData.ContainsKey(ID))
		{
			return;
		}
		int viplevel = this.m_MapAward[ID].VIPLevel;
		int selfNumber = this.m_MapAward[ID].SinglePurchase - this.m_MapData[ID].SelfNum;
		int fullNumber = this.m_MapAward[ID].FullPurchase - this.m_MapData[ID].AllNum;
		if (Global.Data.roleData.VIPLevel < viplevel)
		{
			this.m_Button.Text = Global.GetLang("提升VIP");
			this.m_Button.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				MUDebug.Log<string>(new string[]
				{
					"提升VIP"
				});
				PlayZone.GlobalPlayZone.SetVipPart(1);
			};
		}
		else
		{
			this.m_Button.Text = Global.GetLang("购买");
			this.m_Button.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				long num = DateTime.Parse(this.m_Activities.AwardEndDate).Ticks / 10000L;
				long correctLocalTime = Global.GetCorrectLocalTime();
				if ((num - correctLocalTime) / 1000L < 0L)
				{
					Super.HintMainText(Global.GetLang("抱歉！活动时间已结束"), 10, 3);
					return;
				}
				if (selfNumber <= 0)
				{
					Super.HintMainText(Global.GetLang("无剩余购买次数"), 10, 3);
				}
				else if (fullNumber <= 0)
				{
					Super.HintMainText(Global.GetLang("非常抱歉，礼包已售罄"), 10, 3);
				}
				else if (Global.Data.roleData.UserMoney < this.m_MapAward[ID].Price)
				{
					Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
				}
				else
				{
					Super.ShowNetWaiting(null);
					GameInstance.Game.LingJiangVIPLiBao(ID);
				}
			};
		}
	}

	public ShowNetImage m_VIPLiBaoBak;

	public ShowNetImage m_VIPLiBaoImage;

	public UILabel m_XiangouNumber;

	public GButton m_Button;

	public UILabel m_VIPTishi;

	public List<GameObject> m_List;

	public ListBox m_ListBox;

	public GameObject m_Titles;

	public UISprite m_ZuanShi1;

	public UISprite m_ZuanShi2;

	public UILabel m_Title1;

	public UILabel m_Title;

	public UILabel m_Title3;

	private string m_VIPLiBaoImagePath = "NetImages/GameRes/Images/VIPJieRiLiBao";

	private Dictionary<int, JieRiVIPlibaoPart.Data> m_MapData = new Dictionary<int, JieRiVIPlibaoPart.Data>();

	private JieRiVIPlibaoPart.Activities m_Activities = default(JieRiVIPlibaoPart.Activities);

	private Dictionary<int, JieRiVIPlibaoPart.Award> m_MapAward = new Dictionary<int, JieRiVIPlibaoPart.Award>();

	private Dictionary<int, GameObject> m_MapItem = new Dictionary<int, GameObject>();

	public ObservableCollection jiangliListBoxOBC;

	private int m_OnitemBack = 1;

	public struct Activities
	{
		public string FromDate;

		public string ToDate;

		public string AwardStartDate;

		public string AwardEndDate;
	}

	public struct Award
	{
		public int ID;

		public int VIPLevel;

		public string GoodsOne;

		public string GoodsTwo;

		public string GoodsThr;

		public string EffectiveTime;

		public string Image;

		public string Name;

		public string ItemImage;

		public string Description;

		public int OrigPrice;

		public int Price;

		public int SinglePurchase;

		public int FullPurchase;
	}

	public struct Data
	{
		public int AllNum;

		public int SelfNum;
	}
}
