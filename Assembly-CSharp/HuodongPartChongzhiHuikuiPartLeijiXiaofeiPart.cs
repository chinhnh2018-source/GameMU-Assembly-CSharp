using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class HuodongPartChongzhiHuikuiPartLeijiXiaofeiPart : UserControl
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
		this.m_VecPnl = this.m_Pnl.gameObject.transform.localPosition;
		this.m_VecClipPnl = this.m_Pnl.clipRange;
		GameInstance.Game.QueryPayActiveInfo(Global.Data.roleData.RoleID, 39);
		this.ItemCollection = this.List.ItemsSource;
	}

	public void ReSetUIByCommd(string[] strArr)
	{
		string[] array = strArr[0].Split(new char[]
		{
			','
		});
		if (array == null || array.Length <= 0)
		{
			return;
		}
		int num = Math.Min(this.ItemCollection.Count, array.Length);
		for (int i = 0; i < num; i++)
		{
			GameObject at = this.ItemCollection.GetAt(i);
			if (null != at)
			{
				HuodongPartChongzhiHuikuiPartMeiriChongzhiPartItem component = at.GetComponent<HuodongPartChongzhiHuikuiPartMeiriChongzhiPartItem>();
				component.LingquFlag = Convert.ToInt32(array[i]);
			}
		}
	}

	public void InitPartData(string xmlPath)
	{
		XElement isolateResXml = Global.GetIsolateResXml(xmlPath);
		if (isolateResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(isolateResXml, "GiftList");
		if (xelement == null)
		{
			return;
		}
		this.TxtHint.Text = Global.GetXElementAttributeStr(xelement, "Description");
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Award");
		this.state = new StateObject01();
		this.state.AwardID = 0;
		this.state.LingquState = 0;
		this.showList(xelementList);
	}

	private void showList(List<XElement> xmlList)
	{
		if (xmlList == null)
		{
			return;
		}
		this.ItemCollection.Clear();
		int num = (this.state != null) ? this.state.AwardID : 0;
		for (int i = 0; i < xmlList.Count; i++)
		{
			XElement xelement = xmlList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ShowMinYuanBao");
			if (this.sumConsumedDiamonds < xelementAttributeInt)
			{
				break;
			}
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "ID");
			HuodongPartChongzhiHuikuiPartMeiriChongzhiPartItem item = U3DUtils.NEW<HuodongPartChongzhiHuikuiPartMeiriChongzhiPartItem>();
			item.TxtValue.Text = StringUtil.substitute(Global.GetLang("累计消费{0}钻石"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "MinYuanBao")
			});
			item.TxtValue.gameObject.SetActive(false);
			item.m_TextKey.text = Global.GetLang(string.Empty);
			item.m_LblShowText.text = StringUtil.substitute(Global.GetLang("累计消费{0}钻石"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "MinYuanBao")
			});
			item.GoodsIDs = Global.GetXElementAttributeStr(xelement, "GoodsTwo") + "@" + Global.GetXElementAttributeStr(xelement, "GoodsOne");
			item.SubmitBtn.BtnTag = xelementAttributeInt2.ToString();
			item.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (item.LingquFlag != 1)
				{
					return;
				}
				GameInstance.Game.GetChongZhiJiangLi(Global.Data.roleData.RoleID, 39, Convert.ToInt32(item.SubmitBtn.BtnTag));
			};
			if (this.isLingqu(i))
			{
				item.LingquFlag = 0;
			}
			else if (num >= Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "MinYuanBao")))
			{
				item.LingquFlag = 1;
			}
			else
			{
				item.LingquFlag = -1;
			}
			this.ItemCollection.Add(item);
		}
	}

	private bool isLingqu(int index)
	{
		return false;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox List;

	public TextBlock TxtHint;

	public Vector3 m_VecPnl = default(Vector3);

	public Vector4 m_VecClipPnl = default(Vector4);

	public UIPanel m_Pnl;

	private StateObject01 state;

	public int sumConsumedDiamonds;

	private ObservableCollection _ItemCollection;
}
