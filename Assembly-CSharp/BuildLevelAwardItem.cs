using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class BuildLevelAwardItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this._ItemListBox)
		{
			this._ItemCollection = this._ItemListBox.ItemsSource;
		}
		if (this._BtnLingQv.Text != null)
		{
			this._BtnLingQv.Text = Global.GetLang("领取");
		}
		if (null != this._BtnLingQv)
		{
			this._BtnLingQv.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this._BtnLingQv.isEnabled && 0 < this.m_AwardId)
				{
					GameInstance.Game.SendGetLevelAward(this.m_AwardId);
					Super.ShowNetWaiting(string.Empty);
				}
			};
		}
		this._ItemListBox.MouseLeftButtonDownEx = new MouseLeftButtonUpEventHandler(this.BtnClick);
	}

	public int AwardId
	{
		get
		{
			return this.m_AwardId;
		}
		set
		{
			this.m_AwardId = value;
		}
	}

	public BuildLevelAwardItem.BuildLevelAwardState GiftGainState
	{
		get
		{
			return this.m_State;
		}
		set
		{
			this.m_State = value;
			switch (this.m_State)
			{
			case BuildLevelAwardItem.BuildLevelAwardState.CanGain:
				this._BtnLingQv.isEnabled = true;
				this._BtnLingQv.gameObject.SetActive(true);
				this._StateGObj.SetActive(false);
				break;
			case BuildLevelAwardItem.BuildLevelAwardState.Gained:
				this._BtnLingQv.gameObject.SetActive(false);
				this._StateGObj.SetActive(true);
				break;
			case BuildLevelAwardItem.BuildLevelAwardState.CanNotGain:
				this._BtnLingQv.isEnabled = false;
				this._BtnLingQv.gameObject.SetActive(true);
				this._StateGObj.SetActive(false);
				break;
			}
		}
	}

	public int TiaoJianLev
	{
		get
		{
			return this.m_TiaoJianLev;
		}
		set
		{
			this.m_TiaoJianLev = value;
		}
	}

	public string TiaoJian
	{
		set
		{
			if (value == null || value == string.Empty)
			{
				this._TiaojianLabel.Visibility = false;
				this._TiaojianLabel.text = string.Empty;
			}
			else
			{
				this._TiaojianLabel.Visibility = true;
				this._TiaojianLabel.text = value;
			}
		}
	}

	public int TiaoJianValue
	{
		get
		{
			return this.TiaoJianValueInstance;
		}
		set
		{
			this.TiaoJianValueInstance = value;
		}
	}

	public string GoodsList
	{
		get
		{
			return this._GoodsList;
		}
		set
		{
			this._GoodsList = value;
		}
	}

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

	public void LoadGoodsList(string[] str_Award, string str_Tiaojian, int idx, UIDraggablePanel dragpanel)
	{
		string[] array = new string[7];
		byte b = 0;
		byte b2 = 0;
		while ((int)b2 < str_Award.Length)
		{
			array[(int)b] = str_Award[(int)b2];
			byte b3 = b;
			b = b3 + 1;
			if (b3 == 6)
			{
				this.initGood(array, (int)b, dragpanel);
				b = 0;
			}
			b2 += 1;
		}
		this._TiaojianLabel.text = "{fdf7dd}" + Global.GetLang("总等级达到") + str_Tiaojian + "{-}";
	}

	private void initGood(string[] str_Award, int idx, UIDraggablePanel dragpanel)
	{
		int num = Convert.ToInt32(str_Award[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			GoodsData goodsData = new GoodsData();
			goodsData.GoodsID = num;
			goodsData.GCount = Convert.ToInt32(str_Award[1]);
			goodsData.Binding = Convert.ToInt32(str_Award[2]);
			goodsData.Forge_level = Convert.ToInt32(str_Award[3]);
			goodsData.AppendPropLev = Convert.ToInt32(str_Award[4]);
			goodsData.Lucky = Convert.ToInt32(str_Award[5]);
			goodsData.ExcellenceInfo = Convert.ToInt32(str_Award[6]);
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.ItemCode = num;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this._ItemListBox.gameObject, ggoodIcon.gameObject, true);
			this._ItemCollection.Add(ggoodIcon);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, Global.CanUseGoods(num, false, true), IconTextTypes.Qianghua);
			BoxCollider component2 = ggoodIcon.transform.GetComponent<BoxCollider>();
			ggoodIcon.transform.gameObject.AddComponent<UIDragPanelContents>();
			component2.center = new Vector3(0f, 0f, -1f);
			UIDragPanelContents component3 = ggoodIcon.GetComponent<UIDragPanelContents>();
			component3.draggablePanel = dragpanel;
		}
	}

	private void BtnClick(object sender, MouseEvent e)
	{
		GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
		if (null != ggoodIcon)
		{
			GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
			if (goodsData != null)
			{
				GTipServiceEx.SelfBagOnly = false;
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			}
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void ShowGoodsTip(object icon)
	{
	}

	public GButton _BtnLingQv;

	public ListBox _ItemListBox;

	public GTextBlockOutLine _TiaojianLabel;

	public GameObject _StateGObj;

	private List<GGoodIcon> GoodIcon = new List<GGoodIcon>();

	private int m_AwardId;

	private int m_TiaoJianLev;

	public UISprite _ItemBg;

	private ObservableCollection _ItemCollection;

	public int ID = -1;

	private int TiaoJianValueInstance;

	private BuildLevelAwardItem.BuildLevelAwardState m_State = BuildLevelAwardItem.BuildLevelAwardState.CanNotGain;

	private string _GoodsList = string.Empty;

	public enum BuildLevelAwardState
	{
		CanGain,
		Gained,
		CanNotGain
	}
}
