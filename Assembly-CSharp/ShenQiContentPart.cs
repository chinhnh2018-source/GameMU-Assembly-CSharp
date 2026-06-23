using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenQiContentPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitButton();
		this.m_CheckBox.isChecked = false;
	}

	public void InitValue(int id, bool isManJi)
	{
		this.m_ShenQiDict = ShenQiManager.GetCurrentShenQiDataDict();
		this.m_CurrentID = id;
		this.RefreshUIData(this.GetCurrentData(), isManJi);
		if (this.m_CurrentID == 1)
		{
			this.IsShowLeftArrow(false);
		}
		if (ShenQiManager.GetShenQiXMLDict().Count == this.m_CurrentID)
		{
			this.IsShowRightArrow(false);
		}
		this.Show3DModel(false);
	}

	private void InitTextInPrefabs()
	{
		try
		{
			this.m_PriceJingHuaFont.Pivot = 5;
			this.m_PriceJingHuaFont.X = -80.0;
			this.m_BaoJiLabel.Pivot = 3;
			this.m_BaoJiLabel.X = 65.0;
			this.m_PropertyTitle.Text = Global.GetLang("基础属性");
			this.m_Life.Text = Global.GetLang("生命值：");
			this.m_Attack.Text = Global.GetLang("攻击力：");
			this.m_Defend.Text = Global.GetLang("防御力：");
			this.m_Toughness.Text = Global.GetLang("韧    性：");
			this.m_TiShengTitle.Text = Global.GetLang("神器提升");
			this.m_PriceJinBiFont.Text = Global.GetLang("消耗金币：");
			this.m_PriceJingHuaFont.Text = Global.GetLang("神力精华：");
			this.m_CheckBox.Text = Global.GetLang("额外消耗");
			this.m_BaoJiLabel.Text = Global.GetLang("100必暴击");
			this.m_ZhuRuBtn.Text = Global.GetLang("注入");
			if (this.m_WeiKaiQi.transform.GetChild(0).name.Equals("Label"))
			{
				this.m_WeiKaiQi.transform.GetChild(0).GetComponent<UILabel>().text = Global.GetLang("需要将上一级神器升至满级");
			}
			if (this.m_CheckBox.transform.GetChild(3).name.Equals("IconDiamond"))
			{
				this.m_CheckBox.transform.GetChild(3).transform.localPosition = new Vector3(100f, 0f, 0f);
			}
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"越南用，赋值不成功"
			});
		}
	}

	private new void Update()
	{
		if (this.isClickDown)
		{
			this.waitTime -= Time.deltaTime;
			if (this.waitTime < 0f)
			{
				this.isClickDown = false;
				this.waitTime = 0.5f;
			}
		}
	}

	private void ShowQieHaunTeXiao()
	{
		this.mTeXiao = (Object.Instantiate(Resources.Load("UITeXiao/Perfabs/shengwu/shengwu_gaojie")) as GameObject);
		U3DUtils.AddChild(base.gameObject, this.mTeXiao, false);
		this.mTeXiao.transform.localPosition = new Vector3(-173f, -40f, -510f);
		base.StartCoroutine<bool>(this.DestroyTeXiao());
	}

	private IEnumerator DestroyTeXiao()
	{
		yield return new WaitForSeconds(0.5f);
		if (null != this.mTeXiao)
		{
			Object.Destroy(this.mTeXiao);
			this.mTeXiao = null;
		}
		yield break;
	}

	private void InitButton()
	{
		this.m_HelpBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.HelpWindow();
		};
		this.m_LeftArrow.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isClickDown)
			{
				return;
			}
			this.isClickDown = true;
			if (this.MoveLeftNext())
			{
				this.RefreshUIData(this.GetCurrentData(), false);
				this.IsShowRightArrow(true);
				this.Show3DModel(false);
				this.SetStatus((!this.IsManJi()) ? ShenQiContentPart.ShenQiStatus.WeiManJi : ShenQiContentPart.ShenQiStatus.ManJi);
			}
			else
			{
				this.IsShowLeftArrow(false);
			}
		};
		this.m_RightArrow.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isClickDown)
			{
				return;
			}
			this.isClickDown = true;
			if (this.MoveRightNext())
			{
				this.RefreshUIData(this.GetCurrentData(), false);
				this.IsShowLeftArrow(true);
				this.Show3DModel(false);
			}
			else
			{
				this.IsShowRightArrow(false);
			}
		};
		this.m_ZhuRuBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			ShenQiXMLData currentData = this.GetCurrentData();
			if (currentData.CostDiamond > Global.Data.roleData.UserMoney && this.m_CheckBox.isChecked)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.CloseCallback, string.Empty, string.Empty);
				return;
			}
			bool flag = false;
			if (!string.IsNullOrEmpty(currentData.CostGoldGoods))
			{
				string[] array = currentData.CostGoldGoods.Split(new char[]
				{
					','
				});
				if (array.Length == 2)
				{
					int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(array[0].SafeToInt32(0));
					if (roleGoodsNumberCountByGoodsID >= array[1].SafeToInt32(0))
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				if (currentData.CostGoldCoin > Global.GetRoleOwnNumByMoneyType(8))
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.CloseCallback, string.Empty, string.Empty);
					return;
				}
			}
			if (currentData.CostShenLiJingHua > Global.Data.ShenLiJingHuaCount)
			{
				Super.HintMainText(Global.GetLang("神力精华不足"), 10, 3);
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedShenLiJingHua, this.CloseCallback, string.Empty, string.Empty);
				return;
			}
			if (this.m_CheckBox.isChecked)
			{
				IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ShenQiXiTong", this.GetCurrentData().CostDiamond, false);
			}
			GameInstance.Game.SendShenQiZhuRuRequest(this.isUseDiamond);
		};
		this.m_CheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.m_CheckBox.isChecked)
			{
				this.m_CheckBox.isChecked = false;
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "ShenQiXiTong", this.m_TmpShenQiXMLData.CostDiamond);
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("选择后每次需要消耗{0}，确定执行吗"), text)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.m_CheckBox.isChecked = true;
					}
					else
					{
						this.m_CheckBox.isChecked = false;
					}
					this.isUseDiamond = ((!this.m_CheckBox.isChecked) ? 0 : 1);
					string[] array3 = this.m_TmpShenQiXMLData.QiangHua.Split(new char[]
					{
						'|'
					});
					string[] array4 = array3[(!this.m_CheckBox.isChecked) ? 0 : 1].Split(new char[]
					{
						','
					});
					this.m_LifeValue.Text = string.Format("{0}", this.m_TmpShenQiXMLData.LifeV);
					this.m_LifeValueUpValue.Text = array4[1];
					this.m_AttackValue.Text = string.Format("{0}", this.m_TmpShenQiXMLData.AddAttack);
					this.m_AttackValueUpValue.Text = array4[2];
					this.m_DefendValue.Text = string.Format("{0}", this.m_TmpShenQiXMLData.AddDefense);
					this.m_DefendValueUpValue.Text = array4[3];
					this.m_ToughnessValue.Text = string.Format("{0}", this.m_TmpShenQiXMLData.Toughness);
					this.m_ToughnessValueUpValue.Text = array4[4];
					return true;
				};
				return;
			}
			this.m_CheckBox.isChecked = false;
			this.isUseDiamond = ((!this.m_CheckBox.isChecked) ? 0 : 1);
			string[] array = this.m_TmpShenQiXMLData.QiangHua.Split(new char[]
			{
				'|'
			});
			string[] array2 = array[(!this.m_CheckBox.isChecked) ? 0 : 1].Split(new char[]
			{
				','
			});
			this.m_LifeValue.Text = string.Format("{0}", this.m_TmpShenQiXMLData.LifeV);
			this.m_LifeValueUpValue.Text = array2[1];
			this.m_AttackValue.Text = string.Format("{0}", this.m_TmpShenQiXMLData.AddAttack);
			this.m_AttackValueUpValue.Text = array2[2];
			this.m_DefendValue.Text = string.Format("{0}", this.m_TmpShenQiXMLData.AddDefense);
			this.m_DefendValueUpValue.Text = array2[3];
			this.m_ToughnessValue.Text = string.Format("{0}", this.m_TmpShenQiXMLData.Toughness);
			this.m_ToughnessValueUpValue.Text = array2[4];
		};
		this.m_DetailsBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenShenQiPropertyPart(ShenQiPropertyType.RenXing, Global.GetLang("韧性属性总览"), (int)Global.GetCurrentRoleProp(2, 101), null);
		};
	}

	private bool IsManJi()
	{
		ShenQiXMLData currentShenQiDataByID = ShenQiManager.GetCurrentShenQiDataByID(this.m_CurrentID);
		float num = (float)(currentShenQiDataByID.LifeV + currentShenQiDataByID.AddDefense + currentShenQiDataByID.Toughness + currentShenQiDataByID.AddAttack);
		ShenQiXMLData shenQiDataByID = ShenQiManager.GetShenQiDataByID(this.m_CurrentID);
		float num2 = (float)(shenQiDataByID.LifeV + shenQiDataByID.AddDefense + shenQiDataByID.Toughness + shenQiDataByID.AddAttack);
		return num >= num2;
	}

	private bool IsPreManJi()
	{
		bool result = true;
		if (this.m_CurrentID >= 2)
		{
			int id = this.m_CurrentID - 1;
			ShenQiXMLData currentShenQiDataByID = ShenQiManager.GetCurrentShenQiDataByID(id);
			float num = (float)(currentShenQiDataByID.LifeV + currentShenQiDataByID.AddDefense + currentShenQiDataByID.Toughness + currentShenQiDataByID.AddAttack);
			ShenQiXMLData shenQiDataByID = ShenQiManager.GetShenQiDataByID(id);
			float num2 = (float)(shenQiDataByID.LifeV + shenQiDataByID.AddDefense + shenQiDataByID.Toughness + shenQiDataByID.AddAttack);
			result = (num >= num2);
		}
		return result;
	}

	private void Show3DModel(bool isShowEffect = false)
	{
		if (isShowEffect)
		{
			if (null != this.medalModal)
			{
				Object.Destroy(this.medalModal.gameObject);
				this.medalModal = null;
				int childCount = this.m_3DModelParent.childCount;
				if (childCount > 0)
				{
					for (int i = 0; i < childCount; i++)
					{
						Object.Destroy(this.m_3DModelParent.GetChild(i).gameObject);
					}
				}
			}
			this.ShowQieHaunTeXiao();
			base.StartCoroutine<bool>(this.WaitForSeconds());
		}
		else
		{
			ShenQiXMLData currentData = this.GetCurrentData();
			int modID = currentData.ModID;
			this.m_Title.Text = Global.GetLang(currentData.Name);
			this.ShowMedalModalByID(modID, 1f);
		}
	}

	private IEnumerator WaitForSeconds()
	{
		yield return new WaitForSeconds(0.3f);
		ShenQiXMLData data = this.GetCurrentData();
		int modelID = data.ModID;
		this.m_Title.Text = Global.GetLang(data.Name);
		this.ShowMedalModalByID(modelID, 1f);
		yield break;
	}

	public void ShowMedalModalByID(int modelID, float scale = 1f)
	{
		if (null != this.medalModal)
		{
			Object.Destroy(this.medalModal.gameObject);
			this.medalModal = null;
			int childCount = this.m_3DModelParent.childCount;
			if (childCount > 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					Object.Destroy(this.m_3DModelParent.GetChild(i).gameObject);
				}
			}
		}
		this.medalModal = U3DUtils.NEW<Modal3DShow>();
		U3DUtils.AddChild(this.m_3DModelParent.gameObject, this.medalModal.gameObject, true);
		Transform transform = this.medalModal.transform;
		transform.localPosition = new Vector3(0f, -90f, -0.8f);
		transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		transform.localScale = new Vector3(270f, 270f, 270f);
		this.medalModal.CanRotate = false;
		UIHelper.SetModalPosZ(this.medalModal.transform);
		if (this.resourceLoader != null)
		{
			this.resourceLoader.Stop();
		}
		this.resourceLoader = UIHelper.LoadModelResource(this.medalModal, modelID, scale, null);
		this.medalModal.LoadCompleteCallBack = delegate(object s, DPSelectedItemEventArgs e)
		{
			MeshRenderer componentInChildren = this.medalModal.gameObject.GetComponentInChildren<MeshRenderer>();
			componentInChildren.sharedMaterial.shader = Shader.Find("Artist/PlayerCharacterForUI");
		};
	}

	private void IsShowLeftArrow(bool isShow)
	{
		this.m_LeftArrow.gameObject.SetActive(isShow);
	}

	private void IsShowRightArrow(bool isShow)
	{
		this.m_RightArrow.gameObject.SetActive(isShow);
	}

	private bool MoveLeftNext()
	{
		this.m_CurrentID--;
		if (this.m_CurrentID == 1)
		{
			this.IsShowLeftArrow(false);
		}
		return this.m_CurrentID > 0;
	}

	private bool MoveRightNext()
	{
		this.m_CurrentID++;
		if (this.m_ShenQiDict.Count <= ShenQiManager.GetShenQiXMLDict().Count)
		{
			if (this.m_ShenQiDict.Count == this.m_CurrentID)
			{
				this.IsShowRightArrow(false);
				this.SetStatus(ShenQiContentPart.ShenQiStatus.WeiKaiQi);
			}
			if (this.m_ShenQiDict.Count - 1 == this.m_CurrentID)
			{
				this.SetStatus(ShenQiContentPart.ShenQiStatus.WeiManJi);
			}
		}
		else if (this.m_ShenQiDict.Count == this.m_CurrentID + 1)
		{
			this.IsShowRightArrow(false);
		}
		else
		{
			this.IsShowRightArrow(true);
		}
		return this.m_CurrentID <= this.m_ShenQiDict.Count;
	}

	private ShenQiXMLData GetCurrentData()
	{
		if (this.m_CurrentID >= 1 && this.m_CurrentID <= this.m_ShenQiDict.Count)
		{
			return this.m_ShenQiDict[this.m_CurrentID];
		}
		return default(ShenQiXMLData);
	}

	private void RefreshUIData(ShenQiXMLData data, bool _IsMamJi = false)
	{
		if (data.ID == 0)
		{
			return;
		}
		if (string.IsNullOrEmpty(data.QiangHua))
		{
			return;
		}
		this.m_TmpShenQiXMLData = data;
		string[] array = data.QiangHua.Split(new char[]
		{
			'|'
		});
		string[] array2 = array[(!this.m_CheckBox.isChecked) ? 0 : 1].Split(new char[]
		{
			','
		});
		this.m_LifeValue.Text = string.Format("{0}", data.LifeV);
		this.m_LifeValueUpValue.Text = array2[1];
		this.m_AttackValue.Text = string.Format("{0}", data.AddAttack);
		this.m_AttackValueUpValue.Text = array2[2];
		this.m_DefendValue.Text = string.Format("{0}", data.AddDefense);
		this.m_DefendValueUpValue.Text = array2[3];
		this.m_ToughnessValue.Text = string.Format("{0}", data.Toughness);
		this.m_ToughnessValueUpValue.Text = array2[4];
		if (data.ID == ShenQiManager.GetShenQiXMLDict().Count && _IsMamJi)
		{
			this.SetStatus(ShenQiContentPart.ShenQiStatus.ManJi);
			return;
		}
		bool flag = false;
		if (!string.IsNullOrEmpty(data.CostGoldGoods))
		{
			string[] array3 = data.CostGoldGoods.Split(new char[]
			{
				','
			});
			if (array3.Length == 2)
			{
				int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(array3[0].SafeToInt32(0));
				if (roleGoodsNumberCountByGoodsID >= array3[1].SafeToInt32(0))
				{
					this.m_PriceJinBi.Text = Global.GetColorStringForNGUIText(new object[]
					{
						"f0f0f0",
						array3[1].SafeToInt32(0)
					});
					flag = true;
					this.m_PriceJinBiSp.gameObject.SetActive(false);
					this.m_PriceJinBiFont.Text = Global.GetLang("消耗代券：");
				}
			}
		}
		if (!flag)
		{
			string text = (data.CostGoldCoin <= Global.GetRoleOwnNumByMoneyType(8)) ? "f0f0f0" : "ff0000";
			this.m_PriceJinBi.Text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				data.CostGoldCoin
			});
			this.m_PriceJinBiSp.spriteName = "icon_jinBi";
			this.m_PriceJinBiSp.gameObject.SetActive(true);
			this.m_PriceJinBiFont.Text = Global.GetLang("消耗金币：");
		}
		string text2 = (data.CostShenLiJingHua <= Global.Data.ShenLiJingHuaCount) ? "f0f0f0" : "ff0000";
		this.m_PriceJingHua.Text = Global.GetColorStringForNGUIText(new object[]
		{
			text2,
			data.CostShenLiJingHua
		});
		this.m_BaoJiLabel.Text = string.Format("{0}{1}", data.CostDiamond, Global.GetLang("必暴击"));
		float num = (float)(data.LifeV + data.AddDefense + data.Toughness + data.AddAttack);
		ShenQiXMLData shenQiDataByID = ShenQiManager.GetShenQiDataByID(this.m_CurrentID);
		float num2 = (float)(shenQiDataByID.LifeV + shenQiDataByID.AddDefense + shenQiDataByID.Toughness + shenQiDataByID.AddAttack);
		float num3 = num / num2;
		if (data.ID == ShenQiManager.GetShenQiXMLDict().Count && num3 != 1f && this.IsPreManJi())
		{
			this.SetStatus(ShenQiContentPart.ShenQiStatus.WeiManJi);
		}
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "ShenQiXiTong", this.m_TmpShenQiXMLData.CostDiamond, string.Empty);
		this.SetProgressValue(num3);
	}

	public void RefreshUIByServerData(ShenQiData data)
	{
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "ShenQiXiTong", this.m_TmpShenQiXMLData.CostDiamond, string.Empty);
		if (this.m_CheckBox.isChecked && IConfigbase<ConfigDaiBiShiYong>.Instance.CloseZiDong("ShenQiXiTong", this.GetCurrentData().CostDiamond))
		{
			this.m_CheckBox.isChecked = false;
		}
		if (data == null)
		{
			Super.HintMainText(Global.GetLang("服务器返回数据为空"), 10, 3);
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		ShenQiActionResultType upResultType = (ShenQiActionResultType)data.UpResultType;
		switch (upResultType + 5)
		{
		case ShenQiActionResultType.Efail:
			break;
		case ShenQiActionResultType.Success:
			Super.HintMainText(Global.GetLang("金币不足！"), 10, 3);
			break;
		case ShenQiActionResultType.Next:
			Super.HintMainText(Global.GetLang("钻石不足！"), 10, 3);
			break;
		case ShenQiActionResultType.End:
			Super.HintMainText(Global.GetLang("神力精华不足！"), 10, 3);
			break;
		case (ShenQiActionResultType)4:
			break;
		case (ShenQiActionResultType)5:
			break;
		case (ShenQiActionResultType)6:
			flag2 = true;
			break;
		case (ShenQiActionResultType)7:
			flag = true;
			flag2 = true;
			break;
		case (ShenQiActionResultType)8:
			flag3 = true;
			flag2 = true;
			Super.HintMainText(Global.GetLang("已经达到最大级！"), 10, 3);
			break;
		default:
			if (upResultType != ShenQiActionResultType.None)
			{
			}
			break;
		}
		if (!flag2)
		{
			return;
		}
		int shenQiID = data.ShenQiID;
		if (this.RefreshJingHuaCallback != null)
		{
			this.RefreshJingHuaCallback(null, new DPSelectedItemEventArgs
			{
				ID = data.ShenLiJingHuaLeft
			});
		}
		this.ShowEffect(data.BurstType);
		ShenQiManager.RefreshCurrentShenQiDict(data, flag);
		this.RefreshUIData(this.GetCurrentData(), false);
		this.m_ShenQiDict[shenQiID] = ShenQiManager.GetCurrentShenQiDataByID(shenQiID);
		if (flag)
		{
			this.SetStatus(ShenQiContentPart.ShenQiStatus.ManJi);
			if (this.MoveRightNext())
			{
				this.RefreshUIData(this.GetCurrentData(), false);
				this.IsShowLeftArrow(true);
				this.Show3DModel(true);
				this.SetProgressValue(0f);
			}
			else
			{
				this.IsShowRightArrow(false);
			}
		}
		if (flag3)
		{
			this.SetStatus(ShenQiContentPart.ShenQiStatus.ManJi);
		}
		if (this.m_CurrentID == 1)
		{
			this.IsShowLeftArrow(false);
		}
		if (ShenQiManager.GetShenQiXMLDict().Count == this.m_CurrentID)
		{
			this.IsShowRightArrow(false);
		}
	}

	private void ShowEffect(int typeIndex)
	{
		if (this.burstEffects.Count == 0 || this.burstEffects.Count <= typeIndex || typeIndex == 0)
		{
			return;
		}
		if (this.burstEffects[typeIndex].activeSelf)
		{
			this.burstEffects[typeIndex].SetActive(false);
		}
		this.burstEffects[typeIndex].SetActive(true);
	}

	private void SetStatus(ShenQiContentPart.ShenQiStatus status)
	{
		switch (status + 1)
		{
		case ShenQiContentPart.ShenQiStatus.WeiManJi:
			this.SetActive(this.m_RightDownProperty, false);
			this.SetActive(this.m_WeiKaiQi, true);
			this.SetActive(this.m_ManJi, false);
			this.HideUpArrow(false);
			this.SetProgressValue(0f);
			this.m_ProgressBar.gameObject.SetActive(false);
			this.IsShowMaxValue(true);
			break;
		case ShenQiContentPart.ShenQiStatus.ManJi:
		{
			this.SetActive(this.m_RightDownProperty, true);
			this.SetActive(this.m_WeiKaiQi, false);
			this.SetActive(this.m_ManJi, false);
			this.HideUpArrow(true);
			this.m_ProgressBar.gameObject.SetActive(true);
			ShenQiXMLData currentShenQiDataByID = ShenQiManager.GetCurrentShenQiDataByID(this.m_CurrentID);
			float num = (float)(currentShenQiDataByID.LifeV + currentShenQiDataByID.AddDefense + currentShenQiDataByID.Toughness + currentShenQiDataByID.AddAttack);
			ShenQiXMLData shenQiDataByID = ShenQiManager.GetShenQiDataByID(this.m_CurrentID);
			float num2 = (float)(shenQiDataByID.LifeV + shenQiDataByID.AddDefense + shenQiDataByID.Toughness + shenQiDataByID.AddAttack);
			float progressValue = num / num2;
			this.SetProgressValue(progressValue);
			this.IsShowMaxValue(false);
			break;
		}
		case (ShenQiContentPart.ShenQiStatus)2:
			this.SetActive(this.m_RightDownProperty, false);
			this.SetActive(this.m_WeiKaiQi, false);
			this.SetActive(this.m_ManJi, true);
			this.HideUpArrow(false);
			this.SetProgressValue(1f);
			this.m_ProgressBar.gameObject.SetActive(true);
			this.IsShowMaxValue(false);
			break;
		}
	}

	private void HideUpArrow(bool isShow)
	{
		this.SetActive(this.m_LifeValueUpArrow.gameObject, isShow);
		this.SetActive(this.m_AttackValueUpArrow.gameObject, isShow);
		this.SetActive(this.m_DefendValueUpArrow.gameObject, isShow);
		this.SetActive(this.m_ToughnessValueUpArrow.gameObject, isShow);
		this.SetActive(this.m_LifeValueUpValue.gameObject, isShow);
		this.SetActive(this.m_AttackValueUpValue.gameObject, isShow);
		this.SetActive(this.m_DefendValueUpValue.gameObject, isShow);
		this.SetActive(this.m_ToughnessValueUpValue.gameObject, isShow);
	}

	private void IsShowMaxValue(bool isShow)
	{
		this.SetActive(this.m_MaxLifeValue.gameObject, isShow);
		this.SetActive(this.m_MaxAttackValue.gameObject, isShow);
		this.SetActive(this.m_MaxDefendValue.gameObject, isShow);
		this.SetActive(this.m_MaxToughnessValue.gameObject, isShow);
		if (isShow)
		{
			ShenQiXMLData shenQiDataByID = ShenQiManager.GetShenQiDataByID(this.m_CurrentID);
			this.m_MaxLifeValue.Text = string.Format(Global.GetLang("（{0}{1}）"), Global.GetLang("最大值："), shenQiDataByID.LifeV);
			this.m_MaxAttackValue.Text = string.Format(Global.GetLang("（{0}{1}）"), Global.GetLang("最大值："), shenQiDataByID.AddAttack);
			this.m_MaxDefendValue.Text = string.Format(Global.GetLang("（{0}{1}）"), Global.GetLang("最大值："), shenQiDataByID.AddDefense);
			this.m_MaxToughnessValue.Text = string.Format(Global.GetLang("（{0}{1}）"), Global.GetLang("最大值："), shenQiDataByID.Toughness);
		}
	}

	private void SetActive(GameObject ob, bool isOpen)
	{
		if (null != ob)
		{
			ob.SetActive(isOpen);
		}
	}

	private void SetProgressValue(float percent)
	{
		this.m_ProgressBar.ProgessText = percent.ToString("p");
		this.m_ProgressBar.Percent = (double)percent;
		this.progressBarTeXiaoBg.transform.localScale = new Vector3(this.progressBarTeXiaoBgValue.x + Mathf.Abs(this.progressBarTeXiaoBgValue.x * percent), this.progressBarTeXiaoBgValue.y, this.progressBarTeXiaoBgValue.z);
	}

	private void HelpWindow()
	{
		if (null == this.shenQiHelpWindow)
		{
			this.shenQiHelpWindow = U3DUtils.NEW<GChildWindow>();
			this.shenQiHelpWindow.ModalType = ChildWindowModalType.Translucent;
			this.shenQiHelpWindow.IsShowModal = true;
			Super.InitChildWindow(this.shenQiHelpWindow, "ShenQiHelpWindow");
			Super.GData.GlobalPlayZone.Children.Add(this.shenQiHelpWindow);
		}
		this.shenQiHelp = U3DUtils.NEW<ShenQiHelpWindow>();
		this.shenQiHelpWindow.Body.Add(this.shenQiHelp);
		this.shenQiHelp.CloseCallback = delegate(object s1, DPSelectedItemEventArgs args)
		{
			if (args.ID == 0 && null != this.shenQiHelp)
			{
				Object.Destroy(this.shenQiHelp.gameObject);
				this.shenQiHelp = null;
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.shenQiHelpWindow);
			}
		};
		this.shenQiHelpWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			if (null != this.shenQiHelp)
			{
				Object.Destroy(this.shenQiHelp.gameObject);
				this.shenQiHelp = null;
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.shenQiHelpWindow);
			}
			return true;
		};
	}

	protected override void OnDestroy()
	{
		this.m_ShenQiDict.Clear();
		base.StopCoroutine(this.DestroyTeXiao());
		base.StopCoroutine(this.WaitForSeconds());
		this.m_HelpBtn = null;
		this.m_LeftArrow = null;
		this.m_RightArrow = null;
		this.m_ProgressBar = null;
		this.m_PropertyTitle = null;
		this.m_LifeValue = null;
		this.m_LifeValueUpArrow = null;
		this.m_LifeValueUpValue = null;
		this.m_AttackValue = null;
		this.m_AttackValueUpArrow = null;
		this.m_AttackValueUpValue = null;
		this.m_DefendValue = null;
		this.m_DefendValueUpArrow = null;
		this.m_DefendValueUpValue = null;
		this.m_ToughnessValue = null;
		this.m_ToughnessValueUpArrow = null;
		this.m_ToughnessValueUpValue = null;
		this.m_RightDownProperty = null;
		this.m_TiShengTitle = null;
		this.m_PriceJinBi = null;
		this.m_PriceJingHua = null;
		this.m_CheckBox = null;
		this.m_BaoJiLabel = null;
		this.m_ZhuRuBtn = null;
		this.CloseCallback = null;
		this.shenQiHelp = null;
		this.shenQiHelpWindow = null;
		if (this.resourceLoader != null)
		{
			this.resourceLoader.Stop();
		}
	}

	public GButton m_HelpBtn;

	public GButton m_LeftArrow;

	public GButton m_RightArrow;

	public TextBlock m_Title;

	public GImgProgressBar m_ProgressBar;

	public Transform m_3DModelParent;

	public GameObject m_HelpWindow;

	public TextBlock m_PropertyTitle;

	public TextBlock m_Life;

	public TextBlock m_LifeValue;

	public UISprite m_LifeValueUpArrow;

	public TextBlock m_LifeValueUpValue;

	public TextBlock m_MaxLifeValue;

	public TextBlock m_Attack;

	public TextBlock m_AttackValue;

	public UISprite m_AttackValueUpArrow;

	public TextBlock m_AttackValueUpValue;

	public TextBlock m_MaxAttackValue;

	public TextBlock m_Defend;

	public TextBlock m_DefendValue;

	public UISprite m_DefendValueUpArrow;

	public TextBlock m_DefendValueUpValue;

	public TextBlock m_MaxDefendValue;

	public TextBlock m_Toughness;

	public TextBlock m_ToughnessValue;

	public UISprite m_ToughnessValueUpArrow;

	public TextBlock m_ToughnessValueUpValue;

	public TextBlock m_MaxToughnessValue;

	public GameObject m_RightDownProperty;

	public TextBlock m_TiShengTitle;

	public TextBlock m_PriceJinBiFont;

	public TextBlock m_PriceJinBi;

	public UISprite m_PriceJinBiSp;

	public TextBlock m_PriceJingHuaFont;

	public TextBlock m_PriceJingHua;

	public GCheckBox m_CheckBox;

	public TextBlock m_BaoJiLabel;

	public GButton m_ZhuRuBtn;

	public GameObject m_WeiKaiQi;

	public GameObject m_ManJi;

	public GButton m_DetailsBtn;

	public List<UISprite> listDaiBi = new List<UISprite>();

	public ShenQiHelpWindow shenQiHelp;

	public GChildWindow shenQiHelpWindow;

	private int m_CurrentID;

	private Dictionary<int, ShenQiXMLData> m_ShenQiDict;

	private int isUseDiamond;

	public List<GameObject> burstEffects = new List<GameObject>();

	private ShenQiXMLData m_TmpShenQiXMLData;

	private Vector3 progressBarTeXiaoBgValue = new Vector3(-418f, 32f, 0f);

	public UISprite progressBarTeXiaoBg;

	public DPSelectedItemEventHandler CloseCallback;

	public DPSelectedItemEventHandler RefreshJingHuaCallback;

	private float waitTime = 0.5f;

	private bool isClickDown;

	private GameObject mTeXiao;

	private Modal3DShow medalModal;

	private ResourceResLoader resourceLoader;

	private enum ShenQiStatus
	{
		WeiKaiQi = -1,
		WeiManJi,
		ManJi
	}
}
