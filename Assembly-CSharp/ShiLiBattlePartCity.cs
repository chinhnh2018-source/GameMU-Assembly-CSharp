using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;
using XMLCreater;

public class ShiLiBattlePartCity : MonoBehaviour
{
	public MUForceCraft CityInfo
	{
		get
		{
			return this.m_cityInfo;
		}
	}

	public ShiLiType ShiLiType
	{
		get
		{
			return this.m_shiLiType;
		}
		set
		{
			this.m_shiLiType = value;
			this.lblCityName.text = this.GetCityName();
			this.imgShiLi.spriteName = "shili" + (int)this.m_shiLiType;
		}
	}

	public bool BeInBattle
	{
		get
		{
			return this.m_beInBattle;
		}
		set
		{
			this.m_beInBattle = value;
			this.goTeXiao.SetActive(this.m_beInBattle);
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected void Awake()
	{
		this.InitTextInPrefabs();
		this.m_cityInfo = ShiLiData.GetForceCraftByID(this.cityID);
		if (this.m_cityInfo != null)
		{
			this.lblCityName.text = this.GetCityName();
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"未找到城市ID为" + this.cityID + Global.GetLang("的配置信息")
			});
		}
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickCity);
	}

	private void OnClickCity(GameObject go)
	{
		if (this.OnSelectCity != null)
		{
			this.OnSelectCity.Invoke(this);
		}
	}

	private string GetCityName()
	{
		if (this.m_cityInfo == null)
		{
			return string.Empty;
		}
		return Global.GetColorStringForNGUIText(new object[]
		{
			this.ShiLiColor[(int)this.m_shiLiType],
			this.m_cityInfo.Name
		});
	}

	public int cityID;

	public UILabel lblCityName;

	public UISprite imgShiLi;

	public GameObject goTeXiao;

	public Action<ShiLiBattlePartCity> OnSelectCity;

	private string[] ShiLiColor = new string[]
	{
		"FFFFFF",
		"B73838",
		"3681FF",
		"FAC60D"
	};

	private MUForceCraft m_cityInfo;

	private ShiLiType m_shiLiType;

	private bool m_beInBattle;
}
