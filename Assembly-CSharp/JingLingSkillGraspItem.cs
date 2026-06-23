using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class JingLingSkillGraspItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
		this.m_SignData = U3DUtils.NEW<jinglingSkillSignItem>();
		this.m_SignData.SetSignType(jinglingSkillSignItem.SignType.SkillGrasp, false);
		this.m_SignData.SlotIndex = 0;
		this.m_SignData.IsOpen = false;
		this.m_SignData.Hander = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (this.m_SignData.IsOpen)
			{
				if (this.m_SignData.SkillId != 0)
				{
					this.EffectActive = false;
					GChildWindow w = U3DUtils.NEW<GChildWindow>();
					w.ModalType = ChildWindowModalType.TransBak;
					w.ModalBakSprite.transform.localScale = new Vector3(2000f, 2000f, 1f);
					w.transform.localPosition = new Vector3(-50f, -70f, -800f);
					w.transform.SetParent(base.transform, false);
					JingLingSkillTips jingLingSkillTips = U3DUtils.NEW<JingLingSkillTips>();
					jingLingSkillTips.SetSkillId(this.m_SignData.SkillId, this.m_SignData.Lev);
					w.ChildWindowModalBakClick = delegate(object we, EventArgs fg)
					{
						Object.Destroy(w.gameObject, 0.01f);
						this.EffectActive = false;
						return true;
					};
					w.Body.Add(jingLingSkillTips);
					if (this.Selecehandler != null)
					{
						this.Selecehandler(this);
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("技能槽位没有技能"), 10, 3);
				}
			}
			else
			{
				Super.HintMainText(string.Format(Global.GetLang("槽位开启需要精灵等级达到{0}级！"), this.GetJingLingSlotOpenLev(this.m_SignData.SlotIndex)), 10, 3);
			}
		};
		this.m_SignData.transform.SetParent(this.m_SkillRoot, false);
	}

	private int GetJingLingSlotOpenLev(int slotIidnex)
	{
		if (this.m_DicSlotOpenLev.Count == 0)
		{
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("PatSkillCostLevel", '|');
			byte b = 0;
			while ((int)b < systemParamStringArrayByName.Length)
			{
				string[] array = systemParamStringArrayByName[(int)b].Split(new char[]
				{
					','
				});
				this.m_DicSlotOpenLev.Add((int)b, Convert.ToInt32(array[1]));
				b += 1;
			}
		}
		if (this.m_DicSlotOpenLev.ContainsKey(slotIidnex))
		{
			return this.m_DicSlotOpenLev[slotIidnex];
		}
		return 0;
	}

	public override void Update()
	{
		base.Update();
		if (0f <= this.m_TeXiaoLifeTime)
		{
			this.m_TeXiaoLifeTime -= Time.deltaTime;
		}
		else
		{
			this.TeXiao_UpSkillActive = false;
		}
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.m_CheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.CheckBoxhandler != null)
			{
				if (this.m_SignData.IsOpen && this.m_SignData.SkillId != 0)
				{
					this.CheckBoxhandler(s);
				}
				else
				{
					this.IsLock = false;
					if (this.m_SignData.SkillId == 0 && this.m_SignData.IsOpen)
					{
						Super.HintMainText(Global.GetLang("槽位没有技能"), 10, 3);
					}
					else
					{
						Super.HintMainText(string.Format(Global.GetLang("槽位开启需要精灵等级达到{0}级！"), this.GetJingLingSlotOpenLev(this.m_SignData.SlotIndex)), 10, 3);
					}
				}
			}
		};
	}

	public bool IsLock
	{
		get
		{
			return !this.m_CheckBox.isChecked;
		}
		set
		{
			this.m_CheckBox.isChecked = !value;
		}
	}

	public bool EffectActive
	{
		get
		{
			return this.m_SignData.SelectEffectActive;
		}
		set
		{
			this.m_SignData.SelectEffectActive = value;
		}
	}

	public int SlotID
	{
		set
		{
			this.m_SignData.SlotIndex = value;
		}
	}

	public bool TeXiao_NewActive
	{
		get
		{
			return NGUITools.GetActive(this.m_TeXiao_New_Root.gameObject);
		}
		set
		{
			if (value)
			{
				if (0 >= this.m_TeXiao_New_Root.childCount)
				{
					GameObject gameObject = Global.LoadTeXiaoObj("UITeXiao/Perfabs/chongwujineng/chongwu_new", this.m_TeXiao_New_Root);
					gameObject.transform.localPosition = Vector3.one;
				}
				NGUITools.SetActive(this.m_TeXiao_New_Root, true);
			}
			else
			{
				NGUITools.SetActive(this.m_TeXiao_New_Root, false);
			}
		}
	}

	public bool TeXiao_UpSkillActive
	{
		get
		{
			if (NGUITools.GetActive(this.m_TeXiao_UpSkill_Root.gameObject))
			{
				Animation[] componentsInChildren = this.m_TeXiao_UpSkill_Root.GetComponentsInChildren<Animation>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (null != componentsInChildren[i] && componentsInChildren[i].isPlaying)
					{
						return true;
					}
				}
			}
			return false;
		}
		set
		{
			if (value)
			{
				this.m_TeXiaoLifeTime = 3f;
				if (0 >= this.m_TeXiao_UpSkill_Root.childCount)
				{
					GameObject gameObject = Global.LoadTeXiaoObj("UITeXiao/Perfabs/chongwujineng/chongwu_lingwujineng", this.m_TeXiao_UpSkill_Root);
					gameObject.transform.localPosition = Vector3.one;
				}
				NGUITools.SetActive(this.m_TeXiao_UpSkill_Root, true);
			}
			else
			{
				this.m_TeXiaoLifeTime = -1f;
				NGUITools.SetActive(this.m_TeXiao_UpSkill_Root, false);
			}
		}
	}

	public int Lev
	{
		get
		{
			return this.m_SignData.Lev;
		}
		set
		{
			this.m_SignData.Lev = value;
		}
	}

	public int SkillID
	{
		get
		{
			return this.m_SignData.SkillId;
		}
		set
		{
			this.m_SignData.SkillId = value;
			if (this.m_SignData.IsOpen)
			{
				if (0 < this.m_SignData.SkillId)
				{
					this.m_NameLabel.text = Global.GetJingLinfSkillName(this.SkillID, true);
				}
				else
				{
					this.m_NameLabel.text = string.Empty;
				}
			}
		}
	}

	public bool SkillIsOpen
	{
		get
		{
			return this.m_SignData.IsOpen;
		}
		set
		{
			this.m_SignData.IsOpen = value;
			if (!this.m_SignData.IsOpen)
			{
				this.m_NameLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"808081",
					Global.GetLang("未开启")
				});
				if (Global.GetRolePatHaveJingling())
				{
					this.m_Conditionlabel.text = string.Format(Global.GetLang("达到Lv{0}开启！"), this.GetJingLingSlotOpenLev(this.m_SignData.SlotIndex));
				}
				else
				{
					this.m_Conditionlabel.text = string.Empty;
				}
			}
			else
			{
				this.m_Conditionlabel.text = string.Empty;
			}
		}
	}

	public object CheckBoxobject
	{
		get
		{
			return this.m_CheckBox;
		}
	}

	public bool CheckBoxActive
	{
		get
		{
			return NGUITools.GetActive(this.m_CheckBox.gameObject);
		}
		set
		{
			NGUITools.SetActive(this.m_CheckBox, value);
		}
	}

	public UILabel m_NameLabel;

	public GCheckBox m_CheckBox;

	public Transform m_TeXiao_New_Root;

	public Transform m_TeXiao_UpSkill_Root;

	public UILabel m_Conditionlabel;

	public Transform m_SkillRoot;

	private jinglingSkillSignItem m_SignData;

	private float m_TeXiaoLifeTime = 3f;

	private Dictionary<int, int> m_DicSlotOpenLev = new Dictionary<int, int>();

	public ObjectEventHandler CheckBoxhandler;

	public ObjectEventHandler Selecehandler;
}
