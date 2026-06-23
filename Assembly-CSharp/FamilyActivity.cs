using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class FamilyActivity : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.TabBtnOBC = this.ListTabBtn.ItemsSource;
		this.InitBtnItem();
		this.ListTabBtn.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectedBtn);
		this.ListTabBtn.SelectedIndex = 0;
	}

	private void InitBtnItem()
	{
		if (this.BtnClose != null)
		{
			this.BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			};
		}
		XElement gameResXml = Global.GetGameResXml("Config/ZhanMengHuoDongTab.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HuoDong");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				if (xelementAttributeInt == 5)
				{
					ConfigZhanMengLianSaiLeagueOpen configZhanMengLianSaiLeagueOpen = new ConfigZhanMengLianSaiLeagueOpen();
					if (configZhanMengLianSaiLeagueOpen != null && configZhanMengLianSaiLeagueOpen.GetLianSaiISOpenOnPlatform())
					{
						GameInstance.Game.SendZhanMengLianSaiCompetitionEnterState(true);
					}
				}
				else
				{
					FamilyActivityBtnitem familyActivityBtnitem = U3DUtils.NEW<FamilyActivityBtnitem>();
					familyActivityBtnitem.label.text = xelementAttributeStr;
					familyActivityBtnitem.label.color = NGUIMath.HexToColorEx(8350293U);
					familyActivityBtnitem.TipIcon.gameObject.SetActive(false);
					familyActivityBtnitem.Id = xelementAttributeInt;
					this.TabBtnOBC.AddNoUpdate(familyActivityBtnitem);
				}
			}
		}
		ActivityTipManager.RegActivityTipItem(15001, delegate(int s, ActivityTipItem e)
		{
			if (this.TabBtnOBC == null)
			{
				return;
			}
			FamilyActivityBtnitem familyActivityBtnitem2 = U3DUtils.AS<FamilyActivityBtnitem>(this.TabBtnOBC[0]);
			familyActivityBtnitem2.TipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(15002, delegate(int s, ActivityTipItem e)
		{
			if (this.TabBtnOBC == null)
			{
				return;
			}
			FamilyActivityBtnitem familyActivityBtnitem2 = U3DUtils.AS<FamilyActivityBtnitem>(this.TabBtnOBC[2]);
			familyActivityBtnitem2.TipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(15004, delegate(int s, ActivityTipItem e)
		{
			if (this.TabBtnOBC == null)
			{
				return;
			}
			FamilyActivityBtnitem familyActivityBtnitem2 = U3DUtils.AS<FamilyActivityBtnitem>(this.TabBtnOBC[3]);
			familyActivityBtnitem2.TipIcon.gameObject.SetActive(e.IsActive);
		});
	}

	private ConfigZhanMengLianSaiLeagueMatch ConfigZhanMengLianSaiLeagueMatch
	{
		get
		{
			if (this.mConfigZhanMengLianSaiLeagueMatch == null)
			{
				this.mConfigZhanMengLianSaiLeagueMatch = new ConfigZhanMengLianSaiLeagueMatch();
			}
			return this.mConfigZhanMengLianSaiLeagueMatch;
		}
	}

	public void RefreshZhanMengLianSaiState(int type, BangHuiMatchGameStates state, BangHuiMatchType OldBangHuiMatchType, int Rank, int NextBeginTime)
	{
		if (type == 0)
		{
			return;
		}
		this.mBangHuiMatchGameStates = state;
		this.mBangHuiMatchType = type;
		this.mOldBangHuiMatchType = OldBangHuiMatchType;
		this.mRank = Rank;
		this.mNextBeginTime = NextBeginTime;
		byte b = 0;
		for (int i = 0; i < this.TabBtnOBC.Count; i++)
		{
			GameObject at = this.TabBtnOBC.GetAt(i);
			if (null != at)
			{
				FamilyActivityBtnitem component = at.GetComponent<FamilyActivityBtnitem>();
				if (component.Id == 5)
				{
					b = 1;
					break;
				}
			}
		}
		if (b == 0)
		{
			XElement gameResXml = Global.GetGameResXml("Config/ZhanMengHuoDongTab.xml");
			if (gameResXml != null)
			{
				XElement xelement = Global.GetXElement(gameResXml, "HuoDong", "ID", "5");
				if (xelement != null)
				{
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
					FamilyActivityBtnitem familyActivityBtnitem = U3DUtils.NEW<FamilyActivityBtnitem>();
					familyActivityBtnitem.label.text = xelementAttributeStr;
					familyActivityBtnitem.label.color = NGUIMath.HexToColorEx(8350293U);
					familyActivityBtnitem.TipIcon.gameObject.SetActive(3 == state);
					familyActivityBtnitem.Id = xelementAttributeInt;
					this.TabBtnOBC.AddNoUpdate(familyActivityBtnitem);
					this.ListTabBtn.repositionNow = true;
				}
			}
		}
		if (null != this.mZhanMengLianSaiMainPart)
		{
			this.mZhanMengLianSaiMainPart.RefreshInf(this.mBangHuiMatchType, this.mOldBangHuiMatchType, this.mRank, this.mBangHuiMatchGameStates, this.ConfigZhanMengLianSaiLeagueMatch, this.mNextBeginTime);
		}
	}

	private void SelectedBtn(object sender, MouseEvent e)
	{
		FamilyActivityBtnitem familyActivityBtnitem = U3DUtils.AS<FamilyActivityBtnitem>(this.ListTabBtn.SelectedItem);
		if (null == familyActivityBtnitem)
		{
			return;
		}
		if (familyActivityBtnitem.Id == 3 && Global.zhanmengLevel < this.GetZhanmengLevelLimit())
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.LangHunLingYu, ref trigger, ref param, ref param2);
			UIHelper.HintGongNengOpenCondition(GongNengIDs.LangHunLingYu, trigger, param, param2, true);
			return;
		}
		if (familyActivityBtnitem.Id == 4)
		{
			int trigger2 = 0;
			int param3 = 0;
			int param4 = 0;
			int num = 6;
			SystemOpenVO systemOpenVOByID = ConfigSystemOpen.GetSystemOpenVOByID(84);
			if (systemOpenVOByID != null)
			{
				num = systemOpenVOByID.TimeParameters;
			}
			if (Global.zhanmengLevel < num)
			{
				GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhengDuoZhiDi, ref trigger2, ref param3, ref param4);
				UIHelper.HintGongNengOpenCondition(GongNengIDs.ZhengDuoZhiDi, trigger2, param3, param4, true);
				return;
			}
		}
		if (this.familyBtnItem != null && this.familyBtnItem != familyActivityBtnitem)
		{
			this.familyBtnItem.Bak.spriteName = "btn_left_normal";
			this.familyBtnItem.label.color = NGUIMath.HexToColorEx(8350293U);
		}
		if (familyActivityBtnitem == this.familyBtnItem)
		{
			return;
		}
		this.familyBtnItem = familyActivityBtnitem;
		this.familyBtnItem.Bak.spriteName = "btn_left_selected";
		familyActivityBtnitem.label.color = NGUIMath.HexToColorEx(15461355U);
		this.ShowPage(familyActivityBtnitem);
	}

	public void ShowPage(FamilyActivityBtnitem item)
	{
		this.SprPnlContent.Clear();
		this.m_FamilyBossPart = null;
		this.m_luolanPart = null;
		this.m_wolfsoulfieldPart = null;
		if (null != this.mZhanMengLianSaiMainPart)
		{
			this.mZhanMengLianSaiMainPart.gameObject.SetActive(false);
		}
		switch (item.Id)
		{
		case 1:
			this.m_FamilyBossPart = U3DUtils.NEW<FamilyBossPart>();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_FamilyBossPart.gameObject, true);
			break;
		case 2:
			if (null == this.m_luolanPart)
			{
				this.m_luolanPart = U3DUtils.NEW<luolan_part>();
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_luolanPart.gameObject, true);
				this.m_luolanPart.chengZhanShenQing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 10
					});
				};
				this.m_luolanPart.yanHui.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					if (null == this.m_luolanPart)
					{
						return;
					}
					if (this.m_luolanPart.LuoLanChengZhuInfo != null)
					{
						if (this.m_luolanPart.LuoLanChengZhuInfo.BHName == null || this.m_luolanPart.LuoLanChengZhuInfo.BHName == string.Empty)
						{
							Super.HintMainText(Global.GetLang("罗兰城主尚未产生"), 10, 3);
							return;
						}
						MUDebug.Log<string>(new string[]
						{
							string.Empty + Global.HaveYanHui
						});
						if (this.m_luolanPart.LuoLanChengZhuInfo.RoleInfoList[0].RoleID != Global.Data.RoleID)
						{
							GameInstance.Game.GetYanHuiInfo();
						}
						else
						{
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								ID = 12
							});
						}
					}
				};
				this.m_luolanPart.chaKanGuiZe.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					if (null == this.m_luoLanRolePart)
					{
						this.m_luoLanRolePart = U3DUtils.NEW<LuoLanRolePart>();
						U3DUtils.AddChild(this.PnlContent.gameObject, this.m_luoLanRolePart.gameObject, true);
						if (null != this.m_luolanPart)
						{
							this.m_luolanPart.gameObject.SetActive(false);
						}
						this.m_luoLanRolePart.DPSelectedItem = delegate(object ss, DPSelectedItemEventArgs ee)
						{
							Object.Destroy(this.m_luoLanRolePart.gameObject);
							this.m_luoLanRolePart = null;
							this.m_luolanPart.gameObject.SetActive(true);
						};
					}
				};
			}
			break;
		case 3:
			if (null == this.m_wolfsoulfieldPart)
			{
				this.m_wolfsoulfieldPart = U3DUtils.NEW<WolfSoulField_Part>();
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_wolfsoulfieldPart.gameObject, true);
				this.m_wolfsoulfieldPart.DPSelecetItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.ID == -1)
					{
						PlayZone.GlobalPlayZone.OpenWolfSoulFieldRuleWindow();
					}
					if (e.ID == -2)
					{
						PlayZone.GlobalPlayZone.OpenWolfSoulFieldAwardWindow();
					}
					if (e.ID == -3)
					{
						PlayZone.GlobalPlayZone.OpenWolfSoulFieldAttackWindow();
					}
					if (e.ID == -4)
					{
						PlayZone.GlobalPlayZone.OpenWolfSoulFieldWorldWindow();
					}
				};
			}
			break;
		case 4:
			if (null == this.competeCityPart)
			{
				this.competeCityPart = U3DUtils.NEW<CompeteCityPart>();
				U3DUtils.AddChild(this.PnlContent.gameObject, this.competeCityPart.gameObject, true);
				this.competeCityPart.Handler = delegate(object s, DPSelectedItemEventArgs e)
				{
				};
			}
			break;
		case 5:
			if (null == this.mZhanMengLianSaiMainPart)
			{
				this.mZhanMengLianSaiMainPart = U3DUtils.NEW<ZhanMengLianSaiMainPart>();
				this.mZhanMengLianSaiMainPart.transform.SetParent(this.PnlContent.transform, false);
				this.mZhanMengLianSaiMainPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
				{
					if (s != null && s.ID == 13)
					{
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = 13
						});
					}
				};
			}
			GameInstance.Game.SendZhanMengLianSaiCompetitionEnterState(true);
			this.mZhanMengLianSaiMainPart.gameObject.SetActive(true);
			this.mZhanMengLianSaiMainPart.RefreshInf(this.mBangHuiMatchType, this.mOldBangHuiMatchType, this.mRank, this.mBangHuiMatchGameStates, this.ConfigZhanMengLianSaiLeagueMatch, this.mNextBeginTime);
			break;
		}
	}

	public void RefreshChengzhuInfo(LuoLanChengZhuInfo chengzhuInfo)
	{
		if (this.m_luolanPart != null)
		{
			this.m_luolanPart.RefreshChengzhuInfo(chengzhuInfo);
		}
	}

	public void Reload3DModal()
	{
		if (this.m_luolanPart != null)
		{
			this.m_luolanPart.Reload3DModal();
		}
	}

	public void RefreshChengzhanShenqingInfo(List<LuoLanChengZhanRequestInfoEx> listResponse)
	{
		if (this.m_luolanPart != null)
		{
			this.m_luolanPart.RefreshChengzhanShenqingInfo(listResponse);
		}
	}

	public void RefreshRewardResult(int result)
	{
		if (this.m_luolanPart != null)
		{
			this.m_luolanPart.RefreshRewardResult(result);
		}
	}

	public void NoticeJionCallBack(string ret)
	{
		if (null != this.mZhanMengLianSaiMainPart)
		{
			this.mZhanMengLianSaiMainPart.NoticeJionCallBack(ret);
		}
	}

	public void NotifyGetLianSaiANALYSISCallBack(List<int> dataList)
	{
		if (null != this.mZhanMengLianSaiMainPart)
		{
			this.mZhanMengLianSaiMainPart.NotifyGetLianSaiANALYSISCallBack(dataList);
		}
	}

	public void NoticeGetSaiJiGetBHMatch_GetAwardCallBack()
	{
		if (null != this.mZhanMengLianSaiMainPart)
		{
			this.mZhanMengLianSaiMainPart.NoticeGetSaiJiGetBHMatch_GetAwardCallBack();
		}
	}

	public void NoticeGetShiPinDataCallBack(Dictionary<int, OrnamentData> data)
	{
		if (null != this.mZhanMengLianSaiMainPart)
		{
			this.mZhanMengLianSaiMainPart.NoticeGetShiPinDataCallBack(data);
		}
	}

	public void NoticeGetSaiJiGetRankInfMiniCallBack(List<BangHuiMatchRankInfo> Datalist)
	{
		if (null != this.mZhanMengLianSaiMainPart)
		{
			this.mZhanMengLianSaiMainPart.NoticeGetSaiJiGetRankInfMiniCallBack(Datalist);
		}
	}

	public void NotifyLianSaiRankCallBack(List<BangHuiMatchRankInfo> data)
	{
		if (null != this.mZhanMengLianSaiMainPart)
		{
			this.mZhanMengLianSaiMainPart.NotifyLianSaiRankCallBack(data);
		}
	}

	public void NoticeGetSaiJiAwardCallBack(int ret)
	{
		if (null != this.mZhanMengLianSaiMainPart)
		{
			this.mZhanMengLianSaiMainPart.NoticeGetSaiJiAwardCallBack(ret);
		}
	}

	public void NoticeGetSaiJiGetBHMatch_AwardCallBack(BangHuiMatchAwardsData data)
	{
		if (null != this.mZhanMengLianSaiMainPart)
		{
			this.mZhanMengLianSaiMainPart.NoticeGetSaiJiGetBHMatch_AwardCallBack(data);
		}
	}

	private new void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(15001, null);
		ActivityTipManager.RegActivityTipItem(15002, null);
		ActivityTipManager.RegActivityTipItem(15004, null);
	}

	private int GetZhanmengLevelLimit()
	{
		SystemOpenVO systemOpenVOByID = ConfigSystemOpen.GetSystemOpenVOByID(71);
		if (systemOpenVOByID != null)
		{
			return systemOpenVOByID.TimeParameters;
		}
		return 5;
	}

	public GameObject PnlContent;

	public GameObject BtnItem;

	public SpriteSL SprPnlContent;

	private ObservableCollection TabBtnOBC;

	public ListBox ListTabBtn;

	public GButton BtnClose;

	public DPSelectedItemEventHandler DPSelectedItem;

	public FamilyBossPart m_FamilyBossPart;

	public luolan_part m_luolanPart;

	public LuoLanRolePart m_luoLanRolePart;

	public WolfSoulField_Part m_wolfsoulfieldPart;

	public CompeteCityPart competeCityPart;

	private ConfigZhanMengLianSaiLeagueMatch mConfigZhanMengLianSaiLeagueMatch;

	private BangHuiMatchType mBangHuiMatchType = 2;

	private BangHuiMatchType mOldBangHuiMatchType = 2;

	private int mRank;

	private BangHuiMatchGameStates mBangHuiMatchGameStates;

	private ZhanMengLianSaiMainPart mZhanMengLianSaiMainPart;

	private int mNextBeginTime;

	private FamilyActivityBtnitem familyBtnItem;
}
