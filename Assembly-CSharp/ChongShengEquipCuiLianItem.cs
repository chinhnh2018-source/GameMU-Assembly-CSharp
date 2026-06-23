using System;

public class ChongShengEquipCuiLianItem : UserControl
{
	public EquipCuiLian EquipCuiLianKey
	{
		get
		{
			return this.mequipCuiLianKey;
		}
		set
		{
			this.mequipCuiLianKey = value;
			this.m_SpBack.spriteName = string.Format("wupinkuang_{0}", (int)value);
		}
	}

	public bool OnClick
	{
		get
		{
			return this.m_BoolOnClick;
		}
		set
		{
			this.m_BoolOnClick = value;
			if (this.m_BoolOnClick)
			{
				this.m_SpOn.gameObject.SetActive(true);
			}
			else
			{
				this.m_SpOn.gameObject.SetActive(false);
			}
		}
	}

	public UISprite m_SpBack;

	public UISprite m_SpOn;

	private EquipCuiLian mequipCuiLianKey;

	private bool m_BoolOnClick;
}
