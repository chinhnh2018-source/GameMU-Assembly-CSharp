using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;

public class HefuPartLuolanZhengba : UserControl
{
	public ObservableCollection ItemCollectionHaoli
	{
		get
		{
			return this._ItemCollectionHaoli;
		}
		set
		{
			this._ItemCollectionHaoli = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.labChengzhu.text = Global.GetLang("罗兰城主奖励：");
		this.labGuizu.text = Global.GetLang("罗兰贵族奖励：");
		this.btnJoin.Text = Global.GetLang("参与活动");
		this.staticText.Text = Global.GetLang("周三     21:30（战盟需要战盟首领提前竞标参与资格）");
		this.huodongStartime.Z = -0.10000000149011612;
		this.huodongEndtime.Z = -0.10000000149011612;
		this.lingquStarttime.Z = -0.10000000149011612;
		this.lingquEndtime.Z = -0.10000000149011612;
		this.btnJoin.Label.lineWidth = 95;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollectionHaoli = this.haoliList.ItemsSource;
		this.InitTime();
		this.InitItem();
		this.btnJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhamMeng))
			{
				if (string.IsNullOrEmpty(Global.Data.roleData.BHName))
				{
					Super.HintMainText(Global.GetLang("需要加入任意战盟后才可参与"), 10, 3);
				}
				else
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = 208
					});
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("需要开启战盟功能并加入任意战盟后才可参与"), 10, 3);
			}
		};
		GameInstance.Game.QueryPayActiveInfo(Global.Data.roleData.RoleID, 43);
	}

	private void InitTime()
	{
		this.startTime = Global.GetServerMergeHuodongTimeDateTime(0, 0, 0, 0);
		this.endTime = Global.GetServerMergeHuodongTimeDateTime(6, 23, 59, 59);
		this.lingquEndTime = Global.GetServerMergeHuodongTimeDateTime(6, 23, 59, 59);
		DateTime dateTime;
		dateTime..ctor(this.endTime.Year, this.endTime.Month, this.endTime.Day, 0, 0, 0);
		this.startTimeStr = this.startTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.endTimeStr = this.endTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.lingquEndTimeStr = this.lingquEndTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.huodongStartime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.startTimeStr
		});
		this.huodongEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.endTimeStr
		});
		this.lingquStarttime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.startTimeStr
		});
		this.lingquEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.lingquEndTimeStr
		});
	}

	private void InitItem()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HeFuLuoLan.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement == null)
			{
				return;
			}
			HefuPartLuolanZhengbaItem hefuPartLuolanZhengbaItem = U3DUtils.NEW<HefuPartLuolanZhengbaItem>();
			this.ItemCollectionHaoli.Add(hefuPartLuolanZhengbaItem);
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "WinNum");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsOne");
			hefuPartLuolanZhengbaItem.Id = xelementAttributeInt;
			hefuPartLuolanZhengbaItem.WinNum = xelementAttributeInt2;
			hefuPartLuolanZhengbaItem.GoodsIDs = xelementAttributeStr;
			hefuPartLuolanZhengbaItem.state = AwardGiftGainState.CanNotGain;
			this.CurrentItems.Add(hefuPartLuolanZhengbaItem);
			hefuPartLuolanZhengbaItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(s, e);
				}
			};
			if (!this.DicIDtoItem.ContainsKey(xelementAttributeInt))
			{
				this.DicIDtoItem.Add(xelementAttributeInt, hefuPartLuolanZhengbaItem);
			}
		}
	}

	public void GetBtnState(string result, int flag)
	{
		int num = 0;
		int num2 = 0;
		string[] array = result.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length != 2)
			{
				return;
			}
			if (Convert.ToInt32(array2[0]) == Global.Data.roleData.Faction)
			{
				num++;
			}
			if (Convert.ToInt32(array2[1]) == Global.Data.roleData.RoleID)
			{
				num2++;
			}
		}
		int[] array3 = new int[4];
		for (int j = 0; j < this.CurrentItems.Count; j++)
		{
			array3[j] = Global.GetIntSomeBit(flag, j + 1);
		}
		this.SetState(num2, array3, 0);
		this.SetState(num - num2, array3, 2);
	}

	private void SetState(int num, int[] state, int n)
	{
		if (num == 0)
		{
			this.CurrentItems[n].state = AwardGiftGainState.CanNotGain;
			this.CurrentItems[n + 1].state = AwardGiftGainState.CanNotGain;
		}
		if (num == 1)
		{
			if (state[n] == 1)
			{
				this.CurrentItems[n].state = AwardGiftGainState.Gained;
			}
			else
			{
				this.CurrentItems[n].state = AwardGiftGainState.CanGain;
			}
		}
		if (num == 2)
		{
			if (state[n] == 1)
			{
				this.CurrentItems[n].state = AwardGiftGainState.Gained;
			}
			else
			{
				this.CurrentItems[n].state = AwardGiftGainState.CanGain;
			}
			if (state[n + 1] == 1)
			{
				this.CurrentItems[n + 1].state = AwardGiftGainState.Gained;
			}
			else
			{
				this.CurrentItems[n + 1].state = AwardGiftGainState.CanGain;
			}
		}
	}

	public void GetResult(int id)
	{
		HefuPartLuolanZhengbaItem hefuPartLuolanZhengbaItem = null;
		if (this.DicIDtoItem.TryGetValue(id, ref hefuPartLuolanZhengbaItem))
		{
			hefuPartLuolanZhengbaItem.state = AwardGiftGainState.Gained;
		}
	}

	public TextBlock staticText;

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public ListBox haoliList;

	public UIPanel haoliPanel;

	public GButton btnJoin;

	public TextBlock labChengzhu;

	public TextBlock labGuizu;

	private DateTime startTime;

	private DateTime endTime;

	private DateTime lingquEndTime;

	private string startTimeStr;

	private string endTimeStr;

	private string lingquEndTimeStr;

	private ObservableCollection _ItemCollectionHaoli;

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<HefuPartLuolanZhengbaItem> CurrentItems = new List<HefuPartLuolanZhengbaItem>();

	private Dictionary<int, HefuPartLuolanZhengbaItem> DicIDtoItem = new Dictionary<int, HefuPartLuolanZhengbaItem>();
}
