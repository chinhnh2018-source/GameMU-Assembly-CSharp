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

public class ZhongShenZhengBaPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Bisai1.pivot = 5;
		this.BisaiJincheng.text = string.Empty;
		this.Buy.Text = Global.GetLang("战功商店");
		int i = 0;
		int num = this.players.Length;
		while (i < num)
		{
			this.players[i].Name.text = Global.GetColorStringForNGUIText(new object[]
			{
				"826e53",
				Global.GetLang("虚位以待")
			});
			i++;
		}
		this.kingName.text = string.Empty;
		this.Bisai1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("比赛进程：")
		});
		this.Bisai2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("比赛时间：")
		});
		this.Looks8_1.isEnabled = false;
		this.Looks8_2.isEnabled = false;
		this.Looks8_3.isEnabled = false;
		this.Looks8_4.isEnabled = false;
		this.Looks8_5.isEnabled = false;
		this.Looks8_6.isEnabled = false;
		this.Looks8_7.isEnabled = false;
		this.Looks8_8.isEnabled = false;
		this.Looks4_1.isEnabled = false;
		this.Looks4_2.isEnabled = false;
		this.Looks4_3.isEnabled = false;
		this.Looks4_4.isEnabled = false;
		this.Looks2_1.isEnabled = false;
		this.Looks2_2.isEnabled = false;
		this.WudaoCount.text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZhengBaPoint).ToString();
		this.barkGround.URL = "NetImages/GameRes/Images/Plate/zhongshen/tongyongdikuang.png.qj";
		this.LooksKing.gameObject.GetComponent<ShowNetImage>().URL = "NetImages/GameRes/Images/Plate/zhongshen/xunzhang.png.qj";
		if (this.LooksKing != null)
		{
			UITexture component = this.LooksKing.gameObject.GetComponent<UITexture>();
			component.shader = Shader.Find("Unlit/Gray");
			BoxCollider component2 = this.LooksKing.gameObject.GetComponent<BoxCollider>();
			component2.isTrigger = true;
		}
		this.touxiangKuang.SetActive(false);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (Context.IsHaiwai)
		{
			this.Rule.Text = Global.GetLang("详细规则");
			this.ZhanBao.Text = Global.GetLang("战报");
			this.State.Text = Global.GetLang("参赛状态");
			this.Award.Text = Global.GetLang("奖励预览");
		}
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.Rule.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.ZSRule)
			{
				this.OpenRuleInterFace();
			}
		};
		this.Award.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.ZSAward)
			{
				this.OpenAwardInterFace();
			}
		};
		this.ZhanBao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.ZSZhanBao)
			{
				this.OpenZhanBaoInterFace();
			}
		};
		this.State.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.ZSPlayerState)
			{
				this.OpenPlayerStateInterFace();
			}
		};
		this.Buy.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ItemHandler(this, new DPSelectedItemEventArgs());
		};
		this.Looks8_1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 3)
			{
				this.InitPlayerGroup16_8(8, 1, this.Data.RankResultOfDay, 3, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks8_2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 3)
			{
				this.InitPlayerGroup16_8(8, 2, this.Data.RankResultOfDay, 3, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks8_3.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 3)
			{
				this.InitPlayerGroup16_8(8, 3, this.Data.RankResultOfDay, 3, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks8_4.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 3)
			{
				this.InitPlayerGroup16_8(8, 4, this.Data.RankResultOfDay, 3, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks8_5.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 3)
			{
				this.InitPlayerGroup16_8(8, 5, this.Data.RankResultOfDay, 3, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks8_6.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 3)
			{
				this.InitPlayerGroup16_8(8, 6, this.Data.RankResultOfDay, 3, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks8_7.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 3)
			{
				this.InitPlayerGroup16_8(8, 7, this.Data.RankResultOfDay, 3, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks8_8.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 3)
			{
				this.InitPlayerGroup16_8(8, 8, this.Data.RankResultOfDay, 3, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks4_1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 4)
			{
				this.InitPlayerGroup(4, 1, this.Data.RankResultOfDay, 4, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks4_2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 4)
			{
				this.InitPlayerGroup(4, 2, this.Data.RankResultOfDay, 4, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks4_3.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 4)
			{
				this.InitPlayerGroup(4, 3, this.Data.RankResultOfDay, 4, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks4_4.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 4)
			{
				this.InitPlayerGroup(4, 4, this.Data.RankResultOfDay, 4, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks2_1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 5)
			{
				this.InitPlayerGroup(2, 1, this.Data.RankResultOfDay, 5, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		this.Looks2_2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Data != null && this.Data.RealActDay >= 5)
			{
				this.InitPlayerGroup(2, 2, this.Data.RankResultOfDay, 5, this.Data.RealActDay);
			}
			else
			{
				string lang = Global.GetLang("您当前不能点击此按钮查看！");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			}
		};
		UIEventListener.Get(this.LooksKing.gameObject).onClick = delegate(GameObject e)
		{
			if (this.Data != null && this.Data.RankResultOfDay == 6)
			{
				this.InitPlayerGroup(1, 1, this.Data.RankResultOfDay, 6, this.Data.RealActDay);
			}
		};
		GameInstance.Game.GetMainInfo();
		GameInstance.Game.SendIsCanYu();
		Super.ShowNetWaiting(null);
	}

	private void ChaKanButtonGry(int day)
	{
		if (day < 1 || day > 7)
		{
			return;
		}
		DateTime dateTime = default(DateTime);
		DateTime.TryParse(string.Format("{0}-{1}-{2} {3}:00", new object[]
		{
			Global.GetCorrectDateTime().Year,
			Global.GetCorrectDateTime().Month,
			Global.GetCorrectDateTime().Day,
			ZhongShenZhengBaPart.GetDicMatch()[day].EndTime
		}), ref dateTime);
		bool flag = Global.GetCorrectDateTime().Ticks > dateTime.Ticks;
		if ((this.players[0].RoleData != null && day > 3) || (this.players[1].RoleData != null && day > 3) || (day == 3 && flag))
		{
			this.Looks8_1.isEnabled = true;
		}
		if ((this.players[2].RoleData != null && day > 3) || (this.players[3].RoleData != null && day > 3) || (day == 3 && flag))
		{
			this.Looks8_2.isEnabled = true;
		}
		if ((this.players[4].RoleData != null && day > 3) || (this.players[5].RoleData != null && day > 3) || (day == 3 && flag))
		{
			this.Looks8_3.isEnabled = true;
		}
		if ((this.players[6].RoleData != null && day > 3) || (this.players[7].RoleData != null && day > 3) || (day == 3 && flag))
		{
			this.Looks8_4.isEnabled = true;
		}
		if ((this.players[8].RoleData != null && day > 3) || (this.players[9].RoleData != null && day > 3) || (day == 3 && flag))
		{
			this.Looks8_5.isEnabled = true;
		}
		if ((this.players[10].RoleData != null && day > 3) || (this.players[11].RoleData != null && day > 3) || (day == 3 && flag))
		{
			this.Looks8_6.isEnabled = true;
		}
		if ((this.players[12].RoleData != null && day > 3) || (this.players[13].RoleData != null && day > 3) || (day == 3 && flag))
		{
			this.Looks8_7.isEnabled = true;
		}
		if ((this.players[14].RoleData != null && day > 3) || (this.players[15].RoleData != null && day > 3) || (day == 3 && flag))
		{
			this.Looks8_8.isEnabled = true;
		}
		if ((this.Looks8_1.isEnabled && day > 4) || (this.Looks8_2.isEnabled && day > 4) || (day == 4 && flag))
		{
			this.Looks4_1.isEnabled = true;
		}
		if ((this.Looks8_3.isEnabled && day > 4) || (this.Looks8_4.isEnabled && day > 4) || (day == 4 && flag))
		{
			this.Looks4_2.isEnabled = true;
		}
		if ((this.Looks8_5.isEnabled && day > 4) || (this.Looks8_6.isEnabled && day > 4) || (day == 4 && flag))
		{
			this.Looks4_3.isEnabled = true;
		}
		if ((this.Looks8_7.isEnabled && day > 4) || (this.Looks8_8.isEnabled && day > 4) || (day == 4 && flag))
		{
			this.Looks4_4.isEnabled = true;
		}
		if ((this.Looks4_1.isEnabled && day > 5) || (this.Looks4_2.isEnabled && day > 5) || (day == 5 && flag))
		{
			this.Looks2_1.isEnabled = true;
		}
		if ((this.Looks4_3.isEnabled && day > 5) || (this.Looks4_4.isEnabled && day > 5) || (day == 5 && flag))
		{
			this.Looks2_2.isEnabled = true;
		}
	}

	public void GetReServerMainInfo(ZhengBaMainInfoData mainData)
	{
		if (mainData == null)
		{
			return;
		}
		this.Data = mainData;
		this.OpenLingQuAwardInterFace(mainData.CanGetAwardId);
		XElement xelement = Global.GetXElement(Global.GetGameResXml("Config/Match.xml"), "config");
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MuLeiTaiDay");
		string text = string.Empty;
		string text2 = string.Empty;
		if (Global.GetCorrectDateTime().Day < xelementAttributeInt)
		{
			text = string.Format("{0}    {1}", Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format("{0}-{1}-10", Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month)
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				ZhongShenZhengBaPart.GetDicMatch()[1].BeginTime
			}));
			text2 = Global.GetColorStringForNGUIText(new object[]
			{
				"ffcc19",
				ZhongShenZhengBaPart.GetDicMatch()[1].Name
			});
		}
		else if (mainData.RealActDay <= 0)
		{
			text = string.Format("{0}    {1}", Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format("{0}-{1}-{2}", Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month, Global.GetCorrectDateTime().Day)
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				ZhongShenZhengBaPart.GetDicMatch()[1].BeginTime
			}));
			text2 = Global.GetColorStringForNGUIText(new object[]
			{
				"ffcc19",
				ZhongShenZhengBaPart.GetDicMatch()[1].Name
			});
		}
		else if (mainData.RealActDay > 7)
		{
			text = string.Empty;
			text2 = string.Empty;
			this.Bisai1.gameObject.SetActive(false);
			this.Bisai2.gameObject.SetActive(false);
		}
		else
		{
			DateTime dateTime = default(DateTime);
			DateTime dateTime2 = default(DateTime);
			DateTime.TryParse(string.Format("{0}-{1}-{2} {3}:00", new object[]
			{
				Global.GetCorrectDateTime().Year,
				Global.GetCorrectDateTime().Month,
				Global.GetCorrectDateTime().Day,
				ZhongShenZhengBaPart.GetDicMatch()[mainData.RealActDay].BeginTime
			}), ref dateTime);
			DateTime.TryParse(string.Format("{0}-{1}-{2} {3}:00", new object[]
			{
				Global.GetCorrectDateTime().Year,
				Global.GetCorrectDateTime().Month,
				Global.GetCorrectDateTime().Day,
				ZhongShenZhengBaPart.GetDicMatch()[mainData.RealActDay].EndTime
			}), ref dateTime2);
			if (mainData.RankResultOfDay != mainData.RealActDay)
			{
				if (Global.GetCorrectDateTime().Ticks < dateTime.Ticks)
				{
					text = string.Format("{0}    {1}", Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format("{0}-{1}-{2}", Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month, Global.GetCorrectDateTime().Day)
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						ZhongShenZhengBaPart.GetDicMatch()[mainData.RealActDay].BeginTime
					}));
				}
				else
				{
					text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("比赛进行中...")
					});
				}
				text2 = Global.GetColorStringForNGUIText(new object[]
				{
					"ffcc19",
					ZhongShenZhengBaPart.GetDicMatch()[mainData.RealActDay].Name
				});
			}
			else if (mainData.RealActDay >= 7)
			{
				text = string.Empty;
				text2 = string.Empty;
				this.Bisai1.gameObject.SetActive(false);
				this.Bisai2.gameObject.SetActive(false);
			}
			else
			{
				text = string.Format("{0}    {1}", Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format("{0}-{1}-{2}", Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month, Global.GetCorrectDateTime().Day + 1)
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					ZhongShenZhengBaPart.GetDicMatch()[mainData.RealActDay + 1].BeginTime
				}));
				text2 = Global.GetColorStringForNGUIText(new object[]
				{
					"ffcc19",
					ZhongShenZhengBaPart.GetDicMatch()[mainData.RealActDay + 1].Name
				});
			}
		}
		this.BisaiJincheng.text = text2;
		this.BisaiTime.text = text;
		if (mainData.Top16List == null)
		{
			return;
		}
		int i = 0;
		int count = mainData.Top16List.Count;
		while (i < count)
		{
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(mainData.Top16List[i].ZoneId, out ztBuffServerInfo))
			{
				if (mainData.Top16List[i].ZhengBaState == 1)
				{
					this.players[mainData.Top16List[i].ZhengBaGroup - 1].Name.text = Global.GetColorStringForNGUIText(new object[]
					{
						"c3b096",
						ztBuffServerInfo.strServerName,
						"c3b096",
						mainData.Top16List[i].RoleName
					});
				}
				else
				{
					this.players[mainData.Top16List[i].ZhengBaGroup - 1].Name.text = Global.GetColorStringForNGUIText(new object[]
					{
						"826e53",
						ztBuffServerInfo.strServerName,
						"826e53",
						mainData.Top16List[i].RoleName
					});
				}
			}
			else if (mainData.Top16List[i].ZhengBaState == 1)
			{
				this.players[mainData.Top16List[i].ZhengBaGroup - 1].Name.text = Global.GetColorStringForNGUIText(new object[]
				{
					"c3b096",
					"s" + mainData.Top16List[i].ZoneId,
					"c3b096",
					mainData.Top16List[i].RoleName
				});
			}
			else
			{
				this.players[mainData.Top16List[i].ZhengBaGroup - 1].Name.text = Global.GetColorStringForNGUIText(new object[]
				{
					"826e53",
					"s" + mainData.Top16List[i].ZoneId,
					"826e53",
					mainData.Top16List[i].RoleName
				});
			}
			this.players[mainData.Top16List[i].ZhengBaGroup - 1].RoleData = mainData.Top16List[i];
			if (mainData.Top16List[i].ZhengBaGrade == 1)
			{
				if (Global.GetNowServerIsZhuTiFu(mainData.Top16List[i].ZoneId, out ztBuffServerInfo))
				{
					this.kingName.text = Global.GetColorStringForNGUIText(new object[]
					{
						"b266ff",
						ztBuffServerInfo.strServerName,
						"b266ff",
						mainData.Top16List[i].RoleName
					});
				}
				else
				{
					this.kingName.text = Global.GetColorStringForNGUIText(new object[]
					{
						"b266ff",
						"s" + mainData.Top16List[i].ZoneId,
						"b266ff",
						mainData.Top16List[i].RoleName
					});
				}
				string url = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
				{
					Global.CalcOriginalOccupationID(mainData.Top16List[i].Occupation),
					mainData.Top16List[i].RoleData4Selector.RoleSex
				});
				this.touxiangKing.URL = url;
				this.touxiangKuang.SetActive(true);
			}
			this.GetLuXian(mainData.Top16List[i].ZhengBaGrade, mainData.Top16List[i].ZhengBaState, mainData.Top16List[i].ZhengBaGroup);
			MUDebug.Log<string>(new string[]
			{
				string.Concat(new object[]
				{
					"区号=",
					mainData.Top16List[i].ZoneId,
					";角色名=",
					mainData.Top16List[i].RoleName,
					";分组情况=",
					mainData.Top16List[i].ZhengBaGroup,
					";争霸状态=",
					mainData.Top16List[i].ZhengBaState,
					";争霸成绩=",
					mainData.Top16List[i].ZhengBaGrade
				})
			});
			i++;
		}
		this.GetZanBianTexiao(mainData.MaxSupportGroup, mainData.MaxOpposeGroup);
		this.ChaKanButtonGry(mainData.RealActDay);
		this.EndGameXianLu();
		if (mainData.RealActDay > 6 && mainData.RankResultOfDay >= 6 && this.LooksKing != null)
		{
			UITexture component = this.LooksKing.gameObject.GetComponent<UITexture>();
			component.shader = Shader.Find("Unlit/Transparent Colored");
			BoxCollider component2 = this.LooksKing.gameObject.GetComponent<BoxCollider>();
			if (mainData.RankResultOfDay >= 7)
			{
				component2.isTrigger = false;
			}
			else
			{
				component2.isTrigger = true;
			}
		}
	}

	private void InitPlayerGroup(int Grade, int weizhi, int resultday, int paihangday, int realday)
	{
		int num = 16 / Grade * weizhi - 16 / Grade;
		int num2 = 16 / Grade * weizhi - 1;
		bool isnotenable = resultday != paihangday;
		this.GroupList.Clear();
		int i = num;
		int num3 = num2;
		while (i <= num3)
		{
			if (this.players[i].RoleData != null)
			{
				if (this.players[i].RoleData.ZhengBaGrade <= 2 * Grade)
				{
					this.GroupList.Add(this.players[i].RoleData);
				}
			}
			i++;
		}
		this.OpenChaKanPlayerInterFace(this.GroupList, resultday, isnotenable, paihangday);
	}

	private void InitPlayerGroup16_8(int Grade, int weizhi, int resultday, int paihangday, int realday)
	{
		int num = 16 / Grade * weizhi - 16 / Grade;
		int num2 = 16 / Grade * weizhi - 1;
		bool isnotenable = resultday != paihangday;
		this.GroupList.Clear();
		if (this.players[num].RoleData != null)
		{
			this.GroupList.Add(this.players[num].RoleData);
		}
		if (this.players[num2].RoleData != null)
		{
			this.GroupList.Add(this.players[num2].RoleData);
		}
		this.OpenChaKanPlayerInterFace(this.GroupList, realday, isnotenable, paihangday);
	}

	private void GetLuXian(int grade, int state, int group)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (group == 1 || group == 2)
		{
			num = 1;
			num2 = 1;
			num3 = 1;
		}
		else if (group == 3 || group == 4)
		{
			num = 2;
			num2 = 1;
			num3 = 1;
		}
		else if (group == 5 || group == 6)
		{
			num = 3;
			num2 = 2;
			num3 = 1;
		}
		else if (group == 7 || group == 8)
		{
			num = 4;
			num2 = 2;
			num3 = 1;
		}
		else if (group == 9 || group == 10)
		{
			num = 5;
			num2 = 3;
			num3 = 2;
		}
		else if (group == 11 || group == 12)
		{
			num = 6;
			num2 = 3;
			num3 = 2;
		}
		else if (group == 13 || group == 14)
		{
			num = 7;
			num2 = 4;
			num3 = 2;
		}
		else if (group == 15 || group == 16)
		{
			num = 8;
			num2 = 4;
			num3 = 2;
		}
		if (grade == 16 && state == 1)
		{
			this.luXian16_8[group - 1].spriteName = "yellow1";
			return;
		}
		if (grade == 8 && state == 1)
		{
			this.luXian16_8[group - 1].spriteName = "yellow1";
			this.luXian8_4[num - 1].spriteName = "yellow2";
			return;
		}
		if (grade == 4 && state == 1)
		{
			this.luXian16_8[group - 1].spriteName = "yellow1";
			this.luXian8_4[num - 1].spriteName = "yellow2";
			this.luXian4_2[num2 - 1].spriteName = "yellow3";
			return;
		}
		if ((grade == 2 && state == 1) || (grade == 1 && state == 1))
		{
			this.luXian16_8[group - 1].spriteName = "yellow1";
			this.luXian8_4[num - 1].spriteName = "yellow2";
			this.luXian4_2[num2 - 1].spriteName = "yellow3";
			this.luXian2_1[num3 - 1].spriteName = "yellow4";
			return;
		}
		if (grade == 1 && state == 1)
		{
			this.luXian16_8[group - 1].spriteName = "yellow1";
			this.luXian8_4[num - 1].spriteName = "yellow2";
			this.luXian4_2[num2 - 1].spriteName = "yellow3";
			this.luXian2_1[num3 - 1].spriteName = "yellow4";
			return;
		}
	}

	private void EndGameXianLu()
	{
		for (int i = 0; i < this.players.Length; i++)
		{
			if (this.players[i].RoleData != null)
			{
				if (this.players[i].RoleData.ZhengBaState == 2)
				{
					if (this.players[i].RoleData.ZhengBaGrade != 16)
					{
						if (this.players[i].RoleData.ZhengBaGrade == 8)
						{
							this.luXian16_8[i].spriteName = "yellow1";
						}
						else if (this.players[i].RoleData.ZhengBaGrade == 4)
						{
							if (this.players[i].RoleData.ZhengBaGroup % 2 == 0)
							{
								this.luXian16_8[i].spriteName = "yellow1";
								this.luXian8_4[this.players[i].RoleData.ZhengBaGroup / 2 - 1].spriteName = "yellow2";
							}
							else
							{
								this.luXian16_8[i].spriteName = "yellow1";
								this.luXian8_4[(this.players[i].RoleData.ZhengBaGroup + 1) / 2 - 1].spriteName = "yellow2";
							}
						}
						else if (this.players[i].RoleData.ZhengBaGrade == 2)
						{
							int num = 0;
							if (this.players[i].RoleData.ZhengBaGroup > 4 && this.players[i].RoleData.ZhengBaGroup <= 8)
							{
								num = 1;
							}
							else if (this.players[i].RoleData.ZhengBaGroup > 8 && this.players[i].RoleData.ZhengBaGroup <= 12)
							{
								num = 2;
							}
							else if (this.players[i].RoleData.ZhengBaGroup > 12)
							{
								num = 3;
							}
							if (this.players[i].RoleData.ZhengBaGroup % 2 == 0)
							{
								this.luXian16_8[i].spriteName = "yellow1";
								this.luXian8_4[this.players[i].RoleData.ZhengBaGroup / 2 - 1].spriteName = "yellow2";
							}
							else
							{
								this.luXian16_8[i].spriteName = "yellow1";
								this.luXian8_4[(this.players[i].RoleData.ZhengBaGroup + 1) / 2 - 1].spriteName = "yellow2";
							}
							this.luXian4_2[num].spriteName = "yellow3";
						}
					}
				}
			}
		}
	}

	private void GetZanBianTexiao(int support, int oppose)
	{
		string text = "UITeXiao/Perfabs/zhongshenzhengba/";
		if (support >= 1 && support <= 16)
		{
			int i = 0;
			int num = this.players.Length;
			while (i < num)
			{
				if (this.players[i].RoleData != null)
				{
					if (support == this.players[i].RoleData.ZhengBaGroup)
					{
						if (support <= 8)
						{
							this.LoadTeXiao(text + "zhongshenzhengba_zan", this.players[i].transform, 0f);
						}
						else
						{
							this.LoadTeXiao(text + "zhongshenzhengba_zan", this.players[i].transform, 180f);
						}
					}
				}
				i++;
			}
		}
		if (oppose >= 1 && oppose <= 16)
		{
			int j = 0;
			int num2 = this.players.Length;
			while (j < num2)
			{
				if (this.players[j].RoleData != null)
				{
					if (oppose == this.players[j].RoleData.ZhengBaGroup)
					{
						if (oppose <= 8)
						{
							this.LoadTeXiao(text + "zhongshenzhengba_cai", this.players[j].transform, 0f);
						}
						else
						{
							this.LoadTeXiao(text + "zhongshenzhengba_cai", this.players[j].transform, 180f);
						}
					}
				}
				j++;
			}
		}
	}

	private GameObject LoadTeXiao(string path, Transform parent, float Y)
	{
		Object @object = Resources.Load(path);
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.SetParent(parent, false);
			U3DUtils.ResetLayer(gameObject, "MUUI");
			gameObject.transform.localPosition = new Vector3(0f, 0f, -15f);
			gameObject.transform.localRotation = new Quaternion(0f, Y, 0f, 0f);
			return gameObject;
		}
		return null;
	}

	private void OpenAwardInterFace()
	{
		if (this.ZSAwardWindow == null)
		{
			this.ZSAwardWindow = U3DUtils.NEW<GChildWindow>();
			this.ZSAwardWindow.IsShowModal = true;
			this.ZSAwardWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.ZSAwardWindow, Global.GetLang("争霸奖励预览"));
			Super.GData.GlobalPlayZone.Children.Add(this.ZSAwardWindow);
		}
		if (this.ZSAward == null)
		{
			this.ZSAward = U3DUtils.NEW<ZhongShenZhengBaPart_Award>();
			this.ZSAward.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseAwardInterFace();
			};
		}
		this.ZSAwardWindow.SetContent(this.ZSAwardWindow.BodyPresenter, this.ZSAward, 0.0, 0.0, true);
	}

	private void CloseAwardInterFace()
	{
		if (this.ZSAward)
		{
			this.ZSAward.transform.parent = null;
			Object.Destroy(this.ZSAward.gameObject);
			this.ZSAward = null;
		}
		if (this.ZSAwardWindow)
		{
			Super.CloseChildWindow(base.Children, this.ZSAwardWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.ZSAwardWindow, true);
			this.ZSAwardWindow = null;
		}
	}

	private void OpenChaKanPlayerInterFace(List<TianTiPaiHangRoleData> group, int day, bool isnotenable, int paihangday)
	{
		if (this.Data == null)
		{
			return;
		}
		if (group == null)
		{
			string lang = Global.GetLang("您当前不能点击此按钮查看！");
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			return;
		}
		if (this.ZSChaKanPlayerWindow == null)
		{
			this.ZSChaKanPlayerWindow = U3DUtils.NEW<GChildWindow>();
			this.ZSChaKanPlayerWindow.IsShowModal = true;
			this.ZSChaKanPlayerWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.ZSChaKanPlayerWindow, Global.GetLang("查看选手界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.ZSChaKanPlayerWindow);
		}
		if (this.ZSChaKanPlayer == null)
		{
			this.ZSChaKanPlayer = U3DUtils.NEW<ZhongShenZhengBaPartChaKanPlayer>();
			this.ZSChaKanPlayer.IsNotEnable = isnotenable;
			this.ZSChaKanPlayer.GroupList = group;
			this.ZSChaKanPlayer.Day = day;
			this.ZSChaKanPlayer.PaiHangDay = paihangday;
			this.ZSChaKanPlayer.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseChaKanPlayerInterFace();
			};
		}
		this.ZSChaKanPlayerWindow.SetContent(this.ZSChaKanPlayerWindow.BodyPresenter, this.ZSChaKanPlayer, 0.0, 0.0, true);
	}

	private void CloseChaKanPlayerInterFace()
	{
		if (this.ZSChaKanPlayer)
		{
			this.ZSChaKanPlayer.transform.parent = null;
			Object.Destroy(this.ZSChaKanPlayer.gameObject);
			this.ZSChaKanPlayer = null;
		}
		if (this.ZSChaKanPlayerWindow)
		{
			Super.GData.GlobalPlayZone.Children.Remove(this.ZSChaKanPlayerWindow, true);
			Super.CloseChildWindow(base.Children, this.ZSChaKanPlayerWindow);
			this.ZSChaKanPlayerWindow = null;
		}
	}

	private void OpenZhanBaoInterFace()
	{
		if (this.ZSZhanBaoWindow == null)
		{
			this.ZSZhanBaoWindow = U3DUtils.NEW<GChildWindow>();
			this.ZSZhanBaoWindow.IsShowModal = true;
			this.ZSZhanBaoWindow.ModalType = ChildWindowModalType.Translucent;
			Super.GData.GlobalPlayZone.Children.Add(this.ZSZhanBaoWindow);
			Super.InitChildWindow(this.ZSZhanBaoWindow, Global.GetLang("战报界面"));
		}
		if (this.ZSZhanBao == null)
		{
			this.ZSZhanBao = U3DUtils.NEW<ZhongShenZhengBaPartZhanBao>();
			this.ZSZhanBao.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseZhanBaoInterFace();
			};
		}
		this.ZSZhanBaoWindow.SetContent(this.ZSZhanBaoWindow.BodyPresenter, this.ZSZhanBao, 0.0, 0.0, true);
	}

	private void CloseZhanBaoInterFace()
	{
		if (this.ZSZhanBao)
		{
			this.ZSZhanBao.transform.parent = null;
			Object.Destroy(this.ZSZhanBao.gameObject);
			this.ZSZhanBao = null;
		}
		if (this.ZSZhanBaoWindow)
		{
			Super.GData.GlobalPlayZone.Children.Remove(this.ZSZhanBaoWindow, true);
			Super.CloseChildWindow(base.Children, this.ZSZhanBaoWindow);
		}
	}

	private void OpenLingQuAwardInterFace(int day)
	{
		if (day >= 1 && day <= 8)
		{
			if (this.ZSLingQuAwardWindow == null)
			{
				this.ZSLingQuAwardWindow = U3DUtils.NEW<GChildWindow>();
				this.ZSLingQuAwardWindow.IsShowModal = true;
				this.ZSLingQuAwardWindow.ModalType = ChildWindowModalType.Translucent;
				Super.InitChildWindow(this.ZSLingQuAwardWindow, Global.GetLang("领取奖励界面"));
				Super.GData.GlobalPlayZone.Children.Add(this.ZSLingQuAwardWindow);
			}
			if (this.ZSLingQuAward == null)
			{
				this.ZSLingQuAward = U3DUtils.NEW<ZhongShenZhengBaPartAward>();
				this.ZSLingQuAward.InitListItem(day);
				this.ZSLingQuAward.DPSHandler = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.CloseLingQuAwardInterFace();
				};
			}
			this.ZSLingQuAwardWindow.SetContent(this.ZSLingQuAwardWindow.BodyPresenter, this.ZSLingQuAward, 0.0, 0.0, true);
		}
	}

	private void CloseLingQuAwardInterFace()
	{
		if (this.ZSLingQuAward)
		{
			this.ZSLingQuAward.transform.parent = null;
			Object.Destroy(this.ZSLingQuAward.gameObject);
			this.ZSLingQuAward = null;
		}
		if (this.ZSLingQuAwardWindow)
		{
			Super.CloseChildWindow(base.Children, this.ZSLingQuAwardWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.ZSLingQuAwardWindow, true);
			this.ZSLingQuAwardWindow = null;
		}
	}

	private void OpenPlayerStateInterFace()
	{
		if (this.ZSPlayerStateWindow == null)
		{
			this.ZSPlayerStateWindow = U3DUtils.NEW<GChildWindow>();
			this.ZSPlayerStateWindow.IsShowModal = true;
			this.ZSPlayerStateWindow.ModalType = ChildWindowModalType.Translucent;
			Super.GData.GlobalPlayZone.Children.Add(this.ZSPlayerStateWindow);
			Super.InitChildWindow(this.ZSPlayerStateWindow, Global.GetLang("参赛状态"));
		}
		if (this.ZSPlayerState == null)
		{
			this.ZSPlayerState = U3DUtils.NEW<ZhongShenZhengBaPartPlayerState>();
			this.ZSPlayerState.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.ClosePlayerStateInterFace();
			};
		}
		this.ZSPlayerStateWindow.SetContent(this.ZSPlayerStateWindow.BodyPresenter, this.ZSPlayerState, 0.0, 0.0, true);
	}

	private void ClosePlayerStateInterFace()
	{
		if (this.ZSPlayerState)
		{
			this.ZSPlayerState.transform.parent = null;
			Object.Destroy(this.ZSPlayerState.gameObject);
			this.ZSPlayerState = null;
		}
		if (this.ZSPlayerStateWindow)
		{
			Super.GData.GlobalPlayZone.Children.Remove(this.ZSPlayerStateWindow, true);
			Super.CloseChildWindow(base.Children, this.ZSPlayerStateWindow);
			this.ZSPlayerStateWindow = null;
		}
	}

	private void OpenRuleInterFace()
	{
		if (this.ZSRuleWindow == null)
		{
			this.ZSRuleWindow = U3DUtils.NEW<GChildWindow>();
			this.ZSRuleWindow.IsShowModal = true;
			this.ZSRuleWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.ZSRuleWindow, Global.GetLang("规则界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.ZSRuleWindow);
		}
		if (this.ZSRule == null)
		{
			this.ZSRule = U3DUtils.NEW<ZhongShenZhengBaPart_Rule>();
			this.ZSRule.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseRuleInterFace();
			};
		}
		this.ZSRuleWindow.SetContent(this.ZSRuleWindow.BodyPresenter, this.ZSRule, 0.0, 0.0, true);
	}

	private void CloseRuleInterFace()
	{
		if (null != this.ZSRule)
		{
			this.ZSRule.transform.parent = null;
			Object.Destroy(this.ZSRule.gameObject);
			this.ZSRule = null;
		}
		if (null != this.ZSRuleWindow)
		{
			Super.CloseChildWindow(base.Children, this.ZSRuleWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.ZSRuleWindow, true);
			this.ZSRuleWindow = null;
		}
	}

	public void InitZhanBaoList(List<ZhengBaPkLogData> PKLogData)
	{
		if (PKLogData == null)
		{
			return;
		}
		if (this.ZSZhanBao)
		{
			this.ZSZhanBao.InitList(PKLogData);
		}
	}

	public void InitPlayerStateList(List<TianTiPaiHangRoleData> StateData)
	{
		if (StateData == null)
		{
			return;
		}
		if (this.ZSPlayerState)
		{
			this.ZSPlayerState.InitPlayerStateListItem(StateData);
		}
	}

	public void InitChaKanPlayerList(ZhengBaUnionGroupData ChaKanData)
	{
		if (ChaKanData == null)
		{
			return;
		}
		if (this.ZSChaKanPlayer)
		{
			this.ZSChaKanPlayer.InitChaKanInterFace(ChaKanData);
		}
	}

	public void UpDataInfo()
	{
		this.WudaoCount.text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZhengBaPoint).ToString();
	}

	public static void ClearXMLData()
	{
		if (0 < ZhongShenZhengBaPart.dicSustain.Count)
		{
			ZhongShenZhengBaPart.dicSustain.Clear();
		}
		if (0 < ZhongShenZhengBaPart.dicMatch.Count)
		{
			ZhongShenZhengBaPart.dicMatch.Clear();
		}
		if (0 < ZhongShenZhengBaPart.dicMatchAward.Count)
		{
			ZhongShenZhengBaPart.dicMatchAward.Clear();
		}
	}

	public static Dictionary<int, Sustain> GetDicSustain()
	{
		if (ZhongShenZhengBaPart.dicSustain.Count > 0)
		{
			return ZhongShenZhengBaPart.dicSustain;
		}
		XElement gameResXml = Global.GetGameResXml("Config/Sustain.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ZhiChi");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			Sustain sustain = new Sustain();
			sustain.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			sustain.Day = Global.GetXElementAttributeInt(xelementList[i], "Day");
			sustain.CostZhiChi = Global.GetXElementAttributeInt(xelementList[i], "CostZhiChi");
			sustain.MaxNum = Global.GetXElementAttributeInt(xelementList[i], "MaxNum");
			sustain.WinAward = Global.GetXElementAttributeStr(xelementList[i], "WinAward");
			sustain.FaillAward = Global.GetXElementAttributeStr(xelementList[i], "FaillAward");
			sustain.Group = Global.GetXElementAttributeStr(xelementList[i], "Group");
			sustain.BeginTime = Global.GetXElementAttributeStr(xelementList[i], "BeginTime");
			sustain.EndTime = Global.GetXElementAttributeStr(xelementList[i], "EndTime");
			sustain.MinLevel = Global.GetXElementAttributeStr(xelementList[i], "MinLevel");
			if (!ZhongShenZhengBaPart.dicSustain.ContainsKey(sustain.Day))
			{
				ZhongShenZhengBaPart.dicSustain.Add(sustain.Day, sustain);
			}
			i++;
		}
		return ZhongShenZhengBaPart.dicSustain;
	}

	public static Dictionary<int, Match> GetDicMatch()
	{
		if (ZhongShenZhengBaPart.dicMatch.Count > 0)
		{
			return ZhongShenZhengBaPart.dicMatch;
		}
		XElement gameResXml = Global.GetGameResXml("Config/Match.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Match");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			Match match = new Match();
			match.Day = Global.GetXElementAttributeInt(xelementList[i], "Day");
			match.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			match.MapCode = Global.GetXElementAttributeInt(xelementList[i], "MapCode");
			match.MinLevelRank = Global.GetXElementAttributeInt(xelementList[i], "MinLevelRank");
			match.Week = Global.GetXElementAttributeStr(xelementList[i], "Week");
			match.BeginTime = Global.GetXElementAttributeStr(xelementList[i], "BeginTime");
			match.EndTime = Global.GetXElementAttributeStr(xelementList[i], "EndTime");
			match.WaitTime = Global.GetXElementAttributeInt(xelementList[i], "WaitTime");
			match.FightTime = Global.GetXElementAttributeInt(xelementList[i], "FightTime");
			match.ClearTime = Global.GetXElementAttributeStr(xelementList[i], "ClearTime");
			match.IntervalTime = Global.GetXElementAttributeInt(xelementList[i], "IntervalTime");
			match.MatchingType = Global.GetXElementAttributeInt(xelementList[i], "MatchingType");
			match.MatchingPrice = Global.GetXElementAttributeStr(xelementList[i], "MatchingPrice");
			match.NeedWinNum = Global.GetXElementAttributeInt(xelementList[i], "NeedWinNum");
			match.RankNum = Global.GetXElementAttributeInt(xelementList[i], "RankNum");
			if (!ZhongShenZhengBaPart.dicMatch.ContainsKey(match.Day))
			{
				ZhongShenZhengBaPart.dicMatch.Add(match.Day, match);
			}
			i++;
		}
		return ZhongShenZhengBaPart.dicMatch;
	}

	public static Dictionary<int, MatchAward> GetDicMatchAward()
	{
		if (ZhongShenZhengBaPart.dicMatchAward.Count > 0)
		{
			return ZhongShenZhengBaPart.dicMatchAward;
		}
		XElement gameResXml = Global.GetGameResXml("Config/MatchAward.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			MatchAward matchAward = new MatchAward();
			matchAward.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			matchAward.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			matchAward.FinalPassDay = Global.GetXElementAttributeInt(xelementList[i], "FinalPassDay");
			matchAward.Award = Global.GetXElementAttributeStr(xelementList[i], "Award");
			if (!ZhongShenZhengBaPart.dicMatchAward.ContainsKey(matchAward.ID))
			{
				ZhongShenZhengBaPart.dicMatchAward.Add(matchAward.ID, matchAward);
			}
			i++;
		}
		return ZhongShenZhengBaPart.dicMatchAward;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ItemHandler;

	public ShowNetImage barkGround;

	public GButton Close;

	public GButton Rule;

	public GButton Award;

	public GButton ZhanBao;

	public GButton State;

	public GButton Buy;

	public GButton Looks8_1;

	public GButton Looks8_2;

	public GButton Looks8_3;

	public GButton Looks8_4;

	public GButton Looks8_5;

	public GButton Looks8_6;

	public GButton Looks8_7;

	public GButton Looks8_8;

	public GButton Looks4_1;

	public GButton Looks4_2;

	public GButton Looks4_3;

	public GButton Looks4_4;

	public GButton Looks2_1;

	public GButton Looks2_2;

	public Players[] players;

	public UISprite[] luXian16_8;

	public UISprite[] luXian8_4;

	public UISprite[] luXian4_2;

	public UISprite[] luXian2_1;

	public UIEventListener LooksKing;

	public UILabel kingName;

	public ShowNetImage touxiangKing;

	public GameObject touxiangKuang;

	public UILabel BisaiJincheng;

	public UILabel BisaiTime;

	public UILabel WudaoCount;

	public UILabel Bisai1;

	public UILabel Bisai2;

	public GChildWindow ZSAwardWindow;

	public ZhongShenZhengBaPart_Award ZSAward;

	public GChildWindow ZSLingQuAwardWindow;

	public ZhongShenZhengBaPartAward ZSLingQuAward;

	public GChildWindow ZSZhanBaoWindow;

	public ZhongShenZhengBaPartZhanBao ZSZhanBao;

	public GChildWindow ZSPlayerStateWindow;

	public ZhongShenZhengBaPartPlayerState ZSPlayerState;

	public GChildWindow ZSChaKanPlayerWindow;

	public ZhongShenZhengBaPartChaKanPlayer ZSChaKanPlayer;

	public GChildWindow ZSRuleWindow;

	public ZhongShenZhengBaPart_Rule ZSRule;

	private ZhengBaMainInfoData Data;

	private List<TianTiPaiHangRoleData> GroupList = new List<TianTiPaiHangRoleData>();

	private static Dictionary<int, Sustain> dicSustain = new Dictionary<int, Sustain>();

	private static Dictionary<int, Match> dicMatch = new Dictionary<int, Match>();

	private static Dictionary<int, MatchAward> dicMatchAward = new Dictionary<int, MatchAward>();
}
