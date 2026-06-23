using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class SoulCometStoneGathering : UserControl
{
	public static void ClearXMLData()
	{
		if (SoulCometStoneGathering.dic_soulCometStone != null && 0 < SoulCometStoneGathering.dic_soulCometStone.Count)
		{
			SoulCometStoneGathering.dic_soulCometStone.Clear();
		}
	}

	private void InitTextInPrefabs()
	{
		this.BtnSubmits[0].Text = Global.GetLang("聚魂1次");
		this.BtnSubmits[1].Text = Global.GetLang("聚魂10次");
		for (int i = 1; i <= 4; i++)
		{
			this.txtDang[i - 1].text = Global.GetLang(string.Format(Global.GetLang("{0}档"), i));
		}
		this.checkBoxPlayAnim._Lable.lineWidth = 95;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.GetSoulCometStonesConfig();
		this.InitProgress();
		this.BtnSubmits[0].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int bagCount = (this.extraFuncType != 1 || !this.checkedExtraFunc) ? 1 : 2;
			if (this.IsEnabledGathering(this.currentStoneGroupID, bagCount))
			{
				this.StartGathering(1);
			}
		};
		this.BtnSubmits[1].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int bagCount = (this.extraFuncType != 1 || !this.checkedExtraFunc) ? 10 : 11;
			if (this.IsEnabledGathering(this.currentStoneGroupID, bagCount))
			{
				this.StartGathering(10);
			}
		};
		if (null != this.checkBox)
		{
			this.checkBox.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.SetCheckExtraFunctionState();
			};
			this.checkBox.gameObject.SetActive(false);
		}
		this.checkBoxPlayAnim.Check = false;
		this.checkBoxPlayAnim._Lable.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("不播放动画")
		});
		this.checkBoxPlayAnim._Lable.transform.localScale = Vector3.one * 18f;
	}

	private new void OnDestroy()
	{
		Global.isSoulCometStoneGathering = false;
	}

	private void OnEnable()
	{
		this.gatheringEnable = true;
		Super.ActiveGameObject(this.Get10Anim, false);
		this.GetSoulCometStoneGroupInfo();
	}

	private void OnDisable()
	{
		this.RemoveIcons();
	}

	public bool gatheringEnable
	{
		get
		{
			return this._gatheringEnable;
		}
		set
		{
			this._gatheringEnable = value;
			Global.isSoulCometStoneGathering = !this._gatheringEnable;
		}
	}

	private void InitProgress()
	{
		this.currentStoneGroupID = 10;
		this.SetProgressResult(this.currentStoneGroupID, -1, true);
	}

	private void SetProgress(int level)
	{
		if (this.NetImages == null)
		{
			return;
		}
		int num = level / 10;
		int num2 = level % 10;
		if (num < 1 || num > this.NetImages.Length)
		{
			return;
		}
		this.SetProgressResult(num, num2, false);
		this.currentStoneGroupID = num * 10 + num2;
	}

	private void SetProgressResult(int level, int stoneType, bool isInit = false)
	{
		int num = level - 1;
		if (!isInit)
		{
			for (int i = 0; i < this.NetImages.Length; i++)
			{
				SoulCometStoneGatheringAttribute soulCometStoneGatheringDataByID = this.GetSoulCometStoneGatheringDataByID(10 * (i + 1) + stoneType);
				int iconID = (soulCometStoneGatheringDataByID == null) ? -1 : soulCometStoneGatheringDataByID.iconCode;
				this.NetImages[i].URL = Global.GetGoodsIconString(iconID);
				this.TextMoneys[i].Text = ((soulCometStoneGatheringDataByID == null) ? string.Empty : soulCometStoneGatheringDataByID.needSoulCometPowder.ToString());
				this.NetImages[i].ToGrayBitmap = (num < i);
			}
		}
		else
		{
			for (int j = 0; j < this.NetImages.Length; j++)
			{
				this.NetImages[j].ToGrayBitmap = (num != j);
			}
		}
	}

	private void RefreshAddGoodIcons(List<int> list_goods, List<int> list_extraGoods)
	{
		this.RemoveIcons();
		this.IsStopped = false;
		if (list_goods == null || list_goods.Count <= 0)
		{
			return;
		}
		List<GoodsData> list = new List<GoodsData>();
		for (int i = 0; i < list_goods.Count; i++)
		{
			int goodsID = list_goods[i];
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(goodsID, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			Global.SetGoodsDataYuansuProps(dummyGoodsDataMu, 1, 0);
			list.Add(dummyGoodsDataMu);
		}
		base.StartCoroutine<bool>(this.AddGoodListIcon(list, list_extraGoods));
	}

	private IEnumerator AddGoodListIcon(List<GoodsData> goodsList, List<int> list_extraGoods)
	{
		this.IsStopped = false;
		int goodsCount = goodsList.Count;
		if (goodsCount == 1)
		{
			this.AddGoodIcon(goodsList[0], new Vector3(0f, 0f, 0f));
		}
		else
		{
			int beginX = -180;
			float interval = 90f;
			float realY = 30f;
			float realX = 0f;
			for (int i = 0; i < goodsCount; i++)
			{
				if (i >= 5)
				{
					realY = -50f;
					realX = (float)beginX + interval * (float)(i - 5);
				}
				else
				{
					realX = (float)beginX + interval * (float)i;
				}
				if (this.IsStopped)
				{
					yield break;
				}
				this.AddGoodIcon(goodsList[i], new Vector3(realX, realY, 0f));
				yield return null;
			}
		}
		yield return new WaitForSeconds(1.5f);
		if (list_extraGoods != null && list_extraGoods.Count > 0)
		{
			for (int j = 1; j <= list_extraGoods.Count; j++)
			{
				this.OnTweenComplete2();
			}
			yield return new WaitForSeconds(0.3f);
		}
		this.gatheringEnable = true;
		yield break;
	}

	private void AddGoodIcon(GoodsData gd, Vector3 localPos)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		GameObject flashPrefab = this.GetFlashPrefab();
		flashPrefab.gameObject.transform.localPosition = localPos;
		U3DUtils.AddChild(this.IconContainer, flashPrefab, true);
		this.AnimList.Add(flashPrefab);
		GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
		icon.Width = 78.0;
		icon.Height = 78.0;
		icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		icon.TipType = 1;
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.BoxTypes = 0;
		icon.TextSize = 16;
		icon.TextShadowColor = 4278190080U;
		icon.Tag = gd.ExcellenceInfo;
		icon.BackSpriteName0 = "bagGrid4_bak";
		icon.TextColor = 15793920U;
		int num = 0;
		int soulCometStoneLevel = Global.GetSoulCometStoneLevel(gd, out num);
		icon.ContentText.Text = "Lv" + soulCometStoneLevel;
		int equipGoodsSuitID = Global.GetEquipGoodsSuitID(gd.GoodsID);
		if (equipGoodsSuitID == 1)
		{
			icon.BackSpriteName1 = "iconState_zuoyue";
		}
		if (equipGoodsSuitID == 2)
		{
			icon.BackSpriteName1 = "iconState_zuoyue1";
		}
		if (equipGoodsSuitID == 3)
		{
			icon.BackSpriteName1 = "iconState_zuoyue2";
		}
		if (equipGoodsSuitID >= 4 && equipGoodsSuitID <= 10)
		{
			icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
			icon.TeXiao.gameObject.SetActive(true);
		}
		Super.InitGoodsGIcon(icon, gd, true, IconTextTypes.Qianghua);
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GTipServiceEx.ShowTip(icon, TipTypes.SoulCometStoneBagTip, GoodsOwnerTypes.SoulCometStoneBag, gd);
		};
		U3DUtils.AddChild(this.IconContainer, icon.gameObject, true);
		icon.transform.localPosition = localPos;
		icon.transform.localScale = new Vector3(0f, 0f, 0f);
		iTween.ScaleTo(icon.gameObject, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(1f, 1f, 1f),
			"time",
			0.3f,
			"easeType",
			18,
			"oncomplete",
			"OnTweenComplete",
			"oncompletetarget",
			base.gameObject
		}));
		this.IconList.Add(icon);
	}

	private bool IsEnabledGathering(int index, int bagCount)
	{
		if (!this.gatheringEnable)
		{
			Super.HintMainText(Global.GetLang("聚魂尚未结束！"), 10, 3);
			return false;
		}
		if (Global.GetSoulCometStoneBagEmptyGrid() < bagCount)
		{
			Super.HintMainText(string.Format(Global.GetLang("至少需要{0}个格子"), bagCount), 10, 3);
			return false;
		}
		SoulCometStoneGatheringAttribute soulCometStoneGatheringAttribute = null;
		if (SoulCometStoneGathering.dic_soulCometStone.TryGetValue(index, ref soulCometStoneGatheringAttribute))
		{
			int needSoulCometPowder = soulCometStoneGatheringAttribute.needSoulCometPowder;
			int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.LangHunFenMo);
			if (roleCommonUseParamsValue < needSoulCometPowder)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedLangHunFenMo, this.callback, string.Empty, string.Empty);
				this.gatheringEnable = true;
				return false;
			}
		}
		if (this.checkedExtraFunc)
		{
			int num = 0;
			bool result = true;
			switch (this.extraCostType)
			{
			case 1:
				num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan);
				break;
			case 2:
				num = Global.Data.roleData.StarSoulValue;
				break;
			case 3:
				num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ChengJiu);
				break;
			case 4:
				num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWang);
				break;
			case 5:
				num = Global.Data.roleData.UserMoney;
				break;
			}
			if (num < this.currencyValue)
			{
				result = false;
				this.ShowExtraCostNotEnough();
			}
			return result;
		}
		return true;
	}

	public void OnTweenComplete()
	{
		base.StartCoroutine<bool>(this.StartTweenComplete());
	}

	public void OnTweenComplete2()
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1
			});
		}
	}

	public IEnumerator StartTweenComplete()
	{
		yield return new WaitForSeconds(0.5f);
		int count = this.IconList.Count;
		if (count > 0)
		{
			GGoodIcon icon = null;
			float disposeTime = 0.5f;
			Vector3 disposePos = new Vector3(463f, 32f, 0f);
			for (int i = 0; i < count; i++)
			{
				icon = this.IconList[i];
				iTween.MoveTo(icon.gameObject, iTween.Hash(new object[]
				{
					"position",
					disposePos,
					"time",
					disposeTime,
					"islocal",
					true,
					"oncomplete",
					"OnTweenComplete2",
					"oncompletetarget",
					base.gameObject
				}));
				iTween.ScaleTo(icon.gameObject, Vector3.zero, disposeTime);
			}
		}
		yield break;
	}

	private void RemoveIcons()
	{
		this.IsStopped = true;
		base.StopCoroutine("AddGoodListIcon");
		iTween.Stop();
		int count = this.IconList.Count;
		for (int i = count - 1; i >= 0; i--)
		{
			NGUITools.Destroy(this.IconList[i].gameObject);
			NGUITools.Destroy(this.AnimList[i].gameObject);
		}
		this.IconList.Clear();
		this.AnimList.Clear();
	}

	public GameObject GetFlashPrefab()
	{
		if (this.FlashPrefab == null)
		{
			this.FlashPrefab = (Resources.Load(string.Format("UITeXiao/UI_yuansuzhixin/Yuansu_tilian_shan/yuansu_tilian_shan", new object[0])) as GameObject);
		}
		return SpawnManager.Instantiate(this.FlashPrefab) as GameObject;
	}

	public GameObject GetFlyPrefab()
	{
		if (this.FlyPrefab == null)
		{
			this.FlyPrefab = (Resources.Load(string.Format("UITeXiao/UI_yuansuzhixin/Yuansu_tilianwei/yuansu_tilianwei", new object[0])) as GameObject);
		}
		return SpawnManager.Instantiate(this.FlyPrefab) as GameObject;
	}

	private void SetExtraFunction(int randID, int funcType, int costType)
	{
		string text = string.Empty;
		if (funcType == 0)
		{
			text = Global.GetLang("聚魂星数越多，出现高级魂石的几率更高");
		}
		else
		{
			this.currencyValue = this.GetSoulCometStoneGatheringExtraCurrency(randID, funcType, costType);
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				this.currencyValue
			});
			text = string.Format("{0}{1}  {2}", Global.GetLang("每次额外消耗") + this.fillBlanks, colorStringForNGUIText, this.ExtraFunctionDescription(funcType));
		}
		if (null != this.extraFuncText)
		{
			this.extraFuncText.Text = text;
		}
		this.SetCurrentIcon(costType);
		this.SetExtraFunctionCheckBox(funcType != 0);
	}

	private string ExtraFunctionDescription(ESoulStoneExtFuncType functype)
	{
		string result = string.Empty;
		switch (functype)
		{
		case 1:
			result = Global.GetLang("有几率获得魂石精华");
			break;
		case 2:
			result = Global.GetLang("有几率不耗狼魂粉末");
			break;
		case 3:
			result = Global.GetLang("提高升华几率");
			break;
		case 4:
			result = Global.GetLang("锁定元素种类");
			break;
		}
		return result;
	}

	private void SetCurrentIcon(ESoulStoneExtCostType iconType = 0)
	{
		if (null == this.currencyIcon)
		{
			return;
		}
		string spriteName = string.Empty;
		switch (iconType)
		{
		case 1:
			spriteName = "mojing";
			break;
		case 2:
			spriteName = "xinghun";
			break;
		case 3:
			spriteName = "chengjiu";
			break;
		case 4:
			spriteName = "shengwang";
			break;
		case 5:
			spriteName = "diamond";
			break;
		}
		this.currencyIcon.spriteName = spriteName;
		this.currencyIcon.gameObject.SetActive(iconType != 0);
	}

	private void SetExtraFunctionCheckBox(bool visible = false)
	{
		if (null != this.checkBox)
		{
			this.checkBox.gameObject.SetActive(visible);
		}
	}

	private void SetCheckExtraFunctionState()
	{
		if (null == this.checkBox)
		{
			return;
		}
		this.checkedExtraFunc = this.checkBox.isChecked;
	}

	private void StartGathering(int count)
	{
		this.gatheringEnable = false;
		base.StartCoroutine<bool>(this.ExeGathering(count));
	}

	private IEnumerator ExeGathering(int count)
	{
		this.gatheringCount = count;
		if (!this.checkBoxPlayAnim.Check && count == 10)
		{
			Super.PlayAnim(this.Get10Anim);
			yield return new WaitForSeconds(1f);
		}
		this.ShowModalDialog();
		TCPGame game = GameInstance.Game;
		string extraFunc;
		if (this.checkedExtraFunc)
		{
			int num = this.extraFuncType;
			extraFunc = num.ToString();
		}
		else
		{
			extraFunc = string.Empty;
		}
		game.GetSoulCometStone(count, extraFunc);
		yield break;
	}

	public void SetGatheringResult(SoulStoneGetData soulStoneData)
	{
		this.CloseModalDialog();
		if (soulStoneData == null)
		{
			this.gatheringEnable = true;
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("聚魂时发生错误"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int error = soulStoneData.Error;
		if (error == 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("聚魂成功"), new object[0]), 0, -1, -1, 0);
			this.SetProgress(soulStoneData.NewRandId);
			if (this.gatheringCount == 10)
			{
				if (!this.checkBoxPlayAnim.Check)
				{
					this.RefreshAddGoodIcons(soulStoneData.Stones, soulStoneData.ExtGoods);
				}
				else
				{
					this.gatheringEnable = true;
					if (soulStoneData.Stones != null && 0 < soulStoneData.Stones.Count)
					{
						for (int i = 0; i < soulStoneData.Stones.Count; i++)
						{
							GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(soulStoneData.Stones[i], 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
							Global.SetGoodsDataYuansuProps(dummyGoodsDataMu, 1, 0);
							if (dummyGoodsDataMu != null)
							{
								Super.HintMainText(Global.GetGoodsNameByID(soulStoneData.Stones[i], true) + " * 1", 10, 3);
							}
							this.OnTweenComplete2();
						}
					}
					if (soulStoneData.ExtGoods != null && 0 < soulStoneData.ExtGoods.Count)
					{
						for (int j = 0; j < soulStoneData.ExtGoods.Count; j++)
						{
							GoodsData dummyGoodsDataMu2 = Global.GetDummyGoodsDataMu(soulStoneData.ExtGoods[j], 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
							Global.SetGoodsDataYuansuProps(dummyGoodsDataMu2, 1, 0);
							if (dummyGoodsDataMu2 != null)
							{
								Super.HintMainText(Global.GetGoodsNameByID(soulStoneData.ExtGoods[j], true) + " * 1", 10, 3);
							}
							this.OnTweenComplete2();
						}
					}
				}
			}
			else
			{
				this.RefreshAddGoodIcons(soulStoneData.Stones, soulStoneData.ExtGoods);
			}
			if (soulStoneData.RealDoTimes < soulStoneData.RequestTimes)
			{
				if (this.extraFuncType == null)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedLangHunFenMo, this.callback, string.Empty, string.Empty);
				}
				else
				{
					Super.HintMainText(Global.GetLang("材料不足或格子不足"), 10, 3);
				}
			}
		}
		else if (error == 5)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedLangHunFenMo, this.callback, string.Empty, string.Empty);
			this.SetProgress(soulStoneData.NewRandId);
		}
		else if (error == 6)
		{
			this.SetProgress(soulStoneData.NewRandId);
			this.ShowExtraCostNotEnough();
		}
		if (error != 0)
		{
			this.gatheringEnable = true;
		}
	}

	private void ShowExtraCostNotEnough()
	{
		string msg = string.Empty;
		switch (this.extraCostType)
		{
		case 1:
			msg = Global.GetLang("魔晶不足");
			break;
		case 2:
			msg = Global.GetLang("星魂不足");
			break;
		case 3:
			msg = Global.GetLang("成就不足");
			break;
		case 4:
			msg = Global.GetLang("声望不足");
			break;
		case 5:
			msg = Global.GetLang("钻石不足");
			break;
		}
		Super.HintMainText(msg, 10, 3);
	}

	public void GetSoulCometStoneGroupInfo()
	{
		this.ShowModalDialog();
		GameInstance.Game.GetSoulCometStoneGroupInfo();
	}

	public void SetSoulCometStoneGroupInfo(SoulStoneQueryGetData randStoneGroupInfo)
	{
		this.CloseModalDialog();
		if (randStoneGroupInfo == null)
		{
			return;
		}
		int num = Math.Max(randStoneGroupInfo.CurrRandId, 1);
		this.SetProgress(num);
		List<SoulStoneExtFuncItem> extFuncList = randStoneGroupInfo.ExtFuncList;
		SoulStoneExtFuncItem soulStoneExtFuncItem = (extFuncList != null && extFuncList.Count > 0) ? extFuncList[0] : null;
		int funcType = (soulStoneExtFuncItem == null) ? 0 : soulStoneExtFuncItem.FuncType;
		int costType = (soulStoneExtFuncItem == null) ? 0 : soulStoneExtFuncItem.CostType;
		this.extraFuncType = funcType;
		this.extraCostType = costType;
		this.SetExtraFunction(num, funcType, costType);
	}

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	private void GetSoulCometStonesConfig()
	{
		if (SoulCometStoneGathering.dic_soulCometStone != null && SoulCometStoneGathering.dic_soulCometStone.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/HunShiType.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HunShiType");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return;
		}
		SoulCometStoneGathering.dic_soulCometStone = new Dictionary<int, SoulCometStoneGatheringAttribute>(xelementList.Count);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			SoulCometStoneGatheringAttribute soulCometStoneGatheringAttribute = new SoulCometStoneGatheringAttribute();
			soulCometStoneGatheringAttribute.id = Global.GetXElementAttributeInt(xelement, "ID");
			soulCometStoneGatheringAttribute.groupLevel = soulCometStoneGatheringAttribute.id / 10;
			soulCometStoneGatheringAttribute.type = soulCometStoneGatheringAttribute.id % 10;
			soulCometStoneGatheringAttribute.iconCode = Global.GetXElementAttributeInt(xelement, "Mod");
			soulCometStoneGatheringAttribute.needSoulCometPowder = Global.GetXElementAttributeInt(xelement, "NeedLangHunFenMo");
			string[] array = new string[]
			{
				"AddedGoodsNeed",
				"ReduceNeed",
				"AdvanceSuccessNeed",
				"HoldTypeNeed"
			};
			for (int j = 0; j < array.Length; j++)
			{
				soulCometStoneGatheringAttribute.list_extraGoodsNeed[j] = Global.GetXElementAttributeStr(xelement, array[j]);
			}
			if (!SoulCometStoneGathering.dic_soulCometStone.ContainsKey(soulCometStoneGatheringAttribute.id))
			{
				SoulCometStoneGathering.dic_soulCometStone.Add(soulCometStoneGatheringAttribute.id, soulCometStoneGatheringAttribute);
			}
		}
	}

	private SoulCometStoneGatheringAttribute GetSoulCometStoneGatheringDataByID(int id)
	{
		if (SoulCometStoneGathering.dic_soulCometStone == null || SoulCometStoneGathering.dic_soulCometStone.Count <= 0)
		{
			return null;
		}
		SoulCometStoneGatheringAttribute result = null;
		SoulCometStoneGathering.dic_soulCometStone.TryGetValue(id, ref result);
		return result;
	}

	private int GetSoulCometStoneGatheringExtraCurrency(int stoneType, int extraFuncID, int costType)
	{
		if (extraFuncID < 1 || extraFuncID > 4)
		{
			return 0;
		}
		if (costType < 1 || extraFuncID > 5)
		{
			return 0;
		}
		SoulCometStoneGatheringAttribute soulCometStoneGatheringDataByID = this.GetSoulCometStoneGatheringDataByID(stoneType);
		if (soulCometStoneGatheringDataByID == null || soulCometStoneGatheringDataByID.list_extraGoodsNeed == null || soulCometStoneGatheringDataByID.list_extraGoodsNeed.Length <= 0)
		{
			return 0;
		}
		string text = soulCometStoneGatheringDataByID.list_extraGoodsNeed[extraFuncID - 1];
		if (string.IsNullOrEmpty(text))
		{
			return 0;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array == null || array.Length <= 0 || array.Length < costType)
		{
			return 0;
		}
		return Convert.ToInt32(array[costType - 1]);
	}

	private const int gatheringCount_one = 1;

	private const int gatheringCount_ten = 10;

	private const string fontColor = "e3b36c";

	private const string fontColor_white = "f0f0f0";

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler callback;

	public GameObject Get10Anim;

	public ShowNetImage[] NetImages;

	public TextBlock[] TextMoneys;

	public GButton[] BtnSubmits;

	public GameObject IconContainer;

	public TextBlock extraFuncText;

	public UISprite currencyIcon;

	public GCheckBox checkBox;

	private bool checkedExtraFunc;

	public GCheckBox checkBoxPlayAnim;

	private int currentStoneGroupID = 10;

	private List<GGoodIcon> IconList = new List<GGoodIcon>();

	private List<GameObject> AnimList = new List<GameObject>();

	private bool IsStopped;

	public TextBlock[] txtDang;

	private static Dictionary<int, SoulCometStoneGatheringAttribute> dic_soulCometStone;

	private string fillBlanks = "        ";

	private bool _gatheringEnable = true;

	private GameObject FlashPrefab;

	private GameObject FlyPrefab;

	private ESoulStoneExtFuncType extraFuncType;

	private ESoulStoneExtCostType extraCostType;

	private int currencyValue;

	private int gatheringCount;
}
