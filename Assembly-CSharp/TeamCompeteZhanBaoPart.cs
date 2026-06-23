using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeamCompeteZhanBaoPart : UserControl, IMUEventManagerHandler
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
		this.ItemCollection = this.mListBox.Items;
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.LblTitle.Label.text = Global.GetLang("战队信息");
		this.LblDuanWei.Label.text = Global.GetLang("段位：");
		this.LblShengLv.Label.text = Global.GetLang("胜率：");
		this.LblLianSheng.Label.text = Global.GetLang("连胜：");
		this.LblScore.Label.text = Global.GetLang("段位积分：");
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, null);
			}
		};
	}

	private void InitValue()
	{
		this.RefershSelfInfo(TeamCompeteDataManager.MainZhanDuiData);
		this.RequestLogList();
	}

	private void LoadItems(List<TianTi5v5LogItemData> list)
	{
		if (list == null || list.Count <= 0)
		{
			return;
		}
		string text = "00FF00";
		string text2 = "FF0000";
		string text3 = "DEC69C";
		string text4 = "F9F702";
		for (int i = 0; i < list.Count; i++)
		{
			TianTi5v5LogItemData tianTi5v5LogItemData = list[i];
			string text5 = string.Empty;
			int success = tianTi5v5LogItemData.Success;
			if (success != 0)
			{
				if (success == 1)
				{
					text5 = string.Format(Global.GetLang("你挑战{0},{1},获得段位积分{2}"), Global.GetColorStringForNGUIText(new object[]
					{
						text,
						tianTi5v5LogItemData.RoleName2
					}), Global.GetColorStringForNGUIText(new object[]
					{
						text4,
						Global.GetLang("你胜利了")
					}), Global.GetColorStringForNGUIText(new object[]
					{
						text3,
						tianTi5v5LogItemData.DuanWeiJiFenAward
					}));
				}
			}
			else
			{
				text5 = string.Format(Global.GetLang("你挑战{0},{1},扣除段位积分{2}"), Global.GetColorStringForNGUIText(new object[]
				{
					text,
					tianTi5v5LogItemData.RoleName2
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text2,
					Global.GetLang("你失败了")
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text2,
					Math.Abs(tianTi5v5LogItemData.DuanWeiJiFenAward)
				}));
			}
			ZhanbaoItem zhanbaoItem = U3DUtils.NEW<ZhanbaoItem>();
			zhanbaoItem.transform.localPosition = new Vector3(0f, 0f, -0.01f);
			zhanbaoItem.m_Text.Text = text5;
			this.ItemCollection.Add(zhanbaoItem);
		}
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GET_LOG_LIST", new Action<MUSocketConnectEventArgs>(this.RespondLogList));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GET_LOG_LIST", new Action<MUSocketConnectEventArgs>(this.RespondLogList));
	}

	public void RequestLogList()
	{
		GameInstance.Game.RequestZhanBaoInfoMsg();
	}

	public void RespondLogList(MUSocketConnectEventArgs e)
	{
		List<TianTi5v5LogItemData> list = DataHelper.BytesToObject<List<TianTi5v5LogItemData>>(e.bytesData, 0, e.bytesData.Length);
		this.LoadItems(list);
	}

	private void RefershSelfInfo(TianTi5v5ZhanDuiData data)
	{
		if (data == null)
		{
			return;
		}
		this.LblDuanWei.Text = Global.GetString(new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				this.mColor,
				Global.GetLang("段位：")
			}),
			TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId)
		});
		this.LblShengLv.Text = Global.GetString(new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				this.mColor,
				Global.GetLang("胜率：")
			}),
			this.ShengLv(data.SuccessCount, data.FightCount)
		});
		this.LblLianSheng.Text = Global.GetString(new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				this.mColor,
				Global.GetLang("连胜：")
			}),
			data.LianSheng
		});
		this.LblScore.Text = Global.GetString(new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				this.mColor,
				Global.GetLang("段位积分：")
			}),
			data.DuanWeiJiFen
		});
	}

	private string ShengLv(int lianSheng, int fightCount)
	{
		if (fightCount <= 0)
		{
			return "0%";
		}
		return ((float)lianSheng / (float)fightCount).ToString("p1");
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTitle;

	public TextBlock LblDuanWei;

	public TextBlock LblShengLv;

	public TextBlock LblLianSheng;

	public TextBlock LblScore;

	public GButton BtnClose;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	private string mColor = "dac7ae";
}
