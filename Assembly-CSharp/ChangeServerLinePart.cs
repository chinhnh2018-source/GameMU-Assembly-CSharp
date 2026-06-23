using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class ChangeServerLinePart : UserControl
{
	public ChangeServerLinePart()
	{
		this.thisCtrl = this;
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 213.0;
		this.listBox.Height = 160.0;
		Canvas.SetLeft(this.listBox, 20);
		Canvas.SetTop(this.listBox, 37);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public GTabControl tc
	{
		get
		{
			return this._tc;
		}
		set
		{
			this._tc = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public void InitPartData()
	{
	}

	public void ResetGetNewData()
	{
		this.FirstGetNewData = true;
	}

	public void GetNewData()
	{
		if (!this.FirstGetNewData)
		{
			return;
		}
		this.FirstGetNewData = false;
		this.ShowLineListBox(-1);
	}

	private string GetLineID()
	{
		if (Global.CurrentListData == null)
		{
			return string.Empty;
		}
		if (Global.CurrentListData.LineID <= 0 || Global.CurrentListData.LineID >= 11)
		{
			return string.Empty;
		}
		return this.LineNames[Global.CurrentListData.LineID - 1];
	}

	private void ShowLineListBox(int oldLineID = -1)
	{
		int oldSelectedIndex = 0;
		this.SelectedLineListItem = null;
		this.ItemCollection.Clear();
		if (Global.LineDataList == null)
		{
			return;
		}
		string lineID = this.GetLineID();
		int selectedIndex = 0;
		for (int i = 0; i < Global.LineDataList.Count; i++)
		{
			if (Global.LineDataList[i].LineID > 0 && Global.LineDataList[i].LineID < 11)
			{
				ChangeServerLineItem changeServerLineItem = U3DUtils.NEW<ChangeServerLineItem>();
				changeServerLineItem.BodyWidth = 213.0;
				changeServerLineItem.BodyHeight = 20.0;
				changeServerLineItem.Width = 213.0;
				changeServerLineItem.Height = 20.0;
				changeServerLineItem.Tag = Global.LineDataList[i];
				changeServerLineItem.Tip = Global.GetLang("线路依照在线人数排序\n排序越靠后的线路会越拥挤\n我们推荐您进入较为顺畅的线路");
				changeServerLineItem.LineTextBlock.TextColor = Super.GetLineDataBrush(Global.LineDataList[i]);
				changeServerLineItem.LineTextBlock.Text = StringUtil.substitute(Global.GetLang("{0}线（{1}）"), new object[]
				{
					this.LineNames[Global.LineDataList[i].LineID - 1],
					Super.GetLineDataText(Global.LineDataList[i])
				});
				this.ItemCollection.AddNoUpdate(changeServerLineItem);
				if (this.LineNames[Global.LineDataList[i].LineID - 1] == lineID)
				{
					changeServerLineItem.CurrentLineTextBlock.Text = Global.GetLang("当前：");
				}
				if (oldLineID == Global.LineDataList[i].LineID)
				{
					selectedIndex = i;
				}
				changeServerLineItem.MouseLeftButtonDown = delegate(object s1, EventArgs e1)
				{
					double num = (double)Global.GetCorrectLocalTime();
					if (num - this.LastLeftButtonDownTicks >= 250.0)
					{
						this.LastLeftButtonDownTicks = num;
						return;
					}
					if (this.SelectedLineListItem == null)
					{
						return;
					}
					if (!Global.Data.GameScene.CanLeaderChangeLineByAttack())
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("战斗结束30秒内不允许切线"), 0, -1, -1, 0);
						return;
					}
					if (!Global.Data.GameScene.CanLeaderChangeLineByInjured())
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("被攻击后30秒内不允许切线"), 0, -1, -1, 0);
						return;
					}
					LineData lineData = this.SelectedLineListItem.Tag as LineData;
					if (lineData.LineID == Global.CurrentListData.LineID)
					{
						this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
						{
							ID = 1,
							IDType = 2
						});
					}
					else
					{
						Global.CurrentListData = lineData;
						string xapParamByName = Super.GetXapParamByName("serverip", "127.0.0.1");
						Global.CurrentListData.GameServerIP = xapParamByName;
						if (this.DPSelectedItem != null)
						{
							this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
							{
								ID = 1,
								IDType = 1
							});
						}
					}
				};
			}
		}
		this.ItemCollection.DelayUpdate();
		this.listBox.SelectedIndex = selectedIndex;
		this.SelectListBox(oldSelectedIndex);
	}

	private void SelectListBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMin(oldSelectedIndex, this.ItemCollection.Count);
			int num = oldSelectedIndex;
			if (num < 0)
			{
				num = 0;
			}
			this.listBox.SelectedIndex = num;
		}
		else
		{
			this.UnSelectItem();
		}
	}

	private void UnSelectItem()
	{
		this.SelectedLineListItem = null;
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (this.listBox.SelectedIndex < 0)
		{
			this.UnSelectItem();
			return;
		}
		if (null != this.SelectedLineListItem)
		{
			this.SelectedLineListItem.BodyBackground = null;
		}
		this.SelectedLineListItem = U3DUtils.AS<ChangeServerLineItem>(this.listBox.SelectedItem);
		if (null == this.SelectedLineListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedLineListItem.BodyBackground = this.SelectedPetsListItemBakImg;
	}

	private UserControl thisCtrl;

	public DPSelectedItemEventHandler DPSelectedItem;

	private bool FirstGetNewData = true;

	private string[] LineNames = new string[]
	{
		Global.GetLang("一"),
		Global.GetLang("二"),
		Global.GetLang("三"),
		Global.GetLang("四"),
		Global.GetLang("五"),
		Global.GetLang("六"),
		Global.GetLang("七"),
		Global.GetLang("八"),
		Global.GetLang("九"),
		Global.GetLang("十")
	};

	private double LastLeftButtonDownTicks;

	private ChangeServerLineItem SelectedLineListItem;

	private ImageBrush SelectedPetsListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 213.0, 20.0, 5.0, 5.0));

	private ListBox listBox = new ListBox();

	public ObservableCollection ItemCollection;

	private GTabControl _tc;
}
