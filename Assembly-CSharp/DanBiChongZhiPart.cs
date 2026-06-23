using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class DanBiChongZhiPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.SetData();
		this.m_ObservableCollection_ListBox = this.m_ListBox.ItemsSource;
	}

	private void Init()
	{
		this.m_ImgeDiTu.URL = "NetImages/GameRes/Images/VIPJieRiLiBao/danbichongzhipeitu.jpg";
		this.m_LabTime1.text = Global.GetLang(string.Format("{0}{1} {2} {3}", new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("活动时间：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				this.m_Activities.FromDate
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("至")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				this.m_Activities.ToDate
			})
		}));
		this.m_LabTime2.text = Global.GetLang(string.Format("{0}{1} {2} {3}", new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("领取时间：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				this.m_Activities.AwardStartDate
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("至")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				this.m_Activities.AwardEndDate
			})
		}));
		this.m_LabNeiRong.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("活动内容：")
		}) + Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.m_Description
		});
	}

	private void SetData()
	{
		GameInstance.Game.SetDataDanBiChongZhi();
	}

	public void InitXml(string XmlData)
	{
		XElement xelement = XElement.Parse(XmlData);
		if (xelement == null)
		{
			return;
		}
		XElement xelement2 = Global.GetXElement(xelement, "GiftList");
		this.m_Description = Global.GetXElementAttributeStr(xelement2, "Description");
		XElement xelement3 = Global.GetXElement(xelement, "Activities");
		this.m_Activities.ActivityType = Global.GetXElementAttributeInt(xelement3, "ActivityType");
		this.m_Activities.FromDate = Global.GetXElementAttributeStr(xelement3, "FromDate");
		this.m_Activities.ToDate = Global.GetXElementAttributeStr(xelement3, "ToDate");
		this.m_Activities.AwardStartDate = Global.GetXElementAttributeStr(xelement3, "AwardStartDate");
		this.m_Activities.AwardEndDate = Global.GetXElementAttributeStr(xelement3, "AwardEndDate");
		List<XElement> xelementList = Global.GetXElementList(xelement, "Award");
		for (int i = 0; i < xelementList.Count; i++)
		{
			DanBiChongZhiPart.Award award = default(DanBiChongZhiPart.Award);
			award.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			award.MInYuanBao = Global.GetXElementAttributeInt(xelementList[i], "MinYuanBao");
			award.MaxYuanBao = Global.GetXElementAttributeInt(xelementList[i], "MaxYuanBao");
			award.SinglePurchase = Global.GetXElementAttributeInt(xelementList[i], "SinglePurchase");
			award.GoodsOne = Global.GetXElementAttributeStr(xelementList[i], "GoodsOne");
			award.GoodsTwo = Global.GetXElementAttributeStr(xelementList[i], "GoodsTwo");
			award.GoodsThr = Global.GetXElementAttributeStr(xelementList[i], "GoodsThr");
			award.EffectiveTime = Global.GetXElementAttributeStr(xelementList[i], "EffectiveTime");
			if (!this.m_MapXml.ContainsKey(award.ID))
			{
				this.m_MapXml.Add(award.ID, award);
			}
			else
			{
				this.m_MapXml[award.ID] = award;
			}
		}
	}

	public void refershData(int ID)
	{
		for (int i = 0; i < this.m_List.Count; i++)
		{
			if (this.m_List[i].ID == ID && this.m_MapData.ContainsKey(ID))
			{
				this.m_List[i].m_YiLingQu++;
				if (this.m_MapXml[ID].SinglePurchase != -1)
				{
					this.m_List[i].Number = this.m_MapXml[ID].SinglePurchase - this.m_List[i].m_YiLingQu;
					if (this.m_List[i].Number <= 0)
					{
						this.m_List[i].Number = 0;
					}
				}
				if (this.m_List[i].m_YiLingQu >= this.m_MapXml[ID].SinglePurchase)
				{
					this.m_List[i].RefreshBtn(2);
				}
				else if (this.m_List[i].m_KeLingQu > this.m_List[i].m_YiLingQu)
				{
					this.m_List[i].RefreshBtn(4);
				}
				else if (this.m_List[i].m_YiLingQu < this.m_MapXml[ID].SinglePurchase)
				{
					this.m_List[i].RefreshBtn(3);
				}
				else
				{
					MUDebug.Log<string>(new string[]
					{
						"错误需要修改"
					});
				}
			}
		}
	}

	public void GetData(Dictionary<int, string> map)
	{
		this.m_MapData.Clear();
		this.m_List.Clear();
		this.m_ObservableCollection_ListBox.Clear();
		this.m_MapData = map;
		this.Init();
		this.AddItem();
	}

	private void AddItem()
	{
		Dictionary<int, DanBiChongZhiPart.Award>.Enumerator enumerator = this.m_MapXml.GetEnumerator();
		while (enumerator.MoveNext())
		{
			DanBiChongZhiPartItem danBiChongZhiPartItem = U3DUtils.NEW<DanBiChongZhiPartItem>();
			this.m_ObservableCollection_ListBox.AddNoUpdate(danBiChongZhiPartItem);
			DanBiChongZhiPartItem danBiChongZhiPartItem2 = danBiChongZhiPartItem;
			KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair = enumerator.Current;
			danBiChongZhiPartItem2.ID = keyValuePair.Value.ID;
			KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair2 = enumerator.Current;
			if (keyValuePair2.Value.MaxYuanBao != -1)
			{
				DanBiChongZhiPartItem danBiChongZhiPartItem3 = danBiChongZhiPartItem;
				string lang = Global.GetLang("单笔充值满{0}到{1}钻石可领取");
				KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair3 = enumerator.Current;
				object obj = keyValuePair3.Value.MInYuanBao;
				KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair4 = enumerator.Current;
				danBiChongZhiPartItem3.Title = string.Format(lang, obj, keyValuePair4.Value.MaxYuanBao);
			}
			else
			{
				DanBiChongZhiPartItem danBiChongZhiPartItem4 = danBiChongZhiPartItem;
				string lang2 = Global.GetLang("单笔充值满{0}钻石可领取");
				KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair5 = enumerator.Current;
				danBiChongZhiPartItem4.Title = string.Format(lang2, keyValuePair5.Value.MInYuanBao);
			}
			if (this.m_MapData.ContainsKey(danBiChongZhiPartItem.ID))
			{
				string text = this.m_MapData[danBiChongZhiPartItem.ID].ToString();
				if (text.Equals(string.Empty))
				{
					text = "0_0";
				}
				string[] array = text.Split(new char[]
				{
					'_'
				});
				danBiChongZhiPartItem.m_KeLingQu = int.Parse(array[0]);
				danBiChongZhiPartItem.m_YiLingQu = int.Parse(array[1]);
				KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair6 = enumerator.Current;
				if (keyValuePair6.Value.SinglePurchase != -1)
				{
					DanBiChongZhiPartItem danBiChongZhiPartItem5 = danBiChongZhiPartItem;
					KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair7 = enumerator.Current;
					danBiChongZhiPartItem5.Number = keyValuePair7.Value.SinglePurchase - int.Parse(array[1]);
					if (danBiChongZhiPartItem.Number <= 0)
					{
						danBiChongZhiPartItem.Number = 0;
					}
				}
				int num = int.Parse(array[1]);
				KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair8 = enumerator.Current;
				if (num >= keyValuePair8.Value.SinglePurchase)
				{
					danBiChongZhiPartItem.RefreshBtn(2);
				}
				else if (int.Parse(array[0]) > int.Parse(array[1]))
				{
					danBiChongZhiPartItem.RefreshBtn(4);
				}
				else
				{
					int num2 = int.Parse(array[1]);
					KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair9 = enumerator.Current;
					if (num2 < keyValuePair9.Value.SinglePurchase)
					{
						DanBiChongZhiPartItem danBiChongZhiPartItem6 = danBiChongZhiPartItem;
						KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair10 = enumerator.Current;
						danBiChongZhiPartItem6.Number = keyValuePair10.Value.SinglePurchase - int.Parse(array[1]);
						danBiChongZhiPartItem.RefreshBtn(3);
					}
					else
					{
						MUDebug.Log<string>(new string[]
						{
							"错误需要修改"
						});
					}
				}
			}
			danBiChongZhiPartItem.DPSelectedItem = delegate(object e, DPSelectedItemEventArgs s)
			{
			};
			string text2 = string.Empty;
			string text3 = string.Empty;
			string effect = string.Empty;
			KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair11 = enumerator.Current;
			if (!string.IsNullOrEmpty(keyValuePair11.Value.GoodsTwo))
			{
				KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair12 = enumerator.Current;
				string goodsOne = keyValuePair12.Value.GoodsOne;
				string text4 = "@";
				KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair13 = enumerator.Current;
				text2 = goodsOne + text4 + keyValuePair13.Value.GoodsTwo;
				KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair14 = enumerator.Current;
				string[] array2 = StringUtil.trim(keyValuePair14.Value.GoodsOne).Split(new char[]
				{
					'|'
				});
				KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair15 = enumerator.Current;
				string[] array3 = StringUtil.trim(keyValuePair15.Value.GoodsTwo).Split(new char[]
				{
					'|'
				});
			}
			else
			{
				KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair16 = enumerator.Current;
				text2 = keyValuePair16.Value.GoodsOne;
				string[] array4 = text2.Split(new char[]
				{
					'|'
				});
			}
			KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair17 = enumerator.Current;
			text3 = keyValuePair17.Value.GoodsThr;
			KeyValuePair<int, DanBiChongZhiPart.Award> keyValuePair18 = enumerator.Current;
			effect = keyValuePair18.Value.EffectiveTime;
			if (!string.IsNullOrEmpty(text2))
			{
				Super.LoadGoodsList(text2, danBiChongZhiPartItem.m_ListOBC);
			}
			if (!string.IsNullOrEmpty(text3))
			{
				Super.LoadOtherGoodsList(text3, danBiChongZhiPartItem.m_ListOBC, effect);
			}
			GGoodIcon[] componentsInChildren = this.m_ListBox.GetComponentsInChildren<GGoodIcon>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UIPanel component = componentsInChildren[i].gameObject.GetComponent<UIPanel>();
				if (component != null)
				{
					Object.Destroy(component);
				}
			}
			UIPanel component2 = danBiChongZhiPartItem.gameObject.GetComponent<UIPanel>();
			if (component2 != null)
			{
				Object.Destroy(component2);
			}
			danBiChongZhiPartItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				long num3 = DateTime.Parse(this.m_Activities.AwardEndDate).Ticks / 10000L;
				long correctLocalTime = Global.GetCorrectLocalTime();
				if ((num3 - correctLocalTime) / 1000L < 0L)
				{
					Super.HintMainText(Global.GetLang("抱歉！活动时间已结束"), 10, 3);
					return;
				}
				if (e.Type == 4)
				{
					GameInstance.Game.LingJiangDanBiChongZhi(e.ID);
				}
				else if (e.Type == 3)
				{
					PlayZone.GlobalPlayZone.ShowChongZhiWindow();
				}
			};
			this.m_List.Add(danBiChongZhiPartItem);
		}
	}

	private void Refresh(int ID, int number)
	{
		for (int i = 0; i < this.m_List.Count; i++)
		{
			if (this.m_List[i].ID == ID)
			{
				this.m_List[i].Number = number;
			}
		}
	}

	public ShowNetImage m_ImgeDiTu;

	public ListBox m_ListBox;

	public UILabel m_LabTime1;

	public UILabel m_LabTime2;

	public UILabel m_LabNeiRong;

	private List<DanBiChongZhiPartItem> m_List = new List<DanBiChongZhiPartItem>();

	private ObservableCollection m_ObservableCollection_ListBox;

	private string m_Description = string.Empty;

	private Dictionary<int, DanBiChongZhiPart.Award> m_MapXml = new Dictionary<int, DanBiChongZhiPart.Award>();

	private DanBiChongZhiPart.Activities m_Activities = default(DanBiChongZhiPart.Activities);

	private Dictionary<int, string> m_MapData = new Dictionary<int, string>();

	private struct Award
	{
		public int ID;

		public int MInYuanBao;

		public int MaxYuanBao;

		public int SinglePurchase;

		public string GoodsOne;

		public string GoodsTwo;

		public string GoodsThr;

		public string EffectiveTime;
	}

	private struct Activities
	{
		public int ActivityType;

		public string FromDate;

		public string ToDate;

		public string AwardStartDate;

		public string AwardEndDate;
	}
}
