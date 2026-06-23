using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class SkillSelectPart : UserControl
{
	protected override void InitializeComponent()
	{
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject go)
		{
			this.OnClose();
		};
		this.ItemCollection = this._ListBox.ItemsSource;
		if (null == this._UIAnchor)
		{
			this._UIAnchor = base.GetComponent<UIAnchor>();
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				GameObject at = this._ListBox.ItemsSource.GetAt(this._ListBox.ItemsSource.Count - 1);
				if (null != at)
				{
					SystemHelpPart.SetMaskEx(at.transform, Vector4.zero, 2);
				}
				else
				{
					SystemHelpPart.HideMask();
				}
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public override void Destroy()
	{
		this.ItemCollection.Clear();
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void RefreshSkillItems(int index, bool isShenShi = false)
	{
		this.ItemCollection.Clear();
		if (Global.Data.roleData.SkillDataList == null)
		{
			if (index != 4 || 0 >= Global.GetRoleHorseSkillData(Global.Data.RoleID).Count)
			{
				return;
			}
		}
		MJSSkillType mjstype = Global.GetMJSType();
		int[] callMagicByOccupation = Global.GetCallMagicByOccupation(Global.Data.roleData.Occupation);
		List<MagicInfoVO> skillListByOccupation = Global.GetSkillListByOccupation(Global.Data.roleData.Occupation);
		for (int i = 0; i < skillListByOccupation.Count; i++)
		{
			MagicInfoVO magicInfoVO = skillListByOccupation[i];
			int skillID = magicInfoVO.ID;
			bool flag = false;
			if (isShenShi)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("FuWenMagic", ',');
				int j = 0;
				int num = systemParamIntArrayByName.Length;
				while (j < num)
				{
					if (skillID == systemParamIntArrayByName[j])
					{
						flag = true;
						break;
					}
					j++;
				}
			}
			if (!isShenShi || flag)
			{
				List<HorseSkillData> roleHorseSkillData = Global.GetRoleHorseSkillData(Global.Data.RoleID);
				if (4 > index)
				{
					int magicIcon = magicInfoVO.MagicIcon;
					int actionIndex = magicInfoVO.ActionIndex;
					if (actionIndex >= 1000)
					{
						goto IL_3D9;
					}
					int parentMagicID = magicInfoVO.ParentMagicID;
					if (parentMagicID > 0)
					{
						goto IL_3D9;
					}
					int damageType = magicInfoVO.DamageType;
					if (Global.Data.roleData.Occupation == 3)
					{
						if (mjstype == MJSSkillType.Strength_Sword)
						{
							if ((1 & damageType) == 0 && damageType != -1)
							{
								goto IL_3D9;
							}
						}
						else
						{
							if ((2 & damageType) == 0 && damageType != -1)
							{
								goto IL_3D9;
							}
							if (this.isSkillPriorityModel && Global.GetBaseSkillID(Global.Data.roleData.Occupation) == skillID)
							{
								goto IL_3D9;
							}
						}
					}
					if (this.isSkillPriorityModel && skillID == callMagicByOccupation[0])
					{
						goto IL_3D9;
					}
					if (roleHorseSkillData != null && roleHorseSkillData.Find((HorseSkillData e) => e.SkillID == skillID) != null)
					{
						goto IL_3D9;
					}
				}
				else
				{
					if (roleHorseSkillData == null)
					{
						goto IL_3D9;
					}
					if (roleHorseSkillData.Find((HorseSkillData e) => e.SkillID == skillID) == null)
					{
						goto IL_3D9;
					}
				}
				if (!this.isGuaJiModel || Global.IsAutoFightSkill(skillID))
				{
					if (!this.isSkillPriorityModel || Global.GetBaseSkillID(Global.Data.roleData.Occupation) != skillID)
					{
						SkillData skillDataByID = Global.GetSkillDataByID(skillID);
						if (skillDataByID != null)
						{
							GSkillIcon gskillIcon = U3DUtils.NEW<GSkillIcon>();
							gskillIcon.Width = 64.0;
							gskillIcon.Height = 64.0;
							gskillIcon.InnerWidth = 60.0;
							gskillIcon.InnerHeight = 60.0;
							gskillIcon.OuterWidth = 66.0;
							gskillIcon.OuterHeight = 66.0;
							gskillIcon.NameSize = 16;
							gskillIcon.Tag = skillDataByID;
							gskillIcon.Name = ConfigMagicInfos.GetSkillNameByID(skillID);
							if (skillDataByID.SkillID < 12000)
							{
								gskillIcon.Level = skillDataByID.SkillLevel + Global.getSkillAddPoin(skillDataByID.SkillID);
							}
							else
							{
								gskillIcon.Level = skillDataByID.SkillLevel;
							}
							gskillIcon.BodyURL = Global.GetSkillIconString(ConfigMagicInfos.GetSkillIconIDByID(skillID));
							gskillIcon.StillIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
							{
								this.SelectSkillID = skillID;
								this.OnClose();
							};
							if (this.SelectSkillID == skillDataByID.SkillID)
							{
								gskillIcon.SelectState = true;
							}
							this.ItemCollection.AddNoUpdate(gskillIcon);
						}
					}
				}
			}
			IL_3D9:;
		}
		int num2 = 12;
		if (num2 <= 4)
		{
			this._Bak.transform.localScale = new Vector3(378f, 98f, 0f);
		}
		else
		{
			int num3 = (num2 - 1) / 4 + 1;
			this._Bak.transform.localScale = new Vector3(364f, (float)(38 + 76 * num3), 0f);
		}
		if (this.Index >= 0)
		{
			this._UIAnchor.relativeOffset = new Vector2(-0.35f, -0.02f);
			this._UIAnchor.side = 5;
		}
		else
		{
			this._UIAnchor.relativeOffset = new Vector2(0f, 0f);
			this._UIAnchor.side = 8;
		}
		this.ItemCollection.DelayUpdate();
		if (this._ListBox.transform.childCount <= 0)
		{
			Super.HintMainText(Global.GetLang("暂无技能"), 10, 3);
		}
	}

	public void InitPartData(int index, int skillID, DPSelectedItemEventHandler selectSkillEndHandler, bool isShenShi = false)
	{
		this.Index = index;
		this.DPSelectedItem = selectSkillEndHandler;
		this.SelectSkillID = skillID;
		this.RefreshSkillItems(index, isShenShi);
		SystemHelpMgr.OnAction(UIObjIDs.SkillSelectPart, HelpStateEvents.Actived, 1);
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this);
	}

	public void GetNewData(bool onlyOnce = false)
	{
	}

	private void OnClose()
	{
		SystemHelpMgr.OnAction(UIObjIDs.SkillSelectPart, HelpStateEvents.Inactived, 1);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = this.SelectSkillID,
				Index = this.Index
			});
		}
		SkillSelectPart.GClose();
	}

	public static SkillSelectPart GGetInstance()
	{
		if (null == SkillSelectPart._Instance)
		{
			SkillSelectPart._Instance = U3DUtils.NEW<SkillSelectPart>();
		}
		return SkillSelectPart._Instance;
	}

	public static void GClose()
	{
		if (null != SkillSelectPart._Instance)
		{
			Object.Destroy(SkillSelectPart._Instance.gameObject);
			SkillSelectPart._Instance = null;
		}
	}

	public static SkillSelectPart GShow()
	{
		if (null == SkillSelectPart._Instance)
		{
			SkillSelectPart.GGetInstance();
		}
		else if (SkillSelectPart._Instance.DPSelectedItem != null)
		{
			SkillSelectPart._Instance.DPSelectedItem(SkillSelectPart._Instance, new DPSelectedItemEventArgs
			{
				ID = SkillSelectPart._Instance.SelectSkillID,
				Index = SkillSelectPart._Instance.Index
			});
		}
		if (null != SkillSelectPart._Instance)
		{
			SkillSelectPart._Instance.gameObject.SetActive(true);
		}
		return SkillSelectPart._Instance;
	}

	public static void GHide()
	{
		if (null != SkillSelectPart._Instance)
		{
			SkillSelectPart._Instance.gameObject.SetActive(false);
		}
	}

	public SkillSelectPart Show()
	{
		if (null != SkillSelectPart._Instance)
		{
			base.gameObject.SetActive(true);
		}
		return this;
	}

	public void Hide()
	{
		this.DPSelectedItem = null;
		base.gameObject.SetActive(false);
	}

	public static SkillSelectPart _Instance;

	public ListBox _ListBox;

	public UISprite _Bak;

	public UISprite _BakDir;

	public UIAnchor _UIAnchor;

	public bool isGuaJiModel;

	public bool isSkillPriorityModel;

	private int Index = -1;

	private int SelectSkillID = -1;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection _ItemCollection;

	public delegate void SelectSkillEndHandler(int index, int skillID);
}
