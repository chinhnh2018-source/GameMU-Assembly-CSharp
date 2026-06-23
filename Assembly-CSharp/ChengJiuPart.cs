using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class ChengJiuPart : UserControl
{
	protected override void InitializeComponent()
	{
		GameInstance.Game.SpriteQueryChengJiuData();
		this.ChengJiuGoodsIDArr = ConfigSystemParam.GetSystemParamIntArrayByName("ChengJiuBufferGoodsIDs", ',');
		this.TypeItemCollection = this.TypelistBox.ItemsSource;
		this.ItemCollection = this.ItemlistBox.ItemsSource;
		this.HutiItemCollection = this.HuTiListBox.ItemsSource;
		this.HuTiListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.huTiListBox_SelectionChanged);
		this.TypelistBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.InitTypeData();
		this.hutiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.SendEvent("900", Global.GetLang("成就护体页面查看次数"));
			this.ShowHuTiWindow();
		};
		this.CloshutiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseShowHuTiWindow();
		};
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		this.InitControl();
	}

	private void InitControl()
	{
		if (Global.Data.ChengJiuData == null)
		{
			GameInstance.Game.SpriteQueryChengJiuData();
		}
		else
		{
			this.myChengJiutext.Text = Global.Data.ChengJiuData.ChengJiuPoints.ToString();
			this.chengjiuText.Text = Global.Data.ChengJiuData.ChengJiuPoints.ToString();
		}
		this.TypelistBox.SelectedIndex = 0;
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void InitTypeData()
	{
		XElement gameResXml = Global.GetGameResXml("Config/ChengJiuTab.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "ChengJiu"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
				ChengJiuTypeItem chengJiuTypeItem = U3DUtils.NEW<ChengJiuTypeItem>();
				this.TypeItemCollection.Add(chengJiuTypeItem);
				chengJiuTypeItem.NameText = xelementAttributeStr;
				chengJiuTypeItem.imageUrl = string.Format("NetImages/GameRes/Images/Hybrid/chengjiu_{0}.png", Global.GetXElementAttributeStr(xelement, "ID"));
				chengJiuTypeItem.typeID = Global.GetXElementAttributeStr(xelement, "ID");
			}
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		ChengJiuTypeItem chengJiuTypeItem = U3DUtils.AS<ChengJiuTypeItem>(this.TypelistBox.SelectedItem);
		if (null == chengJiuTypeItem)
		{
			return;
		}
		if (this.tempitem != null && this.tempitem != chengJiuTypeItem)
		{
			this.tempitem.Bak.spriteName = "lianluEquipItem_bak";
		}
		this.tempitem = chengJiuTypeItem;
		this.tempitem.Bak.spriteName = "lianluEquipItem_bak2";
		this.RefeshChengjiuItemList(chengJiuTypeItem.typeID, false);
	}

	private void huTiListBox_SelectionChanged(object sender, EventArgs e)
	{
		HuTiItem huTiItem = U3DUtils.AS<HuTiItem>(this.HuTiListBox.SelectedItem);
		if (null == huTiItem)
		{
			return;
		}
		if (this.tempHuTiItem != null && this.tempHuTiItem != huTiItem)
		{
			this.tempitem.Bak.spriteName = "lianluEquipItem_bak";
		}
		if (this.tempHuTiItem == huTiItem)
		{
			return;
		}
		this.tempHuTiItem = huTiItem;
		this.tempHuTiItem.Bak.spriteName = "HuTiitem";
		this.showHuTiInfo(huTiItem);
	}

	protected void SetBuffTipGicon()
	{
		BitmapData value = Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/menu_item_unselected.png"), 95.0, 31.0, 3.0, 2.0);
		for (int i = 0; i < this.ChengJiuGoodsIDArr.Length; i++)
		{
			int num = this.ChengJiuGoodsIDArr[i];
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 95.0;
			gicon.Height = 31.0;
			gicon.BodySource = new ImageBrush(value);
			gicon.Cursor = Cursors.Hand;
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				num,
				0,
				-1,
				20
			});
			this.Container.Children.Add(gicon);
			Canvas.SetLeft(gicon, 544);
			Canvas.SetTop(gicon, 43 + 31 * i + 6 * i);
		}
	}

	public override void Destroy()
	{
	}

	private void huTiLevelCanvas_MOUSE_MOVE(MouseEvent e)
	{
	}

	private void huTiLevelCanvas_MOUSE_OUT(MouseEvent e)
	{
	}

	private void NextPage()
	{
		if (this.CurrentSelectedPage < this.MaxPageCount)
		{
			this.CurrentSelectedPage++;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void PrevPage()
	{
		if (this.CurrentSelectedPage > 0)
		{
			this.CurrentSelectedPage--;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	public void ShowPage(int pageIndex)
	{
	}

	private string FormatNumString(int nValue)
	{
		if (nValue >= 100000000)
		{
			nValue -= nValue % 1000000;
			return string.Format(Global.GetLang("{0}亿"), (double)nValue / 100000000.0);
		}
		if (nValue >= 10000)
		{
			nValue -= nValue % 100;
			return string.Format(Global.GetLang("{0}万"), (double)nValue / 10000.0);
		}
		return nValue.ToString();
	}

	private void RefeshChengjiuItemList(string sTypeID, bool forceNew = false)
	{
		this.CurrentShowChengJiuTabPage = sTypeID;
		this.ItemCollection.Clear();
		UIPanel component = this.ItemlistBox.transform.parent.GetComponent<UIPanel>();
		UIScrollBar verticalScrollBar = component.GetComponent<UIDraggablePanel>().verticalScrollBar;
		verticalScrollBar.scrollValue = 0f;
		List<ChengJiuVO> chengJiuVOListByID = ConfigChengJiu.GetChengJiuVOListByID(Global.SafeConvertToInt32(sTypeID));
		if (chengJiuVOListByID == null || chengJiuVOListByID.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < chengJiuVOListByID.Count; i++)
		{
			ChengJiuVO chengJiuVO = chengJiuVOListByID[i];
			if (chengJiuVO != null)
			{
				int num = 1;
				if (chengJiuVO.ZhuanShengLimit > 0)
				{
					num = chengJiuVO.ZhuanShengLimit;
				}
				else if (chengJiuVO.LevelLimit > 0)
				{
					num = chengJiuVO.LevelLimit;
				}
				else if (chengJiuVO.LoginDayOne > 0)
				{
					num = chengJiuVO.LoginDayOne;
				}
				else if (chengJiuVO.LoginDayTwo > 0)
				{
					num = chengJiuVO.LoginDayTwo;
				}
				else if (chengJiuVO.KillMonster > 0)
				{
					num = chengJiuVO.KillMonster;
				}
				else if (chengJiuVO.KillBoss > 0)
				{
					num = chengJiuVO.KillBoss;
				}
				else if (chengJiuVO.TongQianLimit > 0)
				{
					num = chengJiuVO.TongQianLimit;
				}
				else if (string.Empty != chengJiuVO.QiangHuaLimit)
				{
					string[] array = chengJiuVO.QiangHuaLimit.Split(new char[]
					{
						','
					});
					num = Global.SafeConvertToInt32(array[1]);
				}
				else if (string.Empty != chengJiuVO.ZhuiJiaLimit)
				{
					string[] array = chengJiuVO.ZhuiJiaLimit.Split(new char[]
					{
						','
					});
					num = Global.SafeConvertToInt32(array[1]);
				}
				else if (string.Empty != chengJiuVO.HeChengLimit)
				{
					string[] array = chengJiuVO.HeChengLimit.Split(new char[]
					{
						','
					});
					num = Global.SafeConvertToInt32(array[1]);
				}
				else if (chengJiuVO.KillRaid > 0)
				{
					num = chengJiuVO.KillRaid;
				}
				else if (string.Empty != chengJiuVO.GoodsLimit)
				{
					string[] array = chengJiuVO.GoodsLimit.Split(new char[]
					{
						','
					});
					num = Global.SafeConvertToInt32(array[0]);
				}
				else if (chengJiuVO.SkillLevel > 0)
				{
					num = chengJiuVO.SkillLevel;
				}
				int chengJiuID = chengJiuVO.ChengJiuID;
				int num2 = this.GetChengJinDuValue(chengJiuID);
				int chengJiuState = this.GetChengJiuState(chengJiuID);
				if (chengJiuState > 1)
				{
					num2 = num;
				}
				ChengJiuItem chengJiuItem = U3DUtils.NEW<ChengJiuItem>();
				this.ItemCollection.AddNoUpdate(chengJiuItem);
				chengJiuItem.titleText.Text = chengJiuVO.Name;
				chengJiuItem.intText.Text = chengJiuVO.Description;
				chengJiuItem.itemState = chengJiuState;
				chengJiuItem.jinduText.Text = string.Format(Global.GetLang("{0}/{1}"), this.FormatNumString(num2), this.FormatNumString(num));
				chengJiuItem.progressBar.Percent = (double)(num2 / num);
				chengJiuItem.jiangliCJText.Text = chengJiuVO.ChengJiu.ToString();
				chengJiuItem.jingliBDZuanshiText.Text = string.Empty;
				chengJiuItem.chengJiuID = chengJiuVO.ChengJiuID;
				chengJiuItem.chengJiuType = chengJiuVO.ID.ToString();
				this.ItemsDict[chengJiuID] = chengJiuItem;
			}
		}
	}

	public void UpdateChengJiuItem(int chengJiuID)
	{
		if (chengJiuID < 0 || !this.ItemsDict.ContainsKey(chengJiuID))
		{
			return;
		}
		ChengJiuVO chengJiuVOByChengJiuID = ConfigChengJiu.GetChengJiuVOByChengJiuID(chengJiuID);
		if (chengJiuVOByChengJiuID != null)
		{
			int num = 1;
			if (chengJiuVOByChengJiuID.LevelLimit > 0)
			{
				num = chengJiuVOByChengJiuID.LevelLimit;
			}
			else if (chengJiuVOByChengJiuID.LoginDayOne > 0)
			{
				num = chengJiuVOByChengJiuID.LoginDayOne;
			}
			else if (chengJiuVOByChengJiuID.LoginDayTwo > 0)
			{
				num = chengJiuVOByChengJiuID.LoginDayTwo;
			}
			else if (chengJiuVOByChengJiuID.KillMonster > 0)
			{
				num = chengJiuVOByChengJiuID.KillMonster;
			}
			else if (chengJiuVOByChengJiuID.KillBoss > 0)
			{
				num = chengJiuVOByChengJiuID.KillBoss;
			}
			else if (chengJiuVOByChengJiuID.TongQianLimit > 0)
			{
				num = chengJiuVOByChengJiuID.TongQianLimit;
			}
			else if (string.Empty != chengJiuVOByChengJiuID.QiangHuaLimit)
			{
				string[] array = chengJiuVOByChengJiuID.QiangHuaLimit.Split(new char[]
				{
					','
				});
				num = Global.SafeConvertToInt32(array[1]);
			}
			else if (string.Empty != chengJiuVOByChengJiuID.ZhuiJiaLimit)
			{
				string[] array = chengJiuVOByChengJiuID.ZhuiJiaLimit.Split(new char[]
				{
					','
				});
				num = Global.SafeConvertToInt32(array[1]);
			}
			else if (string.Empty != chengJiuVOByChengJiuID.HeChengLimit)
			{
				string[] array = chengJiuVOByChengJiuID.HeChengLimit.Split(new char[]
				{
					','
				});
				num = Global.SafeConvertToInt32(array[1]);
			}
			else if (chengJiuVOByChengJiuID.SkillLevel > 0)
			{
				num = chengJiuVOByChengJiuID.SkillLevel;
			}
			int chengJiuID2 = chengJiuVOByChengJiuID.ChengJiuID;
			int num2 = this.GetChengJinDuValue(chengJiuID2);
			int chengJiuState = this.GetChengJiuState(chengJiuID2);
			if (chengJiuState > 1)
			{
				num2 = num;
			}
			this.ItemsDict[chengJiuID].jinduText.Text = string.Format(Global.GetLang("{0}/{1}"), this.FormatNumString(num2), this.FormatNumString(Convert.ToInt32(num)));
			this.ItemsDict[chengJiuID].itemState = chengJiuState;
			this.ItemsDict[chengJiuID].progressBar.Percent = (double)(num2 / Convert.ToInt32(num));
		}
	}

	private void GetHintGoodsIDInfo(List<ChengJiuItem> itemsList, out int pageIndex)
	{
		pageIndex = 0;
	}

	public void RefreshChengJiuPoints(int value)
	{
		if (Global.Data.ChengJiuData != null)
		{
			Global.Data.ChengJiuData.ChengJiuPoints = (long)value;
			this.myChengJiutext.Text = Global.Data.ChengJiuData.ChengJiuPoints.ToString();
			this.chengjiuText.Text = Global.Data.ChengJiuData.ChengJiuPoints.ToString();
		}
	}

	public void RefreshChengJiuData(ChengJiuData chengJiuData)
	{
		ChengJiuData chengJiuData2 = Global.Data.ChengJiuData;
		Global.Data.ChengJiuData = chengJiuData;
		if (chengJiuData != null)
		{
			if (chengJiuData.NowCompletedChengJiu > 0)
			{
			}
			this.myChengJiutext.Text = chengJiuData.ChengJiuPoints.ToString();
			this.chengjiuText.Text = chengJiuData.ChengJiuPoints.ToString();
			if (chengJiuData2 == null)
			{
				this.RefeshChengjiuItemList(this.CurrentShowChengJiuTabPage, true);
			}
			else
			{
				this.UpdateChengJiuItem(chengJiuData.NowCompletedChengJiu);
			}
		}
	}

	private int GetChengJiuLevelIndex(int chengJiuPoints)
	{
		return 2;
	}

	private int GetChengJiuState(int chengJiuID)
	{
		if (Global.Data.ChengJiuData == null || Global.Data.ChengJiuData.ChengJiuFlags == null)
		{
			return 1;
		}
		List<ushort> chengJiuFlags = Global.Data.ChengJiuData.ChengJiuFlags;
		if (chengJiuFlags == null)
		{
			return 1;
		}
		int i = 0;
		while (i < chengJiuFlags.Count)
		{
			uint num = (uint)chengJiuFlags[i];
			uint num2 = num >> 2;
			if ((ulong)num2 == (ulong)((long)chengJiuID))
			{
				uint num3 = 1U;
				if ((num & num3) == 0U)
				{
					return 1;
				}
				return 3;
			}
			else
			{
				i++;
			}
		}
		return 1;
	}

	private int GetChengJinDuValue(int chengJiuID)
	{
		switch (chengJiuID / 100)
		{
		case 1:
			return 0;
		case 2:
			return Global.Data.roleData.Level;
		case 3:
			return Global.Data.roleData.ChangeLifeCount;
		case 4:
			if (Global.Data.ChengJiuData != null)
			{
				return Global.Data.ChengJiuData.ContinueLoginNum;
			}
			break;
		case 5:
			if (Global.Data.ChengJiuData != null)
			{
				return (int)Global.Data.ChengJiuData.TotalLoginNum;
			}
			break;
		case 6:
			return Global.Data.roleData.YinLiang;
		case 7:
			if (Global.Data.ChengJiuData != null)
			{
				return (int)Global.Data.ChengJiuData.TotalKilledMonsterNum;
			}
			break;
		case 8:
			if (Global.Data.ChengJiuData != null)
			{
				return (int)Global.Data.ChengJiuData.TotalKilledBossNum;
			}
			break;
		case 9:
			if (Global.Data.ChengJiuData != null)
			{
				return (int)Global.Data.ChengJiuData.CompleteNormalCopyMapCount;
			}
			break;
		case 10:
			if (Global.Data.ChengJiuData != null)
			{
				return (int)Global.Data.ChengJiuData.CompleteHardCopyMapCount;
			}
			break;
		case 11:
			if (Global.Data.ChengJiuData != null)
			{
				return (int)Global.Data.ChengJiuData.CompleteDifficltCopyMapCount;
			}
			break;
		}
		return 0;
	}

	private void SetselectItemImage(string Type)
	{
	}

	protected void GetLingQuAllChengJiuArr()
	{
		if (Global.Data.ChengJiuData == null || Global.Data.ChengJiuData.ChengJiuFlags == null)
		{
			return;
		}
		List<ushort> chengJiuFlags = Global.Data.ChengJiuData.ChengJiuFlags;
		if (chengJiuFlags == null)
		{
			return;
		}
		this.weiLingQUchengJiuArr.RemoveAt(0);
		for (int i = 0; i < chengJiuFlags.Count; i++)
		{
			uint num = (uint)chengJiuFlags[i];
			uint num2 = num >> 2;
			uint num3 = 1U;
			int num4 = 2;
			if ((num & num3) != 0U)
			{
				if (((ulong)num & (ulong)((long)num4)) == 0UL)
				{
					this.weiLingQUchengJiuArr.Add(num2);
				}
			}
		}
	}

	private void CloseShowHuTiWindow()
	{
		this.hutiPanel.gameObject.SetActive(false);
		this._ModalBak.gameObject.SetActive(false);
	}

	private void ShowHuTiWindow()
	{
		this.hutiPanel.gameObject.SetActive(true);
		this._ModalBak.gameObject.SetActive(true);
		this.HutiItemCollection.Clear();
		for (int i = 0; i < this.ChengJiuGoodsIDArr.Length; i++)
		{
			HuTiItem huTiItem = U3DUtils.NEW<HuTiItem>();
			this.HutiItemCollection.Add(huTiItem);
			huTiItem.titleText.Text = Global.GetGoodsNameByID(this.ChengJiuGoodsIDArr[i], false);
			huTiItem.goodsID = this.ChengJiuGoodsIDArr[i];
			XElement xelement = Global.GetXElement(Global.GetGameResXml("config/ChengJiuBuff.xml"), "ChengJiu", "ID", (i + 1).ToString());
			if (xelement != null)
			{
				if (Global.Data.ChengJiuData.ChengJiuPoints >= (long)Global.GetXElementAttributeInt(xelement, "ChengJiu"))
				{
					huTiItem.titleText.textColor = 16751880U;
					huTiItem.infoText.Text = string.Format(Global.GetLang("{{c39550}}需要成就点:{{-}} {0}"), Global.GetXElementAttributeStr(xelement, "ChengJiu"));
				}
				else
				{
					huTiItem.infoText.Text = string.Format(Global.GetLang("{{757575}}需要成就点: {0}{{-}}"), Global.GetXElementAttributeStr(xelement, "ChengJiu"));
					huTiItem.titleText.textColor = 7697781U;
				}
			}
		}
		this.HuTiListBox.SelectedIndex = 0;
	}

	private void showHuTiInfo(HuTiItem item)
	{
		if (item == null)
		{
			return;
		}
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(item.goodsID);
		if (this.textArray != null && this.textArray.Length == 4)
		{
			this.textArray[0].Text = string.Format("{0}{1}-{2}", Global.GetColorStringForNGUIText(new object[]
			{
				"c39550",
				Global.GetLang("物理攻击：")
			}), goodsEquipPropsDoubleList[7], goodsEquipPropsDoubleList[8]);
			this.textArray[1].Text = string.Format("{0}{1}-{2}", Global.GetColorStringForNGUIText(new object[]
			{
				"c39550",
				Global.GetLang("魔法攻击：")
			}), goodsEquipPropsDoubleList[9], goodsEquipPropsDoubleList[10]);
			this.textArray[2].Text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"c39550",
				Global.GetLang("生命上限：")
			}), goodsEquipPropsDoubleList[13]);
			this.textArray[3].Text = string.Format("{0}{1}%", Global.GetColorStringForNGUIText(new object[]
			{
				"c39550",
				Global.GetLang("伤害吸收：")
			}), goodsEquipPropsDoubleList[24] * 100.0);
		}
		this.hutiTitleText.Text = item.titleText.Text;
	}

	public void StartHeart()
	{
		this.StopHeart();
		this._Timer = new DispatcherTimer("ChengJiuPart_Timer");
		this._Timer.Interval = TimeSpan.FromMilliseconds(50.0);
		this._Timer.Tick = new DispatcherTimerEventHandler(this.ForgeTimer_Tick);
		this._TimerCount = 0;
		this._Timer.Start();
	}

	public void StopHeart()
	{
		if (this._Timer == null)
		{
			return;
		}
		this._Timer.Stop();
		this._Timer.Tick = new DispatcherTimerEventHandler(this.ForgeTimer_Tick);
		this._Timer = null;
		this._TimerCount = 0;
	}

	private void ForgeTimer_Tick(object sender, object e)
	{
		this._TimerCount++;
		if (this._TimerCount >= 15)
		{
			if (this.weiLingQUchengJiuArr.Count != 0)
			{
				GameInstance.Game.SpriteFetchChengJiuAward((int)this.weiLingQUchengJiuArr[0]);
				this.weiLingQUchengJiuArr.RemoveAt(0);
			}
			else
			{
				this.StopHeart();
			}
		}
	}

	public ListBox TypelistBox;

	public ListBox ItemlistBox;

	public TextBlock myChengJiutext;

	public ListBox HuTiListBox;

	public GButton hutiBtn;

	public GButton CloshutiBtn;

	public UIPanel hutiPanel;

	public UISprite _ModalBak;

	public TextBlock chengjiuText;

	public TextBlock[] textArray;

	public TextBlock hutiTitleText;

	public UILabel m_LblShengMing;

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private Dictionary<string, List<ChengJiuItem>> TabItemsDict = new Dictionary<string, List<ChengJiuItem>>();

	private Dictionary<int, ChengJiuItem> ItemsDict = new Dictionary<int, ChengJiuItem>();

	private int[] ChengJiuGoodsIDArr;

	private string CurrentShowChengJiuTabPage = "1";

	public DPSelectedItemEventHandler DPSelectedItem;

	private DispatcherTimer _Timer;

	private int _TimerCount;

	private List<uint> weiLingQUchengJiuArr = new List<uint>();

	private ObservableCollection TypeItemCollection;

	private ObservableCollection ItemCollection;

	private ObservableCollection HutiItemCollection;

	private ChengJiuTypeItem tempitem;

	private HuTiItem tempHuTiItem;
}
