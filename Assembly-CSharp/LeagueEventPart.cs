using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class LeagueEventPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.totalCollection = this.totalBox.ItemsSource;
		this.juanzCollection = this.juanzBox.ItemsSource;
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.transform.parent = null;
			Object.Destroy(base.transform.gameObject);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, null);
			}
		};
		this.changeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.totalBox.gameObject.activeInHierarchy)
			{
				this.totalBox.gameObject.SetActive(false);
				this.juanzBox.gameObject.SetActive(true);
			}
			else
			{
				this.totalBox.gameObject.SetActive(true);
				this.juanzBox.gameObject.SetActive(false);
			}
			this.scrollBar.scrollValue = 0f;
		};
		this.request();
		base.StartCoroutine<bool>(this.CheckBarValue());
	}

	public void refresh(List<ZhanMengShiJianData> list)
	{
		if (list == null)
		{
			this.canRefresh = false;
			return;
		}
		string text = "{00FF00}";
		string text2 = "{FF0000}";
		string text3 = "{DEC69C}";
		string text4 = "{FF9D08}";
		for (int i = 0; i < list.Count; i++)
		{
			ZhanMengShiJianData zhanMengShiJianData = list[i];
			string eventStr = string.Empty;
			int num = 1;
			switch (zhanMengShiJianData.ShiJianType)
			{
			case 0:
				eventStr = string.Concat(new string[]
				{
					text,
					zhanMengShiJianData.RoleName,
					"{-}",
					text3,
					Global.GetLang("创建了战盟"),
					"{-}"
				});
				break;
			case 1:
				eventStr = string.Concat(new string[]
				{
					text,
					zhanMengShiJianData.RoleName,
					"{-}",
					text2,
					Global.GetLang("离开了战盟"),
					"{-}"
				});
				break;
			case 2:
				eventStr = string.Concat(new string[]
				{
					text,
					zhanMengShiJianData.RoleName,
					"{-}",
					text3,
					Global.GetLang("加入了战盟"),
					"{-}"
				});
				break;
			case 3:
			{
				string text5 = string.Empty;
				if (zhanMengShiJianData.SubValue2 == 1)
				{
					text5 = Global.GetLang("金币");
				}
				else if (zhanMengShiJianData.SubValue2 == 2)
				{
					text5 = Global.GetLang("钻石");
				}
				if (zhanMengShiJianData.SubValue3 == -1)
				{
					eventStr = string.Concat(new object[]
					{
						text,
						zhanMengShiJianData.RoleName,
						"{-}",
						text3,
						Global.GetLang("捐赠了"),
						"{-}",
						text,
						zhanMengShiJianData.SubValue1,
						"{-}",
						text3,
						text5,
						"{-}"
					});
				}
				else
				{
					eventStr = string.Concat(new object[]
					{
						text,
						zhanMengShiJianData.RoleName,
						"{-}",
						text3,
						Global.GetLang("捐赠了"),
						"{-}",
						text,
						zhanMengShiJianData.SubValue1,
						"{-}",
						text3,
						text5,
						",",
						Global.GetLang("获得"),
						"{-}",
						text,
						zhanMengShiJianData.SubValue3,
						"{-}",
						text3,
						Global.GetLang("战功"),
						"{-}"
					});
				}
				num = 2;
				break;
			}
			case 4:
				eventStr = string.Concat(new string[]
				{
					text,
					zhanMengShiJianData.RoleName,
					"{-}",
					text3,
					Global.GetLang("成为了"),
					"{-}",
					text4,
					Global.GetBHZhiWu(zhanMengShiJianData.SubValue1),
					"{-}"
				});
				break;
			case 5:
				eventStr = Global.GetLang("先不做");
				break;
			case 6:
			{
				string text5 = Global.GetLang("道具");
				if (zhanMengShiJianData.SubValue3 == -1)
				{
					eventStr = string.Concat(new string[]
					{
						text,
						zhanMengShiJianData.RoleName,
						"{-}",
						text3,
						Global.GetLang("捐赠了"),
						"{-}",
						text,
						Global.GetGoodsNameByID(zhanMengShiJianData.SubValue1, false),
						"{-}",
						text3,
						text5,
						"{-}"
					});
				}
				else
				{
					eventStr = string.Concat(new object[]
					{
						text,
						zhanMengShiJianData.RoleName,
						"{-}",
						text3,
						Global.GetLang("捐赠了"),
						"{-}",
						text,
						Global.GetGoodsNameByID(zhanMengShiJianData.SubValue1, false),
						"{-}",
						text3,
						text5,
						",",
						Global.GetLang("获得"),
						"{-}",
						text,
						zhanMengShiJianData.SubValue3,
						"{-}",
						text3,
						Global.GetLang("战功"),
						"{-}"
					});
				}
				break;
			}
			case 7:
			{
				MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(zhanMengShiJianData.SubValue1);
				int npcorMonsterMapCodeByID = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID.MapCode);
				string mapNameByCode = ConfigSettings.GetMapNameByCode(npcorMonsterMapCodeByID, false);
				string sname = monsterXmlNodeByID.SName;
				eventStr = string.Concat(new string[]
				{
					text,
					zhanMengShiJianData.RoleName,
					"{-}",
					text3,
					Global.GetLang("击杀了战盟BOSS"),
					"{-}",
					text3,
					Global.GetLang("【"),
					text,
					Global.GetLang(sname),
					text3,
					Global.GetLang("】")
				});
				break;
			}
			case 8:
				eventStr = string.Concat(new string[]
				{
					text,
					zhanMengShiJianData.RoleName,
					"{-}",
					text3,
					Global.GetLang(Global.GetLang("将战盟改名为")),
					text,
					zhanMengShiJianData.SubSzValue1,
					"{-}{-}"
				});
				break;
			}
			for (int j = 0; j < num; j++)
			{
				LeagueEventItem leagueEventItem = U3DUtils.NEW<LeagueEventItem>();
				leagueEventItem.EventStr = eventStr;
				leagueEventItem.TimeStr = zhanMengShiJianData.CreateTime;
				if (j == 1)
				{
					this.juanzCollection.Add(leagueEventItem);
				}
				else
				{
					this.totalCollection.Add(leagueEventItem);
				}
				UIPanel component = leagueEventItem.gameObject.GetComponent<UIPanel>();
				if (null != component)
				{
					Object.Destroy(component);
				}
			}
		}
		this.currPage++;
		base.StartCoroutine<bool>(this.Continue());
	}

	private IEnumerator Continue()
	{
		yield return new WaitForSeconds(0.5f);
		this.canRefresh = true;
		yield break;
	}

	private IEnumerator CheckBarValue()
	{
		for (;;)
		{
			if (this.canRefresh && this.scrollBar.scrollValue > 0.98f)
			{
				this.request();
			}
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	private void request()
	{
		this.canRefresh = false;
		if (this.currPage > 9)
		{
			return;
		}
		GameInstance.Game.SpriteGetZhanMengShiJianDetailCmd(this.currPage);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton closeBtn;

	public ListBox totalBox;

	public ListBox juanzBox;

	public UIScrollBar scrollBar;

	public GButton changeBtn;

	private ObservableCollection totalCollection;

	private ObservableCollection juanzCollection;

	private bool canRefresh;

	private int currPage;
}
