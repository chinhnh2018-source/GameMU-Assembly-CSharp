using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class JingjiAwardPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_LingquBtn.Text = Global.GetLang("领取");
		this.lblRanking.transform.localPosition = new Vector3(-275f, 185f, 0f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					Type = -10
				});
			}
		};
		this.m_LingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.GetBaoGuoSpaceCount() < 1)
			{
				Super.HintMainText(Global.GetLang("领取失败，背包空间不足！"), 10, 3);
				return;
			}
			GameInstance.Game.SpriteJingJiRankingRewardCmd();
		};
		this.collection = this.m_ListBox.ItemsSource;
	}

	public void init(List<XElement> xmlList)
	{
		for (int i = 0; i < xmlList.Count; i++)
		{
			XElement xelement = xmlList[i];
			if (xelement != null)
			{
				JingjiAwardItem jingjiAwardItem = U3DUtils.NEW<JingjiAwardItem>();
				this.collection.Add(jingjiAwardItem);
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "MinRank");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "MaxRank");
				if (xelementAttributeStr2 == "100000")
				{
					jingjiAwardItem.Context.Text = StringUtil.substitute(Global.GetLang("第{0}名后奖励"), new object[]
					{
						xelementAttributeStr
					});
				}
				else if (xelementAttributeStr == xelementAttributeStr2)
				{
					jingjiAwardItem.Context.Text = StringUtil.substitute(Global.GetLang("第{0}名奖励"), new object[]
					{
						xelementAttributeStr2
					});
				}
				else
				{
					jingjiAwardItem.Context.Text = StringUtil.substitute(Global.GetLang("第{0}-{1}名奖励"), new object[]
					{
						xelementAttributeStr,
						xelementAttributeStr2
					});
					if (jingjiAwardItem.Context.Text == "Hạng 501-")
					{
						jingjiAwardItem.Context.Text = "Hạng 501 và về sau";
					}
				}
				string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "ExpCoefficient2");
				string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "ShengWang2");
				string[] array = Global.GetXElementAttributeStr(xelement, "GoodsID").Split(new char[]
				{
					','
				});
				if (array.Length > 0)
				{
					jingjiAwardItem.initGood(array, 0);
				}
				array = ("8002," + xelementAttributeStr3 + ",0,0,0,0,0").Split(new char[]
				{
					','
				});
				jingjiAwardItem.initGood(array, 1);
				array = ("8016," + xelementAttributeStr4 + ",0,0,0,0,0").Split(new char[]
				{
					','
				});
				jingjiAwardItem.initGood(array, 2);
			}
		}
	}

	public void SetLingquInfo(long cd)
	{
		if (cd >= 0L)
		{
			this.m_LingquCDLabel.text = UIHelper.FormatSecs(cd, "-");
			if (this.m_LingquBtn.isEnabled)
			{
				this.m_LingquBtn.isEnabled = false;
			}
		}
		else
		{
			if (this.m_LingquCDLabel.text != Global.GetLang("无"))
			{
				this.m_LingquCDLabel.text = Global.GetLang("无");
			}
			if (!this.m_LingquBtn.isEnabled)
			{
				this.m_LingquBtn.isEnabled = true;
			}
		}
	}

	public void SetRewardGoodIcon(List<GGoodIcon> iconList)
	{
		int num = 0;
		foreach (GGoodIcon ggoodIcon in iconList)
		{
			if (ggoodIcon != null)
			{
				U3DUtils.AddChild(base.gameObject, ggoodIcon.gameObject, true);
				Super.InitGoodsGIcon(ggoodIcon, (GoodsData)ggoodIcon.ItemObject, Global.CanUseGoods(ggoodIcon.ItemCode, false, true), IconTextTypes.Qianghua);
				ggoodIcon.transform.localPosition = new Vector3(-418f + (float)num++ * 90f, -100f, 0f);
			}
		}
	}

	public void SetRankingData(int ranking)
	{
		if (ranking < 1)
		{
			this.lblRanking.text = Global.GetLang("5000名后");
		}
		else
		{
			this.lblRanking.text = string.Empty + ranking;
		}
	}

	public GButton m_CloseBtn;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox m_ListBox;

	private ObservableCollection collection;

	public UILabel lblJingyan;

	public UILabel lblShengwang;

	public UILabel m_LingquCDLabel;

	public GButton m_LingquBtn;

	public UILabel lblRanking;
}
