using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class MeiLanShuPart : UserControl
{
	private void InitPerfabText()
	{
		this.staticRule.lineWidth = 290;
		this.staticText[0].text = Global.GetLang("基础属性");
		this.staticText[1].text = Global.GetLang("魔法书升级");
		this.staticText[2].text = Global.GetLang("升级消耗");
		this.staticText[3].text = Global.GetLang("擦拭消耗");
		this.staticRule.text = string.Concat(new string[]
		{
			Global.GetLang("                      秘语规则{dac7ae}"),
			"\n",
			Global.GetLang("1、使用魔法扫帚{17e43e}擦拭{-}梅林之书，可"),
			"\n",
			Global.GetLang("获得{17e43e}秘语属性{-}增益"),
			"\n",
			Global.GetLang("2、秘语属性可触发以下效果   "),
			"\n",
			Global.GetLang("冻结：追加{17e43e}50%{-}伤害，并冻结目标   "),
			"\n",
			Global.GetLang("麻痹：追加{17e43e}50%{-}伤害，并麻痹目标   "),
			"\n",
			Global.GetLang("减速：追加{17e43e}50%{-}伤害，并减速目标   "),
			"\n",
			Global.GetLang("重击：追加{17e43e}100%{-}伤害"),
			"\n",
			Global.GetLang("3、秘语属性触发概率在擦拭后随机   "),
			"\n",
			Global.GetLang("产生，保存后生效{17e43e}70小时{-}{-}")
		});
		this.staticRule.pivot = 3;
		this.staticRule.transform.localPosition = new Vector3(-130f, this.staticRule.transform.localPosition.y, this.staticRule.transform.localPosition.z);
		if ((double)Global.VersionCode < 7.0)
		{
			try
			{
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					"越南用，可能预制报空"
				});
			}
		}
	}

	protected override void InitializeComponent()
	{
		this.InitPerfabText();
		base.InitializeComponent();
		Super.ShowNetWaiting(string.Empty);
		this.m_chkAuto.CheckChanged = delegate(object e, BaseEventArgs s)
		{
			if (this.m_chkAuto.Check)
			{
				this.m_chkAuto.Check = false;
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "MeiLingZhiShu", this.zuanShiXiaoHao);
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
						this.m_chkAuto.Check = true;
					}
					return true;
				};
				return;
			}
			this.m_chkAuto.Check = false;
		};
		UIEventListener.Get(this.m_BtnClose.gameObject).onClick = delegate(GameObject s)
		{
			PlayZone.GlobalPlayZone.CloseMeiLinMoFaShuWindow();
		};
		TCPGameServerCmds.CMD_SPR_MERLIN_QUERY.SendDataUseRoleID();
		UIEventListener.Get(this.m_BtnCaShi.gameObject).onClick = delegate(GameObject s)
		{
			if (this.IsCashiZhong)
			{
				return;
			}
			if (null != this.m_cashiPart)
			{
				return;
			}
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(2102);
			if (totalGoodsCountByID <= 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("擦拭秘语所需物品不足"), new object[0]), 0, -1, -1, 0);
				PlayZone.GlobalPlayZone.ShowBaodianGuideWindow(BaodianGuidePart.GuideType.NeedMoFaSaoZhou, delegate(object s1, DPSelectedItemEventArgs e1)
				{
					Object.Destroy(base.gameObject.GetComponentInParent<GChildWindow>().gameObject);
				}, string.Empty, string.Empty, true);
				return;
			}
			this.IsCashiZhong = true;
			TCPGameServerCmds.CMD_SPR_MERLIN_SECRET_ATTR_UPDATE.SendDataUseRoleID();
		};
		UIEventListener.Get(this.m_BtnXianShiMiYu.gameObject).onClick = delegate(GameObject s)
		{
			DateTime dateTime;
			dateTime..ctor(this.DataBag._ToTicks * 10000L);
			DateTime dateTime2;
			dateTime2..ctor(Global.GetCorrectLocalTime() * 10000L);
			if (this.DataBag._ToTicks == 0L || dateTime < dateTime2)
			{
				this.m_Rule.gameObject.SetActive(true);
				UIEventListener.Get(this.m_BtnCloseRule.gameObject).onClick = delegate(GameObject s1)
				{
					this.m_Rule.gameObject.SetActive(false);
				};
				return;
			}
			if (null != this.m_cashiPart)
			{
				return;
			}
			this.m_cashiPart = U3DUtils.NEW<MeiLanShuCaShiPart>();
			this.m_cashiPart.DataBag = this.DataBag;
			this.m_cashiPart.IsShuXingChaKan = true;
			this.m_cashiPart.transform.parent = base.transform;
			this.m_cashiPart.transform.localPosition = new Vector3(0f, 0f, -200f);
			this.m_cashiPart.transform.localScale = new Vector3(1f, 1f, 1f);
			this.m_cashiPart.m_BtnSave.gameObject.SetActive(false);
			this.m_cashiPart.m_BtnClose.GetComponentInChildren<UILabel>().text = Global.GetLang("关闭");
			this.m_cashiPart.m_BtnClose.transform.localPosition = new Vector3(-0.4157115f, -113.089783f, 0f);
			this.m_cashiPart.m_Attribut.transform.localPosition = new Vector3(-37.75141f, 0f, 0f);
			ModelShowHide modelShowHide = this.m_cashiPart.gameObject.AddComponent<ModelShowHide>();
			modelShowHide.goShowHide = this.m_3D.gameObject;
		};
		this.m_BtnCaShi.GetComponentInChildren<UILabel>().text = Global.GetLang("擦拭");
		DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(9100);
		string resName = decorationVOByCode.ResName;
		this.decoMiYu = new GDecoration(resName);
		this.decoMiYu.Layer = LayerMask.NameToLayer("MUUI");
		this.decoMiYu.Position3D = this.m_TransdecoMiYu.transform.position;
		this.decoMiYu.Start();
		this.decoMiYu.Parent = this.m_TransdecoMiYu.transform;
	}

	public override void Destroy()
	{
		if (this.resourceLoader != null)
		{
			this.resourceLoader.Stop();
			this.resourceLoader = null;
		}
		base.Destroy();
	}

	public IEnumerator LoadModelResource(int Lev)
	{
		if (this.show3DModel.transform.childCount == 1)
		{
			if (this.show3DModel.transform.GetChild(0).name == "UI_Model_" + (300 + (int)((double)((float)Lev / 2f) + 0.5)))
			{
				yield break;
			}
			Object.Destroy(this.show3DModel.transform.GetChild(0).gameObject);
		}
		this.m_Modal3DShow = U3DUtils.NEW<Modal3DShow>();
		U3DUtils.AddChild(this.show3DModel, this.m_Modal3DShow.gameObject, false);
		UIHelper.SetModalPosZ(this.m_Modal3DShow.transform);
		if (this.resourceLoader != null)
		{
			this.resourceLoader.Stop();
		}
		this.resourceLoader = UIHelper.LoadModelResource(this.m_Modal3DShow, 300 + (int)((float)Lev / 2f + 0.5f), 1f, delegate(object s, DPSelectedItemEventArgs e)
		{
			this._Animation = this.m_Modal3DShow.transform.GetChild(0).GetComponentInChildren<Animation>();
		});
		yield break;
	}

	private IEnumerator DestoryObj(GameObject o)
	{
		yield return new WaitForSeconds(3f);
		Object.Destroy(o);
		yield break;
	}

	public override void Update()
	{
		base.Update();
		if (null != this._Animation)
		{
			this.ControlTime -= Time.deltaTime;
			if (this.ControlTime <= 0f)
			{
				if (this._Animation.IsPlaying("Stand"))
				{
					this._Animation.Play("Relax");
					AnimationState animationState = this._Animation["Relax"];
					animationState.wrapMode = 2;
					DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(9000 + (int)((float)this.DataBag._Level / 2f - 0.5f));
					string resName = decorationVOByCode.ResName;
					GDecoration gdecoration = new GDecoration(resName);
					gdecoration.Layer = LayerMask.NameToLayer("MUUI");
					gdecoration.Start();
					gdecoration.Parent = this.m_Modal3DShowTeXiao.transform;
					gdecoration.Position3D = this.m_Modal3DShowTeXiao.transform.position;
					this.ControlTime = animationState.length;
					base.StartCoroutine<bool>(this.DestoryObj(gdecoration.The3DGameObject));
				}
				else
				{
					this._Animation.Play("Stand");
					AnimationState animationState2 = this._Animation["Stand"];
					animationState2.wrapMode = 2;
					this.ControlTime = animationState2.length * 2f;
				}
			}
		}
	}

	public IEnumerator PlayAni(Animation _Animation, Transform m_Modal3DShowTeXiao)
	{
		if (!_Animation)
		{
			yield break;
		}
		if (_Animation.IsPlaying("Stand"))
		{
			_Animation.Play("Relax");
			AnimationState _AnimationClip = _Animation["Relax"];
			_AnimationClip.wrapMode = 2;
			DecorationVO cachingDecoConfig = ConfigDecoration.GetDecorationVOByCode(9000 + (int)((float)this.DataBag._Level / 2f - 0.5f));
			string resName = cachingDecoConfig.ResName;
			GDecoration deco = new GDecoration(resName);
			deco.Layer = LayerMask.NameToLayer("MUUI");
			deco.Start();
			deco.Parent = m_Modal3DShowTeXiao.transform;
			deco.Position3D = m_Modal3DShowTeXiao.transform.position;
			yield return new WaitForSeconds(_AnimationClip.length);
			base.StartCoroutine(this.PlayAni(_Animation, m_Modal3DShowTeXiao));
			yield return new WaitForSeconds(2f);
			Object.Destroy(deco.The3DGameObject);
		}
		else
		{
			_Animation.Play("Stand");
			AnimationState _AnimationClip2 = _Animation["Stand"];
			_AnimationClip2.wrapMode = 2;
			yield return new WaitForSeconds(_AnimationClip2.length * 2f);
			base.StartCoroutine(this.PlayAni(_Animation, m_Modal3DShowTeXiao));
		}
		yield break;
	}

	public void aciton_CMD_SPR_MERLIN_QUERY(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		this.DataBag = DataHelper.BytesToObject<MerlinGrowthSaveDBData>(e.bytesData, 0, e.bytesData.Length);
		this.InitData();
		this.refreshUIType(this.DataBag);
		if (this.m_cashiPart != null)
		{
			this.m_cashiPart.DataBag = this.DataBag;
		}
	}

	public void aciton_CMD_SPR_MERLIN_SECRET_ATTR_UPDATE(MUSocketConnectEventArgs e)
	{
		base.StartCoroutine(this.aciton_CMD_SPR_MERLIN_SECRET_ATTR_UPDATEIEnumerator(e));
	}

	private IEnumerator aciton_CMD_SPR_MERLIN_SECRET_ATTR_UPDATEIEnumerator(MUSocketConnectEventArgs e)
	{
		int JieGuo = ConvertExt.SafeConvertToInt32(e.fields[0]);
		if (JieGuo == 0)
		{
			UIHelper.LoadModelResource(this.m_Modal3DShowSaoBa, 400, 1f, null);
			while (this.m_Modal3DShowSaoBa._Target.transform.childCount == 0)
			{
				yield return new WaitForSeconds(0.01f);
			}
			Animation ani = this.m_Modal3DShowSaoBa._Target.transform.GetComponentInChildren<Animation>();
			ani.Play(ani.clip.name);
			yield return new WaitForSeconds(ani[ani.clip.name].length + 0.5f);
			Object.Destroy(this.m_Modal3DShowSaoBa._Target.gameObject, 0.2f);
			this.IsCashiZhong = false;
			this.DataBag._UnActiveAttr[0] = (double)float.Parse(e.fields[1]);
			this.DataBag._UnActiveAttr[1] = (double)float.Parse(e.fields[2]);
			this.DataBag._UnActiveAttr[2] = (double)float.Parse(e.fields[3]);
			this.DataBag._UnActiveAttr[3] = (double)float.Parse(e.fields[4]);
			this.InitData();
			if (null != this.m_cashiPart)
			{
				this.m_cashiPart.DataBag = this.DataBag;
				yield break;
			}
			this.m_cashiPart = U3DUtils.NEW<MeiLanShuCaShiPart>();
			this.m_cashiPart.DataBag = this.DataBag;
			this.m_cashiPart.transform.parent = base.transform;
			this.m_cashiPart.transform.localPosition = new Vector3(0f, 0f, -200f);
			this.m_cashiPart.transform.localScale = new Vector3(1f, 1f, 1f);
			ModelShowHide _ModelShowHide = this.m_cashiPart.gameObject.AddComponent<ModelShowHide>();
			_ModelShowHide.goShowHide = this.m_3D.gameObject;
		}
		else if (JieGuo == 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("阶数异常"), new object[0]), 0, -1, -1, 0);
		}
		else if (JieGuo == 2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("秘语数据异常"), new object[0]), 0, -1, -1, 0);
		}
		else if (JieGuo == 3)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("擦拭秘语所需物品ID异常"), new object[0]), 0, -1, -1, 0);
		}
		else if (JieGuo == 4)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("擦拭秘语所需物品数量异常"), new object[0]), 0, -1, -1, 0);
		}
		else if (JieGuo == 5)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("擦拭秘语所需物品不足"), new object[0]), 0, -1, -1, 0);
			this.refreshUIType(this.DataBag);
			PlayZone.GlobalPlayZone.ShowBaodianGuideWindow(BaodianGuidePart.GuideType.NeedMoFaSaoZhou, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				Object.Destroy(base.gameObject.GetComponentInParent<GChildWindow>().gameObject);
			}, string.Empty, string.Empty, true);
		}
		yield break;
	}

	public void InitData()
	{
		this.InnitData(this.DataBag);
		base.StartCoroutine(this.LoadModelResource(this.DataBag._Level));
	}

	public void InnitData(MerlinGrowthSaveDBData DataBag)
	{
		base.StartCoroutine(this.ShowShengyuShijian());
		this.m_LabelJieShu.text = DataBag._Level.ToString();
		this.m_LabelZuanShi.text = Global.Data.roleData.UserMoney.ToString();
		if (DataBag._StarNum != 10)
		{
			XElement gameResXml = Global.GetGameResXml(string.Format("Config/Merlin/MagicBookStar.xml", new object[0]));
			if (gameResXml == null)
			{
				return;
			}
			for (int i = 0; i < 10; i++)
			{
				if (i < DataBag._StarNum)
				{
					this.m_BgXinJiArr[i].enabled = true;
				}
				else
				{
					this.m_BgXinJiArr[i].enabled = false;
				}
			}
			XElement xelement = Enumerable.ToList<XElement>(gameResXml.Elements("MagicBook")).Find((XElement s) => s.AttributeStr("Level") == DataBag._Level.ToString() && s.AttributeStr("Star") == (DataBag._StarNum + 1).ToString());
			this.m_LableMeiLanJingYan.text = DataBag._StarExp + "/" + xelement.AttributeStr("StarExp");
			this.m_SpriteMeiLanJingYan.fillAmount = (float)DataBag._StarExp / float.Parse(xelement.AttributeStr("StarExp"));
			XElement xelement2 = xelement;
			if (xelement2 == null)
			{
				return;
			}
			int goodsID = int.Parse(xelement2.AttributeStr("NeedGoods").Split(new char[]
			{
				','
			})[0]);
			bool flag = true;
			if (this.m_IconShengJi.childCount == 1)
			{
				GGoodIcon component = this.m_IconShengJi.GetChild(0).gameObject.GetComponent<GGoodIcon>();
				if (component.BodyURL != null && component.BodyURL.ImageSource.Contains(goodsID.ToString()))
				{
					flag = false;
				}
				else
				{
					Object.Destroy(this.m_IconShengJi.GetChild(0).gameObject);
				}
			}
			if (flag)
			{
				GGoodIcon goodsItemIconEx = this.GetGoodsItemIconEx(xelement2.AttributeStr("NeedGoods").Split(new char[]
				{
					','
				}), Global.GetTotalGoodsCountByID(goodsID));
				goodsItemIconEx.transform.parent = this.m_IconShengJi;
				goodsItemIconEx.transform.localScale = Vector3.one * 0.9f;
				goodsItemIconEx.transform.localPosition = Vector3.zero;
			}
			this.m_LableShengJiXiaoHao.text = Global.GetTotalGoodsCountByID(goodsID) + "/" + xelement2.AttributeStr("NeedGoods").Split(new char[]
			{
				','
			})[1];
			this.mcountGoodNum = xelement2.AttributeStr("NeedGoods").Split(new char[]
			{
				','
			})[1].SafeToInt32(0);
			this.goodid = goodsID;
			if (int.Parse(xelement2.AttributeStr("NeedGoods").Split(new char[]
			{
				','
			})[1]) > Global.GetTotalGoodsCountByID(goodsID))
			{
				this.m_LableShengJiXiaoHao.color = Color.red;
			}
			this.m_LableXiaoHaoZuanShi.text = Global.GetLang("道具不足时,自动消耗") + "      " + xelement2.AttributeStr("NeedZuanShi");
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "MeiLingZhiShu", xelement2.AttributeStr("NeedZuanShi").SafeToInt32(0), string.Empty);
			this.zuanShiXiaoHao = xelement2.AttributeStr("NeedZuanShi").SafeToInt32(0);
		}
		else
		{
			XElement gameResXml2 = Global.GetGameResXml(string.Format("Config/Merlin/MagicBook.xml", new object[0]));
			XElement xelement3 = Enumerable.ToList<XElement>(gameResXml2.Elements("MagicBook")).Find((XElement s) => s.AttributeStr("Level") == DataBag._Level.ToString());
			this.m_LableMeiLanJingYan.text = string.Empty;
			double num = Enumerable.ToList<XElement>(gameResXml2.Elements("MagicBook")).Find((XElement s) => s.AttributeInt("Level") == DataBag._Level + 1).AttributeDouble("LuckyOne");
			this.m_SpriteMeiLanJingYan.fillAmount = (float)((double)(DataBag.LevelUpFailNum * 3) / (110000.0 - num + (110000.0 - num)) * 0.2);
			if (DataBag._Level != 20)
			{
				XElement gameResXml3 = Global.GetGameResXml(string.Format("Config/Merlin/MagicBook.xml", new object[0]));
				XElement xelement4 = Enumerable.ToList<XElement>(gameResXml3.Elements("MagicBook")).Find((XElement s) => s.AttributeStr("Level") == (DataBag._Level + 1).ToString());
				int goodsID2 = int.Parse(xelement4.AttributeStr("NeedGoods").Split(new char[]
				{
					','
				})[0]);
				bool flag2 = true;
				if (this.m_IconShengJi.childCount == 1)
				{
					GGoodIcon component2 = this.m_IconShengJi.GetChild(0).gameObject.GetComponent<GGoodIcon>();
					if (component2.BodyURL != null && component2.BodyURL.ImageSource.Contains(goodsID2.ToString()))
					{
						flag2 = false;
					}
					else
					{
						Object.Destroy(this.m_IconShengJi.GetChild(0).gameObject);
					}
				}
				if (flag2)
				{
					GGoodIcon goodsItemIconEx2 = this.GetGoodsItemIconEx(xelement4.AttributeStr("NeedGoods").Split(new char[]
					{
						','
					}), Global.GetTotalGoodsCountByID(goodsID2));
					goodsItemIconEx2.transform.parent = this.m_IconShengJi;
					goodsItemIconEx2.transform.localScale = Vector3.one * 0.9f;
					goodsItemIconEx2.transform.localPosition = Vector3.zero;
				}
				this.m_LableXiaoHaoZuanShi.text = Global.GetLang("道具不足时,自动消耗") + "      " + xelement4.AttributeStr("NeedZuanShi");
				IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "MeiLingZhiShu", xelement4.AttributeStr("NeedZuanShi").SafeToInt32(0), string.Empty);
				this.zuanShiXiaoHao = xelement4.AttributeStr("NeedZuanShi").SafeToInt32(0);
				this.m_LableShengJiXiaoHao.text = Global.GetTotalGoodsCountByID(goodsID2) + "/" + xelement4.AttributeStr("NeedGoods").Split(new char[]
				{
					','
				})[1];
				if (int.Parse(xelement4.AttributeStr("NeedGoods").Split(new char[]
				{
					','
				})[1]) > Global.GetTotalGoodsCountByID(goodsID2))
				{
					this.m_LableShengJiXiaoHao.color = Color.red;
				}
			}
			else
			{
				this.m_LableShengJiXiaoHao.enabled = false;
			}
		}
		XElement gameResXml4 = Global.GetGameResXml(string.Format("Config/MagicWord.xml", new object[0]));
		XElement xelement5 = Enumerable.ToList<XElement>(gameResXml4.Elements("MagicWord")).Find((XElement s) => s.AttributeStr("Level") == DataBag._Level.ToString());
		int goodsID3 = int.Parse(xelement5.AttributeStr("NeedGoods").Split(new char[]
		{
			','
		})[0]);
		bool flag3 = true;
		if (this.m_IconCaShiXiaoHao.childCount == 1)
		{
			GGoodIcon component3 = this.m_IconCaShiXiaoHao.GetChild(0).gameObject.GetComponent<GGoodIcon>();
			if (component3.BodyURL != null && component3.BodyURL.ImageSource.Contains(goodsID3.ToString()))
			{
				flag3 = false;
			}
			else
			{
				Object.Destroy(this.m_IconCaShiXiaoHao.GetChild(0).gameObject);
			}
		}
		if (flag3)
		{
			GGoodIcon goodsItemIconEx3 = this.GetGoodsItemIconEx(xelement5.AttributeStr("NeedGoods").Split(new char[]
			{
				','
			}), Global.GetTotalGoodsCountByID(goodsID3));
			goodsItemIconEx3.transform.parent = this.m_IconCaShiXiaoHao;
			goodsItemIconEx3.transform.localScale = Vector3.one * 0.9f;
			goodsItemIconEx3.transform.localPosition = Vector3.zero;
		}
		this.m_LableCaShiXiaoHao.text = Global.GetTotalGoodsCountByID(goodsID3) + "/" + xelement5.AttributeStr("NeedGoods").Split(new char[]
		{
			','
		})[1];
		if (int.Parse(xelement5.AttributeStr("NeedGoods").Split(new char[]
		{
			','
		})[1]) > Global.GetTotalGoodsCountByID(goodsID3))
		{
			this.m_LableCaShiXiaoHao.color = Color.red;
		}
		this.InnitJiChuXinXi(DataBag._Level.ToString(), DataBag._StarNum.ToString());
	}

	private IEnumerator ShowShengyuShijian()
	{
		DateTime date = new DateTime(this.DataBag._ToTicks * 10000L);
		DateTime now = new DateTime(Global.GetCorrectLocalTime() * 10000L);
		if (this.DataBag._ToTicks != 0L && date > now)
		{
			string str_Hours = ((date - now).Hours + (date - now).Days * 24).ToString();
			string str_Minutes = (date - now).Minutes.ToString();
			string str_Seconds = (date - now).Seconds.ToString();
			if (str_Hours.Length == 1)
			{
				str_Hours = "0" + str_Hours;
			}
			if (str_Minutes.Length == 1)
			{
				str_Minutes = "0" + str_Minutes;
			}
			if (str_Seconds.Length == 1)
			{
				str_Seconds = "0" + str_Seconds;
			}
			this.m_LableShengYuShiJian.text = string.Concat(new string[]
			{
				Global.GetLang("剩余"),
				":{0EFF04}",
				str_Hours,
				":",
				str_Minutes,
				":",
				str_Seconds
			});
			this.m_TransdecoMiYu.gameObject.SetActive(true);
		}
		else
		{
			this.m_LableShengYuShiJian.text = Global.GetLang("需要擦拭");
			this.m_TransdecoMiYu.gameObject.SetActive(false);
			for (int i = 0; i < this.DataBag._ActiveAttr.Count; i++)
			{
				this.DataBag._ActiveAttr[i] = 0.0;
			}
		}
		yield return new WaitForSeconds(1f);
		base.StartCoroutine(this.ShowShengyuShijian());
		yield break;
	}

	private void InnitJiChuXinXi(string Lev, string Star)
	{
		if (Lev == "20" && Star == "10")
		{
			foreach (UISprite uisprite in this.m_BackgroundShuXingArr)
			{
				uisprite.enabled = false;
			}
			this.m_LableAttackC.text = string.Empty;
			this.m_LableDefenseC.text = string.Empty;
			this.m_LableMDefenseVC.text = string.Empty;
			this.m_LableHitVC.text = string.Empty;
			this.m_LableDodgeC.text = string.Empty;
			this.m_LableMaxLifeVC.text = string.Empty;
			this.m_LableReviveC.text = string.Empty;
			this.m_LableMagicRecoverC.text = string.Empty;
			this.m_chkAuto.gameObject.SetActive(false);
			XElement gameResXml = Global.GetGameResXml(string.Format("Config/Merlin/MagicBookStar.xml", new object[0]));
			XElement xelement = Enumerable.ToList<XElement>(gameResXml.Elements("MagicBook")).Find((XElement s) => s.Attribute("Level").Value == Lev && s.Attribute("Star").Value == Star);
			float num = xelement.AttributeFloat("MinAttackV");
			float num2 = xelement.AttributeFloat("MaxAttackV");
			float num3 = xelement.AttributeFloat("MinMAttackV");
			float num4 = xelement.AttributeFloat("MaxMAttackV");
			float num5 = xelement.AttributeFloat("MinDefenseV");
			float num6 = xelement.AttributeFloat("MaxDefenseV");
			float num7 = xelement.AttributeFloat("MinMDefenseV");
			float num8 = xelement.AttributeFloat("MaxMDefenseV");
			float num9 = xelement.AttributeFloat("HitV");
			float num10 = xelement.AttributeFloat("Dodge");
			float num11 = xelement.AttributeFloat("MaxLifeV");
			float num12 = xelement.AttributeFloat("Revive");
			float num13 = xelement.AttributeFloat("MagicRecover");
			this.m_LableDefense.text = string.Concat(new object[]
			{
				Global.GetLang("{E2B272}物理防御:{-}"),
				num5,
				"-",
				num6,
				string.Empty
			});
			this.m_LableMDefenseV.text = string.Concat(new object[]
			{
				Global.GetLang("{E2B272}魔法防御:{-}"),
				num7,
				"-",
				num8,
				string.Empty
			});
			this.m_LableHitV.text = Global.GetLang("{E2B272}命       中:{-}") + num9;
			this.m_LableDodge.text = Global.GetLang("{E2B272}闪       避:{-}") + num10;
			this.m_LableMaxLifeV.text = Global.GetLang("{E2B272}生命上限:{-}") + num11;
			this.m_LableRevive.text = Global.GetLang("{E2B272}重生几率:{-}+") + (int)((double)(num12 * 100f) + 0.5) + "%";
			this.m_LableMagicRecover.text = Global.GetLang("{E2B272}魔法完全回复:{-}+") + (int)((double)(num13 * 100f) + 0.5) + "%";
			if (Global.Data.roleData.Occupation == 1 || (Global.Data.roleData.Occupation == 3 && Global.GetMJSType() == MJSSkillType.Magic_Sword))
			{
				this.m_LableAttack.text = string.Concat(new object[]
				{
					Global.GetLang("{E2B272}攻       击:{-}"),
					num3,
					"-",
					num4,
					string.Empty
				});
			}
			else
			{
				this.m_LableAttack.text = string.Concat(new object[]
				{
					Global.GetLang("{E2B272}攻       击:{-}"),
					num,
					"-",
					num2,
					string.Empty
				});
			}
			return;
		}
		XElement gameResXml2 = Global.GetGameResXml(string.Format("Config/Merlin/MagicBookStar.xml", new object[0]));
		XElement xelement2 = Enumerable.ToList<XElement>(gameResXml2.Elements("MagicBook")).Find((XElement s) => s.Attribute("Level").Value == Lev && s.Attribute("Star").Value == Star);
		XElement xelement3;
		if (Star == "10")
		{
			xelement3 = Enumerable.ToList<XElement>(gameResXml2.Elements("MagicBook")).Find((XElement s) => s.Attribute("Level").Value == int.Parse(Lev) + 1 + string.Empty && s.Attribute("Star").Value == "0");
		}
		else
		{
			xelement3 = Enumerable.ToList<XElement>(gameResXml2.Elements("MagicBook")).Find((XElement s) => s.Attribute("Level").Value == Lev && s.Attribute("Star").Value == int.Parse(Star) + 1 + string.Empty);
		}
		float num14 = xelement2.AttributeFloat("MinAttackV");
		float num15 = xelement2.AttributeFloat("MaxAttackV");
		float num16 = xelement2.AttributeFloat("MinMAttackV");
		float num17 = xelement2.AttributeFloat("MaxMAttackV");
		float num18 = xelement2.AttributeFloat("MinDefenseV");
		float num19 = xelement2.AttributeFloat("MaxDefenseV");
		float num20 = xelement2.AttributeFloat("MinMDefenseV");
		float num21 = xelement2.AttributeFloat("MaxMDefenseV");
		float num22 = xelement2.AttributeFloat("HitV");
		float num23 = xelement2.AttributeFloat("Dodge");
		float num24 = xelement2.AttributeFloat("MaxLifeV");
		float num25 = xelement2.AttributeFloat("Revive");
		float num26 = xelement2.AttributeFloat("MagicRecover");
		float num27 = xelement3.AttributeFloat("MinAttackV");
		float num28 = xelement3.AttributeFloat("MaxAttackV");
		float num29 = xelement3.AttributeFloat("MinMAttackV");
		float num30 = xelement3.AttributeFloat("MaxMAttackV");
		float num31 = xelement3.AttributeFloat("MinDefenseV");
		float num32 = xelement3.AttributeFloat("MaxDefenseV");
		float num33 = xelement3.AttributeFloat("MinMDefenseV");
		float num34 = xelement3.AttributeFloat("MaxMDefenseV");
		float num35 = xelement3.AttributeFloat("HitV");
		float num36 = xelement3.AttributeFloat("Dodge");
		float num37 = xelement3.AttributeFloat("MaxLifeV");
		float num38 = xelement3.AttributeFloat("Revive");
		float num39 = xelement3.AttributeFloat("MagicRecover");
		this.m_LableDefense.text = string.Concat(new object[]
		{
			Global.GetLang("{E2B272}物理防御:{-}"),
			num18,
			"-",
			num19,
			string.Empty
		});
		this.m_LableMDefenseV.text = string.Concat(new object[]
		{
			Global.GetLang("{E2B272}魔法防御:{-}"),
			num20,
			"-",
			num21,
			string.Empty
		});
		this.m_LableHitV.text = Global.GetLang("{E2B272}命       中:{-}") + num22;
		this.m_LableDodge.text = Global.GetLang("{E2B272}闪       避:{-}") + num23;
		this.m_LableMaxLifeV.text = Global.GetLang("{E2B272}生命上限:{-}") + num24;
		this.m_LableRevive.text = Global.GetLang("{E2B272}重生几率:{-}+") + (int)((double)(num25 * 100f) + 0.5) + "%";
		this.m_LableMagicRecover.text = Global.GetLang("{E2B272}魔法完全回复:{-}+") + (int)((double)(num26 * 100f) + 0.5) + "%";
		if (Global.Data.roleData.Occupation == 1 || (Global.Data.roleData.Occupation == 3 && Global.GetMJSType() == MJSSkillType.Magic_Sword))
		{
			this.m_LableAttack.text = string.Concat(new object[]
			{
				Global.GetLang("{E2B272}攻       击:{-}"),
				num16,
				"-",
				num17,
				string.Empty
			});
		}
		else
		{
			this.m_LableAttack.text = string.Concat(new object[]
			{
				Global.GetLang("{E2B272}攻       击:{-}"),
				num14,
				"-",
				num15,
				string.Empty
			});
		}
		this.m_LableDefenseC.text = (num31 - num18).ToString();
		this.m_LableMDefenseVC.text = (num33 - num20).ToString();
		this.m_LableHitVC.text = (num35 - num22).ToString();
		this.m_LableDodgeC.text = (num36 - num23).ToString();
		this.m_LableMaxLifeVC.text = (num37 - num24).ToString();
		this.m_LableReviveC.text = (int)((double)((num38 - num25) * 100f) + 0.5) + "%";
		this.m_LableMagicRecoverC.text = (int)((double)((num39 - num26) * 100f) + 0.5) + "%";
		if (Global.Data.roleData.Occupation == 1 || (Global.Data.roleData.Occupation == 3 && Global.GetMJSType() == MJSSkillType.Magic_Sword))
		{
			this.m_LableAttackC.text = ((int)(num29 - num16)).ToString();
		}
		else
		{
			this.m_LableAttackC.text = ((int)(num27 - num14)).ToString();
		}
		if (num39 - num26 == 0f)
		{
			this.m_BackgroundMagicRecover.enabled = false;
			this.m_LableMagicRecoverC.text = string.Empty;
		}
		else
		{
			this.m_BackgroundMagicRecover.enabled = true;
		}
		if (num38 - num25 == 0f)
		{
			this.m_BackgroundRevive.enabled = false;
			this.m_LableReviveC.text = string.Empty;
		}
		else
		{
			this.m_BackgroundRevive.enabled = true;
		}
	}

	private GGoodIcon GetGoodsItemIconEx(string[] goods, int count = 0)
	{
		GoodsData goodsData = new GoodsData();
		goodsData.GoodsID = int.Parse(goods[0]);
		GGoodIcon ggoodIcon;
		if (goodsData != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			ggoodIcon.SecondText.Text = count + "/" + goods[1];
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.BackSpriteName0 = "bagGrid_bak";
		}
		ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GGoodIcon ggoodIcon2 = s as GGoodIcon;
			if (null != ggoodIcon2)
			{
				GoodsData goodData = ggoodIcon2.ItemObject as GoodsData;
				GTipServiceEx.ShowTip(ggoodIcon2, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
			}
		};
		UIButtonOffset componentInChildren = ggoodIcon.GetComponentInChildren<UIButtonOffset>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		return ggoodIcon;
	}

	private void refreshUIType(MerlinGrowthSaveDBData DataBag)
	{
		if (base.gameObject == null)
		{
			return;
		}
		if (DataBag._StarNum == 10)
		{
			this.m_BgXinJi.gameObject.SetActive(false);
			this.m_chkAuto.Check = false;
			this.m_BtnShengJi.transform.GetComponentInChildren<UILabel>().text = Global.GetLang("升阶");
			this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text = Global.GetLang("自动升阶");
			this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UISprite>().spriteName = "tongyongBtn_normal";
			this.IsZiDongShengJi = false;
			UIEventListener.Get(this.m_BtnShengJi.gameObject).onClick = delegate(GameObject s)
			{
				if (0f < Global.GetBtnCD(this.m_BtnShengJi.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this.m_BtnShengJi.GetInstanceID(), 0.5f);
				string strcmd = string.Empty;
				if (this.m_chkAuto.Check)
				{
					strcmd = StringUtil.substitute("{0}:{1}", new object[]
					{
						Global.Data.roleData.RoleID,
						1
					});
				}
				else
				{
					strcmd = StringUtil.substitute("{0}:{1}", new object[]
					{
						Global.Data.roleData.RoleID,
						0
					});
				}
				TCPGameServerCmds.CMD_SPR_MERLIN_LEVEL_UP.SendData(strcmd);
			};
			UIEventListener.Get(this.m_BtnZiDongShengJi.gameObject).onClick = delegate(GameObject s)
			{
				if (this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text == Global.GetLang("自动升阶"))
				{
					string strcmd = string.Empty;
					if (this.m_chkAuto.Check)
					{
						IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("MeiLingZhiShu", this.zuanShiXiaoHao, false);
						strcmd = StringUtil.substitute("{0}:{1}", new object[]
						{
							Global.Data.roleData.RoleID,
							1
						});
					}
					else
					{
						strcmd = StringUtil.substitute("{0}:{1}", new object[]
						{
							Global.Data.roleData.RoleID,
							0
						});
					}
					TCPGameServerCmds.CMD_SPR_MERLIN_LEVEL_UP.SendData(strcmd);
					this.IsZiDongShengJi = true;
				}
				if (this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text == Global.GetLang("自动升阶"))
				{
					this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text = Global.GetLang("取消自动");
					this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UISprite>().spriteName = "tongyongBtn6_normal";
				}
				else
				{
					this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text = Global.GetLang("自动升阶");
					this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UISprite>().spriteName = "tongyongBtn_normal";
					this.IsZiDongShengJi = false;
				}
			};
		}
		else
		{
			this.m_BgXinJi.gameObject.SetActive(true);
			this.m_chkAuto.Check = false;
			this.m_BtnShengJi.transform.GetComponentInChildren<UILabel>().text = Global.GetLang("升级");
			this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text = Global.GetLang("自动升级");
			this.IsZiDongShengJi = false;
			this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UISprite>().spriteName = "tongyongBtn_normal";
			UIEventListener.Get(this.m_BtnShengJi.gameObject).onClick = delegate(GameObject s)
			{
				if (0f < Global.GetBtnCD(this.m_BtnShengJi.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this.m_BtnShengJi.GetInstanceID(), 0.5f);
				string strcmd = string.Empty;
				if (this.m_chkAuto.Check)
				{
					strcmd = StringUtil.substitute("{0}:{1}", new object[]
					{
						Global.Data.roleData.RoleID,
						1
					});
				}
				else
				{
					strcmd = StringUtil.substitute("{0}:{1}", new object[]
					{
						Global.Data.roleData.RoleID,
						0
					});
				}
				TCPGameServerCmds.CMD_SPR_MERLIN_STAR_UP.SendData(strcmd);
			};
			UIEventListener.Get(this.m_BtnZiDongShengJi.gameObject).onClick = delegate(GameObject s)
			{
				if (this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text == Global.GetLang("自动升级"))
				{
					string strcmd = string.Empty;
					if (this.m_chkAuto.Check)
					{
						IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("MeiLingZhiShu", this.zuanShiXiaoHao, false);
						strcmd = StringUtil.substitute("{0}:{1}", new object[]
						{
							Global.Data.roleData.RoleID,
							1
						});
					}
					else
					{
						strcmd = StringUtil.substitute("{0}:{1}", new object[]
						{
							Global.Data.roleData.RoleID,
							0
						});
					}
					TCPGameServerCmds.CMD_SPR_MERLIN_STAR_UP.SendData(strcmd);
					this.IsZiDongShengJi = true;
				}
				if (this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text == Global.GetLang("自动升级"))
				{
					this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text = Global.GetLang("取消自动");
					this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UISprite>().spriteName = "tongyongBtn6_normal";
				}
				else
				{
					this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text = Global.GetLang("自动升级");
					this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UISprite>().spriteName = "tongyongBtn_normal";
					this.IsZiDongShengJi = false;
				}
			};
		}
	}

	public void aciton_CMD_SPR_MERLIN_LEVEL_UP(MUSocketConnectEventArgs e)
	{
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "MeiLingZhiShu", this.zuanShiXiaoHao, string.Empty);
		int num = ConvertExt.SafeConvertToInt32(e.fields[0]);
		int starNum = ConvertExt.SafeConvertToInt32(e.fields[1]);
		int starExp = ConvertExt.SafeConvertToInt32(e.fields[2]);
		int level = ConvertExt.SafeConvertToInt32(e.fields[3]);
		int levelUpFailNum = ConvertExt.SafeConvertToInt32(e.fields[4]);
		if (num == 0 || num == 9)
		{
			this.DataBag._StarNum = starNum;
			this.DataBag._StarExp = starExp;
			this.DataBag._Level = level;
			this.DataBag.LevelUpFailNum = levelUpFailNum;
			this.InnitData(this.DataBag);
			if (this.m_cashiPart != null)
			{
				this.m_cashiPart.DataBag = this.DataBag;
			}
			base.StartCoroutine(this.BaoJi());
			if (this.IsZiDongShengJi && (!IConfigbase<ConfigDaiBiShiYong>.Instance.CloseZiDong("MeiLingZhiShu", this.zuanShiXiaoHao) || !this.m_chkAuto.Check))
			{
				base.StartCoroutine(this.CMD_SPR_MERLIN_LEVEL_UP_SendToServer());
			}
			else
			{
				this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text = Global.GetLang("自动升阶");
			}
			if (this.DataBag._StarNum != 10)
			{
				base.StartCoroutine(this.BaoJi());
				Transform deco = U3DUtils.NEW<Transform>("Wing_JinJieChengGong");
				if (null != deco)
				{
					U3DUtils.AddChild(base.gameObject, deco.gameObject, false);
					deco.transform.localPosition = new Vector3(-60f, -200f, -1000f);
					UIHelper.DelayInvoke(1.5f, delegate(object s, EventArgs e1)
					{
						Object.Destroy(deco.gameObject);
					});
				}
				this.refreshUIType(this.DataBag);
				base.StartCoroutine(this.LoadModelResource(this.DataBag._Level));
			}
		}
		else
		{
			this.refreshUIType(this.DataBag);
			this.InnitData(this.DataBag);
			if (num == 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("阶数异常"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("已达最高阶"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("未达最高星，无法升阶"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 4)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升阶数据异常"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升阶所需物品ID异常"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升阶所需物品数量异常"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 7)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升阶所需物品不足"), new object[0]), 0, -1, -1, 0);
				PlayZone.GlobalPlayZone.ShowBaodianGuideWindow(BaodianGuidePart.GuideType.NeedMoFaShuiJing, delegate(object s1, DPSelectedItemEventArgs e1)
				{
					Object.Destroy(base.gameObject.GetComponentInParent<GChildWindow>().gameObject);
				}, string.Empty, string.Empty, true);
			}
			else if (num == 8)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升阶所需钻石不足"), new object[0]), 0, -1, -1, 0);
			}
		}
	}

	public void aciton_CMD_SPR_MERLIN_SECRET_ATTR_REPLACE(MUSocketConnectEventArgs e)
	{
		int num = ConvertExt.SafeConvertToInt32(e.fields[0]);
		if (num == 0)
		{
			this.InitData();
			TCPGameServerCmds.CMD_SPR_MERLIN_QUERY.SendDataUseRoleID();
			this.m_ChengGong.gameObject.SetActive(true);
			ActiveAnimation.Play(this.m_ChengGong, 1);
		}
		else if (num == 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请先擦拭秘语"), new object[0]), 0, -1, -1, 0);
		}
		Object.Destroy(this.m_cashiPart.gameObject, 0.01f);
	}

	private IEnumerator CMD_SPR_MERLIN_LEVEL_UP_SendToServer()
	{
		yield return new WaitForSeconds(0.02f);
		string strcmd = string.Empty;
		if (this.m_chkAuto.Check)
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("MeiLingZhiShu", this.zuanShiXiaoHao, false);
			strcmd = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				1
			});
		}
		else
		{
			strcmd = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				0
			});
		}
		if (this.DataBag._StarNum == 10)
		{
			TCPGameServerCmds.CMD_SPR_MERLIN_LEVEL_UP.SendData(strcmd);
		}
		yield break;
	}

	public void aciton_CMD_SPR_MERLIN_STAR_UP(MUSocketConnectEventArgs e)
	{
		int num = ConvertExt.SafeConvertToInt32(e.fields[0]);
		int starNum = ConvertExt.SafeConvertToInt32(e.fields[1]);
		int starExp = ConvertExt.SafeConvertToInt32(e.fields[2]);
		int num2 = ConvertExt.SafeConvertToInt32(e.fields[3]);
		int num3 = 0;
		if (e.fields.Length > 4)
		{
			num3 = ConvertExt.SafeConvertToInt32(e.fields[4]);
		}
		if (num == 0)
		{
			this.DataBag._StarNum = starNum;
			this.DataBag._StarExp = starExp;
			if (this.IsZiDongShengJi && (!IConfigbase<ConfigDaiBiShiYong>.Instance.CloseZiDong("MeiLingZhiShu", this.zuanShiXiaoHao) || !this.m_chkAuto.Check))
			{
				base.StartCoroutine(this.CMD_SPR_MERLIN_STAR_UP_SendToServer());
			}
			else
			{
				this.m_BtnZiDongShengJi.transform.GetComponentInChildren<UILabel>().text = Global.GetLang("自动升级");
			}
			if (this.DataBag._StarNum == 10)
			{
				this.refreshUIType(this.DataBag);
			}
			if (num2 == 1)
			{
				base.StartCoroutine(this.BaoJi());
			}
			if (num2 == 1)
			{
				Super.HintMainText(Global.GetLang("暴增{17e43e}+") + num3 + "{-}", 10, 3);
			}
			else
			{
				Super.HintMainText(Global.GetLang("{17e43e}+") + num3 + "{-}", 10, 3);
			}
			this.InnitData(this.DataBag);
		}
		else
		{
			this.InnitData(this.DataBag);
			this.refreshUIType(this.DataBag);
			if (num == 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("已达最高星"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升星数据异常"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升星所需物品ID异常"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 4)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升星所需物品数量异常"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升星所需物品不足"), new object[0]), 0, -1, -1, 0);
				PlayZone.GlobalPlayZone.ShowBaodianGuideWindow(BaodianGuidePart.GuideType.NeedMoFaJingYanShu, delegate(object s1, DPSelectedItemEventArgs e1)
				{
					Object.Destroy(base.gameObject.GetComponentInParent<GChildWindow>().gameObject);
				}, string.Empty, string.Empty, true);
			}
			else if (num == 6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升星所需物品不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 7)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("阶数异常"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == 8)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("星数异常"), new object[0]), 0, -1, -1, 0);
			}
		}
	}

	private IEnumerator BaoJi()
	{
		this.m_SpriteMeiLanJingYanBg.enabled = true;
		this.m_SpriteMeiLanJingYanBg1.enabled = true;
		yield return new WaitForSeconds(0.2f);
		this.m_SpriteMeiLanJingYanBg.enabled = false;
		this.m_SpriteMeiLanJingYanBg1.enabled = false;
		yield break;
	}

	private IEnumerator CMD_SPR_MERLIN_STAR_UP_SendToServer()
	{
		yield return new WaitForSeconds(0.01f);
		string strcmd = string.Empty;
		if (this.m_chkAuto.Check)
		{
			strcmd = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				1
			});
		}
		else
		{
			strcmd = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				0
			});
		}
		if (this.DataBag._StarNum != 10)
		{
			TCPGameServerCmds.CMD_SPR_MERLIN_STAR_UP.SendData(strcmd);
		}
		yield break;
	}

	public static void OpenWindow()
	{
		GChildWindow gchildWindow = U3DUtils.NEW<GChildWindow>();
		gchildWindow.IsShowModal = true;
		gchildWindow.Modal = true;
		MeiLanShuPart obj = U3DUtils.NEW<MeiLanShuPart>();
		Super.InitChildWindow(gchildWindow, "_meilanshuWindow");
		Super.GData.GlobalPlayZone.Children.Add(gchildWindow);
		gchildWindow.ModalType = ChildWindowModalType.BlackBak;
		gchildWindow.SetContent(gchildWindow.BodyPresenter, obj, 0.0, 0.0, true);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public TextBlock[] staticText;

	public UILabel staticRule;

	public UIButton m_BtnClose;

	public UIButton m_BtnShengJi;

	public UIButton m_BtnZiDongShengJi;

	public UIButton m_BtnCaShi;

	public UIButton m_BtnXianShiMiYu;

	public UIButton m_BtnCloseRule;

	private Modal3DShow m_Modal3DShow;

	public GameObject show3DModel;

	public Modal3DShow m_Modal3DShowSaoBa;

	public Modal3DShow m_Modal3DShowTeXiao;

	public Transform m_3D;

	public Transform m_Rule;

	public UILabel m_LableJiChuShuXing;

	public UILabel m_LableMeiLanJingYan;

	public UILabel m_LabelJieShu;

	public UILabel m_LabelZuanShi;

	public UILabel m_LableShengYuShiJian;

	public UILabel m_LableShengJiXiaoHao;

	public UILabel m_LableCaShiXiaoHao;

	public UILabel m_LableXiaoHaoZuanShi;

	public UISprite m_SpriteMeiLanJingYan;

	public UISprite m_SpriteMeiLanJingYanBg;

	public UISprite m_SpriteMeiLanJingYanBg1;

	public UISprite m_BackgroundRevive;

	public UISprite m_BackgroundMagicRecover;

	public UISprite[] m_BackgroundShuXingArr;

	public UISprite[] m_BgXinJiArr;

	public Transform m_BgXinJi;

	public GCheckBox m_chkAuto;

	public Transform m_IconShengJi;

	public Transform m_IconCaShiXiaoHao;

	public Animation m_ChengGong;

	public UILabel m_LableAttack;

	public UILabel m_LableDefense;

	public UILabel m_LableMDefenseV;

	public UILabel m_LableHitV;

	public UILabel m_LableDodge;

	public UILabel m_LableMaxLifeV;

	public UILabel m_LableRevive;

	public UILabel m_LableMagicRecover;

	public UILabel m_LableAttackC;

	public UILabel m_LableDefenseC;

	public UILabel m_LableMDefenseVC;

	public UILabel m_LableHitVC;

	public UILabel m_LableDodgeC;

	public UILabel m_LableMaxLifeVC;

	public UILabel m_LableReviveC;

	public UILabel m_LableMagicRecoverC;

	private bool IsCashiZhong;

	private bool IsZiDongShengJi;

	public GDecoration decoMiYu;

	public Transform m_TransdecoMiYu;

	private MeiLanShuCaShiPart m_cashiPart;

	public List<UISprite> listDaiBi = new List<UISprite>();

	private int mcountGoodNum;

	private int goodid = -1;

	private int zuanShiXiaoHao;

	private ResourceResLoader resourceLoader;

	private Animation _Animation;

	private float ControlTime;

	public MerlinGrowthSaveDBData DataBag;
}
