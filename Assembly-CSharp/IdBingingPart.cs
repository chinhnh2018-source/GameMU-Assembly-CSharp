using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class IdBingingPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
		this.Init();
	}

	private void InitPrefabText()
	{
		this.m_TitleLabel.text = Global.GetLang("账号绑定");
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.m_Collection = this.m_ListBox.ItemsSource;
		this.m_BingingBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_BtnType == 0)
			{
				IOSSDKPlugin.ShowUserCenter();
			}
			if (this.ChildWindowClose != null)
			{
				this.ChildWindowClose(null, null);
			}
		};
	}

	private void Init()
	{
		Global.DetectionIDBinding();
		this.BtnType = ((!Global.IsBinding) ? 0 : 1);
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("App_BindPhoneAward", true);
		if (!string.IsNullOrEmpty(systemParamByName))
		{
			string[] array = systemParamByName.Split(new char[]
			{
				'|'
			});
			this.m_DRPanel = this.m_ListBox.Parent.GetComponent<UIDraggablePanel>();
			foreach (string text in array)
			{
				string[] str_Award = text.Split(new char[]
				{
					','
				});
				this.initGood(str_Award, this.m_DRPanel);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"fuck哦!越南产品，请检查App_BindPhoneAward"
			});
		}
	}

	private void initGood(string[] str_Award, UIDraggablePanel dragpanel)
	{
		int num = Convert.ToInt32(str_Award[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			GoodsData goodsData = new GoodsData();
			goodsData.GoodsID = num;
			goodsData.GCount = Convert.ToInt32(str_Award[1]);
			goodsData.Binding = Convert.ToInt32(str_Award[2]);
			goodsData.Forge_level = Convert.ToInt32(str_Award[3]);
			goodsData.AppendPropLev = Convert.ToInt32(str_Award[4]);
			goodsData.Lucky = Convert.ToInt32(str_Award[5]);
			goodsData.ExcellenceInfo = Convert.ToInt32(str_Award[6]);
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.ItemCode = num;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this.m_ListBox.gameObject, ggoodIcon.gameObject, true);
			this.m_Collection.Add(ggoodIcon);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, Global.CanUseGoods(num, false, true), IconTextTypes.Qianghua);
			ggoodIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.BtnClick);
			BoxCollider component2 = ggoodIcon.transform.GetComponent<BoxCollider>();
			ggoodIcon.transform.gameObject.AddComponent<UIDragPanelContents>();
			component2.center = new Vector3(0f, 0f, -1f);
			UIDragPanelContents panels = ggoodIcon.GetComponent<UIDragPanelContents>();
			panels.draggablePanel = dragpanel;
			panels.draggablePanel.onDragIng = delegate()
			{
				float y = panels.draggablePanel.currentMomentum.y;
				if (y != 0f)
				{
					this.BtnCanCilck = false;
				}
			};
			panels.draggablePanel.onDragFinished = delegate()
			{
				this.StartCoroutine<bool>(this.ChangeBtnCanCilckState());
			};
		}
	}

	private void BtnClick(object sender, MouseEvent e)
	{
		if (this.BtnCanCilck)
		{
			this.ShowGoodsTip(sender);
		}
	}

	private IEnumerator ChangeBtnCanCilckState()
	{
		yield return new WaitForSeconds(0.2f);
		this.BtnCanCilck = true;
		yield break;
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
	}

	public void ErrorLog(EUserActivateState state)
	{
		string text = string.Empty;
		switch (state + 6)
		{
		case EUserActivateState.Default:
			text = Global.GetLang("背包位置不足");
			break;
		case EUserActivateState.Success:
			text = Global.GetLang("奖励错误");
			break;
		case (EUserActivateState)2:
			text = Global.GetLang("领取失败");
			break;
		case (EUserActivateState)3:
			text = Global.GetLang("已经领取");
			break;
		case (EUserActivateState)4:
			text = Global.GetLang("平台错误（仅ios）");
			break;
		case (EUserActivateState)5:
			text = Global.GetLang("校验错误");
			break;
		case (EUserActivateState)7:
			text = Global.GetLang("成功，已领取");
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			Super.HintMainText(Global.GetLang(text), 10, 3);
		}
	}

	public string BindIngStr
	{
		set
		{
			if (!string.IsNullOrEmpty(value) && null != this.m_BingingBtn.Label)
			{
				this.m_BingingBtn.Label.text = value;
			}
		}
	}

	public string ContentStr
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				this.m_Content.text = value;
			}
		}
	}

	public int BtnType
	{
		set
		{
			this.m_BtnType = value;
			if (this.m_BtnType == 0)
			{
				this.m_BingingBtn.Label.text = Global.GetLang("立即绑定");
			}
			else
			{
				this.m_BingingBtn.Label.text = Global.GetLang("领取奖励");
			}
		}
	}

	public GButton m_CloseBtn;

	public UILabel m_TitleLabel;

	public UILabel m_Content;

	public ListBox m_ListBox;

	public GButton m_BingingBtn;

	private ObservableCollection m_Collection;

	private UIDraggablePanel m_DRPanel;

	private int m_BtnType;

	private bool BtnCanCilck = true;
}
