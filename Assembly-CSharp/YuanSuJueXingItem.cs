using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class YuanSuJueXingItem : UserControl
{
	public int ItemCount
	{
		get
		{
			return this.m_ItemCount;
		}
		set
		{
			this.m_ItemCount = value;
		}
	}

	public override void Destroy()
	{
		base.StopCoroutine("TwreenStart");
		base.Destroy();
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_Handler(this, new DPSelectedItemEventArgs
			{
				Index = this.ItemCount
			});
		};
		this.m_BtnUpLevel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.OnItemClick)
			{
				this.m_Handler(this, new DPSelectedItemEventArgs
				{
					Index = this.ItemCount
				});
				return;
			}
			if (Global.Data.roleData.JingLingYuanSuJueXingData == null)
			{
				GameInstance.Game.SenYuanSuJueXingJiHuo((YuanSuJueXingType)this.m_Type);
				return;
			}
			if (this.m_Type == Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType)
			{
				this.m_HandlerClose(this, new DPSelectedItemEventArgs());
				PlayZone.GlobalPlayZone.OpenYuanSuJueXingUpLevelWindow();
				return;
			}
			GameInstance.Game.SenYuanSuJueXingJiHuo((YuanSuJueXingType)this.m_Type);
		};
	}

	public bool OnItemClick
	{
		get
		{
			return this.m_boolOnItem;
		}
		set
		{
			this.m_boolOnItem = value;
			base.StartCoroutine<bool>(this.TwreenStart(this.m_boolOnItem));
		}
	}

	private IEnumerator TwreenStart(bool flag)
	{
		yield return new WaitForSeconds(this.m_Duration / 2f);
		if (flag)
		{
			this.m_ShowBak.URL = "NetImages/GameRes/Images/YuanSuJueXing/item_press.png";
			this.m_TypeName.color = Color.white;
			this.m_Lv.color = Color.white;
			this.m_Content.color = Color.white;
			this.m_ShowLogo.URL = string.Format("NetImages/GameRes/Images/YuanSuJueXing/Logo_normal_{0}.png", this.m_Type);
			this.m_ShowImg.URL = string.Format("NetImages/GameRes/Images/YuanSuJueXing/Img_normal_{0}.png", this.m_Type);
			if (Global.Data.roleData.JingLingYuanSuJueXingData != null)
			{
				if (Global.Data.roleData.JingLingYuanSuJueXingData.ActiveType == this.m_Type)
				{
					this.m_BtnUpLevel.Text = Global.GetLang("查看");
				}
				else
				{
					this.m_BtnUpLevel.Text = Global.GetLang("激活");
				}
			}
			else
			{
				this.m_BtnUpLevel.Text = Global.GetLang("激活");
			}
		}
		else
		{
			this.m_ShowBak.URL = "NetImages/GameRes/Images/YuanSuJueXing/item_normal.png";
			this.m_TypeName.color = new Color(1f, 1f, 1f, 0.2f);
			this.m_Lv.color = new Color(1f, 1f, 1f, 0.2f);
			this.m_Content.color = new Color(1f, 1f, 1f, 0.2f);
			this.m_ShowLogo.URL = string.Format("NetImages/GameRes/Images/YuanSuJueXing/Logo_press_{0}.png", this.m_Type);
			this.m_ShowImg.URL = string.Format("NetImages/GameRes/Images/YuanSuJueXing/Img_press_{0}.png", this.m_Type);
		}
		this.m_BtnUpLevel.gameObject.SetActive(flag);
		yield break;
	}

	public float Duration
	{
		get
		{
			return this.m_Duration;
		}
		set
		{
			this.m_Duration = value;
			this.m_TweenPosition.duration = this.m_Duration;
			this.m_TweenRotation.duration = this.m_Duration;
			this.m_TweenScale.duration = this.m_Duration;
		}
	}

	public new string Name
	{
		set
		{
			this.m_TypeName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				value
			});
		}
	}

	public string LV
	{
		set
		{
			this.m_Lv.text = value;
		}
	}

	public string Content
	{
		set
		{
			this.m_Content.text = value;
			this.m_Content.lineWidth = 150;
			this.m_Content.pivot = 0;
			this.m_Content.transform.localPosition = new Vector3(-75f, -80f, 0f);
		}
	}

	public int Type
	{
		set
		{
			this.m_Type = value;
		}
	}

	public TweenPosition m_TweenPosition;

	public TweenRotation m_TweenRotation;

	public TweenScale m_TweenScale;

	public ShowNetImage m_ShowBak;

	public ShowNetImage m_ShowLogo;

	public ShowNetImage m_ShowImg;

	public UILabel m_Title;

	public UILabel m_TypeName;

	public UILabel m_Lv;

	public UILabel m_Content;

	public GButton m_BtnUpLevel;

	public DPSelectedItemEventHandler m_Handler;

	public DPSelectedItemEventHandler m_HandlerClose;

	public GButton m_Btn;

	private int m_ItemCount = -1;

	private int m_Type = -1;

	private float m_Duration;

	private bool m_boolOnItem;
}
