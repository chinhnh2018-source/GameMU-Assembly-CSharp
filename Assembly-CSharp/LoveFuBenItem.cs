using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class LoveFuBenItem : UserControl
{
	public XElement XmlItem
	{
		set
		{
			this.MapCode = Global.GetXElementAttributeInt(value, "MapCode");
			this.CopyId = Global.GetXElementAttributeInt(value, "ID");
			this.CopyName = Global.GetXElementAttributeStr(value, "CopyName");
			this.MaxEnterNum = Global.GetXElementAttributeInt(value, "EnterNumber");
			this.MaxFinishNum = Global.GetXElementAttributeInt(value, "FinishNumber");
		}
	}

	protected override void InitializeComponent()
	{
		this.FuBenTitle.text = string.Empty;
		this.LeftTime.text = string.Format(Global.GetLang("今日剩余次数：{0}/{1}"), 0, 1);
		this.FuBenTitle.textColor = Global.ParseStringColorToUint("#fac60d");
		this.HelpBtn.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnHelpBtnClick);
		this.LeftHead.URL = string.Format("NetImages/GameRes/Images/Plate/lovefuben_head{0}.png", Global.Data.roleData.OccupationList[0]);
		this.RightHead.URL = string.Format("NetImages/GameRes/Images/Plate/lovefuben_head{0}.png", Global.Data.MarryOtherData.occupationId);
	}

	private void OnHelpBtnClick(GameObject go)
	{
		PlayZone.GlobalPlayZone.OpenLoveFubenHelpPart();
	}

	public void SetMyState(int state)
	{
		if (state == -1)
		{
			this.LeftHead.ToGrayBitmap = true;
			this.LeftReady.spriteName = "unready";
		}
		else if (state == 0)
		{
			this.LeftHead.ToGrayBitmap = false;
			this.LeftReady.spriteName = "unready";
		}
		else if (state == 1)
		{
			this.LeftHead.ToGrayBitmap = false;
			this.LeftReady.spriteName = "ready";
		}
		else if (state == 2)
		{
			this.LeftHead.ToGrayBitmap = false;
			this.LeftReady.spriteName = "ready";
		}
		this.LeftReady.transform.localPosition = new Vector3(-160f, -130f, -1f);
		this.LeftReady.MakePixelPerfect();
	}

	public void SetOtherState(int state)
	{
		if (state == -1)
		{
			this.RightHead.ToGrayBitmap = true;
			this.RightReady.spriteName = "unready";
		}
		if (state == 0)
		{
			this.RightHead.ToGrayBitmap = false;
			this.RightReady.spriteName = "unready";
		}
		else if (state == 1)
		{
			this.RightHead.ToGrayBitmap = false;
			this.RightReady.spriteName = "ready";
		}
		else if (state == 2)
		{
			this.RightHead.ToGrayBitmap = false;
			this.RightReady.spriteName = "ready";
		}
		this.RightReady.MakePixelPerfect();
	}

	public void SetLeftNum(int num)
	{
		this.CurrFinishNum = num;
		this.LeftTime.text = string.Format(Global.GetLang("今日剩余次数：{0}/{1}"), num, this.MaxFinishNum);
	}

	public void SetTitle()
	{
		this.FuBenTitle.text = Global.GetLang(this.CopyName);
	}

	public bool CanReady()
	{
		if (this.CurrFinishNum < this.MaxFinishNum || this.MaxFinishNum == -1)
		{
			return true;
		}
		Super.HintMainText(Global.GetLang("今日完成次数已满"), 10, 3);
		return false;
	}

	public TextBlock FuBenTitle;

	public UISprite HelpBtn;

	public UILabel LeftTime;

	public ShowNetImage LeftHead;

	public ShowNetImage RightHead;

	public UISprite LeftReady;

	public UISprite RightReady;

	[HideInInspector]
	public int CopyId;

	[HideInInspector]
	public int MaxEnterNum;

	[HideInInspector]
	public int MapCode;

	[HideInInspector]
	public int MaxFinishNum;

	private string CopyName = string.Empty;

	private int CurrFinishNum;
}
