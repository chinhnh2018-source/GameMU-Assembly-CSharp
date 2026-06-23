using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ShiPinYuLanItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		Vector3 localPosition = this._ListBox.transform.localPosition;
		localPosition.x = -198f;
		this._ListBox.transform.localPosition = localPosition;
	}

	private void DelectObjPabel(Component o)
	{
		if (null != o)
		{
			UIPanel component = o.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	private void DelectObjPabel(GameObject o)
	{
		if (null != o)
		{
			UIPanel component = o.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	private void InitPrefabText()
	{
		if (this._AttLabel != null)
		{
			for (int i = 0; i < this._AttLabel.Length; i++)
			{
				if (null != this._AttLabel[i])
				{
					this._AttLabel[i].text = string.Empty;
				}
			}
		}
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.m_Obc = this._ListBox.ItemsSource;
	}

	private IEnumerator InitIcon(XElement xml, int[] GoodsIDs, bool[] HaveActive)
	{
		if (xml != null)
		{
			bool Satisfy = true;
			int i = 0;
			int max = GoodsIDs.Length;
			while (i < max)
			{
				GGoodIcon Icon = this.initGood(Global.GetEmptyGoodsData(GoodsIDs[i], 0, 1, 0, 1, 0, 0, 0, 0));
				if (!HaveActive[i])
				{
					Icon.GoodImg.ToGrayBitmap = true;
					Icon.NoUseSpriteVisible = false;
					Icon.TeXiao.gameObject.SetActive(false);
					Satisfy = false;
				}
				this.m_Obc.AddNoUpdate(Icon);
				this.DelectObjPabel(Icon);
				i++;
			}
			yield return null;
			string GroupProperty = xml.GetXElementAttrStr("GroupProperty");
			if (!string.IsNullOrEmpty(GroupProperty))
			{
				string[] Prop = GroupProperty.Split(new char[]
				{
					'|'
				});
				if (Prop != null && 0 < Prop.Length)
				{
					int j = 0;
					int max2 = Prop.Length;
					while (j < max2)
					{
						if (!string.IsNullOrEmpty(Prop[j]))
						{
							string[] propProp = Prop[j].Split(new char[]
							{
								','
							});
							if (propProp.Length == 2)
							{
								string srtPropName = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(propProp[0], true);
								string strAtt = string.Empty;
								if (!ConfigExtPropIndexes.GetPercentByWord(propProp[0]))
								{
									strAtt = "+ " + propProp[1];
								}
								else
								{
									strAtt = "+ " + float.Parse(propProp[1]) * 100f + "%";
								}
								if (j < this._Att.Length)
								{
									this._Att[j].SetActive(true);
								}
								if (j < this._AttLabel.Length && null != this._AttLabel[j])
								{
									if (Satisfy)
									{
										this._AttLabel[j].text = Global.GetColorStringForNGUIText(new object[]
										{
											"80ff00",
											srtPropName + strAtt
										});
									}
									else
									{
										this._AttLabel[j].text = Global.GetColorStringForNGUIText(new object[]
										{
											"808080",
											srtPropName + strAtt
										});
									}
								}
							}
						}
						j++;
					}
				}
			}
		}
		yield break;
	}

	public void RefreshUI(XElement xml, int[] GoodsIDs, bool[] Active)
	{
		base.StartCoroutine<bool>(this.InitIcon(xml, GoodsIDs, Active));
	}

	private GGoodIcon initGood(GoodsData data)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			ggoodIcon.ItemObject = data;
			ggoodIcon.ItemCode = goodsXmlNodeByID.ID;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			Super.InitGoodsGIcon(ggoodIcon, data, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
			BoxCollider component = ggoodIcon.GetComponent<BoxCollider>();
			if (null != component)
			{
				Vector3 center = component.center;
				center.z -= 2f;
				component.center = center;
			}
		}
		return ggoodIcon;
	}

	public bool DelectPanel
	{
		set
		{
			if (value)
			{
				this.DelectObjPabel(base.transform);
			}
		}
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			if (null == this.m_UIDragPanelContents)
			{
				this.m_UIDragPanelContents = base.GetComponent<UIDragPanelContents>();
			}
			if (null == this.m_UIDragPanelContents)
			{
				this.m_UIDragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
			}
			this.m_UIDragPanelContents.draggablePanel = value;
			BoxCollider boxCollider = base.GetComponent<BoxCollider>();
			if (null == boxCollider)
			{
				boxCollider = base.gameObject.AddComponent<BoxCollider>();
			}
			boxCollider.center = new Vector3(0f, 0f, -1f);
			boxCollider.size = new Vector3(500f, 136f, 0f);
		}
	}

	public GameObject[] _Att;

	public UILabel[] _AttLabel;

	public ListBox _ListBox;

	private UIDragPanelContents m_UIDragPanelContents;

	private ObservableCollection m_Obc;
}
