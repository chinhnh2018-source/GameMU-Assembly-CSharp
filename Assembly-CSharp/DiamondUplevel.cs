using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class DiamondUplevel : UserControl
{
	private void InitTextInPrefabs()
	{
		this.uplevelBtn.Text = Global.GetLang("升级");
		this.title.Text = Global.GetLang("宝石升级");
		this.needGoodsLabel.Text = Global.GetLang("消耗背包内材料");
		this.needCoinsLabel.Text = Global.GetLang("消耗金币：");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.uplevelBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.UplevelDiamondRequest();
		};
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.defaultPosition = this.draggalePanel.transform.localPosition;
	}

	protected override void OnDestroy()
	{
	}

	public GoodsData targetGoodsData
	{
		set
		{
			this.currentGoodData = value;
			this.SetCurrentLevelGoodsIcon(this.currentGoodData);
			GoodsData goodsData = null;
			DiamondAttribute nextLevelDiamondData = Global.GetNextLevelDiamondData((this.currentGoodData == null) ? -1 : this.currentGoodData.GoodsID, out goodsData, (this.levelupType != 0) ? SaleGoodsConsts.EquipedFluorescentDiamondGoodsID : SaleGoodsConsts.FluorescentDiamondGoodsID);
			this.SetNextLevelGoodsIcon(goodsData);
			this.ResetDraggablePanel();
			bool flag = false;
			if (nextLevelDiamondData != null && nextLevelDiamondData.nextLevelGoodsID > 0)
			{
				flag = this.RefreshDiamondNeeded(nextLevelDiamondData.nextLevelGoodsID);
			}
			else
			{
				this.SetTopLevel();
			}
			this.SetUplevelButtonStatus(!flag);
			this.SetCurrentLevelProperty((this.currentGoodData == null) ? -1 : this.currentGoodData.GoodsID);
			this.SetNextLevelProperty((goodsData == null) ? -1 : goodsData.GoodsID);
			DiamondAttribute diamondAttribute = null;
			if (goodsData != null)
			{
				int num = -1;
				diamondAttribute = Global.GetDiamondAttributeByGoodsID(goodsData.GoodsID, out num);
			}
			this.SetNeedCoins((diamondAttribute == null) ? 0 : diamondAttribute.needCoins);
		}
	}

	public int uplevelDiamondType
	{
		set
		{
			this.levelupType = value;
		}
	}

	public void InitGoodsBox(int count)
	{
		if (count > 0)
		{
			int num = (count + 5 - 1) / 5;
			this.rowsInPage = ((num <= this.rowsInPage) ? this.rowsInPage : num);
		}
		this.goodsBox.RowCount = this.rowsInPage;
		this.goodsBox.ColCount = 5;
		this.goodsBox.InitBox();
	}

	private void ResetDraggablePanel()
	{
		if (null == this.draggalePanel)
		{
			return;
		}
		this.draggalePanel.repositionClipping = true;
		this.draggalePanel.transform.localPosition = this.defaultPosition;
	}

	private void ParseSpecailPartID(int specialID, out int slotID, out int shapeType)
	{
		shapeType = specialID / 100;
		slotID = specialID % 100;
	}

	private void SetCurrentLevelGoodsIcon(GoodsData gd)
	{
		if (gd == null)
		{
			return;
		}
		if (null != this.goodIcon_current)
		{
			Object.Destroy(this.goodIcon_current.gameObject);
		}
		this.goodIcon_current = DiamondUplevel.CreateGoodsIcon(gd, false, false, true);
		if (null != this.goodIcon_current && null != this.goodsIconContainer_current)
		{
			this.goodIcon_current.addEventListener("click", new MouseEventHandler(DiamondUplevel.MouseLeftButtonUp));
			U3DUtils.AddChild(this.goodsIconContainer_current, this.goodIcon_current.gameObject, false);
		}
	}

	private void SetNextLevelGoodsIcon(GoodsData gd)
	{
		if (null != this.goodIcon_nextLevel)
		{
			Object.Destroy(this.goodIcon_nextLevel.gameObject);
		}
		this.goodIcon_nextLevel = DiamondUplevel.CreateGoodsIcon(gd, false, false, null != gd);
		if (null != this.goodIcon_nextLevel && null != this.goodsIconContainer_nextLevel)
		{
			this.goodIcon_nextLevel.addEventListener("click", new MouseEventHandler(DiamondUplevel.MouseLeftButtonUp));
			U3DUtils.AddChild(this.goodsIconContainer_nextLevel, this.goodIcon_nextLevel.gameObject, true);
		}
	}

	public static GGoodIcon CreateGoodsIcon(GoodsData goodData, bool grayShow = false, bool showCount = true, bool activeBackground = true)
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.OutSizeX = 78;
		ggoodIcon.OutSizeY = 78;
		ggoodIcon.BackSpriteName0 = ((!activeBackground) ? "bagGridLock_bak" : "bagGrid4_bak");
		if (goodData != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				ggoodIcon.TipType = 1;
				ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
				ggoodIcon.ItemCode = goodData.GoodsID;
				ggoodIcon.ItemObject = goodData;
				ggoodIcon.BoxTypes = -1;
				ggoodIcon.BodyURL = new ImageURL(Global.GetGoodsIconString(int.Parse(goodsXmlNodeByID.IconCode)), false, 0);
				ggoodIcon.EnableIcon = !grayShow;
				ggoodIcon.STextColor = ((!grayShow) ? 16777215U : 16711680U);
				ggoodIcon.SecondText.Text = goodData.GCount.ToString();
				ggoodIcon.TextColor = 15793920U;
				ggoodIcon.ContentText.Text = "Lv" + Global.GetDiamondLevelByGoodsID(goodData.GoodsID);
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
				int diamondLevelByGoodsID = Global.GetDiamondLevelByGoodsID(goodData.GoodsID);
				if (diamondLevelByGoodsID >= 2 && diamondLevelByGoodsID <= 3)
				{
					ggoodIcon.BackSpriteName1 = "iconState_zuoyue";
				}
				if (diamondLevelByGoodsID >= 4 && diamondLevelByGoodsID <= 5)
				{
					ggoodIcon.BackSpriteName1 = "iconState_zuoyue1";
				}
				if (diamondLevelByGoodsID >= 6 && diamondLevelByGoodsID <= 7)
				{
					ggoodIcon.BackSpriteName1 = "iconState_zuoyue2";
				}
				if (diamondLevelByGoodsID >= 8 && diamondLevelByGoodsID <= 12)
				{
					ggoodIcon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
					ggoodIcon.TeXiao.gameObject.SetActive(true);
				}
				bool canUse = Global.CanUseGoods(goodData.GoodsID, false, true);
				Super.InitGoodsGIcon(ggoodIcon, goodData, canUse, IconTextTypes.Qianghua);
				if (!showCount)
				{
					ggoodIcon.SecondText.Text = string.Empty;
				}
			}
		}
		return ggoodIcon;
	}

	private static void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.FluorescentDiamondBagTip, GoodsOwnerTypes.None, goodsData);
	}

	private void SetCurrentLevelProperty(int goodsID)
	{
		if (null == this.property_current)
		{
			return;
		}
		if (goodsID < 0)
		{
			this.property_current.Text = string.Empty;
		}
		else
		{
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsID);
			this.property_current.Text = Global.GetBaseAttributeStrFromPropertyList(goodsEquipPropsDoubleList, true, 0);
		}
	}

	private void SetNextLevelProperty(int goodsID)
	{
		if (null == this.property_nextLevel)
		{
			return;
		}
		if (goodsID < 0)
		{
			this.property_nextLevel.Text = string.Empty;
		}
		else
		{
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsID);
			this.property_nextLevel.Text = Global.GetBaseAttributeStrFromPropertyList(goodsEquipPropsDoubleList, true, 0);
		}
	}

	private void SetNeedCoins(int coins)
	{
		if (null == this.needCoins)
		{
			return;
		}
		int num = Global.GetRoleOwnNumByMoneyType(8) + Global.GetRoleOwnNumByMoneyType(1);
		string text = (num >= coins) ? "ffffff" : "ff0000";
		this.needCoins.Text = Global.GetColorStringForNGUIText(new object[]
		{
			text,
			coins.ToString()
		});
	}

	private void SetUplevelButtonStatus(bool disable = false)
	{
		if (null != this.uplevelBtn)
		{
			this.uplevelBtn.isEnabled = !disable;
		}
	}

	private void SetTopLevel()
	{
		if (null != this.topLevel)
		{
			this.topLevel.SetActive(true);
		}
		this.goodsBox.ClearAllGoodsIcon();
	}

	private bool RefreshDiamondNeeded(int goodsID)
	{
		List<GoodsData> list = null;
		List<GoodsData> list2 = null;
		this.goodsBox.ClearAllGoodsIcon();
		int levelupNeedDiamonds = this.GetLevelupNeedDiamonds(goodsID, out list, out list2, out this.dic_availableDimaond);
		if (levelupNeedDiamonds >= 0)
		{
			List<GoodsData> list3 = new List<GoodsData>();
			int partitionIndex = -1;
			if (list != null && list.Count > 0)
			{
				list3.AddRange(list);
				partitionIndex = list.Count;
			}
			if (list2 != null && list2.Count > 0)
			{
				list3.AddRange(list2);
			}
			this.RefreshDiamondList(list3, partitionIndex);
		}
		return 1 == levelupNeedDiamonds;
	}

	private void RefreshDiamondList(List<GoodsData> goodsDataList, int partitionIndex)
	{
		this.goodsBox.ClearAllGoodsIcon();
		if (goodsDataList == null || goodsDataList.Count <= 0)
		{
			return;
		}
		this.InitGoodsBox(goodsDataList.Count);
		for (int i = 0; i < goodsDataList.Count; i++)
		{
			GoodsData goodData = goodsDataList[i];
			GGoodIcon ggoodIcon = DiamondUplevel.CreateGoodsIcon(goodData, i >= partitionIndex, true, true);
			if (null != ggoodIcon)
			{
				if (ggoodIcon.GetComponent<UIPanel>())
				{
					Object.Destroy(ggoodIcon.GetComponent<UIPanel>());
				}
				this.goodsBox.SetGoodsIcon(this.Getindex(i), ggoodIcon);
				ggoodIcon.addEventListener("click", new MouseEventHandler(DiamondUplevel.MouseLeftButtonUp));
			}
		}
	}

	private int GetLevelupNeedDiamonds(int goodsID, out List<GoodsData> list_availableDiamond, out List<GoodsData> list_needed, out Dictionary<int, int> dic_diamondCountNeeded)
	{
		list_availableDiamond = null;
		list_needed = null;
		dic_diamondCountNeeded = null;
		if (goodsID <= 0)
		{
			return -1;
		}
		int num = -1;
		DiamondAttribute diamondAttributeByGoodsID = Global.GetDiamondAttributeByGoodsID(goodsID, out num);
		if (diamondAttributeByGoodsID == null || num < 0)
		{
			return -2;
		}
		int previousGoodsID = diamondAttributeByGoodsID.previousGoodsID;
		List<GoodsData> list = Global.GetUpgradableDiamondByType(diamondAttributeByGoodsID.type, num, previousGoodsID);
		if (list == null)
		{
			list = new List<GoodsData>();
		}
		list.Sort(new Comparison<GoodsData>(this.SortGoodsDataList));
		int num2 = diamondAttributeByGoodsID.needLevelOneNum;
		if (this.levelupType == 1)
		{
			DiamondAttribute diamondAttributeByGoodsID2 = Global.GetDiamondAttributeByGoodsID(this.currentGoodData.GoodsID, out num);
			if (diamondAttributeByGoodsID2 != null)
			{
				num2 -= diamondAttributeByGoodsID2.needLevelOneNum;
			}
		}
		if (list == null || list.Count <= 0)
		{
			list_needed = new List<GoodsData>();
			GoodsData dummyLevelOneDiamondByType = this.GetDummyLevelOneDiamondByType(diamondAttributeByGoodsID.type, num, num2);
			list_needed.Add(dummyLevelOneDiamondByType);
			return 0;
		}
		list_availableDiamond = new List<GoodsData>();
		dic_diamondCountNeeded = new Dictionary<int, int>();
		int num3 = 0;
		for (int i = 0; i < list.Count; i++)
		{
			GoodsData goodsData = list[i];
			DiamondAttribute diamondAttributeByGoodsID3 = Global.GetDiamondAttributeByGoodsID(goodsData.GoodsID, out num);
			if (diamondAttributeByGoodsID3 != null)
			{
				int num4 = (num2 - num3 + diamondAttributeByGoodsID3.needLevelOneNum - 1) / diamondAttributeByGoodsID3.needLevelOneNum;
				int num5 = Mathf.Min(num4, goodsData.GCount);
				if (goodsData.Site == 7000)
				{
					dic_diamondCountNeeded.Add(goodsData.BagIndex, num5);
				}
				GoodsData goodsData2 = Global.CloneGoodsData(goodsData, false);
				goodsData2.GCount = num5;
				list_availableDiamond.Add(goodsData2);
				num3 += num5 * diamondAttributeByGoodsID3.needLevelOneNum;
				if (num3 >= num2)
				{
					break;
				}
			}
		}
		if (num3 < num2)
		{
			list_needed = new List<GoodsData>();
			GoodsData dummyLevelOneDiamondByType2 = this.GetDummyLevelOneDiamondByType(diamondAttributeByGoodsID.type, num, num2 - num3);
			list_needed.Add(dummyLevelOneDiamondByType2);
			return 0;
		}
		return 1;
	}

	private GoodsData GetDummyLevelOneDiamondByType(int type, int shapeType, int count)
	{
		int goodsID = -1;
		DiamondAttribute levelOneDiamondByType = Global.GetLevelOneDiamondByType(type, shapeType, out goodsID);
		GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsID);
		dummyGoodsData.GCount = count;
		return dummyGoodsData;
	}

	private int SortGoodsDataList(GoodsData x, GoodsData y)
	{
		if (y.GoodsID == x.GoodsID)
		{
			return y.GCount - x.GCount;
		}
		return y.GoodsID - x.GoodsID;
	}

	private int GetGoodsIconIndex(GoodsData goodsData)
	{
		return this.Getindex(goodsData.BagIndex);
	}

	private int Getindex(int bagIndex)
	{
		int result = -1;
		if (this.bagOrient == BagOrientTypes.Vertical && !this.isPage)
		{
			result = bagIndex;
		}
		else if (this.bagOrient == BagOrientTypes.Horizontal && this.isPage)
		{
			int num = 5;
			int num2 = this.rowsInPage;
			int num3 = 0 / this.rowsInPage;
			this.goodsBox.listBox.maxPerLine = num3;
			int num4 = bagIndex / num / num2;
			int num5 = bagIndex % (num * num2);
			int num6 = num5 % num;
			int num7 = num5 / num % num2;
			result = num6 + num7 * num3 + num4 * num;
		}
		return result;
	}

	private void PlayAnimation(Animation anim, ActiveAnimation.OnFinished onFinished)
	{
		ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, 1);
		if (activeAnimation == null)
		{
			return;
		}
		activeAnimation.onFinished = onFinished;
	}

	private void PlayFinished(ActiveAnimation anim)
	{
		anim.gameObject.SetActive(false);
	}

	public void SetUplevelDiamondResult(int status, int dbid)
	{
		Super.HideNetWaiting();
		string textMsg = string.Empty;
		switch (status + 2)
		{
		case 0:
			textMsg = Global.GetLang("功能未开启");
			break;
		case 1:
			textMsg = Global.GetLang("异常");
			break;
		case 2:
			this.OnLevelupSucceed(dbid);
			break;
		case 3:
			textMsg = Global.GetLang("物品不存在");
			break;
		case 4:
			textMsg = Global.GetLang("升级数据错误");
			break;
		case 5:
			textMsg = Global.GetLang("宝石已达最高级");
			break;
		case 6:
			textMsg = Global.GetLang("下一级宝石数据异常");
			break;
		case 7:
			textMsg = Global.GetLang("金币不足");
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
			return;
		case 8:
			textMsg = Global.GetLang("部位索引错误");
			break;
		case 9:
			textMsg = Global.GetLang("宝石类型错误");
			break;
		case 10:
			textMsg = Global.GetLang("宝石不足");
			break;
		case 11:
			textMsg = Global.GetLang("新增物品失败");
			break;
		case 12:
			textMsg = Global.GetLang("扣除物品失败");
			break;
		case 13:
			textMsg = Global.GetLang("要扣除的物品不存在");
			break;
		case 14:
			textMsg = Global.GetLang("要扣除的物品不足");
			break;
		case 15:
			textMsg = Global.GetLang("装备宝石失败");
			break;
		case 16:
			textMsg = Global.GetLang("不是荧光宝石");
			break;
		case 17:
			textMsg = Global.GetLang("宝石背包空间不足");
			break;
		}
		if (status != 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
		}
	}

	private void OnLevelupSucceed(int dbid)
	{
		if (null != this.levelupAnimation)
		{
			this.levelupAnimation.gameObject.SetActive(true);
			this.PlayAnimation(this.levelupAnimation, new ActiveAnimation.OnFinished(this.PlayFinished));
		}
		GoodsData goodsData = null;
		if (this.levelupType == 0)
		{
			if (dbid >= 0)
			{
				goodsData = Global.GetDiamondGoodsDataByDBID(dbid);
			}
		}
		else if (this.levelupType == 1)
		{
			Dictionary<int, GoodsData> equipedDiamondsBySlotID = Global.GetEquipedDiamondsBySlotID(this.index);
			if (equipedDiamondsBySlotID != null)
			{
				goodsData = equipedDiamondsBySlotID.GetValue(this.shapeType);
				if (goodsData != null)
				{
					goodsData.Id = this.currentGoodData.Id;
				}
			}
		}
		this.targetGoodsData = goodsData;
		if (this.levelupType == 1)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 101
			});
		}
	}

	private void UplevelDiamondRequest()
	{
		if (this.currentGoodData == null)
		{
			return;
		}
		Super.ShowNetWaiting(null);
		if (this.levelupType == 0)
		{
			this.index = this.currentGoodData.BagIndex;
		}
		else if (this.levelupType == 1)
		{
			this.ParseSpecailPartID(this.currentGoodData.Id, out this.index, out this.shapeType);
		}
		GameInstance.Game.UplevelDiamond(this.levelupType, this.index, this.shapeType, this.dic_availableDimaond);
	}

	private const int maxGridCount = 10;

	private const int columnsInPage = 5;

	private const int columns = 0;

	private const int aGridSize = 78;

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler callback;

	public GButton uplevelBtn;

	public GButton closeBtn;

	public UIDraggablePanel draggalePanel;

	private Vector3 defaultPosition;

	public GGoodsBox goodsBox;

	public TextBlock title;

	public TextBlock needGoodsLabel;

	public TextBlock needCoinsLabel;

	public TextBlock property_current;

	public TextBlock property_nextLevel;

	public TextBlock needCoins;

	public GameObject goodsIconContainer_current;

	public GameObject goodsIconContainer_nextLevel;

	private GGoodIcon goodIcon_current;

	private GGoodIcon goodIcon_nextLevel;

	public GameObject topLevel;

	public Animation levelupAnimation;

	private GoodsData currentGoodData;

	private int levelupType;

	private BagOrientTypes bagOrient = BagOrientTypes.Vertical;

	private bool isPage;

	private int rowsInPage = 2;

	private Dictionary<int, int> dic_availableDimaond;

	private int index = -1;

	private int shapeType = -1;
}
