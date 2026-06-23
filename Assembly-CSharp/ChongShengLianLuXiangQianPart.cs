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

public class ChongShengLianLuXiangQianPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_BtnFangRu.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetLang("放入")
		});
		this.m_Obser = this.m_ListBoxBaoShi.ItemsSource;
		this.m_ListBoxBaoShi.SelectionChanged = new MouseLeftButtonUpEventHandler(this.EquipOnClick);
		this.m_PanelXuanZeBaoShi.gameObject.SetActive(false);
		this.m_VecXuanZeIcon = this.m_SpringXuanZe.transform.localPosition;
		this.m_BtnShanChu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.AddEqip(null);
		};
		this.m_BtnXuanZeClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_PanelXuanZeBaoShi.gameObject.SetActive(false);
		};
		this.m_BtnXiangQianBack.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_PanelXuanZeBaoShi.gameObject.SetActive(false);
		};
		this.m_BtnBack.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GGoodIcon componentInChildren = this.m_EquipParent.GetComponentInChildren<GGoodIcon>();
			if (componentInChildren == null)
			{
				return;
			}
			GoodsData goodsData = componentInChildren.ItemObject as GoodsData;
			if (goodsData == null)
			{
				return;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID.Categoriy == 37)
			{
				return;
			}
			if (string.IsNullOrEmpty(goodsData.Props))
			{
				Super.HintMainText(Global.GetLang("当前无可插入宝石的孔位，请先打孔"), 10, 3);
			}
		};
		this.m_BtnXuanCai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GGoodIcon componentInChildren = this.m_EquipParent.GetComponentInChildren<GGoodIcon>();
			if (componentInChildren != null)
			{
				GoodsData goodsData = componentInChildren.ItemObject as GoodsData;
				if (goodsData == null)
				{
					return;
				}
				string[] array = goodsData.Props.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						int num = array[i].Split(new char[]
						{
							'_'
						})[0].SafeToInt32(0);
						if (num == -1)
						{
							int num2 = array[i].Split(new char[]
							{
								'_'
							})[2].SafeToInt32(0);
							if (num2 > 0)
							{
								GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(num2, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
								this.OnClickXieXia(-1, dummyGoodsDataMu);
								return;
							}
						}
					}
				}
				this.OnClickXiangQian(-1);
			}
		};
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

	public void AddEqip(GoodsData gd = null)
	{
		GGoodIcon ggoodIcon = this.m_EquipParent.GetComponentInChildren<GGoodIcon>();
		if (ggoodIcon != null)
		{
			Object.Destroy(ggoodIcon.gameObject);
		}
		if (gd == null)
		{
			for (int i = 0; i < this.m_ListBaoShiBak.Count; i++)
			{
				this.m_ListBaoShiBak[i].gameObject.SetActive(true);
				this.m_ListBaoShiBak[i].spriteName = "bukedakong";
				this.m_ListShowNetBaoShi[i].gameObject.SetActive(false);
				this.m_ListBtn[i].gameObject.SetActive(false);
				this.m_ListJiaHao[i].gameObject.SetActive(false);
			}
			this.m_BtnShanChu.gameObject.SetActive(false);
			this.m_ImageXuanCaiBaoShi.gameObject.SetActive(false);
			this.m_SpXuanCaiBak.spriteName = "bukedakong";
			this.m_BtnXuanCai.gameObject.SetActive(false);
			return;
		}
		ggoodIcon = Global.CreateGoodsIcon(gd, false, true);
		ggoodIcon.transform.SetParent(this.m_EquipParent.transform, false);
		ggoodIcon.BackgroundSprite0.gameObject.SetActive(false);
		this.m_BtnShanChu.gameObject.SetActive(true);
		ggoodIcon.BackgroundSprite1.transform.localScale = new Vector3(84f, 135f, 0f);
		ggoodIcon.NoUseSprite.transform.localScale = new Vector3(84f, 135f, 0f);
		ggoodIcon.BackgroundSprite15.transform.localScale = new Vector3(84f, 135f, 0f);
		ggoodIcon.BindingSprite.transform.localPosition = new Vector3(-32f, -55f, -0.03f);
		ggoodIcon.TeXiao.transform.localScale = new Vector3(1.2f, 2f, 1f);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		int num = Global.GetZhuoyueAttributeCount(gd);
		if (num >= 5)
		{
			num = 5;
		}
		ZhuangBeiDaKongVO daKongDataByJieShu = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetDaKongDataByJieShu(goodsXmlNodeByID.SuitID, num);
		if (daKongDataByJieShu == null)
		{
			for (int j = 0; j < this.m_ListBaoShiBak.Count; j++)
			{
				this.m_ListShowNetBaoShi[j].gameObject.SetActive(false);
				this.m_ListBaoShiBak[j].gameObject.SetActive(false);
			}
			return;
		}
		string[] array = gd.Props.Split(new char[]
		{
			'|'
		});
		if (goodsXmlNodeByID.Categoriy == 37)
		{
			this.m_SpXuanCaiBak.spriteName = "kuang_xuancai";
			this.m_BtnXuanCai.gameObject.SetActive(true);
			int num2 = 0;
			for (int k = 0; k < array.Length; k++)
			{
				if (!string.IsNullOrEmpty(array[k]))
				{
					int num3 = array[k].Split(new char[]
					{
						'_'
					})[0].SafeToInt32(0);
					if (num3 == -1)
					{
						num2 = array[k].Split(new char[]
						{
							'_'
						})[2].SafeToInt32(0);
						break;
					}
				}
			}
			if (num2 > 0)
			{
				this.m_BtnXuanCai.target.gameObject.SetActive(false);
				this.m_ImageXuanCaiBaoShi.gameObject.SetActive(true);
				this.m_ImageXuanCaiBaoShi.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png", num2);
			}
			else
			{
				this.m_ImageXuanCaiBaoShi.gameObject.SetActive(false);
				this.m_BtnXuanCai.target.gameObject.SetActive(true);
			}
		}
		else
		{
			this.m_BtnXuanCai.gameObject.SetActive(false);
			this.m_ImageXuanCaiBaoShi.gameObject.SetActive(false);
			this.m_SpXuanCaiBak.spriteName = "bukedakong";
		}
		for (int l = 0; l < this.m_ListBaoShiBak.Count; l++)
		{
			if (l >= daKongDataByJieShu.DaKongShuLiang)
			{
				this.m_ListJiaHao[l].gameObject.SetActive(false);
				this.m_ListBtn[l].gameObject.SetActive(false);
				this.m_ListShowNetBaoShi[l].gameObject.SetActive(false);
				this.m_ListBaoShiBak[l].spriteName = "bukedakong";
			}
			else
			{
				this.m_ListBaoShiBak[l].gameObject.SetActive(true);
				if (!string.IsNullOrEmpty(gd.Props))
				{
					int num4 = 0;
					int baoShiId = 0;
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
								num4 = array[m].Split(new char[]
								{
									'_'
								})[1].SafeToInt32(0);
								baoShiId = array[m].Split(new char[]
								{
									'_'
								})[2].SafeToInt32(0);
							}
						}
					}
					if (baoShiId > 0)
					{
						this.m_ListShowNetBaoShi[l].gameObject.SetActive(true);
						this.m_ListShowNetBaoShi[l].URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png", baoShiId);
						if (num4 > 0)
						{
							this.m_ListJiaHao[l].gameObject.SetActive(false);
							this.m_ListBtn[l].gameObject.SetActive(true);
							this.m_ListBtn[l].target.gameObject.SetActive(false);
							int daKongKey = l;
							this.m_ListBtn[daKongKey].MouseLeftButtonUp = delegate(object s, MouseEvent e)
							{
								GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(baoShiId, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
								this.OnClickXieXia(daKongKey + 1, dummyGoodsDataMu);
							};
							this.m_ListBaoShiBak[l].spriteName = IConfigbase<ConfigChongShengZhuangBei>.Instance.PinZhiKuang(num4);
						}
						else
						{
							this.m_ListJiaHao[l].gameObject.SetActive(false);
							this.m_ListBtn[l].gameObject.SetActive(false);
							this.m_ListBtn[l].target.gameObject.SetActive(false);
							this.m_ListBaoShiBak[l].spriteName = "dakong_no";
						}
					}
					else
					{
						this.m_ListShowNetBaoShi[l].gameObject.SetActive(false);
						if (num4 > 0)
						{
							this.m_ListJiaHao[l].spriteName = IConfigbase<ConfigChongShengZhuangBei>.Instance.PinZhiXiangQian(num4);
							this.m_ListJiaHao[l].gameObject.SetActive(true);
							this.m_ListBtn[l].gameObject.SetActive(true);
							this.m_ListBtn[l].target.gameObject.SetActive(false);
							int daKongKey = l;
							this.m_ListBtn[daKongKey].MouseLeftButtonUp = delegate(object s, MouseEvent e)
							{
								this.OnClickXiangQian(daKongKey + 1);
							};
							this.m_ListBaoShiBak[l].spriteName = IConfigbase<ConfigChongShengZhuangBei>.Instance.PinZhiKuang(num4);
						}
						else
						{
							this.m_ListJiaHao[l].gameObject.SetActive(false);
							this.m_ListBtn[l].gameObject.SetActive(false);
							this.m_ListBtn[l].target.gameObject.SetActive(false);
							this.m_ListBaoShiBak[l].spriteName = "dakong_no";
						}
					}
				}
				else
				{
					this.m_ListJiaHao[l].gameObject.SetActive(false);
					this.m_ListBaoShiBak[l].spriteName = "dakong_no";
					this.m_ListShowNetBaoShi[l].gameObject.SetActive(false);
					this.m_ListBtn[l].gameObject.SetActive(false);
				}
			}
		}
	}

	private void OnClickXieXia(int key, GoodsData gd)
	{
		this.m_Key = key;
		this.m_PanelXuanZeBaoShi.gameObject.SetActive(true);
		this.m_PanelLeft.gameObject.SetActive(false);
		this.m_PanelContent.transform.localPosition = new Vector3(-130f, 0f, -0.1f);
		for (int i = 0; i < this.m_ListBtn.Count; i++)
		{
			if (key - 1 == i)
			{
				this.m_ListBtn[i].target.gameObject.SetActive(true);
			}
			else
			{
				this.m_ListBtn[i].target.gameObject.SetActive(false);
			}
		}
		this.OpenGoods(gd);
		this.m_BtnFangRu.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetLang("卸下")
		});
		this.m_BtnFangRu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GGoodIcon componentInChildren = this.m_EquipParent.GetComponentInChildren<GGoodIcon>();
			if (componentInChildren == null)
			{
				return;
			}
			GoodsData goodsData = componentInChildren.ItemObject as GoodsData;
			if (goodsData == null)
			{
				return;
			}
			GameInstance.Game.SendChongShengXieXia(goodsData.Id, this.m_Key);
		};
	}

	private void OnClickXiangQian(int key)
	{
		this.m_Key = key;
		this.m_PanelContent.transform.localPosition = new Vector3(0f, 0f, -0.1f);
		for (int i = 0; i < this.m_ListBtn.Count; i++)
		{
			if (key - 1 == i)
			{
				this.m_ListBtn[i].target.gameObject.SetActive(true);
			}
			else
			{
				this.m_ListBtn[i].target.gameObject.SetActive(false);
			}
		}
		this.m_Obser.Clear();
		this.m_SpringXuanZe.target = this.m_VecXuanZeIcon;
		this.m_SpringXuanZe.enabled = true;
		if (Global.Data.roleData.RebornGoodsDataList == null)
		{
			return;
		}
		bool flag = true;
		for (int j = 0; j < Global.Data.roleData.RebornGoodsDataList.Count; j++)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.roleData.RebornGoodsDataList[j].GoodsID);
			if (key == -1)
			{
				if (goodsXmlNodeByID.Categoriy == 960)
				{
					GGoodIcon ggoodIcon = Super.AddGoodsIcon(Global.Data.roleData.RebornGoodsDataList[j], this.m_Obser, false, false);
					if (ggoodIcon.GetComponent<UIPanel>() != null)
					{
						Object.Destroy(ggoodIcon.GetComponent<UIPanel>());
					}
					if (flag)
					{
						this.OpenGoods(Global.Data.roleData.RebornGoodsDataList[j]);
						flag = false;
						Vector3 localPosition = ggoodIcon.transform.localPosition;
						localPosition.z -= 1f;
						this.m_SpOnBack.transform.localPosition = localPosition;
					}
					ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
					};
				}
			}
			else if (goodsXmlNodeByID.Categoriy == 950)
			{
				GGoodIcon ggoodIcon2 = Super.AddGoodsIcon(Global.Data.roleData.RebornGoodsDataList[j], this.m_Obser, false, false);
				if (ggoodIcon2.GetComponent<UIPanel>() != null)
				{
					Object.Destroy(ggoodIcon2.GetComponent<UIPanel>());
				}
				if (flag)
				{
					this.OpenGoods(Global.Data.roleData.RebornGoodsDataList[j]);
					flag = false;
					Vector3 localPosition2 = ggoodIcon2.transform.localPosition;
					localPosition2.z -= 1f;
					this.m_SpOnBack.transform.localPosition = localPosition2;
				}
				ggoodIcon2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
				};
			}
		}
		this.m_BtnFangRu.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetLang("镶嵌")
		});
		this.m_BtnFangRu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GGoodIcon componentInChildren = this.m_EquipParent.GetComponentInChildren<GGoodIcon>();
			if (componentInChildren == null)
			{
				return;
			}
			GoodsData gd = componentInChildren.ItemObject as GoodsData;
			if (gd == null)
			{
				return;
			}
			if (gd.Binding == 0 && this.m_BaoShiData.Binding == 1)
			{
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("镶嵌宝石绑定，镶嵌后为绑定为绑定装备，是否确定绑定")
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SendChongShengXiangQian(gd.Id, this.m_BaoShiData.Id, this.m_Key);
					}
					return true;
				};
				return;
			}
			if (gd.Binding == 1 && this.m_BaoShiData.Binding == 0)
			{
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("装备已绑定，镶嵌后宝石为绑定，是否确定绑定")
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SendChongShengXiangQian(gd.Id, this.m_BaoShiData.Id, this.m_Key);
					}
					return true;
				};
				return;
			}
			GameInstance.Game.SendChongShengXiangQian(gd.Id, this.m_BaoShiData.Id, this.m_Key);
		};
		if (this.m_Obser.Count > 0)
		{
			this.m_PanelLeft.gameObject.SetActive(true);
			this.m_PanelXuanZeBaoShi.gameObject.SetActive(true);
		}
		else if (key == -1)
		{
			Super.HintMainText(Global.GetLang("当前无可镶嵌的炫彩宝石"), 10, 3);
		}
		else
		{
			Super.HintMainText(Global.GetLang("当前无可镶嵌的重生宝石"), 10, 3);
		}
	}

	private void EquipOnClick(object sender, MouseEvent e)
	{
		if (this.m_ListBoxBaoShi.SelectedItem == null)
		{
			return;
		}
		GGoodIcon component = this.m_ListBoxBaoShi.SelectedItem.GetComponent<GGoodIcon>();
		if (component == null)
		{
			return;
		}
		Vector3 localPosition = component.transform.localPosition;
		localPosition.z -= 1f;
		this.m_SpOnBack.transform.localPosition = localPosition;
		GoodsData goodsData = component.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		this.OpenGoods(goodsData);
	}

	private void OpenGoods(GoodsData gd)
	{
		this.m_BaoShiData = gd;
		if (this.m_IconXuanZe != null)
		{
			Object.Destroy(this.m_IconXuanZe.gameObject);
			this.m_IconXuanZe = null;
		}
		this.m_IconXuanZe = Global.CreateGoodsIcon(gd, false, true);
		this.m_IconXuanZe.transform.SetParent(this.m_GmaeXuanZhe.transform, false);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		this.m_LabXuanZhongName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang(goodsXmlNodeByID.Title)
		});
		StringBuilder stringBuilder = new StringBuilder();
		if (goodsXmlNodeByID.Categoriy == 950)
		{
			if (!IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO.ContainsKey(gd.GoodsID))
			{
				this.m_LabXuanZhongContent.text = string.Empty;
				this.m_LabXuanZhongName.text = string.Empty;
				return;
			}
			string[] array = IConfigbase<ConfigChongShengZhuangBei>.Instance.DicChongShengBaoShiVO[gd.GoodsID].ShuXing.Split(new char[]
			{
				'|'
			});
			ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array[0].Split(new char[]
			{
				','
			})[0]);
			stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("攻击装备佩戴") + Environment.NewLine
			}));
			if (ConfigExtPropIndexes.GetPercentByWord(array[0].Split(new char[]
			{
				','
			})[0]))
			{
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang(string.Format("{0}:{1}%", extPropIndexesVOByWord.Description, (float.Parse(array[0].Split(new char[]
					{
						','
					})[1]) * 100f).ToString("f1")) + Environment.NewLine)
				}));
			}
			else
			{
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang(string.Format("{0}:{1}", extPropIndexesVOByWord.Description, array[0].Split(new char[]
					{
						','
					})[1]) + Environment.NewLine)
				}));
			}
			ExtPropIndexesVO extPropIndexesVOByWord2 = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array[1].Split(new char[]
			{
				','
			})[0]);
			stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("防御装备佩戴") + Environment.NewLine
			}));
			if (ConfigExtPropIndexes.GetPercentByWord(array[1].Split(new char[]
			{
				','
			})[0]))
			{
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang(string.Format("{0}:{1}%", extPropIndexesVOByWord2.Description, (float.Parse(array[1].Split(new char[]
					{
						','
					})[1]) * 100f).ToString("f1")) + Environment.NewLine)
				}));
			}
			else
			{
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang(string.Format("{0}:{1}", extPropIndexesVOByWord2.Description, array[1].Split(new char[]
					{
						','
					})[1]) + Environment.NewLine)
				}));
			}
		}
		else if (goodsXmlNodeByID.Categoriy == 960)
		{
			List<XuanCaiShuXingVO> xuanCaiShuXingByGoodId = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetXuanCaiShuXingByGoodId(gd.GoodsID);
			List<bool> levelXuanCai = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetLevelXuanCai(Global.Data.RoleID, gd.GoodsID);
			for (int i = 0; i < xuanCaiShuXingByGoodId.Count; i++)
			{
				string text = string.Empty;
				if (levelXuanCai[i])
				{
					text = "5ed243";
				}
				else
				{
					text = "c8c8c8";
				}
				string[] array2 = xuanCaiShuXingByGoodId[i].JiHuoShuXing.Split(new char[]
				{
					'|'
				});
				for (int j = 0; j < array2.Length; j++)
				{
					if (!string.IsNullOrEmpty(array2[j]))
					{
						string[] array3 = array2[j].Split(new char[]
						{
							','
						});
						ExtPropIndexesVO extPropIndexesVOByWord3 = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array3[0]);
						if (j <= 0)
						{
							stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
							{
								text,
								Global.GetLang(string.Format(Global.GetLang("属性{0}："), i + 1))
							}));
						}
						else
						{
							stringBuilder.Append("              ");
						}
						if (ConfigExtPropIndexes.GetPercentByWord(array3[0]))
						{
							stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
							{
								text,
								Global.GetLang(string.Format("{1}{2}%", i + 1, extPropIndexesVOByWord3.Description, (float.Parse(array3[1]) * 100f).ToString("f1")) + Environment.NewLine)
							}));
						}
						else
						{
							stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
							{
								text,
								Global.GetLang(string.Format("{1}{2}", i + 1, extPropIndexesVOByWord3.Description, array3[1]) + Environment.NewLine)
							}));
						}
					}
				}
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"dd9130",
					Global.GetLang(xuanCaiShuXingByGoodId[i].Tips + Environment.NewLine + Environment.NewLine)
				}));
			}
		}
		this.m_LabXuanZhongContent.text = Global.GetColorStringForNGUIText(new object[]
		{
			"98d66c",
			stringBuilder.ToString()
		});
	}

	public void Refresh(string props, int binDing = -1)
	{
		this.m_PanelXuanZeBaoShi.gameObject.SetActive(false);
		GGoodIcon componentInChildren = this.m_EquipParent.GetComponentInChildren<GGoodIcon>();
		if (componentInChildren == null)
		{
			return;
		}
		GoodsData goodsData = componentInChildren.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		if (Global.Data.roleData.RebornGoodsDataList == null)
		{
			return;
		}
		for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
		{
			if (Global.Data.roleData.RebornGoodsDataList[i].Id == goodsData.Id)
			{
				Global.Data.roleData.RebornGoodsDataList[i].Props = props;
				if (binDing >= 0)
				{
					Global.Data.roleData.RebornGoodsDataList[i].Binding = binDing;
				}
				this.AddEqip(Global.Data.roleData.RebornGoodsDataList[i]);
			}
		}
		this.dpsRefreshEquip(this, new DPSelectedItemEventArgs
		{
			ZhuZhuangBei = goodsData
		});
	}

	[SerializeField]
	private GameObject m_EquipParent;

	[SerializeField]
	private GButton m_BtnShanChu;

	[SerializeField]
	private List<UISprite> m_ListBaoShiBak;

	[SerializeField]
	private List<UISprite> m_ListJiaHao;

	[SerializeField]
	private List<ShowNetImage> m_ListShowNetBaoShi;

	[SerializeField]
	private List<GButton> m_ListBtn;

	[SerializeField]
	private ListBox m_ListBoxBaoShi;

	[SerializeField]
	private UISprite m_SpXuanCaiBak;

	[SerializeField]
	private GButton m_BtnXuanCai;

	[SerializeField]
	private ShowNetImage m_ImageXuanCaiBaoShi;

	[SerializeField]
	private GameObject m_PanelLeft;

	[SerializeField]
	private GameObject m_PanelContent;

	[SerializeField]
	private UILabel m_LabXuanZhongName;

	[SerializeField]
	private UILabel m_LabXuanZhongContent;

	[SerializeField]
	private UIPanel m_PanelXuanZeBaoShi;

	[SerializeField]
	private UIDraggablePanel m_DragXuanZeBaoShi;

	[SerializeField]
	private GButton m_BtnXuanZeClose;

	[SerializeField]
	private GButton m_BtnFangRu;

	[SerializeField]
	private UISprite m_SpOnBack;

	[SerializeField]
	private GButton m_BtnBack;

	[SerializeField]
	private GButton m_BtnXiangQianBack;

	[SerializeField]
	private GameObject m_GmaeXuanZhe;

	[SerializeField]
	private SpringPanel m_SpringXuanZe;

	[SerializeField]
	private GameObject m_TexiaoBack;

	private ObservableCollection m_Obser;

	private int m_Key = -1;

	private Vector3 m_VecXuanZeIcon;

	private GoodsData m_BaoShiData;

	public DPSelectedItemEventHandler dpsRefreshEquip;

	private GGoodIcon m_IconXuanZe;
}
