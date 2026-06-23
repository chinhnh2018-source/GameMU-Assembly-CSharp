using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ChengJiuChengHaoPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_NextLevelName.Text = string.Empty;
		this.m_ConstNextName.Text = string.Empty;
		this.m_ConstCurrentName.Text = string.Empty;
		this.m_ConstDes.Text = string.Empty;
		this.m_LabelNoLevel.Text = string.Empty;
		this.m_BtnUpLevel.Text = string.Empty;
		this.m_ConstCurrentName.Text = Global.GetLang("当前称号");
		this.m_ConstNextName.Text = Global.GetLang("下一级称号");
		this.m_ConstDes.Text = Global.GetLang("获得途径：阵营战、PK之王活动获得");
		this.m_LabelNoLevel.Text = Global.GetLang("无");
		this.m_BtnUpLevel.Text = Global.GetLang("提升");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.m_ItemOBC = this.m_ListItems.ItemsSource;
		this.m_BtnUpLevel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.NeedGoodsID < 0)
			{
				GameInstance.Game.SpriteUpdateChengJiuLevel();
			}
			else if (this.IsGoodsEnough())
			{
				GameInstance.Game.SpriteUpdateChengJiuLevel();
			}
			else
			{
				Super.HintMainText(Global.GetLang("物品不足！"), 10, 3);
			}
		};
		this.m_BtnUpLevel.isEnabled = false;
		this.ChengJiuGoodsIDArr = ConfigSystemParam.GetSystemParamIntArrayByName("ChengJiuBufferGoodsIDs", ',');
		this.InitItemList();
		this.RefreshUI();
		this.CheckPos = true;
	}

	public new void Update()
	{
		if (this.CheckPos)
		{
			if (Global.GetChengJiuLevel(0) > 0)
			{
				float num = 216f;
				float num2 = -(num * (float)(Global.GetChengJiuLevel(0) - 1));
				this.DragPanel.MoveRelativeEx(new Vector3(num2, 0f, 0f));
			}
			this.CheckPos = false;
		}
	}

	private void InitItemList()
	{
		this.m_ItemOBC.Clear();
		this.ItemList.Clear();
		for (int i = 0; i < this.ChengJiuGoodsIDArr.Length; i++)
		{
			ChengJiuLevelItem chengJiuLevelItem = U3DUtils.NEW<ChengJiuLevelItem>();
			this.m_ItemOBC.Add(chengJiuLevelItem);
			this.ItemList.Add(chengJiuLevelItem);
			chengJiuLevelItem.refreshUI(this.ChengJiuGoodsIDArr[i], i);
		}
	}

	public void RefreshItemEnableState()
	{
		int count = this.ItemList.Count;
		for (int i = 0; i < count; i++)
		{
			this.ItemList[i].RefreshEnable();
		}
	}

	public void RefreshUI()
	{
		if (Global.GetChengJiuLevel(0) > 0)
		{
			this.m_LabelNoLevel.gameObject.SetActive(false);
			int num = this.ChengJiuGoodsIDArr[Global.GetChengJiuLevel(0) - 1];
			this.m_TextureCurrentLevel.URL = "NetImages/GameRes/Images/ChengJiuLevel/cj_" + num + ".png";
		}
		else
		{
			this.m_LabelNoLevel.gameObject.SetActive(true);
		}
		int chengJiuLevel = Global.GetChengJiuLevel(0);
		if (chengJiuLevel < this.ChengJiuGoodsIDArr.Length)
		{
			this.m_NextLevelName.Text = Global.GetGoodsNameByID(this.ChengJiuGoodsIDArr[chengJiuLevel], false);
			XElement xelement = Global.GetXElement(Global.GetGameResXml("config/ChengJiuBuff.xml"), "ChengJiu", "ID", (chengJiuLevel + 1).ToString());
			if (xelement != null)
			{
				this.AddGoods(xelement);
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ChengJiu");
				if (Global.Data.ChengJiuData.ChengJiuPoints >= (long)xelementAttributeInt)
				{
					this.m_BtnUpLevel.isEnabled = true;
					this.m_ProgessBar.Percent = 1.0;
				}
				else
				{
					this.m_BtnUpLevel.isEnabled = false;
					float num2 = (float)Global.Data.ChengJiuData.ChengJiuPoints / (float)xelementAttributeInt;
					this.m_ProgessBar.Percent = (double)num2;
				}
				this.m_TextJinDu.Text = Global.Data.ChengJiuData.ChengJiuPoints + "/" + xelementAttributeInt;
			}
		}
		else
		{
			this.GoodIconParent.Clear();
			this.m_NextLevelName.Text = Global.GetLang("无");
			this.m_BtnUpLevel.isEnabled = false;
			this.m_ProgessBar.Percent = 1.0;
		}
	}

	private void AddGoods(XElement chengJiuXml)
	{
		string xelementAttributeStr = Global.GetXElementAttributeStr(chengJiuXml, "NeedGoods");
		if (string.IsNullOrEmpty(xelementAttributeStr))
		{
			this.NeedGoodsID = -1;
			return;
		}
		this.GoodIconParent.Clear();
		string[] array = xelementAttributeStr.Split(new char[]
		{
			','
		});
		this.NeedGoodsID = Global.SafeConvertToInt32(array[0]);
		this.NeedGoodsCount = Global.SafeConvertToInt32(array[1]);
		GoodsData dummyGoodsData = Global.GetDummyGoodsData(this.NeedGoodsID);
		GGoodIcon ggoodIcon = UIHelper.AddGoodsIcon(this.GoodIconParent, dummyGoodsData, null, false);
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(this.NeedGoodsID);
		ggoodIcon.SecondText.Text = string.Format("{0}/{1}", totalGoodsCountByID, this.NeedGoodsCount);
		if (totalGoodsCountByID >= this.NeedGoodsCount)
		{
			ggoodIcon.SecondText.textColor = 16777215U;
		}
		else
		{
			ggoodIcon.SecondText.textColor = 16711680U;
		}
	}

	private bool IsGoodsEnough()
	{
		bool result = false;
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(this.NeedGoodsID);
		if (totalGoodsCountByID >= this.NeedGoodsCount)
		{
			result = true;
		}
		return result;
	}

	public GButton m_BtnUpLevel;

	public TextBlock m_NextLevelName;

	public ShowNetImage m_TextureCurrentLevel;

	public GImgProgressBar m_ProgessBar;

	public TextBlock m_TextJinDu;

	public TextBlock m_LabelNoLevel;

	public TextBlock m_ConstCurrentName;

	public TextBlock m_ConstNextName;

	public TextBlock m_ConstDes;

	private ObservableCollection m_ItemOBC;

	public ListBox m_ListItems = new ListBox();

	public List<ChengJiuLevelItem> ItemList = new List<ChengJiuLevelItem>();

	public DPSelectedItemEventHandler DPSelectedItem;

	private int[] ChengJiuGoodsIDArr;

	public UIDraggablePanel DragPanel;

	private bool CheckPos;

	public SpriteSL GoodIconParent;

	private int NeedGoodsID;

	private int NeedGoodsCount;
}
