using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;

public class ZhongShenZhengBaPartPlayerStateList : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public int ServerText
	{
		set
		{
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(value, out ztBuffServerInfo))
			{
				this.Server.text = Global.GetColorStringForNGUIText(new object[]
				{
					"6F9CE0",
					string.Format("{0}", ztBuffServerInfo.strServerName)
				});
			}
			else
			{
				this.Server.text = Global.GetColorStringForNGUIText(new object[]
				{
					"6F9CE0",
					string.Format("s{0}", value)
				});
			}
		}
	}

	public string NameText
	{
		set
		{
			this.Name.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				value
			});
		}
	}

	public int ChengjiText
	{
		set
		{
			if (value == 0)
			{
				this.Chengji.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					string.Empty
				});
			}
			else if (value == 1)
			{
				this.Chengji.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("冠军")
				});
			}
			else if (value == 2)
			{
				this.Chengji.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("亚军")
				});
			}
			else if (value == 4)
			{
				this.Chengji.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("4强")
				});
			}
			else if (value == 8)
			{
				this.Chengji.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("8强")
				});
			}
			else if (value == 16)
			{
				this.Chengji.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("16强")
				});
			}
			else if (value == 32)
			{
				this.Chengji.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("32强")
				});
			}
			else if (value == 64)
			{
				this.Chengji.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("64强")
				});
			}
			else if (value == 100)
			{
				this.Chengji.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("100强")
				});
			}
		}
	}

	public int ZhanliText
	{
		set
		{
			this.Zhanli.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				value
			});
		}
	}

	public int StateText
	{
		set
		{
			if (value == 0)
			{
				this.State.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					string.Empty
				});
			}
			else if (value == 1)
			{
				this.State.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("晋级")
				});
			}
			else if (value == 2)
			{
				this.State.text = Global.GetColorStringForNGUIText(new object[]
				{
					"DD0B0B",
					Global.GetLang("淘汰")
				});
			}
		}
	}

	public UILabel Server;

	public new UILabel Name;

	public UILabel Chengji;

	public UILabel Zhanli;

	public UILabel State;
}
