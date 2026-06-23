using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class Adenda : UserControl
{
	private void InitTextInPrefabs()
	{
		if (null != this.strengthenBtn)
		{
			this.strengthenBtn.Text = Global.GetLang("提升");
		}
	}

	protected override void InitializeComponent()
	{
		this.hornourTitle.MakePixelPerfect();
		this.hornourTitle.transform.localPosition = new Vector3(-170f, 440f, -1f);
		this.InitTextInPrefabs();
		this.extraBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ExtraProperties();
		};
		this.strengthenBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Super.MessageBoxIsHint[4] == 0)
			{
				DPSelectedItemEventHandler handler = delegate(object sender, DPSelectedItemEventArgs args)
				{
					if (args.ID == 0)
					{
						this.DoUpHornourProperties();
					}
					else
					{
						Super.MessageBoxIsHint[4] = 0;
					}
				};
				if (!this.adendaPropertyController.diamondLabel.text.Equals(Global.GetLang("免费")))
				{
					Super.ZuanShiShowMessageBox(Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("本次操作需要花费       {0} ,  确认要执行该操作吗？"), this.adendaPropertyController.diamondLabel.text)
					}), 2, handler, MessBoxIsHintTypes.HuaFeiZuanShiNeedHint, -15f, "ShengWangYinJi", Global.Data.adendaData.Diamond);
				}
				else
				{
					this.DoUpHornourProperties();
				}
			}
			else
			{
				this.DoUpHornourProperties();
			}
		};
		this.hornourItemController.DPSelectedItem = new DPSelectedItemEventHandler(this.DoSelectedHornourItem);
		this.adendaPropertyController.Init();
		this.hornourItemController.Init();
		this.closeBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (this.closeAdendaViewEventHandler != null)
			{
				this.closeAdendaViewEventHandler(this, new DPSelectedItemEventArgs
				{
					Type = -10
				});
			}
		};
		this.previousLevelBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DoSelectedNextHornorItem(this, new DPSelectedItemEventArgs
			{
				ID = 100
			});
		};
		this.nextLevelBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DoSelectedNextHornorItem(this, new DPSelectedItemEventArgs
			{
				ID = 101
			});
		};
	}

	public void RefreshView(bool upLevepAction = false)
	{
		this.DealWithServerResult(upLevepAction);
	}

	public void DealWithServerResult(bool upLevepAction = false)
	{
		if (Global.Data.adendaData == null)
		{
			Super.HideNetWaiting();
			Super.HintMainText(StringUtil.substitute(Global.GetLang("功能未开放"), new object[0]), 10, 3);
			return;
		}
		if (!upLevepAction)
		{
			Super.HideNetWaiting();
			this.medalIndex = Mathf.Min(6, Global.Data.adendaData.MedalID);
			this.DoRefreshViewOnLevelChanged(false);
			return;
		}
		int upResultType = Global.Data.adendaData.UpResultType;
		switch (upResultType + 4)
		{
		case 1:
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			break;
		case 2:
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedShengWang, null, string.Empty, string.Empty);
			break;
		case 3:
			Super.HintMainText(StringUtil.substitute(Global.GetLang("功能未开放"), new object[0]), 10, 3);
			break;
		case 4:
			Super.HintMainText(StringUtil.substitute(Global.GetLang("提升失败"), new object[0]), 10, 3);
			break;
		case 5:
			this.RefreshValueUpEffect();
			this.RefreshAvailableDiamondField();
			this.RefreshAvailablePrestigeField();
			break;
		case 6:
			this.medalIndex = Global.Data.adendaData.MedalID;
			this.DoRefreshViewOnLevelChanged(true);
			break;
		case 7:
			this.DoRefreshViewOnLevelChanged(false);
			break;
		}
	}

	private void DoRefreshViewOnLevelChanged(bool isLevepup = false)
	{
		if (!isLevepup)
		{
			this.hornourItemController.SetHornourItemsStatus();
		}
		else
		{
			this.lockWhenAnimation = true;
			this.CreateLevelupAnimation(this.hornourItemController.GetLevelupAnimationPosition(this.medalIndex));
			base.StopCoroutine("AddGoodListIcon");
			base.StartCoroutine<bool>(this.AddGoodListIcon());
		}
		this.hornourItemController.RefreshLevelProgress(!isLevepup);
		this.DoSelectHornourItem(this.medalIndex);
		this.RefreshNewProperties((!isLevepup) ? this.medalIndex : (this.medalIndex - 1));
		this.RefreshAvailableDiamondField();
		this.RefreshAvailablePrestigeField();
	}

	private void CreateLevelupAnimation(GameObject parent)
	{
		if (null == this.animation)
		{
			GameObject original = Resources.Load("UITeXiao/JunXian/HuiZhangKaiQi_effect") as GameObject;
			this.animation = (SpawnManager.Instantiate(original) as GameObject);
			this.animation.transform.parent = parent.transform;
			this.animation.transform.localPosition = new Vector3(0f, 60f, -1.2f);
			this.animation.transform.localScale = Vector3.one;
		}
		else
		{
			this.animation.transform.parent = parent.transform;
			this.animation.transform.localPosition = new Vector3(0f, 60f, -1.2f);
			this.animation.transform.localScale = Vector3.one;
		}
		this.animation.SetActive(false);
		this.animation.SetActive(true);
	}

	public IEnumerator AddGoodListIcon()
	{
		yield return new WaitForSeconds(2.2f);
		this.lockWhenAnimation = false;
		this.animation.SetActive(false);
		this.adendaPropertyController.SetStarVisible(false);
		this.hornourItemController.SetHornourItemsStatus();
		this.RefreshNewProperties(this.medalIndex);
		yield break;
	}

	public void DoSelectedNextHornorItem(object sender, DPSelectedItemEventArgs args)
	{
		int num = this.medalIndex;
		if (args.ID == 100)
		{
			if (num <= 1)
			{
				return;
			}
			num--;
		}
		else if (args.ID == 101)
		{
			if (num >= 6 || num >= Global.Data.adendaData.MedalID)
			{
				return;
			}
			num++;
		}
		this.DoSelectedHornourItem(sender, new DPSelectedItemEventArgs
		{
			ID = num
		});
	}

	public void DoSelectedHornourItem(object sender, DPSelectedItemEventArgs args)
	{
		int id = args.ID;
		if (this.medalIndex == id)
		{
			return;
		}
		if (!this.hornourItemController.ItemActive(id))
		{
			return;
		}
		this.DoSelectHornourItem(id);
		this.medalIndex = id;
	}

	private void DoSelectHornourItem(int select_index)
	{
		if (null != this.adendaPropertyController)
		{
			this.adendaPropertyController.SetStarVisible(false);
			this.adendaPropertyController.SetThumbnail(select_index);
		}
		this.hornourItemController.SetItemSelected(this.medalIndex, select_index);
		this.SetRelatedLevelStatus(select_index);
		this.RefreshNewProperties(select_index);
		this.SetHornourItemTitle(select_index);
		this.ShowMedalModelByID(select_index);
	}

	public void RefreshNewProperties(int id = 1)
	{
		if (Global.Data.adendaData.MedalID > id)
		{
			this.strengthenBtn.isEnabled = false;
		}
		else
		{
			this.strengthenBtn.isEnabled = true;
		}
		this.adendaPropertyController.RefreshNewPropertiesByMedalID(id);
	}

	public void GetAdendaInfo()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.GetAdendaInfo();
	}

	private void ExtraProperties()
	{
		if (this.extraPropertyController.gameObject.activeSelf)
		{
			return;
		}
		this.extraPropertyController.gameObject.SetActive(true);
		this.extraPropertyController.SetLevelupExtraProperty();
	}

	private void DoUpHornourProperties()
	{
		if (Global.Data.adendaData == null)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("功能未开放"), new object[0]), 10, 3);
			return;
		}
		int num = 0;
		if (Global.Data.roleData != null && Global.Data.roleData.RoleCommonUseIntPamams != null && Global.Data.roleData.RoleCommonUseIntPamams.Count > 18)
		{
			num = Global.Data.roleData.RoleCommonUseIntPamams[18];
		}
		if (Global.Data.adendaData.Prestige > num)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedShengWang, null, string.Empty, string.Empty);
			return;
		}
		if (Global.Data.adendaData.Diamond > Global.Data.roleData.UserMoney && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ShengWangYinJi", Global.Data.adendaData.Diamond, false))
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		}
		GameInstance.Game.UpAdenda(this.medalIndex);
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

	private void ShowMedalModelByID(int id)
	{
		XElement xelementByID = this.hornourItemController.GetXElementByID(id);
		int modelID = 0;
		if (xelementByID != null)
		{
			modelID = Global.GetXElementAttributeInt(xelementByID, "ModID");
		}
		if (null != this.modalHelper)
		{
			if (this.resourceLoader != null)
			{
				this.resourceLoader.Stop();
			}
			this.resourceLoader = this.modalHelper.ShowMedalModalByID(modelID, 1f);
		}
	}

	public void RefreshValueUpEffect()
	{
		if (this.lockWhenAnimation)
		{
			return;
		}
		this.adendaPropertyController.RefreshNewPropertiesByMedalID(this.medalIndex);
		if (Global.Data.adendaData.BurstType == 1)
		{
			this.adendaPropertyController.PlayBurstEffectByType(1);
		}
		else if (Global.Data.adendaData.BurstType == 2)
		{
			this.adendaPropertyController.PlayBurstEffectByType(2);
		}
		else if (Global.Data.adendaData.BurstType == 0)
		{
			this.adendaPropertyController.PlayBurstEffectByType(0);
		}
	}

	public void RefreshAvailablePrestigeField()
	{
		if (null != this.adendaPropertyController)
		{
			this.adendaPropertyController.RefreshAvailablePrestigeField();
		}
	}

	public void RefreshAvailableDiamondField()
	{
		if (null != this.adendaPropertyController)
		{
			this.adendaPropertyController.RefreshAvailableDiamondField();
		}
	}

	private void SetHornourItemTitle(int index)
	{
		if (index < 1 || index > 6)
		{
			return;
		}
		if (null != this.hornourTitle)
		{
			this.hornourTitle.spriteName = "hornor_glory_" + index;
			this.hornourTitle.MakePixelPerfect();
			this.hornourTitle.transform.localPosition = new Vector3(-170f, 440f, -1f);
		}
	}

	private void SetRelatedLevelStatus(int select_index)
	{
		UIControlStatus control_status;
		if (select_index >= Global.Data.adendaData.MedalID)
		{
			control_status = UIControlStatus.UIControlStatus_Disabled;
		}
		else if (select_index <= 1 || select_index >= 6)
		{
			control_status = UIControlStatus.UIControlStauts_Hidden;
		}
		else
		{
			control_status = UIControlStatus.UIControlStatus_Normal;
		}
		this.SetRelatedLevelStatus((this.medalIndex <= select_index) ? UISlideAction.UISlideAction_Right : UISlideAction.UISlideAction_Left, control_status);
		this.SetPagingEnabled(select_index);
	}

	private void SetPagingEnabled(int select_index)
	{
		int medalID = Global.Data.adendaData.MedalID;
		if (this.medalIndex <= select_index)
		{
			this.leftArrow.gameObject.SetActive(1 != medalID);
			this.rightArrow.gameObject.SetActive(select_index != 6 && select_index <= medalID);
		}
	}

	private void SetRelatedLevelStatus(UISlideAction slide_action, UIControlStatus control_status)
	{
		if (slide_action != UISlideAction.UISlideAction_Left)
		{
			if (slide_action == UISlideAction.UISlideAction_Right)
			{
				this.rightArrow.spriteName = "right_arrow" + this.UIControlStatusToString(control_status);
				this.leftArrow.spriteName = "left_arrow_normal";
				this.rightArrow.gameObject.SetActive(control_status != UIControlStatus.UIControlStauts_Hidden);
				this.leftArrow.gameObject.SetActive(true);
			}
		}
		else
		{
			this.leftArrow.spriteName = "left_arrow" + this.UIControlStatusToString(control_status);
			this.rightArrow.spriteName = "right_arrow_normal";
			this.leftArrow.gameObject.SetActive(control_status != UIControlStatus.UIControlStauts_Hidden);
			this.rightArrow.gameObject.SetActive(true);
		}
	}

	private string UIControlStatusToString(UIControlStatus control_status)
	{
		string result = "_normal";
		switch (control_status)
		{
		case UIControlStatus.UIControlStatus_Normal:
			result = "_normal";
			break;
		case UIControlStatus.UIControlStatus_Disabled:
			result = "_disabled";
			break;
		case UIControlStatus.UIControlStauts_Hidden:
			result = "_hidden";
			break;
		}
		return result;
	}

	private const string levelupEffectPath = "UITeXiao/JunXian/HuiZhangKaiQi_effect";

	public AdendaPropertyViewController adendaPropertyController;

	public HornourItemController hornourItemController;

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler closeAdendaViewEventHandler;

	public GButton extraBtn;

	public GButton strengthenBtn;

	public GButton closeBtn;

	public GButton previousLevelBtn;

	public GButton nextLevelBtn;

	public UISprite leftArrow;

	public UISprite rightArrow;

	public UISprite hornourTitle;

	public AdendaLevelupExtraPropertyController extraPropertyController;

	public AdendaModalHelper modalHelper;

	private int medalIndex;

	private GameObject animation;

	private bool lockWhenAnimation;

	private ResourceResLoader resourceLoader;
}
