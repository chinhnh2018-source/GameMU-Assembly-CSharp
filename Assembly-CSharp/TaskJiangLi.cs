using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class TaskJiangLi : UserControl
{
	public TaskJiangLi()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.ItemCollection2 = this.listBox2.ItemsSource;
		this.Container.Children.Add(this.thisPanel);
		this.thisPanel.Orientation = global::Layout.Vertical;
		this.thisPanel.Width = 262.0;
		this.thisPanel.Height = 116.0;
		this.thisPanel.Children.Add(this.listBox);
		this.listBox.Width = 262.0;
		this.listBox.Height = 47.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 3.0, 0.0);
		this.thisPanel.Children.Add(this.listBox2);
		this.listBox2.Width = 262.0;
		this.listBox2.Height = 67.0;
		this.listBox2.VerticalIcon = true;
		this.listBox2.ItemMargin = new Thickness(2.0, 0.0, 0.0, 0.0);
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public ObservableCollection ItemCollection2
	{
		get
		{
			return this._ItemCollection2;
		}
		set
		{
			this._ItemCollection2 = value;
		}
	}

	public int TaskID
	{
		get
		{
			return this._TaskID;
		}
		set
		{
			this._TaskID = value;
			TaskData taskDataByTaskID = Super.GetTaskDataByTaskID(this._TaskID);
			if (taskDataByTaskID != null)
			{
				this.SetTaskJiangLiDate(taskDataByTaskID.TaskAwards);
			}
			else
			{
				this.SetTaskJiangLiDate(null);
			}
		}
	}

	public TaskAwardsData TaskAwardsData
	{
		set
		{
			this.SetTaskJiangLiDate(value);
		}
	}

	private void SetTaskJiangLiDate(TaskAwardsData taskAwards)
	{
		this.ItemCollection.Clear();
		this.ItemCollection2.Clear();
		this.GetTaskAwards(taskAwards);
		Super.GData.ViewTaskInfoGoodsDataList = this.ParseAwards(taskAwards);
		if (Super.GData.ViewTaskInfoGoodsDataList != null && Super.GData.ViewTaskInfoGoodsDataList.Count > 0)
		{
			this.listBox.Visibility = true;
			for (int i = 0; i < Super.GData.ViewTaskInfoGoodsDataList.Count; i++)
			{
				CompoundItem compoundItem = U3DUtils.NEW<CompoundItem>();
				compoundItem.Width = 38.0;
				compoundItem.Height = 38.0;
				compoundItem.GoodsImgBacks = this.backImg;
				compoundItem.GoodsImgs = this.GetIcon(Super.GData.ViewTaskInfoGoodsDataList[i]);
				compoundItem.SetGoodsImgBackXY(3, 5);
				compoundItem.SetGoodsImgXY(6, 8);
				Canvas.SetZIndex(compoundItem.GoodsImg, 100.0);
				this.ItemCollection.AddNoUpdate(compoundItem);
			}
			this.ItemCollection.DelayUpdate();
		}
		else
		{
			this.listBox.Visibility = false;
		}
	}

	private void GetTaskAwards(TaskAwardsData taskAwards)
	{
		if (taskAwards == null)
		{
			return;
		}
		if (taskAwards.Experienceaward > 0L)
		{
			this.SetCanvas(taskAwards.Experienceaward.ToString(), Global.GetLang("奖励经验："));
		}
		if (taskAwards.Moneyaward > 0)
		{
			this.SetCanvas(taskAwards.Moneyaward.ToString(), Global.GetLang("绑定金币："));
		}
		if (taskAwards.YinLiangaward > 0)
		{
			this.SetCanvas(taskAwards.YinLiangaward.ToString(), Global.GetLang("绑定银两："));
		}
		if (taskAwards.BindYuanBaoaward > 0)
		{
			this.SetCanvas(taskAwards.BindYuanBaoaward.ToString(), Global.GetLang("绑定钻石："));
		}
		if (taskAwards.ZhenQiaward > 0)
		{
			this.SetCanvas(taskAwards.ZhenQiaward.ToString(), Global.GetLang("奖励真气："));
		}
		if (taskAwards.LieShaaward > 0)
		{
			this.SetCanvas(taskAwards.LieShaaward.ToString(), Global.GetLang("猎杀值："));
		}
		if (taskAwards.WuXingaward > 0)
		{
			this.SetCanvas(taskAwards.WuXingaward.ToString(), Global.GetLang("奖励悟性："));
		}
		if (taskAwards.JunGongaward > 0)
		{
			this.SetCanvas(taskAwards.JunGongaward.ToString(), Global.GetLang("奖励军功："));
		}
		if (taskAwards.RongYuaward > 0)
		{
			this.SetCanvas(taskAwards.RongYuaward.ToString(), Global.GetLang("奖励荣耀："));
		}
		this.ItemCollection2.DelayUpdate();
	}

	private List<GoodsData> ParseAwards(TaskAwardsData taskAwards)
	{
		if (taskAwards == null)
		{
			return null;
		}
		List<GoodsData> list = new List<GoodsData>();
		if (taskAwards.TaskawardList != null)
		{
			for (int i = 0; i < taskAwards.TaskawardList.Count; i++)
			{
				this.ParseAwardsItem(taskAwards.TaskawardList[i], list, true, true);
			}
		}
		if (taskAwards.OtherTaskawardList != null)
		{
			for (int j = 0; j < taskAwards.OtherTaskawardList.Count; j++)
			{
				this.ParseAwardsItem(taskAwards.OtherTaskawardList[j], list, false, false);
			}
		}
		return list;
	}

	private GGoodIcon GetIcon(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty);
		if (goodsImageURLFromIconCode == null)
		{
			return null;
		}
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 32.0;
		ggoodIcon.Height = 32.0;
		ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		ggoodIcon.TipType = 1;
		ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			goodsData.GoodsID,
			0,
			goodsData.Id,
			15
		});
		ggoodIcon.ItemCategory = Global.GetXElementAttributeInt(this.xmlItem, "Categoriy");
		ggoodIcon.ItemCode = goodsData.GoodsID;
		ggoodIcon.ItemObject = goodsData;
		ggoodIcon.BoxTypes = -1;
		Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
		return ggoodIcon;
	}

	private void ParseAwardsItem(AwardsItemData awardsItemData, List<GoodsData> goodsDataList, bool occupation, bool sex)
	{
		if (awardsItemData == null)
		{
			return;
		}
		GoodsData goodsData = null;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(awardsItemData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			bool flag = true;
			if (occupation && goodsXmlNodeByID.ToOccupation >= 0 && (1 << Global.Data.roleData.Occupation & goodsXmlNodeByID.ToOccupation) == 0)
			{
				flag = false;
			}
			if (sex && goodsXmlNodeByID.ToSex >= 0 && Global.Data.roleData.RoleSex != goodsXmlNodeByID.ToSex)
			{
				flag = false;
			}
			if (flag)
			{
				goodsData = new GoodsData();
				goodsData.Id = goodsDataList.Count + 1;
				goodsData.GoodsID = awardsItemData.GoodsID;
				goodsData.Using = 0;
				goodsData.Forge_level = awardsItemData.Level;
				goodsData.Starttime = "1900-01-01 12:00:00";
				goodsData.Endtime = awardsItemData.EndTime;
				goodsData.Site = 0;
				goodsData.Quality = awardsItemData.Quality;
				goodsData.Props = string.Empty;
				goodsData.GCount = awardsItemData.GoodsNum;
				goodsData.Binding = awardsItemData.Binding;
				goodsData.Jewellist = string.Empty;
				goodsData.BagIndex = 0;
				goodsData.AddPropIndex = 0;
				goodsData.BornIndex = 0;
				goodsData.Lucky = 0;
				goodsData.Strong = 0;
				goodsData.ExcellenceInfo = awardsItemData.ExcellencePorpValue;
				goodsData.AppendPropLev = awardsItemData.IsHaveLuckyProp;
			}
		}
		if (goodsData != null)
		{
			goodsDataList.Add(goodsData);
		}
	}

	private void SetCanvas(string sText, string sImgName)
	{
		this.JiangLItem = U3DUtils.NEW<TaskJingLiItem>();
		this.JiangLItem.Text = Global.GetColorStringForHtmlText(new object[]
		{
			"#ffaddcae",
			sImgName,
			"#ffffff37",
			sText
		});
		this.ItemCollection2.AddNoUpdate(this.JiangLItem);
	}

	private StackPanel thisPanel = new StackPanel();

	private BitmapData backImg = Global.GetGameResImage("Images/Plate/rm_listItem.png");

	private ListBox listBox = new ListBox();

	private ListBox listBox2 = new ListBox();

	private XElement xmlItem;

	private TaskJingLiItem JiangLItem;

	private ObservableCollection _ItemCollection;

	private ObservableCollection _ItemCollection2;

	private int _TaskID;
}
