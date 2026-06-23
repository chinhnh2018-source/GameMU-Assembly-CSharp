using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class JingLingJiNengChuanchengPart : UserControl
{
	protected override void OnDestroy()
	{
		if (this.messageBoxWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
		}
		base.OnDestroy();
	}

	private void InitTextInPrefabs()
	{
		this.SubmitBtn.Text = Global.GetLang("传承");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.m_BtnhuodongListOBC = this.m_ListBox.ItemsSource;
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.spZuanShi, "JingLingJiNengChuanCheng", ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoZhuanShi", true).SafeToInt32(0), string.Empty);
		if (IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("JingLingJiNengChuanCheng", ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoZhuanShi", true).SafeToInt32(0), false))
		{
			this.spXiaoHao.spriteName = "XiaoHaoHuanLeDaiBi";
		}
		else
		{
			this.spXiaoHao.spriteName = "XiaoHaoZuanShi";
		}
	}

	public void InitValue()
	{
		this.RadioTongqian._Lable.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoJinBi", true)
		});
		this.RadioYuanbao._Lable.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoZhuanShi", true)
		});
		this.LingJingText.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			"0"
		});
	}

	public void InitPartSize()
	{
		this.ClearSubIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearSubIcon.gameObject.SetActive(false);
			if (this.EquipSub.GetComponentInChildren<GGoodIcon>() != null)
			{
				Object.Destroy(this.EquipSub.GetComponentInChildren<GGoodIcon>().gameObject);
			}
			this.Clear();
			if (this.EquipAdd.GetComponentInChildren<GGoodIcon>() != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					FilterType = 3,
					ZhuZhuangBei = null,
					FuZhuangBei = (this.EquipAdd.GetComponentInChildren<GGoodIcon>().ItemObject as GoodsData)
				});
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					FilterType = 3,
					ZhuZhuangBei = null,
					FuZhuangBei = null
				});
			}
		};
		this.ClearAddIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearAddIcon.gameObject.SetActive(false);
			if (this.EquipAdd.GetComponentInChildren<GGoodIcon>() != null)
			{
				Object.Destroy(this.EquipAdd.GetComponentInChildren<GGoodIcon>().gameObject);
			}
			this.LingJingText.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				"0"
			});
			if (this.EquipSub.GetComponentInChildren<GGoodIcon>() != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					FilterType = 3,
					ZhuZhuangBei = (this.EquipSub.GetComponentInChildren<GGoodIcon>().ItemObject as GoodsData),
					FuZhuangBei = null
				});
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					FilterType = 3,
					ZhuZhuangBei = null,
					FuZhuangBei = null
				});
			}
		};
		this.RadioTongqian.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.RadioIndex = 0;
			this.RadioTongqian.isChecked = true;
			this.RadioYuanbao.isChecked = false;
		};
		this.RadioYuanbao.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.RadioIndex = 1;
			this.RadioTongqian.isChecked = false;
			this.RadioYuanbao.isChecked = true;
		};
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.RadioIndex == 1 && Global.Data.roleData.UserMoney >= ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoZhuanShi", true).SafeToInt32(0) && ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoZhuanShi", true).SafeToInt32(0) > 0)
			{
				if (this.messageBoxWindow != null)
				{
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
				}
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "JingLingJiNengChuanCheng", ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoZhuanShi", true).SafeToInt32(0));
				this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoZhuanShi", true).SafeToInt32(0), text)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.StartChuancheng();
					}
					return true;
				};
				return;
			}
			this.StartChuancheng();
		};
	}

	public void AddEquip(GoodsData gd)
	{
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.spZuanShi, "JingLingJiNengChuanCheng", ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoZhuanShi", true).SafeToInt32(0), string.Empty);
		if (IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("JingLingJiNengChuanCheng", ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoZhuanShi", true).SafeToInt32(0), false))
		{
			this.spXiaoHao.spriteName = "XiaoHaoHuanLeDaiBi";
		}
		else
		{
			this.spXiaoHao.spriteName = "XiaoHaoZuanShi";
		}
		if (gd != null)
		{
			if (this.EquipSub.Length() > 0 && this.EquipAdd.Length() > 0)
			{
				return;
			}
			if (this.EquipSub.Length() <= 0 && this.EquipAdd.Length() <= 0)
			{
				this.AddEquipGoodsIcon(gd, this.EquipSub, false, 0);
				this.RefrshJiNnengList(gd.ElementhrtsProps, gd.Forge_level);
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					FilterType = 3,
					ZhuZhuangBei = gd,
					FuZhuangBei = null
				});
				return;
			}
			if (this.EquipSub.Length() <= 0 && this.EquipAdd.Length() > 0)
			{
				this.AddEquipGoodsIcon(gd, this.EquipSub, false, 0);
				this.RefrshJiNnengList(gd.ElementhrtsProps, gd.Forge_level);
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					FilterType = 3,
					ZhuZhuangBei = gd,
					FuZhuangBei = (this.EquipAdd.GetComponentInChildren<GGoodIcon>().ItemObject as GoodsData)
				});
				return;
			}
			if (this.EquipSub.Length() > 0 && this.EquipAdd.Length() <= 0)
			{
				this.AddEquipGoodsIcon(gd, this.EquipAdd, false, 0);
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					FilterType = 3,
					ZhuZhuangBei = (this.EquipSub.GetComponentInChildren<GGoodIcon>().ItemObject as GoodsData),
					FuZhuangBei = gd
				});
				return;
			}
		}
	}

	private void StartChuancheng()
	{
		if (this.EquipSub.Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离精灵不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.EquipAdd.Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承精灵不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int srcPetID = -1;
		int tarPetID = -1;
		GoodsData goodsData = this.EquipSub.GetComponentInChildren<GGoodIcon>().ItemObject as GoodsData;
		goodsData = Global.GetEquipDataByID(goodsData.Id);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离精灵不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.InitSkillsData(goodsData.ElementhrtsProps, goodsData.Forge_level) <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前剥离精灵身上没有技能"), new object[0]), 0, -1, -1, 0);
			return;
		}
		srcPetID = goodsData.Id;
		goodsData = (this.EquipAdd.GetComponentInChildren<GGoodIcon>().ItemObject as GoodsData);
		goodsData = Global.GetEquipDataByID(goodsData.Id);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		tarPetID = goodsData.Id;
		if (this.RadioIndex == 0)
		{
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoJinBi", true).SafeToInt32(0))
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
				return;
			}
		}
		else if (this.RadioIndex == 1 && Global.Data.roleData.UserMoney < ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoZhuanShi", true).SafeToInt32(0) && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("JingLingJiNengChuanCheng", ConfigSystemParam.GetSystemParamByName("JingLingChuanChengXiaoHaoZhuanShi", true).SafeToInt32(0), false))
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		}
		if (this.InitSkillsData(goodsData.ElementhrtsProps, goodsData.Forge_level) > 0)
		{
			string chineseText = Global.GetColorStringForNGUIText(new object[]
			{
				"ffcc19",
				Global.GetLang("传承精灵当前已拥有技能，传承后")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("原有技能会被移除")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"ffcc19",
				Global.GetLang("，原有技能升级消耗灵晶会全额返还，确定要传承？")
			});
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang(chineseText), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					GameInstance.Game.GetPetSkill(srcPetID, tarPetID, this.RadioIndex);
				}
				return true;
			};
			return;
		}
		GameInstance.Game.GetPetSkill(srcPetID, tarPetID, this.RadioIndex);
	}

	public void RefreshData(int ret, string leftData = null, string rightData = null)
	{
		if (ret != 1)
		{
			this.ErrorHandle((EPetSkillState)ret);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = -1
			});
			return;
		}
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < leftData.Split(new char[]
		{
			','
		}).Length; i++)
		{
			list.Add(leftData.Split(new char[]
			{
				','
			})[i].SafeToInt32(0));
		}
		for (int j = 0; j < rightData.Split(new char[]
		{
			','
		}).Length; j++)
		{
			list2.Add(rightData.Split(new char[]
			{
				','
			})[j].SafeToInt32(0));
		}
		GoodsData goodsData = this.EquipSub.GetComponentInChildren<GGoodIcon>().ItemObject as GoodsData;
		goodsData.ElementhrtsProps = list;
		GoodsData goodsData2 = this.EquipAdd.GetComponentInChildren<GGoodIcon>().ItemObject as GoodsData;
		goodsData2.ElementhrtsProps = list2;
		for (int k = 0; k < Global.Data.roleData.GoodsDataList.Count; k++)
		{
			if (goodsData.Id == Global.Data.roleData.GoodsDataList[k].Id)
			{
				Global.Data.roleData.GoodsDataList[k] = goodsData;
			}
			else if (goodsData2.Id == Global.Data.roleData.GoodsDataList[k].Id)
			{
				Global.Data.roleData.GoodsDataList[k] = goodsData2;
			}
		}
		for (int l = 0; l < Global.Data.equipPet.Count; l++)
		{
			if (goodsData.Id == Global.Data.equipPet[l].Id)
			{
				Global.Data.equipPet[l] = goodsData;
			}
			else if (goodsData2.Id == Global.Data.equipPet[l].Id)
			{
				Global.Data.equipPet[l] = goodsData2;
			}
		}
		Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			FilterType = 3,
			ZhuZhuangBei = goodsData,
			FuZhuangBei = goodsData2
		});
		this.AddEquipGoodsIcon(goodsData, this.EquipSub, false, 0);
		this.AddEquipGoodsIcon(goodsData2, this.EquipAdd, false, 0);
		this.DPEffectItem(this, new NotifyLianluEffectEventArgs
		{
			EffectID = 1
		});
		this.Clear();
	}

	private void ErrorHandle(EPetSkillState state)
	{
		string text = string.Empty;
		switch (state + 14)
		{
		case EPetSkillState.Default:
			text = Global.GetLang("传承失败");
			break;
		case EPetSkillState.Success:
			text = Global.GetLang("金币不足");
			break;
		case (EPetSkillState)4:
			text = Global.GetLang("没有技能可以领悟");
			break;
		case (EPetSkillState)5:
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, Global.GetLang("钻石"));
			break;
		case (EPetSkillState)6:
			text = Global.GetLang("没有槽位可以觉醒");
			break;
		case (EPetSkillState)7:
			text = Global.GetLang("槽位未开放");
			break;
		case (EPetSkillState)8:
			text = Global.GetLang("槽位是最高级");
			break;
		case (EPetSkillState)9:
			text = Global.GetLang("槽位错误");
			break;
		case (EPetSkillState)10:
			text = Global.GetLang("灵晶不足");
			break;
		case (EPetSkillState)11:
			text = Global.GetLang("没有入库");
			break;
		case (EPetSkillState)12:
			text = Global.GetLang("宠物不存在");
			break;
		case (EPetSkillState)13:
			text = Global.GetLang("功能未开放");
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			Super.HintMainText(text, 10, 3);
		}
	}

	private void AddEquipGoodsIcon(GoodsData gd, SpriteSL parent, bool grayShow = false, int goodsOwnerType = 0)
	{
		parent.Clear();
		ObservableCollection itemsSource = parent.GetComponent<ListBox>().ItemsSource;
		itemsSource.Clear();
		Super.AddGoodsIcon(gd, itemsSource, false, true);
		this.RefreshBtn();
		if (this.EquipSub.Length() > 0 && this.EquipAdd.Length() > 0 && this.EquipAdd.GetComponentInChildren<GGoodIcon>() != null && this.EquipAdd.GetComponentInChildren<GGoodIcon>().ItemObject != null)
		{
			int[] jingLingRecoverAward = Global.GetJingLingRecoverAward(this.EquipAdd.GetComponentInChildren<GGoodIcon>().ItemObject as GoodsData);
			if (jingLingRecoverAward.Length >= 3)
			{
				this.LingJingText.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					jingLingRecoverAward[2].ToString()
				});
			}
		}
	}

	public int InitSkillsData(List<int> data, int Forge_level = 0)
	{
		int num = 0;
		bool[] array = this.CheckJingLingLev(Forge_level + 1);
		if (data == null)
		{
			return 0;
		}
		if (0 < data.Count)
		{
			int i = 0;
			int num2 = 2;
			int num3 = 0;
			while (i < data.Count)
			{
				if (array[num3] && data[i] == 1 && data[num2] > 0)
				{
					num++;
				}
				i = num2 + 1;
				int num4 = i + 1;
				num2 = num4 + 1;
				num3++;
			}
		}
		return num;
	}

	private new void Clear()
	{
		this.m_BtnhuodongListOBC.Clear();
		this.LingJingText.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			"0"
		});
	}

	private void RefrshJiNnengList(List<int> data, int Forge_level = 0)
	{
		this.m_BtnhuodongListOBC.Clear();
		if (data == null)
		{
			return;
		}
		bool[] array = this.CheckJingLingLev(Forge_level + 1);
		if (0 < data.Count)
		{
			int i = 0;
			int num = 1;
			int num2 = 2;
			int num3 = 0;
			while (i < data.Count)
			{
				if (array[num3] && data[i] == 1 && data[num2] > 0)
				{
					JingLingJiNengChuanchengItem jingLingJiNengChuanchengItem = U3DUtils.NEW<JingLingJiNengChuanchengItem>();
					jingLingJiNengChuanchengItem.Lev = data[num];
					jingLingJiNengChuanchengItem.SkillId = data[num2];
					jingLingJiNengChuanchengItem.SetSkillId(data[num2], data[num]);
					this.m_BtnhuodongListOBC.AddNoUpdate(jingLingJiNengChuanchengItem);
				}
				i = num2 + 1;
				num = i + 1;
				num2 = num + 1;
				num3++;
			}
		}
	}

	private bool[] CheckJingLingLev(int Forge_level)
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("PatSkillCostLevel", '|');
		bool[] array = new bool[4];
		byte b = 0;
		while ((int)b < systemParamStringArrayByName.Length)
		{
			string[] array2 = systemParamStringArrayByName[(int)b].Split(new char[]
			{
				','
			});
			if (Forge_level >= Convert.ToInt32(array2[1]))
			{
				array[(int)b] = true;
			}
			b += 1;
		}
		return array;
	}

	private void RefreshBtn()
	{
		if (this.EquipSub.Length() > 0)
		{
			this.ClearSubIcon.gameObject.SetActive(true);
		}
		else
		{
			this.ClearSubIcon.gameObject.SetActive(false);
		}
		if (this.EquipAdd.Length() > 0)
		{
			this.ClearAddIcon.gameObject.SetActive(true);
		}
		else
		{
			this.ClearAddIcon.gameObject.SetActive(false);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	public SpriteSL Body;

	public ShowNetImage Bak;

	public GButton SubmitBtn;

	public GCheckBox RadioTongqian;

	public GCheckBox RadioYuanbao;

	public TextBlock LingJingText;

	public SpriteSL EquipSub;

	public SpriteSL EquipAdd;

	public GButton ClearSubIcon;

	public GButton ClearAddIcon;

	public ListBox m_ListBox;

	private int RadioIndex = 1;

	public UISprite spZuanShi;

	public UISprite spXiaoHao;

	private ObservableCollection m_BtnhuodongListOBC;

	private GChildWindow messageBoxWindow;
}
