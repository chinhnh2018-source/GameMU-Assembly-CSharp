using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class EquipDiamondAttributes : UserControl
{
	private void InitTextInPrefabs()
	{
		this.attributeTxt.Text = Global.GetLang("激活属性");
		this.unloadAllBtn.Text = Global.GetLang("全部卸载");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.unloadAllBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 101,
				Data = this.index
			});
		};
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.inlayBtn_1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnButtonClicked(1);
		};
		this.inlayBtn_2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnButtonClicked(2);
		};
		this.inlayBtn_3.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnButtonClicked(3);
		};
		if (null != this.attributeListBox)
		{
			ObservableCollection itemsSource = this.attributeListBox.ItemsSource;
			itemsSource.AddNoUpdate(this.diamondAttributeObj_1);
			itemsSource.AddNoUpdate(this.diamondAttributeObj_2);
			itemsSource.AddNoUpdate(this.diamondAttributeObj_3);
			itemsSource.DelayUpdate();
		}
	}

	private void OnButtonClicked(int shapeType)
	{
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			ID = this.index,
			Data = shapeType
		});
	}

	public int slotID
	{
		get
		{
			return this.index;
		}
		set
		{
			this.index = value;
			this.SetEquipPartImage(this.index);
		}
	}

	public Dictionary<int, GoodsData> dic_equipedDiamond
	{
		set
		{
			this.dic_diamondData = value;
			this.SetDiamondsIcon();
			this.SetEquipedDiamondAttributes();
			this.SetUnloadBtnStatus();
			this.SetTipsIcons();
		}
	}

	private void SetEquipPartImage(int slotID)
	{
		if (null != this.equipPart)
		{
			this.equipPart.URL = "NetImages/GameRes/Images/FluorescentDiamond/equip_part_" + slotID + ".png";
		}
	}

	private void SetUnloadBtnStatus()
	{
		if (this.dic_diamondData != null && this.dic_diamondData.Count > 0)
		{
			this.unloadAllBtn.isEnabled = true;
		}
		else
		{
			this.unloadAllBtn.isEnabled = false;
		}
	}

	private void SetDiamondsIcon()
	{
		Dictionary<int, GoodsData> dictionary = this.dic_diamondData;
		if (dictionary == null)
		{
			dictionary = new Dictionary<int, GoodsData>();
		}
		for (int i = 1; i <= 3; i++)
		{
			GoodsData value = dictionary.GetValue(i);
			this.SetDiamondIconByType(i, value);
			this.SetDiamondLevelByType(i, (value == null) ? -1 : value.GoodsID);
		}
	}

	private void SetDiamondIconByType(int shapeID, GoodsData gd)
	{
		if (shapeID < 1 || shapeID > 3)
		{
			return;
		}
		string url = null;
		if (gd != null)
		{
			url = Global.GetGoodsIconString(Global.GetGoodsIconCodeByID(gd.GoodsID));
		}
		switch (shapeID)
		{
		case 1:
			this.diamondIcon_1.URL = url;
			break;
		case 2:
			this.diamondIcon_2.URL = url;
			break;
		case 3:
			this.diamondIcon_3.URL = url;
			break;
		}
	}

	private void SetDiamondLevelByType(int shapeType, int goodsID)
	{
		if (shapeType < 1 || shapeType > 3)
		{
			return;
		}
		TextBlock textBlock = null;
		switch (shapeType)
		{
		case 1:
			textBlock = this.level_diamond_1;
			break;
		case 2:
			textBlock = this.level_diamond_2;
			break;
		case 3:
			textBlock = this.level_diamond_3;
			break;
		}
		if (null == textBlock)
		{
			return;
		}
		textBlock.Text = ((goodsID <= 0) ? string.Empty : ("Lv" + Global.GetDiamondLevelByGoodsID(goodsID)));
	}

	private void SetEquipedDiamondAttributes()
	{
		Dictionary<int, GoodsData> dictionary = this.dic_diamondData;
		if (dictionary == null)
		{
			dictionary = new Dictionary<int, GoodsData>();
		}
		for (int i = 1; i <= 3; i++)
		{
			GoodsData value = dictionary.GetValue(i);
			int num = -1;
			if (value != null)
			{
				DiamondAttribute diamondAttributeByGoodsID = Global.GetDiamondAttributeByGoodsID(value.GoodsID, out num);
				if (diamondAttributeByGoodsID != null)
				{
					int type = diamondAttributeByGoodsID.type;
				}
			}
			int num2 = (value == null) ? -1 : value.GoodsID;
			this.SetDiamondIconAtSlot(i, num2);
			this.SetDiamondAttributeByType(i, num2);
			this.SetDiamondAttributeStatusByType(i, num2 > 0);
		}
		if (null != this.attributeListBox)
		{
			this.attributeListBox.repositionNow = true;
		}
	}

	private void SetDiamondAttributeStatusByType(int shapeType, bool visible = true)
	{
		if (shapeType < 1 || shapeType > 3)
		{
			return;
		}
		GameObject gameObject = null;
		switch (shapeType)
		{
		case 1:
			gameObject = this.diamondAttributeObj_1;
			break;
		case 2:
			gameObject = this.diamondAttributeObj_2;
			break;
		case 3:
			gameObject = this.diamondAttributeObj_3;
			break;
		}
		gameObject.SetActive(visible);
	}

	private void SetDiamondAttributeByType(int shapeType, int goodsID)
	{
		if (shapeType < 1 || shapeType > 3)
		{
			return;
		}
		TextBlock diamondLabel = null;
		switch (shapeType)
		{
		case 1:
			diamondLabel = this.diamondAttribute_1;
			break;
		case 2:
			diamondLabel = this.diamondAttribute_2;
			break;
		case 3:
			diamondLabel = this.diamondAttribute_3;
			break;
		}
		this.SetDiamondAttributeByGoodsID(diamondLabel, goodsID);
	}

	private void SetDiamondAttributeByGoodsID(TextBlock diamondLabel, int goodsID)
	{
		if (null == diamondLabel)
		{
			return;
		}
		if (goodsID > 0)
		{
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsID);
			diamondLabel.Text = Global.GetBaseAttributeStrFromPropertyList(goodsEquipPropsDoubleList, true, 0);
			MUDebug.Log<string>(new string[]
			{
				"attribute text: " + diamondLabel.Text
			});
		}
		else
		{
			diamondLabel.Text = string.Empty;
		}
	}

	public void SetDiamondIconAtSlot(int shapeID, int goodsID)
	{
		if (shapeID < 1 || shapeID > 3)
		{
			return;
		}
		string url = null;
		if (goodsID > 0)
		{
			url = Global.GetGoodsIconString(Global.GetGoodsIconCodeByID(goodsID));
		}
		switch (shapeID)
		{
		case 1:
			this.diamondIcon_attribute_1.URL = url;
			break;
		case 2:
			this.diamondIcon_attribute_2.URL = url;
			break;
		case 3:
			this.diamondIcon_attribute_3.URL = url;
			break;
		}
	}

	private void SetTipsIcons()
	{
		Dictionary<int, GoodsData> dictionary = this.dic_diamondData;
		if (dictionary == null)
		{
			dictionary = new Dictionary<int, GoodsData>();
		}
		this.tipsIcon_1.SetActive(Global.IsUpgradableDiamondAtSlot(dictionary, 1));
		this.tipsIcon_2.SetActive(Global.IsUpgradableDiamondAtSlot(dictionary, 2));
		this.tipsIcon_3.SetActive(Global.IsUpgradableDiamondAtSlot(dictionary, 3));
	}

	private const string netImagePath = "NetImages/GameRes/Images/FluorescentDiamond/";

	private const string imagePrefix = "equip_part_";

	private const string imageSuffix = ".png";

	public GButton inlayBtn_1;

	public GButton inlayBtn_2;

	public GButton inlayBtn_3;

	public ShowNetImage equipPart;

	public ShowNetImage diamondIcon_1;

	public ShowNetImage diamondIcon_2;

	public ShowNetImage diamondIcon_3;

	public TextBlock level_diamond_1;

	public TextBlock level_diamond_2;

	public TextBlock level_diamond_3;

	public GameObject tipsIcon_1;

	public GameObject tipsIcon_2;

	public GameObject tipsIcon_3;

	public ListBox attributeListBox;

	public GameObject diamondAttributeObj_1;

	public GameObject diamondAttributeObj_2;

	public GameObject diamondAttributeObj_3;

	public ShowNetImage diamondIcon_attribute_1;

	public ShowNetImage diamondIcon_attribute_2;

	public ShowNetImage diamondIcon_attribute_3;

	public TextBlock diamondAttribute_1;

	public TextBlock diamondAttribute_2;

	public TextBlock diamondAttribute_3;

	public GButton unloadAllBtn;

	public GButton closeBtn;

	public TextBlock attributeTxt;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int index;

	private Dictionary<int, GoodsData> dic_diamondData;
}
