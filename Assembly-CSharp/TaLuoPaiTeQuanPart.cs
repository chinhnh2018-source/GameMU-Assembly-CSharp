using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TaLuoPaiTeQuanPart : UserControl
{
	protected override void OnDestroy()
	{
		this.StopTimer();
		base.OnDestroy();
	}

	protected override void InitializeComponent()
	{
		this.init();
		this.m_BtnKing.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.ShowTaLuoPaiTeQuan2Part();
		};
	}

	public List<TaLuoPaiItem> ListTaLuoPaiItem
	{
		get
		{
			return this.m_ListTaLuoPaiItem;
		}
		set
		{
			if (value != null)
			{
				if (value.Count == 3)
				{
					NGUITools.SetActive(this.m_LabelWeiJiHuo, false);
					for (int i = 0; i < value.Count; i++)
					{
						TaLuoPaiTeQuanItem taLuoPaiTeQuanItem = U3DUtils.NEW<TaLuoPaiTeQuanItem>();
						U3DUtils.AddChild(base.gameObject, taLuoPaiTeQuanItem.gameObject, false);
						taLuoPaiTeQuanItem.transform.localPosition = this.m_ItemPosition[i];
						TaLuoPaiItem taLuoPaiItem = U3DUtils.NEW<TaLuoPaiItem>();
						U3DUtils.AddChild(taLuoPaiTeQuanItem.gameObject, taLuoPaiItem.gameObject, false);
						if (null != taLuoPaiItem)
						{
							if (null != taLuoPaiItem.GetComponent<BoxCollider>())
							{
								taLuoPaiItem.GetComponent<BoxCollider>().size = new Vector3(240f, 319f, 0f);
							}
							if (null != taLuoPaiItem.m_JingYans)
							{
								NGUITools.SetActive(taLuoPaiItem.m_JingYans, false);
							}
							taLuoPaiItem.IsActivate = value[i].IsActivate;
							taLuoPaiItem.ItemGoodsId = value[i].ItemGoodsId;
							taLuoPaiItem.Name = value[i].Name;
							taLuoPaiItem.Level = value[i].Level;
							taLuoPaiItem.transform.localPosition = new Vector3(0f, 0f, 0f);
							taLuoPaiItem.transform.localScale = new Vector3(1f, 1f, 1f);
							taLuoPaiTeQuanItem.TaLuoPaiItem = taLuoPaiItem;
							taLuoPaiTeQuanItem.TaLuoPaiItem.Level = value[i].Level;
							taLuoPaiItem.ExtraLevel = value[i].ExtraLevel;
							if (value[i].ExtraLevel > 0)
							{
								taLuoPaiTeQuanItem.str = this.GetPropretyStr(value[i].ItemGoodsId, value[i].ExtraLevel);
								taLuoPaiTeQuanItem.m_UILabel.text = Global.GetColorStringForNGUIText(new object[]
								{
									"17e43e",
									string.Format("{0}", taLuoPaiTeQuanItem.str)
								});
							}
							else
							{
								NGUITools.SetActive(taLuoPaiTeQuanItem.m_sp, false);
							}
							this.m_ListObj.Add(taLuoPaiTeQuanItem);
						}
						if (!taLuoPaiItem.IsActivate)
						{
							NGUITools.SetActive(this.m_LabelWeiJiHuo, true);
						}
					}
					this.m_BtnKing.Text = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Format("{0}", Global.GetLang("重置"))
					});
				}
				else if (value.Count == 0)
				{
					this.LeftBtn();
				}
				else
				{
					this.LeftBtn();
				}
			}
		}
	}

	public void setItem(List<TaLuoPaiItem> list)
	{
		NGUITools.SetActive(this.m_LabelWeiJiHuo, false);
		for (int i = 0; i < list.Count; i++)
		{
			if (!list[i].IsActivate)
			{
				NGUITools.SetActive(this.m_LabelWeiJiHuo, true);
			}
			this.m_ListObj[i].TaLuoPaiItem.ItemGoodsId = list[i].ItemGoodsId;
			this.m_ListObj[i].TaLuoPaiItem.IsActivate = list[i].IsActivate;
			this.m_ListObj[i].TaLuoPaiItem.Name = list[i].Name;
			this.m_ListObj[i].TaLuoPaiItem.Level = list[i].Level;
			this.m_ListObj[i].TaLuoPaiItem.ExtraLevel = list[i].ExtraLevel;
			this.m_ListObj[i].TaLuoPaiItem.IsActivate = list[i].IsActivate;
			if (list[i].ExtraLevel > 0)
			{
				this.m_ListObj[i].str = this.GetPropretyStr(list[i].ItemGoodsId, list[i].Level + list[i].ExtraLevel);
				this.m_ListObj[i].m_UILabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format("{0}", Global.GetLang(this.m_ListObj[i].str))
				});
				if (null != this.m_TaLuoPaiTeQuan2Part)
				{
					this.CloseTaLuoPaiKingItemPart();
				}
			}
			else
			{
				NGUITools.SetActive(this.m_ListObj[i].m_sp, false);
			}
		}
		this.m_BtnKing.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			string.Format("{0}", Global.GetLang("重置"))
		});
	}

	public long StartTime
	{
		get
		{
			return this.m_StartTime;
		}
		set
		{
			this.m_StartTime = value;
		}
	}

	public long BufferSecs
	{
		get
		{
			return this.m_BufferSecs;
		}
		set
		{
			this.m_BufferSecs = value;
			if (this.m_StartTime != -1L && this.m_BufferSecs != -1L)
			{
				this.m_Time = this.m_BufferSecs - (Global.GetCorrectLocalTime() - this.m_StartTime) / 1000L;
			}
		}
	}

	public void StartTimer()
	{
		if (this.m_Timer != null)
		{
			this.m_Timer.Tick = null;
			this.m_Timer.Stop();
			this.m_Timer = null;
		}
		this.m_Timer = new DispatcherTimer("TaLuoPaiTeQuanPart_Timer");
		this.m_Timer.Interval = TimeSpan.FromSeconds(1.0);
		this.m_Timer.Tick = new DispatcherTimerEventHandler(this.Timer_Tick);
		this.m_Timer.Start();
	}

	public void StopTimer()
	{
		if (this.m_Timer != null)
		{
			this.m_Timer.Tick = null;
			this.m_Timer.Stop();
			this.m_Timer.Dispose();
			this.m_Timer = null;
		}
		this.Visibility = false;
	}

	private void init()
	{
		this.m_LabelWeiJiHuo.pivot = 5;
		this.m_LableTimer.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ff0000",
			string.Format("{0}", Global.GetLang(string.Empty))
		});
		this.m_LabelWeiJiHuo.text = Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			string.Format("{0}", Global.GetLang("未激活的塔罗牌需要激活才可使用"))
		});
		this.m_BtnKing.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			string.Format("{0}", Global.GetLang("重置"))
		});
		NGUITools.SetActive(this.m_LabelWeiJiHuo, false);
	}

	private string GetPropretyStr(int goodsId, int Level)
	{
		string text = string.Empty;
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsId);
		if (goodsXmlNodeByID != null)
		{
			double[] equipProps = goodsXmlNodeByID.EquipProps;
			for (int i = 0; i < equipProps.Length; i++)
			{
				if (0.0 < equipProps[i])
				{
					if (dictionary.ContainsKey(i))
					{
						dictionary[i] = equipProps[i];
					}
					else
					{
						dictionary.Add(i, equipProps[i]);
					}
				}
			}
			foreach (KeyValuePair<int, double> keyValuePair in dictionary)
			{
				int key = keyValuePair.Key;
				if (key != 0)
				{
					if (ConfigExtPropIndexes.GetPercentByID(key))
					{
						if (equipProps.Length == 177)
						{
							double num = equipProps[key] * 100.0 * (double)Level;
							text = text + string.Format(Global.GetLang("&{0}：{1}%"), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true), num) + Environment.NewLine;
						}
					}
					else if (equipProps.Length == 177)
					{
						int[] array = Array.ConvertAll<double, int>(goodsXmlNodeByID.EquipProps, (double d) => (int)d);
						double num2 = (double)(array[key] * Level);
						text = text + string.Format(Global.GetLang("&{0}：{1}"), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true), num2) + Environment.NewLine;
					}
				}
			}
		}
		return text;
	}

	private void ShowTaLuoPaiTeQuan2Part()
	{
		if (null == this.m_TaLuoPaiTeQuan2Part)
		{
			this.m_TalLuoPaiTeQuan2Wind = U3DUtils.NEW<GChildWindow>();
			this.m_TalLuoPaiTeQuan2Wind.ModalType = ChildWindowModalType.Translucent;
			this.m_TalLuoPaiTeQuan2Wind.transform.SetParent(Super.GData.PlayZoneRoot.Children.transform, false);
			this.m_TalLuoPaiTeQuan2Wind.transform.localPosition = new Vector3(0f, 0f, WindowManage.GetZ(base.transform.parent.parent) - 10f);
			UIEventListener.Get(this.m_TalLuoPaiTeQuan2Wind.ModalBak).onClick = delegate(GameObject e)
			{
				this.CloseTaLuoPaiKingItemPart();
			};
			this.m_TalLuoPaiTeQuan2Wind.ChildWindowClose = delegate(object e, EventArgs s)
			{
				this.CloseTaLuoPaiKingItemPart();
				return true;
			};
			this.m_TaLuoPaiTeQuan2Part = U3DUtils.NEW<TaLuoPaiTeQuan2Part>();
			this.m_TalLuoPaiTeQuan2Wind.Body.Add(this.m_TaLuoPaiTeQuan2Part);
			this.m_TaLuoPaiTeQuan2Part.m_BtnLeft.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.LeftBtn();
			};
			this.m_TaLuoPaiTeQuan2Part.m_BtnRight.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.CloseTaLuoPaiKingItemPart();
			};
		}
	}

	private void CloseTaLuoPaiKingItemPart()
	{
		if (null != this.m_TalLuoPaiTeQuan2Wind)
		{
			Object.Destroy(this.m_TalLuoPaiTeQuan2Wind.gameObject);
		}
		if (null != this.m_TaLuoPaiTeQuan2Part)
		{
			Object.Destroy(this.m_TaLuoPaiTeQuan2Part.gameObject);
		}
		this.m_TalLuoPaiTeQuan2Wind = null;
		this.m_TaLuoPaiTeQuan2Part = null;
	}

	private void LeftBtn()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.SendUsingKingTeQuan();
		this.CloseTaLuoPaiKingItemPart();
	}

	private void Timer_Tick(object sender, object e)
	{
		if (this.m_Time > 0L)
		{
			this.m_Time -= 1L;
			DateTime dateTime;
			dateTime..ctor(this.m_Time);
			int num = (int)(this.m_Time / 3600L);
			int num2 = (int)(this.m_Time % 3600L / 60L);
			int num3 = (int)(this.m_Time % 3600L % 60L);
			if (0 < num)
			{
				this.m_LableTimer.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format(Global.GetLang("剩余：{0}时{1}分{2}秒"), num, num2, num3)
				});
			}
			else if (0 < num2)
			{
				this.m_LableTimer.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format(Global.GetLang("剩余：{0}分{1}秒"), num2, num3)
				});
			}
			else
			{
				this.m_LableTimer.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format(Global.GetLang("剩余：{0}秒"), num3)
				});
			}
		}
	}

	public UILabel m_LabelWeiJiHuo;

	public GButton m_BtnKing;

	public GameObject m_shuxings;

	public TaLuoPaiTeQuan2Part m_TaLuoPaiTeQuan2Part;

	public GButton m_CloseBtn;

	public List<TaLuoPaiTeQuanItem> m_ListObj = new List<TaLuoPaiTeQuanItem>();

	public UILabel m_LableTimer;

	public GChildWindow m_TalLuoPaiTeQuan2Wind;

	private List<TaLuoPaiItem> m_ListTaLuoPaiItem = new List<TaLuoPaiItem>();

	private Vector3[] m_ItemPosition = new Vector3[]
	{
		new Vector3(-280f, 85.38f, -0.1f),
		new Vector3(8.5f, 85.38f, -0.1f),
		new Vector3(280f, 85.38f, -0.1f)
	};

	private DispatcherTimer m_Timer;

	private long m_BufferSecs = -1L;

	private long m_StartTime = -1L;

	private long m_Time = -1L;

	private List<GameObject> m_ListShuxing = new List<GameObject>();
}
