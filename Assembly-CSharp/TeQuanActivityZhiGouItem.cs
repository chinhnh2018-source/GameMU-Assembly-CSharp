using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class TeQuanActivityZhiGouItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		try
		{
			this._Title.text = Global.GetLang("超级直购");
			this._BuyBtn.Text = Global.GetLang("购买");
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			this._BuyBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.BuyNumber <= 0)
				{
					Super.HintMainText(Global.GetLang("剩余次数不足,不能购买"), 10, 3);
					return;
				}
				if (0f < Global.GetBtnCD(this._BuyBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._BuyBtn.GetInstanceID(), 0.2f);
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						ID = this.mID
					});
				}
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public GGoodIcon AddGoodIcon(GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		ggoodIcon.TipType = 1;
		ggoodIcon.ItemCode = gd.GoodsID;
		ggoodIcon.ItemObject = gd;
		ggoodIcon.BoxTypes = 0;
		ggoodIcon.TextSize = 16;
		ggoodIcon.TextShadowColor = 4278190080U;
		ggoodIcon.Tag = gd.ExcellenceInfo;
		ggoodIcon.SecondText.Text = gd.GCount.ToString();
		ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
		Super.InitGoodsGIcon(ggoodIcon, gd, true, IconTextTypes.Qianghua);
		ggoodIcon.SecondText.transform.localPosition = new Vector3(32f, -24f, -2.5f);
		ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		ggoodIcon.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, -1f);
		ggoodIcon.GetComponent<BoxCollider>().isTrigger = false;
		if (Global.GetZhuoyueAttributeCount(gd) >= 5)
		{
			ggoodIcon.TeXiao.gameObject.SetActive(true);
		}
		return ggoodIcon;
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		if (0f < Global.GetBtnCD(this._BuyBtn.GetInstanceID()))
		{
			return;
		}
		Global.AddBtnCD(this._BuyBtn.GetInstanceID(), 0.2f);
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null != ggoodIcon)
		{
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
		}
	}

	public override void Update()
	{
		base.Update();
		if (this.Refresh)
		{
			this.Refresh = false;
			this.UpdataTransPos(this._GoodsRoot);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		for (int i = 0; i < this.mMaterials.Count; i++)
		{
			if (null != this.mMaterials[i])
			{
				Object.Destroy(this.mMaterials[i]);
			}
		}
	}

	private void UpdataTransPos(Transform trans)
	{
		if (null != trans && 0 < trans.childCount)
		{
			int childCount = trans.childCount;
			if (trans.childCount == 1)
			{
				Transform child = trans.GetChild(0);
				child.transform.localPosition = new Vector3((float)(this.Space * 2), 0f, 0f);
			}
			else if (childCount == 2)
			{
				Transform child2 = trans.GetChild(0);
				child2.transform.localPosition = new Vector3((float)(this.Space / 2 + this.Space), 0f, 0f);
				Transform child3 = trans.GetChild(1);
				child3.transform.localPosition = new Vector3((float)(this.Space / 2 + this.Space * 2), 0f, 0f);
			}
			else if (childCount == 3)
			{
				for (int i = 0; i < childCount; i++)
				{
					Transform child4 = trans.GetChild(i);
					child4.transform.localPosition = new Vector3((float)(this.Space + i * this.Space), 0f, 0f);
				}
			}
			else
			{
				for (int j = 0; j < childCount; j++)
				{
					Transform child5 = trans.GetChild(j);
					float num = (3 <= j) ? (-(float)this.Space / 2f) : ((float)this.Space / 2f);
					child5.transform.localPosition = new Vector3((float)(this.Space + j % 3 * this.Space), num, 0f);
				}
			}
		}
	}

	public void SetData(TeQuanZhiGouVO vo)
	{
		this.mID = vo.ID;
		this.BuyNumber = vo.GouMaiCiShu.SafeToInt32(0);
		TeQuanZhiGouVO vos = IConfigbase<ConfigTeQuan>.Instance.GetZhiGouVOItemByID(this.mID);
		if (vos != null)
		{
			string rechargeItemConf = Global.GetRechargeItemCfgTypeByPlatform();
			XElement gameResXml = Global.GetGameResXml("Config/MU_ChongZhi.xml");
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Type");
			XElement xelement = xelementList.Find((XElement e) => Global.GetXElementAttributeStr(e, "TypeID") == rechargeItemConf);
			if (xelement != null)
			{
				List<XElement> xelementList2 = Global.GetXElementList(xelement, "ChongZhi");
				XElement xelement2 = xelementList2.Find((XElement e) => Global.GetXElementAttributeStr(e, "ID") == vos.ChongZhiID.ToString());
				if (xelement2 != null)
				{
					this._Price.text = Global.GetLang("临时促销价格：") + Global.GetXElementAttributeInt(xelement2, "RMB");
				}
			}
		}
		BetterList<GoodsData> wuPin = vo.WuPin;
		if (0 < wuPin.size)
		{
			for (int i = 0; i < wuPin.size; i++)
			{
				if (wuPin[i] != null)
				{
					GGoodIcon ggoodIcon = this.AddGoodIcon(wuPin[i]);
					ggoodIcon.transform.SetParent(this._GoodsRoot, false);
				}
			}
			this.UpdataTransPos(this._GoodsRoot);
		}
		this._BakImage.URL = "NetImages/GameRes/Images/SuperDirectBuy/" + vo.ZhiGouPinZhi + ".png";
		if (vo.ZhiGouPinZhi == 1)
		{
			this._BakImage.transform.localScale = new Vector3(266f, 450f, 1f);
		}
		else if (vo.ZhiGouPinZhi == 2)
		{
			this._BakImage.transform.localScale = new Vector3(266f, 450f, 1f);
		}
		else if (vo.ZhiGouPinZhi == 3)
		{
			this._BakImage.transform.localScale = new Vector3(266f, 450f, 1f);
		}
		else if (vo.ZhiGouPinZhi == 4)
		{
			this._BakImage.transform.localScale = new Vector3(266f, 450f, 1f);
		}
	}

	public GameObject[] TeXiaoRender
	{
		get
		{
			return this.mTeXiaoRender;
		}
		private set
		{
			this.mTeXiaoRender = value;
		}
	}

	public void SetTeXiao(bool bShow, Vector4 Panel)
	{
		if (bShow)
		{
			Renderer[] componentsInChildren = this._TeXiaoObj.GetComponentsInChildren<Renderer>(true);
			this.TeXiaoRender = new GameObject[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (null != componentsInChildren[i])
				{
					if ("Plane_01".Equals(componentsInChildren[i].name))
					{
						componentsInChildren[i].gameObject.SetActive(false);
					}
					else if (!string.IsNullOrEmpty(componentsInChildren[i].sharedMaterials[0].shader.name) && !componentsInChildren[i].sharedMaterials[0].shader.name.Contains("EX"))
					{
						Shader shader = Shader.Find(componentsInChildren[i].sharedMaterials[0].shader.name + "EX");
						if (null != shader)
						{
							componentsInChildren[i].sharedMaterials[0].shader = shader;
							componentsInChildren[i].sharedMaterials[0].SetVector("_Panel", Panel);
						}
						else
						{
							MUDebug.Log<string>(new string[]
							{
								componentsInChildren[i].sharedMaterials[0].shader.name + "EX" + Global.GetLang("没有找到")
							});
						}
					}
				}
			}
		}
		this._TeXiaoObj.SetActive(bShow);
	}

	public UIDraggablePanel DragPanel
	{
		set
		{
			UIDragPanelContents uidragPanelContents = this._BakImage.GetComponent<UIDragPanelContents>();
			if (null == uidragPanelContents)
			{
				uidragPanelContents = this._BakImage.gameObject.AddComponent<UIDragPanelContents>();
			}
			uidragPanelContents.draggablePanel = value;
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	public int BuyNumber
	{
		get
		{
			return this._buyNumber;
		}
		set
		{
			this._GoumaiCiShu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e42e",
				Global.GetLang("个人限购："),
				(0 < value) ? "17e42e" : "ff0000",
				value.ToString()
			});
			this._buyNumber = value;
		}
	}

	public int ID
	{
		get
		{
			return this.mID;
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private ShowNetImage _BakImage;

	[SerializeField]
	private UILabel _Title;

	[SerializeField]
	private UILabel _Price;

	[SerializeField]
	private UILabel _GoumaiCiShu;

	[SerializeField]
	private GButton _BuyBtn;

	[SerializeField]
	private Transform _GoodsRoot;

	[SerializeField]
	private GameObject _TeXiaoObj;

	public int Space = 76;

	public bool Refresh;

	private GameObject[] mTeXiaoRender;

	private List<Material> mMaterials = new List<Material>();

	private int _buyNumber;

	private int mID;
}
