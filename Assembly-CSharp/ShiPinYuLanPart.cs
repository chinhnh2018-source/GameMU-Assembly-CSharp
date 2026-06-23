using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;

public class ShiPinYuLanPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		base.StartCoroutine(this.InitUI());
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Super.HideNetWaiting();
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.m_OBC = this._ListBox.ItemsSource;
		this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(e, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
	}

	private IEnumerator InitUI()
	{
		this.m_ActiveLst = Global.GetRoleDecorationList().FindAll((GoodsData e) => 1 == e.Using);
		XElement xml = Global.GetGameResXml("Config/OrnamentGroup.xml");
		if (xml != null)
		{
			Super.ShowNetWaiting(null);
			List<XElement> lst = Global.GetXElementList(xml, "OrnamentGroup");
			if (lst != null && 0 < lst.Count)
			{
				List<ShiPinYuLanPart.XmlData> lstActive = new List<ShiPinYuLanPart.XmlData>();
				List<ShiPinYuLanPart.XmlData> lstnotActive = new List<ShiPinYuLanPart.XmlData>();
				List<ShiPinYuLanPart.XmlData> lstAll = new List<ShiPinYuLanPart.XmlData>();
				yield return null;
				for (int i = 0; i < lst.Count; i++)
				{
					ShiPinYuLanPart.XmlData data = new ShiPinYuLanPart.XmlData();
					data.xml = lst[i];
					data.Number = 0;
					data.HaveActive = true;
					data.SetActice(this.m_ActiveLst);
					if (data.HaveActive)
					{
						lstActive.Add(data);
					}
					else
					{
						lstnotActive.Add(data);
					}
				}
				yield return null;
				if (0 < lstActive.Count)
				{
					lstActive.Sort((ShiPinYuLanPart.XmlData x, ShiPinYuLanPart.XmlData y) => x.ID - y.ID);
					lstAll.AddRange(lstActive);
				}
				if (0 < lstnotActive.Count)
				{
					lstnotActive.Sort(delegate(ShiPinYuLanPart.XmlData x, ShiPinYuLanPart.XmlData y)
					{
						if (y.Number == x.Number && y.Number == 0)
						{
							return x.ID - y.ID;
						}
						return y.Number - x.Number;
					});
					lstAll.AddRange(lstnotActive);
				}
				for (int j = 0; j < lstAll.Count; j++)
				{
					yield return null;
					ShiPinYuLanItem item = U3DUtils.NEW<ShiPinYuLanItem>();
					item.RefreshUI(lstAll[j].xml, lstAll[j].GoodsArray, lstAll[j].Active);
					this.m_OBC.AddNoUpdate(item);
					item.DelectPanel = true;
					item.DraggablePanel = this._DraggablePanel;
				}
			}
		}
		Super.HideNetWaiting();
		this._ListBox.repositionNow = true;
		yield break;
	}

	public GButton _CloseBtn;

	public UIDraggablePanel _DraggablePanel;

	public ListBox _ListBox;

	public UILabel _TitleLabel;

	private ObservableCollection m_OBC;

	private List<GoodsData> m_ActiveLst;

	public DPSelectedItemEventHandler Hander;

	private class XmlData
	{
		public int[] GoodsArray
		{
			get
			{
				if (this._GoodsArray == null)
				{
					string xelementAttributeStr = Global.GetXElementAttributeStr(this.xml, "OrnamentGoods");
					if (!string.IsNullOrEmpty(xelementAttributeStr))
					{
						string[] array = xelementAttributeStr.Split(new char[]
						{
							'|'
						});
						this._GoodsArray = new int[array.Length];
						if (array != null && 0 < array.Length)
						{
							for (int i = 0; i < array.Length; i++)
							{
								this._GoodsArray[i] = array[i].SafeToInt32(0);
							}
						}
					}
				}
				return this._GoodsArray;
			}
		}

		public int ID
		{
			get
			{
				if (this._ID == -1)
				{
					this._ID = Global.GetXElementAttributeInt(this.xml, "ID");
				}
				return this._ID;
			}
		}

		public void SetActice(List<GoodsData> UsingGoods)
		{
			this._Actice = new bool[this.GoodsArray.Length];
			for (int i = 0; i < this.GoodsArray.Length; i++)
			{
				this._Actice[i] = false;
				int GoodsID = this.GoodsArray[i];
				if (UsingGoods.Find((GoodsData e) => GoodsID == e.GoodsID) == null)
				{
					this.HaveActive = false;
				}
				else
				{
					this.Number++;
					this._Actice[i] = true;
				}
			}
		}

		public bool[] Active
		{
			get
			{
				return this._Actice;
			}
		}

		public XElement xml;

		public int Number;

		public bool HaveActive;

		private int[] _GoodsArray;

		private int _ID = -1;

		private bool[] _Actice;
	}

	public delegate void voidDelegate();
}
