using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class YuanSuJueXingUpLevelPart : UserControl
{
	protected override void OnDestroy()
	{
		base.StopCoroutine("TeXiao");
		base.StopCoroutine("TeXiaoQiu");
		base.OnDestroy();
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (Global.Data.roleData.JingLingYuanSuJueXingData != null)
		{
			this.m_GongJiLevel = ConfigYuanSuJueXing.instance.GetDatalGongJiAndChuanTou(YuanSuJueXingQiangHuaType.GongJi).QiangHuaLevel;
			this.m_ChuanTouLevel = ConfigYuanSuJueXing.instance.GetDatalGongJiAndChuanTou(YuanSuJueXingQiangHuaType.ChuanTou).QiangHuaLevel;
		}
		this.InitData();
		this.InitOnClick();
		this.Refresh(this.m_GongJiLevel, this.m_ChuanTouLevel);
		this.RefreshYuanQuan();
		this.RefreshMainStar();
		this.ReffreshData();
		this.StarPanelBool = false;
	}

	private void InitData()
	{
		this.m_BtnGongJiQiangHua.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("攻击强化")
		});
		this.m_BtnChuanTouQiangHua.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("穿透强化")
		});
		this.m_LabXiaoHaoTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("消耗：")
		});
		this.m_LabSuccessTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("成功率：")
		});
		this.m_LabShuXingTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("元素属性")
		});
		this.m_CheckShenYou.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			Global.GetLang("始终使用神佑晶石")
		});
		this.m_BtnQiangHua.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetLang("强化")
		});
		this.m_LabSuiPianNumber.text = Global.GetLang(Global.GetRoleOwnNumByMoneyType(144).ToString());
		this.m_CheckShenYou.Check = true;
		this.m_BtnGongJiQiangHua.Label.lineWidth = 85;
		this.m_BtnGongJiQiangHua.Label.transform.localPosition = new Vector3(-10f, 0f, -1f);
		this.m_BtnChuanTouQiangHua.Label.lineWidth = 85;
		this.m_BtnChuanTouQiangHua.Label.transform.localPosition = new Vector3(5f, 0f, -1f);
		this.m_CheckShenYou._Lable.lineWidth = 90;
	}

	private void InitOnClick()
	{
		this.m_QiangHuaType = YuanSuJueXingQiangHuaType.GongJi;
		this.m_BtnGongJiQiangHua.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_QiangHuaType = YuanSuJueXingQiangHuaType.GongJi;
			this.ReffreshData();
		};
		this.m_BtnChuanTouQiangHua.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_QiangHuaType = YuanSuJueXingQiangHuaType.ChuanTou;
			this.ReffreshData();
		};
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_Handler(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
		};
		this.m_BtnXuanZeYuanSu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_Handler(this, new DPSelectedItemEventArgs
			{
				ID = 2
			});
		};
		this.m_BtnQiangHua.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			MUDebug.Log<string>(new string[]
			{
				"元素觉醒石头5360"
			});
			GGoodIcon[] componentsInChildren = this.m_ListGoods.GetComponentsInChildren<GGoodIcon>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					GoodsData goodsData = componentsInChildren[i].ItemObject as GoodsData;
					if (goodsData != null)
					{
						int num = 0;
						if (Global.Data.roleData.GoodsDataList != null)
						{
							for (int j = 0; j < Global.Data.roleData.GoodsDataList.Count; j++)
							{
								if (Global.Data.roleData.GoodsDataList[j].GoodsID == goodsData.GoodsID)
								{
									num += Global.Data.roleData.GoodsDataList[j].GCount;
								}
							}
						}
						if (num < goodsData.GCount)
						{
							if (goodsData.GoodsID != 2005)
							{
								GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
								Super.HintMainText(Global.GetLang(string.Format(Global.GetLang("{0}道具不足"), goodsXmlNodeByID.Title)), 10, 3);
								return;
							}
							if (this.m_CheckShenYou.isChecked)
							{
								GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
								Super.HintMainText(Global.GetLang(string.Format(Global.GetLang("{0}道具不足"), goodsXmlNodeByID2.Title)), 10, 3);
								return;
							}
						}
					}
				}
			}
			if (this.m_OnClickStarBool && this.m_OnClickYuanQuanBool)
			{
				if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.GongJi)
				{
					GameInstance.Game.SenYuanSuJueXingUpLevel((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType, YuanSuJueXingQiangHuaType.GongJi, this.m_CheckShenYou.Check);
				}
				else if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.ChuanTou)
				{
					GameInstance.Game.SenYuanSuJueXingUpLevel((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType, YuanSuJueXingQiangHuaType.ChuanTou, this.m_CheckShenYou.Check);
				}
			}
		};
		this.m_BtnSuiPian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.JueXingJingHua, null, string.Empty, string.Empty);
		};
	}

	private float GetProgress(YuanSuJueXingQiangHuaType type)
	{
		float num = 0f;
		if (type == YuanSuJueXingQiangHuaType.GongJi)
		{
			if (this.m_GongJiLevel >= ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType))
			{
				return 1f;
			}
			int num2 = this.m_GongJiLevel / 4;
			if (num2 > 0)
			{
				num += this.m_LeftProgressStarStar[num2];
			}
			float num3 = (float)(this.m_GongJiLevel % 4) / 4f;
			num += num3 * (this.m_LeftProgressStarEnd[num2] - this.m_LeftProgressStarStar[num2]);
		}
		if (type == YuanSuJueXingQiangHuaType.ChuanTou)
		{
			if (this.m_ChuanTouLevel >= ConfigYuanSuJueXing.instance.MaxLevelChuanTou((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType))
			{
				return 1f;
			}
			int num4 = this.m_ChuanTouLevel / 4;
			if (num4 > 0)
			{
				num += this.m_RightProgressStarStar[num4];
			}
			float num5 = (float)(this.m_ChuanTouLevel % 4) / 4f;
			num += num5 * (this.m_RightProgressStarEnd[num4] - this.m_RightProgressStarStar[num4]);
		}
		return num;
	}

	private void RefreshYuanQuan()
	{
		if (this.m_GongJiLevel > ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType))
		{
			return;
		}
		int num = this.m_GongJiLevel / 4;
		for (int i = 0; i < this.m_ListLeftStarSp.Count; i++)
		{
			if (i < num)
			{
				if (!this.m_ListLeftStarSp[i].gameObject.activeSelf)
				{
					base.StartCoroutine<bool>(this.TeXiaoQiu(0, i, this.m_ListLeftStarSp[i].gameObject));
				}
			}
			else
			{
				this.m_ListLeftStarSp[i].gameObject.SetActive(false);
				if (this.m_LeftStarTeXiao.ContainsKey(i))
				{
					Object.Destroy(this.m_LeftStarTeXiao[i].gameObject);
					this.m_LeftStarTeXiao.Remove(i);
				}
			}
		}
		this.m_ProBarLeft.Percent = (double)this.GetProgress(YuanSuJueXingQiangHuaType.GongJi);
		if (this.m_ChuanTouLevel > ConfigYuanSuJueXing.instance.MaxLevelChuanTou((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType))
		{
			return;
		}
		num = this.m_ChuanTouLevel / 4;
		for (int j = 0; j < this.m_ListRightStarSp.Count; j++)
		{
			if (j < num)
			{
				if (!this.m_ListRightStarSp[j].gameObject.activeSelf)
				{
					base.StartCoroutine<bool>(this.TeXiaoQiu(1, j, this.m_ListRightStarSp[j].gameObject));
				}
			}
			else
			{
				this.m_ListRightStarSp[j].gameObject.SetActive(false);
				if (this.m_RightStarTeXiao.ContainsKey(j))
				{
					Object.Destroy(this.m_RightStarTeXiao[j].gameObject);
					this.m_RightStarTeXiao.Remove(j);
				}
			}
		}
		this.m_ProBarRight.Percent = (double)this.GetProgress(YuanSuJueXingQiangHuaType.ChuanTou);
	}

	private void RefreshMainStar()
	{
		int num = Mathf.Min(this.m_GongJiLevel, this.m_ChuanTouLevel) / 4;
		if (num > ConfigYuanSuJueXing.instance.MaxLevelTeShu)
		{
			return;
		}
		if (num <= 0)
		{
			this.m_LabLevel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				string.Format("Lv.{0}", num)
			});
			this.dataZuHe = ConfigYuanSuJueXing.instance.GetJingLingYuanSuShuXingLevel(1);
			this.m_LabName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				this.dataZuHe.Name
			});
			return;
		}
		this.m_LabLevel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			string.Format("Lv.{0}", num)
		});
		this.dataZuHe = ConfigYuanSuJueXing.instance.GetJingLingYuanSuShuXingLevel(num);
		if (this.dataZuHe == null)
		{
			return;
		}
		this.m_LabName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			this.dataZuHe.Name
		});
		for (int i = 0; i < this.m_ListCenterStarSp.Count; i++)
		{
			if (i < num)
			{
				if (!this.m_ListCenterStarSp[i].gameObject.activeSelf)
				{
					base.StartCoroutine<bool>(this.TeXiao(this.m_ListCenterStarSp[i].gameObject));
				}
			}
			else
			{
				this.m_ListCenterStarSp[i].gameObject.SetActive(false);
			}
		}
	}

	private IEnumerator TeXiao(GameObject gm)
	{
		if (!this.StarPanelBool)
		{
			this.m_OnClickStarBool = false;
			GameObject teXiaoGm = Global.LoadTeXiaoObj("UITeXiao/Perfabs/jinglingshenji/jingling_jieuo_bao", base.transform);
			teXiaoGm.transform.localPosition = new Vector3(-108.7f, 30f, -10f);
			teXiaoGm.AddComponent<DelayDestroy>();
			teXiaoGm.GetComponent<DelayDestroy>().delayTime = 1.8f;
			yield return new WaitForSeconds(1.2f);
			GameObject teXiaoShanGuang = Global.LoadTeXiaoObj("UITeXiao/Perfabs/jinglingshenji/jingling_jiesuo", gm.transform.parent);
			teXiaoShanGuang.transform.localPosition = new Vector3(gm.transform.localPosition.x, gm.transform.localPosition.y, gm.transform.localPosition.z - 10f);
			teXiaoShanGuang.transform.localRotation = gm.transform.localRotation;
			teXiaoShanGuang.AddComponent<DelayDestroy>();
			teXiaoShanGuang.GetComponent<DelayDestroy>().delayTime = 1f;
			yield return new WaitForSeconds(0.7f);
			GameObject teXiaoUpQuan = Global.LoadTeXiaoObj("UITeXiao/Perfabs/jinglingshenji/jingling_baoshi_effect", gm.transform.parent);
			teXiaoUpQuan.transform.localPosition = new Vector3(gm.transform.localPosition.x, gm.transform.localPosition.y, gm.transform.localPosition.z - 15f);
			teXiaoUpQuan.transform.localRotation = gm.transform.localRotation;
			teXiaoUpQuan.AddComponent<DelayDestroy>();
			teXiaoUpQuan.GetComponent<DelayDestroy>().delayTime = 1f;
			yield return new WaitForSeconds(0.9f);
			gm.gameObject.SetActive(true);
			this.m_OnClickStarBool = true;
		}
		else
		{
			gm.gameObject.SetActive(true);
		}
		this.RefreshMainStar();
		yield break;
	}

	private IEnumerator TeXiaoQiu(int type, int key, GameObject gm)
	{
		if (!this.StarPanelBool)
		{
			this.m_OnClickYuanQuanBool = false;
			GameObject teXiaoGm = Global.LoadTeXiaoObj("UITeXiao/Perfabs/jinglingshenji/jingling_dianliang_qiu", gm.transform.parent);
			teXiaoGm.transform.localPosition = new Vector3(gm.transform.localPosition.x, gm.transform.localPosition.y, gm.transform.localPosition.z - 5f);
			teXiaoGm.AddComponent<DelayDestroy>();
			teXiaoGm.GetComponent<DelayDestroy>().delayTime = 1f;
			yield return new WaitForSeconds(0.2f);
		}
		GameObject QiuGm = Global.LoadTeXiaoObj("UITeXiao/Perfabs/jinglingshenji/jingling_lanqiu", gm.transform.parent);
		QiuGm.transform.localPosition = new Vector3(gm.transform.localPosition.x, gm.transform.localPosition.y, gm.transform.localPosition.z - 0.2f);
		gm.gameObject.SetActive(true);
		if (type == 0)
		{
			if (!this.m_LeftStarTeXiao.ContainsKey(key))
			{
				this.m_LeftStarTeXiao.Add(key, QiuGm);
			}
		}
		else if (type == 1 && !this.m_RightStarTeXiao.ContainsKey(key))
		{
			this.m_RightStarTeXiao.Add(key, QiuGm);
		}
		this.m_OnClickYuanQuanBool = true;
		yield break;
	}

	private void ReffreshData()
	{
		if (this.m_GongJiLevel >= ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType) && this.m_ChuanTouLevel >= ConfigYuanSuJueXing.instance.MaxLevelChuanTou((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType))
		{
			this.m_SpShuXingBianKuang.transform.localScale = new Vector3(this.m_SpShuXingBianKuang.transform.localScale.x, 478.2f, 1f);
			this.m_SpShuXingBack.transform.localScale = new Vector3(this.m_SpShuXingBack.transform.localScale.x, 473f, 1f);
			this.m_BoxShuXing.center = new Vector3(this.m_BoxShuXing.center.x, -178f, this.m_BoxShuXing.center.z);
			this.m_BoxShuXing.size = new Vector3(this.m_BoxShuXing.size.x, 380f, this.m_BoxShuXing.size.z);
			this.m_PanelShuXing.clipRange = new Vector4(this.m_PanelShuXing.clipRange.x, -178f, this.m_PanelShuXing.clipRange.z, 380f);
			this.m_PanelShuXing.transform.localPosition = new Vector3(0f, -4f, 0f);
			this.m_QiangHuaPanel.SetActive(false);
			this.m_QiangHuaTeShuMax.gameObject.SetActive(true);
			return;
		}
		if (this.m_GongJiLevel >= ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType))
		{
			this.m_BtnGongJiQiangHua.disabledSprite = "qianghua1_onrmal";
			this.m_BtnGongJiQiangHua.isEnabled = false;
			this.m_BtnChuanTouQiangHua.disabledSprite = "qianghua2_press";
			this.m_BtnChuanTouQiangHua.isEnabled = false;
			this.m_QiangHuaType = YuanSuJueXingQiangHuaType.ChuanTou;
		}
		else if (this.m_ChuanTouLevel >= ConfigYuanSuJueXing.instance.MaxLevelChuanTou((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType))
		{
			this.m_BtnGongJiQiangHua.disabledSprite = "qianghua1_press";
			this.m_BtnGongJiQiangHua.isEnabled = false;
			this.m_BtnChuanTouQiangHua.disabledSprite = "qianghua2_onrmal";
			this.m_BtnChuanTouQiangHua.isEnabled = false;
			this.m_QiangHuaType = YuanSuJueXingQiangHuaType.GongJi;
		}
		else if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.GongJi)
		{
			this.m_BtnGongJiQiangHua.normalSprite = "qianghua1_press";
			this.m_BtnGongJiQiangHua.target.spriteName = "qianghua1_press";
			this.m_BtnChuanTouQiangHua.normalSprite = "qianghua2_onrmal";
			this.m_BtnChuanTouQiangHua.target.spriteName = "qianghua2_onrmal";
		}
		else if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.ChuanTou)
		{
			this.m_BtnGongJiQiangHua.normalSprite = "qianghua1_onrmal";
			this.m_BtnGongJiQiangHua.target.spriteName = "qianghua1_onrmal";
			this.m_BtnChuanTouQiangHua.normalSprite = "qianghua2_press";
			this.m_BtnChuanTouQiangHua.target.spriteName = "qianghua2_press";
		}
		JingLingYuanSuVO jingLingYuanSuVO = null;
		if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.GongJi)
		{
			jingLingYuanSuVO = ConfigYuanSuJueXing.instance.GetDatalGongJiAndChuanTou(YuanSuJueXingQiangHuaType.GongJi);
		}
		else if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.ChuanTou)
		{
			jingLingYuanSuVO = ConfigYuanSuJueXing.instance.GetDatalGongJiAndChuanTou(YuanSuJueXingQiangHuaType.ChuanTou);
		}
		if (jingLingYuanSuVO == null)
		{
			return;
		}
		this.m_QiangHuaPanel.SetActive(true);
		this.m_LabSuccessNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			string.Format("{0}%", (jingLingYuanSuVO.Success * 100f).ToString("f0"))
		});
		if (jingLingYuanSuVO.JieXingCurrency <= Global.GetRoleOwnNumByMoneyType(144))
		{
			this.m_LabXiaoHaoNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				string.Format("{0}", jingLingYuanSuVO.JieXingCurrency)
			});
		}
		else
		{
			this.m_LabXiaoHaoNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				string.Format("{0}", jingLingYuanSuVO.JieXingCurrency)
			});
		}
		string[] array = jingLingYuanSuVO.NeedGoods.Split(new char[]
		{
			'|'
		});
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			if (i < array.Length - 1)
			{
				text = text + array[i] + ",0,0,0,0,0|";
			}
			else
			{
				text = text + array[i] + ",0,0,0,0,0";
				if (!string.IsNullOrEmpty(jingLingYuanSuVO.Failtofail))
				{
					text += string.Format("|{0},0,0,0,0,0", jingLingYuanSuVO.Failtofail);
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(jingLingYuanSuVO.Failtofail.Split(new char[]
					{
						','
					})[0].SafeToInt32(0));
					if (goodsXmlNodeByID != null)
					{
						this.m_CheckShenYou.Text = Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							Global.GetLang(string.Format(Global.GetLang("始终使用{0}"), goodsXmlNodeByID.Title))
						});
					}
					this.m_CheckShenYou.gameObject.SetActive(true);
				}
				else
				{
					this.m_CheckShenYou.gameObject.SetActive(false);
				}
			}
		}
		Super.LoadGoodsList(text, this.m_ListGoods.ItemsSource);
		GGoodIcon[] componentsInChildren = this.m_ListGoods.GetComponentsInChildren<GGoodIcon>();
		if (componentsInChildren != null)
		{
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				GoodsData goodsData = componentsInChildren[j].ItemObject as GoodsData;
				if (goodsData != null)
				{
					int num = 0;
					if (Global.Data.roleData.GoodsDataList != null)
					{
						for (int k = 0; k < Global.Data.roleData.GoodsDataList.Count; k++)
						{
							if (Global.Data.roleData.GoodsDataList[k].GoodsID == goodsData.GoodsID)
							{
								num += Global.Data.roleData.GoodsDataList[k].GCount;
							}
						}
					}
					if (num >= goodsData.GCount)
					{
						if (num > 999)
						{
							componentsInChildren[j].SecondText.text = "999/" + goodsData.GCount.ToString();
						}
						else
						{
							componentsInChildren[j].SecondText.text = num.ToString() + "/" + goodsData.GCount.ToString();
						}
						componentsInChildren[j].SecondText.textColor = 4074946303U;
					}
					else
					{
						componentsInChildren[j].SecondText.text = num.ToString() + "/" + goodsData.GCount.ToString();
						componentsInChildren[j].SecondText.textColor = 16711680U;
					}
				}
			}
		}
	}

	private void Refresh(int levelGongJi, int levelChuanTou)
	{
		for (int i = 0; i < this.m_ListGameLab.Count; i++)
		{
			Object.Destroy(this.m_ListGameLab[i]);
		}
		this.m_ListGameLab.Clear();
		float num = 0f;
		float num2 = 10f;
		JingLingYuanSuVO jingLingYuanSuVO;
		if (this.m_GongJiLevel >= ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType))
		{
			jingLingYuanSuVO = ConfigYuanSuJueXing.instance.GetYuanSuShuXingGongJiLevel(ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType));
			this.m_SpMaxGongJi.gameObject.SetActive(true);
		}
		else
		{
			jingLingYuanSuVO = ConfigYuanSuJueXing.instance.GetYuanSuShuXingGongJiLevel(levelGongJi);
			this.m_SpMaxGongJi.gameObject.SetActive(false);
		}
		if (jingLingYuanSuVO == null)
		{
			return;
		}
		this.m_LabGongJiLv.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format("Lv.{0}", jingLingYuanSuVO.QiangHuaLevel)
		});
		this.m_ImgGongJi.URL = string.Format("NetImages/GameRes/Images/YuanSuJueXing/Skill_{0}_{1}.png", jingLingYuanSuVO.YuanSuType, jingLingYuanSuVO.ShuXingType);
		string[] array = jingLingYuanSuVO.Attribute.Split(new char[]
		{
			'|'
		});
		string[] addYuanSuShuXing = ConfigYuanSuJueXing.instance.GetAddYuanSuShuXing(YuanSuJueXingQiangHuaType.GongJi, levelGongJi);
		GameObject gameObject = U3DUtils.Clone(this.m_LabShuXingTypeTitle.transform.parent.gameObject, this.m_LabShuXingTypeTitle.gameObject);
		this.m_ListGameLab.Add(gameObject);
		if (gameObject != null && gameObject.GetComponent<UILabel>() != null)
		{
			gameObject.GetComponent<UILabel>().text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("攻击属性加成")
			});
			gameObject.transform.localPosition = new Vector3(0f, num, -1f);
			float y = gameObject.GetComponent<UILabel>().relativeSize.y;
			num = num - gameObject.GetComponent<UILabel>().relativeSize.y * gameObject.GetComponent<UILabel>().transform.localScale.y - num2;
		}
		GameObject gameObject2 = U3DUtils.Clone(this.m_LabShuXingTypeContent.transform.parent.gameObject, this.m_LabShuXingTypeContent.gameObject);
		this.m_ListGameLab.Add(gameObject2);
		if (levelGongJi <= 0)
		{
			gameObject2.GetComponent<UILabel>().text = Global.GetLang("未激活");
			gameObject2.transform.localPosition = new Vector3(0f, num, -1f);
			num = num - gameObject2.GetComponent<UILabel>().relativeSize.y * gameObject2.GetComponent<UILabel>().transform.localScale.y - num2 / 3f;
			GameObject gameObject3 = U3DUtils.Clone(this.m_LabShuXingTypeContent.transform.parent.gameObject, this.m_LabShuXingTypeFenGeFu.gameObject);
			gameObject3.gameObject.SetActive(true);
			this.m_ListGameLab.Add(gameObject3);
			gameObject3.transform.localPosition = new Vector3(-11f, num, -1f);
			num -= num2 * 1.5f;
		}
		else if (gameObject2 != null && gameObject2.GetComponent<UILabel>() != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int j = 0; j < array.Length; j++)
			{
				if (!ConfigExtPropIndexes.GetPercentByWord(array[j].Split(new char[]
				{
					','
				})[0]))
				{
					if (j < array.Length - 1)
					{
						stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array[j].Split(new char[]
							{
								','
							})[0], false) + ":"
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							array[j].Split(new char[]
							{
								','
							})[1].SafeToInt32(0)
						}) + Environment.NewLine);
						if (levelGongJi < ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)jingLingYuanSuVO.YuanSuType))
						{
							stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"17e43e",
								addYuanSuShuXing[j].Split(new char[]
								{
									','
								})[1].SafeToInt32(0)
							}) + Environment.NewLine);
							GameObject gameObject4 = U3DUtils.Clone(this.m_LabShuXingTypeJianTou.transform.parent.gameObject, this.m_LabShuXingTypeJianTou.gameObject);
							this.m_ListGameLab.Add(gameObject4);
							gameObject4.transform.localPosition = new Vector3(160f, num - (float)(gameObject2.GetComponent<TextBlock>().LineHeight / 2) - (float)j * ((float)(gameObject2.GetComponent<TextBlock>().LineHeight / 2) + gameObject2.GetComponent<UILabel>().transform.localScale.y), -1f);
						}
					}
					else
					{
						stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array[j].Split(new char[]
							{
								','
							})[0], false) + ":"
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							array[j].Split(new char[]
							{
								','
							})[1].SafeToInt32(0)
						}));
						if (levelGongJi < ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)jingLingYuanSuVO.YuanSuType))
						{
							stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"17e43e",
								addYuanSuShuXing[j].Split(new char[]
								{
									','
								})[1].SafeToInt32(0)
							}));
							GameObject gameObject5 = U3DUtils.Clone(this.m_LabShuXingTypeJianTou.transform.parent.gameObject, this.m_LabShuXingTypeJianTou.gameObject);
							this.m_ListGameLab.Add(gameObject5);
							gameObject5.transform.localPosition = new Vector3(160f, num - (float)(gameObject2.GetComponent<TextBlock>().LineHeight / 2) - (float)j * ((float)(gameObject2.GetComponent<TextBlock>().LineHeight / 2) + gameObject2.GetComponent<UILabel>().transform.localScale.y), -1f);
						}
					}
				}
				else if (j < array.Length - 1)
				{
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array[j].Split(new char[]
						{
							','
						})[0], false) + ":"
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						(float.Parse(array[j].Split(new char[]
						{
							','
						})[1]) * 100f).ToString("f0") + "%"
					}) + Environment.NewLine);
					if (levelGongJi < ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)jingLingYuanSuVO.YuanSuType))
					{
						stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							(float.Parse(addYuanSuShuXing[j].Split(new char[]
							{
								','
							})[1]) * 100f).ToString("f0") + "%"
						}) + Environment.NewLine);
						GameObject gameObject6 = U3DUtils.Clone(this.m_LabShuXingTypeJianTou.transform.parent.gameObject, this.m_LabShuXingTypeJianTou.gameObject);
						this.m_ListGameLab.Add(gameObject6);
						gameObject6.transform.localPosition = new Vector3(160f, num - (float)(gameObject2.GetComponent<TextBlock>().LineHeight / 2) - (float)j * ((float)(gameObject2.GetComponent<TextBlock>().LineHeight / 2) + gameObject2.GetComponent<UILabel>().transform.localScale.y), -1f);
					}
				}
				else
				{
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array[j].Split(new char[]
						{
							','
						})[0], false) + ":"
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						(float.Parse(array[j].Split(new char[]
						{
							','
						})[1]) * 100f).ToString("f0") + "%"
					}));
					if (levelGongJi < ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)jingLingYuanSuVO.YuanSuType))
					{
						stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							(float.Parse(addYuanSuShuXing[j].Split(new char[]
							{
								','
							})[1]) * 100f).ToString("f0") + "%"
						}));
						GameObject gameObject7 = U3DUtils.Clone(this.m_LabShuXingTypeJianTou.transform.parent.gameObject, this.m_LabShuXingTypeJianTou.gameObject);
						this.m_ListGameLab.Add(gameObject7);
						gameObject7.transform.localPosition = new Vector3(160f, num - (float)(gameObject2.GetComponent<TextBlock>().LineHeight / 2) - (float)j * ((float)(gameObject2.GetComponent<TextBlock>().LineHeight / 2) + gameObject2.GetComponent<UILabel>().transform.localScale.y), -1f);
					}
				}
			}
			gameObject2.GetComponent<UILabel>().text = stringBuilder.ToString();
			if (levelGongJi < ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)jingLingYuanSuVO.YuanSuType))
			{
				GameObject gameObject8 = U3DUtils.Clone(this.m_LabShuXingTypeContent.transform.parent.gameObject, this.m_LabShuXingTypeContent.gameObject);
				this.m_ListGameLab.Add(gameObject8);
				gameObject8.GetComponent<UILabel>().text = stringBuilder2.ToString();
				gameObject8.transform.localPosition = new Vector3(180f, num, -1f);
			}
			gameObject2.transform.localPosition = new Vector3(0f, num, -1f);
			num = num - gameObject2.GetComponent<UILabel>().relativeSize.y * gameObject2.GetComponent<UILabel>().transform.localScale.y - num2 / 3f;
			GameObject gameObject9 = U3DUtils.Clone(this.m_LabShuXingTypeContent.transform.parent.gameObject, this.m_LabShuXingTypeFenGeFu.gameObject);
			gameObject9.gameObject.SetActive(true);
			this.m_ListGameLab.Add(gameObject9);
			gameObject9.transform.localPosition = new Vector3(-11f, num, -1f);
			num -= num2 * 1.5f;
		}
		if (levelChuanTou >= ConfigYuanSuJueXing.instance.MaxLevelChuanTou((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType))
		{
			this.m_SpMaxChuanTou.gameObject.SetActive(true);
			jingLingYuanSuVO = ConfigYuanSuJueXing.instance.GetYuanSuShuXingChuanTouLevel(ConfigYuanSuJueXing.instance.MaxLevelGongJi((YuanSuJueXingType)Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType));
		}
		else
		{
			this.m_SpMaxChuanTou.gameObject.SetActive(false);
			jingLingYuanSuVO = ConfigYuanSuJueXing.instance.GetYuanSuShuXingChuanTouLevel(levelChuanTou);
		}
		if (jingLingYuanSuVO == null)
		{
			return;
		}
		this.m_LabChuanTouLv.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format("Lv.{0}", jingLingYuanSuVO.QiangHuaLevel)
		});
		this.m_ImgChuanTou.URL = string.Format("NetImages/GameRes/Images/YuanSuJueXing/Skill_{0}_{1}.png", jingLingYuanSuVO.YuanSuType, jingLingYuanSuVO.ShuXingType);
		this.m_ImgLogo.URL = string.Format("NetImages/GameRes/Images/YuanSuJueXing/Logo_UpLevel_{0}.png", jingLingYuanSuVO.YuanSuType);
		string[] array2 = jingLingYuanSuVO.Attribute.Split(new char[]
		{
			'|'
		});
		string[] addYuanSuShuXing2 = ConfigYuanSuJueXing.instance.GetAddYuanSuShuXing(YuanSuJueXingQiangHuaType.ChuanTou, levelChuanTou);
		GameObject gameObject10 = U3DUtils.Clone(this.m_LabShuXingTypeTitle.transform.parent.gameObject, this.m_LabShuXingTypeTitle.gameObject);
		this.m_ListGameLab.Add(gameObject10);
		if (gameObject10 != null && gameObject10.GetComponent<UILabel>() != null)
		{
			gameObject10.GetComponent<UILabel>().text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("穿透属性加成")
			});
			gameObject10.transform.localPosition = new Vector3(0f, num, -1f);
			num = num - gameObject10.GetComponent<UILabel>().relativeSize.y * gameObject10.GetComponent<UILabel>().transform.localScale.y - num2;
		}
		GameObject gameObject11 = U3DUtils.Clone(this.m_LabShuXingTypeContent.transform.parent.gameObject, this.m_LabShuXingTypeContent.gameObject);
		this.m_ListGameLab.Add(gameObject11);
		if (levelChuanTou <= 0)
		{
			gameObject11.GetComponent<UILabel>().text = Global.GetLang("未激活");
			gameObject11.transform.localPosition = new Vector3(0f, num, -1f);
			num = num - gameObject11.GetComponent<UILabel>().relativeSize.y * gameObject11.GetComponent<UILabel>().transform.localScale.y - num2 / 3f;
		}
		else if (gameObject11 != null && gameObject11.GetComponent<UILabel>() != null)
		{
			StringBuilder stringBuilder3 = new StringBuilder();
			StringBuilder stringBuilder4 = new StringBuilder();
			for (int k = 0; k < array2.Length; k++)
			{
				if (!ConfigExtPropIndexes.GetPercentByWord(array2[k].Split(new char[]
				{
					','
				})[0]))
				{
					if (k < array2.Length - 1)
					{
						stringBuilder3.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array2[k].Split(new char[]
							{
								','
							})[0], false) + Global.GetColorStringForNGUIText(new object[]
							{
								"dac7ae",
								array2[k].Split(new char[]
								{
									','
								})[1].SafeToInt32(0)
							})
						}) + Environment.NewLine);
						if (levelChuanTou < ConfigYuanSuJueXing.instance.MaxLevelChuanTou((YuanSuJueXingType)jingLingYuanSuVO.YuanSuType))
						{
							stringBuilder4.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"17e43e",
								addYuanSuShuXing2[k].Split(new char[]
								{
									','
								})[1].SafeToInt32(0)
							}) + Environment.NewLine);
							GameObject gameObject12 = U3DUtils.Clone(this.m_LabShuXingTypeJianTou.transform.parent.gameObject, this.m_LabShuXingTypeJianTou.gameObject);
							this.m_ListGameLab.Add(gameObject12);
							gameObject12.transform.localPosition = new Vector3(160f, num - (float)(gameObject11.GetComponent<TextBlock>().LineHeight / 2) - (float)k * ((float)(gameObject11.GetComponent<TextBlock>().LineHeight / 2) + gameObject11.GetComponent<UILabel>().transform.localScale.y), -1f);
						}
					}
					else
					{
						stringBuilder3.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array2[k].Split(new char[]
							{
								','
							})[0], false) + Global.GetColorStringForNGUIText(new object[]
							{
								"dac7ae",
								array2[k].Split(new char[]
								{
									','
								})[1].SafeToInt32(0)
							})
						}));
						if (levelChuanTou < ConfigYuanSuJueXing.instance.MaxLevelChuanTou((YuanSuJueXingType)jingLingYuanSuVO.YuanSuType))
						{
							stringBuilder4.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"17e43e",
								addYuanSuShuXing2[k].Split(new char[]
								{
									','
								})[1].SafeToInt32(0)
							}));
							GameObject gameObject13 = U3DUtils.Clone(this.m_LabShuXingTypeJianTou.transform.parent.gameObject, this.m_LabShuXingTypeJianTou.gameObject);
							this.m_ListGameLab.Add(gameObject13);
							gameObject13.transform.localPosition = new Vector3(160f, num - (float)(gameObject11.GetComponent<TextBlock>().LineHeight / 2) - (float)k * ((float)(gameObject11.GetComponent<TextBlock>().LineHeight / 2) + gameObject11.GetComponent<UILabel>().transform.localScale.y), -1f);
						}
					}
				}
				else if (k < array2.Length - 1)
				{
					stringBuilder3.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array2[k].Split(new char[]
						{
							','
						})[0], false)
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						(float.Parse(array2[k].Split(new char[]
						{
							','
						})[1]) * 100f).ToString("f0") + "%"
					}) + Environment.NewLine);
					if (levelChuanTou < ConfigYuanSuJueXing.instance.MaxLevelChuanTou((YuanSuJueXingType)jingLingYuanSuVO.YuanSuType))
					{
						stringBuilder4.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							(float.Parse(addYuanSuShuXing2[k].Split(new char[]
							{
								','
							})[1]) * 100f).ToString("f0") + "%"
						}) + Environment.NewLine);
						GameObject gameObject14 = U3DUtils.Clone(this.m_LabShuXingTypeJianTou.transform.parent.gameObject, this.m_LabShuXingTypeJianTou.gameObject);
						this.m_ListGameLab.Add(gameObject14);
						gameObject14.transform.localPosition = new Vector3(160f, num - (float)(gameObject11.GetComponent<TextBlock>().LineHeight / 2) - (float)k * ((float)(gameObject11.GetComponent<TextBlock>().LineHeight / 2) + gameObject11.GetComponent<UILabel>().transform.localScale.y), -1f);
					}
				}
				else
				{
					stringBuilder3.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array2[k].Split(new char[]
						{
							','
						})[0], false)
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						(float.Parse(array2[k].Split(new char[]
						{
							','
						})[1]) * 100f).ToString("f0") + "%"
					}));
					if (levelChuanTou < ConfigYuanSuJueXing.instance.MaxLevelChuanTou((YuanSuJueXingType)jingLingYuanSuVO.YuanSuType))
					{
						stringBuilder4.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							(float.Parse(addYuanSuShuXing2[k].Split(new char[]
							{
								','
							})[1]) * 100f).ToString("f0") + "%"
						}));
						GameObject gameObject15 = U3DUtils.Clone(this.m_LabShuXingTypeJianTou.transform.parent.gameObject, this.m_LabShuXingTypeJianTou.gameObject);
						this.m_ListGameLab.Add(gameObject15);
						gameObject15.transform.localPosition = new Vector3(160f, num - (float)(gameObject11.GetComponent<TextBlock>().LineHeight / 2) - (float)k * ((float)(gameObject11.GetComponent<TextBlock>().LineHeight / 2) + gameObject11.GetComponent<UILabel>().transform.localScale.y), -1f);
					}
				}
			}
			if (levelChuanTou < ConfigYuanSuJueXing.instance.MaxLevelChuanTou((YuanSuJueXingType)jingLingYuanSuVO.YuanSuType))
			{
				GameObject gameObject16 = U3DUtils.Clone(this.m_LabShuXingTypeContent.transform.parent.gameObject, this.m_LabShuXingTypeContent.gameObject);
				this.m_ListGameLab.Add(gameObject16);
				gameObject16.GetComponent<UILabel>().text = stringBuilder4.ToString();
				gameObject16.transform.localPosition = new Vector3(180f, num, -1f);
			}
			gameObject11.GetComponent<UILabel>().text = stringBuilder3.ToString();
			gameObject11.transform.localPosition = new Vector3(0f, num, -1f);
			num = num - gameObject11.GetComponent<UILabel>().relativeSize.y * gameObject11.GetComponent<UILabel>().transform.localScale.y - num2 / 3f;
			GameObject gameObject17 = U3DUtils.Clone(this.m_LabShuXingTypeContent.transform.parent.gameObject, this.m_LabShuXingTypeFenGeFu.gameObject);
			gameObject17.gameObject.SetActive(true);
			this.m_ListGameLab.Add(gameObject17);
			gameObject17.transform.localPosition = new Vector3(-11f, num, -1f);
			num -= num2 * 1.5f;
		}
		int[] array3 = new int[]
		{
			levelGongJi,
			levelChuanTou
		};
		int num3 = Mathf.Min(array3) / 4;
		JingLingYuanSuShuXingVO jingLingYuanSuShuXingLevel;
		if (num3 >= ConfigYuanSuJueXing.instance.MaxLevelTeShu)
		{
			jingLingYuanSuShuXingLevel = ConfigYuanSuJueXing.instance.GetJingLingYuanSuShuXingLevel(ConfigYuanSuJueXing.instance.MaxLevelTeShu);
		}
		else
		{
			jingLingYuanSuShuXingLevel = ConfigYuanSuJueXing.instance.GetJingLingYuanSuShuXingLevel(num3);
		}
		string[] array4 = null;
		if (jingLingYuanSuShuXingLevel != null)
		{
			array4 = jingLingYuanSuShuXingLevel.AcetiveElement.Split(new char[]
			{
				'|'
			});
			GameObject gameObject18 = U3DUtils.Clone(this.m_LabShuXingTypeTitle.transform.parent.gameObject, this.m_LabShuXingTypeTitle.gameObject);
			this.m_ListGameLab.Add(gameObject18);
			if (gameObject18 != null && gameObject18.GetComponent<UILabel>() != null)
			{
				gameObject18.GetComponent<UILabel>().text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format("{0}(LV{1})", jingLingYuanSuShuXingLevel.Name, jingLingYuanSuShuXingLevel.Level)
				});
				gameObject18.transform.localPosition = new Vector3(0f, num, -1f);
				num = num - gameObject18.GetComponent<UILabel>().relativeSize.y * gameObject18.GetComponent<UILabel>().transform.localScale.y - num2;
			}
		}
		string[] addTeShuShuXing = ConfigYuanSuJueXing.instance.GetAddTeShuShuXing(num3 + 1);
		GameObject gameObject19 = U3DUtils.Clone(this.m_LabShuXingTypeContent.transform.parent.gameObject, this.m_LabShuXingTypeContent.gameObject);
		this.m_ListGameLab.Add(gameObject19);
		if (gameObject19 != null && gameObject19.GetComponent<UILabel>() != null)
		{
			StringBuilder stringBuilder5 = new StringBuilder();
			StringBuilder stringBuilder6 = new StringBuilder();
			int num4 = 0;
			if (num3 < ConfigYuanSuJueXing.instance.MaxLevelTeShu)
			{
				num4 = addTeShuShuXing.Length;
			}
			else if (num3 >= ConfigYuanSuJueXing.instance.MaxLevelTeShu)
			{
				num4 = array4.Length;
			}
			for (int l = 0; l < num4; l++)
			{
				if (array4 != null)
				{
					if (!ConfigExtPropIndexes.GetPercentByWord(array4[l].Split(new char[]
					{
						','
					})[0]))
					{
						if (l < num4 - 1)
						{
							stringBuilder5.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"e3b36c",
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array4[l].Split(new char[]
								{
									','
								})[0], false) + Global.GetColorStringForNGUIText(new object[]
								{
									"dac7ae",
									array4[l].Split(new char[]
									{
										','
									})[1]
								})
							}) + Environment.NewLine);
						}
						else
						{
							stringBuilder5.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"e3b36c",
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array4[l].Split(new char[]
								{
									','
								})[0], false) + Global.GetColorStringForNGUIText(new object[]
								{
									"dac7ae",
									array4[l].Split(new char[]
									{
										','
									})[1]
								})
							}));
						}
					}
					else if (l < num4 - 1)
					{
						stringBuilder5.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array4[l].Split(new char[]
							{
								','
							})[0], false)
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							(float.Parse(array4[l].Split(new char[]
							{
								','
							})[1]) * 100f).ToString("f0") + "%"
						}) + Environment.NewLine);
					}
					else
					{
						stringBuilder5.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array4[l].Split(new char[]
							{
								','
							})[0], false)
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							(float.Parse(array4[l].Split(new char[]
							{
								','
							})[1]) * 100f).ToString("f0") + "%"
						}));
					}
				}
				if (addTeShuShuXing != null)
				{
					if (!ConfigExtPropIndexes.GetPercentByWord(addTeShuShuXing[l].Split(new char[]
					{
						','
					})[0]))
					{
						if (l < num4 - 1)
						{
							stringBuilder6.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"808081",
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(addTeShuShuXing[l].Split(new char[]
								{
									','
								})[0], false) + Global.GetColorStringForNGUIText(new object[]
								{
									"808081",
									addTeShuShuXing[l].Split(new char[]
									{
										','
									})[1]
								})
							}) + Environment.NewLine);
						}
						else
						{
							stringBuilder6.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"808081",
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(addTeShuShuXing[l].Split(new char[]
								{
									','
								})[0], false) + Global.GetColorStringForNGUIText(new object[]
								{
									"808081",
									addTeShuShuXing[l].Split(new char[]
									{
										','
									})[1]
								})
							}));
						}
					}
					else if (l < num4 - 1)
					{
						stringBuilder6.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"808081",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(addTeShuShuXing[l].Split(new char[]
							{
								','
							})[0], false)
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"808081",
							(float.Parse(addTeShuShuXing[l].Split(new char[]
							{
								','
							})[1]) * 100f).ToString("f0") + "%"
						}) + Environment.NewLine);
					}
					else
					{
						stringBuilder6.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"808081",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(addTeShuShuXing[l].Split(new char[]
							{
								','
							})[0], false)
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"808081",
							(float.Parse(addTeShuShuXing[l].Split(new char[]
							{
								','
							})[1]) * 100f).ToString("f0") + "%"
						}));
					}
				}
			}
			if (!string.IsNullOrEmpty(stringBuilder5.ToString()))
			{
				gameObject19.GetComponent<UILabel>().text = stringBuilder5.ToString();
				gameObject19.transform.localPosition = new Vector3(0f, num, -1f);
				num = num - gameObject19.GetComponent<UILabel>().relativeSize.y * gameObject19.GetComponent<UILabel>().transform.localScale.y - num2;
			}
			if (num3 < ConfigYuanSuJueXing.instance.MaxLevelTeShu)
			{
				GameObject gameObject20 = U3DUtils.Clone(this.m_LabShuXingTypeTitle.transform.parent.gameObject, this.m_LabShuXingTypeTitle.gameObject);
				this.m_ListGameLab.Add(gameObject20);
				if (gameObject20 != null && gameObject20.GetComponent<UILabel>() != null)
				{
					if (jingLingYuanSuShuXingLevel == null)
					{
						JingLingYuanSuShuXingVO jingLingYuanSuShuXingLevel2 = ConfigYuanSuJueXing.instance.GetJingLingYuanSuShuXingLevel(1);
						gameObject20.GetComponent<UILabel>().text = Global.GetColorStringForNGUIText(new object[]
						{
							"808081",
							string.Format(Global.GetLang("{0}Lv{1}（等级均达到{2}）"), jingLingYuanSuShuXingLevel2.Name, num3 + 1, (num3 + 1) * 4)
						});
					}
					else
					{
						gameObject20.GetComponent<UILabel>().text = Global.GetColorStringForNGUIText(new object[]
						{
							"808081",
							string.Format(Global.GetLang("{0}Lv{1}（等级均达到{2}）"), jingLingYuanSuShuXingLevel.Name, num3 + 1, (num3 + 1) * 4)
						});
					}
					gameObject20.transform.localPosition = new Vector3(0f, num, -1f);
					num = num - gameObject20.GetComponent<UILabel>().relativeSize.y * gameObject20.GetComponent<UILabel>().transform.localScale.y - num2;
				}
				GameObject gameObject21 = U3DUtils.Clone(this.m_LabShuXingTypeContent.transform.parent.gameObject, this.m_LabShuXingTypeContent.gameObject);
				this.m_ListGameLab.Add(gameObject21);
				gameObject21.GetComponent<UILabel>().text = stringBuilder6.ToString();
				gameObject21.transform.localPosition = new Vector3(0f, num, -1f);
			}
		}
	}

	public void RefreshData(string[] data)
	{
		if (data[0].SafeToInt32(0) >= 0)
		{
			int[] activeIDs = Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs;
			if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType == 1)
			{
				if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.GongJi)
				{
					if (data[1].SafeToInt32(0) > activeIDs[0])
					{
						Super.HintMainText(Global.GetLang("升级成功"), 10, 3);
					}
					else
					{
						Super.HintMainText(Global.GetLang("升级失败"), 10, 3);
					}
					activeIDs[0] = data[1].SafeToInt32(0);
				}
				else if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.ChuanTou)
				{
					if (data[1].SafeToInt32(0) > activeIDs[1])
					{
						Super.HintMainText(Global.GetLang("升级成功"), 10, 3);
					}
					else
					{
						Super.HintMainText(Global.GetLang("升级失败"), 10, 3);
					}
					activeIDs[1] = data[1].SafeToInt32(0);
				}
			}
			else if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType == 2)
			{
				if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.GongJi)
				{
					if (data[1].SafeToInt32(0) > activeIDs[2])
					{
						Super.HintMainText(Global.GetLang("升级成功"), 10, 3);
					}
					else
					{
						Super.HintMainText(Global.GetLang("升级失败"), 10, 3);
					}
					activeIDs[2] = data[1].SafeToInt32(0);
				}
				else if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.ChuanTou)
				{
					if (data[1].SafeToInt32(0) > activeIDs[3])
					{
						Super.HintMainText(Global.GetLang("升级成功"), 10, 3);
					}
					else
					{
						Super.HintMainText(Global.GetLang("升级失败"), 10, 3);
					}
					activeIDs[3] = data[1].SafeToInt32(0);
				}
			}
			else if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType == 3)
			{
				if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.GongJi)
				{
					if (data[1].SafeToInt32(0) > activeIDs[4])
					{
						Super.HintMainText(Global.GetLang("升级成功"), 10, 3);
					}
					else
					{
						Super.HintMainText(Global.GetLang("升级失败"), 10, 3);
					}
					activeIDs[4] = data[1].SafeToInt32(0);
				}
				else if (this.m_QiangHuaType == YuanSuJueXingQiangHuaType.ChuanTou)
				{
					if (data[1].SafeToInt32(0) > activeIDs[5])
					{
						Super.HintMainText(Global.GetLang("升级成功"), 10, 3);
					}
					else
					{
						Super.HintMainText(Global.GetLang("升级失败"), 10, 3);
					}
					activeIDs[5] = data[1].SafeToInt32(0);
				}
			}
			this.m_LabSuiPianNumber.text = Global.GetLang(Global.GetRoleOwnNumByMoneyType(144).ToString());
			Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs = activeIDs;
			this.m_GongJiLevel = ConfigYuanSuJueXing.instance.GetDatalGongJiAndChuanTou(YuanSuJueXingQiangHuaType.GongJi).QiangHuaLevel;
			this.m_ChuanTouLevel = ConfigYuanSuJueXing.instance.GetDatalGongJiAndChuanTou(YuanSuJueXingQiangHuaType.ChuanTou).QiangHuaLevel;
			this.Refresh(this.m_GongJiLevel, this.m_ChuanTouLevel);
			this.RefreshYuanQuan();
			this.RefreshMainStar();
			this.ReffreshData();
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(data[0].SafeToInt32(0), false, false)), 10, 3);
		}
	}

	[SerializeField]
	private GButton m_BtnClose;

	[SerializeField]
	private GButton m_BtnQiangHua;

	[SerializeField]
	private GButton m_BtnXuanZeYuanSu;

	[SerializeField]
	private GButton m_BtnGongJiQiangHua;

	[SerializeField]
	private GButton m_BtnChuanTouQiangHua;

	[SerializeField]
	private GButton m_BtnSuiPian;

	[SerializeField]
	private UILabel m_LabSuiPianNumber;

	[SerializeField]
	private UILabel m_LabName;

	[SerializeField]
	private UILabel m_LabLevel;

	[SerializeField]
	private UILabel m_LabXiaoHaoTitle;

	[SerializeField]
	private UILabel m_LabXiaoHaoNumber;

	[SerializeField]
	private UILabel m_LabSuccessTitle;

	[SerializeField]
	private UILabel m_LabSuccessNumber;

	[SerializeField]
	private UILabel m_LabShuXingTitle;

	[SerializeField]
	private UILabel m_LabShuXingTypeTitle;

	[SerializeField]
	private UILabel m_LabShuXingTypeContent;

	[SerializeField]
	private UISprite m_LabShuXingTypeJianTou;

	[SerializeField]
	private UISprite m_LabShuXingTypeFenGeFu;

	[SerializeField]
	private UISprite m_SpMaxGongJi;

	[SerializeField]
	private UISprite m_SpMaxChuanTou;

	[SerializeField]
	private UILabel m_LabGongJiLv;

	[SerializeField]
	private UILabel m_LabChuanTouLv;

	[SerializeField]
	private ShowNetImage m_ImgGongJi;

	[SerializeField]
	private ShowNetImage m_ImgChuanTou;

	[SerializeField]
	private ShowNetImage m_ImgLogo;

	[SerializeField]
	private GCheckBox m_CheckShenYou;

	[SerializeField]
	private ListBox m_ListGoods;

	[SerializeField]
	private GImgProgressBar m_ProBarLeft;

	[SerializeField]
	private GImgProgressBar m_ProBarRight;

	[SerializeField]
	private GameObject m_QiangHuaPanel;

	[SerializeField]
	private UISprite m_QiangHuaTeShuMax;

	[SerializeField]
	private List<UISprite> m_ListLeftStarSp;

	[SerializeField]
	private List<UISprite> m_ListRightStarSp;

	[SerializeField]
	private List<GameObject> m_ListCenterStarSp;

	[SerializeField]
	private UISprite m_SpShuXingBianKuang;

	[SerializeField]
	private UISprite m_SpShuXingBack;

	[SerializeField]
	private BoxCollider m_BoxShuXing;

	[SerializeField]
	private UIPanel m_PanelShuXing;

	public DPSelectedItemEventHandler m_Handler;

	private YuanSuJueXingQiangHuaType m_QiangHuaType;

	private int m_GongJiLevel;

	private int m_ChuanTouLevel;

	private JingLingYuanSuShuXingVO dataZuHe;

	private List<GameObject> m_ListGameLab = new List<GameObject>();

	private bool StarPanelBool = true;

	private bool m_OnClickStarBool = true;

	private bool m_OnClickYuanQuanBool = true;

	private float[] m_LeftProgressStarEnd = new float[]
	{
		0.125f,
		0.287f,
		0.48f,
		0.67f,
		0.9f
	};

	private float[] m_LeftProgressStarStar = new float[]
	{
		0f,
		0.2f,
		0.38f,
		0.57f,
		0.77f
	};

	private float[] m_RightProgressStarEnd = new float[]
	{
		0.125f,
		0.287f,
		0.48f,
		0.67f,
		0.9f
	};

	private float[] m_RightProgressStarStar = new float[]
	{
		0f,
		0.2f,
		0.38f,
		0.57f,
		0.77f
	};

	private Dictionary<int, GameObject> m_LeftStarTeXiao = new Dictionary<int, GameObject>();

	private Dictionary<int, GameObject> m_RightStarTeXiao = new Dictionary<int, GameObject>();
}
