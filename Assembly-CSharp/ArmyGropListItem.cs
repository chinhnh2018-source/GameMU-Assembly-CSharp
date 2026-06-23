using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ArmyGropListItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.BSelect = false;
	}

	private void InitPrefabText()
	{
		this.BtnS[0].Text = Global.GetLang("同意");
		this.BtnS[1].Text = Global.GetLang("拒绝");
		NGUITools.SetActive(this.mVoiceIcon.gameObject, false);
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.BtnS[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (Global.Data.roleData.JunTuanZhiWu == 1)
			{
				Super.ShowNetWaiting(null);
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Index = this.mIndex
					});
				}
				GameInstance.Game.SendArmyGroupRespondShengQing(1, this.mID);
			}
		};
		this.BtnS[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (Global.Data.roleData.JunTuanZhiWu == 1)
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendArmyGroupRespondShengQing(0, this.mID);
			}
		};
	}

	public void RefreshShengQingCallBack(int ret)
	{
		if (0 < ret)
		{
		}
	}

	public void SetContent(string[] strContent, ArmyGropListItem.ItemUIType Type = ArmyGropListItem.ItemUIType.JingYIngDian)
	{
		this.mType = Type;
		Vector3 localScale = this.BgSp.transform.localScale;
		localScale.x = 912f;
		this.BoxCollider = base.GetComponent<BoxCollider>();
		Vector3 size = this.BoxCollider.size;
		UILabel[] array = null;
		List<Vector3> list = new List<Vector3>();
		NGUITools.SetActive(this.Root[0], false);
		NGUITools.SetActive(this.Root[1], false);
		NGUITools.SetActive(this.Root[2], false);
		if (strContent != null)
		{
			if (Type == ArmyGropListItem.ItemUIType.JingYIngDian)
			{
				localScale.y = 40f;
				array = this.Labels;
				size.y = 41f;
				NGUITools.SetActive(this.Root[0], true);
				list.Add(new Vector3(-334f, 0f, -0.1f));
				list.Add(new Vector3(-77f, 0f, -0.1f));
				list.Add(new Vector3(162f, 0f, -0.1f));
				list.Add(new Vector3(375f, 0f, -0.1f));
			}
			else if (Type == ArmyGropListItem.ItemUIType.Envent)
			{
				localScale.y = 76f;
				array = this.Labels2;
				size.y = 84f;
				NGUITools.SetActive(this.Root[1], true);
				list.Add(new Vector3(-290f, 0f, -2f));
				list.Add(new Vector3(200f, 0f, -2f));
				list.Add(new Vector3(-158f, 0f, -2f));
				list.Add(new Vector3(-371f, 0f, -2f));
			}
			else if (Type == ArmyGropListItem.ItemUIType.ShengQingList)
			{
				localScale.y = 76f;
				array = this.Labels3;
				size.y = 84f;
				NGUITools.SetActive(this.Root[2], true);
				list.Add(new Vector3(-360f, 0f, -0.1f));
				list.Add(new Vector3(-68f, 0f, -0.1f));
				list.Add(new Vector3(-158f, 0f, -0.1f));
				list.Add(new Vector3(-371f, 0f, -0.1f));
			}
			else if (Type == ArmyGropListItem.ItemUIType.JingYingGUanLi1 || Type == ArmyGropListItem.ItemUIType.JingYingGUanLi2 || Type == ArmyGropListItem.ItemUIType.JingYingGUanLi3 || Type == ArmyGropListItem.ItemUIType.JingYingGUanLi4)
			{
				localScale.y = 40f;
				array = this.Labels;
				size.y = 41f;
				NGUITools.SetActive(this.Root[0], true);
				list.Add(new Vector3(-345f, 0f, -0.1f));
				list.Add(new Vector3(-144f, 0f, -0.1f));
				list.Add(new Vector3(8f, 0f, -0.1f));
				list.Add(new Vector3(160f, 0f, -0.1f));
				list.Add(new Vector3(365f, 0f, -0.1f));
			}
			else if (Type == ArmyGropListItem.ItemUIType.AllArmyGroupList || Type == ArmyGropListItem.ItemUIType.ArmyGroupList)
			{
				localScale.y = 40f;
				array = this.Labels;
				size.y = 41f;
				NGUITools.SetActive(this.Root[0], true);
				list.Add(new Vector3(-335f, 0f, -0.1f));
				list.Add(new Vector3(-74f, 0f, -0.1f));
				list.Add(new Vector3(166f, 0f, -0.1f));
				list.Add(new Vector3(372f, 0f, -0.1f));
			}
			for (int i = 0; i < array[0].transform.parent.childCount; i++)
			{
				Transform child = array[0].transform.parent.GetChild(i);
				if (null != child && child.name.Contains("Clone"))
				{
					NGUITools.Destroy(child.gameObject);
				}
			}
			byte b = 0;
			while ((int)b < strContent.Length)
			{
				if (!string.IsNullOrEmpty(strContent[(int)b]))
				{
					UILabel uilabel;
					if (array.Length > (int)b)
					{
						uilabel = array[(int)b];
					}
					else
					{
						uilabel = Object.Instantiate<UILabel>(array[0]);
						uilabel.transform.SetParent(array[0].transform.parent, false);
					}
					NGUITools.SetActive(uilabel, true);
					uilabel.text = strContent[(int)b];
					if (Type == ArmyGropListItem.ItemUIType.JingYIngDian || Type == ArmyGropListItem.ItemUIType.ArmyGroupList || Type == ArmyGropListItem.ItemUIType.AllArmyGroupList || Type == ArmyGropListItem.ItemUIType.Envent || Type == ArmyGropListItem.ItemUIType.JingYingGUanLi1 || Type == ArmyGropListItem.ItemUIType.JingYingGUanLi2 || Type == ArmyGropListItem.ItemUIType.JingYingGUanLi3 || Type == ArmyGropListItem.ItemUIType.JingYingGUanLi4)
					{
						uilabel.pivot = 4;
						uilabel.transform.localPosition = list[(int)b];
					}
				}
				b += 1;
			}
		}
		this.BgSp.transform.localScale = localScale;
		this.BoxCollider.size = size;
		this.SelectEffectObj.transform.localScale = localScale;
	}

	public void AddTexMenContent(string str)
	{
		this.mTxtMenuContent.Add(str);
	}

	public new void Clear()
	{
		this.mTxtMenuContent.Clear();
	}

	public void CreateMenuWindow()
	{
		if (Global.Data.RoleID == this.mID)
		{
			return;
		}
		if (null != this.menuPart)
		{
			Object.Destroy(this.menuPart.gameObject);
		}
		if (this.mTxtMenuContent == null || 0 >= this.mTxtMenuContent.Count)
		{
			return;
		}
		this.SetVoiceLabel();
		this.menuPart = U3DUtils.NEW<GTxtMenuPart>();
		this.menuPart.Width = 150.0;
		this.menuPart.ItemHeight = 35;
		if (this.mTxtMenuContent != null && 0 < this.mTxtMenuContent.Count)
		{
			for (int i = 0; i < this.mTxtMenuContent.Count; i++)
			{
				this.menuPart.AddMenuItem(i, this.mTxtMenuContent[i]);
			}
		}
		this.menuPart.RenderMenu();
		this.menuPart.Closehandler = delegate(object e, DPSelectedItemEventArgs s)
		{
			this.CloseMenuPart();
		};
		this.menuPart.MaskBak.transform.GetComponent<BoxCollider>().isTrigger = false;
		this.menuPart.MenuItemClick = delegate(object s, EventArgs e)
		{
			GTxtMenuItem gtxtMenuItem = s as GTxtMenuItem;
			if (null == gtxtMenuItem)
			{
				return;
			}
			if (gtxtMenuItem.MenuItemID >= 0 && gtxtMenuItem.MenuItemID <= 6)
			{
				this.ItemFunction(gtxtMenuItem);
			}
			this.menuPart.Visibility = false;
		};
		this.menuPart.SelectIndex = -1;
		U3DUtils.AddChild(base.transform.gameObject, this.menuPart.gameObject, true);
		this.menuPart.transform.localPosition = new Vector3(0f, 0f, -1.5f);
	}

	private void SetVoiceLabel()
	{
		if (this.Speech == 2)
		{
			int num = this.mTxtMenuContent.FindIndex((string tmp) => tmp == Global.GetLang("允许语音"));
			if (num >= 0)
			{
				this.mTxtMenuContent[num] = Global.GetLang("禁止语音");
			}
			int num2 = this.mTxtMenuContent.FindIndex((string tmp) => tmp == Global.GetLang("禁止语音"));
			if (num2 >= 0)
			{
				this.mTxtMenuContent[num2] = Global.GetLang("禁止语音");
			}
		}
		else if (this.Speech == 3)
		{
			int num3 = this.mTxtMenuContent.FindIndex((string tmp) => tmp == Global.GetLang("允许语音"));
			if (num3 >= 0)
			{
				this.mTxtMenuContent[num3] = Global.GetLang("允许语音");
			}
			int num4 = this.mTxtMenuContent.FindIndex((string tmp) => tmp == Global.GetLang("禁止语音"));
			if (num4 >= 0)
			{
				this.mTxtMenuContent[num4] = Global.GetLang("允许语音");
			}
		}
	}

	private void ItemFunction(GTxtMenuItem p)
	{
		if (this.mType == ArmyGropListItem.ItemUIType.JingYingGUanLi2)
		{
			if (Global.GetLang("查 看") == p.MenuItemText)
			{
				GameInstance.Game.SpriteGetOtherAttrib(this.mID);
			}
			else if (Global.GetLang("私 聊") == p.MenuItemText)
			{
				if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()))
				{
					Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				}
				else
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Index = 44,
						Title = this.mRoleName
					});
				}
			}
			else if (Global.GetLang("升为军团精英") == p.MenuItemText)
			{
				if (this.mID == Global.Data.RoleID)
				{
					Super.HintMainText(Global.GetLang("操作的是自己"), 10, 3);
				}
				else if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Index = 10,
						ID = this.mID,
						Type = 3
					});
				}
			}
			else if (Global.GetLang("降为普通成员") == p.MenuItemText)
			{
				if (this.mID == Global.Data.RoleID)
				{
					Super.HintMainText(Global.GetLang("操作的是自己"), 10, 3);
				}
				else
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Index = 10,
						ID = this.mID,
						Type = 0
					});
				}
			}
		}
		else if (this.mType == ArmyGropListItem.ItemUIType.JingYIngDian)
		{
			if (p.MenuItemID == 0)
			{
				if (Global.Data.roleData.Faction != this.mBhId)
				{
					Super.HintMainText(Global.GetLang("其他战盟成员无法查看"), 10, 3);
					return;
				}
				GameInstance.Game.SpriteGetOtherAttrib(this.mID);
			}
			else if (p.MenuItemID == 1)
			{
				if (Global.Data.roleData.Faction != this.mBhId)
				{
					Super.HintMainText(Global.GetLang("其他战盟成员无法查看"), 10, 3);
					return;
				}
				if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()))
				{
					Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				}
				else if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Index = 44,
						Title = this.mRoleName
					});
				}
			}
			else if (p.MenuItemID == 2 && this.SpeechPriorityCallBack != null)
			{
				int num = (this.Speech != 2) ? 2 : 3;
				this.SpeechPriorityCallBack.Invoke(this.ID, num);
			}
		}
	}

	private void CloseMenuPart()
	{
		if (null != this.menuPart)
		{
			this.menuPart.Visibility = false;
		}
	}

	public string MRoleName
	{
		get
		{
			return this.mRoleName;
		}
		set
		{
			this.mRoleName = value;
		}
	}

	public int ID
	{
		get
		{
			return this.mID;
		}
		set
		{
			this.mID = value;
		}
	}

	public int MBhId
	{
		set
		{
			this.mBhId = value;
		}
	}

	public ArmyGropListItem.ItemUIType Type
	{
		get
		{
			return this.mType;
		}
	}

	public int MClickCount
	{
		get
		{
			return this.mClickCount;
		}
	}

	public bool BSelect
	{
		get
		{
			return this.mBSelect;
		}
		set
		{
			this.mBSelect = value;
			NGUITools.SetActive(this.SelectEffectObj, this.mBSelect);
			if (this.mBSelect)
			{
				if (0 < this.mClickCount++ && (this.mType == ArmyGropListItem.ItemUIType.JingYIngDian || this.mType == ArmyGropListItem.ItemUIType.JingYIngDian || this.mType == ArmyGropListItem.ItemUIType.JingYingGUanLi2))
				{
					this.CreateMenuWindow();
				}
			}
			else
			{
				this.mClickCount = 0;
				this.CloseMenuPart();
			}
		}
	}

	public int Index
	{
		get
		{
			return this.mIndex;
		}
		set
		{
			this.mIndex = value;
		}
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			UIDragPanelContents uidragPanelContents = base.GetComponent<UIDragPanelContents>();
			if (null == uidragPanelContents)
			{
				uidragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
			}
			uidragPanelContents.draggablePanel = value;
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	public bool SetVoiceToFalse
	{
		set
		{
			NGUITools.SetActive(this.mVoiceIcon.gameObject, false);
		}
	}

	public int Speech
	{
		get
		{
			return this.mEnableSpeech;
		}
		set
		{
			if (Global.Data != null && Global.Data.roleData.RoleID == this.ID && Global.Data.roleData.JunTuanZhiWu == 1)
			{
				this.mEnableSpeech = 1;
				NGUITools.SetActive(this.mVoiceIcon.gameObject, false);
			}
			else
			{
				this.mEnableSpeech = value;
				NGUITools.SetActive(this.mVoiceIcon.gameObject, false);
			}
		}
	}

	private string GetSpeechPermissionText(int type)
	{
		return null;
	}

	public int JunTuanZhiWu { get; set; }

	public UISprite BgSp;

	public GameObject SelectEffectObj;

	public UILabel[] Labels;

	public UILabel[] Labels2;

	public UILabel[] Labels3;

	public BoxCollider BoxCollider;

	public GameObject[] Root;

	public GButton[] BtnS;

	public UISprite mVoiceIcon;

	private int mID;

	private bool mBSelect;

	private int mIndex;

	private ArmyGropListItem.ItemUIType mType;

	private List<string> mTxtMenuContent = new List<string>();

	private string mRoleName = string.Empty;

	private int mEnableSpeech = 1;

	public Action<int, int> SpeechPriorityCallBack;

	private GTxtMenuPart menuPart;

	public DPSelectedItemEventHandler Hander;

	private int mBhId;

	private int mClickCount;

	public enum ItemUIType
	{
		JingYIngDian,
		Envent,
		ShengQingList,
		JingYingGUanLi1,
		JingYingGUanLi2,
		AllArmyGroupList,
		ArmyGroupList,
		JingYingGUanLi3,
		JingYingGUanLi4
	}
}
