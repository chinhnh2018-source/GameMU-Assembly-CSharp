using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class TuJianPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_TypeCollection = this.m_TypeList.ItemsSource;
		this.m_TypeList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.TujianTypeItemSelectChange);
		this.InitControlProc();
		this.InitTypeList();
		this.CheckGuardStatueOpenStatus();
	}

	private void InitControlProc()
	{
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.m_BtnSubmit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartSubmit();
		};
		UIEventListener.Get(this.m_BtnProps.gameObject).onClick = delegate(GameObject s)
		{
			GTipServiceEx.ShowTip(Global.GetLang("累计加成属性"), this.GetTotalPropsStr(), TipTypes.NormalText, false);
		};
		this.souleRecyclingBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.activetedMaps == null || this.activetedMaps.Count <= 0)
			{
				Super.HintMainText(Global.GetLang("激活整张图鉴地图后，可回收该地图图鉴获得守护点数"), 10, 3);
				return;
			}
			this.ShowSoulRecyclingWindow();
		};
		this.statueGuardBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ShowGuardStatueWindow();
		};
	}

	private void CheckGuardStatueOpenStatus()
	{
		bool active = false;
		if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GuardStatue))
		{
			active = true;
		}
		this.souleRecyclingBtn.gameObject.SetActive(active);
		this.statueGuardBtn.gameObject.SetActive(active);
	}

	private void InitTypeList()
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/TuJianType.xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(isolateResXml, "TuJian"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ID");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Name");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "Description");
			string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "KaiQiLevel");
			TuJianTypeItem tuJianTypeItem = U3DUtils.NEW<TuJianTypeItem>();
			tuJianTypeItem.OpenStr = xelementAttributeStr4;
			tuJianTypeItem.strTypeID = xelementAttributeStr;
			tuJianTypeItem.PropsStr = Global.GetXElementAttributeStr(xelement, "ShuXiangJiaCheng");
			this.m_TypeCollection.AddNoUpdate(tuJianTypeItem);
		}
		this.m_TypeCollection.DelayUpdate();
		this.RefreshTypeList();
	}

	private void RefreshTypeList()
	{
		this._TotalNum = 0;
		this._TotalActivedNum = 0;
		if (this.activetedMaps != null)
		{
			this.activetedMaps.Clear();
		}
		for (int i = 0; i < this.m_TypeCollection.Length; i++)
		{
			TuJianTypeItem tuJianTypeItem = U3DUtils.AS<TuJianTypeItem>(this.m_TypeCollection[i]);
			int num = 0;
			int activedTujianTypeNumByTypeID = TuJianPart.GetActivedTujianTypeNumByTypeID(tuJianTypeItem.strTypeID.SafeToInt32(0), out num);
			tuJianTypeItem.m_LblInfo.text = string.Format("{0}/{1}", activedTujianTypeNumByTypeID, num);
			tuJianTypeItem.IsActived = (activedTujianTypeNumByTypeID >= num);
			this._TotalNum += num;
			this._TotalActivedNum += activedTujianTypeNumByTypeID;
			if (tuJianTypeItem.IsActived)
			{
				int num2 = tuJianTypeItem.strTypeID.SafeToInt32(0);
				if (!this.activetedMaps.Contains(num2))
				{
					this.activetedMaps.Add(num2);
				}
			}
			if (!string.IsNullOrEmpty(tuJianTypeItem.OpenStr))
			{
				string[] array = tuJianTypeItem.OpenStr.Split(new char[]
				{
					','
				});
				if (array != null)
				{
					if (!UIHelper.AvalidLevel(array[1].SafeToInt32(0), array[0].SafeToInt32(0), false))
					{
						tuJianTypeItem.IsOpen = false;
					}
					else
					{
						tuJianTypeItem.IsOpen = true;
					}
				}
			}
		}
		this.ProgressBar.Percent = (double)this._TotalActivedNum / (double)this._TotalNum;
		this.ProgressBar.uiLabel.text = string.Format("{0}/{1}", this._TotalActivedNum, this._TotalNum);
		TuJianPart.InitTujianListInBag();
		for (int j = 0; j < this.m_TypeCollection.Length; j++)
		{
			TuJianTypeItem tuJianTypeItem = U3DUtils.AS<TuJianTypeItem>(this.m_TypeCollection[j]);
			tuJianTypeItem.IsCanSubmit = this.GetIsCanSubmitByTypeID(tuJianTypeItem.strTypeID.SafeToInt32(0));
		}
	}

	private void TujianTypeItemSelectChange(object obj, MouseEvent e)
	{
		TuJianTypeItem tuJianTypeItem = U3DUtils.AS<TuJianTypeItem>(this.m_TypeList.SelectedItem);
		if (null == tuJianTypeItem)
		{
			return;
		}
		if (this.SelectedItem == tuJianTypeItem)
		{
		}
		if (!(this.SelectedItem != null) || this.SelectedItem != tuJianTypeItem)
		{
		}
		this.SelectedItem = tuJianTypeItem;
		if (this.SelectedItem.IsOpen)
		{
			this.ShowTuJianDetilPartWindow(this.SelectedItem.strTypeID.SafeToInt32(0), this.SelectedItem.PropsStr, this.SelectedItem.IsCanSubmit);
			SystemHelpMgr.OnAction(UIObjIDs.TuJianPart, HelpStateEvents.Clicked, -1);
		}
	}

	public static void ClearXMLData()
	{
		if (TuJianPart.TujiaXmlDataDict != null)
		{
			TuJianPart.TujiaXmlDataDict.Clear();
		}
		TuJianPart.TujiaXmlDataDict = null;
	}

	private static void InitTujiaXmlDataDict()
	{
		if (TuJianPart.TujiaXmlDataDict == null)
		{
			XElement isolateResXml = Global.GetIsolateResXml("Config/TuJianItems.xml");
			if (isolateResXml == null)
			{
				return;
			}
			TuJianPart.TujiaXmlDataDict = new Dictionary<int, TujianXmlData>();
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(isolateResXml, "config"), "*");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				TujianXmlData tujianXmlData = new TujianXmlData();
				tujianXmlData.ID = Global.GetXElementAttributeInt(xelement, "ID");
				tujianXmlData.TypeID = Global.GetXElementAttributeInt(xelement, "Type");
				tujianXmlData.MonsterID = Global.GetXElementAttributeInt(xelement, "GLMonsterID");
				tujianXmlData.Props = Global.GetXElementAttributeStr(xelement, "ShuXing");
				tujianXmlData.NeedGoods = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				tujianXmlData.Name = Global.GetXElementAttributeStr(xelement, "Name");
				if (!TuJianPart.TujiaXmlDataDict.ContainsKey(tujianXmlData.ID))
				{
					TuJianPart.TujiaXmlDataDict.Add(tujianXmlData.ID, tujianXmlData);
				}
			}
		}
	}

	public static int GetActivedTujianNumByTypeID(int typeID, out int totalNum)
	{
		totalNum = 0;
		if (TuJianPart.TujiaXmlDataDict == null)
		{
			TuJianPart.InitTujiaXmlDataDict();
		}
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<int, TujianXmlData> keyValuePair in TuJianPart.TujiaXmlDataDict)
		{
			TujianXmlData value = keyValuePair.Value;
			if (value.TypeID == typeID)
			{
				string[] array = value.NeedGoods.Split(new char[]
				{
					','
				});
				if (array != null)
				{
					num2 = array[1].SafeToInt32(0);
					totalNum += num2;
				}
				int tiJiaoTuJianNum = Global.GetTiJiaoTuJianNum(value.ID);
				num += tiJiaoTuJianNum;
				if (tiJiaoTuJianNum >= num2)
				{
					value.IsActived = true;
				}
				else
				{
					value.IsActived = false;
				}
			}
		}
		return num;
	}

	public static int GetActivedTujianTypeNumByTypeID(int typeID, out int totalTypeNum)
	{
		totalTypeNum = 0;
		if (TuJianPart.TujiaXmlDataDict == null)
		{
			TuJianPart.InitTujiaXmlDataDict();
		}
		int num = 0;
		int num2 = 0;
		foreach (TujianXmlData tujianXmlData in TuJianPart.TujiaXmlDataDict.Values)
		{
			if (tujianXmlData.TypeID == typeID)
			{
				totalTypeNum++;
				string[] array = tujianXmlData.NeedGoods.Split(new char[]
				{
					','
				});
				if (array != null)
				{
					num2 = array[1].SafeToInt32(0);
				}
				int tiJiaoTuJianNum = Global.GetTiJiaoTuJianNum(tujianXmlData.ID);
				if (tiJiaoTuJianNum >= num2)
				{
					tujianXmlData.IsActived = true;
					num++;
				}
				else
				{
					tujianXmlData.IsActived = false;
				}
			}
		}
		return num;
	}

	public static void InitTujianListInBag()
	{
		if (TuJianPart.TujiaXmlDataDict == null)
		{
			TuJianPart.InitTujiaXmlDataDict();
		}
		if (TuJianPart.TujianListInBagDict == null)
		{
			TuJianPart.TujianListInBagDict = new Dictionary<int, int>();
		}
		TuJianPart.TujianListInBagDict.Clear();
		foreach (TujianXmlData tujianXmlData in TuJianPart.TujiaXmlDataDict.Values)
		{
			if (!tujianXmlData.IsActived)
			{
				string[] array = tujianXmlData.NeedGoods.Split(new char[]
				{
					','
				});
				if (array != null)
				{
					int goodsID = array[0].SafeToInt32(0);
					if (Global.GetTotalGoodsCountByID(goodsID) > 0 && !TuJianPart.TujianListInBagDict.ContainsKey(tujianXmlData.ID))
					{
						TuJianPart.TujianListInBagDict.Add(tujianXmlData.ID, tujianXmlData.TypeID);
					}
				}
			}
		}
	}

	private List<SoulRecyclingItem> GetAvailableMonsterSoulInBag()
	{
		if (this.activetedMaps == null)
		{
			return null;
		}
		List<SoulRecyclingItem> list = new List<SoulRecyclingItem>();
		Dictionary<int, SoulRecyclingItem> monsterSoulRecyclingPoints = TuJianPart.GetMonsterSoulRecyclingPoints();
		foreach (int num in this.activetedMaps)
		{
			foreach (TujianXmlData tujianXmlData in TuJianPart.TujiaXmlDataDict.Values)
			{
				if (tujianXmlData.TypeID == num)
				{
					string[] array = tujianXmlData.NeedGoods.Split(new char[]
					{
						','
					});
					if (array != null)
					{
						int goodsID = array[0].SafeToInt32(0);
						int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
						if (totalGoodsCountByID > 0)
						{
							SoulRecyclingItem soulRecyclingItem = new SoulRecyclingItem();
							soulRecyclingItem.id = tujianXmlData.ID;
							soulRecyclingItem.goodsID = goodsID;
							soulRecyclingItem.goodsCount = totalGoodsCountByID;
							if (monsterSoulRecyclingPoints.ContainsKey(tujianXmlData.ID))
							{
								SoulRecyclingItem soulRecyclingItem2 = null;
								monsterSoulRecyclingPoints.TryGetValue(tujianXmlData.ID, ref soulRecyclingItem2);
								if (soulRecyclingItem2 != null)
								{
									soulRecyclingItem.recyclingPoint = soulRecyclingItem2.recyclingPoint;
								}
							}
							list.Add(soulRecyclingItem);
						}
					}
				}
			}
		}
		return list;
	}

	public static Dictionary<int, SoulRecyclingItem> GetMonsterSoulRecyclingPoints()
	{
		XElement gameResXml = Global.GetGameResXml("Config/JingPoShouHu.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShouHu");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return null;
		}
		Dictionary<int, SoulRecyclingItem> dictionary = new Dictionary<int, SoulRecyclingItem>(xelementList.Count);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			SoulRecyclingItem soulRecyclingItem = new SoulRecyclingItem();
			soulRecyclingItem.id = Global.GetXElementAttributeInt(xelement, "ID");
			soulRecyclingItem.typeID = Global.GetXElementAttributeInt(xelement, "Type");
			soulRecyclingItem.goodsID = Global.GetXElementAttributeInt(xelement, "GoodsID");
			soulRecyclingItem.name = Global.GetXElementAttributeStr(xelement, "Name");
			soulRecyclingItem.recyclingPoint = Global.GetXElementAttributeInt(xelement, "ShouHuAward");
			if (!dictionary.ContainsKey(soulRecyclingItem.id))
			{
				dictionary.Add(soulRecyclingItem.id, soulRecyclingItem);
			}
		}
		return dictionary;
	}

	public void ShowTuJianDetilPartWindow(int typeID, string propsStr, bool isCanSubmit)
	{
		if (null != this.tuJianDetilPart)
		{
			return;
		}
		this.tuJianDetilPart = U3DUtils.NEW<TuJianDetilPart>();
		this.tuJianDetilPart.InitPartData(typeID, propsStr, isCanSubmit);
		this.tuJianDetilPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 0 || e.ID == 1)
			{
				this.CloseTuJianDetilPartWindow();
			}
			if (e.ID == 1)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
		this.ModalPart.Add(this.tuJianDetilPart);
	}

	public void CloseTuJianDetilPartWindow()
	{
		if (null != this.tuJianDetilPart)
		{
			this.ModalPart.Remove(this.tuJianDetilPart, true);
			this.tuJianDetilPart = null;
		}
	}

	public void ShowSoulRecyclingWindow()
	{
		if (null == this.soulRecycling)
		{
			this.soulRecycling = U3DUtils.NEW<MonsterSoulRecycling>();
			List<SoulRecyclingItem> availableMonsterSoulInBag = this.GetAvailableMonsterSoulInBag();
			if (availableMonsterSoulInBag != null)
			{
				this.soulRecycling.InitBag(availableMonsterSoulInBag);
				this.soulRecycling.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.ID == 0)
					{
						this.CloseSoulRecyclingWindow();
					}
				};
			}
			this.ModalPart.Add(this.soulRecycling);
		}
	}

	public void CloseSoulRecyclingWindow()
	{
		if (null != this.soulRecycling)
		{
			this.ModalPart.Remove(this.soulRecycling, true);
			this.soulRecycling = null;
		}
	}

	private string GetTotalPropsStr()
	{
		if (this.TotalPropsDict == null)
		{
			this.TotalPropsDict = new Dictionary<int, string>();
		}
		this.TotalPropsDict.Clear();
		foreach (TujianXmlData tujianXmlData in TuJianPart.TujiaXmlDataDict.Values)
		{
			this.AddPorpsStringToDict(tujianXmlData.Props, this.TotalPropsDict, !tujianXmlData.IsActived);
		}
		for (int i = 0; i < this.m_TypeCollection.Length; i++)
		{
			TuJianTypeItem tuJianTypeItem = U3DUtils.AS<TuJianTypeItem>(this.m_TypeCollection[i]);
			this.AddPorpsStringToDict(tuJianTypeItem.PropsStr, this.TotalPropsDict, !tuJianTypeItem.IsActived);
		}
		return this.GetFormatPropsStrFromPropsDict(this.TotalPropsDict);
	}

	private void AddPorpsStringToDict(string str, Dictionary<int, string> dict, bool isForceShowProps = false)
	{
		if (dict == null)
		{
			return;
		}
		string[] array = str.Split(new char[]
		{
			'|'
		});
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2 != null)
				{
					int num = ExtPropIndexes.ExtPropIndexNames.IndexOf(array2[0].ToLower());
					string text = array2[1];
					if (!dict.ContainsKey(num))
					{
						if (isForceShowProps)
						{
							if (text.Split(new char[]
							{
								'-'
							}).Length == 2)
							{
								text = "0-0";
							}
							else
							{
								text = "0";
							}
						}
						dict.Add(num, text);
					}
					else
					{
						if (isForceShowProps)
						{
							return;
						}
						string[] array3 = text.Split(new char[]
						{
							'-'
						});
						if (array3 == null)
						{
							return;
						}
						if (array3.Length == 1)
						{
							dict[num] = (dict[num].SafeToInt32(0) + text.SafeToInt32(0)).ToString();
						}
						else
						{
							string[] array4 = dict[num].Split(new char[]
							{
								'-'
							});
							if (array4 == null)
							{
								return;
							}
							dict[num] = string.Format("{0}-{1}", array4[0].SafeToInt32(0) + array3[0].SafeToInt32(0), array4[1].SafeToInt32(0) + array3[1].SafeToInt32(0));
						}
					}
				}
			}
		}
	}

	private string GetFormatPropsStrFromPropsDict(Dictionary<int, string> dict)
	{
		string text = string.Empty;
		if (dict == null)
		{
			return string.Empty;
		}
		foreach (int num in dict.Keys)
		{
			text += Global.GetColorStringForNGUIText(new object[]
			{
				"E5BB6F",
				string.Format("{0}: ", Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[num])),
				"F5E3BB",
				string.Format("{0}", dict[num])
			});
			text += "\n";
		}
		return text;
	}

	private string ProcessStr(string str)
	{
		if (str.Length > 0 && str.Substring(str.Length - 1) == ",")
		{
			str = str.Substring(0, str.Length - 1);
		}
		return str;
	}

	private bool GetIsCanSubmitByTypeID(int typeID)
	{
		if (TuJianPart.TujianListInBagDict == null || TuJianPart.TujianListInBagDict.Keys.Count <= 0)
		{
			return false;
		}
		foreach (int num in TuJianPart.TujianListInBagDict.Keys)
		{
			if (TuJianPart.TujianListInBagDict[num] == typeID)
			{
				return true;
			}
		}
		return false;
	}

	private void StartSubmit()
	{
		TuJianPart.InitTujianListInBag();
		if (TuJianPart.TujianListInBagDict == null || TuJianPart.TujianListInBagDict.Keys.Count <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中没有图鉴"), new object[0]), 0, -1, -1, 0);
			return;
		}
		string text = string.Empty;
		foreach (int num in TuJianPart.TujianListInBagDict.Keys)
		{
			text += num;
			text += ",";
		}
		GameInstance.Game.SpriteReferPictureJudgeCmd(this.ProcessStr(text));
	}

	public void NotifySubmitResult()
	{
		this.RefreshTypeList();
		SystemHelpMgr.OnAction(UIObjIDs.TuJianPart, HelpStateEvents.Clicked, -1);
	}

	public void SetRecyclingResult(int status, int recycledPoints, int maxPoints)
	{
		if (null != this.soulRecycling)
		{
			this.soulRecycling.SetRecyclingResult(status, recycledPoints, maxPoints);
			List<SoulRecyclingItem> availableMonsterSoulInBag = this.GetAvailableMonsterSoulInBag();
			this.soulRecycling.RefreshMonsterSoulList(availableMonsterSoulInBag);
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 303)
			{
				SystemHelpPart.SetMask(this.m_TypeList.GetItemByIndex(0).GetComponent("TuJianTypeItem"), default(Vector4));
			}
			if (id == 304)
			{
				SystemHelpPart.SetMask(this.tuJianDetilPart.SubmitBtn, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox m_TypeList;

	private ObservableCollection m_TypeCollection;

	public GButton m_BtnClose;

	public GButton m_BtnSubmit;

	public UIButton m_BtnProps;

	public GImgProgressBar ProgressBar;

	public static Dictionary<int, TujianXmlData> TujiaXmlDataDict;

	public static Dictionary<int, int> TujianListInBagDict;

	public SpriteSL ModalPart;

	private int _TotalNum;

	private int _TotalActivedNum;

	public GButton souleRecyclingBtn;

	public GButton statueGuardBtn;

	[HideInInspector]
	public MonsterSoulRecycling soulRecycling;

	private HashSet<int> activetedMaps = new HashSet<int>();

	private TuJianTypeItem SelectedItem;

	public TuJianDetilPart tuJianDetilPart;

	private Dictionary<int, string> TotalPropsDict;
}
