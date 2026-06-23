using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class RingListItem : UserControl
{
	public int ID
	{
		get
		{
			return this.UID;
		}
		set
		{
			this.UID = value;
		}
	}

	public string RingNameVal
	{
		get
		{
			return this.RingName.text;
		}
		set
		{
			this.RingName.text = value;
		}
	}

	public int GoodsId
	{
		get
		{
			return this.GoodsID;
		}
		set
		{
			this.GoodsID = value;
		}
	}

	public int State
	{
		get
		{
			return this.CurrState;
		}
		set
		{
			this.CurrState = value;
			if (value == 0)
			{
				this.RingState.text = Global.GetLang("未提升");
				this.RingState.textColor = Global.ParseStringColorToUint("#808081");
				this.RingName.textColor = Global.ParseStringColorToUint("#808081");
				this.RingBtn.normalSprite = "ringbak_diable";
				this.RingBtn.Refresh();
			}
			else if (value == 1)
			{
				this.RingState.text = Global.GetLang("可提升");
				this.RingState.textColor = Global.ParseStringColorToUint("#17e43e");
				this.SetNameColor();
				this.RingBtn.normalSprite = "ringbak_normal";
				this.RingBtn.Refresh();
			}
			else if (value == 2)
			{
				this.RingState.text = string.Empty;
				this.SetNameColor();
				this.RingBtn.normalSprite = "ringbak_normal";
				this.RingBtn.Refresh();
			}
		}
	}

	private void SetNameColor()
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.GoodsId);
		this.RingName.textColor = Global.ParseStringColorToUint("#" + goodsXmlNodeByID.GoodsColor);
	}

	protected override void InitializeComponent()
	{
	}

	public void SetGoodIcon(int GoodsID)
	{
		if (this.Icon == null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			this.Icon = U3DUtils.NEW<GGoodIcon>();
			this.Icon.GoodsID = GoodsID;
			this.Icon.Width = 78.0;
			this.Icon.Height = 78.0;
			this.Icon.ItemCategory = categoriy;
			this.Icon.isAutoSize = false;
			this.Icon.BackSpriteName0 = string.Empty;
			this.Icon.BackgroundSprite0.MakePixelPerfect();
			this.Icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			this.Icon.Tip = Global.GetGoodsNameByID(GoodsID, false);
			this.Icon.SecondText.Text = string.Format("{0}", Global.GetTotalGoodsCountByID(GoodsID));
			U3DUtils.AddChild(this.RingIconObj, this.Icon.gameObject, true);
			this.Icon.transform.localPosition = new Vector3(0f, 0f, -0.3f);
			this.Icon.transform.localScale = new Vector3(0.72f, 0.72f, 1f);
			BoxCollider component = this.Icon.GetComponent<BoxCollider>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
	}

	public void SetSelected(GameObject child)
	{
		U3DUtils.AddChild(this.RingBtn.gameObject, child, true);
		child.transform.localPosition = new Vector3(0f, 0f, -0.1f);
	}

	public GButton RingBtn;

	public TextBlock RingState;

	public TextBlock RingName;

	public GameObject RingIconObj;

	private GGoodIcon Icon;

	private int GoodsID;

	public int NeedGoodsId;

	public string RingName_Icon;

	public string NeedGoodsName;

	public int PrePrice;

	private int CurrState;

	private int UID;

	public int ReduceGoldNum;
}
