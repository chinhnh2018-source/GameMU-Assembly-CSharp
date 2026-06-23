using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class GTab : UserControl
{
	public Color NomailTextColor
	{
		get
		{
			return this.m_NomailTextColor;
		}
		set
		{
			if (!this.m_NomailTextColor.Equals(value))
			{
				this.m_NomailTextColor = value;
			}
		}
	}

	public void Init()
	{
		for (int i = 0; i < this.TabBtns.Count; i++)
		{
			this.TabBtns[i].transform.localPosition = this.GetTabPos(i);
			this.TabBtns[i].name = "TabBtn_" + i;
			this.TabBtns[i].MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.SetTab(s as GameObject);
			};
			this.btnNormalImg = this.TabBtns[i].normalSprite;
			this.btnHoverImg = this.TabBtns[i].hoverSprite;
		}
		for (int j = 0; j < this.TabPages.Count; j++)
		{
			this.TabPages[j].name = "TabPage_" + j;
		}
		this.SetTabBtn(this.startedPageIdx);
		this.SetTabPage(this.startedPageIdx);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.Body = U3DUtils.NEW<ContentPresenter>();
		if (this.m_bInitNow)
		{
			this.Init();
		}
	}

	public GTab.Arrangement arrangement
	{
		get
		{
			return this._arrangement;
		}
		set
		{
			this._arrangement = value;
			this.ReArrangement(value);
		}
	}

	private void OnDrawGizmosSelected()
	{
		this.ReArrangement(this.arrangement);
	}

	private void SetTabSize(int tabSize)
	{
		int count = this.TabBtns.Count;
		GameObject original = Resources.Load("Prefabs/UI/TabButton") as GameObject;
		for (int i = this.TabBtns.Count; i < count + tabSize; i++)
		{
			GameObject gameObject = SpawnManager.Instantiate(original) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.layer = base.transform.gameObject.layer;
			gameObject.name = "TabBtn_" + i;
			gameObject.transform.localPosition = this.GetTabPos(i);
			gameObject.GetComponent<GButton>().MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.SetTab(s as GameObject);
			};
			GameObject gameObject2 = new GameObject();
			gameObject2.transform.parent = base.transform;
			gameObject2.name = "TabPage" + i;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.rotation = Quaternion.identity;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject2.layer = base.transform.gameObject.layer;
			gameObject2.AddComponent<UIPanel>();
			gameObject2.name = "TabPage_" + i;
			if (i != 0)
			{
				gameObject2.transform.gameObject.SetActive(false);
			}
			this.TabBtns.Add(gameObject.GetComponent<GButton>());
			this.TabPages.Add(gameObject2.GetComponent<UIPanel>());
		}
	}

	public void AddTabPage(int count)
	{
		this.SetTabSize(count - 2);
	}

	public void AddPageContent(GameObject pageContent, int idx)
	{
		if (this.TabPages.Count > 0 && idx < this.TabPages.Count)
		{
			UIPanel uipanel = this.TabPages[idx];
			pageContent.transform.parent = uipanel.transform;
			this.ResetTabPage(pageContent);
		}
	}

	public void SetTabButtonBackground(string btnNormal, string btnHover)
	{
		for (int i = 0; i < this.TabBtns.Count; i++)
		{
			GButton gbutton = this.TabBtns[i];
			GButton component = gbutton.transform.GetComponent<GButton>();
			component.normalSprite = btnNormal;
			component.hoverSprite = btnHover;
			component.pressedSprite = btnHover;
		}
		this.btnNormalImg = btnNormal;
		this.btnHoverImg = btnHover;
	}

	public void SetTabButtonName(string btnLabelName, int tag)
	{
		this.TabBtns[tag].Label.text = btnLabelName;
	}

	private void SetTab(GameObject btn)
	{
		string[] array = btn.name.Split(new char[]
		{
			'_'
		});
		int num = int.Parse(array[1]);
		this.SetTabBtn(num);
		this.SetTabPage(num);
	}

	public void SetActivePage(int idx)
	{
		if (idx < 0 || idx > this.TabBtns.Count || idx > this.TabPages.Count)
		{
			return;
		}
		this.SetTabBtn(idx);
		this.SetTabPage(idx);
	}

	private void SetTabBtn(int index)
	{
		if (this.TabBtns != null)
		{
			foreach (GButton gbutton in this.TabBtns)
			{
				string[] array = gbutton.name.Split(new char[]
				{
					'_'
				});
				if (int.Parse(array[1]) == index)
				{
					gbutton.normalSprite = this.btnHoverImg;
					gbutton.hoverSprite = this.btnHoverImg;
					gbutton.pressedSprite = this.btnHoverImg;
					UILabel componentInChildren = gbutton.GetComponentInChildren<UILabel>();
					if (null != componentInChildren)
					{
						componentInChildren.color = this.m_HotTextColor;
					}
				}
				else
				{
					gbutton.normalSprite = this.btnNormalImg;
					gbutton.hoverSprite = this.btnNormalImg;
					gbutton.pressedSprite = this.btnNormalImg;
					UILabel componentInChildren2 = gbutton.GetComponentInChildren<UILabel>();
					if (null != componentInChildren2)
					{
						componentInChildren2.color = this.m_NomailTextColor;
					}
				}
				gbutton.Refresh();
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = index
				});
				this.SelectIndex = index;
			}
		}
	}

	private void SetTabPage(int index)
	{
		if (this.TabPages != null)
		{
			foreach (UIPanel uipanel in this.TabPages)
			{
				string[] array = uipanel.name.Split(new char[]
				{
					'_'
				});
				if (int.Parse(array[1]) == index)
				{
					uipanel.transform.gameObject.SetActive(true);
					if (this.showTabFadeInTimeSec > 0.0001f)
					{
						TweenAlpha tweenAlpha = uipanel.transform.gameObject.GetComponent<TweenAlpha>();
						if (!tweenAlpha)
						{
							tweenAlpha = uipanel.transform.gameObject.AddComponent<TweenAlpha>();
						}
						tweenAlpha.enabled = true;
						tweenAlpha.from = 0f;
						tweenAlpha.to = 1f;
						tweenAlpha.Reset();
						tweenAlpha.duration = 0.4f;
						tweenAlpha.onFinished = null;
						tweenAlpha.Play(true);
					}
				}
				else
				{
					uipanel.transform.gameObject.SetActive(false);
				}
			}
		}
	}

	private void ResetTabPage(GameObject pageContent)
	{
		pageContent.transform.localPosition = Vector3.zero;
		pageContent.transform.localScale = Vector3.one;
	}

	private Vector3 GetTabPos(int index)
	{
		if (this._arrangement == GTab.Arrangement.Horizontal)
		{
			return new Vector3(this.BtnsPosition.x + (float)index * this.BtnsSize.x, this.BtnsPosition.y, 0f);
		}
		if (this._arrangement == GTab.Arrangement.Vertical)
		{
			return new Vector3(this.BtnsPosition.x, this.BtnsPosition.y + (float)index * this.BtnsSize.y, 0f);
		}
		return new Vector3(this.BtnsPosition.x, this.BtnsPosition.y + (float)index * this.BtnsSize.y, 0f);
	}

	private void ReArrangement(GTab.Arrangement arrangement)
	{
		for (int i = 0; i < this.TabBtns.Count; i++)
		{
			this.TabBtns[i].transform.localPosition = this.GetTabPos(i);
		}
		for (int j = 0; j < this.TabPages.Count; j++)
		{
			this.ResetTabPage(this.TabPages[j].gameObject);
		}
	}

	public string TabItemOrientation
	{
		set
		{
			this.Head.Orientation = value;
		}
	}

	public double TabItemWidth
	{
		set
		{
			this.Head.Width = value;
		}
	}

	public Thickness TabItemPos
	{
		set
		{
			this.Head.Margin = value;
			this.Head.X = value.Left;
			this.Head.Y = value.Top;
		}
	}

	public double TabItemHeight
	{
		set
		{
			this.Head.Height = value;
		}
	}

	public double TabWidth
	{
		get
		{
			return this.Container.Width;
		}
		set
		{
			this.Container.Width = value;
		}
	}

	public double TabHeight
	{
		get
		{
			return this.Container.Height;
		}
		set
		{
			this.Container.Height = value;
		}
	}

	public double BodyLeft
	{
		set
		{
		}
	}

	public double BodyTop
	{
		set
		{
		}
	}

	public void SetBody(object content)
	{
		this.Body.Content = content;
	}

	public uint TabItemTextColor_Normal
	{
		set
		{
			this._tabItemTextColor_Normal = value;
		}
	}

	public uint TabItemTextColor_Active
	{
		set
		{
			this._tabItemTextColor_Active = value;
		}
	}

	public void AddItem(int width, int height, int margin, string defaultImageUri, string newImageUri, string hitDefaultImageUri, string hitNewImageUri, string text)
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = (double)width;
		gicon.Height = (double)height;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage(defaultImageUri), (double)width, (double)height, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage(newImageUri), (double)width, (double)height, 3.0, 2.0));
		gicon.HitBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage(hitDefaultImageUri), (double)width, (double)height, 3.0, 2.0));
		gicon.HitNewBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage(hitNewImageUri), (double)width, (double)height, 3.0, 2.0));
		gicon.Text = text;
		gicon.TextColor = new SolidColorBrush(this._tabItemTextColor_Normal);
		gicon.Margin = new Thickness((double)margin, 0.0, 0.0, 0.0);
		if (this.tabItemList.Count == 0)
		{
			gicon.Hit = true;
			gicon.TextColor = new SolidColorBrush(this._tabItemTextColor_Active);
			gicon.Container.Background = gicon.HitBodySource;
		}
		gicon.addEventListener("mouseDown", new MouseEventHandler(this.TabItem_MouseDown));
		this.Head.Children.Add(gicon);
		this.tabItemList.Add(gicon);
	}

	private void TabItem_MouseDown(MouseEvent e)
	{
		GIcon gicon = e.currentTarget as GIcon;
		if (gicon)
		{
			foreach (GIcon gicon2 in this.tabItemList)
			{
				if (gicon2 == gicon)
				{
					gicon2.Hit = true;
					gicon2.Container.Background = gicon2.HitNewBodySource;
					gicon2.TextColor = new SolidColorBrush(this._tabItemTextColor_Active);
				}
				else
				{
					gicon2.Hit = false;
					gicon2.Container.Background = gicon2.BodySource;
					gicon2.TextColor = new SolidColorBrush(this._tabItemTextColor_Normal);
				}
			}
			this.SelectionChanged(this, gicon);
		}
	}

	public void ShowItem(string tabName, bool visibility)
	{
		for (int i = 0; i < this.tabItemList.Count; i++)
		{
			if (tabName == this.tabItemList[i].Text)
			{
				this.tabItemList[i].Visibility = visibility;
			}
		}
	}

	public void SelectItem(params object[] args)
	{
		if (args[0] is int)
		{
			this.SelectItem1((int)args[0]);
		}
		else if (args[0] is string)
		{
			this.SelectItem2(args[0] as string);
		}
	}

	public void SelectItem1(int index)
	{
		for (int i = 0; i < this.tabItemList.Count; i++)
		{
			if (i == index)
			{
				this.tabItemList[i].Hit = true;
				this.tabItemList[i].Container.Background = this.tabItemList[i].HitNewBodySource;
				this.tabItemList[i].TextColor = new SolidColorBrush(10551295U);
			}
			else
			{
				this.tabItemList[i].Hit = false;
				this.tabItemList[i].Container.Background = this.tabItemList[i].BodySource;
				this.tabItemList[i].TextColor = new SolidColorBrush(6592150U);
			}
		}
	}

	public void SelectItem2(string tabName)
	{
		for (int i = 0; i < this.tabItemList.Count; i++)
		{
			if (tabName == this.tabItemList[i].Text.ToString())
			{
				this.tabItemList[i].Hit = true;
				this.tabItemList[i].Container.Background = this.tabItemList[i].HitNewBodySource;
				this.tabItemList[i].TextColor = new SolidColorBrush(uint.MaxValue);
			}
			else
			{
				this.tabItemList[i].Hit = false;
				this.tabItemList[i].Container.Background = this.tabItemList[i].BodySource;
				this.tabItemList[i].TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 2, 232, 253));
			}
		}
	}

	private const float TAB_BUTTON_START_POS_X = -342f;

	private const float TAB_BUTTON_START_POS_Y = 212f;

	private const float TAB_BUTTON_START_POS_X_INC = 93f;

	private const float TAB_BUTTON_START_POS_Y_INC = -37f;

	private const float TAB_PAGE_HOR_POS_X = 1f;

	private const float TAB_PAGE_HOR_POS_Y = -12f;

	private const float TAB_PAGE_HOR_SCALE_X = 1f;

	private const float TAB_PAGE_HOR_SCALE_Y = 0.9f;

	private const float TAB_PAGE_VER_POS_X = 48f;

	private const float TAB_PAGE_VER_POS_Y = 2f;

	private const float TAB_PAGE_VER_SCALE_X = 0.84f;

	private const float TAB_PAGE_VER_SCALE_Y = 0.96f;

	public Color m_NomailTextColor = Color.white;

	public Color m_HotTextColor = Color.white;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	public bool m_bInitNow = true;

	public Vector3 BtnsPosition = new Vector3(-342f, 212f, 0f);

	public Vector3 BtnsSize = new Vector3(93f, -37f, 0f);

	public List<GButton> TabBtns;

	public List<UIPanel> TabPages;

	public GTab.Arrangement _arrangement;

	private string btnNormalImg;

	private string btnHoverImg;

	public int startedPageIdx;

	public float showTabFadeInTimeSec;

	public int SelectIndex;

	private StackPanel Head;

	private ContentPresenter Body;

	private uint _tabItemTextColor_Normal = 6592150U;

	private uint _tabItemTextColor_Active = 10551295U;

	public List<GIcon> tabItemList = new List<GIcon>();

	public ObjectEventHandler2 SelectionChanged;

	public enum Arrangement
	{
		Horizontal,
		Vertical
	}
}
