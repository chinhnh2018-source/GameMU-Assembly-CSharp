using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class BaodianGuidePart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblHint.text = Global.GetLang("获得途径");
		this.lblPrompt.text = Global.GetLang("提示");
	}

	public void SetWindowType(int GoodsID, int[] baoDianIDs)
	{
		base.StartCoroutine<bool>(this.ActiveWind());
		if (baoDianIDs != null)
		{
			int num = baoDianIDs.Length;
			if (4 <= num)
			{
				num = 4;
			}
			this.SetBtnPosition(num);
			for (int i = 0; i < num; i++)
			{
				GButton gbutton = null;
				if (i == 0)
				{
					gbutton = this.btn1;
				}
				else if (i == 1)
				{
					gbutton = this.btn2;
				}
				else if (i == 2)
				{
					gbutton = this.btn3;
				}
				else if (i == 3)
				{
					gbutton = this.btn4;
				}
				if (null != gbutton)
				{
					XElement ele = null;
					if (this.baodianPromptDict.TryGetValue(baoDianIDs[i], ref ele))
					{
						this.InitButton(gbutton, baoDianIDs[i], ele);
					}
				}
			}
		}
	}

	protected override void InitializeComponent()
	{
		UIWidget[] componentsInChildren = base.GetComponentsInChildren<UIWidget>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.wList.Add(componentsInChildren[i]);
				if (null != componentsInChildren[i])
				{
					this.aList.Add(componentsInChildren[i].alpha);
					componentsInChildren[i].alpha = 0.0001f;
				}
				else
				{
					this.aList.Add(0f);
				}
			}
		}
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			}
		};
		this.LoadBaoDianList();
	}

	private void LoadBaoDianList()
	{
		XElement gameResXml = Global.GetGameResXml("Config/BaoDian.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "BaoDian");
		for (int i = 0; i < xelementList.Count; i++)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "ID");
			this.baodianPromptDict.Add(xelementAttributeInt, xelementList[i]);
		}
	}

	private bool ParseBtnAction(int key, out string desc, out int actionType, out string action)
	{
		XElement confEle = null;
		bool result = false;
		desc = string.Empty;
		actionType = -1;
		action = string.Empty;
		if (this.baodianPromptDict.TryGetValue(key, ref confEle))
		{
			result = BaoDianPartPage.ConditionCheck(confEle, out desc);
			actionType = BaoDianPartPage.ParseAction(confEle, out action);
		}
		return result;
	}

	private void SetBtnPosition(int btnCount)
	{
		if (btnCount == 1)
		{
			this.btn1.gameObject.SetActive(true);
			this.btn2.gameObject.SetActive(false);
			this.btn3.gameObject.SetActive(false);
			this.btn4.gameObject.SetActive(false);
		}
		else if (btnCount == 2)
		{
			this.btn1.gameObject.SetActive(true);
			this.btn2.gameObject.SetActive(true);
			this.btn3.gameObject.SetActive(false);
			this.btn4.gameObject.SetActive(false);
		}
		else if (btnCount == 3)
		{
			this.btn1.gameObject.SetActive(true);
			this.btn2.gameObject.SetActive(true);
			this.btn3.gameObject.SetActive(true);
			this.btn4.gameObject.SetActive(false);
		}
		else if (btnCount == 4)
		{
			this.btn1.gameObject.SetActive(true);
			this.btn2.gameObject.SetActive(true);
			this.btn3.gameObject.SetActive(true);
			this.btn4.gameObject.SetActive(true);
		}
	}

	private void InitDPSelectedItem(int actionType, string actionValue)
	{
		if (this.DPSelectedItem != null)
		{
			if (actionType != 0)
			{
				if (actionType == 1)
				{
					string[] array = actionValue.Split(new char[]
					{
						','
					});
					if (array.Length == 2)
					{
						int id = int.Parse(array[0]);
						int index = int.Parse(array[1]);
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							IDType = actionType,
							ID = id,
							Index = index
						});
					}
				}
			}
			else if (actionValue == "409" && Global.Data.roleData.Faction == 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetColorStringForNGUIText(new object[]
				{
					"ffffff",
					Global.GetLang("您未加入任何战盟！！！")
				}), 0, -1, -1, 0);
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = actionType,
					ID = int.Parse(actionValue)
				});
			}
		}
	}

	private void OnEnable()
	{
		if (0 >= this.wList.Count)
		{
			UIWidget[] componentsInChildren = base.GetComponentsInChildren<UIWidget>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					this.wList.Add(componentsInChildren[i]);
					if (null != componentsInChildren[i])
					{
						this.aList.Add(componentsInChildren[i].alpha);
						componentsInChildren[i].alpha = 0.0001f;
					}
					else
					{
						this.aList.Add(0f);
					}
				}
			}
		}
	}

	private void OnDisable()
	{
		if (0 < this.wList.Count)
		{
			for (int i = 0; i < this.wList.Count; i++)
			{
				if (null != this.wList[i])
				{
					this.wList[i].alpha = this.aList[i];
				}
			}
		}
		this.wList.Clear();
		this.aList.Clear();
	}

	private void InitButton(GButton btn, int key, XElement ele)
	{
		string empty = string.Empty;
		string action = string.Empty;
		int actionType = -1;
		string text = string.Empty;
		btn.isEnabled = this.ParseBtnAction(key, out empty, out actionType, out action);
		btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.InitDPSelectedItem(actionType, action);
		};
		text = Global.GetXElementAttributeStr(ele, "Image");
		btn.Label.text = Global.GetXElementAttributeStr(ele, "Name");
		btn.normalSprite = text;
		btn.hoverSprite = text;
		btn.pressedSprite = text;
		btn.disabledSprite = text;
		btn.target.spriteName = text;
	}

	private IEnumerator ActiveWind()
	{
		yield return new WaitForEndOfFrame();
		if (0 < this.wList.Count)
		{
			for (int i = 0; i < this.wList.Count; i++)
			{
				if (null != this.wList[i])
				{
					this.wList[i].alpha = this.aList[i];
				}
			}
		}
		yield break;
	}

	private IEnumerator CloseMessage()
	{
		yield return new WaitForEndOfFrame();
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = -10
			});
		}
		yield break;
	}

	public void SetWindowType(BaodianGuidePart.GuideType type, string customLinkIDs = "", string customTitle = "")
	{
		GoodsObtainVO goodsObtainCoinId = ConfigGoodsObtain.GetGoodsObtainCoinId((int)type);
		if (goodsObtainCoinId != null)
		{
			int[] baoDianIDArray = goodsObtainCoinId.BaoDianIDArray;
			if (baoDianIDArray != null && 0 < baoDianIDArray.Length)
			{
				this.SetWindowType(goodsObtainCoinId.GoodsID, baoDianIDArray);
				return;
			}
			string name = goodsObtainCoinId.Name;
			if (!string.IsNullOrEmpty(name))
			{
				Super.HintMainText(name, 10, 3);
				base.StartCoroutine<bool>(this.CloseMessage());
				return;
			}
		}
		base.StartCoroutine<bool>(this.ActiveWind());
		string text = "{ff9c00}";
		XElement ele = null;
		switch (type)
		{
		case BaodianGuidePart.GuideType.NeedJinbi:
			this.lblPrompt.text = text + Global.GetLang("【金币/绑定金币】{-}");
			this.SetBtnPosition(4);
			if (this.baodianPromptDict.TryGetValue(602, ref ele))
			{
				this.InitButton(this.btn1, 602, ele);
			}
			if (this.baodianPromptDict.TryGetValue(301, ref ele))
			{
				this.InitButton(this.btn2, 301, ele);
			}
			if (this.baodianPromptDict.TryGetValue(501, ref ele))
			{
				this.InitButton(this.btn3, 501, ele);
			}
			if (this.baodianPromptDict.TryGetValue(300, ref ele))
			{
				this.InitButton(this.btn4, 300, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedMojing:
			this.lblPrompt.text = text + Global.GetLang("【魔晶】{-}");
			this.SetBtnPosition(3);
			if (this.baodianPromptDict.TryGetValue(405, ref ele))
			{
				this.InitButton(this.btn1, 405, ele);
			}
			if (this.baodianPromptDict.TryGetValue(101, ref ele))
			{
				this.InitButton(this.btn2, 101, ele);
			}
			if (this.baodianPromptDict.TryGetValue(105, ref ele))
			{
				this.InitButton(this.btn3, 105, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedZuanshi:
			this.lblPrompt.text = text + Global.GetLang("【钻石】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(601, ref ele))
			{
				this.InitButton(this.btn1, 601, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedYumao:
			this.lblPrompt.text = text + Global.GetLang("【羽毛】{-}");
			this.SetBtnPosition(3);
			if (this.baodianPromptDict.TryGetValue(602, ref ele))
			{
				this.InitButton(this.btn1, 602, ele);
			}
			if (this.baodianPromptDict.TryGetValue(504, ref ele))
			{
				this.InitButton(this.btn2, 504, ele);
			}
			if (this.baodianPromptDict.TryGetValue(406, ref ele))
			{
				this.InitButton(this.btn3, 406, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedHuozhong:
			this.lblPrompt.text = text + Global.GetLang("【神鹰火种】{-}");
			this.SetBtnPosition(3);
			if (this.baodianPromptDict.TryGetValue(602, ref ele))
			{
				this.InitButton(this.btn1, 602, ele);
			}
			if (this.baodianPromptDict.TryGetValue(504, ref ele))
			{
				this.InitButton(this.btn2, 504, ele);
			}
			if (this.baodianPromptDict.TryGetValue(406, ref ele))
			{
				this.InitButton(this.btn3, 406, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedZhufujingshi:
			this.lblPrompt.text = text + Global.GetLang("【祝福晶石】{-}");
			this.SetBtnPosition(4);
			if (this.baodianPromptDict.TryGetValue(407, ref ele))
			{
				this.InitButton(this.btn1, 407, ele);
			}
			if (this.baodianPromptDict.TryGetValue(300, ref ele))
			{
				this.InitButton(this.btn2, 300, ele);
			}
			if (this.baodianPromptDict.TryGetValue(406, ref ele))
			{
				this.InitButton(this.btn3, 406, ele);
			}
			if (this.baodianPromptDict.TryGetValue(403, ref ele))
			{
				this.InitButton(this.btn4, 403, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedLinghunjingshi:
			this.lblPrompt.text = text + Global.GetLang("【灵魂晶石】{-}");
			this.SetBtnPosition(4);
			if (this.baodianPromptDict.TryGetValue(407, ref ele))
			{
				this.InitButton(this.btn1, 407, ele);
			}
			if (this.baodianPromptDict.TryGetValue(500, ref ele))
			{
				this.InitButton(this.btn2, 500, ele);
			}
			if (this.baodianPromptDict.TryGetValue(406, ref ele))
			{
				this.InitButton(this.btn3, 406, ele);
			}
			if (this.baodianPromptDict.TryGetValue(403, ref ele))
			{
				this.InitButton(this.btn4, 403, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedMayajingshi:
			this.lblPrompt.text = text + Global.GetLang("【玛雅晶石】{-}");
			this.SetBtnPosition(4);
			if (this.baodianPromptDict.TryGetValue(407, ref ele))
			{
				this.InitButton(this.btn1, 407, ele);
			}
			if (this.baodianPromptDict.TryGetValue(500, ref ele))
			{
				this.InitButton(this.btn2, 500, ele);
			}
			if (this.baodianPromptDict.TryGetValue(406, ref ele))
			{
				this.InitButton(this.btn3, 406, ele);
			}
			if (this.baodianPromptDict.TryGetValue(403, ref ele))
			{
				this.InitButton(this.btn4, 403, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedShengmingjingshi:
			this.lblPrompt.text = text + Global.GetLang("【生命晶石】{-}");
			this.SetBtnPosition(3);
			if (this.baodianPromptDict.TryGetValue(500, ref ele))
			{
				this.InitButton(this.btn1, 500, ele);
			}
			if (this.baodianPromptDict.TryGetValue(406, ref ele))
			{
				this.InitButton(this.btn2, 406, ele);
			}
			if (this.baodianPromptDict.TryGetValue(403, ref ele))
			{
				this.InitButton(this.btn3, 403, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedChuangzaojingshi:
			this.lblPrompt.text = text + Global.GetLang("【创造晶石】{-}");
			this.SetBtnPosition(2);
			if (this.baodianPromptDict.TryGetValue(503, ref ele))
			{
				this.InitButton(this.btn1, 503, ele);
			}
			if (this.baodianPromptDict.TryGetValue(403, ref ele))
			{
				this.InitButton(this.btn2, 403, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedShenyoujingshi:
			this.lblPrompt.text = text + Global.GetLang("【神佑晶石】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(407, ref ele))
			{
				this.InitButton(this.btn1, 407, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedJinhuajingshi:
			this.lblPrompt.text = text + Global.GetLang("【进化晶石】{-}");
			this.SetBtnPosition(2);
			if (this.baodianPromptDict.TryGetValue(503, ref ele))
			{
				this.InitButton(this.btn1, 503, ele);
			}
			if (this.baodianPromptDict.TryGetValue(406, ref ele))
			{
				this.InitButton(this.btn2, 406, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedCustom:
		{
			this.lblPrompt.text = text + Global.GetLang("【") + customTitle + Global.GetLang("】{-}");
			string[] array = customLinkIDs.Split(new char[]
			{
				','
			});
			int num = array.Length;
			this.SetBtnPosition(num);
			for (int i = 0; i < num; i++)
			{
				GButton btn = this.btn1;
				if (i == 0)
				{
					btn = this.btn1;
				}
				else if (i == 1)
				{
					btn = this.btn2;
				}
				else if (i == 2)
				{
					btn = this.btn3;
				}
				else if (i == 3)
				{
					btn = this.btn4;
				}
				int num2 = Convert.ToInt32(array[i]);
				if (this.baodianPromptDict.TryGetValue(num2, ref ele))
				{
					this.InitButton(btn, num2, ele);
				}
			}
			break;
		}
		case BaodianGuidePart.GuideType.NeedQifujifen:
			this.lblPrompt.text = text + Global.GetLang("【祈福积分】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(400, ref ele))
			{
				this.InitButton(this.btn1, 400, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedLingJing:
			this.lblPrompt.text = text + Global.GetLang("【") + customTitle + Global.GetLang("】{-}");
			this.SetBtnPosition(2);
			if (this.baodianPromptDict.TryGetValue(603, ref ele))
			{
				this.InitButton(this.btn1, 603, ele);
			}
			if (this.baodianPromptDict.TryGetValue(604, ref ele))
			{
				this.InitButton(this.btn2, 604, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedYuansuFenmo:
			this.lblPrompt.text = text + Global.GetLang("【") + customTitle + Global.GetLang("】{-}");
			this.SetBtnPosition(2);
			if (this.baodianPromptDict.TryGetValue(503, ref ele))
			{
				this.InitButton(this.btn1, 503, ele);
			}
			if (this.baodianPromptDict.TryGetValue(604, ref ele))
			{
				this.InitButton(this.btn2, 604, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedZaizaodian:
			this.lblPrompt.text = text + Global.GetLang("【再造点数】{-}");
			this.SetBtnPosition(3);
			if (this.baodianPromptDict.TryGetValue(602, ref ele))
			{
				this.InitButton(this.btn1, 602, ele);
			}
			if (this.baodianPromptDict.TryGetValue(500, ref ele))
			{
				this.InitButton(this.btn2, 500, ele);
			}
			if (this.baodianPromptDict.TryGetValue(405, ref ele))
			{
				this.InitButton(this.btn3, 405, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedChengJiu:
			this.lblPrompt.text = text + Global.GetLang("【成就点数】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(602, ref ele))
			{
				this.InitButton(this.btn1, 602, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedZaizaojingshi:
			this.lblPrompt.text = text + Global.GetLang("【再造晶石】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(602, ref ele))
			{
				this.InitButton(this.btn1, 602, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedShangguSuipian:
			this.lblPrompt.text = text + Global.GetLang("【上古装备碎片】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(300, ref ele))
			{
				this.InitButton(this.btn1, 300, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedZhuLing:
			this.lblPrompt.text = text + Global.GetLang("【") + customTitle + Global.GetLang("】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(606, ref ele))
			{
				this.InitButton(this.btn1, 606, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedShengWang:
			this.lblPrompt.text = text + Global.GetLang("【声望点数】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(602, ref ele))
			{
				this.InitButton(this.btn1, 602, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedGuardStone:
			this.lblPrompt.text = text + Global.GetLang("【守护精华】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(100, ref ele))
			{
				this.InitButton(this.btn1, 100, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedMoFaJingYanShu:
			this.lblPrompt.text = text + Global.GetLang("【魔法经验书】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(608, ref ele))
			{
				this.InitButton(this.btn1, 608, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedMoFaShuiJing:
			this.lblPrompt.text = text + Global.GetLang("【魔法结晶】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(608, ref ele))
			{
				this.InitButton(this.btn1, 608, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedMoFaSaoZhou:
			this.lblPrompt.text = text + Global.GetLang("【魔法扫帚】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(608, ref ele))
			{
				this.InitButton(this.btn1, 608, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedSuiPian:
			this.lblPrompt.text = text + Global.GetLang("【部件碎片】{-}");
			this.SetBtnPosition(2);
			if (this.baodianPromptDict.TryGetValue(609, ref ele))
			{
				this.InitButton(this.btn1, 609, ele);
			}
			if (this.baodianPromptDict.TryGetValue(611, ref ele))
			{
				this.InitButton(this.btn2, 611, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedFluorescentPoint:
			this.lblPrompt.text = text + Global.GetLang("【荧光粉末】{-}");
			this.SetBtnPosition(2);
			if (this.baodianPromptDict.TryGetValue(610, ref ele))
			{
				this.InitButton(this.btn1, 610, ele);
			}
			if (this.baodianPromptDict.TryGetValue(611, ref ele))
			{
				this.InitButton(this.btn2, 611, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedLangHunFenMo:
			this.lblPrompt.text = text + Global.GetLang("【狼魂粉末】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(612, ref ele))
			{
				this.InitButton(this.btn1, 612, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedYuanguSuipian:
			this.lblPrompt.text = text + Global.GetLang("【远古装备碎片】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(613, ref ele))
			{
				this.InitButton(this.btn1, 613, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedJuHunJingShi:
			this.lblPrompt.text = text + Global.GetLang("【聚魂晶石】{-}");
			this.SetBtnPosition(3);
			if (this.baodianPromptDict.TryGetValue(614, ref ele))
			{
				this.InitButton(this.btn1, 614, ele);
			}
			if (this.baodianPromptDict.TryGetValue(615, ref ele))
			{
				this.InitButton(this.btn2, 615, ele);
			}
			if (this.baodianPromptDict.TryGetValue(616, ref ele))
			{
				this.InitButton(this.btn3, 616, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedJuLingJingShi:
			this.lblPrompt.text = text + Global.GetLang("【聚灵晶石】{-}");
			this.SetBtnPosition(3);
			if (this.baodianPromptDict.TryGetValue(614, ref ele))
			{
				this.InitButton(this.btn1, 614, ele);
			}
			if (this.baodianPromptDict.TryGetValue(615, ref ele))
			{
				this.InitButton(this.btn2, 615, ele);
			}
			if (this.baodianPromptDict.TryGetValue(616, ref ele))
			{
				this.InitButton(this.btn3, 616, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedShenLiJingHua:
			this.lblPrompt.text = text + Global.GetLang("【神力精华】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(617, ref ele))
			{
				this.InitButton(this.btn1, 617, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedHuiJiZhiGuang:
			this.lblPrompt.text = text + Global.GetLang("【徽记之光】{-}");
			this.SetBtnPosition(2);
			if (this.baodianPromptDict.TryGetValue(615, ref ele))
			{
				this.InitButton(this.btn1, 615, ele);
			}
			if (this.baodianPromptDict.TryGetValue(604, ref ele))
			{
				this.InitButton(this.btn2, 604, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedHuiJiShouHu:
			this.lblPrompt.text = text + Global.GetLang("【徽记守护】{-}");
			this.SetBtnPosition(2);
			if (this.baodianPromptDict.TryGetValue(615, ref ele))
			{
				this.InitButton(this.btn1, 615, ele);
			}
			if (this.baodianPromptDict.TryGetValue(604, ref ele))
			{
				this.InitButton(this.btn2, 604, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedFuWen:
			this.lblPrompt.text = text + Global.GetLang("【符文大厅】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(618, ref ele))
			{
				this.InitButton(this.btn1, 618, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedJueXing:
			this.lblPrompt.text = text + Global.GetLang("【觉醒碎片】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(619, ref ele))
			{
				this.InitButton(this.btn1, 619, ele);
			}
			break;
		case BaodianGuidePart.GuideType.HorseLieQu:
			this.lblPrompt.text = text + Global.GetLang("【坐骑】{-}");
			this.SetBtnPosition(2);
			if (this.baodianPromptDict.TryGetValue(628, ref ele))
			{
				this.InitButton(this.btn1, 628, ele);
			}
			if (this.baodianPromptDict.TryGetValue(629, ref ele))
			{
				this.InitButton(this.btn2, 629, ele);
			}
			break;
		case BaodianGuidePart.GuideType.NeedMoShenMiBao:
			this.lblPrompt.text = text + Global.GetLang("【魔神秘宝】{-}");
			this.SetBtnPosition(1);
			if (this.baodianPromptDict.TryGetValue(608, ref ele))
			{
				this.InitButton(this.btn1, 608, ele);
			}
			break;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton btnClose;

	public GButton btn1;

	public GButton btn2;

	public GButton btn3;

	public GButton btn4;

	public UILabel lblPrompt;

	public UILabel lblHint;

	private Dictionary<int, XElement> baodianPromptDict = new Dictionary<int, XElement>();

	private List<UIWidget> wList = new List<UIWidget>();

	private List<float> aList = new List<float>();

	public enum GuideType
	{
		NeedJinbi,
		NeedMojing,
		NeedZuanshi,
		NeedYumao,
		NeedHuozhong,
		NeedZhufujingshi,
		NeedLinghunjingshi,
		NeedMayajingshi,
		NeedShengmingjingshi,
		NeedChuangzaojingshi,
		NeedShenyoujingshi,
		NeedJinhuajingshi,
		NeedCustom,
		NeedQifujifen,
		NeedLingJing,
		NeedYuansuFenmo,
		NeedZaizaodian,
		NeedChengJiu,
		NeedZaizaojingshi,
		NeedShangguSuipian,
		NeedZhuLing,
		NeedShengWang,
		NeedGuardStone,
		NeedMoFaJingYanShu,
		NeedMoFaShuiJing,
		NeedMoFaSaoZhou,
		NeedSuiPian,
		NeedFluorescentPoint,
		NeedLangHunFenMo,
		NeedYuanguSuipian,
		NeedJuHunJingShi,
		NeedJuLingJingShi,
		NeedShenLiJingHua,
		NeedHuiJiZhiGuang,
		NeedHuiJiShouHu,
		NeedFuWen,
		NeedJueXing,
		HorseLieQu,
		JueXingJingHua,
		NeedMoShenMiBao
	}

	public enum GuideGoodsID
	{
		Yumao = 2016,
		Huozhong,
		LuoKeZhiYu,
		Zhufujingshi = 2001,
		Mayajingshi = 2000,
		Linghunjingshi = 2002,
		Shengmingjingshi,
		Chuangzaojingshi,
		Shenyoujingshi,
		Zaizaojingshi = 2031,
		ShangguSuipianFrom = 2070,
		ShangguSuipianTo = 2075,
		YuanguSuipianFrom,
		YuanguSuipianTo = 2081,
		ZhenZhiZhuFu = 2130,
		JuLingJingShi = 2033,
		JuHunJingShi,
		ShenLiJingHua
	}
}
