using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class LianluJinglianPart : UserControl
{
	public ObservableCollection[] equipIcon
	{
		get
		{
			return this._equipIcon;
		}
		set
		{
			this._equipIcon = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(3669815U);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Width = 59.0;
		Canvas.SetLeft(gtextBlockOutLine, 118);
		Canvas.SetTop(gtextBlockOutLine, 235);
		this.Container.Children.Add(gtextBlockOutLine);
		this.TongqianText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(3669815U);
		gtextBlockOutLine.Text = "0%";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Width = 59.0;
		Canvas.SetLeft(gtextBlockOutLine, 226);
		Canvas.SetTop(gtextBlockOutLine, 235);
		this.Container.Children.Add(gtextBlockOutLine);
		this.ChenggonglvText = gtextBlockOutLine;
		this.Container.Children.Add(this.ZhuZhuangbei);
		this.ZhuZhuangbei.Width = 32.0;
		this.ZhuZhuangbei.Height = 32.0;
		Canvas.SetLeft(this.ZhuZhuangbei, 126);
		Canvas.SetTop(this.ZhuZhuangbei, 13);
		this.ZhuZhuangbei.Background = new SolidColorBrush(16777215U);
		this.ZhuZhuangbei.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(0);
			this.ClearEquip(3);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 0,
				PlayID = 1
			});
			this.SetEquipWei(0, 0);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				EquipIDs = this.EquipWeiArr
			});
		};
		this.Container.Children.Add(this.FuZhuangbeiLeft);
		this.FuZhuangbeiLeft.Width = 32.0;
		this.FuZhuangbeiLeft.Height = 32.0;
		Canvas.SetLeft(this.FuZhuangbeiLeft, 47);
		Canvas.SetTop(this.FuZhuangbeiLeft, 110);
		this.FuZhuangbeiLeft.Background = new SolidColorBrush(16777215U);
		this.FuZhuangbeiLeft.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(1);
			this.ClearEquip(3);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 0,
				PlayID = 1
			});
			this.SetEquipWei(1, 0);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				EquipIDs = this.EquipWeiArr
			});
		};
		this.Container.Children.Add(this.FuZhuangbeiRight);
		this.FuZhuangbeiRight.Width = 32.0;
		this.FuZhuangbeiRight.Height = 32.0;
		Canvas.SetLeft(this.FuZhuangbeiRight, 206);
		Canvas.SetTop(this.FuZhuangbeiRight, 110);
		this.FuZhuangbeiRight.Background = new SolidColorBrush(16777215U);
		this.FuZhuangbeiRight.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(2);
			this.ClearEquip(3);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 0,
				PlayID = 1
			});
			this.SetEquipWei(2, 0);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				EquipIDs = this.EquipWeiArr
			});
		};
		this.Container.Children.Add(this.NewEquip);
		this.NewEquip.Width = 32.0;
		this.NewEquip.Height = 32.0;
		this.NewEquip.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.NewEquip, 126);
		Canvas.SetTop(this.NewEquip, 114);
		this.Container.Children.Add(this.Rock);
		this.Rock.Width = 32.0;
		this.Rock.Height = 32.0;
		Canvas.SetLeft(this.Rock, 98);
		Canvas.SetTop(this.Rock, 189);
		this.Rock.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.XYfu);
		this.XYfu.Width = 32.0;
		this.XYfu.Height = 32.0;
		Canvas.SetLeft(this.XYfu, 155);
		Canvas.SetTop(this.XYfu, 189);
		this.XYfu.Background = new SolidColorBrush(16777215U);
		this.XYfu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(5);
			if (this.equipIcon[0].Length > 0)
			{
				this.ChenggonglvText.Text = StringUtil.substitute("{0}%", new object[]
				{
					this.GetChenggonglv(0)
				});
			}
		};
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("精 炼");
		gicon.TextColor = new SolidColorBrush(10551295U);
		Canvas.SetLeft(gicon, 104);
		Canvas.SetTop(gicon, 254);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartJinglian();
		};
	}

	public void InitPartData()
	{
		this.equipIcon = new ObservableCollection[6];
		this.equipIcon[0] = this.ZhuZhuangbei.ItemsSource;
		this.equipIcon[1] = this.FuZhuangbeiLeft.ItemsSource;
		this.equipIcon[2] = this.FuZhuangbeiRight.ItemsSource;
		this.equipIcon[3] = this.NewEquip.ItemsSource;
		this.equipIcon[4] = this.Rock.ItemsSource;
		this.equipIcon[5] = this.XYfu.ItemsSource;
	}

	public void InitAllValue()
	{
		for (int i = 0; i < this.equipIcon.Length; i++)
		{
			this.equipIcon[i].Clear();
		}
		for (int j = 0; j < this.EquipWeiArr.Length; j++)
		{
			this.EquipWeiArr[j] = 0;
		}
		this.TongqianText.Text = "0";
		this.ChenggonglvText.Text = "0%";
	}

	private void ClearEquip(int index)
	{
		if (index >= 0 && index < 6)
		{
			this.equipIcon[index].Clear();
		}
	}

	private int GetChenggonglv(int luckyID = 0)
	{
		int num = 0;
		if (luckyID != 0)
		{
			num = Global.GetForgeLuckyPercentByGoodsID(luckyID);
		}
		else if (this.equipIcon[5].Length > 0)
		{
			luckyID = (U3DUtils.AS<GIcon>(this.equipIcon[5][0]).ItemObject as GoodsData).GoodsID;
			num = Global.GetForgeLuckyPercentByGoodsID(luckyID);
		}
		int num2 = this.JichuChenglonglv;
		num2 += num;
		return Math.Min(100, num2);
	}

	private void SetXiaohao(XElement xml)
	{
		this.NeedTongqian = Global.GetXElementAttributeInt(xml, "Money");
		this.NeedNum = Global.GetXElementAttributeInt(xml, "GoodsNum");
		this.TongqianText.Text = this.NeedTongqian.ToString();
		this.JichuChenglonglv = Global.GetXElementAttributeInt(xml, "Succeed");
		this.ChenggonglvText.Text = StringUtil.substitute("{0}%", new object[]
		{
			this.GetChenggonglv(0)
		});
		this.AddGoodsIcon(Global.GetXElementAttributeInt(xml, "NeedGoodsID"), 4, this.NeedNum);
		if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedTongqian)
		{
			this.TongqianText.TextColor = new SolidColorBrush(16711680U);
		}
		else
		{
			this.TongqianText.TextColor = new SolidColorBrush(3669815U);
		}
	}

	public int SearchItem()
	{
		if (this.EquipWeiArr != null)
		{
			for (int i = 0; i < this.EquipWeiArr.Length; i++)
			{
				if (this.EquipWeiArr[i] > 0)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public int GetEquipKongwei()
	{
		return Array.IndexOf<int>(this.EquipWeiArr, 0);
	}

	public void SetEquipWei(int index, int value)
	{
		if (index < 0 || index >= this.EquipWeiArr.Length)
		{
			return;
		}
		this.EquipWeiArr[index] = value;
	}

	public int IsEquipOK()
	{
		if (this.equipIcon[0].Length > 0 && this.equipIcon[1].Length > 0 && this.equipIcon[2].Length > 0)
		{
			return 1;
		}
		if (this.equipIcon[0].Length <= 0 && this.equipIcon[1].Length <= 0 && this.equipIcon[2].Length <= 0)
		{
			return 0;
		}
		return -1;
	}

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			int num = this.IsEquipOK();
			if (num == 1)
			{
				return;
			}
			if (num == 0)
			{
			}
			int equipKongwei = this.GetEquipKongwei();
			if (equipKongwei != -1)
			{
				this.AddEquipGoodsIcon(gd, equipKongwei, false, 0);
				this.SetEquipWei(equipKongwei, gd.Id);
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0,
					EquipIDs = this.EquipWeiArr
				});
			}
			if (this.IsEquipOK() == 1)
			{
				GoodsData goodsData = (this.equipIcon[0].Length <= 0) ? null : (U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData);
				if (goodsData != null)
				{
					GoodsData nextQualityEquipGoodsData = this.GetNextQualityEquipGoodsData(goodsData);
					if (nextQualityEquipGoodsData != null)
					{
						this.AddEquipGoodsIcon(nextQualityEquipGoodsData, 3, false, 0);
						this.DPEffectItem(this, new NotifyLianluEffectEventArgs
						{
							EffectID = 0,
							PlayID = 0
						});
					}
				}
			}
		}
	}

	private GoodsData GetNextQualityEquipGoodsData(GoodsData goodsData)
	{
		GoodVO goodVO = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodVO == null)
		{
			return null;
		}
		int categoriy = goodVO.Categoriy;
		int shouShiSuitID = goodVO.ShouShiSuitID;
		int qualityID = goodVO.QualityID;
		int toOccupation = goodVO.ToOccupation;
		XElement qualityUpXmlNode = Global.GetQualityUpXmlNode(categoriy, shouShiSuitID, qualityID + 1);
		if (qualityUpXmlNode == null)
		{
			return null;
		}
		this.SetXiaohao(qualityUpXmlNode);
		goodVO = ConfigGoods.GetGoodsXmlNodeByQualityUp(categoriy, shouShiSuitID, goodVO.QualityID, toOccupation);
		int id = goodVO.ID;
		return new GoodsData
		{
			BagIndex = goodsData.BagIndex,
			Binding = goodsData.Binding,
			Endtime = goodsData.Endtime,
			Forge_level = goodsData.Forge_level,
			GCount = goodsData.GCount,
			GoodsID = id,
			Id = 10000,
			Jewellist = goodsData.Jewellist,
			Props = goodsData.Props,
			Quality = goodsData.Quality,
			SaleMoney1 = goodsData.SaleMoney1,
			SaleYinPiao = goodsData.SaleYinPiao,
			SaleYuanBao = goodsData.SaleYuanBao,
			Site = goodsData.Site,
			Starttime = goodsData.Starttime,
			Using = goodsData.Using,
			AddPropIndex = goodsData.AddPropIndex,
			BornIndex = goodsData.BornIndex,
			Lucky = goodsData.Lucky,
			Strong = goodsData.Strong,
			ExcellenceInfo = goodsData.ExcellenceInfo,
			AppendPropLev = goodsData.AppendPropLev,
			ChangeLifeLevForEquip = goodsData.ChangeLifeLevForEquip
		};
	}

	private void AddEquipGoodsIcon(GoodsData gd, int index, bool grayShow = false, int goodsOwnerType = 0)
	{
		this.equipIcon[index].Clear();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 32.0;
			ggoodIcon.Height = 32.0;
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				1,
				gd.Id,
				goodsOwnerType
			});
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = gd.GoodsID;
			ggoodIcon.ItemObject = gd;
			ggoodIcon.BoxTypes = 12;
			ggoodIcon.Text = ((gd.Forge_level <= 0) ? string.Empty : StringUtil.substitute("{0}", new object[]
			{
				gd.Forge_level.ToString()
			}));
			ggoodIcon.TextSize = 12;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Left;
			ggoodIcon.TextVerticalAlignment = global::Layout.Top;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 4294901760U;
			if (!grayShow)
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool liuguang = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitEquipGIcon(ggoodIcon, gd, liuguang, IconTextTypes.Qianghua);
			this.equipIcon[index].Add(ggoodIcon);
		}
	}

	public void AddGoodsIcon(int goodsID, int index, int iNeedNub)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
			gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = Global.GetGoodsDataByID(goodsID);
			gicon.BoxTypes = 5;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			gicon.Text = iNeedNub.ToString();
			gicon.TextHorizontalAlignment = global::Layout.Right;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			gicon.DisableHandCursor = true;
			if (Global.IsForgeRockGoodsID(goodsID))
			{
				gicon.STextVisibility = true;
				gicon.SText = StringUtil.substitute("{0}", new object[]
				{
					Global.GetForgeRockLevelNames(goodsID)
				});
				gicon.STextHorizontalAlignment = global::Layout.Left;
				gicon.STextVerticalAlignment = global::Layout.Top;
				gicon.STextColor = new SolidColorBrush(uint.MaxValue);
				gicon.STextShadowColor = 24831U;
			}
			if (Global.IsLuckyGoodsID(goodsID) && this.equipIcon[0].Length > 0)
			{
				this.ChenggonglvText.Text = StringUtil.substitute("{0}%", new object[]
				{
					this.GetChenggonglv(goodsID)
				});
			}
			if (Global.GetTotalGoodsCountByID(goodsID) > 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			this.equipIcon[index].Clear();
			this.equipIcon[index].Add(gicon);
		}
	}

	public void StartJinglian()
	{
		int equipKongwei = this.GetEquipKongwei();
		if (equipKongwei != -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请将需要的装备放入对应位置"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("主装不在背包中，无法精炼"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int id = goodsData.Id;
		goodsData = (U3DUtils.AS<GIcon>(this.equipIcon[1][0]).ItemObject as GoodsData);
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("辅装不在背包中，无法精炼"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int id2 = goodsData.Id;
		goodsData = (U3DUtils.AS<GIcon>(this.equipIcon[2][0]).ItemObject as GoodsData);
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("辅装不在背包中，无法精炼"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int id3 = goodsData.Id;
		if (this.equipIcon[3].Length <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备已经精炼到了最高品质，无法精炼"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedTongqian)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			return;
		}
		int itemCode = U3DUtils.AS<GIcon>(this.equipIcon[4][0]).ItemCode;
		goodsData = Global.GetGoodsDataByID(itemCode);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中没有需要的精炼石，无法精炼"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsData.GoodsID);
		if (totalGoodsCountByID < this.NeedNum)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("需要的精炼石数量不足, 需要{0}个精炼石"), new object[]
			{
				this.NeedNum
			}), 0, -1, -1, 0);
			return;
		}
		int luckyGoodsID = 0;
		if (this.equipIcon[5].Count > 0)
		{
			itemCode = U3DUtils.AS<GIcon>(this.equipIcon[5][0]).ItemCode;
			goodsData = Global.GetGoodsDataByID(itemCode);
			if (goodsData == null)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中没有需要的幸运符，无法精炼"), new object[0]), 0, -1, -1, 0);
				return;
			}
			luckyGoodsID = goodsData.GoodsID;
		}
		this.ShowModalDialog();
		GameInstance.Game.SpriteLianLuJingLian(id, id2, id3, luckyGoodsID);
	}

	public void NotifyJinglianResult(int dbID)
	{
		this.CloseModalDialog();
		if (dbID < 0)
		{
			if (dbID == -1000 || dbID == -5555)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("本次精炼失败！"), new object[0]), 0, -1, -1, 0);
			}
			else if (dbID == -9998)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备已经不在背包中，无法精炼"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备精炼时发生错误:{0}"), new object[]
				{
					dbID
				}), 0, -1, -1, 0);
			}
			Global.PlaySoundAudio(this.equip_failed, false);
			if (this.equipIcon[4].Length > 0)
			{
				this.AddGoodsIcon(U3DUtils.AS<GIcon>(this.equipIcon[4][0]).ItemCode, 4, this.NeedNum);
			}
			this.ClearEquip(5);
			if (this.equipIcon[0].Length > 0)
			{
				this.ChenggonglvText.Text = StringUtil.substitute("{0}%", new object[]
				{
					this.GetChenggonglv(0)
				});
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1,
				Flag = 0
			});
			return;
		}
		this.InitAllValue();
		Global.PlaySoundAudio(this.equip_ok, false);
		this.DPEffectItem(this, new NotifyLianluEffectEventArgs
		{
			EffectID = 1,
			PlayID = 1
		});
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备精炼成功!"), new object[0]), 0, -1, -1, 0);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1,
				Flag = 1
			});
		}
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(dbID, null);
		if (goodsDataByDbID == null)
		{
			return;
		}
		this.AddEquipGoodsIcon(goodsDataByDbID, 3, false, 0);
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.Opacity = 0.01;
		this.PlaceHolder.Width = this.Width;
		this.PlaceHolder.Height = this.Height;
		Canvas.SetZIndex(this.PlaceHolder, 9000.0);
		this.Container.Children.Add(this.PlaceHolder);
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Container.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	private Canvas PlaceHolder;

	private GTextBlockOutLine TongqianText;

	private GTextBlockOutLine ChenggonglvText;

	private ListBox ZhuZhuangbei = new ListBox();

	private ListBox FuZhuangbeiLeft = new ListBox();

	private ListBox FuZhuangbeiRight = new ListBox();

	private ListBox NewEquip = new ListBox();

	private ListBox Rock = new ListBox();

	private ListBox XYfu = new ListBox();

	private int NeedNum = -1;

	private int NeedTongqian = -1;

	private int JichuChenglonglv;

	public int[] EquipWeiArr = new int[3];

	private ObservableCollection[] _equipIcon;

	private string equip_ok = StringUtil.substitute("Media/Music/UI/equip_ok.mp3", new object[0]);

	private string equip_failed = StringUtil.substitute("Media/Music/UI/equip_failed.mp3", new object[0]);
}
