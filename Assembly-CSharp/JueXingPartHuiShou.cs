using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;
using XMLCreater;

public class JueXingPartHuiShou : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnHuiShou.Text = Global.GetLang("立即回收");
		this.btnClose.Text = Global.GetLang("取消");
		this.btnSelect.Text = Global.GetLang("只回收绑定碎片");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.m_beOnlyBinding = (PlayerPrefs.GetInt("OnlyBingdingStr", 1) == 1);
		this.imgSelectSprte.enabled = this.m_beOnlyBinding;
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.btnHuiShou.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_beShowEnd)
			{
				this.OnClickHuiShou();
			}
			else
			{
				Super.HintMainText(Global.GetLang("碎片加载中，请稍等"), 10, 3);
			}
		};
		this.btnSelect.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_beShowEnd)
			{
				this.OnclickSelect();
			}
			else
			{
				Super.HintMainText(Global.GetLang("碎片加载中，请稍等"), 10, 3);
			}
		};
		this.InitData();
	}

	private void InitData()
	{
		this.m_lstBinging.Clear();
		this.m_lstNotBinding.Clear();
		this.m_lstSelect.Clear();
		List<int> list = new List<int>();
		List<int> selfJiHuoList = JueXingData.GetSelfJiHuoList();
		for (int i = 0; i < selfJiHuoList.Count; i++)
		{
			MUAwakenActivationDetail jueXingShiInfoById = JueXingData.GetJueXingShiInfoById(selfJiHuoList[i]);
			if (jueXingShiInfoById != null)
			{
				int materialId = jueXingShiInfoById.Material.MaterialId;
				if (list.IndexOf(materialId) <= -1)
				{
					list.Add(materialId);
					int totalGoodsCountByID = Global.GetTotalGoodsCountByID(materialId);
					int totalBindingGoodsCountByID = Global.GetTotalBindingGoodsCountByID(materialId);
					int num = totalGoodsCountByID - totalBindingGoodsCountByID;
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(materialId);
					for (int j = 0; j < totalBindingGoodsCountByID; j++)
					{
						this.m_lstBinging.Add(goodsXmlNodeByID);
					}
					for (int k = 0; k < num; k++)
					{
						this.m_lstNotBinding.Add(goodsXmlNodeByID);
					}
					if (this.m_lstBinging.Count >= 100)
					{
						break;
					}
				}
			}
		}
		this.InitContents();
	}

	private void OnClickHuiShou()
	{
		if (this.m_lstSelect.Count == 0)
		{
			Super.HintMainText(Global.GetLang("请选择碎片"), 10, 3);
			return;
		}
		if (this.BeNotBingdingItemSelect())
		{
			string lang = Global.GetLang("本次回收有非绑定碎片，是否确认回收？");
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					string serverFormat2 = this.GetServerFormat();
					this.SendServerHuiShou(serverFormat2);
				}
			}, buttons);
		}
		else
		{
			string serverFormat = this.GetServerFormat();
			this.SendServerHuiShou(serverFormat);
		}
	}

	private void OnclickSelect()
	{
		this.SetOnlyBindingSelect(!this.m_beOnlyBinding);
	}

	private void SetOnlyBindingSelect(bool beOblyBingSelect)
	{
		this.m_beOnlyBinding = beOblyBingSelect;
		this.imgSelectSprte.enabled = beOblyBingSelect;
		PlayerPrefs.SetInt("OnlyBingdingStr", (!this.m_beOnlyBinding) ? 0 : 1);
		if (beOblyBingSelect)
		{
			for (int i = 0; i < this.m_lstAllIcons.Count; i++)
			{
				GGoodIcon ggoodIcon = this.m_lstAllIcons[i];
				if (ggoodIcon.gameObject.activeSelf)
				{
					if (!this.BeBinging(ggoodIcon))
					{
						this.SetIconState(ggoodIcon, false);
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < this.m_lstAllIcons.Count; j++)
			{
				GGoodIcon ggoodIcon2 = this.m_lstAllIcons[j];
				if (ggoodIcon2.gameObject.activeSelf)
				{
					this.SetIconState(ggoodIcon2, true);
				}
			}
		}
		this.RefershSellNum(this.GetCanSellNum());
	}

	public void InitContents()
	{
		if (this.dragPanel != null)
		{
			SpringPanel.Begin(this.dragPanel.gameObject, Vector3.zero, 30f);
		}
		int num = this.m_lstBinging.Count + this.m_lstNotBinding.Count;
		if (num >= 100)
		{
			num = 100;
		}
		int num2 = (num - 1) / 5 + 1;
		num2 = Mathf.Max(num2, 4);
		this.background.localScale = new Vector3(this.background.localScale.x, (float)(num2 * 78), 1f);
		for (int i = 0; i < num; i++)
		{
			if (i >= this.lstContainers.Count)
			{
				GameObject gameObject = new GameObject("item_" + i);
				gameObject.transform.SetParent(this.grid.transform);
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localPosition = Vector3.zero;
				this.lstContainers.Add(gameObject.transform);
			}
		}
		this.grid.Reposition();
		base.StartCoroutine<bool>(this.ShowContents(num));
	}

	private IEnumerator ShowContents(int total)
	{
		this.m_beShowEnd = false;
		int index = 0;
		while (index < this.m_lstBinging.Count && index < total)
		{
			GGoodIcon icon = this.AddIcon(this.m_lstBinging[index], this.lstContainers[index], true, index);
			this.SetIconState(icon, true);
			this.RefershSellNum(this.GetCanSellNum());
			yield return null;
			index++;
		}
		while (index < total)
		{
			GGoodIcon icon2 = this.AddIcon(this.m_lstNotBinding[index - this.m_lstBinging.Count], this.lstContainers[index], false, index);
			if (!this.m_beOnlyBinding)
			{
				this.SetIconState(icon2, true);
			}
			else
			{
				this.SetIconState(icon2, false);
			}
			this.RefershSellNum(this.GetCanSellNum());
			yield return null;
			index++;
		}
		yield return null;
		this.m_beShowEnd = true;
		this.RefershSellNum(this.GetCanSellNum());
		yield break;
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon icon = evt.target.SafeGetComponent<GGoodIcon>();
		this.SetIconState(icon, !this.beIconSelect(icon));
		this.RefershSellNum(this.GetCanSellNum());
	}

	private bool beIconSelect(GGoodIcon icon)
	{
		return this.m_lstSelect.IndexOf(icon) >= 0;
	}

	private bool BeBinging(GGoodIcon icon)
	{
		return icon.BindingSprite.enabled;
	}

	private void SetIconState(GGoodIcon icon, bool beSelect)
	{
		if (beSelect)
		{
			if (!this.beIconSelect(icon))
			{
				this.m_lstSelect.Add(icon);
				icon.BackgroundSprite2.enabled = true;
			}
		}
		else if (this.beIconSelect(icon))
		{
			this.m_lstSelect.Remove(icon);
			icon.BackgroundSprite2.enabled = false;
		}
	}

	private bool BeNotBingdingItemSelect()
	{
		for (int i = 0; i < this.m_lstSelect.Count; i++)
		{
			if (!this.BeBinging(this.m_lstSelect[i]))
			{
				return true;
			}
		}
		return false;
	}

	private GGoodIcon AddIcon(GoodVO goodVO, Transform parent, bool beBinding, int index)
	{
		if (goodVO != null)
		{
			GGoodIcon ggoodIcon;
			if (index < this.m_lstAllIcons.Count)
			{
				ggoodIcon = this.m_lstAllIcons[index];
			}
			else
			{
				ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				this.m_lstAllIcons.Add(ggoodIcon);
				ggoodIcon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
			}
			ggoodIcon.gameObject.SetActive(true);
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				Super.GetIconCode(goodVO)
			}), false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodVO.Categoriy;
			ggoodIcon.ItemCode = goodVO.ID;
			ggoodIcon.ItemObject = goodVO;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.BindingSprite.gameObject.SetActive(true);
			if (beBinding)
			{
				ggoodIcon.BindingSprite.enabled = true;
			}
			else
			{
				ggoodIcon.BindingSprite.enabled = false;
			}
			ggoodIcon.BackSpriteName2 = "iconState_selected";
			ggoodIcon.BackgroundSprite2.enabled = false;
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
			};
			ggoodIcon.transform.SetParent(parent);
			ggoodIcon.transform.localScale = Vector3.one;
			ggoodIcon.transform.localPosition = Vector3.zero;
			return ggoodIcon;
		}
		return null;
	}

	private void RefershSellNum(int num)
	{
		string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
		{
			"ffffff",
			JueXingData.GetJueXingZhiChenNum().ToString(),
			"00ff00",
			"+" + num
		});
		this.lblSellNum.text = colorStringForNGUIText;
	}

	private int GetCanSellNum()
	{
		int num = 0;
		for (int i = 0; i < this.m_lstSelect.Count; i++)
		{
			GGoodIcon ggoodIcon = this.m_lstSelect[i];
			GoodVO goodVO = (GoodVO)ggoodIcon.ItemObject;
			bool flag = this.BeBinging(ggoodIcon);
			int id = goodVO.ID;
			num += JueXingData.GetAwakenNumByGoodsID(id);
		}
		return num;
	}

	private string GetServerFormat()
	{
		string empty = string.Empty;
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
		for (int i = 0; i < this.m_lstSelect.Count; i++)
		{
			GGoodIcon ggoodIcon = this.m_lstSelect[i];
			GoodVO goodVO = (GoodVO)ggoodIcon.ItemObject;
			bool flag = this.BeBinging(ggoodIcon);
			Dictionary<int, int> dictionary3;
			if (flag)
			{
				dictionary3 = dictionary2;
			}
			else
			{
				dictionary3 = dictionary;
			}
			if (!dictionary3.ContainsKey(goodVO.ID))
			{
				dictionary3[goodVO.ID] = 1;
			}
			else
			{
				dictionary3[goodVO.ID] = dictionary3[goodVO.ID] + 1;
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		Dictionary<int, int>.Enumerator enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			StringBuilder stringBuilder2 = stringBuilder;
			string text = "{0},{1},{2}|";
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			object obj = keyValuePair.Key;
			KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
			stringBuilder2.AppendFormat(text, obj, keyValuePair2.Value, 0);
		}
		Dictionary<int, int>.Enumerator enumerator2 = dictionary2.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			StringBuilder stringBuilder3 = stringBuilder;
			string text2 = "{0},{1},{2}|";
			KeyValuePair<int, int> keyValuePair3 = enumerator2.Current;
			object obj2 = keyValuePair3.Key;
			KeyValuePair<int, int> keyValuePair4 = enumerator2.Current;
			stringBuilder3.AppendFormat(text2, obj2, keyValuePair4.Value, 1);
		}
		if (stringBuilder.Length == 0)
		{
			return string.Empty;
		}
		stringBuilder.Remove(stringBuilder.Length - 1, 1);
		return stringBuilder.ToString();
	}

	private void RefreshData()
	{
		for (int i = 0; i < this.m_lstAllIcons.Count; i++)
		{
			this.m_lstAllIcons[i].BodyURL = new ImageURL(string.Empty, false, 0);
			this.m_lstAllIcons[i].BindingSprite.enabled = false;
			this.m_lstAllIcons[i].BackgroundSprite2.enabled = false;
			this.m_lstAllIcons[i].gameObject.SetActive(false);
		}
		this.InitData();
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener("CMD_SPR_JUEXING_HUISHOU", new Action(this.ServerHuiShou));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener("CMD_SPR_JUEXING_HUISHOU", new Action(this.ServerHuiShou));
	}

	private void SendServerHuiShou(string ids)
	{
		GameInstance.Game.JueXingHuiShou(ids);
	}

	private void ServerHuiShou()
	{
		this.RefreshData();
	}

	private const string OnlyBingdingStr = "OnlyBingdingStr";

	private const int MaxRow = 20;

	private const int ColNum = 5;

	private const int aGridSize = 78;

	public DPSelectedItemEventHandler CloseHandler;

	public GButton btnHuiShou;

	public GButton btnClose;

	public GButton btnSelect;

	public UISprite imgSelectSprte;

	public UILabel lblSellNum;

	public Transform background;

	public UIGrid grid;

	public UIDraggablePanel dragPanel;

	private List<GGoodIcon> m_lstSelect = new List<GGoodIcon>();

	private List<GGoodIcon> m_lstAllIcons = new List<GGoodIcon>();

	private List<Transform> lstContainers = new List<Transform>();

	private List<GoodVO> m_lstBinging = new List<GoodVO>();

	private List<GoodVO> m_lstNotBinding = new List<GoodVO>();

	private bool m_beOnlyBinding = true;

	private bool m_beShowEnd = true;
}
