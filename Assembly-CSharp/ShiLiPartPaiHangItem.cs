using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ShiLiPartPaiHangItem : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public void Init(int paiMing, string name, int junXian)
	{
		if (paiMing < 1)
		{
			this.lblPaiMing.text = Global.GetLang("未上榜");
		}
		else
		{
			this.lblPaiMing.text = paiMing.ToString();
		}
		this.lblName.text = name;
		this.lblJunXian.text = junXian.ToString();
		this.lblPaiMing.pivot = 3;
		this.lblPaiMing.transform.localPosition = new Vector3(-220f, this.lblPaiMing.transform.localPosition.y, this.lblPaiMing.transform.localPosition.z);
	}

	public void InitNull()
	{
		this.lblPaiMing.text = string.Empty;
		this.lblName.text = string.Empty;
		this.lblJunXian.text = string.Empty;
	}

	public bool Select
	{
		get
		{
			return this.mSelect;
		}
		set
		{
			this.mSelect = value;
			if (null != this._Select)
			{
				this._Select.gameObject.SetActive(this.mSelect);
			}
		}
	}

	public UILabel lblPaiMing;

	public UILabel lblName;

	public UILabel lblJunXian;

	public UISprite _Select;

	private bool mSelect;
}
