using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;

public class BaoDianPartPage : UserControl
{
	protected override void InitializeComponent()
	{
		this._ItemCollection = this.listBox.ItemsSource;
	}

	public int TabID
	{
		get
		{
			return this._tabId;
		}
		set
		{
			this._tabId = value;
		}
	}

	public void SetListContent(List<XElement> itemsContents)
	{
		this.lblGonggao.gameObject.SetActive(false);
		string empty = string.Empty;
		string empty2 = string.Empty;
		this._ItemCollection.Clear();
		for (int i = 0; i < itemsContents.Count; i++)
		{
			BaoDianListItem baoDianListItem = U3DUtils.NEW<BaoDianListItem>();
			this._ItemCollection.AddNoUpdate(baoDianListItem);
			baoDianListItem.ItemName = Global.GetXElementAttributeStr(itemsContents[i], "Name");
			baoDianListItem.ItemDesc = Global.GetXElementAttributeStr(itemsContents[i], "Depict");
			baoDianListItem.iconTexture = Global.GetXElementAttributeStr(itemsContents[i], "Image");
			baoDianListItem.RecommondValue = Global.GetXElementAttributeInt(itemsContents[i], "Star");
			baoDianListItem.ActionType = BaoDianPartPage.ParseAction(itemsContents[i], out empty);
			baoDianListItem.ActionValue = empty;
			baoDianListItem.CheckOk = BaoDianPartPage.ConditionCheck(itemsContents[i], out empty2);
			baoDianListItem.NeedTitle = empty2;
			baoDianListItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(s, e);
				}
			};
		}
	}

	public void SetNoticeInfo(List<string> notices)
	{
		this.lblGonggao.gameObject.SetActive(true);
		for (int i = 0; i < notices.Count; i++)
		{
			string text = notices[i];
			UILabel uilabel = this.lblGonggao;
			uilabel.text = uilabel.text + text + "\n\n";
		}
	}

	public static int ParseAction(XElement confEle, out string action)
	{
		action = Global.GetXElementAttributeStr(confEle, "Link");
		int result;
		if (action.Length > 0)
		{
			result = 0;
		}
		else
		{
			action = Global.GetXElementAttributeStr(confEle, "GoTo");
			if (action.Length > 0)
			{
				result = 1;
			}
			else
			{
				result = -1;
			}
		}
		return result;
	}

	public static bool ConditionCheck(XElement confEle, out string desc)
	{
		bool flag = false;
		desc = string.Empty;
		switch (Global.GetXElementAttributeInt(confEle, "Type"))
		{
		case 1:
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(confEle, "NeedZhuanSheng");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(confEle, "NeedLevel");
			int num = Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level;
			int num2 = xelementAttributeInt * 100 + xelementAttributeInt2;
			flag = (num >= num2);
			if (!flag)
			{
				desc = string.Concat(new object[]
				{
					Global.GetLang("需要等级\n["),
					xelementAttributeInt,
					Global.GetLang("转]"),
					xelementAttributeInt2,
					Global.GetLang("级")
				});
			}
			break;
		}
		case 2:
		{
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(confEle, "NeedTask");
			flag = (Global.Data.roleData.CompletedMainTaskID >= xelementAttributeInt3);
			if (!flag)
			{
				desc = Global.GetLang("需要完成任务\n") + Global.GetTaskTitleByID(xelementAttributeInt3);
			}
			break;
		}
		case 3:
		{
			int viplevel = Global.Data.roleData.VIPLevel;
			int xelementAttributeInt4 = Global.GetXElementAttributeInt(confEle, "NeedVIPLevel");
			flag = (viplevel >= xelementAttributeInt4);
			if (!flag)
			{
				desc = Global.GetLang("需要VIP") + xelementAttributeInt4;
			}
			break;
		}
		}
		return flag;
	}

	public ListBox listBox;

	private ObservableCollection _ItemCollection;

	public UILabel lblGonggao;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _tabId;
}
