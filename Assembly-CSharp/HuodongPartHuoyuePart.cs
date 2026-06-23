using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class HuodongPartHuoyuePart : UserControl
{
	public ObservableCollection InfoCollection
	{
		get
		{
			return this.infoCollection;
		}
		set
		{
			this.infoCollection = value;
		}
	}

	public ObservableCollection AwardCollection
	{
		get
		{
			return this.awardCollection;
		}
		set
		{
			this.awardCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.infoCollection = this.listBox1.ItemsSource;
		this.initInfoData();
		this.awardCollection = this.listBox2.ItemsSource;
		this.initAwardData();
	}

	private void initInfoData()
	{
		XElement gameResXml = Global.GetGameResXml("Config/DailyActiveInfor.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "DailyActive"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				if (Global.Data.roleData.ChangeLifeCount >= Global.GetXElementAttributeInt(xelement, "MinZhuanshengleve") && Global.Data.roleData.Level >= Global.GetXElementAttributeInt(xelement, "Minleve"))
				{
					HuoyueInfoItem huoyueInfoItem = U3DUtils.NEW<HuoyueInfoItem>();
					this.infoCollection.Add(huoyueInfoItem);
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
					huoyueInfoItem.DesText = xelementAttributeStr;
					string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Award");
					huoyueInfoItem.AwardText = "+" + xelementAttributeStr2;
					huoyueInfoItem.CurrText = "0";
					long num = 0L;
					if (string.Empty != Global.GetXElementAttributeStr(xelement, "Login"))
					{
						num = Global.GetXElementAttributeLong(xelement, "Login");
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "Online"))
					{
						num = Global.GetXElementAttributeLong(xelement, "Online");
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "Consumption"))
					{
						num = Global.GetXElementAttributeLong(xelement, "Consumption");
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "RiChang"))
					{
						num = Global.GetXElementAttributeLong(xelement, "RiChang");
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "KillRaid"))
					{
						num = Global.GetXElementAttributeLong(xelement, "KillRaid");
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "HuoDongLimit"))
					{
						num = Global.GetXElementAttributeLong(xelement, "HuoDongLimit");
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "QiangHuaLimit"))
					{
						num = Global.GetXElementAttributeLong(xelement, "QiangHuaLimit");
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "ZhuiJiaLimit"))
					{
						num = Global.GetXElementAttributeLong(xelement, "ZhuiJiaLimit");
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "KillMonster"))
					{
						num = Global.GetXElementAttributeLong(xelement, "KillMonster");
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "KillBoss"))
					{
						num = Global.GetXElementAttributeLong(xelement, "KillBoss");
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "ZhuanShengLimit"))
					{
						num = Global.GetXElementAttributeLong(xelement, "ZhuanShengLimit");
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "HeChengLimit"))
					{
						string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "HeChengLimit");
						string[] array = xelementAttributeStr3.Split(new char[]
						{
							','
						});
						num = Convert.ToInt64(array[1]);
					}
					huoyueInfoItem.TotalText = StringUtil.substitute("/{0}", new object[]
					{
						num
					});
					UIPanel component = huoyueInfoItem.transform.GetComponent<UIPanel>();
					if (null != component)
					{
						Object.Destroy(component);
					}
				}
			}
		}
	}

	public void refresh()
	{
		if (Global.Data.DailyActiveInfor == null)
		{
			return;
		}
		this.infoCollection.Clear();
		XElement gameResXml = Global.GetGameResXml("Config/DailyActiveInfor.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "DailyActive"), "*");
		List<HuoyueInfoItem> list = new List<HuoyueInfoItem>();
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				if (Global.Data.roleData.ChangeLifeCount >= Global.GetXElementAttributeInt(xelement, "MinZhuanshengleve") && Global.Data.roleData.Level >= Global.GetXElementAttributeInt(xelement, "Minleve"))
				{
					HuoyueInfoItem huoyueInfoItem = U3DUtils.NEW<HuoyueInfoItem>();
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
					huoyueInfoItem.DesText = xelementAttributeStr;
					string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Award");
					if (0 < Global.Data.roleData.VIPLevel)
					{
						string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("VIPHuoYueAdd", ',');
						string text = systemParamStringArrayByName[Global.Data.roleData.VIPLevel];
						string awardText = string.Format("+{0}(+{1})", xelementAttributeStr2, text);
						huoyueInfoItem.AwardText = awardText;
					}
					else
					{
						huoyueInfoItem.AwardText = "+" + xelementAttributeStr2;
					}
					long num = 0L;
					long num2 = 0L;
					if (string.Empty != Global.GetXElementAttributeStr(xelement, "Login"))
					{
						num = Global.GetXElementAttributeLong(xelement, "Login");
						num2 = Global.Data.DailyActiveInfor.DailyActiveTotalLoginCount;
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "Online"))
					{
						num = Global.GetXElementAttributeLong(xelement, "Online");
						num2 = (long)Global.Data.DailyActiveInfor.DailyActiveOnLineTimer;
						num2 /= 60L;
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "Consumption"))
					{
						num = Global.GetXElementAttributeLong(xelement, "Consumption");
						num2 = (long)Global.Data.DailyActiveInfor.BuyItemInMall;
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "RiChang"))
					{
						num = Global.GetXElementAttributeLong(xelement, "RiChang");
						num2 = (long)Global.Data.DailyActiveInfor.CompleteDailyTaskCount;
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "KillRaid"))
					{
						num = Global.GetXElementAttributeLong(xelement, "KillRaid");
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "DailyActiveID");
						if (xelementAttributeInt != 500)
						{
							if (xelementAttributeInt != 600)
							{
								if (xelementAttributeInt == 700)
								{
									num2 = (long)Global.Data.DailyActiveInfor.PassDifficultCopySceneNum;
								}
							}
							else
							{
								num2 = (long)Global.Data.DailyActiveInfor.PassHardCopySceneNum;
							}
						}
						else
						{
							num2 = (long)Global.Data.DailyActiveInfor.PassNormalCopySceneNum;
						}
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "HuoDongLimit"))
					{
						num = Global.GetXElementAttributeLong(xelement, "HuoDongLimit");
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "DailyActiveID");
						if (xelementAttributeInt != 800)
						{
							if (xelementAttributeInt != 900)
							{
								if (xelementAttributeInt == 1000)
								{
									num2 = (long)Global.Data.DailyActiveInfor.CompleteBattleCount;
								}
							}
							else
							{
								num2 = (long)Global.Data.DailyActiveInfor.CompleteDaimonSquareCount;
							}
						}
						else
						{
							num2 = (long)Global.Data.DailyActiveInfor.CompleteBloodCastleCount;
						}
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "QiangHuaLimit"))
					{
						num = Global.GetXElementAttributeLong(xelement, "QiangHuaLimit");
						num2 = (long)Global.Data.DailyActiveInfor.EquipForge;
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "ZhuiJiaLimit"))
					{
						num = Global.GetXElementAttributeLong(xelement, "ZhuiJiaLimit");
						num2 = (long)Global.Data.DailyActiveInfor.EquipAppend;
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "KillMonster"))
					{
						num = Global.GetXElementAttributeLong(xelement, "KillMonster");
						num2 = Global.Data.DailyActiveInfor.TotalKilledMonsterCount;
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "KillBoss"))
					{
						num = Global.GetXElementAttributeLong(xelement, "KillBoss");
						num2 = Global.Data.DailyActiveInfor.TotalKilledBossCount;
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "ZhuanShengLimit"))
					{
						num = Global.GetXElementAttributeLong(xelement, "ZhuanShengLimit");
						num2 = (long)Global.Data.DailyActiveInfor.ChangeLife;
					}
					else if (string.Empty != Global.GetXElementAttributeStr(xelement, "HeChengLimit"))
					{
						string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "HeChengLimit");
						string[] array = xelementAttributeStr3.Split(new char[]
						{
							','
						});
						num = Convert.ToInt64(array[1]);
						num2 = (long)Global.Data.DailyActiveInfor.MergeFruit;
					}
					huoyueInfoItem.TotalText = StringUtil.substitute("/{0}", new object[]
					{
						num
					});
					if (num2 >= num)
					{
						huoyueInfoItem.CurrText = string.Empty + num;
						list.Add(huoyueInfoItem);
					}
					else
					{
						huoyueInfoItem.CurrText = string.Empty + num2;
						this.infoCollection.Add(huoyueInfoItem);
						UIPanel component = huoyueInfoItem.transform.GetComponent<UIPanel>();
						if (null != component)
						{
							Object.Destroy(component);
						}
					}
				}
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			HuoyueInfoItem huoyueInfoItem2 = list[j];
			this.infoCollection.Add(huoyueInfoItem2);
			huoyueInfoItem2.m_AwardLabel.textColor = 65280U;
			huoyueInfoItem2.m_CurrLabel.textColor = 65280U;
			huoyueInfoItem2.m_DesLabel.textColor = 65280U;
			huoyueInfoItem2.m_TotalLabel.textColor = 65280U;
			UIPanel component2 = huoyueInfoItem2.transform.GetComponent<UIPanel>();
			if (null != component2)
			{
				Object.Destroy(component2);
			}
		}
		this.m_ValueLabel.Text = Global.Data.DailyActiveInfor.DailyActiveValues + string.Empty;
		for (int k = 0; k < this.awardCollection.Count; k++)
		{
			GameObject at = this.awardCollection.GetAt(k);
			HuoyueAwardItem component3 = at.GetComponent<HuoyueAwardItem>();
			int states;
			if (((int)Math.Pow(2.0, (double)component3.ID) & Global.Data.DailyActiveInfor.GetAwardFlag) != 0)
			{
				states = 2;
			}
			else if (Global.Data.DailyActiveInfor.DailyActiveValues >= (long)component3.Need)
			{
				states = 1;
			}
			else
			{
				states = 0;
			}
			component3.setStates(states);
		}
	}

	private void initAwardData()
	{
		XElement gameResXml = Global.GetGameResXml("Config/DailyActiveAward.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "DailyActiveAward"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				string[] array = Global.GetXElementAttributeStr(xelement, "GoodsID").Split(new char[]
				{
					'|'
				});
				if (array.Length <= 0)
				{
					return;
				}
				HuoyueAwardItem huoyueAwardItem = U3DUtils.NEW<HuoyueAwardItem>();
				this.awardCollection.Add(huoyueAwardItem);
				huoyueAwardItem.init(array);
				huoyueAwardItem.Need = Global.GetXElementAttributeInt(xelement, "NeedhuoYue");
				huoyueAwardItem.m_States[0].GetComponent<UILabel>().text = StringUtil.substitute(Global.GetLang("需要活跃") + "{0}", new object[]
				{
					huoyueAwardItem.Need
				});
				huoyueAwardItem.setStates(0);
				huoyueAwardItem.ID = Global.GetXElementAttributeInt(xelement, "ID");
				UIPanel component = huoyueAwardItem.transform.GetComponent<UIPanel>();
				if (null != component)
				{
					Object.Destroy(component);
				}
			}
		}
	}

	public void refreshAwardItem(int id, int value)
	{
		this.m_ValueLabel.Text = value + string.Empty;
		for (int i = 0; i < this.awardCollection.Count; i++)
		{
			GameObject at = this.awardCollection.GetAt(i);
			HuoyueAwardItem component = at.GetComponent<HuoyueAwardItem>();
			if (component.ID == id)
			{
				component.setStates(2);
				return;
			}
		}
	}

	public ListBox listBox1;

	public ListBox listBox2;

	public TextBlock m_ValueLabel;

	private ObservableCollection infoCollection;

	private ObservableCollection awardCollection;
}
