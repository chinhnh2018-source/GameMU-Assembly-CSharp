using System;
using UnityEngine;
using XMLCreater;

public class JueXingPartSelectItem : MonoBehaviour
{
	private void Awake()
	{
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickItem);
	}

	private void OnClickItem(GameObject go)
	{
		if (this.OnItemSelect != null)
		{
			this.OnItemSelect.Invoke(this);
		}
	}

	public MUAwakenSuitDetail GetTaoZhuang()
	{
		return this.m_taoZhuang;
	}

	public void SetEquip(bool beEquiped)
	{
		this.m_textureEquip = this.beEquipIcon.gameObject.GetComponent<UITexture>();
		this.m_textureEquip.enabled = beEquiped;
	}

	public void SetSelect(bool beSelected)
	{
		this.guangquan.enabled = beSelected;
	}

	public void SetTaoZhuang(MUAwakenSuitDetail taoZhuang)
	{
		this.m_taoZhuang = taoZhuang;
		this.icon.URL = JueXingData.GetJueXingTaoZhuangIconUrl(this.m_taoZhuang);
	}

	public ShowNetImage icon;

	public UISprite guangquan;

	public ShowNetImage beEquipIcon;

	private MUAwakenSuitDetail m_taoZhuang;

	private UITexture m_textureEquip;

	public Action<JueXingPartSelectItem> OnItemSelect;
}
