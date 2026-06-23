using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class ZuduiMemberItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ShowTrans = ZuduiMemberItem.ShowType.eZuDui;
		if (this.btnLeaderInvete != null)
		{
			this.btnLeaderInvete.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				ZuduiMemberItem.sendInviteTeamMsg(true, false);
			};
		}
	}

	public string MemberName
	{
		get
		{
			return this.lblMemberName.text;
		}
		set
		{
			this.lblMemberName.text = value;
		}
	}

	public string MemberForce
	{
		get
		{
			return this.lblForce.text;
		}
		set
		{
			this.lblForce.text = value;
		}
	}

	public bool IsCap
	{
		get
		{
			return this.texCap.gameObject.activeInHierarchy;
		}
		set
		{
			this.texCap.gameObject.SetActive(value);
		}
	}

	public bool IsReady
	{
		get
		{
			return this.texReady.gameObject.activeInHierarchy;
		}
		set
		{
			this.texReady.gameObject.SetActive(value);
		}
	}

	public string FaceImg
	{
		get
		{
			return this.texFace.URL;
		}
		set
		{
			this.texFace.URL = value;
		}
	}

	public int RoleID
	{
		get
		{
			return this._roleId;
		}
		set
		{
			this._roleId = value;
		}
	}

	public ZuduiMemberItem.ShowType ShowTrans
	{
		get
		{
			return this._ShowTrans;
		}
		set
		{
			if (value < (ZuduiMemberItem.ShowType)this.transLst.Length)
			{
				this._ShowTrans = value;
				for (int i = 0; i < this.transLst.Length; i++)
				{
					if (i == (int)value)
					{
						this.transLst[i].gameObject.SetActive(true);
					}
					else
					{
						this.transLst[i].gameObject.SetActive(false);
					}
				}
			}
		}
	}

	public void OnClick()
	{
		if (this.ShowTrans == ZuduiMemberItem.ShowType.eYaoQingZuDui)
		{
			return;
		}
		if (ZuduiFubenPart.IsKuaFuFuBen)
		{
			if (this.RoleID != Global.Data.roleData.RoleID && Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
			{
				this.CreateMenuWindow();
			}
		}
		else if (this.RoleID != Global.Data.roleData.RoleID)
		{
			this.CreateMenuWindow();
		}
	}

	public void CreateMenuWindow()
	{
		if (null != this.menuPart)
		{
			if (this.menuPart.Visibility)
			{
				this.menuPart.Visibility = false;
			}
			else
			{
				this.ResetMenuPartPos();
				this.menuPart.Visibility = true;
			}
			return;
		}
		this.menuPart = U3DUtils.NEW<GTxtMenuPart>();
		this.menuPart.Width = 150.0;
		this.menuPart.ItemHeight = 35;
		if (!ZuduiFubenPart.IsKuaFuFuBen)
		{
			this.menuPart.AddMenuItem(0, Global.GetLang("查 看"));
		}
		if (Global.Data.CurrentCopyTeamData.LeaderRoleID == Global.Data.roleData.RoleID)
		{
			this.menuPart.AddMenuItem(1, Global.GetLang("踢出房间"));
		}
		this.menuPart.RenderMenu();
		this.menuPart.Closehandler = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (null != this.menuPart)
			{
				NGUITools.Destroy(this.menuPart.gameObject);
			}
			this.menuPart = null;
		};
		this.menuPart.MenuItemClick = delegate(object s, EventArgs e)
		{
			GTxtMenuItem gtxtMenuItem = s as GTxtMenuItem;
			if (null == gtxtMenuItem)
			{
				return;
			}
			if (gtxtMenuItem.MenuItemID >= 0 && gtxtMenuItem.MenuItemID <= 1)
			{
				this.ItemFunction(gtxtMenuItem.MenuItemID);
			}
			this.menuPart.Visibility = false;
		};
		U3DUtils.AddChild(base.transform.gameObject, this.menuPart.gameObject, true);
		this.ResetMenuPartPos();
		this.menuPart.SelectIndex = -1;
	}

	private void ResetMenuPartPos()
	{
		Vector3 zero = Vector3.zero;
		if (UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX > 754f)
		{
			zero..ctor(-190f, 97f, -3f);
		}
		else
		{
			zero..ctor(39f, 97f, -3f);
		}
		if (this.menuPart != null)
		{
			this.menuPart.transform.localPosition = zero;
		}
	}

	private void ItemFunction(int idx)
	{
		if (idx != 0)
		{
			if (idx == 1)
			{
				GameInstance.Game.SpriteCopyTeam(TeamCmds.Remove, (long)this.RoleID, 0, 0, 0);
			}
		}
		else
		{
			GameInstance.Game.SpriteGetOtherAttrib(this.RoleID);
		}
	}

	public static void sendInviteTeamMsg(bool showFailHint = true, bool skipCD = false)
	{
		if (Global.Data.CurrentCopyTeamData == null)
		{
			return;
		}
		long num = (Global.GetCorrectLocalTime() - ZuduiMemberItem.llLeaderInviteMsgTimestamp) / 1000L;
		if (!skipCD && num < ZuduiMemberItem.llLeaderInviteCD)
		{
			if (showFailHint)
			{
				Super.HintMainText(Global.GetLang("冷却中稍后再试"), 10, 3);
			}
			return;
		}
		ZuduiMemberItem.llLeaderInviteMsgTimestamp = Global.GetCorrectLocalTime();
		string text = Global.GetLang("组队邀请");
		XElement xelement = null;
		XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		foreach (XElement xelement2 in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ID");
			if (xelementAttributeInt == Global.Data.CurrentCopyTeamData.SceneIndex)
			{
				xelement = xelement2;
				text = Global.GetXElementAttributeStr(xelement2, "CopyName");
				break;
			}
		}
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "TabID");
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
		int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MinLevel");
		int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "FinishNumber");
		int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement, "ZhanLi");
		string text2 = string.Empty;
		text2 = text2 + Global.GetLang("｛team_") + string.Format("{0}_{1}#{2}#{3}#{4}", new object[]
		{
			Global.Data.CurrentCopyTeamData.SceneIndex,
			Global.Data.CurrentCopyTeamData.MinZhanLi,
			Global.Data.CurrentCopyTeamData.LeaderRoleID,
			Global.Data.CurrentCopyTeamData.TeamID,
			ZuduiFubenPart.ItemCount
		}) + Global.GetLang("｝");
		text2 = text2 + "{ff08ff}" + string.Format("[{0}]", text) + Global.GetLang("{-}｛-｝");
		string text3 = string.Empty;
		string text4 = string.Empty;
		string empty = string.Empty;
		if (Global.Data.CurrentCopyTeamData.MinZhanLi > 0)
		{
			text3 = StringUtil.substitute(Global.GetLang("，要求战力：{0}"), new object[]
			{
				Global.Data.CurrentCopyTeamData.MinZhanLi
			});
			text4 = StringUtil.substitute(Global.GetLang("发出邀请{0}{1}"), new object[]
			{
				text2,
				string.Empty
			});
			text4 += text3;
		}
		else
		{
			text4 = StringUtil.substitute(Global.GetLang("邀请您加入{0}{1}"), new object[]
			{
				text2,
				string.Empty
			});
			text4 += text3;
		}
		ChatType chatType = ChatType.TextOrSymbol;
		GameInstance.Game.SpriteSendChat(2, Global.FormatRoleName(Global.Data.roleData), string.Empty, text4 + empty, chatType, 0);
		Super.HintMainText(Global.GetLang("已经成功发送邀请"), 10, 3);
	}

	public override void Update()
	{
		if (this._ShowTrans == ZuduiMemberItem.ShowType.eYaoQingZuDui)
		{
			long num = (Global.GetCorrectLocalTime() - ZuduiMemberItem.llLeaderInviteMsgTimestamp) / 1000L;
			if (num <= ZuduiMemberItem.llLeaderInviteCD)
			{
				int num2 = (int)(ZuduiMemberItem.llLeaderInviteCD - num);
				if (num2 < 0)
				{
					return;
				}
				this.lblLeaderInviteCD.text = string.Format(Global.GetLang("{0}秒"), num2);
				this.lblLeaderInviteCD.gameObject.SetActive(true);
			}
			else if (this.lblLeaderInviteCD.gameObject.activeSelf)
			{
				this.lblLeaderInviteCD.gameObject.SetActive(false);
			}
		}
	}

	public UILabel lblMemberName;

	public UILabel lblForce;

	public UISprite texCap;

	public UISprite texReady;

	public ShowNetImage texFace;

	public Transform[] transLst;

	public GButton btnLeaderInvete;

	public UILabel lblLeaderInviteCD;

	private int _roleId;

	private ZuduiMemberItem.ShowType _ShowTrans;

	public GTxtMenuPart menuPart;

	private static long llLeaderInviteMsgTimestamp;

	private static long llLeaderInviteCD = 10L;

	public enum ShowType
	{
		eZuDui,
		eYaoQingZuDui
	}
}
