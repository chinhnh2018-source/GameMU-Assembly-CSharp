using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;
using XMLCreater;

public class LianluChongShengJinJiePart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblZhanLi.text = Global.GetLang(string.Empty);
		this.lblNeed1Value.text = Global.GetLang(string.Empty);
		this.lblNeed2Value.text = Global.GetLang(string.Empty);
		this.lblNeed3Value.text = Global.GetLang(string.Empty);
		this.btnSubmit.Label.text = Global.GetLang("升阶");
		this.lblSuccessRate.text = string.Empty;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnSubmit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.m_beCanDoNext)
			{
				return;
			}
			this.StartZaizaoEquip();
		};
	}

	public void InitPartSize(int p1, int p2)
	{
	}

	public void InitPartData()
	{
	}

	public void InitAllValue(bool beContainNextEquip = true)
	{
		this.slEquipNow.Clear();
		if (beContainNextEquip)
		{
			this.slEquipNext.Clear();
		}
		this.lblZhanLi.text = Global.GetLang(string.Empty);
		this.lblNeed1Value.transform.parent.gameObject.SetActive(true);
		this.lblNeed2Value.transform.parent.gameObject.SetActive(true);
		this.lblNeed3Value.transform.parent.gameObject.SetActive(true);
		this.lblNeed1Value.text = Global.GetLang("重生淬炼点: ") + ChongShengData.GetRebornCuiLianNum();
		this.lblNeed2Value.text = Global.GetLang("重生锻造点: ") + ChongShengData.GetRebornDuanZaoNum();
		this.lblNeed3Value.text = Global.GetLang("重生涅槃点: ") + ChongShengData.GetRebornNiePanNum();
		this.lblSuccessRate.text = Global.GetLang("请放入重生装备");
		this.ResetPosition();
	}

	public void AddEquip(GoodsData gd)
	{
		if (!this.m_beCanDoNext)
		{
			return;
		}
		if (gd != null)
		{
			this.InitAllValue(true);
			if (this.effect2.activeSelf)
			{
				this.effect2.SetActive(false);
			}
			if (ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID) == null)
			{
				return;
			}
			MURebornEquipEvolution rebornEquipEvolutionByGoodsID = IConfigbase<ConfigReborn>.Instance.GetRebornEquipEvolutionByGoodsID(gd.GoodsID);
			if (rebornEquipEvolutionByGoodsID == null)
			{
				return;
			}
			this.m_evolutionInfo = rebornEquipEvolutionByGoodsID;
			GoodsData gd2 = Global.GetDummyGoodsData(rebornEquipEvolutionByGoodsID.NewEquitID);
			gd2 = gd.Clone(rebornEquipEvolutionByGoodsID.NewEquitID);
			this.AddEquipGoodsIcon(gd, this.slEquipNow);
			this.AddEquipGoodsIcon(gd2, this.slEquipNext);
			this.RefershInfos(rebornEquipEvolutionByGoodsID);
			this.lblZhanLi.text = string.Format(Global.GetLang("战斗力 +{0}"), Global.GetGoodsDataZhanLi(gd2) - Global.GetGoodsDataZhanLi(gd));
		}
	}

	public void AddEquipGoodsIcon(GoodsData gd, SpriteSL parent)
	{
		GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIcon(gd, true);
		Super.InitGoodsGIcon(ggoodIcon, gd, false, IconTextTypes.Qianghua);
		parent.Add(ggoodIcon);
	}

	private void RefershInfos(MURebornEquipEvolution evolutionInfo)
	{
		long rebornCuiLianNum = ChongShengData.GetRebornCuiLianNum();
		string text = rebornCuiLianNum + " / " + evolutionInfo.NeedCuiLian;
		if ((long)evolutionInfo.NeedCuiLian > rebornCuiLianNum)
		{
			text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				text
			});
		}
		this.lblNeed1Value.text = Global.GetLang("重生淬炼点: ") + text;
		if (evolutionInfo.NeedCuiLian <= 0)
		{
			this.lblNeed1Value.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			this.lblNeed1Value.transform.parent.gameObject.SetActive(true);
		}
		long rebornDuanZaoNum = ChongShengData.GetRebornDuanZaoNum();
		string text2 = rebornDuanZaoNum + " / " + evolutionInfo.NeedDuanZao;
		if ((long)evolutionInfo.NeedDuanZao > rebornDuanZaoNum)
		{
			text2 = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				text2
			});
		}
		this.lblNeed2Value.text = Global.GetLang("重生锻造点: ") + text2;
		if (evolutionInfo.NeedDuanZao <= 0)
		{
			this.lblNeed2Value.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			this.lblNeed2Value.transform.parent.gameObject.SetActive(true);
		}
		long rebornNiePanNum = ChongShengData.GetRebornNiePanNum();
		string text3 = rebornNiePanNum + " / " + evolutionInfo.NeedNiePan;
		if ((long)evolutionInfo.NeedNiePan > rebornNiePanNum)
		{
			text3 = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				text3
			});
		}
		this.lblNeed3Value.text = Global.GetLang("重生涅槃点: ") + text3;
		if (evolutionInfo.NeedNiePan <= 0)
		{
			this.lblNeed3Value.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			this.lblNeed3Value.transform.parent.gameObject.SetActive(true);
		}
		this.ResetPosition();
		this.lblSuccessRate.text = Global.GetLang("成功率: ") + Mathf.RoundToInt(evolutionInfo.SuccessRate * 100f) + "%";
	}

	private void ResetPosition()
	{
		this.gridNeeds.Reposition();
		int num = 0;
		float num2 = 0f;
		if (this.lblNeed1Value.transform.parent.gameObject.activeSelf)
		{
			num++;
			float x = NGUIMath.CalculateRelativeWidgetBounds(this.lblNeed1Value.transform.parent, this.lblNeed1Value.transform).size.x;
			if (x > num2)
			{
				num2 = x;
			}
		}
		if (this.lblNeed2Value.transform.parent.gameObject.activeSelf)
		{
			num++;
			float x2 = NGUIMath.CalculateRelativeWidgetBounds(this.lblNeed2Value.transform.parent, this.lblNeed2Value.transform).size.x;
			if (x2 > num2)
			{
				num2 = x2;
			}
		}
		if (this.lblNeed3Value.transform.parent.gameObject.activeSelf)
		{
			num++;
			float x3 = NGUIMath.CalculateRelativeWidgetBounds(this.lblNeed3Value.transform.parent, this.lblNeed3Value.transform).size.x;
			if (x3 > num2)
			{
				num2 = x3;
			}
		}
		int num3 = Mathf.RoundToInt((0f - num2) / 2f);
		this.lblNeed1Value.transform.localPosition = new Vector3((float)num3, 0f, 0f);
		this.lblNeed2Value.transform.localPosition = new Vector3((float)num3, 0f, 0f);
		this.lblNeed3Value.transform.localPosition = new Vector3((float)num3, 0f, 0f);
		Vector3 localPosition = this.gridNeeds.transform.localPosition;
		localPosition.y = 0f - (float)(3 - num) * (this.gridNeeds.cellHeight / 2f);
		this.gridNeeds.transform.localPosition = localPosition;
	}

	private void StartZaizaoEquip()
	{
		if (this.slEquipNow.Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请放入重生装备"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.slEquipNow[0]).ItemObject as GoodsData;
		int binding = goodsData.Binding;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备不在重生背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		MURebornEquipEvolution rebornEquipEvolutionByGoodsID = IConfigbase<ConfigReborn>.Instance.GetRebornEquipEvolutionByGoodsID(goodsData.GoodsID);
		if (rebornEquipEvolutionByGoodsID == null)
		{
			return;
		}
		if ((long)rebornEquipEvolutionByGoodsID.NeedCuiLian > ChongShengData.GetRebornCuiLianNum())
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("重生淬炼点数不足"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if ((long)rebornEquipEvolutionByGoodsID.NeedDuanZao > ChongShengData.GetRebornDuanZaoNum())
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("重生锻造点数不足"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if ((long)rebornEquipEvolutionByGoodsID.NeedNiePan > ChongShengData.GetRebornNiePanNum())
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("重生涅槃点数不足"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData2 = U3DUtils.AS<GGoodIcon>(this.slEquipNext[0]).ItemObject as GoodsData;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
		if (!ChongShengData.BeCanEquip(goodsXmlNodeByID))
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			string message = string.Format(Global.GetLang("装备进阶后需要重生{0}阶{1}级才可穿戴，是否确认进阶？"), goodsXmlNodeByID.ToReborn, goodsXmlNodeByID.ToRebornLevel);
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					this.ExecuteJinJieEquip(goodsData.Id);
				}
			}, buttons);
			return;
		}
		this.ExecuteJinJieEquip(goodsData.Id);
	}

	private void ExecuteJinJieEquip(int dbId)
	{
		GameInstance.Game.SpriteRebornJinJie(dbId, 1);
	}

	public void NotifyResult(int result)
	{
		switch (result)
		{
		case 4:
			base.StartCoroutine<bool>(this.ShowEffect(false));
			break;
		case 5:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("涅槃点不足"), new object[0]), 0, -1, -1, 0);
			break;
		case 6:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("锻造点不足"), new object[0]), 0, -1, -1, 0);
			break;
		case 7:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("淬炼点不足"), new object[0]), 0, -1, -1, 0);
			break;
		case 8:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("不存在更高级的装备"), new object[0]), 0, -1, -1, 0);
			break;
		case 13:
			base.StartCoroutine<bool>(this.ShowEffect(true));
			break;
		}
	}

	private IEnumerator ShowEffect(bool beSuccess)
	{
		this.m_beCanDoNext = false;
		this.effect1.SetActive(true);
		yield return new WaitForSeconds(3f);
		this.effect1.SetActive(false);
		if (beSuccess)
		{
			Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
			this.effect2.SetActive(true);
			this.InitAllValue(false);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 1
			});
		}
		else
		{
			Global.PlaySoundAudio("Audio/UI/hecheng_failed", false);
			this.RefershInfos(this.m_evolutionInfo);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = -1
			});
		}
		this.m_beCanDoNext = true;
		yield break;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	public UILabel lblZhanLi;

	public UILabel lblNeed1Value;

	public UILabel lblNeed2Value;

	public UILabel lblNeed3Value;

	public UIGrid gridNeeds;

	public UILabel lblSuccessRate;

	public GButton btnSubmit;

	public SpriteSL slEquipNow;

	public SpriteSL slEquipNext;

	public GameObject effect1;

	public GameObject effect2;

	private bool m_beCanDoNext = true;

	private MURebornEquipEvolution m_evolutionInfo;
}
