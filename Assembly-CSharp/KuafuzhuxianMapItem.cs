using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class KuafuzhuxianMapItem : UserControl
{
	public int StateValue
	{
		get
		{
			return this._statevalue;
		}
		set
		{
			this._statevalue = value;
			this.SetState(this._statevalue);
		}
	}

	public int BossID
	{
		get
		{
			return this.bossID;
		}
		set
		{
			this.bossID = value;
		}
	}

	public int TeleportID
	{
		get
		{
			return this.teleportID;
		}
		set
		{
			this.teleportID = value;
		}
	}

	public int MapCode
	{
		get
		{
			return this.mapCode;
		}
		set
		{
			this.mapCode = value;
		}
	}

	public int LineNum
	{
		get
		{
			return this.lineNum;
		}
		set
		{
			this.lineNum = value;
			string text = string.Empty;
			if (this.MapCode == 50)
			{
				text = Global.GetLang("卡伦特");
			}
			else if (this.MapCode == 60)
			{
				text = Global.GetLang("幻术园");
			}
			else if (this.MapCode == 70)
			{
				text = Global.GetLang("阿卡伦西");
			}
			else if (this.MapCode == 80)
			{
				text = Global.GetLang("阿卡伦东");
			}
			else if (this.MapCode == 90)
			{
				text = Global.GetLang("邪恶寺庙");
			}
			else if (this.MapCode == 100)
			{
				text = Global.GetLang("魅音迷窟");
			}
			else
			{
				text = ConfigSettings.GetMapNameByCode(this.MapCode, false);
			}
			this.Line.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36e",
				string.Format(Global.GetLang("{0}{1}线"), text, this.lineNum)
			});
		}
	}

	private void SetState(int state)
	{
		if (state == -1)
		{
			this.StateLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"d02929",
				Global.GetLang("( 满员 )")
			});
			return;
		}
		if (state == 0)
		{
			this.StateLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("( 拥挤 )")
			});
			return;
		}
		if (state == 1)
		{
			this.StateLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("( 流畅 )")
			});
			return;
		}
		if (state == -2)
		{
			this.StateLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"d02929",
				Global.GetLang("( 不可用 )")
			});
			return;
		}
	}

	private void InitTextInPrefabs()
	{
		this.Line.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36e",
			string.Format(Global.GetLang("卡伦特{0}线"), this.lineNum)
		});
		this.StateLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"d02929",
			Global.GetLang("( 不可用 )")
		});
		this.Line.transform.localScale = new Vector3(20f, 20f, 1f);
		this.StateLab.transform.localScale = new Vector3(20f, 20f, 1f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnMap.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this._statevalue == -2)
			{
				return;
			}
			if (0f < Global.GetBtnCD(this.BtnMap.GetInstanceID()))
			{
				return;
			}
			Global.AddBtnCD(this.BtnMap.GetInstanceID(), 5f);
			int num = 0;
			int num2 = 0;
			Global.GetMapMinLevelAndZhuanSheng(this.mapCode, out num2, out num);
			if (Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level < num * 100 + num2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(string.Format(Global.GetLang("进入地图需要等级达到{0}转{1}级！"), num, num2), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (this.TeleportID == 0)
			{
				string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ShiJieChuanSong", '|');
				for (int i = 0; i < systemParamStringArrayByName.Length; i++)
				{
					if (this.mapCode == int.Parse(systemParamStringArrayByName[i].Split(new char[]
					{
						','
					})[0]))
					{
						int num3 = Global.GetRoleOwnNumByMoneyType(8) + Global.Data.roleData.Money1;
						if (num3 < int.Parse(systemParamStringArrayByName[i].Split(new char[]
						{
							','
						})[1]))
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("金币不足！"), new object[0]), 0, -1, -1, 0);
							return;
						}
					}
				}
			}
			if (this._statevalue == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该线路已满，无法进入，请选择其他线路！"), new object[0]), 0, -1, -1, 0);
				return;
			}
			GameInstance.Game.EnterKuaFuMap(this.mapCode, this.lineNum, this.BossID, this.TeleportID);
			base.StartCoroutine<bool>(this.ShowNetWaiting());
			this.DPS(this, new DPSelectedItemEventArgs());
		};
	}

	private IEnumerator ShowNetWaiting()
	{
		Super.ShowNetWaiting(null);
		yield return new WaitForSeconds(5f);
		Super.HideNetWaiting();
		yield break;
	}

	public GButton BtnMap;

	public UILabel Line;

	public UILabel StateLab;

	public DPSelectedItemEventHandler DPS;

	private int _statevalue;

	private int bossID;

	private int teleportID;

	private int mapCode;

	private int lineNum = 1;
}
