using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;

public class ChengJiuOverviewPart : UserControl
{
	protected override void InitializeComponent()
	{
		GameInstance.Game.SpriteQueryChengJiuData();
		this.ChengJiuGoodsIDArr = ConfigSystemParam.GetSystemParamIntArrayByName("ChengJiuBufferGoodsIDs", ',');
		this.TypeItemCollection = this.TypelistBox.ItemsSource;
		this.ItemCollection = this.ItemlistBox.ItemsSource;
		this.TypelistBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.InitTypeData();
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
			this.ChengJiuLevel = Global.GetChengJiuLevel((int)Global.Data.ChengJiuData.ChengJiuPoints);
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
				ChengJiuTypeNewItem chengJiuTypeNewItem = U3DUtils.NEW<ChengJiuTypeNewItem>();
				this.TypeItemCollection.Add(chengJiuTypeNewItem);
				chengJiuTypeNewItem.NameText = xelementAttributeStr;
				chengJiuTypeNewItem.imageUrl = string.Format("NetImages/GameRes/Images/Hybrid/chengjiu_{0}.png", Global.GetXElementAttributeStr(xelement, "ID"));
				chengJiuTypeNewItem.typeID = Global.GetXElementAttributeStr(xelement, "ID");
			}
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		ChengJiuTypeNewItem chengJiuTypeNewItem = U3DUtils.AS<ChengJiuTypeNewItem>(this.TypelistBox.SelectedItem);
		if (null == chengJiuTypeNewItem)
		{
			return;
		}
		if (this.tempitem != null && this.tempitem != chengJiuTypeNewItem)
		{
			this.tempitem.Bak.spriteName = "lianluEquipItem_bak";
		}
		this.tempitem = chengJiuTypeNewItem;
		this.tempitem.Bak.spriteName = "lianluEquipItem_bak2";
		if (this.CurrentShowChengJiuTabPage != chengJiuTypeNewItem.typeID)
		{
			this.RefeshChengjiuItemList(chengJiuTypeNewItem.typeID, false);
		}
	}

	public override void Destroy()
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
		if (chengJiuVOListByID != null && chengJiuVOListByID.Count > 0)
		{
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
						string[] array2 = chengJiuVO.ZhuiJiaLimit.Split(new char[]
						{
							','
						});
						num = Global.SafeConvertToInt32(array2[1]);
					}
					else if (string.Empty != chengJiuVO.HeChengLimit)
					{
						string[] array3 = chengJiuVO.HeChengLimit.Split(new char[]
						{
							','
						});
						num = Global.SafeConvertToInt32(array3[1]);
					}
					else if (chengJiuVO.KillRaid > 0)
					{
						num = chengJiuVO.KillRaid;
					}
					else if (string.Empty != chengJiuVO.GoodsLimit)
					{
						string[] array4 = chengJiuVO.GoodsLimit.Split(new char[]
						{
							','
						});
						num = Global.SafeConvertToInt32(array4[0]);
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
					ChengJiuNewItem chengJiuNewItem = U3DUtils.NEW<ChengJiuNewItem>();
					this.ItemCollection.AddNoUpdate(chengJiuNewItem);
					chengJiuNewItem.titleText.Text = chengJiuVO.Name;
					chengJiuNewItem.intText.Text = chengJiuVO.Description;
					chengJiuNewItem.itemState = chengJiuState;
					chengJiuNewItem.jinduText.Text = string.Format(Global.GetLang("{0}/{1}"), this.FormatNumString(num2), this.FormatNumString(num));
					float num3 = (float)num2 / (float)Convert.ToInt32(num);
					chengJiuNewItem.progressBar.Percent = (double)num3;
					chengJiuNewItem.jiangliCJText.Text = chengJiuVO.ChengJiu.ToString();
					chengJiuNewItem.jingliBDZuanshiText.Text = chengJiuVO.BindZuanShi.ToString();
					chengJiuNewItem.chengJiuID = chengJiuVO.ChengJiuID;
					chengJiuNewItem.chengJiuType = chengJiuVO.ID.ToString();
					this.ItemsDict[chengJiuID] = chengJiuNewItem;
				}
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
			int num2 = this.GetChengJinDuValue(chengJiuID);
			int chengJiuState = this.GetChengJiuState(chengJiuID);
			if (chengJiuState > 1)
			{
				num2 = num;
			}
			this.ItemsDict[chengJiuID].jinduText.Text = string.Format(Global.GetLang("{0}/{1}"), this.FormatNumString(num2), this.FormatNumString(num));
			this.ItemsDict[chengJiuID].itemState = chengJiuState;
			float num3 = (float)num2 / (float)num;
			this.ItemsDict[chengJiuID].progressBar.Percent = (double)num3;
		}
	}

	private void GetHintGoodsIDInfo(List<ChengJiuNewItem> itemsList, out int pageIndex)
	{
		pageIndex = 0;
	}

	public void RefreshChengJiuPoints(int value)
	{
		if (Global.Data.ChengJiuData != null)
		{
			Global.Data.ChengJiuData.ChengJiuPoints = (long)value;
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
			this.ChengJiuLevel = Global.GetChengJiuLevel((int)chengJiuData.ChengJiuPoints);
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

	public ListBox TypelistBox;

	public ListBox ItemlistBox;

	private Dictionary<string, List<ChengJiuNewItem>> TabItemsDict = new Dictionary<string, List<ChengJiuNewItem>>();

	private Dictionary<int, ChengJiuNewItem> ItemsDict = new Dictionary<int, ChengJiuNewItem>();

	private int ChengJiuLevel;

	private int[] ChengJiuGoodsIDArr;

	private string CurrentShowChengJiuTabPage = "-1";

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<uint> weiLingQUchengJiuArr = new List<uint>();

	private ObservableCollection TypeItemCollection;

	private ObservableCollection ItemCollection;

	private ChengJiuTypeNewItem tempitem;
}
