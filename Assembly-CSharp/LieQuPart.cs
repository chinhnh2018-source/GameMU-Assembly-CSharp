using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class LieQuPart : UserControl, IDisposable
{
	void IDisposable.Dispose()
	{
		MUDebug.Log<string>(new string[]
		{
			"<color=yellow>Dispose</color>"
		});
	}

	public List<int> HaveActiviteList
	{
		set
		{
			this.mHaveActiviteList = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitLab();
		this.InitEffect();
		this.InitOnClick();
		this.InitValue();
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.baoshiOne, "ZuoQiBuHuo", ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0], "xingyunzhixing");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.baoshiTen, "ZuoQiBuHuo", ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1], "xingyunzhixing");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.iconXingYunXing, "ZuoQiBuHuo", ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0], "xingyunzhixing");
		U3DUtils.ReplaceLayerInChildren(this._TeXiaoJuNeng, LayerMask.NameToLayer("MUUI"), null);
		GameInstance.Game.GetRidePetMainData();
		GameInstance.Game.GetJieriFanbeiInfo();
		base.StartCoroutine<bool>(this.InitShowGoodsEXEX());
		this.NoticeRefreshCangKuNum();
		this.AddBangZhuPanel();
	}

	private void AddBangZhuPanel()
	{
		this.m_PanelBangZhu.gameObject.SetActive(false);
		Dictionary<int, HorseIntro>.Enumerator enumerator = IConfigbase<ConfigRidePet>.Instance.DicHorseIntro.GetEnumerator();
		float num = 0f;
		bool flag = true;
		while (enumerator.MoveNext())
		{
			if (flag)
			{
				flag = false;
				UILabel labBangZhuTitle = this.m_LabBangZhuTitle;
				KeyValuePair<int, HorseIntro> keyValuePair = enumerator.Current;
				labBangZhuTitle.text = Global.GetLang(keyValuePair.Value.Intro);
				KeyValuePair<int, HorseIntro> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.Bold == 1)
				{
					this.m_LabBangZhuTitle.renderStyle = 1;
				}
				else
				{
					UILabel labBangZhuTitle2 = this.m_LabBangZhuTitle;
					KeyValuePair<int, HorseIntro> keyValuePair3 = enumerator.Current;
					labBangZhuTitle2.text = Global.GetLang(keyValuePair3.Value.Intro);
				}
			}
			else
			{
				UILabel uilabel = NGUITools.AddWidget<UILabel>(this.m_PanelBangZhuList.gameObject);
				uilabel.font = this.m_Font;
				uilabel.lineWidth = 525;
				uilabel.pivot = 0;
				uilabel.transform.localScale = Vector3.one * 18f;
				uilabel.transform.localPosition = new Vector3(0f, num, -0.001f);
				uilabel.color = new Color(218f, 199f, 174f);
				UILabel uilabel2 = uilabel;
				KeyValuePair<int, HorseIntro> keyValuePair4 = enumerator.Current;
				uilabel2.renderStyle = ((keyValuePair4.Value.Bold != 1) ? 0 : 1);
				TextBlock textBlock = uilabel.gameObject.AddComponent<TextBlock>();
				textBlock.Label = uilabel;
				textBlock._CharMargin = new Vector2(0f, 16f);
				TextBlock textBlock2 = textBlock;
				KeyValuePair<int, HorseIntro> keyValuePair5 = enumerator.Current;
				textBlock2.Text = Global.GetLang(keyValuePair5.Value.Intro);
				Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(uilabel.transform);
				num -= uilabel.relativeSize.y * uilabel.transform.localScale.y;
			}
		}
		BoxCollider boxCollider = this.m_PanelBangZhuList.GetComponent<BoxCollider>();
		if (null == boxCollider)
		{
			boxCollider = this.m_PanelBangZhuList.gameObject.AddComponent<BoxCollider>();
		}
		UIEventListener.Get(this.m_PanelBangZhuList.gameObject);
		UIDragPanelContents uidragPanelContents = this.m_PanelBangZhuList.GetComponent<UIDragPanelContents>();
		if (null == uidragPanelContents)
		{
			uidragPanelContents = this.m_PanelBangZhuList.gameObject.AddComponent<UIDragPanelContents>();
		}
		uidragPanelContents.draggablePanel = this.mDragPanelContent;
	}

	private IEnumerator InitShowGoodsEXEX()
	{
		yield return null;
		List<GoodsData> ShowGoodsDataList = new List<GoodsData>();
		int[] ShowHorseArray = (!Global.isFanbei(252)) ? ConfigSystemParam.GetSystemParamIntArrayByName("HorseShow", ',') : ConfigSystemParam.GetSystemParamIntArrayByName("TeQuanHorseShow", ',');
		if (ShowHorseArray != null && 0 < ShowHorseArray.Length)
		{
			for (int i = 0; i < ShowHorseArray.Length; i++)
			{
				if (0 < ShowHorseArray[i])
				{
					ShowGoodsDataList.Add(Global.GetEmptyGoodsData(ShowHorseArray[i], 0, 0, 0, 1, 0, 0, 0, 0));
				}
			}
		}
		if (0 < ShowGoodsDataList.Count)
		{
			yield return null;
			this.rc = this._GoodsRoot.gameObject.AddComponent<RollAinmationControl>();
			this.rc.EndPos = new Vector2(-88f, 0f);
			this.rc.Interval = new Vector2(88f, 0f);
			this.rc.Speed = (float)ShowGoodsDataList.Count;
			for (int j = 0; j < ShowGoodsDataList.Count; j++)
			{
				GGoodIcon icon = this.AddIcon(ShowGoodsDataList[j]);
				if (null != icon)
				{
					this.rc.AddObj(icon);
					UIPanel p = icon.GetComponent<UIPanel>();
					if (null != p)
					{
						Object.Destroy(p);
					}
				}
			}
		}
		this.HaveLoadAllGoodsShow = true;
		yield break;
	}

	private IEnumerator InitShowGoodsEX()
	{
		List<GoodsData> ShowGoodsDataList = new List<GoodsData>();
		int[] ShowHorseArray = ConfigSystemParam.GetSystemParamIntArrayByName("HorseShow", ',');
		if (ShowHorseArray != null && 0 < ShowHorseArray.Length)
		{
			for (int i = 0; i < ShowHorseArray.Length; i++)
			{
				if (0 < ShowHorseArray[i])
				{
					ShowGoodsDataList.Add(Global.GetEmptyGoodsData(ShowHorseArray[i], 0, 0, 0, 1, 0, 0, 0, 0));
				}
			}
		}
		if (0 < ShowGoodsDataList.Count)
		{
			yield return null;
			this.mLastX = (float)((ShowGoodsDataList.Count - 1) * 88);
			this.mGoodsShowTweenPositionArray = new TweenPosition[ShowGoodsDataList.Count];
			for (int j = 0; j < ShowGoodsDataList.Count; j++)
			{
				GGoodIcon icon = this.AddIcon(ShowGoodsDataList[j]);
				icon.transform.localPosition = new Vector3((float)(0 + 88 * j), 0f, 0f);
				if (null != icon)
				{
					TweenPosition tPos = icon.gameObject.AddComponent<TweenPosition>();
					tPos.style = 0;
					tPos.from = icon.transform.localPosition;
					tPos.to = new Vector3(-88f, 0f, 0f);
					tPos.duration = (float)(j + 1);
					this.mGoodsShowTweenPositionArray[j] = tPos;
					tPos.onFinished = new UITweener.OnFinished(this.TweenFinidshed);
					UIPanel p = tPos.GetComponent<UIPanel>();
					if (null != p)
					{
						Object.Destroy(p);
					}
					tPos.enabled = true;
					tPos.name = j.ToString();
				}
			}
			this.mAinmationRunTime = new float[ShowGoodsDataList.Count];
		}
		this.HaveLoadAllGoodsShow = true;
		yield break;
	}

	private IEnumerator startAinmation(UITweener tween)
	{
		yield return new WaitForEndOfFrame();
		tween.enabled = true;
		yield break;
	}

	private void TweenFinidshed(UITweener tween)
	{
		if (null != tween)
		{
			TweenPosition component = tween.GetComponent<TweenPosition>();
			if (component != null)
			{
				component.from = new Vector3(this.mLastX + 20f, 0f, 0f);
				component.to = new Vector3(-88f, 0f, 0f);
				component.style = 1;
				component.duration = (float)this.mGoodsShowTweenPositionArray.Length;
				component.enabled = true;
				component.Reset();
				component.Play(true);
			}
		}
	}

	private IEnumerator InitShowGoods()
	{
		List<GoodsData> ShowGoodsDataList = new List<GoodsData>();
		int[] ShowHorseArray = ConfigSystemParam.GetSystemParamIntArrayByName("HorseShow", ',');
		if (ShowHorseArray != null && 0 < ShowHorseArray.Length)
		{
			for (int i = 0; i < ShowHorseArray.Length; i++)
			{
				if (0 < ShowHorseArray[i])
				{
					ShowGoodsDataList.Add(Global.GetEmptyGoodsData(ShowHorseArray[i], 0, 0, 0, 1, 0, 0, 0, 0));
				}
			}
		}
		if (0 < ShowGoodsDataList.Count)
		{
			this.mGoodsShowAnimationArray = new Animation[ShowGoodsDataList.Count];
			for (int j = 0; j < ShowGoodsDataList.Count; j++)
			{
				GGoodIcon icon = this.AddIcon(ShowGoodsDataList[j]);
				if (null != icon)
				{
					Animation animation = icon.GetComponent<Animation>();
					if (null == animation)
					{
						animation = icon.gameObject.AddComponent<Animation>();
					}
					AnimationState state = animation["zuoqi_liequ"];
					if (null == state)
					{
						animation.AddClip(this._GoodsShowCilp, "zuoqi_liequ");
					}
					animation.clip = this._GoodsShowCilp;
					animation.CrossFade("zuoqi_liequ", 0f);
					this.mGoodsShowAnimationArray[j] = animation;
					UIPanel p = animation.GetComponent<UIPanel>();
					if (null != p)
					{
						Object.Destroy(p);
					}
					animation.Play();
					state = animation["zuoqi_liequ"];
					state.time = this._GoodsShowCilp.length / 8f * (float)(8 - j);
					yield return null;
				}
			}
			this.mAinmationRunTime = new float[ShowGoodsDataList.Count];
		}
		this.HaveLoadAllGoodsShow = true;
		yield break;
	}

	private GGoodIcon AddIcon(GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = backSpriteName;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = gd.GoodsID;
			ggoodIcon.ItemObject = gd;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, gd, canUse, IconTextTypes.None);
			UIEventListener.Get(ggoodIcon.gameObject).onClick = delegate(GameObject s)
			{
				if (this.HaveLoadAllGoodsShow)
				{
					GGoodIcon ggoodIcon2 = s.SafeGetComponent<GGoodIcon>();
					if (null == ggoodIcon2)
					{
						return;
					}
					if (this.m_DpsHandler != null)
					{
						this.m_DpsHandler(this, new DPSelectedItemEventArgs
						{
							Type = 2,
							Data = ggoodIcon2
						});
					}
				}
			};
			ggoodIcon.transform.SetParent(this._GoodsRoot, false);
			return ggoodIcon;
		}
		return null;
	}

	public void ActiveAnimationEX(bool active)
	{
		int i = 0;
		int num = this.mGoodsShowTweenPositionArray.Length;
		while (i < num)
		{
			TweenPosition tweenPosition = this.mGoodsShowTweenPositionArray[i];
			if (null != tweenPosition)
			{
				tweenPosition.enabled = active;
			}
			i++;
		}
	}

	public void ActiveAnimation(bool active)
	{
		this.rc.Stop = !active;
	}

	private void ItemClick(object sender, DPSelectedItemEventArgs args)
	{
		this.ActiveAnimation(true);
	}

	private void InitEffect()
	{
		GameObject gameObject = Global.LoadTeXiaoObj("UITeXiao/Perfabs/zuoqi/zuoqi_zhen", this.mTexiaoRoot);
		if (null != gameObject)
		{
		}
	}

	private void OpenShop()
	{
		if (this.mDuiHuanWindow != null)
		{
			this.CloseDuiHuanWindow();
		}
		this.mDuiHuanWindow = U3DUtils.NEW<GChildWindow>();
		this.mDuiHuanWindow.ModalType = ChildWindowModalType.Translucent;
		this.mDuiHuanWindow.IsShowModal = true;
		Super.InitChildWindow(this.mDuiHuanWindow, "mDuiHuanWindow");
		Super.GData.GlobalPlayZone.Children.Add(this.mDuiHuanWindow);
		this.mDuiHuanPart = U3DUtils.NEW<MUDuiHuanPart>();
		this.mDuiHuanWindow.Body.Add(this.mDuiHuanPart);
		this.mDuiHuanPart.InitPartData(8, 0);
		this.mDuiHuanPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs args)
		{
			this.CloseDuiHuanWindow();
			return false;
		};
	}

	private void CloseDuiHuanWindow()
	{
		if (null != this.mDuiHuanPart)
		{
			Object.Destroy(this.mDuiHuanPart.gameObject);
			this.mDuiHuanPart = null;
		}
		if (null != this.mDuiHuanWindow)
		{
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.mDuiHuanWindow);
		}
	}

	public void NoticeRefreshCangKuNum()
	{
		if (Global.Data.roleData.MountStoreList != null && 0 < Global.Data.roleData.MountStoreList.Count)
		{
			this._CangKuNumber.transform.parent.gameObject.SetActive(true);
			this._CangKuNumber.text = Global.Data.roleData.MountStoreList.Count.ToString();
		}
		else
		{
			this._CangKuNumber.transform.parent.gameObject.SetActive(false);
		}
	}

	public override void Update()
	{
		this.mTimeSatr += Time.deltaTime;
		if (this.mTimeSatr >= 4f)
		{
			this.mStartOnClick = true;
		}
	}

	public void StartTimeOnClick()
	{
		this.mStartOnClick = false;
		this.mTimeSatr = 0f;
	}

	public void InitLab()
	{
		this.m_BtnOne.Text = Global.GetLang("召唤一次");
		this.m_BtnTen.Text = Global.GetLang("召唤十次");
		this.m_LabZuanShi.text = ((!IConfigbase<ConfigXingYunXingShiYong>.Instance.XingYunXingKaiGuan("ZuoQiBuHuo")) ? Global.GetRoleOwnNumByMoneyType(40).ToString() : Global.GetRoleOwnNumByMoneyType(163).ToString());
		this.m_LabJinFen.text = Global.GetRoleOwnNumByMoneyType(140).ToString();
	}

	private void ShowHorseLieQuGoodsShowPart(ZuoQiChouQuResult chouQuData)
	{
		if (null != this.mHorseLieQuGoodsShowPartWind)
		{
			this.mHorseLieQuGoodsShowPartWind.Visibility = true;
		}
		else
		{
			this.mHorseLieQuGoodsShowPartWind = U3DUtils.NEW<GChildWindow>();
			this.mHorseLieQuGoodsShowPartWind.Modal = true;
			this.mHorseLieQuGoodsShowPartWind.ModalType = ChildWindowModalType.Translucent2;
			Global.MainStage.Add(this.mHorseLieQuGoodsShowPartWind);
			WindowManage.AddWindows(this.mHorseLieQuGoodsShowPartWind, true, null);
		}
		if (null == this.mHorseLieQuGoodsShowPart)
		{
			this.mHorseLieQuGoodsShowPart = U3DUtils.NEW<HorseLieQuGoodsShowPart>();
			this.mHorseLieQuGoodsShowPart.HaveActiviteList = this.mHaveActiviteList;
			this.mHorseLieQuGoodsShowPartWind.Body.Add(this.mHorseLieQuGoodsShowPart);
			this.mHorseLieQuGoodsShowPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s.Type == 0)
				{
					this.CloseHorseLieQuGoodsShowPart();
				}
			};
		}
		this.mHorseLieQuGoodsShowPart.RefreshChouQu(chouQuData);
	}

	private void CloseHorseLieQuGoodsShowPart()
	{
		if (null != this.mHorseLieQuGoodsShowPart)
		{
			Object.Destroy(this.mHorseLieQuGoodsShowPart.gameObject);
			this.mHorseLieQuGoodsShowPart = null;
		}
		if (null != this.mHorseLieQuGoodsShowPartWind)
		{
			WindowManage.RemoveWindows(this.mHorseLieQuGoodsShowPartWind);
			Object.Destroy(this.mHorseLieQuGoodsShowPartWind.gameObject);
			this.mHorseLieQuGoodsShowPartWind = null;
		}
	}

	private void InitOnClick()
	{
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_DpsHandler != null)
			{
				this.m_DpsHandler(null, new DPSelectedItemEventArgs
				{
					Type = 0
				});
			}
		};
		this.m_BtnChongZhi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_DpsHandler != null)
			{
				this.m_DpsHandler(null, new DPSelectedItemEventArgs
				{
					Type = 3
				});
			}
		};
		this.m_BtnJiFen.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (ConfigSystemParam.GetSystemParamByName("HorseShopOpen", true).SafeToInt32(0) == 0)
			{
				Super.HintMainText(Global.GetLang("积分商城暂未开启"), 10, 3);
				return;
			}
			this.OpenShop();
		};
		this.m_BtnCangKu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_DpsHandler != null)
			{
				this.m_DpsHandler(null, new DPSelectedItemEventArgs
				{
					Type = 0,
					ID = 1
				});
			}
		};
		this.m_BtnOne.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "ZuoQiBuHuo", ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0]);
			if (this.mStartOnClick)
			{
				if (this.m_Time <= Global.GetCorrectDateTime())
				{
					this.StartTimeOnClick();
					GameInstance.Game.GetRidePetChouQu(0, 0);
				}
				else
				{
					if (Global.GetRoleOwnNumByMoneyType(163) < ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0] && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ZuoQiBuHuo", ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0], true))
					{
						IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0] - Global.GetRoleOwnNumByMoneyType(163);
						string lang = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							Global.GetLang(string.Format(lang, IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi))
						}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								GameInstance.Game.SendBoCaiDaiBi(IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi, 2);
							}
							return true;
						};
						return;
					}
					if (Global.GetZuanShi(ZuanShiPartClass.RidePetChouQu))
					{
						if (this.messageBoxWindow != null)
						{
							Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
						}
						this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0], text)
						}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
						if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
						{
							this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
						}
						this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
							if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
							{
								Global.SetZuanShi(ZuanShiPartClass.RidePetChouQu, !this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
							}
							Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								this.StartTimeOnClick();
								GameInstance.Game.GetRidePetChouQu(0, ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0]);
							}
							return true;
						};
						return;
					}
					this.StartTimeOnClick();
					GameInstance.Game.GetRidePetChouQu(0, ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0]);
				}
			}
		};
		this.m_BtnTen.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "ZuoQiBuHuo", ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1]);
			if (this.mStartOnClick)
			{
				if (Global.GetRoleOwnNumByMoneyType(163) < ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1] && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ZuoQiBuHuo", ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1], true))
				{
					IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1] - Global.GetRoleOwnNumByMoneyType(163);
					string lang = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang(string.Format(lang, IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi))
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							GameInstance.Game.SendBoCaiDaiBi(IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi, 2);
						}
						return true;
					};
					return;
				}
				if (Global.GetZuanShi(ZuanShiPartClass.RidePetChouQu))
				{
					if (this.messageBoxWindow != null)
					{
						Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
					}
					this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1], text)
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
					{
						this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
					}
					this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
						if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
						{
							Global.SetZuanShi(ZuanShiPartClass.RidePetChouQu, !this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
						}
						Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							this.StartTimeOnClick();
							GameInstance.Game.GetRidePetChouQu(1, ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1]);
						}
						return true;
					};
					return;
				}
				this.StartTimeOnClick();
				GameInstance.Game.GetRidePetChouQu(1, ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1]);
			}
		};
		if (this.m_BtnOpenBangZhuPanel != null)
		{
			this.m_BtnOpenBangZhuPanel.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.m_PanelBangZhu.gameObject.SetActive(true);
			};
		}
		if (this.m_BtnCloseBangZhuPanel != null)
		{
			this.m_BtnCloseBangZhuPanel.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.m_PanelBangZhu.gameObject.SetActive(false);
			};
		}
	}

	private void InitValue()
	{
		if (ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',') != null)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',');
			this.m_LabZuanShLeft.text = ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0].ToString();
			this.m_LabZuanShiRight.text = ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1].ToString();
		}
		else
		{
			this.m_LabZuanShLeft.text = string.Empty;
			this.m_LabZuanShiRight.text = string.Empty;
		}
	}

	public void GetData(ZuoQiMainData data)
	{
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.baoshiOne, "ZuoQiBuHuo", ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0], "xingyunzhixing");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.baoshiTen, "ZuoQiBuHuo", ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1], "xingyunzhixing");
		this.m_Time = data.NextFreeTime;
		DateTime nextFreeTime = data.NextFreeTime;
		if (nextFreeTime > Global.GetCorrectDateTime())
		{
			if (!NGUITools.GetActive(base.gameObject))
			{
				return;
			}
			base.StartCoroutine<bool>(this.TimCorutine(nextFreeTime));
		}
		else
		{
			base.StopCoroutine("TimCorutine");
			this.m_LabTitme.text = string.Empty;
			this.m_LabZuanShLeft.text = Global.GetLang("免费");
		}
	}

	private IEnumerator StarChouQu(ZuoQiChouQuResult chouQuData)
	{
		if (this.mHorseLieQuGoodsShowPart == null)
		{
			this._TeXiaoJuNeng.SetActive(false);
			this._TeXiaoJuNeng.SetActive(true);
			this.StartTimeOnClick();
			yield return new WaitForSeconds(2.2f);
		}
		this.ShowHorseLieQuGoodsShowPart(chouQuData);
		yield break;
	}

	public void GetRefreshChouQu(ZuoQiChouQuResult chouQuData)
	{
		if (!NGUITools.GetActive(base.gameObject))
		{
			return;
		}
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.baoshiOne, "ZuoQiBuHuo", ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0], "xingyunzhixing");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.baoshiTen, "ZuoQiBuHuo", ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1], "xingyunzhixing");
		if (chouQuData.Result != 0)
		{
			Super.HintMainText(Global.GetLang(ZuoQiActionResultTypeErr.GetErrMsg(chouQuData.Result, false, false)), 10, 3);
		}
		else
		{
			this.m_Time = chouQuData.FreeTime;
			if (this.m_Time > Global.GetCorrectDateTime())
			{
				base.StartCoroutine<bool>(this.TimCorutine(this.m_Time));
				this.m_LabZuanShLeft.text = ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0].ToString();
			}
			else
			{
				base.StopCoroutine("TimCorutine");
				this.m_LabTitme.text = string.Empty;
				this.m_LabZuanShLeft.text = Global.GetLang("免费");
			}
			base.StartCoroutine<bool>(this.StarChouQu(chouQuData));
		}
		this.m_LabZuanShi.text = ((!IConfigbase<ConfigXingYunXingShiYong>.Instance.XingYunXingKaiGuan("ZuoQiBuHuo")) ? Global.GetRoleOwnNumByMoneyType(40).ToString() : Global.GetRoleOwnNumByMoneyType(163).ToString());
		this.m_LabJinFen.text = Global.GetRoleOwnNumByMoneyType(140).ToString();
	}

	private void AddTitle(List<GoodsData> list)
	{
		this.m_Collection.Clear();
		if (list != null)
		{
			Vector3[] array = new Vector3[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				Super.AddGoodsIcon(list[i], this.m_Collection, false, true);
			}
			for (int j = 0; j < this.m_Collection.Count; j++)
			{
				array[j] = new Vector3((float)(80 * j), 0f, -0.5f);
			}
			base.StartCoroutine<bool>(this.TitleCorutine(array));
		}
	}

	private IEnumerator TimCorutine(DateTime datetTime)
	{
		long time = datetTime.Ticks / 10000L - Global.GetCorrectLocalTime();
		for (;;)
		{
			time -= 1000L;
			if (time < 86400000L)
			{
				UILabel labTitme = this.m_LabTitme;
				object[] array = new object[2];
				array[0] = "17e43e";
				int num = 1;
				DateTime dateTime;
				dateTime..ctor(time * 10000L);
				array[num] = dateTime.ToString("HH:mm:ss") + Global.GetLang("后免费");
				labTitme.text = Global.GetColorStringForNGUIText(array);
			}
			else
			{
				UILabel labTitme2 = this.m_LabTitme;
				object[] array2 = new object[2];
				array2[0] = "17e43e";
				int num2 = 1;
				DateTime dateTime2;
				dateTime2..ctor(time * 10000L);
				array2[num2] = dateTime2.ToString("dd HH:mm:ss") + Global.GetLang("后免费");
				labTitme2.text = Global.GetColorStringForNGUIText(array2);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	private IEnumerator TitleCorutine(Vector3[] vecs)
	{
		for (;;)
		{
			this.m_TimeNumber++;
			if (this.m_TimeNumber >= vecs.Length)
			{
				this.m_TimeNumber = 0;
			}
			yield return new WaitForSeconds(0.3f);
		}
		yield break;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		base.StopCoroutine("TimCorutine");
		base.StopCoroutine("TitleCorutine");
	}

	private const float ShowGoodsSpeed = 1f;

	public GButton m_BtnClose;

	public GButton m_BtnChongZhi;

	public UILabel m_LabZuanShi;

	public UILabel m_LabJinFen;

	public Transform _GoodsRoot;

	public GButton m_BtnJiFen;

	public GButton m_BtnCangKu;

	public GButton m_BtnOne;

	public GButton m_BtnTen;

	public GButton m_BtnBangZhu;

	public UILabel m_LabZuanShLeft;

	public UILabel m_LabZuanShiRight;

	public UILabel m_LabTitme;

	public UISprite baoshiOne;

	public UISprite baoshiTen;

	public UIPanel m_PanelBangZhuList;

	private UIDraggablePanel mDragPanelContent;

	public UIFont m_Font;

	public UIPanel m_PanelBangZhu;

	public GButton m_BtnOpenBangZhuPanel;

	public GButton m_BtnCloseBangZhuPanel;

	public UILabel m_LabBangZhuContent;

	public UILabel m_LabBangZhuTitle;

	[SerializeField]
	private Transform mTexiaoRoot;

	[SerializeField]
	private AnimationClip _GoodsShowCilp;

	[SerializeField]
	private UILabel _CangKuNumber;

	[SerializeField]
	private GameObject _TeXiaoJuNeng;

	public ObservableCollection m_Collection;

	private int m_TimeNumber;

	private DateTime m_Time;

	public DPSelectedItemEventHandler m_DpsHandler;

	private HorseLieQuGoodsShowPart mHorseLieQuGoodsShowPart;

	private GChildWindow mHorseLieQuGoodsShowPartWind;

	private Animation[] mGoodsShowAnimationArray;

	private TweenPosition[] mGoodsShowTweenPositionArray;

	private bool HaveLoadAllGoodsShow;

	private List<int> mHaveActiviteList;

	private float mLastX;

	private bool mStartOnClick = true;

	private RollAinmationControl rc;

	private GChildWindow messageBoxWindow;

	public UISprite iconXingYunXing;

	private float[] mAinmationRunTime;

	private GChildWindow mDuiHuanWindow;

	private MUDuiHuanPart mDuiHuanPart;

	private float mTimeSatr;
}
