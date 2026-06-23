using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class HongBaoPaiHangItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_ObservableCollection = this.m_ListBox.ItemsSource;
		base.InitializeComponent();
		this.m_Btn.Text = Global.GetLang("领取");
	}

	public JieriHongBaoKingItemData LingQuData
	{
		set
		{
			this.ID = value.RoleID;
			this.Name = value.Rolename;
			this.Number = value.TotalRecv;
			if (value.GetAwardTimes > 0)
			{
				if (Global.Data.RoleID == value.RoleID)
				{
					this.SetisEnabled(0);
				}
				else
				{
					this.SetisEnabled(1);
				}
			}
			else if (Global.Data.RoleID == value.RoleID)
			{
				this.SetisEnabled(2);
			}
			else
			{
				this.SetisEnabled(1);
			}
		}
	}

	public JieRiHongBaoBang XmlData
	{
		set
		{
			this.xmlData = value;
			this.m_Btn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = value.ID
				});
			};
			this.m_LabXianZhi.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"fe0000",
				Global.GetLang("最低抢到") + value.MinNum + Global.GetLang("个红包")
			}));
			if (value.Ranking <= 3)
			{
				this.m_ShowRanking.URL = "NetImages/GameRes/Images/HongBao/ranking" + value.Ranking + ".png";
			}
			else
			{
				NGUITools.SetActive(this.m_ShowRanking.gameObject, false);
				NGUITools.SetActive(this.m_LabMingCi.gameObject, true);
				this.m_LabMingCi.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("第") + value.Ranking + Global.GetLang("名")
				}));
			}
			this.SetList(value);
			this.m_Box.size = new Vector3(780f, 90f, 1f);
		}
	}

	public void SetisEnabled(int key)
	{
		if (key == 0)
		{
			NGUITools.SetActive(this.m_Btn.gameObject, false);
			NGUITools.SetActive(this.m_GameYiLingQu, true);
		}
		else if (key == 1)
		{
			NGUITools.SetActive(this.m_Btn.gameObject, true);
			NGUITools.SetActive(this.m_GameYiLingQu, false);
			this.m_Btn.isEnabled = false;
		}
		else if (key == 2)
		{
			NGUITools.SetActive(this.m_Btn.gameObject, true);
			NGUITools.SetActive(this.m_GameYiLingQu, false);
			this.m_Btn.isEnabled = true;
		}
	}

	public int ID
	{
		get
		{
			return this.m_ID;
		}
		set
		{
			this.m_ID = value;
		}
	}

	private new string Name
	{
		set
		{
			this.m_LabName.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				value
			}));
		}
	}

	private int Number
	{
		set
		{
			if (value < this.xmlData.MinNum)
			{
				this.m_Btn.isEnabled = false;
			}
			else
			{
				this.m_Btn.isEnabled = true;
			}
		}
	}

	private void SetList(JieRiHongBaoBang data)
	{
		string goodsIDs = string.Empty;
		string goodsStr = string.Empty;
		string effect = string.Empty;
		if (!string.IsNullOrEmpty(data.GoodsTwo))
		{
			goodsIDs = data.GoodsOne + "@" + data.GoodsTwo;
		}
		else
		{
			goodsIDs = data.GoodsOne;
		}
		goodsStr = data.GoodsThr;
		effect = data.EffectiveTime;
		if (!string.IsNullOrEmpty(data.GoodsOne))
		{
			Super.LoadGoodsList(goodsIDs, this.m_ObservableCollection);
		}
		if (!string.IsNullOrEmpty(data.GoodsThr))
		{
			Super.LoadOtherGoodsList(goodsStr, this.m_ObservableCollection, effect);
		}
		GGoodIcon[] componentsInChildren = this.m_ListBox.GetComponentsInChildren<GGoodIcon>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UIPanel component = componentsInChildren[i].GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
	}

	public UILabel m_LabMingCi;

	public UILabel m_LabName;

	public ListBox m_ListBox;

	public GButton m_Btn;

	public UILabel m_LabXianZhi;

	public ShowNetImage m_ShowRanking;

	public BoxCollider m_Box;

	public GameObject m_GameYiLingQu;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection m_ObservableCollection;

	private JieRiHongBaoBang xmlData;

	private int m_ID = -1;
}
