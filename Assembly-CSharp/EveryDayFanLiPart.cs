using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class EveryDayFanLiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_LabDangQIanXiaoFeiNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			0
		});
		this.m_LabDangQIanChongZhiNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			0
		});
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		};
		this.m_BtnLingQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang("同一合服区内，同一账号下，每日只能领取一次，确认是否领取"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					GameInstance.Game.GetChongZhiJiangLi(Global.Data.roleData.RoleID, 48, 0);
				}
				return true;
			};
		};
		this.m_BtnLingQu.Text = Global.GetLang("领取");
		this.m_LabDangQIanChongZhiTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("今日账号充值：")
		});
		this.m_LabDangQIanXiaoFeiTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("当前角色消费：")
		});
		this.m_Obser = this.m_ListBox.ItemsSource;
		this.InitXml();
		this.AddList(this.m_ListXml);
		GameInstance.Game.QueryPayActiveInfo(Global.Data.roleData.RoleID, 48);
	}

	private void InitXml()
	{
		XElement gameResXml = Global.GetGameResXml("Config/SanZhouNian_ChongZhiFanLi.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "SanZhouNian_ChongZhiFanLi");
		string[] array = Global.GetXElementAttributeStr(xelementList[0], "ChongZhiJinE").Split(new char[]
		{
			','
		});
		string[] array2 = Global.GetXElementAttributeStr(xelementList[0], "FanZuanShuLiang").Split(new char[]
		{
			','
		});
		string[] array3 = Global.GetXElementAttributeStr(xelementList[0], "XiaoFeiZuanShi").Split(new char[]
		{
			','
		});
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[0], "HuoDongKaiQi");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelementList[0], "HuoDongGuanBi");
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[0], "HuoDongKaiGuan");
		DateTime dateTime = DateTime.Parse(xelementAttributeStr);
		DateTime dateTime2 = DateTime.Parse(xelementAttributeStr2);
		this.m_LabTime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			Global.GetLang(string.Format(Global.GetLang("活动时间:{0}-{1}"), dateTime.ToString(Global.GetLang("yyyy年MM月dd日")), dateTime2.ToString(Global.GetLang("yyyy年MM月dd日"))))
		});
		int num = array.Length;
		this.m_ListChongZhi.Add(-1);
		for (int i = 0; i < num; i++)
		{
			SanZhouNian_ChongZhiFanLiVO sanZhouNian_ChongZhiFanLiVO = new SanZhouNian_ChongZhiFanLiVO();
			sanZhouNian_ChongZhiFanLiVO.ChongZhiJinE = array[i].SafeToInt32(0);
			sanZhouNian_ChongZhiFanLiVO.FanZuanShuLiang = array2[i].SafeToInt32(0);
			sanZhouNian_ChongZhiFanLiVO.XiaoFeiZuanShi = array3[i].SafeToInt32(0);
			sanZhouNian_ChongZhiFanLiVO.HuoDongKaiQi = xelementAttributeStr;
			sanZhouNian_ChongZhiFanLiVO.HuoDongGuanBi = xelementAttributeStr2;
			sanZhouNian_ChongZhiFanLiVO.HuoDongKaiGuan = xelementAttributeInt;
			this.m_ListXml.Add(sanZhouNian_ChongZhiFanLiVO);
			this.m_ListChongZhi.Add(sanZhouNian_ChongZhiFanLiVO.ChongZhiJinE);
		}
	}

	private void AddList(List<SanZhouNian_ChongZhiFanLiVO> list)
	{
		this.m_Obser.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			EveryDayFanLiItem everyDayFanLiItem = U3DUtils.NEW<EveryDayFanLiItem>();
			everyDayFanLiItem.ChongZhi = list[i].ChongZhiJinE;
			everyDayFanLiItem.FanZuan = list[i].FanZuanShuLiang;
			everyDayFanLiItem.XiaoHao = list[i].XiaoFeiZuanShi;
			everyDayFanLiItem.Img = string.Format("zuanshi_{0}", (i + 1).ToString());
			this.m_Obser.AddNoUpdate(everyDayFanLiItem);
		}
		Vector3 localPosition = this.m_ListBox.transform.localPosition;
		localPosition.x -= (float)(this.m_Obser.Count - 1) * this.m_ListBox.cellWidth / 2f;
		this.m_ListBox.transform.localPosition = localPosition;
	}

	private void OnItemRefresh(int chongZhiNumber, int input, int consume)
	{
		this.m_LabDangQIanXiaoFeiNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			consume
		});
		this.m_LabDangQIanChongZhiNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			input
		});
		for (int i = this.m_ListXml.Count - 1; i >= 0; i--)
		{
			if (this.m_ListXml[i].ChongZhiJinE == chongZhiNumber && chongZhiNumber > 0)
			{
				this.m_LabEveryNumber.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang(string.Format(Global.GetLang("今日可领取{0}次"), Global.GetColorStringForNGUIText(new object[]
					{
						"f0f0f0",
						0
					})))
				});
				this.m_BtnLingQu.isEnabled = false;
			}
			if (input >= this.m_ListXml[i].ChongZhiJinE && consume >= this.m_ListXml[i].XiaoFeiZuanShi && chongZhiNumber <= 0 && this.texiao == null)
			{
				if (i >= this.m_ListXml.Count - 2)
				{
					if (this.m_Obser.GetAt(i).GetComponent<EveryDayFanLiItem>() != null)
					{
						this.texiao = Global.LoadTeXiaoObj("UITeXiao/Chongzhi/chongzhi_fanli_da", this.m_Obser.GetAt(i).GetComponent<EveryDayFanLiItem>().transform);
						this.texiao.transform.localPosition = new Vector3(0f, 26f, -0.3f);
					}
				}
				else if (this.m_Obser.GetAt(i).GetComponent<EveryDayFanLiItem>() != null)
				{
					this.texiao = Global.LoadTeXiaoObj("UITeXiao/Chongzhi/chongzhi_fanli_xiao", this.m_Obser.GetAt(i).GetComponent<EveryDayFanLiItem>().transform);
					this.texiao.transform.localPosition = new Vector3(0f, 26f, -0.3f);
				}
				this.m_LabEveryNumber.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang(string.Format(Global.GetLang("今日可领取{0}次"), Global.GetColorStringForNGUIText(new object[]
					{
						"f0f0f0",
						1
					})))
				});
				this.m_BtnLingQu.isEnabled = true;
				break;
			}
			if (i <= 0 && chongZhiNumber <= 0)
			{
				this.m_LabEveryNumber.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang(string.Format(Global.GetLang("今日可领取{0}次"), Global.GetColorStringForNGUIText(new object[]
					{
						"f0f0f0",
						1
					})))
				});
				this.m_BtnLingQu.isEnabled = false;
			}
		}
	}

	public void RefreshData(int state, string data)
	{
		int input = data.Split(new char[]
		{
			','
		})[0].SafeToInt32(0);
		int consume = data.Split(new char[]
		{
			','
		})[1].SafeToInt32(0);
		this.OnItemRefresh(this.m_ListChongZhi[state], input, consume);
	}

	public void LingQuData(int ret)
	{
		if (ret == 0)
		{
			this.m_BtnLingQu.isEnabled = false;
			this.m_LabEveryNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang(string.Format(Global.GetLang("今日可领取{0}次"), Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					0
				})))
			});
			if (this.texiao != null)
			{
				Object.Destroy(this.texiao.gameObject);
				this.texiao = null;
			}
		}
		else
		{
			StdErrorCode.GetErrMsg(ret, false, false);
		}
	}

	public UILabel m_LabTime;

	public UILabel m_LabEveryNumber;

	public UILabel m_LabDangQIanXiaoFeiTitle;

	public UILabel m_LabDangQIanXiaoFeiNumber;

	public UILabel m_LabDangQIanChongZhiTitle;

	public UILabel m_LabDangQIanChongZhiNumber;

	public GButton m_BtnLingQu;

	public GButton m_BtnClose;

	public ListBox m_ListBox;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection m_Obser;

	private List<SanZhouNian_ChongZhiFanLiVO> m_ListXml = new List<SanZhouNian_ChongZhiFanLiVO>();

	private List<int> m_ListChongZhi = new List<int>();

	private GameObject texiao;
}
