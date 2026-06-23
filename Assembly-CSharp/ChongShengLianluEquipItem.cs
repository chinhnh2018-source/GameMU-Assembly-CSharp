using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ChongShengLianluEquipItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_LabTiShi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("点击放入")
		});
		this.m_ListVec[0] = this.m_ListGmaeChongSheng[0].transform.localPosition;
		Vector3[] listVec = this.m_ListVec;
		int num = 0;
		listVec[num].x = listVec[num].x + 40f;
		this.m_ListVec[1] = this.m_ListGmaeChongSheng[1].transform.localPosition;
		Vector3[] listVec2 = this.m_ListVec;
		int num2 = 1;
		listVec2[num2].x = listVec2[num2].x + 40f;
		this.m_ListVec[2] = this.m_ListGmaeChongSheng[2].transform.localPosition;
		Vector3[] listVec3 = this.m_ListVec;
		int num3 = 2;
		listVec3[num3].x = listVec3[num3].x + 40f;
	}

	public void SetData(GoodsData gd)
	{
		if (gd == null)
		{
			return;
		}
		if (this.m_GameIcion.GetComponentInChildren<GGoodIcon>() != null)
		{
			Object.Destroy(this.m_GameIcion.GetComponentInChildren<GGoodIcon>().gameObject);
		}
		GGoodIcon icon = Global.CreateGoodsIcon(gd, false, true);
		icon.transform.SetParent(this.m_GameIcion.transform, false);
		if (icon.GetComponent<UIPanel>() != null)
		{
			Object.Destroy(icon.GetComponent<UIPanel>());
		}
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.ChongShengLianlu, gd);
		};
		icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.IDType == 5)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ZhuZhuangBei = gd
				});
			}
		};
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		this.m_LabName.text = UIHelper.FormatGoodsName(gd, false, true, true);
		this.m_GoodsData = gd;
		if (goodsXmlNodeByID.Categoriy == 37)
		{
			this.m_SpBackXuanCai.gameObject.SetActive(true);
			this.m_ListGmaeChongSheng[0].transform.localPosition = this.m_ListVec[0];
			this.m_ListGmaeChongSheng[1].transform.localPosition = this.m_ListVec[1];
			this.m_ListGmaeChongSheng[2].transform.localPosition = this.m_ListVec[2];
		}
		else
		{
			this.m_SpBackXuanCai.gameObject.SetActive(false);
		}
		string[] array = gd.Props.Split(new char[]
		{
			'|'
		});
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				int num2 = array[i].Split(new char[]
				{
					'_'
				})[0].SafeToInt32(0);
				if (num2 == -1)
				{
					num = array[i].Split(new char[]
					{
						'_'
					})[2].SafeToInt32(0);
				}
			}
		}
		if (num > 0)
		{
			this.m_ImgXuanCai.gameObject.SetActive(true);
			this.m_ImgXuanCai.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png", num);
		}
		else
		{
			this.m_ImgXuanCai.gameObject.SetActive(false);
		}
		int daKongPingZhi = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetDaKongPingZhi(Global.GetZhuoyueAttributeCount(gd));
		ZhuangBeiDaKongVO daKongDataByJieShu = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetDaKongDataByJieShu(goodsXmlNodeByID.SuitID, daKongPingZhi);
		for (int j = 0; j < this.m_ListBack.Count; j++)
		{
			if (j >= daKongDataByJieShu.DaKongShuLiang)
			{
				this.m_ListBack[j].gameObject.SetActive(false);
			}
			else
			{
				int num3 = 0;
				int num4 = 0;
				for (int k = 0; k < array.Length; k++)
				{
					if (!string.IsNullOrEmpty(array[k]))
					{
						int num5 = array[k].Split(new char[]
						{
							'_'
						})[0].SafeToInt32(0) - 1;
						if (num5 == j)
						{
							num3 = array[k].Split(new char[]
							{
								'_'
							})[1].SafeToInt32(0);
							num4 = array[k].Split(new char[]
							{
								'_'
							})[2].SafeToInt32(0);
						}
					}
				}
				this.m_ListBack[j].gameObject.SetActive(true);
				if (num3 > 0)
				{
					this.m_ListBack[j].spriteName = IConfigbase<ConfigChongShengZhuangBei>.Instance.PinZhiKuang(num3);
				}
				else
				{
					this.m_ListBack[j].spriteName = "dakong_no";
				}
				if (num4 > 0 && IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO.ContainsKey(num4))
				{
					ChongShengBaoShiVO chongShengBaoShiVO = IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO[num4];
					this.m_ListShowNetBaoShi[j].URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png", num4);
					this.m_ListShowNetBaoShi[j].gameObject.SetActive(true);
				}
				else
				{
					this.m_ListShowNetBaoShi[j].gameObject.SetActive(false);
				}
			}
		}
	}

	public void SetDataXuanCai(int goodsId, int count, int binDing)
	{
		GoodsData gd = Global.GetDummyGoodsDataMu(goodsId, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		gd.GCount = count;
		gd.Binding = binDing;
		if (gd != null)
		{
			GGoodIcon icon = Global.CreateGoodsIcon(gd, false, true);
			if (this.m_GameIcion.GetComponentInChildren<GGoodIcon>() != null)
			{
				Object.Destroy(this.m_GameIcion.GetComponentInChildren<GGoodIcon>().gameObject);
			}
			icon.transform.SetParent(this.m_GameIcion.transform, false);
			if (icon.GetComponent<UIPanel>() != null)
			{
				Object.Destroy(icon.GetComponent<UIPanel>());
			}
			icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 5)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ZhuZhuangBei = gd
					});
				}
			};
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				gd.GCount = this.Number;
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, gd);
				GTipServiceEx.GGoodTipsPart.FootIntroTxtNum.Text = string.Format(Global.GetLang("数量: {0}"), gd.GCount);
			};
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			this.m_LabName.text = UIHelper.FormatGoodsName(gd, false, true, true);
			this.m_GoodsData = gd;
		}
		this.Number = gd.GCount;
	}

	public int Number
	{
		get
		{
			return this.number;
		}
		set
		{
			this.number = value;
			this.m_LabTiShi.gameObject.SetActive(true);
			this.m_LabTiShi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang(string.Format(Global.GetLang("拥有数量:{0}"), this.number))
			});
		}
	}

	public GoodsData GoodsData
	{
		get
		{
			return this.m_GoodsData;
		}
	}

	public bool OnClickItem
	{
		set
		{
			this.m_SpBackOnClick.gameObject.SetActive(value);
		}
	}

	[SerializeField]
	private UISprite m_SpBackXuanCai;

	[SerializeField]
	private ShowNetImage m_ImgXuanCai;

	[SerializeField]
	private List<UISprite> m_ListBack;

	[SerializeField]
	private List<ShowNetImage> m_ListShowNetBaoShi;

	[SerializeField]
	private List<GameObject> m_ListGmaeChongSheng;

	[SerializeField]
	private GameObject m_GameIcion;

	[SerializeField]
	private UILabel m_LabName;

	[SerializeField]
	private UILabel m_LabTiShi;

	[SerializeField]
	private UISprite m_SpBackOnClick;

	private Vector3[] m_ListVec = new Vector3[3];

	public DPSelectedItemEventHandler DPSelectedItem;

	private int number;

	private GoodsData m_GoodsData;
}
