using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class OpenServerActivePart : UserControl
{
	// Note: this type is marked as 'beforefieldinit'.
	static OpenServerActivePart()
	{
		string[,] array = new string[9, 2];
		array[0, 0] = Global.GetLang("冲级狂人");
		array[0, 1] = "Config/XinFuGifts/MuLevel.xml";
		array[1, 0] = Global.GetLang("屠魔勇士");
		array[1, 1] = "Config/XinFuGifts/MuBoss.xml";
		array[2, 0] = Global.GetLang("充值达人");
		array[2, 1] = "Config/XinFuGifts/MuChongZhi.xml";
		array[3, 0] = Global.GetLang("消费达人");
		array[3, 1] = "Config/XinFuGifts/MuXiaoFei.xml";
		array[4, 0] = Global.GetLang("劲爆返利");
		array[4, 1] = "Config/XinFuGifts/LeiJiXiaoFei.xml";
		array[5, 0] = Global.GetLang("冲级狂人双倍");
		array[5, 1] = "Config/XinFuGifts/MuDoubleLevel.xml";
		array[6, 0] = Global.GetLang("屠魔勇士双倍");
		array[6, 1] = "Config/XinFuGifts/MuDoubleBoss.xml";
		array[7, 0] = Global.GetLang("充值达人双倍");
		array[7, 1] = "Config/XinFuGifts/MuDoubleChongZhi.xml";
		array[8, 0] = Global.GetLang("消费达人双倍");
		array[8, 1] = "Config/XinFuGifts/MuDoubleXiaoFei.xml";
		OpenServerActivePart.OperServerActiveItemNames = array;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemList.IsPosYFixed = true;
		this.initList();
		this.InitBtnProc();
	}

	private void InitBtnProc()
	{
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			};
		}
	}

	private void initList()
	{
		if (this.items == null)
		{
			this.items = new OpenServerActivePartItem[this.itemsCount];
		}
		for (int i = 0; i < this.itemsCount; i++)
		{
			if (this.items[i] == null)
			{
				this.items[i] = U3DUtils.NEW<OpenServerActivePartItem>();
				this.items[i].ItemHeadBak.URL = Global.GetGameResImageString(string.Format("kaifuItem{0}.jpg", i + 1));
				this.items[i].ItemIndex = i;
				int id = 0;
				if (i == 0)
				{
					id = 33;
				}
				else if (i == 1)
				{
					id = 36;
				}
				else if (i == 2)
				{
					id = 34;
				}
				else if (i == 3)
				{
					id = 35;
				}
				if (this.OpenXinFuDouble(id))
				{
					this.items[i].IconDouble.SetActive(true);
				}
				else
				{
					this.items[i].IconDouble.SetActive(false);
				}
			}
			this.items[i].DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.initItemPart(e.ID);
			};
			U3DUtils.AddChild(this.ItemList.gameObject, this.items[i].gameObject, false);
		}
	}

	public bool OpenXinFuDouble(int Id)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		DateTime dateTime3 = default(DateTime);
		bool flag = false;
		bool flag2 = false;
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("DoubleXinFu", '|');
		if (systemParamStringArrayByName.Length == 0)
		{
			return false;
		}
		DateTime.TryParse(string.Format("{0}", Global.Data.roleData.KaiFuStartDay), ref dateTime3);
		DateTime.TryParse(systemParamStringArrayByName[0], ref dateTime);
		DateTime.TryParse(systemParamStringArrayByName[1], ref dateTime2);
		if (dateTime3.Ticks >= dateTime.Ticks && dateTime3.Ticks <= dateTime2.Ticks && dateTime3.AddDays(9.0).Ticks >= correctDateTime.Ticks)
		{
			flag = true;
		}
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("DoubleXinFuID", ',');
		for (int i = 0; i < systemParamIntArrayByName.Length; i++)
		{
			if (systemParamIntArrayByName[i] == Id)
			{
				flag2 = true;
			}
		}
		return flag && flag2;
	}

	private void setTab(int index = -1)
	{
		if (index >= 0)
		{
			if (this.selectedIndex >= 0)
			{
				if (index == this.selectedIndex)
				{
					this.items[this.selectedIndex].ToggleState = !this.items[this.selectedIndex].ToggleState;
					return;
				}
				if (this.items[this.selectedIndex].ToggleState)
				{
					this.items[this.selectedIndex].ToggleState = false;
				}
			}
			this.selectedIndex = index;
			this.items[this.selectedIndex].ToggleState = true;
		}
	}

	private void tweenPosPart(int index)
	{
		this.toggleState = !this.toggleState;
		if (index == this.selectedIndex)
		{
			Vector3 vector = this.fromPos;
			this.fromPos = this.toPos;
			this.toPos = vector;
			this.PartTweenPosition.Play(this.toggleState);
		}
		else
		{
			int num = -(index * 181) - 7;
			this.fromPos = this.toPos;
			this.toPos = new Vector3((float)num, this.toPos.y, this.toPos.z);
			this.PartTweenPosition.from = this.fromPos;
			this.PartTweenPosition.to = this.toPos;
			this.PartTweenPosition.Play(this.toggleState);
		}
	}

	public void RefreshActivePartItem(int index, bool canGain)
	{
		if (index >= 0 && index < this.items.Length && !this.toggleState)
		{
			if (canGain)
			{
				this.items[index].CurrentState = OpenServerActivePartItem.IconState.CanGain;
			}
			else
			{
				this.items[index].CurrentState = OpenServerActivePartItem.IconState.ClickForLook;
			}
		}
	}

	private void initItemPart(int index)
	{
		this.tweenPosPart(index);
		switch (index)
		{
		case 0:
			if (null == this.ActiveLevelPart)
			{
				this.ActiveLevelPart = U3DUtils.NEW<OpenServerActiveLevelPart>();
				if (this.OpenXinFuDouble(index + 33))
				{
					this.ActiveLevelPart.LoadConfigFromXML(OpenServerActivePart.OperServerActiveItemNames[index + 5, 1], OpenServerActiveType.LEVEL);
				}
				else
				{
					this.ActiveLevelPart.LoadConfigFromXML(OpenServerActivePart.OperServerActiveItemNames[index, 1], OpenServerActiveType.LEVEL);
				}
				this.items[index].Content.Add(this.ActiveLevelPart);
			}
			this.ActiveLevelPart.ResetPanelGiftsPos();
			if (this.ActiveLevelData != null)
			{
				this.ActiveLevelPart.InitDataFromServerInfoForLevel(this.ActiveLevelData);
				bool totalCanGainStateForLevel = OpenServerActiveLevelPart.GetTotalCanGainStateForLevel(this.ActiveLevelData);
				if (totalCanGainStateForLevel)
				{
					this.RefreshActivePartItem(0, true);
				}
			}
			break;
		case 1:
			if (null == this.BossKingPart)
			{
				this.BossKingPart = U3DUtils.NEW<OpenServerActiveLevelPart>();
				if (this.OpenXinFuDouble(index + 35))
				{
					this.BossKingPart.LoadConfigFromXML(OpenServerActivePart.OperServerActiveItemNames[index + 5, 1], OpenServerActiveType.BossKing);
				}
				else
				{
					this.BossKingPart.LoadConfigFromXML(OpenServerActivePart.OperServerActiveItemNames[index, 1], OpenServerActiveType.BossKing);
				}
				this.items[index].Content.Add(this.BossKingPart);
			}
			this.BossKingPart.ResetPanelGiftsPos();
			if (this.BossKingData != null)
			{
				this.BossKingPart.InitDataFromServerInfo(this.BossKingData);
				bool totalCanGainStateForOther = OpenServerActiveLevelPart.GetTotalCanGainStateForOther(this.BossKingData, OpenServerActiveType.BossKing);
				if (totalCanGainStateForOther)
				{
					this.RefreshActivePartItem(1, true);
				}
			}
			break;
		case 2:
			if (null == this.ChongZhiKingPart)
			{
				this.ChongZhiKingPart = U3DUtils.NEW<OpenServerActiveLevelPart>();
				if (this.OpenXinFuDouble(index + 32))
				{
					this.ChongZhiKingPart.LoadConfigFromXML(OpenServerActivePart.OperServerActiveItemNames[index + 5, 1], OpenServerActiveType.ChongZhiKing);
				}
				else
				{
					this.ChongZhiKingPart.LoadConfigFromXML(OpenServerActivePart.OperServerActiveItemNames[index, 1], OpenServerActiveType.ChongZhiKing);
				}
				this.items[index].Content.Add(this.ChongZhiKingPart);
			}
			this.ChongZhiKingPart.ResetPanelGiftsPos();
			if (this.ChongZhiKingData != null)
			{
				this.ChongZhiKingPart.InitDataFromServerInfo(this.ChongZhiKingData);
				bool totalCanGainStateForOther2 = OpenServerActiveLevelPart.GetTotalCanGainStateForOther(this.ChongZhiKingData, OpenServerActiveType.ChongZhiKing);
				if (totalCanGainStateForOther2)
				{
					this.RefreshActivePartItem(2, true);
				}
			}
			break;
		case 3:
			if (null == this.XiaoFeiKingPart)
			{
				this.XiaoFeiKingPart = U3DUtils.NEW<OpenServerActiveLevelPart>();
				if (this.OpenXinFuDouble(index + 32))
				{
					this.XiaoFeiKingPart.LoadConfigFromXML(OpenServerActivePart.OperServerActiveItemNames[index + 5, 1], OpenServerActiveType.XiaoFeiKing);
				}
				else
				{
					this.XiaoFeiKingPart.LoadConfigFromXML(OpenServerActivePart.OperServerActiveItemNames[index, 1], OpenServerActiveType.XiaoFeiKing);
				}
				this.items[index].Content.Add(this.XiaoFeiKingPart);
			}
			this.XiaoFeiKingPart.ResetPanelGiftsPos();
			if (this.XiaoFeiKingData != null)
			{
				this.XiaoFeiKingPart.InitDataFromServerInfo(this.XiaoFeiKingData);
				bool totalCanGainStateForOther3 = OpenServerActiveLevelPart.GetTotalCanGainStateForOther(this.XiaoFeiKingData, OpenServerActiveType.XiaoFeiKing);
				if (totalCanGainStateForOther3)
				{
					this.RefreshActivePartItem(3, true);
				}
			}
			break;
		case 4:
			if (null == this.FanLiPart)
			{
				this.FanLiPart = U3DUtils.NEW<OpenServerActiveFanLiPart>();
				this.FanLiPart.LoadConfigFromXML(OpenServerActivePart.OperServerActiveItemNames[index, 1]);
				this.items[index].Content.Add(this.FanLiPart);
			}
			if (this.FanLiData != null)
			{
				this.FanLiPart.InitDataFromServerInfo(this.FanLiData);
				bool totalCanGainState = OpenServerActiveFanLiPart.GetTotalCanGainState(this.FanLiData);
				if (totalCanGainState)
				{
					this.RefreshActivePartItem(4, true);
				}
			}
			break;
		}
		this.setTab(index);
	}

	public void RefreshDataFromGainedInfo(int activetype, int rankindex)
	{
		switch (activetype)
		{
		case 33:
			if (this.ActiveLevelPart != null)
			{
				this.ActiveLevelPart.RefreshDataFromGainedInfo(rankindex);
			}
			GameInstance.Game.SpriteQueryNewZoneLevelKing();
			break;
		case 34:
			if (this.ChongZhiKingPart != null)
			{
				this.ChongZhiKingPart.RefreshDataFromGainedInfo(rankindex);
			}
			GameInstance.Game.SpriteQueryNewZoneChongZhiKing();
			break;
		case 35:
			if (this.XiaoFeiKingPart != null)
			{
				this.XiaoFeiKingPart.RefreshDataFromGainedInfo(rankindex);
			}
			GameInstance.Game.SpriteQueryNewZoneXiaoFeiKing();
			break;
		case 36:
			if (this.BossKingPart != null)
			{
				this.BossKingPart.RefreshDataFromGainedInfo(rankindex);
			}
			GameInstance.Game.SpriteQueryNewZoneBossKing();
			break;
		case 37:
			if (this.FanLiPart != null)
			{
				this.FanLiPart.m_BtnLingQv.isEnabled = false;
				this.FanLiPart.m_TxtFanLi.text = "0";
			}
			GameInstance.Game.SpriteQueryNewZoneFanLiKing();
			break;
		}
	}

	public void SendActivityQueryRequest()
	{
		GameInstance.Game.SpriteQueryNewZoneLevelKing();
		GameInstance.Game.SpriteQueryNewZoneBossKing();
		GameInstance.Game.SpriteQueryNewZoneChongZhiKing();
		GameInstance.Game.SpriteQueryNewZoneXiaoFeiKing();
		GameInstance.Game.SpriteQueryNewZoneFanLiKing();
	}

	public void OnQueryUpLevelKingActivityCompleted(NewZoneUpLevelData upLevelData)
	{
		this.ActiveLevelData = upLevelData;
		if (this.ActiveLevelPart != null)
		{
			this.ActiveLevelPart.InitDataFromServerInfoForLevel(this.ActiveLevelData);
		}
		bool totalCanGainStateForLevel = OpenServerActiveLevelPart.GetTotalCanGainStateForLevel(upLevelData);
		if (totalCanGainStateForLevel)
		{
			this.RefreshActivePartItem(0, true);
		}
	}

	public void OnQueryNewZoneActiveCompleted(NewZoneActiveData activeData)
	{
		if (activeData == null)
		{
			return;
		}
		switch (activeData.ActiveId)
		{
		case 34:
			this.OnQueryChongZhiKingActivityCompleted(activeData);
			break;
		case 35:
			this.OnQueryXiaoFeiKingActivityCompleted(activeData);
			break;
		case 36:
			this.OnQueryBossKingActivityCompleted(activeData);
			break;
		case 37:
			this.OnQueryInputFanLiExCompleted(activeData);
			break;
		}
	}

	private void OnQueryBossKingActivityCompleted(NewZoneActiveData activeData)
	{
		this.BossKingData = activeData;
		if (this.BossKingPart != null)
		{
			this.BossKingPart.InitDataFromServerInfo(this.BossKingData);
		}
		bool totalCanGainStateForOther = OpenServerActiveLevelPart.GetTotalCanGainStateForOther(activeData, OpenServerActiveType.BossKing);
		if (totalCanGainStateForOther)
		{
			this.RefreshActivePartItem(1, true);
		}
	}

	private void OnQueryChongZhiKingActivityCompleted(NewZoneActiveData activeData)
	{
		this.ChongZhiKingData = activeData;
		if (this.ChongZhiKingPart != null)
		{
			this.ChongZhiKingPart.InitDataFromServerInfo(this.ChongZhiKingData);
		}
		bool totalCanGainStateForOther = OpenServerActiveLevelPart.GetTotalCanGainStateForOther(activeData, OpenServerActiveType.ChongZhiKing);
		if (totalCanGainStateForOther)
		{
			this.RefreshActivePartItem(2, true);
		}
	}

	private void OnQueryXiaoFeiKingActivityCompleted(NewZoneActiveData activeData)
	{
		this.XiaoFeiKingData = activeData;
		if (this.XiaoFeiKingPart != null)
		{
			this.XiaoFeiKingPart.InitDataFromServerInfo(this.XiaoFeiKingData);
		}
		bool totalCanGainStateForOther = OpenServerActiveLevelPart.GetTotalCanGainStateForOther(activeData, OpenServerActiveType.XiaoFeiKing);
		if (totalCanGainStateForOther)
		{
			this.RefreshActivePartItem(3, true);
		}
	}

	private void OnQueryInputFanLiExCompleted(NewZoneActiveData activeData)
	{
		if (activeData != null)
		{
			this.FanLiData = activeData;
			if (this.FanLiPart != null)
			{
				this.FanLiPart.InitDataFromServerInfo(activeData);
			}
			bool totalCanGainState = OpenServerActiveFanLiPart.GetTotalCanGainState(activeData);
			if (totalCanGainState)
			{
				this.RefreshActivePartItem(4, true);
			}
		}
	}

	public GButton m_BtnClose;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UITable ItemList;

	public TweenPosition PartTweenPosition;

	private OpenServerActivePartItem[] items;

	private int itemsCount = 5;

	private int selectedIndex = -1;

	private Vector3 fromPos = Vector3.zero;

	private Vector3 toPos = Vector3.zero;

	private bool toggleState;

	private static string[,] OperServerActiveItemNames;

	private OpenServerActiveLevelPart ActiveLevelPart;

	private OpenServerActiveLevelPart BossKingPart;

	private OpenServerActiveLevelPart ChongZhiKingPart;

	private OpenServerActiveLevelPart XiaoFeiKingPart;

	private OpenServerActiveFanLiPart FanLiPart;

	private NewZoneUpLevelData ActiveLevelData;

	private NewZoneActiveData BossKingData;

	private NewZoneActiveData ChongZhiKingData;

	private NewZoneActiveData XiaoFeiKingData;

	private NewZoneActiveData FanLiData;
}
