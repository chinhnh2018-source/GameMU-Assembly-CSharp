using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class GGoodsBox : UserControl
{
	public int RowCount { get; set; }

	public int ColCount { get; set; }

	protected override void InitializeComponent()
	{
		this.Items = this.listBox.ItemsSource;
	}

	public void InitBox()
	{
		this.Items = this.listBox.ItemsSource;
		for (int i = 0; i < this.RowCount; i++)
		{
			for (int j = 0; j < this.ColCount; j++)
			{
				this.Items.Add(this.GetEmptyGO(i * this.ColCount + j));
			}
		}
		this.Items.DelayUpdate();
	}

	public override void Destroy()
	{
		for (int i = 0; i < this.RowCount; i++)
		{
			for (int j = 0; j < this.ColCount; j++)
			{
				int num = i * this.ColCount + j;
			}
		}
	}

	private void ClearBoxItem(GGoodIcon boxItem)
	{
		boxItem.Children.Clear();
		boxItem.Container.Children.Clear();
	}

	public void SetGoodsIcon(int num, GGoodIcon icon)
	{
		if (num >= this.RowCount * this.ColCount || null == icon)
		{
			return;
		}
		this.listBox.Replace(num, icon.gameObject);
	}

	public GGoodIcon GetGoodsIcon(int num)
	{
		GGoodIcon result = null;
		if (this.Items.Count > 0)
		{
			return U3DUtils.AS<GGoodIcon>(this.Items[num]);
		}
		return result;
	}

	public bool RemoveGoodsIcon(int num)
	{
		if (num >= this.Items.Count)
		{
			return false;
		}
		if (null == this.Items[num])
		{
			return false;
		}
		this.listBox.Replace(num, this.GetEmptyGGoodIcon(num).gameObject);
		return true;
	}

	public int FindByGoodsDbID(int id)
	{
		for (int i = 0; i < this.RowCount; i++)
		{
			for (int j = 0; j < this.ColCount; j++)
			{
				GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(this.Items[i * this.ColCount + j]);
				if (!(null == ggoodIcon))
				{
					GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
					if (goodsData != null && goodsData.Id == id)
					{
						return i * this.ColCount + j;
					}
				}
			}
		}
		return -1;
	}

	public int FindByGoodsPos(int goodsID, int level, int quality, out PointSL pointSL)
	{
		pointSL = new PointSL(0.0, 0.0);
		for (int i = 0; i < this.RowCount; i++)
		{
			for (int j = 0; j < this.ColCount; j++)
			{
				GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(this.Items[i * this.ColCount + j]);
				if (!(null == ggoodIcon))
				{
					GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
					if (goodsData.GoodsID == goodsID && goodsData.Forge_level == level && goodsData.Quality == quality)
					{
						pointSL = new PointSL(U3DUtils.AS<GGoodIcon>(this.Items[i * this.ColCount + j]).X, U3DUtils.AS<GGoodIcon>(this.Items[i * this.ColCount + j]).Y);
						return 0;
					}
				}
			}
		}
		return -1;
	}

	public int FindByGoodsPosByDbID(int goodsDbID, out PointSL pointSL)
	{
		pointSL = new PointSL(0.0, 0.0);
		for (int i = 0; i < this.RowCount; i++)
		{
			for (int j = 0; j < this.ColCount; j++)
			{
				GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(this.Items[i * this.ColCount + j]);
				if (!(null == ggoodIcon))
				{
					GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
					if (goodsData.Id == goodsDbID)
					{
						pointSL = new PointSL(U3DUtils.AS<GGoodIcon>(this.Items[i * this.ColCount + j]).X, U3DUtils.AS<GGoodIcon>(this.Items[i * this.ColCount + j]).Y);
						return 0;
					}
				}
			}
		}
		return -1;
	}

	public void ClearAllGoodsIcon()
	{
		for (int i = 0; i < this.RowCount; i++)
		{
			for (int j = 0; j < this.ColCount; j++)
			{
				this.listBox.Replace(i * this.ColCount + j, this.GetEmptyGGoodIcon(i * this.ColCount + j).gameObject);
			}
		}
	}

	public bool GoodsIconExists(int num)
	{
		return !(U3DUtils.AS<GGoodIcon>(this.Items[num]) == null);
	}

	private GameObject GetEmptyGO(int index)
	{
		return new GameObject
		{
			name = StringUtil.substitute("EmpteItem{0}", new object[]
			{
				index
			})
		};
	}

	private EmptePart GetEmptyGGoodIcon(int index)
	{
		EmptePart newEmptyPart = Global.GetNewEmptyPart();
		newEmptyPart.name = StringUtil.substitute("EmpteItem{0}", new object[]
		{
			index
		});
		return newEmptyPart;
	}

	private ObservableCollection Items;

	public ListBox listBox;

	public DPSelectedItemBoolEventHandler DPSelectedItem;
}
