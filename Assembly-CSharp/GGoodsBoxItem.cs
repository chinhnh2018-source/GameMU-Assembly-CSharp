using System;
using HSGameEngine.GameEngine.Logic;

public class GGoodsBoxItem : UserControl
{
	public GGoodsBoxItem()
	{
		this.ItemCollection = this.listBox.Items;
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

	protected override void InitializeComponent()
	{
	}

	public void SetGoodsIcon(GIcon icon)
	{
		this.ItemCollection.Clear();
		this.ItemCollection.Add(icon);
	}

	public GIcon GetGoodsIcon()
	{
		if (this.ItemCollection.Count > 0)
		{
			return U3DUtils.AS<GIcon>(this.ItemCollection[0]);
		}
		return null;
	}

	public void RemoveGoodsIcon()
	{
		this.ItemCollection.Clear();
	}

	public bool GoodsIconExists()
	{
		return this.ItemCollection.Count > 0;
	}

	public ListBox listBox;

	private ObservableCollection _ItemCollection;
}
