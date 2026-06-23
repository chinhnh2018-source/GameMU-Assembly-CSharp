using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class GTabControl : UserControl
{
	public GButton SelectedItem { get; protected set; }

	public uint BtnTextPressedColor
	{
		get
		{
			return this._btnTextPressedColor;
		}
		set
		{
			this._btnTextPressedColor = value;
		}
	}

	public uint BtnTextNormalColor
	{
		get
		{
			return this._btnTextNormalColor;
		}
		set
		{
			this._btnTextNormalColor = value;
		}
	}

	public string BtnPrefabName
	{
		get
		{
			return this._btnPrefabName;
		}
		set
		{
			this._btnPrefabName = value;
		}
	}

	public int SelectIndex
	{
		get
		{
			return this._SelectIndex;
		}
		set
		{
			this.SetTabBtn(this._SelectIndex);
		}
	}

	public int SelectIndex_IfNotChangeHappen
	{
		get
		{
			return this._SelectIndex_IfNotChangeHappen;
		}
	}

	public void Init()
	{
		for (int i = 0; i < this.TabBtns.Count; i++)
		{
			this.TabBtns[i].transform.localPosition = this.GetTabPos(i);
			int index = i;
			this.TabBtns[i].name = "TabBtn_" + i;
			this.TabBtns[i].MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				e.Index = index;
				if (this.BeforeTabBtnClick != null)
				{
					this.BeforeTabBtnClick(s, e);
				}
				else
				{
					this.SetTab(s as GameObject);
				}
			};
			this.btnNormalImg = this.TabBtns[i].normalSprite;
			this.btnHoverImg = this.TabBtns[i].pressedSprite;
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
		this.Init();
	}

	public GTabControl.Arrangement arrangement
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

	public int GetTabCount()
	{
		return this.TabBtns.Count;
	}

	private void SetTabSize(int tabSize)
	{
		int count = this.TabBtns.Count;
		GameObject gameObject = this.TabBtns[0].gameObject;
		for (int i = this.TabBtns.Count; i < count + tabSize; i++)
		{
			GameObject gameObject2 = SpawnManager.Instantiate(gameObject) as GameObject;
			gameObject2.transform.parent = ((!(null == this._Btns)) ? this._Btns.transform : base.transform);
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.rotation = Quaternion.identity;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject2.layer = base.transform.gameObject.layer;
			gameObject2.name = "TabBtn_" + i;
			gameObject2.transform.localPosition = this.GetTabPos(i);
			int index = i;
			gameObject2.GetComponent<GButton>().MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				e.Index = index;
				if (this.BeforeTabBtnClick != null)
				{
					this.BeforeTabBtnClick(s, e);
				}
				else
				{
					this.SetTab(s as GameObject);
				}
			};
			GameObject gameObject3 = new GameObject();
			gameObject3.transform.parent = ((!(null == this._Pages)) ? this._Pages.transform : base.transform);
			gameObject3.name = "TabPage" + i;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.rotation = Quaternion.identity;
			gameObject3.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject3.layer = base.transform.gameObject.layer;
			gameObject3.AddComponent<UIPanel>();
			gameObject3.name = "TabPage_" + i;
			if (i != 0)
			{
				gameObject3.transform.gameObject.SetActive(false);
			}
			this.TabBtns.Add(gameObject2.GetComponent<GButton>());
			this.TabPages.Add(gameObject3.GetComponent<UIPanel>());
		}
	}

	public void AddTabPage(int count)
	{
		this.SetTabSize(count - this.TabBtns.Count);
	}

	public void ClearPages()
	{
		int count = this.TabBtns.Count;
		for (int i = 2; i < count; i++)
		{
			GameObject gameObject = this.TabBtns[2].gameObject;
			gameObject.transform.parent = null;
			Object.Destroy(gameObject);
			gameObject = this.TabPages[2].gameObject;
			gameObject.transform.parent = null;
			Object.Destroy(gameObject);
			this.TabBtns.RemoveAt(2);
			this.TabPages.RemoveAt(2);
		}
		foreach (Transform transform in this.TabPages[0].gameObject.GetComponentsInChildren<Transform>())
		{
			if (transform != this.TabPages[0].transform)
			{
				transform.parent = null;
				Object.Destroy(transform.gameObject);
			}
		}
		foreach (Transform transform2 in this.TabPages[1].gameObject.GetComponentsInChildren<Transform>())
		{
			if (transform2 != this.TabPages[1].transform)
			{
				transform2.parent = null;
				Object.Destroy(transform2.gameObject);
			}
		}
	}

	public void AddPageContent(GameObject pageContent, int idx)
	{
		if (this.TabPages.Count > 0 && idx >= 0 && idx < this.TabPages.Count)
		{
			UIPanel uipanel = this.TabPages[idx];
			if (null == pageContent)
			{
				U3DUtils.DestoryAllChild(uipanel.gameObject);
			}
			else
			{
				pageContent.transform.parent = uipanel.transform;
				this.ResetTabPage(pageContent);
			}
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

	public void SetMaskBtn(int tag, GongNengIDs gongNengIDs)
	{
		GButton gbutton = this.TabBtns[tag];
		gbutton.gameObject.AddComponent<TabButtonOpen>().SetTabState(gongNengIDs, "roleTab_normal", null, null);
	}

	public void SetTab(GameObject btn)
	{
		string[] array = btn.name.Split(new char[]
		{
			'_'
		});
		int num = int.Parse(array[1]);
		this.SetTabBtn(num);
		this.SetTabPage(num);
		if (this.OnTabBtnClick != null)
		{
			this.OnTabBtnClick(this, new DPSelectedItemEventArgs
			{
				Index = num
			});
		}
	}

	public void SetActivePage(int idx)
	{
		if (idx < 0 || idx > this.TabBtns.Count || idx > this.TabPages.Count)
		{
			return;
		}
		this.SetTabBtn(idx);
		this.SetTabPage(idx);
		if (this.OnTabBtnClick != null)
		{
			this.OnTabBtnClick(this, new DPSelectedItemEventArgs
			{
				Index = idx
			});
		}
	}

	private void SetTabBtn(int index)
	{
		if (this.TabBtns != null)
		{
			if (index != this._SelectIndex)
			{
				this._SelectIndex_IfNotChangeHappen = this._SelectIndex;
			}
			this._SelectIndex = index;
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
					if (gbutton.Label != null)
					{
						gbutton.Label.color = NGUIMath.HexToColorEx(this.BtnTextPressedColor);
					}
					this.SelectedItem = gbutton;
					if (this.SelectionChanged != null)
					{
						this.SelectionChanged(this, gbutton);
					}
				}
				else
				{
					gbutton.normalSprite = this.btnNormalImg;
					gbutton.hoverSprite = this.btnNormalImg;
					gbutton.pressedSprite = this.btnNormalImg;
					if (gbutton.Label != null)
					{
						gbutton.Label.color = NGUIMath.HexToColorEx(this.BtnTextNormalColor);
					}
				}
				gbutton.Refresh();
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
		if (this._arrangement == GTabControl.Arrangement.Horizontal)
		{
			return new Vector3(this.BtnsPosition.x + (float)index * this.BtnsSize.x, this.BtnsPosition.y, 0f);
		}
		if (this._arrangement == GTabControl.Arrangement.Vertical)
		{
			return new Vector3(this.BtnsPosition.x, this.BtnsPosition.y + (float)index * this.BtnsSize.y, 0f);
		}
		return new Vector3(this.BtnsPosition.x, this.BtnsPosition.y + (float)index * this.BtnsSize.y, 0f);
	}

	private void ReArrangement(GTabControl.Arrangement arrangement)
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
			if (tabName == this.tabItemList[i].Text)
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

	public DPSelectedItemEventHandler OnTabBtnClick;

	public MouseLeftButtonUpEventHandler BeforeTabBtnClick;

	public Vector3 BtnsPosition = new Vector3(-342f, 212f, 0f);

	public Vector3 BtnsSize = new Vector3(93f, -37f, 0f);

	public GameObject _Btns;

	public GameObject _Pages;

	public List<GButton> TabBtns;

	public List<UIPanel> TabPages;

	public GTabControl.Arrangement _arrangement;

	private string btnNormalImg;

	private string btnHoverImg;

	public int startedPageIdx;

	private uint _btnTextPressedColor = 16766048U;

	private uint _btnTextNormalColor = 7697781U;

	private string _btnPrefabName = "TabButton";

	private int _SelectIndex;

	private int _SelectIndex_IfNotChangeHappen;

	public bool NeedMoreEffect_diffPosition;

	public float normalHeight;

	public float normalWidth;

	public float hoverHeight;

	public float hoverWidth;

	public float normalPosX;

	public float normalPosY;

	public float hoverPosX;

	public float hoverPosY;

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
