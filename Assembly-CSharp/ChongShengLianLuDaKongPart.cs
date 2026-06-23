using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ChongShengLianLuDaKongPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.Init();
		this.m_Obser = this.m_ListBoxCaiLiao.ItemsSource;
		this.m_BtnDaKong.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendDaKong(this.m_OnClickKey);
		};
		for (int i = 0; i < this.m_ListBtnChongSheng.Count; i++)
		{
			int key = i;
			this.m_ListBtnChongSheng[key].MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.btnOnClickBaoShi(key);
			};
		}
		this.AddEqip(null);
		base.StartCoroutine<bool>(this.StartTeXiao());
	}

	private IEnumerator StartTeXiao()
	{
		yield return new WaitForSeconds(0.2f);
		GameObject texiao = Global.LoadTeXiaoObj("UITeXiao/Perfabs/chongsheng/chongsheng_beijing_01", this.m_TexiaoBack.transform.parent);
		Vector3 vec = texiao.transform.localPosition;
		vec.z -= 0.1f;
		texiao.transform.localPosition = vec;
		base.StopCoroutine(this.StartTeXiao());
		yield break;
	}

	private void Init()
	{
		this.m_CheCk.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("优先使用绑定材料")
		});
		Dictionary<int, double[]> systemParamIntDoubleDictByName = ConfigSystemParam.GetSystemParamIntDoubleDictByName("DaKongShuXing", '|', ',');
		this.m_ListLab[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang(string.Format(Global.GetLang("属性X{0}倍"), ConfigSystemParam.GetSystemParamIntDoubleDictByName("DaKongShuXing", '|', ',')[5][1].ToString("f1")))
		});
		this.m_ListLab[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang(string.Format(Global.GetLang("属性X{0}倍"), ConfigSystemParam.GetSystemParamIntDoubleDictByName("DaKongShuXing", '|', ',')[4][1].ToString("f1")))
		});
		this.m_ListLab[2].text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang(string.Format(Global.GetLang("属性X{0}倍"), ConfigSystemParam.GetSystemParamIntDoubleDictByName("DaKongShuXing", '|', ',')[3][1].ToString("f1")))
		});
		this.m_LabNullTiShi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("请放入需要打孔的装备")
		});
		this.m_BtnDaKong.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			Global.GetLang("打孔")
		});
	}

	private void btnOnClickBaoShi(int key)
	{
		this.m_OnClickKey = key;
		this.m_BtnDaKong.isEnabled = true;
		this.RefreshGoodList();
		if (this.m_EquipIcon == null)
		{
			Super.HintMainText(Global.GetLang("请放入要打孔的装备"), 10, 3);
			return;
		}
		GoodsData goodsData = this.m_EquipIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			Super.HintMainText(Global.GetLang("请放入要打孔的装备"), 10, 3);
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		int daKongPingZhi = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetDaKongPingZhi(Global.GetZhuoyueAttributeCount(goodsData));
		ZhuangBeiDaKongVO daKongDataByJieShu = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetDaKongDataByJieShu(goodsXmlNodeByID.SuitID, daKongPingZhi);
		if (!string.IsNullOrEmpty(goodsData.Props))
		{
			string[] array = goodsData.Props.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					if (array[i].Split(new char[]
					{
						'_'
					})[0].SafeToInt32(0) == this.m_OnClickKey + 1)
					{
						string[] array2 = daKongDataByJieShu.GaiLv.Split(new char[]
						{
							'|'
						});
						int[] array3 = new int[array2.Length];
						for (int j = 0; j < array2.Length; j++)
						{
							if (!string.IsNullOrEmpty(array2[j]) && float.Parse(array2[j].Split(new char[]
							{
								','
							})[1]) > 0f)
							{
								array3[j] = array2[j].Split(new char[]
								{
									','
								})[0].SafeToInt32(0);
							}
						}
						int num = Mathf.Max(array3);
						if (!string.IsNullOrEmpty(array[i].Split(new char[]
						{
							'_'
						})[1]) && array[i].Split(new char[]
						{
							'_'
						})[1].SafeToInt32(0) >= num)
						{
							Super.HintMainText(Global.GetLang("已经是最优品质"), 10, 3);
							this.m_BtnDaKong.isEnabled = false;
						}
					}
				}
			}
		}
		for (int k = 0; k < this.m_ListBtnChongSheng.Count; k++)
		{
			if (key == k)
			{
				this.m_ListBtnChongSheng[k].target.gameObject.SetActive(true);
			}
			else
			{
				this.m_ListBtnChongSheng[k].target.gameObject.SetActive(false);
			}
		}
	}

	public void AddEqip(GoodsData gd = null)
	{
		this.m_OnClickKey = -1;
		if (this.m_EquipIcon != null)
		{
			Object.Destroy(this.m_EquipIcon.gameObject);
		}
		if (gd == null)
		{
			this.m_BtnShanChu.gameObject.SetActive(false);
			this.m_Obser.Clear();
			for (int i = 0; i < this.m_ListBaoShiBak.Count; i++)
			{
				this.m_ListBaoShiBak[i].gameObject.SetActive(true);
				this.m_ListShowNetBaoShi[i].gameObject.SetActive(false);
				this.m_ListLab[i].gameObject.SetActive(true);
				this.m_ListBtnChongSheng[i].gameObject.SetActive(false);
			}
			this.m_ListBaoShiBak[0].spriteName = "kuang_zishan";
			this.m_ListBaoShiBak[1].spriteName = "kuang_zi";
			this.m_ListBaoShiBak[2].spriteName = "kuang_lan";
			this.m_ImageXuanCaiBaoShi.gameObject.SetActive(false);
			this.m_SpXuanCaiBak.gameObject.SetActive(true);
			this.m_SpXuanCaiBak.spriteName = "bukedakong";
			this.m_LabNullTiShi.gameObject.SetActive(true);
			this.m_CheCk.gameObject.SetActive(false);
			this.m_BtnDaKong.isEnabled = false;
			return;
		}
		this.m_LabNullTiShi.gameObject.SetActive(false);
		this.m_CheCk.gameObject.SetActive(true);
		this.m_BtnDaKong.isEnabled = true;
		GGoodIcon ggoodIcon = Global.CreateGoodsIcon(gd, false, true);
		ggoodIcon.transform.SetParent(this.m_EquipParent.transform, false);
		this.m_EquipIcon = ggoodIcon;
		ggoodIcon.BackgroundSprite0.gameObject.SetActive(false);
		this.m_BtnShanChu.gameObject.SetActive(true);
		ggoodIcon.BackgroundSprite1.transform.localScale = new Vector3(84f, 135f, 0f);
		ggoodIcon.NoUseSprite.transform.localScale = new Vector3(84f, 135f, 0f);
		ggoodIcon.BackgroundSprite15.transform.localScale = new Vector3(84f, 135f, 0f);
		ggoodIcon.BindingSprite.transform.localPosition = new Vector3(-32f, -55f, -0.03f);
		ggoodIcon.TeXiao.transform.localScale = new Vector3(1.2f, 2f, 1f);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		int daKongPingZhi = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetDaKongPingZhi(Global.GetZhuoyueAttributeCount(gd));
		ZhuangBeiDaKongVO daKongDataByJieShu = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetDaKongDataByJieShu(goodsXmlNodeByID.SuitID, daKongPingZhi);
		if (daKongDataByJieShu == null)
		{
			for (int j = 0; j < this.m_ListBaoShiBak.Count; j++)
			{
				this.m_ListLab[j].gameObject.SetActive(false);
				this.m_ListShowNetBaoShi[j].gameObject.SetActive(false);
				this.m_ListBaoShiBak[j].gameObject.SetActive(false);
				this.m_ListBtnChongSheng[j].gameObject.SetActive(false);
			}
			return;
		}
		string[] array = gd.Props.Split(new char[]
		{
			'|'
		});
		this.m_SpXuanCaiBak.gameObject.SetActive(true);
		if (goodsXmlNodeByID.Categoriy == 37)
		{
			this.m_ImageXuanCaiBaoShi.gameObject.SetActive(true);
			int num = 0;
			for (int k = 0; k < array.Length; k++)
			{
				if (!string.IsNullOrEmpty(array[k]))
				{
					int num2 = array[k].Split(new char[]
					{
						'_'
					})[0].SafeToInt32(0);
					if (num2 == -1)
					{
						num = array[k].Split(new char[]
						{
							'_'
						})[2].SafeToInt32(0);
					}
				}
			}
			if (num > 0)
			{
				this.m_ImageXuanCaiBaoShi.gameObject.SetActive(true);
				this.m_ImageXuanCaiBaoShi.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png", num);
			}
			else
			{
				this.m_ImageXuanCaiBaoShi.gameObject.SetActive(false);
			}
			this.m_SpXuanCaiBak.spriteName = "kuang_xuancai";
		}
		else
		{
			this.m_ImageXuanCaiBaoShi.gameObject.SetActive(false);
			this.m_SpXuanCaiBak.spriteName = "bukedakong";
		}
		for (int l = 0; l < this.m_ListBaoShiBak.Count; l++)
		{
			this.m_ListLab[l].gameObject.SetActive(false);
			if (l >= daKongDataByJieShu.DaKongShuLiang)
			{
				this.m_ListBtnChongSheng[l].gameObject.SetActive(false);
				this.m_ListShowNetBaoShi[l].gameObject.SetActive(false);
				this.m_ListBaoShiBak[l].spriteName = "bukedakong";
			}
			else
			{
				this.m_ListBaoShiBak[l].gameObject.SetActive(true);
				this.m_ListBtnChongSheng[l].gameObject.SetActive(true);
				this.m_ListBtnChongSheng[l].target.gameObject.SetActive(false);
				if (!string.IsNullOrEmpty(gd.Props))
				{
					int num3 = 0;
					int num4 = 0;
					for (int m = 0; m < array.Length; m++)
					{
						if (!string.IsNullOrEmpty(array[m]))
						{
							int num5 = array[m].Split(new char[]
							{
								'_'
							})[0].SafeToInt32(0) - 1;
							if (num5 == l)
							{
								num3 = array[m].Split(new char[]
								{
									'_'
								})[1].SafeToInt32(0);
								num4 = array[m].Split(new char[]
								{
									'_'
								})[2].SafeToInt32(0);
								break;
							}
						}
					}
					if (num3 > 0)
					{
						this.m_ListBaoShiBak[l].spriteName = IConfigbase<ConfigChongShengZhuangBei>.Instance.PinZhiKuang(num3);
					}
					else
					{
						this.m_ListBaoShiBak[l].spriteName = "dakong_no";
					}
					if (num4 > 0 && IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO.ContainsKey(num4))
					{
						this.m_ListShowNetBaoShi[l].URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png", num4);
						this.m_ListShowNetBaoShi[l].gameObject.SetActive(true);
					}
					else
					{
						this.m_ListShowNetBaoShi[l].gameObject.SetActive(false);
					}
				}
				else
				{
					this.m_ListBaoShiBak[l].spriteName = "dakong_no";
					this.m_ListShowNetBaoShi[l].gameObject.SetActive(false);
				}
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		this.strVO = daKongDataByJieShu.XiaoHaoDaoJu.Split(new char[]
		{
			'|'
		});
		for (int n = 0; n < this.strVO.Length; n++)
		{
			if (n <= this.strVO.Length - 1)
			{
				stringBuilder.Append(string.Format("{0},0,0,0,0,0|", this.strVO[n]));
			}
			else
			{
				stringBuilder.Append(string.Format("{0},0,0,0,0,0", this.strVO[n]));
			}
		}
		this.m_Obser.Clear();
		Super.LoadGoodsList(stringBuilder.ToString(), this.m_Obser);
		this.RefreshGoodList();
	}

	private void RefreshGoodList()
	{
		for (int i = 0; i < this.m_Obser.Count; i++)
		{
			GGoodIcon componentInChildren = this.m_Obser.GetAt(i).GetComponentInChildren<GGoodIcon>();
			if (componentInChildren == null)
			{
				return;
			}
			componentInChildren.SecondText.transform.localScale = new Vector3(22f, 22f, 1f);
			componentInChildren.SecondText.Label.pivot = 4;
			componentInChildren.SecondText.transform.localPosition = new Vector3(0f, -55f, -2f);
			if (Global.Data.roleData.RebornGoodsDataList != null)
			{
				int num = this.strVO[i].Split(new char[]
				{
					','
				})[0].SafeToInt32(0);
				int num2 = 0;
				int num3 = 0;
				for (int j = 0; j < Global.Data.roleData.RebornGoodsDataList.Count; j++)
				{
					if (Global.Data.roleData.RebornGoodsDataList[j].GoodsID == num)
					{
						if (Global.Data.roleData.RebornGoodsDataList[j].Binding == 1)
						{
							num2 += Global.Data.roleData.RebornGoodsDataList[j].GCount;
						}
						else
						{
							num3 += Global.Data.roleData.RebornGoodsDataList[j].GCount;
						}
					}
				}
				componentInChildren.SecondText.Label.supportEncoding = true;
				if (num2 + num3 >= this.strVO[i].Split(new char[]
				{
					','
				})[1].SafeToInt32(0))
				{
					componentInChildren.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						string.Format("{0}/{1}", num2 + num3, this.strVO[i].Split(new char[]
						{
							','
						})[1].SafeToInt32(0))
					});
					this.m_BtnDaKong.isEnabled = true;
				}
				else
				{
					componentInChildren.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format("{0}/{1}", num2 + num3, this.strVO[i].Split(new char[]
						{
							','
						})[1].SafeToInt32(0))
					});
					this.m_BtnDaKong.isEnabled = false;
				}
			}
			else
			{
				componentInChildren.SText = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format("0/{1}", this.strVO[i].Split(new char[]
					{
						','
					})[1].SafeToInt32(0))
				});
				this.m_BtnDaKong.isEnabled = false;
			}
		}
	}

	private void SendDaKong(int key)
	{
		this.m_OnClickKey = key;
		if (this.m_EquipIcon == null)
		{
			Super.HintMainText(Global.GetLang("请放入要打孔的装备"), 10, 3);
			return;
		}
		GoodsData gd = this.m_EquipIcon.ItemObject as GoodsData;
		if (gd == null)
		{
			Super.HintMainText(Global.GetLang("请放入要打孔的装备"), 10, 3);
			return;
		}
		if (this.m_OnClickKey < 0)
		{
			Super.HintMainText(Global.GetLang("请选择要打孔的位置"), 10, 3);
			return;
		}
		string props = gd.Props;
		int num = 0;
		if (!string.IsNullOrEmpty(props))
		{
			num = props.Split(new char[]
			{
				'|'
			}).Length;
		}
		bool IsCreal = false;
		for (int i = 0; i < num; i++)
		{
			if (props.Split(new char[]
			{
				'|'
			})[i].Split(new char[]
			{
				'_'
			})[0].SafeToInt32(0) == this.m_OnClickKey + 1)
			{
				IsCreal = true;
			}
		}
		for (int j = 0; j < this.m_Obser.Count; j++)
		{
			if (Global.Data.roleData.RebornGoodsDataList != null)
			{
				int num2 = this.strVO[j].Split(new char[]
				{
					','
				})[1].SafeToInt32(0);
				int num3 = this.strVO[j].Split(new char[]
				{
					','
				})[0].SafeToInt32(0);
				int num4 = 0;
				int num5 = 0;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num3);
				for (int k = 0; k < Global.Data.roleData.RebornGoodsDataList.Count; k++)
				{
					if (Global.Data.roleData.RebornGoodsDataList[k].GoodsID == num3)
					{
						if (Global.Data.roleData.RebornGoodsDataList[k].Binding == 1)
						{
							num4 += Global.Data.roleData.RebornGoodsDataList[k].GCount;
						}
						else
						{
							num5 += Global.Data.roleData.RebornGoodsDataList[k].GCount;
						}
					}
				}
				if (num4 + num5 < num2)
				{
					Super.HintMainText(Global.GetLang("打孔所需材料不足"), 10, 3);
					return;
				}
				if (this.m_CheCk.isChecked)
				{
					if (num4 > 0 && gd.Binding == 0)
					{
						Super.ShowMessageBox(Global.GetLang("提示"), string.Format(Global.GetLang("存在绑定的材料，操作后您的装备将变为绑定，确认要执行该操作吗?"), Environment.NewLine), 1, delegate(object s, DPSelectedItemEventArgs e)
						{
							if (e.ID == 0)
							{
								GameInstance.Game.SendChongShengDaKong(gd.Id, this.m_OnClickKey + 1, this.m_CheCk.isChecked, IsCreal);
							}
						}, MessBoxIsHintTypes.None);
						return;
					}
				}
				else if (num5 < num2 && gd.Binding == 0)
				{
					Super.ShowMessageBox(Global.GetLang("提示"), string.Format(Global.GetLang("存在绑定的材料，操作后您的装备将变为绑定，确认要执行该操作吗?"), Environment.NewLine), 1, delegate(object s, DPSelectedItemEventArgs e)
					{
						if (e.ID == 0)
						{
							GameInstance.Game.SendChongShengDaKong(gd.Id, this.m_OnClickKey + 1, this.m_CheCk.isChecked, IsCreal);
						}
					}, MessBoxIsHintTypes.None);
					return;
				}
			}
		}
		GameInstance.Game.SendChongShengDaKong(gd.Id, this.m_OnClickKey + 1, this.m_CheCk.isChecked, IsCreal);
	}

	public void Refresh(string props, int binDing)
	{
		GGoodIcon componentInChildren = this.m_EquipParent.GetComponentInChildren<GGoodIcon>();
		if (componentInChildren == null)
		{
			Super.HintMainText(Global.GetLang("请放入要打孔的装备"), 10, 3);
			return;
		}
		GoodsData goodsData = componentInChildren.ItemObject as GoodsData;
		if (goodsData == null)
		{
			Super.HintMainText(Global.GetLang("请放入要打孔的装备"), 10, 3);
			return;
		}
		goodsData.Props = props;
		if (Global.Data.roleData.RebornGoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.RebornGoodsDataList[i].Id == goodsData.Id)
				{
					Global.Data.roleData.RebornGoodsDataList[i].Props = goodsData.Props;
					Global.Data.roleData.RebornGoodsDataList[i].Binding = binDing;
					goodsData.Binding = binDing;
				}
			}
		}
		int onClickKey = this.m_OnClickKey;
		this.AddEqip(goodsData);
		this.m_OnClickKey = onClickKey;
		this.btnOnClickBaoShi(this.m_OnClickKey);
		this.dpsRefreshEquip(this, new DPSelectedItemEventArgs
		{
			ZhuZhuangBei = goodsData
		});
		GameObject gameObject = Global.LoadTeXiaoObj("UITeXiao/Perfabs/lianjin/lianjin_jihuo", this.m_ListBaoShiBak[this.m_OnClickKey].transform.parent);
		if (gameObject.GetComponent<DelayDestroy>() == null)
		{
			gameObject.AddComponent<DelayDestroy>();
		}
		gameObject.GetComponent<DelayDestroy>().delayTime = 2f;
	}

	[SerializeField]
	private ListBox m_ListBoxCaiLiao;

	[SerializeField]
	private GCheckBox m_CheCk;

	[SerializeField]
	private GButton m_BtnDaKong;

	[SerializeField]
	private GameObject m_EquipParent;

	public GButton m_BtnShanChu;

	[SerializeField]
	private UILabel m_LabNullTiShi;

	[SerializeField]
	private List<UISprite> m_ListBaoShiBak;

	[SerializeField]
	private List<ShowNetImage> m_ListShowNetBaoShi;

	[SerializeField]
	private List<UILabel> m_ListLab;

	[SerializeField]
	private List<GButton> m_ListBtnChongSheng;

	[SerializeField]
	private UISprite m_SpXuanCaiBak;

	[SerializeField]
	private ShowNetImage m_ImageXuanCaiBaoShi;

	[SerializeField]
	private GameObject m_TexiaoBack;

	private ObservableCollection m_Obser;

	public DPSelectedItemEventHandler dpsRefreshEquip;

	private int m_OnClickKey = -1;

	private string[] strVO;

	private GGoodIcon m_EquipIcon;
}
