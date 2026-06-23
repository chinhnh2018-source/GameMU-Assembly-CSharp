using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhongShenZhengBaPartPlayerState : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private void InitTextInPerfabs()
	{
		this.Server.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("服务器")
		});
		this.Name.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("名字")
		});
		this.Chengji.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("成绩")
		});
		this.Zhanli.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("战力")
		});
		this.State.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("当前状态")
		});
		this.MyGroude.text = string.Empty;
		this.back.URL = "NetImages/GameRes/Images/Plate/zhongshen/tongyongdikuang1.png.qj";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPerfabs();
		this.ItemCollection = this.list.ItemsSource;
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		GameInstance.Game.GetAllPKState();
		Super.ShowNetWaiting(null);
	}

	public void InitPlayerStateListItem(List<TianTiPaiHangRoleData> Data)
	{
		int i = 0;
		int count = Data.Count;
		while (i < count)
		{
			ZhongShenZhengBaPartPlayerStateList zhongShenZhengBaPartPlayerStateList = U3DUtils.NEW<ZhongShenZhengBaPartPlayerStateList>();
			zhongShenZhengBaPartPlayerStateList.ServerText = Data[i].ZoneId;
			zhongShenZhengBaPartPlayerStateList.NameText = Data[i].RoleName;
			zhongShenZhengBaPartPlayerStateList.ChengjiText = Data[i].ZhengBaGrade;
			zhongShenZhengBaPartPlayerStateList.ZhanliText = Data[i].ZhanLi;
			zhongShenZhengBaPartPlayerStateList.StateText = Data[i].ZhengBaState;
			this.ItemCollection.AddNoUpdate(zhongShenZhengBaPartPlayerStateList);
			UIPanel component = zhongShenZhengBaPartPlayerStateList.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			if (Data[i].ZoneId == Global.Data.roleData.ZoneID && Data[i].RoleId == Global.Data.roleData.RoleID)
			{
				this.MyGroude.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					string.Format(Global.GetLang("我的成绩：{0}{1}"), this.GetMyGroude(Data[i].ZhengBaGrade), this.GetMyState(Data[i].ZhengBaState))
				});
			}
			i++;
		}
	}

	private string GetMyGroude(int value)
	{
		string result = string.Empty;
		if (value == 0)
		{
			result = string.Empty;
		}
		else if (value == 1)
		{
			result = Global.GetLang("冠军");
		}
		else if (value == 2)
		{
			result = Global.GetLang("亚军");
		}
		else if (value == 4)
		{
			result = Global.GetLang("4强");
		}
		else if (value == 8)
		{
			result = Global.GetLang("8强");
		}
		else if (value == 16)
		{
			result = Global.GetLang("16强");
		}
		else if (value == 32)
		{
			result = Global.GetLang("32强");
		}
		else if (value == 64)
		{
			result = Global.GetLang("64强");
		}
		else if (value == 100)
		{
			result = Global.GetLang("100强");
		}
		return result;
	}

	private string GetMyState(int value)
	{
		string result = string.Empty;
		if (value == 2)
		{
			result = Global.GetLang("(淘汰)");
		}
		return result;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public ShowNetImage back;

	public UILabel Server;

	public new UILabel Name;

	public UILabel Chengji;

	public UILabel Zhanli;

	public UILabel State;

	public UILabel MyGroude;

	public GButton BtnClose;

	public ListBox list;

	private ObservableCollection _ItemCollection;
}
