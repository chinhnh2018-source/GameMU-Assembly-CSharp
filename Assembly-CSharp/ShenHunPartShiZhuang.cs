using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;
using XMLCreater;

public class ShenHunPartShiZhuang : UserControl
{
	private void InitTextInPrefabs()
	{
		try
		{
			this.btnShengJi.Text = Global.GetLang("升级");
			this.btnEquip.Text = Global.GetLang("装备");
			for (int i = 0; i < this.lstInfoNowWord.Count; i++)
			{
				this.lstInfoNowWord[i].pivot = 3;
				this.lstInfoNowWord[i].transform.localPosition = new Vector3(-40f, this.lstInfoNowWord[i].transform.localPosition.y, -1f);
			}
		}
		catch
		{
			MUDebug.Log<string>(new string[]
			{
				"越南东南亚报空！"
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.LoadAllShiZhuang();
		this.btnShengJi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_nowSelected.GoodsData == null)
			{
				return;
			}
			if (this.BeMaterialsEnough())
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendFashionUp(this.m_nowSelected.GoodsData.Id);
			}
			else
			{
				Super.HintMainText(Global.GetLang("所需材料不足"), 10, 3);
			}
		};
		this.btnEquip.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_nowSelected.GoodsData == null)
			{
				return;
			}
			GoodsData equipedBianShenFashion = ShenHunData.GetEquipedBianShenFashion();
			if (equipedBianShenFashion != this.m_nowSelected.GoodsData)
			{
				if (equipedBianShenFashion == null)
				{
					GameInstance.Game.SpriteModGoods(1, this.m_nowSelected.GoodsData.Id, this.m_nowSelected.GoodsData.GoodsID, 1, 6000, this.m_nowSelected.GoodsData.GCount, this.m_nowSelected.GoodsData.BagIndex, string.Empty);
				}
				else
				{
					GameInstance.Game.SpriteModGoods(2, equipedBianShenFashion.Id, equipedBianShenFashion.GoodsID, 0, 6000, this.m_nowSelected.GoodsData.GCount, this.m_nowSelected.GoodsData.BagIndex, string.Empty);
					GameInstance.Game.SpriteModGoods(1, this.m_nowSelected.GoodsData.Id, this.m_nowSelected.GoodsData.GoodsID, 1, 6000, this.m_nowSelected.GoodsData.GCount, this.m_nowSelected.GoodsData.BagIndex, string.Empty);
				}
			}
			else
			{
				GameInstance.Game.SpriteModGoods(2, this.m_nowSelected.GoodsData.Id, this.m_nowSelected.GoodsData.GoodsID, 0, 6000, this.m_nowSelected.GoodsData.GCount, this.m_nowSelected.GoodsData.BagIndex, string.Empty);
			}
		};
	}

	private void LoadAllShiZhuang()
	{
		this.m_allItems.Clear();
		ShenHunPartShiZhuangItem shenHunPartShiZhuangItem = null;
		List<MUTransfigurationFashion> allFashionTopLevel = ShenHunData.GetAllFashionTopLevel();
		int num = allFashionTopLevel.Count / 4;
		if (num < 2)
		{
			num = 2;
		}
		for (int i = 0; i < allFashionTopLevel.Count; i++)
		{
			bool flag = false;
			GoodsData goodsData = this.HaveOwnData(allFashionTopLevel[i].GoodsID, ref flag);
			if (goodsData != null)
			{
				ShenHunPartShiZhuangItem shenHunPartShiZhuangItem2 = U3DUtils.NEW<ShenHunPartShiZhuangItem>();
				shenHunPartShiZhuangItem2.transform.SetParent(this.gridShiZhuang.transform);
				shenHunPartShiZhuangItem2.IntShiZhuang(allFashionTopLevel[i], goodsData);
				shenHunPartShiZhuangItem2.OnSelectShiZhuang = new Action<ShenHunPartShiZhuangItem>(this.OnSelectShiZhuang);
				shenHunPartShiZhuangItem2.transform.localScale = Vector3.one;
				shenHunPartShiZhuangItem2.transform.localPosition = Vector3.zero;
				shenHunPartShiZhuangItem2.BeEquip = false;
				this.m_allItems.Add(shenHunPartShiZhuangItem2);
				if (shenHunPartShiZhuangItem == null)
				{
					shenHunPartShiZhuangItem = shenHunPartShiZhuangItem2;
				}
				if (flag)
				{
					shenHunPartShiZhuangItem = shenHunPartShiZhuangItem2;
					shenHunPartShiZhuangItem2.BeEquip = true;
				}
			}
		}
		this.imgShiZhuangBg.transform.localScale = new Vector3(this.gridShiZhuang.cellWidth * 4f, this.gridShiZhuang.cellHeight * (float)num);
		this.gridShiZhuang.Reposition();
		this.OnSelectShiZhuang(shenHunPartShiZhuangItem);
	}

	private GoodsData HaveOwnData(int goodsId, ref bool beUse)
	{
		beUse = false;
		if (Global.Data.fashionAndTitleList == null)
		{
			return null;
		}
		for (int i = 0; i < Global.Data.fashionAndTitleList.Count; i++)
		{
			if (Global.Data.fashionAndTitleList[i].GoodsID == goodsId)
			{
				beUse = (Global.Data.fashionAndTitleList[i].Using == 1);
				return Global.Data.fashionAndTitleList[i];
			}
		}
		return null;
	}

	public void RefershInofs()
	{
		if (this.m_nowSelected == null)
		{
			return;
		}
		ShenHunPartShiZhuangItem nowSelected = this.m_nowSelected;
		this.m_nowSelected = null;
		this.m_modelId = 0;
		this.OnSelectShiZhuang(nowSelected);
	}

	private void OnSelectShiZhuang(ShenHunPartShiZhuangItem shiZhuang)
	{
		if (shiZhuang == null)
		{
			return;
		}
		if (this.m_nowSelected == shiZhuang)
		{
			return;
		}
		if (this.m_nowSelected != null)
		{
			this.m_nowSelected.BSelect = false;
		}
		if (this.m_nowSelected != shiZhuang)
		{
			this.m_nowSelected = shiZhuang;
			shiZhuang.BSelect = true;
			int goodsID = shiZhuang.Fashion.GoodsID;
			GoodsData goodsData = shiZhuang.GoodsData;
			int num = 0;
			if (goodsData != null)
			{
				num = goodsData.Forge_level;
			}
			if (num > 0)
			{
				this.lblName.text = shiZhuang.Fashion.Name + "    +" + num;
			}
			else
			{
				this.lblName.text = shiZhuang.Fashion.Name;
			}
			this.SetUpdateInfo(goodsID, num);
			this.LoadSkill(goodsID, num);
			this.LoadEffectSkill(goodsID, num);
			this.LoadRewards(goodsID, num);
			this.Load3DModel(shiZhuang.Fashion.MOD);
			this.btnShengJi.gameObject.SetActive(this.m_nowSelected.BeOwn);
			this.btnEquip.gameObject.SetActive(this.m_nowSelected.BeOwn);
			if (this.m_nowSelected.BeOwn)
			{
				MUTransfigurationFashion transfigurationFashion = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationFashion(goodsID, num + 1);
				bool flag = transfigurationFashion == null;
				this.btnShengJi.gameObject.SetActive(!flag);
				if (ShenHunData.IsFashionEquiped(goodsID))
				{
					this.btnEquip.Text = Global.GetLang("卸载");
				}
				else
				{
					this.btnEquip.Text = Global.GetLang("装备");
				}
			}
			else
			{
				this.btnShengJi.gameObject.SetActive(false);
			}
		}
	}

	private void SetUpdateInfo(int goodsId, int nowLevel)
	{
		int level = nowLevel + 1;
		MUTransfigurationFashion transfigurationFashion = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationFashion(goodsId, nowLevel);
		if (transfigurationFashion == null)
		{
			Debug.LogError(Global.GetLang("读取神魂附体等级数据错误"));
			return;
		}
		MUTransfigurationFashion transfigurationFashion2 = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationFashion(goodsId, level);
		bool flag = transfigurationFashion2 == null;
		this.btnShengJi.gameObject.SetActive(!flag);
		List<MUPropInfo> proPerty = transfigurationFashion.ProPerty;
		this.lblLevel.text = string.Empty;
		this.lstInfoNowWord[0].text = Global.GetLang("持续时间");
		this.lstInfoNowValue[0].text = transfigurationFashion.Duration + Global.GetLang("秒");
		if (flag)
		{
			this.lstInfoAdd[0].gameObject.SetActive(false);
			this.lstInfoAddSprite[0].gameObject.SetActive(false);
		}
		else
		{
			int num = transfigurationFashion2.Duration - transfigurationFashion.Duration;
			if (num <= 0)
			{
				this.lstInfoAdd[0].gameObject.SetActive(false);
				this.lstInfoAddSprite[0].gameObject.SetActive(false);
			}
			else
			{
				this.lstInfoAdd[0].text = num + Global.GetLang("秒");
			}
		}
		for (int i = 0; i < this.lstInfoNowWord.Count - 1; i++)
		{
			if (i < proPerty.Count)
			{
				this.lstInfoNowWord[i + 1].gameObject.SetActive(true);
				this.lstInfoNowValue[i + 1].gameObject.SetActive(true);
				this.lstInfoAdd[i + 1].gameObject.SetActive(true);
				this.lstInfoAddSprite[i + 1].gameObject.SetActive(true);
				string chinesePropName = proPerty[i].ChinesePropName;
				this.lstInfoNowWord[i + 1].text = chinesePropName;
				if (proPerty[i].BePercent)
				{
					this.lstInfoNowValue[i + 1].text = Mathf.RoundToInt(proPerty[i].PropNum * 100f) + "%";
				}
				else
				{
					this.lstInfoNowValue[i + 1].text = Mathf.RoundToInt(proPerty[i].PropNum).ToString();
				}
				if (flag)
				{
					this.lstInfoAdd[i + 1].gameObject.SetActive(false);
					this.lstInfoAddSprite[i + 1].gameObject.SetActive(false);
				}
				else
				{
					List<MUPropInfo> proPerty2 = transfigurationFashion2.ProPerty;
					if (proPerty[i].BePercent)
					{
						int num2 = Mathf.RoundToInt((proPerty2[i].PropNum - proPerty[i].PropNum) * 100f);
						if (num2 <= 0)
						{
							this.lstInfoAdd[i + 1].gameObject.SetActive(false);
							this.lstInfoAddSprite[i + 1].gameObject.SetActive(false);
						}
						else
						{
							this.lstInfoAdd[i + 1].text = num2 + "%";
						}
					}
					else
					{
						int num3 = Mathf.RoundToInt(proPerty2[i].PropNum - proPerty[i].PropNum);
						if (num3 <= 0)
						{
							this.lstInfoAdd[i + 1].gameObject.SetActive(false);
							this.lstInfoAddSprite[i + 1].gameObject.SetActive(false);
						}
						else
						{
							this.lstInfoAdd[i + 1].text = num3.ToString();
						}
					}
				}
			}
			else
			{
				this.lstInfoNowWord[i + 1].gameObject.SetActive(false);
				this.lstInfoAdd[i + 1].gameObject.SetActive(false);
				this.lstInfoNowValue[i + 1].gameObject.SetActive(false);
				this.lstInfoAddSprite[i + 1].gameObject.SetActive(false);
			}
		}
	}

	private void LoadSkill(int goodsId, int fashionLevel)
	{
		List<int> allFashionSkills = ShenHunData.GetAllFashionSkills(goodsId);
		for (int i = 0; i < allFashionSkills.Count; i++)
		{
			ShenHunPartSkill shenHunPartSkill = this.skills[i];
			shenHunPartSkill.OnClickSkill = new Action<ShenHunPartSkill>(this.OnClickSkill);
			int bianShen = ShenHunData.GetSelfShenHunLevel().BianShen;
			int fashionSkillJieSuoLevel = IConfigbase<ConfigShenHun>.Instance.GetFashionSkillJieSuoLevel(goodsId, i);
			shenHunPartSkill.InitSkill(allFashionSkills[i], bianShen, fashionSkillJieSuoLevel, fashionLevel);
		}
		this.gridSkill.Reposition();
	}

	private void LoadEffectSkill(int goodsId, int level)
	{
		MUTransfigurationFashion transfigurationFashion = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationFashion(goodsId, level);
		ShenHunPartSkill shenHunPartSkill = this.skills[3];
		shenHunPartSkill.OnClickSkill = new Action<ShenHunPartSkill>(this.OnClickEffectSkill);
		int fashionEffectJieSuoLevel = IConfigbase<ConfigShenHun>.Instance.GetFashionEffectJieSuoLevel(transfigurationFashion.GoodsID);
		int bianShen = ShenHunData.GetSelfShenHunLevel().BianShen;
		int level2 = (fashionEffectJieSuoLevel <= level) ? level : fashionEffectJieSuoLevel;
		MUTransfigurationFashion transfigurationFashion2 = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationFashion(goodsId, level2);
		shenHunPartSkill.InitShiZhuangEffect(transfigurationFashion2.Effect, fashionEffectJieSuoLevel, level);
	}

	private void OnClickSkill(ShenHunPartSkill skill)
	{
		this.skillDetailWindow.gameObject.SetActive(true);
		this.skillDetailWindow.InitSkill(skill.SkillInfo, skill.SkillLevel);
	}

	private void OnClickEffectSkill(ShenHunPartSkill skill)
	{
		this.skillDetailWindow.gameObject.SetActive(true);
		MUTransfigurationFashion nextEffectByNowEffectID = IConfigbase<ConfigShenHun>.Instance.GetNextEffectByNowEffectID(this.m_nowSelected.Fashion.GoodsID, skill.EffectInfo.ID);
		MUTransfigurationFashionEffect nextEffect = null;
		int nextlevel = 1;
		if (nextEffectByNowEffectID != null)
		{
			nextEffect = IConfigbase<ConfigShenHun>.Instance.GetFashionEffectByID(nextEffectByNowEffectID.Effect);
			nextlevel = nextEffectByNowEffectID.level;
		}
		this.skillDetailWindow.InitEffect(skill.EffectInfo, nextEffect, nextlevel);
	}

	private void LoadRewards(int goodsId, int nowLevel)
	{
		for (int i = 0; i < this.gridIcon.transform.childCount; i++)
		{
			Object.Destroy(this.gridIcon.transform.GetChild(i).gameObject);
		}
		MUTransfigurationFashion transfigurationFashion = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationFashion(goodsId, nowLevel + 1);
		if (transfigurationFashion == null)
		{
			return;
		}
		List<MUMaterialInfo> needGoods = transfigurationFashion.NeedGoods;
		for (int j = 0; j < needGoods.Count; j++)
		{
			if (needGoods[j].MaterialId > 0)
			{
				GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIconByGoodsID(needGoods[j].MaterialId, true);
				ggoodIcon.transform.SetParent(this.gridIcon.transform);
				ggoodIcon.transform.localScale = new Vector3(1f, 1f, 1f);
				ggoodIcon.transform.localPosition = new Vector3(this.gridIcon.cellWidth * (float)j, 0f, 0f);
				int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(needGoods[j].MaterialId);
				ggoodIcon.SecondText.Label.supportEncoding = true;
				if (roleGoodsNumberCountByGoodsID >= needGoods[j].Num)
				{
					ggoodIcon.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Format("{0}/{1}", roleGoodsNumberCountByGoodsID, needGoods[j].Num)
					});
				}
				else
				{
					ggoodIcon.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format("{0}/{1}", roleGoodsNumberCountByGoodsID, needGoods[j].Num)
					});
				}
			}
		}
		float num = (float)(1 - needGoods.Count) * this.gridIcon.cellWidth / 2f;
		this.gridIcon.transform.localPosition = new Vector3(num, 0f, 0f);
	}

	private void Load3DModel(int modelId)
	{
		if (this.m_modelId == modelId)
		{
			return;
		}
		this.m_modelId = modelId;
		this.modal3DShow.Clear();
		if (this.resLoader != null)
		{
			this.resLoader.Stop();
		}
		this.resLoader = UIHelper.LoadBianShenRes(this.modal3DShow, modelId, -1);
	}

	private void RefershEquipState()
	{
		for (int i = 0; i < this.m_allItems.Count; i++)
		{
			GoodsData goodsData = this.m_allItems[i].GoodsData;
			if (goodsData != null)
			{
				this.m_allItems[i].BeEquip = ShenHunData.IsFashionEquiped(goodsData.GoodsID);
			}
		}
	}

	private bool BeMaterialsEnough()
	{
		int goodsID = this.m_nowSelected.Fashion.GoodsID;
		int forge_level = this.m_nowSelected.GoodsData.Forge_level;
		MUTransfigurationFashion transfigurationFashion = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationFashion(goodsID, forge_level);
		List<MUMaterialInfo> needGoods = transfigurationFashion.NeedGoods;
		for (int i = 0; i < needGoods.Count; i++)
		{
			int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(needGoods[i].MaterialId);
			if (roleGoodsNumberCountByGoodsID < needGoods[i].Num)
			{
				return false;
			}
		}
		return true;
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<int, int>("CMD_SPR_FASHION_FORGE", new Action<int, int>(this.ServerFashionForge));
		MUEventManager.AddEventListener<int, bool>("CMD_SPR_FASHION_EQUIP", new Action<int, bool>(this.ServerFashionEquip));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<int, int>("CMD_SPR_FASHION_FORGE", new Action<int, int>(this.ServerFashionForge));
		MUEventManager.RemoveEventListener<int, bool>("CMD_SPR_FASHION_EQUIP", new Action<int, bool>(this.ServerFashionEquip));
	}

	private void ServerFashionForge(int goodsId, int level)
	{
		Super.HintMainText(Global.GetLang("升级成功"), 10, 3);
		ShenHunPartShiZhuangItem nowSelected = this.m_nowSelected;
		this.m_nowSelected = null;
		for (int i = 0; i < this.m_allItems.Count; i++)
		{
			GoodsData goodsData = this.m_allItems[i].GoodsData;
			if (goodsData != null)
			{
				if (goodsData.Id == goodsId)
				{
					goodsData.Forge_level = level;
					break;
				}
			}
		}
		this.OnSelectShiZhuang(nowSelected);
	}

	private void ServerFashionEquip(int goodsId, bool beEquip)
	{
		if (this.m_nowSelected.GoodsData.GoodsID == goodsId)
		{
			if (beEquip)
			{
				this.btnEquip.Text = Global.GetLang("卸载");
				Super.HintMainText(Global.GetLang("装备成功"), 10, 3);
			}
			else
			{
				this.btnEquip.Text = Global.GetLang("装备");
				Super.HintMainText(Global.GetLang("卸载成功"), 10, 3);
			}
		}
		this.RefershEquipState();
	}

	private const int ColNum = 4;

	public UILabel lblName;

	public UILabel lblLevel;

	public List<UILabel> lstInfoNowWord;

	public List<UILabel> lstInfoNowValue;

	public List<UILabel> lstInfoAdd;

	public List<UISprite> lstInfoAddSprite;

	public UIGrid gridIcon;

	public UIGrid gridSkill;

	public UIGrid gridShiZhuang;

	public GButton btnShengJi;

	public GButton btnEquip;

	public List<ShenHunPartSkill> skills;

	public UISprite imgShiZhuangBg;

	public GameObject modelContainer;

	public Modal3DShow modal3DShow;

	private ShenHunPartShiZhuangItem m_nowSelected;

	private List<ShenHunPartShiZhuangItem> m_allItems = new List<ShenHunPartShiZhuangItem>();

	[HideInInspector]
	public ShenHunPartSkillDetail skillDetailWindow;

	private MonsterNPCResLoader resLoader;

	private int m_modelId;
}
