using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class GaiMingPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_lblHint.text = string.Empty;
		this.m_InputXinMingZi.text = string.Empty;
		this.m_ConfirmIcon.Text = Global.GetLang("确定");
		this.staticText[0].text = Global.GetLang("设置新名字");
		this.staticText[1].text = Global.GetLang("请输入新名字：");
		this.m_lblHint.transform.localPosition = new Vector3(-10f, -20f, -1f);
		this.m_texZuanshi.gameObject.transform.localPosition = new Vector3(35f, -18f, -0.5f);
	}

	protected override void InitializeComponent()
	{
		GaiMingPart.Instance = this;
		base.InitializeComponent();
		this.InitTextInPrefabs();
		MUDebug.Log<string>(new string[]
		{
			"链表 = " + GaiMingPart._ChangeNameInfo.RoleList[0]
		});
		this.m_RandomName.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.RandomName_MouseLeftButtonUp);
		this.m_Input.TextChanged = delegate(object s, EventArgs e)
		{
			if (this.changeInput)
			{
				if (GaiMingPart._ChangeNameInfo.RoleList[this.index].LeftFreeTimes > 0)
				{
					this.m_lblHint.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"FFFFFF",
						string.Format(Global.GetLang("本次免费"), new object[0])
					}));
				}
				else
				{
					this.m_lblHint.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"FFFFFF",
						string.Format(Global.GetLang("消耗钻石 ：         {0}"), this.xiaofeizuanshi)
					}));
					this.m_texZuanshi.gameObject.SetActive(true);
				}
				this.changeInput = false;
			}
		};
		this.Close();
		this.m_ConfirmIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.ConfirmIcon_MouseLeftButtonUp);
	}

	private void ConfirmIcon_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		this.newName = this.m_InputXinMingZi.text;
		if (Global.IncludeReplaceFilterFileds(this.newName))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,您的昵称当中含有敏感词汇，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		if (this.newName.IndexOf(" ") != -1)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,昵称当中不允许包含空格，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		this.newName = Global.StringReplaceAll(this.newName, "'", string.Empty);
		this.newName = Global.StringReplaceAll(this.newName, "|", string.Empty);
		this.newName = Global.StringReplaceAll(this.newName, "$", string.Empty);
		this.newName = Global.StringReplaceAll(this.newName, ":", string.Empty);
		this.newName = Global.ReplaceFilterFileds(this.newName);
		if (this.newName.Contains("{") || this.newName.Contains("}"))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,您的昵称当中含有非法字符，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		if (string.IsNullOrEmpty(this.newName) || Global.StringTrim(this.newName) == string.Empty)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), Global.GetLang("抱歉,请输入的您要创建的角色名称!"), -1, -1, -1, -1, false);
			return;
		}
		if (!Global.IsKoreanOrEn(this.newName))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,您的昵称当中含有非法字符，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"roleid = ",
				this.roleid,
				"zoneid = ",
				this.zoneid,
				"newname1 = ",
				this.newName
			})
		});
		GameInstance.Game.SpriteChangeName(this.roleid, this.zoneid, this.newName);
	}

	private void RandomName_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		this.m_texZuanshi.gameObject.SetActive(false);
		if (GaiMingPart._ChangeNameInfo.RoleList[this.index].LeftFreeTimes > 0)
		{
			this.m_lblHint.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"FFFFFF",
				string.Format(Global.GetLang("本次免费"), new object[0])
			}));
		}
		else
		{
			this.m_lblHint.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"FFFFFF",
				string.Format(Global.GetLang("消耗钻石 ：         {0}"), this.xiaofeizuanshi)
			}));
			this.m_texZuanshi.gameObject.SetActive(true);
		}
		if (null != this.m_SpriteAnimation)
		{
			this.m_SpriteAnimation.gameObject.SetActive(true);
			this.m_SpriteAnimation.Reset();
			this.m_SpriteAnimation.isFinishedHiddle = true;
		}
		int sex = Random.Range(0, 2);
		this.SetRandomPreName(Global.GetRandomCreatorRoleName(sex));
	}

	public void SetRandomPreName(string name)
	{
		this.m_InputXinMingZi.text = name;
	}

	public void Close()
	{
		this.m_BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			Object.Destroy(base.gameObject);
		};
	}

	public void GaiMingInfo(ChangeNameInfo _ChangeNameInfo)
	{
		this.m_texZuanshi.gameObject.SetActive(false);
		MUDebug.Log<string>(new string[]
		{
			"人物id" + this.roleid
		});
		for (int i = 0; i < _ChangeNameInfo.RoleList.Count; i++)
		{
			MUDebug.Log<string>(new string[]
			{
				"改名信息id" + _ChangeNameInfo.RoleList[i].RoleId
			});
			if (this.roleid == _ChangeNameInfo.RoleList[i].RoleId)
			{
				this.index = i;
				if (_ChangeNameInfo.RoleList[i].LeftFreeTimes > 0)
				{
					this.m_lblHint.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"FFFFFF",
						string.Format(Global.GetLang("本次免费"), new object[0])
					}));
				}
				else
				{
					List<XElement> list = Enumerable.ToList<XElement>(Global.GetGameResXml(string.Format("Config/SystemParams.xml", new object[0])).Element("Params").Elements("Param"));
					string text = list.Find((XElement s) => s.Attribute("Name") != null && s.Attribute("Name").Value == "ModName").Attribute("Value").Value.ToString();
					this.xiaohaozuanshicount = Convert.ToInt32(text.Split(new char[]
					{
						','
					})[0]);
					this.xiaohaozuanshishangxian = Convert.ToInt32(text.Split(new char[]
					{
						','
					})[1]);
					this.xiaofeizuanshi = this.xiaohaozuanshicount * (_ChangeNameInfo.RoleList[i].AlreadyZuanShiTimes + 1);
					if (this.xiaofeizuanshi > this.xiaohaozuanshishangxian)
					{
						this.xiaofeizuanshi = this.xiaohaozuanshishangxian;
					}
					this.m_lblHint.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"FFFFFF",
						string.Format(Global.GetLang("消耗钻石 ：         {0}"), this.xiaofeizuanshi)
					}));
					this.m_texZuanshi.gameObject.SetActive(true);
				}
			}
		}
	}

	public void GaiMingHuiDiao(ChangeNameResult data)
	{
		this.m_texZuanshi.gameObject.SetActive(false);
		if (data.ErrCode == 0)
		{
			this.m_lblHint.text = Global.GetLang("改名成功");
		}
		if (data.ErrCode == 1)
		{
			this.m_lblHint.text = Global.GetLang("名字中含有非法字符或屏蔽字");
		}
		if (data.ErrCode == 2)
		{
			this.m_lblHint.text = Global.GetLang("数据库错误");
		}
		if (data.ErrCode == 3)
		{
			this.m_lblHint.text = Global.GetLang("您没有改名权限");
		}
		if (data.ErrCode == 4)
		{
			this.m_lblHint.text = Global.GetLang("跨服服务器中不可改名");
		}
		if (data.ErrCode == 5)
		{
			this.m_lblHint.text = Global.GetLang("名字已被占用，请更换后再试");
		}
		if (data.ErrCode == 6)
		{
			this.m_lblHint.text = string.Concat(new object[]
			{
				Global.GetLang("名字长度必须为"),
				Global.NameLengthRange[0],
				"-",
				Global.NameLengthRange[1],
				Global.GetLang("个汉字或字符")
			});
		}
		if (data.ErrCode == 7)
		{
			this.m_lblHint.text = Global.GetLang("账号下没有该角色");
		}
		if (data.ErrCode == 8)
		{
			this.m_lblHint.text = Global.GetLang("需要验证二级密码");
		}
		if (data.ErrCode == 9)
		{
			this.m_lblHint.text = Global.GetLang("钻石不足");
		}
		if (data.ErrCode == 10)
		{
			this.m_lblHint.text = Global.GetLang("服务器拒绝(改名操作未开放)");
		}
		if (data.ErrCode == 11)
		{
			this.m_lblHint.text = Global.GetLang("只能在选角色界面改名");
		}
		this.changeInput = true;
		this.zoneid = data.ZoneId;
		this.newName = data.NewName;
		if (data.ErrCode == 8)
		{
			Super.ShowVerifySecondPasswordWindow(GaiMingPart._ChangeNameInfo.RoleList[this.index].RoleId);
		}
		if (data.ErrCode == 0)
		{
			this.changeInput = false;
			GaiMingPart._ChangeNameInfo.ZuanShi = data.NameInfo.ZuanShi;
			GaiMingPart._ChangeNameInfo.RoleList[this.index].LeftFreeTimes = data.NameInfo.RoleList[this.index].LeftFreeTimes;
			GaiMingPart._ChangeNameInfo.RoleList[this.index].AlreadyZuanShiTimes = data.NameInfo.RoleList[this.index].AlreadyZuanShiTimes;
			this.listBox.SelectedItem.GetComponent<RoleSelectorListItem>().TextBlockName.text = this.newName;
			Object.Destroy(base.gameObject);
		}
	}

	public TextBlock[] staticText;

	public static ChangeNameInfo _ChangeNameInfo;

	public TextBox m_Input;

	public GButton m_ConfirmIcon;

	public GButton m_RandomName;

	public UILabel m_lblHint;

	public GButton m_BtnClose;

	public UILabel m_InputXinMingZi;

	public UISprite m_texZuanshi;

	public static GaiMingPart Instance;

	public int roleid;

	public int zoneid;

	public int index;

	public string newName;

	public UISpriteAnimation m_SpriteAnimation;

	public ListBox listBox;

	public DPSelectedItemEventHandler DPSelectedItem;

	protected GChildWindow SetSecondPasswordWindow;

	protected SetSecondPasswordPart SetSecondPasswordPart;

	private int xiaohaozuanshicount;

	private int xiaohaozuanshishangxian;

	private int xiaofeizuanshi;

	private bool changeInput;
}
