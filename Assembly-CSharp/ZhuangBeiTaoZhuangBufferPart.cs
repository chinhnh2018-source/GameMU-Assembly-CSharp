using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhuangBeiTaoZhuangBufferPart : UserControl
{
	private void InitTextInPrefabs()
	{
		if (this.ConstTexts != null && this.ConstTexts.Length == 3)
		{
			this.ConstTexts[0].Text = Global.GetLang("强化效果加成");
			this.ConstTexts[1].Text = Global.GetLang("当前效果");
			this.ConstTexts[2].Text = Global.GetLang("下级效果");
		}
		this.m_txtCurBuffValue.X = 10.0;
		this.m_txtNextBuffValue.X = 10.0;
		this.m_LabelCurActive.X = 220.0;
		this.m_LabelNextActive.X = 205.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		};
	}

	public void RefreshUI(int currentTaoZhuangLevel)
	{
		List<TaoZhuangVO> taoZhuangList = Global.GetTaoZhuangList();
		TaoZhuangVO taoZhuangVO = null;
		if (currentTaoZhuangLevel == -1)
		{
			this.Bak.transform.localScale = new Vector3(this.BackLocalScale.x, this.BackLocalScale.y - 50f, this.BackLocalScale.z);
			this.NextLevelContainer.transform.localPosition = new Vector3(this.NextContainerLocalPosition.x, this.NextContainerLocalPosition.y + 50f, this.NextContainerLocalPosition.z);
			string text = "f9f702";
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				Global.GetLang("无")
			});
			this.m_txtCurBuffValue.text = colorStringForNGUIText;
			this.m_txtCurShengMingValue.text = string.Empty;
			this.m_txtCurShangHaiValue.text = string.Empty;
			this.m_LabelCurActive.text = string.Empty;
			taoZhuangVO = taoZhuangList[0];
			int num = 0;
			Dictionary<int, GoodsData> roleUsingGoodsDataList = Super.GData.RoleUsingGoodsDataList;
			if (roleUsingGoodsDataList.Count > 0)
			{
				foreach (KeyValuePair<int, GoodsData> keyValuePair in roleUsingGoodsDataList)
				{
					if (Global.IsCalcTaoZhuangProps(keyValuePair.Value))
					{
						if (keyValuePair.Value.Forge_level >= taoZhuangVO.Level)
						{
							num++;
						}
					}
				}
			}
			this.m_txtNextBuffValue.text = string.Format(Global.GetLang("任意8件装备强化+{0} ({1}/8)"), taoZhuangVO.Level, num);
			this.m_txtNextShengMingValue.text = string.Format(Global.GetLang("生命上限 +{0}%"), taoZhuangVO.HPAdded * 100f);
			this.m_txtNextShangHaiValue.text = string.Format(Global.GetLang("伤害加成 +{0}%"), taoZhuangVO.HurtAdded * 100f);
			this.m_LabelNextActive.text = Global.GetLang("【未激活】");
		}
		else if (currentTaoZhuangLevel >= taoZhuangList.Count - 1)
		{
			this.Bak.transform.localScale = new Vector3(this.BackLocalScale.x, this.BackLocalScale.y - 50f, this.BackLocalScale.z);
			TaoZhuangVO taoZhuangVO2 = taoZhuangList[taoZhuangList.Count - 1];
			this.m_txtCurBuffValue.text = string.Format(Global.GetLang("任意8件装备强化+{0} (8/8)"), taoZhuangVO2.Level);
			this.m_txtCurShengMingValue.text = string.Format(Global.GetLang("生命上限 +{0}%"), taoZhuangVO2.HPAdded * 100f);
			this.m_txtCurShangHaiValue.text = string.Format(Global.GetLang("伤害加成 +{0}%"), taoZhuangVO2.HurtAdded * 100f);
			this.m_LabelCurActive.text = Global.GetLang("【已激活】");
			string text2 = "f9f702";
			string colorStringForNGUIText2 = Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				Global.GetLang("无")
			});
			this.m_txtNextBuffValue.text = colorStringForNGUIText2;
			this.m_txtNextShengMingValue.text = string.Empty;
			this.m_txtNextShangHaiValue.text = string.Empty;
			this.m_LabelNextActive.text = string.Empty;
		}
		else
		{
			TaoZhuangVO taoZhuangVO2 = taoZhuangList[currentTaoZhuangLevel];
			this.m_txtCurBuffValue.text = string.Format(Global.GetLang("任意8件装备强化+{0} (8/8)"), taoZhuangVO2.Level);
			this.m_txtCurShengMingValue.text = string.Format(Global.GetLang("生命上限 +{0}%"), taoZhuangVO2.HPAdded * 100f);
			this.m_txtCurShangHaiValue.text = string.Format(Global.GetLang("伤害加成 +{0}%"), taoZhuangVO2.HurtAdded * 100f);
			this.m_LabelCurActive.text = Global.GetLang("【已激活】");
			taoZhuangVO = taoZhuangList[currentTaoZhuangLevel + 1];
			int num2 = 0;
			Dictionary<int, GoodsData> roleUsingGoodsDataList2 = Super.GData.RoleUsingGoodsDataList;
			if (roleUsingGoodsDataList2.Count > 0)
			{
				foreach (KeyValuePair<int, GoodsData> keyValuePair2 in roleUsingGoodsDataList2)
				{
					if (Global.IsCalcTaoZhuangProps(keyValuePair2.Value))
					{
						if (keyValuePair2.Value.Forge_level >= taoZhuangVO.Level)
						{
							num2++;
						}
					}
				}
			}
			this.m_txtNextBuffValue.text = string.Format(Global.GetLang("任意8件装备强化+{0} ({1}/8)"), taoZhuangVO.Level, num2);
			this.m_txtNextShengMingValue.text = string.Format(Global.GetLang("生命上限 +{0}%"), taoZhuangVO.HPAdded * 100f);
			this.m_txtNextShangHaiValue.text = string.Format(Global.GetLang("伤害加成 +{0}%"), taoZhuangVO.HurtAdded * 100f);
			this.m_LabelNextActive.text = Global.GetLang("【未激活】");
		}
	}

	public TextBlock m_txtCurBuffValue;

	public TextBlock m_txtCurShengMingValue;

	public TextBlock m_txtCurShangHaiValue;

	public TextBlock m_txtNextBuffValue;

	public TextBlock m_txtNextShengMingValue;

	public TextBlock m_txtNextShangHaiValue;

	public TextBlock m_LabelCurActive;

	public TextBlock m_LabelNextActive;

	public GButton CloseBtn;

	public UISprite Bak;

	public GameObject NextLevelContainer;

	public TextBlock[] ConstTexts;

	public DPSelectedItemEventHandler DPSelectedItem;

	private Vector3 BackLocalScale = new Vector3(338f, 280f, 1f);

	private Vector3 NextContainerLocalPosition = new Vector3(-130f, 75f, 0f);
}
