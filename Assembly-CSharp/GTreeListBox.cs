using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class GTreeListBox : GBasePart
{
	public void InitControls2(int width, int height)
	{
		this.Container.Width = (double)(width - 20);
		this.wrapper.Width = (double)(width - 20);
		this.ContainViewer = new GScrollView(width, height, 0);
		this.ContainViewer.Viewer = this.wrapper;
		this.wrapper.Height = this.ContainViewer.Height - 4.0;
		Canvas.SetLeft(this.ContainViewer, 0);
		Canvas.SetTop(this.ContainViewer, 0);
		this.Container.Children.Add(this.ContainViewer);
	}

	public double IdentWidth
	{
		get
		{
			return this._IdentWidth;
		}
		set
		{
			this._IdentWidth = value;
		}
	}

	public double IdentWidth2
	{
		get
		{
			return this._IdentWidth2;
		}
		set
		{
			this._IdentWidth2 = value;
		}
	}

	public int SelectedOneLevelItem
	{
		get
		{
			return this._SelectedOneLevelItem;
		}
		set
		{
			this._SelectedOneLevelItem = value;
		}
	}

	public int SelectedSubLevelItem
	{
		get
		{
			return this._SelectedSubLevelItem;
		}
		set
		{
			this._SelectedSubLevelItem = value;
		}
	}

	public GTreeListBoxOneLeveItem AddOneLevelItem(string text)
	{
		return this.AddOneLevelItem1(this.OneLevelItemList.Count, text);
	}

	private GTreeListBoxOneLeveItem AddOneLevelItem1(int index, string text)
	{
		GTreeListBoxOneLeveItem item = U3DUtils.NEW<GTreeListBoxOneLeveItem>();
		item.BodyHeight = 18.0;
		item.Tag = index;
		item.BodyWidth = this.Container.Width;
		item.MouseEnter = delegate(object s, EventArgs e)
		{
			int num = (int)(s as GTreeListBoxOneLeveItem).Tag;
			if (this._SelectedOneLevelItem != num)
			{
				this.OneLevelItemList[num].SelectedState = true;
			}
			if (Global.Data.GameCursorImageID < 100)
			{
			}
		};
		item.MouseLeave = delegate(object s, EventArgs e)
		{
			int num = (int)(s as GTreeListBoxOneLeveItem).Tag;
			if (this._SelectedOneLevelItem != num)
			{
				this.OneLevelItemList[num].SelectedState = false;
			}
			if (Global.Data.GameCursorImageID < 100)
			{
			}
		};
		item.MouseLeftButtonUp = delegate(object s, EventArgs e)
		{
			int num = (int)(s as GTreeListBoxOneLeveItem).Tag;
			if (this._SelectedOneLevelItem != num)
			{
				if (this._SelectedOneLevelItem != -1)
				{
					this.OneLevelItemList[this._SelectedOneLevelItem].FindName("SubIcon").SafeGetComponent<GIcon>().Visibility = false;
					this.OneLevelItemList[this._SelectedOneLevelItem].FindName("AddIcon").SafeGetComponent<GIcon>().Visibility = true;
					this.OneLevelItemList[this._SelectedOneLevelItem].SelectedState = false;
				}
				this.OneLevelItemList[num].SelectedState = true;
				this.OneLevelItemList[num].FindName("SubIcon").SafeGetComponent<GIcon>().Visibility = true;
				this.OneLevelItemList[num].FindName("AddIcon").SafeGetComponent<GIcon>().Visibility = false;
				this._SelectedOneLevelItem = num;
				this.SelectedSubLevelItem = -1;
				this.RefreshListBox();
			}
			else
			{
				this.OneLevelItemList[this._SelectedOneLevelItem].FindName("SubIcon").SafeGetComponent<GIcon>().Visibility = false;
				this.OneLevelItemList[this._SelectedOneLevelItem].FindName("AddIcon").SafeGetComponent<GIcon>().Visibility = true;
				this.OneLevelItemList[this._SelectedOneLevelItem].SelectedState = false;
				this._SelectedOneLevelItem = -1;
				this.SelectedSubLevelItem = -1;
				this.RefreshListBox();
			}
			if (this.ItemClick != null)
			{
				this.ItemClick.Invoke(item, EventArgs.Empty);
			}
		};
		item.ItemName.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 143, 115, 82));
		item.ItemName.Text = text;
		item.ItemName.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 113, 167, 180));
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "AddIcon";
		gicon.Width = 16.0;
		gicon.Height = 16.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/add.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/add.png"));
		gicon.Tag = index;
		Canvas.SetLeft(gicon, 1);
		Canvas.SetTop(gicon, 1);
		item.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "SubIcon";
		gicon.Width = 16.0;
		gicon.Height = 16.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/sub.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/sub.png"));
		gicon.Tag = index;
		Canvas.SetLeft(gicon, 1);
		Canvas.SetTop(gicon, 1);
		item.Children.Add(gicon);
		gicon.Visibility = false;
		this.OneLevelItemList.Add(item);
		return item;
	}

	public void AddSubLevelItem(int oneLevelIndex, SpriteSL uc)
	{
		List<GTreeListBoxOneLeveItem> list;
		if (!this.SubItemDict.ContainsKey(oneLevelIndex))
		{
			list = new List<GTreeListBoxOneLeveItem>();
			this.SubItemDict.Add(oneLevelIndex, list);
		}
		list = this.SubItemDict.GetValue(oneLevelIndex);
		double actualHeight = uc.ActualHeight;
		GTreeListBoxOneLeveItem item = U3DUtils.NEW<GTreeListBoxOneLeveItem>();
		item.BodyHeight = actualHeight;
		item.Tag = list.Count;
		item.BodyWidth = this.Container.Width;
		item.Container.Children.Remove(item.ItemName, true);
		Canvas.SetTop(uc, 3);
		item.Container.Children.Add(uc);
		item.MouseEnter = delegate(object s, EventArgs e)
		{
			int num = (int)(s as GTreeListBoxOneLeveItem).Tag;
			if (this.SelectedSubLevelItem != num)
			{
				(s as GTreeListBoxOneLeveItem).SelectedState = true;
			}
			if (Global.Data.GameCursorImageID < 100)
			{
			}
		};
		item.MouseLeave = delegate(object s, EventArgs e)
		{
			int num = (int)(s as GTreeListBoxOneLeveItem).Tag;
			if (this.SelectedSubLevelItem != num)
			{
				(s as GTreeListBoxOneLeveItem).SelectedState = false;
			}
			if (Global.Data.GameCursorImageID < 100)
			{
			}
		};
		item.MouseLeftButtonUp = delegate(object s, EventArgs e)
		{
			int num = (int)(s as GTreeListBoxOneLeveItem).Tag;
			if (this.SelectedSubLevelItem != num)
			{
				if (this.SelectedSubLevelItem != -1)
				{
					this.SubItemDict[this._SelectedOneLevelItem][this.SelectedSubLevelItem].SelectedState = false;
				}
				this.OneLevelItemList[this._SelectedOneLevelItem].SelectedState = false;
				this.SelectedSubLevelItem = num;
				this.SubItemDict[this._SelectedOneLevelItem][this.SelectedSubLevelItem].SelectedState = true;
			}
			if (this.ItemClick != null)
			{
				this.ItemClick.Invoke(item, EventArgs.Empty);
			}
		};
		list.Add(item);
	}

	public SpriteSL GetSelectedSubLevelItem()
	{
		if (this._SelectedOneLevelItem < 0)
		{
			return null;
		}
		if (this._SelectedSubLevelItem < 0)
		{
			return null;
		}
		if (!this.SubItemDict.ContainsKey(this._SelectedOneLevelItem))
		{
			return null;
		}
		List<GTreeListBoxOneLeveItem> value = this.SubItemDict.GetValue(this._SelectedOneLevelItem);
		if (this._SelectedSubLevelItem >= value.Count)
		{
			return null;
		}
		if (value[this._SelectedSubLevelItem].SelectedState)
		{
			return value[this._SelectedSubLevelItem].Root.Children.getChildAt(2).SafeGetComponent<SpriteSL>();
		}
		return null;
	}

	public int GetSubLevelItemCount(int index)
	{
		if (!this.SubItemDict.ContainsKey(index))
		{
			return -1;
		}
		List<GTreeListBoxOneLeveItem> value = this.SubItemDict.GetValue(index);
		return value.Count;
	}

	public void ClearSubLevelItem(int index)
	{
		if (!this.SubItemDict.ContainsKey(index))
		{
			return;
		}
		List<GTreeListBoxOneLeveItem> value = this.SubItemDict.GetValue(index);
		value.RemoveRange(0, value.Count);
	}

	public void ClearAll()
	{
		this.Container.Children.Clear();
		this.OneLevelItemList.RemoveRange(0, this.OneLevelItemList.Count);
		this.SubItemDict.Clear();
	}

	public void ResetListBox()
	{
		if (this._SelectedOneLevelItem >= 0 && this._SelectedOneLevelItem < this.OneLevelItemList.Count)
		{
			this.OneLevelItemList[this._SelectedOneLevelItem].FindName("SubIcon").SafeGetComponent<GIcon>().Visibility = false;
			this.OneLevelItemList[this._SelectedOneLevelItem].FindName("AddIcon").SafeGetComponent<GIcon>().Visibility = true;
			this.OneLevelItemList[this._SelectedOneLevelItem].SelectedState = false;
		}
		this._SelectedOneLevelItem = -1;
		this.SelectedSubLevelItem = -1;
		this.RefreshListBox();
	}

	public void RefreshListBox()
	{
		this.Container.Children.Clear();
		this.Container.Height = this.ContainViewer.Height - 4.0;
		double num = 0.0;
		for (int i = 0; i < this.OneLevelItemList.Count; i++)
		{
			Canvas.SetLeft(this.OneLevelItemList[i], this.IdentWidth);
			Canvas.SetTop(this.OneLevelItemList[i], num);
			this.Container.Children.Add(this.OneLevelItemList[i]);
			num += this.OneLevelItemList[i].BodyHeight;
			if (i == this.SelectedOneLevelItem)
			{
				this.OneLevelItemList[i].SelectedState = true;
				this.OneLevelItemList[i].FindName("SubIcon").SafeGetComponent<GIcon>().Visibility = true;
				this.OneLevelItemList[i].FindName("AddIcon").SafeGetComponent<GIcon>().Visibility = false;
				if (this.SubItemDict.ContainsKey(i))
				{
					List<GTreeListBoxOneLeveItem> value = this.SubItemDict.GetValue(i);
					for (int j = 0; j < value.Count; j++)
					{
						Canvas.SetLeft(value[j], this.IdentWidth + this.IdentWidth2);
						Canvas.SetTop(value[j], num);
						this.Container.Children.Add(value[j]);
						num += value[j].BodyHeight;
						if (this._SelectedSubLevelItem == j)
						{
							this.OneLevelItemList[i].SelectedState = false;
							value[j].SelectedState = true;
						}
					}
				}
			}
		}
		this.Container.Height = Global.GMax(num, this.ContainViewer.Height - 4.0);
		this.ContainViewer.ResetScrollView();
	}

	private GScrollView ContainViewer;

	public EventHandler ItemClick;

	public Canvas wrapper = new Canvas();

	private int _SelectedOneLevelItem = -1;

	private double _IdentWidth;

	private double _IdentWidth2;

	private int _SelectedSubLevelItem = -1;

	private List<GTreeListBoxOneLeveItem> OneLevelItemList = new List<GTreeListBoxOneLeveItem>();

	private Dictionary<int, List<GTreeListBoxOneLeveItem>> SubItemDict = new Dictionary<int, List<GTreeListBoxOneLeveItem>>();
}
