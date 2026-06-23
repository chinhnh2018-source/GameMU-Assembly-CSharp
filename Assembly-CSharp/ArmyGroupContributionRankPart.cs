using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ArmyGroupContributionRankPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		GameInstance.Game.SendGetArmyGroupPointPaiHang();
		GameInstance.Game.SendGetRoleArmyGroupData(Global.Data.RoleID);
	}

	private void InitPrefabText()
	{
		this.TitleLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("贡献排行榜")
		});
		string[] array = new string[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("排行")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("军团")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("本周贡献")
			})
		};
		byte b = 0;
		while ((int)b < this.TitleClassLabels.Length)
		{
			this.TitleClassLabels[(int)b].text = array[(int)b];
			this.TitleClassLabels[(int)b].transform.localPosition = new Vector3(0f, 0f, -1.52f);
			b += 1;
		}
		this.Inflabels[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			Global.GetLang("每周六24点排行前4名获得领地争夺资格")
		});
		this.Inflabels[0].pivot = 3;
		this.Inflabels[0].transform.localPosition = new Vector3(-218f, 29f, 0f);
		this.Inflabels[0].lineWidth = 440;
		this.Inflabels[1].pivot = 5;
		this.Inflabels[1].transform.localPosition = new Vector3(223f, 0f, 0f);
		this.Inflabels[1].lineWidth = 440;
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.mObc = this.ViewListBox.ItemsSource;
		this.SetGongXian(0);
		this.SetRank(0);
	}

	private void SetGongXian(int num)
	{
		this.Inflabels[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("本周贡献：") + num
		});
	}

	private void SetRank(int num)
	{
		if (0 > num)
		{
			this.Inflabels[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("当前排名：") + Global.GetLang("未上榜")
			});
		}
		else
		{
			this.Inflabels[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("当前排名：") + num
			});
		}
	}

	private ArmyGroupRankItem GetItem(int i)
	{
		ArmyGroupRankItem result = null;
		GameObject at = this.mObc.GetAt(i);
		if (null != at)
		{
			result = at.GetComponent<ArmyGroupRankItem>();
		}
		return result;
	}

	private IEnumerator RefreshRankUI(List<JunTuanRankData> data)
	{
		Super.ShowNetWaiting(null);
		yield return null;
		int Count = data.Count;
		for (byte i = 0; i < 25; i += 1)
		{
			ArmyGroupRankItem item = this.GetItem((int)i);
			if (null == item)
			{
				item = U3DUtils.NEW<ArmyGroupRankItem>();
			}
			if ((int)i < Count && data[(int)i] != null)
			{
				string[] str = new string[]
				{
					(i <= 2) ? data[(int)i].Rank.ToString() : Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7ff",
						data[(int)i].Rank.ToString()
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						(i <= 3) ? "17e43e" : "fdf7ff",
						data[(int)i].JunTuanName
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7ff",
						data[(int)i].Point.ToString()
					})
				};
				item.SetContent(str);
			}
			else
			{
				string[] str2 = new string[]
				{
					(i <= 2) ? ((int)(i + 1)).ToString() : Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7ff",
						((int)(i + 1)).ToString()
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						(i <= 3) ? "17e43e" : "fdf7ff",
						Global.GetLang("暂无")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7ff",
						"0"
					})
				};
				item.SetContent(str2);
			}
			this.mObc.Add(item);
			item.DraggablePanel = this.ViewDraggablePanel;
			if (i % 3 == 0)
			{
				yield return null;
			}
		}
		yield return null;
		this.ViewDraggablePanel.Press(false);
		Super.HideNetWaiting();
		yield break;
	}

	public void RefreshRank(List<JunTuanRankData> data)
	{
		if (data == null)
		{
			data = new List<JunTuanRankData>();
			this.SetRank(-1);
			this.SetRank(0);
		}
		data.Sort((JunTuanRankData a, JunTuanRankData b) => a.Rank - b.Rank);
		base.StartCoroutine<bool>(this.RefreshRankUI(data));
		JunTuanRankData junTuanRankData = data.Find((JunTuanRankData e) => e.JunTuanId == Global.Data.roleData.JunTuanId);
		if (junTuanRankData != null)
		{
			this.SetGongXian(junTuanRankData.Point);
			this.SetRank(junTuanRankData.Rank);
		}
		else
		{
			this.SetRank(0);
			this.SetRank(-1);
		}
	}

	public void RefreshJunTuanGongxian(JunTuanData data)
	{
		if (data != null)
		{
			this.SetGongXian(data.WeekPoint);
		}
		else
		{
			this.SetGongXian(0);
		}
	}

	public int MPoint
	{
		get
		{
			return this.mPoint;
		}
		set
		{
			this.mPoint = value;
		}
	}

	public GButton Closebtn;

	public UILabel TitleLabel;

	public UILabel[] TitleClassLabels;

	public ListBox ViewListBox;

	public UIDraggablePanel ViewDraggablePanel;

	public UILabel[] Inflabels;

	private ObservableCollection mObc;

	private int mPoint;
}
