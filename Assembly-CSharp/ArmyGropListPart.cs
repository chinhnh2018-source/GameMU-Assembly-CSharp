using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ArmyGropListPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTexture();
		this.InitHandler();
		NGUITools.SetActive(this.lblSpeechVoiceMembers.gameObject, false);
	}

	private void InitPrefabText()
	{
		this.PopupList.selection = string.Empty;
		this.JingYingGuanLiBtn.Label.lineWidth = 115;
	}

	private void InitTexture()
	{
		this._Bg.URL = "NetImages/GameRes/Images/ArmyGroup/ArmyGroupCreatBg1.jpg";
		if (this._BowmBtns.Length > 2)
		{
			this._BowmBtns[0].Text = Global.GetLang("创建军团");
			this._BowmBtns[1].Text = Global.GetLang("军团信息");
			this._BowmBtns[2].Text = Global.GetLang("申请加入");
		}
	}

	private void InitHandler()
	{
		if (null != this._CloseBtn)
		{
			this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mType == ArmyGropListPart.ArmyGroupListType.BHJingYing)
				{
					if (this.mArmyGroupJingYingLisGuanLiOf1MiniPart != null)
					{
						this.mArmyGroupJingYingLisGuanLiOf1MiniPart = null;
						this.SetUIType(ArmyGropListPart.ArmyGroupListType.BHJingYing);
					}
					else if (this.mArmyGroupJingYingLisGuanLitMiniPart != null)
					{
						this.mArmyGroupJingYingLisGuanLitMiniPart = null;
						this.SetUIType(ArmyGropListPart.ArmyGroupListType.ArmyGroupJingYing);
					}
					else if (this.Hander != null)
					{
						this.Hander(e, new DPSelectedItemEventArgs
						{
							Index = 0
						});
					}
				}
				else if (this.Hander != null)
				{
					this.Hander(e, new DPSelectedItemEventArgs
					{
						Index = 0
					});
				}
			};
		}
	}

	private void ShowJingYingGuanLiPiLiangShengJi()
	{
		if (this.mArmyGroupJingYingLisGuanLiOf1MiniPart != null)
		{
			this.mArmyGroupJingYingLisGuanLiOf1MiniPart = null;
		}
		this.mArmyGroupJingYingLisGuanLiOf1MiniPart = new ArmyGropListPart.ArmyGroupJingYingLisGuanLiOf1MiniPart(this.ArmyGroupTitleInf, this._ListBox, this._UIDraggablePanel, this._JingYIngBtns, this._TitleSP, this.PopupList, this.JingYingInf);
		this.mArmyGroupJingYingLisGuanLiOf1MiniPart.SetUIType(ArmyGropListItem.ItemUIType.JingYingGUanLi3);
		this.mArmyGroupJingYingLisGuanLiOf1MiniPart.mCBHander = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (s.Index == 44)
			{
				if (this.Hander != null)
				{
					this.Hander(e, s);
				}
			}
			else
			{
				this.RefreshArmyGroupJingYingUi(this.mJingYingDataLst, (int)((byte)s.Index));
			}
		};
	}

	private void ShowJingYingGuanLiPiLiangJiangJi()
	{
		if (this.mArmyGroupJingYingLisGuanLiOf1MiniPart != null)
		{
			this.mArmyGroupJingYingLisGuanLiOf1MiniPart = null;
		}
		this.mArmyGroupJingYingLisGuanLiOf1MiniPart = new ArmyGropListPart.ArmyGroupJingYingLisGuanLiOf1MiniPart(this.ArmyGroupTitleInf, this._ListBox, this._UIDraggablePanel, this._JingYIngBtns, this._TitleSP, this.PopupList, this.JingYingInf);
		this.mArmyGroupJingYingLisGuanLiOf1MiniPart.SetUIType(ArmyGropListItem.ItemUIType.JingYingGUanLi4);
		this.mArmyGroupJingYingLisGuanLiOf1MiniPart.mCBHander = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (s.Index == 44)
			{
				if (this.Hander != null)
				{
					this.Hander(e, s);
				}
			}
			else
			{
				this.RefreshArmyGroupJingYingUi(this.mJingYingDataLst, (int)((byte)s.Index));
			}
		};
	}

	public void SetUIType(ArmyGropListPart.ArmyGroupListType type)
	{
		this.mType = type;
		NGUITools.SetActive(this._Root[0], false);
		NGUITools.SetActive(this._Root[1], false);
		NGUITools.SetActive(this._Root[2], false);
		NGUITools.SetActive(this._Root[3], false);
		NGUITools.SetActive(this.lblSpeechVoiceMembers.gameObject, false);
		switch (type)
		{
		case ArmyGropListPart.ArmyGroupListType.ArmyGroupList:
			NGUITools.SetActive(this._Root[0], true);
			if (this.mArmyGroupListMiniPart != null)
			{
				this.mArmyGroupListMiniPart = null;
			}
			this.mArmyGroupListMiniPart = new ArmyGropListPart.ArmyGroupListMiniPart(this.ArmyGroupTitleInf, this._ListBox, this._UIDraggablePanel, this._BowmBtns, this._TitleSP);
			this.mArmyGroupListMiniPart.MType = 0;
			this.mArmyGroupListMiniPart.CBHander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s.Index == 0 && this.Hander != null)
				{
					this.Hander(e, s);
				}
			};
			break;
		case ArmyGropListPart.ArmyGroupListType.ArmyGroupJingYing:
			NGUITools.SetActive(this.lblSpeechVoiceMembers.gameObject, false);
			NGUITools.SetActive(this._Root[3], true);
			if (this.mArmyGroupJingYingListMiniPart != null)
			{
				this.mArmyGroupJingYingListMiniPart = null;
			}
			this.mArmyGroupJingYingListMiniPart = new ArmyGropListPart.ArmyGroupJingYingListMiniPart(this.ArmyGroupTitleInf, this._ListBox, this._UIDraggablePanel, this._TitleSP, this.JingYingInf, this.JingYingGuanLiBtn, this.lblSpeechVoiceMembers, null);
			this.mArmyGroupJingYingListMiniPart.SetUIType(ArmyGropListItem.ItemUIType.JingYIngDian);
			this.mArmyGroupJingYingListMiniPart.mCBHander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s.ID == 14)
				{
					this.mArmyGroupJingYingListMiniPart = null;
					this.SetUIType(ArmyGropListPart.ArmyGroupListType.BHJingYing);
				}
				else if (s.Index == 44 && this.Hander != null)
				{
					this.Hander(e, s);
				}
			};
			break;
		case ArmyGropListPart.ArmyGroupListType.BHJingYing:
			NGUITools.SetActive(this._Root[1], true);
			if (this.mArmyGroupJingYingLisGuanLitMiniPart != null)
			{
				this.mArmyGroupJingYingLisGuanLitMiniPart = null;
			}
			this.mArmyGroupJingYingLisGuanLiOf1MiniPart = null;
			this.mArmyGroupJingYingLisGuanLitMiniPart = new ArmyGropListPart.ArmyGroupJingYingLisGuanLitMiniPart(this.ArmyGroupTitleInf, this._ListBox, this._UIDraggablePanel, this._JingYIngBtns, this._TitleSP, this.PopupList, this.JingYingInf);
			this.mArmyGroupJingYingLisGuanLitMiniPart.SetUIType(ArmyGropListItem.ItemUIType.JingYingGUanLi2);
			this.mArmyGroupJingYingLisGuanLitMiniPart.mCBHander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s != null)
				{
					if (s.Index == 22)
					{
						this.ShowJingYingGuanLiPiLiangShengJi();
						this.mArmyGroupJingYingLisGuanLitMiniPart = null;
					}
					else if (s.Index == 23)
					{
						this.ShowJingYingGuanLiPiLiangJiangJi();
						this.mArmyGroupJingYingLisGuanLitMiniPart = null;
					}
					else if (s.Index == 44)
					{
						if (this.Hander != null)
						{
							this.Hander(e, s);
						}
					}
					else
					{
						this.RefreshArmyGroupJingYingUi(this.mJingYingDataLst, (int)((byte)s.Index));
					}
				}
			};
			break;
		case ArmyGropListPart.ArmyGroupListType.ShenQingLst:
			NGUITools.SetActive(this._Root[2], true);
			if (this.mArmyGroupShenQingListMiniPart != null)
			{
				this.mArmyGroupShenQingListMiniPart = null;
			}
			this.mArmyGroupShenQingListMiniPart = new ArmyGropListPart.ArmyGroupShenQingListMiniPart(this.ArmyGroupTitleInf, this._ListBox, this._UIDraggablePanel, this._ShengQingBtns, this._TitleSP);
			this.mArmyGroupShenQingListMiniPart.CBHander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s != null && 0 < s.EquipIDs.Length)
				{
					base.StartCoroutine<bool>(this.mArmyGroupShenQingListMiniPart.SendTurnDown(s.EquipIDs, (byte)s.Type));
				}
			};
			break;
		case ArmyGropListPart.ArmyGroupListType.AllArmyGroupList:
			NGUITools.SetActive(this._Root[0], true);
			if (this.mArmyGroupListMiniPart != null)
			{
				this.mArmyGroupListMiniPart = null;
			}
			this.mArmyGroupListMiniPart = new ArmyGropListPart.ArmyGroupListMiniPart(this.ArmyGroupTitleInf, this._ListBox, this._UIDraggablePanel, this._BowmBtns, this._TitleSP);
			this.mArmyGroupListMiniPart.MType = 1;
			this.mArmyGroupListMiniPart.CBHander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s.Index == 0 && this.Hander != null)
				{
					this.Hander(e, s);
				}
			};
			break;
		}
		this.InitPrefabText();
	}

	public void RefreshArmyJionUI(int ret)
	{
		if (0 <= ret)
		{
			Super.HintMainText(Global.GetLang("您已申请加入军团,请耐心等待审批！"), 10, 3);
			this.mArmyGroupListMiniPart.RefreshShengQingTime();
			this.Hander(null, new DPSelectedItemEventArgs
			{
				Index = 10,
				ID = 12
			});
		}
		else if (ret == -1008)
		{
			Super.HintMainText(string.Format(Global.GetLang("只有{0}级战盟盟主才能创建军团"), ConfigSystemParam.GetSystemParamIntByName("LegionsNeed")), 10, 3);
		}
		else
		{
			ArmyGroupPart.ErrorLog(ret);
		}
	}

	public void RespondShengQing(int ret)
	{
		if (0 <= ret)
		{
			if (this.mArmyGroupShenQingListMiniPart != null)
			{
				this.mArmyGroupShenQingListMiniPart.ShengQingCallBack(ret);
			}
		}
		else
		{
			ArmyGroupPart.ErrorLog(ret);
		}
	}

	public void RefresArmyGroupListUI(List<JunTuanMiniData> lst)
	{
		if (0 < lst.Count && this.mArmyGroupListMiniPart != null)
		{
			base.StopCoroutine("RefreshArmyGroupList");
			base.StartCoroutine<bool>(this.mArmyGroupListMiniPart.RefreshArmyGroupList(lst));
		}
	}

	public void RefreshArmyGroupJingYingUi(List<JunTuanRoleData> lst, int rankIndex = 0)
	{
		if (lst != null && 0 < lst.Count)
		{
			this.mJingYingDataLst = lst;
			if (this.mJingYingDataLst != null)
			{
				if (this.mArmyGroupJingYingListMiniPart != null)
				{
					base.StopCoroutine("RefreshArmyGroupJingYingList");
					base.StartCoroutine<bool>(this.mArmyGroupJingYingListMiniPart.RefreshArmyGroupJingYingList(this.mJingYingDataLst, 0, this.CurrentSpeeches));
					List<JunTuanRoleData> list = this.mJingYingDataLst.FindAll((JunTuanRoleData e) => 3 == e.JuTuanZhiWu);
					this.mArmyGroupJingYingListMiniPart.JingYingCount = ((list != null) ? list.Count : 0);
				}
				if (this.mArmyGroupJingYingLisGuanLitMiniPart != null)
				{
					base.StopCoroutine("RefreshArmyGroupJingYingList");
					base.StartCoroutine<bool>(this.mArmyGroupJingYingLisGuanLitMiniPart.RefreshArmyGroupJingYingList(this.mJingYingDataLst, 0));
					List<JunTuanRoleData> list2 = this.mJingYingDataLst.FindAll((JunTuanRoleData e) => e.JuTuanZhiWu == 3 && Global.Data.roleData.Faction == e.BhId);
					this.mArmyGroupJingYingLisGuanLitMiniPart.JingYingCount = ((list2 != null) ? list2.Count : 0);
				}
				if (this.mArmyGroupJingYingLisGuanLiOf1MiniPart != null)
				{
					base.StopCoroutine("RefreshArmyGroupJingYingList");
					base.StartCoroutine<bool>(this.mArmyGroupJingYingLisGuanLiOf1MiniPart.RefreshArmyGroupJingYingList(this.mJingYingDataLst, 0));
					List<JunTuanRoleData> list3 = this.mJingYingDataLst.FindAll((JunTuanRoleData e) => e.JuTuanZhiWu == 3 && Global.Data.roleData.Faction == e.BhId);
					this.mArmyGroupJingYingLisGuanLiOf1MiniPart.JingYingCount = ((list3 != null) ? list3.Count : 0);
				}
			}
			else
			{
				if (this.mArmyGroupJingYingListMiniPart != null)
				{
					this.mArmyGroupJingYingListMiniPart.ClearJIngYingList();
				}
				if (this.mArmyGroupJingYingLisGuanLitMiniPart != null)
				{
					this.mArmyGroupJingYingLisGuanLitMiniPart.ClearJIngYingList();
				}
				if (this.mArmyGroupJingYingLisGuanLiOf1MiniPart != null)
				{
					this.mArmyGroupJingYingLisGuanLiOf1MiniPart.ClearJIngYingList();
				}
			}
		}
	}

	public void RefreshArmyGroupShenQingLSt(List<JunTuanRequestData> lst)
	{
		if (lst == null)
		{
			lst = new List<JunTuanRequestData>();
		}
		base.StartCoroutine<bool>(this.mArmyGroupShenQingListMiniPart.RefreshArmyGroupShenQingList(lst));
	}

	public void SortAymyMembersByVoicePriority(GVoicePriorityData data)
	{
		GameInstance.Game.SendGetArmyGroupJingYingDataLst();
		if (data == null)
		{
			return;
		}
		if (data.Type == 1)
		{
			return;
		}
		string roleIdList = data.RoleIdList;
		this.RefreshJingYingSpeechers(roleIdList);
	}

	public void RefreshJingYingSpeechers(string speechers)
	{
		if (this.mArmyGroupJingYingListMiniPart != null)
		{
			this.CurrentSpeeches = speechers;
		}
	}

	public UISprite _TitleSP;

	public GameObject ArmyGroupTitleInf;

	public GButton _CloseBtn;

	public ListBox _ListBox;

	public UIDraggablePanel _UIDraggablePanel;

	public ShowNetImage _Bg;

	public GameObject[] _Root;

	public GButton[] _BowmBtns;

	public GButton[] _JingYIngBtns;

	public UIPopupList PopupList;

	public TextBlock[] JingYingInf;

	public GButton JingYingGuanLiBtn;

	public TextBlock lblSpeechVoiceMembers;

	public GButton[] _ShengQingBtns;

	private ArmyGropListPart.ArmyGroupListMiniPart mArmyGroupListMiniPart;

	private ArmyGropListPart.ArmyGroupJingYingListMiniPart mArmyGroupJingYingListMiniPart;

	private ArmyGropListPart.ArmyGroupJingYingLisGuanLitMiniPart mArmyGroupJingYingLisGuanLitMiniPart;

	private ArmyGropListPart.ArmyGroupShenQingListMiniPart mArmyGroupShenQingListMiniPart;

	private ArmyGropListPart.ArmyGroupJingYingLisGuanLiOf1MiniPart mArmyGroupJingYingLisGuanLiOf1MiniPart;

	private List<JunTuanRoleData> mJingYingDataLst;

	private ArmyGropListPart.ArmyGroupListType mType;

	private string CurrentSpeeches = string.Empty;

	public DPSelectedItemEventHandler Hander;

	public class ArmyGroupListMiniPart
	{
		public ArmyGroupListMiniPart(GameObject title, ListBox Listbox, UIDraggablePanel DraggablePanel, GButton[] BowmBtns, UISprite TitleSp)
		{
			for (int i = 1; i < title.transform.parent.childCount; i++)
			{
				GameObject gameObject = title.transform.parent.GetChild(i).gameObject;
				if (gameObject.name != "name")
				{
					Object.Destroy(gameObject);
				}
			}
			this._BowmBtns = BowmBtns;
			this._DraggablePanel = DraggablePanel;
			this._Listbox = Listbox;
			TitleSp.spriteName = "ArnyGroupList";
			Vector3[] array = new Vector3[]
			{
				new Vector3(260f, 44f, 1f),
				new Vector3(273f, 44f, 1f),
				new Vector3(204f, 44f, 1f),
				new Vector3(212f, 44f, 1f)
			};
			Vector3[] array2 = new Vector3[]
			{
				new Vector3(-339f, 0f, 0f),
				new Vector3(-77f, 0f, 0f),
				new Vector3(159f, 0f, 0f),
				new Vector3(363f, 0f, 0f)
			};
			string[] array3 = new string[]
			{
				Global.GetLang("军团名称"),
				Global.GetLang("军团首领"),
				Global.GetLang("战盟数"),
				Global.GetLang("军团领地")
			};
			byte b = 0;
			while ((int)b < array3.Length)
			{
				GameObject gameObject2;
				if (b == 0)
				{
					gameObject2 = title;
				}
				else
				{
					gameObject2 = Object.Instantiate<GameObject>(title);
					this.lst.Add(gameObject2);
				}
				UILabel component = gameObject2.transform.FindChild("label").GetComponent<UILabel>();
				Transform transform = gameObject2.transform.FindChild("Sprite");
				gameObject2.transform.SetParent(title.transform.parent, false);
				gameObject2.transform.localPosition = array2[(int)b];
				component.pivot = 1;
				component.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36d",
					array3[(int)b]
				});
				transform.transform.localScale = array[(int)b];
				b += 1;
			}
			this.InitMiniHander();
			string text = string.Format("{0}{1}", Global.Data.roleData.Faction, "LastUseShengQingTime");
			if (PlayerPrefs.HasKey(text))
			{
				string @string = PlayerPrefs.GetString(text);
				if (!string.IsNullOrEmpty(@string))
				{
					this.mLastUseShengQingTime = Convert.ToInt64(@string);
				}
			}
		}

		private void InitMiniHander()
		{
			this.m_OBC = this._Listbox.ItemsSource;
			this._Listbox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ListBoxSelectChange);
			NGUITools.SetActive(this._BowmBtns[0].gameObject, true);
			NGUITools.SetActive(this._BowmBtns[1].gameObject, true);
			NGUITools.SetActive(this._BowmBtns[2].gameObject, true);
			this._BowmBtns[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.CBHander != null)
				{
					this.CBHander(e, new DPSelectedItemEventArgs
					{
						Index = 0,
						Flag = 1
					});
				}
			};
			this._BowmBtns[2].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				long num = (long)((int)ConfigSystemParam.GetSystemParamIntByName("LegionsJionCD") * 60);
				long num2 = 0L;
				if (this.mLastUseShengQingTime != 0L)
				{
					num2 = num - (long)((int)((Global.GetCorrectLocalTime() - this.mLastUseShengQingTime) / 1000L));
				}
				if (num2 > 0L)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("{0}分钟之后才能申请加入军团"), new object[]
					{
						(int)num2 / 60 + 1
					}), 10, 3);
					return;
				}
				if (null != this._Listbox.SelectedItem)
				{
					ArmyGropListItem component = this._Listbox.SelectedItem.GetComponent<ArmyGropListItem>();
					if (null != component && this.RoleCanCreatArmyGroup(true))
					{
						Super.ShowNetWaiting(null);
						GameInstance.Game.SendJionGroup(component.ID);
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("请选择您要加入的军团"), 10, 3);
				}
			};
			this._BowmBtns[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (null != this._Listbox.SelectedItem)
				{
					ArmyGropListItem component = this._Listbox.SelectedItem.GetComponent<ArmyGropListItem>();
					if (null != component)
					{
						PlayZone.GlobalPlayZone.ShowArmyGroupRenwuPart(2, 0, 0, component.ID);
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("请选择您要查看的军团"), 10, 3);
				}
			};
		}

		public void RefreshShengQingTime()
		{
			this.mLastUseShengQingTime = Global.GetCorrectLocalTime();
			string text = string.Format("{0}{1}", Global.Data.roleData.Faction, "LastUseShengQingTime");
			PlayerPrefs.SetString(text, this.mLastUseShengQingTime.ToString());
		}

		private bool RoleCanCreatArmyGroup(bool HaveErrHint = false)
		{
			if (!Global.IsHavingBangHui())
			{
				if (HaveErrHint)
				{
					Super.HintMainText(Global.GetLang("请创建或加入一个战盟"), 10, 3);
				}
				return false;
			}
			int num = (int)ConfigSystemParam.GetSystemParamIntByName("LegionsNeed");
			if (!Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
			{
				if (HaveErrHint)
				{
					Super.HintMainText(string.Format(Global.GetLang("只有{0}级战盟盟主才能申请加入军团"), num), 10, 3);
				}
				return false;
			}
			if (Global.RoleHaveArmyGroup())
			{
				if (HaveErrHint)
				{
					Super.HintMainText(Global.GetLang("您已加入军团，无法再申请"), 10, 3);
				}
				return false;
			}
			if (!GongnengYugaoMgr.IsIconOpened(69))
			{
				if (HaveErrHint)
				{
					Super.HintMainText(Global.GetLang("功能未开启"), 10, 3);
				}
				return false;
			}
			return true;
		}

		private void InitArmyGroupListData()
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendGetArmyGroupListData();
		}

		private string GetLingDiName(int index)
		{
			string result = string.Empty;
			switch (index)
			{
			case 0:
				result = Global.GetLang("无");
				break;
			case 1:
				result = Global.GetLang("阿卡伦西");
				break;
			case 2:
				result = Global.GetLang("阿卡伦东");
				break;
			case 3:
				result = Global.GetLang("全境");
				break;
			}
			return result;
		}

		public IEnumerator RefreshArmyGroupList(List<JunTuanMiniData> list)
		{
			if (list != null && 0 < list.Count)
			{
				Super.ShowNetWaiting(null);
				List<JunTuanMiniData> HaveLingdiLst = list.FindAll((JunTuanMiniData e) => 0 != e.LingDi);
				List<JunTuanMiniData> NotHaveLingdiLst = list.FindAll((JunTuanMiniData e) => 0 == e.LingDi);
				List<JunTuanMiniData> SortLst = new List<JunTuanMiniData>();
				if (HaveLingdiLst != null && 0 < HaveLingdiLst.Count)
				{
					HaveLingdiLst.Sort((JunTuanMiniData s, JunTuanMiniData e) => e.BangHuiNum - s.BangHuiNum);
					SortLst.AddRange(HaveLingdiLst);
				}
				if (NotHaveLingdiLst != null && 0 < NotHaveLingdiLst.Count)
				{
					NotHaveLingdiLst.Sort((JunTuanMiniData s, JunTuanMiniData e) => e.BangHuiNum - s.BangHuiNum);
					SortLst.AddRange(NotHaveLingdiLst);
				}
				Vector3 pos = this._Listbox.transform.localPosition;
				pos.x = 9f;
				if (7 > SortLst.Count)
				{
					pos.y = 120f;
				}
				this._Listbox.transform.localPosition = pos;
				yield return null;
				for (int i = 0; i < SortLst.Count; i++)
				{
					if (SortLst[i] != null)
					{
						ArmyGropListItem item = this.GetItem(this.m_OBC, i);
						if (null == item)
						{
							item = U3DUtils.NEW<ArmyGropListItem>();
						}
						string[] content = new string[]
						{
							Global.GetColorStringForNGUIText(new object[]
							{
								"ff6600",
								Global.FormatRoleNameZoneid(SortLst[i].LeaderZoneId, SortLst[i].JunTuanName, 1, 1)
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"17e43e",
								Global.FormatRoleNameZoneid(SortLst[i].LeaderZoneId, SortLst[i].LeaderName, 1, 1)
							}),
							SortLst[i].BangHuiNum.ToString() + "/4",
							this.GetLingDiName(SortLst[i].LingDi)
						};
						item.Index = i;
						item.ID = SortLst[i].JunTuanId;
						if (this.mType != 0)
						{
							item.SetContent(content, ArmyGropListItem.ItemUIType.AllArmyGroupList);
						}
						else
						{
							item.SetContent(content, ArmyGropListItem.ItemUIType.ArmyGroupList);
						}
						this.m_OBC.AddNoUpdate(item);
						item.DraggablePanel = this._DraggablePanel;
					}
					if (i % 6 == 0 && i != 0)
					{
						yield return null;
					}
				}
				Super.HideNetWaiting();
			}
			yield break;
		}

		private ArmyGropListItem GetItem(ObservableCollection OBC, int index)
		{
			if (OBC != null && 0 < index && OBC.Count > index)
			{
				GameObject at = OBC.GetAt(index);
				if (null != at)
				{
					return at.GetComponent<ArmyGropListItem>();
				}
			}
			return null;
		}

		private void ListBoxSelectChange(object sender, MouseEvent e)
		{
			GameObject selectedItem = this._Listbox.SelectedItem;
			if (null != selectedItem)
			{
				ArmyGropListItem component = selectedItem.GetComponent<ArmyGropListItem>();
				if (null != component)
				{
					component.BSelect = true;
				}
			}
			GameObject lastSelectedItem = this._Listbox.LastSelectedItem;
			if (null != lastSelectedItem && lastSelectedItem != selectedItem)
			{
				ArmyGropListItem component2 = lastSelectedItem.GetComponent<ArmyGropListItem>();
				if (null != component2)
				{
					component2.BSelect = false;
				}
			}
		}

		public byte MType
		{
			set
			{
				this.mType = value;
				if (value == 0)
				{
					this._BowmBtns[0].transform.parent.gameObject.SetActive(true);
				}
				else
				{
					this._BowmBtns[0].transform.parent.gameObject.SetActive(false);
				}
				this.InitArmyGroupListData();
			}
		}

		private ListBox _Listbox;

		private UIDraggablePanel _DraggablePanel;

		private GButton[] _BowmBtns;

		private ObservableCollection m_OBC;

		private long mLastUseShengQingTime;

		private List<GameObject> lst = new List<GameObject>();

		private byte mType;

		public DPSelectedItemEventHandler CBHander;
	}

	public class ArmyGroupJingYingListMiniPart
	{
		public ArmyGroupJingYingListMiniPart(GameObject title, ListBox Listbox, UIDraggablePanel DraggablePanel, UISprite titlesp, TextBlock[] InfText, GButton JIngYingGuanli, TextBlock tmpSpeechVoiceMembers, string CurrentSpeeches = null)
		{
			for (int i = 1; i < title.transform.parent.childCount; i++)
			{
				GameObject gameObject = title.transform.parent.GetChild(i).gameObject;
				if (gameObject.name != "name")
				{
					Object.Destroy(gameObject);
				}
			}
			this.mDraggablePanel = DraggablePanel;
			this.mListbox = Listbox;
			this.mInfText = InfText;
			this.mJIngYingGuanli = JIngYingGuanli;
			titlesp.spriteName = "ArmyGroupJIngYing";
			this.speechVoiceMembers = tmpSpeechVoiceMembers;
			this.speechVoiceMembers.Text = string.Empty;
			this.InitVoiceMembers();
			string[] array = new string[]
			{
				Global.GetLang("成员名称"),
				Global.GetLang("战盟"),
				Global.GetLang("个人战斗力"),
				Global.GetLang("职务")
			};
			Vector3[] array2 = new Vector3[]
			{
				new Vector3(254f, 44f, 1f),
				new Vector3(274f, 44f, 1f),
				new Vector3(205f, 44f, 1f),
				new Vector3(213f, 44f, 1f)
			};
			Vector3[] array3 = new Vector3[]
			{
				new Vector3(-343f, 0f, 0f),
				new Vector3(-83f, 0f, 0f),
				new Vector3(155f, 0f, 0f),
				new Vector3(363f, 0f, 0f)
			};
			byte b = 0;
			while ((int)b < array.Length)
			{
				GameObject gameObject2;
				if (b == 0)
				{
					gameObject2 = title;
				}
				else
				{
					gameObject2 = Object.Instantiate<GameObject>(title);
				}
				UILabel component = gameObject2.transform.FindChild("label").GetComponent<UILabel>();
				component.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36d",
					array[(int)b]
				});
				Transform transform = gameObject2.transform.FindChild("Sprite");
				gameObject2.transform.SetParent(title.transform.parent, false);
				gameObject2.transform.localPosition = array3[(int)b];
				transform.localScale = array2[(int)b];
				b += 1;
			}
			this.mMaxJingYingNum = ConfigSystemParam.GetSystemParamByName("LegionsEliteNum", true).SafeToInt32(0);
			this.JingYingCount = 0;
			this.InitMiniHander();
			this.InitArmyGroupJingYingData();
		}

		private void InitVoiceMembers()
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("VoicePowerNum", ',');
			if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length >= 2)
			{
				this.SumCanSpeechMembers = systemParamIntArrayByName[1];
			}
		}

		public void ClearJIngYingList()
		{
			if (this.mOBC != null && 0 < this.mOBC.Count)
			{
				this.mOBC.Clear();
			}
		}

		public int JingYingCount
		{
			set
			{
				this.mInfText[0].Text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("军团精英人数：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					string.Format("{0}/{1}", value, this.mMaxJingYingNum * 4)
				});
			}
		}

		public IEnumerator RefreshArmyGroupJingYingList(List<JunTuanRoleData> list, int jingYingCount = 0, string speechers = null)
		{
			if (list != null)
			{
				Super.ShowNetWaiting(null);
				list = list.FindAll((JunTuanRoleData e) => 4 != ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu));
				List<JunTuanRoleData> SortLst = new List<JunTuanRoleData>();
				List<JunTuanRoleData> JingYingLst = list.FindAll((JunTuanRoleData e) => ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu) == 1 || 2 == ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu));
				if (0 < JingYingLst.Count)
				{
					JingYingLst.Sort((JunTuanRoleData s, JunTuanRoleData e) => s.JuTuanZhiWu - e.JuTuanZhiWu);
					SortLst.AddRange(JingYingLst);
				}
				List<JunTuanRoleData> JingYingLst2 = list.FindAll((JunTuanRoleData e) => e.BhId == Global.Data.roleData.Faction && (ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu) == 4 || 3 == ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu)));
				if (0 < JingYingLst2.Count)
				{
					JingYingLst2.Sort(delegate(JunTuanRoleData s, JunTuanRoleData e)
					{
						if (e.JuTuanZhiWu != s.JuTuanZhiWu)
						{
							return ArmyGroupPart.GetZhiWu(s.JuTuanZhiWu) - ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu);
						}
						if (e.BhId == s.BhId)
						{
							return 0;
						}
						return e.ZhanLi - s.ZhanLi;
					});
					SortLst.AddRange(JingYingLst2);
				}
				List<JunTuanRoleData> JingYingLst3 = list.FindAll((JunTuanRoleData e) => e.BhId != Global.Data.roleData.Faction && (ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu) == 4 || 3 == ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu)));
				if (0 < JingYingLst3.Count)
				{
					JingYingLst3.Sort(delegate(JunTuanRoleData s, JunTuanRoleData e)
					{
						if (e.JuTuanZhiWu != s.JuTuanZhiWu)
						{
							return ArmyGroupPart.GetZhiWu(s.JuTuanZhiWu) - ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu);
						}
						if (e.BhId == s.BhId)
						{
							return 0;
						}
						return e.ZhanLi - s.ZhanLi;
					});
					SortLst.AddRange(JingYingLst3);
				}
				Vector3 pos = this.mListbox.transform.localPosition;
				pos.x = 9f;
				if (7 > SortLst.Count)
				{
					pos.y = 120f;
				}
				this.mListbox.transform.localPosition = pos;
				yield return null;
				if (!string.IsNullOrEmpty(speechers))
				{
					string[] roleIdList = speechers.Split(new char[]
					{
						','
					});
					List<JunTuanRoleData> existList = new List<JunTuanRoleData>();
					if (roleIdList.Length > 0)
					{
						for (int i = 0; i < roleIdList.Length; i++)
						{
							int roleId = Global.SafeConvertToInt32(roleIdList[i]);
							JunTuanRoleData tmp = SortLst.Find((JunTuanRoleData result) => result.RoleId == roleId);
							if (tmp != null)
							{
								existList.Add(tmp);
							}
						}
					}
					if (existList != null && existList.Count > 0)
					{
						for (int j = 0; j < existList.Count; j++)
						{
							int index = SortLst.FindIndex((JunTuanRoleData result) => result.RoleId == existList[j].RoleId);
							if (index > 0)
							{
								SortLst.RemoveAt(index);
							}
						}
						for (int k = 0; k < existList.Count; k++)
						{
							SortLst.Insert(k + 1, existList[k]);
						}
					}
				}
				List<JunTuanRoleData> filterResult = null;
				filterResult = SortLst.FindAll((JunTuanRoleData result) => result.JuTuanZhiWu == 1);
				if (filterResult.Count >= 2)
				{
					SortLst.Remove(filterResult[0]);
				}
				byte Add = 0;
				for (int l = 0; l < SortLst.Count; l++)
				{
					if (SortLst[l] != null)
					{
						GameObject Itemobj = this.mOBC.GetAt(l);
						ArmyGropListItem item = null;
						if (null != Itemobj)
						{
							item = Itemobj.GetComponent<ArmyGropListItem>();
						}
						if (null == item)
						{
							item = U3DUtils.NEW<ArmyGropListItem>();
							Add = 1;
						}
						string[] content = new string[]
						{
							Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								Global.FormatRoleNameZoneid(SortLst[l].ZoneId, SortLst[l].RoleName, 1, 1)
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"99ccff",
								Global.FormatRoleNameZoneid(SortLst[l].BhZoneId, SortLst[l].BhName, 1, 1)
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								SortLst[l].ZhanLi.ToString()
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								ConfigArmyGroupLegions.GetZhiWuNameByID(SortLst[l].JuTuanZhiWu)
							})
						};
						if (SortLst[l].OnlineState != 1 && content != null && 0 < content.Length)
						{
							byte m = 0;
							while ((int)m < content.Length)
							{
								content[(int)m] = Global.GetColorStringForNGUIText(new object[]
								{
									"808081",
									Super.ClearStringColor(content[(int)m])
								});
								m += 1;
							}
						}
						item.Index = l;
						item.ID = SortLst[l].RoleId;
						item.MBhId = SortLst[l].BhId;
						item.JunTuanZhiWu = SortLst[l].JuTuanZhiWu;
						item.BSelect = false;
						item.Clear();
						item.SetContent(content, this.mType);
						item.AddTexMenContent(Global.GetLang("查 看"));
						item.AddTexMenContent(Global.GetLang("私 聊"));
						if (ArmyGroupPart.GetZhiWu(item.JunTuanZhiWu) == 1)
						{
							item.Speech = 1;
						}
						else
						{
							item.Speech = 3;
							item.SetVoiceToFalse = false;
						}
						if (Global.Data == null || ArmyGroupPart.GetZhiWu(Global.Data.roleData.JunTuanZhiWu) == 1)
						{
						}
						item.MRoleName = SortLst[l].RoleName;
						item.Hander = delegate(object e, DPSelectedItemEventArgs s)
						{
							if (this.mCBHander != null)
							{
								this.mCBHander(e, s);
							}
						};
						if (Add == 1)
						{
							this.mOBC.AddNoUpdate(item);
							item.DraggablePanel = this.mDraggablePanel;
						}
						item.SpeechPriorityCallBack = delegate(int id, int type)
						{
							this.SpeechPermission(id, type);
						};
					}
					if (l == 10)
					{
						this.mDraggablePanel.Press(false);
					}
					if (l % 6 == 0)
					{
						yield return null;
					}
				}
				ArmyGroupPart.CheckListBoxChild(SortLst.Count, this.mOBC);
				yield return null;
				if (null != this.mDraggablePanel.verticalScrollBar)
				{
					this.mDraggablePanel.verticalScrollBar.scrollValue = 0f;
				}
				Super.HideNetWaiting();
				this.RefreshSpeechers(speechers);
			}
			yield break;
		}

		public void RefreshSpeechers(string speechers)
		{
			int num = 1;
			if (this.mOBC != null && this.mOBC.Count > 0)
			{
				string[] array = null;
				if (!string.IsNullOrEmpty(speechers))
				{
					array = speechers.Split(new char[]
					{
						','
					});
				}
				int count = this.mOBC.Count;
				for (int i = 0; i < count; i++)
				{
					ArmyGropListItem armyGropListItem = U3DUtils.AS<ArmyGropListItem>(this.mOBC[i]);
					if (ArmyGroupPart.GetZhiWu(armyGropListItem.JunTuanZhiWu) == 1)
					{
						armyGropListItem.Speech = 1;
					}
					else
					{
						armyGropListItem.Speech = 3;
						armyGropListItem.SetVoiceToFalse = false;
					}
				}
				for (int j = 0; j < count; j++)
				{
					ArmyGropListItem armyGropListItem2 = U3DUtils.AS<ArmyGropListItem>(this.mOBC[j]);
					if (ArmyGroupPart.GetZhiWu(armyGropListItem2.JunTuanZhiWu) == 1)
					{
						armyGropListItem2.Speech = 1;
					}
					if (array != null && array.Length > 0)
					{
						for (int k = 0; k < array.Length; k++)
						{
							ArmyGropListItem armyGropListItem3 = U3DUtils.AS<ArmyGropListItem>(this.mOBC[j]);
							if (ArmyGroupPart.GetZhiWu(armyGropListItem3.JunTuanZhiWu) != 1)
							{
								if (armyGropListItem3.ID == Global.SafeConvertToInt32(array[k]))
								{
									armyGropListItem3.Speech = 2;
									num++;
								}
							}
						}
					}
				}
			}
			this.speechVoiceMembers.Text = string.Empty;
		}

		private void SpeechPermission(int setedID, int type)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.mOBC != null && this.mOBC.Count > 0)
			{
				int count = this.mOBC.Count;
				for (int i = 0; i < count; i++)
				{
					ArmyGropListItem armyGropListItem = U3DUtils.AS<ArmyGropListItem>(this.mOBC[i]);
					if (ArmyGroupPart.GetZhiWu(armyGropListItem.JunTuanZhiWu) != 1)
					{
						if (type != 3 || armyGropListItem.ID != setedID)
						{
							if (armyGropListItem.Speech < 3)
							{
								stringBuilder.Append(armyGropListItem.ID);
								stringBuilder.Append(",");
							}
						}
					}
				}
			}
			if (type == 3)
			{
				stringBuilder.ToString().TrimEnd(new char[]
				{
					','
				});
			}
			else
			{
				stringBuilder.Append(setedID);
			}
			GameInstance.Game.SendSetRealTimePriority(2, stringBuilder.ToString());
		}

		private string GetSpeechPermissionText(int type)
		{
			return null;
		}

		private void InitArmyGroupJingYingData()
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendGetRealTimePriority(2);
		}

		private void InitMiniHander()
		{
			this.mOBC = this.mListbox.ItemsSource;
			this.mListbox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ListBoxSelectChange);
			if (Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
			{
				this.mJIngYingGuanli.Text = Global.GetLang("精英管理");
				this.mJIngYingGuanli.MouseLeftButtonUp = delegate(object e, MouseEvent s)
				{
					if (this.mCBHander != null)
					{
						this.mCBHander(null, new DPSelectedItemEventArgs
						{
							ID = 14
						});
					}
				};
			}
			else
			{
				NGUITools.SetActive(this.mJIngYingGuanli.gameObject, false);
			}
		}

		private void ListBoxSelectChange(object sender, MouseEvent e)
		{
			GameObject selectedItem = this.mListbox.SelectedItem;
			if (null != selectedItem)
			{
				ArmyGropListItem component = selectedItem.GetComponent<ArmyGropListItem>();
				if (null != component)
				{
					component.BSelect = true;
				}
			}
			GameObject lastSelectedItem = this.mListbox.LastSelectedItem;
			if (null != lastSelectedItem && lastSelectedItem != selectedItem)
			{
				ArmyGropListItem component2 = lastSelectedItem.GetComponent<ArmyGropListItem>();
				if (null != component2)
				{
					component2.BSelect = false;
				}
			}
		}

		public void SetUIType(ArmyGropListItem.ItemUIType Type)
		{
			this.mType = Type;
		}

		private ListBox mListbox;

		private TextBlock[] mInfText;

		private UIDraggablePanel mDraggablePanel;

		private ObservableCollection mOBC;

		public DPSelectedItemEventHandler mCBHander;

		private int mMaxJingYingNum;

		private GButton mJIngYingGuanli;

		private ArmyGropListItem.ItemUIType mType;

		private TextBlock speechVoiceMembers;

		private int SumCanSpeechMembers = 5;
	}

	public class ArmyGroupJingYingLisGuanLitMiniPart
	{
		public ArmyGroupJingYingLisGuanLitMiniPart(GameObject title, ListBox Listbox, UIDraggablePanel DraggablePanel, GButton[] JingYIngBtns, UISprite titlesp, UIPopupList popupList, TextBlock[] InfText)
		{
			for (int i = 1; i < title.transform.parent.childCount; i++)
			{
				GameObject gameObject = title.transform.parent.GetChild(i).gameObject;
				if (gameObject.name != "name")
				{
					Object.Destroy(gameObject);
				}
			}
			this.mDraggablePanel = DraggablePanel;
			this.mListbox = Listbox;
			this.mJingYIngBtns = JingYIngBtns;
			this.mPopupList = popupList;
			this.mInfText = InfText;
			titlesp.spriteName = "BhJingYing";
			string[] array = new string[]
			{
				Global.GetLang("成员名称"),
				Global.GetLang("职业"),
				Global.GetLang("等级"),
				Global.GetLang("个人战斗力"),
				Global.GetLang("职务")
			};
			Vector3[] array2 = new Vector3[]
			{
				new Vector3(-342f, 0f, -0.5f),
				new Vector3(-142f, 0f, -0.5f),
				new Vector3(0f, 0f, -0.5f),
				new Vector3(170f, 0f, -0.5f),
				new Vector3(369f, 0f, -0.5f)
			};
			Vector3[] array3 = new Vector3[]
			{
				new Vector3(256f, 44f, 1f),
				new Vector3(144f, 44f, 1f),
				new Vector3(144f, 44f, 1f),
				new Vector3(200f, 44f, 1f),
				new Vector3(202f, 44f, 1f)
			};
			byte b = 0;
			while ((int)b < array.Length)
			{
				GameObject gameObject2;
				if (b == 0)
				{
					gameObject2 = title;
				}
				else
				{
					gameObject2 = Object.Instantiate<GameObject>(title);
				}
				UILabel component = gameObject2.transform.FindChild("label").GetComponent<UILabel>();
				Transform transform = gameObject2.transform.FindChild("Sprite");
				gameObject2.transform.SetParent(title.transform.parent, false);
				gameObject2.transform.localPosition = array2[(int)b];
				transform.localScale = array3[(int)b];
				component.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36d",
					array[(int)b]
				});
				b += 1;
			}
			this.mMaxJingYingNum = ConfigSystemParam.GetSystemParamByName("LegionsEliteNum", true).SafeToInt32(0);
			this.JingYingCount = 0;
			this.InitMiniHander();
			this.InitArmyGroupJingYingData();
		}

		public void ClearJIngYingList()
		{
			if (this.mOBC != null && 0 < this.mOBC.Count)
			{
				this.mOBC.Clear();
			}
		}

		public int JingYingCount
		{
			set
			{
				this.mInfText[0].Text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("军团精英人数：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					string.Format("{0}/{1}", value, this.mMaxJingYingNum)
				});
			}
		}

		public IEnumerator RefreshArmyGroupJingYingList(List<JunTuanRoleData> list, int jingyingCount)
		{
			if (list != null)
			{
				Super.ShowNetWaiting(null);
				if (0 <= this.mPopupListClickIndex && 4 > this.mPopupListClickIndex)
				{
					this.mPopupList.selection = this.mPopupList.items[(int)this.mPopupListClickIndex];
				}
				this.MlistJunTuanRoleData = list.FindAll((JunTuanRoleData e) => Global.Data.roleData.Faction == e.BhId);
				this.MlistJunTuanRoleData = this.MlistJunTuanRoleData.FindAll((JunTuanRoleData e) => ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu) == 4 || 3 == ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu));
				List<JunTuanRoleData> JingYingLst = null;
				if (this.mPopupListClickIndex == 0)
				{
					JingYingLst = this.MlistJunTuanRoleData;
				}
				else if (this.mPopupListClickIndex == 1)
				{
					JingYingLst = new List<JunTuanRoleData>();
					for (int i = 0; i < this.MlistJunTuanRoleData.Count; i++)
					{
						if (this.MlistJunTuanRoleData[i] != null && this.MlistJunTuanRoleData[i].OnlineState == 1)
						{
							JingYingLst.Add(this.MlistJunTuanRoleData[i]);
						}
					}
				}
				else if (this.mPopupListClickIndex == 2)
				{
					JingYingLst = this.MlistJunTuanRoleData.FindAll((JunTuanRoleData e) => 3 == e.JuTuanZhiWu);
				}
				else if (this.mPopupListClickIndex == 3)
				{
					JingYingLst = this.MlistJunTuanRoleData.FindAll((JunTuanRoleData e) => ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu) == 4);
				}
				JingYingLst.Sort(delegate(JunTuanRoleData s, JunTuanRoleData e)
				{
					if (e.JuTuanZhiWu != s.JuTuanZhiWu)
					{
						return ArmyGroupPart.GetZhiWu(s.JuTuanZhiWu) - ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu);
					}
					return e.ZhanLi - s.ZhanLi;
				});
				List<JunTuanRoleData> SortLst = new List<JunTuanRoleData>();
				SortLst.AddRange(JingYingLst);
				Vector3 pos = this.mListbox.transform.localPosition;
				pos.x = 9f;
				if (7 > SortLst.Count)
				{
					pos.y = 120f;
				}
				this.mListbox.transform.localPosition = pos;
				yield return null;
				byte Add = 0;
				for (int j = 0; j < SortLst.Count; j++)
				{
					if (SortLst[j] != null)
					{
						GameObject Itemobj = this.mOBC.GetAt(j);
						ArmyGropListItem item = null;
						if (null != Itemobj)
						{
							item = Itemobj.GetComponent<ArmyGropListItem>();
						}
						if (null == item)
						{
							item = U3DUtils.NEW<ArmyGropListItem>();
							Add = 1;
						}
						string[] content = new string[]
						{
							Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								SortLst[j].RoleName
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"99ccff",
								Global.GetOccupationStr(SortLst[j].Occu)
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								string.Format("Lv:{0}[{1}{2}]", SortLst[j].Level, SortLst[j].ChangeLifeCount, Global.GetLang("转"))
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								SortLst[j].ZhanLi
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								ConfigArmyGroupLegions.GetZhiWuNameByID(SortLst[j].JuTuanZhiWu)
							})
						};
						if (SortLst[j].OnlineState != 1 && content != null && 0 < content.Length)
						{
							byte k = 0;
							while ((int)k < content.Length)
							{
								content[(int)k] = Global.GetColorStringForNGUIText(new object[]
								{
									"808081",
									Super.ClearStringColor(content[(int)k])
								});
								k += 1;
							}
						}
						item.Index = j;
						item.ID = SortLst[j].RoleId;
						item.BSelect = false;
						item.MRoleName = SortLst[j].RoleName;
						item.Clear();
						item.SetContent(content, this.mType);
						item.AddTexMenContent(Global.GetLang("查 看"));
						item.AddTexMenContent(Global.GetLang("私 聊"));
						item.SetVoiceToFalse = false;
						item.Hander = new DPSelectedItemEventHandler(this.ItemClick);
						if (this.mType == ArmyGropListItem.ItemUIType.JingYingGUanLi2)
						{
							if (ArmyGroupPart.GetZhiWu(SortLst[j].JuTuanZhiWu) == 4)
							{
								item.AddTexMenContent(string.Format(Global.GetLang("升为{0}"), ConfigArmyGroupLegions.GetZhiWuNameByID(3)));
							}
							else if (ArmyGroupPart.GetZhiWu(SortLst[j].JuTuanZhiWu) == 2)
							{
								item.AddTexMenContent(string.Format(Global.GetLang("降为{0}"), ConfigArmyGroupLegions.GetZhiWuNameByID(4)));
							}
							else if (ArmyGroupPart.GetZhiWu(SortLst[j].JuTuanZhiWu) == 3)
							{
								item.AddTexMenContent(string.Format(Global.GetLang("降为{0}"), ConfigArmyGroupLegions.GetZhiWuNameByID(4)));
							}
						}
						if (Add == 1)
						{
							this.mOBC.AddNoUpdate(item);
							item.DraggablePanel = this.mDraggablePanel;
						}
					}
					if (j % 6 == 0 && j != 0)
					{
						yield return null;
					}
				}
				ArmyGroupPart.CheckListBoxChild(SortLst.Count, this.mOBC);
				yield return null;
				if (null != this.mDraggablePanel.verticalScrollBar)
				{
					this.mDraggablePanel.verticalScrollBar.scrollValue = 0f;
				}
				Super.HideNetWaiting();
			}
			yield break;
		}

		private void ItemClick(object sender, DPSelectedItemEventArgs args)
		{
			if (args.Index == 10)
			{
				int id = args.ID;
				int type = args.Type;
				List<int> olodJingYingYingList = this.GetOLodJingYingYingList(3);
				List<int> list = new List<int>();
				list.Add(Global.Data.RoleID);
				list.Add(3);
				if (type == 0)
				{
					foreach (int num in olodJingYingYingList)
					{
						if (num != id)
						{
							list.Add(num);
						}
					}
				}
				else
				{
					list.AddRange(olodJingYingYingList);
					list.Add(id);
				}
				GameInstance.Game.SendChangeArmyGroupRoleZhiWu(list);
			}
			else if (args.Index == 44)
			{
				this.mCBHander(sender, args);
			}
		}

		private void InitArmyGroupJingYingData()
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendGetArmyGroupJingYingDataLst();
		}

		private void PopupListSelectChange(string item)
		{
			int num = this.mPopupList.items.FindIndex((string e) => e == item);
			if (0 <= num && 4 > num)
			{
				this.mPopupListClickIndex = (byte)num;
				if (this.mCBHander != null)
				{
					this.mCBHander(null, new DPSelectedItemEventArgs
					{
						Index = num
					});
				}
			}
		}

		private void InitMiniHander()
		{
			this.mPopupList.onSelectionChange = new UIPopupList.OnSelectionChange(this.PopupListSelectChange);
			this.mPopupList.items.Clear();
			this.mPopupList.items.Clear();
			this.mPopupList.items.Add(Global.GetLang("所有成员"));
			this.mPopupList.items.Add(Global.GetLang("在线成员"));
			this.mPopupList.items.Add(Global.GetLang("军团精英"));
			this.mPopupList.items.Add(Global.GetLang("普通成员"));
			this.mPopupList.selection = this.mPopupList.items[(int)this.mPopupListClickIndex];
			this.mPopupList.SelectItemTextHaveColor = true;
			this.mPopupList.SelectItemTextColor = NGUIMath.HexToColorEx(16644061U);
			this.mPopupList.NormalItemTextColor = NGUIMath.HexToColorEx(8421505U);
			this.mOBC = this.mListbox.ItemsSource;
			this.mListbox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ListBoxSelectChange);
			this.mJingYIngBtns[1].Label.text = Global.GetLang("批量提升");
			this.mJingYIngBtns[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.mCBHander(e, new DPSelectedItemEventArgs
				{
					Index = 22
				});
			};
			this.mJingYIngBtns[0].Label.text = Global.GetLang("批量降级");
			this.mJingYIngBtns[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.mCBHander(e, new DPSelectedItemEventArgs
				{
					Index = 23
				});
			};
			Vector3 localPosition = this.mJingYIngBtns[0].transform.localPosition;
			localPosition.x = -70f;
			this.mJingYIngBtns[0].transform.localPosition = localPosition;
			localPosition.x = 70f;
			this.mJingYIngBtns[1].transform.localPosition = localPosition;
			NGUITools.SetActive(this.mJingYIngBtns[1].gameObject, true);
			NGUITools.SetActive(this.mJingYIngBtns[0].gameObject, true);
		}

		private List<int> GetOLodJingYingYingList(int zhiWu)
		{
			List<int> list = new List<int>();
			if (this.MlistJunTuanRoleData != null)
			{
				for (int i = 0; i < this.MlistJunTuanRoleData.Count; i++)
				{
					if (Global.Data.roleData.Faction == this.MlistJunTuanRoleData[i].BhId && zhiWu == this.MlistJunTuanRoleData[i].JuTuanZhiWu && 0 < this.MlistJunTuanRoleData[i].JuTuanZhiWu)
					{
						list.Add(this.MlistJunTuanRoleData[i].RoleId);
					}
				}
			}
			return list;
		}

		private List<int> GetAllSeleceRoleID()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < this.mOBC.Count; i++)
			{
				GameObject at = this.mOBC.GetAt(i);
				if (null != at)
				{
					ArmyGropListItem component = at.GetComponent<ArmyGropListItem>();
					if (null != component && component.BSelect)
					{
						list.Add(component.ID);
					}
				}
			}
			return list;
		}

		private void PopupListOnHover(GameObject go, bool state)
		{
		}

		private void ListBoxSelectChange(object sender, MouseEvent e)
		{
			GameObject selectedItem = this.mListbox.SelectedItem;
			if (null != selectedItem)
			{
				ArmyGropListItem component = selectedItem.GetComponent<ArmyGropListItem>();
				if (null != component)
				{
					if (1 < component.MClickCount)
					{
						component.BSelect = false;
					}
					else
					{
						component.BSelect = true;
					}
				}
			}
			GameObject lastSelectedItem = this.mListbox.LastSelectedItem;
			if (null != lastSelectedItem && lastSelectedItem != selectedItem)
			{
				ArmyGropListItem component2 = lastSelectedItem.GetComponent<ArmyGropListItem>();
				if (null != component2)
				{
					component2.BSelect = false;
				}
			}
		}

		public void SetUIType(ArmyGropListItem.ItemUIType Type)
		{
			this.mType = Type;
		}

		private ListBox mListbox;

		private TextBlock[] mInfText;

		private UIDraggablePanel mDraggablePanel;

		private ObservableCollection mOBC;

		private GButton[] mJingYIngBtns;

		public DPSelectedItemEventHandler mCBHander;

		private UIPopupList mPopupList;

		private byte mPopupListClickIndex;

		private int mMaxJingYingNum;

		private ArmyGropListItem.ItemUIType mType;

		private List<JunTuanRoleData> MlistJunTuanRoleData;
	}

	public class ArmyGroupJingYingLisGuanLiOf1MiniPart
	{
		public ArmyGroupJingYingLisGuanLiOf1MiniPart(GameObject title, ListBox Listbox, UIDraggablePanel DraggablePanel, GButton[] JingYIngBtns, UISprite titlesp, UIPopupList popupList, TextBlock[] InfText)
		{
			for (int i = 1; i < title.transform.parent.childCount; i++)
			{
				GameObject gameObject = title.transform.parent.GetChild(i).gameObject;
				if (gameObject.name != "name")
				{
					Object.Destroy(gameObject);
				}
			}
			this.mDraggablePanel = DraggablePanel;
			this.mListbox = Listbox;
			this.mJingYIngBtns = JingYIngBtns;
			this.mPopupList = popupList;
			this.mInfText = InfText;
			titlesp.spriteName = "BhJingYing";
			string[] array = new string[]
			{
				Global.GetLang("成员名称"),
				Global.GetLang("职业"),
				Global.GetLang("等级"),
				Global.GetLang("个人战斗力"),
				Global.GetLang("职务")
			};
			Vector3[] array2 = new Vector3[]
			{
				new Vector3(-342f, 0f, -0.5f),
				new Vector3(-142f, 0f, -0.5f),
				new Vector3(0f, 0f, -0.5f),
				new Vector3(170f, 0f, -0.5f),
				new Vector3(369f, 0f, -0.5f)
			};
			Vector3[] array3 = new Vector3[]
			{
				new Vector3(256f, 44f, 1f),
				new Vector3(144f, 44f, 1f),
				new Vector3(144f, 44f, 1f),
				new Vector3(200f, 44f, 1f),
				new Vector3(202f, 44f, 1f)
			};
			byte b = 0;
			while ((int)b < array.Length)
			{
				GameObject gameObject2;
				if (b == 0)
				{
					gameObject2 = title;
				}
				else
				{
					gameObject2 = Object.Instantiate<GameObject>(title);
				}
				UILabel component = gameObject2.transform.FindChild("label").GetComponent<UILabel>();
				Transform transform = gameObject2.transform.FindChild("Sprite");
				gameObject2.transform.SetParent(title.transform.parent, false);
				gameObject2.transform.localPosition = array2[(int)b];
				transform.localScale = array3[(int)b];
				component.pivot = 1;
				component.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36d",
					array[(int)b]
				});
				b += 1;
			}
			this.mMaxJingYingNum = ConfigSystemParam.GetSystemParamByName("LegionsEliteNum", true).SafeToInt32(0);
			this.JingYingCount = 0;
		}

		public void ClearJIngYingList()
		{
			if (this.mOBC != null && 0 < this.mOBC.Count)
			{
				this.mOBC.Clear();
			}
		}

		public int JingYingCount
		{
			set
			{
				this.mInfText[0].Text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("军团精英人数：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					string.Format("{0}/{1}", value, this.mMaxJingYingNum)
				});
			}
		}

		public IEnumerator RefreshArmyGroupJingYingList(List<JunTuanRoleData> list, int JingYingCount = 0)
		{
			if (list != null)
			{
				if (0 <= this.mPopupListClickIndex && 4 > this.mPopupListClickIndex)
				{
					this.mPopupList.selection = this.mPopupList.items[(int)this.mPopupListClickIndex];
				}
				this.MlistJunTuanRoleDataBack = list;
				Super.ShowNetWaiting(null);
				if (this.mType == ArmyGropListItem.ItemUIType.JingYingGUanLi4)
				{
					this.MlistJunTuanRoleData = list.FindAll((JunTuanRoleData e) => Global.Data.roleData.Faction == e.BhId && 3 == e.JuTuanZhiWu);
				}
				else
				{
					this.MlistJunTuanRoleData = list.FindAll((JunTuanRoleData e) => Global.Data.roleData.Faction == e.BhId && 0 == e.JuTuanZhiWu);
				}
				List<JunTuanRoleData> JingYingLst = null;
				if (this.mPopupListClickIndex == 0)
				{
					JingYingLst = this.MlistJunTuanRoleData;
				}
				else if (this.mPopupListClickIndex == 1)
				{
					JingYingLst = new List<JunTuanRoleData>();
					for (int i = 0; i < this.MlistJunTuanRoleData.Count; i++)
					{
						if (this.MlistJunTuanRoleData[i] != null && this.MlistJunTuanRoleData[i].OnlineState == 1)
						{
							JingYingLst.Add(this.MlistJunTuanRoleData[i]);
						}
					}
				}
				else if (this.mPopupListClickIndex == 2)
				{
					JingYingLst = this.MlistJunTuanRoleData.FindAll((JunTuanRoleData e) => 3 == e.JuTuanZhiWu);
				}
				else if (this.mPopupListClickIndex == 3)
				{
					JingYingLst = this.MlistJunTuanRoleData.FindAll((JunTuanRoleData e) => ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu) == 4);
				}
				JingYingLst.Sort(delegate(JunTuanRoleData s, JunTuanRoleData e)
				{
					if (e.JuTuanZhiWu != s.JuTuanZhiWu)
					{
						return ArmyGroupPart.GetZhiWu(s.JuTuanZhiWu) - ArmyGroupPart.GetZhiWu(e.JuTuanZhiWu);
					}
					return e.ZhanLi - s.ZhanLi;
				});
				List<JunTuanRoleData> SortLst = new List<JunTuanRoleData>();
				SortLst.AddRange(JingYingLst);
				Vector3 pos = this.mListbox.transform.localPosition;
				pos.x = 9f;
				if (7 > SortLst.Count)
				{
					pos.y = 120f;
				}
				this.mListbox.transform.localPosition = pos;
				yield return null;
				byte Add = 0;
				for (int j = 0; j < SortLst.Count; j++)
				{
					if (SortLst[j] != null)
					{
						GameObject Itemobj = this.mOBC.GetAt(j);
						ArmyGropListItem item = null;
						if (null != Itemobj)
						{
							item = Itemobj.GetComponent<ArmyGropListItem>();
						}
						if (null == item)
						{
							item = U3DUtils.NEW<ArmyGropListItem>();
							Add = 1;
						}
						string[] content = new string[]
						{
							Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								SortLst[j].RoleName
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"99ccff",
								Global.GetOccupationStr(SortLst[j].Occu)
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								string.Format("Lv:{0}[{1}{2}]", SortLst[j].Level, SortLst[j].ChangeLifeCount, Global.GetLang("转"))
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								SortLst[j].ZhanLi
							}),
							Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								ConfigArmyGroupLegions.GetZhiWuNameByID(SortLst[j].JuTuanZhiWu)
							})
						};
						if (SortLst[j].OnlineState != 1 && content != null && 0 < content.Length)
						{
							byte k = 0;
							while ((int)k < content.Length)
							{
								content[(int)k] = Global.GetColorStringForNGUIText(new object[]
								{
									"808081",
									Super.ClearStringColor(content[(int)k])
								});
								k += 1;
							}
						}
						item.Index = j;
						item.ID = SortLst[j].RoleId;
						item.BSelect = false;
						item.Clear();
						item.SetContent(content, this.mType);
						item.AddTexMenContent(Global.GetLang("查 看"));
						item.AddTexMenContent(Global.GetLang("私 聊"));
						item.Hander = new DPSelectedItemEventHandler(this.ItemClick);
						if (Add == 1)
						{
							this.mOBC.AddNoUpdate(item);
							item.DraggablePanel = this.mDraggablePanel;
						}
					}
					if (j == 10)
					{
						this.mDraggablePanel.Press(false);
						Super.HideNetWaiting();
					}
					if (j % 10 == 0 && j != 0)
					{
						yield return null;
					}
				}
				ArmyGroupPart.CheckListBoxChild(SortLst.Count, this.mOBC);
				yield return null;
				if (null != this.mDraggablePanel.verticalScrollBar)
				{
					this.mDraggablePanel.verticalScrollBar.scrollValue = 0f;
				}
				Super.HideNetWaiting();
			}
			yield break;
		}

		private void ItemClick(object sender, DPSelectedItemEventArgs args)
		{
			if (args.Index == 44)
			{
				this.mCBHander(sender, args);
			}
		}

		private void InitArmyGroupJingYingData()
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendGetArmyGroupJingYingDataLst();
		}

		private void PopupListSelectChange(string item)
		{
			int num = this.mPopupList.items.FindIndex((string e) => e == item);
			if (0 <= num && 4 > num)
			{
				this.mPopupListClickIndex = (byte)num;
				if (this.mCBHander != null)
				{
					this.mCBHander(null, new DPSelectedItemEventArgs
					{
						Index = num
					});
				}
			}
		}

		private void InitMiniHander()
		{
			this.mPopupList.onSelectionChange = new UIPopupList.OnSelectionChange(this.PopupListSelectChange);
			this.mPopupList.items.Clear();
			this.mPopupList.items.Add(Global.GetLang("所有成员"));
			this.mPopupList.items.Add(Global.GetLang("在线成员"));
			this.mPopupList.selection = this.mPopupList.items[(int)this.mPopupListClickIndex];
			this.mPopupList.SelectItemTextHaveColor = true;
			this.mPopupList.SelectItemTextColor = NGUIMath.HexToColorEx(16644061U);
			this.mPopupList.NormalItemTextColor = NGUIMath.HexToColorEx(8421505U);
			UIEventListener uieventListener = this.mPopupList.gameObject.GetComponent<UIEventListener>();
			if (null == uieventListener)
			{
				uieventListener = this.mPopupList.gameObject.AddComponent<UIEventListener>();
			}
			uieventListener.onClick = new UIEventListener.VoidDelegate(this.PopupListOnClickr);
			this.mOBC = this.mListbox.ItemsSource;
			this.mListbox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ListBoxSelectChange);
			if (this.mType == ArmyGropListItem.ItemUIType.JingYingGUanLi3)
			{
				Vector3 localPosition = this.mJingYIngBtns[1].transform.localPosition;
				localPosition.x = 0f;
				this.mJingYIngBtns[1].transform.localPosition = localPosition;
				NGUITools.SetActive(this.mJingYIngBtns[0], false);
				NGUITools.SetActive(this.mJingYIngBtns[1], true);
				this.mJingYIngBtns[1].Label.text = Global.GetLang("升为精英");
				this.mJingYIngBtns[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
				{
					List<int> allSeleceRoleID = this.GetAllSeleceRoleID();
					if (0 < allSeleceRoleID.Count)
					{
						List<int> olodJingYingYingList = this.GetOLodJingYingYingList(3);
						List<int> list = new List<int>();
						list.Add(Global.Data.RoleID);
						list.Add(3);
						list.AddRange(olodJingYingYingList);
						foreach (int num in allSeleceRoleID)
						{
							if (!olodJingYingYingList.Contains(num))
							{
								list.Add(num);
							}
						}
						if (27 >= list.Count)
						{
							GameInstance.Game.SendChangeArmyGroupRoleZhiWu(list);
						}
						else
						{
							Super.HintMainText(Global.GetLang("精英数量达到上限"), 10, 3);
						}
					}
					else
					{
						Super.HintMainText(Global.GetLang("请选选择要操作的角色"), 10, 3);
					}
				};
			}
			else if (this.mType == ArmyGropListItem.ItemUIType.JingYingGUanLi4)
			{
				Vector3 localPosition2 = this.mJingYIngBtns[0].transform.localPosition;
				localPosition2.x = 0f;
				this.mJingYIngBtns[0].transform.localPosition = localPosition2;
				NGUITools.SetActive(this.mJingYIngBtns[1], false);
				NGUITools.SetActive(this.mJingYIngBtns[0], true);
				this.mJingYIngBtns[0].Label.text = Global.GetLang("降为普通");
				this.mJingYIngBtns[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
				{
					List<int> allSeleceRoleID = this.GetAllSeleceRoleID();
					if (0 < allSeleceRoleID.Count)
					{
						List<int> olodJingYingYingList = this.GetOLodJingYingYingList(3);
						List<int> list = new List<int>();
						list.Add(Global.Data.RoleID);
						list.Add(3);
						foreach (int num in olodJingYingYingList)
						{
							if (!allSeleceRoleID.Contains(num))
							{
								list.Add(num);
							}
						}
						GameInstance.Game.SendChangeArmyGroupRoleZhiWu(list);
					}
					else
					{
						Super.HintMainText(Global.GetLang("请选选择要操作的角色"), 10, 3);
					}
				};
			}
		}

		private List<int> GetOLodJingYingYingList(int zhiWu)
		{
			List<int> list = new List<int>();
			if (this.MlistJunTuanRoleDataBack != null)
			{
				for (int i = 0; i < this.MlistJunTuanRoleDataBack.Count; i++)
				{
					if (Global.Data.roleData.Faction == this.MlistJunTuanRoleDataBack[i].BhId && zhiWu == this.MlistJunTuanRoleDataBack[i].JuTuanZhiWu && 0 < this.MlistJunTuanRoleDataBack[i].JuTuanZhiWu)
					{
						list.Add(this.MlistJunTuanRoleDataBack[i].RoleId);
					}
				}
			}
			return list;
		}

		private List<int> GetAllSeleceRoleID()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < this.mOBC.Count; i++)
			{
				GameObject at = this.mOBC.GetAt(i);
				if (null != at)
				{
					ArmyGropListItem component = at.GetComponent<ArmyGropListItem>();
					if (null != component && component.BSelect)
					{
						list.Add(component.ID);
					}
				}
			}
			return list;
		}

		private void PopupListOnClickr(GameObject go)
		{
			if (this.mPopupList.isOpen)
			{
			}
		}

		private void ListBoxSelectChange(object sender, MouseEvent e)
		{
			GameObject selectedItem = this.mListbox.SelectedItem;
			if (null != selectedItem)
			{
				ArmyGropListItem component = selectedItem.GetComponent<ArmyGropListItem>();
				if (null != component)
				{
					if (1 < component.MClickCount)
					{
						component.BSelect = false;
					}
					else
					{
						component.BSelect = true;
					}
				}
			}
		}

		public void SetUIType(ArmyGropListItem.ItemUIType Type)
		{
			this.mType = Type;
			this.InitMiniHander();
			this.InitArmyGroupJingYingData();
		}

		private ListBox mListbox;

		private TextBlock[] mInfText;

		private UIDraggablePanel mDraggablePanel;

		private ObservableCollection mOBC;

		private GButton[] mJingYIngBtns;

		public DPSelectedItemEventHandler mCBHander;

		private UIPopupList mPopupList;

		private byte mPopupListClickIndex;

		private int mMaxJingYingNum;

		private ArmyGropListItem.ItemUIType mType;

		private List<JunTuanRoleData> MlistJunTuanRoleData;

		private List<JunTuanRoleData> MlistJunTuanRoleDataBack;

		private List<GameObject> lst = new List<GameObject>();
	}

	public class ArmyGroupShenQingListMiniPart : IDisposable
	{
		public ArmyGroupShenQingListMiniPart(GameObject title, ListBox Listbox, UIDraggablePanel DraggablePanel, GButton[] ShengQingBtns, UISprite titlesp)
		{
			this._DraggablePanel = DraggablePanel;
			this._Listbox = Listbox;
			this._ShengQingBtns = ShengQingBtns;
			titlesp.spriteName = "ShenQingList";
			Vector3[] array = new Vector3[]
			{
				new Vector3(-340f, 0f, 0f),
				new Vector3(-147f, 0f, 0f),
				new Vector3(35f, 0f, 0f),
				new Vector3(308f, 0f, 0f)
			};
			Vector3[] array2 = new Vector3[]
			{
				new Vector3(254f, 44f, 1f),
				new Vector3(137f, 44f, 1f),
				new Vector3(228f, 44f, 1f),
				new Vector3(320f, 44f, 1f)
			};
			string[] array3 = new string[]
			{
				Global.GetLang("战盟名称"),
				Global.GetLang("战盟人数"),
				Global.GetLang("战盟战力"),
				Global.GetLang("操作")
			};
			byte b = 0;
			while ((int)b < array3.Length)
			{
				GameObject gameObject;
				if (b == 0)
				{
					gameObject = title;
				}
				else
				{
					gameObject = Object.Instantiate<GameObject>(title);
				}
				UILabel component = gameObject.transform.FindChild("label").GetComponent<UILabel>();
				Transform transform = gameObject.transform.FindChild("Sprite");
				gameObject.transform.SetParent(title.transform.parent, false);
				gameObject.transform.localPosition = array[(int)b];
				transform.transform.localScale = array2[(int)b];
				component.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36d",
					array3[(int)b]
				});
				b += 1;
			}
			this.InitMiniHander();
			this.InitArmyGrouShenQingListData();
		}

		public void Dispose()
		{
			for (int i = 0; i < this.lst.Count; i++)
			{
				Object.Destroy(this.lst[i]);
			}
		}

		private void InitArmyGrouShenQingListData()
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendGetArmyGroupJionList();
		}

		private void InitMiniHander()
		{
			this.m_OBC = this._Listbox.ItemsSource;
			this._Listbox.cellHeight = 76f;
			this._Listbox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ListBoxSelectChange);
			this._ShengQingBtns[0].Label.text = Global.GetLang("全部同意");
			this._ShengQingBtns[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				int[] array = new int[this.m_OBC.Count];
				for (int i = 0; i < this.m_OBC.Count; i++)
				{
					GameObject at = this.m_OBC.GetAt(i);
					if (null != at)
					{
						ArmyGropListItem component = at.GetComponent<ArmyGropListItem>();
						array[i] = component.ID;
					}
				}
				this.CBHander(null, new DPSelectedItemEventArgs
				{
					Index = 55,
					EquipIDs = array,
					Type = 1
				});
			};
			this._ShengQingBtns[1].Label.text = Global.GetLang("全部拒绝");
			this._ShengQingBtns[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				int[] array = new int[this.m_OBC.Count];
				for (int i = 0; i < this.m_OBC.Count; i++)
				{
					GameObject at = this.m_OBC.GetAt(i);
					if (null != at)
					{
						ArmyGropListItem component = at.GetComponent<ArmyGropListItem>();
						array[i] = component.ID;
					}
				}
				this.CBHander(null, new DPSelectedItemEventArgs
				{
					Index = 55,
					EquipIDs = array,
					Type = 0
				});
			};
		}

		public void ShengQingCallBack(int ret)
		{
			if (0 <= ret)
			{
				this.m_OBC.RemoveAt(this.mLastShengQingItemIndex);
			}
		}

		public IEnumerator SendTurnDown(int[] lst, byte type)
		{
			for (int i = 0; i < lst.Length; i++)
			{
				GameInstance.Game.SendArmyGroupRespondShengQing((int)type, lst[i]);
				yield return null;
			}
			yield break;
		}

		public IEnumerator RefreshArmyGroupShenQingList(List<JunTuanRequestData> list)
		{
			if (0 >= list.Count)
			{
				this._DraggablePanel.showScrollBars = 1;
			}
			else
			{
				this._DraggablePanel.showScrollBars = 0;
			}
			if (list != null && 0 < list.Count)
			{
				this.m_OBC.Clear();
				Super.ShowNetWaiting(null);
				list.Sort(delegate(JunTuanRequestData s, JunTuanRequestData e)
				{
					if (e.ZhanLi != s.ZhanLi)
					{
						return (int)(s.ZhanLi - e.ZhanLi);
					}
					return s.RoleNum - e.RoleNum;
				});
				List<JunTuanRequestData> SortLst = new List<JunTuanRequestData>();
				SortLst.AddRange(list);
				Vector3 pos = this._Listbox.transform.localPosition;
				pos.x = 9f;
				if (7 > SortLst.Count)
				{
					pos.y = 120f;
				}
				this._Listbox.transform.localPosition = pos;
				yield return null;
				for (int i = 0; i < SortLst.Count; i++)
				{
					if (SortLst[i] != null)
					{
						ArmyGropListItem item = U3DUtils.NEW<ArmyGropListItem>();
						string[] content = new string[]
						{
							Global.FormatRoleNameZoneid(SortLst[i].BhZoneId, SortLst[i].BhName, 1, 1),
							string.Format("{0}/50", SortLst[i].RoleNum),
							SortLst[i].ZhanLi.ToString()
						};
						item.Index = i;
						item.ID = SortLst[i].BhId;
						item.SetContent(content, ArmyGropListItem.ItemUIType.ShengQingList);
						item.Hander = new DPSelectedItemEventHandler(this.ItemCallBack);
						this.m_OBC.AddNoUpdate(item);
						item.DraggablePanel = this._DraggablePanel;
					}
					if (i % 6 == 0 && i != 0)
					{
						yield return null;
					}
				}
				Super.HideNetWaiting();
			}
			yield break;
		}

		private void ItemCallBack(object sender, DPSelectedItemEventArgs args)
		{
			if (args != null)
			{
				this.mLastShengQingItemIndex = args.Index;
			}
		}

		private void ListBoxSelectChange(object sender, MouseEvent e)
		{
			GameObject selectedItem = this._Listbox.SelectedItem;
			if (null != selectedItem)
			{
				ArmyGropListItem component = selectedItem.GetComponent<ArmyGropListItem>();
				if (null != component)
				{
					component.BSelect = true;
				}
			}
			GameObject lastSelectedItem = this._Listbox.LastSelectedItem;
			if (null != lastSelectedItem && lastSelectedItem != selectedItem)
			{
				ArmyGropListItem component2 = lastSelectedItem.GetComponent<ArmyGropListItem>();
				if (null != component2)
				{
					component2.BSelect = false;
				}
			}
		}

		private ListBox _Listbox;

		private UIDraggablePanel _DraggablePanel;

		private GButton[] _ShengQingBtns;

		private ObservableCollection m_OBC;

		public DPSelectedItemEventHandler CBHander;

		private int mLastShengQingItemIndex;

		private List<GameObject> lst = new List<GameObject>();
	}

	public enum ArmyGroupListType
	{
		ArmyGroupList,
		ArmyGroupJingYing,
		BHJingYing,
		ShenQingLst,
		AllArmyGroupList
	}
}
