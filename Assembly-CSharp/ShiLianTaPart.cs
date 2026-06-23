using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class ShiLianTaPart : UserControl
{
	public void InitPartSize(int width, int height)
	{
		GameInstance.Game.SpriteQueryShiLianTaAwardInfoData();
		this.BtnArr.RemoveAt(0);
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.txtBenChengGuaiShuLiang);
		Canvas.SetLeft(this.txtBenChengGuaiShuLiang, 136);
		Canvas.SetTop(this.txtBenChengGuaiShuLiang, 48);
		this.txtBenChengGuaiShuLiang.TextColor = new SolidColorBrush(16777215U);
		this.txtBenChengGuaiShuLiang.Text = "0";
		this.Container.Children.Add(this.txtBenChengExp);
		Canvas.SetLeft(this.txtBenChengExp, 136);
		Canvas.SetTop(this.txtBenChengExp, 72);
		this.txtBenChengExp.TextColor = new SolidColorBrush(16777215U);
		this.txtBenChengExp.Text = "0";
		this.Container.Children.Add(this.txtXiaChengDaoJuShuLiang);
		Canvas.SetLeft(this.txtXiaChengDaoJuShuLiang, 136);
		Canvas.SetTop(this.txtXiaChengDaoJuShuLiang, 96);
		this.txtXiaChengDaoJuShuLiang.TextColor = new SolidColorBrush(65280U);
		this.txtXiaChengDaoJuShuLiang.Text = "0";
		this.Container.Children.Add(this.txtXiaChengExp);
		Canvas.SetLeft(this.txtXiaChengExp, 136);
		Canvas.SetTop(this.txtXiaChengExp, 120);
		this.txtXiaChengExp.TextColor = new SolidColorBrush(65280U);
		this.txtXiaChengExp.Text = "0";
		this.Container.Children.Add(this.txtZhongDaoJuShuLiang);
		Canvas.SetLeft(this.txtZhongDaoJuShuLiang, 136);
		Canvas.SetTop(this.txtZhongDaoJuShuLiang, 197);
		this.txtZhongDaoJuShuLiang.TextColor = new SolidColorBrush(65280U);
		this.txtZhongDaoJuShuLiang.Text = "0";
		GCheckBox gcheckBox = new GCheckBox();
		gcheckBox.Name = "AotoBuy";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Check = false;
		gcheckBox.Text = Global.GetLang("自动购买通行令");
		gcheckBox.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 113, 167, 180));
		Canvas.SetLeft(gcheckBox, 90);
		Canvas.SetTop(gcheckBox, 229);
		this.AutoBuyCheckBox = gcheckBox;
		this.AutoBuyCheckBox.Check = ShiLianTaPart.AllowAutoBuy;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 112.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 112.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 112.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 112.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("领取经验离开");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (base.EnableIcon)
			{
				base.EnableIcon = false;
				GameInstance.Game.SpriteFetchShiLianTaFuBenAward(1, (!this.AutoBuyCheckBox.Check) ? 0 : 1);
			}
		};
		Canvas.SetLeft(gicon, 25);
		Canvas.SetTop(gicon, 269);
		this.Container.Children.Add(gicon);
		gicon.EnableIcon = true;
		this.BtnArr.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 112.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 112.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 112.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 112.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("领取经验到下层");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (base.EnableIcon)
			{
				ShiLianTaPart.AllowAutoBuy = this.AutoBuyCheckBox.Check;
				if (!this.AutoBuyCheckBox.Check && this.AwardInfoData != null && this.AwardInfoData.NextFloorNeedGoodsNum > Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShiLianLing))
				{
					int toBuyGoodsID = (int)ConfigSystemParam.GetSystemParamIntByName("ShiLianLingGoodsID");
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("进入下一层时需要的通天令不足【商城中使用钻石或绑定钻石可以购买通天令】"), new object[0]), 24, -1, -1, toBuyGoodsID);
				}
				else
				{
					base.EnableIcon = false;
					GameInstance.Game.SpriteFetchShiLianTaFuBenAward(0, (!this.AutoBuyCheckBox.Check) ? 0 : 1);
				}
			}
		};
		Canvas.SetLeft(gicon, 149);
		Canvas.SetTop(gicon, 269);
		this.Container.Children.Add(gicon);
		this.BtnArr.Add(gicon);
		gicon.HintDecoType = 1;
	}

	public void InitPartData()
	{
		this.txtBenChengGuaiShuLiang.Text = "100";
		this.txtBenChengExp.Text = "100";
		this.txtXiaChengDaoJuShuLiang.Text = "100";
		this.txtXiaChengExp.Text = "100";
		this.txtZhongDaoJuShuLiang.Text = "100";
	}

	public void OnQueryShiLianTaAwardsInfoDataCompleted(ShiLianTaAwardsInfoData myData)
	{
		if (myData != null)
		{
			this.txtBenChengGuaiShuLiang.Text = myData.CurrentFloorTotalMonsterNum.ToString();
			this.txtBenChengExp.Text = myData.CurrentFloorExperienceAward.ToString();
			this.txtXiaChengDaoJuShuLiang.Text = myData.NextFloorNeedGoodsNum.ToString();
			this.txtXiaChengExp.Text = myData.NextFloorExperienceAward.ToString();
			this.txtZhongDaoJuShuLiang.Text = StringUtil.substitute("{0}", new object[]
			{
				Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShiLianLing)
			});
			if (myData.NextFloorExperienceAward <= 0)
			{
				this.BtnArr[1].EnableIcon = false;
				this.BtnArr[1].HintDecoType = -1;
				this.BtnArr[0].HintDecoType = 1;
			}
			this.AwardInfoData = myData;
		}
	}

	public void OnFetchShiLianTaFuBenAwardCompleted(int result, int roleID)
	{
		this.BtnArr[0].EnableIcon = true;
		this.BtnArr[1].EnableIcon = true;
		if (result < 0 && !this.NoHintMoreError)
		{
			if (result == -2300)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("自动购买时钻石不足"), 0, -1, -1, 0);
			}
			else if (result != -88 && result != -6)
			{
				if (result == -1005)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("进入下一层是背包内需要的通天令不足"), new object[0]), 0, -1, -1, 0);
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励时错误:{0}"), new object[]
					{
						result
					}), 0, -1, -1, 0);
				}
			}
		}
		else if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 1
			});
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private GTextBlockOutLine txtBenChengGuaiShuLiang = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtBenChengExp = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtXiaChengDaoJuShuLiang = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtXiaChengExp = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtZhongDaoJuShuLiang = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GCheckBox AutoBuyCheckBox;

	private List<GIcon> BtnArr = new List<GIcon>();

	private ShiLianTaAwardsInfoData AwardInfoData;

	private bool NoHintMoreError;

	private static bool AllowAutoBuy;
}
