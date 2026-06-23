using System;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class CfgWnd : CfgBitmapWindow
{
	public CfgWnd(int windowID, UserControl parent, string wndName = "", int left = 0, int top = 0, bool isUnique = true) : base(parent, left, top)
	{
		this.MyParent = parent;
		this.IsUnique = isUnique;
		this.WndName = wndName;
		this.WindowID = windowID;
		if (this.IsUnique && !StringUtil.isWhitespace(this.WndName))
		{
			CfgSingleWndMgr.Instance().Add(this.WndName, this);
		}
		this.InitUI();
	}

	protected bool InitUI()
	{
		XElement windowConfigXmlNodeByID = Global.GetWindowConfigXmlNodeByID(this.WindowID);
		if (windowConfigXmlNodeByID == null)
		{
			GError.AddErrMsg(StringUtil.substitute(Global.GetLang("找不到活动 {0} 界面相关的配置信息"), new object[]
			{
				this.WindowID
			}));
			return false;
		}
		base.InitUIByXmlCfg(windowConfigXmlNodeByID);
		CfgWndPart cfgWndPart = U3DUtils.NEW<CfgWndPart>();
		cfgWndPart.OnAnyBtnClick = new MouseLeftButtonUpEventHandler(this.OnAnyBtnClick);
		return true;
	}

	protected override void OnClose()
	{
		base.OnClose();
		if (this.IsUnique && !StringUtil.isWhitespace(this.WndName) && this.WndName.Length > 0)
		{
			CfgSingleWndMgr.Instance().Remove(this.WndName);
		}
	}

	protected void OnAnyBtnClick(object sender, MouseEvent e)
	{
		CfgButton cfgButton = sender as CfgButton;
		if (null == cfgButton)
		{
			return;
		}
		if ("close" == cfgButton.TagKey)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Normal, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("关闭按钮被点击,按钮绑定值: {0}"), new object[]
			{
				cfgButton.BindValue
			}), 0, -1, -1, 0);
			base.NotifyClose(1);
		}
		else if ("nowgo" == cfgButton.TagKey)
		{
			if (Super.CanTransport(-1, true, false))
			{
				GameInstance.Game.SpriteActivityTransport(cfgButton.BindValue.SafeToInt32(0));
			}
		}
		else if (cfgButton.TagKey.IndexOf("toactivity") >= 0)
		{
			PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
			if (null != playZone)
			{
				playZone.ShowActivityWindow(false, 1, -1);
			}
		}
		else if ("OpenIntroduceWindow" == cfgButton.TagKey)
		{
			CfgSingleWndMgr.Instance().ShowWindow(CfgWndName.GetCfgWndNameByID(cfgButton.BindValue.SafeToInt32(0)), true, null);
		}
	}

	public void Close()
	{
		base.NotifyClose(1);
	}

	private UserControl MyParent;

	private bool IsUnique;

	public string WndName = string.Empty;

	public int WindowID = -6666;
}
