using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class GTipSprite : UserControl
{
	public string Tip
	{
		get
		{
			return this._Tip;
		}
		set
		{
			this._Tip = value;
		}
	}

	public int TipType
	{
		get
		{
			return this._TipType;
		}
		set
		{
			this._TipType = value;
		}
	}

	public int type { get; set; }

	public int GoodsCount { get; set; }

	public int GoodsID { get; set; }

	public int ForgeLevel { get; set; }

	public int ZhuijiaLevel { get; set; }

	public int ExcellenceInfo { get; set; }

	public int Lucky { get; set; }

	public int Binding { get; set; }

	public void NormalGoodsTipsHandler(object sender, EventArgs e)
	{
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(this.GoodsID, this.ForgeLevel, this.ZhuijiaLevel, this.ExcellenceInfo, this.Lucky, this.Binding, this.GoodsCount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		GTipServiceEx.ShowTip(null, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, dummyGoodsDataMu);
	}

	public void NormalBuffTipsHandler(object sender, EventArgs e)
	{
		Transform transform = (sender as GameObject).transform;
		Vector3 senderPos;
		senderPos..ctor(transform.position.x, transform.position.y);
		GTipServiceEx.ShowTip(senderPos, TipTypes.BufferTip, (this.Tag as BufferData).BufferID);
	}

	private string _Tip;

	private int _TipType;
}
