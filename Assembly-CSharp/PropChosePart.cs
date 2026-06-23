using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class PropChosePart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefab();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.ChoseChange(this.m_Index);
	}

	private void InitPrefab()
	{
		for (byte b = 0; b < 3; b += 1)
		{
			this.m_ItemList.Add(new PropChosePart.PropItem(this._PropOb[(int)b], this._PropChoseObj[(int)b], this._Title[(int)b], this._Prop[(int)b]));
			this._BgSp[(int)b].alpha = 0.6f;
		}
	}

	private void InitPrefabText()
	{
		this._TitleBig.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("卓越属性选择")
		});
		this._OKBtn.Label.text = Global.GetLang("确认");
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(this, new DPSelectedItemEventArgs
				{
					ID = this.m_Index,
					IDType = 0
				});
			}
		};
		this._OKBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(this, new DPSelectedItemEventArgs
				{
					ID = this.m_Index,
					IDType = 1
				});
			}
		};
		UIEventListener.Get(this._PropOb[0].transform.GetChild(2).gameObject).onClick = delegate(GameObject o)
		{
			this.ChoseChange(0);
		};
		UIEventListener.Get(this._PropOb[1].transform.GetChild(2).gameObject).onClick = delegate(GameObject o)
		{
			this.ChoseChange(1);
		};
		UIEventListener.Get(this._PropOb[2].transform.GetChild(2).gameObject).onClick = delegate(GameObject o)
		{
			this.ChoseChange(2);
		};
	}

	private void ChoseChange(int index)
	{
		this.m_Index = index;
		byte b = 0;
		while ((int)b < this._PropChoseObj.Length)
		{
			NGUITools.SetActive(this._PropChoseObj[(int)b], (int)b == index);
			b += 1;
		}
	}

	public void SetInf(string[] strInf)
	{
		if (strInf != null)
		{
			for (int i = 0; i < strInf.Length; i++)
			{
				if (!string.IsNullOrEmpty(strInf[i]))
				{
					if (i < this.m_ItemList.Count)
					{
						this.m_ItemList[i].Title = Global.GetColorStringForNGUIText(new object[]
						{
							"fac60d",
							string.Format(Global.GetLang("卓越属性 ：{0}"), this.GetLineNum(strInf[i]))
						});
						this.m_ItemList[i].Prop = strInf[i];
						this.m_ItemList[i].bChose = (0 == i);
					}
				}
				else if (i < this.m_ItemList.Count)
				{
					this.m_ItemList[i].bShow = false;
				}
			}
		}
	}

	private int GetLineNum(string str)
	{
		string[] array = str.Split(new char[]
		{
			'\n'
		});
		if (array != null)
		{
			return array.Length;
		}
		return 0;
	}

	public GameObject[] _PropOb;

	public GameObject[] _PropChoseObj;

	public TextBlock[] _Title;

	public TextBlock[] _Prop;

	public GButton _CloseBtn;

	public TextBlock _TitleBig;

	public UISprite[] _BgSp;

	public GButton _OKBtn;

	private List<PropChosePart.PropItem> m_ItemList = new List<PropChosePart.PropItem>();

	private int m_Index;

	public DPSelectedItemEventHandler Hander;

	private class PropItem
	{
		public PropItem(GameObject obj, GameObject ChoseObj, TextBlock title, TextBlock Prop)
		{
			this.m_obj = obj;
			this.m_ChoseObj = ChoseObj;
			this.m_Title = title;
			this.m_Prop = Prop;
		}

		public bool bShow
		{
			set
			{
				if (null != this.m_obj)
				{
					NGUITools.SetActive(this.m_obj, value);
				}
			}
		}

		public string Title
		{
			set
			{
				if (null != this.m_Title)
				{
					this.m_Title.Text = Global.GetColorStringForNGUIText(new object[]
					{
						"fac60d",
						value
					});
				}
			}
		}

		public string Prop
		{
			set
			{
				if (null != this.m_Prop)
				{
					this.m_Prop.LineHeight = 16;
					this.m_Prop.Text = value;
					Vector3 localPosition = this.m_Prop.transform.localPosition;
					localPosition.z = -1f;
					this.m_Prop.transform.localPosition = localPosition;
				}
			}
		}

		public bool bChose
		{
			set
			{
				NGUITools.SetActive(this.m_ChoseObj, value);
			}
		}

		private GameObject m_obj;

		private GameObject m_ChoseObj;

		private TextBlock m_Title;

		private TextBlock m_Prop;
	}
}
