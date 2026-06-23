using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;
using XMLCreater;

public class MoShenMiBaoPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblNowWord.text = Global.GetLang("当前星级属性");
		this.lblNextWord.text = Global.GetLang("下一阶级属性");
		this.lblCostWord.text = Global.GetLang("消耗道具");
		this.btnAuto.Label.text = Global.GetLang("自动升阶");
		this.btnShengJie.Label.text = Global.GetLang("升阶");
	}

	private void ResetList()
	{
		this.lstNowWalue.Clear();
		this.lstNowAdd.Clear();
		this.lstNextValue.Clear();
		this.lstImgAdd.Clear();
		this.lstNowWalue.Add(this.lblNowValue1);
		this.lstNowWalue.Add(this.lblNowValue2);
		this.lstNowWalue.Add(this.lblNowValue3);
		this.lstNowWalue.Add(this.lblNowValue4);
		this.lstNowAdd.Add(this.lblNowAdd1);
		this.lstNowAdd.Add(this.lblNowAdd2);
		this.lstNowAdd.Add(this.lblNowAdd3);
		this.lstNowAdd.Add(this.lblNowAdd4);
		this.lstNextValue.Add(this.lblNextValue1);
		this.lstNextValue.Add(this.lblNextValue2);
		this.lstNextValue.Add(this.lblNextValue3);
		this.lstNextValue.Add(this.lblNextValue4);
		this.lstImgAdd.Add(this.imgAdd1);
		this.lstImgAdd.Add(this.imgAdd2);
		this.lstImgAdd.Add(this.imgAdd3);
		this.lstImgAdd.Add(this.imgAdd4);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ResetList();
		this.btnYan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StopAuto();
			this.LoadInfo(MoShenMiBaoType.MoShenYan);
		};
		this.btnYu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StopAuto();
			this.LoadInfo(MoShenMiBaoType.MoShenYu);
		};
		this.btnJiao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StopAuto();
			this.LoadInfo(MoShenMiBaoType.MoShenJiao);
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ChildWindowClose != null)
			{
				this.ChildWindowClose(this, null);
			}
		};
		this.btnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenHelpWindow(string.Empty);
		};
		this.btnAuto.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.m_beAuto)
			{
				if (!this.m_beEnough)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedMoShenMiBao, null, string.Empty, string.Empty);
					return;
				}
				this.m_beAuto = true;
				base.StartCoroutine<bool>(this.StartAuto());
				this.btnAuto.Text = Global.GetLang("取消自动");
			}
			else
			{
				this.StopAuto();
			}
		};
		this.btnShengJie.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.m_beEnough)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedMoShenMiBao, null, string.Empty, string.Empty);
				return;
			}
			this.StopAuto();
			this.SendUpdate();
		};
		this.btnArrowRight.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StopAuto();
			this.m_showLevel++;
			this.LoadInfo(this.m_type, this.m_showLevel);
		};
		this.btnArrowLeft.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StopAuto();
			this.m_showLevel--;
			this.LoadInfo(this.m_type, this.m_showLevel);
		};
		this.LoadInfo(MoShenMiBaoType.MoShenJiao);
	}

	private void LoadInfo(MoShenMiBaoType type)
	{
		int stageLevel = MoShenMiBaoData.GetStageLevel(type);
		int starLevel = MoShenMiBaoData.GetStarLevel(type);
		this.LoadInfo(type, stageLevel);
	}

	private void LoadInfo(MoShenMiBaoType type, int stageLevel)
	{
		if (this.m_type != type)
		{
			this.m_type = type;
			this.ResetMoShenType();
		}
		this.m_showLevel = stageLevel;
		int stageLevel2 = MoShenMiBaoData.GetStageLevel(type);
		int num;
		if (stageLevel2 < stageLevel)
		{
			num = 0;
		}
		else if (stageLevel2 > stageLevel)
		{
			num = 10;
		}
		else
		{
			num = MoShenMiBaoData.GetStarLevel(type);
		}
		this.ResetArrowButton(type, stageLevel);
		MoShenMiBaoXingVO moShenMiBaoXingVOData = IConfigbase<ConfigMoShenMiBao>.Instance.GetMoShenMiBaoXingVOData((int)type, stageLevel, num);
		this.lblJie.text = stageLevel.ToString();
		this.SetStarNum(num);
		this.LoadModel(moShenMiBaoXingVOData);
		if (num == 10)
		{
			MoShenMiBaoJieVO moShenMiBaoJieVOData = IConfigbase<ConfigMoShenMiBao>.Instance.GetMoShenMiBaoJieVOData((int)type, stageLevel);
			this.LoadCostGoods(moShenMiBaoJieVOData.NeedGoods);
		}
		else
		{
			this.LoadCostGoods(moShenMiBaoXingVOData.NeedGoods);
		}
		if (num == 10 && stageLevel == IConfigbase<ConfigMoShenMiBao>.Instance.GetMaxStage())
		{
			this.goCost.SetActive(false);
			this.goMax.SetActive(true);
		}
		else
		{
			this.goCost.SetActive(true);
			this.goMax.SetActive(false);
		}
		this.LoadPropertyInfo(moShenMiBaoXingVOData);
		this.SetShengJieState(moShenMiBaoXingVOData, type, stageLevel);
	}

	private void LoadModel(MoShenMiBaoXingVO starInfo)
	{
		MoShenMiBaoJieVO moShenMiBaoJieVOData = IConfigbase<ConfigMoShenMiBao>.Instance.GetMoShenMiBaoJieVOData(starInfo.MiBaoType, starInfo.MiBaoStageLevel);
		this.Load3DModel(moShenMiBaoJieVOData.MibaoModerID);
	}

	private void LoadPropertyInfo(MoShenMiBaoXingVO starInfo)
	{
		bool flag = false;
		int stageLevel = MoShenMiBaoData.GetStageLevel((MoShenMiBaoType)starInfo.MiBaoType);
		MoShenMiBaoXingVO moShenMiBaoXingVO = null;
		if (starInfo.MiBaoStageLevel == stageLevel)
		{
			if (starInfo.MibaoStarLevel >= 10)
			{
				flag = false;
			}
			else
			{
				flag = true;
				moShenMiBaoXingVO = IConfigbase<ConfigMoShenMiBao>.Instance.GetMoShenMiBaoXingVOData(starInfo.MiBaoType, starInfo.MiBaoStageLevel, starInfo.MibaoStarLevel + 1);
			}
		}
		for (int i = 0; i < 4; i++)
		{
			this.lstNowWalue[i].gameObject.SetActive(false);
			this.lstNowAdd[i].gameObject.SetActive(false);
			this.lstImgAdd[i].gameObject.SetActive(false);
		}
		float num = 0f;
		for (int j = 0; j < 4; j++)
		{
			if (j < starInfo.MiBaoAttribute.Count)
			{
				this.lstNowWalue[j].gameObject.SetActive(true);
				string text = starInfo.MiBaoAttribute[j].ChinesePropName + ":";
				if (starInfo.MiBaoAttribute[j].BePercent)
				{
					string text2 = (starInfo.MiBaoAttribute[j].PropNum * 100f).ToString("f1") + "%";
					string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
					{
						"cea46c",
						text,
						"ffeece",
						text2
					});
					this.lstNowWalue[j].text = colorStringForNGUIText;
					if (flag)
					{
						float num2 = this.lstNowWalue[j].relativeSize.x * this.lstNowWalue[j].transform.localScale.x;
						if (num < num2)
						{
							num = num2;
						}
						float num3 = moShenMiBaoXingVO.MiBaoAttribute[j].PropNum * 100f - starInfo.MiBaoAttribute[j].PropNum * 100f;
						if (num3 > 0f)
						{
							string text3 = num3.ToString("f1") + "%";
							this.lstNowAdd[j].text = text3;
							this.lstNowAdd[j].gameObject.SetActive(true);
							this.lstImgAdd[j].gameObject.SetActive(true);
						}
					}
				}
				else
				{
					string text4 = starInfo.MiBaoAttribute[j].PropNum.ToString();
					string colorStringForNGUIText2 = Global.GetColorStringForNGUIText(new object[]
					{
						"cea46c",
						text,
						"ffeece",
						text4
					});
					this.lstNowWalue[j].text = colorStringForNGUIText2;
					if (flag)
					{
						float num4 = this.lstNowWalue[j].relativeSize.x * this.lstNowWalue[j].transform.localScale.x;
						if (num < num4)
						{
							num = num4;
						}
						float num5 = moShenMiBaoXingVO.MiBaoAttribute[j].PropNum - starInfo.MiBaoAttribute[j].PropNum;
						if (num5 > 0f)
						{
							this.lstNowAdd[j].text = num5.ToString();
							this.lstNowAdd[j].gameObject.SetActive(true);
							this.lstImgAdd[j].gameObject.SetActive(true);
						}
					}
				}
			}
		}
		if (flag)
		{
			for (int k = 0; k < 4; k++)
			{
				float num6 = this.lstNowWalue[k].transform.localPosition.x + num + 15f;
				Vector3 localPosition = this.lstImgAdd[k].transform.localPosition;
				localPosition.x = num6;
				this.lstImgAdd[k].transform.localPosition = localPosition;
				Vector3 localPosition2 = this.lstNowAdd[k].transform.localPosition;
				localPosition2.x = num6 + 15f;
				this.lstNowAdd[k].transform.localPosition = localPosition2;
			}
		}
		if (starInfo.MiBaoStageLevel == IConfigbase<ConfigMoShenMiBao>.Instance.GetMaxStage())
		{
			this.lstNextValue[0].gameObject.SetActive(true);
			this.lstNextValue[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"adadb3",
				Global.GetLang("已达最高阶数")
			});
			for (int l = 1; l < 4; l++)
			{
				this.lstNextValue[l].gameObject.SetActive(false);
			}
		}
		else
		{
			MoShenMiBaoXingVO moShenMiBaoXingVOData = IConfigbase<ConfigMoShenMiBao>.Instance.GetMoShenMiBaoXingVOData(starInfo.MiBaoType, starInfo.MiBaoStageLevel + 1, 0);
			for (int m = 0; m < 4; m++)
			{
				if (m < moShenMiBaoXingVOData.MiBaoAttribute.Count)
				{
					this.lstNextValue[m].gameObject.SetActive(true);
					string text5 = moShenMiBaoXingVOData.MiBaoAttribute[m].ChinesePropName + ":";
					if (moShenMiBaoXingVOData.MiBaoAttribute[m].BePercent)
					{
						string text6 = (moShenMiBaoXingVOData.MiBaoAttribute[m].PropNum * 100f).ToString("f1") + "%";
						string colorStringForNGUIText3 = Global.GetColorStringForNGUIText(new object[]
						{
							"adadb3",
							text5,
							"adadb3",
							text6
						});
						this.lstNextValue[m].text = colorStringForNGUIText3;
					}
					else
					{
						string text7 = moShenMiBaoXingVOData.MiBaoAttribute[m].PropNum.ToString();
						string colorStringForNGUIText4 = Global.GetColorStringForNGUIText(new object[]
						{
							"adadb3",
							text5,
							"adadb3",
							text7
						});
						this.lstNextValue[m].text = colorStringForNGUIText4;
					}
				}
				else
				{
					this.lstNextValue[m].gameObject.SetActive(false);
				}
			}
		}
	}

	private void ResetMoShenType()
	{
		switch (this.m_type)
		{
		case MoShenMiBaoType.MoShenJiao:
			this.imgType.spriteName = "titleJiao";
			break;
		case MoShenMiBaoType.MoShenYu:
			this.imgType.spriteName = "titleyu";
			break;
		case MoShenMiBaoType.MoShenYan:
			this.imgType.spriteName = "titleyan";
			break;
		}
		GButton gbutton = null;
		if (this.m_type == MoShenMiBaoType.MoShenJiao)
		{
			gbutton = this.btnJiao;
		}
		else if (this.m_type == MoShenMiBaoType.MoShenYan)
		{
			gbutton = this.btnYan;
		}
		else if (this.m_type == MoShenMiBaoType.MoShenYu)
		{
			gbutton = this.btnYu;
		}
		if (gbutton != null)
		{
			this.imgSelected.transform.position = gbutton.transform.position;
		}
	}

	private void ResetArrowButton(MoShenMiBaoType type, int stageLevel)
	{
		int stageLevel2 = MoShenMiBaoData.GetStageLevel(type);
		this.btnArrowLeft.isEnabled = (stageLevel > 1);
		if (stageLevel >= IConfigbase<ConfigMoShenMiBao>.Instance.GetMaxStage())
		{
			this.btnArrowRight.isEnabled = false;
		}
		else
		{
			this.btnArrowRight.isEnabled = (stageLevel <= stageLevel2);
		}
	}

	private void SetShengJieState(MoShenMiBaoXingVO starInfo, MoShenMiBaoType type, int stageLevel)
	{
		int stageLevel2 = MoShenMiBaoData.GetStageLevel(type);
		int starLevel = MoShenMiBaoData.GetStarLevel(type);
		this.lblCostWord.text = Global.GetLang("消耗道具");
		if (stageLevel2 < stageLevel)
		{
			this.btnShengJie.Text = Global.GetLang("升星");
			this.btnAuto.Text = (this.m_beAuto ? Global.GetLang("取消自动") : Global.GetLang("自动升星"));
			this.btnShengJie.isEnabled = false;
			this.btnAuto.isEnabled = false;
			this.lblCostWord.text = Global.GetLang("未开启");
			this.imgStarExp.fillAmount = 0f;
			this.goStageExp.SetActive(false);
			this.imgStageExp.fillAmount = 0f;
		}
		else if (stageLevel2 > stageLevel)
		{
			this.btnShengJie.Text = Global.GetLang("升星");
			this.btnAuto.Text = (this.m_beAuto ? Global.GetLang("取消自动") : Global.GetLang("自动升星"));
			this.btnShengJie.isEnabled = false;
			this.btnAuto.isEnabled = false;
			this.imgStarExp.fillAmount = 1f;
			this.goStageExp.SetActive(true);
			this.imgStageExp.fillAmount = 1f;
		}
		else if (starLevel == 10)
		{
			this.btnShengJie.Text = Global.GetLang("升阶");
			GButton gbutton = this.btnAuto;
			string text = this.m_beAuto ? Global.GetLang("取消自动") : Global.GetLang("自动升阶");
			this.btnAuto.Text = text;
			gbutton.Text = text;
			if (stageLevel2 == IConfigbase<ConfigMoShenMiBao>.Instance.GetMaxStage())
			{
				this.btnShengJie.isEnabled = false;
				this.btnAuto.isEnabled = false;
				this.imgStarExp.fillAmount = 1f;
				this.goStageExp.SetActive(true);
				this.imgStageExp.fillAmount = 1f;
			}
			else
			{
				this.btnShengJie.isEnabled = true;
				this.btnAuto.isEnabled = true;
				this.imgStarExp.fillAmount = 1f;
				this.goStageExp.SetActive(true);
				int exp = MoShenMiBaoData.GetExp(type);
				this.imgStageExp.fillAmount = (float)exp * 1f / (float)IConfigbase<ConfigMoShenMiBao>.Instance.GetJieUpExp((int)type, stageLevel);
			}
		}
		else
		{
			this.btnShengJie.Text = Global.GetLang("升星");
			GButton gbutton2 = this.btnAuto;
			string text = this.m_beAuto ? Global.GetLang("取消自动") : Global.GetLang("自动升星");
			this.btnAuto.Text = text;
			gbutton2.Text = text;
			this.btnShengJie.isEnabled = true;
			this.btnAuto.isEnabled = true;
			int exp2 = MoShenMiBaoData.GetExp(type);
			this.imgStarExp.fillAmount = (float)exp2 * 1f / (float)starInfo.MibaoStarExp;
			this.goStageExp.SetActive(false);
			this.imgStageExp.fillAmount = 0f;
		}
	}

	private void SetStarNum(int num)
	{
		if (num < 0)
		{
			num = 0;
		}
		if (num > 10)
		{
			num = 10;
		}
		for (int i = 0; i < num; i++)
		{
			this.lstStars[i].spriteName = "xingxing1";
		}
		for (int j = num; j < 10; j++)
		{
			this.lstStars[j].spriteName = "xingxing2";
		}
	}

	private void LoadCostGoods(List<MUMaterialInfo> costGoods)
	{
		int childCount = this.gridCost.transform.childCount;
		for (int i = childCount - 1; i >= 0; i--)
		{
			Object.Destroy(this.gridCost.transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < costGoods.Count; j++)
		{
			GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIconByGoodsID(costGoods[j].MaterialId, true);
			if (!(ggoodIcon == null))
			{
				ggoodIcon.transform.SetParent(this.gridCost.transform);
				ggoodIcon.isAutoSize = false;
				ggoodIcon.Width = 70.0;
				ggoodIcon.Height = 70.0;
				ggoodIcon.transform.localPosition = new Vector3((float)j * this.gridCost.cellWidth, 0f, 0f);
				ggoodIcon.transform.localScale = Vector3.one;
				int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(costGoods[j].MaterialId);
				ggoodIcon.SecondText.Label.supportEncoding = true;
				if (roleGoodsNumberCountByGoodsID >= costGoods[j].Num)
				{
					this.m_beEnough = true;
					ggoodIcon.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Format("{0}/{1}", roleGoodsNumberCountByGoodsID, costGoods[j].Num)
					});
				}
				else
				{
					this.m_beEnough = false;
					ggoodIcon.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format("{0}/{1}", roleGoodsNumberCountByGoodsID, costGoods[j].Num)
					});
				}
			}
		}
		Vector3 localPosition = this.gridCost.transform.localPosition;
		if (costGoods.Count == 1)
		{
			localPosition.x = this.gridCost.cellWidth / 2f;
			this.gridCost.transform.localPosition = localPosition;
		}
		else
		{
			localPosition.x = 0f;
			this.gridCost.transform.localPosition = localPosition;
		}
	}

	private void AddTeXiao(int type)
	{
		try
		{
			for (int i = 0; i < this.lstTeXiao.Length; i++)
			{
				this.lstTeXiao[i].SetActive(false);
			}
			this.lstTeXiao[type].SetActive(true);
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				ex.Message
			});
		}
	}

	private IEnumerator StartAuto()
	{
		while (this.m_beAuto)
		{
			this.SendUpdate();
			yield return new WaitForSeconds(1f);
		}
		yield break;
		yield break;
	}

	private void StopAuto()
	{
		this.m_beAuto = false;
		base.StopAllCoroutines();
		int starLevel = MoShenMiBaoData.GetStarLevel(this.m_type);
		if (starLevel == 10)
		{
			this.btnAuto.Text = Global.GetLang("自动升阶");
		}
		else
		{
			this.btnAuto.Text = Global.GetLang("自动升星");
		}
	}

	private void OpenHelpWindow(string path)
	{
		if (this.m_helpWindow == null)
		{
			this.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			this.m_helpWindow.IsShowModal = true;
			this.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_helpWindow, Global.GetLang("NewCommonHelpWindow"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_helpWindow);
		}
		if (this.m_helpPart == null)
		{
			this.m_helpPart = U3DUtils.NEW<NewCommonHelpWindow>();
			this.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHelpWindow();
			};
		}
		this.m_helpWindow.SetContent(this.m_helpWindow.BodyPresenter, this.m_helpPart, 0.0, 0.0, true);
		XElement gameResXml = Global.GetGameResXml(IConfigbase<ConfigMoShenMiBao>.Instance.MoShenMiBaoIntroOXMLPath);
		if (gameResXml == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				string.Format("加载{0}出现错误", path)
			});
		}
		ChangeableRulePart.RuleXml ruleXml = new ChangeableRulePart.RuleXml(gameResXml);
		this.m_helpPart.SetHelpInfo(ruleXml.list, true);
	}

	private void CloseHelpWindow()
	{
		if (null != this.m_helpPart)
		{
			this.m_helpPart.transform.parent = null;
			Object.Destroy(this.m_helpPart.gameObject);
			this.m_helpPart = null;
		}
		if (null != this.m_helpWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_helpWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_helpWindow, true);
			this.m_helpWindow = null;
		}
	}

	protected new virtual void OnDestroy()
	{
		base.OnDestroy();
		base.StopAllCoroutines();
	}

	private void Load3DModel(int modelId)
	{
		if (this.m_currentId == modelId)
		{
			return;
		}
		this.m_currentId = modelId;
		Modal3DShow[] componentsInChildren = this.modelContainer.GetComponentsInChildren<Modal3DShow>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Clear();
				Object.DestroyObject(componentsInChildren[i].gameObject);
			}
		}
		Modal3DShow modal3DShow = U3DUtils.NEW<Modal3DShow>();
		U3DUtils.AddChild(this.modelContainer, modal3DShow.gameObject, false);
		ResourceResLoader resourceResLoader = UIHelper.LoadModelResource(modal3DShow, modelId, 1f, delegate(object e, DPSelectedItemEventArgs s)
		{
			Modal3DShow componentInChildren = this.modelContainer.GetComponentInChildren<Modal3DShow>();
			if (componentInChildren != null)
			{
				UIHelper.SetReanderQueue(modal3DShow.gameObject);
				SkinnedMeshRenderer[] componentsInChildren2 = componentInChildren.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
				if (componentsInChildren2 == null || componentsInChildren2.Length <= 0)
				{
					return;
				}
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					if ("Artist/PlayerCharacter".Equals(componentsInChildren2[j].sharedMaterial.shader.name))
					{
						componentsInChildren2[j].sharedMaterial.shader = Shader.Find("Artist/PlayerCharacterForUI");
					}
				}
			}
		});
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
		MUEventManager.AddEventListener<MazingerStore>("CMD_SPR_MAZINGERSTORE_UPGRADE ", new Action<MazingerStore>(this.ServerShengXing));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MazingerStore>("CMD_SPR_MAZINGERSTORE_UPGRADE ", new Action<MazingerStore>(this.ServerShengXing));
	}

	private void SendUpdate()
	{
		if (Time.realtimeSinceStartup - this.m_lastSendTime < 0.8f)
		{
			return;
		}
		Super.ShowNetWaiting(null);
		this.m_lastSendTime = Time.realtimeSinceStartup;
		int starLevel = MoShenMiBaoData.GetStarLevel(this.m_type);
		if (starLevel == 10)
		{
			GameInstance.Game.SendMoShenShengJie((int)this.m_type, 1);
		}
		else
		{
			GameInstance.Game.SendMoShenShengJie((int)this.m_type, 0);
		}
	}

	private void ShowShengXingTeXiao(int starLevel)
	{
		Vector3 localPosition = this.lstStars[starLevel - 1].transform.localPosition;
		this.goStarTeXiao.transform.localPosition = localPosition;
		this.goStarTeXiao.SetActive(true);
	}

	private void ServerShengXing(MazingerStore data)
	{
		if (data.result != 1)
		{
			this.StopAuto();
			return;
		}
		MazingerStoreData mazingerStoreData = MoShenMiBaoData.GetMazingerStoreData(this.m_type);
		int stageLevel = MoShenMiBaoData.GetStageLevel(this.m_type);
		int starLevel = MoShenMiBaoData.GetStarLevel(this.m_type);
		int exp = MoShenMiBaoData.GetExp(this.m_type);
		mazingerStoreData.Stage = data.data.Stage;
		mazingerStoreData.StarLevel = data.data.StarLevel;
		mazingerStoreData.Exp = data.data.Exp;
		MoShenMiBaoXingVO moShenMiBaoXingVOData = IConfigbase<ConfigMoShenMiBao>.Instance.GetMoShenMiBaoXingVOData((int)this.m_type, stageLevel, starLevel);
		this.LoadInfo(this.m_type);
		if (data.IsBoom == 1)
		{
			this.AddTeXiao(2);
		}
		if (data.data.Stage > stageLevel)
		{
			this.AddTeXiao(3);
			this.StopAuto();
		}
		else if (data.data.StarLevel > starLevel)
		{
			this.ShowShengXingTeXiao(data.data.StarLevel);
			Super.HintMainText(Global.GetLang("升星成功"), 10, 3);
			if (data.data.StarLevel == 10)
			{
				this.StopAuto();
			}
		}
		else if (starLevel != 10)
		{
			Super.HintMainText("+" + (data.data.Exp - exp), 10, 3);
		}
	}

	private const int MaxLevel = 10;

	public UILabel lblNowWord;

	public UILabel lblNowValue1;

	public UILabel lblNowAdd1;

	public UILabel lblNowValue2;

	public UILabel lblNowAdd2;

	public UILabel lblNowValue3;

	public UILabel lblNowAdd3;

	public UILabel lblNowValue4;

	public UILabel lblNowAdd4;

	public UILabel lblNextWord;

	public UILabel lblNextValue1;

	public UILabel lblNextValue2;

	public UILabel lblNextValue3;

	public UILabel lblNextValue4;

	public UILabel lblCostWord;

	public UILabel lblJie;

	public GButton btnYan;

	public GButton btnYu;

	public GButton btnJiao;

	public GButton btnClose;

	public GButton btnHelp;

	public GButton btnAuto;

	public GButton btnShengJie;

	public GButton btnArrowRight;

	public GButton btnArrowLeft;

	public UISprite imgType;

	public UISprite imgAdd1;

	public UISprite imgAdd2;

	public UISprite imgAdd3;

	public UISprite imgAdd4;

	public UISprite imgSelected;

	public List<UISprite> lstStars;

	public GameObject goStarTeXiao;

	public UISprite imgStarExp;

	public GameObject goStageExp;

	public UISprite imgStageExp;

	public UIGrid gridCost;

	public GameObject[] lstTeXiao;

	private List<UILabel> lstNowWalue = new List<UILabel>();

	private List<UILabel> lstNowAdd = new List<UILabel>();

	private List<UILabel> lstNextValue = new List<UILabel>();

	private List<UISprite> lstImgAdd = new List<UISprite>();

	public GameObject goCost;

	public GameObject goMax;

	public GameObject modelContainer;

	private int m_showLevel = 1;

	private MoShenMiBaoType m_type;

	private bool m_beAuto;

	private float m_lastSendTime;

	private bool m_beEnough = true;

	protected GChildWindow m_helpWindow;

	protected NewCommonHelpWindow m_helpPart;

	private int m_currentId = -1;
}
