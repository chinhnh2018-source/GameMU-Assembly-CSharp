using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;

public class FuBenTiShiPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.BianQiangXML = Global.GetGameResXml("BianQiang");
		this.BianQiangList = Global.GetXElementList(this.BianQiangXML, "BianQiang");
		this.BianQiangStandardXML = Global.GetGameResXml("Standard");
		this.BianQiangStandardList = Global.GetXElementList(this.BianQiangStandardXML, "BianQiang");
		this.m_LabTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("副本提示")
		});
		this.m_LabTiShi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ff0000",
			Global.GetLang("当前战斗力，挑战该副本难度较大建议提升战斗力后再来挑战")
		});
		this.m_LabTuiJianTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("提升战斗力方式")
		});
		this.m_BtnLeft.Label.text = Global.GetLang("稍后再来");
		this.m_BtnRight.Label.text = Global.GetLang("强行进入");
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.dpsHandlerClose(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.m_BtnLeft.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.dpsHandlerClose(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.m_BtnRight.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.dpsHandler(this, new DPSelectedItemEventArgs
			{
				ID = this.m_TypeID
			});
		};
		this.m_TabOBC = this.m_ListBox.ItemsSource;
		Global.StartRequest();
	}

	public void GetData(int TypeID, int zhanDouLi)
	{
		this.m_TypeID = TypeID;
		this.m_LabTuiJianContent.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("建议战斗力：")
		}) + Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			zhanDouLi
		});
	}

	public void RefreshImprovingView()
	{
		Super.HideNetWaiting();
		this.SetUIList();
	}

	public void SetUIList()
	{
		this.m_TabOBC.Clear();
		this.JiXuListData.Clear();
		this.JiXuListData.AddRange(Global.GetDataList(0));
		if (this.JiXuListData.Count < 4 && Global.GetDataList(1) != null)
		{
			this.JiXuListData.AddRange(Global.GetDataList(1));
		}
		for (int i = 0; i < 4; i++)
		{
			if (i >= this.JiXuListData.Count)
			{
				break;
			}
			FuBenTiShiItem fuBenTiShiItem = U3DUtils.NEW<FuBenTiShiItem>();
			fuBenTiShiItem.Img = "NetImages/GameRes/Images/BianQiang/" + this.JiXuListData[i].ImageName + ".png";
			this.m_TabOBC.AddNoUpdate(fuBenTiShiItem);
			int linkId = this.JiXuListData[i].LinkID;
			fuBenTiShiItem.m_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = linkId
				});
			};
		}
		this.m_EndAddBool = true;
	}

	public UILabel m_LabTitle;

	public UILabel m_LabTiShi;

	public UILabel m_LabTuiJianTitle;

	public UILabel m_LabTuiJianContent;

	public GButton m_BtnClose;

	public GButton m_BtnLeft;

	public GButton m_BtnRight;

	public ListBox m_ListBox;

	public DPSelectedItemEventHandler dpsHandler;

	public DPSelectedItemEventHandler dpsHandlerClose;

	private ObservableCollection m_TabOBC;

	private List<TiShengZhanLiItemVO> JiXuListData = new List<TiShengZhanLiItemVO>();

	private XElement BianQiangXML;

	private List<XElement> BianQiangList;

	private XElement BianQiangStandardXML;

	private List<XElement> BianQiangStandardList;

	private int m_TypeID = -1;

	public bool m_EndAddBool;
}
