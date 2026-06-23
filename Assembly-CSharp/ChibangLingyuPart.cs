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

public class ChibangLingyuPart : UserControl
{
	private int MaxNextPreviewPropertyID
	{
		get
		{
			return this.MaxJie - 1;
		}
	}

	private void InitTextInPrefabs()
	{
		this.tsNeedZuanshi.X = 240.0;
		this.tsGoodIcon.X = -60.0;
		this.tsCheckbox.transform.localPosition = new Vector3(-85f, -200f, 0f);
		this.lingyuTitle.text = string.Empty;
		this.staticText[0].text = Global.GetLang("生命");
		this.staticText[1].text = Global.GetLang("物攻");
		this.staticText[2].text = Global.GetLang("魔攻");
		this.staticText[3].text = Global.GetLang("物防");
		this.staticText[4].text = Global.GetLang("魔防");
		this.staticText[5].text = Global.GetLang("命中");
		this.staticText[6].text = Global.GetLang("物理攻击：");
		this.staticText[7].text = Global.GetLang("魔法攻击：");
		this.staticText[8].text = Global.GetLang("物理防御：");
		this.staticText[9].text = Global.GetLang("魔法防御：");
		this.staticText[10].text = Global.GetLang("命       中：");
		this.staticText[11].text = Global.GetLang("生命上限：");
		this.staticText[12].text = Global.GetLang("提升消耗:");
		this.staticText[13].text = Global.GetLang("翎羽属性提升");
		this.staticText[14].text = Global.GetLang("翎羽效果加成");
		this.staticText[15].text = Global.GetLang("当前效果");
		this.staticText[16].text = Global.GetLang("下级效果");
		this.staticCheckBox.text = Global.GetLang("材料不足时，消耗");
		this.btnDaojuTisheng.Text = Global.GetLang("升级");
		this.btnAutoTisheng.Text = Global.GetLang("自动升级");
		this.btnShengJie.Text = Global.GetLang("升阶");
		this.beforeLevel.text = string.Empty;
		this.afterLevel.text = string.Empty;
		this.staticText[6].Pivot = 5;
		this.staticText[7].Pivot = 5;
		this.staticText[8].Pivot = 5;
		this.staticText[9].Pivot = 5;
		this.staticText[10].Pivot = 5;
		this.staticText[11].Pivot = 5;
		this.tsSpecialGoodIcon.gameObject.transform.localPosition = new Vector3(25f, -138f, this.tsSpecialGoodIcon.gameObject.transform.localPosition.z);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.MaxJie = (int)ConfigSystemParam.GetSystemParamIntByName("LingYuMax");
		this.MaxLevel = (int)ConfigSystemParam.GetSystemParamIntByName("LingYuMaxLevel");
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.ItemCollection = this.listLingYu.ItemsSource;
		GameInstance.Game.GetLingyuList();
		this.InitPartData();
		this.GetZuanshiNum();
		this.GetjiachengInfo();
		this.listLingYu.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectChanged);
		this.btnDaojuTisheng.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartGoodsTiSheng();
		};
		this.btnAutoTisheng.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.btnAutoTisheng.Text == Global.GetLang("取消升级"))
			{
				this.StopAutoTiSheng();
			}
			else
			{
				this.btnAutoTisheng.Text = Global.GetLang("取消升级");
				this.btnAutoTisheng.pressedSprite = "tongyongBtn6_normal";
				this.btnAutoTisheng.Pressed = true;
				base.InvokeRepeating("AutoTiSheng_Tick", 1f, 0.5f);
			}
		};
		this.btnShengJie.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartShengjie();
		};
		UIEventListener.Get(this.btnJiacheng.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowJiaCheng();
		};
		this.JiachengCloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ModalPart.gameObject.SetActive(false);
		};
		this.uiDragPanel.onDragFinished = delegate()
		{
			float num = Mathf.Round(this.panel.clipRange.x / 88f) * 88f;
			SpringPanel.Begin(this.panel.gameObject, new Vector3(-num + 4f, 153f, 0f), 10f);
		};
		this.tsCheckbox.Check = false;
		this.tsCheckbox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.tsCheckbox.Check)
			{
				this.tsCheckbox.Check = false;
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "ChiBangShengJie", this.zuanShiXiaoHao);
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("选择后每次需要消耗{0}，确定执行吗？"), text)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.tsCheckbox.Check = true;
					}
					else
					{
						this.tsCheckbox.Check = false;
					}
					return true;
				};
				return;
			}
		};
	}

	public override void Destroy()
	{
		if (this.lingyuResLoader != null)
		{
			this.lingyuResLoader.Stop();
			this.lingyuResLoader = null;
		}
		base.Destroy();
	}

	private void SelectChanged(object sender, EventArgs e)
	{
		if (this.isTishenging)
		{
			return;
		}
		LingYuItem lingYuItem = U3DUtils.AS<LingYuItem>(this.listLingYu.SelectedItem);
		if (null == lingYuItem)
		{
			return;
		}
		if (this.temp != null && this.temp != lingYuItem)
		{
			this.temp.stat.gameObject.SetActive(false);
		}
		this.temp = lingYuItem;
		this.temp.stat.gameObject.SetActive(true);
		this.type = lingYuItem.Type;
		this.InitLingyuInfo(lingYuItem);
		this.attributeMap.item = lingYuItem;
		this.attributeMap.BeginInitMapView();
	}

	private void AutoTiSheng_Tick()
	{
		this.isTishenging = true;
		if (this.tsCheckbox.Check)
		{
			if (this.tsNeedGoodsNum > Global.GetTotalGoodsCountByID(2016))
			{
				IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("LingYuShengXing", this.zuanShiXiaoHao, false);
				GameInstance.Game.GetShengjiLingyuCmd(this.type, 1);
			}
			else
			{
				this.StartGoodsTiSheng();
			}
		}
		else
		{
			this.StartGoodsTiSheng();
		}
	}

	private void StopAutoTiSheng()
	{
		base.CancelInvoke("AutoTiSheng_Tick");
		this.isTishenging = false;
		this.btnAutoTisheng.Text = Global.GetLang("自动升级");
		this.btnAutoTisheng.normalSprite = "tongyongBtn3_normal";
		this.btnAutoTisheng.Pressed = false;
	}

	private void StartGoodsTiSheng()
	{
		if (this.tsCheckbox.Check)
		{
			GameInstance.Game.GetShengjiLingyuCmd(this.type, 1);
		}
		else
		{
			bool flag = this.tsNeedGoodsNum > Global.GetTotalGoodsCountByID(2016);
			if (flag)
			{
				this.StopAutoTiSheng();
				if (flag)
				{
					Super.ShowGoodsGuideForGoodsTips(2016, null);
				}
			}
			else
			{
				GameInstance.Game.GetShengjiLingyuCmd(this.type, 0);
			}
		}
	}

	private void StartShengjie()
	{
		if (this.tsCheckbox.Check)
		{
			GameInstance.Game.GetShengjieLingyuCmd(this.type, 1);
		}
		else
		{
			bool flag = this.tsNeedGoodsNum > Global.GetTotalGoodsCountByID(2017);
			bool flag2 = this.tsNeedSpecialGoodsNum > Global.GetTotalGoodsCountByID(this.tsSpecialGoodID);
			if (flag || (flag2 && this.tsIsNeedSpecialGoods))
			{
				this.StopAutoTiSheng();
				if (flag)
				{
					Super.ShowGoodsGuideForGoodsTips(2017, null);
				}
				else if (flag2)
				{
					Super.HintMainText(Global.GetLang("物品不足！"), 10, 3);
					Super.ShowGoodsGuideForGoodsTips(this.tsSpecialGoodID, null);
				}
			}
			else
			{
				GameInstance.Game.GetShengjieLingyuCmd(this.type, 0);
			}
		}
	}

	private void InitPartData()
	{
		XElement gameResXml = Global.GetGameResXml("Config/LingyuType.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Type");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				LingYuItem lingYuItem = U3DUtils.NEW<LingYuItem>();
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TypeID");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Picture1");
				float xelementAttributeFloat = Global.GetXElementAttributeFloat(xelement, "LifeScale");
				float xelementAttributeFloat2 = Global.GetXElementAttributeFloat(xelement, "AttackScale");
				float xelementAttributeFloat3 = Global.GetXElementAttributeFloat(xelement, "DefenseScale");
				float xelementAttributeFloat4 = Global.GetXElementAttributeFloat(xelement, "MAttackScale");
				float xelementAttributeFloat5 = Global.GetXElementAttributeFloat(xelement, "MDefenseScale");
				float xelementAttributeFloat6 = Global.GetXElementAttributeFloat(xelement, "HitScale");
				string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "Mode");
				this.ItemCollection.Add(lingYuItem);
				if (!this.DicIdtoItem.ContainsKey(xelementAttributeInt))
				{
					this.DicIdtoItem.Add(xelementAttributeInt, lingYuItem);
				}
				float[] attribute = new float[]
				{
					xelementAttributeFloat,
					xelementAttributeFloat2,
					xelementAttributeFloat3,
					xelementAttributeFloat4,
					xelementAttributeFloat5,
					xelementAttributeFloat6
				};
				lingYuItem.icon.GoodImg.URL = string.Format("NetImages/GameRes/Images/Lingyu/{0}", xelementAttributeStr2);
				lingYuItem.gameObject.AddComponent<UIDragPanelContents>();
				lingYuItem.icon.Tag = i.ToString();
				lingYuItem.icon.BackgroundSprite0.gameObject.SetActive(true);
				lingYuItem.Lingyuname = xelementAttributeStr;
				lingYuItem.Mode = xelementAttributeStr3;
				lingYuItem.Type = xelementAttributeInt;
				lingYuItem.attribute = attribute;
				UIPanel component = lingYuItem.GetComponent<UIPanel>();
				if (component)
				{
					Object.Destroy(component);
				}
			}
		}
	}

	private XElement GetLevelConfig(LingYuData lingyuData, bool next = false)
	{
		XElement gameResXml = Global.GetGameResXml("Config/LingyuLevelUp.xml");
		if (gameResXml == null)
		{
			return null;
		}
		XElement xelement = Global.GetXElement(gameResXml, "LingYuLevel", "TypeID", lingyuData.Type.ToString());
		if (xelement == null)
		{
			return null;
		}
		xelement = Global.GetXElement(xelement, "LingYuLevelUp", "Level", (!next) ? lingyuData.Level.ToString() : (lingyuData.Level + 1).ToString());
		if (xelement == null)
		{
			return null;
		}
		return xelement;
	}

	private XElement GetSuitConfig(LingYuData lingyuData, bool next = false)
	{
		XElement gameResXml = Global.GetGameResXml("Config/LingyuSuitUp.xml");
		if (gameResXml == null)
		{
			return null;
		}
		XElement xelement = Global.GetXElement(gameResXml, "LingYuSuit", "TypeID", lingyuData.Type.ToString());
		if (xelement == null)
		{
			return null;
		}
		xelement = Global.GetXElement(xelement, "LingyuSuitUp", "SuitID", (!next) ? lingyuData.Suit.ToString() : (lingyuData.Suit + 1).ToString());
		if (xelement == null)
		{
			return null;
		}
		return xelement;
	}

	private void GetjiachengInfo()
	{
		int num = 0;
		XElement gameResXml = Global.GetGameResXml("Config/LingYucollect.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Lingyu");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Num");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "NeedSuit");
				float xelementAttributeFloat = Global.GetXElementAttributeFloat(xelement, "Luck");
				float xelementAttributeFloat2 = Global.GetXElementAttributeFloat(xelement, "DeLuck");
				ChibangLingyuPart.JiaChengData jiaChengData = new ChibangLingyuPart.JiaChengData();
				jiaChengData.id = xelementAttributeInt;
				jiaChengData.num = xelementAttributeInt2;
				jiaChengData.needSuit = xelementAttributeInt3;
				jiaChengData.luck = xelementAttributeFloat;
				jiaChengData.deluck = xelementAttributeFloat2;
				this.listNeedSuit.Add(xelementAttributeInt3);
				if (!this.roleDicSuitToNum.ContainsKey(xelementAttributeInt3))
				{
					this.roleDicSuitToNum.Add(xelementAttributeInt3, num);
				}
				if (!this.configDicSuitToNum.ContainsKey(xelementAttributeInt3))
				{
					this.configDicSuitToNum.Add(xelementAttributeInt3, xelementAttributeInt2);
				}
				if (!this.DicSuitToData.ContainsKey(xelementAttributeInt3))
				{
					this.DicSuitToData.Add(xelementAttributeInt3, jiaChengData);
				}
				if (!this.DicIDToData.ContainsKey(xelementAttributeInt))
				{
					this.DicIDToData.Add(xelementAttributeInt, jiaChengData);
				}
			}
		}
	}

	private void InitLingyuInfo(LingYuItem item)
	{
		this.listLingYu.SelectedIndex = item.Type - 1;
		LingYuData lingYuData = null;
		if (this.DicIDInfo.TryGetValue(item.Type, ref lingYuData))
		{
			if (null != this.lingyuModal)
			{
				Object.Destroy(this.lingyuModal.gameObject);
				this.lingyuModal = null;
			}
			this.lingyuModal = U3DUtils.NEW<Modal3DShow>();
			U3DUtils.AddChild(base.gameObject, this.lingyuModal.gameObject, false);
			Transform transform = this.lingyuModal.transform;
			transform.localPosition = new Vector3(-310f, -77f, -0.8f);
			transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			UIHelper.SetModalPosZ(this.lingyuModal.transform);
			if (this.lingyuResLoader != null)
			{
				this.lingyuResLoader.Stop();
			}
			this.lingyuResLoader = UIHelper.LoadWuPinRes(this.lingyuModal, item.dealStr(this.GetCorrectJieShu(lingYuData.Suit)), "Equip", 1f);
			int num = lingYuData.Level - lingYuData.Suit * 10;
			this.lingyuTitle.text = string.Concat(new object[]
			{
				item.Lingyuname,
				"  ",
				(lingYuData.Suit <= 0) ? string.Empty : (this.GetCorrectJieShu(lingYuData.Suit).ToString() + Global.GetLang("阶")),
				num,
				Global.GetLang("级")
			});
			XElement suitConfig = this.GetSuitConfig(lingYuData, false);
			XElement levelConfig = this.GetLevelConfig(lingYuData, false);
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			string text = string.Empty;
			int num12 = 0;
			int num13 = 0;
			int num14 = 0;
			int num15 = 0;
			int num16 = 0;
			int num17 = 0;
			int num18 = 0;
			int num19 = 0;
			int num20 = 0;
			int num21 = 0;
			string text2 = string.Empty;
			if (suitConfig != null)
			{
				num2 = Global.GetXElementAttributeInt(suitConfig, "MinAttackV");
				num3 = Global.GetXElementAttributeInt(suitConfig, "MaxAttackV");
				num4 = Global.GetXElementAttributeInt(suitConfig, "MinMAttackV");
				num5 = Global.GetXElementAttributeInt(suitConfig, "MaxMAttackV");
				num6 = Global.GetXElementAttributeInt(suitConfig, "MinDefenseV");
				num7 = Global.GetXElementAttributeInt(suitConfig, "MaxDefenseV");
				num8 = Global.GetXElementAttributeInt(suitConfig, "MinMDefenseV");
				num9 = Global.GetXElementAttributeInt(suitConfig, "MaxMDefenseV");
				num10 = Global.GetXElementAttributeInt(suitConfig, "HitV");
				num11 = Global.GetXElementAttributeInt(suitConfig, "LifeV");
				text = Global.GetXElementAttributeStr(suitConfig, "GoodsCost");
				int xelementAttributeInt = Global.GetXElementAttributeInt(suitConfig, "JinBiCost");
			}
			if (levelConfig != null)
			{
				num12 = Global.GetXElementAttributeInt(levelConfig, "MinAttackV");
				num13 = Global.GetXElementAttributeInt(levelConfig, "MaxAttackV");
				num14 = Global.GetXElementAttributeInt(levelConfig, "MinMAttackV");
				num15 = Global.GetXElementAttributeInt(levelConfig, "MaxMAttackV");
				num16 = Global.GetXElementAttributeInt(levelConfig, "MinDefenseV");
				num17 = Global.GetXElementAttributeInt(levelConfig, "MaxDefenseV");
				num18 = Global.GetXElementAttributeInt(levelConfig, "MinMDefenseV");
				num19 = Global.GetXElementAttributeInt(levelConfig, "MaxMDefenseV");
				num20 = Global.GetXElementAttributeInt(levelConfig, "HitV");
				num21 = Global.GetXElementAttributeInt(levelConfig, "LifeV");
				text2 = Global.GetXElementAttributeStr(levelConfig, "GoodsCost");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(levelConfig, "JinBiCost");
			}
			this.wuliAttc.text = string.Format("{0}-{1}", num2 + num12, num3 + num13);
			this.mofaAttc.text = string.Format("{0}-{1}", num4 + num14, num5 + num15);
			this.wuliDef.text = string.Format("{0}-{1}", num6 + num16, num7 + num17);
			this.mofaDef.text = string.Format("{0}-{1}", num8 + num18, num9 + num19);
			this.mingzhong.text = (num10 + num20).ToString();
			this.lifeMax.text = (num11 + num21).ToString();
			this.tsGoodIcon.gameObject.SetActive(true);
			string text3 = string.Empty;
			string text4 = string.Empty;
			if (lingYuData.Level % 10 != 0 || (lingYuData.Level % 10 == 0 && lingYuData.Level / 10 == lingYuData.Suit))
			{
				XElement levelConfig2 = this.GetLevelConfig(lingYuData, true);
				if (levelConfig2 == null)
				{
					return;
				}
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(levelConfig2, "MaxAttackV");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(levelConfig2, "MinMAttackV");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(levelConfig2, "MaxMAttackV");
				int xelementAttributeInt6 = Global.GetXElementAttributeInt(levelConfig2, "MinDefenseV");
				int xelementAttributeInt7 = Global.GetXElementAttributeInt(levelConfig2, "MaxDefenseV");
				int xelementAttributeInt8 = Global.GetXElementAttributeInt(levelConfig2, "MinMDefenseV");
				int xelementAttributeInt9 = Global.GetXElementAttributeInt(levelConfig2, "MaxMDefenseV");
				int xelementAttributeInt10 = Global.GetXElementAttributeInt(levelConfig2, "HitV");
				int xelementAttributeInt11 = Global.GetXElementAttributeInt(levelConfig2, "LifeV");
				text4 = Global.GetXElementAttributeStr(levelConfig2, "GoodsCost");
				int xelementAttributeInt12 = Global.GetXElementAttributeInt(levelConfig2, "JinBiCost");
				this.nextWuliAttc.text = (xelementAttributeInt3 - num13).ToString();
				this.nextMofaAttc.text = (xelementAttributeInt5 - num15).ToString();
				this.nextWuliDef.text = (xelementAttributeInt7 - num17).ToString();
				this.nextMofaDef.text = (xelementAttributeInt9 - num19).ToString();
				this.nextMingzhong.text = (xelementAttributeInt10 - num20).ToString();
				this.nextLifeMax.text = (xelementAttributeInt11 - num21).ToString();
				this.beforeLevel.text = Global.GetLang("级") + num;
				this.afterLevel.text = Global.GetLang("级") + (num + 1).ToString();
				this.jiantou.gameObject.SetActive(true);
				this.levelMax.text = string.Empty;
				string[] array = text4.Split(new char[]
				{
					'|'
				});
				string[] array2 = array[0].Split(new char[]
				{
					','
				});
				this.AddGoodsIcon(this.tsGoodIcon, Global.SafeConvertToInt32(array2[0]), Global.SafeConvertToInt32(array2[1]));
				this.tsNeedGoodsNum = Global.SafeConvertToInt32(array2[1]);
				string[] array3 = null;
				this.tsSpecialGoodIcon.gameObject.SetActive(false);
				int xelementAttributeInt13 = Global.GetXElementAttributeInt(levelConfig2, "JinBiCost");
				if (xelementAttributeInt13 <= Global.GetRoleOwnNumByMoneyType(1) || xelementAttributeInt13 <= Global.GetRoleOwnNumByMoneyType(8))
				{
					this.tsNeedJinbi.text = xelementAttributeInt13.ToString();
				}
				else
				{
					this.tsNeedJinbi.text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						xelementAttributeInt13.ToString()
					});
				}
				if (array3 != null && this.DicIDtoNum.ContainsKey(Global.SafeConvertToInt32(array2[0])) && this.DicIDtoNum.ContainsKey(Global.SafeConvertToInt32(array3[0])))
				{
					int num22 = Global.SafeConvertToInt32(this.DicIDtoNum[Global.SafeConvertToInt32(array2[0])]) * this.tsNeedGoodsNum;
					int num23 = Global.SafeConvertToInt32(this.DicIDtoNum[Global.SafeConvertToInt32(array3[0])]) * this.tsNeedSpecialGoodsNum;
					this.tsNeedZuanshi.text = (num22 + num23).ToString();
				}
				else if (this.DicIDtoNum.ContainsKey(Global.SafeConvertToInt32(array2[0])))
				{
					this.tsNeedZuanshi.text = (Global.SafeConvertToInt32(this.DicIDtoNum[Global.SafeConvertToInt32(array2[0])]) * this.tsNeedGoodsNum).ToString();
				}
				else if (array3 != null && this.DicIDtoNum.ContainsKey(Global.SafeConvertToInt32(array3[0])))
				{
					this.tsNeedZuanshi.text = (Global.SafeConvertToInt32(this.DicIDtoNum[Global.SafeConvertToInt32(array3[0])]) * this.tsNeedSpecialGoodsNum).ToString();
				}
				else
				{
					this.tsNeedZuanshi.text = (5 * this.tsNeedGoodsNum).ToString();
				}
				this.zuanShiXiaoHao = this.tsNeedZuanshi.text.SafeToInt32(0);
				IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "LingYuShengXing", this.zuanShiXiaoHao, string.Empty);
				if (IConfigbase<ConfigDaiBiShiYong>.Instance.CloseZiDong("LingYuShengXing", this.zuanShiXiaoHao) && this.tsCheckbox.Check)
				{
					this.StopAutoTiSheng();
				}
				this.operState = ChibangLingyuPart.OperState.OperStateShengji;
				this.SetBtnState(this.operState);
			}
			if (lingYuData.Level % 10 == 0 && lingYuData.Level / 10 != lingYuData.Suit)
			{
				XElement suitConfig2;
				if (lingYuData.Level == this.MaxLevel && lingYuData.Suit == this.MaxJie)
				{
					suitConfig2 = this.GetSuitConfig(lingYuData, false);
				}
				else
				{
					suitConfig2 = this.GetSuitConfig(lingYuData, true);
				}
				if (suitConfig2 == null)
				{
					return;
				}
				int xelementAttributeInt14 = Global.GetXElementAttributeInt(suitConfig2, "MaxAttackV");
				int xelementAttributeInt15 = Global.GetXElementAttributeInt(suitConfig2, "MinMAttackV");
				int xelementAttributeInt16 = Global.GetXElementAttributeInt(suitConfig2, "MaxMAttackV");
				int xelementAttributeInt17 = Global.GetXElementAttributeInt(suitConfig2, "MinDefenseV");
				int xelementAttributeInt18 = Global.GetXElementAttributeInt(suitConfig2, "MaxDefenseV");
				int xelementAttributeInt19 = Global.GetXElementAttributeInt(suitConfig2, "MinMDefenseV");
				int xelementAttributeInt20 = Global.GetXElementAttributeInt(suitConfig2, "MaxMDefenseV");
				int xelementAttributeInt21 = Global.GetXElementAttributeInt(suitConfig2, "HitV");
				int xelementAttributeInt22 = Global.GetXElementAttributeInt(suitConfig2, "LifeV");
				text3 = Global.GetXElementAttributeStr(suitConfig2, "GoodsCost");
				int xelementAttributeInt23 = Global.GetXElementAttributeInt(suitConfig2, "JinBiCost");
				this.nextWuliAttc.text = (xelementAttributeInt14 - num3).ToString();
				this.nextMofaAttc.text = (xelementAttributeInt16 - num5).ToString();
				this.nextWuliDef.text = (xelementAttributeInt18 - num7).ToString();
				this.nextMofaDef.text = (xelementAttributeInt20 - num9).ToString();
				this.nextMingzhong.text = (xelementAttributeInt21 - num10).ToString();
				this.nextLifeMax.text = (xelementAttributeInt22 - num11).ToString();
				string[] array4 = text3.Split(new char[]
				{
					'|'
				});
				string[] array5 = array4[0].Split(new char[]
				{
					','
				});
				this.tsNeedGoodsNum = Global.SafeConvertToInt32(array5[1]);
				this.AddGoodsIcon(this.tsGoodIcon, Global.SafeConvertToInt32(array5[0]), Global.SafeConvertToInt32(array5[1]));
				string[] array6 = null;
				this.tsSpecialGoodIcon.gameObject.SetActive(false);
				if (array4.Length >= 2)
				{
					this.tsIsNeedSpecialGoods = true;
					this.tsSpecialGoodIcon.gameObject.SetActive(true);
					array6 = array4[1].Split(new char[]
					{
						','
					});
					this.tsSpecialGoodID = Global.SafeConvertToInt32(array6[0]);
					this.AddGoodsIcon(this.tsSpecialGoodIcon, Global.SafeConvertToInt32(array6[0]), Global.SafeConvertToInt32(array6[1]));
					this.tsNeedSpecialGoodsNum = Global.SafeConvertToInt32(array6[1]);
				}
				else
				{
					this.tsIsNeedSpecialGoods = false;
				}
				int xelementAttributeInt24 = Global.GetXElementAttributeInt(suitConfig2, "JinBiCost");
				if (lingYuData.Level == this.MaxLevel && lingYuData.Suit == this.MaxJie)
				{
					this.levelMax.text = Global.GetLang("已升到最高阶");
					this.beforeLevel.text = string.Empty;
					this.afterLevel.text = string.Empty;
					this.jiantou.gameObject.SetActive(false);
					this.btnShengJie.isEnabled = false;
					this.tsNeedJinbi.text = string.Empty;
					this.tsNeedZuanshi.text = string.Empty;
					this.tsGoodIcon.Text = string.Empty;
					this.tsSpecialGoodIcon.Text = string.Empty;
					this.nextLevel.Text = "0";
					this.nextWuliAttc.Text = "0";
					this.nextMofaAttc.Text = "0";
					this.nextWuliDef.Text = "0";
					this.nextMofaDef.Text = "0";
					this.nextMingzhong.Text = "0";
					this.nextLifeMax.Text = "0";
					this.tsGoodIcon.gameObject.SetActive(false);
					this.tsSpecialGoodIcon.gameObject.SetActive(false);
				}
				else
				{
					this.tsGoodIcon.gameObject.SetActive(true);
					this.beforeLevel.text = Global.GetLang("阶") + lingYuData.Suit.ToString();
					this.afterLevel.text = Global.GetLang("阶") + (lingYuData.Suit + 1).ToString();
					this.jiantou.gameObject.SetActive(true);
					this.levelMax.text = string.Empty;
					this.btnShengJie.isEnabled = true;
					if (xelementAttributeInt24 <= Global.GetRoleOwnNumByMoneyType(1) || xelementAttributeInt24 <= Global.GetRoleOwnNumByMoneyType(8))
					{
						this.tsNeedJinbi.text = xelementAttributeInt24.ToString();
					}
					else
					{
						this.tsNeedJinbi.text = Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							xelementAttributeInt24.ToString()
						});
					}
					if (array6 != null && this.DicIDtoNum.ContainsKey(Global.SafeConvertToInt32(array5[0])) && this.DicIDtoNum.ContainsKey(Global.SafeConvertToInt32(array6[0])))
					{
						int num24 = Global.SafeConvertToInt32(this.DicIDtoNum[Global.SafeConvertToInt32(array5[0])]) * this.tsNeedGoodsNum;
						int num25 = Global.SafeConvertToInt32(this.DicIDtoNum[Global.SafeConvertToInt32(array6[0])]) * this.tsNeedSpecialGoodsNum;
						this.tsNeedZuanshi.text = (num24 + num25).ToString();
					}
					else if (this.DicIDtoNum.ContainsKey(Global.SafeConvertToInt32(array5[0])))
					{
						this.tsNeedZuanshi.text = (Global.SafeConvertToInt32(this.DicIDtoNum[Global.SafeConvertToInt32(array5[0])]) * this.tsNeedGoodsNum).ToString();
					}
					else if (array6 != null && this.DicIDtoNum.ContainsKey(Global.SafeConvertToInt32(array6[0])))
					{
						this.tsNeedZuanshi.text = (Global.SafeConvertToInt32(this.DicIDtoNum[Global.SafeConvertToInt32(array6[0])]) * this.tsNeedSpecialGoodsNum).ToString();
					}
					else
					{
						this.tsNeedZuanshi.text = (20 * this.tsNeedGoodsNum).ToString();
					}
					this.zuanShiXiaoHao = this.tsNeedZuanshi.text.SafeToInt32(0);
					IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "LingYuShengJie", this.zuanShiXiaoHao, string.Empty);
				}
				this.operState = ChibangLingyuPart.OperState.OperStateShengjie;
				this.SetBtnState(this.operState);
			}
		}
	}

	private void GetZuanshiNum()
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("WinCaiLiaoZuanShi", '|');
		if (systemParamStringArrayByName.Length == 0)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			string[] array = systemParamStringArrayByName[i].Split(new char[]
			{
				','
			});
			if (array.Length == 2 && int.TryParse(array[0], ref num) && !this.DicIDtoNum.ContainsKey(num))
			{
				this.DicIDtoNum.Add(num, array[1]);
			}
		}
	}

	private ChibangLingyuPart.JiaChengData GetJiaChengData()
	{
		ChibangLingyuPart.JiaChengData jiaChengData = null;
		this.roleDicSuitToNum = new Dictionary<int, int>();
		for (int i = 0; i < this.listNeedSuit.Count; i++)
		{
			if (!this.roleDicSuitToNum.ContainsKey(this.listNeedSuit[i]))
			{
				this.roleDicSuitToNum.Add(this.listNeedSuit[i], 0);
			}
		}
		foreach (int num in this.DicIDInfo.Keys)
		{
			LingYuData lingYuData = this.DicIDInfo[num];
			for (int j = 0; j < this.listNeedSuit.Count; j++)
			{
				if (this.GetCorrectJieShu(lingYuData.Suit) == this.listNeedSuit[j] && this.roleDicSuitToNum.ContainsKey(this.GetCorrectJieShu(lingYuData.Suit)))
				{
					Dictionary<int, int> dictionary2;
					Dictionary<int, int> dictionary = dictionary2 = this.roleDicSuitToNum;
					int num3;
					int num2 = num3 = this.GetCorrectJieShu(lingYuData.Suit);
					num3 = dictionary2[num3];
					dictionary[num2] = num3 + 1;
				}
			}
		}
		for (int k = 0; k < this.listNeedSuit.Count; k++)
		{
			for (int l = k + 1; l < this.listNeedSuit.Count; l++)
			{
				Dictionary<int, int> dictionary4;
				Dictionary<int, int> dictionary3 = dictionary4 = this.roleDicSuitToNum;
				int num3;
				int num4 = num3 = this.listNeedSuit[k];
				num3 = dictionary4[num3];
				dictionary3[num4] = num3 + this.roleDicSuitToNum[this.listNeedSuit[l]];
			}
		}
		foreach (int num5 in this.configDicSuitToNum.Keys)
		{
			if (this.roleDicSuitToNum[num5] >= this.configDicSuitToNum[num5])
			{
				if (jiaChengData != null)
				{
					jiaChengData = new ChibangLingyuPart.JiaChengData();
				}
				jiaChengData = this.DicSuitToData[num5];
			}
		}
		return jiaChengData;
	}

	private int GetCorrectJieShu(int jieShu)
	{
		return (jieShu <= this.MaxJie) ? jieShu : this.MaxJie;
	}

	private void AddGoodsIcon(GGoodIcon tsGoodIcons, int goodsID, int iNeedNub)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsID);
			tsGoodIcons.Width = 78.0;
			tsGoodIcons.Height = 78.0;
			tsGoodIcons.BackgroundSprite0.transform.localScale = new Vector3(78f, 78f, 0f);
			tsGoodIcons.isAutoSize = true;
			tsGoodIcons.BackSpriteName0 = backSpriteName;
			tsGoodIcons.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			tsGoodIcons.TipType = 1;
			tsGoodIcons.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			tsGoodIcons.ItemNum = iNeedNub;
			tsGoodIcons.ItemCode = goodsID;
			tsGoodIcons.ItemObject = dummyGoodsData;
			tsGoodIcons.BoxTypes = 5;
			tsGoodIcons.Text = iNeedNub.ToString();
			tsGoodIcons.TextShadowColor = 4278190080U;
			tsGoodIcons.TextColor = 16777215U;
			tsGoodIcons.DisableTextColor = 8421504U;
			tsGoodIcons.TextHorizontalAlignment = global::Layout.Right;
			tsGoodIcons.TextVerticalAlignment = global::Layout.Bottom;
			tsGoodIcons.STextVisibility = false;
			bool canUse = Global.CanUseGoods(dummyGoodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(tsGoodIcons, dummyGoodsData, canUse, IconTextTypes.Qianghua);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
			tsGoodIcons.Text = string.Format("{0}/{1}", totalGoodsCountByID, iNeedNub);
			tsGoodIcons.EnableIcon = true;
			tsGoodIcons.TextColor = 16777215U;
			tsGoodIcons.TeXiao.gameObject.SetActive(false);
			if (totalGoodsCountByID >= iNeedNub)
			{
				tsGoodIcons.TextColor = 16777215U;
			}
			else
			{
				tsGoodIcons.TextColor = 16711680U;
			}
			tsGoodIcons.TextShadowColor = 4278190080U;
			tsGoodIcons.TextHorizontalAlignment = global::Layout.Right;
			tsGoodIcons.TextVerticalAlignment = global::Layout.Bottom;
			tsGoodIcons.gameObject.AddComponent<UIDragPanelContents>();
			UIPanel component = tsGoodIcons.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			tsGoodIcons.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	private void SetBtnState(ChibangLingyuPart.OperState state)
	{
		if (state == ChibangLingyuPart.OperState.OperStateShengji)
		{
			this.btnShengJie.gameObject.SetActive(false);
			this.btnDaojuTisheng.gameObject.SetActive(true);
			this.btnAutoTisheng.gameObject.SetActive(true);
		}
		if (state == ChibangLingyuPart.OperState.OperStateShengjie)
		{
			this.btnDaojuTisheng.gameObject.SetActive(false);
			this.btnAutoTisheng.gameObject.SetActive(false);
			this.btnShengJie.gameObject.SetActive(true);
		}
	}

	private void ShowJiaCheng()
	{
		this.ModalPart.gameObject.SetActive(true);
		ChibangLingyuPart.JiaChengData jiaChengData = this.GetJiaChengData();
		ChibangLingyuPart.JiaChengData jiaChengData2 = new ChibangLingyuPart.JiaChengData();
		float num;
		float num2;
		float num3;
		float num4;
		if (jiaChengData == null)
		{
			jiaChengData = new ChibangLingyuPart.JiaChengData();
			jiaChengData.id = 0;
			jiaChengData2 = this.DicIDToData[jiaChengData.id + 1];
			num = 0f;
			num2 = 0f;
			num3 = ((jiaChengData2.luck <= 0f) ? 0f : jiaChengData2.luck);
			num4 = ((jiaChengData2.deluck <= 0f) ? 0f : jiaChengData2.deluck);
		}
		else
		{
			if (this.DicIDToData.ContainsKey(jiaChengData.id + 1))
			{
				jiaChengData2 = this.DicIDToData[jiaChengData.id + 1];
			}
			else
			{
				jiaChengData2 = this.DicIDToData[jiaChengData.id];
			}
			num = ((jiaChengData.luck <= 0f) ? 0f : jiaChengData.luck);
			num2 = ((jiaChengData.deluck <= 0f) ? 0f : jiaChengData.deluck);
			num3 = ((jiaChengData2.luck <= 0f) ? 0f : jiaChengData2.luck);
			num4 = ((jiaChengData2.deluck <= 0f) ? 0f : jiaChengData2.deluck);
		}
		this.labLuck.text = Global.GetLang("幸运一击几率：+") + num * 100f + "%";
		this.labDeLuck.text = Global.GetLang("抵抗幸运一击率：+") + num2 * 100f + "%";
		if (jiaChengData.id < this.MaxNextPreviewPropertyID)
		{
			this.labNextNeed.text = string.Format(Global.GetLang("任意{0}件翎羽达到{1}阶"), jiaChengData2.num, jiaChengData2.needSuit) + Global.GetLang("【未激活】");
			this.labnextLuck.text = Global.GetLang("幸运一击几率：+") + num3 * 100f + "%";
			this.labnextDeLuck.text = Global.GetLang("抵抗幸运一击率：+") + num4 * 100f + "%";
		}
		else
		{
			this.labNextNeed.text = Global.GetLang("无");
			this.labnextLuck.text = string.Empty;
			this.labnextDeLuck.text = string.Empty;
		}
	}

	public void GetLingyuInfo(List<LingYuData> lingyuList)
	{
		this.DicIDInfo.Clear();
		for (int i = 0; i < lingyuList.Count; i++)
		{
			if (!this.DicIDInfo.ContainsKey(lingyuList[i].Type))
			{
				this.DicIDInfo.Add(lingyuList[i].Type, lingyuList[i]);
			}
		}
		this.listLingYu.SelectedIndex = this.nIndex - 1;
		if (this.DicIdtoItem.ContainsKey(this.nIndex))
		{
			this.InitLingyuInfo(this.DicIdtoItem[this.nIndex]);
		}
	}

	public void GetShengjiResult(int code, int type, int currentLevel)
	{
		if (code == 0)
		{
			if (this.operState == ChibangLingyuPart.OperState.OperStateShengji)
			{
				this.SJobject.gameObject.SetActive(false);
				this.SJobject.gameObject.SetActive(true);
			}
			if (this.operState == ChibangLingyuPart.OperState.OperStateShengjie)
			{
				this.shengjieAnimator.gameObject.SetActive(false);
				this.shengjieAnimator.gameObject.SetActive(true);
			}
			this.nIndex = type;
			GameInstance.Game.GetLingyuList();
		}
		else if (code == 1)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("翅膀阶数或星级不满足翎羽开放条件"), new object[0]), 0, -1, -1, 0);
		}
		else if (code == 2)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("等级已满，无法提升！"), new object[0]), 0, -1, -1, 0);
		}
		else if (code == 3)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("必须先提升等级！"), new object[0]), 0, -1, -1, 0);
		}
		else if (code == 4)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("必须先提升品阶!"), new object[0]), 0, -1, -1, 0);
		}
		else if (code == 5)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("品阶已满，无法提升！"), new object[0]), 0, -1, -1, 0);
		}
		else if (code == 6)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("提升等级所需材料不足！"), new object[0]), 0, -1, -1, 0);
		}
		else if (code == 7)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("提升等级所需金币不足！"), new object[0]), 0, -1, -1, 0);
		}
		else if (code == 8)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("提升品阶所需材料不足！"), new object[0]), 0, -1, -1, 0);
		}
		else if (code == 9)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("提升品阶所需金币不足！"), new object[0]), 0, -1, -1, 0);
		}
		else if (code == 10)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("配置错误！"), new object[0]), 0, -1, -1, 0);
		}
		else if (code == 11)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传来的参数错误！"), new object[0]), 0, -1, -1, 0);
		}
		else if (code == 12)
		{
			this.StopAutoTiSheng();
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
		}
		else if (code == 13)
		{
			this.StopAutoTiSheng();
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("与dbserver通信失败!"), new object[0]), 0, -1, -1, 0);
		}
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject ModalPart;

	public GButton CloseBtn;

	public GButton JiachengCloseBtn;

	public GameObject MapViewPanel;

	public GameObject Manjie;

	public UIPanel panel;

	public UIDraggablePanel uiDragPanel;

	public GButton btnDaojuTisheng;

	public GButton btnAutoTisheng;

	public UIButton btnJiacheng;

	public GButton btnShengJie;

	public ListBox listLingYu;

	private ObservableCollection ItemCollection;

	public TextBlock lingyuTitle;

	public Modal3DShow lingyuModal;

	public TextBlock level;

	public TextBlock wuliAttc;

	public TextBlock mofaAttc;

	public TextBlock wuliDef;

	public TextBlock mofaDef;

	public TextBlock mingzhong;

	public TextBlock lifeMax;

	public TextBlock nextLevel;

	public TextBlock nextWuliAttc;

	public TextBlock nextMofaAttc;

	public TextBlock nextWuliDef;

	public TextBlock nextMofaDef;

	public TextBlock nextMingzhong;

	public TextBlock nextLifeMax;

	public TextBlock beforeLevel;

	public TextBlock afterLevel;

	public UISprite jiantou;

	public TextBlock levelMax;

	public GGoodIcon tsGoodIcon;

	public GGoodIcon tsSpecialGoodIcon;

	private int tsSpecialGoodID = -1;

	public TextBlock tsNeedJinbi;

	public GCheckBox tsCheckbox;

	public TextBlock tsNeedZuanshi;

	public TextBlock labLuck;

	public TextBlock labDeLuck;

	public TextBlock labnextLuck;

	public TextBlock labnextDeLuck;

	public TextBlock labNextNeed;

	public LingyuAttributeMap attributeMap;

	public Animator shengjiAnimator;

	public GameObject SJobject;

	public Animator shengjieAnimator;

	public TextBlock[] staticText;

	public UILabel staticCheckBox;

	private int tsNeedGoodsNum = -1;

	private int tsNeedSpecialGoodsNum = -1;

	private bool tsIsNeedSpecialGoods;

	private int type = -1;

	private int nIndex = 1;

	private bool isTishenging;

	private int zuanShiXiaoHao;

	public List<UISprite> listDaiBi = new List<UISprite>();

	private int MaxJie = -1;

	private int MaxLevel = -1;

	private LoadingWindow LoadingWin;

	private Dictionary<int, LingYuItem> DicIdtoItem = new Dictionary<int, LingYuItem>();

	private Dictionary<int, LingYuData> DicIDInfo = new Dictionary<int, LingYuData>();

	private Dictionary<int, string> DicIDtoNum = new Dictionary<int, string>();

	private ChibangLingyuPart.OperState operState;

	private LingYuItem temp;

	private List<int> listNeedSuit = new List<int>();

	private Dictionary<int, int> roleDicSuitToNum = new Dictionary<int, int>();

	private Dictionary<int, int> configDicSuitToNum = new Dictionary<int, int>();

	private Dictionary<int, ChibangLingyuPart.JiaChengData> DicSuitToData = new Dictionary<int, ChibangLingyuPart.JiaChengData>();

	private Dictionary<int, ChibangLingyuPart.JiaChengData> DicIDToData = new Dictionary<int, ChibangLingyuPart.JiaChengData>();

	private WingsLingyuResLoader lingyuResLoader;

	public class JiaChengData
	{
		public int id;

		public int num;

		public int needSuit;

		public float luck;

		public float deluck;
	}

	private enum OperState
	{
		OperStateShengji,
		OperStateShengjie
	}
}
