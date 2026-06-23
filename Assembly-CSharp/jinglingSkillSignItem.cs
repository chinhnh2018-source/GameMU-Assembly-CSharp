using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class jinglingSkillSignItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
		this.collider = this.m_Colder.GetComponent<BoxCollider>();
		this.Active(this.m_SelectOnj, false);
	}

	private void Active(GameObject obj, bool bActive)
	{
		NGUITools.SetActive(obj, bActive);
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		UIEventListener.Get(this.m_Colder).onClick = delegate(GameObject s)
		{
			DPSelectedItemEventArgs dpselectedItemEventArgs = new DPSelectedItemEventArgs();
			if (this.m_SignType == jinglingSkillSignItem.SignType.jingLing)
			{
				dpselectedItemEventArgs.ID = ((!this.m_IsOpen) ? 1 : 0);
			}
			else if (this.m_SignType == jinglingSkillSignItem.SignType.SkillGrasp)
			{
				dpselectedItemEventArgs.ID = ((!this.m_IsOpen) ? 3 : 2);
			}
			else
			{
				dpselectedItemEventArgs.ID = ((!this.m_IsOpen) ? 5 : 4);
			}
			dpselectedItemEventArgs.MyID = this.m_SlotIndex;
			this.Hander(s, dpselectedItemEventArgs);
		};
	}

	public void SetSignType(jinglingSkillSignItem.SignType type, bool bRefreshData = false)
	{
		this.m_SignType = type;
		if (bRefreshData)
		{
			this.SlotIndex = this.m_SlotIndex;
			this.IsOpen = this.m_IsOpen;
			this.SkillId = this.m_SkillId;
		}
	}

	public void ClearData()
	{
		this.m_IsOpen = false;
		this.m_SkillId = 0;
		this.m_Step = -1;
		this.m_Lev = 0;
	}

	public bool SelectEffectActive
	{
		get
		{
			return this.m_SelectEffectActive;
		}
		set
		{
			this.m_SelectEffectActive = value;
			if (this.m_SignType == jinglingSkillSignItem.SignType.SkillUp)
			{
				this.Active(this.m_SelectOnj, false);
			}
			else
			{
				this.Active(this.m_SelectOnj, this.m_SelectEffectActive);
			}
			if (!this.m_IsOpen)
			{
				this.Active(this.m_SelectOnj, false);
			}
		}
	}

	public int SkillId
	{
		get
		{
			return this.m_SkillId;
		}
		set
		{
			this.m_SkillId = value;
			if (this.m_SignType == jinglingSkillSignItem.SignType.SkillUp)
			{
				this.m_Sign.transform.localScale = new Vector3(64f, 64f, 1f);
				this.m_SelectOnj.transform.localScale = new Vector3(72f, 72f, 1f);
				this.m_BgSp.transform.localScale = new Vector3(86f, 90f, 1f);
				this.m_StepSp.transform.localScale = new Vector3(72f, 72f, 1f);
			}
			else if (this.m_SignType == jinglingSkillSignItem.SignType.jingLing)
			{
				this.m_Sign.transform.localScale = new Vector3(48f, 48f, 1f);
				this.m_SelectOnj.transform.localScale = new Vector3(58f, 58f, 1f);
				this.m_BgSp.transform.localScale = new Vector3(58f, 58f, 1f);
				this.m_StepSp.transform.localScale = new Vector3(58f, 58f, 1f);
			}
			else if (this.m_SignType == jinglingSkillSignItem.SignType.SkillGrasp)
			{
				this.m_Sign.transform.localScale = new Vector3(64f, 64f, 1f);
				this.m_SelectOnj.transform.localScale = new Vector3(72f, 72f, 1f);
				this.m_BgSp.transform.localScale = new Vector3(72f, 72f, 1f);
				this.m_StepSp.transform.localScale = new Vector3(72f, 72f, 1f);
			}
			if (0 >= this.m_SkillId)
			{
				this.Active(this.m_Sign.gameObject, false);
				this.SkillStep = -1;
				this.Active(this.m_Sign.gameObject, false);
			}
			else
			{
				this.Active(this.m_Sign.gameObject, true);
				this.SkillStep = Global.GetSkillMagicColor(this.m_SkillId);
				string jingLingSkillSignURL = Global.GetJingLingSkillSignURL(this.m_SkillId);
				if ("NoImage" != jingLingSkillSignURL)
				{
					this.Active(this.m_Sign.gameObject, true);
					this.m_Sign.URL = jingLingSkillSignURL;
				}
				if (!this.m_IsOpen)
				{
					this.Active(this.m_Sign.gameObject, false);
				}
			}
		}
	}

	public int SlotIndex
	{
		get
		{
			return this.m_SlotIndex;
		}
		set
		{
			this.m_SlotIndex = value;
		}
	}

	public bool IsOpen
	{
		get
		{
			return this.m_IsOpen;
		}
		set
		{
			this.m_IsOpen = value;
			if (this.m_SignType == jinglingSkillSignItem.SignType.SkillUp)
			{
				this.m_BgSp.spriteName = (this.m_IsOpen ? "waikuang2" : "waikuang1");
				this.Active(this.m_BgSp.gameObject, true);
				this.m_BgSp.transform.localScale = new Vector3(78f, 78f, 1f);
			}
			else
			{
				if (this.m_SignType == jinglingSkillSignItem.SignType.jingLing)
				{
					base.transform.localPosition = this.m_SkillsPos_jingling[this.m_SlotIndex];
					this.m_BgSp.transform.localScale = new Vector3(72f, 72f, 1f);
				}
				else if (this.m_SignType == jinglingSkillSignItem.SignType.SkillGrasp)
				{
					this.m_BgSp.transform.localScale = new Vector3(72f, 72f, 1f);
				}
				if (this.m_IsOpen)
				{
					this.m_BgSp.spriteName = "Bag";
					this.Active(this.m_BgSp.gameObject, true);
				}
				else
				{
					this.m_BgSp.spriteName = "NotOpen";
					this.Active(this.m_BgSp.gameObject, true);
				}
			}
			if (!this.m_IsOpen)
			{
				this.Active(this.m_SelectOnj, this.m_IsOpen);
				this.Active(this.m_StepSp.gameObject, this.m_IsOpen);
			}
			this.Active(this.m_LevLanel.gameObject, this.m_IsOpen);
			Global.RefreshUI(this.m_BgSp.gameObject);
		}
	}

	public int Lev
	{
		get
		{
			return this.m_Lev;
		}
		set
		{
			if (0 <= this.m_SkillId)
			{
				this.m_Lev = ((1 < value) ? value : 1);
			}
			else
			{
				this.m_Lev = value;
			}
			if (this.m_SignType != jinglingSkillSignItem.SignType.jingLing)
			{
				this.m_LevLanel.transform.localPosition = new Vector3(32f, 14.52f, -2f);
			}
			else
			{
				this.m_LevLanel.transform.localPosition = new Vector3(27f, 9.28f, -1f);
			}
			this.Active(this.m_LevLanel.gameObject, true);
			if (this.m_IsOpen)
			{
				this.m_LevLanel.text = ((0 >= this.m_Lev) ? string.Empty : string.Format("Lv{0}", this.m_Lev));
			}
			else
			{
				this.m_LevLanel.text = string.Empty;
			}
			if (!this.m_IsOpen)
			{
				this.Active(this.m_LevLanel.gameObject, false);
			}
		}
	}

	public int Collider_z
	{
		set
		{
			this.collider.center = new Vector3(0f, 0f, (float)((value < -500) ? value : -501));
		}
	}

	public int SkillStep
	{
		get
		{
			return this.m_Step;
		}
		set
		{
			this.m_Step = value;
			if (this.m_Step == 1)
			{
				this.Active(this.m_StepSp.gameObject, true);
				this.m_StepSp.spriteName = "iconState_zuoyue";
			}
			else if (this.m_Step == 2)
			{
				this.Active(this.m_StepSp.gameObject, true);
				this.m_StepSp.spriteName = "iconState_zuoyue1";
			}
			else if (this.m_Step == 3)
			{
				this.Active(this.m_StepSp.gameObject, true);
				this.m_StepSp.spriteName = "iconState_zuoyue2";
			}
			else
			{
				this.Active(this.m_StepSp.gameObject, true);
				this.Active(this.m_StepSp.gameObject, false);
			}
			if (!this.m_IsOpen)
			{
				this.Active(this.m_StepSp.gameObject, false);
			}
		}
	}

	public ShowNetImage m_Sign;

	public UISprite m_StepSp;

	public UISprite m_BgSp;

	public UILabel m_LevLanel;

	public GameObject m_SelectOnj;

	public GameObject m_Colder;

	private Vector3[] m_SkillsPos_jingling = new Vector3[]
	{
		new Vector3(-110f, 0f, 0f),
		new Vector3(-36.5f, 0f, 0f),
		new Vector3(36.5f, 0f, 0f),
		new Vector3(110f, 0f, 0f)
	};

	private Vector3[] m_SkillsPos_Skills = new Vector3[]
	{
		new Vector3(-85.3f, 2.7f, -1f),
		new Vector3(154.7f, 4.2f, -1f),
		new Vector3(-81.3f, -243.1f, -1f),
		new Vector3(157.5f, -239.9f, -1f)
	};

	private int m_SkillId = -1;

	private int m_SlotIndex = -1;

	private int m_Step = -1;

	private bool m_IsOpen;

	private bool m_SelectEffectActive;

	private int m_Lev;

	private jinglingSkillSignItem.SignType m_SignType = jinglingSkillSignItem.SignType.SkillGrasp;

	private BoxCollider collider;

	public DPSelectedItemEventHandler Hander;

	public enum SignType
	{
		SkillUp,
		SkillGrasp,
		jingLing
	}
}
