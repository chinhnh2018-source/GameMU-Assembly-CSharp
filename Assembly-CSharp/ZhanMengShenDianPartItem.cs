using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengShenDianPartItem : UserControl
{
	private void InitTextInPrefabs()
	{
		for (int i = 0; i < this.JianTou.Length; i++)
		{
			this.JianTou[i].transform.gameObject.SetActive(false);
		}
		for (int j = 1; j < this.ShouHuSprite.Length; j++)
		{
			this.ShouHuSprite[j - 1].GetComponent<ZhanMengShenDianSHouHu>().ID = j;
			this.ShouHuSprite[j - 1].GetComponent<ZhanMengShenDianSHouHu>().Level = 0;
			this.ShouHuSprite[j - 1].spriteName = string.Format("{0}gry", this.spriteName[j]);
		}
		this.ShowBaoJi.SetActive(false);
		this.ShowChaoJiBaoJi.SetActive(false);
		this.ManJi.gameObject.SetActive(false);
		this.Zhangong.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("战功消耗：")
		});
		this.MiaoShu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			Global.GetLang("战功消耗随升级次数增加，每日0点重置")
		});
		this.back.URL = "NetImages/GameRes/Images/Plate/shendianditu.jpg.qj";
		this.Zhangong.transform.localPosition = new Vector3(205f, -139f, -1f);
		this.ShouHuRightName.transform.localPosition = new Vector3(292f, 20f, -1f);
		this.MiaoShu.lineWidth = 300;
		this.Tisheng.Text = Global.GetLang("提升");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.Help.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenRuleInterFace();
		};
		this.Tisheng.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CurrentData.UnionLevel < ZhanMengShenDianPart.GetDicShenDianLevelUp()[this.CurrentData.StatueID].NeedZhanMengLevel)
			{
				string textMsg = string.Format(Global.GetLang("需要战盟等级提升至{0}级"), ZhanMengShenDianPart.GetDicShenDianLevelUp()[this.CurrentData.StatueID].NeedZhanMengLevel);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
				return;
			}
			if (this.CurrentData.ZhanGongNeed > this.CurrentData.ZhanGongLeft)
			{
				string textMsg2 = string.Format(Global.GetLang("需要消耗战功{0}"), this.CurrentData.ZhanGongNeed);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg2, 0, -1, -1, 0);
				return;
			}
			GameInstance.Game.SendShenDianTiShengInfo();
			Super.ShowNetWaiting(null);
		};
		this.JiaCheng.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenJiaChengInterFace();
		};
		this.ShuXiing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenShuXingInterFace();
		};
		UIEventListener.Get(this.ShouHuSprite[0].gameObject).onClick = delegate(GameObject e)
		{
			this.ClearXuanZhongTeXiao();
			this.LoadXuanZhongTeXiao(0);
			this.ChangeShouHu(0);
		};
		UIEventListener.Get(this.ShouHuSprite[1].gameObject).onClick = delegate(GameObject e)
		{
			this.ClearXuanZhongTeXiao();
			this.LoadXuanZhongTeXiao(1);
			this.ChangeShouHu(1);
		};
		UIEventListener.Get(this.ShouHuSprite[2].gameObject).onClick = delegate(GameObject e)
		{
			this.ClearXuanZhongTeXiao();
			this.LoadXuanZhongTeXiao(2);
			this.ChangeShouHu(2);
		};
		UIEventListener.Get(this.ShouHuSprite[3].gameObject).onClick = delegate(GameObject e)
		{
			this.ClearXuanZhongTeXiao();
			this.LoadXuanZhongTeXiao(3);
			this.ChangeShouHu(3);
		};
		UIEventListener.Get(this.ShouHuSprite[4].gameObject).onClick = delegate(GameObject e)
		{
			this.ClearXuanZhongTeXiao();
			this.LoadXuanZhongTeXiao(4);
			this.ChangeShouHu(4);
		};
		UIEventListener.Get(this.ShouHuSprite[5].gameObject).onClick = delegate(GameObject e)
		{
			this.ClearXuanZhongTeXiao();
			this.LoadXuanZhongTeXiao(5);
			this.ChangeShouHu(5);
		};
		UIEventListener.Get(this.ShouHuSprite[6].gameObject).onClick = delegate(GameObject e)
		{
			this.ClearXuanZhongTeXiao();
			this.LoadXuanZhongTeXiao(6);
			this.ChangeShouHu(6);
		};
		UIEventListener.Get(this.ShouHuSprite[7].gameObject).onClick = delegate(GameObject e)
		{
			this.ClearXuanZhongTeXiao();
			this.LoadXuanZhongTeXiao(7);
			this.ChangeShouHu(7);
		};
	}

	public override void Destroy()
	{
		if (this.lingyuList != null)
		{
			int count = this.lingyuList.Count;
			for (int i = 0; i < count; i++)
			{
				this.lingyuList[i].Stop();
			}
		}
		this.lingyuList.Clear();
		base.Destroy();
	}

	private void ChangeShouHu(int xuanzhongID)
	{
		this.JianTouLab[0].text = string.Empty;
		this.JianTouLab[1].text = string.Empty;
		this.JianTouLab[2].text = string.Empty;
		this.JianTouLab[3].text = string.Empty;
		if (xuanzhongID + 1 == this.CurrentID)
		{
			this.Tisheng.isEnabled = true;
		}
		else
		{
			this.Tisheng.isEnabled = false;
		}
		for (int i = 0; i < this.JianTou.Length; i++)
		{
			this.JianTou[i].transform.gameObject.SetActive(false);
		}
		this.ShouHuRight.spriteName = string.Format("{0}right", this.spriteName[xuanzhongID]);
		this.ShouHuRightName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			ZhanMengShenDianPart.GetDicShenDianTab()[xuanzhongID + 1].Name + "  Lv" + this.CurrentLevel
		});
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		if (xuanzhongID + 1 < this.CurrentID)
		{
			this.JinDuBaiFenBi.text = "100%";
			this.JinDutiao.transform.localScale = new Vector3(270f, 12f, 1f);
			this.ShouHuRightName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				ZhanMengShenDianPart.GetDicShenDianTab()[xuanzhongID + 1].Name + "  Lv" + (this.CurrentLevel + 1)
			});
			if (this.CurrentData.StatueID - (this.CurrentID - (xuanzhongID + 1)) >= 9)
			{
				for (int j = this.CurrentData.StatueID - (this.CurrentID - (xuanzhongID + 1)); j > 0; j -= 8)
				{
					num += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].MaxLifeV;
					num2 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].AddAttack;
					num3 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].AddDefense;
					num4 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].AddAttackInjure;
				}
			}
			else
			{
				num += ZhanMengShenDianPart.GetDicShenDianLevelUp()[this.CurrentData.StatueID - (this.CurrentID - (xuanzhongID + 1))].MaxLifeV;
				num2 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[this.CurrentData.StatueID - (this.CurrentID - (xuanzhongID + 1))].AddAttack;
				num3 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[this.CurrentData.StatueID - (this.CurrentID - (xuanzhongID + 1))].AddDefense;
				num4 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[this.CurrentData.StatueID - (this.CurrentID - (xuanzhongID + 1))].AddAttackInjure;
			}
			this.ShuXingLab[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("生命上限：{0}"), new object[0]),
				num
			});
			this.ShuXingLab[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("攻  击  力：{0}"), new object[0]),
				num2
			});
			this.ShuXingLab[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("防  御  力：{0}"), new object[0]),
				num3
			});
			this.ShuXingLab[3].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("附加伤害：{0}"), new object[0]),
				num4
			});
		}
		else if (xuanzhongID + 1 == this.CurrentID)
		{
			double num5 = (double)(this.CurrentData.LifeAdd + this.CurrentData.AttackAdd + this.CurrentData.AttackInjureAdd + this.CurrentData.DefenseAdd);
			double num6 = (double)(ZhanMengShenDianPart.GetDicShenDianLevelUp()[this.CurrentData.StatueID].MaxLifeV + ZhanMengShenDianPart.GetDicShenDianLevelUp()[this.CurrentData.StatueID].AddAttack + ZhanMengShenDianPart.GetDicShenDianLevelUp()[this.CurrentData.StatueID].AddDefense + ZhanMengShenDianPart.GetDicShenDianLevelUp()[this.CurrentData.StatueID].AddAttackInjure);
			this.JinDuBaiFenBi.text = (int)(num5 / num6 * 100.0) + "%";
			this.JinDutiao.transform.localScale = new Vector3((float)(270.0 * (num5 / num6)), 12f, 1f);
			if (this.CurrentData.StatueID >= 9)
			{
				for (int k = this.CurrentData.StatueID - 8; k > 0; k -= 8)
				{
					num += ZhanMengShenDianPart.GetDicShenDianLevelUp()[k].MaxLifeV;
					num2 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[k].AddAttack;
					num3 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[k].AddDefense;
					num4 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[k].AddAttackInjure;
				}
			}
			num += this.CurrentData.LifeAdd;
			num2 += this.CurrentData.AttackAdd;
			num3 += this.CurrentData.DefenseAdd;
			num4 += this.CurrentData.AttackInjureAdd;
			this.ShuXingLab[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("生命上限：{0}"), (this.CurrentData != null) ? num : 0)
			});
			this.ShuXingLab[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("攻  击  力：{0}"), (this.CurrentData != null) ? num2 : 0)
			});
			this.ShuXingLab[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("防  御  力：{0}"), (this.CurrentData != null) ? num3 : 0)
			});
			this.ShuXingLab[3].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("附加伤害：{0}"), (this.CurrentData != null) ? num4 : 0)
			});
		}
		else
		{
			this.JinDuBaiFenBi.text = "0%";
			this.JinDutiao.transform.localScale = new Vector3(0f, 12f, 1f);
			if (this.CurrentLevel == 0)
			{
				this.ShuXingLab[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("生命上限：0")
				});
				this.ShuXingLab[1].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("攻  击  力：0")
				});
				this.ShuXingLab[2].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("防  御  力：0")
				});
				this.ShuXingLab[3].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("附加伤害：0")
				});
			}
			else
			{
				if (this.CurrentData.StatueID - (this.CurrentID - (xuanzhongID + 1)) >= 9)
				{
					for (int l = this.CurrentData.StatueID - (this.CurrentID - (xuanzhongID + 1)) - 8; l > 0; l -= 8)
					{
						num += ZhanMengShenDianPart.GetDicShenDianLevelUp()[l].MaxLifeV;
						num2 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[l].AddAttack;
						num3 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[l].AddDefense;
						num4 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[l].AddAttackInjure;
					}
				}
				this.ShuXingLab[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("生命上限：{0}"), num)
				});
				this.ShuXingLab[1].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("攻  击  力：{0}"), num2)
				});
				this.ShuXingLab[2].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("防  御  力：{0}"), num3)
				});
				this.ShuXingLab[3].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("附加伤害：{0}"), num4)
				});
			}
		}
	}

	private void ClearXuanZhongTeXiao()
	{
		for (int i = 0; i < this.ShouHuTeXiao.Length; i++)
		{
			if (this.ShouHuTeXiao[i].childCount > 0)
			{
				for (int j = 0; j < this.ShouHuTeXiao[i].childCount; j++)
				{
					Transform transform = this.ShouHuTeXiao[i].FindChild("zhanshendian_shouhu_xuanzhong(Clone)");
					if (transform != null)
					{
						Object.Destroy(transform.gameObject);
					}
				}
			}
		}
	}

	public void analyzedShenDianServerInfo(UnionPalaceData data)
	{
		if (data == null)
		{
			return;
		}
		this.CurrentID = data.StatueType;
		this.CurrentLevel = data.StatueLevel;
		this.CurrentData = data;
		if (data.StatueLevel > data.UnionLevel)
		{
			this.InitShouHuItem(data, data.UnionLevel);
		}
		else
		{
			this.InitShouHuItem(data, data.StatueLevel);
		}
		if (data.StatueLevel != 0 && this.CurrentModalLevel != data.StatueLevel)
		{
			this.CurrentModalLevel = data.StatueLevel;
			if (this.ShowShengJiTeXiao.transform.childCount > 0)
			{
				for (int i = 0; i < this.ShowShengJiTeXiao.transform.childCount; i++)
				{
					GameObject gameObject = this.ShowShengJiTeXiao.transform.GetChild(i).gameObject;
					NGUITools.Destroy(gameObject);
				}
			}
			if (this.ShowJianModal.transform.childCount > 0)
			{
				for (int j = 0; j < this.ShowJianModal.transform.childCount; j++)
				{
					GameObject gameObject2 = this.ShowJianModal.transform.GetChild(j).gameObject;
					NGUITools.Destroy(gameObject2);
				}
			}
			this.LoadShengJiTeXiao(data.StatueLevel);
		}
	}

	private void InitShouHuItem(UnionPalaceData data, int level)
	{
		for (int i = 0; i < this.JianTou.Length; i++)
		{
			this.JianTou[i].transform.gameObject.SetActive(false);
		}
		if (data.LifeAdd + data.AttackAdd + data.AttackInjureAdd + data.DefenseAdd == 0 && data.StatueLevel == 0)
		{
			this.ShouHuSprite[data.StatueType - 1].spriteName = string.Format("{0}gry", this.spriteName[data.StatueType - 1]);
			this.Tisheng.Label.text = Global.GetLang("激活");
			this.isGry = true;
			this.ShuXingLab[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("生命上限：0")
			});
			this.ShuXingLab[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("攻  击  力：0")
			});
			this.ShuXingLab[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("防  御  力：0")
			});
			this.ShuXingLab[3].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("附加伤害：0")
			});
			this.JianTouLab[0].text = string.Empty;
			this.JianTouLab[1].text = string.Empty;
			this.JianTouLab[2].text = string.Empty;
			this.JianTouLab[3].text = string.Empty;
		}
		else
		{
			this.ShouHuSprite[data.StatueType - 1].spriteName = string.Format("{0}hover", this.spriteName[data.StatueType - 1]);
			this.Tisheng.Label.text = Global.GetLang("升级");
			if (this.isGry)
			{
				this.LoadQieHuanTeXiao(data.StatueType - 1);
				this.isGry = false;
			}
			int num = data.LifeAdd;
			int num2 = data.AttackAdd;
			int num3 = data.DefenseAdd;
			int num4 = data.AttackInjureAdd;
			if (data.StatueID >= 9)
			{
				for (int j = data.StatueID - 8; j > 0; j -= 8)
				{
					num += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].MaxLifeV;
					num2 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].AddAttack;
					num3 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].AddDefense;
					num4 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].AddAttackInjure;
				}
			}
			this.ShuXingLab[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("生命上限：{0}"), num)
			});
			this.ShuXingLab[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("攻  击  力：{0}"), num2)
			});
			this.ShuXingLab[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("防  御  力：{0}"), num3)
			});
			this.ShuXingLab[3].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("附加伤害：{0}"), num4)
			});
			string[] array = (level <= ZhanMengShenDianPart.GetDicShenDianScale().Count - 1) ? ZhanMengShenDianPart.GetDicShenDianScale()[level].Scale.Split(new char[]
			{
				'|'
			}) : null;
			if (array != null && data.ResultType != 4 && this.isFirstOpen)
			{
				string[] array2 = array[data.BurstType].Split(new char[]
				{
					','
				});
				string text = (data.BurstType != 0) ? ((data.BurstType != 1) ? "b266ff" : "3681f3") : "17e43e";
				this.JianTouLab[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					text,
					array2[1]
				});
				this.JianTouLab[1].text = Global.GetColorStringForNGUIText(new object[]
				{
					text,
					array2[2]
				});
				this.JianTouLab[2].text = Global.GetColorStringForNGUIText(new object[]
				{
					text,
					array2[3]
				});
				this.JianTouLab[3].text = Global.GetColorStringForNGUIText(new object[]
				{
					text,
					array2[4]
				});
				for (int k = 0; k < this.JianTou.Length; k++)
				{
					this.JianTou[k].gameObject.SetActive(true);
				}
			}
			else
			{
				this.JianTouLab[0].text = string.Empty;
				this.JianTouLab[1].text = string.Empty;
				this.JianTouLab[2].text = string.Empty;
				this.JianTouLab[3].text = string.Empty;
			}
		}
		this.isFirstOpen = true;
		this.ShouHuSprite[data.StatueType - 1].GetComponent<ZhanMengShenDianSHouHu>().ID = data.StatueType;
		this.ShouHuSprite[data.StatueType - 1].GetComponent<ZhanMengShenDianSHouHu>().Level = data.StatueLevel;
		this.InitRightAttr(data, level);
		if (data.StatueType - 2 >= 0 && this.ShouHuTeXiao[data.StatueType - 2].transform.childCount != 0 && data.LifeAdd + data.DefenseAdd + data.AttackAdd + data.AttackInjureAdd == 0)
		{
			for (int l = 0; l < this.ShouHuTeXiao[data.StatueType - 2].childCount; l++)
			{
				GameObject gameObject = this.ShouHuTeXiao[data.StatueType - 2].GetChild(l).gameObject;
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
			}
			this.LoadQieHuanTeXiao(data.StatueType - 2);
			this.LoadShouHuTeXiao(data.StatueType - 2, data.StatueLevel + 1);
			this.ShouHuSprite[data.StatueType - 2].spriteName = string.Format("{0}hover", this.spriteName[data.StatueType - 2]);
		}
		else if (data.StatueType == 1 && this.ShouHuTeXiao[7].transform.childCount != 0 && data.LifeAdd + data.DefenseAdd + data.AttackAdd + data.AttackInjureAdd == 0)
		{
			for (int m = 0; m < this.ShouHuTeXiao[7].childCount; m++)
			{
				GameObject gameObject2 = this.ShouHuTeXiao[7].GetChild(m).gameObject;
				if (gameObject2 != null)
				{
					Object.Destroy(gameObject2);
				}
			}
			this.LoadQieHuanTeXiao(7);
			this.LoadShouHuTeXiao(7, data.StatueLevel);
		}
		for (int n = data.StatueType - 2; n >= 0; n--)
		{
			if (this.ShouHuTeXiao[n].transform.childCount == 0)
			{
				this.ShouHuSprite[n].spriteName = string.Format("{0}hover", this.spriteName[n]);
				this.LoadShouHuTeXiao(n, data.StatueLevel + 1);
			}
		}
		for (int num5 = data.StatueType - 1; num5 < this.ShouHuSprite.Length; num5++)
		{
			if (this.ShouHuTeXiao[num5].transform.childCount == 0 && this.CurrentLevel != 0)
			{
				this.ShouHuSprite[num5].spriteName = string.Format("{0}hover", this.spriteName[num5]);
				this.LoadShouHuTeXiao(num5, data.StatueLevel);
			}
		}
		Transform transform = this.ShouHuTeXiao[data.StatueType - 1].FindChild("zhanshendian_shouhu_xuanzhong(Clone)");
		if (transform == null)
		{
			this.LoadXuanZhongTeXiao(data.StatueType - 1);
		}
	}

	private void InitRightAttr(UnionPalaceData data, int level)
	{
		this.ShouHuRight.spriteName = string.Format("{0}right", this.spriteName[data.StatueType - 1]);
		this.ShouHuRightName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			ZhanMengShenDianPart.GetDicShenDianTab()[data.StatueType].Name + "  Lv" + level
		});
		this.ZhangGongXiaohao.text = data.ZhanGongNeed.ToString();
		double num = (double)(data.LifeAdd + data.AttackAdd + data.AttackInjureAdd + data.DefenseAdd);
		double num2 = (double)(ZhanMengShenDianPart.GetDicShenDianLevelUp()[data.StatueID].MaxLifeV + ZhanMengShenDianPart.GetDicShenDianLevelUp()[data.StatueID].AddAttack + ZhanMengShenDianPart.GetDicShenDianLevelUp()[data.StatueID].AddDefense + ZhanMengShenDianPart.GetDicShenDianLevelUp()[data.StatueID].AddAttackInjure);
		if (data.StatueLevel == 5)
		{
			this.Tisheng.gameObject.SetActive(false);
			this.Zhangong.gameObject.SetActive(false);
			this.zhangongbiao.gameObject.SetActive(false);
			this.zhangongdi.gameObject.SetActive(false);
			this.ZhangGongXiaohao.text = string.Empty;
			this.MiaoShu.text = string.Empty;
			this.ManJi.gameObject.SetActive(true);
		}
		this.JinDuBaiFenBi.text = (int)(num / num2 * 100.0) + "%";
		this.JinDutiao.transform.localScale = new Vector3((float)(270.0 * (num / num2)), 12f, 1f);
		this.ShowBaoJi.SetActive(false);
		this.ShowChaoJiBaoJi.SetActive(false);
		if (data.BurstType == 1)
		{
			this.ShowBaoJi.SetActive(true);
		}
		else if (data.BurstType == 2)
		{
			this.ShowChaoJiBaoJi.SetActive(true);
		}
	}

	private void LoadXuanZhongTeXiao(int m)
	{
		string path = string.Format("UITeXiao/Perfabs/zhanmengshendian/zhanshendian_shouhu_xuanzhong", new object[0]);
		this.LoadTeXiao(path, this.ShouHuTeXiao[m].transform);
	}

	private void LoadQieHuanTeXiao(int m)
	{
		string path = string.Format("UITeXiao/Perfabs/zhanmengshendian/zhanshendian_shouhu_qiehuan", new object[0]);
		this.LoadTeXiao(path, this.ShouHuTeXiao[m].transform);
	}

	private void LoadShouHuTeXiao(int m, int n)
	{
		string path = string.Format("UITeXiao/Perfabs/zhanmengshendian/zhanshendian_shouhu_lv{0}", n);
		this.LoadTeXiao(path, this.ShouHuTeXiao[m].transform);
	}

	private void LoadShengJiTeXiao(int level)
	{
		base.StartCoroutine<bool>(this.ShengJiTeXiao(level));
	}

	private IEnumerator ShengJiTeXiao(int level)
	{
		string pathTeXiao = string.Format("UITeXiao/Perfabs/zhanmengshendian/ZhanMengShenDian_JieSuoXingXiang", new object[0]);
		this.LoadTeXiao(pathTeXiao, this.ShowShengJiTeXiao.transform);
		if (level == 5)
		{
			for (int i = 4; i <= level; i++)
			{
				string name = string.Format("ZMSD_JS_0{0}.unity3d", i);
				this.ShowModalByName(this.ShowJianModal, name, "Decoration", 1f);
			}
		}
		else
		{
			string name2 = string.Format("ZMSD_JS_0{0}.unity3d", level);
			this.ShowModalByName(this.ShowJianModal, name2, "Decoration", 1f);
		}
		yield return new WaitForSeconds(1f);
		for (int j = 1; j <= level; j++)
		{
			string name3 = string.Format("ZMSD_shouhu_0{0}.unity3d", j);
			this.ShowModalByName(this.ShowJianModal, name3, "Decoration", 1f);
		}
		yield break;
	}

	private GameObject LoadTeXiao(string path, Transform parent)
	{
		Object @object = Resources.Load(path);
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.SetParent(parent, false);
			U3DUtils.ResetLayer(gameObject, "MUUI");
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			return gameObject;
		}
		return null;
	}

	private void ShowModalByName(Modal3DShow ShowModal, string name, string path = "Decoration", float scale = 1f)
	{
		U3DUtils.AddChild(base.gameObject, ShowModal.gameObject, false);
		Transform transform = ShowModal.transform;
		UIHelper.SetModalPosZ(ShowModal.transform);
		ShowModal.transform.localPosition = new Vector3(0f, 0f, 0f);
		WingsLingyuResLoader wingsLingyuResLoader = UIHelper.LoadWuPinRes(ShowModal, name, path, scale);
		if (wingsLingyuResLoader != null)
		{
			this.lingyuList.Add(wingsLingyuResLoader);
		}
	}

	private void OpenRuleInterFace()
	{
		if (this.RuleWindow == null)
		{
			this.RuleWindow = U3DUtils.NEW<GChildWindow>();
			this.RuleWindow.IsShowModal = true;
			this.RuleWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.RuleWindow, Global.GetLang("规则"));
			Super.GData.GlobalPlayZone.Children.Add(this.RuleWindow);
		}
		if (this.RulePart == null)
		{
			this.RulePart = U3DUtils.NEW<ShenDianRule>();
			this.RulePart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseRuleInterFace();
			};
		}
		this.RuleWindow.SetContent(this.RuleWindow.BodyPresenter, this.RulePart, 0.0, 0.0, true);
	}

	private void CloseRuleInterFace()
	{
		if (this.RulePart)
		{
			this.RulePart.transform.parent = null;
			Object.Destroy(this.RulePart.gameObject);
			this.RulePart = null;
		}
		if (this.RuleWindow)
		{
			Super.CloseChildWindow(base.Children, this.RuleWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.RuleWindow, true);
			this.RuleWindow = null;
		}
	}

	private void OpenJiaChengInterFace()
	{
		if (this.jiachengWindow == null)
		{
			this.jiachengWindow = U3DUtils.NEW<GChildWindow>();
			this.jiachengWindow.IsShowModal = true;
			this.jiachengWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.jiachengWindow, Global.GetLang("加成"));
			Super.GData.GlobalPlayZone.Children.Add(this.jiachengWindow);
		}
		if (this.jiachengPart == null)
		{
			this.jiachengPart = U3DUtils.NEW<ShenDianEWaiJiaCheng>();
			this.jiachengPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseJiaChengInterFace();
			};
			this.jiachengPart.Title.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("守护额外加成")
			});
			this.jiachengPart.Curtitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("当前效果")
			});
			this.jiachengPart.Lowtitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("下级效果")
			});
			Dictionary<int, ShenDianExtra>.Enumerator enumerator = ZhanMengShenDianPart.GetDicShenDianExtra().GetEnumerator();
			int num = 0;
			if (this.CurrentData.ResultType == 4 && this.CurrentData != null)
			{
				if (this.CurrentData.StatueLevel != 0)
				{
					if (this.CurrentData.UnionLevel % 2 == 1)
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<int, ShenDianExtra> keyValuePair = enumerator.Current;
							ShenDianExtra value = keyValuePair.Value;
							if (value.ZhanMengLevel == this.CurrentData.UnionLevel)
							{
								num = value.StatueLevel;
								break;
							}
						}
					}
					else if (this.CurrentData.UnionLevel % 2 == 0)
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<int, ShenDianExtra> keyValuePair2 = enumerator.Current;
							ShenDianExtra value2 = keyValuePair2.Value;
							if (value2.ZhanMengLevel == this.CurrentData.UnionLevel - 1)
							{
								num = value2.StatueLevel;
								break;
							}
						}
					}
					this.jiachengPart.wenben1[0].text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("生命上限提高：+"),
						"17e43e",
						ZhanMengShenDianPart.GetDicShenDianExtra()[num].MaxLifePercent * 100.0 + "%"
					});
					this.jiachengPart.wenben2[0].text = Global.GetColorStringForNGUIText(new object[]
					{
						(this.CurrentData.StatueLevel < ((num + 1 <= 5) ? (num + 1) : 5)) ? "ff0000" : "17e43e",
						string.Format(Global.GetLang("需要守护等级全部达到{0}级"), (num + 1 <= 5) ? (num + 1) : 5)
					});
					this.jiachengPart.wenben2[1].text = Global.GetColorStringForNGUIText(new object[]
					{
						(this.CurrentData.UnionLevel < ZhanMengShenDianPart.GetDicShenDianExtra()[num + 1].ZhanMengLevel) ? "ff0000" : "17e43e",
						string.Format(Global.GetLang("需要战盟等级达到{0}级"), ZhanMengShenDianPart.GetDicShenDianExtra()[(num + 1 <= 5) ? (num + 1) : 5].ZhanMengLevel)
					});
					this.jiachengPart.wenben2[2].text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("生命上限提高：+"),
						"ff0000",
						ZhanMengShenDianPart.GetDicShenDianExtra()[(num + 1 <= 5) ? (num + 1) : 5].MaxLifePercent * 100.0 + "%"
					});
				}
				else
				{
					this.jiachengPart.wenben1[0].text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("无效果")
					});
					this.jiachengPart.wenben2[0].text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("需要守护等级全部达到{1}级")
					});
					this.jiachengPart.wenben2[1].text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("需要战盟等级达到{0}级")
					});
					this.jiachengPart.wenben2[2].text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("生命上限提高：+"),
						"ff0000",
						ZhanMengShenDianPart.GetDicShenDianExtra()[1].MaxLifePercent * 100.0 + "%"
					});
				}
			}
			else if (this.CurrentData != null && this.CurrentData.StatueLevel != 0)
			{
				if (ZhanMengShenDianPart.GetDicShenDianExtra()[this.CurrentData.StatueLevel].ZhanMengLevel <= this.CurrentData.UnionLevel)
				{
					this.jiachengPart.wenben1[0].text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("生命上限提高：+"),
						"17e43e",
						ZhanMengShenDianPart.GetDicShenDianExtra()[this.CurrentData.StatueLevel].MaxLifePercent * 100.0 + "%"
					});
					if (this.CurrentData.StatueLevel >= 5)
					{
						this.jiachengPart.wenben2[0].text = Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							string.Format(Global.GetLang("战盟神殿已达到满级"), new object[0])
						});
					}
					else
					{
						this.jiachengPart.wenben2[0].text = Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							string.Format(Global.GetLang("需要守护等级全部达到{0}级"), this.CurrentData.StatueLevel + 1)
						});
						this.jiachengPart.wenben2[1].text = Global.GetColorStringForNGUIText(new object[]
						{
							(this.CurrentData.UnionLevel < ZhanMengShenDianPart.GetDicShenDianExtra()[this.CurrentData.StatueLevel + 1].ZhanMengLevel) ? "ff0000" : "17e43e",
							string.Format(Global.GetLang("需要战盟等级达到{0}级"), ZhanMengShenDianPart.GetDicShenDianExtra()[this.CurrentData.StatueLevel + 1].ZhanMengLevel)
						});
						this.jiachengPart.wenben2[2].text = Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							Global.GetLang("生命上限提高：+"),
							"ff0000",
							ZhanMengShenDianPart.GetDicShenDianExtra()[this.CurrentData.StatueLevel + 1].MaxLifePercent * 100.0 + "%"
						});
					}
				}
				else
				{
					this.jiachengPart.wenben1[0].text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("生命上限提高：+"),
						"17e43e",
						ZhanMengShenDianPart.GetDicShenDianExtra()[this.CurrentData.StatueLevel - 1].MaxLifePercent * 100.0 + "%"
					});
					this.jiachengPart.wenben2[0].text = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format(Global.GetLang("需要守护等级全部达到{0}级"), this.CurrentData.StatueLevel)
					});
					this.jiachengPart.wenben2[1].text = Global.GetColorStringForNGUIText(new object[]
					{
						(this.CurrentData.UnionLevel < ZhanMengShenDianPart.GetDicShenDianExtra()[this.CurrentData.StatueLevel].ZhanMengLevel) ? "ff0000" : "17e43e",
						string.Format(Global.GetLang("需要战盟等级达到{0}级"), ZhanMengShenDianPart.GetDicShenDianExtra()[this.CurrentData.StatueLevel].ZhanMengLevel)
					});
					this.jiachengPart.wenben2[2].text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("生命上限提高：+"),
						"ff0000",
						ZhanMengShenDianPart.GetDicShenDianExtra()[this.CurrentData.StatueLevel].MaxLifePercent * 100.0 + "%"
					});
				}
			}
			else
			{
				this.jiachengPart.wenben1[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("无效果")
				});
				this.jiachengPart.wenben2[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("需要守护等级全部达到1级")
				});
				this.jiachengPart.wenben2[1].text = Global.GetColorStringForNGUIText(new object[]
				{
					(this.CurrentData.UnionLevel < 1) ? "ff0000" : "17e43e",
					Global.GetLang("需要战盟等级达到1级")
				});
				this.jiachengPart.wenben2[2].text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("生命上限提高：+"),
					"ff0000",
					ZhanMengShenDianPart.GetDicShenDianExtra()[1].MaxLifePercent * 100.0 + "%"
				});
			}
		}
		this.jiachengWindow.SetContent(this.jiachengWindow.BodyPresenter, this.jiachengPart, 0.0, 0.0, true);
	}

	private void CloseJiaChengInterFace()
	{
		if (this.jiachengPart)
		{
			this.jiachengPart.transform.parent = null;
			Object.Destroy(this.jiachengPart.gameObject);
			this.jiachengPart = null;
		}
		if (this.jiachengWindow)
		{
			Super.CloseChildWindow(base.Children, this.jiachengWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.jiachengWindow, true);
			this.jiachengWindow = null;
		}
	}

	private void OpenShuXingInterFace()
	{
		if (this.shuxingWindow == null)
		{
			this.shuxingWindow = U3DUtils.NEW<GChildWindow>();
			this.shuxingWindow.IsShowModal = true;
			this.shuxingWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.shuxingWindow, Global.GetLang("属性"));
			Super.GData.GlobalPlayZone.Children.Add(this.shuxingWindow);
		}
		if (this.shuxingPart == null)
		{
			this.shuxingPart = U3DUtils.NEW<ShenDianSHuXingZonglan>();
			this.shuxingPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseShuXingInterFace();
			};
		}
		this.shuxingWindow.SetContent(this.shuxingWindow.BodyPresenter, this.shuxingPart, 0.0, 0.0, true);
		this.shuxingPart.Shuzhi[0].gameObject.SetActive(true);
		this.shuxingPart.Shuzhi[1].gameObject.SetActive(true);
		this.shuxingPart.Shuzhi[2].gameObject.SetActive(true);
		this.shuxingPart.Shuzhi[3].gameObject.SetActive(true);
		this.shuxingPart.Shuzhi[4].gameObject.SetActive(true);
		this.shuxingPart.Shuzhi[5].gameObject.SetActive(true);
		if (this.CurrentData == null)
		{
			this.shuxingPart.Title1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("属性总览")
			});
			this.shuxingPart.Shuzhi[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("生命上限：0")
			});
			this.shuxingPart.Shuzhi[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("攻  击  力：0")
			});
			this.shuxingPart.Shuzhi[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("防  御  力：0")
			});
			this.shuxingPart.Shuzhi[3].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("附加伤害：0")
			});
			this.shuxingPart.Shuzhi[4].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("生命上限提高：0")
			});
		}
		else
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			Dictionary<int, ShenDianLevelUp>.Enumerator enumerator = ZhanMengShenDianPart.GetDicShenDianLevelUp().GetEnumerator();
			int num9 = 0;
			Dictionary<int, ShenDianExtra>.Enumerator enumerator2 = ZhanMengShenDianPart.GetDicShenDianExtra().GetEnumerator();
			int num10 = 0;
			for (int i = 1; i < this.CurrentData.StatueID; i++)
			{
				num += ZhanMengShenDianPart.GetDicShenDianLevelUp()[i].MaxLifeV;
				num2 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[i].AddAttack;
				num3 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[i].AddDefense;
				num4 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[i].AddAttackInjure;
			}
			num += this.CurrentData.LifeAdd;
			num2 += this.CurrentData.AttackAdd;
			num3 += this.CurrentData.DefenseAdd;
			num4 += this.CurrentData.AttackInjureAdd;
			if (this.CurrentData.UnionLevel % 2 == 0)
			{
				for (int j = 1; j <= this.CurrentData.UnionLevel / 2 * 8; j++)
				{
					num5 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].MaxLifeV;
					num6 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].AddAttack;
					num7 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].AddDefense;
					num8 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[j].AddAttackInjure;
				}
			}
			else if (this.CurrentData.UnionLevel % 2 == 1)
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, ShenDianLevelUp> keyValuePair = enumerator.Current;
					ShenDianLevelUp value = keyValuePair.Value;
					if (value.NeedZhanMengLevel == this.CurrentData.UnionLevel)
					{
						num9 = value.Level;
						break;
					}
				}
				for (int k = 1; k <= (1 + num9) * 8; k++)
				{
					num5 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[k].MaxLifeV;
					num6 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[k].AddAttack;
					num7 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[k].AddDefense;
					num8 += ZhanMengShenDianPart.GetDicShenDianLevelUp()[k].AddAttackInjure;
				}
			}
			if (this.CurrentData.ResultType == 4 && num5 != num)
			{
				this.shuxingPart.Title1.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("属性总览")
				});
				this.shuxingPart.Shuzhi[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("生命上限："),
					"dac7ae",
					num5,
					"b266ff",
					string.Format(Global.GetLang("(全部激活{0})"), num)
				});
				this.shuxingPart.Shuzhi[1].text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("攻  击  力："),
					"dac7ae",
					num6,
					"b266ff",
					string.Format(Global.GetLang("(全部激活{0})"), num2)
				});
				this.shuxingPart.Shuzhi[2].text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("防  御  力："),
					"dac7ae",
					num7,
					"b266ff",
					string.Format(Global.GetLang("(全部激活{0})"), num3)
				});
				this.shuxingPart.Shuzhi[3].text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("附加伤害："),
					"dac7ae",
					num8,
					"b266ff",
					string.Format(Global.GetLang("(全部激活{0})"), num4)
				});
				this.shuxingPart.Shuzhi[5].text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("因战盟等级不足，仅激活部分属性")
				});
			}
			else
			{
				this.shuxingPart.Title1.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("属性总览")
				});
				this.shuxingPart.Shuzhi[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("生命上限："),
					"dac7ae",
					num
				});
				this.shuxingPart.Shuzhi[1].text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("攻  击  力："),
					"dac7ae",
					num2
				});
				this.shuxingPart.Shuzhi[2].text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("防  御  力："),
					"dac7ae",
					num3
				});
				this.shuxingPart.Shuzhi[3].text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("附加伤害："),
					"dac7ae",
					num4
				});
			}
			if (this.CurrentData.StatueLevel != 0)
			{
				if (ZhanMengShenDianPart.GetDicShenDianLevelUp()[this.CurrentData.StatueID].NeedZhanMengLevel <= this.CurrentData.UnionLevel && this.CurrentData.StatueLevel != 5)
				{
					this.shuxingPart.Shuzhi[4].text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("生命上限提高："),
						"dac7ae",
						ZhanMengShenDianPart.GetDicShenDianExtra()[this.CurrentData.StatueLevel].MaxLifePercent * 100.0 + "%"
					});
				}
				else
				{
					if (this.CurrentData.UnionLevel % 2 == 1)
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<int, ShenDianExtra> keyValuePair2 = enumerator2.Current;
							ShenDianExtra value2 = keyValuePair2.Value;
							if (value2.ZhanMengLevel == this.CurrentData.UnionLevel)
							{
								num10 = ((this.CurrentData.StatueLevel < value2.StatueLevel) ? (value2.StatueLevel - 1) : value2.StatueLevel);
								break;
							}
						}
					}
					else if (this.CurrentData.UnionLevel % 2 == 0)
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<int, ShenDianExtra> keyValuePair3 = enumerator2.Current;
							ShenDianExtra value3 = keyValuePair3.Value;
							if (value3.ZhanMengLevel == this.CurrentData.UnionLevel - 1)
							{
								num10 = ((this.CurrentData.StatueLevel < value3.StatueLevel) ? (value3.StatueLevel - 1) : value3.StatueLevel);
								break;
							}
						}
					}
					this.shuxingPart.Shuzhi[4].text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("生命上限提高："),
						"dac7ae",
						ZhanMengShenDianPart.GetDicShenDianExtra()[num10].MaxLifePercent * 100.0 + "%"
					});
				}
			}
			else
			{
				this.shuxingPart.Shuzhi[4].text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("生命上限提高：0")
				});
			}
		}
	}

	private void CloseShuXingInterFace()
	{
		if (this.shuxingPart)
		{
			this.shuxingPart.transform.parent = null;
			Object.Destroy(this.shuxingPart.gameObject);
			this.shuxingPart = null;
		}
		if (this.shuxingWindow)
		{
			Super.CloseChildWindow(base.Children, this.shuxingWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.shuxingWindow, true);
			this.shuxingWindow = null;
		}
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton Help;

	public GButton Tisheng;

	public GButton JiaCheng;

	public GButton ShuXiing;

	public UISprite[] ShouHuSprite;

	public Transform[] ShouHuTeXiao;

	public UISprite ShouHuRight;

	public UISprite JinDutiao;

	public UISprite ManJi;

	public UILabel[] ShuXingLab;

	public UILabel[] JianTouLab;

	public UISprite[] JianTou;

	public UISprite zhangongbiao;

	public UISprite zhangongdi;

	public UILabel Zhangong;

	public UILabel ZhangGongXiaohao;

	public UILabel MiaoShu;

	public UILabel JinDuBaiFenBi;

	public UILabel ShouHuRightName;

	public ShowNetImage back;

	public GameObject ShowShengJiTeXiao;

	public GameObject ShowBaoJi;

	public GameObject ShowChaoJiBaoJi;

	public Modal3DShow ShowJianModal;

	public Modal3DShow ShowJieSuoModal;

	protected GChildWindow RuleWindow;

	protected ShenDianRule RulePart;

	protected GChildWindow jiachengWindow;

	protected ShenDianEWaiJiaCheng jiachengPart;

	protected GChildWindow shuxingWindow;

	protected ShenDianSHuXingZonglan shuxingPart;

	private int CurrentID;

	private int CurrentLevel;

	private UnionPalaceData CurrentData;

	private int CurrentModalLevel;

	private bool isGry;

	private bool isFirstOpen;

	private string[] spriteName = new string[]
	{
		"yongqishouhu_",
		"youqingshouhu_",
		"aixinshouhu_",
		"zhishishouhu_",
		"chengshishouhu_",
		"chunzhenshouhu_",
		"xiwangshouhu_",
		"guangmingshouhu_"
	};

	private List<WingsLingyuResLoader> lingyuList = new List<WingsLingyuResLoader>();
}
