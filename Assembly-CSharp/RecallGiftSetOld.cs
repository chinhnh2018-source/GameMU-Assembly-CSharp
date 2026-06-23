using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class RecallGiftSetOld : RecallGoodsEx
{
	protected override void InitTextInPrefabs()
	{
		base.InitTextInPrefabs();
		if (null != this.signBtn)
		{
			this.signBtn.Text = Global.GetLang("领取");
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this.signBtn)
		{
			this.signBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.PickupRecallGiftSetRequest();
			};
		}
	}

	public override void OnDragFinished()
	{
		int num = this.goodsList.Count();
		Vector3 vector = this.dragPanel.transform.localPosition - this.panelPosition;
		Vector3 size = RecallRewardItem.GetSize(base.GetType());
		int num2 = 0;
		if (vector.y > 0f)
		{
			num2 = Mathf.RoundToInt(vector.y / size.y);
		}
		if (num2 >= num - 1)
		{
			num2 = num - 1;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num2 == 0 || num2 == num - 1)
		{
			Vector3 vector2 = this.panelPosition + new Vector3(0f, size.y * (float)num2, 0f);
			SpringPanel.Begin(this.dragPanel.gameObject, vector2, 10f);
		}
	}

	public override bool InitRewards()
	{
		if (!base.InitRewards())
		{
			return false;
		}
		XElement xelement = Global.GetXElement(base.xml, "GiftList");
		string text = Global.GetLang("成为回归用户即可领取(每个帐号只能领取1次)");
		if (xelement != null)
		{
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Title");
			if (!string.IsNullOrEmpty(xelementAttributeStr))
			{
				text = xelementAttributeStr;
			}
		}
		if (null != this.activityInfo)
		{
			this.activityInfo.text = text;
		}
		this.SetActivityTime();
		if (this.mTab.xmlList == null || this.mTab.xmlList.Count > 0)
		{
		}
		this.GetRecallGiftSetRequest();
		return true;
	}

	public void SetGiftSetStatus(int status)
	{
		switch (status + 7)
		{
		case 0:
		case 5:
		case 9:
		case 10:
			this.signBtn.isEnabled = false;
			this.signBtn.gameObject.SetActive(true);
			break;
		case 7:
			this.signBtn.isEnabled = true;
			this.signBtn.gameObject.SetActive(true);
			this.signStatus.gameObject.SetActive(false);
			break;
		case 8:
			this.signBtn.isEnabled = false;
			this.signBtn.gameObject.SetActive(false);
			this.signStatus.gameObject.SetActive(true);
			break;
		}
	}

	private void SetActivityTime()
	{
		int day = ConfigLaoWanJiaZhaoHui.PlayerRecallDaysNum() + ConfigLaoWanJiaZhaoHui.recallDelay;
		DateTime dataTime = ConfigLaoWanJiaZhaoHui.CalPlayerRecallDateTime(day, 0, 0, 0);
		string text = ConfigLaoWanJiaZhaoHui.PlayerRecallStartTime().toString("yyyy-MM-dd HH:mm:ss");
		string text2 = dataTime.toString("yyyy-MM-dd HH:mm:ss");
		if (null != this.startTime)
		{
			this.startTime.text = text;
		}
		if (null != this.endTime)
		{
			this.endTime.text = text2;
		}
	}

	private static bool IsGoodsToOccupation(int nGoodsID)
	{
		if (0 >= nGoodsID)
		{
			return false;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(nGoodsID);
		int mainOccupation = goodsXmlNodeByID.MainOccupation;
		return mainOccupation == -1 || (mainOccupation == Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) && (mainOccupation != 3 || (Global.GetMJSTypeByAttr() == MJSSkillType.Strength_Sword && goodsXmlNodeByID.Strength > goodsXmlNodeByID.Intelligence) || (Global.GetMJSTypeByAttr() == MJSSkillType.Magic_Sword && goodsXmlNodeByID.Intelligence > goodsXmlNodeByID.Strength)));
	}

	private void ShowGoodsIconList()
	{
		this._itemCollection.Clear();
		this.goodsIconCanvas.Clear();
		for (int i = 0; i < this.itemsList.Count; i++)
		{
			base.ItemCollection.AddNoUpdate(this.itemsList[i]);
		}
		base.ItemCollection.DelayUpdate();
	}

	private void AddGoodsIconList(string goodsStr, bool isOcc = false)
	{
		string[] array = goodsStr.Split(new char[]
		{
			'|'
		});
		if (isOcc)
		{
			int num = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					','
				});
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array3[0]), Convert.ToInt32(array3[3]), Convert.ToInt32(array3[4]), Convert.ToInt32(array3[6]), Convert.ToInt32(array3[5]), Convert.ToInt32(array3[2]), Convert.ToInt32(array3[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				if (RecallGiftSetOld.IsGoodsToOccupation(dummyGoodsDataMu.GoodsID))
				{
					this.AddGoodsIcon(dummyGoodsDataMu, true, false);
				}
			}
		}
		else
		{
			for (int j = 0; j < array.Length; j++)
			{
				string[] array3 = array[j].Split(new char[]
				{
					','
				});
				if (array3.Length != 7)
				{
					return;
				}
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array3[0]), Convert.ToInt32(array3[3]), Convert.ToInt32(array3[4]), Convert.ToInt32(array3[6]), Convert.ToInt32(array3[5]), Convert.ToInt32(array3[2]), Convert.ToInt32(array3[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				this.AddGoodsIcon(dummyGoodsDataMu, false, false);
			}
		}
	}

	private void AddGoodsIcon(GoodsData gd, bool isOcc = false, bool grayShow = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.BackSpriteName0 = backSpriteName;
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = gd.GoodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = -1;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			if (!grayShow)
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			this.itemsList.Add(icon);
			if (isOcc)
			{
				this.goodsName.Text = goodsXmlNodeByID.Title;
				this.goodsSummary.Text = goodsXmlNodeByID.Description;
			}
		}
	}

	public void GetGiftSetInfo(int status)
	{
		this.SetGiftSetStatus(status);
	}

	public void SetPickupRecallGiftSetResult(int status)
	{
		string textMsg = string.Empty;
		switch (status + 7)
		{
		case 0:
			textMsg = PlayerRecall.HintMessage_Unserviceable();
			break;
		case 3:
			textMsg = string.Format(Global.GetLang("不能重复领取"), new object[0]);
			break;
		case 4:
			textMsg = string.Format(Global.GetLang("背包空间不足"), new object[0]);
			break;
		case 5:
			textMsg = string.Format(Global.GetLang("活动已结束"), new object[0]);
			break;
		case 6:
		case 9:
		case 10:
			textMsg = string.Format(Global.GetLang("领取失败"), new object[0]);
			break;
		case 8:
			this.signBtn.gameObject.SetActive(false);
			this.signStatus.gameObject.SetActive(true);
			break;
		}
		if (status != 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
		}
	}

	public void GetRecallGiftSetRequest()
	{
		if (string.IsNullOrEmpty(Global.Data.UserID))
		{
			return;
		}
		GameInstance.Game.GetRecallGiftSet(Global.Data.UserID);
	}

	private void PickupRecallGiftSetRequest()
	{
		if (string.IsNullOrEmpty(Global.Data.UserID))
		{
			return;
		}
		GameInstance.Game.PickupRecallGiftSet(Global.Data.UserID);
	}

	public SpriteSL goodsIconCanvas;

	public TextBlock goodsName;

	public TextBlock goodsSummary;

	public new UILabel startTime;

	public new UILabel endTime;

	public new UILabel activityInfo;

	public GButton signBtn;

	public UISprite signStatus;

	private List<GGoodIcon> itemsList = new List<GGoodIcon>();
}
