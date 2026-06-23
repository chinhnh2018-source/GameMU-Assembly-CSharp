using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class FamilyGaiMingPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_Input.label.text = Global.GetLang("请输入新名字:");
		this.m_lblHint.text = Global.GetLang("合服后重名战盟获得一次改名机会\n请盟主慎重使用");
		this.m_InputXinMingZi.text = string.Empty;
		this.m_ConfirmIcon.Text = Global.GetLang("确定");
		this.m_label.Text = Global.GetLang("请输入新名字:");
		this.m_title.Text = Global.GetLang("设置新名字");
	}

	protected override void InitializeComponent()
	{
		FamilyGaiMingPart.Instance = this;
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.m_RandomName.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.RandomName_MouseLeftButtonUp);
		this.m_BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			FamilyGaiMingPart.Instance = null;
			Object.Destroy(base.gameObject);
		};
		this.m_ConfirmIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.ConfirmIcon_MouseLeftButtonUp);
	}

	private void SetErrorHint(string hint)
	{
		Super.HintMainText(hint, 10, 3);
		this.m_lblHint.text = hint;
	}

	private void ConfirmIcon_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		this.m_Input.text = this.m_Input.text.Trim();
		this.m_Input.text = Global.StringReplaceAll(this.m_Input.text, "'", string.Empty);
		this.m_Input.text = Global.StringReplaceAll(this.m_Input.text, "|", string.Empty);
		this.m_Input.text = Global.StringReplaceAll(this.m_Input.text, "$", string.Empty);
		this.m_Input.text = Global.StringReplaceAll(this.m_Input.text, ":", string.Empty);
		this.m_Input.text = Global.StringReplaceAll(this.m_Input.text, Global.GetLang("："), string.Empty);
		if (string.IsNullOrEmpty(this.m_Input.text))
		{
			this.SetErrorHint(Global.GetLang("请输入新名称!"));
			return;
		}
		if (Global.IncludeReplaceFilterFileds(this.m_Input.text))
		{
			this.SetErrorHint(Global.GetLang("战盟名称当中含有敏感词汇，请重新输入!"));
			return;
		}
		if (this.m_Input.text == Global.GetLang("战盟名称七个字"))
		{
			this.SetErrorHint(Global.GetLang("请输入要创建的战盟的名称!"));
			return;
		}
		int num = (int)Global.CheckRoleNameLenght(this.m_Input.text);
		if ((int)Global.NameLengthRange[0] == num)
		{
			this.SetErrorHint(Global.GetLang("名称不能少于") + num + Global.GetLang("个汉字，请重新输入!"));
			return;
		}
		if ((int)Global.NameLengthRange[1] == num)
		{
			this.SetErrorHint(Global.GetLang("名称不能多于") + num + Global.GetLang("个汉字，请重新输入！"));
			return;
		}
		if (this.m_Input.text.Length > 100)
		{
			this.SetErrorHint(Global.GetLang("您输入的战盟公告超过了100汉字，请重新输入！"));
			return;
		}
		if (!flag)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			return;
		}
		if (!flag2)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("创建战盟需要一个【{0}】，你的背包中没有该物品，无法创建!"), new object[]
			{
				Global.GetGoodsNameByID(Global.CreateBangHuiNeedDaoju, false)
			}), 10, 3);
			return;
		}
		if (!flag3)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("创建战盟需要角色等级达到【{0}】"), new object[]
			{
				Global.CreateBangHuiNeedLevel
			}), 10, 3);
			return;
		}
		WordsFilterMgr.ExecWordsFilter(this.m_Input.text, delegate(object content, ExecWordsFilterEventArgs result)
		{
			if (result.ret > 0)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
				{
					result.ret,
					result.msg
				}), 10, 3);
				return;
			}
			if (result.is_dirty > 0)
			{
				this.SetErrorHint(Global.GetLang("名称中含有敏感词，请重新输入!"));
				return;
			}
			this.Send_CMD_SPR_CHANGE_BANGHUI_NAME(Global.StringTrim(this.m_Input.text));
		});
	}

	private void RandomName_MouseLeftButtonUp(object sender, MouseEvent e)
	{
	}

	public void SetRandomPreName(string name)
	{
		this.m_InputXinMingZi.text = name;
	}

	private static bool IsNetResultSuccess(MUSocketConnectEventArgs e)
	{
		switch (Global.SafeConvertToInt32(e.fields[0]))
		{
		case 0:
			if (FamilyGaiMingPart.Instance)
			{
				FamilyGaiMingPart.Instance.SetErrorHint(Global.GetLang("改名成功"));
				FamilyGaiMingPart.Instance.m_ConfirmIcon.gameObject.SetActive(false);
			}
			return true;
		case 1:
			if (FamilyGaiMingPart.Instance)
			{
				FamilyGaiMingPart.Instance.SetErrorHint(Global.GetLang("新名字非法"));
			}
			return false;
		case 2:
			if (FamilyGaiMingPart.Instance)
			{
				FamilyGaiMingPart.Instance.SetErrorHint(Global.GetLang("数据库错误"));
			}
			return false;
		case 3:
			if (FamilyGaiMingPart.Instance)
			{
				FamilyGaiMingPart.Instance.SetErrorHint(Global.GetLang("名字已被占用"));
			}
			return false;
		case 4:
			if (FamilyGaiMingPart.Instance)
			{
				FamilyGaiMingPart.Instance.SetErrorHint(Global.GetLang("服务器拒绝"));
			}
			return false;
		case 5:
			if (FamilyGaiMingPart.Instance)
			{
				FamilyGaiMingPart.Instance.SetErrorHint(Global.GetLang("名字长度错误"));
			}
			return false;
		default:
			return false;
		}
	}

	private void Send_CMD_SPR_CHANGE_BANGHUI_NAME(string newname)
	{
		GameInstance.Game.SpriteChangeBangHuiName(newname);
	}

	public static void Action_CMD_SPR_CHANGE_BANGHUI_NAME(MUSocketConnectEventArgs e)
	{
		if (!FamilyGaiMingPart.IsNetResultSuccess(e))
		{
			return;
		}
	}

	public TextBox m_Input;

	public GButton m_ConfirmIcon;

	public GButton m_RandomName;

	public UILabel m_lblHint;

	public GButton m_BtnClose;

	public UILabel m_InputXinMingZi;

	public UISprite m_texZuanshi;

	public static FamilyGaiMingPart Instance;

	public int roleid;

	public int zoneid;

	public int index;

	public string newName;

	public DPSelectedItemEventHandler DPSelectedItem;

	protected GChildWindow SetSecondPasswordWindow;

	protected SetSecondPasswordPart SetSecondPasswordPart;

	public TextBlock m_label;

	public TextBlock m_title;

	public enum EChangeGuildNameError
	{
		Success,
		InvalidName,
		DBFailed,
		NameAlreadyUsed,
		OperatorDenied,
		LengthError
	}
}
