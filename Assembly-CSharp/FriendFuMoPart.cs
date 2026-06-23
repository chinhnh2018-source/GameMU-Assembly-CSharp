using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class FriendFuMoPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitText();
		GameInstance.Game.SenFuMoMailList();
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				Type = 0
			});
		};
		this.m_BtnTiaoZhuan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				Type = 1
			});
		};
		this.m_BtnKuaiSu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_Observable == null || this.m_Observable.Count <= 0)
			{
				Super.HintMainText(Global.GetLang("当前没有可领取的附魔币，快去找好友赠送吧~~"), 10, 3);
				return;
			}
			this.m_LinQuType = 1;
			this.m_MailID = -1;
			GameInstance.Game.SenFuMoLingQu(this.m_LinQuType, this.m_MailID);
		};
		this.m_BtnLingQuClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_PanelLingQu.gameObject.SetActive(false);
		};
		this.m_BtnLingQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SenFuMoLingQu(this.m_LinQuType, this.m_MailID);
		};
		this.m_Observable = this.m_ListBox.ItemsSource;
	}

	private void InitText()
	{
		try
		{
			this.m_BtnLingQu.Label.text = Global.GetLang("领取");
			this.m_DraggablePanel.onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
			this.m_BtnKuaiSu.Text = Global.GetLang("快速领取");
			this.m_BtnTiaoZhuan.Text = Global.GetLang("装备附魔");
			this.m_LabLingQuTitle1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("标题：")
			});
			this.m_LabLingQuTitle2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("好友给你赠送了附魔灵石")
			});
			this.m_LabLingQuContent1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("内容：")
			});
			this.m_LabLingQuContent2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("您的好友赠送了附魔灵石给您，快快来收下TA的心意吧~可以到装备附魔中消耗附魔灵石。")
			});
			this.m_LabLingQuName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("好友邮件")
			});
			this.m_LabGoodsName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("附魔灵石")
			});
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"东南亚英文调试用：" + base.GetType().Name + "报空！"
			});
		}
	}

	private void onDragFinished()
	{
		float cellWidth = this.m_ListBox.cellWidth;
		if (Math.Abs(Math.Abs(this.m_Spring.transform.localPosition.x) - cellWidth * (float)this.m_ItemCount * 5f) > 300f)
		{
			if (this.m_Spring.transform.localPosition.x > -(cellWidth * (float)this.m_ItemCount * 5f))
			{
				this.m_ItemCount--;
				if (this.m_ItemCount <= 0)
				{
					this.m_ItemCount = 0;
				}
			}
			else
			{
				this.m_ItemCount++;
				if (this.m_ItemCount >= this.m_MaxPage - 1)
				{
					this.m_ItemCount = this.m_MaxPage - 1;
				}
			}
		}
		this.m_Spring.target.x = (float)(-(float)this.m_ItemCount) * cellWidth * 5f;
		this.m_Spring.enabled = true;
		this.RefreshPage();
	}

	private void AddPage(int count)
	{
		this.m_MaxPage = count;
		float num = 50f;
		for (int i = 0; i < this.m_ListSpPage.Count; i++)
		{
			Object.Destroy(this.m_ListSpPage[i].gameObject);
		}
		this.m_ListSpPage.Clear();
		for (int j = 0; j < count; j++)
		{
			GameObject gameObject = U3DUtils.Clone(this.m_ListPage.gameObject, this.m_GamePage.gameObject);
			this.m_ListPage.AddNoUpdate(gameObject);
			gameObject.SetActive(true);
			float num2 = (float)j * num - (float)(count - 1) * num / 2f;
			gameObject.transform.localPosition = new Vector3(num2, 0f, -0.2f);
			if (gameObject.GetComponent<UISprite>() != null)
			{
				this.m_ListSpPage.Add(gameObject.GetComponent<UISprite>());
			}
		}
		this.m_GamePage.SetActive(false);
	}

	private void RefreshPage()
	{
		if (this.m_ItemCount > this.m_ListSpPage.Count - 1)
		{
			this.m_ItemCount = this.m_ListSpPage.Count - 1;
		}
		else if (this.m_ItemCount < 0)
		{
			this.m_ItemCount = 0;
		}
		for (int i = 0; i < this.m_ListSpPage.Count; i++)
		{
			if (this.m_ItemCount != i)
			{
				this.m_ListSpPage[i].spriteName = "selectState_normal";
			}
			else
			{
				this.m_ListSpPage[i].spriteName = "selectState_hover";
			}
		}
	}

	public void LingQuRefresh(int num)
	{
		this.m_PanelLingQu.gameObject.SetActive(false);
		if (this.m_MailID != -1)
		{
			if (this.m_Dic.Count > 0 && this.m_Dic.ContainsKey(Global.Data.roleData.RoleID))
			{
				List<FuMoMailData> list = this.m_Dic[Global.Data.roleData.RoleID];
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].MaillID == this.m_MailID)
					{
						list.RemoveAt(i);
						break;
					}
				}
				this.AddList(this.m_Dic);
			}
		}
		else
		{
			GameInstance.Game.SenFuMoMailList();
		}
	}

	public void GetDataRefreshFuMoIsRed()
	{
		for (int i = 0; i < this.m_Observable.Count; i++)
		{
			FriendFuMoItem component = this.m_Observable.GetAt(i).GetComponent<FriendFuMoItem>();
			if (component != null && this.m_MailID == component.MailID)
			{
				component.IsRed = false;
			}
		}
	}

	public void AddList(Dictionary<int, List<FuMoMailData>> gdDic)
	{
		this.m_Dic = gdDic;
		this.m_Observable.Clear();
		if (Global.Data.roleData != null && gdDic != null && gdDic.ContainsKey(Global.Data.roleData.RoleID))
		{
			List<FuMoMailData> list = gdDic[Global.Data.roleData.RoleID];
			if (list == null)
			{
				return;
			}
			if (list.Count % 5 > 0)
			{
				this.AddPage(list.Count / 5 + 1);
			}
			else
			{
				this.AddPage(list.Count / 5);
			}
			for (int i = 0; i < this.m_MaxPage * 5; i++)
			{
				FriendFuMoItem friendFuMoItem = U3DUtils.NEW<FriendFuMoItem>();
				this.m_Observable.AddNoUpdate(friendFuMoItem);
				if (i >= list.Count)
				{
					if (friendFuMoItem.GetComponent<UIPanel>() != null)
					{
						Object.Destroy(friendFuMoItem.GetComponent<UIPanel>());
					}
					friendFuMoItem.m_GamePanel.SetActive(false);
				}
				else
				{
					friendFuMoItem.Name = list[i].SenderRName;
					friendFuMoItem.MailID = list[i].MaillID;
					friendFuMoItem.RoleID = list[i].ReceiverRID;
					friendFuMoItem.Img = StringUtil.substitute("NetImages/Face/{0}0_0.png", new object[]
					{
						Global.CalcOriginalOccupationID(list[i].SenderJob)
					});
					friendFuMoItem.Cotent = list[i].Content;
					if (list[i].IsRead == 1)
					{
						friendFuMoItem.IsRed = false;
					}
					else
					{
						friendFuMoItem.IsRed = true;
					}
					if (friendFuMoItem.GetComponent<UIPanel>() != null)
					{
						Object.Destroy(friendFuMoItem.GetComponent<UIPanel>());
					}
					DateTime correctDateTime = Global.GetCorrectDateTime();
					DateTime dateTime = DateTime.Parse(list[i].SendTime);
					DateTime dateTime2;
					dateTime2..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, 1, 1, 1);
					DateTime dateTime3;
					dateTime3..ctor(dateTime.Year, dateTime.Month, dateTime.Day, 1, 1, 1);
					int days = (dateTime2 - dateTime3).Days;
					friendFuMoItem.Time = days;
				}
			}
			this.RefreshPage();
			this.m_Spring.target.x = (float)(-(float)this.m_ItemCount) * this.m_ListBox.cellWidth * 5f;
			this.m_Spring.enabled = true;
		}
		this.m_ListBox.SelectionChanged = delegate(object s, MouseEvent e)
		{
			this.m_LinQuType = 0;
			FriendFuMoItem friendFuMoItem2 = U3DUtils.AS<FriendFuMoItem>(this.m_ListBox.SelectedItem);
			if (friendFuMoItem2 != null)
			{
				this.m_MailID = friendFuMoItem2.MailID;
				if (this.m_MailID <= 0)
				{
					return;
				}
				this.m_PanelLingQu.gameObject.SetActive(true);
				GameInstance.Game.SenFuMoIsRead(this.m_MailID);
				MUDebug.Log<string>(new string[]
				{
					"MailID:" + this.m_MailID
				});
			}
		};
	}

	[SerializeField]
	private GButton m_BtnClose;

	[SerializeField]
	private GButton m_BtnKuaiSu;

	[SerializeField]
	private GButton m_BtnTiaoZhuan;

	[SerializeField]
	private ListBox m_ListBox;

	[SerializeField]
	private UIPanel m_PanelLingQu;

	[SerializeField]
	private GButton m_BtnLingQuClose;

	[SerializeField]
	private GButton m_BtnLingQu;

	[SerializeField]
	private UILabel m_LabLingQuName;

	[SerializeField]
	private UILabel m_LabLingQuTitle1;

	[SerializeField]
	private UILabel m_LabLingQuTitle2;

	[SerializeField]
	private UILabel m_LabLingQuContent1;

	[SerializeField]
	private UILabel m_LabLingQuContent2;

	[SerializeField]
	private UIDraggablePanel m_DraggablePanel;

	[SerializeField]
	private SpringPanel m_Spring;

	private int m_ItemCount;

	private ObservableCollection m_Observable;

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<UISprite> m_ListSpPage = new List<UISprite>();

	private int m_LinQuType = -1;

	public ListBox m_ListPage;

	public GameObject m_GamePage;

	[SerializeField]
	private UILabel m_LabGoodsName;

	private int m_MaxPage;

	private int m_MailID = -1;

	private Dictionary<int, List<FuMoMailData>> m_Dic = new Dictionary<int, List<FuMoMailData>>();
}
