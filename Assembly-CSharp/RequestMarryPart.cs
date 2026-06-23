using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class RequestMarryPart : UserControl
{
	private void InitPrefabs()
	{
		this.RequestBtn.Text = Global.GetLang("求婚");
		this.AgreeBtn.Text = Global.GetLang("我愿意");
		this.RefuseBtn.Text = Global.GetLang("拒绝");
		this.NeedReduceTxt.text = Global.GetLang("消耗：");
		this.NeedReduceTxt.textColor = Global.ParseStringColorToUint("#e3b36c");
		this.qiuhun_Sprite.MakePixelPerfect();
		this.responsed_Sprite.MakePixelPerfect();
		this.RequestBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.OtherRoleId != -1)
			{
				string systemParamByName = ConfigSystemParam.GetSystemParamByName("JieHunCost", true);
				int num = Convert.ToInt32(systemParamByName);
				if (Global.Data.roleData.UserMoney < num)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, new DPSelectedItemEventHandler(this.CloseThisWindow), string.Empty, string.Empty);
					return;
				}
				GameInstance.Game.SpriteRequestMarry(this.OtherRoleId);
				PlayZone.GlobalPlayZone.CloseRequestMarryWindow();
			}
		};
		this.AgreeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.OtherRoleId != -1)
			{
				GameInstance.Game.SpriteResponseMarry(this.OtherRoleId, 1);
				PlayZone.GlobalPlayZone.CloseRequestMarryWindow();
			}
		};
		this.RefuseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.OtherRoleId != -1)
			{
				GameInstance.Game.SpriteResponseMarry(this.OtherRoleId, 0);
				PlayZone.GlobalPlayZone.CloseRequestMarryWindow();
			}
		};
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.CloseRequestMarryWindow();
		};
		XElement gameResXml = Global.GetGameResXml("Config/WeddingRing.xml");
		if (gameResXml == null)
		{
			MUDebug.LogWarning<string>(new string[]
			{
				"Card XML IS NULL"
			});
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Ring");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			if (xelementAttributeInt == 1)
			{
				this.RingID = Global.GetXElementAttributeInt(xelement, "GoodsID");
				break;
			}
		}
	}

	private string GetRingIconName()
	{
		XElement gameResXml = Global.GetGameResXml("Config/WeddingRing.xml");
		string result = string.Empty;
		if (gameResXml == null)
		{
			MUDebug.LogWarning<string>(new string[]
			{
				"Card XML IS NULL"
			});
			return result;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Ring");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "GoodsID");
			if (xelementAttributeInt == this.RingID)
			{
				result = Global.GetXElementAttributeStr(xelement, "RingIntro");
				break;
			}
		}
		return result;
	}

	private void CloseThisWindow(object sender, DPSelectedItemEventArgs args)
	{
		PlayZone.GlobalPlayZone.CloseRequestMarryWindow();
	}

	protected override void InitializeComponent()
	{
		this.InitPrefabs();
	}

	public override void Destroy()
	{
		if (this.wingsResLoader != null)
		{
			this.wingsResLoader.Stop();
			this.wingsResLoader = null;
		}
		base.Destroy();
	}

	public void SetType(int type, int otherRoleId, string roleName)
	{
		this.OtherRoleId = otherRoleId;
		if (null == this.RingModal)
		{
			this.RingModal = U3DUtils.NEW<Modal3DShow>();
			U3DUtils.AddChild(base.gameObject, this.RingModal.gameObject, false);
			Transform transform = this.RingModal.transform;
			transform.parent = this.RingObj.transform;
			transform.localPosition = new Vector3(0f, 0f, -0.8f);
			transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			UIHelper.SetModalPosZ(this.RingModal.transform);
			int num;
			if (GameInstance.Game.CurrentSession != null && GameInstance.Game.CurrentSession.MarriageData != null && GameInstance.Game.CurrentSession.MarriageData.nRingID != -1)
			{
				num = GameInstance.Game.CurrentSession.MarriageData.nRingID;
				this.RingID = num;
			}
			else
			{
				num = this.RingID;
			}
			if (this.wingsResLoader != null)
			{
				this.wingsResLoader.Stop();
			}
			this.wingsResLoader = UIHelper.LoadGoodsRes(this.RingModal, num, 1f, 0.005f, 0, "UIModel", false);
			this.RingModal.ClickCallBack = new DPSelectedItemEventHandler(this.MouseLeftButtonUp);
			BoxCollider component = this.RingModal.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.size = new Vector3(160f, 120f, 1f);
			}
			this.RingIconName.spriteName = this.GetRingIconName();
			this.RingIconName.MakePixelPerfect();
		}
		if (type == 0)
		{
			this.RequestObj.SetActive(true);
			this.ResponseObj.SetActive(false);
			this.CloseBtn.gameObject.SetActive(true);
			this.RequestRoleName.text = roleName;
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("JieHunCost", true);
			this.NeedDiamond.text = systemParamByName;
		}
		else
		{
			this.ResponseObj.SetActive(true);
			this.RequestObj.SetActive(false);
			this.CloseBtn.gameObject.SetActive(false);
			this.ResponseRoleName.text = roleName;
			string systemParamByName2 = ConfigSystemParam.GetSystemParamByName("MarriageTipsTime", true);
			int num2 = Global.SafeConvertToInt32(systemParamByName2);
			this.EndTicks = Global.GetCorrectLocalTime() + (long)(num2 * 1000);
			base.InvokeRepeating("Timer_Tick", 0f, 0.12f);
		}
	}

	private void MouseLeftButtonUp(object sender, DPSelectedItemEventArgs args)
	{
		if (this.RingModal == null)
		{
			return;
		}
		string text = this.RingModal._Target.name.Replace("UI_Goods_", string.Empty);
		if (string.Empty == text)
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
			GTipServiceEx.SelfBagOnly = false;
			if (goodsXmlNodeByID != null)
			{
				GoodsData goodsData = new GoodsData();
				goodsData.GoodsID = num;
				if ((int)Global.Data.MarryData.byGoodwilllevel <= 0)
				{
					Global.Data.MarryData.byGoodwilllevel = 1;
				}
				if ((int)Global.Data.MarryData.byGoodwillstar < 0)
				{
					Global.Data.MarryData.byGoodwillstar = 0;
				}
				goodsData.Forge_level = (int)Global.Data.MarryData.byGoodwilllevel;
				goodsData.AppendPropLev = (int)Global.Data.MarryData.byGoodwillstar;
				GTipServiceEx.ShowTip(null, TipTypes.GoodsText, GoodsOwnerTypes.None, goodsData);
			}
		}
	}

	private void Timer_Tick()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime < this.EndTicks)
		{
			this.RefuseBtn.Text = string.Format(Global.GetLang("拒绝 {0}秒"), Mathf.CeilToInt((float)(this.EndTicks - correctLocalTime) / 1000f));
		}
		else if (this.EndTicks != 0L)
		{
			this.EndTicks = 0L;
			this.RefuseBtn.Text = string.Format(Global.GetLang("拒绝 {0}秒"), 0);
			if (this.OtherRoleId != -1)
			{
				PlayZone.GlobalPlayZone.CloseRequestMarryWindow();
			}
		}
	}

	public GButton RequestBtn;

	public GButton AgreeBtn;

	public GButton RefuseBtn;

	public GButton CloseBtn;

	public UILabel RequestRoleName;

	public UILabel ResponseRoleName;

	public UILabel NeedDiamond;

	public TextBlock NeedReduceTxt;

	public UISprite RingIconName;

	public GameObject RequestObj;

	public GameObject ResponseObj;

	public GameObject RingObj;

	private Modal3DShow RingModal;

	private int OtherRoleId = -1;

	private int RingID = -1;

	public UISprite qiuhun_Sprite;

	public UISprite responsed_Sprite;

	private WingsResLoader wingsResLoader;

	private long EndTicks;
}
