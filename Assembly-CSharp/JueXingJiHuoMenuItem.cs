using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;
using XMLCreater;

public class JueXingJiHuoMenuItem : MonoBehaviour
{
	public void Awake()
	{
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component != null)
		{
			this.m_height = component.size.y;
		}
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnMenuItemClick);
		this.SetSelectState(false);
	}

	public void SetMenuContent(MUAwakenSuitDetail taoZhuang)
	{
		this.m_taoZhuang = taoZhuang;
		this.lblName.text = string.Concat(new object[]
		{
			Global.GetLang(taoZhuang.Name),
			" ",
			this.GetOwnJueXingShiNum(),
			"/",
			this.m_taoZhuang.AwakenIDs.Count
		});
		if (this.hasCanJiHuoItem())
		{
			this.objAni.SetActive(true);
		}
		else
		{
			this.objAni.SetActive(false);
		}
	}

	public void RefershContent()
	{
		this.lblName.text = string.Concat(new object[]
		{
			Global.GetLang(this.m_taoZhuang.Name),
			" ",
			this.GetOwnJueXingShiNum(),
			"/",
			this.m_taoZhuang.AwakenIDs.Count
		});
		if (this.hasCanJiHuoItem())
		{
			this.objAni.SetActive(true);
		}
		else
		{
			this.objAni.SetActive(false);
		}
	}

	public MUAwakenSuitDetail GetTaoZhuang()
	{
		return this.m_taoZhuang;
	}

	public float GetHeight()
	{
		return this.m_height;
	}

	public void SetSelectState(bool beSelected)
	{
		if (beSelected)
		{
			this.spriteBg.gameObject.SetActive(true);
		}
		else
		{
			this.spriteBg.gameObject.SetActive(false);
		}
	}

	private void OnMenuItemClick(GameObject go)
	{
		if (this.OnMenuItemSelect != null)
		{
			this.OnMenuItemSelect.Invoke(this);
		}
	}

	private int GetOwnJueXingShiNum()
	{
		return JueXingData.GetJiHuoJueXingShiNum(this.m_taoZhuang);
	}

	private bool hasCanJiHuoItem()
	{
		bool result = false;
		for (int i = 0; i < this.m_taoZhuang.AwakenIDs.Count; i++)
		{
			int num = this.m_taoZhuang.AwakenIDs[i];
			if (!JueXingData.IsSelfJiHuo(num, this.m_taoZhuang.ID))
			{
				MUAwakenActivationDetail jueXingShiInfoById = JueXingData.GetJueXingShiInfoById(num);
				int materialNum = JueXingData.GetMaterialNum(jueXingShiInfoById.Material.MaterialId);
				int num2 = jueXingShiInfoById.Material.Num;
				if (materialNum >= num2)
				{
					return true;
				}
			}
		}
		return result;
	}

	public UISprite spriteBg;

	public UILabel lblName;

	public GameObject objAni;

	public Action<JueXingJiHuoMenuItem> OnMenuItemSelect;

	private float m_height = 35f;

	private MUAwakenSuitDetail m_taoZhuang;
}
