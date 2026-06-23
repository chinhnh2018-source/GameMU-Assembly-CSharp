using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhongShenZhengBaPartZhanBao : UserControl
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

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.List.ItemsSource;
		this.back.URL = "NetImages/GameRes/Images/Plate/zhongshen/zhanbao.jpg.qj";
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		GameInstance.Game.GetAllPKLog();
		Super.ShowNetWaiting(null);
	}

	public void InitList(List<ZhengBaPkLogData> PKLogData)
	{
		int i = 0;
		int count = PKLogData.Count;
		while (i < count)
		{
			string text = Global.GetColorStringForNGUIText(new object[]
			{
				"ffcc19",
				string.Format("[{0}]", this.name[PKLogData[i].Day - 1])
			});
			text = text + Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				PKLogData[i].RoleName1
			}) + Global.GetLang("对战");
			text = text + Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				PKLogData[i].RoleName2
			}) + ",";
			if (PKLogData[i].PkResult == 1)
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					PKLogData[i].RoleName1
				});
			}
			else if (PKLogData[i].PkResult == 2)
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					PKLogData[i].RoleName2
				});
			}
			else
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("PK异常")
				});
			}
			if (PKLogData[i].PkResult != 0)
			{
				if (PKLogData[i].Day == 7)
				{
					if (PKLogData[i].UpGrade)
					{
						text += Global.GetLang("获得了冠军！");
					}
				}
				else if (PKLogData[i].UpGrade)
				{
					text += Global.GetLang("获得了胜利，成功晋级！");
				}
			}
			ZhongShenZhengBaPartZhanBaoList zhongShenZhengBaPartZhanBaoList = U3DUtils.NEW<ZhongShenZhengBaPartZhanBaoList>();
			zhongShenZhengBaPartZhanBaoList.Miaoshu = text;
			this.ItemCollection.AddNoUpdate(zhongShenZhengBaPartZhanBaoList);
			UIPanel component = zhongShenZhengBaPartZhanBaoList.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			i++;
		}
	}

	public GButton BtnClose;

	public ListBox List;

	public DPSelectedItemEventHandler CloseHandler;

	public ShowNetImage back;

	private ObservableCollection _ItemCollection;

	private string[] name = new string[]
	{
		Global.GetLang("100强"),
		Global.GetLang("64强"),
		Global.GetLang("32强"),
		Global.GetLang("16强"),
		Global.GetLang("8强"),
		Global.GetLang("4强"),
		Global.GetLang("2强"),
		Global.GetLang("冠军")
	};
}
