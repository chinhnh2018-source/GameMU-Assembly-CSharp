using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ArmyGroupRenWuPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		try
		{
			this.TaskInfBtns[0].Text = Global.GetLang("贡献排行榜");
			this.ShengQingLstBtn.Text = Global.GetLang("申请列表");
			this.ShengQingLstBtn.Label.lineWidth = 115;
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"YN/DNY预制可能报空！"
			});
		}
	}

	private void InitTexture()
	{
		this.BgTexture.URL = "NetImages/GameRes/Images/ArmyGroup/ArmyGroupCreatBg1.jpg";
	}

	private void InitHandler()
	{
		this.OBC = this.ListBox.ItemsSource;
		this.Closebtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.mArmyGroupInfMiniPart = null;
			this.mArmyGroupInfMiniOf1Part = null;
			this.mArmyGroupTaskMminPart = null;
			if (this.Hander != null)
			{
				this.Hander(e, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	public void SetUIType(byte type = 0)
	{
		this.mUIType = type;
		NGUITools.SetActive(this.UIRoot[1], false);
		NGUITools.SetActive(this.UIRoot[0], false);
		if (type == 0)
		{
			if (this.mArmyGroupTaskMminPart != null)
			{
				this.mArmyGroupTaskMminPart = null;
			}
			this.mArmyGroupTaskMminPart = new ArmyGroupRenWuPart.ArmyGroupTaskMminPart(this.UIRoot[0], this.ListBox, this.DraggablePanel, this.TitleSp, this.TaskInflabel, this.TaskInfBtns);
			this.mArmyGroupTaskMminPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (this.Hander != null)
				{
					this.Hander(e, s);
				}
			};
		}
		else if (type == 1)
		{
			if (this.mArmyGroupInfMiniPart != null)
			{
				this.mArmyGroupInfMiniPart = null;
			}
			this.mArmyGroupInfMiniPart = new ArmyGroupRenWuPart.ArmyGroupInfMiniPart(this.UIRoot[1], this.ListBox, this.DraggablePanel, this.TitleSp, this.ShengQingLstBtn);
			this.mArmyGroupInfMiniPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (this.Hander != null)
				{
					this.Hander(e, s);
				}
			};
		}
		else if (type == 2)
		{
			if (this.mArmyGroupInfMiniPart != null)
			{
				this.mArmyGroupInfMiniPart = null;
			}
			this.mArmyGroupInfMiniOf1Part = new ArmyGroupRenWuPart.ArmyGroupInfMiniOf1Part(this.UIRoot[1], this.ListBox, this.DraggablePanel, this.TitleSp, this.ShengQingLstBtn);
		}
	}

	public void RefreshRenWuUI(List<JunTuanTaskData> lst)
	{
		if (lst != null && 0 < lst.Count)
		{
			List<JunTuanTaskData> list = lst.FindAll((JunTuanTaskData e) => e.TaskState == 1L && 0 == e.HasGet);
			List<JunTuanTaskData> list2 = lst.FindAll((JunTuanTaskData e) => 0L == e.TaskState);
			List<JunTuanTaskData> list3 = lst.FindAll((JunTuanTaskData e) => e.TaskState == 1L && 1 == e.HasGet);
			List<JunTuanTaskData> list4 = lst.FindAll((JunTuanTaskData e) => 2L == e.TaskState);
			List<JunTuanTaskData> list5 = new List<JunTuanTaskData>();
			if (0 < list.Count)
			{
				list.Sort((JunTuanTaskData a, JunTuanTaskData b) => a.TaskId - b.TaskId);
				list5.AddRange(list);
			}
			if (0 < list2.Count)
			{
				list2.Sort((JunTuanTaskData a, JunTuanTaskData b) => a.TaskId - b.TaskId);
				list5.AddRange(list2);
			}
			if (0 < list3.Count)
			{
				list3.Sort((JunTuanTaskData a, JunTuanTaskData b) => a.TaskId - b.TaskId);
				list5.AddRange(list3);
			}
			if (0 < list4.Count)
			{
				list4.Sort((JunTuanTaskData a, JunTuanTaskData b) => a.TaskId - b.TaskId);
				list5.AddRange(list4);
			}
			if (0 < list5.Count && this.mArmyGroupTaskMminPart != null)
			{
				base.StartCoroutine<bool>(this.mArmyGroupTaskMminPart.RefreshTaskUI(list5));
			}
		}
	}

	public void RefreshInFUI(List<JunTuanBangHuiData> lst)
	{
		if (lst != null && 0 < lst.Count)
		{
			lst.Sort((JunTuanBangHuiData a, JunTuanBangHuiData b) => b.JuTuanZhiWu - a.JuTuanZhiWu);
			if (this.mArmyGroupInfMiniPart != null)
			{
				base.StartCoroutine<bool>(this.mArmyGroupInfMiniPart.RefreshInfUI(lst));
			}
			if (this.mArmyGroupInfMiniOf1Part != null)
			{
				base.StartCoroutine<bool>(this.mArmyGroupInfMiniOf1Part.RefreshInfUI(lst));
			}
		}
	}

	public int MWeekRank
	{
		get
		{
			return this.mWeekRank;
		}
		set
		{
			this.mWeekRank = value;
		}
	}

	public int GongXian
	{
		set
		{
			if (this.mArmyGroupTaskMminPart != null)
			{
				this.mArmyGroupTaskMminPart.GongXian = value;
			}
		}
	}

	public int Rank
	{
		set
		{
			if (this.mArmyGroupTaskMminPart != null)
			{
				this.mArmyGroupTaskMminPart.Rank = value;
			}
		}
	}

	public byte UIType
	{
		get
		{
			return this.mUIType;
		}
	}

	public ShowNetImage BgTexture;

	public UISprite TitleSp;

	public GButton Closebtn;

	public ListBox ListBox;

	public UIDraggablePanel DraggablePanel;

	public UILabel[] TaskInflabel;

	public GButton[] TaskInfBtns;

	public GButton ShengQingLstBtn;

	public GameObject[] UIRoot;

	private ObservableCollection OBC;

	private byte mUIType;

	private int mWeekRank;

	private ArmyGroupRenWuPart.ArmyGroupInfMiniPart mArmyGroupInfMiniPart;

	private ArmyGroupRenWuPart.ArmyGroupInfMiniOf1Part mArmyGroupInfMiniOf1Part;

	private ArmyGroupRenWuPart.ArmyGroupTaskMminPart mArmyGroupTaskMminPart;

	public DPSelectedItemEventHandler Hander;

	public class JunTuanTaskDataXml
	{
		public JunTuanTaskDataXml(XElement item)
		{
			this.ID = Global.GetXElementAttributeInt(item, "ID");
			this.CompleteType = Global.GetXElementAttributeInt(item, "CompleteType");
			this.TypeID = Global.GetXElementAttributeStr(item, "TypeID");
			this.NumInterval = Global.GetXElementAttributeInt(item, "NumInterval");
			this.Name = Global.GetXElementAttributeStr(item, "Name");
			this.Describtion = Global.GetXElementAttributeStr(item, "Describtion");
			this.Exp = Global.GetXElementAttributeInt(item, "Exp");
			this.ZhanGong = Global.GetXElementAttributeInt(item, "ZhanGong");
			this.Item = Global.GetXElementAttributeStr(item, "Item");
			this.Score = Global.GetXElementAttributeInt(item, "Score");
		}

		public int ID;

		public int CompleteType;

		public string TypeID;

		public int NumInterval;

		public string Name;

		public string Describtion;

		public int Exp;

		public int ZhanGong;

		public string Item;

		public int Score;
	}

	private class ArmyGroupInfMiniPart
	{
		public ArmyGroupInfMiniPart(GameObject rootObj, ListBox listBox, UIDraggablePanel draggablePanel, UISprite titleBgSp, GButton shengQingLstBtn)
		{
			this.mRootObj = rootObj;
			this.mListBox = listBox;
			this.mDraggablePanel = draggablePanel;
			this.mTitleBgSp = titleBgSp;
			this.mShengQingLstBtn = shengQingLstBtn;
			this.mTitleBgSp.spriteName = "ArmyGroupInf";
			NGUITools.SetActive(this.mRootObj, true);
			this.mOBC = this.mListBox.ItemsSource;
			if (Global.Data.roleData.JunTuanZhiWu == 1)
			{
				NGUITools.SetActive(this.mShengQingLstBtn, true);
				this.mShengQingLstBtn.Text = Global.GetLang("申请列表");
			}
			else
			{
				NGUITools.SetActive(this.mShengQingLstBtn, false);
			}
			this.mShengQingLstBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(e, new DPSelectedItemEventArgs
					{
						ID = 1,
						Type = 1
					});
				}
			};
		}

		public IEnumerator RefreshInfUI(List<JunTuanBangHuiData> lst)
		{
			if (Global.Data.roleData.JunTuanZhiWu == 1)
			{
				NGUITools.SetActive(this.mShengQingLstBtn, true);
				this.mShengQingLstBtn.Text = Global.GetLang("申请列表");
			}
			else
			{
				NGUITools.SetActive(this.mShengQingLstBtn, false);
			}
			Super.ShowNetWaiting(null);
			yield return null;
			if (lst != null && 0 < lst.Count)
			{
				for (int i = 0; i < lst.Count; i++)
				{
					JunTuanBangHuiData d = lst[i];
					if (d != null)
					{
						ArmyGroupItem item = null;
						GameObject itemobj = this.mOBC.GetAt(i);
						if (null != itemobj)
						{
							item = itemobj.GetComponent<ArmyGroupItem>();
						}
						if (null == item)
						{
							item = U3DUtils.NEW<ArmyGroupItem>();
							this.mOBC.Add(item);
						}
						item.SetType(ArmyGroupItem.AwryGroupItemType.LianMeng);
						item.RefreshLianMengUI(d);
						item.DraggablePanel = this.mDraggablePanel;
						item.Hander = new DPSelectedItemEventHandler(this.TaskItemClickCallBack);
					}
					if (i % 2 == 0 && i != 0)
					{
						yield return null;
					}
				}
				if (lst.Count < this.mOBC.Count)
				{
					for (int j = lst.Count; j < this.mOBC.Count; j++)
					{
						NGUITools.SetActive(this.mOBC.GetAt(j), false);
					}
				}
				else
				{
					for (int k = lst.Count; k < this.mOBC.Count; k++)
					{
						NGUITools.SetActive(this.mOBC.GetAt(k), true);
					}
				}
				if (4 > lst.Count)
				{
					Vector3 pos = this.mListBox.transform.localPosition;
					pos.y = 150f;
					this.mListBox.transform.localPosition = pos;
				}
				yield return null;
				this.mDraggablePanel.Press(false);
			}
			Super.HideNetWaiting();
			yield break;
		}

		private void TaskItemClickCallBack(object sender, DPSelectedItemEventArgs args)
		{
			if (args != null && args.IDType == 0)
			{
				int myID = args.MyID;
				if (myID == 1)
				{
					if (args.Data != null)
					{
						JunTuanBangHuiData data = args.Data as JunTuanBangHuiData;
						if (data != null)
						{
							Super.ShowMessageBox(Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
							{
								"dac7ae",
								Global.GetLang("您确定要让出军团长？让出军团长后将降为士官长")
							}), 1, delegate(object e, DPSelectedItemEventArgs s)
							{
								if (s.ID == 0)
								{
									GameInstance.Game.SendChangeArmyGroupZhiWu(data.BhId);
								}
							}, MessBoxIsHintTypes.None);
						}
					}
				}
				else if (myID == 11)
				{
					JunTuanBangHuiData data = args.Data as JunTuanBangHuiData;
					if (data != null)
					{
						Super.ShowMessageBox(Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							Global.GetLang("您确认要退出军团吗？退出军团不会影响到战盟")
						}), 1, delegate(object e, DPSelectedItemEventArgs s)
						{
							if (s.ID == 0)
							{
								GameInstance.Game.SendQUITArmyGroup(data.BhId);
							}
						}, MessBoxIsHintTypes.None);
					}
				}
				else if (myID == 12)
				{
					Super.ShowMessageBox(Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("您确认要解散军团吗？军团解散以后将不可恢复，请谨慎操作！\n解散军团不会影响到战盟")
					}), 1, delegate(object e, DPSelectedItemEventArgs s)
					{
						if (s.ID == 0)
						{
							GameInstance.Game.SendDestoryArmyGroup();
						}
					}, MessBoxIsHintTypes.None);
				}
				else if (myID == 13 && args.Data != null)
				{
					JunTuanBangHuiData data = args.Data as JunTuanBangHuiData;
					if (data != null)
					{
						string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							string.Format(Global.GetLang("您确认要将{0}踢出军团吗？"), Global.FormatRoleNameZoneid(data.BhZoneId, data.BhName, 1, 1))
						});
						Super.ShowMessageBox(Global.GetLang("提示"), colorStringForNGUIText, 1, delegate(object e, DPSelectedItemEventArgs s)
						{
							if (s.ID == 0)
							{
								GameInstance.Game.SendQUITArmyGroup(data.BhId);
							}
						}, MessBoxIsHintTypes.None);
					}
				}
			}
		}

		private ListBox mListBox;

		private UIDraggablePanel mDraggablePanel;

		private GameObject mRootObj;

		private ObservableCollection mOBC;

		private UISprite mTitleBgSp;

		private GButton mShengQingLstBtn;

		public DPSelectedItemEventHandler Hander;
	}

	private class ArmyGroupInfMiniOf1Part
	{
		public ArmyGroupInfMiniOf1Part(GameObject rootObj, ListBox listBox, UIDraggablePanel draggablePanel, UISprite titleBgSp, GButton shengQingLstBtn)
		{
			this.mRootObj = rootObj;
			this.mListBox = listBox;
			this.mDraggablePanel = draggablePanel;
			this.mTitleBgSp = titleBgSp;
			this.mShengQingLstBtn = shengQingLstBtn;
			this.mTitleBgSp.spriteName = "ArmyGroupInf";
			NGUITools.SetActive(this.mRootObj, true);
			this.mOBC = this.mListBox.ItemsSource;
			if (Global.Data.roleData.JunTuanZhiWu == 1)
			{
				NGUITools.SetActive(this.mShengQingLstBtn, true);
				this.mShengQingLstBtn.Text = Global.GetLang("申请列表");
			}
			else
			{
				NGUITools.SetActive(this.mShengQingLstBtn, false);
			}
		}

		public IEnumerator RefreshInfUI(List<JunTuanBangHuiData> lst)
		{
			if (Global.Data.roleData.JunTuanZhiWu == 1)
			{
				NGUITools.SetActive(this.mShengQingLstBtn, true);
				this.mShengQingLstBtn.Text = Global.GetLang("申请列表");
			}
			else
			{
				NGUITools.SetActive(this.mShengQingLstBtn, false);
			}
			Super.ShowNetWaiting(null);
			yield return null;
			if (lst != null && 0 < lst.Count)
			{
				for (int i = 0; i < lst.Count; i++)
				{
					JunTuanBangHuiData d = lst[i];
					if (d != null)
					{
						ArmyGroupItem item = null;
						GameObject itemobj = this.mOBC.GetAt(i);
						if (null != itemobj)
						{
							item = itemobj.GetComponent<ArmyGroupItem>();
						}
						if (null == item)
						{
							item = U3DUtils.NEW<ArmyGroupItem>();
							this.mOBC.Add(item);
						}
						item.SetType(ArmyGroupItem.AwryGroupItemType.LianMeng1);
						item.RefreshLianMengUI(d);
						item.DraggablePanel = this.mDraggablePanel;
					}
					if (i % 2 == 0 && i != 0)
					{
						yield return null;
					}
				}
				if (lst.Count < this.mOBC.Count)
				{
					for (int j = lst.Count; j < this.mOBC.Count; j++)
					{
						NGUITools.SetActive(this.mOBC.GetAt(j), false);
					}
				}
				else
				{
					for (int k = lst.Count; k < this.mOBC.Count; k++)
					{
						NGUITools.SetActive(this.mOBC.GetAt(k), true);
					}
				}
				if (4 > lst.Count)
				{
					Vector3 pos = this.mListBox.transform.localPosition;
					pos.y = 150f;
					this.mListBox.transform.localPosition = pos;
				}
				yield return null;
				this.mDraggablePanel.Press(false);
			}
			Super.HideNetWaiting();
			yield break;
		}

		private ListBox mListBox;

		private UIDraggablePanel mDraggablePanel;

		private GameObject mRootObj;

		private ObservableCollection mOBC;

		private UISprite mTitleBgSp;

		private GButton mShengQingLstBtn;
	}

	private class ArmyGroupTaskMminPart
	{
		public ArmyGroupTaskMminPart(GameObject rootObj, ListBox listBox, UIDraggablePanel draggablePanel, UISprite titleBgSp, UILabel[] taskInflabel, GButton[] taskInfBtns)
		{
			this.mRootObj = rootObj;
			this.mListBox = listBox;
			this.mDraggablePanel = draggablePanel;
			this.mTitleBgSp = titleBgSp;
			this.mTitleBgSp.spriteName = "ArmyGroupRenwuTitle";
			this.mTaskInflabel = taskInflabel;
			this.mTaskInfBtns = taskInfBtns;
			NGUITools.SetActive(this.mRootObj, true);
			this.mOBC = this.mListBox.ItemsSource;
			this.InitTaskXml();
			this.Rank = 0;
			this.GongXian = 0;
			this.mTaskInfBtns[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				Global.ShowHelpPart(this.str_Help[0], Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					this.str_Help[1]
				}), true, -11f);
			};
			this.mTaskInfBtns[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				PlayZone.GlobalPlayZone.ShowArmyGroupGongXianRank();
			};
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendGetArmyGroupTaskList();
		}

		public IEnumerator RefreshTaskUI(List<JunTuanTaskData> lst)
		{
			Super.ShowNetWaiting(null);
			yield return null;
			if (lst != null && 0 < lst.Count)
			{
				for (int i = 0; i < lst.Count; i++)
				{
					JunTuanTaskData d = lst[i];
					if (d != null && this.mDicJuntTuanTaskDataXml.ContainsKey(d.TaskId))
					{
						ArmyGroupItem item = null;
						GameObject itemobj = this.mOBC.GetAt(i);
						if (null != itemobj)
						{
							item = itemobj.GetComponent<ArmyGroupItem>();
						}
						if (null == item)
						{
							item = U3DUtils.NEW<ArmyGroupItem>();
							this.mOBC.Add(item);
						}
						item.SetType(ArmyGroupItem.AwryGroupItemType.RenWu);
						item.TaskValue = d.TaskValue;
						item.TaskData = this.mDicJuntTuanTaskDataXml[d.TaskId];
						item.AwardType = this.GetTaskState(d);
						item.DraggablePanel = this.mDraggablePanel;
						item.Hander = new DPSelectedItemEventHandler(this.TaskItemClickCallBack);
					}
					if (i % 3 == 0 && i != 0)
					{
						yield return new WaitForSeconds(0.01f);
					}
				}
				yield return null;
				this.mDraggablePanel.Press(false);
				if (3 >= lst.Count)
				{
					Vector3 pos = this.mListBox.transform.localPosition;
					pos.y = 130f;
					this.mListBox.transform.localPosition = pos;
				}
			}
			Super.HideNetWaiting();
			yield break;
		}

		public int GongXian
		{
			set
			{
				this.mTaskInflabel[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("本周贡献：{0}"), value)
				});
			}
		}

		public int Rank
		{
			set
			{
				if (0 >= value)
				{
					this.mTaskInflabel[1].text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						string.Format(Global.GetLang("贡献值排名：{0}"), Global.GetLang("暂无排名"))
					});
				}
				else
				{
					this.mTaskInflabel[1].text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						string.Format(Global.GetLang("贡献值排名：{0}"), value)
					});
				}
			}
		}

		private void TaskItemClickCallBack(object sender, DPSelectedItemEventArgs args)
		{
			if (args != null && args.IDType == 4)
			{
				ArmyGroupRenWuPart.JunTuanTaskDataXml junTuanTaskDataXml = args.Data as ArmyGroupRenWuPart.JunTuanTaskDataXml;
				switch (args.Type)
				{
				case 1:
					if (junTuanTaskDataXml != null)
					{
						GameInstance.Game.SendGetArmyGroupTaskAward(junTuanTaskDataXml.ID);
					}
					break;
				}
			}
		}

		private ArmyGroupItem.AwardStateOrGetRenWu GetTaskState(JunTuanTaskData d)
		{
			if (d.HasGet == 1)
			{
				return ArmyGroupItem.AwardStateOrGetRenWu.HaveGet;
			}
			if (d.TaskState == 0L)
			{
				return ArmyGroupItem.AwardStateOrGetRenWu.Go;
			}
			if (d.TaskState == 1L)
			{
				return ArmyGroupItem.AwardStateOrGetRenWu.CanGet;
			}
			if (d.TaskState == 2L)
			{
				return ArmyGroupItem.AwardStateOrGetRenWu.TimeD;
			}
			return ArmyGroupItem.AwardStateOrGetRenWu.HaveGet;
		}

		private void InitTaskXml()
		{
			XElement gameResXml = Global.GetGameResXml("Config/LegionTasks.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "LegionTasks");
				if (xelementList != null && 0 < xelementList.Count)
				{
					for (int i = 0; i < xelementList.Count; i++)
					{
						XElement xelement = xelementList[i];
						if (xelement != null)
						{
							ArmyGroupRenWuPart.JunTuanTaskDataXml junTuanTaskDataXml = new ArmyGroupRenWuPart.JunTuanTaskDataXml(xelement);
							if (this.mDicJuntTuanTaskDataXml.ContainsKey(junTuanTaskDataXml.ID))
							{
								this.mDicJuntTuanTaskDataXml[junTuanTaskDataXml.ID] = junTuanTaskDataXml;
							}
							else
							{
								this.mDicJuntTuanTaskDataXml.Add(junTuanTaskDataXml.ID, junTuanTaskDataXml);
							}
						}
					}
				}
			}
		}

		private ListBox mListBox;

		private UIDraggablePanel mDraggablePanel;

		private GameObject mRootObj;

		private ObservableCollection mOBC;

		private UISprite mTitleBgSp;

		private Dictionary<int, ArmyGroupRenWuPart.JunTuanTaskDataXml> mDicJuntTuanTaskDataXml = new Dictionary<int, ArmyGroupRenWuPart.JunTuanTaskDataXml>();

		private UILabel[] mTaskInflabel;

		private GButton[] mTaskInfBtns;

		public DPSelectedItemEventHandler Hander;

		private string[] str_Help = new string[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("军团任务说明：")
			}),
			string.Concat(new string[]
			{
				Global.GetLang("1、军团任务在"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("周一0点")
				}),
				Global.GetLang("重置、"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("周六24点")
				}),
				Global.GetLang("结束"),
				Environment.NewLine,
				Global.GetLang("2、每周军团任务数量固定，完成就可以获得"),
				Environment.NewLine,
				Global.GetLang("对应任务的"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("贡献值")
				}),
				Environment.NewLine,
				Global.GetLang("3、根据"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("当周的贡献")
				}),
				Global.GetLang("给军团排名，贡献越多排"),
				Environment.NewLine,
				Global.GetLang("名越高，相同贡献，时间越早排名越高"),
				Environment.NewLine,
				Global.GetLang("4、排名"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("前四")
				}),
				Global.GetLang("的军团会获得"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("军团战")
				}),
				Global.GetLang("的资格"),
				Environment.NewLine,
				Global.GetLang("5、奖励的"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("贡献值")
				}),
				Global.GetLang("累积到"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("军团繁荣度")
				}),
				Global.GetLang("当中"),
				Environment.NewLine,
				Global.GetLang("6、每个任务的奖励每周只可以领取"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("一次")
				})
			})
		};
	}
}
