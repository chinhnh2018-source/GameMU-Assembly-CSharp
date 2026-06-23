using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class RebirthRankPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.InitRankTypeBtn();
		this._RankDetailsRoot.SetActive(false);
		Super.ShowNetWaiting(null);
		TCPGameServerCmds.CMD_SPR_REBORN_ADMIRE_DATA.SendDataUseRoleID();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
		}
	}

	private void RefreshModal(RoleData4Selector rd)
	{
		if (rd != null)
		{
			if (this.roleResLoader != null)
			{
				this.roleResLoader.Stop();
			}
			if (null != this._ModalRoot)
			{
				this._ModalRoot.Clear();
				int fashionGoodsID = Global.GetFashionGoodsID(rd.FashionWingsID);
				List<GoodsData> list = new List<GoodsData>();
				if (rd.GoodsDataList != null)
				{
					for (int i = 0; i < rd.GoodsDataList.Count; i++)
					{
						if (rd.GoodsDataList[i] != null && rd.GoodsDataList[i].Using == 1)
						{
							int categoriyByGoodsID = Global.GetCategoriyByGoodsID(rd.GoodsDataList[i].GoodsID);
							if (30 <= categoriyByGoodsID && 36 >= categoriyByGoodsID)
							{
								list.Add(rd.GoodsDataList[i]);
							}
							else if (categoriyByGoodsID == 37 || categoriyByGoodsID == 38)
							{
								GoodVO rebornEquipsByGoodsIDAndOccForShengWuAndShengQi = IConfigbase<ConfigReborn>.Instance.GetRebornEquipsByGoodsIDAndOccForShengWuAndShengQi(rd.GoodsDataList[i].GoodsID, Global.CheckRoleOcc(rd.Occupation, rd.SubOccupation));
								if (rebornEquipsByGoodsIDAndOccForShengWuAndShengQi != null)
								{
									GoodsData goodsData = rd.GoodsDataList[i].Clone(rebornEquipsByGoodsIDAndOccForShengWuAndShengQi.ID);
									list.Add(goodsData);
								}
							}
							else if (Global.IsFashion(rd.GoodsDataList[i].GoodsID) && rd.GoodsDataList[i].Using == 1)
							{
								list.Add(rd.GoodsDataList[i]);
							}
							else if (categoriyByGoodsID == 9 || categoriyByGoodsID == 10)
							{
								list.Add(rd.GoodsDataList[i]);
							}
						}
					}
				}
				this.roleResLoader = UIHelper.LoadRoleRes(this._ModalRoot, rd.SettingBitFlags, rd.Occupation, rd.SubOccupation, rd.RoleName, list, null, rd.MyWingData, 2f, fashionGoodsID, null, true);
			}
		}
		else if (null != this._ModalRoot)
		{
			this._ModalRoot.Clear();
		}
	}

	private void RefreshRankMoBai(int index)
	{
		if (this.mRebornRankAdmireDataDic != null)
		{
			RebornRankAdmireData rebornRankAdmireData = null;
			if (this.mRebornRankAdmireDataDic.TryGetValue(index, ref rebornRankAdmireData))
			{
				this.RefreshInF(rebornRankAdmireData, index);
				this.RefreshModal(rebornRankAdmireData.RoleData4Selector);
			}
			else
			{
				this.RefreshModal(null);
				MUDebug.Log<string>(new string[]
				{
					"<color=yellow>RebornRankAdmireData 字典不包含Key " + index + "</color>"
				});
			}
		}
	}

	private void RefreshInF(RebornRankAdmireData selectData, int index)
	{
		this._RankRoleName.pivot = 0;
		this._RankRoleName.transform.localPosition = new Vector3(-68f, -37f, 0f);
		this._RankRoleName.text = string.Empty;
		this._RankRoleValue.text = string.Empty;
		this._MoBaiNumLabel.text = string.Empty;
		string empty = string.Empty;
		string text = string.Empty;
		this._RankIcon.URL = "NetImages/GameRes/Images/RebornWin/RankTypeIcon" + index + ".png";
		if (selectData != null)
		{
			if (selectData.RoleData4Selector != null)
			{
				text = text + Global.GetLang("区：") + Global.FormatRoleNameZoneid(selectData.RoleData4Selector.ZoneId, string.Empty, 0, 1) + "\n";
				text = text + Global.GetLang("名字：") + selectData.RoleData4Selector.RoleName + "\n";
				if (index == 0)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						Global.GetLang("重生等级："),
						selectData.RebornLevel,
						"\n"
					});
				}
				else if (index == 1)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						Global.GetLang("猎杀个数："),
						selectData.RebornLevel,
						"\n"
					});
				}
				else if (index == 2)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						Global.GetLang("猎杀个数："),
						selectData.RebornLevel,
						"\n"
					});
				}
				else if (index == 3)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						Global.GetLang("最大连杀："),
						selectData.RebornLevel,
						"\n"
					});
				}
				else
				{
					text += "\n";
				}
				if (!Global.isHaiWai)
				{
					text = text + Global.GetLang("平台渠道：") + selectData.Param + "\n";
				}
			}
			else
			{
				text = text + Global.GetLang("区：") + Global.GetLang("暂无") + "\n";
				text = text + Global.GetLang("名字：") + Global.GetLang("暂无") + "\n";
				if (index == 0)
				{
					text = text + Global.GetLang("重生等级：") + Global.GetLang("暂无") + "\n";
				}
				else if (index == 1)
				{
					text = text + Global.GetLang("猎杀个数：") + Global.GetLang("暂无") + "\n";
				}
				else if (index == 2)
				{
					text = text + Global.GetLang("猎杀个数：") + Global.GetLang("暂无") + "\n";
				}
				else if (index == 3)
				{
					text = text + Global.GetLang("最大连杀：") + Global.GetLang("暂无") + "\n";
				}
				else
				{
					text += "\n";
				}
				if (!Global.isHaiWai)
				{
					text = text + Global.GetLang("平台渠道：") + Global.GetLang("暂无") + "\n";
				}
			}
		}
		this._RankRoleName.text = text;
		if (selectData != null)
		{
			this._MoBaiNumLabel.text = string.Concat(new object[]
			{
				Global.GetLang("膜拜次数："),
				selectData.AdmireCount,
				"/",
				1
			});
		}
	}

	private void RefreshTitle(int index)
	{
		if (null != this._RankTypeTitleSP)
		{
			if (index == 2)
			{
				index = 3;
			}
			else if (index == 3)
			{
				index = 2;
			}
			this._RankTypeTitleSP.spriteName = "PaiHangBang" + (index + 1);
			if (index == 0)
			{
				this._RankTypeTitleSP.transform.localScale = new Vector3(151f, 23f, 1f);
			}
			else if (index == 1)
			{
				this._RankTypeTitleSP.transform.localScale = new Vector3(160f, 26f, 1f);
			}
			else if (index == 2)
			{
				this._RankTypeTitleSP.transform.localScale = new Vector3(120f, 31f, 1f);
			}
			else if (index == 3)
			{
				this._RankTypeTitleSP.transform.localScale = new Vector3(191f, 43f, 1f);
			}
		}
	}

	private void InitRankTypeBtn()
	{
		this.mRankTypeBtns = new RebirthRankPart.BtnHanders[this._BtnItems.Length];
		byte b = 0;
		while ((int)b < this._BtnItems.Length)
		{
			this._BtnItems[(int)b].name = ((int)(b + 1)).ToString();
			RebirthRankPart.BtnHanders btnHanders = new RebirthRankPart.BtnHanders(this._BtnItems[(int)b]);
			btnHanders.BtnClick = new RebirthRankPart.BtnHanderDelegate(this.RankTypeBtnCallBack);
			this.mRankTypeBtns[(int)b] = btnHanders;
			b += 1;
		}
		this.RankTypeBtnCallBack(1);
	}

	private void RankTypeBtnCallBack(int btnID)
	{
		byte index = 0;
		byte b = 0;
		while ((int)b < this.mRankTypeBtns.Length)
		{
			if (btnID == this.mRankTypeBtns[(int)b].ID)
			{
				this.mRankTypeBtns[(int)b].Select = true;
				index = b;
			}
			else
			{
				this.mRankTypeBtns[(int)b].Select = false;
			}
			b += 1;
		}
		this.RefreshTitle((int)index);
		this.RefreshTitles((int)index);
		this.RefreshRankMoBai((int)index);
	}

	private void InitPrefabText()
	{
		try
		{
			this._MoBaiBtn.Text = Global.GetLang("膜拜");
			this.RankView_Title.text = Global.GetLang("排行榜");
			this._MoBaiNumLabel.text = string.Empty;
			this._RankRoleName.Margin = new Vector2(0f, 32f);
			this._RankRoleValue.Margin = new Vector2(0f, 32f);
			this.RefreshTitles(0);
			for (byte b = 0; b < 4; b += 1)
			{
				this._RankInfTiems[(int)b].Margin = new Vector2(0f, 26f);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogError<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void RefreshTitles(int index)
	{
		this._RankInfTitleItems[0].text = Global.GetLang("排行");
		this._RankInfTitleItems[1].text = Global.GetLang("角色");
		if (index == 0)
		{
			this._RankInfTitleItems[2].text = Global.GetLang("重生等级");
		}
		else if (index == 1)
		{
			this._RankInfTitleItems[2].text = Global.GetLang("猎杀个数");
		}
		else if (index == 2)
		{
			this._RankInfTitleItems[2].text = Global.GetLang("猎杀个数");
		}
		else if (index == 3)
		{
			this._RankInfTitleItems[2].text = Global.GetLang("最大连杀");
		}
		if (Global.isHaiWai)
		{
			this._RankInfTitleItems[3].text = Global.GetLang("区号");
		}
		else
		{
			this._RankInfTitleItems[3].text = Global.GetLang("渠道");
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
			this._BtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
			this._MoBaiBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._MoBaiBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._MoBaiBtn.GetInstanceID(), 1f);
				byte b = 0;
				byte b2 = 0;
				while ((int)b2 < this.mRankTypeBtns.Length)
				{
					if (this.mRankTypeBtns[(int)b2].Select)
					{
						b = b2;
						break;
					}
					b2 += 1;
				}
				if (this.mRebornRankAdmireDataDic != null)
				{
					RebornRankAdmireData rebornRankAdmireData = null;
					if (this.mRebornRankAdmireDataDic.TryGetValue((int)b, ref rebornRankAdmireData))
					{
						if (0 < rebornRankAdmireData.AdmireCount)
						{
							Super.HintMainText(Global.GetLang("今日膜拜次数已用完"), 10, 3);
						}
						else
						{
							GameInstance.Game.SendRoleToRebirthADMoBai((int)b);
						}
					}
					else
					{
						GameInstance.Game.SendRoleToRebirthADMoBai((int)b);
						MUDebug.Log<string>(new string[]
						{
							"<color=yellow>RebornRankAdmireData 字典不包含Key " + b + "</color>"
						});
					}
				}
			};
			this._PangHangXiangQing.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._PangHangXiangQing.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._PangHangXiangQing.GetInstanceID(), 1f);
				byte b = 0;
				byte b2 = 0;
				while ((int)b2 < this.mRankTypeBtns.Length)
				{
					if (this.mRankTypeBtns[(int)b2].Select)
					{
						b = b2;
						break;
					}
					b2 += 1;
				}
				Super.ShowNetWaiting(null);
				MUDebug.Log<string>(new string[]
				{
					"请求的排行类型：" + b + string.Empty
				});
				GameInstance.Game.SendRoleGetRebirthRankdata((int)b);
				this.ShowRankDetails(true);
			};
			this._RankDetailsInfCloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._RankDetailsInfCloseBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._RankDetailsInfCloseBtn.GetInstanceID(), 1f);
				this.ShowRankDetails(false);
			};
			UIEventListener.Get(this._RankInfBgObj).onClick = delegate(GameObject g)
			{
				if (0f < Global.GetBtnCD(this._RankInfBgObj.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._RankInfBgObj.GetInstanceID(), 1f);
				this.ShowRankDetails(false);
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

	private void ShowRankDetails(bool show)
	{
		this._RankDetailsRoot.SetActive(show);
	}

	private void RefreshRankInf(RebornRankInfoToClient data)
	{
		int num = 0;
		int num2 = 0;
		for (byte b = 0; b < 4; b += 1)
		{
			this._RankInfTiems[(int)b].text = string.Empty;
		}
		if (data != null)
		{
			MUDebug.Log<string>(new string[]
			{
				string.Concat(new object[]
				{
					"返回的排行类型：",
					data.RankType,
					Global.GetLang("：：： "),
					data.rankList
				})
			});
			if (data.rankList != null && 0 < data.rankList.Count)
			{
				num2 = data.rankList.Count;
				for (int i = 0; i < num2; i++)
				{
					KFRebornRankInfo kfrebornRankInfo = data.rankList[i];
					if (3 > i)
					{
						UILabel uilabel = this._RankInfTiems[0];
						uilabel.text += "\n";
					}
					else
					{
						UILabel uilabel2 = this._RankInfTiems[0];
						uilabel2.text = uilabel2.text + Global.GetColorStringForNGUIText(new object[]
						{
							"f5ece1",
							i + 1
						}) + "\n";
					}
					if (Global.isHaiWai)
					{
						string[] array = kfrebornRankInfo.Param1.Split(new char[]
						{
							'·'
						});
						string text = kfrebornRankInfo.Param1.Substring(array[0].Length + 1, kfrebornRankInfo.Param1.Length - array[0].Length - 1);
						UILabel uilabel3 = this._RankInfTiems[1];
						uilabel3.text = uilabel3.text + Global.GetColorStringForNGUIText(new object[]
						{
							"ebb877",
							text
						}) + "\n";
						UILabel uilabel4 = this._RankInfTiems[3];
						uilabel4.text = uilabel4.text + Global.GetColorStringForNGUIText(new object[]
						{
							"ebb877",
							array[0]
						}) + "\n";
					}
					else
					{
						UILabel uilabel5 = this._RankInfTiems[1];
						uilabel5.text = uilabel5.text + Global.GetColorStringForNGUIText(new object[]
						{
							"ebb877",
							kfrebornRankInfo.Param1
						}) + "\n";
						UILabel uilabel6 = this._RankInfTiems[3];
						uilabel6.text = uilabel6.text + Global.GetColorStringForNGUIText(new object[]
						{
							"ebb877",
							kfrebornRankInfo.Param2
						}) + "\n";
					}
					UILabel uilabel7 = this._RankInfTiems[2];
					uilabel7.text = uilabel7.text + Global.GetColorStringForNGUIText(new object[]
					{
						"f5ece1",
						kfrebornRankInfo.Value
					}) + "\n";
					if (kfrebornRankInfo.Key == Global.Data.RoleID && kfrebornRankInfo.PtID == Global.Data.roleData.PTID)
					{
						num = i + 1;
					}
				}
				if (num2 < 10)
				{
					for (int j = num2; j < 10; j++)
					{
						if (3 > j)
						{
							UILabel uilabel8 = this._RankInfTiems[0];
							uilabel8.text += "\n";
						}
						else
						{
							UILabel uilabel9 = this._RankInfTiems[0];
							uilabel9.text = uilabel9.text + Global.GetColorStringForNGUIText(new object[]
							{
								"f5ece1",
								j + 1
							}) + "\n";
						}
						UILabel uilabel10 = this._RankInfTiems[1];
						uilabel10.text = uilabel10.text + Global.GetColorStringForNGUIText(new object[]
						{
							"ebb877",
							Global.GetLang("暂无")
						}) + "\n";
						UILabel uilabel11 = this._RankInfTiems[2];
						uilabel11.text = uilabel11.text + Global.GetColorStringForNGUIText(new object[]
						{
							"f5ece1",
							Global.GetLang("暂无")
						}) + "\n";
						UILabel uilabel12 = this._RankInfTiems[3];
						uilabel12.text = uilabel12.text + Global.GetColorStringForNGUIText(new object[]
						{
							"ebb877",
							Global.GetLang("暂无")
						}) + "\n";
					}
					num2 = 10;
				}
			}
			if (num2 == 0)
			{
				num2 = 10;
				for (int k = 0; k < num2; k++)
				{
					if (3 > k)
					{
						UILabel uilabel13 = this._RankInfTiems[0];
						uilabel13.text += "\n";
					}
					else
					{
						UILabel uilabel14 = this._RankInfTiems[0];
						uilabel14.text = uilabel14.text + Global.GetColorStringForNGUIText(new object[]
						{
							"f5ece1",
							k + 1
						}) + "\n";
					}
					UILabel uilabel15 = this._RankInfTiems[1];
					uilabel15.text = uilabel15.text + Global.GetColorStringForNGUIText(new object[]
					{
						"ebb877",
						Global.GetLang("暂无")
					}) + "\n";
					UILabel uilabel16 = this._RankInfTiems[2];
					uilabel16.text = uilabel16.text + Global.GetColorStringForNGUIText(new object[]
					{
						"f5ece1",
						Global.GetLang("暂无")
					}) + "\n";
					UILabel uilabel17 = this._RankInfTiems[3];
					uilabel17.text = uilabel17.text + Global.GetColorStringForNGUIText(new object[]
					{
						"ebb877",
						Global.GetLang("暂无")
					}) + "\n";
				}
			}
			this._UICollider.updataNow = true;
			this._BgSP.transform.localScale = new Vector3(this._BgSP.transform.localScale.x, (float)(29 * num2), this._BgSP.transform.localScale.z);
		}
		else
		{
			if (num2 == 0)
			{
				num2 = 10;
				for (int l = 0; l < num2; l++)
				{
					if (3 > l)
					{
						UILabel uilabel18 = this._RankInfTiems[0];
						uilabel18.text += "\n";
					}
					else
					{
						UILabel uilabel19 = this._RankInfTiems[0];
						uilabel19.text = uilabel19.text + Global.GetColorStringForNGUIText(new object[]
						{
							"f5ece1",
							l + 1
						}) + "\n";
					}
					UILabel uilabel20 = this._RankInfTiems[1];
					uilabel20.text = uilabel20.text + Global.GetColorStringForNGUIText(new object[]
					{
						"ebb877",
						Global.GetLang("暂无")
					}) + "\n";
					UILabel uilabel21 = this._RankInfTiems[2];
					uilabel21.text = uilabel21.text + Global.GetColorStringForNGUIText(new object[]
					{
						"f5ece1",
						Global.GetLang("暂无")
					}) + "\n";
					UILabel uilabel22 = this._RankInfTiems[3];
					uilabel22.text = uilabel22.text + Global.GetColorStringForNGUIText(new object[]
					{
						"ebb877",
						Global.GetLang("暂无")
					}) + "\n";
				}
			}
			this._UICollider.updataNow = true;
			this._BgSP.transform.localScale = new Vector3(this._BgSP.transform.localScale.x, (float)(29 * num2), this._BgSP.transform.localScale.z);
		}
		if (0 >= num)
		{
			this._MyRankInf.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ebb877",
				Global.GetLang("我的排名："),
				"ffffff",
				Global.GetLang("未上榜")
			});
		}
		else
		{
			this._MyRankInf.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ebb877",
				Global.GetLang("我的排名："),
				"ffffff",
				num
			});
		}
		SpringPanel.Begin(this._RankViewPanel.gameObject, Vector3.zero, 10f);
	}

	internal void NoticeRebirthRankDetailsdataCallBack(RebornRankInfoToClient data)
	{
		this.RefreshRankInf(data);
	}

	internal void NoticeMoBaiCallBack(string[] p)
	{
		if (0 <= p[0].SafeToInt32(0))
		{
			int num = p[1].SafeToInt32(0);
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("EveryDayMaxRebornExp", ',');
			Super.HintMainText(Global.GetLang("重生经验 +") + systemParamIntArrayByName[num], 10, 3);
			if (this.mRebornRankAdmireDataDic.ContainsKey(num))
			{
				this.mRebornRankAdmireDataDic[num].AdmireCount = 1;
				byte b = 0;
				byte b2 = 0;
				while ((int)b2 < this.mRankTypeBtns.Length)
				{
					if (this.mRankTypeBtns[(int)b2].Select)
					{
						b = b2;
						break;
					}
					b2 += 1;
				}
				this.RankTypeBtnCallBack((int)(b + 1));
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(p[0].SafeToInt32(0), false, false)), 10, 3);
		}
	}

	internal void NoticeRebirthRankdataCallBack(Dictionary<int, RebornRankAdmireData> data)
	{
		this.mRebornRankAdmireDataDic = data;
		this.RankTypeBtnCallBack(1);
	}

	private const int MoBaiMaxNumber = 1;

	private const int TempTestRankCount = 10;

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private GButton _BtnClose;

	[SerializeField]
	private ShowNetImage _RankIcon;

	[SerializeField]
	private UISprite _RankTypeTitleSP;

	[SerializeField]
	private UILabel _RankRoleName;

	[SerializeField]
	private UILabel _RankRoleValue;

	[SerializeField]
	private GameObject[] _BtnItems;

	[SerializeField]
	private Modal3DShow _ModalRoot;

	[SerializeField]
	private UILabel _MoBaiNumLabel;

	[SerializeField]
	private GButton _MoBaiBtn;

	[SerializeField]
	private GButton _PangHangXiangQing;

	[SerializeField]
	private UILabel[] _RankInfTitleItems;

	[SerializeField]
	private UILabel[] _RankInfTiems;

	[SerializeField]
	private GameObject _RankDetailsRoot;

	[SerializeField]
	private UISprite _BgSP;

	[SerializeField]
	private GButton _RankDetailsInfCloseBtn;

	[SerializeField]
	private UIPanel _RankViewPanel;

	[SerializeField]
	private UICollider _UICollider;

	[SerializeField]
	private UILabel _MyRankInf;

	[SerializeField]
	private GameObject _RankInfBgObj;

	[SerializeField]
	public UILabel RankView_Title;

	private RoleResLoader roleResLoader;

	private RebirthRankPart.BtnHanders[] mRankTypeBtns;

	private Dictionary<int, RebornRankAdmireData> mRebornRankAdmireDataDic;

	public enum RebornRankType
	{
		Level,
		Rarity,
		Boss,
		LianSha
	}

	private class BtnHanders
	{
		public BtnHanders(GameObject obj)
		{
			RebirthRankPart.BtnHanders <>f__this = this;
			if (!NGUITools.GetActive(obj))
			{
				obj.SetActive(true);
			}
			this.mID = obj.name.SafeToInt32(0);
			UIEventListener.Get(obj).onClick = delegate(GameObject g)
			{
				if (0f < Global.GetBtnCD(obj.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(obj.GetInstanceID(), 0.1f);
				if (<>f__this.BtnClick != null)
				{
					<>f__this.BtnClick(<>f__this.mID);
				}
			};
			this.ThisObj = obj;
			this._label = obj.transform.FindChild("UILabel").GetComponent<UILabel>();
			this._BakSp = obj.transform.FindChild("Background").GetComponent<UISprite>();
			this._label.text = this.GetItemsName(this.mID - 1);
		}

		private string GetItemsName(int type)
		{
			string[] array = new string[]
			{
				"重生等级",
				Global.GetLang("精英猎杀"),
				Global.GetLang("BOSS猎杀"),
				Global.GetLang("连杀"),
				string.Empty
			};
			return Global.GetLang(array[type]);
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
				if (this.mSelect)
				{
					this._BakSp.spriteName = "button_2";
					this._label.color = NGUIMath.HexToColorEx(16777215U);
				}
				else
				{
					this._BakSp.spriteName = "button_1";
					this._label.color = NGUIMath.HexToColorEx(16768943U);
				}
			}
		}

		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		public RebirthRankPart.BtnHanderDelegate BtnClick;

		private UISprite _BakSp;

		private UILabel _label;

		private int mID = -1;

		private GameObject ThisObj;

		private bool mSelect;
	}

	private delegate void BtnHanderDelegate(int btnID);
}
