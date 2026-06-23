using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ZhongShenZhengBaPartChaKanPlayer : UserControl
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

	public List<TianTiPaiHangRoleData> GroupList
	{
		set
		{
			if (value.Count == 2)
			{
				this.LeftRoleData = value[0];
				this.RightRoleData = value[1];
				if (this.LeftRoleData.ZhengBaGrade < this.RightRoleData.ZhengBaGrade)
				{
					this.WinLeft.gameObject.SetActive(true);
					this.WinRight.gameObject.SetActive(false);
				}
				else if (this.LeftRoleData.ZhengBaGrade > this.RightRoleData.ZhengBaGrade)
				{
					this.WinLeft.gameObject.SetActive(false);
					this.WinRight.gameObject.SetActive(true);
				}
				else
				{
					this.WinLeft.gameObject.SetActive(false);
					this.WinRight.gameObject.SetActive(false);
				}
			}
			else if (value.Count == 1)
			{
				this.LeftRoleData = value[0];
				this.RightRoleData = null;
			}
			this.MakeGroup(this.LeftRoleData, this.RightRoleData, out this.LeftRoleGroup, out this.RightRoleGroup);
			GameInstance.Game.GetPKState(this.MakeUnionGroup(this.LeftRoleGroup, this.RightRoleGroup));
			Super.ShowNetWaiting(null);
		}
	}

	public int PaiHangDay
	{
		set
		{
			this.paihangday = value;
			int id = Convert.ToInt32(ZhongShenZhengBaPart.GetDicSustain()[(value <= 6) ? value : 6].WinAward.Split(new char[]
			{
				','
			})[0]);
			int num = Convert.ToInt32(ZhongShenZhengBaPart.GetDicSustain()[(value <= 6) ? value : 6].WinAward.Split(new char[]
			{
				','
			})[1]);
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
			int num2 = Convert.ToInt32(goodsXmlNodeByID.ExecMagic.Split(new char[]
			{
				'('
			})[1].Split(new char[]
			{
				')'
			})[0]);
			this.YaZhuSuoDei.text = (num2 * num).ToString();
			this.BtnYaZhu.Text = ZhongShenZhengBaPart.GetDicSustain()[(value <= 6) ? value : 6].CostZhiChi.ToString();
		}
	}

	public int Day
	{
		get
		{
			return this.day;
		}
		set
		{
			this.day = value;
			DateTime dateTime = default(DateTime);
			DateTime dateTime2 = default(DateTime);
			DateTime dateTime3 = default(DateTime);
			string text = string.Format("{0}-{1}-{2}", Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month, Global.GetCorrectDateTime().Day);
			DateTime.TryParse(text + string.Format(" 15:30:00", new object[0]), ref dateTime);
			DateTime.TryParse(text + string.Format(" 16:30:00", new object[0]), ref dateTime2);
			dateTime3 = Global.GetCorrectDateTime();
			if ((dateTime3.Ticks > dateTime.Ticks && dateTime3.Ticks < dateTime2.Ticks) || this.IsNotEnable)
			{
				this.BtnLeftZan.isEnabled = false;
				this.BtnLeftBian.isEnabled = false;
				this.BtnRightZan.isEnabled = false;
				this.BtnRightBian.isEnabled = false;
				this.BtnYaZhu.isEnabled = false;
			}
			else
			{
				this.BtnLeftZan.isEnabled = true;
				this.BtnLeftBian.isEnabled = true;
				this.BtnRightZan.isEnabled = true;
				this.BtnRightBian.isEnabled = true;
				this.BtnYaZhu.isEnabled = true;
			}
		}
	}

	private void InitTextInPrefabs()
	{
		if (Context.IsHaiwai && this.staticText != null)
		{
			this.staticText.text = Global.GetLang("押注获胜可得");
		}
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("查看选手")
		});
		this.playerLeft.URL = "NetImages/GameRes/Images/Plate/zhongshen/player1.png.qj";
		this.playerRight.URL = "NetImages/GameRes/Images/Plate/zhongshen/player2.png.qj";
		this.VS.URL = "NetImages/GameRes/Images/Plate/zhongshen/vs.png.qj";
		this.YaZhuRenShuLeft.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("押注人数：")
		}), 0);
		this.BtnLeftZan.Label.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			0
		});
		this.BtnLeftBian.Label.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			0
		});
		this.YaZhuRenShuRight.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("押注人数：")
		}), 0);
		this.BtnRightZan.Label.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			0
		});
		this.BtnRightBian.Label.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			0
		});
		this.LeftYaZhu.gameObject.SetActive(false);
		this.RightYaZhu.gameObject.SetActive(false);
		this.XuanZhongKuangLeft.gameObject.SetActive(false);
		this.XuanZhongKuangRight.gameObject.SetActive(false);
		this.staticText.pivot = 5;
		this.staticText.text = Global.GetLang("押注获胜可得");
		this.staticText.transform.localPosition = new Vector3(135f, this.staticText.transform.localPosition.y, -1f);
		this.LeftName.text = "s999\r\nTên người chơi 7 chữ";
		this.RightName.text = "s999\r\nTên người chơi 7 chữ";
		this.ZhanLiLeft.text = "{e3b36c}Lực chiến:{-}1000000000";
		this.ZhanLiRight.text = "{e3b36c}Lực chiến:{-}1000000000";
		this.TianTiPaiMingRight.text = "{e3b36c}XH PK Ranking:{-}100";
		this.TianTiPaiMingLeft.text = "{e3b36c}XH PK Ranking:{-}100";
		if (this.BtnYaZhu.transform.GetChild(1).name.Equals("Label"))
		{
			this.BtnYaZhu.transform.GetChild(1).transform.GetComponent<UILabel>().text = Global.GetLang("押注");
			this.BtnYaZhu.transform.GetChild(1).transform.localPosition = new Vector3(43f, 3f, -1f);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.List.ItemsSource;
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnLeftZan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.IsCanZanBian(this.LeftRoleData))
			{
				GameInstance.Game.GetZhengBaSupport(this.MakeUnionGroup(this.LeftRoleGroup, this.RightRoleGroup), this.LeftRoleGroup, 1);
				Super.ShowNetWaiting(null);
			}
		};
		this.BtnLeftBian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.IsCanZanBian(this.LeftRoleData))
			{
				GameInstance.Game.GetZhengBaSupport(this.MakeUnionGroup(this.LeftRoleGroup, this.RightRoleGroup), this.LeftRoleGroup, 2);
				Super.ShowNetWaiting(null);
			}
		};
		this.BtnRightZan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.IsCanZanBian(this.RightRoleData))
			{
				GameInstance.Game.GetZhengBaSupport(this.MakeUnionGroup(this.LeftRoleGroup, this.RightRoleGroup), this.RightRoleGroup, 1);
				Super.ShowNetWaiting(null);
			}
		};
		this.BtnRightBian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.IsCanZanBian(this.RightRoleData))
			{
				GameInstance.Game.GetZhengBaSupport(this.MakeUnionGroup(this.LeftRoleGroup, this.RightRoleGroup), this.RightRoleGroup, 2);
				Super.ShowNetWaiting(null);
			}
		};
		this.BtnYaZhu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.GetRoleOwnNumByMoneyType(8) + Global.Data.roleData.Money1 < ZhongShenZhengBaPart.GetDicSustain()[(this.Day <= 6) ? this.Day : 6].CostZhiChi)
			{
				string lang = Global.GetLang("金币不足");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
				return;
			}
			if (!this.isSelectRole)
			{
				string lang2 = Global.GetLang("点击选手头像选择押注");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang2, 0, -1, -1, 0);
				return;
			}
			string[] array = ZhongShenZhengBaPart.GetDicSustain()[(this.Day <= 6) ? this.Day : 6].MinLevel.Split(new char[]
			{
				','
			});
			if (Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level <= int.Parse(array[0]) * 100 + int.Parse(array[1]))
			{
				string textMsg = string.Format(Global.GetLang("此功能需要{0}转{1}级，才能执行操作！"), array[0], array[1]);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
				return;
			}
			if (this.isSelect)
			{
				GameInstance.Game.GetZhengBaSupport(this.MakeUnionGroup(this.LeftRoleGroup, this.RightRoleGroup), this.LeftRoleGroup, 3);
				Super.ShowNetWaiting(null);
			}
			else
			{
				GameInstance.Game.GetZhengBaSupport(this.MakeUnionGroup(this.LeftRoleGroup, this.RightRoleGroup), this.RightRoleGroup, 3);
				Super.ShowNetWaiting(null);
			}
		};
		UIEventListener.Get(this.TouXiangLeft.gameObject).onClick = delegate(GameObject e)
		{
			if (this.LeftRoleData == null)
			{
				string lang = Global.GetLang("此选手已经淘汰，不能进行押注");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
				return;
			}
			this.isSelect = true;
			this.isSelectRole = true;
			this.XuanZhongKuangLeft.gameObject.SetActive(true);
			this.XuanZhongKuangRight.gameObject.SetActive(false);
		};
		UIEventListener.Get(this.TouXiangRight.gameObject).onClick = delegate(GameObject e)
		{
			if (this.RightRoleData == null)
			{
				string lang = Global.GetLang("此选手已经淘汰，不能进行押注");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
				return;
			}
			this.isSelect = false;
			this.isSelectRole = true;
			this.XuanZhongKuangLeft.gameObject.SetActive(false);
			this.XuanZhongKuangRight.gameObject.SetActive(true);
		};
	}

	private bool IsCanZanBian(TianTiPaiHangRoleData Data)
	{
		if (Data == null)
		{
			string lang = Global.GetLang("此选手已经淘汰，不能进行赞操作");
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, lang, 0, -1, -1, 0);
			return false;
		}
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ZhongShenZhengBaPraiseMinLevel", ',');
		if (Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level <= systemParamIntArrayByName[0] * 100 + systemParamIntArrayByName[1])
		{
			string textMsg = string.Format(Global.GetLang("此功能需要{0}转{1}级，才能执行操作！"), systemParamIntArrayByName[0], systemParamIntArrayByName[1]);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
			return false;
		}
		return true;
	}

	public void InitChaKanInterFace(ZhengBaUnionGroupData Data)
	{
		if (this.LeftRoleData != null)
		{
			string url = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(this.LeftRoleData.Occupation),
				this.LeftRoleData.RoleData4Selector.RoleSex
			});
			this.TouXiangLeft.GetComponent<ShowNetImage>().URL = url;
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(this.LeftRoleData.ZoneId, out ztBuffServerInfo))
			{
				this.LeftName.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ffcc19",
					ztBuffServerInfo.strServerName
				}) + "\r\n" + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					this.LeftRoleData.RoleName
				});
			}
			else
			{
				this.LeftName.text = string.Format("{0}\r\n{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"ffcc19",
					this.LeftRoleData.ZoneId
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					this.LeftRoleData.RoleName
				}));
			}
			this.ZhanLiLeft.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("战力：")
			}), this.LeftRoleData.ZhanLi);
			this.TianTiPaiMingLeft.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("天梯排名：")
			}), this.LeftRoleData.DuanWeiRank);
		}
		if (this.RightRoleData != null)
		{
			string url2 = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(this.RightRoleData.Occupation),
				this.RightRoleData.RoleData4Selector.RoleSex
			});
			this.TouXiangRight.GetComponent<ShowNetImage>().URL = url2;
			ZtBuffServerInfo ztBuffServerInfo2 = null;
			if (Global.GetNowServerIsZhuTiFu(this.RightRoleData.ZoneId, out ztBuffServerInfo2))
			{
				this.RightName.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ffcc19",
					ztBuffServerInfo2.strServerName
				}) + "\r\n" + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					this.RightRoleData.RoleName
				});
			}
			else
			{
				this.RightName.text = string.Format("{0}\r\n{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"ffcc19",
					this.RightRoleData.ZoneId
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					this.RightRoleData.RoleName
				}));
			}
			this.ZhanLiRight.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("战力：")
			}), this.RightRoleData.ZhanLi);
			this.TianTiPaiMingRight.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("天梯排名：")
			}), this.RightRoleData.DuanWeiRank);
		}
		if (Data.SupportDatas != null)
		{
			int i = 0;
			int count = Data.SupportDatas.Count;
			while (i < count)
			{
				if (Data.SupportDatas[i].Group == this.LeftRoleGroup)
				{
					this.YaZhuRenShuLeft.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("押注人数：")
					}), Data.SupportDatas[i].TotalYaZhu);
					this.BtnLeftZan.Label.text = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Data.SupportDatas[i].TotalSupport
					});
					this.BtnLeftBian.Label.text = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Data.SupportDatas[i].TotalOppose
					});
				}
				else if (Data.SupportDatas[i].Group == this.RightRoleGroup)
				{
					this.YaZhuRenShuRight.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("押注人数：")
					}), Data.SupportDatas[i].TotalYaZhu);
					this.BtnRightZan.Label.text = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Data.SupportDatas[i].TotalSupport
					});
					this.BtnRightBian.Label.text = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Data.SupportDatas[i].TotalOppose
					});
				}
				i++;
			}
		}
		else
		{
			this.YaZhuRenShuLeft.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("押注人数：")
			}), 0);
			this.BtnLeftZan.Label.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				0
			});
			this.BtnLeftBian.Label.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				0
			});
			this.YaZhuRenShuRight.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("押注人数：")
			}), 0);
			this.BtnRightZan.Label.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				0
			});
			this.BtnRightBian.Label.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				0
			});
		}
		if (Data.SupportFlags != null)
		{
			bool flag = false;
			bool flag2 = false;
			int j = 0;
			int count2 = Data.SupportFlags.Count;
			while (j < count2)
			{
				if (Data.SupportFlags[j].Group == this.LeftRoleGroup)
				{
					this.LeftYaZhu.gameObject.SetActive(Data.SupportFlags[j].IsYaZhu);
					this.BtnLeftZan.isEnabled = (((!Data.SupportFlags[j].IsOppose && !Data.SupportFlags[j].IsSupport) ? 0 : 1) == 0);
					this.BtnLeftBian.isEnabled = (((!Data.SupportFlags[j].IsOppose && !Data.SupportFlags[j].IsSupport) ? 0 : 1) == 0);
					flag = Data.SupportFlags[j].IsYaZhu;
				}
				else if (Data.SupportFlags[j].Group == this.RightRoleGroup)
				{
					this.RightYaZhu.gameObject.SetActive(Data.SupportFlags[j].IsYaZhu);
					this.BtnRightZan.isEnabled = (((!Data.SupportFlags[j].IsOppose && !Data.SupportFlags[j].IsSupport) ? 0 : 1) == 0);
					this.BtnRightBian.isEnabled = (((!Data.SupportFlags[j].IsOppose && !Data.SupportFlags[j].IsSupport) ? 0 : 1) == 0);
					flag2 = Data.SupportFlags[j].IsYaZhu;
				}
				j++;
			}
			if (this.IsInGameTime())
			{
				this.BtnRightZan.isEnabled = false;
				this.BtnRightBian.isEnabled = false;
				this.BtnYaZhu.isEnabled = false;
			}
			else
			{
				this.BtnYaZhu.isEnabled = (!flag && !flag2 && !this.IsNotEnable);
			}
		}
		else
		{
			this.LeftYaZhu.gameObject.SetActive(false);
			this.RightYaZhu.gameObject.SetActive(false);
		}
		if (this.ItemCollection.Count > 0)
		{
			for (int k = 0; k < this.ItemCollection.Count; k++)
			{
				ZhongShenZhengBaPartChaKanPlayerList zhongShenZhengBaPartChaKanPlayerList = U3DUtils.AS<ZhongShenZhengBaPartChaKanPlayerList>(this.ItemCollection[k]);
				if (zhongShenZhengBaPartChaKanPlayerList != null)
				{
					Object.Destroy(zhongShenZhengBaPartChaKanPlayerList.gameObject);
				}
			}
		}
		if (Data.SupportLogs != null)
		{
			this.ItemCollection.Clear();
			int l = 0;
			int count3 = Data.SupportLogs.Count;
			while (l < count3)
			{
				string miaoshu = string.Empty;
				string text = string.Empty;
				if (Data.SupportLogs[l].ToGroup == this.LeftRoleData.ZhengBaGroup)
				{
					text = this.LeftRoleData.RoleName;
				}
				else if (Data.SupportLogs[l].ToGroup == this.RightRoleData.ZhengBaGroup)
				{
					text = this.RightRoleData.RoleName;
				}
				if (Data.SupportLogs[l].SupportType == 1)
				{
					miaoshu = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						text,
						"dac7ae",
						Global.GetLang("获得了赞赏，人气暴涨！")
					});
					goto IL_AF1;
				}
				if (Data.SupportLogs[l].SupportType == 2)
				{
					miaoshu = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						text,
						"dac7ae",
						Global.GetLang("被人踩了一脚，人气大跌！")
					});
					goto IL_AF1;
				}
				if (Data.SupportLogs[l].SupportType == 3)
				{
					miaoshu = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						text,
						"dac7ae",
						Global.GetLang("获得了重金押注！")
					});
					goto IL_AF1;
				}
				if (Data.SupportLogs[l].SupportType != 0)
				{
					goto IL_AF1;
				}
				IL_B2A:
				l++;
				continue;
				IL_AF1:
				ZhongShenZhengBaPartChaKanPlayerList zhongShenZhengBaPartChaKanPlayerList2 = U3DUtils.NEW<ZhongShenZhengBaPartChaKanPlayerList>();
				zhongShenZhengBaPartChaKanPlayerList2.Miaoshu = miaoshu;
				this.ItemCollection.AddNoUpdate(zhongShenZhengBaPartChaKanPlayerList2);
				UIPanel component = zhongShenZhengBaPartChaKanPlayerList2.GetComponent<UIPanel>();
				if (component)
				{
					Object.Destroy(component);
					goto IL_B2A;
				}
				goto IL_B2A;
			}
		}
	}

	private void MakeGroup(TianTiPaiHangRoleData leftrole, TianTiPaiHangRoleData rightrole, out int LeftRoleGroup, out int RightRoleGroup)
	{
		LeftRoleGroup = 0;
		RightRoleGroup = 0;
		if (leftrole != null)
		{
			LeftRoleGroup = leftrole.ZhengBaGroup;
		}
		if (rightrole != null)
		{
			RightRoleGroup = rightrole.ZhengBaGroup;
		}
	}

	private int MakeUnionGroup(int group1, int group2)
	{
		if (group1 > group2)
		{
			int num = group1;
			group1 = group2;
			group2 = num;
		}
		return group1 * 1000 + group2;
	}

	private void UnionGroup(int union, out int group1, out int group2)
	{
		group1 = union / 1000;
		group2 = union % 1000;
	}

	private bool IsInGameTime()
	{
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		DateTime dateTime3 = default(DateTime);
		string text = string.Format("{0}-{1}-{2}", Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month, Global.GetCorrectDateTime().Day);
		DateTime.TryParse(text + string.Format(" 15:30:00", new object[0]), ref dateTime);
		DateTime.TryParse(text + string.Format(" 16:30:00", new object[0]), ref dateTime2);
		dateTime3 = Global.GetCorrectDateTime();
		return dateTime3.Ticks > dateTime.Ticks && dateTime3.Ticks < dateTime2.Ticks;
	}

	public UILabel staticText;

	public DPSelectedItemEventHandler CloseHandler;

	public GButton BtnClose;

	public GButton BtnLeftZan;

	public GButton BtnLeftBian;

	public GButton BtnRightZan;

	public GButton BtnRightBian;

	public GButton BtnYaZhu;

	public UIButton TouXiangLeft;

	public UIButton TouXiangRight;

	public UILabel Title;

	public UILabel LeftName;

	public UILabel RightName;

	public UILabel ZhanLiLeft;

	public UILabel ZhanLiRight;

	public UILabel TianTiPaiMingLeft;

	public UILabel TianTiPaiMingRight;

	public UILabel YaZhuRenShuLeft;

	public UILabel YaZhuRenShuRight;

	public UILabel YaZhuSuoDei;

	public ListBox List;

	public UISprite LeftYaZhu;

	public UISprite RightYaZhu;

	public UISprite XuanZhongKuangLeft;

	public UISprite XuanZhongKuangRight;

	public UISprite WinLeft;

	public UISprite WinRight;

	public ShowNetImage playerLeft;

	public ShowNetImage playerRight;

	public ShowNetImage VS;

	private ObservableCollection _ItemCollection;

	public bool IsNotEnable;

	private int paihangday = 3;

	private int day = 3;

	private TianTiPaiHangRoleData LeftRoleData;

	private TianTiPaiHangRoleData RightRoleData;

	private int LeftRoleGroup;

	private int RightRoleGroup;

	private bool isSelect;

	private bool isSelectRole;
}
