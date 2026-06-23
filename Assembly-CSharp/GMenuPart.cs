using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class GMenuPart : UserControl
{
	public GMenuPart()
	{
		this.thisCtrl = this;
	}

	public void Init()
	{
	}

	public int ItemCount
	{
		get
		{
			return this.ItemIDList.Count;
		}
	}

	protected override void InitializeComponent()
	{
		this.Init();
	}

	public void InitPartSize(int widthI, int heightI)
	{
		this.thisCtrl.Width = (double)widthI;
		this.thisCtrl.Height = (double)heightI;
		this.Container.Width = (double)widthI;
		this.Container.Height = (double)heightI;
		this.Bak.width = (double)widthI;
		this.Bak.height = (double)heightI;
	}

	private void MyMenuItemClick(object sender, object e)
	{
		if (this.MenuItemClick != null)
		{
			this.MenuItemClick.Invoke(sender, e as EventArgs);
		}
	}

	public void AddMenuItem(int id, string imageFileName, string text, GIcon icon = null)
	{
		this.ItemIDList.Add(id);
		this.ItemImageFileNameList.Add(imageFileName);
		this.ItemTextList.Add(text);
		this.ItemIconList.Add(icon);
	}

	public void RenderMenu(int height = 21)
	{
		if (this.ItemIDList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this.ItemIDList.Count; i++)
		{
			GMenuItem gmenuItem = U3DUtils.NEW<GMenuItem>();
			gmenuItem.Left = 5.0;
			gmenuItem.Top = (double)(5 + i * height);
			gmenuItem.BodyWidth = this.thisCtrl.Width - 10.0;
			gmenuItem.BodyHeight = (double)height;
			gmenuItem.MenuItemID = this.ItemIDList[i];
			gmenuItem.GoodImg = ((!(this.ItemIconList[i] != null)) ? null : this.ItemIconList[i].Clone());
			gmenuItem.MenuItemText = this.ItemTextList[i];
			gmenuItem.MenuItemIcon = Global.GetGameResImage(this.ItemImageFileNameList[i]);
			gmenuItem.BodyBackground = ((!(this.ItemIconList[i] != null)) ? new ImageBrush(null) : new ImageBrush(Global.GetGameResImage("Images/Plate/rm_listItem.png")));
			gmenuItem.ImgWidth = (double)((!(this.ItemIconList[i] != null)) ? 10 : 40);
			gmenuItem.ImgHeight = (double)((!(this.ItemIconList[i] != null)) ? 10 : 40);
			gmenuItem.MenuItemClick = new EventHandler(this.MyMenuItemClick);
			this.Container.Children.Add(gmenuItem);
		}
		double num = (double)this.Container.numChildren;
	}

	private Canvas Bak = new Canvas();

	private SpriteSL thisCtrl;

	public EventHandler MenuItemClick;

	private List<int> ItemIDList = new List<int>();

	private List<string> ItemImageFileNameList = new List<string>();

	private List<string> ItemTextList = new List<string>();

	private List<GIcon> ItemIconList = new List<GIcon>();
}
