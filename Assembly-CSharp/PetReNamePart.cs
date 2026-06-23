using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class PetReNamePart : UserControl
{
	public PetReNamePart()
	{
		this.thisCtrl = this;
	}

	protected override void InitializeComponent()
	{
	}

	public int PetDbID
	{
		get
		{
			return this._petDbID;
		}
		set
		{
			this._petDbID = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.txtNewPetName = U3DUtils.NEW<GTextBlock>();
		this.txtNewPetName.BodyWidth = 136.0;
		this.txtNewPetName.BodyHeight = 21.0;
		this.txtNewPetName.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 136.0, 21.0, 3.0, 2.0));
		this.txtNewPetName.FontSize = 12;
		this.txtNewPetName.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtNewPetName, 145);
		Canvas.SetTop(this.txtNewPetName, 22);
		this.Container.Children.Add(this.txtNewPetName);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("确定");
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.PetReNameOk();
		};
		Canvas.SetLeft(gicon, 100);
		Canvas.SetTop(gicon, 75);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("取消");
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 192);
		Canvas.SetTop(gicon, 75);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData()
	{
		if (this._petDbID == -1)
		{
			return;
		}
		PetData petDataByDbID = Global.GetPetDataByDbID(this._petDbID);
		this.txtNewPetName.EditText = petDataByDbID.PetName;
	}

	private void PetReNameOk()
	{
		string petName = this.txtNewPetName.EditText;
		if (string.IsNullOrEmpty(petName))
		{
			return;
		}
		if (petName.Length > 7)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("宠物名称不能超过7个汉字!"), 0, -1, -1, 0);
			return;
		}
		WordsFilterMgr.ExecWordsFilter(petName, delegate(object content, ExecWordsFilterEventArgs result)
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
			petName = result.msg;
			petName = Global.StringReplaceAll(petName, "'", string.Empty);
			petName = Global.StringReplaceAll(petName, "|", string.Empty);
			petName = Global.StringReplaceAll(petName, "$", string.Empty);
			petName = Global.StringReplaceAll(petName, ":", string.Empty);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
			GameInstance.Game.SpriteModPet(this._petDbID, 0, petName);
		});
	}

	private SpriteSL thisCtrl;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _petDbID = -1;

	private GTextBlock txtNewPetName;
}
