using System;

public class InfoLinkPlate : UserControl
{
	public InfoLinkPlate()
	{
		this.ItemCollection = this.listBox.ItemsSource;
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

	private ListBox listBox = new ListBox();

	private ObservableCollection _ItemCollection;
}
