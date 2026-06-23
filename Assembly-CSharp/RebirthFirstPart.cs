using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class RebirthFirstPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		byte b = 0;
		if (Global.Data != null && Global.Data.roleData != null && 0 < Global.Data.roleData.RebornCount)
		{
			b = 1;
		}
		this._SuccessRoot.SetActive(false);
		if (b == 0)
		{
			this._OneRoot.SetActive(true);
			this._TwoRoot.SetActive(false);
		}
		else
		{
			this._OneRoot.SetActive(false);
			this._TwoRoot.SetActive(true);
		}
		TCPGameServerCmds.CMD_SPR_MERLIN_QUERY.SendDataUseRoleID();
		TCPGameServerCmds.CMD_SPR_GET_LINGYU_LIST.SendDataUseRoleID();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.mRoleResLoader != null)
		{
			this.mRoleResLoader.Stop();
		}
	}

	private void RefreshRebirthInf()
	{
		if (Global.Data != null && Global.Data.roleData != null)
		{
			byte[] array = new byte[3];
			byte[] array2 = new byte[3];
			int id = Global.Data.roleData.RebornCount + 1;
			RebornStageVO rebornStageVOByID = IConfigbase<ConfigRebirth>.Instance.GetRebornStageVOByID(id);
			if (rebornStageVOByID != null)
			{
				string[] array3 = rebornStageVOByID.ExtProp.Split(new char[]
				{
					'|'
				});
				string[] array4 = array3[Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation)].Split(new char[]
				{
					','
				});
				string text = string.Empty;
				if (ConfigExtPropIndexes.GetPercentByWord(array4[0]))
				{
					text = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array4[0], false) + Global.GetLang("：") + (double.Parse(array4[1]) * 100.0).ToString("f1") + "%";
				}
				else
				{
					text = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array4[0], false) + array4[1];
				}
				int num = rebornStageVOByID.GetIntro.LastIndexOf('@');
				this._Content1.text = rebornStageVOByID.GetIntro.Remove(num, rebornStageVOByID.GetIntro.Length - num) + text;
				this._ColliderC.updataNow = true;
				if (rebornStageVOByID.NeedZhuanShengInf[0] != -1 && rebornStageVOByID.NeedZhuanShengInf[1] != -1)
				{
					bool flag = false;
					if (rebornStageVOByID.NeedZhuanShengInf[0] < Global.Data.roleData.ChangeLifeCount)
					{
						flag = true;
					}
					else if (rebornStageVOByID.NeedZhuanShengInf[0] == Global.Data.roleData.ChangeLifeCount && rebornStageVOByID.NeedZhuanShengInf[1] <= Global.Data.roleData.Level)
					{
						flag = true;
					}
					if (flag)
					{
						array2[this.GetZoneIndex(array)] = 1;
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedRebornLevel != -1)
				{
					if (rebornStageVOByID.NeedRebornLevel <= Global.Data.roleData.RebornLevel)
					{
						array2[this.GetZoneIndex(array)] = 1;
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedZhanLi != -1)
				{
					if (rebornStageVOByID.NeedZhanLi <= Global.Data.roleData.CombatForce)
					{
						array2[this.GetZoneIndex(array)] = 1;
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedMaxWingInf[0] != -1 && rebornStageVOByID.NeedMaxWingInf[1] != -1 && rebornStageVOByID.NeedMaxWingInf[2] != -1 && rebornStageVOByID.NeedMaxWingInf[3] != -1 && rebornStageVOByID.NeedMaxWingInf[4] != -1)
				{
					if (Global.Data.roleData.MyWingData != null)
					{
						int num2 = 0;
						if (this.mLingyuList != null && 0 < this.mLingyuList.Count)
						{
							for (int i = 0; i < this.mLingyuList.Count; i++)
							{
								if (this.mLingyuList[i].Level > 0)
								{
									num2 += this.mLingyuList[i].Level;
								}
							}
						}
						bool flag2 = false;
						bool flag3 = false;
						if (rebornStageVOByID.NeedMaxWingInf[0] < Global.Data.roleData.MyWingData.WingID)
						{
							flag3 = true;
						}
						else if (rebornStageVOByID.NeedMaxWingInf[0] == Global.Data.roleData.MyWingData.WingID && rebornStageVOByID.NeedMaxWingInf[1] <= Global.Data.roleData.MyWingData.ForgeLevel)
						{
							flag3 = true;
						}
						if (flag3)
						{
							XElement gameResXml = Global.GetGameResXml("Config/MaxWinZhuLing.xml");
							List<XElement> xelementList = Global.GetXElementList(gameResXml, "ZhuLing");
							int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[xelementList.Count - 1], "PlainZhuLing");
							int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[xelementList.Count - 1], "SeniorZhuLing");
							if (rebornStageVOByID.NeedMaxWingInf[2] <= num2 && (double)rebornStageVOByID.NeedMaxWingInf[3] <= (double)Global.Data.roleData.MyWingData.ZhuLingNum / (double)xelementAttributeInt && (double)rebornStageVOByID.NeedMaxWingInf[3] <= (double)Global.Data.roleData.MyWingData.ZhuHunNum / (double)xelementAttributeInt2)
							{
								flag2 = true;
							}
						}
						if (flag2)
						{
							array2[this.GetZoneIndex(array)] = 1;
						}
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedChengJie != -1)
				{
					if (rebornStageVOByID.NeedChengJie <= Global.GetChengJiuLevel(0))
					{
						array2[this.GetZoneIndex(array)] = 1;
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedShengWang != -1)
				{
					if (rebornStageVOByID.NeedShengWang <= Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel))
					{
						array2[this.GetZoneIndex(array)] = 1;
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (rebornStageVOByID.NeedMagicBookInf[0] != -1 && rebornStageVOByID.NeedMagicBookInf[1] != -1)
				{
					if (this.mMeiLanDataBag != null)
					{
						if (rebornStageVOByID.NeedMagicBookInf[0] < this.mMeiLanDataBag._Level)
						{
							array2[this.GetZoneIndex(array)] = 1;
						}
						else if (rebornStageVOByID.NeedMagicBookInf[0] == this.mMeiLanDataBag._Level && rebornStageVOByID.NeedMagicBookInf[1] <= this.mMeiLanDataBag._StarNum)
						{
							array2[this.GetZoneIndex(array)] = 1;
						}
					}
					array[this.GetZoneIndex(array)] = 1;
				}
				if (!string.IsNullOrEmpty(rebornStageVOByID.NeedIntro))
				{
					this._Content2.text = string.Empty;
					string[] array5 = rebornStageVOByID.NeedIntro.Split(new char[]
					{
						'|'
					});
					byte b = 0;
					while ((int)b < array5.Length)
					{
						if (array2[(int)b] == 1)
						{
							array5[(int)b] = Global.GetColorStringForNGUIText(new object[]
							{
								"7c6753",
								array5[(int)b]
							});
							this._DuiHao[(int)b].SetActive(true);
						}
						else
						{
							array5[(int)b] = Global.GetColorStringForNGUIText(new object[]
							{
								"ff0000",
								array5[(int)b]
							});
							this._DuiHao[(int)b].SetActive(false);
						}
						UILabel content = this._Content2;
						content.text = content.text + array5[(int)b] + "\n";
						b += 1;
					}
				}
			}
			else if (0 < Global.Data.roleData.RebornCount)
			{
				this._Content1.text = string.Empty;
				RebornStageVO rebornStageVOByID2 = IConfigbase<ConfigRebirth>.Instance.GetRebornStageVOByID(Global.Data.roleData.RebornCount);
				if (rebornStageVOByID2 != null)
				{
					string[] array6 = rebornStageVOByID2.ExtProp.Split(new char[]
					{
						'|'
					});
					string[] array7 = array6[Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation)].Split(new char[]
					{
						','
					});
					string text2 = string.Empty;
					if (ConfigExtPropIndexes.GetPercentByWord(array7[0]))
					{
						text2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array7[0], false) + Global.GetLang("：") + (double.Parse(array7[1]) * 100.0).ToString("f1") + "%";
					}
					else
					{
						text2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array7[0], false) + array7[1];
					}
					this._Content1.text = string.Format(rebornStageVOByID2.GetIntro, text2);
					this._ColliderC.updataNow = true;
				}
				this._Content2.text = Global.GetLang("已达到大等级");
			}
			this._RoleModal.Clear();
			if (this.mRoleResLoader != null)
			{
				this.mRoleResLoader.Stop();
			}
			List<GoodsData> list = new List<GoodsData>();
			List<GoodsData> showModalGoodsDataList = rebornStageVOByID.ShowModalGoodsDataList;
			if (showModalGoodsDataList != null)
			{
				for (int j = 0; j < showModalGoodsDataList.Count; j++)
				{
					if (showModalGoodsDataList[j] != null && showModalGoodsDataList[j].Using == 1)
					{
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(showModalGoodsDataList[j].GoodsID);
						if (30 <= categoriyByGoodsID && 36 >= categoriyByGoodsID)
						{
							list.Add(showModalGoodsDataList[j]);
						}
						else if (categoriyByGoodsID == 37 || categoriyByGoodsID == 38)
						{
							GoodVO rebornEquipsByGoodsIDAndOccForShengWuAndShengQi = IConfigbase<ConfigReborn>.Instance.GetRebornEquipsByGoodsIDAndOccForShengWuAndShengQi(showModalGoodsDataList[j].GoodsID, Global.CheckRoleOcc(Global.Data.roleData.Occupation, Global.Data.roleData.SubOccupation));
							if (rebornEquipsByGoodsIDAndOccForShengWuAndShengQi != null)
							{
								GoodsData goodsData = showModalGoodsDataList[j].Clone(rebornEquipsByGoodsIDAndOccForShengWuAndShengQi.ID);
								list.Add(goodsData);
							}
						}
						else if (Global.IsFashion(showModalGoodsDataList[j].GoodsID) && showModalGoodsDataList[j].Using == 1)
						{
							list.Add(showModalGoodsDataList[j]);
						}
						else if (categoriyByGoodsID == 9 || categoriyByGoodsID == 10)
						{
							list.Add(showModalGoodsDataList[j]);
						}
					}
				}
			}
			WingData wingData = new WingData
			{
				ForgeLevel = Global.Data.roleData.MyWingData.WingID,
				Using = 1,
				AddDateTime = Global.Data.roleData.MyWingData.AddDateTime,
				DbID = Global.Data.roleData.MyWingData.DbID,
				JinJieFailedNum = Global.Data.roleData.MyWingData.JinJieFailedNum,
				WingID = rebornStageVOByID.Wing
			};
			this.mRoleResLoader = UIHelper.LoadRoleRes(this._RoleModal, 0L, Global.Data.roleData.Occupation, Global.Data.roleData.SubOccupation, "1", list, null, wingData, 2f, 0, delegate(object e, DPSelectedItemEventArgs s)
			{
			}, true);
		}
	}

	private int GetZoneIndex(byte[] index)
	{
		int result = 0;
		if (index != null)
		{
			for (byte b = 0; b < 3; b += 1)
			{
				if (index[(int)b] == 0)
				{
					result = (int)b;
					break;
				}
			}
		}
		return result;
	}

	private void InitPrefabText()
	{
		try
		{
			this._Content1.Margin = new Vector2(0f, 16f);
			this._Content2.Margin = new Vector2(0f, 16f);
			this._RebirthBtnOne.Text = Global.GetLang("我要重生");
			this._RebirthBtnTwo.Text = Global.GetLang("我要重生");
			this._SuccessBtn.Text = Global.GetLang("确定");
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
			if (Global.Data == null || Global.Data.roleData != null)
			{
			}
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
			this._ColseBtnOne.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0,
						ID = 0
					});
				}
			};
			this._RebirthBtnOne.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this._TwoRoot.SetActive(true);
				this._OneRoot.SetActive(false);
			};
			this._RebirthBtnTwo.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (Global.Data != null && Global.Data.roleData != null)
				{
					byte[] array = new byte[3];
					byte[] array2 = new byte[3];
					int id = Global.Data.roleData.RebornCount + 1;
					RebornStageVO rebornStageVOByID = IConfigbase<ConfigRebirth>.Instance.GetRebornStageVOByID(id);
					if (rebornStageVOByID != null)
					{
						if (rebornStageVOByID.NeedZhuanShengInf[0] != -1 && rebornStageVOByID.NeedZhuanShengInf[1] != -1)
						{
							bool flag = false;
							if (rebornStageVOByID.NeedZhuanShengInf[0] < Global.Data.roleData.ChangeLifeCount)
							{
								flag = true;
							}
							else if (rebornStageVOByID.NeedZhuanShengInf[0] == Global.Data.roleData.ChangeLifeCount && rebornStageVOByID.NeedZhuanShengInf[1] <= Global.Data.roleData.Level)
							{
								flag = true;
							}
							if (!flag)
							{
								MUDebug.Log<string>(new string[]
								{
									string.Concat(new object[]
									{
										"转生条件",
										rebornStageVOByID.NeedZhuanShengInf[0],
										"::",
										rebornStageVOByID.NeedZhuanShengInf[1]
									})
								});
								MUDebug.Log<string>(new string[]
								{
									"转生条件不足"
								});
								Super.HintMainText(Global.GetLang("不符合重生条件"), 10, 3);
								return;
							}
							array2[this.GetZoneIndex(array)] = 1;
							array[this.GetZoneIndex(array)] = 1;
						}
						if (rebornStageVOByID.NeedRebornLevel != -1)
						{
							if (rebornStageVOByID.NeedRebornLevel > Global.Data.roleData.RebornLevel)
							{
								MUDebug.Log<string>(new string[]
								{
									"重生条件" + rebornStageVOByID.NeedRebornLevel
								});
								MUDebug.Log<string>(new string[]
								{
									"重生等级条件不足"
								});
								Super.HintMainText(Global.GetLang("不符合重生条件"), 10, 3);
								return;
							}
							array2[this.GetZoneIndex(array)] = 1;
							array[this.GetZoneIndex(array)] = 1;
						}
						if (rebornStageVOByID.NeedZhanLi != -1)
						{
							if (rebornStageVOByID.NeedZhanLi > Global.Data.roleData.CombatForce)
							{
								MUDebug.Log<string>(new string[]
								{
									"战力条件" + rebornStageVOByID.NeedZhanLi
								});
								MUDebug.Log<string>(new string[]
								{
									"战力条件不足"
								});
								Super.HintMainText(Global.GetLang("不符合重生条件"), 10, 3);
								return;
							}
							array2[this.GetZoneIndex(array)] = 1;
							array[this.GetZoneIndex(array)] = 1;
						}
						if (rebornStageVOByID.NeedMaxWingInf[0] != -1 && rebornStageVOByID.NeedMaxWingInf[1] != -1 && rebornStageVOByID.NeedMaxWingInf[2] != -1 && rebornStageVOByID.NeedMaxWingInf[3] != -1 && rebornStageVOByID.NeedMaxWingInf[4] != -1)
						{
							if (Global.Data.roleData.MyWingData == null)
							{
								MUDebug.Log<string>(new string[]
								{
									"翅膀条件不足"
								});
								Super.HintMainText(Global.GetLang("不符合重生条件"), 10, 3);
								return;
							}
							int num = 0;
							if (this.mLingyuList != null && 0 < this.mLingyuList.Count)
							{
								for (int i = 0; i < this.mLingyuList.Count; i++)
								{
									if (this.mLingyuList[i].Level > 0)
									{
										num += this.mLingyuList[i].Level;
									}
								}
							}
							bool flag2 = false;
							bool flag3 = false;
							if (rebornStageVOByID.NeedMaxWingInf[0] < Global.Data.roleData.MyWingData.WingID)
							{
								flag3 = true;
							}
							else if (rebornStageVOByID.NeedMaxWingInf[0] == Global.Data.roleData.MyWingData.WingID && rebornStageVOByID.NeedMaxWingInf[1] <= Global.Data.roleData.MyWingData.ForgeLevel)
							{
								flag3 = true;
							}
							if (flag3 && rebornStageVOByID.NeedMaxWingInf[2] <= num && rebornStageVOByID.NeedMaxWingInf[3] <= Global.Data.roleData.MyWingData.ZhuLingNum && rebornStageVOByID.NeedMaxWingInf[3] <= Global.Data.roleData.MyWingData.ZhuHunNum)
							{
								flag2 = true;
							}
							if (!flag2)
							{
								MUDebug.Log<string>(new string[]
								{
									"翅膀条件不足"
								});
								Super.HintMainText(Global.GetLang("不符合重生条件"), 10, 3);
								return;
							}
							array2[this.GetZoneIndex(array)] = 1;
							array[this.GetZoneIndex(array)] = 1;
						}
						if (rebornStageVOByID.NeedChengJie != -1)
						{
							if (rebornStageVOByID.NeedChengJie > Global.GetChengJiuLevel(0))
							{
								MUDebug.Log<string>(new string[]
								{
									"成就条件不足"
								});
								Super.HintMainText(Global.GetLang("不符合重生条件"), 10, 3);
								return;
							}
							array2[this.GetZoneIndex(array)] = 1;
							array[this.GetZoneIndex(array)] = 1;
						}
						if (rebornStageVOByID.NeedShengWang != -1)
						{
							if (rebornStageVOByID.NeedShengWang > Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel))
							{
								MUDebug.Log<string>(new string[]
								{
									"声望条件不足"
								});
								Super.HintMainText(Global.GetLang("不符合重生条件"), 10, 3);
								return;
							}
							array2[this.GetZoneIndex(array)] = 1;
							array[this.GetZoneIndex(array)] = 1;
						}
						if (rebornStageVOByID.NeedMagicBookInf[0] != -1 && rebornStageVOByID.NeedMagicBookInf[1] != -1)
						{
							if (this.mMeiLanDataBag == null)
							{
								MUDebug.Log<string>(new string[]
								{
									"梅林之书条件不足"
								});
								Super.HintMainText(Global.GetLang("不符合重生条件"), 10, 3);
								return;
							}
							bool flag4 = false;
							if (rebornStageVOByID.NeedMagicBookInf[0] < this.mMeiLanDataBag._Level)
							{
								flag4 = true;
							}
							else if (rebornStageVOByID.NeedMagicBookInf[0] == this.mMeiLanDataBag._Level && rebornStageVOByID.NeedMagicBookInf[1] <= this.mMeiLanDataBag._StarNum)
							{
								flag4 = true;
							}
							if (!flag4)
							{
								MUDebug.Log<string>(new string[]
								{
									"梅林之书条件不足"
								});
								Super.HintMainText(Global.GetLang("不符合重生条件"), 10, 3);
								return;
							}
							array2[this.GetZoneIndex(array)] = 1;
							array[this.GetZoneIndex(array)] = 1;
						}
					}
					TCPGameServerCmds.CMD_SPR_REBORN_UPGRADE.SendDataUseRoleID();
				}
			};
			this._SuccessBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 4,
						ID = 0
					});
				}
			};
			this._ColseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0,
						ID = 0
					});
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

	public void NoticeGetLingYuDataCakkBack(List<LingYuData> lingyuList)
	{
		this.mLingyuList = lingyuList;
		this.RefreshRebirthInf();
	}

	internal void NOticeGetMeiLanDataCallBack(MerlinGrowthSaveDBData DataBag)
	{
		this.mMeiLanDataBag = DataBag;
		this.RefreshRebirthInf();
	}

	public void NoticeRoleRebirthCallBack(string[] msg)
	{
		if ("1".Equals(msg[0]))
		{
			this._SuccessRoot.SetActive(true);
			this._OneRoot.SetActive(false);
			this._TwoRoot.SetActive(false);
			if (Global.Data != null && Global.Data.roleData != null && Global.Data.RoleID == msg[1].SafeToInt32(0))
			{
				Global.Data.roleData.RebornCount = msg[2].SafeToInt32(0);
			}
		}
		else
		{
			Super.HintMainText("不符合重生条件", 10, 3);
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private GButton _ColseBtn;

	[SerializeField]
	private GButton _ColseBtnOne;

	[SerializeField]
	private GameObject _OneRoot;

	[SerializeField]
	private GameObject _TwoRoot;

	[SerializeField]
	private GameObject _SuccessRoot;

	[SerializeField]
	private GButton _SuccessBtn;

	[SerializeField]
	private GButton _RebirthBtnOne;

	[SerializeField]
	private GButton _RebirthBtnTwo;

	[SerializeField]
	private UILabel _Content1;

	[SerializeField]
	private UILabel _Content2;

	[SerializeField]
	private Modal3DShow _RoleModal;

	[SerializeField]
	private UICollider _ColliderC;

	[SerializeField]
	private GameObject[] _DuiHao;

	private RoleResLoader mRoleResLoader;

	private MerlinGrowthSaveDBData mMeiLanDataBag;

	private List<LingYuData> mLingyuList;
}
