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

public class HuoDongPartPnlMeiRiZaiXian : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnMeiRiZaiXianChouJiang.Text = Global.GetLang("领奖");
		this.m_LblMeiRiZaiXianShiChang.pivot = 4;
		this.m_LblMeiRiZaiXianShiChang.transform.localPosition = new Vector3(0f, -211f, -0.2f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.m_ListMeiRiZaiXianObC = this.m_ListMeiRiZaiXian.ItemsSource;
		Super.ShowNetWaiting(string.Empty);
		GameInstance.Game.SpriteQureyeEverydayOnlineAwardGiftInfo();
		this.m_nShowTime = 0;
		if (null != this.m_BtnMeiRiZaiXianChouJiang)
		{
			this.m_BtnMeiRiZaiXianChouJiang.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				if (Global.GetBaoGuoSpaceCount() < 5)
				{
					Super.HintMainText(Global.GetLang("领取失败，背包空间不足！"), 10, 3);
					return;
				}
				this.m_nLastStep = Global.Data.MyHuoDongData.EveryDayOnLineAwardStep;
				Super.ShowNetWaiting(string.Empty);
				this.m_BtnMeiRiZaiXianChouJiang.isEnabled = false;
				GameInstance.Game.SpriteGetEveryDayOnLineAwardGiftCmd(1);
			};
		}
	}

	private GGoodIcon GetGoodsItemIcon(string strID, bool isDrag = false)
	{
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(strID), 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		GGoodIcon ggoodIcon;
		if (dummyGoodsDataMu != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(dummyGoodsDataMu.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.GoodsID = dummyGoodsDataMu.GoodsID;
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemObject = dummyGoodsDataMu;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.Tip = Global.GetGoodsNameByID(dummyGoodsDataMu.GoodsID, false);
			bool canUse = Global.CanUseGoods(dummyGoodsDataMu.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataMu, canUse, IconTextTypes.Qianghua);
			if (isDrag)
			{
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			}
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.BackSpriteName0 = "bagGrid_bak";
		}
		ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowGoodsTip(s);
		};
		UIButtonOffset componentInChildren = ggoodIcon.GetComponentInChildren<UIButtonOffset>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		return ggoodIcon;
	}

	private void SetGoodsIconImageGray(GGoodIcon icon)
	{
		if (null != icon)
		{
			UITexture component = icon.GoodImg.gameObject.gameObject.GetComponent<UITexture>();
			if (null != component)
			{
				component.shader = Shader.Find("Unlit/Gray");
				component.gameObject.transform.localPosition = new Vector3(0f, 0f, -0.1f);
				component.gameObject.SetActive(false);
				component.gameObject.SetActive(true);
			}
		}
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		if (null == ggoodIcon)
		{
			return;
		}
		string text = Convert.ToString(ggoodIcon.GoodsID);
		if (string.Empty == text)
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(num, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, dummyGoodsDataMu);
		}
	}

	public void GetMeiRiZaiXianChouJiangNum()
	{
		string[] array = Global.Data.MyHuoDongData.EveryDayOnLineAwardGoodsID.Split(new char[]
		{
			','
		});
		float time = Time.time;
		int num = (int)time;
		if (0 >= Global.Data.roleData.DayOnlineSecond)
		{
			Global.Data.roleData.DayOnlineSecond = num + 1;
		}
		int dayOnlineSecond = Global.Data.roleData.DayOnlineSecond;
		this.m_nZaiXianShiChang = dayOnlineSecond;
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/MUNewRoleGift.xml");
		if (isolateResXml == null)
		{
			return;
		}
		this.m_ListMeiRiZaiXianObC.Clear();
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Item");
		int num2 = Global.Data.MyHuoDongData.EveryDayOnLineAwardStep;
		int num3 = 0;
		for (int i = 0; i < xelementList.Count; i++)
		{
			MeiRiZaiXianItem meiRiZaiXianItem = U3DUtils.NEW<MeiRiZaiXianItem>();
			meiRiZaiXianItem.m_ListJiangPinObC = meiRiZaiXianItem.m_ListJiangPin.ItemsSource;
			meiRiZaiXianItem.m_ListJiangPinObC.Clear();
			this.m_ListMeiRiZaiXianObC.AddNoUpdate(meiRiZaiXianItem);
			XElement xelement = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ShowGoods");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "ID");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "TimeSecs");
			meiRiZaiXianItem.m_LblShowTime.text = string.Format(Global.GetLang("{0}分钟"), xelementAttributeStr3);
			meiRiZaiXianItem.m_nMinTime = Convert.ToInt32(xelementAttributeStr3);
			if (Convert.ToInt32(xelementAttributeStr3) * 60 < this.m_nZaiXianShiChang)
			{
				meiRiZaiXianItem.m_ProgressBar.sliderValue = 1f;
				meiRiZaiXianItem.m_bChouJiang = true;
				if (0 < num2)
				{
					meiRiZaiXianItem.m_bEnd = true;
					meiRiZaiXianItem.m_LblLiPinState.text = Global.GetLang("已领取");
					meiRiZaiXianItem.m_LblLiPinState.gameObject.SetActive(false);
					meiRiZaiXianItem.m_SprYiLingQu.gameObject.SetActive(true);
					meiRiZaiXianItem.m_ProgressBar.gameObject.SetActive(false);
					meiRiZaiXianItem.m_SprProgressFull.gameObject.SetActive(false);
				}
				else
				{
					meiRiZaiXianItem.m_bEnd = false;
					meiRiZaiXianItem.m_LblLiPinState.text = Global.GetLang("可领取");
					meiRiZaiXianItem.m_LblLiPinState.gameObject.SetActive(true);
					meiRiZaiXianItem.m_SprProgressFull.gameObject.SetActive(true);
					num3++;
				}
			}
			else
			{
				int num4 = Convert.ToInt32(xelementAttributeStr3) * 60;
				meiRiZaiXianItem.m_ProgressBar.sliderValue = (float)this.m_nZaiXianShiChang / (float)num4;
				meiRiZaiXianItem.m_LblLiPinState.text = Global.GetLang("未达成");
				meiRiZaiXianItem.m_LblLiPinState.gameObject.SetActive(false);
			}
			string[] array2 = xelementAttributeStr.Split(new char[]
			{
				','
			});
			int num5 = 0;
			if (0 >= num2)
			{
				meiRiZaiXianItem.m_LblLiPinState.text = string.Empty;
				meiRiZaiXianItem.m_LblLiPinState.gameObject.SetActive(true);
				foreach (string strID in array2)
				{
					if (num5 > 5)
					{
						break;
					}
					GGoodIcon goodsItemIcon = this.GetGoodsItemIcon(strID, false);
					meiRiZaiXianItem.m_ListJiangPinObC.AddNoUpdate(goodsItemIcon);
					num5++;
				}
				meiRiZaiXianItem.m_ListJiangPinObC.DelayUpdate();
				this.FixItem(meiRiZaiXianItem, xelementAttributeStr);
			}
			if (meiRiZaiXianItem.m_bEnd)
			{
				GGoodIcon goodsItemIcon2 = this.GetGoodsItemIcon(array[i], false);
				meiRiZaiXianItem.m_ListJiangPinObC.AddNoUpdate(goodsItemIcon2);
				if (null != goodsItemIcon2)
				{
				}
				meiRiZaiXianItem.m_ListJiangPinObC.DelayUpdate();
			}
			num2--;
		}
		if (0 >= num3 && null != this.m_BtnMeiRiZaiXianChouJiang)
		{
			this.m_BtnMeiRiZaiXianChouJiang.isEnabled = false;
		}
		this.m_ListMeiRiZaiXianObC.DelayUpdate();
	}

	private void FixItem(MeiRiZaiXianItem item, string strGoods)
	{
		if (5 < item.m_ListJiangPinObC.Count)
		{
			for (int i = 0; i < item.m_ListJiangPinObC.Count; i++)
			{
				if (5 <= i)
				{
					item.m_ListJiangPinObC.RemoveAt(i);
				}
			}
			return;
		}
		if (5 > item.m_ListJiangPinObC.Count && 2 < item.m_ListJiangPinObC.Count)
		{
			string[] array = strGoods.Split(new char[]
			{
				','
			});
			foreach (string strID in array)
			{
				GGoodIcon goodsItemIcon = this.GetGoodsItemIcon(strID, false);
				item.m_ListJiangPinObC.AddNoUpdate(goodsItemIcon);
				if (5 <= item.m_ListJiangPinObC.Count)
				{
					break;
				}
			}
			return;
		}
	}

	private void MeiRiZaiXianChouJiang()
	{
		for (int i = 0; i < this.m_ListMeiRiZaiXianObC.Count; i++)
		{
			GameObject at = this.m_ListMeiRiZaiXianObC.GetAt(i);
			if (null != at)
			{
				MeiRiZaiXianItem component = at.gameObject.GetComponent<MeiRiZaiXianItem>();
				if (null != component && 3 < component.m_ListJiangPinObC.Count)
				{
					TweenPosition component2 = component.m_ListJiangPin.gameObject.GetComponent<TweenPosition>();
					if (null != component2 && component.m_bChouJiang && !component.m_bEnd)
					{
						component.m_bBegining = true;
						component2.enabled = true;
					}
				}
			}
		}
	}

	protected virtual void OnEnable()
	{
		base.StopCoroutine("TimeProc");
		base.StartCoroutine("TimeProc");
		if (this.m_bIsRecvReady)
		{
			base.StopCoroutine("TimeProc2");
			base.StartCoroutine("TimeProc2");
			this.m_nShowTime = 2;
		}
	}

	private new void OnDestroy()
	{
		this.m_bIsRecvReady = false;
		this.m_nShowTime = 0;
	}

	protected IEnumerator TimeProc2()
	{
		for (;;)
		{
			if (this.m_bIsRecvReady && 5 < this.m_nShowTime)
			{
				this.MeiRiChouJiangStop((float)Global.Data.roleData.DayOnlineSecond);
			}
			this.m_nShowTime++;
			yield return new WaitForSeconds(0.2f);
		}
		yield break;
	}

	protected IEnumerator TimeProc()
	{
		for (;;)
		{
			DateTime now = Global.GetCorrectDateTime();
			float dealTime = Time.time;
			int nDealTime = (int)dealTime;
			if (0 >= Global.Data.roleData.DayOnlineSecond)
			{
				Global.Data.roleData.DayOnlineSecond = nDealTime + 1;
			}
			int nWeiLingQu = 0;
			long nTime = (Global.GetCorrectLocalTime() - Global.g_nLoginTime) / 1000L;
			long nTotalTime = (long)Global.Data.roleData.DayOnlineSecond + nTime;
			int nJiangPinNum = this.MeiRiChouJiangProgressValue((float)nTotalTime, ref nWeiLingQu);
			if (this.m_bIsRecvReady)
			{
			}
			if (null != this.m_LblMeiRiZaiXianShiChang)
			{
				if (0 >= nJiangPinNum)
				{
					if (0 < nWeiLingQu)
					{
						this.m_LblMeiRiZaiXianShiChang.text = string.Format(Global.GetLang("今日在线时长{0}，今日还有{1}个奖励没有领取"), Global.GetColorStringForNGUIText(new object[]
						{
							"00ff00",
							UIHelper.FormatSecs(nTotalTime, string.Empty)
						}), Global.GetColorStringForNGUIText(new object[]
						{
							"00ff00",
							Convert.ToString(nWeiLingQu)
						}));
					}
					else
					{
						this.m_LblMeiRiZaiXianShiChang.text = string.Format(Global.GetLang("今日在线时长{0}，今日奖励已全部领取"), Global.GetColorStringForNGUIText(new object[]
						{
							"00ff00",
							UIHelper.FormatSecs(nTotalTime, string.Empty)
						}));
					}
				}
				else
				{
					this.m_LblMeiRiZaiXianShiChang.text = string.Format(Global.GetLang("今日在线时长{0}，请点击{1}按钮领取奖励"), Global.GetColorStringForNGUIText(new object[]
					{
						"00ff00",
						UIHelper.FormatSecs(nTotalTime, string.Empty)
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"00ff00",
						Global.GetLang("【领奖】")
					}));
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public void MeiZiZaiXianChouJiangWuPin(MUSocketConnectEventArgs e)
	{
		if (e != null)
		{
			int num = Convert.ToInt32(e.fields[1]);
			int num2 = Convert.ToInt32(e.fields[2]);
			Global.Data.MyHuoDongData.EveryDayOnLineAwardStep = num2;
			num2 -= this.m_nLastStep;
			string text = string.Format(Global.GetLang("领奖进行到{0}步,在线时长{1},物品列表{2},{3},{4},{5},{6},上一次领奖步骤{7}"), new object[]
			{
				e.fields[2],
				e.fields[3],
				e.fields[4],
				e.fields[5],
				e.fields[6],
				e.fields[7],
				e.fields[8],
				this.m_nLastStep
			});
			List<int> list = new List<int>();
			for (int i = 4; i < 4 + num2; i++)
			{
				list.Add(Convert.ToInt32(e.fields[i]));
			}
			this.m_nRecvNum = list.Count;
			this.MeiRiChouJiangSetRecvWuPin(num2, list);
			this.MeiRiZaiXianChouJiang();
			base.StartCoroutine("TimeProc2");
		}
	}

	private void MeiRiChouJiangSetRecvWuPin(int nJiangLiStep, List<int> listWuPin)
	{
		if (0 < this.m_ListMeiRiZaiXianObC.Count)
		{
			for (int i = 0; i < nJiangLiStep; i++)
			{
				GameObject at = this.m_ListMeiRiZaiXianObC.GetAt(this.m_nLastStep + i);
				if (null != at)
				{
					MeiRiZaiXianItem component = at.gameObject.GetComponent<MeiRiZaiXianItem>();
					if (null != component)
					{
						component.m_strJiangLiWuPin = Convert.ToString(listWuPin[i]);
					}
				}
			}
		}
		this.m_nLastStep = Global.Data.MyHuoDongData.EveryDayOnLineAwardStep;
		this.m_bIsRecvReady = true;
	}

	private int MeiRiChouJiangProgressValue(float dealTime, ref int nWeiLingQu)
	{
		if (0 < this.m_ListMeiRiZaiXianObC.Count)
		{
			int num = 0;
			for (int i = 0; i < this.m_ListMeiRiZaiXianObC.Count; i++)
			{
				GameObject at = this.m_ListMeiRiZaiXianObC.GetAt(i);
				if (null != at)
				{
					MeiRiZaiXianItem component = at.gameObject.GetComponent<MeiRiZaiXianItem>();
					if (null != component.m_LblShowTime)
					{
						if (1f <= component.m_ProgressBar.sliderValue)
						{
							component.m_bChouJiang = true;
							if (!component.m_bEnd && !component.m_bBegining)
							{
								component.m_SprProgressFull.gameObject.SetActive(true);
								num++;
								if (null != this.m_BtnMeiRiZaiXianChouJiang)
								{
									this.m_BtnMeiRiZaiXianChouJiang.isEnabled = true;
								}
							}
						}
						if (1f > component.m_ProgressBar.sliderValue)
						{
							nWeiLingQu++;
							component.m_ProgressBar.sliderValue = dealTime / (float)(component.m_nMinTime * 60);
						}
					}
				}
			}
			return num;
		}
		return 0;
	}

	private void MeiRiChouJiangStop(float dealTime)
	{
		if (0 < this.m_ListMeiRiZaiXianObC.Count)
		{
			for (int i = 0; i < this.m_ListMeiRiZaiXianObC.Count; i++)
			{
				GameObject at = this.m_ListMeiRiZaiXianObC.GetAt(i);
				if (null != at)
				{
					MeiRiZaiXianItem component = at.gameObject.GetComponent<MeiRiZaiXianItem>();
					if (null != component.m_LblShowTime && component.m_bBegining)
					{
						if (60f < component.m_ListJiangPin.gameObject.transform.localPosition.y && !component.m_bIsChangeIcon)
						{
							GameObject at2 = component.m_ListJiangPinObC.GetAt(0);
							this.ChangeIcon(at2, component.m_strJiangLiWuPin);
							GameObject at3 = component.m_ListJiangPinObC.GetAt(component.m_ListJiangPinObC.Count - 1);
							this.ChangeIcon(at3, component.m_strJiangLiWuPin);
							component.m_bIsChangeIcon = true;
						}
						TweenPosition component2 = component.m_ListJiangPin.gameObject.GetComponent<TweenPosition>();
						component2.duration += 0.2f;
						if (null != component2 && 30f > component.m_ListJiangPin.gameObject.transform.localPosition.y && 0 < component.m_ListJiangPinObC.Count && component.m_bBegining)
						{
							component.m_bBegining = false;
							component.m_bEnd = true;
							SpringPosition component3 = component.m_ListJiangPin.gameObject.GetComponent<SpringPosition>();
							component3.target.x = 0f;
							component3.target.y = 5f;
							component3.enabled = true;
							component2.enabled = false;
							component2.duration = 0.2f;
							GameObject at4 = component.m_ListJiangPinObC.GetAt(0);
							component.m_ProgressBar.gameObject.SetActive(false);
							component.m_SprProgressFull.gameObject.SetActive(false);
							component.m_LblLiPinState.gameObject.SetActive(false);
							component.m_SprYiLingQu.gameObject.SetActive(true);
							if (null != at4)
							{
							}
							if (null != this.m_BtnMeiRiZaiXianChouJiang)
							{
								this.m_BtnMeiRiZaiXianChouJiang.isEnabled = false;
							}
							this.m_nChouJiangWancheng++;
						}
						component.m_nLastTick++;
					}
				}
			}
			if (this.m_nRecvNum == this.m_nChouJiangWancheng && this.m_nChouJiangWancheng != 0)
			{
				this.m_nChouJiangWancheng = 0;
				this.m_nRecvNum = 0;
				GameInstance.Game.SpriteGetEveryDayOnLineAwardGiftCmd(2);
				this.m_bIsRecvReady = false;
				base.StopCoroutine("TimeProc2");
				this.m_nShowTime = 0;
			}
		}
	}

	private void ShowGetJiangLi(string strGooodsID)
	{
		string goodsNameByID = Global.GetGoodsNameByID(Convert.ToInt32(strGooodsID), false);
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang(string.Format(Global.GetLang("抽取到{0}"), goodsNameByID)), new object[0]), 0, -1, -1, 0);
	}

	private void ChangeIcon(GameObject GObject, string strRecvWuPin = "")
	{
		if (null != GObject)
		{
			GGoodIcon component = GObject.gameObject.GetComponent<GGoodIcon>();
			if (string.Empty != strRecvWuPin)
			{
				if (null != component)
				{
					component.BackgroundSprite0.gameObject.SetActive(false);
					component.BackgroundSprite1.gameObject.SetActive(false);
					component.BackgroundSprite2.gameObject.SetActive(false);
					component.TeXiao.gameObject.SetActive(false);
					component.BindingSprite.gameObject.SetActive(false);
					component.NoUseSprite.gameObject.SetActive(false);
					component.EndTimeSprite.gameObject.SetActive(false);
					component.ZhanLiSprite.gameObject.SetActive(false);
					component.GoodImg.gameObject.SetActive(false);
					component.ContentText.gameObject.SetActive(false);
					component.SecondText.gameObject.SetActive(false);
					BoxCollider componentInChildren = GObject.GetComponentInChildren<BoxCollider>();
					if (null != componentInChildren)
					{
						componentInChildren.enabled = false;
					}
				}
				GGoodIcon goodsItemIcon = this.GetGoodsItemIcon(strRecvWuPin, false);
				U3DUtils.AddChild(GObject.gameObject, goodsItemIcon.gameObject, true);
			}
			else
			{
				component.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					"Images/Goods/-1.png"
				}), false, 0);
			}
		}
	}

	public UILabel m_LblMeiRiZaiXianShiChang;

	public GButton m_BtnMeiRiZaiXianChouJiang;

	public int m_nZaiXianShiChang;

	public bool m_bIsRecvReady;

	public ListBox m_ListMeiRiZaiXian = new ListBox();

	private ObservableCollection m_ListMeiRiZaiXianObC;

	public int m_nLastStep;

	public int m_nShowTime;

	public int m_nChouJiangWancheng;

	public int m_nRecvNum;
}
