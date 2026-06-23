using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class WeekendDepositExtraAwardItem : UserControl
{
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

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.GoodsList.ItemsSource;
	}

	public string goodsIDs
	{
		get
		{
			return this._goodsIDs;
		}
		set
		{
			this._goodsIDs = value;
			this.LoadGoodsList(this._goodsIDs);
		}
	}

	private void LoadGoodsList(string goodsIDs)
	{
		this.ItemCollection.Clear();
		if (!(string.Empty == goodsIDs))
		{
			string[] array = goodsIDs.Split(new char[]
			{
				'@'
			});
			if (array.Length == 1)
			{
				this.LoadOtherJiangLiGoodsList(goodsIDs, false);
			}
			else
			{
				this.LoadOtherJiangLiGoodsList(array[0], true);
				this.LoadOtherJiangLiGoodsList(array[1], false);
			}
		}
	}

	private void LoadOtherJiangLiGoodsList(string goodsStr, bool isOcc = false)
	{
		string text = StringUtil.trim(goodsStr);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				if (!isOcc || !MUJieripartChongzhiKingItem.IsTongGuo(array2[0], roleOcc))
				{
					GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[6]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
					this.AddExtraAwardGoodsIcon(dummyGoodsDataMu, false);
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void AddExtraAwardGoodsIcon(GoodsData gd, bool grayShow = false)
	{
		ExtraAwardGoodIcon extraAwardGoodIcon = U3DUtils.NEW<ExtraAwardGoodIcon>();
		extraAwardGoodIcon.gray = grayShow;
		extraAwardGoodIcon.dragable = true;
		extraAwardGoodIcon.goodsData = gd;
		if (null != extraAwardGoodIcon.goodsIcon)
		{
			this.ItemCollection.Add(extraAwardGoodIcon);
			extraAwardGoodIcon.goodsIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public ListBox GoodsList;

	public int index;

	private ObservableCollection _ItemCollection;

	private string _goodsIDs = string.Empty;
}
