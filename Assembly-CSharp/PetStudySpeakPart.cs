using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class PetStudySpeakPart : UserControl
{
	public PetStudySpeakPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 252.0;
		this.listBox.Height = 102.0;
		Canvas.SetLeft(this.listBox, 22);
		Canvas.SetTop(this.listBox, 14);
		this.listBox.Background = new SolidColorBrush(16777215U);
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

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.txtPetSpeakItem = U3DUtils.NEW<GTextBlock>();
		this.txtPetSpeakItem.BodyWidth = 258.0;
		this.txtPetSpeakItem.BodyHeight = 21.0;
		this.txtPetSpeakItem.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 258.0, 21.0, 3.0, 2.0));
		this.txtPetSpeakItem.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtPetSpeakItem, 18);
		Canvas.SetTop(this.txtPetSpeakItem, 144);
		this.Container.Children.Add(this.txtPetSpeakItem);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("添加");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 44, 163, 190));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.listBox.Items.Length >= 5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("主人，我只会这5句了，学不会了"), 0, -1, -1, 0);
			}
			string editText = this.txtPetSpeakItem.EditText;
			if (string.IsNullOrEmpty(editText))
			{
				return;
			}
			if (editText.Length > 50)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("哎呀，主人，这句话太长了，我不会~~呜呜…"), 0, -1, -1, 0);
				return;
			}
		};
		Canvas.SetLeft(gicon, 18);
		Canvas.SetTop(gicon, 178);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("修改");
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string petWord = this.txtPetSpeakItem.EditText;
			if (string.IsNullOrEmpty(petWord))
			{
				return;
			}
			if (petWord.Length > 50)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("哎呀，主人，这句话太长了，我不会~~呜呜…"), 0, -1, -1, 0);
				return;
			}
			WordsFilterMgr.ExecWordsFilter(petWord, delegate(object content, ExecWordsFilterEventArgs result)
			{
				if (result.ret > 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
					{
						result.ret,
						result.msg
					}), 0, -1, -1, 0);
					return;
				}
				if (result.is_dirty > 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("宠物名称不能包含国家规定禁止的词汇!"), 0, -1, -1, 0);
					return;
				}
				MountsListItem mountsListItem = U3DUtils.NEW<MountsListItem>();
				mountsListItem.BodyWidth = 248.0;
				mountsListItem.BodyHeight = 20.0;
				petWord = result.msg;
				mountsListItem.MountName.Text = petWord;
				this.ItemCollection.Insert(this.listBox.SelectedIndex, mountsListItem);
				this.ItemCollection.RemoveAt(this.listBox.SelectedIndex);
			});
		};
		Canvas.SetLeft(gicon, 194);
		Canvas.SetTop(gicon, 178);
		this.Container.Children.Add(gicon);
	}

	private void UnSelectItem()
	{
		this.SelectedPetsListItem = null;
	}

	private void listBox_SelectionChanged(object sender, object e)
	{
		if (this.listBox.SelectedIndex < 0)
		{
			this.UnSelectItem();
			return;
		}
		if (null != this.SelectedPetsListItem)
		{
			this.SelectedPetsListItem.BodyBackground = null;
		}
		this.SelectedPetsListItem = U3DUtils.AS<MountsListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedPetsListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedPetsListItem.BodyBackground = this.SelectedPetsListItemBakImg;
		this.txtPetSpeakItem.EditText = this.SelectedPetsListItem.MountName.Text;
	}

	private ListBox listBox = new ListBox();

	private GTextBlock txtPetSpeakItem;

	private MountsListItem SelectedPetsListItem;

	private ImageBrush SelectedPetsListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 248.0, 20.0, 5.0, 5.0));

	private ObservableCollection _ItemCollection;
}
