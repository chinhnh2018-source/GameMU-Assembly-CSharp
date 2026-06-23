using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class HorseLieQuGoodsShowPart : UserControl
{
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
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.m_GameDongHua.SetActive(true);
		U3DUtils.ReplaceLayerInChildren(this.m_GameDongHua, LayerMask.NameToLayer("MUUI"), null);
	}

	public override void Update()
	{
		base.Update();
		if (this.mCD > 0f)
		{
			this.mCD -= Time.deltaTime;
		}
	}

	private void ShowHorseLieQuChengPinShowPart(GoodsData Goodsdata)
	{
		if (IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(Goodsdata.GoodsID) != null)
		{
			if (null != this.mHorseLieQuChengPinShowPartWind)
			{
				this.mHorseLieQuChengPinShowPartWind.Visibility = true;
			}
			else
			{
				this.mHorseLieQuChengPinShowPartWind = U3DUtils.NEW<GChildWindow>();
				this.mHorseLieQuChengPinShowPartWind.Modal = true;
				this.mHorseLieQuChengPinShowPartWind.ModalType = ChildWindowModalType.Translucent2;
				Global.MainStage.Add(this.mHorseLieQuChengPinShowPartWind);
				WindowManage.AddWindows(this.mHorseLieQuChengPinShowPartWind, true, null);
			}
			this.mHorseLieQuChengPinShowPart = U3DUtils.NEW<HorseLieQuChengPinShowPart>();
			this.mHorseLieQuChengPinShowPart.HaveActiviteGoodsList = this.mHaveActiviteList;
			this.mHorseLieQuChengPinShowPart.Show3DModal(Goodsdata);
			this.mHorseLieQuChengPinShowPartWind.Body.Add(this.mHorseLieQuChengPinShowPart);
			this.mHorseLieQuChengPinShowPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s.ID == 1)
				{
					this.m_Falg = false;
					MUDebug.Log<string>(new string[]
					{
						"继续播放1状态：" + this.m_Falg
					});
				}
				else if (s.ID == 2)
				{
					this.m_Falg = false;
					MUDebug.Log<string>(new string[]
					{
						"继续播放2状态：" + this.m_Falg
					});
					if (this.mShowDataList != null && 0 < this.mShowDataList.Count)
					{
						for (int i = 0; i < this.mShowDataList.Count; i++)
						{
							if (!this.mShowDataList[i].HaveShow)
							{
								base.StartCoroutine<bool>(this.ChouQuCorutine(false));
								break;
							}
						}
					}
				}
				if (s.Type == 0)
				{
					this.CloseHorseLieQuChengPinShowPart();
				}
			};
		}
	}

	private void CloseHorseLieQuChengPinShowPart()
	{
		if (null != this.mHorseLieQuChengPinShowPart)
		{
			Object.Destroy(this.mHorseLieQuChengPinShowPart.gameObject);
			this.mHorseLieQuChengPinShowPart = null;
		}
		if (null != this.mHorseLieQuChengPinShowPartWind)
		{
			WindowManage.RemoveWindows(this.mHorseLieQuChengPinShowPartWind);
			Object.Destroy(this.mHorseLieQuChengPinShowPartWind.gameObject);
			this.mHorseLieQuChengPinShowPartWind = null;
		}
	}

	private void InitPrefabText()
	{
		try
		{
			this.mBtnGetTenTimes.Label.text = Global.GetLang("抽取十次");
			this.mBtnSure.Text = Global.GetLang("确定");
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
			this.mBtnSure.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				MUDebug.Log<string>(new string[]
				{
					"确定按钮状态：" + this.m_Falg
				});
				if (this.mCD > 0f || this.m_Falg)
				{
					return;
				}
				this.mCD = 0.8f;
				if (this.mShowDataList != null && 0 < this.mShowDataList.Count)
				{
					int i = 0;
					while (i < this.mShowDataList.Count)
					{
						if (!this.mShowDataList[i].HaveShow)
						{
							if (1 > this.mSureBtnClickCount++)
							{
								return;
							}
							break;
						}
						else
						{
							i++;
						}
					}
				}
				if (this.Hander != null)
				{
					this.CloseHorseLieQuChengPinShowPart();
					this.Hander(this, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
			this.mBtnGetTenTimes.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				MUDebug.Log<string>(new string[]
				{
					"十连抽按钮状态：" + this.m_Falg
				});
				if (this.mCD > 0f || this.m_Falg)
				{
					return;
				}
				this.mCD = 2.5f;
				if (this.mShowDataList != null && 0 < this.mShowDataList.Count)
				{
					for (int i = 0; i < this.mShowDataList.Count; i++)
					{
						if (!this.mShowDataList[i].HaveShow && this.mShowDataList[i].time + 2000L < Global.GetCorrectLocalTime())
						{
							Super.HintMainText(Global.GetLang("当前召唤尚未结束"), 10, 3);
							return;
						}
					}
				}
				int num = (this.mShowDataList.Count != 1) ? ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1] : ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0];
				if (Global.GetRoleOwnNumByMoneyType(163) < num && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ZuoQiBuHuo", num, true))
				{
					IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = num - Global.GetRoleOwnNumByMoneyType(163);
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
					string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "ZuoQiBuHuo", num);
					this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), num, text)
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
							if (this.mShowDataList.Count == 10)
							{
								GameInstance.Game.GetRidePetChouQu(1, ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1]);
							}
							else if (this.mShowDataList.Count == 1)
							{
								GameInstance.Game.GetRidePetChouQu(0, ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0]);
							}
						}
						return true;
					};
					return;
				}
				if (this.mShowDataList.Count == 10)
				{
					GameInstance.Game.GetRidePetChouQu(1, ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[1]);
				}
				else if (this.mShowDataList.Count == 1)
				{
					GameInstance.Game.GetRidePetChouQu(0, ConfigSystemParam.GetSystemParamIntArrayByName("HorsePay", ',')[0]);
				}
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

	private GameObject GetGoodsTeXiao()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this._GoodsTeXiao);
		gameObject.SetActive(true);
		gameObject.transform.localPosition = Vector3.zero;
		return gameObject;
	}

	protected override void OnDestroy()
	{
		if (this.messageBoxWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
		}
		base.OnDestroy();
	}

	private GGoodIcon AddGoodsIcon(GoodsData gd, GameObject parent, bool grayShow = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.BackSpriteName0 = backSpriteName;
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = gd.GoodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = -1;
			if (!grayShow)
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			icon.gameObject.AddComponent<UIPanel>();
			this.m_ListChouJiang.Add(icon);
			icon.transform.SetParent(parent.transform, false);
			icon.gameObject.AddComponent<UIDragPanelContents>();
			icon.addEventListener("click", delegate(MouseEvent s)
			{
				GGoodIcon ggoodIcon = s.target.SafeGetComponent<GGoodIcon>();
				if (null == ggoodIcon)
				{
					return;
				}
				GoodsData goodsData = icon.ItemObject as GoodsData;
				if (goodsData == null)
				{
					return;
				}
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			});
			return icon;
		}
		return null;
	}

	private IEnumerator WaiteTimeDoHander(float waiteTime, DPSelectedItemEventHandler Hander)
	{
		yield return new WaitForSeconds(waiteTime);
		if (Hander != null)
		{
			Hander(null, null);
		}
		yield break;
	}

	private IEnumerator ChouQuCorutine(bool Wait = false)
	{
		if (Wait)
		{
			yield return new WaitForSeconds(0.1f);
			this.mFirstOpenWin = false;
		}
		this.m_Falg = false;
		float duration = 0.15f;
		if (this.mShowDataList == null || 0 >= this.mShowDataList.Count)
		{
			yield break;
		}
		for (int i = 0; i < this.mShowDataList.Count; i++)
		{
			if (!this.mShowDataList[i].HaveShow)
			{
				GGoodIcon icon = this.AddGoodsIcon(this.mShowDataList[i].GGoodsData, this.m_Game, false);
				TweenPosition tweenPosition = icon.GetComponent<TweenPosition>();
				TweenRotation tweenRotation = icon.GetComponent<TweenRotation>();
				TweenScale tweenScale = icon.GetComponent<TweenScale>();
				if (tweenPosition == null)
				{
					tweenPosition = icon.gameObject.AddComponent<TweenPosition>();
				}
				if (tweenRotation == null)
				{
					tweenRotation = icon.gameObject.AddComponent<TweenRotation>();
				}
				if (tweenScale == null)
				{
					tweenScale = icon.gameObject.AddComponent<TweenScale>();
				}
				tweenPosition.Reset();
				tweenRotation.Reset();
				tweenScale.Reset();
				tweenPosition = icon.gameObject.GetComponent<TweenPosition>();
				tweenPosition.duration = duration;
				tweenPosition.from = new Vector3(0f, 200f, -1f);
				tweenPosition.to = this.mShowDataList[i].Pos;
				tweenRotation = icon.gameObject.GetComponent<TweenRotation>();
				tweenRotation.duration = duration;
				tweenRotation.from = new Vector3(0f, 0f, 180f);
				tweenRotation.to = new Vector3(0f, 0f, 360f);
				tweenScale = icon.gameObject.GetComponent<TweenScale>();
				tweenScale.duration = duration;
				tweenScale.from = new Vector3(0f, 0f, 0f);
				tweenScale.to = new Vector3(1f, 1f, 1f);
				tweenPosition.Play(true);
				tweenRotation.Play(true);
				tweenScale.Play(true);
				yield return new WaitForSeconds(duration);
				int type = Global.GetCategoriyByGoodsID(this.mShowDataList[i].GGoodsData.GoodsID);
				if (type == 340)
				{
					GameObject TeXiaoObj = this.GetGoodsTeXiao();
					U3DUtils.AddChild(icon.gameObject, TeXiaoObj, true);
				}
				else if (type >= 40 && type <= 45)
				{
					if (3 <= Super.GetHorseQuality(this.mShowDataList[i].GGoodsData))
					{
						GameObject TeXiaoObj2 = this.GetGoodsTeXiao();
						U3DUtils.AddChild(icon.gameObject, TeXiaoObj2, true);
					}
				}
				else
				{
					int level = Super.GetGoodsQuality(this.mShowDataList[i].GGoodsData.GoodsID);
					if (level >= 3)
					{
						GameObject TeXiaoObj3 = this.GetGoodsTeXiao();
						U3DUtils.AddChild(icon.gameObject, TeXiaoObj3, true);
					}
				}
				this.mShowDataList[i].HaveShow = true;
				this.mShowDataList[i].time = Global.GetCorrectLocalTime();
				yield return new WaitForSeconds(0.25f);
				if (IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(this.mShowDataList[i].GGoodsData.GoodsID) != null)
				{
					this.m_Falg = true;
					MUDebug.Log<string>(new string[]
					{
						"播放动画时状态：" + this.m_Falg
					});
					this.ShowHorseLieQuChengPinShowPart(this.mShowDataList[i].GGoodsData);
					yield break;
				}
			}
		}
		yield break;
	}

	private void RefreshGetBtn()
	{
		if (this.mShowDataList.Count == 10)
		{
			this.mBtnGetTenTimes.Label.text = Global.GetLang("抽取十次");
		}
		else if (this.mShowDataList.Count == 1)
		{
			this.mBtnGetTenTimes.Label.text = Global.GetLang("抽取一次");
		}
	}

	private void PlayAnimation(byte type = 0)
	{
	}

	public void RefreshChouQu(ZuoQiChouQuResult chouQuData)
	{
		Transform[] componentsInChildren = this.m_Game.GetComponentsInChildren<Transform>();
		if (0 < componentsInChildren.Length)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (null != componentsInChildren[i])
				{
					if (!(componentsInChildren[i] == this.m_Game.transform))
					{
						Object.Destroy(componentsInChildren[i].gameObject);
					}
				}
			}
		}
		this.PlayAnimation(0);
		if (chouQuData != null)
		{
			string[] array = chouQuData.GoodsList.Split(new char[]
			{
				','
			});
			if (array != null)
			{
				List<GoodsData> list = new List<GoodsData>();
				for (int j = 0; j < array.Length; j++)
				{
					if (!string.IsNullOrEmpty(array[j]))
					{
						for (int k = 0; k < Global.Data.roleData.MountStoreList.Count; k++)
						{
							GoodsData goodsData = Global.Data.roleData.MountStoreList[k];
							if (goodsData != null)
							{
								if (goodsData.Id == array[j].SafeToInt32(0))
								{
									list.Add(goodsData);
									break;
								}
							}
						}
					}
				}
				this.mShowDataList.Clear();
				int num = 0;
				if (list.Count == 1)
				{
					HorseLieQuGoodsShowPart.HorseGoodsShowData horseGoodsShowData = new HorseLieQuGoodsShowPart.HorseGoodsShowData();
					horseGoodsShowData.GGoodsData = list[0];
					horseGoodsShowData.Pos = new Vector3(0f, 0f, -1f);
					horseGoodsShowData.HaveShow = false;
					this.mShowDataList.Add(horseGoodsShowData);
				}
				else
				{
					for (int l = 0; l < 2; l++)
					{
						for (int m = 0; m < 5; m++)
						{
							HorseLieQuGoodsShowPart.HorseGoodsShowData horseGoodsShowData2 = new HorseLieQuGoodsShowPart.HorseGoodsShowData();
							horseGoodsShowData2.GGoodsData = list[num++];
							horseGoodsShowData2.Pos = new Vector3((float)(-220 + m * 110), (float)(l * -120), -1f);
							horseGoodsShowData2.HaveShow = false;
							this.mShowDataList.Add(horseGoodsShowData2);
						}
					}
				}
				this.RefreshGetBtn();
				base.StartCoroutine<bool>(this.ChouQuCorutine(true));
			}
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private GameObject m_Game;

	[SerializeField]
	private GameObject m_GameDongHua;

	[SerializeField]
	private GameObject _GoodsTeXiao;

	[SerializeField]
	private GButton mBtnSure;

	[SerializeField]
	private GButton mBtnGetTenTimes;

	private List<HorseLieQuGoodsShowPart.HorseGoodsShowData> mShowDataList = new List<HorseLieQuGoodsShowPart.HorseGoodsShowData>();

	private int mSureBtnClickCount;

	private bool m_Falg;

	private List<GGoodIcon> m_ListChouJiang = new List<GGoodIcon>();

	private HorseLieQuChengPinShowPart mHorseLieQuChengPinShowPart;

	private GChildWindow mHorseLieQuChengPinShowPartWind;

	private bool mFirstOpenWin = true;

	private List<int> mHaveActiviteList;

	private float mCD = 0.8f;

	private GChildWindow messageBoxWindow;

	private class HorseGoodsShowData
	{
		public GoodsData GGoodsData;

		public Vector3 Pos;

		public bool HaveShow;

		public long time;
	}
}
