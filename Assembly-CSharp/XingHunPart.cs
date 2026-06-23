using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class XingHunPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_LblChildPageTopTile.text = Global.GetLang("永久属性加成");
		this.m_LblWindowTitle2.text = this.m_LblChildPageTopTile.text;
		this.m_LblBottomChildPageTopTile.text = Global.GetLang("激活需要");
		this.m_BtnJiHuo.Text = Global.GetLang("激活");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.m_ObjMainPage.gameObject.SetActive(true);
		this.m_ObjChildPage.gameObject.SetActive(false);
		this.m_BtnJiHuo.isEnabled = false;
		this.m_TexLineBak.gameObject.SetActive(false);
		this.m_TexLineBak.alpha = 0f;
		this.ItemCollection = this.m_LstXingZuo.ItemsSource;
		this.m_LstXingZuo.SelectionChanged = new MouseLeftButtonUpEventHandler(this.LstXingZuo_SelectionChanged);
		if (null != this.m_LblWindowTitle)
		{
			this.m_LblWindowTitle.text = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
			{
				"e9bb6f",
				Global.GetLang("星魂")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"e9bb6f",
				": "
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"f5e2ba",
				Convert.ToString(Global.Data.roleData.StarSoulValue)
			}));
		}
		this.LblItemCollection = this.m_LstLbl.ItemsSource;
		this.LblChildItemCollection = this.m_LstChildLbl.ItemsSource;
		this.StarNodeCollection = this.m_LstStarNode.ItemsSource;
		if (null != this.m_PosPnl)
		{
			UIDraggablePanel component = this.m_PosPnl.GetComponent<UIDraggablePanel>();
			if (null != component)
			{
				component.onDragFinished = delegate()
				{
					this.ChangeDragPos();
				};
			}
		}
		UIEventListener.Get(this.m_DragPnl.gameObject).onClick = delegate(GameObject gameObject)
		{
			this.ChangeDragPos();
		};
		UIEventListener.Get(this.m_DragPnl.gameObject).onDrag = delegate(GameObject gameObject, Vector2 delta)
		{
			this.ChangeDragPos();
		};
		UIEventListener.Get(this.m_TexBtn.gameObject).onClick = delegate(GameObject gameObject)
		{
			this.XingZuoItemClick();
		};
		if (null != this.m_BtnJiHuo)
		{
			this.m_BtnJiHuo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.ShowMsg())
				{
					Super.ShowNetWaiting(null);
					if (this.m_bIsNodeActive || null != this.m_ClickXingHunItem || this.m_bIsReFlush)
					{
						Super.HideNetWaiting();
						return;
					}
					int nStarSite = this.m_nSelectIndex + 1;
					GameInstance.Game.SpriteActivationStarConstelltionInfoCmd(nStarSite);
				}
				SystemHelpMgr.OnAction(UIObjIDs.XingHunPart_JiHuo, HelpStateEvents.Clicked, -1);
			};
		}
		GameInstance.Game.SpriteQueryStarConstelltionInfoCmd();
	}

	private void RetunrMainUI()
	{
		this.m_BtnJiHuo.isEnabled = false;
		this.m_SprJieShu.gameObject.SetActive(false);
		this.m_bIsClick = false;
		this.m_ClickXingHunItem = null;
		this.m_DicTotalProperty.Clear();
		this.m_DicXingZuo.Clear();
		this.m_DicActiveStar.Clear();
		this.StarNodeCollection.Clear();
		this.ItemCollection.Clear();
		this.LblItemCollection.Clear();
		this.m_TexLineScaleBak.gameObject.SetActive(false);
		this.ReSetControlState(false, true);
		GameInstance.Game.SpriteQueryStarConstelltionInfoCmd();
	}

	private void ShowText(string str)
	{
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang(str), new object[0]), 0, -1, -1, 0);
	}

	private bool JudgeCondition(int nType)
	{
		int num = 0;
		if (this.m_DicActiveStar.ContainsKey(this.m_nSelectIndex + 1))
		{
			num = this.m_DicActiveStar[this.m_nSelectIndex + 1];
		}
		if (num >= this.m_nMaxJieShu)
		{
			num--;
		}
		switch (nType)
		{
		case 0:
		{
			string[] array = this.m_DicXingZuo[this.m_strCurrentXingZuoName].LstNode[num].strLevelLimit.Split(new char[]
			{
				','
			});
			return Global.Data.roleData.ChangeLifeCount > Convert.ToInt32(array[0]) || (Global.Data.roleData.ChangeLifeCount == Convert.ToInt32(array[0]) && Global.Data.roleData.Level >= Convert.ToInt32(array[1]));
		}
		case 1:
		{
			string strNeedGoods = this.m_DicXingZuo[this.m_strCurrentXingZuoName].LstNode[num].strNeedGoods;
			return Convert.ToInt32(strNeedGoods) <= Global.Data.roleData.YinLiang + Global.Data.roleData.Money1;
		}
		case 2:
			return Global.Data.roleData.StarSoulValue >= this.m_DicXingZuo[this.m_strCurrentXingZuoName].LstNode[num].nXingHun;
		default:
			return false;
		}
	}

	private bool ShowMsg()
	{
		int num = 0;
		if (this.m_DicActiveStar.ContainsKey(this.m_nSelectIndex + 1))
		{
			num = this.m_DicActiveStar[this.m_nSelectIndex + 1];
		}
		if (this.m_nMaxNode <= num)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetColorStringForNGUIText(new object[]
			{
				"ffffff",
				Global.GetLang("当前星座星魂已全部激活！")
			}), 0, -1, -1, 0);
			return false;
		}
		if (Global.Data.roleData.StarSoulValue < this.m_DicXingZuo[this.m_strCurrentXingZuoName].LstNode[num].nXingHun)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetColorStringForNGUIText(new object[]
			{
				"ffffff",
				Global.GetLang("星魂值不足！！！")
			}), 0, -1, -1, 0);
			return false;
		}
		string strNeedGoods = this.m_DicXingZuo[this.m_strCurrentXingZuoName].LstNode[num].strNeedGoods;
		if (Convert.ToInt32(strNeedGoods) > Global.Data.roleData.YinLiang + Global.Data.roleData.Money1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetColorStringForNGUIText(new object[]
			{
				"ffffff",
				Global.GetLang("金币不足！！！")
			}), 0, -1, -1, 0);
			return false;
		}
		string[] array = this.m_DicXingZuo[this.m_strCurrentXingZuoName].LstNode[num].strLevelLimit.Split(new char[]
		{
			','
		});
		if (Global.Data.roleData.ChangeLifeCount <= Convert.ToInt32(array[0]))
		{
			if (Global.Data.roleData.ChangeLifeCount != Convert.ToInt32(array[0]))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetColorStringForNGUIText(new object[]
				{
					"ffffff",
					Global.GetLang("转生等级不足！！！")
				}), 0, -1, -1, 0);
				return false;
			}
			if (Global.Data.roleData.Level < Convert.ToInt32(array[1]))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetColorStringForNGUIText(new object[]
				{
					"ffffff",
					Global.GetLang("等级不足！！！")
				}), 0, -1, -1, 0);
				return false;
			}
		}
		return true;
	}

	private Dictionary<int, int> FixedNode(Dictionary<int, int> dic)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (KeyValuePair<int, int> keyValuePair in dic)
		{
			if (keyValuePair.Value > this.m_nMaxNode)
			{
				dictionary.Add(keyValuePair.Key, this.m_nMaxNode);
			}
			else
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}
		return dictionary;
	}

	public void RecvServerCommd(Dictionary<int, int> dic)
	{
		if (dic != null)
		{
			this.m_DicActiveStar = this.FixedNode(dic);
		}
		base.StartCoroutine<bool>(this.InitXmlProc());
		SystemHelpMgr.OnAction(UIObjIDs.XingHunPart, HelpStateEvents.Actived, -1);
	}

	public void ReFlushUI(Dictionary<int, int> dic)
	{
		Super.HideNetWaiting();
		this.m_bIsReFlush = true;
		if (dic != null)
		{
			if (this.m_DicActiveStar.ContainsKey(this.m_nSelectIndex + 1))
			{
				this.m_nDangQianJie = this.m_DicActiveStar[this.m_nSelectIndex + 1] / 12;
			}
			this.m_DicActiveStar = this.FixedNode(dic);
			dic = this.m_DicActiveStar;
			int num = this.m_nSelectIndex + 1;
			if (dic.ContainsKey(num))
			{
				int num2 = dic[num];
				num2--;
				if (this.m_nDangQianJie != (num2 + 1) / 12 && 1 < num2)
				{
					string xingZuoPropertyStr = this.GetXingZuoPropertyStr(this.m_nDangQianJie);
					PlayZone.GlobalPlayZone.OpenXingHunJinJie(xingZuoPropertyStr);
					if (null != this.m_SprJieShu)
					{
						this.m_SprJieShu.spriteName = Convert.ToString(this.m_nDangQianJie + 1);
					}
				}
				GameObject at;
				XingHunStarNodeItem componentInChildren;
				if (this.m_nMaxNode < num2)
				{
					at = this.StarNodeCollection.GetAt(this.m_nMaxNode - 1);
					componentInChildren = at.GetComponentInChildren<XingHunStarNodeItem>();
					if (null != this.m_CurrentActiveStatNode && componentInChildren != this.m_CurrentActiveStatNode)
					{
						this.m_CurrentActiveStatNode = componentInChildren;
						Global.SendEvent("1200", Global.GetLang("激活星魂次数"));
						this.m_bIsNodeActive = true;
						componentInChildren.m_ObjTeXiaoYiDong.transform.position = this.m_BtnJiHuo.transform.position;
					}
					this.m_nDangQianJie = (num2 + 1) / 12;
					this.ReSetControlState(true, false);
					this.m_bIsReFlush = false;
					return;
				}
				GameObject at2 = this.ItemCollection.GetAt(this.m_nSelectIndex);
				XingHunItem componentInChildren2 = at2.GetComponentInChildren<XingHunItem>();
				if (this.m_nMaxNode - 1 == num2)
				{
					this.ReSetControlState(true, false);
				}
				int num3 = (num2 + 1) % 12;
				at = this.StarNodeCollection.GetAt(num2);
				componentInChildren = at.GetComponentInChildren<XingHunStarNodeItem>();
				if (null != this.m_CurrentActiveStatNode && componentInChildren != this.m_CurrentActiveStatNode)
				{
					this.m_CurrentActiveStatNode = componentInChildren;
					Global.SendEvent("1200", Global.GetLang("激活星魂次数"));
					this.m_bIsNodeActive = true;
					componentInChildren.m_ObjTeXiaoYiDong.transform.position = this.m_BtnJiHuo.transform.position;
				}
				if (null == this.m_CurrentActiveStatNode && num2 == 0 && !componentInChildren.m_bActive)
				{
					this.m_CurrentActiveStatNode = componentInChildren;
					Global.SendEvent("1200", Global.GetLang("激活星魂次数"));
					this.m_bIsNodeActive = true;
					componentInChildren.m_ObjTeXiaoYiDong.transform.position = this.m_BtnJiHuo.transform.position;
				}
				componentInChildren.m_bActive = true;
				int num4 = -1;
				StarNode starNode;
				if (this.m_DicActiveStar.ContainsKey(this.m_nSelectIndex + 1))
				{
					num4 = this.m_DicActiveStar[this.m_nSelectIndex + 1];
					num4--;
					starNode = this.m_DicXingZuo[this.m_strCurrentXingZuoName].LstNode[num4];
				}
				else
				{
					starNode = this.m_DicXingZuo[this.m_strCurrentXingZuoName].LstNode[0];
				}
				if (this.m_nMaxNode - 1 <= num4)
				{
					starNode = null;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (starNode != null)
				{
					string[] array = starNode.strProperty.Split(new char[]
					{
						','
					});
					this.PlusPlusTotalProperty(array[0], array[1], dictionary);
				}
				this.m_nDangQianJie = (num2 + 1) / 12;
				for (int i = 0; i < this.LblChildItemCollection.Count; i++)
				{
					at = this.LblChildItemCollection.GetAt(i);
					Label componentInChildren3 = at.GetComponentInChildren<Label>();
					if (null != componentInChildren3 && string.Empty != componentInChildren3.m_strPropertyName && dictionary.ContainsKey(componentInChildren3.m_strPropertyName))
					{
						string text = dictionary[componentInChildren3.m_strPropertyName];
						string[] array2 = text.Split(new char[]
						{
							'-'
						});
						if (Enumerable.Count<string>(array2) >= 2)
						{
							text = array2[1];
						}
						componentInChildren3.m_LblNextNodeProperty.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
						{
							"00ff00",
							text
						}));
						this.ReSetControlState(true, false);
					}
				}
				this.m_CurrentActiveStatNode = componentInChildren;
			}
		}
		if (null != this.m_LblWindowTitle)
		{
			this.m_LblWindowTitle.text = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
			{
				"e9bb6f",
				Global.GetLang("星魂")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"e9bb6f",
				": "
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"f5e2ba",
				Convert.ToString(Global.Data.roleData.StarSoulValue)
			}));
		}
		this.m_bIsReFlush = false;
	}

	public void SetStarNodeGray()
	{
		if (this.m_DicActiveStar.ContainsKey(this.m_nSelectIndex + 1) && this.m_nMaxNode == this.m_DicActiveStar[this.m_nSelectIndex + 1])
		{
			return;
		}
		for (int i = 0; i < this.StarNodeCollection.Count; i++)
		{
			GameObject at = this.StarNodeCollection.GetAt(i);
			XingHunStarNodeItem componentInChildren = at.GetComponentInChildren<XingHunStarNodeItem>();
			componentInChildren.m_bActive = false;
			componentInChildren.m_SprFor.gameObject.SetActive(false);
			componentInChildren.m_ObjTeXiaoXuanZhuan.gameObject.SetActive(false);
		}
	}

	private void InitStarNode(int nIndex)
	{
		XingHunPart.Position position = new XingHunPart.Position();
		Vector3[] array = null;
		switch (nIndex)
		{
		case 0:
			array = position.BaiYangPostion;
			break;
		case 1:
			array = position.JinNiuPostion;
			break;
		case 2:
			array = position.ShuangZiPostion;
			break;
		case 3:
			array = position.JvXiePostion;
			break;
		case 4:
			array = position.ShiZiPostion;
			break;
		case 5:
			array = position.ChuNvPostion;
			break;
		case 6:
			array = position.TianPingPostion;
			break;
		case 7:
			array = position.TianXiePostion;
			break;
		case 8:
			array = position.SheShouPostion;
			break;
		case 9:
			array = position.MoJiePostion;
			break;
		case 10:
			array = position.ShuiPingPostion;
			break;
		case 11:
			array = position.ShuangYuPostion;
			break;
		}
		for (int i = 0; i < this.m_nMaxNode; i++)
		{
			XingHunStarNodeItem xingHunStarNodeItem = U3DUtils.NEW<XingHunStarNodeItem>();
			int num = this.m_nSelectIndex + 1;
			int num3;
			if (this.m_DicActiveStar.ContainsKey(num))
			{
				int num2 = this.m_DicActiveStar[num];
				num3 = i % 12;
				if (num2 % 12 == 0 && num2 != this.m_nMaxNode)
				{
					this.SetStarNodeGray();
				}
				if (i > num2)
				{
					xingHunStarNodeItem.m_bActive = false;
					xingHunStarNodeItem.m_SprFor.gameObject.SetActive(false);
					xingHunStarNodeItem.m_ObjTeXiaoXuanZhuan.gameObject.SetActive(false);
				}
				int num4 = i / 12;
				int num5 = (num2 - 1) / 12;
				int num6 = (num2 - 1) % 12;
				int num7 = i % 12;
				if (num4 == 0)
				{
					num3 = i;
				}
				if (num4 == num5 && num6 >= num7 && 11 >= num6)
				{
					xingHunStarNodeItem.m_bActive = true;
					xingHunStarNodeItem.m_SprFor.gameObject.SetActive(true);
					if (this.m_nMaxNode == num2)
					{
						xingHunStarNodeItem.m_ObjMove.gameObject.SetActive(true);
					}
					xingHunStarNodeItem.m_ObjTeXiaoXuanZhuan.gameObject.SetActive(true);
				}
			}
			else
			{
				num3 = i % 12;
				xingHunStarNodeItem.m_bActive = false;
				xingHunStarNodeItem.m_SprFor.gameObject.SetActive(false);
				xingHunStarNodeItem.m_ObjTeXiaoXuanZhuan.gameObject.SetActive(false);
			}
			xingHunStarNodeItem.m_SprBak.gameObject.transform.localPosition = array[num3];
			xingHunStarNodeItem.m_SprBak.alpha = 0f;
			xingHunStarNodeItem.m_SprFor.gameObject.transform.localPosition = array[num3];
			xingHunStarNodeItem.m_SprFor.alpha = 0f;
			xingHunStarNodeItem.m_AniJiHuo.gameObject.transform.localPosition = new Vector3(xingHunStarNodeItem.m_SprFor.gameObject.transform.localPosition.x, xingHunStarNodeItem.m_SprFor.gameObject.transform.localPosition.y, -0.1f);
			xingHunStarNodeItem.m_ObjMove.gameObject.transform.localPosition = array[num3];
			this.StarNodeCollection.AddNoUpdate(xingHunStarNodeItem);
		}
		this.StarNodeCollection.DelayUpdate();
	}

	private void LstXingZuo_SelectionChanged(object sender, object e)
	{
		if (0 > this.m_LstXingZuo.SelectedIndex)
		{
			return;
		}
		this.m_nSelectIndex = this.m_LstXingZuo.SelectedIndex;
		GameObject at = this.ItemCollection.GetAt(this.m_LstXingZuo.SelectedIndex);
		if (null == at)
		{
			return;
		}
		XingHunItem componentInChildren = at.GetComponentInChildren<XingHunItem>();
		if (componentInChildren.m_bIsGray)
		{
			string[] array = componentInChildren.m_strLimit.Split(new char[]
			{
				','
			});
			string str = string.Format(Global.GetLang("等级需达到{0}转{1}级才可开启该星座"), array[0], array[1]);
			this.ShowText(str);
			return;
		}
		this.InitStarNode(this.m_LstXingZuo.SelectedIndex);
		this.ReSetControlState(true, true);
	}

	public void ReSetControlState(bool bSet, bool bBrush = true)
	{
		if (bSet)
		{
			if (bBrush)
			{
				this.m_ClickXingHunItem = U3DUtils.AS<XingHunItem>(this.m_LstXingZuo.SelectedItem);
				this.m_bIsClick = true;
				this.m_TexLineBak.gameObject.SetActive(true);
				BoxCollider boxCollider = null;
				if (null != this.m_ClickXingHunItem)
				{
					boxCollider = this.m_ClickXingHunItem.m_TexBtnBak.gameObject.GetComponentInChildren<BoxCollider>();
				}
				if (null != boxCollider)
				{
					boxCollider.enabled = true;
				}
				this.m_TexLineBak.alpha = 0f;
				this.m_ObjMainPage.gameObject.SetActive(false);
				this.m_ObjChildPage.gameObject.SetActive(true);
			}
			if (null == this.m_ClickXingHunItem && bBrush)
			{
				return;
			}
			string text = string.Empty;
			if (bBrush)
			{
				text = this.m_ClickXingHunItem.strXingZuoName;
				this.m_strCurrentXingZuoName = text;
			}
			else
			{
				text = this.m_strCurrentXingZuoName;
			}
			if (null != this.m_LblChildPageTopTile)
			{
				this.m_LblChildPageTopTile.text = string.Format(Global.GetLang("{0}永久属性加成"), this.m_strCurrentXingZuoName);
			}
			this.LblChildItemCollection.Clear();
			this.m_DicChildProperty.Clear();
			this.PlusPlusTotalProperty("Attack", "0 - 0", this.m_DicChildProperty);
			this.PlusPlusTotalProperty("Mattack", "0 - 0", this.m_DicChildProperty);
			this.PlusPlusTotalProperty("Defense", "0 - 0", this.m_DicChildProperty);
			this.PlusPlusTotalProperty("Mdefense", "0 - 0", this.m_DicChildProperty);
			this.PlusPlusTotalProperty("HitV", "0", this.m_DicChildProperty);
			this.PlusPlusTotalProperty("Dodge", "0", this.m_DicChildProperty);
			this.PlusPlusTotalProperty("MaxLifeV", "0", this.m_DicChildProperty);
			int num = -1;
			if (this.m_DicActiveStar.ContainsKey(this.m_nSelectIndex + 1))
			{
				num = this.m_DicActiveStar[this.m_nSelectIndex + 1];
			}
			num--;
			for (int i = 0; i < this.m_DicXingZuo[text].LstNode.Count; i++)
			{
				int nXingWeiID = this.m_DicXingZuo[text].LstNode[i].nXingWeiID;
				int nXingHun = this.m_DicXingZuo[text].LstNode[i].nXingHun;
				string strLevelLimit = this.m_DicXingZuo[text].LstNode[i].strLevelLimit;
				string strProperty = this.m_DicXingZuo[text].LstNode[i].strProperty;
				string strSucceed = this.m_DicXingZuo[text].LstNode[i].strSucceed;
				if (num >= i)
				{
					string[] array = strProperty.Split(new char[]
					{
						'|'
					});
					foreach (string text2 in array)
					{
						string[] array3 = text2.Split(new char[]
						{
							','
						});
						this.PlusPlusTotalProperty(array3[0], array3[1], this.m_DicChildProperty);
					}
				}
			}
			this.m_TexBtnURL.URL = this.GetImageString(1, this.m_nSelectIndex + 1);
			if (this.m_nMaxNode - 1 > num)
			{
				this.SetImageGray(this.m_TexBtn, true);
				this.m_LblShowText.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
				{
					"6f6f6f",
					Global.GetLang("激活后额外加成")
				}));
			}
			else
			{
				this.SetImageGray(this.m_TexBtn, false);
				this.m_LblShowText.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
				{
					"3681f3",
					Global.GetLang("激活后额外加成")
				}));
			}
			int num2 = 0;
			num++;
			if (0 < num / 12)
			{
				num2 = num / 12;
				this.FixedJieShu(ref num2);
				string strShuXingJiaCheng = this.m_DicXingZuo[text].strShuXingJiaCheng;
				string[] array4 = this.m_DicXingZuo[text].strJiaChengBiLi.Split(new char[]
				{
					'|'
				});
				string text3 = array4[num2 - 1];
				array4 = text3.Split(new char[]
				{
					','
				});
				string[] array5 = strShuXingJiaCheng.Split(new char[]
				{
					'|'
				});
				foreach (string text4 in array5)
				{
					string[] array7 = text4.Split(new char[]
					{
						','
					});
					string[] array8 = array7[1].Split(new char[]
					{
						'-'
					});
					if (1 < Enumerable.Count<string>(array8))
					{
						this.PlusPlusTotalProperty(array7[0], Convert.ToString(Convert.ToInt32(array8[0]) * Convert.ToInt32(array4[1])) + "-" + Convert.ToString(Convert.ToInt32(array8[1]) * Convert.ToInt32(array4[1])), this.m_DicChildProperty);
					}
					else
					{
						this.PlusPlusTotalProperty(array7[0], Convert.ToString(Convert.ToInt32(array7[1]) * Convert.ToInt32(array4[1])), this.m_DicChildProperty);
					}
				}
			}
			string text5 = this.IntToHanZi(num2);
			if (null != this.m_LblXingHunJieShu)
			{
				this.m_LblXingHunJieShu.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					text5
				});
			}
			if (null != this.m_SprJieShu)
			{
				this.m_SprJieShu.spriteName = Convert.ToString(num2 + 1);
			}
			int num3 = num + 1;
			if (0 <= num)
			{
				if (this.m_nMaxNode <= num3)
				{
					num3 = this.m_nMaxNode - 1;
				}
			}
			int num4 = num;
			if (0 > num)
			{
				num4 = 0;
			}
			else if (this.m_nMaxNode <= num4)
			{
				num4 = this.m_nMaxNode - 1;
			}
			float num5 = Convert.ToSingle(this.m_DicXingZuo[text].LstNode[num4].strSucceed);
			num5 *= 100f;
			int num6 = Convert.ToInt32(num5);
			string text6 = string.Format("{0}%", num6);
			this.m_LblBottomJiLv.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e9bb6f",
				Global.GetLang("成功几率: ")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"f5e2ba",
				text6
			}));
			string strLevelLimit2 = this.m_DicXingZuo[text].LstNode[num4].strLevelLimit;
			string[] array9 = strLevelLimit2.Split(new char[]
			{
				','
			});
			string text7 = (!this.JudgeCondition(0)) ? "ff0000" : "f5e2ba";
			this.m_LblBottomLevel.text = string.Format("{0}{1}{2}{3}{4}", new object[]
			{
				Global.GetColorStringForNGUIText(new object[]
				{
					"e9bb6f",
					Global.GetLang("人物等级: ")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					text7,
					array9[0]
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"f5e2ba",
					Global.GetLang("转")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					text7,
					array9[1]
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"f5e2ba",
					Global.GetLang("级")
				})
			});
			text7 = ((!this.JudgeCondition(2)) ? "ff0000" : "f5e2ba");
			this.m_LblBottomXingHun.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e9bb6f",
				Global.GetLang("星       魂: ")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				text7,
				Convert.ToString(this.m_DicXingZuo[text].LstNode[num4].nXingHun)
			}));
			text7 = ((!this.JudgeCondition(1)) ? "ff0000" : "f5e2ba");
			this.m_LblBottomJinBi.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e9bb6f",
				Global.GetLang("金       币: ")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				text7,
				Convert.ToString(this.m_DicXingZuo[text].LstNode[num4].strNeedGoods)
			}));
			StarNode starNode;
			if (this.m_DicActiveStar.ContainsKey(this.m_nSelectIndex + 1))
			{
				int num7 = this.m_DicActiveStar[this.m_nSelectIndex + 1];
				num7--;
				if (this.m_nMaxNode - 1 == num7)
				{
					starNode = null;
				}
				else
				{
					starNode = this.m_DicXingZuo[this.m_strCurrentXingZuoName].LstNode[num7 + 1];
				}
			}
			else
			{
				starNode = this.m_DicXingZuo[this.m_strCurrentXingZuoName].LstNode[0];
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (starNode != null)
			{
				string[] array10 = starNode.strProperty.Split(new char[]
				{
					'|'
				});
				if (2 <= Enumerable.Count<string>(array10))
				{
					foreach (string text8 in array10)
					{
						string[] array12 = text8.Split(new char[]
						{
							','
						});
						this.PlusPlusTotalProperty(array12[0], array12[1], dictionary);
					}
				}
				else
				{
					array10 = starNode.strProperty.Split(new char[]
					{
						','
					});
					this.PlusPlusTotalProperty(array10[0], array10[1], dictionary);
				}
			}
			foreach (KeyValuePair<string, string> keyValuePair in this.m_DicChildProperty)
			{
				Label label = U3DUtils.NEW<Label>();
				if (dictionary.ContainsKey(keyValuePair.Key))
				{
					string text9 = dictionary[keyValuePair.Key];
					string[] array13 = text9.Split(new char[]
					{
						'-'
					});
					if (Enumerable.Count<string>(array13) >= 2)
					{
						text9 = array13[1];
					}
					label.m_LblNextNodeProperty.gameObject.SetActive(true);
					label.m_SprUp.gameObject.SetActive(true);
					label.m_LblNextNodeProperty.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
					{
						"00ff00",
						text9
					}));
				}
				label.m_strPropertyName = keyValuePair.Key;
				label.m_strPropertyValue = keyValuePair.Value;
				label.m_Label.text = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
				{
					"e9bb6f",
					keyValuePair.Key
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"e9bb6f",
					": "
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"f5e2ba",
					keyValuePair.Value
				}));
				this.LblChildItemCollection.AddNoUpdate(label);
			}
			this.LblChildItemCollection.DelayUpdate();
			SystemHelpMgr.OnAction(UIObjIDs.XingHunPart_BaiYang, HelpStateEvents.Actived, -1);
		}
		else
		{
			int num8 = 0;
			this.ItemCollection.Clear();
			foreach (KeyValuePair<string, XingZuo> keyValuePair2 in this.m_DicXingZuo)
			{
				XingHunItem xingHunItem = U3DUtils.NEW<XingHunItem>();
				xingHunItem.m_nIndex = keyValuePair2.Value.nXingZuoID;
				xingHunItem.m_TexBak.URL = this.GetImageString(0, keyValuePair2.Value.nXingZuoID);
				xingHunItem.m_TexBtnBak.URL = this.GetImageString(1, keyValuePair2.Value.nXingZuoID);
				xingHunItem.strXingZuoName = keyValuePair2.Key;
				xingHunItem.m_strLimit = keyValuePair2.Value.strKaiQiLevel;
				string[] array14 = keyValuePair2.Value.strKaiQiLevel.Split(new char[]
				{
					','
				});
				int nZhuanSheng = Convert.ToInt32(array14[0]);
				int nLevel = Convert.ToInt32(array14[1]);
				this.SetXingZuoSate(xingHunItem, nZhuanSheng, nLevel);
				xingHunItem.transform.name = string.Format(Global.GetLang("迭代加 {0}"), num8++);
				this.ItemCollection.AddNoUpdate(xingHunItem);
			}
			this.ItemCollection.DelayUpdate();
			UIDraggablePanel componentInChildren = this.m_PosPnl.GetComponentInChildren<UIDraggablePanel>();
			componentInChildren.enabled = true;
			this.m_TexLineBak.alpha = 0f;
			this.m_TexLineBak.gameObject.SetActive(false);
			this.m_ObjMainPage.gameObject.SetActive(true);
			this.m_ObjChildPage.gameObject.SetActive(false);
			this.ChangeXingZuoPosition();
		}
	}

	private void SetXingZuoSate(XingHunItem item, int nZhuanSheng, int nLevel)
	{
		if (Global.Data.roleData.ChangeLifeCount <= nZhuanSheng)
		{
			if (Global.Data.roleData.ChangeLifeCount == nZhuanSheng)
			{
				if (Global.Data.roleData.Level < nLevel)
				{
					this.SetImageGray(item.m_UITextBak, true);
					this.SetImageGray(item.m_UITexBtnBak, true);
					item.m_bIsGray = true;
				}
			}
			else
			{
				this.SetImageGray(item.m_UITextBak, true);
				this.SetImageGray(item.m_UITexBtnBak, true);
				item.m_bIsGray = true;
			}
		}
	}

	private string IntToHanZi(int nNum)
	{
		switch (nNum)
		{
		case 0:
			return Global.GetLang("·一阶");
		case 1:
			return Global.GetLang("·二阶");
		case 2:
			return Global.GetLang("·三阶");
		case 3:
			return Global.GetLang("·四阶");
		case 4:
			return Global.GetLang("·五阶");
		default:
			return string.Empty;
		}
	}

	private string GetXingZuoPropertyStr(int nJieNum)
	{
		this.FixedJieShu(ref nJieNum);
		string text = string.Empty;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		this.PlusPlusTotalProperty("Attack", "0 - 0", dictionary);
		this.PlusPlusTotalProperty("Mattack", "0 - 0", dictionary);
		this.PlusPlusTotalProperty("Defense", "0 - 0", dictionary);
		this.PlusPlusTotalProperty("Mdefense", "0 - 0", dictionary);
		this.PlusPlusTotalProperty("HitV", "0", dictionary);
		this.PlusPlusTotalProperty("Dodge", "0", dictionary);
		this.PlusPlusTotalProperty("MaxLifeV", "0", dictionary);
		string[] array = this.m_DicXingZuo[this.m_strCurrentXingZuoName].strJiaChengBiLi.Split(new char[]
		{
			'|'
		});
		string[] array2 = array[nJieNum].Split(new char[]
		{
			','
		});
		int num = Convert.ToInt32(array2[1]);
		foreach (KeyValuePair<string, XingZuo> keyValuePair in this.m_DicXingZuo)
		{
			if (keyValuePair.Key == this.m_strCurrentXingZuoName)
			{
				string[] array3 = keyValuePair.Value.strShuXingJiaCheng.Split(new char[]
				{
					'|'
				});
				foreach (string text2 in array3)
				{
					string[] array5 = text2.Split(new char[]
					{
						','
					});
					string[] array6 = array5[1].Split(new char[]
					{
						'-'
					});
					if (1 < Enumerable.Count<string>(array6))
					{
						string strValue = Convert.ToString(Convert.ToInt32(array6[0]) * num) + "-" + Convert.ToString(Convert.ToInt32(array6[1]) * num);
						this.PlusPlusTotalProperty(array5[0], strValue, dictionary);
					}
					else
					{
						this.PlusPlusTotalProperty(array5[0], Convert.ToString(Convert.ToInt32(array5[1]) * num), dictionary);
					}
				}
			}
		}
		foreach (KeyValuePair<string, string> keyValuePair2 in dictionary)
		{
			string text3 = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
			{
				"e9bb6f",
				keyValuePair2.Key
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"e9bb6f",
				": "
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"f5e2ba",
				keyValuePair2.Value
			}));
			text += text3;
			text += "\n";
		}
		return text;
	}

	private void XingZuoItemClick()
	{
		string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
		{
			"6f6f6f",
			Global.GetLang("【未激活】")
		});
		int num = 0;
		int num2 = -1;
		if (this.m_DicActiveStar.ContainsKey(this.m_nSelectIndex + 1))
		{
			num2 = this.m_DicActiveStar[this.m_nSelectIndex + 1];
			int num3 = num2 % 12;
			num = num2 / 12;
			this.FixedJieShu(ref num);
			if (this.m_nMaxNode <= num2)
			{
			}
		}
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		if (0 < num)
		{
			if (this.m_nMaxNode <= num2)
			{
				string text4 = this.IntToHanZi(num);
				colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
				{
					"00ff00",
					Global.GetLang("【已激活】\n\n")
				});
				text3 = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"ffffff",
					this.m_strCurrentXingZuoName + text4
				}), colorStringForNGUIText);
				text3 += string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
				{
					"3681f3",
					Global.GetLang("激活所有星位可获得额外属性\n")
				}));
				text += text3;
				text += this.GetXingZuoPropertyStr(4);
			}
			else
			{
				int num4 = num - 1;
				string text5 = this.IntToHanZi(num4);
				colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
				{
					"00ff00",
					Global.GetLang("【已激活】\n\n")
				});
				text3 = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"ffffff",
					this.m_strCurrentXingZuoName + text5
				}), colorStringForNGUIText);
				text3 += string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
				{
					"3681f3",
					Global.GetLang("激活所有星位可获得额外属性\n")
				}));
				text += text3;
				text += this.GetXingZuoPropertyStr(num4);
				text5 = this.IntToHanZi(num);
				colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
				{
					"6f6f6f",
					Global.GetLang("【未激活】\n\n")
				});
				text3 = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"6f6f6f",
					this.m_strCurrentXingZuoName + text5
				}), colorStringForNGUIText);
				text3 += string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
				{
					"3681f3",
					Global.GetLang("激活所有星位可获得额外属性\n")
				}));
				text2 += text3;
				text2 += this.GetXingZuoPropertyStr(num);
			}
		}
		else if (num == 0)
		{
			string text6 = this.IntToHanZi(num);
			colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				"6f6f6f",
				Global.GetLang("【未激活】\n\n")
			});
			text3 = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"6f6f6f",
				this.m_strCurrentXingZuoName + text6
			}), colorStringForNGUIText);
			text3 += string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
			{
				"3681f3",
				Global.GetLang("激活所有星位可获得额外属性\n")
			}));
			text += text3;
			text += this.GetXingZuoPropertyStr(num);
		}
		MUDebug.Log<string>(new string[]
		{
			text
		});
		MUDebug.Log<string>(new string[]
		{
			text2
		});
		PlayZone.GlobalPlayZone.OpenXingHunTips(text, text2);
	}

	protected IEnumerator InitXmlProc()
	{
		this.InitXml();
		yield return null;
		yield break;
	}

	private void FixedJieShu(ref int nJieShu)
	{
		if (this.m_nMaxJieShu <= nJieShu)
		{
			nJieShu = this.m_nMaxJieShu - 1;
		}
	}

	private void InitXml()
	{
		XElement gameResXml = Global.GetGameResXml("Config/XingZuo/XingZuoType.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "XingZuo"), "*");
		this.PlusPlusTotalProperty("Attack", "0 - 0", this.m_DicTotalProperty);
		this.PlusPlusTotalProperty("Mattack", "0 - 0", this.m_DicTotalProperty);
		this.PlusPlusTotalProperty("Defense", "0 - 0", this.m_DicTotalProperty);
		this.PlusPlusTotalProperty("Mdefense", "0 - 0", this.m_DicTotalProperty);
		this.PlusPlusTotalProperty("HitV", "0", this.m_DicTotalProperty);
		this.PlusPlusTotalProperty("Dodge", "0", this.m_DicTotalProperty);
		this.PlusPlusTotalProperty("MaxLifeV", "0", this.m_DicTotalProperty);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ShuXiangJiaCheng");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "ID");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "Name");
			string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "KaiQiLevel");
			string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement, "JiaChengBiLie");
			XingHunItem xingHunItem = U3DUtils.NEW<XingHunItem>();
			xingHunItem.m_nIndex = Convert.ToInt32(xelementAttributeStr2);
			xingHunItem.m_TexBak.URL = this.GetImageString(0, Convert.ToInt32(xelementAttributeStr2));
			xingHunItem.m_TexBtnBak.URL = this.GetImageString(1, Convert.ToInt32(xelementAttributeStr2));
			xingHunItem.m_TexBtnGrayBak.URL = this.GetImageString(1, Convert.ToInt32(xelementAttributeStr2));
			xingHunItem.m_TexBtnGrayBak.gameObject.SetActive(false);
			xingHunItem.strXingZuoName = xelementAttributeStr3;
			xingHunItem.m_strLimit = xelementAttributeStr4;
			xingHunItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.RetunrMainUI();
			};
			string[] array = xelementAttributeStr4.Split(new char[]
			{
				','
			});
			int nZhuanSheng = Convert.ToInt32(array[0]);
			int nLevel = Convert.ToInt32(array[1]);
			this.SetXingZuoSate(xingHunItem, nZhuanSheng, nLevel);
			this.ItemCollection.AddNoUpdate(xingHunItem);
			XingZuo xingZuo = new XingZuo();
			xingZuo.nXingZuoID = Convert.ToInt32(xelementAttributeStr2);
			xingZuo.strXingZuoName = xelementAttributeStr3;
			xingZuo.strKaiQiLevel = xelementAttributeStr4;
			xingZuo.strShuXingJiaCheng = xelementAttributeStr;
			xingZuo.strJiaChengBiLi = xelementAttributeStr5;
			this.m_DicXingZuo.Add(xelementAttributeStr3, xingZuo);
			if (this.m_DicActiveStar.ContainsKey(i + 1))
			{
				int num = this.m_DicActiveStar[i + 1];
				if (0 < num / 12)
				{
					int num2 = num / 12;
					this.FixedJieShu(ref num2);
					string[] array2 = xelementAttributeStr5.Split(new char[]
					{
						'|'
					});
					string[] array3 = array2[num2 - 1].Split(new char[]
					{
						','
					});
					int num3 = Convert.ToInt32(array3[1]);
					string[] array4 = xelementAttributeStr.Split(new char[]
					{
						'|'
					});
					foreach (string text in array4)
					{
						string[] array6 = text.Split(new char[]
						{
							','
						});
						string[] array7 = array6[1].Split(new char[]
						{
							'-'
						});
						if (1 < Enumerable.Count<string>(array7))
						{
							string strValue = Convert.ToString(Convert.ToInt32(array7[0]) * num3) + "-" + Convert.ToString(Convert.ToInt32(array7[1]) * num3);
							this.PlusPlusTotalProperty(array6[0], strValue, this.m_DicTotalProperty);
						}
						else
						{
							this.PlusPlusTotalProperty(array6[0], Convert.ToString(Convert.ToInt32(array6[1]) * num3), this.m_DicTotalProperty);
						}
					}
				}
			}
		}
		this.ItemCollection.DelayUpdate();
		int num4 = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		string xmlName = string.Format("Config/XingZuo/XingZuo_{0}.xml", num4);
		gameResXml = Global.GetGameResXml(xmlName);
		if (gameResXml == null)
		{
			return;
		}
		xelementList = Global.GetXElementList(gameResXml, "*");
		for (int k = 0; k < xelementList.Count; k++)
		{
			XElement xelement2 = xelementList[k];
			string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement2, "Title");
			List<XElement> xelementList2 = Global.GetXElementList(xelement2, "*");
			int num5 = -1;
			if (this.m_DicActiveStar.ContainsKey(k + 1))
			{
				num5 = this.m_DicActiveStar[k + 1];
			}
			for (int l = 0; l < xelementList2.Count; l++)
			{
				XElement xelement3 = xelementList2[l];
				string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement3, "ID");
				string xelementAttributeStr8 = Global.GetXElementAttributeStr(xelement3, "NeedJinBi");
				string xelementAttributeStr9 = Global.GetXElementAttributeStr(xelement3, "ShuXing");
				string xelementAttributeStr10 = Global.GetXElementAttributeStr(xelement3, "XingHun");
				string xelementAttributeStr11 = Global.GetXElementAttributeStr(xelement3, "Succeed");
				string xelementAttributeStr12 = Global.GetXElementAttributeStr(xelement3, "LevelLimit");
				StarNode starNode = new StarNode();
				starNode.nXingWeiID = Convert.ToInt32(xelementAttributeStr7);
				starNode.strNeedGoods = xelementAttributeStr8;
				starNode.nXingHun = Convert.ToInt32(xelementAttributeStr10);
				starNode.strLevelLimit = xelementAttributeStr12;
				starNode.strProperty = xelementAttributeStr9;
				starNode.strSucceed = xelementAttributeStr11;
				string[] array8 = xelementAttributeStr9.Split(new char[]
				{
					'|'
				});
				this.m_DicXingZuo[xelementAttributeStr6].LstNode.Add(starNode);
				if (num5 > l)
				{
					foreach (string text2 in array8)
					{
						string[] array10 = text2.Split(new char[]
						{
							','
						});
						this.PlusPlusTotalProperty(array10[0], array10[1], this.m_DicTotalProperty);
					}
				}
			}
		}
		foreach (KeyValuePair<string, string> keyValuePair in this.m_DicTotalProperty)
		{
			Label label = U3DUtils.NEW<Label>();
			label.m_strPropertyName = keyValuePair.Key;
			label.m_strPropertyValue = keyValuePair.Value;
			label.m_Label.text = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
			{
				"e9bb6f",
				keyValuePair.Key
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"e9bb6f",
				": "
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"f5e2ba",
				keyValuePair.Value
			}));
			label.m_Label.gameObject.transform.localScale = new Vector3(16f, 16f, 1f);
			this.LblItemCollection.AddNoUpdate(label);
		}
		this.LblItemCollection.DelayUpdate();
		this.ChangeXingZuoPosition();
	}

	private void ChangeXingZuoPosition()
	{
		XingHunPart.Position position = new XingHunPart.Position();
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			GameObject at = this.ItemCollection.GetAt(i);
			XingHunItem componentInChildren = at.GetComponentInChildren<XingHunItem>();
			BoxCollider componentInChildren2 = componentInChildren.GetComponentInChildren<BoxCollider>();
			componentInChildren2.center = position.XingZuoPostion[i];
			componentInChildren.m_UITexBtnBak.gameObject.transform.localPosition = new Vector3(position.XingZuoPostion[i].x, position.XingZuoPostion[i].y, componentInChildren.m_UITexBtnBak.gameObject.transform.localPosition.z);
			componentInChildren.m_UITextBak.gameObject.transform.localPosition = position.XingZuoPostion[i];
			componentInChildren.m_ObgTeXiao.gameObject.transform.localPosition = componentInChildren.m_UITexBtnBak.gameObject.transform.localPosition;
		}
	}

	private void PlusPlusTotalProperty(string strPropertyName, string strValue, Dictionary<string, string> dic)
	{
		string propNameByEnglishName = this.GetPropNameByEnglishName(strPropertyName);
		if (string.Empty == propNameByEnglishName)
		{
			return;
		}
		if (dic.ContainsKey(propNameByEnglishName))
		{
			string text = string.Empty;
			if (Global.GetLang("物理防御") == propNameByEnglishName || Global.GetLang("魔法防御") == propNameByEnglishName || Global.GetLang("物理攻击") == propNameByEnglishName || Global.GetLang("魔法攻击") == propNameByEnglishName)
			{
				text = dic[propNameByEnglishName];
				string[] array = dic[propNameByEnglishName].Split(new char[]
				{
					'-'
				});
				string[] array2 = strValue.Split(new char[]
				{
					'-'
				});
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array2[0]);
				int num3 = Convert.ToInt32(array[1]);
				int num4 = Convert.ToInt32(array2[1]);
				string text2 = string.Format("{0} - {1}", num + num2, num3 + num4);
				dic[propNameByEnglishName] = text2;
			}
			else
			{
				text = dic[propNameByEnglishName];
				int num5 = Convert.ToInt32(text);
				num5 += Convert.ToInt32(strValue);
				dic[propNameByEnglishName] = Convert.ToString(num5);
			}
		}
		else
		{
			dic.Add(propNameByEnglishName, strValue);
		}
	}

	private string GetPropNameByEnglishName(string strEnglishName)
	{
		if ("Defense" == strEnglishName)
		{
			return Global.GetLang("物理防御");
		}
		if ("Mdefense" == strEnglishName)
		{
			return Global.GetLang("魔法防御");
		}
		if ("Attack" == strEnglishName)
		{
			return Global.GetLang("物理攻击");
		}
		if ("Mattack" == strEnglishName)
		{
			return Global.GetLang("魔法攻击");
		}
		if ("HitV" == strEnglishName)
		{
			return Global.GetLang("命       中");
		}
		if ("Dodge" == strEnglishName)
		{
			return Global.GetLang("闪       避");
		}
		if ("MaxLifeV" == strEnglishName)
		{
			return Global.GetLang("生命上限");
		}
		return string.Empty;
	}

	private void SetCollider(XingHunItem item, bool bEnabled)
	{
		if (null == item)
		{
			return;
		}
		BoxCollider componentInChildren = item.GetComponentInChildren<BoxCollider>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = bEnabled;
		}
	}

	private void SetImageGray(UITexture texture, bool bGray = true)
	{
		if (null != texture)
		{
			if (bGray)
			{
				texture.shader = Shader.Find("Unlit/Gray");
			}
			else
			{
				texture.shader = Shader.Find("Unlit/Transparent Colored");
			}
		}
	}

	private new void Update()
	{
		if (1f > this.m_TexLineBak.alpha && this.m_bIsClick)
		{
			this.m_TexLineBak.alpha += 0.02f;
		}
		if (1f <= this.m_TexLineBak.alpha)
		{
			this.m_bIsClick = false;
		}
		if (null != this.m_ClickXingHunItem)
		{
			this.SetItemPostion();
		}
		if (this.m_bIsNodeActive && null != this.m_CurrentActiveStatNode)
		{
			this.SetNodePostion(this.m_CurrentActiveStatNode);
		}
	}

	private void SetNodePostion(XingHunStarNodeItem nodeitem)
	{
		this.m_bIsReFlush = true;
		this.UsedTime += Time.deltaTime;
		if (Vector3.Distance(nodeitem.m_ObjTeXiaoYiDong.transform.position, nodeitem.m_SprBak.transform.position) <= 1E-05f)
		{
			nodeitem.m_AniJiHuo.gameObject.SetActive(true);
			nodeitem.m_SprFor.gameObject.SetActive(true);
			nodeitem.m_ObjTeXiaoYiDong.gameObject.SetActive(false);
			nodeitem.m_ObjTeXiaoXuanZhuan.gameObject.SetActive(true);
			Object.Destroy(nodeitem.m_ObjTeXiaoYiDong.gameObject);
			nodeitem.m_ObjTeXiaoYiDong = null;
			this.m_bIsNodeActive = false;
			this.m_bIsReFlush = false;
			this.UsedTime = 0f;
		}
		else
		{
			nodeitem.m_ObjTeXiaoYiDong.gameObject.SetActive(true);
			nodeitem.m_ObjTeXiaoYiDong.transform.position = Vector3.Lerp(this.m_BtnJiHuo.transform.position, nodeitem.m_SprBak.transform.position, this.UsedTime / this.TotalTime);
		}
	}

	private void SetItemPostion()
	{
		this.UsedTime += Time.deltaTime;
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			GameObject at = this.ItemCollection.GetAt(i);
			XingHunItem componentInChildren = at.GetComponentInChildren<XingHunItem>();
			BoxCollider componentInChildren2 = componentInChildren.GetComponentInChildren<BoxCollider>();
			componentInChildren2.enabled = false;
			if (i != this.m_LstXingZuo.SelectedIndex)
			{
				if (i < this.m_LstXingZuo.SelectedIndex)
				{
					componentInChildren.m_TexBak.transform.position = Vector3.Lerp(componentInChildren.m_TexBak.transform.position, this.m_HideLeftTarget.transform.position, this.UsedTime / this.TotalTime);
					componentInChildren.m_TexBtnBak.transform.position = Vector3.Lerp(componentInChildren.m_TexBtnBak.transform.position, this.m_HideLeftTarget.transform.position, this.UsedTime / this.TotalTime);
					componentInChildren.m_ObgTeXiao.gameObject.transform.position = Vector3.Lerp(componentInChildren.m_TexBak.transform.position, this.m_HideLeftTarget.transform.position, this.UsedTime / this.TotalTime);
					componentInChildren.m_TexBak.transform.localScale = Vector3.Lerp(componentInChildren.m_TexBak.transform.localScale, this.m_BakScaleTarget.transform.localScale, this.UsedTime / this.TotalTime);
				}
				if (i > this.m_LstXingZuo.SelectedIndex)
				{
					componentInChildren.m_TexBak.transform.position = Vector3.Lerp(componentInChildren.m_TexBak.transform.position, this.m_HideRightTarget.transform.position, this.UsedTime / this.TotalTime);
					componentInChildren.m_TexBtnBak.transform.position = Vector3.Lerp(componentInChildren.m_TexBtnBak.transform.position, this.m_HideRightTarget.transform.position, this.UsedTime / this.TotalTime);
					componentInChildren.m_ObgTeXiao.gameObject.transform.position = Vector3.Lerp(componentInChildren.m_TexBak.transform.position, this.m_HideLeftTarget.transform.position, this.UsedTime / this.TotalTime);
					componentInChildren.m_TexBak.transform.localScale = Vector3.Lerp(componentInChildren.m_TexBak.transform.localScale, this.m_BakScaleTarget.transform.localScale, this.UsedTime / this.TotalTime);
				}
			}
		}
		UIDraggablePanel componentInChildren3 = this.m_PosPnl.GetComponentInChildren<UIDraggablePanel>();
		componentInChildren3.enabled = false;
		this.m_ClickXingHunItem.m_TexBtnBak.transform.position = Vector3.Slerp(this.m_ClickXingHunItem.m_TexBtnBak.transform.position, this.m_BtnTarget.transform.position, this.UsedTime / this.TotalTime1);
		this.m_ClickXingHunItem.m_ObgTeXiao.gameObject.transform.position = Vector3.Slerp(this.m_ClickXingHunItem.m_TexBak.transform.position, this.m_BtnTarget.transform.position, this.UsedTime / this.TotalTime1);
		this.m_ClickXingHunItem.m_TexBtnGrayBak.transform.position = Vector3.Slerp(this.m_ClickXingHunItem.m_TexBtnBak.transform.position, this.m_BtnTarget.transform.position, this.UsedTime / this.TotalTime1);
		this.m_ClickXingHunItem.m_TexBak.transform.position = Vector3.Slerp(this.m_ClickXingHunItem.m_TexBak.transform.position, this.m_BakTarget.transform.position, this.UsedTime / this.TotalTime1);
		this.m_ClickXingHunItem.m_TexBak.transform.localScale = Vector3.Slerp(this.m_ClickXingHunItem.m_TexBak.transform.localScale, this.m_BakScaleTarget.transform.localScale, this.UsedTime / this.TotalTime1);
		if (this.m_TexLineBakURL.URL != this.GetImageString(2, this.m_ClickXingHunItem.m_nIndex))
		{
			this.m_TexLineBakURL.URL = this.GetImageString(2, this.m_ClickXingHunItem.m_nIndex);
		}
		if (this.m_ClickXingHunItem.m_TexBtnBak.transform.position == this.m_BtnTarget.transform.position && this.m_ClickXingHunItem.m_TexBak.transform.position == this.m_BakTarget.transform.position)
		{
			this.m_BtnJiHuo.isEnabled = true;
			this.m_SprJieShu.gameObject.SetActive(true);
			this.m_ClickXingHunItem.m_TexBak.transform.localPosition = new Vector3(this.m_ClickXingHunItem.m_TexBak.transform.localPosition.x, this.m_ClickXingHunItem.m_TexBak.transform.localPosition.y, 0f);
			this.m_ClickXingHunItem.m_nFirstActive = 1;
			XingHunLogoTeXiao xingHunLogoTeXiao = U3DUtils.NEW<XingHunLogoTeXiao>();
			U3DUtils.AddChild(this.m_ClickXingHunItem.m_ObgTeXiao.gameObject, xingHunLogoTeXiao.gameObject, true);
			this.m_ClickXingHunItem.m_TexBtnGrayBak.gameObject.SetActive(true);
			this.m_ClickXingHunItem.m_TexBtnBak.gameObject.transform.localPosition = new Vector3(this.m_ClickXingHunItem.m_TexBtnBak.gameObject.transform.localPosition.x, this.m_ClickXingHunItem.m_TexBtnBak.gameObject.transform.localPosition.y, -0.2f);
			this.m_ClickXingHunItem.m_ObgTeXiao.gameObject.transform.localPosition = this.m_ClickXingHunItem.m_TexBtnBak.gameObject.transform.localPosition;
			int num;
			if (this.m_DicActiveStar.ContainsKey(this.m_LstXingZuo.SelectedIndex + 1))
			{
				num = this.m_DicActiveStar[this.m_LstXingZuo.SelectedIndex + 1];
			}
			else
			{
				num = 0;
			}
			if (this.m_nMaxNode == num)
			{
				this.m_ClickXingHunItem.m_ObgTeXiao.gameObject.SetActive(true);
			}
			this.UsedTime = 0f;
			this.m_ClickXingHunItem = null;
		}
	}

	private void SetXingHunItemProgress(XingHunItem Item, float nValue)
	{
		if (null == Item)
		{
			return;
		}
		float num = Item.m_TexBtnGrayBak.gameObject.transform.localPosition.y - Item.m_TexBtnGrayBak.gameObject.transform.localScale.y / 2f + Item.m_TexBtnGrayBak.gameObject.transform.localScale.y / 12f * nValue / 2f;
		Item.m_TexBtnBak.gameObject.transform.localPosition = new Vector3(Item.m_TexBtnBak.gameObject.transform.localPosition.x, num, -0.2f);
		Item.m_UITexBtnBak.gameObject.transform.localScale = new Vector3(118f, Item.m_TexBtnGrayBak.gameObject.transform.localScale.y / 12f * nValue, 0f);
		Item.m_UITexBtnBak.uvRect = new Rect(0f, 0f, 1f, 0.0833333358f * nValue);
		Item.m_TexBtnBak.gameObject.SetActive(true);
	}

	private string GetImageString(int nResType, int nResIndex)
	{
		return string.Format("NetImages/GameRes/Images/XingZuo/{0}.png", this.GetResName(nResType, nResIndex));
	}

	private string GetResName(int nResType, int nResIndex)
	{
		string text = string.Empty;
		switch (nResIndex)
		{
		case 1:
			text = "bai_yang";
			break;
		case 2:
			text = "jin_niu";
			break;
		case 3:
			text = "shuang_zi";
			break;
		case 4:
			text = "jv_xie";
			break;
		case 5:
			text = "shi_zi";
			break;
		case 6:
			text = "chu_nv";
			break;
		case 7:
			text = "tian_ping";
			break;
		case 8:
			text = "tian_xie";
			break;
		case 9:
			text = "she_shou";
			break;
		case 10:
			text = "mo_jie";
			break;
		case 11:
			text = "shui_ping";
			break;
		case 12:
			text = "shuang_yu";
			break;
		}
		if (nResType == 0)
		{
			text += "_model";
		}
		if (nResType == 1)
		{
			text += "_btn";
		}
		if (nResType == 2)
		{
			text += "_line";
		}
		return text;
	}

	private void ChangeDragPos()
	{
		if (null != this.m_PosPnl)
		{
			Vector3 localPosition = this.m_PosPnl.gameObject.transform.localPosition;
			if (0f > localPosition.x)
			{
				this.m_DragPnl.transform.localPosition = new Vector3(Math.Abs(localPosition.x), 0f, 0f);
			}
			else
			{
				this.m_DragPnl.transform.localPosition = new Vector3(-1f * localPosition.x, 0f, 0f);
			}
		}
	}

	private new void Start()
	{
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 403)
			{
				if (null != this.m_LstXingZuo)
				{
					SystemHelpPart.SetMask(this.m_LstXingZuo.GetItemByIndex(0).GetComponent("XingHunItem"), default(Vector4));
				}
			}
			else if (id == 404)
			{
				SystemHelpPart.SetMask(this.m_BtnJiHuo, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	public ListBox m_LstXingZuo;

	private ObservableCollection ItemCollection;

	public ListBox m_LstLbl;

	private ObservableCollection LblItemCollection;

	public ListBox m_LstChildLbl;

	private ObservableCollection LblChildItemCollection;

	public ListBox m_LstStarNode;

	private ObservableCollection StarNodeCollection;

	public UIPanel m_DragPnl;

	public UIPanel m_PosPnl;

	public Transform m_BtnTarget;

	public Transform m_BakTarget;

	public Transform m_BakScaleTarget;

	public Transform m_HideLeftTarget;

	public Transform m_HideRightTarget;

	public UISprite m_SprNode;

	public UISprite m_SprJieShu;

	public UITexture m_TexLineBak;

	public ShowNetImage m_TexLineBakURL;

	public UITexture m_TexLineScaleBak;

	public ShowNetImage m_TexLineScaleBakURL;

	public UITexture m_TexBtn;

	public ShowNetImage m_TexBtnURL;

	public GButton m_BtnJiHuo;

	public UILabel m_LblBottomJiLv;

	public UILabel m_LblBottomLevel;

	public UILabel m_LblBottomXingHun;

	public UILabel m_LblBottomJinBi;

	public UILabel m_LblWindowTitle;

	public UILabel m_LblWindowTitle2;

	public UILabel m_LblChildPageTopTile;

	public UILabel m_LblShowText;

	public UILabel m_LblXingHunJieShu;

	public UILabel m_LblBottomChildPageTopTile;

	public GameObject m_ObjMainPage;

	public GameObject m_ObjChildPage;

	public GameObject m_ObjJinJieChengGong;

	public int m_nSelectIndex = -1;

	public float smooth = 0.1f;

	private Dictionary<string, XingZuo> m_DicXingZuo = new Dictionary<string, XingZuo>();

	private Dictionary<string, string> m_DicTotalProperty = new Dictionary<string, string>();

	private Dictionary<string, string> m_DicChildProperty = new Dictionary<string, string>();

	private Dictionary<int, int> m_DicActiveStar = new Dictionary<int, int>();

	private bool m_bIsClick;

	private bool m_bIsNodeActive;

	private bool m_bIsReFlush;

	private XingHunItem m_ClickXingHunItem;

	private XingHunStarNodeItem m_CurrentActiveStatNode;

	private string m_strCurrentXingZuoName = string.Empty;

	private int m_nMaxNode = 60;

	private int m_nMaxJieShu = 5;

	private int m_nDangQianJie;

	private float UsedTime;

	private float TotalTime = 0.5f;

	private float TotalTime1 = 0.3f;

	public enum EnumXingZuoIndex
	{
		BaiYang,
		JinNiu,
		ShuangZi,
		JvXie,
		ShiZi,
		ChuNv,
		TianPing,
		TianXie,
		SheShou,
		MoJie,
		ShuiPing,
		ShuangYu
	}

	public class Position
	{
		public Vector3[] BaiYangPostion = new Vector3[]
		{
			new Vector3(145f, 125f, 0f),
			new Vector3(96f, 52f, 0f),
			new Vector3(35f, -2f, 0f),
			new Vector3(-33f, 70f, 0f),
			new Vector3(-145f, 85f, 0f),
			new Vector3(-206f, 57f, 0f),
			new Vector3(-295f, -97f, 0f),
			new Vector3(-205f, -45f, 0f),
			new Vector3(-122f, -20f, 0f),
			new Vector3(-90f, 35f, 0f),
			new Vector3(-30f, -85f, 0f),
			new Vector3(65f, -115f, 0f)
		};

		public Vector3[] JinNiuPostion = new Vector3[]
		{
			new Vector3(-258f, 137f, 0f),
			new Vector3(-168f, 85f, 0f),
			new Vector3(-76f, 107f, 0f),
			new Vector3(26f, 70f, 0f),
			new Vector3(125f, 146f, 0f),
			new Vector3(-9f, -85f, 0f),
			new Vector3(-6f, -5f, 0f),
			new Vector3(-73f, -23f, 0f),
			new Vector3(-73f, -140f, 0f),
			new Vector3(-161f, -140f, 0f),
			new Vector3(-152f, -53f, 0f),
			new Vector3(-283f, 43f, 0f)
		};

		public Vector3[] ShuangZiPostion = new Vector3[]
		{
			new Vector3(90f, 67f, 0f),
			new Vector3(44f, 137f, 0f),
			new Vector3(-55f, 138f, 0f),
			new Vector3(-160f, 146f, 0f),
			new Vector3(-86f, 75f, 0f),
			new Vector3(-40f, 7f, 0f),
			new Vector3(20f, -77f, 0f),
			new Vector3(-65f, -145f, 0f),
			new Vector3(-105f, -65f, 0f),
			new Vector3(-146f, 19f, 0f),
			new Vector3(-246f, 40f, 0f),
			new Vector3(-253f, -45f, 0f)
		};

		public Vector3[] JvXiePostion = new Vector3[]
		{
			new Vector3(0f, 145f, 0f),
			new Vector3(106f, 87f, 0f),
			new Vector3(60f, -4f, 0f),
			new Vector3(60f, -70f, 0f),
			new Vector3(-9f, -135f, 0f),
			new Vector3(-60f, -65f, 0f),
			new Vector3(-80f, 11f, 0f),
			new Vector3(-215f, 114f, 0f),
			new Vector3(-191f, 53f, 0f),
			new Vector3(-185f, -40f, 0f),
			new Vector3(-165f, -122f, 0f),
			new Vector3(-279f, -89f, 0f)
		};

		public Vector3[] ShiZiPostion = new Vector3[]
		{
			new Vector3(-117f, -85f, 0f),
			new Vector3(-2f, -128f, 0f),
			new Vector3(122f, -100f, 0f),
			new Vector3(58f, -48f, 0f),
			new Vector3(104f, 23f, 0f),
			new Vector3(95f, 105f, 0f),
			new Vector3(-18f, 125f, 0f),
			new Vector3(-53f, 66f, 0f),
			new Vector3(-135f, 23f, 0f),
			new Vector3(-233f, 11f, 0f),
			new Vector3(-266f, -82f, 0f),
			new Vector3(-288f, 75f, 0f)
		};

		public Vector3[] ChuNvPostion = new Vector3[]
		{
			new Vector3(126f, 122f, 0f),
			new Vector3(98f, -51f, 0f),
			new Vector3(-16f, 46f, 0f),
			new Vector3(-56f, -41f, 0f),
			new Vector3(-40f, -150f, 0f),
			new Vector3(-141f, -127f, 0f),
			new Vector3(-155f, -30f, 0f),
			new Vector3(-101f, 30f, 0f),
			new Vector3(-60f, 103f, 0f),
			new Vector3(-132f, 145f, 0f),
			new Vector3(-180f, 72f, 0f),
			new Vector3(-277f, -60f, 0f)
		};

		public Vector3[] TianPingPostion = new Vector3[]
		{
			new Vector3(135f, 143f, 0f),
			new Vector3(138f, -35f, 0f),
			new Vector3(44f, -138f, 0f),
			new Vector3(-34f, -114f, 0f),
			new Vector3(-153f, -75f, 0f),
			new Vector3(32f, 25f, 0f),
			new Vector3(-65f, 70f, 0f),
			new Vector3(-55f, 137f, 0f),
			new Vector3(-153f, 140f, 0f),
			new Vector3(-213f, 77f, 0f),
			new Vector3(-213f, -20f, 0f),
			new Vector3(-311f, -77f, 0f)
		};

		public Vector3[] TianXiePostion = new Vector3[]
		{
			new Vector3(138f, 1f, 0f),
			new Vector3(87f, 77f, 0f),
			new Vector3(50f, 140f, 0f),
			new Vector3(30f, 55f, 0f),
			new Vector3(62f, -44f, 0f),
			new Vector3(-40f, 22f, 0f),
			new Vector3(-65f, -44f, 0f),
			new Vector3(-89f, -117f, 0f),
			new Vector3(-175f, -138f, 0f),
			new Vector3(-213f, -59f, 0f),
			new Vector3(-292f, -17f, 0f),
			new Vector3(-262f, 64f, 0f)
		};

		public Vector3[] SheShouPostion = new Vector3[]
		{
			new Vector3(136f, -46f, 0f),
			new Vector3(21f, 14f, 0f),
			new Vector3(45f, 138f, 0f),
			new Vector3(-50f, 150f, 0f),
			new Vector3(-68f, 52f, 0f),
			new Vector3(-31f, -62f, 0f),
			new Vector3(-95f, -141f, 0f),
			new Vector3(-135f, -47f, 0f),
			new Vector3(-220f, -108f, 0f),
			new Vector3(-285f, -60f, 0f),
			new Vector3(-145f, 140f, 0f),
			new Vector3(-257f, 130f, 0f)
		};

		public Vector3[] MoJiePostion = new Vector3[]
		{
			new Vector3(103f, 141f, 0f),
			new Vector3(3f, 47f, 0f),
			new Vector3(-55f, -43f, 0f),
			new Vector3(-3f, -152f, 0f),
			new Vector3(-112f, -89f, 0f),
			new Vector3(-249f, -119f, 0f),
			new Vector3(-191f, -70f, 0f),
			new Vector3(-246f, -37f, 0f),
			new Vector3(-255f, 56f, 0f),
			new Vector3(-173f, 62f, 0f),
			new Vector3(-125f, 132f, 0f),
			new Vector3(-224f, 163f, 0f)
		};

		public Vector3[] ShuiPingPostion = new Vector3[]
		{
			new Vector3(-77f, 61f, 0f),
			new Vector3(-92f, -20f, 0f),
			new Vector3(65f, -71f, 0f),
			new Vector3(-67f, -138f, 0f),
			new Vector3(-153f, -81f, 0f),
			new Vector3(-292f, -14f, 0f),
			new Vector3(-213f, 74f, 0f),
			new Vector3(-162f, 133f, 0f),
			new Vector3(-50f, 122f, 0f),
			new Vector3(8f, 19f, 0f),
			new Vector3(50f, 98f, 0f),
			new Vector3(141f, 143f, 0f)
		};

		public Vector3[] ShuangYuPostion = new Vector3[]
		{
			new Vector3(135f, -55f, 0f),
			new Vector3(25f, -35f, 0f),
			new Vector3(-38f, 103f, 0f),
			new Vector3(-62f, 27f, 0f),
			new Vector3(-62f, -52f, 0f),
			new Vector3(-56f, -150f, 0f),
			new Vector3(-150f, -91f, 0f),
			new Vector3(-223f, -40f, 0f),
			new Vector3(-278f, 11f, 0f),
			new Vector3(-230f, 85f, 0f),
			new Vector3(-105f, 140f, 0f),
			new Vector3(17f, 150f, 0f)
		};

		public Vector3[] XingZuoPostion = new Vector3[]
		{
			new Vector3(0f, 100f, 0f),
			new Vector3(200f, -110f, 0f),
			new Vector3(400f, 90f, 0f),
			new Vector3(600f, -110f, 0f),
			new Vector3(800f, 100f, 0f),
			new Vector3(1000f, -110f, 0f),
			new Vector3(1200f, 100f, 0f),
			new Vector3(1400f, -110f, 0f),
			new Vector3(1600f, 100f, 0f),
			new Vector3(1800f, -110f, 0f),
			new Vector3(2000f, 100f, 0f),
			new Vector3(2200f, -110f, 0f)
		};
	}
}
