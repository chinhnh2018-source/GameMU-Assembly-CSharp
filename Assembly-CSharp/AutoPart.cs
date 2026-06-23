using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class AutoPart : UserControl
{
	private void ShowAutoSkillPriorityPart()
	{
		if (null == this.m_GChildWindow)
		{
			this.m_GChildWindow = U3DUtils.NEW<GChildWindow>();
			this.m_GChildWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_GChildWindow, "AutoSkillPriorityWindow");
			Super.GData.GlobalPlayZone.Children.Add(this.m_GChildWindow);
		}
		this.m_AutoSkillPriorityPart = U3DUtils.NEW<AutoSkillPriorityPart>();
		this.m_GChildWindow.Body.Add(this.m_AutoSkillPriorityPart);
		this.m_AutoSkillPriorityPart.InitPart(true, this.m_SkillPriorityDict);
		this.m_AutoSkillPriorityPart.CloseHander = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 0 && null != this.m_AutoSkillPriorityPart)
			{
				Object.Destroy(this.m_AutoSkillPriorityPart.gameObject);
				this.m_AutoSkillPriorityPart = null;
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.m_GChildWindow);
			}
		};
		this.m_AutoSkillPriorityPart.refreshSkillDictCallBack = new AutoSkillPriorityPart.RefreshSkillDictCallBack(this.SaveCurrentSkillPriorityToGlobal);
	}

	private void ResetCurrentSkillPriority(Dictionary<int, int> tmpDict)
	{
		Dictionary<int, int>.Enumerator enumerator = tmpDict.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Dictionary<int, int> skillPriorityDict = this.m_SkillPriorityDict;
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			int key = keyValuePair.Key;
			KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
			skillPriorityDict.Add(key, keyValuePair2.Value);
		}
	}

	private void SaveCurrentSkillPriorityToGlobal(Dictionary<int, int> tmpDict)
	{
		Global.ResetSkillPriorityDict(tmpDict);
		this.m_SkillPriorityDict = tmpDict;
	}

	protected override void InitializeComponent()
	{
		this._AutoRealive.Check = false;
		this._Title.Text = Global.GetLang("挂机设置");
		this._ShiQu_Title.Text = Global.GetLang("拾取设置");
		this._Color_Bai.Text = Global.GetLang("白色装备");
		this._Color_Lan.Text = Global.GetLang("蓝色装备");
		this._Color_Lv.Text = Global.GetLang("绿色装备");
		this._Color_Zi.Text = Global.GetLang("紫色装备");
		this._BaoShi.Text = Global.GetLang("宝石");
		this._YuMao.Text = Global.GetLang("羽毛");
		this._YaoPin.Text = Global.GetLang("药品");
		this._JinBi.Text = Global.GetLang("金币");
		this._MenPiaoCaiLiao.Text = Global.GetLang("门票材料");
		this._QiTaDaoJu.Text = Global.GetLang("其他道具");
		this._Area1.transform.localPosition = new Vector3(-430f, -127f, -0.02f);
		this._Area2.transform.localPosition = new Vector3(-288f, -127f, -0.02f);
		this._Area3.transform.localPosition = new Vector3(-132f, -127f, -0.02f);
		UIEventListener.Get(this._SkillSetting.gameObject).onClick = delegate(GameObject s)
		{
			this.MouseLeftButtonUp();
		};
		this._AutoZhaohuanShenshou.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			this.CopyAutoFightData.AutoZhaohuanShenshou = (sender as GCheckBox).Check;
		};
		this._AutoRealive.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			if ((sender as GCheckBox).Check)
			{
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang("该功能勾选后，频繁死亡可能导致消耗大量钻石，确定要使用该功能?"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.CopyAutoFightData.AutoRealive = (sender as GCheckBox).Check;
					}
					else if (messageBoxReturn == 1)
					{
						this._AutoRealive.Check = false;
					}
					return true;
				};
			}
			else
			{
				this.CopyAutoFightData.AutoRealive = (sender as GCheckBox).Check;
			}
		};
		this._AutoAntiAttack.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			this.CopyAutoFightData.AutoAntiAttack = (sender as GCheckBox).Check;
		};
		this._AutoBuyMedicine.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			this.CopyAutoFightData.AutoBuyMedicine = (sender as GCheckBox).Check;
		};
		this._DontAttackBigBoss.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			this.CopyAutoFightData.DontAttackBigBoss = (sender as GCheckBox).Check;
		};
		this._MagicLessThanXScroll.onChange = delegate(UIScrollBar sb)
		{
			this.CopyAutoFightData.MagicLessThanX = Mathf.RoundToInt(sb.scrollValue * 99f);
			this.MagicLessThanX.Text = this.CopyAutoFightData.MagicLessThanX.ToString() + "%";
		};
		this._LifeLessThanXScroll.onChange = delegate(UIScrollBar sb)
		{
			this.CopyAutoFightData.LifeLessThanX = Mathf.RoundToInt(sb.scrollValue * 99f);
			this.LifeLessThanX.Text = this.CopyAutoFightData.LifeLessThanX.ToString() + "%";
		};
		this._Start.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.SaveSettings();
			this.SwitchAutoFight();
		};
		this._PickSetting.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._ShiQuSheZhi.SetActive(true);
		};
		this._SkillPrioritySetting.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.ShowAutoSkillPriorityPart();
		};
		this._Close.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.SaveSettings();
			this.HideWindow();
		};
		this._Hide.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.UpdateData(true);
			this._ShiQuSheZhi.SetActive(false);
		};
	}

	public Brush BodyBackground
	{
		set
		{
		}
	}

	private void TextBox_TextChanged(object sender, object e)
	{
		TextBox textBox = sender as TextBox;
		if (null == textBox)
		{
			return;
		}
		int num = 1000;
		string xapParamByName = Super.GetXapParamByName("country", string.Empty);
		if ("korea" == xapParamByName)
		{
			num = 500;
		}
		int num2 = 0;
		try
		{
			num2 = Convert.ToInt32(textBox.Text);
			if (num2 >= num)
			{
				num2 = num;
			}
			if (num2 <= 1)
			{
				num2 = 1;
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
			num2 = 1;
		}
		textBox.Text = num2.ToString();
	}

	private void TextBox_TextChanged2(object sender, object e)
	{
		TextBox textBox = sender as TextBox;
		if (null == textBox)
		{
			return;
		}
		int num = 9999;
		string xapParamByName = Super.GetXapParamByName("country", string.Empty);
		if ("korea" == xapParamByName)
		{
			num = 500;
		}
		int num2 = 0;
		try
		{
			num2 = Convert.ToInt32(textBox.Text);
			if (num2 >= num)
			{
				num2 = num;
			}
			if (num2 <= 1)
			{
				num2 = 1;
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
			num2 = 1;
		}
		textBox.Text = num2.ToString();
	}

	private void SwitchAutoFight()
	{
		SystemHelpMgr.Reset();
		if (!Global.IsAutoFighting())
		{
			Global.Data.GameScene.StartAutoFight(-1);
		}
		else
		{
			Global.Data.GameScene.CancelAutoFight(0, true);
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 0
			});
		}
		SystemHelpMgr.OnAction(UIObjIDs.AutoPartStart, HelpStateEvents.Clicked, 1);
	}

	public void AutoFightNotify(int fighting)
	{
		this._Start.Text = ((!Global.IsAutoFighting()) ? Global.GetLang("开始挂机") : Global.GetLang("停止挂机"));
	}

	public void SaveSettings()
	{
		if (Global.IsAutoFighting())
		{
		}
		this.UpdateData(true);
		Global.Data.AutoFightData = this.CopyAutoFightData;
		Dictionary<int, int> skillPriorityDict = Global.GetSkillPriorityDict();
		if (skillPriorityDict != null && skillPriorityDict.Count > 0)
		{
			Global.SetSkillDictsVaue(Global.GetCurrentSkillPriorityKey(), skillPriorityDict);
			Global.isOpenSkillPriority = true;
		}
		else
		{
			Global.SetSkillDictsVaue(Global.GetCurrentSkillPriorityKey(), skillPriorityDict);
			Global.isOpenSkillPriority = false;
		}
		Global.Data.AutoFightData.SkillPriorityDict = Global.ConvertDictionaryToString(Global.GetSkillDicts());
		Global.SaveAutoFightSettings();
		Global.SaveSystemSettings();
		GameInstance.Game.SpriteModAutoDrink(this.CopyAutoFightData.LifeLessThanX, this.CopyAutoFightData.MagicLessThanX);
		this.m_SkillPriorityDict.Clear();
		this.m_SkillPriorityDict = null;
		if (Global.IsAutoFighting())
		{
			Global.InitSkillPriority(Global.Data.AutoFightData.SkillPriorityDict, false, false);
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

	private void OnTimeChanged(float percent)
	{
	}

	public void UpdateData(bool save = false)
	{
		if (save)
		{
			this.CopyAutoFightData.Color_Bai = this._Color_Bai.Check;
			this.CopyAutoFightData.Color_Lv = this._Color_Lv.Check;
			this.CopyAutoFightData.Color_Lan = this._Color_Lan.Check;
			this.CopyAutoFightData.Color_Zi = this._Color_Zi.Check;
			this.CopyAutoFightData.BaoShi = this._BaoShi.Check;
			this.CopyAutoFightData.YuMao = this._YuMao.Check;
			this.CopyAutoFightData.YaoPin = this._YaoPin.Check;
			this.CopyAutoFightData.JinBi = this._JinBi.Check;
			this.CopyAutoFightData.MenPiaoCaiLiao = this._MenPiaoCaiLiao.Check;
			this.CopyAutoFightData.QiTaDaoJu = this._QiTaDaoJu.Check;
			int autoGetThingsFlag = Global.GetAutoGetThingsFlag(this.CopyAutoFightData);
			GameInstance.Game.SpriteUpdateGetThingFlagCmd(autoGetThingsFlag);
			this.CopyAutoFightData.FightRadius = this.FightRadius;
		}
		else
		{
			Global.SetAutoGetThingsValus(this.CopyAutoFightData, this.CopyAutoFightData.AutoPickThingFlags);
			this._Color_Bai.Check = this.CopyAutoFightData.Color_Bai;
			this._Color_Lv.Check = this.CopyAutoFightData.Color_Lv;
			this._Color_Lan.Check = this.CopyAutoFightData.Color_Lan;
			this._Color_Zi.Check = this.CopyAutoFightData.Color_Zi;
			this._BaoShi.Check = this.CopyAutoFightData.BaoShi;
			this._YuMao.Check = this.CopyAutoFightData.YuMao;
			this._YaoPin.Check = this.CopyAutoFightData.YaoPin;
			this._JinBi.Check = this.CopyAutoFightData.JinBi;
			this._MenPiaoCaiLiao.Check = this.CopyAutoFightData.MenPiaoCaiLiao;
			this._QiTaDaoJu.Check = this.CopyAutoFightData.QiTaDaoJu;
			this.FightRadius = this.CopyAutoFightData.FightRadius;
			if (this.FightRadius <= 8)
			{
				this.CopyAutoFightData.FightRadius = 8;
				this._Area1.Check = true;
			}
			else if (this.FightRadius <= 15)
			{
				this.CopyAutoFightData.FightRadius = 15;
				this._Area2.Check = true;
			}
			else
			{
				this.CopyAutoFightData.FightRadius = 100;
				this._Area3.Check = true;
			}
		}
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
		SystemHelpMgr.OnAction(UIObjIDs.AutoPart, HelpStateEvents.Inactived, 1);
	}

	public void InitPartSize(int width, int height)
	{
		this.CopyAutoFightData = Global.CopyAutoFight(Global.Data.AutoFightData);
		this._AutoZhaohuanShenshou.Check = this.CopyAutoFightData.AutoZhaohuanShenshou;
		this._AutoRealive.Check = this.CopyAutoFightData.AutoRealive;
		this._AutoAntiAttack.Check = this.CopyAutoFightData.AutoAntiAttack;
		this._AutoBuyMedicine.Check = this.CopyAutoFightData.AutoBuyMedicine;
		this._DontAttackBigBoss.Check = this.CopyAutoFightData.DontAttackBigBoss;
		string xapParamByName = Super.GetXapParamByName("country", string.Empty);
		if ("korea" == xapParamByName)
		{
			this._AutoRealive.Visibility = false;
		}
		this.LifeLessThanX.Text = this.CopyAutoFightData.LifeLessThanX.ToString() + "%";
		this._LifeLessThanXScroll.scrollValue = (float)this.CopyAutoFightData.LifeLessThanX / 99f;
		this.MagicLessThanX.Text = this.CopyAutoFightData.MagicLessThanX.ToString() + "%";
		this._MagicLessThanXScroll.scrollValue = (float)this.CopyAutoFightData.MagicLessThanX / 99f;
		this._Area1.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.FightRadius = 8;
		};
		this._Area2.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.FightRadius = 15;
		};
		this._Area3.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.FightRadius = 100;
		};
		this._Start.Text = ((!Global.IsAutoFighting()) ? Global.GetLang("开始挂机") : Global.GetLang("停止挂机"));
		this._PickSetting.Text = Global.GetLang("拾取设置");
		this._SkillPrioritySetting.Text = Global.GetLang("技能设置");
		Global.InitSkillPriority(this.CopyAutoFightData.SkillPriorityDict, true, this.CopyAutoFightData.IsUseDefaultSkillList);
		Dictionary<int, int> skillDictsVaueByKey = Global.GetSkillDictsVaueByKey(Global.GetCurrentSkillPriorityKey());
		this.ResetCurrentSkillPriority(skillDictsVaueByKey);
		this.UpdateData(false);
		SystemHelpMgr.OnAction(UIObjIDs.AutoPart, HelpStateEvents.Actived, 1);
	}

	private void RefreshSkillIcon()
	{
		if (!Global.IsAutoFightSkill(this.CopyAutoFightData.SkillID))
		{
			this.CopyAutoFightData.SkillID = Global.GetAutoFightSkillID();
		}
		int skillIconIDByID = ConfigMagicInfos.GetSkillIconIDByID(this.CopyAutoFightData.SkillID);
		this._SkillSetting.URL = Global.GetSkillIconString(skillIconIDByID);
		Global.Data.GameScene.SetDefaultSkillID(this.CopyAutoFightData.SkillID);
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Container, noBorderWindow);
	}

	private void ProcessDropDownMenuClick(int menus_id, int menuItem_id)
	{
	}

	private void HideWindow()
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
		}
	}

	private void UserControl_MouseLeftButtonDown(MouseEvent e)
	{
		this.HideWindow();
	}

	public static void NotifyResult(int type, string message, params string[] param0)
	{
		switch (type)
		{
		case 0:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您被【{0}】击杀，请立刻回到游戏中复仇！"), new object[]
			{
				message
			}), 0, -1, -1, 0);
			break;
		case 1:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("离线挂机时间已达到最大，请您尽快领取收益！"), new object[0]), 0, -1, -1, 0);
			break;
		case 2:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("离线挂机经验已达到最大，请您尽快领取收益！"), new object[0]), 0, -1, -1, 0);
			break;
		case 3:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的背包已满，为防止您损失收益，请您尽快处理！"), new object[0]), 0, -1, -1, 0);
			break;
		case 4:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("使用离线挂机，可让您在离线时继续获得收益！"), new object[0]), 0, -1, -1, 0);
			break;
		}
	}

	private void MouseLeftButtonUp()
	{
		GChildWindow window = U3DUtils.NEW<GChildWindow>();
		Super.InitChildWindow(window, "Skill Select");
		this.Container.Children.Add(window);
		window.ModalType = ChildWindowModalType.TransBak;
		SkillSelectPart skillSelectPart = SkillSelectPart.GShow();
		skillSelectPart.isGuaJiModel = true;
		window.SetContent(window.BodyPresenter, skillSelectPart, 0.0, 0.0, true);
		skillSelectPart.InitPartData(-1, this.CopyAutoFightData.SkillID, delegate(object s, DPSelectedItemEventArgs e1)
		{
			Super.CloseChildWindow(this, window);
			if (e1 != null)
			{
				this.CopyAutoFightData.SkillID = e1.ID;
				this.RefreshSkillIcon();
			}
		}, false);
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this._Start, default(Vector4));
			}
			else if (id == 1)
			{
				SystemHelpPart.SetMask(this._Close, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	public const int FightRadiu0 = 8;

	public const int FightRadiu1 = 15;

	public const int FightRadiu2 = 100;

	private LocalAutoFightData CopyAutoFightData;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int[] MenuItemIDs = new int[]
	{
		default(int),
		1
	};

	private NoBorderWindow MenuWindow;

	public TextBlock LifeLessThanX;

	public TextBlock MagicLessThanX;

	public GCheckBox _Color_Bai;

	public GCheckBox _Color_Lan;

	public GCheckBox _Color_Lv;

	public GCheckBox _Color_Zi;

	public GCheckBox _BaoShi;

	public GCheckBox _YuMao;

	public GCheckBox _YaoPin;

	public GCheckBox _JinBi;

	public GCheckBox _MenPiaoCaiLiao;

	public GCheckBox _QiTaDaoJu;

	public GButton _Hide;

	public TextBlock _Title;

	public TextBlock _ShiQu_Title;

	public GameObject _ShiQuSheZhi;

	public GameObject _Main;

	public GCheckBox _AutoRealive;

	public GCheckBox _AutoZhaohuanShenshou;

	public GCheckBox _AutoAntiAttack;

	public GCheckBox _AutoBuyMedicine;

	public GCheckBox _DontAttackBigBoss;

	public GButton _Close;

	public GButton _Start;

	public GButton _PickSetting;

	public GButton _SkillPrioritySetting;

	public ShowNetImage _SkillSetting;

	public GCheckBox _Area1;

	public GCheckBox _Area2;

	public GCheckBox _Area3;

	private int FightRadius = 100;

	public UIScrollBar _LifeLessThanXScroll;

	public UIScrollBar _MagicLessThanXScroll;

	public AutoSkillPriorityPart m_AutoSkillPriorityPart;

	public GChildWindow m_GChildWindow;

	private static readonly string[] ShiQuItemNames = new string[]
	{
		Global.GetLang("白色装备"),
		Global.GetLang("蓝色装备"),
		Global.GetLang("绿色装备"),
		Global.GetLang("紫色装备"),
		Global.GetLang("宝石"),
		Global.GetLang("羽毛"),
		Global.GetLang("药品"),
		Global.GetLang("金币"),
		Global.GetLang("门票材料"),
		Global.GetLang("其他道具")
	};

	private Dictionary<int, int> m_SkillPriorityDict = new Dictionary<int, int>();
}
