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

public class MarryBuyRingPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.NeedGoods.X = 270.0;
		this.NeedGoddsName.X = 318.0;
		this.ReduceTxt.X = 284.0;
		this.staticText.text = Global.GetLang("详细属性");
		this.RingAttr2.text = Global.GetLang("高级信物可提供更高属性");
		this.ReduceTxt.text = Global.GetLang("消耗钻石:");
		this.NeedGoods.text = Global.GetLang("需要信物:");
		this.TiShengBtn.Text = Global.GetLang("提升");
		this.RingAttr1.effectDistance = new Vector2(2f, 2f);
		this.DeafY = this.ScrollView.transform.localPosition.y;
		this.ObsCollects = this.RingList.ItemsSource;
		this.RingList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.RingList_SelectChange);
		this.InitPropsDic();
		this.InitData();
		this.TiShengBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.CanTiSheng)
			{
				Super.HintMainText(Global.GetLang("当前婚戒不能购买！"), 10, 3);
				return;
			}
			if (Global.SafeConvertToInt32(this.ReduceDiamond.text) > Global.GetRoleOwnNumByMoneyType(40))
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, new DPSelectedItemEventHandler(this.CloseThisWindow), string.Empty, string.Empty);
				return;
			}
			Super.ShowNetWaiting(string.Empty);
			GameInstance.Game.SpriteChangeRing(this.PreSelectItem.GoodsId);
		};
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseCallBack != null)
			{
				this.CloseCallBack(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
			PlayZone.GlobalPlayZone.CloseBuyRingWindow();
		};
	}

	public new void Start()
	{
		this.SetScrollPos(this.RingList.SelectedIndex, true);
	}

	private void CloseThisWindow(object sender, DPSelectedItemEventArgs args)
	{
		PlayZone.GlobalPlayZone.CloseBuyRingWindow();
	}

	private void InitData()
	{
		XElement gameResXml = Global.GetGameResXml("Config/WeddingRing.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Ring");
		int num = xelementList.Count - 1;
		string needGoodsName = string.Empty;
		int prePrice = 0;
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			RingListItem ringListItem = U3DUtils.NEW<RingListItem>();
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "GoodsID");
			ringListItem.GoodsId = xelementAttributeInt;
			ringListItem.ID = Global.GetXElementAttributeInt(xelement, "ID");
			ringListItem.ReduceGoldNum = Global.GetXElementAttributeInt(xelement, "NeedZuanShi");
			ringListItem.RingNameVal = Global.GetXElementAttributeStr(xelement, "Name");
			ringListItem.RingName_Icon = Global.GetXElementAttributeStr(xelement, "RingIntro");
			ringListItem.NeedGoodsId = Global.GetXElementAttributeInt(xelement, "NeedRing");
			ringListItem.NeedGoodsName = needGoodsName;
			ringListItem.PrePrice = prePrice;
			needGoodsName = ringListItem.RingNameVal;
			prePrice = ringListItem.ReduceGoldNum;
			this.SetGoodIcon(ringListItem.GoodsId, ringListItem.gameObject);
			ringListItem.gameObject.AddComponent<UIDragPanelContents>();
			this.RingsDic.Add(ringListItem.GoodsId, ringListItem);
			if (ringListItem.GoodsId == Global.Data.MarryData.nRingID)
			{
				num = i;
			}
			if (i <= num)
			{
				ringListItem.State = 2;
			}
			else if (i == num + 1)
			{
				ringListItem.State = 1;
			}
			else
			{
				ringListItem.State = 0;
			}
			this.ObsCollects.AddNoUpdate(ringListItem.gameObject);
			this.ObsCollects.DelayUpdate();
			UIPanel component = ringListItem.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
		if (num + 1 < this.RingList.Count())
		{
			num++;
		}
		this.RingList.SelectedIndex = num;
	}

	private void RefreshRightList()
	{
		if (this.ObsCollects != null)
		{
			int num = this.ObsCollects.Count - 1;
			for (int i = 0; i < this.ObsCollects.Count; i++)
			{
				RingListItem component = this.ObsCollects.GetAt(i).GetComponent<RingListItem>();
				if (component.GoodsId == Global.Data.MarryData.nRingID)
				{
					num = i;
				}
				if (i <= num)
				{
					component.State = 2;
				}
				else if (i == num + 1)
				{
					component.State = 1;
				}
				else
				{
					component.State = 0;
				}
			}
		}
	}

	private void SetGoodIcon(int GoodsID, GameObject Target)
	{
		GoodsData fakeEquipGoodsData = Global.GetFakeEquipGoodsData(GoodsID, 0, 0);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(GoodsID);
		int categoriy = goodsXmlNodeByID.Categoriy;
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.GoodsID = GoodsID;
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.ItemCategory = categoriy;
		ggoodIcon.isAutoSize = false;
		ggoodIcon.BackgroundSprite0.gameObject.SetActive(false);
		ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		ggoodIcon.Tip = Global.GetGoodsNameByID(GoodsID, false);
		bool canUse = Global.CanUseGoods(GoodsID, false, true);
		Super.InitGoodsGIcon(ggoodIcon, fakeEquipGoodsData, canUse, IconTextTypes.Qianghua);
		U3DUtils.AddChild(Target, ggoodIcon.gameObject, true);
		ggoodIcon.transform.localPosition = new Vector3(-84f, 0f, -0.3f);
		BoxCollider component = ggoodIcon.GetComponent<BoxCollider>();
		if (component != null)
		{
			Object.Destroy(component);
		}
	}

	private void RingList_SelectChange(object sender, EventArgs e)
	{
		RingListItem ringListItem = U3DUtils.AS<RingListItem>(this.RingList.SelectedItem);
		ringListItem.SetSelected(this.RingItemKuang);
		this.PreSelectItem = ringListItem;
		this.SetSelectRing();
	}

	private void SetScrollPos(int selectIndex, bool IsFirst = false)
	{
		int num = selectIndex + 1;
		float y = this.ScrollView.transform.localPosition.y;
		int num2 = (int)(y - this.DeafY) / 100;
		int num3 = (int)(y - this.DeafY) % 100;
		int num4 = (num3 != 0) ? (100 - num3) : 0;
		num2 += ((num3 <= 0) ? 0 : 1);
		int num5 = (457 - num4) / 100;
		int num6 = (457 - num4) % 100;
		int num7 = num2 + num5;
		if (!IsFirst)
		{
			if (num <= num2)
			{
				this.ScrollView.MoveRelativeEx(new Vector3(0f, (float)((num2 - num + 1) * -100), 0f));
			}
			else if (num > num7)
			{
				this.ScrollView.MoveRelativeEx(new Vector3(0f, (float)((num - num7) * 100), 0f));
			}
		}
		else if (num <= num2)
		{
			this.ScrollView.MoveRelative(new Vector3(0f, (float)((num2 - num + 1) * -100), 0f));
		}
		else if (num > num7)
		{
			this.ScrollView.MoveRelative(new Vector3(0f, (float)((num - num7) * 100), 0f));
		}
	}

	private void SetSelectRing()
	{
		if (this.PreSelectItem == null)
		{
			return;
		}
		if (this.RingsDic.ContainsKey(Global.Data.MarryData.nRingID))
		{
			RingListItem ringListItem = this.RingsDic[Global.Data.MarryData.nRingID];
			this.RefreshRightList();
			this.SetRingModal();
			this.RingTitle.spriteName = this.PreSelectItem.RingName_Icon;
			this.RingTitle.MakePixelPerfect();
			this.RingTitle.transform.localPosition = new Vector3(this.RingTitle.transform.localPosition.x, this.RingTitle.transform.localPosition.y, -0.35f);
			if (this.PreSelectItem.ID == ringListItem.ID)
			{
				this.RingAttr1.text = Global.GetLang("正佩戴此物");
				NGUITools.SetActive(this.RingAttr2, false);
				this.RingAttr1.color = NGUITools.ParseColor("fac60d", 0);
				this.TiShengObj.SetActive(false);
				this.SetRingProps(false);
			}
			else if (this.PreSelectItem.ID < ringListItem.ID)
			{
				this.RingAttr1.text = string.Empty;
				NGUITools.SetActive(this.RingAttr2, false);
				this.TiShengObj.SetActive(false);
				this.SetRingProps(false);
			}
			else
			{
				this.RingAttr1.text = Global.GetLang("尚未获得此信物");
				NGUITools.SetActive(this.RingAttr2, true);
				this.RingAttr1.color = NGUITools.ParseColor("808081", 0);
				this.RingAttr2.color = NGUITools.ParseColor("808081", 0);
				this.TiShengObj.SetActive(true);
				this.ReduceDiamond.text = (this.PreSelectItem.ReduceGoldNum - this.PreSelectItem.PrePrice).ToString();
				this.NeedGoddsName.text = this.PreSelectItem.NeedGoodsName;
				if (this.PreSelectItem.State == 1)
				{
					this.CanTiSheng = true;
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.PreSelectItem.NeedGoodsId);
					this.NeedGoddsName.textColor = Global.ParseStringColorToUint("#" + goodsXmlNodeByID.GoodsColor);
				}
				else
				{
					this.CanTiSheng = false;
					this.NeedGoddsName.textColor = Global.ParseStringColorToUint("#ff0000");
				}
				this.SetRingProps(true);
			}
			return;
		}
	}

	public void NotifyBuySuccess()
	{
		this.BuySuccess.gameObject.SetActive(true);
		this.PlayStart(this.BuySuccess, new ActiveAnimation.OnFinished(this.PlayFinished));
		if (this.RingList.Count() > this.RingList.SelectedIndex + 1)
		{
			this.RingList.SelectedIndex = this.RingList.SelectedIndex + 1;
			this.SetScrollPos(this.RingList.SelectedIndex, false);
		}
		else
		{
			this.SetSelectRing();
		}
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

	private void SetRingModal()
	{
		if (this.RingModal != null)
		{
			Object.Destroy(this.RingModal.gameObject);
		}
		this.RingModal = null;
		if (null == this.RingModal)
		{
			this.RingModal = U3DUtils.NEW<Modal3DShow>();
			U3DUtils.AddChild(base.gameObject, this.RingModal.gameObject, false);
			Transform transform = this.RingModal.transform;
			transform.parent = base.transform;
			transform.localPosition = new Vector3(45f, -14f, -0.8f);
			transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			UIHelper.SetModalPosZ(this.RingModal.transform);
			int goodsId = this.PreSelectItem.GoodsId;
			if (this.wingsResLoader != null)
			{
				this.wingsResLoader.Stop();
			}
			this.wingsResLoader = UIHelper.LoadGoodsRes(this.RingModal, goodsId, 1f, 0.005f, 0, "UIModel", false);
		}
	}

	private void PlayStart(Animation anim, ActiveAnimation.OnFinished onFinished)
	{
		ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, 1);
		if (activeAnimation == null)
		{
			return;
		}
		activeAnimation.onFinished = onFinished;
	}

	private void PlayFinished(ActiveAnimation anim)
	{
		anim.gameObject.SetActive(false);
	}

	private void SetRingProps(bool showAddVal)
	{
		if (showAddVal)
		{
			this.GetAttribDataList(this.PreSelectItem.GoodsId, true);
			this.GetAttribDataList(Global.Data.MarryData.nRingID, false);
		}
		else
		{
			this.GetAttribDataList(this.PreSelectItem.GoodsId, true);
		}
		this.InitPropsVal(showAddVal);
	}

	private void InitPropsDic()
	{
		for (int i = 0; i < this.pos.Length; i++)
		{
			this.PropsDic.Add(this.pos[i], string.Empty);
			this.PrePropsDic.Add(this.pos[i], string.Empty);
		}
	}

	private void InitPropsVal(bool ShowDuiVal)
	{
		this.wgongText.Text = string.Format("{{c39550}}" + this.names[0] + "{{-}} {0}", this.PropsDic[this.pos[0]]);
		this.mgongText.Text = string.Format("{{c39550}}" + this.names[1] + "{{-}} {0}", this.PropsDic[this.pos[1]]);
		this.wfangText.Text = string.Format("{{c39550}}" + this.names[2] + "{{-}} {0}", this.PropsDic[this.pos[2]]);
		this.mfangText.Text = string.Format("{{c39550}}" + this.names[3] + "{{-}} {0}", this.PropsDic[this.pos[3]]);
		this.hitvText.Text = string.Format("{{c39550}}" + this.names[4] + "{{-}} {0}", this.PropsDic[this.pos[4]]);
		this.dodgeText.Text = string.Format("{{c39550}}" + this.names[5] + "{{-}} {0}", this.PropsDic[this.pos[5]]);
		this.shengmingText.Text = string.Format("{{c39550}}" + this.names[6] + "{{-}} {0}", this.PropsDic[this.pos[6]]);
		if (ShowDuiVal)
		{
			this.duibiPanel.SetActive(true);
			this.SetDuiBiInfo();
		}
		else
		{
			this.duibiPanel.SetActive(false);
		}
	}

	private void SetDuiBiInfo()
	{
		this.bwgongText.Text = this.GetDuiBiVal(0);
		this.bmgongText.Text = this.GetDuiBiVal(1);
		this.bwfangText.Text = this.GetDuiBiVal(2);
		this.bmfangText.Text = this.GetDuiBiVal(3);
		this.bhitvText.Text = this.GetDuiBiVal(4);
		this.bdodgeText.Text = this.GetDuiBiVal(5);
		this.bshengmingText.Text = this.GetDuiBiVal(6);
	}

	private string GetDuiBiVal(int index)
	{
		if (this.PrePropsDic[this.pos[index]].IndexOf("%") == -1)
		{
			return (Global.SafeConvertToInt32(this.PropsDic[this.pos[index]]) - Global.SafeConvertToInt32(this.PrePropsDic[this.pos[index]])).ToString();
		}
		return Global.SafeConvertToInt32(this.PropsDic[this.pos[index]].Replace("%", string.Empty)) - Global.SafeConvertToInt32(this.PrePropsDic[this.pos[index]].Replace("%", string.Empty)) + "%";
	}

	private void GetAttribDataList(int ringId, bool IsCurrVal = true)
	{
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(ringId);
		for (int i = 0; i < goodsEquipPropsDoubleList.Length; i++)
		{
			if (goodsEquipPropsDoubleList[i] > 0.0)
			{
				string text = ExtPropIndexes.ExtPropIndexChineseNames[i];
				double num = Global.SafeConvertToDouble(ConfigSystemParam.GetSystemParamByName("GoodWillXiShu", true));
				double num2 = goodsEquipPropsDoubleList[i] * ((double)(1 + ((int)Global.Data.MarryData.byGoodwilllevel - 1) * 2) + (double)Global.Data.MarryData.byGoodwillstar * num);
				string text2 = num2.ToString();
				if (ExtPropIndexes.ExtPropIndexPercents[i] > 0)
				{
					text2 = string.Format("+{0}%", (int)(num2 * 100.0));
				}
				if (IsCurrVal && this.PropsDic.ContainsKey(i))
				{
					this.PropsDic[i] = text2;
				}
				if (!IsCurrVal && this.PrePropsDic.ContainsKey(i))
				{
					this.PrePropsDic[i] = text2;
				}
			}
		}
	}

	public UILabel staticText;

	public ListBox RingList;

	private ObservableCollection ObsCollects;

	public UIDraggablePanel ScrollView;

	public UISprite RingTitle;

	public UILabel RingAttr1;

	public UILabel RingAttr2;

	public UILabel ReduceDiamond;

	public TextBlock ReduceTxt;

	public TextBlock NeedGoods;

	public TextBlock NeedGoddsName;

	public GButton TiShengBtn;

	public GButton CloseBtn;

	public Animation BuySuccess;

	public GameObject TiShengObj;

	public GameObject RingItemKuang;

	private Modal3DShow RingModal;

	private bool CanTiSheng;

	public TextBlock wgongText;

	public TextBlock mgongText;

	public TextBlock wfangText;

	public TextBlock mfangText;

	public TextBlock hitvText;

	public TextBlock dodgeText;

	public TextBlock shengmingText;

	public GameObject duibiPanel;

	public TextBlock bwgongText;

	public TextBlock bmgongText;

	public TextBlock bwfangText;

	public TextBlock bmfangText;

	public TextBlock bhitvText;

	public TextBlock bdodgeText;

	public TextBlock bshengmingText;

	private string[] names = new string[]
	{
		Global.GetLang("物理攻击"),
		Global.GetLang("魔法攻击"),
		Global.GetLang("物理防御"),
		Global.GetLang("魔法防御"),
		Global.GetLang("命        中"),
		Global.GetLang("闪        避"),
		Global.GetLang("生命上限")
	};

	private int[] pos = new int[]
	{
		7,
		9,
		3,
		5,
		18,
		19,
		13
	};

	private Dictionary<int, string> PropsDic = new Dictionary<int, string>();

	private Dictionary<int, string> PrePropsDic = new Dictionary<int, string>();

	public DPSelectedItemEventHandler CloseCallBack;

	private Dictionary<int, RingListItem> RingsDic = new Dictionary<int, RingListItem>();

	private float DeafY;

	private RingListItem PreSelectItem;

	private WingsResLoader wingsResLoader;
}
