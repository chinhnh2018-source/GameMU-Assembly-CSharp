using System;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class YongHuXieYi : UserControl
{
	private void OnApplicationPause()
	{
	}

	private void OnApplicationFocus()
	{
	}

	private void OnApplicationQuit()
	{
	}

	private new void Update()
	{
		if (this.scrollBar && this.scrollBar.scrollValue > 0.95f)
		{
			this.confirm.isEnabled = true;
			this.cancel.isEnabled = true;
			if (this.CheckBox)
			{
				this.CheckBox.isChecked = true;
			}
		}
	}

	protected override void InitializeComponent()
	{
		if (this.close)
		{
			this.close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			};
		}
		if (this.confirm)
		{
			this.confirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				YongHuXieYi.SetHaveConfirm();
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			};
			this.confirm.isEnabled = false;
		}
		if (this.cancel)
		{
			this.cancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			};
			this.cancel.isEnabled = false;
		}
		if (this.CheckBox)
		{
			this.CheckBox.isChecked = false;
			this.CheckBox.CheckChanged = new BaseEventHandler2(this.OnCheckChanged);
		}
		if (YongHuXieYi.xmlEle != null && this.contextLbl != null)
		{
			this.contextLbl.text = Global.ReadXmlConfigStr(YongHuXieYi.xmlEle, "txt", "value");
		}
	}

	private void OnCheckChanged(object sender, BaseEventArgs e)
	{
		if (this.CheckBox.Check)
		{
			this.confirm.isEnabled = true;
		}
		else
		{
			this.confirm.isEnabled = false;
		}
	}

	private static string CurKey
	{
		get
		{
			string text = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
			if ("-1" == text)
			{
				return string.Empty;
			}
			return StringUtil.substitute("MstYongHuXieYi_{0}", new object[]
			{
				text
			});
		}
	}

	private static string CurVersion
	{
		get
		{
			if (YongHuXieYi.xmlEle == null)
			{
				return string.Empty;
			}
			return Global.ReadXmlConfigStr(YongHuXieYi.xmlEle, "txtversion", "value");
		}
	}

	public static bool IsHaveConfirm()
	{
		if (string.IsNullOrEmpty(YongHuXieYi.CurKey))
		{
			return true;
		}
		if (string.IsNullOrEmpty(YongHuXieYi.CurVersion))
		{
			return true;
		}
		string @string = LocalStorage.GetString(YongHuXieYi.CurKey);
		return !string.IsNullOrEmpty(@string) && @string.Equals(YongHuXieYi.CurVersion);
	}

	public static bool SetHaveConfirm()
	{
		if (string.IsNullOrEmpty(YongHuXieYi.CurKey))
		{
			return false;
		}
		if (string.IsNullOrEmpty(YongHuXieYi.CurVersion))
		{
			return false;
		}
		LocalStorage.SetString(YongHuXieYi.CurKey, YongHuXieYi.CurVersion);
		return true;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton close;

	public Transform title;

	public Transform bak;

	public UISprite spriteBak;

	public UIScrollBar scrollBar;

	public GButton confirm;

	public GButton cancel;

	public UILabel contextLbl;

	public GCheckBox CheckBox;

	public static XElement xmlEle;
}
