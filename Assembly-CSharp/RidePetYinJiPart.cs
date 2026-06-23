using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class RidePetYinJiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitSoltCanUseGoodsID();
		this.RefreshBagPage();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.InitPage();
		this.InitTopBtnSpIcon();
		IConfigbase<ConfigShengYinShengJi>.Instance.GetData(delegate(object e, DPSelectedItemEventArgs s)
		{
			this.InitSolt();
		});
		if (Global.Data != null && Global.Data.roleData.HolyGoodsDataList != null)
		{
			this.RefreshBag(Global.Data.roleData.HolyGoodsDataList, -1);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		IConfigbase<ConfigShengYinShengJi>.Instance.Hander = null;
		if (null != this.mTipTempIcon)
		{
			Object.Destroy(this.mTipTempIcon.gameObject);
		}
	}

	private void InitSoltCanUseGoodsID()
	{
		Dictionary<int, ShengYinShengJiVO>.Enumerator shengYinShengJiEnumerator = IConfigbase<ConfigShengYinShengJi>.Instance.GetShengYinShengJiEnumerator();
		while (shengYinShengJiEnumerator.MoveNext())
		{
			Dictionary<int, List<int>> dictionary = this.mSlotCanUsegoodsId;
			KeyValuePair<int, ShengYinShengJiVO> keyValuePair = shengYinShengJiEnumerator.Current;
			if (dictionary.ContainsKey(keyValuePair.Value.BuWei))
			{
				Dictionary<int, List<int>> dictionary2 = this.mSlotCanUsegoodsId;
				KeyValuePair<int, ShengYinShengJiVO> keyValuePair2 = shengYinShengJiEnumerator.Current;
				List<int> list = dictionary2[keyValuePair2.Value.BuWei];
				KeyValuePair<int, ShengYinShengJiVO> keyValuePair3 = shengYinShengJiEnumerator.Current;
				if (!list.Contains(keyValuePair3.Value.DaoJuID))
				{
					Dictionary<int, List<int>> dictionary3 = this.mSlotCanUsegoodsId;
					KeyValuePair<int, ShengYinShengJiVO> keyValuePair4 = shengYinShengJiEnumerator.Current;
					List<int> list2 = dictionary3[keyValuePair4.Value.BuWei];
					KeyValuePair<int, ShengYinShengJiVO> keyValuePair5 = shengYinShengJiEnumerator.Current;
					list2.Add(keyValuePair5.Value.DaoJuID);
				}
			}
			else
			{
				List<int> list3 = new List<int>();
				List<int> list4 = list3;
				KeyValuePair<int, ShengYinShengJiVO> keyValuePair6 = shengYinShengJiEnumerator.Current;
				list4.Add(keyValuePair6.Value.DaoJuID);
				Dictionary<int, List<int>> dictionary4 = this.mSlotCanUsegoodsId;
				KeyValuePair<int, ShengYinShengJiVO> keyValuePair7 = shengYinShengJiEnumerator.Current;
				dictionary4.Add(keyValuePair7.Value.BuWei, list3);
			}
		}
	}

	private void onDragFinished()
	{
		if (Math.Abs(Math.Abs(this.UIDragPl.transform.localPosition.x) - (float)(390 * this.CurrentSelectedPage)) > 30f)
		{
			if (this.UIDragPl.transform.localPosition.x > (float)(-390 * this.CurrentSelectedPage))
			{
				this.CurrentSelectedPage--;
				if (this.CurrentSelectedPage <= 0)
				{
					this.CurrentSelectedPage = 0;
				}
			}
			else
			{
				this.CurrentSelectedPage++;
				if (this.CurrentSelectedPage >= 10)
				{
					this.CurrentSelectedPage = 9;
				}
			}
		}
		this.UIDragPl.target.x = (float)(-390 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		this.RefreshBagPage();
	}

	private int Getindex(int dex)
	{
		int num = 50;
		this._GGoodsBox.listBox.maxPerLine = num;
		int num2 = dex / 5 / 4;
		int num3 = dex % 20;
		int num4 = num3 % 5;
		int num5 = num3 / 5 % 4;
		return num4 + num5 * num + num2 * 5;
	}

	private void InitSolt()
	{
		for (byte b = 0; b < 6; b += 1)
		{
			this.mRidePetShengYinSoltHanders[(int)b] = new RidePetYinJiPart.RidePetShengYinSoltHander(this._Solts[(int)b], (int)(b + 1));
			this.mRidePetShengYinSoltHanders[(int)b].Hander = new DPSelectedItemEventHandler(this.RidePetShengYinSoltHanders);
			this.mRidePetShengYinSoltHanders[(int)b].SlotIsOpen = IConfigbase<ConfigShengYinShengJi>.Instance.GetSoltISOpenByRideLevel(this.mRidePetShengYinSoltHanders[(int)b].SlotID);
			GoodsData roleRidePetShenShenYinJiSlotUsingGoodsData = Global.GetRoleRidePetShenShenYinJiSlotUsingGoodsData(Global.Data.roleData.HolyGoodsDataList, this.mRidePetShengYinSoltHanders[(int)b].SlotID);
			this.mRidePetShengYinSoltHanders[(int)b].UsingGoodsData = roleRidePetShenShenYinJiSlotUsingGoodsData;
		}
		this.RefreshTaoZhuang();
	}

	private void TaoZhuangHanderCallBack(object sender, DPSelectedItemEventArgs args)
	{
		int num = -1;
		ShengYinTaoZhuangVO shengYinTaoZhuangVO = null;
		byte b = 0;
		while ((int)b < this.mTaoZhuangHanderlist.Length)
		{
			if (this.mTaoZhuangHanderlist[(int)b] != null && args.ID == this.mTaoZhuangHanderlist[(int)b].ID)
			{
				num = this.mTaoZhuangHanderlist[(int)b].Count;
				shengYinTaoZhuangVO = this.mTaoZhuangHanderlist[(int)b].ShengYinTaoZhuangVO;
				break;
			}
			b += 1;
		}
		if (shengYinTaoZhuangVO != null && num != -1)
		{
			this._TaoZhuangTipsTitleLabel.text = Global.GetLang("套装属性");
			this._TaoZhuangTipsRoot.SetActive(true);
			string text = string.Empty;
			if (1 < num)
			{
				if (!string.IsNullOrEmpty(shengYinTaoZhuangVO.TaoZhuangStr2))
				{
					text += Global.GetColorStringForNGUIText(new object[]
					{
						(2 > num) ? "786F6F" : "16E53B",
						Global.GetLang("2件") + "\n" + shengYinTaoZhuangVO.TaoZhuangStr2
					});
				}
				if (!string.IsNullOrEmpty(shengYinTaoZhuangVO.TaoZhuangStr4))
				{
					text += Global.GetColorStringForNGUIText(new object[]
					{
						(4 > num) ? "786F6F" : "16E53B",
						Global.GetLang("4件") + "\n" + shengYinTaoZhuangVO.TaoZhuangStr4
					});
				}
				if (!string.IsNullOrEmpty(shengYinTaoZhuangVO.TaoZhuangStr6))
				{
					text += Global.GetColorStringForNGUIText(new object[]
					{
						(num != 6) ? "786F6F" : "16E53B",
						Global.GetLang("6件") + "\n" + shengYinTaoZhuangVO.TaoZhuangStr6
					});
				}
			}
			this._TaoZhuangTipsLabel.text = text;
		}
		else
		{
			this._TaoZhuangTipsRoot.SetActive(false);
		}
	}

	private void RidePetShengYinSoltHanders(object sender, DPSelectedItemEventArgs args)
	{
		GoodsData goodsData = null;
		for (byte b = 0; b < 6; b += 1)
		{
			if (this.mRidePetShengYinSoltHanders[(int)b].ID == args.ID)
			{
				goodsData = this.mRidePetShengYinSoltHanders[(int)b].UsingGoodsData;
				break;
			}
		}
		if (goodsData == null || goodsData.Using == 1)
		{
		}
		MUDebug.Log<int>(new int[]
		{
			args.ID
		});
	}

	private void InitPage()
	{
		this._GGoodsBox.RowCount = 4;
		this._GGoodsBox.ColCount = 50;
		this._GGoodsBox.InitBox();
	}

	private void RefreshTaoZhuang()
	{
		List<RidePetYinJiPart.ShengYinTaoZhuangData> list = new List<RidePetYinJiPart.ShengYinTaoZhuangData>();
		for (byte b3 = 1; b3 <= 6; b3 += 1)
		{
			GoodsData roleRidePetShenShenYinJiSlotUsingGoodsData = Global.GetRoleRidePetShenShenYinJiSlotUsingGoodsData(Global.Data.roleData.HolyGoodsDataList, (int)b3);
			if (roleRidePetShenShenYinJiSlotUsingGoodsData != null)
			{
				int LeiXing = IConfigbase<ConfigShengYinShengJi>.Instance.GetLeiXingByGoodsID(roleRidePetShenShenYinJiSlotUsingGoodsData.GoodsID);
				int num = list.FindIndex((RidePetYinJiPart.ShengYinTaoZhuangData e) => e.LeiXing == LeiXing);
				if (num == -1)
				{
					list.Add(new RidePetYinJiPart.ShengYinTaoZhuangData
					{
						LeiXing = LeiXing,
						Count = 1
					});
				}
				else
				{
					list[num].Count++;
				}
			}
		}
		list.Sort((RidePetYinJiPart.ShengYinTaoZhuangData a, RidePetYinJiPart.ShengYinTaoZhuangData b) => -(a.Count - b.Count));
		if (this.mTaoZhuangHanderlist == null)
		{
			List<int> taoZhuangLeiXingList = IConfigbase<ConfigShengYinShengJi>.Instance.GetTaoZhuangLeiXingList();
			this.mTaoZhuangHanderlist = new RidePetYinJiPart.TaoZhuangHander[taoZhuangLeiXingList.Count];
			for (int i = 0; i < taoZhuangLeiXingList.Count; i++)
			{
				this.mTaoZhuangHanderlist[i] = new RidePetYinJiPart.TaoZhuangHander(this._TaoZhuangItem[i], i);
				this.mTaoZhuangHanderlist[i].SetData(IConfigbase<ConfigShengYinShengJi>.Instance.GetShengYinTaoZhuangVODataByLeiXing(taoZhuangLeiXingList[i]), 0);
				this.mTaoZhuangHanderlist[i].Hander = new DPSelectedItemEventHandler(this.TaoZhuangHanderCallBack);
				this.mTaoZhuangHanderlist[i].Active = false;
			}
		}
		for (byte b2 = 0; b2 < 3; b2 += 1)
		{
			if ((int)b2 < list.Count && 1 < list[(int)b2].Count)
			{
				this.mTaoZhuangHanderlist[(int)b2].SetData(IConfigbase<ConfigShengYinShengJi>.Instance.GetShengYinTaoZhuangVODataByLeiXing(list[(int)b2].LeiXing), list[(int)b2].Count);
				this.mTaoZhuangHanderlist[(int)b2].Active = true;
			}
			else
			{
				this.mTaoZhuangHanderlist[(int)b2].Active = false;
			}
		}
	}

	private void InitTopBtnGoodIcon(int[] GoodsID)
	{
		if (GoodsID != null && 0 < GoodsID.Length)
		{
			for (int i = 0; i < GoodsID.Length; i++)
			{
				GGoodIcon ggoodIcon = this._TopBtnsGoodIcon[i];
				GoodsData emptyGoodsData = Global.GetEmptyGoodsData(GoodsID[i], 0, 0, 0, 1, 0, 0, 0, 0);
				ggoodIcon.Width = 78.0;
				ggoodIcon.Height = 78.0;
				ggoodIcon.GoodImg.URL = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
				{
					Super.GetIconCode(emptyGoodsData.GoodsID)
				});
				ggoodIcon.ItemCode = emptyGoodsData.GoodsID;
				ggoodIcon.ItemObject = emptyGoodsData;
				ggoodIcon.ItemCategory = Global.GetCategoriyByGoodsID(emptyGoodsData.GoodsID);
				Super.InitGoodsGIcon(ggoodIcon, emptyGoodsData, true, IconTextTypes.Qianghua);
				this._TopBtnsGoodIcon[i].addEventListener("click", new MouseEventHandler(this.TopGoodsIconBtnHander));
			}
		}
	}

	private void InitTopBtnSpIcon()
	{
		byte b = 0;
		while ((int)b < this._TopBtnsIconSP.Length)
		{
			this._TopBtnsIconSP[(int)b].spriteName = "Type" + ((int)(b + 1)).ToString() + "0";
			this._TopBtnsIconSP[(int)b].gameObject.name = ((int)(b + 1)).ToString();
			UIEventListener.Get(this._TopBtnsIconSP[(int)b].gameObject).onClick = new UIEventListener.VoidDelegate(this.InitTopBtnSpIconHander);
			this.mTopBtnsIconCheck[(int)b] = false;
			b += 1;
		}
	}

	private void InitTopBtnSpIconHander(GameObject go)
	{
		if (null != go)
		{
			int num = go.name.SafeToInt32(0);
			byte b = 0;
			while ((int)b < this._TopBtnsIconSP.Length)
			{
				if (this._TopBtnsIconSP[(int)b].name.Equals(go.name))
				{
					if (this.mTopBtnsIconCheck[(int)b])
					{
						this._TopBtnsIconSP[(int)b].spriteName = "Type" + ((int)(b + 1)).ToString() + "0";
						this.mTopBtnsIconCheck[(int)b] = false;
					}
					else
					{
						this._TopBtnsIconSP[(int)b].spriteName = "Type" + ((int)(b + 1)).ToString() + "1";
						this.mTopBtnsIconCheck[(int)b] = true;
					}
				}
				else
				{
					this.mTopBtnsIconCheck[(int)b] = false;
					this._TopBtnsIconSP[(int)b].spriteName = "Type" + ((int)(b + 1)).ToString() + "0";
				}
				b += 1;
			}
			if (Global.Data != null && Global.Data.roleData.HolyGoodsDataList != null)
			{
				this.RefreshBag(Global.Data.roleData.HolyGoodsDataList, (!this.mTopBtnsIconCheck[num - 1]) ? -1 : num);
			}
		}
	}

	private void TopGoodsIconBtnHander(MouseEvent e)
	{
		GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		MUDebug.Log<int>(new int[]
		{
			goodsData.GoodsID
		});
	}

	private void RefreshBagPage()
	{
		if (this.mTempPaneStat != null)
		{
			this.mTempPaneStat.spriteName = "selectState_normal2";
		}
		this._Pages[this.CurrentSelectedPage].spriteName = "selectState_hover2";
		this.mTempPaneStat = this._Pages[this.CurrentSelectedPage];
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData gd = ggoodIcon.ItemObject as GoodsData;
		if (gd == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.HolyBag, gd);
		GTipServiceEx.TipsSender.DPSelectedItem = delegate(object e, DPSelectedItemEventArgs args)
		{
			GoodsData gd;
			int id = gd.Id;
			TipsOperationTypes idtype = (TipsOperationTypes)args.IDType;
			if (idtype == TipsOperationTypes.Peidai)
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendRidePetShengYinLoad(id);
			}
			else if (idtype == TipsOperationTypes.Xiexia)
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendRidePetShengYinUnLoad(id);
			}
			else if (args.IDType == 10 && args.ID > 0)
			{
				int num = 0;
				List<GoodsData> list = Global.Data.roleData.HolyGoodsDataList.FindAll((GoodsData d) => d.Using == 0);
				if (list != null)
				{
					num = list.Count;
				}
				gd = gd;
				if (num < 200)
				{
					GameInstance.Game.SpriteSplitGoods(gd.Id, gd.Site, gd.GoodsID, args.ID);
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再拆分物品..."), new object[0]), 1, -1, -1, 0);
				}
			}
			this.mLastHandleGoodsDBID = id;
		};
	}

	private GGoodIcon AddIcon(GoodsData goodsData)
	{
		GGoodIcon newGoodIcon = Global.GetNewGoodIcon();
		newGoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		newGoodIcon.Width = 78.0;
		newGoodIcon.Height = 78.0;
		newGoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			Super.GetIconCode(goodsData.GoodsID)
		}), false, 0);
		newGoodIcon.ItemCode = goodsData.GoodsID;
		newGoodIcon.ItemObject = goodsData;
		newGoodIcon.ItemCategory = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		newGoodIcon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
		{
		};
		return newGoodIcon;
	}

	private void InitPrefabText()
	{
		try
		{
			this._TrimBtn.Text = Global.GetLang("整理");
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
			this.UIDragPl.GetComponent<UIDraggablePanel>().onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
			this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
			UIEventListener.Get(this._SoltRoot).onClick = delegate(GameObject e)
			{
				if (0f < Global.GetBtnCD(this._SoltRoot.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._SoltRoot.GetInstanceID(), 0.3f);
				Vector3 mousePosition = Input.mousePosition;
				Vector3 vector = Global.UICamera.WorldToScreenPoint(this._SoltRoot.transform.position);
				Vector3 vector2 = mousePosition - vector;
				float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
				if (0f > num)
				{
					num = 360f + num;
				}
				byte i;
				for (i = 0; i < 6; i += 1)
				{
					if (this.mRidePetShengYinSoltHanders[(int)i].UpDataClick(num))
					{
						break;
					}
				}
				MUDebug.Log<byte>(new byte[]
				{
					i
				});
				if ((int)i < this.mRidePetShengYinSoltHanders.Length)
				{
					if (this.mRidePetShengYinSoltHanders[(int)i].SlotIsOpen)
					{
						if (this.mRidePetShengYinSoltHanders[(int)i].UsingGoodsData != null)
						{
							if (null == this.mTipTempIcon)
							{
								this.mTipTempIcon = U3DUtils.NEW<GGoodIcon>();
								NGUITools.SetActiveChildren(this.mTipTempIcon.gameObject, false);
							}
							GTipServiceEx.ShowTip(this.mTipTempIcon, TipTypes.GoodsText, GoodsOwnerTypes.HolyBag, this.mRidePetShengYinSoltHanders[(int)i].UsingGoodsData);
							GTipServiceEx.TipsSender.DPSelectedItem = delegate(object ez, DPSelectedItemEventArgs args)
							{
								int id = this.mRidePetShengYinSoltHanders[(int)i].UsingGoodsData.Id;
								TipsOperationTypes idtype = (TipsOperationTypes)args.IDType;
								if (idtype == TipsOperationTypes.Peidai)
								{
									Super.ShowNetWaiting(null);
									GameInstance.Game.SendRidePetShengYinLoad(id);
								}
								else if (idtype == TipsOperationTypes.Xiexia)
								{
									Super.ShowNetWaiting(null);
									GameInstance.Game.SendRidePetShengYinUnLoad(id);
								}
								this.mLastHandleGoodsDBID = id;
							};
						}
						else
						{
							MUDebug.Log<string>(new string[]
							{
								"这个部位没有配的物品"
							});
						}
					}
					else
					{
						this.mRidePetShengYinSoltHanders[(int)i].ShowTips();
					}
				}
				else
				{
					MUDebug.Log<string>(new string[]
					{
						"RidePetShengYinSoltHanders  越界了啊啊啊   i==  " + i
					});
				}
			};
			this._TrimBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._TrimBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._TrimBtn.GetInstanceID(), 1f);
				GameInstance.Game.SendRidePetShengYinTrimBag();
			};
			UIEventListener.Get(this._HelpBtn).onClick = delegate(GameObject g)
			{
				if (0f < Global.GetBtnCD(this._HelpBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._HelpBtn.GetInstanceID(), 1f);
				this.OpenHelpWindow();
			};
			UIEventListener.Get(this._HelpBtn).onPress = delegate(GameObject g, bool b)
			{
				TweenScale.Begin(this._HelpBtn, 0.1f, (!b) ? new Vector3(44f, 44f, 1f) : new Vector3(34f, 34f, 1f));
			};
			UIEventListener.Get(this._XiangQingBtn).onClick = delegate(GameObject g)
			{
				if (0f < Global.GetBtnCD(this._XiangQingBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._XiangQingBtn.GetInstanceID(), 1f);
				this.ShowProp();
			};
			UIEventListener.Get(this._XiangQingBtn).onPress = delegate(GameObject g, bool b)
			{
				TweenScale.Begin(this._XiangQingBtn, 0.1f, (!b) ? new Vector3(44f, 44f, 1f) : new Vector3(34f, 34f, 1f));
			};
			UIEventListener.Get(this._TaoZhuangTipsCollider).onClick = delegate(GameObject g)
			{
				this._TaoZhuangTipsLabel.text = string.Empty;
				this._TaoZhuangTipsRoot.SetActive(false);
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

	private void OpenHelpWindow()
	{
		if (this.m_helpWindow == null)
		{
			this.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			this.m_helpWindow.IsShowModal = true;
			this.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_helpWindow, Global.GetLang("NewCommonHelpWindow"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_helpWindow);
		}
		if (this.m_helpPart == null)
		{
			this.m_helpPart = U3DUtils.NEW<NewCommonHelpWindow>();
			this.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHelpWindow();
			};
		}
		this.m_helpWindow.SetContent(this.m_helpWindow.BodyPresenter, this.m_helpPart, 0.0, 0.0, true);
		string text = "Config/ZuoQiShengYinIntro.xml";
		ChangeableRulePart.RuleXml ruleXml = null;
		if (ruleXml == null)
		{
			XElement gameResXml = Global.GetGameResXml(text);
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					string.Format(Global.GetLang("加载{0}出现错误"), text)
				});
			}
			ruleXml = new ChangeableRulePart.RuleXml(gameResXml);
		}
		this.m_helpPart.SetHelpInfo(ruleXml.list, false);
	}

	private void CloseHelpWindow()
	{
		if (null != this.m_helpPart)
		{
			this.m_helpPart.transform.parent = null;
			Object.Destroy(this.m_helpPart.gameObject);
			this.m_helpPart = null;
		}
		if (null != this.m_helpWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_helpWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_helpWindow, true);
			this.m_helpWindow = null;
		}
	}

	private void ShowProp()
	{
		Dictionary<string, double> dictionary = new Dictionary<string, double>();
		if (Global.Data != null && Global.Data.roleData != null && Global.Data.roleData.HolyGoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.HolyGoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.roleData.HolyGoodsDataList[i];
				if (goodsData != null && goodsData.Using == 1)
				{
					Dictionary<string, double> propByGoodsIDAndLevel = IConfigbase<ConfigShengYinShengJi>.Instance.GetPropByGoodsIDAndLevel(goodsData.GoodsID, goodsData.ElementhrtsProps[0]);
					if (propByGoodsIDAndLevel != null && 0 < propByGoodsIDAndLevel.Count)
					{
						foreach (KeyValuePair<string, double> keyValuePair in propByGoodsIDAndLevel)
						{
							if (dictionary.ContainsKey(keyValuePair.Key))
							{
								if (0.0 >= dictionary[keyValuePair.Key])
								{
									dictionary[keyValuePair.Key] = 0.0;
								}
								Dictionary<string, double> dictionary3;
								Dictionary<string, double> dictionary2 = dictionary3 = dictionary;
								string text2;
								string text = text2 = keyValuePair.Key;
								double num = dictionary3[text2];
								dictionary2[text] = num + keyValuePair.Value;
							}
							else
							{
								dictionary.Add(keyValuePair.Key, keyValuePair.Value);
							}
						}
					}
					List<int> washProps = goodsData.WashProps;
					if (washProps != null)
					{
						int j = 0;
						while (j < washProps.Count)
						{
							int id = washProps[++j];
							double num2 = (double)washProps[++j] / 1000.0;
							j++;
							ExtPropIndexesVO extPropIndexesVoByID = ConfigExtPropIndexes.GetExtPropIndexesVoByID(id);
							if (extPropIndexesVoByID != null)
							{
								string word = extPropIndexesVoByID.Word;
								if (dictionary.ContainsKey(word))
								{
									if (0.0 >= dictionary[word])
									{
										dictionary[word] = 0.0;
									}
									Dictionary<string, double> dictionary5;
									Dictionary<string, double> dictionary4 = dictionary5 = dictionary;
									string text2;
									string text3 = text2 = word;
									double num = dictionary5[text2];
									dictionary4[text3] = num + num2;
								}
								else
								{
									dictionary.Add(word, num2);
								}
							}
						}
					}
				}
			}
		}
		byte b = 0;
		while ((int)b < this.mTaoZhuangHanderlist.Length)
		{
			if (this.mTaoZhuangHanderlist[(int)b] != null && 0 < this.mTaoZhuangHanderlist[(int)b].Count)
			{
				string text4 = string.Empty;
				if (1 < this.mTaoZhuangHanderlist[(int)b].Count && 4 > this.mTaoZhuangHanderlist[(int)b].Count)
				{
					if (this.mTaoZhuangHanderlist[(int)b].Active)
					{
						text4 = this.mTaoZhuangHanderlist[(int)b].ShengYinTaoZhuangVO.TaoZhuangShuXingTwo;
					}
				}
				else if (4 <= this.mTaoZhuangHanderlist[(int)b].Count && 6 > this.mTaoZhuangHanderlist[(int)b].Count)
				{
					if (this.mTaoZhuangHanderlist[(int)b].Active)
					{
						text4 = this.mTaoZhuangHanderlist[(int)b].ShengYinTaoZhuangVO.TaoZhuangShuXingFour + "|" + this.mTaoZhuangHanderlist[(int)b].ShengYinTaoZhuangVO.TaoZhuangShuXingTwo;
					}
				}
				else if (this.mTaoZhuangHanderlist[(int)b].Active)
				{
					text4 = string.Concat(new string[]
					{
						this.mTaoZhuangHanderlist[(int)b].ShengYinTaoZhuangVO.TaoZhuangShuXingSix,
						"|",
						this.mTaoZhuangHanderlist[(int)b].ShengYinTaoZhuangVO.TaoZhuangShuXingFour,
						"|",
						this.mTaoZhuangHanderlist[(int)b].ShengYinTaoZhuangVO.TaoZhuangShuXingTwo
					});
				}
				if (!string.IsNullOrEmpty(text4))
				{
					string[] array = text4.Split(new char[]
					{
						'|'
					});
					for (int k = 0; k < array.Length; k++)
					{
						string[] array2 = array[k].Split(new char[]
						{
							','
						});
						if (array2.Length == 2)
						{
							if (dictionary.ContainsKey(array2[0]))
							{
								if (0.0 >= dictionary[array2[0]])
								{
									dictionary[array2[0]] = 0.0;
								}
								Dictionary<string, double> dictionary7;
								Dictionary<string, double> dictionary6 = dictionary7 = dictionary;
								string text2;
								string text5 = text2 = array2[0];
								double num = dictionary7[text2];
								dictionary6[text5] = num + double.Parse(array2[1]);
							}
							else
							{
								dictionary.Add(array2[0], double.Parse(array2[1]));
							}
						}
					}
				}
			}
			b += 1;
		}
		string[] array3 = new string[]
		{
			Global.GetLang("圣印总属性预览"),
			string.Empty
		};
		List<RidePetYinJiPart.PropertyStr> list = new List<RidePetYinJiPart.PropertyStr>();
		Dictionary<int, double> dictionary8 = new Dictionary<int, double>();
		foreach (KeyValuePair<string, double> keyValuePair2 in dictionary)
		{
			double value = keyValuePair2.Value;
			if (0.0 < value)
			{
				int extPropIndexesIDByWord = ConfigExtPropIndexes.GetExtPropIndexesIDByWord(keyValuePair2.Key);
				if (dictionary8.ContainsKey(extPropIndexesIDByWord))
				{
					dictionary8[extPropIndexesIDByWord] = value;
				}
				else
				{
					dictionary8.Add(extPropIndexesIDByWord, value);
				}
			}
		}
		foreach (KeyValuePair<int, double> keyValuePair3 in dictionary8)
		{
			int key = keyValuePair3.Key;
			int extPropIndexesShowListByID = ConfigExtPropIndexes.GetExtPropIndexesShowListByID(key);
			RidePetYinJiPart.PropertyStr propertyStr = default(RidePetYinJiPart.PropertyStr);
			propertyStr.ShowList = extPropIndexesShowListByID;
			propertyStr.Str = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, false);
			propertyStr.Percent = ConfigExtPropIndexes.GetPercentByID(key);
			Dictionary<int, double>.Enumerator enumerator3;
			KeyValuePair<int, double> keyValuePair4 = enumerator3.Current;
			propertyStr.Att = keyValuePair4.Value;
			list.Add(propertyStr);
		}
		for (int l = 0; l < list.Count; l++)
		{
			RidePetYinJiPart.PropertyStr propertyStr2 = list[l];
			if (propertyStr2.Percent)
			{
				string[] array4 = array3;
				int num3 = 1;
				array4[num3] = array4[num3] + string.Format(Global.GetLang("{0}：{1}%"), propertyStr2.Str, this.CutDoubleValue2(propertyStr2.Att * 100.0)) + Environment.NewLine;
			}
			else if (propertyStr2.Att > 1.0)
			{
				int num4 = this.MyDoubleToInt(propertyStr2.Att);
				string[] array5 = array3;
				int num5 = 1;
				array5[num5] = array5[num5] + string.Format(Global.GetLang("{0}：{1}"), propertyStr2.Str, num4) + Environment.NewLine;
			}
		}
		if (!string.IsNullOrEmpty(array3[1]))
		{
			Global.ShowProPerty(1, array3, null);
		}
		else
		{
			Super.HintMainText(Global.GetLang("暂无属性加成"), 10, 3);
		}
	}

	private string CutDoubleValue2(double value)
	{
		string text = string.Empty;
		string text2 = value.ToString();
		string[] array = text2.Split(new char[]
		{
			'.'
		});
		if (array.Length == 2)
		{
			text = text + array[0] + ".";
			if (2 >= array[1].Length)
			{
				text += array[1];
			}
			else
			{
				text += array[1].get_Chars(0);
				text += array[1].get_Chars(1);
			}
			return text;
		}
		return value.ToString();
	}

	private int MyDoubleToInt(double value)
	{
		string text = value.ToString();
		string[] array = text.Split(new char[]
		{
			'.'
		});
		if (array.Length < 1)
		{
			return Mathf.FloorToInt((float)value);
		}
		return int.Parse(array[0]);
	}

	private void GoPage(int Page)
	{
		if (0 >= Page)
		{
			Page = 0;
		}
		if (10 <= Page)
		{
			Page = 9;
		}
		this.CurrentSelectedPage = Page;
		this.UIDragPl.target.x = (float)(-390 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		this.RefreshBagPage();
	}

	public void RefreshGoods(GoodsData goodsData)
	{
		if (goodsData != null)
		{
			int num = this._GGoodsBox.FindByGoodsDbID(goodsData.Id);
			if (goodsData.GCount <= 0 || goodsData.Using > 0)
			{
				if (num != -1)
				{
					this._GGoodsBox.RemoveGoodsIcon(num);
				}
				return;
			}
			GGoodIcon ggoodIcon = this._GGoodsBox.GetGoodsIcon(num);
			if (null != ggoodIcon)
			{
				bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
				ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
				ggoodIcon.ItemObject = goodsData;
				Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			}
			else if (goodsData.GCount > 0 || goodsData.Using == 0)
			{
				bool flag = false;
				int caoWeiIndexByGoodsID = IConfigbase<ConfigShengYinShengJi>.Instance.GetCaoWeiIndexByGoodsID(goodsData.GoodsID);
				if (this.mTopBtnsIconCheck.ContainsKey(caoWeiIndexByGoodsID - 1) && this.mTopBtnsIconCheck[caoWeiIndexByGoodsID - 1])
				{
					flag = true;
				}
				if (!flag)
				{
					int num2 = 0;
					foreach (KeyValuePair<int, bool> keyValuePair in this.mTopBtnsIconCheck)
					{
						if (keyValuePair.Value)
						{
							break;
						}
						num2++;
					}
					if (num2 == 6)
					{
						flag = true;
					}
				}
				if (flag)
				{
					int num3 = -1;
					if (this.mGoodsIndex.ContainsKey(goodsData.GoodsID))
					{
						num3 = this.mGoodsIndex[goodsData.Id];
					}
					if (num3 == -1)
					{
						for (int i = 0; i < 200; i++)
						{
							if (null == this._GGoodsBox.GetGoodsIcon(this.Getindex(i)))
							{
								num3 = i;
								break;
							}
						}
					}
					if (0 <= num3)
					{
						ggoodIcon = this.AddIcon(goodsData);
						this._GGoodsBox.SetGoodsIcon(this.Getindex(num3), ggoodIcon);
						ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
						bool canUse2 = Global.CanUseGoods(goodsData.GoodsID, false, true);
						Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse2, IconTextTypes.Qianghua);
					}
				}
			}
			for (byte b = 0; b < 6; b += 1)
			{
				if (this.mRidePetShengYinSoltHanders[(int)b] != null && this.mRidePetShengYinSoltHanders[(int)b].UsingGoodsData != null && this.mRidePetShengYinSoltHanders[(int)b].UsingGoodsData.Id == goodsData.Id)
				{
					this.mRidePetShengYinSoltHanders[(int)b].UsingGoodsData = goodsData;
				}
			}
		}
	}

	public void RefreshSlot(GoodsData gd)
	{
		if (gd != null)
		{
			for (byte b = 0; b < 6; b += 1)
			{
				if (this.mRidePetShengYinSoltHanders[(int)b].UsingGoodsData != null && this.mRidePetShengYinSoltHanders[(int)b].UsingGoodsData.Id == gd.Id)
				{
					if (gd.Using == 1)
					{
						this.mRidePetShengYinSoltHanders[(int)b].UsingGoodsData = gd;
					}
					else
					{
						this.mRidePetShengYinSoltHanders[(int)b].UsingGoodsData = null;
					}
				}
				else if (gd.Using == 1)
				{
					int bagIndex = gd.BagIndex;
					if (bagIndex == this.mRidePetShengYinSoltHanders[(int)b].SlotID)
					{
						if (gd.Using == 1)
						{
							this.mRidePetShengYinSoltHanders[(int)b].UsingGoodsData = gd;
						}
						else
						{
							this.mRidePetShengYinSoltHanders[(int)b].UsingGoodsData = null;
						}
					}
				}
			}
			this.RefreshGoods(gd);
			this.RefreshTaoZhuang();
		}
	}

	internal void NoticeLoadCallBack(int ret)
	{
		if (ret == 1)
		{
		}
	}

	public void ReSetTopBtns()
	{
		byte b = 0;
		while ((int)b < this._TopBtnsIconSP.Length)
		{
			this.mTopBtnsIconCheck[(int)b] = false;
			this._TopBtnsIconSP[(int)b].spriteName = "Type" + ((int)(b + 1)).ToString() + "0";
			b += 1;
		}
	}

	public void RefreshBag(List<GoodsData> BagDataList, int BagSelecttype = -1)
	{
		if (BagDataList == null)
		{
			return;
		}
		List<GoodsData> list = new List<GoodsData>();
		for (int i = 0; i < BagDataList.Count; i++)
		{
			list.Add(BagDataList[i]);
		}
		list.Sort((GoodsData a, GoodsData b) => a.BagIndex - b.BagIndex);
		base.StartCoroutine<bool>(this.RefreshBagEX(list, BagSelecttype));
	}

	private IEnumerator RefreshBagEX(List<GoodsData> BagDataList, int BagSelecttype = -1)
	{
		this.mGoodsIndex.Clear();
		Dictionary<int, GoodsData> indexDict = new Dictionary<int, GoodsData>();
		indexDict.Clear();
		if (BagDataList != null && 0 < BagDataList.Count)
		{
			int Index = 0;
			if (BagSelecttype == -1)
			{
				int subIndex = 0;
				int max = BagDataList.Count;
				while (subIndex < max)
				{
					GoodsData gd = BagDataList[subIndex];
					if (gd != null && gd.GCount > 0 && gd.Using == 0)
					{
						indexDict[gd.BagIndex] = gd;
					}
					subIndex++;
				}
			}
			else
			{
				List<int> CanUseGoodsID = this.mSlotCanUsegoodsId[BagSelecttype];
				int subIndex2 = 0;
				int max2 = BagDataList.Count;
				while (subIndex2 < max2)
				{
					GoodsData gd2 = BagDataList[subIndex2];
					if (gd2 != null)
					{
						if (gd2.GCount > 0 && gd2.Using == 0 && CanUseGoodsID.Contains(gd2.GoodsID))
						{
							indexDict.Add(Index, gd2);
							Index++;
						}
						else if (Global.GetCategoriyByGoodsID(gd2.GoodsID) == 981)
						{
							indexDict.Add(Index, gd2);
							Index++;
						}
					}
					subIndex2++;
				}
				this.GoPage(0);
			}
		}
		yield return null;
		byte refreshIconCount = 0;
		for (int i = 0; i < 200; i++)
		{
			GoodsData gd3 = null;
			if (indexDict.TryGetValue(i, ref gd3))
			{
				this.mGoodsIndex[gd3.Id] = i;
				GGoodIcon icon = this.AddIcon(gd3);
				this._GGoodsBox.SetGoodsIcon(this.Getindex(i), icon);
				icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
				bool canUseGoods = Global.CanUseGoods(gd3.GoodsID, false, true);
				Super.InitGoodsGIcon(icon, gd3, canUseGoods, IconTextTypes.Qianghua);
				if ((refreshIconCount += 1) > 5)
				{
					refreshIconCount = 0;
					yield return null;
				}
			}
			else
			{
				this._GGoodsBox.RemoveGoodsIcon(this.Getindex(i));
			}
		}
		yield break;
	}

	public static void Error(string err)
	{
		switch (err.SafeToInt32(0))
		{
		case 2:
			Super.HintMainText(Global.GetLang("功能未开启"), 10, 3);
			break;
		case 3:
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			break;
		case 4:
			Super.HintMainText(Global.GetLang("不可升级的圣印类型"), 10, 3);
			break;
		case 5:
			Super.HintMainText(Global.GetLang("圣印背包不包含要升级的物品"), 10, 3);
			break;
		case 6:
			Super.HintMainText(Global.GetLang("圣印背包不包含要吞噬的物品"), 10, 3);
			break;
		case 7:
			Super.HintMainText(Global.GetLang("吞噬的物品的数量出错"), 10, 3);
			break;
		case 8:
			Super.HintMainText(Global.GetLang("系统不包含当前吞噬物品"), 10, 3);
			break;
		case 9:
			Super.HintMainText(Global.GetLang("不包含当前物品等级"), 10, 3);
			break;
		case 10:
			Super.HintMainText(Global.GetLang("已经到达最大等级"), 10, 3);
			break;
		case 11:
			Super.HintMainText(Global.GetLang("吞噬材料类型出错"), 10, 3);
			break;
		case 12:
			Super.HintMainText(Global.GetLang("升级出错"), 10, 3);
			break;
		case 13:
			Super.HintMainText(Global.GetLang("升级计算属性出错"), 10, 3);
			break;
		case 14:
			Super.HintMainText(Global.GetLang("生成属性信息出错"), 10, 3);
			break;
		case 15:
			Super.HintMainText(Global.GetLang("消耗道具出错"), 10, 3);
			break;
		case 16:
			Super.HintMainText(Global.GetLang("物品已经被佩戴"), 10, 3);
			break;
		case 17:
			Super.HintMainText(Global.GetLang("不是佩戴类型"), 10, 3);
			break;
		case 18:
			Super.HintMainText(Global.GetLang("获取槽位个数出错"), 10, 3);
			break;
		case 19:
			Super.HintMainText(Global.GetLang("佩戴的位置未解锁"), 10, 3);
			break;
		case 20:
			Super.HintMainText(Global.GetLang("物品已经被卸下"), 10, 3);
			break;
		case 21:
			Super.HintMainText(Global.GetLang("槽位中没有物品"), 10, 3);
			break;
		case 22:
			Super.HintMainText(Global.GetLang("背包没有空间不能卸下"), 10, 3);
			break;
		case 23:
			Super.HintMainText(Global.GetLang("数据修改错误"), 10, 3);
			break;
		}
	}

	private const int PageConut = 10;

	private const int LineConnt = 5;

	private const int PageLine = 4;

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private UISprite[] _Pages;

	[SerializeField]
	private GGoodsBox _GGoodsBox;

	[SerializeField]
	private ListBox _GoodsListBox;

	[SerializeField]
	private SpringPanel UIDragPl;

	[SerializeField]
	private GButton _CloseBtn;

	[SerializeField]
	private GGoodIcon[] _TopBtnsGoodIcon;

	[SerializeField]
	private UISprite[] _TopBtnsIconSP;

	[SerializeField]
	private GameObject[] _Solts;

	[SerializeField]
	private GameObject _SoltRoot;

	[SerializeField]
	private GButton _TrimBtn;

	[SerializeField]
	private GameObject _XiangQingBtn;

	[SerializeField]
	private GameObject _HelpBtn;

	[SerializeField]
	private GameObject[] _TaoZhuangItem;

	[SerializeField]
	private GameObject _TaoZhuangTipsRoot;

	[SerializeField]
	private GameObject _TaoZhuangTipsCollider;

	[SerializeField]
	private Transform _TaoZhuangTipsBak;

	[SerializeField]
	private UILabel _TaoZhuangTipsLabel;

	[SerializeField]
	private UILabel _TaoZhuangTipsTitleLabel;

	private RaycastHit lh;

	private UISprite mTempPaneStat;

	private int CurrentSelectedPage;

	private Dictionary<int, bool> mTopBtnsIconCheck = new Dictionary<int, bool>();

	private RidePetYinJiPart.RidePetShengYinSoltHander[] mRidePetShengYinSoltHanders = new RidePetYinJiPart.RidePetShengYinSoltHander[6];

	private Dictionary<int, List<int>> mSlotCanUsegoodsId = new Dictionary<int, List<int>>();

	private int mLastHandleGoodsDBID;

	private Dictionary<int, int> mGoodsIndex = new Dictionary<int, int>();

	private GGoodIcon mTipTempIcon;

	private RidePetYinJiPart.TaoZhuangHander[] mTaoZhuangHanderlist;

	protected GChildWindow m_helpWindow;

	protected NewCommonHelpWindow m_helpPart;

	private struct PropertyStr
	{
		public int ShowList;

		public bool Percent;

		public string Str;

		public double Att;
	}

	private class RidePetShengYinSoltHander
	{
		public RidePetShengYinSoltHander(GameObject Root, int id)
		{
			this.mRoot = Root;
			if (id == 1)
			{
				this.SlotID = 6;
				this.angles[0] = 90;
				this.angles[1] = 150;
			}
			else if (id == 2)
			{
				this.SlotID = 1;
				this.angles[0] = 30;
				this.angles[1] = 90;
			}
			else if (id == 3)
			{
				this.SlotID = 2;
				this.angles[0] = 30;
				this.angles[1] = -30;
			}
			else if (id == 4)
			{
				this.SlotID = 3;
				this.angles[0] = 270;
				this.angles[1] = 330;
			}
			else if (id == 5)
			{
				this.SlotID = 4;
				this.angles[0] = 210;
				this.angles[1] = 270;
			}
			else if (id == 6)
			{
				this.SlotID = 5;
				this.angles[0] = 150;
				this.angles[1] = 210;
			}
			this.mID = id;
			this._Image = this.mRoot.transform.FindChild("Slot").GetComponent<ShowNetImage>();
			this._TipsRoot = this.mRoot.transform.FindChild("Tips").gameObject;
			this._Colloider = this._TipsRoot.transform.FindChild("collider").gameObject;
			this._TipsValue = this._TipsRoot.transform.FindChild("Value").GetComponent<UILabel>();
			this._TipsRoot.SetActive(false);
			this._Suo = this.mRoot.transform.FindChild("Suo").gameObject;
		}

		public GoodsData UsingGoodsData
		{
			get
			{
				return this.mUsingGoodsData;
			}
			set
			{
				this.mUsingGoodsData = value;
				if (this.mUsingGoodsData != null && this.mUsingGoodsData.Using == 1)
				{
					this._Image.URL = "NetImages/GameRes/Images/RidePet/" + this.mUsingGoodsData.GoodsID + ".png";
					this._Image.enabled = true;
				}
				else
				{
					this._Image.SetTexture(null);
				}
			}
		}

		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		public bool SlotIsOpen
		{
			get
			{
				return this.mSlotIsOpen;
			}
			set
			{
				this.mSlotIsOpen = value;
				this._Suo.SetActive(!this.mSlotIsOpen);
			}
		}

		public bool UpDataClick(float angle)
		{
			if (330f <= angle && 360f >= angle)
			{
				angle -= 360f;
			}
			if ((float)this.angles[0] <= angle && (float)this.angles[1] > angle)
			{
				return true;
			}
			if (0 > this.angles[0] || 0 > this.angles[1])
			{
				if (0f <= angle && angle <= (float)this.angles[0])
				{
					return true;
				}
				if ((float)this.angles[0] <= angle && angle <= 0f)
				{
					return true;
				}
			}
			return false;
		}

		private Quaternion GetSolrRotation(int ID, byte tips = 0)
		{
			float num = (float)(-110 + ID * 60);
			return Quaternion.Euler(new Vector3(0f, 0f, (tips != 0) ? (-num) : num));
		}

		public void ShowTips()
		{
			int soltOpenLevel = IConfigbase<ConfigShengYinShengJi>.Instance.GetSoltOpenLevel(this.SlotID);
			this._TipsRoot.SetActive(true);
			this._TipsValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("坐骑栏位等级达到：") + soltOpenLevel
			});
			UIEventListener.Get(this._Colloider).onClick = delegate(GameObject g)
			{
				this._TipsRoot.SetActive(false);
			};
			this._TipsRoot.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		}

		public int SlotID;

		private GameObject mRoot;

		private bool mSlotIsOpen;

		public DPSelectedItemEventHandler Hander;

		private ShowNetImage _Image;

		private GameObject _Suo;

		private int mID;

		private GoodsData mUsingGoodsData;

		private GameObject _TipsRoot;

		private GameObject _Colloider;

		private UILabel _TipsValue;

		private Vector3[] mPos = new Vector3[3];

		private int[] angles = new int[2];
	}

	private class TaoZhuangHander
	{
		public TaoZhuangHander(GameObject root, int id)
		{
			this.mRoot = root;
			this._Label = this.mRoot.transform.FindChild("Label").GetComponent<UILabel>();
			this._IconImage = this.mRoot.transform.FindChild("Icon").GetComponent<ShowNetImage>();
			this.mID = id;
			UIEventListener.Get(this.mRoot).onClick = delegate(GameObject e)
			{
				if (0f < Global.GetBtnCD(this.mRoot.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this.mRoot.GetInstanceID(), 0.5f);
				if (this.Hander != null)
				{
					this.Hander(this.mRoot, new DPSelectedItemEventArgs
					{
						ID = this.mID
					});
				}
			};
		}

		public ShengYinTaoZhuangVO ShengYinTaoZhuangVO
		{
			get
			{
				return this.mShengYinTaoZhuangVO;
			}
		}

		public int Count
		{
			get
			{
				return this.mCount;
			}
		}

		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		public bool Active
		{
			get
			{
				return NGUITools.GetActive(this.mRoot);
			}
			set
			{
				this.mRoot.SetActive(value);
			}
		}

		public void SetData(ShengYinTaoZhuangVO vo, int Count)
		{
			this.mCount = Count;
			this.mShengYinTaoZhuangVO = vo;
			this.Active = (1 < Count);
			string url = string.Empty;
			if (2 <= Count && 4 > Count)
			{
				this._Label.text = Global.GetLang("2件套");
				url = "NetImages/GameRes/Images/RidePet/" + vo.LeiXing + "_2.png";
			}
			else if (4 <= Count && 6 > Count)
			{
				this._Label.text = Global.GetLang("4件套");
				url = "NetImages/GameRes/Images/RidePet/" + vo.LeiXing + "_4.png";
			}
			else if (Count == 6)
			{
				this._Label.text = Global.GetLang("6件套");
				url = "NetImages/GameRes/Images/RidePet/" + vo.LeiXing + "_6.png";
			}
			this._IconImage.URL = url;
		}

		public DPSelectedItemEventHandler Hander;

		private GameObject mRoot;

		private UILabel _Label;

		private ShowNetImage _IconImage;

		private int mID = -1;

		private ShengYinTaoZhuangVO mShengYinTaoZhuangVO;

		private int mCount;
	}

	public class ShengYinTaoZhuangData
	{
		public int LeiXing;

		public int Count;
	}
}
