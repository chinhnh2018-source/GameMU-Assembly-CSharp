using System;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ArmyGropCreatPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitData1();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.InitMode();
		if (Global.IsHavingBangHui() && !Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
		{
			NGUITools.SetActive(this._CreatBtn.gameObject, false);
			NGUITools.SetActive(this._TextBox.gameObject.transform.parent.gameObject, false);
		}
	}

	private void InitData1()
	{
		this.m_CreatArmyGroupNeedBHLev = (int)ConfigSystemParam.GetSystemParamIntByName("LegionsNeed");
		this.m_CreatArmyGroupNeedMoney = (int)ConfigSystemParam.GetSystemParamIntByName("LegionsCastZuanShi");
		string text = string.Format("{0}{1}", Global.Data.roleData.Faction, "LastCreatArmyGroupTime");
		if (PlayerPrefs.HasKey(text))
		{
			string @string = PlayerPrefs.GetString(text);
			if (!string.IsNullOrEmpty(@string))
			{
				this.mLastCreatArmyGroupTime = Convert.ToInt64(@string);
			}
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		if (!string.IsNullOrEmpty(this.bundleID))
		{
			MuAssetManager.Instance.StopInstantiate(this.bundleID, new Action<GameObject>(this.OnLoadComplete));
		}
	}

	private void InitMode()
	{
		this.bundleID = MuAssetManager.GetBundleID("UIModel", "juntuanqizhi_01");
		MuAssetManager.Instance.BeginInstantiate(this.bundleID, new Action<GameObject>(this.OnLoadComplete), CacheType.NotCache, CacheType.CacheAutoRelease, CacheType.NotCache);
	}

	private void OnLoadComplete(GameObject obj)
	{
		if (null != obj)
		{
			obj.transform.SetParent(this.ModeRoot, false);
		}
		U3DUtils.LoadRoleShaderAgain(obj);
		if (null != obj)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>(true);
			if (componentsInChildren != null && 0 < componentsInChildren.Length)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
					if (sharedMaterials != null)
					{
						byte b = 0;
						for (int j = 0; j < sharedMaterials.Length; j++)
						{
							string name = sharedMaterials[j].shader.name;
							if ("Artist/Diffuse" == name)
							{
								sharedMaterials[j].shader = Shader.Find("Artist/Diffuse Alpha Cut");
								b = 1;
								break;
							}
						}
						if (b == 1)
						{
							break;
						}
					}
				}
			}
		}
	}

	private void InitPrefabText()
	{
		this._ConditionLabels[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("只有战盟首领能创建军团")
		});
		this._ConditionLabels[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format(Global.GetLang("战盟等级达到{0}级"), this.m_CreatArmyGroupNeedBHLev)
		});
		this._ConditionLabels[2].text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format(Global.GetLang("需要消耗        {0}"), this.m_CreatArmyGroupNeedMoney)
		});
		this._ConditionLabels[3].text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Concat(new object[]
			{
				Global.GetLang("军团名称必须为"),
				Global.NameLengthRange[0],
				"-",
				Global.NameLengthRange[1],
				Global.GetLang("个字符或汉字")
			})
		});
		this._CreatBtn.Text = Global.GetLang("创建军团");
		this._OtherBtn.Text = Global.GetLang("其他军团");
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("LegionProsperityCost", ',');
		if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length == 4)
		{
			this._ConditionLabels[4].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(Global.GetLang("军团初始获得{0}繁荣度"), systemParamIntArrayByName[0])
			});
			this._ConditionLabels[5].text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(Global.GetLang("繁荣度降为{0}军团自动解散"), systemParamIntArrayByName[3])
			});
		}
		NGUITools.SetActive(this._ConditionLabels[6].transform.parent.gameObject, false);
		this._DescribrTextBox.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("请输入军团名字：")
		});
		this._TextBox.label.text = Global.GetLang("点击输入");
	}

	private void InitTexture()
	{
		if (null != this._BGTexture)
		{
			this._BGTexture.URL = "NetImages/GameRes/Images/ArmyGroup/ArmyGroupCreatBg.jpg";
		}
	}

	private void InitHandler()
	{
		if (null != this._ColseBtn)
		{
			this._ColseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.HanderC(e, new DPSelectedItemEventArgs
				{
					Index = 0
				});
			};
		}
		if (null != this._CreatBtn)
		{
			this._CreatBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				string empty = string.Empty;
				if (this.RoleCanCreatArmyGroup(true))
				{
					if (Global.Data.roleData.UserMoney >= this.m_CreatArmyGroupNeedMoney)
					{
						if (this.CheckArayGroupName(out empty))
						{
							this.HanderC(e, new DPSelectedItemEventArgs
							{
								Index = 1
							});
							GameInstance.Game.SendCreatArmyGroup(empty);
						}
					}
					else
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
					}
				}
			};
		}
		if (null != this._OtherBtn)
		{
			this._OtherBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.HanderC(e, new DPSelectedItemEventArgs
				{
					Index = 2
				});
			};
		}
		if (null != this._TextBox)
		{
			this._TextBox.TextChanged = delegate(object e, EventArgs s)
			{
			};
		}
	}

	private bool CheckArayGroupName(out string nameText)
	{
		this._TextBox.Text = Global.StringReplaceAll(this._TextBox.Text, "'", string.Empty);
		this._TextBox.Text = Global.StringReplaceAll(this._TextBox.Text, "|", string.Empty);
		this._TextBox.Text = Global.StringReplaceAll(this._TextBox.Text, "$", string.Empty);
		this._TextBox.Text = Global.StringReplaceAll(this._TextBox.Text, ":", string.Empty);
		if (this._TextBox.Text.Contains("{") || this._TextBox.Text.Contains("}"))
		{
			Super.HintMainText(Global.GetLang("军团名称中含有明感词汇，请重新输入!"), 10, 3);
			nameText = string.Empty;
			return false;
		}
		nameText = this._TextBox.Text;
		if (string.IsNullOrEmpty(this._TextBox.Text))
		{
			Super.HintMainText(Global.GetLang("抱歉,请输入的您要创建的军团名称!"), 10, 3);
			return false;
		}
		if (Global.GetLang("名字乘以五") == this._TextBox.Text)
		{
			Super.HintMainText(Global.GetLang("抱歉,请输入的您要创建的军团名称!"), 10, 3);
			return false;
		}
		byte b = Global.CheckRoleNameLenght(this._TextBox.Text);
		if (Global.NameLengthRange[0] == b)
		{
			Super.HintMainText(Global.GetLang("抱歉,您输入的军团名称不能少于") + b + Global.GetLang("个字，请重新输入!"), 10, 3);
			return false;
		}
		if (Global.NameLengthRange[1] == b)
		{
			Super.HintMainText(Global.GetLang("抱歉,您输入的军团名称已超过") + b + Global.GetLang("个字，请重新输入!"), 10, 3);
			return false;
		}
		if (this._TextBox.Text.IndexOf(" ") != -1)
		{
			Super.HintMainText(Global.GetLang("军团名称当中不允许包含空格，请重新输入!"), 10, 3);
			return false;
		}
		if (Global.IncludeReplaceFilterFileds(this._TextBox.Text))
		{
			Super.HintMainText(Global.GetLang("军团名称中含有明感词汇，请重新输入!"), 10, 3);
			return false;
		}
		return true;
	}

	private void HanderC(object e, DPSelectedItemEventArgs s)
	{
		if (this.Hander != null)
		{
			this.Hander(e, s);
		}
	}

	private bool RoleCanCreatArmyGroup(bool HaveErrHint = false)
	{
		if (!Global.IsHavingBangHui())
		{
			if (HaveErrHint)
			{
				Super.HintMainText(Global.GetLang("请创建或加入一个战盟"), 10, 3);
			}
			return false;
		}
		int num = (int)ConfigSystemParam.GetSystemParamIntByName("LegionsNeed");
		if (!Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
		{
			if (HaveErrHint)
			{
				Super.HintMainText(string.Format(Global.GetLang("只有{0}级战盟盟主才能创建军团"), num), 10, 3);
			}
			return false;
		}
		if (!GongnengYugaoMgr.IsIconOpened(69))
		{
			if (HaveErrHint)
			{
				Super.HintMainText(Global.GetLang("角色暂未开启军团功能"), 10, 3);
			}
			return false;
		}
		long num2 = (long)((int)ConfigSystemParam.GetSystemParamIntByName("LegionsCreateCD") * 60 * 60);
		long num3 = 0L;
		if (this.mLastCreatArmyGroupTime != 0L)
		{
			num3 = num2 - (long)((int)((Global.GetCorrectLocalTime() - this.mLastCreatArmyGroupTime) / 1000L));
		}
		if (num3 > 0L)
		{
			if (60L < num3)
			{
				int num4 = Mathf.FloorToInt((float)(num3 / 60L)) % 60;
				int num5 = Mathf.FloorToInt((float)(num3 / 60L / 60L));
				if (0 < num5)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("{0}小时之后才能创建军团"), new object[]
					{
						num5
					}), 10, 3);
				}
				else
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("{0}分钟之后才能创建军团"), new object[]
					{
						num4
					}), 10, 3);
				}
			}
			else
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("{0}秒之后才能创建军团"), new object[]
				{
					num3
				}), 10, 3);
			}
			return false;
		}
		return true;
	}

	public void CreateArmyGroupCallBack(int ret)
	{
		if (0 <= ret)
		{
			this.mLastCreatArmyGroupTime = Global.GetCorrectLocalTime();
			string text = string.Format("{0}{1}", Global.Data.roleData.Faction, "LastCreatArmyGroupTime");
			PlayerPrefs.SetString(text, this.mLastCreatArmyGroupTime.ToString());
			this.HanderC(null, new DPSelectedItemEventArgs
			{
				Index = 10,
				ID = 10
			});
			if (MUVoiceManager.GetInstance().CanSendVoice(false) && PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GameChatBoxMini != null && PlayZone.GlobalPlayZone.GameChatBoxMini.IsJunTuanVoiceIconIndex)
			{
				PlayZone.GlobalPlayZone.GameChatBoxMini.IsShowQuickVoice = true;
			}
		}
		else if (ret == -1008)
		{
			Super.HintMainText(string.Format(Global.GetLang("只有{0}级战盟盟主才能创建军团"), ConfigSystemParam.GetSystemParamIntByName("LegionsNeed")), 10, 3);
		}
		else
		{
			ArmyGroupPart.ErrorLog(ret);
		}
	}

	public void RefreshZhanMengData(BangHuiDetailData data)
	{
		Global.zhanmengLevel = data.QiLevel;
	}

	public GButton _ColseBtn;

	public GButton _CreatBtn;

	public GButton _OtherBtn;

	public ShowNetImage _BGTexture;

	public UILabel[] _ConditionLabels;

	public TextBox _TextBox;

	public UILabel _DescribrTextBox;

	public Transform ModeRoot;

	private int m_CreatArmyGroupNeedBHLev;

	private int m_CreatArmyGroupNeedMoney;

	private long mLastCreatArmyGroupTime;

	private string bundleID = string.Empty;

	public DPSelectedItemEventHandler Hander;
}
