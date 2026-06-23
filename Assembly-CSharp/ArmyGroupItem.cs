using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ArmyGroupItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		if (this.mAwryGroupItemType == ArmyGroupItem.AwryGroupItemType.RenWu)
		{
			this.Btn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(e, new DPSelectedItemEventArgs
					{
						IDType = 4,
						MyID = this.mID,
						Data = this.mTaskData,
						Type = (int)this.AwardType
					});
				}
			};
		}
		else
		{
			this.Btns[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Btns[0].Label.text == Global.GetLang("任命军团长"))
				{
					this.CHander(1);
				}
			};
			this.Btns[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Btns[1].Label.text == Global.GetLang("退出军团"))
				{
					this.CHander(11);
				}
				else if (this.Btns[1].Label.text == Global.GetLang("解散军团"))
				{
					this.CHander(12);
				}
				else if (this.Btns[1].Label.text == Global.GetLang("踢出军团"))
				{
					this.CHander(13);
				}
			};
		}
	}

	private void CHander(int Type)
	{
		if (this.Hander != null && this.mJunTuanBangHuiData != null)
		{
			this.Hander(null, new DPSelectedItemEventArgs
			{
				IDType = 0,
				MyID = Type,
				Data = this.mJunTuanBangHuiData
			});
		}
	}

	private void SetBtnText()
	{
		if (this.mAwryGroupItemType == ArmyGroupItem.AwryGroupItemType.LianMeng1)
		{
			NGUITools.SetActive(this.Btns[0], false);
			NGUITools.SetActive(this.Btns[1], false);
		}
		else
		{
			ArmyGroupLegionsVO roleArmyGroupLimitsVO = ConfigArmyGroupLegions.GetRoleArmyGroupLimitsVO(ArmyGroupPart.GetZhiWu(Global.Data.roleData.JunTuanZhiWu));
			NGUITools.SetActive(this.Btns[0], false);
			NGUITools.SetActive(this.Btns[1], false);
			if (roleArmyGroupLimitsVO != null)
			{
				if (roleArmyGroupLimitsVO.Manager != 0 && this.mJunTuanBangHuiData.BhId != Global.Data.roleData.Faction)
				{
					NGUITools.SetActive(this.Btns[1], true);
					this.Btns[1].Label.text = Global.GetLang("踢出军团");
				}
				if (roleArmyGroupLimitsVO.AppointLeader != 0 && this.mJunTuanBangHuiData.BhId != Global.Data.roleData.Faction)
				{
					NGUITools.SetActive(this.Btns[0], true);
					this.Btns[0].Label.text = Global.GetLang("任命军团长");
				}
				if (roleArmyGroupLimitsVO.Quit != 0 && this.mJunTuanBangHuiData.BhId == Global.Data.roleData.Faction)
				{
					NGUITools.SetActive(this.Btns[1], true);
					this.Btns[1].Label.text = Global.GetLang("退出军团");
				}
				if (roleArmyGroupLimitsVO.Dissolution != 0 && this.mJunTuanBangHuiData.BhId == Global.Data.roleData.Faction)
				{
					NGUITools.SetActive(this.Btns[1], true);
					this.Btns[1].Label.text = Global.GetLang("解散军团");
				}
			}
		}
	}

	private int GetRoleZhiWu(int BHID, int zhiwu)
	{
		if (this.mAwryGroupItemType == ArmyGroupItem.AwryGroupItemType.LianMeng1)
		{
			return 0;
		}
		if (Global.Data.roleData.JunTuanZhiWu == 1)
		{
			if (BHID == Global.Data.roleData.Faction)
			{
				return 3;
			}
			return 111;
		}
		else
		{
			if (Global.Data.roleData.JunTuanZhiWu != 2)
			{
				return 0;
			}
			if (BHID == Global.Data.roleData.Faction)
			{
				return 2;
			}
			return 0;
		}
	}

	public void SetType(ArmyGroupItem.AwryGroupItemType type = ArmyGroupItem.AwryGroupItemType.RenWu)
	{
		this.mAwryGroupItemType = type;
		this.InitHandler();
		if (type == ArmyGroupItem.AwryGroupItemType.RenWu)
		{
			NGUITools.SetActive(this.RootRenWu, true);
			NGUITools.SetActive(this.RootLianMeng, false);
		}
		else if (type == ArmyGroupItem.AwryGroupItemType.LianMeng)
		{
			NGUITools.SetActive(this.RootRenWu, false);
			NGUITools.SetActive(this.RootLianMeng, true);
		}
		else if (type == ArmyGroupItem.AwryGroupItemType.LianMeng1)
		{
			NGUITools.SetActive(this.RootRenWu, false);
			NGUITools.SetActive(this.RootLianMeng, true);
		}
	}

	public void RefreshLianMengUI(JunTuanBangHuiData data)
	{
		if (data != null)
		{
			this.mJunTuanBangHuiData = data;
			this.mID = data.BhId;
			this.Face.URL = string.Format("NetImages/Face/{0}0_0.png", Global.CalcOriginalOccupationID(data.LeaderOccupation));
			string text = string.Empty;
			if (Global.Data.RoleID == data.LeaderRoleId)
			{
				text = "17e43e";
			}
			else
			{
				text = "dac7ae";
			}
			if (data.JuTuanZhiWu == 1)
			{
				this.ZhiWu.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					ConfigArmyGroupLegions.GetZhiWuNameByID(data.JuTuanZhiWu)
				});
			}
			else
			{
				this.ZhiWu.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					ConfigArmyGroupLegions.GetZhiWuNameByID(2)
				});
			}
			this.BHInF[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				Global.FormatRoleNameZoneid(data.LeaderZoneId, data.LeaderName, 1, 1)
			});
			this.BHInF[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("所属战盟：") + Global.FormatRoleNameZoneid(data.LeaderZoneId, data.BhName, 1, 1)
			});
			this.BHInF[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1}/{2}", Global.GetLang("战盟人数："), data.RoleNum, 50)
			});
			this.BHInF[3].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1}", Global.GetLang("战盟总战力："), data.ZhanLi)
			});
			this.SetBtnText();
		}
	}

	public int ID
	{
		set
		{
			this.mID = value;
		}
	}

	public int TaskValue
	{
		set
		{
			this.mTaskValue = value;
		}
	}

	public ArmyGroupRenWuPart.JunTuanTaskDataXml TaskData
	{
		set
		{
			this.mTaskData = value;
			this.mID = this.mTaskData.ID;
			if (this.mTaskData != null)
			{
				string text = "17e43e";
				string text2 = "17e43e";
				if (this.mTaskData.NumInterval > this.mTaskValue)
				{
					text = "fac60d";
					text2 = "ff0000";
				}
				this.RenWuMiaoShu[0].Text = Global.GetColorStringForNGUIText(new object[]
				{
					text,
					this.mTaskData.Name
				});
				this.RenWuMiaoShu[1].Text = Global.GetColorStringForNGUIText(new object[]
				{
					text,
					string.Format(this.mTaskData.Describtion, this.mTaskData.NumInterval)
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					text2,
					string.Format("({0}/{1})", (this.mTaskValue > this.mTaskData.NumInterval) ? this.mTaskData.NumInterval : this.mTaskValue, this.mTaskData.NumInterval)
				});
				this.AwardLabel[1].text = this.mTaskData.Exp.ToString();
				this.AwardLabel[0].text = this.mTaskData.ZhanGong.ToString();
				this.AwardLabel[2].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format("{0}{1}", Global.GetLang("贡献："), this.mTaskData.Score)
				});
			}
		}
	}

	public ArmyGroupItem.AwardStateOrGetRenWu AwardType
	{
		get
		{
			return this.mAwardType;
		}
		set
		{
			this.mAwardType = value;
			switch (this.mAwardType)
			{
			case ArmyGroupItem.AwardStateOrGetRenWu.HaveGet:
				NGUITools.SetActive(this.Btn, false);
				NGUITools.SetActive(this.HaveGet, true);
				this.HaveGet.GetComponent<UISprite>().spriteName = "yilingqu";
				break;
			case ArmyGroupItem.AwardStateOrGetRenWu.CanGet:
				this.Btn.Label.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ffffff",
					Global.GetLang("领取")
				});
				this.Btn.transform.GetComponent<BoxCollider>().enabled = true;
				NGUITools.SetActive(this.Btn, true);
				NGUITools.SetActive(this.HaveGet, false);
				this.Btn.target.spriteName = "tongyongBtn2_normal";
				break;
			case ArmyGroupItem.AwardStateOrGetRenWu.TimeD:
				this.Btn.Label.text = Global.GetColorStringForNGUIText(new object[]
				{
					"808081",
					Global.GetLang("已过期")
				});
				this.Btn.transform.GetComponent<BoxCollider>().enabled = false;
				this.Btn.target.spriteName = "tongyongBtn2_disable";
				NGUITools.SetActive(this.Btn, true);
				NGUITools.SetActive(this.HaveGet, false);
				break;
			case ArmyGroupItem.AwardStateOrGetRenWu.Go:
				this.Btn.Label.text = Global.GetColorStringForNGUIText(new object[]
				{
					"808081",
					Global.GetLang("未完成")
				});
				this.Btn.transform.GetComponent<BoxCollider>().enabled = false;
				this.Btn.target.spriteName = "tongyongBtn2_disable";
				NGUITools.SetActive(this.Btn, true);
				NGUITools.SetActive(this.HaveGet, false);
				break;
			}
		}
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			if (null != value)
			{
				UIDragPanelContents uidragPanelContents = base.GetComponent<UIDragPanelContents>();
				if (null == uidragPanelContents)
				{
					uidragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
				}
				uidragPanelContents.draggablePanel = value;
			}
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	public TextBlock[] RenWuMiaoShu;

	public UISprite[] AwardSp;

	public UILabel[] AwardLabel;

	public GButton Btn;

	public GameObject HaveGet;

	public GameObject RootRenWu;

	public GameObject RootLianMeng;

	public ShowNetImage Face;

	public UILabel ZhiWu;

	public UILabel[] BHInF;

	public GButton[] Btns;

	private ArmyGroupItem.AwardStateOrGetRenWu mAwardType = ArmyGroupItem.AwardStateOrGetRenWu.Go;

	private int mID;

	private ArmyGroupRenWuPart.JunTuanTaskDataXml mTaskData;

	private ArmyGroupItem.AwryGroupItemType mAwryGroupItemType;

	private JunTuanBangHuiData mJunTuanBangHuiData;

	private int mTaskValue;

	public DPSelectedItemEventHandler Hander;

	public enum AwardStateOrGetRenWu
	{
		HaveGet,
		CanGet,
		TimeD,
		Go
	}

	public enum AwryGroupItemType
	{
		RenWu,
		LianMeng,
		LianMeng1
	}
}
