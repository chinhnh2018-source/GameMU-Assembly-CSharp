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

public class ShiPinPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		base.StartCoroutine<bool>(this.StartInitData());
		base.StartCoroutine<bool>(this.CarryHanderWaitForSeconds(new ShiPinPart.voidDelegate(Super.HideNetWaiting), 0.5f));
		Super.ShowNetWaiting(null);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Super.HideNetWaiting();
	}

	private IEnumerator CarryHanderWaitForSeconds(ShiPinPart.voidDelegate hander, float time)
	{
		yield return new WaitForSeconds(time);
		hander();
		yield break;
	}

	private IEnumerator StartInitData()
	{
		if (this.m_OrnamentXmlConfig == null)
		{
			yield return null;
		}
		this.m_OrnamentXmlConfig = new OrnamentXmlConfig();
		base.StartCoroutine<bool>(this.CarryHanderWaitForSeconds(new ShiPinPart.voidDelegate(this.GetData), 0.1f));
		yield break;
	}

	private void GetData()
	{
		GameInstance.Game.SendGetShiPinData();
	}

	private void InitPage()
	{
		if (null != this.m_PagesOne)
		{
			Object.Destroy(this.m_PagesOne.gameObject);
		}
		if (null != this.m_PagesTwo)
		{
			Object.Destroy(this.m_PagesTwo.gameObject);
		}
		this.m_PagesOne = U3DUtils.NEW<ShiPinPageOne>();
		this.m_PagesOne.transform.SetParent(this._PageRoot, false);
		this.m_PagesOne.Hander = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (s != null)
			{
				if (s.IDType == 2)
				{
					this.ShowShiPinLoadPart(s.Index, s.Flag == 1, s.Level);
				}
				else if (s.IDType == 1)
				{
					this.SendUnLoadShiPIn(s.Index);
				}
			}
		};
		this.m_PagesTwo = U3DUtils.NEW<ShiPinPageTwo>();
		this.m_PagesTwo.transform.SetParent(this._PageRoot, false);
	}

	private void InitPrefabText()
	{
		for (int i = 0; i < this._Btns.Length; i++)
		{
			if (null != this._Btns[i].Label)
			{
				this._Btns[i].Label.text = ((i != 0) ? Global.GetLang("饰品预览") : Global.GetLang("饰品佩戴"));
			}
		}
		this.RefreshRoleMoney();
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		if (null != this._Closebtn)
		{
			this._Closebtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						IDType = 0
					});
				}
			};
		}
		if (this._Btns != null)
		{
			this.mTopbtns = new ShiPinPart.BtnHanders[this._Btns.Length];
			for (int i = 0; i < this._Btns.Length; i++)
			{
				this.mTopbtns[i] = new ShiPinPart.BtnHanders(this._Btns[i], i);
				this.mTopbtns[i].Hander = delegate(object f, DPSelectedItemEventArgs s)
				{
					this.MouseLeftButtonUp(f, s);
				};
			}
		}
	}

	private void MouseLeftButtonUp(object sender, DPSelectedItemEventArgs args)
	{
		for (int i = 0; i < this.mTopbtns.Length; i++)
		{
			if (this.mTopbtns[i].ID == args.ID)
			{
				this.mTopbtns[i].BSelect = true;
				this.m_SelectIndex = this.mTopbtns[i].ID;
			}
			else
			{
				this.mTopbtns[i].BSelect = false;
			}
		}
		this.ChangePage(this.m_SelectIndex);
	}

	private void ChangePage(int index)
	{
		if (null == this.m_PagesOne)
		{
			this.InitPage();
		}
		if (null == this.m_PagesTwo)
		{
			this.InitPage();
		}
		if (null != this.m_PagesOne)
		{
			this.m_PagesOne.OnActive(0 == index);
		}
		if (null != this.m_PagesTwo)
		{
			this.m_PagesTwo.OnActive(1 == index);
		}
	}

	private void ShowShiPinLoadPart(int index, bool bLock, int Level)
	{
		if (!bLock)
		{
			List<GoodsData> roleDecorationList = Global.GetRoleDecorationList();
			int count = Super.ToInt((double)this.m_OrnamentXmlConfig.GetOrnamentXmlDataLst().Count / 3.0, 0) * 3;
			List<GoodsData> list = roleDecorationList.FindAll((GoodsData e) => e.Using != 1);
			if (0 < list.Count)
			{
				if (null != this.m_ShiPinLoadPartWindow)
				{
					this.m_ShiPinLoadPartWindow.Visibility = true;
				}
				else
				{
					this.m_ShiPinLoadPartWindow = U3DUtils.NEW<GChildWindow>();
					this.m_ShiPinLoadPartWindow.ModalType = ChildWindowModalType.Translucent;
					this.m_ShiPinLoadPartWindow.IsShowModal = true;
					Super.InitChildWindow(this.m_ShiPinLoadPartWindow, Global.GetLang("ShiPinLoadPartWindo"));
					this.Container.Children.Add(this.m_ShiPinLoadPartWindow);
				}
				UIEventListener.Get(this.m_ShiPinLoadPartWindow.ModalBak).onClick = delegate(GameObject g)
				{
					this.CloseShiPinLoad();
				};
				if (null != this.m_ShiPinLoadPart)
				{
					Object.Destroy(this.m_ShiPinLoadPart);
				}
				this.m_ShiPinLoadPart = U3DUtils.NEW<ShiPinLoadPart>();
				this.m_ShiPinLoadPartWindow.Body.Add(this.m_ShiPinLoadPart);
				this.m_ShiPinLoadPart.SelectCaoWeiLevel = Level;
				this.m_ShiPinLoadPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
				{
					if (s != null)
					{
						if (s.IDType == 0)
						{
							this.CloseShiPinLoad();
						}
						else if (s.IDType == 20)
						{
							this.SendLoadShiPin(s.ID, index);
						}
					}
					else
					{
						this.CloseShiPinLoad();
					}
				};
				this.m_ShiPinLoadPart.RefreshIcons(list, count);
			}
			else
			{
				Super.HintMainText(Global.GetLang("无可配佩戴的饰品"), 10, 3);
			}
		}
		else
		{
			Super.HintMainText(string.Format(Global.GetLang("需要饰品槽总等级达到{0}开启"), this.GetClearLock(index)), 10, 3);
		}
	}

	private int GetClearLock(int index)
	{
		if (this.m_Rabbte.Count == 0)
		{
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("OrnamentSiteOpen", '|');
			if (systemParamStringArrayByName != null && 0 < systemParamStringArrayByName.Length)
			{
				for (int i = 0; i < systemParamStringArrayByName.Length; i++)
				{
					if (!string.IsNullOrEmpty(systemParamStringArrayByName[i]))
					{
						string[] array = systemParamStringArrayByName[i].Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							this.m_Rabbte.Add(int.Parse(array[0]), int.Parse(array[1]));
						}
					}
				}
			}
		}
		index++;
		if (this.m_Rabbte.ContainsKey(index))
		{
			return this.m_Rabbte[index];
		}
		return 0;
	}

	private void CloseShiPinLoad()
	{
		Object.Destroy(this.m_ShiPinLoadPart);
		this.m_ShiPinLoadPart = null;
		Super.CloseChildWindow(this.Container, this.m_ShiPinLoadPartWindow);
	}

	private void SendLoadShiPin(int GoodsID, int Index)
	{
		Index = this.GetShiPinIndex(Index);
		List<GoodsData> roleDecorationList = Global.GetRoleDecorationList();
		GoodsData goodsData = roleDecorationList.Find((GoodsData s) => s.BagIndex == Index);
		if (goodsData != null)
		{
			this.SendUnLoadShiPIn(goodsData.GoodsID);
		}
		GoodsData goodsData2 = roleDecorationList.Find((GoodsData e) => e.GoodsID == GoodsID);
		if (goodsData2 != null)
		{
			GameInstance.Game.SpriteModGoods(1, goodsData2.Id, goodsData2.GoodsID, 1, 9000, goodsData2.GCount, Index, string.Empty);
		}
		Super.ShowNetWaiting(null);
	}

	private void SendUnLoadShiPIn(int Index)
	{
		Index = this.GetShiPinIndex(Index);
		List<GoodsData> roleDecorationList = Global.GetRoleDecorationList();
		GoodsData goodsData = roleDecorationList.Find((GoodsData s) => s.BagIndex == Index && s.Using == 1);
		if (goodsData != null && goodsData.Using == 1)
		{
			GameInstance.Game.SpriteModGoods(2, goodsData.Id, goodsData.GoodsID, 0, 9000, goodsData.GCount, goodsData.BagIndex, string.Empty);
			Super.ShowNetWaiting(null);
		}
	}

	private int GetShiPinIndex(int index)
	{
		return index + 1;
	}

	public void ShowPage(int Page)
	{
		this.MouseLeftButtonUp(this._Btns[Page], new DPSelectedItemEventArgs
		{
			ID = Page
		});
	}

	public void RefreshUp(string[] field)
	{
		if ("0" == field[0])
		{
			this.m_PagesOne.RefreshShiPinIcon(int.Parse(field[2]), int.Parse(field[3]));
		}
		else
		{
			ShiPinPart.ErrorLog(int.Parse(field[0]));
		}
		this.RefreshRoleMoney();
	}

	public static void ErrorLog(int ret)
	{
		if (ret != 0)
		{
			string errMsg = StdErrorCode.GetErrMsg(ret, false, false);
			Super.HintMainText(Global.GetLang(errMsg), 10, 3);
		}
	}

	public void RefreshRoleMoney()
	{
		int num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.CharmPoint);
		if (0 >= num)
		{
			num = 0;
		}
		this._Moeny.text = num.ToString();
	}

	public void RefreshIcons(SCModGoods ModGoods)
	{
		GoodsData goodsData = Global.GetRoleDecorationList().Find((GoodsData e) => e.Id == ModGoods.ID);
		if (goodsData != null)
		{
			goodsData.GCount = ModGoods.Count;
			goodsData.BagIndex = ModGoods.BagIndex;
			goodsData.Using = ModGoods.IsUsing;
			if (null != this.m_PagesOne)
			{
				this.m_PagesOne.RefreshShiPinIcon(goodsData);
			}
		}
	}

	public void RefreshShiPinLoadPart()
	{
		this.CloseShiPinLoad();
	}

	public void RefreshData(Dictionary<int, OrnamentData> data_ = null)
	{
		if (data_ != null)
		{
			List<OrnamentData> list = new List<OrnamentData>();
			List<OrnamentData> list2 = new List<OrnamentData>();
			foreach (KeyValuePair<int, OrnamentData> keyValuePair in data_)
			{
				if (keyValuePair.Value != null)
				{
					int num = 6;
					Dictionary<int, OrnamentData>.Enumerator enumerator;
					KeyValuePair<int, OrnamentData> keyValuePair2 = enumerator.Current;
					if (num > keyValuePair2.Key)
					{
						int num2 = 1;
						KeyValuePair<int, OrnamentData> keyValuePair3 = enumerator.Current;
						if (num2 <= keyValuePair3.Key)
						{
							Dictionary<int, OrnamentData> shiPinPosData = this.m_ShiPinPosData;
							KeyValuePair<int, OrnamentData> keyValuePair4 = enumerator.Current;
							int key = keyValuePair4.Key;
							KeyValuePair<int, OrnamentData> keyValuePair5 = enumerator.Current;
							shiPinPosData.Add(key, keyValuePair5.Value);
							List<OrnamentData> list3 = list;
							KeyValuePair<int, OrnamentData> keyValuePair6 = enumerator.Current;
							list3.Add(keyValuePair6.Value);
							continue;
						}
					}
					int num3 = 0;
					KeyValuePair<int, OrnamentData> keyValuePair7 = enumerator.Current;
					if (num3 < keyValuePair7.Key)
					{
						Dictionary<int, OrnamentData> shiPinCanActivedata = this.m_ShiPinCanActivedata;
						KeyValuePair<int, OrnamentData> keyValuePair8 = enumerator.Current;
						int key2 = keyValuePair8.Key;
						KeyValuePair<int, OrnamentData> keyValuePair9 = enumerator.Current;
						shiPinCanActivedata.Add(key2, keyValuePair9.Value);
						List<OrnamentData> list4 = list2;
						KeyValuePair<int, OrnamentData> keyValuePair10 = enumerator.Current;
						list4.Add(keyValuePair10.Value);
					}
				}
			}
			this.m_PagesOne.RefreshShiPinIcons(list);
			this.m_PagesTwo.RefreshShiPinIcons(list2, this.m_OrnamentXmlConfig);
		}
	}

	public void RefreshUnLoadData(SCModGoods ModGoods)
	{
		GoodsData goodsData = Global.GetRoleDecorationList().Find((GoodsData e) => e.Id == ModGoods.ID);
		if (goodsData != null)
		{
			goodsData.GCount = ModGoods.Count;
			goodsData.BagIndex = ModGoods.BagIndex;
			goodsData.Using = ModGoods.IsUsing;
			if (null != this.m_PagesOne)
			{
				this.m_PagesOne.RefreshShiPinIcon(goodsData);
			}
		}
	}

	public void RefreshShiPinActive(List<GoodsData> data)
	{
		if (null != this.m_PagesTwo)
		{
			this.m_PagesTwo.RefreshShiPinActive();
		}
	}

	public static string CutDoubleValue2(double value)
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

	public static int ToInt(double value)
	{
		return Mathf.FloorToInt((float)value);
	}

	public GButton[] _Btns;

	public GButton _Closebtn;

	public Transform _PageRoot;

	public UILabel _Moeny;

	private int m_SelectIndex;

	private ShiPinPageOne m_PagesOne;

	private ShiPinPageTwo m_PagesTwo;

	private OrnamentXmlConfig m_OrnamentXmlConfig;

	private GChildWindow m_ShiPinLoadPartWindow;

	private ShiPinLoadPart m_ShiPinLoadPart;

	private Dictionary<int, OrnamentData> m_ShiPinPosData = new Dictionary<int, OrnamentData>();

	private Dictionary<int, OrnamentData> m_ShiPinCanActivedata = new Dictionary<int, OrnamentData>();

	private ShiPinPart.BtnHanders[] mTopbtns;

	private Dictionary<int, int> m_Rabbte = new Dictionary<int, int>();

	[HideInInspector]
	public DPSelectedItemEventHandler Hander;

	private class BtnHanders
	{
		public BtnHanders(GButton btn, int ID)
		{
			this.mBtn = btn;
			this.mId = ID;
			this.mBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this.mBtn, new DPSelectedItemEventArgs
					{
						ID = this.mId
					});
				}
			};
		}

		private void ChangeGBtnBgAndColor(GButton btn, bool bCheck)
		{
			string text = (!bCheck) ? "BtnBg1" : "BtnBg0";
			btn.hoverSprite = text;
			btn.pressedSprite = text;
			btn.normalSprite = text;
			btn.disabledSprite = text;
			btn.target.spriteName = text;
			if (null != btn.Label)
			{
				string text2 = Super.ClearStringColor(btn.Label.text);
				btn.Label.text = ((!bCheck) ? Global.GetColorStringForNGUIText(new object[]
				{
					"808081",
					text2
				}) : Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					text2
				}));
			}
		}

		public bool BSelect
		{
			get
			{
				return this.mBSelect;
			}
			set
			{
				this.mBSelect = value;
				this.ChangeGBtnBgAndColor(this.mBtn, this.mBSelect);
			}
		}

		public int ID
		{
			get
			{
				return this.mId;
			}
		}

		public DPSelectedItemEventHandler Hander;

		private GButton mBtn;

		private int mId;

		private bool mBSelect;
	}

	public delegate void voidDelegate();
}
