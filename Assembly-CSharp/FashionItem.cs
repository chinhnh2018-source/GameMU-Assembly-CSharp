using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class FashionItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		BoxCollider component = base.GetComponent<BoxCollider>();
		if (null != component)
		{
			component.size = this.m_Bg.transform.localScale;
		}
		this.m_Time_S = new TimeSpan(10000000L);
	}

	public override void Destroy()
	{
		this.StopTimer();
		base.Destroy();
	}

	private void StartTime()
	{
		if (this.m_Timer_ != null)
		{
			this.m_Timer_.Tick = null;
			this.m_Timer_.Stop();
			this.m_Timer_ = null;
		}
		this.m_Timer_ = new DispatcherTimer(string.Format("FashitemTine_{0}", this.m_GoodsID));
		this.m_Timer_.Tick = new DispatcherTimerEventHandler(this.TimeGo);
		this.m_Timer_.Interval = TimeSpan.FromSeconds(1.0);
		this.m_Timer_.Start();
	}

	private void TimeGo(object sender, EventArgs args)
	{
		double num = this.m_LifeTime.TotalSeconds;
		num -= 1.0;
		if (0.0 >= num)
		{
			if (this.Hander != null)
			{
				this.Hander(base.gameObject, new DPSelectedItemEventArgs
				{
					ID = this.m_GoodsID
				});
			}
		}
		else
		{
			this.m_TimeLabel.text = ((!this.m_IsForeverFashion) ? this.GetLiftTimeString(this.m_LifeTime.Ticks) : string.Empty);
		}
		this.m_LifeTime -= this.m_Time_S;
	}

	private void StopTimer()
	{
		if (this.m_Timer_ != null)
		{
			this.m_Timer_.Tick = null;
			this.m_Timer_.Stop();
			this.m_Timer_.Dispose();
			this.m_Timer_ = null;
		}
	}

	protected override void OnDestroy()
	{
		this.StopTimer();
		base.OnDestroy();
	}

	private void InitPrefabText()
	{
		this.m_Namelabel.text = string.Empty;
		this.m_TimeLabel.text = string.Empty;
	}

	private void InitTexture()
	{
		this.m_BgUsing.URL = "NetImages/GameRes/Images/Fashionwardrobe/IsUsing.png";
	}

	private void InitHandler()
	{
	}

	private long GetLiftTime(string EndTime)
	{
		DateTime dateTime = DateTime.Parse(EndTime);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (dateTime < correctDateTime)
		{
			return dateTime.Ticks;
		}
		return dateTime.Ticks - correctDateTime.Ticks;
	}

	private string GetLiftTimeString(long time)
	{
		if (time <= 0L)
		{
			return string.Empty;
		}
		TimeSpan lifeTime;
		lifeTime..ctor(time);
		this.m_LifeTime = lifeTime;
		string empty = string.Empty;
		int num = this.MyDoubleToInt(lifeTime.TotalDays);
		int num2 = this.MyDoubleToInt(lifeTime.TotalHours);
		int num3 = this.MyDoubleToInt(lifeTime.TotalMinutes);
		string text = (lifeTime.Days <= 0) ? string.Empty : string.Format(Global.GetLang("{0}天"), num);
		string text2 = (num2 <= 0) ? string.Empty : string.Format(Global.GetLang("{0}时"), lifeTime.Hours);
		string text3 = (99 >= num) ? ((0 >= num3) ? string.Empty : string.Format(Global.GetLang("{0}分"), lifeTime.Minutes)) : string.Empty;
		string text4 = (0 >= num2) ? string.Format(Global.GetLang("{0}秒"), lifeTime.Seconds) : string.Empty;
		return Global.GetColorStringForNGUIText(new object[]
		{
			"ff0000",
			string.Format(Global.GetLang("{0}{1}{2}{3}"), new object[]
			{
				text,
				text2,
				text3,
				text4
			})
		});
	}

	private int MyDoubleToInt(double value)
	{
		string text = value.ToString();
		string[] array = text.Split(new char[]
		{
			'.'
		});
		if (array.Length < 1)
		{
			return Mathf.FloorToInt((float)value);
		}
		return int.Parse(array[0]);
	}

	public void RefreshFashionItemInf(GoodsData data)
	{
		this.m_Namelabel.text = Global.GetGoodsNameByID(data.GoodsID, true) + ((0 >= data.Forge_level) ? string.Empty : Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			string.Format(" +{0}", data.Forge_level)
		}));
		this.m_TimeLabel.text = ((!this.m_IsForeverFashion) ? this.GetLiftTimeString(this.GetLiftTime(data.Endtime)) : string.Empty);
		GGoodIcon ggoodIcon = FashionWardrobePart.initGood(data, false);
		ggoodIcon.ContentText.Text = string.Empty;
		ggoodIcon.transform.SetParent(this.m_GoodIconRoot, false);
		Vector3 localPosition = ggoodIcon.transform.localPosition;
		localPosition.z = -1f;
		ggoodIcon.transform.localPosition = localPosition;
		UIPanel component = ggoodIcon.GetComponent<UIPanel>();
		if (null != component)
		{
			Object.Destroy(component);
		}
		this.m_GoodsID = data.GoodsID;
		this.m_FashLevel = data.Forge_level;
		this.m_GoodsData = data;
		NGUITools.SetActive(this.m_UsingBgObj, 1 == data.Using);
		ggoodIcon.BindingSprite.gameObject.SetActive(false);
		if (!string.IsNullOrEmpty(this.m_TimeLabel.text))
		{
			this.StartTime();
		}
		UIPanel component2 = this.m_UsingBgObj.GetComponent<UIPanel>();
		if (null != component2)
		{
			Object.Destroy(component2);
		}
	}

	public void DeletePanel()
	{
		UIPanel component = base.GetComponent<UIPanel>();
		if (null != component)
		{
			Object.Destroy(component);
		}
	}

	public void SetSelect(bool bSelect = true)
	{
		this.m_Bg.spriteName = ((!bSelect) ? "ViewItemBg_0" : "ViewItemBg_1");
	}

	public bool FashionTimeIsOver
	{
		get
		{
			return this.m_FashionTimeIsOver;
		}
		set
		{
			if (!this.m_IsForeverFashion && 0.0 >= this.m_LifeTime.TotalSeconds)
			{
				this.m_FashionTimeIsOver = value;
				this.m_TimeLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("时装已过期")
				});
			}
		}
	}

	public bool IsForeverFashion
	{
		get
		{
			return this.m_IsForeverFashion;
		}
		set
		{
			this.m_IsForeverFashion = value;
		}
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			if (null != value)
			{
				if (null == this.m_DragpanelContents)
				{
					this.m_DragpanelContents = base.GetComponent<UIDragPanelContents>();
					if (null == this.m_DragpanelContents)
					{
						this.m_DragpanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
					}
				}
				this.m_DragpanelContents.draggablePanel = value;
			}
		}
	}

	public int GoodsID
	{
		get
		{
			return this.m_GoodsID;
		}
	}

	public bool IsUsing
	{
		set
		{
			NGUITools.SetActive(this.m_UsingBgObj, value);
		}
	}

	public int Lev
	{
		get
		{
			return this.m_FashLevel;
		}
		set
		{
			this.m_FashLevel = value;
			this.m_Namelabel.text = Global.GetGoodsNameByID(this.m_GoodsData.GoodsID, true) + ((0 >= this.m_FashLevel) ? string.Empty : Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(" +{0}", this.m_FashLevel)
			}));
			this.m_GoodsData.Forge_level = this.m_FashLevel;
		}
	}

	public GoodsData _GoodsData
	{
		get
		{
			return this.m_GoodsData;
		}
	}

	public UILabel m_Namelabel;

	public UILabel m_TimeLabel;

	public Transform m_GoodIconRoot;

	public UISprite m_Bg;

	public GameObject m_UsingBgObj;

	public ShowNetImage m_BgUsing;

	private UIDragPanelContents m_DragpanelContents;

	private int m_GoodsID;

	private int m_FashLevel;

	private GoodsData m_GoodsData;

	private bool m_IsForeverFashion;

	private DispatcherTimer m_Timer_;

	private TimeSpan m_LifeTime;

	private TimeSpan m_Time_S;

	private bool m_FashionTimeIsOver;

	public DPSelectedItemEventHandler Hander;
}
