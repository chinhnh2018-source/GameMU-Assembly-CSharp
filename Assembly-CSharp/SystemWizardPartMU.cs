using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class SystemWizardPartMU : UserControl
{
	public List<GameObject> children
	{
		get
		{
			if (this.mChildren.Count == 0)
			{
				Transform transform = base.transform;
				this.mChildren.Clear();
				for (int i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					if (child && child.gameObject && NGUITools.GetActive(child.gameObject))
					{
						this.mChildren.Add(child.gameObject);
					}
				}
			}
			return this.mChildren;
		}
	}

	private SpriteSL thisCtrl
	{
		get
		{
			return this;
		}
	}

	protected override void InitializeComponent()
	{
		this._ListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ListBox_MouseLeftButtonUp);
		double[] systemParamDoubleArrayByName = ConfigSystemParam.GetSystemParamDoubleArrayByName("XianShiTime");
		if (systemParamDoubleArrayByName != null && systemParamDoubleArrayByName.Length >= 2)
		{
			this.EquipTipsTime = (float)systemParamDoubleArrayByName[0];
			this.OtherTipsTime = (float)systemParamDoubleArrayByName[1];
		}
		this.MyUIAnchorPosY = (int)this._MyUIAnchor.offset.y;
	}

	public double BodyBackOpacity
	{
		set
		{
		}
	}

	public int TaskID
	{
		get
		{
			return this._TaskID;
		}
		set
		{
			this._TaskID = value;
		}
	}

	public int GoodsDbID
	{
		get
		{
			return this._GoodsDbID;
		}
		set
		{
			this._GoodsDbID = value;
		}
	}

	public int GoodsCount { get; set; }

	public int ToByGoodsID
	{
		get
		{
			return this._ToByGoodsID;
		}
		set
		{
			this._ToByGoodsID = value;
		}
	}

	public int ToByGoodsFromType
	{
		set
		{
			this._ToByGoodsFromType = value;
		}
	}

	public List<int> NewEquipGoodsDbIDList
	{
		get
		{
			return this._NewEquipGoodsDbIDList;
		}
		set
		{
			this._NewEquipGoodsDbIDList = value;
		}
	}

	public int SkillID
	{
		get
		{
			return this._SkillID;
		}
		set
		{
			this._SkillID = value;
		}
	}

	public int YaBiaoID
	{
		get
		{
			return this._YaBiaoID;
		}
		set
		{
			this._YaBiaoID = value;
		}
	}

	public int YaBiaoLineID
	{
		get
		{
			return this._YaBiaoLineID;
		}
		set
		{
			this._YaBiaoLineID = value;
		}
	}

	public int YaBiaoTouBao
	{
		get
		{
			return this._YaBiaoTouBao;
		}
		set
		{
			this._YaBiaoTouBao = value;
		}
	}

	public int YaBiaoFailType
	{
		get
		{
			return this._YaBiaoFailType;
		}
		set
		{
			this._YaBiaoFailType = value;
		}
	}

	public int WizardType
	{
		get
		{
			return this._WizardType;
		}
		set
		{
			this._WizardType = value;
		}
	}

	public string ReloadXmlMsg
	{
		get
		{
			return this._ReloadXmlMsg;
		}
		set
		{
			this._ReloadXmlMsg = value;
		}
	}

	public int SuiTangBattleResultExpAward
	{
		get
		{
			return this._SuiTangBattleResultExpAward;
		}
		set
		{
			this._SuiTangBattleResultExpAward = value;
		}
	}

	public int SuiTangBattleResultBindYuanBaoAward
	{
		get
		{
			return this._SuiTangBattleResultBindYuanBaoAward;
		}
		set
		{
			this._SuiTangBattleResultBindYuanBaoAward = value;
		}
	}

	public int SuiTangBattleResultChengJiuAward
	{
		get
		{
			return this._SuiTangBattleResultChengJiuAward;
		}
		set
		{
			this._SuiTangBattleResultChengJiuAward = value;
		}
	}

	public bool AllInstantAutoFindRoad
	{
		get
		{
			return this._AllInstantAutoFindRoad;
		}
		set
		{
			this._AllInstantAutoFindRoad = value;
		}
	}

	private void InitControls()
	{
	}

	private void ResetListBoxPosition(int count)
	{
		Vector3 localPosition = this._ListBox.transform.localPosition;
		localPosition.y = (float)(count * 78);
		this._ListBox.transform.localPosition = localPosition;
	}

	public void InitPartSize(int width, int height)
	{
	}

	private TextBlock GetTextBlock2(string text, int fontSize, uint color, double maxWidth)
	{
		return null;
	}

	private void AddGoodsImageNames(List<int> goodsDbIDs)
	{
		Canvas canvas = new Canvas();
		canvas.HorizontalAlignment = global::Layout.Center;
		for (int i = 0; i < goodsDbIDs.Count; i++)
		{
			int goodsDbID = goodsDbIDs[i];
			Canvas canvas2 = null;
			string text;
			this.AddGoodsImageNameX(goodsDbID, out canvas2, out text);
			Canvas.SetLeft(canvas2, i % 4 * 41);
			Canvas.SetTop(canvas2, i / 4 * 41);
			canvas.Children.Add(canvas2);
		}
		this.ItemPanel.Children.Add(canvas);
	}

	private string AddGoodsImageName(int goodsDbID)
	{
		Canvas component;
		string result;
		this.AddGoodsImageNameX(goodsDbID, out component, out result);
		this.ItemPanel.Children.Add(component);
		return result;
	}

	private void AddGoodsImageNameX(int goodsDbID, out Canvas canvas, out string goodsName)
	{
		canvas = new Canvas();
		canvas.Width = 41.0;
		canvas.Height = 41.0;
		canvas.HorizontalAlignment = global::Layout.Center;
		goodsName = string.Empty;
		Image image = new Image();
		image.IsHitTestVisible = false;
		image.Width = 41.0;
		image.Height = 42.0;
		image.Source = new ImageBrush(Global.GetGameResImage("Images/Plate/rm_listItem.png"));
		Canvas.SetLeft(image, 2);
		Canvas.SetTop(image, 2);
		canvas.Children.Add(image);
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(goodsDbID, null);
		if (goodsDataByDbID != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataByDbID.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				goodsName = goodsXmlNodeByID.Title;
				GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 32.0;
				ggoodIcon.Height = 32.0;
				ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty), false, 0);
				ggoodIcon.TipType = 1;
				ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsXmlNodeByID.ID,
					1,
					goodsDataByDbID.Id,
					0
				});
				ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
				ggoodIcon.ItemCode = goodsDataByDbID.GoodsID;
				ggoodIcon.ItemObject = goodsDataByDbID;
				ggoodIcon.BoxTypes = -1;
				ggoodIcon.Text = ((goodsDataByDbID.GCount <= 1) ? string.Empty : goodsDataByDbID.GCount.ToString());
				ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
				ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
				ggoodIcon.TextShadowColor = 4278190080U;
				ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
				bool canUse = Global.CanUseGoods(goodsDataByDbID.GoodsID, false, true);
				Super.InitGoodsGIcon(ggoodIcon, goodsDataByDbID, canUse, IconTextTypes.Qianghua);
				Canvas.SetLeft(ggoodIcon, 4);
				Canvas.SetTop(ggoodIcon, 4);
				canvas.Children.Add(ggoodIcon);
			}
		}
	}

	private void AddCheckBox()
	{
		GCheckBox gcheckBox = new GCheckBox();
		gcheckBox.HorizontalAlignment = global::Layout.Center;
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("本次登陆不再提示");
		gcheckBox.TextHorizontalAlignment = global::Layout.Left;
		gcheckBox.TextColor = new SolidColorBrush(4294944000U);
		gcheckBox.DisableTextCheck = true;
		gcheckBox.CheckChanged = new BaseEventHandler2(this.CheckBoxChanged);
		this.ItemPanel.Children.Add(gcheckBox);
	}

	private void CheckBoxChanged(object sender, object e)
	{
		Super.SetShowSystemWizard(this.WizardType, !(sender as GCheckBox).Check);
	}

	private List<GIcon> AddButtons(string[] buttonNames)
	{
		List<GIcon> list = new List<GIcon>();
		Canvas canvas = new Canvas();
		canvas.Width = (double)(buttonNames.Length * 86);
		canvas.Height = 25.0;
		canvas.HorizontalAlignment = global::Layout.Center;
		for (int i = 0; i < buttonNames.Length; i++)
		{
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 81.0;
			gicon.Height = 21.0;
			gicon.BodySource = SystemWizardPartMU._BodySource;
			gicon.NewSource = SystemWizardPartMU._NewSource;
			gicon.DisableBodySource = SystemWizardPartMU._DisableBodySource;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
			gicon.Text = buttonNames[i];
			gicon.ItemCode = i;
			Canvas.SetLeft(gicon, i * 86);
			Canvas.SetTop(gicon, 0);
			canvas.Children.Add(gicon);
			gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonUp);
			list.Add(gicon);
		}
		this.ItemPanel.Children.Add(canvas);
		return list;
	}

	private void ToUseEquips(int[] newEquipDbIDList)
	{
		if (newEquipDbIDList == null || newEquipDbIDList.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < newEquipDbIDList.Length; i++)
		{
			Global.ToUseGoodsDBId(newEquipDbIDList[i], true);
		}
	}

	public void ZhiJieXueXiJiNen()
	{
		MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(this.SkillID);
		if (skillXmlNode == null)
		{
			return;
		}
		object skillDataByID = Global.GetSkillDataByID(this.SkillID);
		if (skillDataByID == null)
		{
			int magicsBook = skillXmlNode.MagicsBook;
			if (magicsBook > 0)
			{
				if (Global.GetTotalGoodsCountByID(magicsBook) <= 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您背包里没有【{0}】, 无法学习技能"), new object[]
					{
						Global.GetGoodsNameByID(magicsBook, false)
					}), 0, -1, -1, 0);
					return;
				}
				GoodsData goodsDataByID = Global.GetGoodsDataByID(magicsBook);
				if (goodsDataByID != null)
				{
					if (!Global.GoodsCoolDown(goodsDataByID.GoodsID))
					{
						GameInstance.Game.SpriteUseGoods(goodsDataByID.Id, goodsDataByID.GoodsID, 1);
					}
					else
					{
						string goodsNameByID = Global.GetGoodsNameByID(goodsDataByID.GoodsID, false);
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】在冷却中, 无法使用"), new object[]
						{
							goodsNameByID
						}), 0, -1, -1, 0);
					}
				}
			}
		}
	}

	private void IconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		DPSelectedItemEventArgs dpselectedItemEventArgs = sender as DPSelectedItemEventArgs;
		switch (dpselectedItemEventArgs.ID)
		{
		case 1:
			if (this.taskVOItem != null && this.TaskID != (int)ConfigSystemParam.GetSystemParamIntByName("FirstMainTaskID"))
			{
				Super.PrccessAutoTaskFindRoad(this.TaskID, false, true, false, false);
			}
			break;
		case 2:
			if (this.taskVOItem != null && this.TaskID != (int)ConfigSystemParam.GetSystemParamIntByName("FirstMainTaskID"))
			{
				Global.Data.GameScene.CancelAutoFight(0, true);
				int mapCode = -1;
				int npcType = -1;
				int npcID = -1;
				Super.GetTaskDestNPCID(this.taskVOItem, ref mapCode, ref npcType, ref npcID);
				Super.AutoFindRoad(mapCode, npcType, npcID, (Global.GetTaskClassByID(this.TaskID) <= 0) ? 1 : 0);
			}
			break;
		case 3:
		case 4:
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("NaviBuyDrugsItems", ',');
			if (systemParamIntArrayByName.Length == 2)
			{
				int num = systemParamIntArrayByName[0];
				int npcID2 = systemParamIntArrayByName[1];
				Super.AutoFindRoad(num, 3, npcID2, 1);
				if (Global.Data.roleData.MapCode != num)
				{
					GameInstance.Game.SpriteGotToMap(num);
				}
			}
			break;
		}
		case 5:
			this.ToUseEquips(dpselectedItemEventArgs.EquipIDs);
			break;
		case 6:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 3
				});
			}
			break;
		case 7:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 4
				});
			}
			break;
		case 8:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 4
				});
			}
			break;
		case 9:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 2
					});
				}
			}
			else if (dpselectedItemEventArgs.ItemCode == 1)
			{
				Global.ToUseGoodsDBId(this.GoodsDbID, true);
			}
			break;
		case 10:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 8
				});
			}
			break;
		case 11:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 9
				});
			}
			break;
		case 12:
			if (dpselectedItemEventArgs.ItemCode == 0 && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 5
				});
			}
			break;
		case 13:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 6
				});
			}
			break;
		case 14:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 7
				});
			}
			break;
		case 15:
		{
			int mapCode2 = 1;
			int npcType2 = 3;
			int npcID3 = 1;
			Super.AutoFindRoad(mapCode2, npcType2, npcID3, 1);
			break;
		}
		case 16:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 2
					});
				}
			}
			else if (dpselectedItemEventArgs.ItemCode == 1)
			{
				Global.ToUseGoodsDBId(this.GoodsDbID, true);
			}
			break;
		case 17:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 5
				});
			}
			break;
		case 18:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 10
				});
			}
			break;
		case 19:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 11
				});
			}
			break;
		case 20:
			Global.ToUseGoodsDBId(this.GoodsDbID, true);
			break;
		case 21:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 2
					});
				}
			}
			else if (dpselectedItemEventArgs.ItemCode == 1)
			{
				Global.ToUseGoodsDBId(this.GoodsDbID, true);
			}
			break;
		case 22:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 2
					});
				}
			}
			else if (dpselectedItemEventArgs.ItemCode == 1)
			{
				Global.ToUseGoodsDBId(this.GoodsDbID, true);
			}
			break;
		case 23:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 2
					});
				}
			}
			else if (dpselectedItemEventArgs.ItemCode == 1)
			{
				Global.ToUseGoodsDBId(this.GoodsDbID, true);
			}
			break;
		case 24:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 2
					});
				}
			}
			else if (dpselectedItemEventArgs.ItemCode == 1)
			{
				Global.ToUseGoodsDBId(this.GoodsDbID, true);
			}
			break;
		case 25:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 12
				});
			}
			break;
		case 26:
			if (Global.GetMapType(Global.Data.roleData.MapCode) == MapTypes.NormalCopy)
			{
				int num2 = -1;
				Global.GetFirstNPCPointAndID(Global.Data.roleData.MapCode, out num2);
				if (num2 != -1)
				{
					Super.AutoFindRoad(Global.Data.roleData.MapCode, 3, num2, 1);
				}
			}
			break;
		case 27:
			if (this.taskVOItem != null)
			{
				int mapCode3 = -1;
				int npcType3 = -1;
				int npcID4 = -1;
				Super.GetTaskSourceNPCID(this.taskVOItem, out mapCode3, out npcType3, out npcID4);
				Super.AutoFindRoad(mapCode3, npcType3, npcID4, 1);
			}
			break;
		case 28:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				int mapCode4 = 2;
				int npcType4 = 3;
				int npcID5 = 240;
				Super.AutoFindRoad(mapCode4, npcType4, npcID5, 1);
			}
			else if (dpselectedItemEventArgs.ItemCode == 1 && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 21
				});
			}
			break;
		case 29:
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(this.TaskID);
			if (taskXmlNodeByID != null)
			{
				int mapCode5 = 2;
				int npcType5 = 3;
				int npcID6 = 0;
				int taskClass = taskXmlNodeByID.TaskClass;
				if (taskClass == 3)
				{
					npcID6 = 207;
				}
				else if (taskClass == 4)
				{
					npcID6 = 204;
				}
				else if (taskClass == 5)
				{
					npcID6 = 225;
				}
				else if (taskClass == 7)
				{
					npcID6 = 236;
				}
				Super.AutoFindRoad(mapCode5, npcType5, npcID6, 1);
			}
			break;
		}
		case 30:
		{
			int laoFangMapCode = Global.GetLaoFangMapCode();
			int npcType6 = 3;
			int npcID7 = 162;
			Super.AutoFindRoad(laoFangMapCode, npcType6, npcID7, 1);
			break;
		}
		case 33:
			GameInstance.Game.SpriteFindBiaoChe();
			break;
		case 34:
			for (int i = 0; i < Global.LineDataList.Count; i++)
			{
				if (Global.LineDataList[i].LineID == this.YaBiaoLineID)
				{
					LineData lineData = Global.LineDataList[i];
					if (lineData.LineID != Global.CurrentListData.LineID)
					{
						Global.CurrentListData = lineData;
						string xapParamByName = Super.GetXapParamByName("serverip", "127.0.0.1");
						Global.CurrentListData.GameServerIP = xapParamByName;
						if (this.DPSelectedItem != null)
						{
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								ID = 0,
								IDType = 13
							});
						}
					}
					break;
				}
			}
			break;
		case 35:
			if (dpselectedItemEventArgs.ItemCode == 0 && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = (int)ConfigSystemParam.GetSystemParamIntByName("DailyChongXueGoodsID"),
					IDType = 14
				});
			}
			break;
		case 36:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 15
					});
				}
			}
			else if (dpselectedItemEventArgs.ItemCode == 1 && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 16
				});
			}
			break;
		case 37:
			if (dpselectedItemEventArgs.ItemCode == 0 && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = (int)ConfigSystemParam.GetSystemParamIntByName("LingFuGoodsID"),
					IDType = 17
				});
			}
			break;
		case 38:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				int mapCode6 = 2;
				int npcType7 = 3;
				int npcID8 = 175;
				Super.AutoFindRoad(mapCode6, npcType7, npcID8, 1);
			}
			break;
		case 39:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				int mapCode7 = 2;
				int npcType8 = 3;
				int npcID9 = 176;
				Super.AutoFindRoad(mapCode7, npcType8, npcID9, 1);
			}
			break;
		case 40:
			if (dpselectedItemEventArgs.ItemCode == 0 && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 18
				});
			}
			break;
		case 41:
			if (dpselectedItemEventArgs.ItemCode == 0 && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 14
				});
			}
			break;
		case 42:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				int mapCode8 = 2;
				int npcType9 = 3;
				int npcID10 = 17;
				Super.AutoFindRoad(mapCode8, npcType9, npcID10, 1);
			}
			break;
		case 43:
			if (this.taskVOItem != null)
			{
				int mapCode9 = -1;
				int npcType10 = -1;
				int npcID11 = -1;
				Super.GetTaskSourceNPCID(this.taskVOItem, out mapCode9, out npcType10, out npcID11);
				Super.AutoFindRoad(mapCode9, npcType10, npcID11, 1);
			}
			break;
		case 44:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				int mapCode10 = 2;
				int npcType11 = 3;
				int npcID12 = 168;
				Super.AutoFindRoad(mapCode10, npcType11, npcID12, 1);
			}
			break;
		case 47:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				int mapCode11 = 2;
				int npcType12 = 3;
				int npcID13 = 186;
				Super.AutoFindRoad(mapCode11, npcType12, npcID13, 1);
			}
			break;
		case 48:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				int mapCode12 = 2;
				int npcType13 = 3;
				int npcID14 = 187;
				Super.AutoFindRoad(mapCode12, npcType13, npcID14, 1);
			}
			break;
		case 49:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				Super.InstantPageRefresh();
			}
			break;
		case 50:
			if (dpselectedItemEventArgs.ItemCode == 0 && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.ToByGoodsID,
					IDType = 19,
					buyFrom = this._ToByGoodsFromType
				});
			}
			break;
		case 51:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 2
					});
				}
			}
			else if (dpselectedItemEventArgs.ItemCode == 1)
			{
				Global.ToUseGoodsDBId(this.GoodsDbID, true);
			}
			break;
		case 53:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 2
					});
				}
			}
			else if (dpselectedItemEventArgs.ItemCode == 1 && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 22
				});
			}
			break;
		case 54:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				if (dpselectedItemEventArgs.ItemCode == 0 && this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = this.ToByGoodsID,
						IDType = 19,
						buyFrom = 0
					});
				}
			}
			else if (dpselectedItemEventArgs.ItemCode == 1 && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 23,
					buyFrom = 0
				});
			}
			break;
		case 55:
			if (dpselectedItemEventArgs.ItemCode == 0)
			{
				if (dpselectedItemEventArgs.ItemCode == 0 && this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = this.ToByGoodsID,
						IDType = 19,
						buyFrom = 0
					});
				}
			}
			else if (dpselectedItemEventArgs.ItemCode == 1 && this.DPSelectedItem != null)
			{
				int mapCode13 = 30;
				int npcType14 = 2;
				int npcID15 = 3000;
				Super.AutoFindRoad(mapCode13, npcType14, npcID15, 0);
			}
			break;
		case 56:
			Global.ToUseGoodsDBId(this.GoodsDbID, true);
			break;
		case 57:
			if (Global.g_bIsYaoQingCeShi)
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventSystemWizard", 1));
			}
			else
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventSystemWizard", 2));
			}
			break;
		case 58:
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventSystemWizard", 1));
			break;
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 0
			});
		}
	}

	private string GetColorText(string text, string colorText = "FFFFFFFF", bool underLine = false)
	{
		return StringUtil.substitute(Global.GetLang("｛color=#{0} uline={1} tag= text={2}｝"), new object[]
		{
			colorText,
			(!underLine) ? "false" : "true",
			text
		});
	}

	public void InitPartData(int wizardType)
	{
		switch (wizardType)
		{
		case 56:
			break;
		case 57:
			break;
		case 58:
			break;
		default:
			if (wizardType != 5)
			{
				return;
			}
			break;
		}
		switch (wizardType)
		{
		case 1:
			this.taskVOItem = ConfigTasks.GetTaskXmlNodeByID(this.TaskID);
			if (this.taskVOItem != null)
			{
				string title = this.taskVOItem.Title;
				string text = StringUtil.substitute(Global.GetLang("接受【{0}】任务"), new object[]
				{
					this.GetColorText(title, "FF00FF00", true)
				});
				TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				text = StringUtil.substitute(Global.GetLang("点击任务追踪{0}可以自动寻路"), new object[]
				{
					this.GetColorText(Global.GetLang("下划线文字"), "FFFFDC4E", true)
				});
				textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				string[] buttonNames = new string[]
				{
					Global.GetLang("立即前往")
				};
				List<GIcon> list = this.AddButtons(buttonNames);
				if ((!Global.IsAutoFighting() || Global.Data.GameScene.GetAutoFightTargetMonsterID() >= 0) && this.AllInstantAutoFindRoad)
				{
					this.IconMouseLeftButtonUp(list[0], null);
				}
			}
			break;
		case 2:
			this.taskVOItem = ConfigTasks.GetTaskXmlNodeByID(this.TaskID);
			if (this.taskVOItem != null)
			{
				string title2 = this.taskVOItem.Title;
				string text = StringUtil.substitute(Global.GetLang("完成【{0}】任务"), new object[]
				{
					this.GetColorText(title2, "FF00FF00", true)
				});
				TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				text = StringUtil.substitute(Global.GetLang("可获得丰厚的经验及绑定金币奖励"), new object[0]);
				textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				string[] buttonNames2 = new string[]
				{
					Global.GetLang("立即前往")
				};
				List<GIcon> list2 = this.AddButtons(buttonNames2);
				if ((!Global.IsAutoFighting() || Global.Data.GameScene.GetAutoFightTargetMonsterID() >= 0) && this.AllInstantAutoFindRoad)
				{
					this.IconMouseLeftButtonUp(list2[0], null);
				}
			}
			break;
		case 3:
		{
			string text = StringUtil.substitute(Global.GetLang("【龙城药店买药】"), new object[0]);
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("您的背包中没有任何补血的药品了"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("为了您的安全，请您去找{0}购买一些"), new object[]
			{
				this.GetColorText(Global.GetLang("药店老板"), "FFFFDC4E", true)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames3 = new string[]
			{
				Global.GetLang("前去买药")
			};
			this.AddButtons(buttonNames3);
			break;
		}
		case 4:
		{
			string text = StringUtil.substitute(Global.GetLang("【龙城药店买药】"), new object[0]);
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("您的背包中没有任何补魔法的药品了"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("为了您的安全，请您去找{0}购买一些"), new object[]
			{
				this.GetColorText(Global.GetLang("药店老板"), "FFFFDC4E", true)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames4 = new string[]
			{
				Global.GetLang("前去买药")
			};
			this.AddButtons(buttonNames4);
			break;
		}
		case 5:
		{
			GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(this.GoodsDbID, null);
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsDataByDbID.GoodsID);
			if (goodsCatetoriy != 9 && goodsCatetoriy != 10 && goodsCatetoriy != 23)
			{
				this.AddWizardItem(goodsDataByDbID, SystemWizardTypes.NewEquip, this.EquipTipsTime, this.AlphaTime);
			}
			return;
		}
		case 6:
		{
			string text = StringUtil.substitute(Global.GetLang("【闭关已经满12小时】"), new object[0]);
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("【经验收益】和【灵力收益】已满，不再增长"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("请您尽快领走您的收益"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("之后系统将自动开始为您积累下一轮闭关收益"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames5 = new string[]
			{
				Global.GetLang("领取收益")
			};
			this.AddButtons(buttonNames5);
			break;
		}
		case 7:
		{
			string skillNameByID = ConfigMagicInfos.GetSkillNameByID(this.SkillID);
			string text = StringUtil.substitute(Global.GetLang("【{0}】技能"), new object[]
			{
				this.GetColorText(skillNameByID, "FF00FF00", false)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("可以升级了"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("按{0}键可以打开技能窗口"), new object[]
			{
				this.GetColorText("V", "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames6 = new string[]
			{
				Global.GetLang("升级技能")
			};
			this.AddButtons(buttonNames6);
			break;
		}
		case 8:
		{
			GoodsData goodsDataByDbID2 = Global.GetGoodsDataByDbID(this.GoodsDbID, null);
			if (goodsDataByDbID2 != null)
			{
				this.SkillID = Global.GetSkillIDByBook(Global.Data.roleData.Occupation, goodsDataByDbID2.GoodsID);
			}
			string text2 = this.AddGoodsImageName(this.GoodsDbID);
			TextBlock textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(text2, "FF00FF00", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			if (this.SkillID == 16)
			{
				text = StringUtil.substitute(Global.GetLang("召唤火精灵附在剑上，从而造成额外伤害"), new object[0]);
			}
			else if (this.SkillID == 32)
			{
				text = StringUtil.substitute(Global.GetLang("召唤强力的暴风雪,伤害法术区域内的生物"), new object[0]);
			}
			else if (this.SkillID == 51)
			{
				text = StringUtil.substitute(Global.GetLang("能够召唤一只强大神兽作自己的随从"), new object[0]);
			}
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames7 = new string[]
			{
				Global.GetLang("立即学习")
			};
			this.AddButtons(buttonNames7);
			Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("系统向导UI"), 680000, 0, 1);
			break;
		}
		case 9:
		{
			string text = StringUtil.substitute(Global.GetLang("【可以领取礼物】"), new object[0]);
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("升级有礼条件达成"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("请打开背包中的升级礼包"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames8 = new string[]
			{
				Global.GetLang("打开背包"),
				Global.GetLang("直接领取")
			};
			this.AddButtons(buttonNames8);
			break;
		}
		case 10:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("登陆有礼"), "FF00FF00", false)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("可以领取了"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击【{0}】按钮可以打开礼物窗口"), new object[]
			{
				this.GetColorText(Global.GetLang("领取礼物"), "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames9 = new string[]
			{
				Global.GetLang("领取礼物")
			};
			this.AddButtons(buttonNames9);
			break;
		}
		case 11:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("在线有礼"), "FF00FF00", false)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("可以领取了"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击【{0}】按钮可以打开礼物窗口"), new object[]
			{
				this.GetColorText(Global.GetLang("领取礼物"), "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames10 = new string[]
			{
				Global.GetLang("领取礼物")
			};
			this.AddButtons(buttonNames10);
			break;
		}
		case 12:
		{
			string text = StringUtil.substitute(Global.GetLang("【灵力已经满了】"), new object[0]);
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("灵力满后不再增长"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("灵力可以用于冲击经脉"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("冲击经脉的穴位成功不但会给你大量经验"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("还可以给周围的其他人大量经验"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames11 = new string[]
			{
				Global.GetLang("冲击经脉")
			};
			this.AddButtons(buttonNames11);
			break;
		}
		case 13:
		{
			string text3 = this.AddGoodsImageName(this.GoodsDbID);
			TextBlock textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string text = StringUtil.substitute(Global.GetLang("恭喜您得到了新的坐骑道具"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(text3, "FF00FF00", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames12 = new string[]
			{
				Global.GetLang("获取坐骑")
			};
			this.AddButtons(buttonNames12);
			break;
		}
		case 14:
		{
			string text4 = this.AddGoodsImageName(this.GoodsDbID);
			TextBlock textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string text = StringUtil.substitute(Global.GetLang("恭喜您得到了新的宠物道具"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(text4, "FF00FF00", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("按{0}键可以打开宠物窗口"), new object[]
			{
				this.GetColorText("P", "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames13 = new string[]
			{
				Global.GetLang("获取宠物")
			};
			this.AddButtons(buttonNames13);
			break;
		}
		case 15:
		{
			string text = StringUtil.substitute(Global.GetLang("接受【{0}】任务"), new object[]
			{
				this.GetColorText(Global.GetLang("第一个"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击下边的按钮接受您的第一个任务"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames14 = new string[]
			{
				Global.GetLang("第一个任务")
			};
			this.AddButtons(buttonNames14);
			break;
		}
		case 16:
		{
			string text5 = this.AddGoodsImageName(this.GoodsDbID);
			TextBlock textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string text = StringUtil.substitute(Global.GetLang("恭喜您得到了【{0}】"), new object[]
			{
				this.GetColorText(text5, "FF00FF00", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击[{0}]，可以获取灵力"), new object[]
			{
				this.GetColorText(Global.GetLang("立即使用"), "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames15 = new string[]
			{
				Global.GetLang("查看背包"),
				Global.GetLang("立即使用")
			};
			this.AddButtons(buttonNames15);
			break;
		}
		case 17:
		{
			string text = StringUtil.substitute(Global.GetLang("【灵力池中的超过了5000】"), new object[0]);
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("您可以使用灵力冲击经脉"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("冲击经脉的穴位成功后会给您带来属性加成"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("冲击经脉的穴位成功不但会给你大量经验"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("还可以给周围的其他人大量经验"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames16 = new string[]
			{
				Global.GetLang("冲击经脉")
			};
			this.AddButtons(buttonNames16);
			break;
		}
		case 18:
		{
			string text = StringUtil.substitute(Global.GetLang("【做任务打怪操作太累？】"), new object[0]);
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("按{0}键可以进入自动挂机打怪的模式"), new object[]
			{
				this.GetColorText("Z", "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("省时省力，悠哉悠哉!"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames17 = new string[]
			{
				Global.GetLang("挂机打怪")
			};
			this.AddButtons(buttonNames17);
			break;
		}
		case 19:
		{
			string text = StringUtil.substitute(Global.GetLang("【新手见面礼物可以领取了】"), new object[0]);
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("礼物不停送，升级更轻松"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击[立即领取]得到新手见面礼物"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames18 = new string[]
			{
				Global.GetLang("立即领取")
			};
			this.AddButtons(buttonNames18);
			break;
		}
		case 20:
		{
			string text6 = this.AddGoodsImageName(this.GoodsDbID);
			TextBlock textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string text = StringUtil.substitute(Global.GetLang("恭喜您得到了【{0}】"), new object[]
			{
				this.GetColorText(text6, "FF00FF00", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("自动为您恢复生命值及魔法值"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames19 = new string[]
			{
				Global.GetLang("立即使用")
			};
			this.AddButtons(buttonNames19);
			break;
		}
		case 21:
		{
			string text7 = this.AddGoodsImageName(this.GoodsDbID);
			TextBlock textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string text = StringUtil.substitute(Global.GetLang("恭喜您得到了【{0}】"), new object[]
			{
				this.GetColorText(text7, "FF00FF00", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击[{0}]，可以魔法值不足时自动补魔法值"), new object[]
			{
				this.GetColorText(Global.GetLang("立即使用"), "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames20 = new string[]
			{
				Global.GetLang("查看背包"),
				Global.GetLang("立即使用")
			};
			this.AddButtons(buttonNames20);
			break;
		}
		case 22:
		{
			string text8 = this.AddGoodsImageName(this.GoodsDbID);
			TextBlock textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string text = StringUtil.substitute(Global.GetLang("恭喜您得到了【{0}】"), new object[]
			{
				this.GetColorText(text8, "FF00FF00", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击[{0}]，可以打怪时收获双倍的经验"), new object[]
			{
				this.GetColorText(Global.GetLang("立即使用"), "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames21 = new string[]
			{
				Global.GetLang("查看背包"),
				Global.GetLang("立即使用")
			};
			this.AddButtons(buttonNames21);
			break;
		}
		case 23:
		{
			string text9 = this.AddGoodsImageName(this.GoodsDbID);
			TextBlock textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string text = StringUtil.substitute(Global.GetLang("恭喜您得到了【{0}】"), new object[]
			{
				this.GetColorText(text9, "FF00FF00", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击[{0}]，打坐时收获双倍的灵力"), new object[]
			{
				this.GetColorText(Global.GetLang("立即使用"), "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames22 = new string[]
			{
				Global.GetLang("查看背包"),
				Global.GetLang("立即使用")
			};
			this.AddButtons(buttonNames22);
			break;
		}
		case 24:
		{
			string text10 = this.AddGoodsImageName(this.GoodsDbID);
			TextBlock textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string text = StringUtil.substitute(Global.GetLang("恭喜您得到了【{0}】"), new object[]
			{
				this.GetColorText(text10, "FF00FF00", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击[{0}]，可以打怪时收获双倍的绑定金币"), new object[]
			{
				this.GetColorText(Global.GetLang("立即使用"), "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames23 = new string[]
			{
				Global.GetLang("查看背包"),
				Global.GetLang("立即使用")
			};
			this.AddButtons(buttonNames23);
			break;
		}
		case 25:
		{
			string text = StringUtil.substitute(Global.GetLang("您背包已经满了"), new object[0]);
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("可以将暂时用不到的物品出售"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("也可以将暂时用不到的物品"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("移动到随身仓库"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames24 = new string[]
			{
				Global.GetLang("存到仓库")
			};
			this.AddButtons(buttonNames24);
			break;
		}
		case 26:
		{
			string text = StringUtil.substitute(Global.GetLang("{0}您太勇猛了！"), new object[]
			{
				Global.GetLang("英雄")
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("本副本地图中怪物已全部被您诛杀"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			if (Global.CanGetFuBenMapAwards())
			{
				text = StringUtil.substitute(Global.GetLang("请到副本NPC处领取奖励"), new object[0]);
				textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
				this.ItemPanel.Children.Add(textBlock);
			}
			else
			{
				text = StringUtil.substitute(Global.GetLang("请到副本NPC处进入下一层"), new object[0]);
				textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
				this.ItemPanel.Children.Add(textBlock);
			}
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			if (Global.CanGetFuBenMapAwards())
			{
				string[] buttonNames25 = new string[]
				{
					Global.GetLang("领取奖励")
				};
				List<GIcon> list3 = this.AddButtons(buttonNames25);
			}
			else
			{
				string[] buttonNames26 = new string[]
				{
					Global.GetLang("离开副本")
				};
				List<GIcon> list3 = this.AddButtons(buttonNames26);
			}
			break;
		}
		case 27:
			this.taskVOItem = ConfigTasks.GetTaskXmlNodeByID(this.TaskID);
			if (this.taskVOItem != null)
			{
				string title3 = this.taskVOItem.Title;
				string text = StringUtil.substitute(Global.GetLang("接取【{0}】任务"), new object[]
				{
					this.GetColorText(title3, "FF00FF00", true)
				});
				TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				text = StringUtil.substitute(Global.GetLang("现在可以重新接取主线任务了"), new object[0]);
				textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				string[] buttonNames27 = new string[]
				{
					Global.GetLang("接取任务")
				};
				this.AddButtons(buttonNames27);
			}
			break;
		case 28:
		{
			string text = StringUtil.substitute(Global.GetLang("主线任务断层"), new object[0]);
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 255, 0, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("最佳途径是进入【{0}】爬楼快速升级"), new object[]
			{
				this.GetColorText(Global.GetLang("通天塔"), "FF00FF00", true)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames28 = new string[]
			{
				Global.GetLang("进入通天塔")
			};
			this.AddButtons(buttonNames28);
			break;
		}
		case 29:
		{
			string text = StringUtil.substitute(Global.GetLang("接受【{0}】任务"), new object[]
			{
				this.GetColorText(Global.GetLang("日常任务"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击下边的按钮接取下一环任务"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames29 = new string[]
			{
				Global.GetLang("下一环任务")
			};
			List<GIcon> list4 = this.AddButtons(buttonNames29);
			this.IconMouseLeftButtonUp(list4[0], null);
			break;
		}
		case 30:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("出狱提醒"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("牢房刑期已满，可以出狱了"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("以后注意，不要恶意杀人过多"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames30 = new string[]
			{
				Global.GetLang("立刻出狱")
			};
			this.AddButtons(buttonNames30);
			break;
		}
		case 31:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("押镖失败"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			if (this.YaBiaoFailType == 1)
			{
				text = StringUtil.substitute(Global.GetLang("运镖所用时间超过了限定期限。"), new object[0]);
			}
			else if (this.YaBiaoFailType == 2)
			{
				text = StringUtil.substitute(Global.GetLang("押运的镖银已被打劫。"), new object[0]);
			}
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("下边这些是安慰奖励:"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			Global.GetYaBiaoReward(this.YaBiaoID, out num, out num2, out num3);
			text = StringUtil.substitute(Global.GetLang("获得经验：{0}"), new object[]
			{
				num2 / 2
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("退还押金：{0} 金币"), new object[]
			{
				0
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames31 = new string[]
			{
				Global.GetLang("关闭")
			};
			this.AddButtons(buttonNames31);
			break;
		}
		case 32:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("押镖成功"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("英雄果然神武，走这趟镖没少出力。"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("下边这些奖励是你应得的:"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			Global.GetYaBiaoReward(this.YaBiaoID, out num4, out num5, out num6);
			if (this.YaBiaoTouBao > 0)
			{
			}
			text = StringUtil.substitute(Global.GetLang("获得金币：{0} 两"), new object[]
			{
				num4
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("获得经验：{0}"), new object[]
			{
				num5
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("退还押金：{0} 金币"), new object[]
			{
				num6
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames32 = new string[]
			{
				Global.GetLang("关闭")
			};
			this.AddButtons(buttonNames32);
			break;
		}
		case 33:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(StringUtil.substitute(Global.GetLang("{0}丢失"), new object[]
				{
					Global.GetYaBiaoName(this.YaBiaoID)
				}), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("英雄你忘记自己的责任了吗？"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("你押运的{0}丢失了！"), new object[]
			{
				StringUtil.substitute(Global.GetLang("找{0}"), new object[]
				{
					Global.GetYaBiaoName(this.YaBiaoID)
				})
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames33 = new string[]
			{
				StringUtil.substitute(Global.GetLang("找{0}"), new object[]
				{
					Global.GetYaBiaoName(this.YaBiaoID)
				})
			};
			this.AddButtons(buttonNames33);
			break;
		}
		case 34:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(StringUtil.substitute(Global.GetLang("{0}丢失"), new object[]
				{
					Global.GetYaBiaoName(this.YaBiaoID)
				}), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("英雄你押运的镖不在这条线上。"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("请转到【{0}】线接镖或交镖!"), new object[]
			{
				this.YaBiaoLineID
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames34 = new string[]
			{
				Global.GetLang("切换线路")
			};
			this.AddButtons(buttonNames34);
			break;
		}
		case 35:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(StringUtil.substitute(Global.GetLang("剩余冲穴次数为{0}"), new object[]
				{
					Global.TodayChongXueNum()
				}), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("系统每日为你提供10次冲穴的机会。"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("您今日的剩余冲穴次数已经为{0}"), new object[]
			{
				Global.TodayChongXueNum()
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("您剩余的灵力还有:{0}, 可能被浪费 "), new object[]
			{
				this.GetColorText(Global.Data.roleData.InterPower.ToString(), "FFFF0000", true)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string goodsNameByID = Global.GetGoodsNameByID((int)ConfigSystemParam.GetSystemParamIntByName("DailyChongXueGoodsID"), false);
			text = StringUtil.substitute(Global.GetLang("使用【{0}】,可以增加今日的冲穴次数。"), new object[]
			{
				goodsNameByID
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames35 = new string[]
			{
				Global.GetLang("立刻购买"),
				Global.GetLang("以后再说")
			};
			this.AddButtons(buttonNames35);
			break;
		}
		case 36:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("剩余灵力不足"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("通过打坐可以获取灵力"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("使用【高级灵力丹】可以快速获得灵力"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames36 = new string[]
			{
				Global.GetLang("立刻购买"),
				Global.GetLang("打坐获取")
			};
			this.AddButtons(buttonNames36);
			break;
		}
		case 37:
		{
			int id = (int)ConfigSystemParam.GetSystemParamIntByName("LingFuGoodsID");
			string goodsNameByID2 = Global.GetGoodsNameByID(id, false);
			string text = StringUtil.substitute(Global.GetLang("没有【{0}】"), new object[]
			{
				this.GetColorText(goodsNameByID2, "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("灵兽峰副本必掉兑换神级装备的【天地精元】"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("进入灵兽峰副本需要扣除一个【{0}】"), new object[]
			{
				goodsNameByID2
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("你可以从商城中购买【{0}】"), new object[]
			{
				goodsNameByID2
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames37 = new string[]
			{
				Global.GetLang("立刻购买"),
				Global.GetLang("以后再说")
			};
			this.AddButtons(buttonNames37);
			break;
		}
		case 38:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("押镖活动开始"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("激动人心的押镖活动已经开始了"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("时间有限，请大家抓紧时间"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("押镖成功可以获取高额的金币和大量的经验"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("劫镖成功也可以获取高额的金币和大量的经验"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames38 = new string[]
			{
				Global.GetLang("立刻前去"),
				Global.GetLang("以后再说")
			};
			this.AddButtons(buttonNames38);
			break;
		}
		case 39:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("灵兽峰副本开放了"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("刺激万分的灵兽峰副本已经开放了"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("灵兽峰副本中掉落极品装备和高级宝石"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("还掉落各种珍稀的商城道具, 机会难得"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("时间有限，请大家抓紧时间"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames39 = new string[]
			{
				Global.GetLang("立刻前去"),
				Global.GetLang("以后再说")
			};
			this.AddButtons(buttonNames39);
			break;
		}
		case 40:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("钻石数量不足"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("您的钻石数量不足,"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("充值可以获取钻石"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("充值还可以额外获取充值大礼包"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames40 = new string[]
			{
				Global.GetLang("立刻充值"),
				Global.GetLang("以后再说")
			};
			this.AddButtons(buttonNames40);
			break;
		}
		case 41:
		{
			int id2 = (int)ConfigSystemParam.GetSystemParamIntByName("YaBiaoLingGoodsID");
			string goodsNameByID3 = Global.GetGoodsNameByID(id2, false);
			string text = StringUtil.substitute(Global.GetLang("没有【{0}】"), new object[]
			{
				this.GetColorText(goodsNameByID3, "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("使用【{0}】可以获取更多的押镖机会"), new object[]
			{
				goodsNameByID3
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("押镖成功可以获取高额的金币和大量的经验"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("你可以从商城中购买【{0}】"), new object[]
			{
				goodsNameByID3
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames41 = new string[]
			{
				Global.GetLang("立刻购买"),
				Global.GetLang("以后再说")
			};
			this.AddButtons(buttonNames41);
			break;
		}
		case 42:
		{
			int id3 = (int)ConfigSystemParam.GetSystemParamIntByName("XiaoLaBaGoodsID");
			string goodsNameByID4 = Global.GetGoodsNameByID(id3, false);
			string text = StringUtil.substitute(Global.GetLang("没有【{0}】"), new object[]
			{
				this.GetColorText(goodsNameByID4, "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("世界聊天频道喊话要使用【{0}】"), new object[]
			{
				goodsNameByID4
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("可在扬州城NPC【杂货商·牛二】用绑定金币购买"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames42 = new string[]
			{
				Global.GetLang("前去购买"),
				Global.GetLang("以后再说")
			};
			this.AddButtons(buttonNames42);
			break;
		}
		case 43:
			this.taskVOItem = ConfigTasks.GetTaskXmlNodeByID(this.TaskID);
			if (this.taskVOItem != null)
			{
				string title4 = this.taskVOItem.Title;
				string text = StringUtil.substitute(Global.GetLang("可接【{0}】支线任务"), new object[]
				{
					this.GetColorText(title4, "FF00FF00", true)
				});
				TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				text = StringUtil.substitute(Global.GetLang("系统提示你，现在你可以接受"), new object[0]);
				textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				text = StringUtil.substitute(Global.GetLang("【{0}】支线任务了"), new object[]
				{
					this.GetColorText(title4, "FFFFDC4E", false)
				});
				textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
				this.ItemPanel.Children.Add(textBlock);
				string[] buttonNames43 = new string[]
				{
					Global.GetLang("前去接取")
				};
				this.AddButtons(buttonNames43);
			}
			break;
		case 44:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("五行奇阵开放了"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("变幻莫测的五行奇阵已经开放了"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("时间有限，请大家抓紧时间"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("五行奇阵通关后可以获取大量的经验"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("五行奇阵每一层都可能有财神|金币、福神|经验"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames44 = new string[]
			{
				Global.GetLang("立刻前去"),
				Global.GetLang("以后再说")
			};
			this.AddButtons(buttonNames44);
			break;
		}
		case 45:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("双倍经验|双倍灵力活动开始"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("活动期间：打怪或打坐经验收益翻2倍，打坐灵力收益翻2倍"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("如果您使用双倍经验卡，打坐或打怪经验可翻3倍"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("如果您使用双倍灵力卡，打坐灵力收益可翻3倍"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("如果您的练级时间不多，那么双倍期间无论如何应该把握"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames45 = new string[]
			{
				Global.GetLang("我知道了")
			};
			this.AddButtons(buttonNames45);
			break;
		}
		case 46:
		{
			string skillNameByID2 = ConfigMagicInfos.GetSkillNameByID(this.SkillID);
			string text = StringUtil.substitute(Global.GetLang("【{0}】技能"), new object[]
			{
				this.GetColorText(skillNameByID2, "FF00FF00", false)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			SkillData skillDataByID = Global.GetSkillDataByID(this.SkillID);
			text = StringUtil.substitute(Global.GetLang("升级到了【{0}】级"), new object[]
			{
				(skillDataByID != null) ? skillDataByID.SkillLevel : 0
			});
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】技能, 升级到了【{1}】级"), new object[]
			{
				skillNameByID2,
				(skillDataByID != null) ? skillDataByID.SkillLevel : 0
			}), 0, -1, -1, 0);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("按{0}键可以打开技能窗口查看详情"), new object[]
			{
				this.GetColorText("V", "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames46 = new string[]
			{
				Global.GetLang("关闭")
			};
			List<GIcon> list5 = this.AddButtons(buttonNames46);
			this.IconMouseLeftButtonUp(list5[0], null);
			break;
		}
		case 47:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("战盟战开始了"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("激动人心的战盟战已经开始了"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("时间有限，请大家抓紧时间"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("夺取领地是君临天下的前提条件，千万莫要错过"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames47 = new string[]
			{
				Global.GetLang("立刻前去"),
				Global.GetLang("以后再说")
			};
			this.AddButtons(buttonNames47);
			break;
		}
		case 48:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("皇城战开始了"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("群雄逐鹿，争霸天下"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("任何一个战盟的首领只要夺取了【舍利之源】"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("就会成为本服令万人敬仰的城主"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames48 = new string[]
			{
				Global.GetLang("立刻前去"),
				Global.GetLang("以后再说")
			};
			this.AddButtons(buttonNames48);
			break;
		}
		case 49:
		{
			int num7 = 0;
			TextBlock textBlock;
			while (this.ReloadXmlMsg != null)
			{
				if (num7 >= this.ReloadXmlMsg.Length)
				{
					break;
				}
				string text = this.ReloadXmlMsg.Substring(num7, Math.Min(12, this.ReloadXmlMsg.Length - num7));
				num7 += text.Length;
				textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
				this.ItemPanel.Children.Add(textBlock);
			}
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames49 = new string[]
			{
				Global.GetLang("重进游戏"),
				Global.GetLang("等会儿再说")
			};
			this.AddButtons(buttonNames49);
			break;
		}
		case 50:
		{
			string text11 = string.Empty;
			string text;
			if (this._ToByGoodsFromType != 3)
			{
				text11 = Global.GetGoodsNameByID(this._ToByGoodsID, false);
				text = StringUtil.substitute(Global.GetLang("背包中没有【{0}】"), new object[]
				{
					this.GetColorText(text11, "FF00FF00", true)
				});
			}
			else
			{
				text = StringUtil.substitute(Global.GetLang("您的【{0}】余额不足"), new object[]
				{
					this.GetColorText(Global.GetLang("绑定金币"), "FF00FF00", true)
				});
			}
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			if (this._ToByGoodsFromType == 0)
			{
				text = StringUtil.substitute(Global.GetLang("你可以从商城中购买【{0}】"), new object[]
				{
					text11
				});
			}
			else if (this._ToByGoodsFromType == 1)
			{
				text = StringUtil.substitute(Global.GetLang("你可以从万宝阁中购买【{0}】"), new object[]
				{
					text11
				});
			}
			else if (this._ToByGoodsFromType == 2)
			{
				text = StringUtil.substitute(Global.GetLang("你可以从交易市场中购买【{0}】"), new object[]
				{
					text11
				});
			}
			else if (this._ToByGoodsFromType == 3)
			{
				text = StringUtil.substitute(Global.GetLang("从金币商•花茵娘用【金币】购买【{0}】"), new object[]
				{
					Global.GetLang("绑定金币")
				});
			}
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames50 = new string[]
			{
				Global.GetLang("立刻购买"),
				Global.GetLang("以后再说")
			};
			this.AddButtons(buttonNames50);
			break;
		}
		case 51:
		{
			string text12 = this.AddGoodsImageName(this.GoodsDbID);
			TextBlock textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string text = StringUtil.substitute(Global.GetLang("恭喜您得到了【{0}】"), new object[]
			{
				this.GetColorText(text12, "FF00FF00", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击[{0}]，可以得到绑定金币"), new object[]
			{
				this.GetColorText(Global.GetLang("立即使用"), "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames51 = new string[]
			{
				Global.GetLang("查看背包"),
				Global.GetLang("立即使用")
			};
			List<GIcon> list6 = this.AddButtons(buttonNames51);
			this.IconMouseLeftButtonUp(list6[1], null);
			break;
		}
		case 52:
		{
			string text = StringUtil.substitute(Global.GetLang("【{0}】"), new object[]
			{
				this.GetColorText(Global.GetLang("阵营战奖励"), "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("经验奖励: {0}"), new object[]
			{
				this.SuiTangBattleResultExpAward
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("绑定钻石奖励: {0}"), new object[]
			{
				this.SuiTangBattleResultBindYuanBaoAward
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("成就奖励: {0}"), new object[]
			{
				this.SuiTangBattleResultChengJiuAward
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames52 = new string[]
			{
				Global.GetLang("我知道了")
			};
			this.AddButtons(buttonNames52);
			break;
		}
		case 53:
		{
			string text13 = this.AddGoodsImageName(this.GoodsDbID);
			TextBlock textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string text = StringUtil.substitute(Global.GetLang("恭喜您得到了【{0}】"), new object[]
			{
				this.GetColorText(text13, "FF00FF00", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("点击[{0}]可以打开杨公宝库"), new object[]
			{
				this.GetColorText(Global.GetLang("立即使用"), "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("可以得到【{0}】的极品装备"), new object[]
			{
				this.GetColorText(Global.GetLang("紫色·无双"), "FFFA800A", false)
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames53 = new string[]
			{
				Global.GetLang("查看背包"),
				Global.GetLang("立即使用")
			};
			this.AddButtons(buttonNames53);
			break;
		}
		case 54:
		{
			string text14 = string.Empty;
			this._ToByGoodsID = Global.MapTransGoodsID;
			text14 = Global.GetGoodsNameByID(this._ToByGoodsID, false);
			string text = StringUtil.substitute(Global.GetLang("背包中没有【{0}】"), new object[]
			{
				this.GetColorText(text14, "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("你可以从商城中购买【{0}】"), new object[]
			{
				text14
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames54 = new string[]
			{
				Global.GetLang("立刻购买")
			};
			this.AddButtons(buttonNames54);
			break;
		}
		case 55:
		{
			string text15 = string.Empty;
			int num8 = (int)ConfigSystemParam.GetSystemParamIntByName("BindTongQianGoodsID");
			this._ToByGoodsID = num8;
			string text = StringUtil.substitute(Global.GetLang("【{0}】余额不足"), new object[]
			{
				this.GetColorText("绑定金币", "FF00FF00", true)
			});
			TextBlock textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text15 = Global.GetGoodsNameByID(num8, false);
			text = StringUtil.substitute(Global.GetLang("你可以从商城中购买【{0}】"), new object[]
			{
				text15
			});
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("双击使用后获得大量绑定金币"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 255), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("去【矿洞】挖矿得到矿石"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			text = StringUtil.substitute(Global.GetLang("出售后可以得到大量绑定金币"), new object[0]);
			textBlock = this.GetTextBlock2(text, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 255), double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			textBlock = this.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, double.NaN);
			this.ItemPanel.Children.Add(textBlock);
			string[] buttonNames55 = new string[]
			{
				Global.GetLang("立刻购买"),
				Global.GetLang("前去挖矿")
			};
			this.AddButtons(buttonNames55);
			break;
		}
		case 56:
		{
			GoodsData goodsDataByDbID3 = Global.GetGoodsDataByDbID(this.GoodsDbID, null);
			SystemWizardItem systemWizardItem = this.AddWizardItem(goodsDataByDbID3, SystemWizardTypes.HintMUTiShiGoods, this.OtherTipsTime, this.AlphaTime);
			break;
		}
		case 57:
		{
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(-2, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			dummyGoodsDataMu.GCount = this.GoodsCount;
			SystemWizardItem systemWizardItem2 = this.AddWizardItem(dummyGoodsDataMu, SystemWizardTypes.HintMUNewChengJiu, this.ChengJiuTipsTime, this.ChengJiuTipsTime / 5f);
			systemWizardItem2.Name = this.NewChengJiuName;
			break;
		}
		case 58:
		{
			GoodsData dummyGoodsDataMu2 = Global.GetDummyGoodsDataMu(-3, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			dummyGoodsDataMu2.GCount = this.GoodsCount;
			SystemWizardItem systemWizardItem3 = this.AddWizardItem(dummyGoodsDataMu2, SystemWizardTypes.HintMUNewHuoYue, this.HuoYuetipsTime, this.HuoYuetipsTime / 5f);
			systemWizardItem3.Name = this.NewChengJiuName;
			break;
		}
		}
	}

	private void Container_MouseRightButtonUp(MouseEvent e)
	{
	}

	private void UserControl_MouseLeftButtonUp(MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 1
			});
		}
	}

	public override void Destroy()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("系统向导UI"), null);
	}

	public static SystemWizardPartMU GetInstance(Component parent)
	{
		if (null == SystemWizardPartMU._Instance)
		{
			SystemWizardPartMU systemWizardPartMU = U3DUtils.NEW<SystemWizardPartMU>();
			U3DUtils.AddChild(parent.gameObject, systemWizardPartMU.gameObject, true);
			SystemWizardPartMU._Instance = systemWizardPartMU;
		}
		return SystemWizardPartMU._Instance;
	}

	public static void SetPosY(int posY)
	{
		if (null != SystemWizardPartMU._Instance)
		{
			SystemWizardPartMU._Instance._MyUIAnchor.offset.y = (float)(SystemWizardPartMU._Instance.MyUIAnchorPosY + posY);
		}
	}

	public static SystemWizardPartMU AddChild(Component parent, SystemWizardPartMU child)
	{
		SystemWizardPartMU instance = SystemWizardPartMU.GetInstance(parent);
		U3DUtils.AddChild(instance.gameObject, child.gameObject, false);
		return child;
	}

	public SystemWizardItem AddWizardItem(GoodsData goodsData, SystemWizardTypes type, float delay = -1f, float during = -1f)
	{
		SystemWizardItem systemWizardItem = null;
		if (this._ListBox.ItemsSource.Count > 2)
		{
			GameObject at = this._ListBox.ItemsSource.GetAt(0);
			SystemWizardItem component = at.GetComponent<SystemWizardItem>();
			if (component.args.ID == 5)
			{
				component.args.IDType = 1;
				this.RemoveSystemWizardItem(component);
			}
			else
			{
				this._ListBox.ItemsSource.RemoveAt(0);
			}
		}
		if (goodsData != null)
		{
			systemWizardItem = SystemWizardItem.AddWizardItem(this._ListBox.ItemsSource, new DPSelectedItemEventHandler(this.DPSelectItemHandler));
			systemWizardItem.args.ID = (int)type;
			systemWizardItem.args.ZhuZhuangBei = goodsData;
			systemWizardItem.args.EquipIDs = new int[]
			{
				goodsData.Id
			};
			systemWizardItem.InitTween(delay, during);
			systemWizardItem.Name = UIHelper.FormatGoodsName(goodsData, false, false, false);
			if (type == SystemWizardTypes.NewEquip)
			{
				if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 23)
				{
					systemWizardItem.ButtonName = Global.GetLang("激活");
				}
				else
				{
					systemWizardItem.ButtonName = Global.GetLang("佩戴");
				}
				string text = Global.GetLang(" ▲") + this.ZhanLiUp;
				systemWizardItem.Desc = Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					Global.GetLang("战力:") + Global.GetGoodsDataZhanLi(goodsData),
					"00ff00",
					text
				});
				if (this.ZhanLiUp <= 0)
				{
					systemWizardItem._Button.gameObject.SetActive(false);
				}
			}
			else if (type == SystemWizardTypes.HintMUTiShiGoods)
			{
				systemWizardItem.ButtonName = Global.GetLang("使用");
				systemWizardItem.Desc = Global.GetLang("获得新物品");
			}
			else if (type == SystemWizardTypes.HintMUNewChengJiu)
			{
				systemWizardItem.Desc = ColorCode.EncodingText(Global.GetLang("获得成就"), "ff9d08");
				systemWizardItem.Name = ColorCode.EncodingText(this.NewChengJiuName, "ff9d08");
				systemWizardItem._Button.gameObject.SetActive(false);
				systemWizardItem.GetComponent<Collider>().enabled = false;
			}
			else if (type == SystemWizardTypes.HintMUNewHuoYue)
			{
				systemWizardItem.Desc = ColorCode.EncodingText(Global.GetLang("完成活跃"), "ff9d08");
				systemWizardItem.Name = ColorCode.EncodingText(this.NewChengJiuName, "ff9d08");
				systemWizardItem._Button.gameObject.SetActive(false);
				systemWizardItem.GetComponent<Collider>().enabled = false;
			}
			systemWizardItem.InitPartData(goodsData);
		}
		this._ListBox.ItemsSource.DelayUpdate();
		this.ResetListBoxPosition(this._ListBox.ItemsSource.Count);
		return systemWizardItem;
	}

	private void ListBox_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		SystemWizardItem systemWizardItem = U3DUtils.AS<SystemWizardItem>(this._ListBox.SelectedItem);
		if (null != systemWizardItem)
		{
			this.RemoveSystemWizardItem(systemWizardItem);
		}
	}

	public void RemoveSystemWizardItem(SystemWizardItem item)
	{
		if (null != item)
		{
			DPSelectedItemEventArgs args = item.args;
			if (args.IDType == 0)
			{
				if (item.args.ID == 56 && item.MyGoodsData.GCount > 1)
				{
					this.ShowGoodsTip(item);
				}
				else
				{
					this.IconMouseLeftButtonUp(item.args, null);
				}
			}
			else if (args.IDType == 1)
			{
				int num = 0;
				GoodsData myGoodsData = item.MyGoodsData;
				if (myGoodsData != null && Super.CanHintEquipGoods(myGoodsData.Id, myGoodsData.GoodsID, ref num))
				{
					int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("AutoEquipLevel", '|');
					if (systemParamIntArrayByName.Length >= 2)
					{
						int num2 = systemParamIntArrayByName[0] * 100 + systemParamIntArrayByName[1];
						if (Global.Data != null && Global.Data.roleData != null)
						{
							int num3 = Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level;
							if (num3 <= num2)
							{
								this.IconMouseLeftButtonUp(item.args, null);
							}
						}
					}
				}
			}
			this._ListBox.ItemsSource.Remove(item.gameObject);
			this._ListBox.ItemsSource.DelayUpdate();
			this.ResetListBoxPosition(this._ListBox.ItemsSource.Count);
		}
	}

	public void DPSelectItemHandler(object sender, DPSelectedItemEventArgs args)
	{
		SystemWizardItem item = sender as SystemWizardItem;
		this.RemoveSystemWizardItem(item);
	}

	private void ShowGoodsTip(SystemWizardItem item)
	{
		if (item.MyGoodsData != null)
		{
			(Super.GData.PlayZoneRoot as PlayZone).ToShowUI(item.MyGoodsData.Id);
		}
	}

	public UISprite _Bak;

	public ListBox _ListBox;

	public string NewChengJiuName;

	private List<GameObject> mChildren = new List<GameObject>();

	private float EquipTipsTime = 15f;

	private float OtherTipsTime = 5f;

	private float HuoYuetipsTime = 3f;

	private float ChengJiuTipsTime = 3f;

	private float AlphaTime = 1f;

	private TaskVO taskVOItem;

	private StackPanel ItemPanel;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _TaskID;

	private int _GoodsDbID;

	public int ZhanLiUp;

	private int _ToByGoodsID;

	private int _ToByGoodsFromType;

	private List<int> _NewEquipGoodsDbIDList;

	private int _SkillID;

	private int _YaBiaoID;

	private int _YaBiaoLineID;

	private int _YaBiaoTouBao;

	private int _YaBiaoFailType;

	private int _WizardType;

	private string _ReloadXmlMsg;

	private int _SuiTangBattleResultExpAward;

	private int _SuiTangBattleResultBindYuanBaoAward;

	private int _SuiTangBattleResultChengJiuAward;

	private bool _AllInstantAutoFindRoad = true;

	private static ImageBrush _BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));

	private static ImageBrush _NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));

	private static ImageBrush _DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));

	private static SystemWizardPartMU _Instance = null;

	public MyUIAnchor _MyUIAnchor;

	private int MyUIAnchorPosY = 62;
}
