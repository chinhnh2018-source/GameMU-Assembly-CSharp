using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class CangBaoMiJingHintPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
		this.m_N_Price = (int)ConfigSystemParam.GetSystemParamIntByName("TreasurePrice");
		this.m_M_Price = (int)ConfigSystemParam.GetSystemParamIntByName("TreasureSuperPrice");
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	protected override void OnDestroy()
	{
		if (this.messageBoxWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
		}
		base.OnDestroy();
	}

	private void InitHandler()
	{
		UIEventListener.Get(this.m_Mask.gameObject).onClick = delegate(GameObject e)
		{
			this.handler(e, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.m_DiceBtns[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.handler(e, new DPSelectedItemEventArgs
			{
				ID = 3,
				MyID = 1
			});
		};
		this.m_DiceBtns[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.handler(e, new DPSelectedItemEventArgs
			{
				ID = 3,
				MyID = 2
			});
		};
		this.m_DiceBtns[2].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.handler(e, new DPSelectedItemEventArgs
			{
				ID = 3,
				MyID = 3
			});
		};
		this.m_DiceBtns[3].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.handler(e, new DPSelectedItemEventArgs
			{
				ID = 3,
				MyID = 4
			});
		};
		this.m_DiceBtns[4].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.handler(e, new DPSelectedItemEventArgs
			{
				ID = 3,
				MyID = 5
			});
		};
		this.m_DiceBtns[5].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.handler(e, new DPSelectedItemEventArgs
			{
				ID = 3,
				MyID = 6
			});
		};
		for (int i = 0; i < this.m_CloseBtn.Length; i++)
		{
			this.m_CloseBtn[i].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.handler(e, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			};
		}
		this.m_SureOrCancelBtn[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.m_InputValue = Convert.ToInt32(this.m_InputText.transform.GetChild(1).GetComponent<UILabel>().text);
			if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_N_Dice)
			{
				if (Global.Data.roleData.UserMoney < this.m_N_Price * this.m_InputValue)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
				}
				else
				{
					if (this.messageBoxWindow != null)
					{
						Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
					}
					this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("需要消耗{0}钻石，确定吗？"), this.m_N_Price * this.m_InputValue)
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							GameInstance.Game.SendOnePieceBuyDICE(DiceType.EDT_Normal, this.m_InputValue);
							this.handler(e, new DPSelectedItemEventArgs
							{
								ID = 1,
								MyID = 0
							});
						}
						return true;
					};
				}
				return;
			}
			if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_M_Dice)
			{
				if (Global.Data.roleData.UserMoney >= this.m_M_Price * this.m_InputValue)
				{
					if (this.messageBoxWindow != null)
					{
						Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
					}
					this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("需要消耗{0}钻石，确定吗？"), this.m_M_Price * this.m_InputValue)
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							GameInstance.Game.SendOnePieceBuyDICE(DiceType.EDT_MIRACLE, this.m_InputValue);
							this.handler(e, new DPSelectedItemEventArgs
							{
								ID = 1,
								MyID = 0
							});
						}
						return true;
					};
					return;
				}
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			}
			this.handler(e, new DPSelectedItemEventArgs
			{
				ID = 1,
				MyID = 0
			});
		};
		this.m_SureOrCancelBtn[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.handler(e, new DPSelectedItemEventArgs
			{
				ID = 1,
				MyID = 1
			});
		};
		this.m_InputBtn[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_InputValue > 1)
			{
				this.m_InputValue--;
				this.m_InputText.transform.GetChild(1).GetComponent<UILabel>().text = this.m_InputValue.ToString();
				int num = 0;
				if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_M_Dice)
				{
					num = this.m_M_Price * this.m_InputValue;
				}
				else if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_N_Dice)
				{
					num = this.m_N_Price * this.m_InputValue;
				}
				this.m_BuyDiceNumLabels[2].text = num.ToString();
			}
		};
		this.m_InputBtn[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_InputValue < this.m_RoleMaxHaveNum - this.m_RoleHaveNum)
			{
				this.m_InputValue++;
				this.m_InputText.transform.GetChild(1).GetComponent<UILabel>().text = this.m_InputValue.ToString();
				int num = 0;
				if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_M_Dice)
				{
					num = this.m_M_Price * this.m_InputValue;
				}
				else if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_N_Dice)
				{
					num = this.m_N_Price * this.m_InputValue;
				}
				this.m_BuyDiceNumLabels[2].text = num.ToString();
			}
			else
			{
				Super.HintMainText(Global.GetLang("骰子数达到上限"), 10, 3);
			}
		};
		this.m_InputBtn[1].OnPress = delegate(GameObject e, bool b)
		{
			this.m_AddNumber = 1;
			if (b)
			{
				base.InvokeRepeating("OnPressBtn_CBAdd", 0.4f, 0.1f);
			}
			else
			{
				base.CancelInvoke("OnPressBtn_CBAdd");
			}
		};
		this.m_InputBtn[0].OnPress = delegate(GameObject e, bool b)
		{
			this.m_AddNumber = 1;
			if (b)
			{
				base.InvokeRepeating("OnPressBtn_CBSub", 0.4f, 0.1f);
			}
			else
			{
				base.CancelInvoke("OnPressBtn_CBSub");
			}
		};
		UIEventListener.Get(this.m_InputText).onClick = delegate(GameObject e)
		{
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(delegate(object es, DPSelectedItemEventArgs s)
			{
				this.m_InputValue = s.ID;
				if (this.m_InputValue > this.m_RoleMaxHaveNum - this.m_RoleHaveNum)
				{
					this.m_InputValue = this.m_RoleMaxHaveNum - this.m_RoleHaveNum;
				}
				int num = 0;
				if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_M_Dice)
				{
					num = this.m_M_Price * this.m_InputValue;
				}
				else if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_N_Dice)
				{
					num = this.m_N_Price * this.m_InputValue;
				}
				this.m_InputText.transform.GetChild(1).GetComponent<UILabel>().text = this.m_InputValue.ToString();
				this.m_BuyDiceNumLabels[2].text = num.ToString();
			}, this.m_InputText.transform.GetChild(1).GetComponent<UILabel>(), 0, -100);
		};
	}

	private void OnPressBtn_CBAdd()
	{
		int num = this.m_RoleMaxHaveNum - this.m_RoleHaveNum;
		int num2 = this.m_InputValue + this.m_AddNumber++;
		this.m_InputValue = ((this.m_InputValue < num) ? ((num2 <= num) ? num2 : num) : num);
		if (this.m_InputValue.ToString() != this.m_InputText.transform.GetChild(1).GetComponent<UILabel>().text)
		{
			this.m_InputText.transform.GetChild(1).GetComponent<UILabel>().text = this.m_InputValue.ToString();
			int num3 = 0;
			if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_M_Dice)
			{
				num3 = this.m_M_Price * this.m_InputValue;
			}
			else if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_N_Dice)
			{
				num3 = this.m_N_Price * this.m_InputValue;
			}
			this.m_BuyDiceNumLabels[2].text = num3.ToString();
		}
	}

	private void OnPressBtn_CBSub()
	{
		int num = this.m_InputValue - this.m_AddNumber++;
		this.m_InputValue = ((this.m_InputValue > 1) ? ((num >= 1) ? num : 1) : 1);
		if (this.m_InputValue.ToString() != this.m_InputText.transform.GetChild(1).GetComponent<UILabel>().text)
		{
			this.m_InputText.transform.GetChild(1).GetComponent<UILabel>().text = this.m_InputValue.ToString();
			int num2 = 0;
			if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_M_Dice)
			{
				num2 = this.m_M_Price * this.m_InputValue;
			}
			else if (this.m_Hinttype == CangBaoMiJingHintPart.HintType.Buy_N_Dice)
			{
				num2 = this.m_N_Price * this.m_InputValue;
			}
			this.m_BuyDiceNumLabels[2].text = num2.ToString();
		}
	}

	private void ChangeContent(int index)
	{
		if (index == 0)
		{
			this.m_Mask.enabled = true;
			NGUITools.SetActive(this.m_ContentObj[0], true);
			NGUITools.SetActive(this.m_ContentObj[1], false);
			this.m_BgSp.transform.localPosition = this.ScaleAndPos_HintType_Miracle[1];
			this.m_BgSp.transform.localScale = this.ScaleAndPos_HintType_Miracle[0];
			this.m_Miraclelabel.text = this.Str_HintType_Miracle[1];
		}
		else
		{
			this.m_Mask.enabled = false;
			NGUITools.SetActive(this.m_ContentObj[1], true);
			NGUITools.SetActive(this.m_ContentObj[0], false);
			this.m_BgSp.transform.localPosition = this.ScaleAndPos_HintType_BuyDice[1];
			this.m_BgSp.transform.localScale = this.ScaleAndPos_HintType_BuyDice[0];
			this.m_SureOrCancelBtn[0].Label.text = this.Str_HintType_BuyDice[6];
			this.m_SureOrCancelBtn[1].Label.text = this.Str_HintType_BuyDice[7];
			this.m_InputText.transform.GetChild(1).GetComponent<UILabel>().text = "1";
			this.m_BuyDiceLiftLabels[0].text = this.Str_HintType_BuyDice[2];
			this.m_BuyDiceLiftLabels[1].text = this.Str_HintType_BuyDice[3];
			this.m_BuyDiceLiftLabels[2].text = this.Str_HintType_BuyDice[4];
			this.m_BuyDiceLiftLabels[3].text = this.Str_HintType_BuyDice[5];
			this.m_BuyDiceNumLabels[0].text = string.Format("{0}/{1}", this.m_RoleHaveNum, this.m_RoleMaxHaveNum);
		}
	}

	public void RefreshContent(CangBaoMiJingHintPart.HintType type = CangBaoMiJingHintPart.HintType.Miracle, int roleHaveNum = 0)
	{
		this.m_Hinttype = type;
		this.m_RoleHaveNum = roleHaveNum;
		if (type == CangBaoMiJingHintPart.HintType.Miracle)
		{
			this.ChangeContent(0);
			this.m_Title[0].text = this.Str_HintType_Miracle[0];
		}
		else if (type == CangBaoMiJingHintPart.HintType.Buy_N_Dice)
		{
			this.ChangeContent(1);
			this.m_Title[1].text = this.Str_HintType_BuyDice[0];
			this.m_BuyDiceNumLabels[1].text = this.m_N_Price.ToString();
			this.m_BuyDiceNumLabels[2].text = (this.m_N_Price * Convert.ToInt32(this.m_InputText.transform.GetChild(1).GetComponent<UILabel>().text)).ToString();
		}
		else if (type == CangBaoMiJingHintPart.HintType.Buy_M_Dice)
		{
			this.ChangeContent(1);
			this.m_Title[1].text = this.Str_HintType_BuyDice[1];
			this.m_BuyDiceNumLabels[1].text = this.m_M_Price.ToString();
			this.m_BuyDiceNumLabels[2].text = (this.m_M_Price * Convert.ToInt32(this.m_InputText.transform.GetChild(1).GetComponent<UILabel>().text)).ToString();
		}
	}

	public UISprite m_Mask;

	public UISprite m_BgSp;

	public UILabel[] m_Title = new UILabel[2];

	public GButton[] m_DiceBtns = new GButton[6];

	public UILabel m_Miraclelabel;

	public UILabel[] m_BuyDiceLiftLabels = new UILabel[4];

	public UILabel[] m_BuyDiceNumLabels = new UILabel[3];

	public GameObject m_InputText;

	public GButton[] m_InputBtn = new GButton[2];

	public GButton[] m_SureOrCancelBtn = new GButton[2];

	public GameObject[] m_ContentObj = new GameObject[2];

	public GButton[] m_CloseBtn = new GButton[2];

	private string[] Str_HintType_Miracle = new string[]
	{
		Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("奇迹骰子")
		}),
		Global.GetLang("请选中需要投出的点数")
	};

	private Vector3[] ScaleAndPos_HintType_Miracle = new Vector3[]
	{
		new Vector3(325f, 259f, 1f),
		new Vector3(0f, 0f, 0f)
	};

	private string[] Str_HintType_BuyDice = new string[]
	{
		Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("购买普通骰子")
		}),
		Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("购买奇迹骰子")
		}),
		Global.GetColorStringForNGUIText(new object[]
		{
			"896f46",
			Global.GetLang("现有次数：")
		}),
		Global.GetColorStringForNGUIText(new object[]
		{
			"896f46",
			Global.GetLang("单        价：")
		}),
		Global.GetColorStringForNGUIText(new object[]
		{
			"896f46",
			Global.GetLang("购买数量：")
		}),
		Global.GetColorStringForNGUIText(new object[]
		{
			"896f46",
			Global.GetLang("消耗钻石：")
		}),
		Global.GetLang("确定"),
		Global.GetLang("取消")
	};

	private Vector3[] ScaleAndPos_HintType_BuyDice = new Vector3[]
	{
		new Vector3(375f, 293f, 1f),
		new Vector3(-100f, -24f, 0f)
	};

	private int m_InputValue = 1;

	private int m_RoleHaveNum;

	private int m_RoleMaxHaveNum = 99;

	private int m_M_Price;

	private int m_N_Price;

	private CangBaoMiJingHintPart.HintType m_Hinttype;

	private int m_AddNumber = 1;

	private GChildWindow messageBoxWindow;

	public DPSelectedItemEventHandler handler;

	public enum HintType
	{
		Miracle,
		Buy_N_Dice,
		Buy_M_Dice
	}
}
