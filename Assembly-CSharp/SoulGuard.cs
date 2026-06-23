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

public class SoulGuard : UserControl
{
	private void InitTextInPrefabs()
	{
		if (Context.IsHaiwai && this.staticText != null)
		{
			this.staticText.text = Global.GetLang("已达到顶阶");
		}
		this.soulGuardInfoTxt.Text = Global.GetLang("守护之灵");
		this.propertyInfoTxt.Text = Global.GetLang("详细属性");
		this.upLevelBtn.Text = Global.GetLang("升级");
		this.needGuardPointInfoTxt.Text = Global.GetLang("消耗守护点数：");
		this.needsInfoTxt.Text = Global.GetLang("升阶消耗");
		NGUITools.SetActive(this.upLevelGoodsIcon.BackgroundSprite0, true);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.maxGrade = (int)ConfigSystemParam.GetSystemParamIntByName("ShouHuShenMax");
		this.starProgressBar.ItemWidth = 28f;
		this.starProgressBar.MaxLevel = 10;
		this.upLevelBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.levelupAniamtionEnable = true;
			if (this.isUpLevel)
			{
				this.UpLevelStatue();
			}
			else
			{
				this.UpGradeStatue();
			}
		};
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		if (null != this.upLevelGoodsIcon)
		{
			this.upLevelGoodsIcon.addEventListener("click", new MouseEventHandler(SoulGuard.MouseLeftButtonUp));
		}
		if (this.specialUpLevelGoodsIcon != null)
		{
			this.specialUpLevelGoodsIcon.addEventListener("click", new MouseEventHandler(SoulGuard.MouseLeftButtonUp));
		}
	}

	public void InitSoulGuard()
	{
		this.GetSlotConfig();
		this.dic_soulGuardAttr = this.GetSoulGuardConfig();
		int num = 6;
		if (this.dic_slot != null)
		{
			num = this.dic_slot.Keys.Count;
		}
		this.goodsBox.RowCount = 3;
		this.goodsBox.ColCount = 4;
		this.goodsBox.InitBox();
		List<GoodsData> list = new List<GoodsData>(num);
		for (int i = 0; i < num; i++)
		{
			GoodsData dummyGoodsDataEx = Global.GetDummyGoodsDataEx(-4, 0, 0, 0, 1, 0);
			list.Add(dummyGoodsDataEx);
		}
		this.InitSoulGuardList(list);
		this.GetGuardStatueInfo();
	}

	protected override void OnDestroy()
	{
	}

	private void InitSoulGuardList(List<GoodsData> goodsDataList)
	{
		if (goodsDataList == null || goodsDataList.Count <= 0)
		{
			return;
		}
		if (this.dic_slot == null)
		{
			return;
		}
		int count = this.dic_slot.Keys.Count;
		for (int i = 0; i < count; i++)
		{
			GoodsData goodsData = goodsDataList[i];
			goodsData.Id = this.serialID++;
			GGoodIcon ggoodIcon = this.AddIcon(goodsData, null);
			this.goodsBox.SetGoodsIcon(this.Getindex(i), ggoodIcon);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
		}
	}

	private void RefreshSoulGuardList(List<GuardSoulData> list_soulData)
	{
		if (list_soulData == null || list_soulData.Count <= 0)
		{
			return;
		}
		int num = this.ActivitedSlot(list_soulData.Count);
		for (int i = 0; i < num; i++)
		{
			int typeID = -1;
			GuardSoulData soulDataBySlotID = this.GetSoulDataBySlotID(list_soulData, i);
			if (soulDataBySlotID != null)
			{
				typeID = soulDataBySlotID.type;
			}
			GoodsData activitedSoulGuardItemGoodsData = this.GetActivitedSoulGuardItemGoodsData(typeID);
			activitedSoulGuardItemGoodsData.Id = this.serialID++;
			GGoodIcon ggoodIcon = this.AddIcon(activitedSoulGuardItemGoodsData, null);
			this.goodsBox.SetGoodsIcon(this.Getindex(i), ggoodIcon);
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
		}
	}

	private GoodsData GetActivitedSoulGuardItemGoodsData(int typeID)
	{
		GoodsData result = null;
		if (typeID == -1)
		{
			result = Global.GetDummyGoodsDataEx(-5, 0, 0, 0, 1, 0);
		}
		else if (this.dic_soulGuardAttr != null && this.dic_soulGuardAttr.Keys.Count >= 0)
		{
			SoulGuardAttribute soulGuardAttribute = this.dic_soulGuardAttr[typeID];
			result = Global.GetDummyGoodsData(soulGuardAttribute.goodsID);
		}
		return result;
	}

	private GuardSoulData GetSoulDataBySlotID(List<GuardSoulData> list_soul, int slotID)
	{
		return list_soul.Find((GuardSoulData soul) => soul.equipSlot == slotID);
	}

	private int ActivitedSlot(int activitedMap)
	{
		if (this.dic_slot == null || this.dic_slot.Keys.Count <= 0)
		{
			return 0;
		}
		int num = 0;
		foreach (int num2 in this.dic_slot.Keys)
		{
			if (activitedMap >= this.dic_slot[num2] && num <= num2)
			{
				num = num2;
			}
		}
		return num;
	}

	private List<GoodsData> GetAvailableSoulGoodsDataList()
	{
		if (this.guardStatue == null || this.guardStatue.soulGuardList == null || this.guardStatue.soulGuardList.Count <= 0)
		{
			return null;
		}
		List<GoodsData> list = new List<GoodsData>();
		for (int i = 0; i < this.guardStatue.soulGuardList.Count; i++)
		{
			GuardSoulData guardSoulData = this.guardStatue.soulGuardList[i];
			if (guardSoulData.equipSlot == -1)
			{
				GoodsData activitedSoulGuardItemGoodsData = this.GetActivitedSoulGuardItemGoodsData(guardSoulData.type);
				if (activitedSoulGuardItemGoodsData != null)
				{
					list.Add(activitedSoulGuardItemGoodsData);
				}
			}
		}
		return list;
	}

	private List<int> GetEquipedSoulGoodsIDList()
	{
		if (this.guardStatue == null || this.guardStatue.soulGuardList == null || this.guardStatue.soulGuardList.Count <= 0)
		{
			return null;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < this.guardStatue.soulGuardList.Count; i++)
		{
			GuardSoulData guardSoulData = this.guardStatue.soulGuardList[i];
			if (guardSoulData.equipSlot != -1)
			{
				GoodsData activitedSoulGuardItemGoodsData = this.GetActivitedSoulGuardItemGoodsData(guardSoulData.type);
				if (activitedSoulGuardItemGoodsData != null)
				{
					list.Add(activitedSoulGuardItemGoodsData.GoodsID);
				}
			}
		}
		return list;
	}

	private int GetSoulGuardTypeIDByGoodsID(int goodsID)
	{
		if (this.dic_soulType == null || this.dic_soulType.Keys.Count <= 0)
		{
			this.dic_soulType = this.GetSoulGuardTypes();
		}
		int result = -1;
		this.dic_soulType.TryGetValue(goodsID, ref result);
		return result;
	}

	private GGoodIcon AddIcon(GoodsData goodsData, MouseLeftButtonUpEventHandler handler = null)
	{
		GGoodIcon ggoodIcon = SoulGuard.AddGoodsIcon(this, goodsData, handler, false);
		if (null != ggoodIcon)
		{
			ggoodIcon.Width = 88.0;
			ggoodIcon.Height = 88.0;
			ggoodIcon.isAutoSize = false;
			ggoodIcon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (ev.IDType == 18)
				{
					this.EquipSoulGuradItem();
				}
				else if (ev.IDType == 2)
				{
					this.UnloadSoulGuradItem();
				}
			};
		}
		return ggoodIcon;
	}

	public static GGoodIcon AddGoodsIcon(SpriteSL parent, GoodsData goodsData, MouseLeftButtonUpEventHandler handler = null, bool disabled = false)
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		parent.Add(ggoodIcon);
		ggoodIcon.Width = 88.0;
		ggoodIcon.Height = 88.0;
		ggoodIcon.OutSizeX = 78;
		ggoodIcon.OutSizeY = 78;
		ggoodIcon.isAutoSize = true;
		ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
		if (goodsData != null)
		{
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsData.GoodsID,
				0,
				goodsData.Id,
				15
			});
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.GoodsID = goodsData.GoodsID;
			ggoodIcon.TipType = 1;
			ggoodIcon.GoodsCount = goodsData.GCount;
			ggoodIcon.Binding = goodsData.Binding;
			ggoodIcon.Lucky = goodsData.Lucky;
			ggoodIcon.ForgeLevel = goodsData.Forge_level;
			ggoodIcon.ZhuijiaLevel = goodsData.AppendPropLev;
			ggoodIcon.ExcellenceInfo = goodsData.ExcellenceInfo;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				string goodsIconString = Global.GetGoodsIconString(int.Parse(goodsXmlNodeByID.IconCode));
				ggoodIcon.BodyURL = new ImageURL(goodsIconString, false, 0);
				ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
				Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(Global.GetGoodsIconString(goodsData.GoodsID), false, 0);
			}
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		}
		UIButtonOffset component = ggoodIcon.GetComponent<UIButtonOffset>();
		if (null != component)
		{
			component.enabled = false;
		}
		if (disabled)
		{
			U3DUtils.EnableCollider(ggoodIcon.gameObject, false);
		}
		return ggoodIcon;
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		this.SetIconHighlight(ggoodIcon);
		this.lastIcon = ggoodIcon;
		this.SelectIcon(ggoodIcon);
	}

	public void Showstatue3DModelByID(int modelID, float scale = 1f)
	{
		if (null != this.statue3DModel)
		{
			Object.Destroy(this.statue3DModel.gameObject);
			this.statue3DModel = null;
		}
		this.statue3DModel = U3DUtils.NEW<Modal3DShow>();
		U3DUtils.AddChild(this.statueModel, this.statue3DModel.gameObject, false);
		Transform transform = this.statue3DModel.transform;
		transform.localPosition = new Vector3(0f, 0f, -0.8f);
		transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		UIHelper.SetModalPosZ(this.statue3DModel.transform);
		DPSelectedItemEventHandler handler = delegate(object sender, DPSelectedItemEventArgs args)
		{
			if (sender != null)
			{
				GameObject gameObject = sender as GameObject;
				Animation componentInChildren = gameObject.GetComponentInChildren<Animation>();
				if (null != componentInChildren)
				{
					base.StartCoroutine<bool>(this.PlayAnimation(componentInChildren));
				}
			}
		};
		if (this.resourceLoader != null)
		{
			this.resourceLoader.Stop();
		}
		this.resourceLoader = UIHelper.LoadModelResource(this.statue3DModel, modelID, scale, handler);
	}

	public override void Destroy()
	{
		if (this.resourceLoader != null)
		{
			this.resourceLoader.Stop();
			this.resourceLoader = null;
		}
		base.Destroy();
	}

	private IEnumerator PlayAnimation(Animation anim)
	{
		if (null == anim)
		{
			yield break;
		}
		AnimationState animationClip = null;
		if (anim.IsPlaying("Stand"))
		{
			anim.Play("Relax");
			animationClip = anim["Relax"];
		}
		else
		{
			anim.Play("Stand");
			animationClip = anim["Stand"];
		}
		yield return new WaitForSeconds(animationClip.length);
		base.StartCoroutine<bool>(this.PlayAnimation(anim));
		yield break;
	}

	private void EquipSoulGuradItem()
	{
		List<GoodsData> availableSoulGoodsDataList = this.GetAvailableSoulGoodsDataList();
		PlayZone.GlobalPlayZone.ShowAvailableSoulGuardWindow(availableSoulGoodsDataList);
	}

	private void UnloadSoulGuradItem()
	{
		if (null != this.goodsBox)
		{
			this.currentSlot = this.goodsBox.listBox.SelectedIndex;
		}
		this.EquipSoulGuard(this.currentSlot, -1);
	}

	public void EquipSoulGuardByGoodsID(int goodsID)
	{
		if (null != this.goodsBox)
		{
			this.currentSlot = this.goodsBox.listBox.SelectedIndex;
		}
		int soulGuardTypeIDByGoodsID = this.GetSoulGuardTypeIDByGoodsID(goodsID);
		this.EquipSoulGuard(this.currentSlot, soulGuardTypeIDByGoodsID);
	}

	private int GetGoodsIconIndex(GoodsData goodsData)
	{
		return this.Getindex(goodsData.BagIndex);
	}

	private int Getindex(int bagIndex)
	{
		int result = -1;
		if (this.bagOrient == BagOrientTypes.Vertical && !this.isPage)
		{
			result = bagIndex;
		}
		else if (this.bagOrient == BagOrientTypes.Horizontal && this.isPage)
		{
			int num = 2;
			int num2 = 3;
			int num3 = 0;
			this.goodsBox.listBox.maxPerLine = num3;
			int num4 = bagIndex / num / num2;
			int num5 = bagIndex % (num * num2);
			int num6 = num5 % num;
			int num7 = num5 / num % num2;
			result = num6 + num7 * num3 + num4 * num;
		}
		return result;
	}

	private void SetIconHighlight(GGoodIcon icon)
	{
		if (null != this.lastIcon)
		{
			this.lastIcon.BackSpriteName1 = "none";
		}
		if (null != icon)
		{
			icon.BackSpriteName1 = "iconState_highlight";
		}
	}

	private void SelectIcon(GGoodIcon icon)
	{
		if (null == icon)
		{
			return;
		}
		GoodsData goodsData = icon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		int goodsID = goodsData.GoodsID;
		if (goodsID == -4)
		{
			int slotID = this.goodsBox.FindByGoodsDbID(goodsData.Id);
			this.ShowUnactiveHintMsg(slotID);
			base.Invoke("HideUnactiveHintMsg", 0.5f);
		}
		else if (goodsID == -5)
		{
			this.EquipSoulGuradItem();
		}
		else
		{
			GTipServiceEx.ShowTip(icon, TipTypes.SoulGuardTip, GoodsOwnerTypes.None, goodsData);
		}
	}

	private void SetStarProgress(int level, bool animEnable = true)
	{
		if (level > 0 && animEnable)
		{
			this.starAnim.gameObject.SetActive(true);
		}
		this.starAnim.transform.localPosition = new Vector3((float)(14 + this.starProgressBar.Level * 28), 1f, -1f);
		this.PlayStarAnimation(this.starAnim, new ActiveAnimation.OnFinished(this.OnStarAnimationPlayFinished));
		this.starProgressBar.Level = level;
	}

	private void PlayStarAnimation(Animation anim, ActiveAnimation.OnFinished onFinished)
	{
		ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, 1);
		if (activeAnimation == null)
		{
			return;
		}
		activeAnimation.onFinished = onFinished;
	}

	private void OnStarAnimationPlayFinished(ActiveAnimation anim)
	{
		anim.gameObject.SetActive(false);
	}

	private void SetStatueProperties()
	{
		if (null != this.propertiesTxt)
		{
			this.propertiesTxt.Text = this.GeStatueAttributeString();
		}
	}

	private void SetLevelupInfo(int flag = 0)
	{
		if (null != this.needsInfoTxt)
		{
			this.needsInfoTxt.Text = ((flag != 0) ? Global.GetLang("升阶消耗：") : Global.GetLang("升级消耗："));
		}
	}

	private void SetNeedGuardPoints(int level)
	{
		int levelupNeedPoints = this.GetLevelupNeedPoints(level);
		string text = (this.leftGuardPoints >= levelupNeedPoints) ? "ffffff" : "ff0000";
		this.needGuardPointsTxt.Text = Global.GetColorStringForNGUIText(new object[]
		{
			text,
			levelupNeedPoints.ToString()
		});
	}

	private void SetAvailableGuardPoints(int leftPoints)
	{
		if (this.guardStatue != null)
		{
			this.leftGuardPointsTxt.Text = leftPoints.ToString();
		}
	}

	private void SetGrade(int grade)
	{
		if (null != this.gradeTxt)
		{
			this.gradeTxt.Text = grade.ToString();
		}
	}

	private void HideUpLevelView(bool hide = true)
	{
		if (null != this.levelupArea)
		{
			this.levelupArea.SetActive(!hide);
		}
	}

	private void HideUpGradeView(bool hide = true)
	{
		if (null != this.upLevelGoodsIcon)
		{
			this.upLevelGoodsIcon.gameObject.SetActive(!hide);
		}
		if (null != this.specialUpLevelGoodsIcon)
		{
			this.specialUpLevelGoodsIcon.gameObject.SetActive(!hide);
		}
	}

	private void ShowUnactiveHintMsg(int slotID)
	{
		string msg = this.UnActiveMsg(slotID);
		PlayZone.GlobalPlayZone.ShowUnactiveHintMsgWindow(msg);
	}

	private void HideUnactiveHintMsg()
	{
		PlayZone.GlobalPlayZone.CloseUnactiveHintMsgWindow();
	}

	private string UnActiveMsg(int slotID)
	{
		int num = 1;
		if (this.guardStatue != null)
		{
			List<GuardSoulData> soulGuardList = this.guardStatue.soulGuardList;
			if (soulGuardList != null)
			{
				this.dic_slot.TryGetValue(slotID + 1, ref num);
				num -= soulGuardList.Count;
			}
		}
		return string.Format(Global.GetLang("再激活{0}个图鉴可开启新槽位"), num);
	}

	private void SetUpGradeGoodsIconByGrade(int grade)
	{
		if (null == this.upLevelGoodsIcon)
		{
			return;
		}
		GoodsItem levelupNeedGoodsByGrade = this.GetLevelupNeedGoodsByGrade(grade);
		if (levelupNeedGoodsByGrade != null)
		{
			this.upLevelGoodsIcon.Width = 78.0;
			this.upLevelGoodsIcon.Height = 78.0;
			this.upLevelGoodsIcon.ItemObject = Global.GetDummyGoodsData(levelupNeedGoodsByGrade.goodsID);
			this.upLevelGoodsIcon.GoodImg.URL = Global.GetGoodsIconString(levelupNeedGoodsByGrade.goodsID);
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(levelupNeedGoodsByGrade.goodsID);
			Super.InitGoodsGIcon(this.upLevelGoodsIcon, dummyGoodsData, true, IconTextTypes.Qianghua);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(levelupNeedGoodsByGrade.goodsID);
			string text = (totalGoodsCountByID >= levelupNeedGoodsByGrade.count) ? "ffffff" : "ff0000";
			this.upLevelGoodsIcon.SecondText.Text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format("{0}/{1}", totalGoodsCountByID, levelupNeedGoodsByGrade.count)
			});
			if (levelupNeedGoodsByGrade.goodsID2 > 0)
			{
				this.specialUpLevelGoodsIcon.Width = 78.0;
				this.specialUpLevelGoodsIcon.Height = 78.0;
				this.specialUpLevelGoodsIcon.ItemObject = Global.GetDummyGoodsData(levelupNeedGoodsByGrade.goodsID2);
				this.specialUpLevelGoodsIcon.GoodImg.URL = Global.GetGoodsIconString(levelupNeedGoodsByGrade.goodsID2);
				GoodsData dummyGoodsData2 = Global.GetDummyGoodsData(levelupNeedGoodsByGrade.goodsID2);
				Super.InitGoodsGIcon(this.specialUpLevelGoodsIcon, dummyGoodsData2, true, IconTextTypes.Qianghua);
				int totalGoodsCountByID2 = Global.GetTotalGoodsCountByID(levelupNeedGoodsByGrade.goodsID2);
				string text2 = (totalGoodsCountByID2 >= levelupNeedGoodsByGrade.goodsCount2) ? "ffffff" : "ff0000";
				this.specialUpLevelGoodsIcon.SecondText.Text = Global.GetColorStringForNGUIText(new object[]
				{
					text2,
					string.Format("{0}/{1}", totalGoodsCountByID2, levelupNeedGoodsByGrade.goodsCount2)
				});
				this.ResetLocalPositionX(this.upLevelGoodsIcon, 50);
			}
			else
			{
				if (null != this.specialUpLevelGoodsIcon)
				{
					this.specialUpLevelGoodsIcon.gameObject.SetActive(false);
				}
				this.ResetLocalPositionX(this.upLevelGoodsIcon, 0);
			}
		}
	}

	private void ResetLocalPositionX(GGoodIcon icon, int x)
	{
		icon.transform.localPosition = new Vector3((float)x, icon.transform.localPosition.y, icon.transform.localPosition.z);
	}

	private static void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.None, goodsData);
	}

	private void PlayUpGradeAnimation()
	{
		if (null == this.upGradeEffect)
		{
			return;
		}
		this.upGradeEffect.SetActive(false);
		this.upGradeEffect.SetActive(true);
	}

	private int GetLevelupNeedPoints(int level)
	{
		if (this.dic_needPoints == null || this.dic_needPoints.Count <= 0)
		{
			this.dic_needPoints = this.GetLevelupPoints();
		}
		int result = 0;
		this.dic_needPoints.TryGetValue(level, ref result);
		return result;
	}

	private GoodsItem GetLevelupNeedGoodsByGrade(int grade)
	{
		if (this.dic_goodsItem == null || this.dic_goodsItem.Count <= 0)
		{
			this.dic_goodsItem = this.GetGradeUpNeedGoods();
		}
		GoodsItem result = null;
		this.dic_goodsItem.TryGetValue(grade, ref result);
		return result;
	}

	private string GeStatueAttributeString()
	{
		double[] guardStatueAttributes = this.GetGuardStatueAttributes();
		if (guardStatueAttributes == null)
		{
			return string.Empty;
		}
		return Global.GetBaseAttributeStrFromPropertyList(guardStatueAttributes, true, 3);
	}

	private double[] GetGuardStatueAttributes()
	{
		double[] totalBaseProperties = this.GetTotalBaseProperties();
		if (totalBaseProperties == null)
		{
			return null;
		}
		for (int i = 0; i < totalBaseProperties.Length; i++)
		{
			double baseAttr = totalBaseProperties[i];
			totalBaseProperties[i] = Global.CalStatueExpress(baseAttr, this.grade, this.level);
		}
		return totalBaseProperties;
	}

	private double[] GetTotalBaseProperties()
	{
		List<int> equipedSoulGoodsIDList = this.GetEquipedSoulGoodsIDList();
		if (equipedSoulGoodsIDList == null)
		{
			return null;
		}
		double[] array = new double[177];
		for (int i = 0; i < equipedSoulGoodsIDList.Count; i++)
		{
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(equipedSoulGoodsIDList[i]);
			for (int j = 0; j < array.Length; j++)
			{
				array[j] += goodsEquipPropsDoubleList[j];
			}
		}
		return array;
	}

	private Dictionary<int, SoulGuardAttribute> GetSoulGuardConfig()
	{
		XElement gameResXml = Global.GetGameResXml("Config/TuJianShouHuType.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "DiaoXiang");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return null;
		}
		Dictionary<int, SoulGuardAttribute> dictionary = new Dictionary<int, SoulGuardAttribute>(xelementList.Count);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			SoulGuardAttribute soulGuardAttribute = new SoulGuardAttribute();
			soulGuardAttribute.id = Global.GetXElementAttributeInt(xelement, "ID");
			soulGuardAttribute.typeID = Global.GetXElementAttributeInt(xelement, "Type");
			soulGuardAttribute.goodsID = Global.GetXElementAttributeInt(xelement, "GoodsID");
			soulGuardAttribute.name = Global.GetXElementAttributeStr(xelement, "Name");
			if (!dictionary.ContainsKey(soulGuardAttribute.id))
			{
				dictionary.Add(soulGuardAttribute.id, soulGuardAttribute);
			}
		}
		return dictionary;
	}

	private Dictionary<int, int> GetSoulGuardTypes()
	{
		XElement gameResXml = Global.GetGameResXml("Config/TuJianShouHuType.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "DiaoXiang");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return null;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>(xelementList.Count);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Type");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "GoodsID");
			if (!dictionary.ContainsKey(xelementAttributeInt2))
			{
				dictionary.Add(xelementAttributeInt2, xelementAttributeInt);
			}
		}
		return dictionary;
	}

	private Dictionary<int, int> GetLevelupPoints()
	{
		XElement gameResXml = Global.GetGameResXml("Config/ShouHuLevelUp.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "DiaoXiang");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return null;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>(xelementList.Count);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Level");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "NeedShouHu");
			if (!dictionary.ContainsKey(xelementAttributeInt))
			{
				dictionary.Add(xelementAttributeInt, xelementAttributeInt2);
			}
		}
		return dictionary;
	}

	private Dictionary<int, GoodsItem> GetGradeUpNeedGoods()
	{
		XElement gameResXml = Global.GetGameResXml("Config/ShouHuSuitUp.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "DiaoXiang");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return null;
		}
		Dictionary<int, GoodsItem> dictionary = new Dictionary<int, GoodsItem>(xelementList.Count);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Model");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "NeedGoods");
			if (string.IsNullOrEmpty(xelementAttributeStr))
			{
				GoodsItem goodsItem = new GoodsItem();
				goodsItem.goodsID = -1;
				goodsItem.count = -1;
				goodsItem.modelID = xelementAttributeInt2;
				goodsItem.goodsID2 = -1;
				goodsItem.goodsCount2 = -1;
				if (!dictionary.ContainsKey(xelementAttributeInt))
				{
					dictionary.Add(xelementAttributeInt, goodsItem);
				}
			}
			else
			{
				string[] array = xelementAttributeStr.Split(new char[]
				{
					'|'
				});
				string[] array2 = array[0].Split(new char[]
				{
					','
				});
				if (array2.Length == 2)
				{
					GoodsItem goodsItem2 = new GoodsItem();
					goodsItem2.goodsID = Convert.ToInt32(array2[0]);
					goodsItem2.count = Convert.ToInt32(array2[1]);
					goodsItem2.modelID = xelementAttributeInt2;
					if (array.Length >= 2)
					{
						string[] array3 = array[1].Split(new char[]
						{
							','
						});
						if (array3.Length != 2)
						{
							goto IL_1B5;
						}
						goodsItem2.goodsID2 = Convert.ToInt32(array3[0]);
						goodsItem2.goodsCount2 = Convert.ToInt32(array3[1]);
					}
					else
					{
						goodsItem2.goodsID2 = -1;
						goodsItem2.goodsCount2 = -1;
					}
					if (!dictionary.ContainsKey(xelementAttributeInt))
					{
						dictionary.Add(xelementAttributeInt, goodsItem2);
					}
				}
			}
			IL_1B5:;
		}
		return dictionary;
	}

	private void GetSlotConfig()
	{
		if (this.dic_slot == null)
		{
			this.dic_slot = new Dictionary<int, int>();
		}
		this.dic_slot.Clear();
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ShouHuDiaoXiang", '|');
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			string[] array = systemParamStringArrayByName[i].Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				this.dic_slot.Add(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
			}
		}
	}

	public void SetGuardStatueInfo(GuardStatueData guardStatueData)
	{
		Super.HideNetWaiting();
		this.guardStatue = guardStatueData;
		if (this.guardStatue == null)
		{
			this.guardStatue = new GuardStatueData();
			this.guardStatue.grade = 1;
			this.guardStatue.level = 0;
		}
		Global.guardStatueGrade = this.guardStatue.grade;
		Global.guardStatueLevel = this.guardStatue.level;
		if (this.guardStatue.soulGuardList.Count >= 0)
		{
			this.RefreshSoulGuardList(this.guardStatue.soulGuardList);
		}
		this.SetGuardStatueNewGrade(this.guardStatue.grade);
		this.SetGuardStatueNewLevel(this.guardStatue.level, this.guardStatue.hasGuardPoint);
	}

	public void SetGuardStatueLevel(int status, int newLevel, int leftGuardPoints, int flag = 0)
	{
		string textMsg = string.Empty;
		switch (status)
		{
		case 0:
			if (flag == 0)
			{
				this.SetGuardStatueNewLevel(newLevel, leftGuardPoints);
			}
			else if (flag == 1)
			{
				this.SetGuardStatueNewGrade(newLevel);
			}
			break;
		case 1:
			textMsg = Global.GetLang("需要完成3转1级的【废柴壁垒】才可以开启守护雕像");
			break;
		case 4:
			textMsg = Global.GetLang("守护点不足!");
			break;
		case 5:
			textMsg = Global.GetLang("道具不足");
			break;
		case 6:
			textMsg = Global.GetLang("品阶尚未提升");
			break;
		case 7:
			textMsg = Global.GetLang("等级尚未达到10星");
			break;
		case 8:
			textMsg = Global.GetLang("品阶已满，无法继续提升");
			break;
		case 9:
			textMsg = Global.GetLang("等级已满，无法继续提升");
			break;
		case 10:
		case 11:
			textMsg = Global.GetLang("请稍后再试");
			break;
		}
		if (status == 5)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedGuardStone, this.callback, string.Empty, string.Empty);
			return;
		}
		if (status != 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
		}
	}

	private void SetGuardStatueNewLevel(int level, int leftGuardPoints)
	{
		this.level = level;
		Global.guardStatueLevel = level;
		this.isUpLevel = ((level + 10) / 10 <= this.grade);
		int num = level % 10;
		if (!this.isUpLevel && num == 0 && level > 0)
		{
			num = 10;
		}
		this.SetStarProgress(num, this.levelupAniamtionEnable);
		this.leftGuardPoints = leftGuardPoints;
		this.SetAvailableGuardPoints(leftGuardPoints);
		this.SetNeedGuardPoints(level + 1);
		this.SetStatueProperties();
		if (this.maxGrade == this.grade && level == this.maxGrade * 10)
		{
			this.upLevelBtn.gameObject.SetActive(false);
			if (null != this.topLevelText)
			{
				this.topLevelText.SetActive(true);
			}
			return;
		}
		if (!this.isUpLevel)
		{
			this.SetLevelupInfo(1);
			this.upLevelBtn.Text = Global.GetLang("升阶");
			this.HideUpLevelView(true);
			this.HideUpGradeView(false);
			this.SetUpGradeGoodsIconByGrade(this.grade + 1);
		}
	}

	private void SetGuardStatueNewGrade(int grade)
	{
		this.grade = grade;
		Global.guardStatueGrade = grade;
		this.isUpLevel = ((this.level + 10) / 10 <= grade);
		this.SetGrade(grade);
		if (this.levelupAniamtionEnable)
		{
			this.PlayUpGradeAnimation();
		}
		this.SetStarProgress(0, this.levelupAniamtionEnable);
		this.SetLevelupInfo(0);
		this.upLevelBtn.Text = Global.GetLang("升级");
		this.SetStatueProperties();
		GoodsItem levelupNeedGoodsByGrade = this.GetLevelupNeedGoodsByGrade(grade);
		if (levelupNeedGoodsByGrade != null)
		{
			this.Showstatue3DModelByID(levelupNeedGoodsByGrade.modelID, 1f);
		}
		this.HideUpGradeView(true);
		this.HideUpLevelView(false);
	}

	private void GetGuardStatueInfo()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.GetGuardStatueInfo();
	}

	private void EquipSoulGuard(int slotID, int typeID)
	{
		this.levelupAniamtionEnable = false;
		Super.ShowNetWaiting(null);
		GameInstance.Game.EquipGuardSoul(slotID, typeID);
	}

	private void UpLevelStatue()
	{
		int levelupNeedPoints = this.GetLevelupNeedPoints(this.level % 10 + 1);
		if (this.leftGuardPoints < levelupNeedPoints)
		{
			Super.HintMainText(Global.GetLang("守护点不足!"), 10, 3);
			return;
		}
		GameInstance.Game.UpLevelGuardStatue();
	}

	private void UpGradeStatue()
	{
		GoodsItem levelupNeedGoodsByGrade = this.GetLevelupNeedGoodsByGrade(this.grade);
		if (levelupNeedGoodsByGrade != null)
		{
			if (Global.GetTotalGoodsCountByID(levelupNeedGoodsByGrade.goodsID) < levelupNeedGoodsByGrade.count)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedGuardStone, this.callback, string.Empty, string.Empty);
				return;
			}
			if (levelupNeedGoodsByGrade.goodsID2 > 0 && Global.GetTotalGoodsCountByID(levelupNeedGoodsByGrade.goodsID2) < levelupNeedGoodsByGrade.goodsCount2)
			{
				Super.HintMainText(Global.GetLang("道具不足，产品道具还未确定，现在是代替道具!"), 10, 3);
				return;
			}
		}
		GameInstance.Game.UpGradeGuardStatue();
	}

	private const int maxGridCount = 12;

	private const int rowsInPage = 3;

	private const int columnsInPage = 2;

	private const int columns = 0;

	private const int bagSizeAPage = 12;

	private const int aGridSize = 78;

	private const int maxLevelPerGrade = 10;

	public UILabel staticText;

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler callback;

	public GImgProgressBar starProgressBar;

	public Animation starAnim;

	public GameObject upGradeEffect;

	public GameObject levelupArea;

	public TextBlock needGuardPointsTxt;

	public TextBlock leftGuardPointsTxt;

	public TextBlock gradeTxt;

	public GameObject topLevelText;

	public GGoodIcon upLevelGoodsIcon;

	public GGoodIcon specialUpLevelGoodsIcon;

	public GButton upLevelBtn;

	public GButton closeBtn;

	public TextBlock propertiesTxt;

	public GameObject statueModel;

	private Modal3DShow statue3DModel;

	public TextBlock soulGuardInfoTxt;

	public TextBlock propertyInfoTxt;

	public TextBlock needsInfoTxt;

	public TextBlock needGuardPointInfoTxt;

	private BagOrientTypes bagOrient = BagOrientTypes.Vertical;

	private bool isPage;

	public GGoodsBox goodsBox;

	public SpriteSL modalPart;

	private GGoodIcon lastIcon;

	private int currentSlot;

	private Dictionary<int, int> dic_slot;

	private Dictionary<int, int> dic_needPoints;

	private Dictionary<int, GoodsItem> dic_goodsItem;

	private Dictionary<int, int> dic_soulType;

	private Dictionary<int, SoulGuardAttribute> dic_soulGuardAttr;

	private GuardStatueData guardStatue;

	private int serialID;

	private int level;

	private int grade = 1;

	private int leftGuardPoints;

	private int maxGrade = -1;

	private bool isUpLevel = true;

	private bool levelupAniamtionEnable;

	private ResourceResLoader resourceLoader;
}
