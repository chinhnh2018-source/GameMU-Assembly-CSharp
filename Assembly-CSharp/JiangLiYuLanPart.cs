using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class JiangLiYuLanPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_Title.text = Global.GetLang("{ffcc19}月度排行奖励预览");
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
		this.collection = this.m_ListBox.ItemsSource;
	}

	public IEnumerator init(List<XElement> xmlList, int rankId = 0, string titleName = null)
	{
		int counter = 0;
		for (int i = 0; i < xmlList.Count; i++)
		{
			XElement args = xmlList[i];
			if (args != null)
			{
				if (!string.IsNullOrEmpty(titleName))
				{
					this.m_Title.text = "{ffcc19}" + titleName;
				}
				JiangLi item = U3DUtils.NEW<JiangLi>();
				this.collection.Add(item);
				item.IsMyAward = (rankId > 0 && i + 1 == rankId);
				string min = Global.GetXElementAttributeStr(args, "StarRank");
				string max = Global.GetXElementAttributeStr(args, "EndRank");
				if (min == "50001" || max == "-1")
				{
					item.Context.Text = Global.GetLang("{e3b36c}" + StringUtil.substitute(Global.GetLang("第{0}名及以后奖励"), new object[]
					{
						min
					}));
				}
				else if (min == max)
				{
					item.Context.Text = Global.GetLang("{e3b36c}" + StringUtil.substitute(Global.GetLang("第{0}名奖励"), new object[]
					{
						max
					}));
				}
				else
				{
					item.Context.Text = Global.GetLang("{e3b36c}" + StringUtil.substitute(Global.GetLang("第{0}-{1}名奖励"), new object[]
					{
						min,
						max
					}));
				}
				string[] goodses = Global.GetXElementAttributeStr(args, "Award").Split(new char[]
				{
					'|'
				});
				if (goodses != null && goodses.Length > 0)
				{
					item.ShowBg(goodses.Length);
				}
				for (int ids = 0; ids < goodses.Length; ids++)
				{
					string[] goods = goodses[ids].Split(new char[]
					{
						','
					});
					item.initGood(goods, ids);
				}
				counter++;
				if (counter % 4 == 0)
				{
					yield return null;
				}
			}
		}
		yield break;
	}

	public IEnumerator InitNormalAward(List<XElement> xmlList, string titleName)
	{
		this.m_Title.text = "{ffcc19}" + titleName;
		int counter = 0;
		for (int i = 0; i < xmlList.Count; i++)
		{
			XElement args = xmlList[i];
			if (args != null)
			{
				JiangLi item = U3DUtils.NEW<JiangLi>();
				this.collection.Add(item);
				string name = Global.GetXElementAttributeStr(args, "Name");
				item.Context.Text = Global.GetLang("{e3b36c}" + name);
				string[] goodses = Global.GetXElementAttributeStr(args, "Award").Split(new char[]
				{
					'|'
				});
				if (goodses != null && goodses.Length > 0)
				{
					item.ShowBg(goodses.Length);
				}
				for (int ids = 0; ids < goodses.Length; ids++)
				{
					string[] goods = goodses[ids].Split(new char[]
					{
						','
					});
					item.initGood(goods, ids);
				}
				counter++;
				if (counter % 4 == 0)
				{
					yield return null;
				}
			}
		}
		yield break;
	}

	public ListBox m_ListBox;

	public GButton m_CloseBtn;

	public UILabel m_Title;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection collection;
}
