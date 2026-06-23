using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class RebirthMainPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.InitBtns();
		this.InitMap();
		this.mRebornBossVOOne = IConfigbase<ConfigRebirth>.Instance.GetRebornBossVOByID(1);
	}

	private void InitMap()
	{
		if (null != this._MapObj)
		{
			for (int i = 0; i < 1; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this._MapObj);
				gameObject.name = ((RebirthType)i).ToString();
				gameObject.transform.SetParent(this._MapRoot, false);
				RebirthMainPart.MapHander mapHander = new RebirthMainPart.MapHander(gameObject, i);
				mapHander.OnClick = new RebirthMainPart.MapHanderDelegate(this.MapOnClick);
				this.mMapItems.Add(mapHander);
			}
		}
	}

	private void MapOnClick(RebitrhMapType btnType)
	{
		for (int i = 0; i < this.mMapItems.Count; i++)
		{
		}
		if (btnType == RebitrhMapType.RebitrhMap)
		{
			PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, this.mRebornBossVOOne.MapID, -1, -1, true, 0, 0, false, false);
		}
	}

	private void OpenHelpWindow(string path)
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
		XElement gameResXml = Global.GetGameResXml("GameRes/Config/RebornIntro.xml");
		if (gameResXml == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				string.Format(Global.GetLang("加载{0}出现错误"), path)
			});
		}
		ChangeableRulePart.RuleXml ruleXml = new ChangeableRulePart.RuleXml(gameResXml);
		this.m_helpPart.SetHelpInfo(ruleXml.list, true);
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

	private void InitBtns()
	{
		this._BtnExample.SetActive(false);
		for (int i = 1; i < 7; i++)
		{
			if ((double)Global.VersionCode >= 8.0 || i != 6)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this._BtnExample);
				gameObject.name = ((RebirthType)i).ToString();
				gameObject.transform.SetParent(this._ItemsBtnRoot, false);
				RebirthMainPart.BtnHander btnHander = new RebirthMainPart.BtnHander(gameObject);
				btnHander.BtnClick = new RebirthMainPart.BtnHanderDelegate(this.BtnHanderClick);
				btnHander.onPress = new RebirthMainPart.BtnHanderDelegate(this.BtnHanderOnPress);
				btnHander.Select = false;
				btnHander.RefreshLock();
				this.mBtnItems.Add(btnHander);
				this._BtnItemRoot.ItemsSource.AddNoUpdate(gameObject);
			}
		}
		this._BtnItemRoot.repositionNow = true;
	}

	private void BtnHanderOnPress(RebirthType btnType)
	{
		if (btnType != RebirthType.RebirthMap)
		{
			for (int i = 0; i < this.mBtnItems.Count; i++)
			{
				if (this.mBtnItems[i] != null)
				{
					if (btnType == this.mBtnItems[i].Type)
					{
						this.mBtnItems[i].Select = true;
					}
					else
					{
						this.mBtnItems[i].Select = false;
					}
				}
			}
		}
	}

	private void BtnHanderClick(RebirthType btnType)
	{
		if (btnType != RebirthType.RebirthMap)
		{
			for (int i = 0; i < this.mBtnItems.Count; i++)
			{
				if (this.mBtnItems[i] != null)
				{
					if (btnType == this.mBtnItems[i].Type)
					{
						this.mBtnItems[i].Select = true;
					}
					else
					{
						this.mBtnItems[i].Select = false;
					}
				}
			}
		}
		if (btnType == RebirthType.RebirthMap)
		{
			PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, this.mRebornBossVOOne.MapID, -1, -1, true, 0, 0, false, false);
		}
		if (this.Hander != null)
		{
			this.Hander(this, new DPSelectedItemEventArgs
			{
				Type = 1,
				ID = (int)btnType
			});
		}
	}

	private void InitPrefabText()
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
			this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
			this._PropBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.mPropType = 0;
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendGetRoleAllRib();
			};
			this._RebirthPropBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.mPropType = 1;
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendGetRoleAllRib();
			};
			this._HelpBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.OpenHelpWindow(string.Empty);
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

	private string[] GetAllPropertyStr(double[] attArray)
	{
		bool flag = this.mPropType == 1;
		string[] array = new string[]
		{
			(!flag) ? Global.GetLang("总属性预览") : Global.GetLang("重生总属性预览"),
			string.Empty
		};
		List<RebirthMainPart.PropertyStr> list = new List<RebirthMainPart.PropertyStr>();
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		for (int i = 1; i < ConfigExtPropIndexes.GetExtPropIndexesVOByWord("MAX").ID; i++)
		{
			if (i < attArray.Length)
			{
				double num = attArray[i];
				if (0.0 < num)
				{
					if (!flag)
					{
						if (!ConfigExtPropIndexes.GetExtPropIndexesIsRebirthPropByID(i))
						{
							if (dictionary.ContainsKey(i))
							{
								dictionary[i] = num;
							}
							else
							{
								dictionary.Add(i, num);
							}
						}
					}
					else if (ConfigExtPropIndexes.GetExtPropIndexesIsRebirthPropByID(i))
					{
						if (dictionary.ContainsKey(i))
						{
							dictionary[i] = num;
						}
						else
						{
							dictionary.Add(i, num);
						}
					}
				}
			}
		}
		foreach (KeyValuePair<int, double> keyValuePair in dictionary)
		{
			int key = keyValuePair.Key;
			int num2 = (!flag) ? ConfigExtPropIndexes.GetExtPropIndexesShowListByID(key) : ConfigExtPropIndexes.GetExtPropIndexesShowList2ByID(key);
			if (0 < num2)
			{
				RebirthMainPart.PropertyStr propertyStr = default(RebirthMainPart.PropertyStr);
				propertyStr.ShowList = num2;
				propertyStr.Str = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, false);
				propertyStr.Percent = ConfigExtPropIndexes.GetPercentByID(key);
				Dictionary<int, double>.Enumerator enumerator;
				KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
				propertyStr.Att = keyValuePair2.Value;
				list.Add(propertyStr);
			}
		}
		if (flag)
		{
			string[] array2 = array;
			int num3 = 1;
			string text = array2[num3];
			array2[num3] = string.Concat(new object[]
			{
				text,
				Global.GetLang("重生等级："),
				Global.Data.roleData.RebornLevel,
				Environment.NewLine
			});
			string[] array3 = array;
			int num4 = 1;
			array3[num4] = array3[num4] + Global.GetLang("每日重生经验上限") + Environment.NewLine;
			string[] array4 = array;
			int num5 = 1;
			text = array4[num5];
			array4[num5] = string.Concat(new string[]
			{
				text,
				Global.GetLang("怪物产出： "),
				(Global.Data.roleData.MoneyData[158] - Global.Data.roleData.MoneyData[148]).ToString(),
				"/",
				Global.Data.roleData.MoneyData[158].ToString(),
				Environment.NewLine
			});
			string[] array5 = array;
			int num6 = 1;
			text = array5[num6];
			array5[num6] = string.Concat(new string[]
			{
				text,
				Global.GetLang("装备回收："),
				(Global.Data.roleData.MoneyData[159] - Global.Data.roleData.MoneyData[149]).ToString(),
				"/",
				Global.Data.roleData.MoneyData[159].ToString(),
				Environment.NewLine,
				Environment.NewLine
			});
		}
		list.Sort((RebirthMainPart.PropertyStr a, RebirthMainPart.PropertyStr b) => a.ShowList - b.ShowList);
		for (int j = 0; j < list.Count; j++)
		{
			RebirthMainPart.PropertyStr propertyStr2 = list[j];
			if (propertyStr2.Percent)
			{
				string[] array6 = array;
				int num7 = 1;
				array6[num7] = array6[num7] + string.Format(Global.GetLang("{0}：{1}%"), propertyStr2.Str, this.CutDoubleValue2(propertyStr2.Att * 100.0)) + Environment.NewLine;
			}
			else if (propertyStr2.Att > 1.0)
			{
				int num8 = this.MyDoubleToInt(propertyStr2.Att);
				string[] array7 = array;
				int num9 = 1;
				array7[num9] = array7[num9] + string.Format(Global.GetLang("{0}：{1}"), propertyStr2.Str, num8) + Environment.NewLine;
			}
		}
		return array;
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

	public void RerfrashLock()
	{
		if (this.mBtnItems != null)
		{
			for (int i = 0; i < this.mBtnItems.Count; i++)
			{
				if (this.mBtnItems[i] != null)
				{
					this.mBtnItems[i].RefreshLock();
				}
			}
		}
	}

	internal void ShowRoleProPerty(double[] attArray)
	{
		if (ConfigExtPropIndexes.GetExtPropIndexesVOByWord("MAX").ID == attArray.Length)
		{
			Global.ShowProPerty(1, this.GetAllPropertyStr(attArray), null);
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private GameObject _BtnExample;

	[SerializeField]
	private Transform _ItemsBtnRoot;

	[SerializeField]
	private GButton _CloseBtn;

	[SerializeField]
	private GButton _PropBtn;

	[SerializeField]
	private GButton _RebirthPropBtn;

	[SerializeField]
	private ListBox _BtnItemRoot;

	[SerializeField]
	private Transform _MapRoot;

	[SerializeField]
	private GameObject _MapObj;

	[SerializeField]
	private GButton _HelpBtn;

	private List<RebirthMainPart.BtnHander> mBtnItems = new List<RebirthMainPart.BtnHander>();

	private List<RebirthMainPart.MapHander> mMapItems = new List<RebirthMainPart.MapHander>();

	private RebornBossVO mRebornBossVOOne;

	private byte mPropType;

	protected GChildWindow m_helpWindow;

	protected NewCommonHelpWindow m_helpPart;

	private struct PropertyStr
	{
		public int ShowList;

		public bool Percent;

		public string Str;

		public double Att;
	}

	private class BtnHander
	{
		public BtnHander(GameObject obj)
		{
			RebirthMainPart.BtnHander <>f__this = this;
			this._Collider = obj.GetComponent<BoxCollider>();
			string text = string.Empty;
			if (!NGUITools.GetActive(obj))
			{
				obj.SetActive(true);
			}
			if (obj.name.Equals(RebirthType.RebirthMap.ToString()))
			{
				this.mType = RebirthType.RebirthMap;
				text = "heianxindalu_n";
			}
			else if (obj.name.Equals(RebirthType.RebirthJinJie.ToString()))
			{
				this.mType = RebirthType.RebirthJinJie;
				text = "anniu_chongshengjinjie_2";
			}
			else if (obj.name.Equals(RebirthType.RebirthYinJi.ToString()))
			{
				this.mType = RebirthType.RebirthYinJi;
				text = "anniu_chongshengyinji_2";
			}
			else if (obj.name.Equals(RebirthType.RebirthRongLu.ToString()))
			{
				this.mType = RebirthType.RebirthRongLu;
				text = "anniu_chongshengzhuangbei_2";
			}
			else if (obj.name.Equals(RebirthType.RebirthBoss.ToString()))
			{
				this.mType = RebirthType.RebirthBoss;
				text = "anniu_chongshengboss_2";
			}
			else if (obj.name.Equals(RebirthType.RebirthRanks.ToString()))
			{
				this.mType = RebirthType.RebirthRanks;
				text = "anniu_chongshengpaihang_2";
			}
			else if (obj.name.Equals(RebirthType.RebirthMiBao.ToString()))
			{
				this.mType = RebirthType.RebirthMiBao;
				text = "anniu_moshenmibao_2";
			}
			UIEventListener.Get(obj).onPress = delegate(GameObject g, bool b)
			{
				if (<>f__this.onPress != null)
				{
					<>f__this.onPress(<>f__this.mType);
				}
				if (!b)
				{
					if (0f < Global.GetBtnCD(obj.GetInstanceID()))
					{
						return;
					}
					Global.AddBtnCD(obj.GetInstanceID(), 0.5f);
					if (<>f__this.BtnClick != null)
					{
						<>f__this.BtnClick(<>f__this.mType);
					}
				}
			};
			this.ThisObj = obj;
			this.label = obj.transform.FindChild("label").GetComponent<UILabel>();
			this.label.transform.localScale = Vector3.one * 18f;
			this._BakSp = obj.transform.FindChild("bg").GetComponent<ShowNetImage>();
			this.SelectTrans = obj.transform.FindChild("BgSelect");
			this.Lock = obj.transform.FindChild("Lock").gameObject;
			this.ItemHasLock = false;
			this.Select = false;
			this._BakSp.URL = "NetImages/GameRes/Images/RebornWin/" + text + ".png";
			this._BakSp.ImageDownloaded = delegate(object g)
			{
				<>f__this.scale = new Vector3((float)<>f__this._BakSp.ItsSizeWidth, (float)<>f__this._BakSp.ItsSizeHeight, 1f);
				<>f__this._BakSp.transform.localScale = <>f__this.scale;
			};
			this.label.text = this.GetItemsName(this.mType);
		}

		public RebirthType Type
		{
			get
			{
				return this.mType;
			}
		}

		private string GetItemsName(RebirthType type)
		{
			string[] array = new string[]
			{
				Global.GetLang("重生地图"),
				Global.GetLang("重生进阶"),
				Global.GetLang("重生印记"),
				Global.GetLang("重生熔炉"),
				Global.GetLang("重生BOSS"),
				Global.GetLang("重生排行"),
				Global.GetLang("魔神秘宝"),
				string.Empty
			};
			return Global.GetLang(array[(int)type]);
		}

		public bool Select
		{
			get
			{
				return this.mSelect;
			}
			set
			{
				this.mSelect = value;
				this.SelectTrans.gameObject.SetActive(this.mSelect);
				if (this.mSelect)
				{
					this._BakSp.transform.localScale = this.scale * 1.2f;
				}
				else
				{
					this._BakSp.transform.localScale = this.scale;
				}
			}
		}

		public bool ItemHasLock
		{
			get
			{
				return NGUITools.GetActive(this.Lock);
			}
			set
			{
				this.Lock.SetActive(value);
				if (value)
				{
					this.Select = false;
					this._BakSp.ToGrayBitmap = true;
					this._Collider.enabled = false;
				}
				else
				{
					this._BakSp.ToGrayBitmap = false;
					this._Collider.enabled = true;
				}
			}
		}

		public void RefreshLock()
		{
			this.ItemHasLock = false;
			if (this.mType == RebirthType.RebirthYinJi)
			{
				this.ItemHasLock = true;
				if (Global.Data != null && Global.Data.roleData != null)
				{
					this.ItemHasLock = (-1 == GongnengYugaoMgr.GetNewIconOnChongShengYinJi(Global.Data.roleData.RebornLevel));
				}
			}
			if (this.mType == RebirthType.RebirthMiBao)
			{
				this.ItemHasLock = true;
				if (Global.Data != null && Global.Data.roleData != null)
				{
					if (!ConfigVersionSystemOpen.IsVersionSystemOpen(100118))
					{
						this.ItemHasLock = true;
						return;
					}
					this.ItemHasLock = (-1 == GongnengYugaoMgr.GetNewIconOnChongShengYinJi(Global.Data.roleData.RebornLevel));
				}
			}
		}

		public RebirthMainPart.BtnHanderDelegate onPress;

		public RebirthMainPart.BtnHanderDelegate BtnClick;

		private ShowNetImage _BakSp;

		private UILabel label;

		private Transform SelectTrans;

		private GameObject Lock;

		private Vector3 scale = Vector3.one;

		private BoxCollider _Collider;

		private RebirthType mType;

		private GameObject ThisObj;

		private bool mSelect;
	}

	private class MapHander
	{
		public MapHander(GameObject obj, int type)
		{
			if (!NGUITools.GetActive(obj))
			{
				obj.SetActive(true);
			}
			this.mType = (RebitrhMapType)type;
			this.image = obj.transform.FindChild("Image").GetComponent<ShowNetImage>();
			this.image.URL = string.Empty + this.ImageName[type];
			UIEventListener.Get(this.image.gameObject).onClick = delegate(GameObject g)
			{
				if (this.OnClick != null)
				{
					this.OnClick(this.mType);
				}
			};
		}

		public RebirthMainPart.MapHanderDelegate OnClick;

		private string[] ImageName = new string[]
		{
			"heianxindalu_p.png"
		};

		private ShowNetImage image;

		private RebitrhMapType mType;
	}

	private delegate void BtnHanderDelegate(RebirthType btnType);

	private delegate void MapHanderDelegate(RebitrhMapType btnType);
}
