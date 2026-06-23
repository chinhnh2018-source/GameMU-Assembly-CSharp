using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShiZhuangItem : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.equipBtn.MouseLeftButtonUp = delegate(object Sender, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = this.goodsData.GoodsID
			});
		};
	}

	public GoodsData goodsData
	{
		get
		{
			return this.goodData;
		}
		set
		{
			this.goodData = value;
			this.endTime = DateTime.Parse(this.goodsData.Endtime);
			this.CreateDressItem((this.goodsData == null) ? -1 : this.goodsData.GoodsID);
		}
	}

	public bool adorned
	{
		set
		{
			this._equipState = value;
			this.SetEquipState(this._equipState);
			this.SetEquipButtonState(this._equipState);
		}
	}

	private void SetRoleTitle(int fashionGoodsID)
	{
		if (null == this.roleTitleImage)
		{
			return;
		}
		if (fashionGoodsID <= 0)
		{
			this.roleTitleImage.URL = null;
			return;
		}
		this.roleTitleImage.URL = "NetImages/GameRes/Images/ChengHao/title_" + fashionGoodsID + ".png";
		this.roleTitleImage.AutoResize = true;
	}

	private void SetEquipState(bool equiped = true)
	{
		if (null != this.equipState)
		{
			this.equipState.SetActive(equiped);
		}
	}

	public void SetLeftSeconds()
	{
		if (!Global.IsTimeLimitGoods(this.goodsData))
		{
			this.timeLimit.text = Global.GetColorStringForNGUIText(new object[]
			{
				"0EFF04",
				Global.GetLang("永久")
			});
			return;
		}
		base.InvokeRepeating("TickProc", 0f, 1f);
	}

	protected void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		long num = this.endTime.Ticks / 10000L;
		if (num > correctLocalTime)
		{
			int num2 = (int)((num - correctLocalTime) / 1000L);
			if (num2 > 0)
			{
				this.timeLimit.text = Global.GetColorStringForNGUIText(new object[]
				{
					"0EFF04",
					string.Format("{0}{1}", Global.GetLang("剩余时间："), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
		}
		else
		{
			this.timeLimit.text = string.Empty;
			base.CancelInvoke("TickProc");
		}
	}

	private void CreateDressItem(int goodsID)
	{
		this.SetRoleTitle(goodsID);
		this.SetProperties(goodsID);
		this.SetLeftSeconds();
	}

	private void SetProperties(int goodsID)
	{
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(this.goodsData.GoodsID);
		string empty = string.Empty;
		string empty2 = string.Empty;
		ShiZhuangItem.GetBaseAttributeStrFromPropertyList(goodsEquipPropsDoubleList, out empty, out empty2);
		this.properties_left.Text = empty;
		this.properties_right.Text = empty2;
	}

	private void SetEquipButtonState(bool equiped = true)
	{
		string text = (!equiped) ? Global.GetLang("佩戴") : Global.GetLang("卸下");
		this.equipBtn.Text = text;
	}

	public static void GetBaseAttributeStrFromPropertyList(double[] equipFields, out string properties_left, out string properties_right)
	{
		string text = "e3b36c";
		int num = 0;
		int num2 = 2;
		properties_left = string.Empty;
		properties_right = string.Empty;
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		for (int i = 0; i < equipFields.Length; i++)
		{
			double num3 = equipFields[i];
			if (0.0 < num3)
			{
				if (dictionary.ContainsKey(i))
				{
					dictionary[i] = num3;
				}
				else
				{
					dictionary.Add(i, num3);
				}
			}
		}
		foreach (KeyValuePair<int, double> keyValuePair in dictionary)
		{
			int key = keyValuePair.Key;
			Dictionary<int, double>.Enumerator enumerator;
			KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
			double value = keyValuePair2.Value;
			if (key != 0)
			{
				num++;
				string text2 = Global.GetColorStringForNGUIText(new object[]
				{
					text,
					ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, false)
				});
				if (ConfigExtPropIndexes.GetPercentByID(key))
				{
					text2 += string.Format(Global.GetLang("：{0}%"), value * 100.0);
				}
				else
				{
					text2 += string.Format(Global.GetLang("：{0}"), value);
				}
				if (num % num2 == 0)
				{
					properties_right = properties_right + text2 + Environment.NewLine;
				}
				else
				{
					properties_left = properties_left + text2 + Environment.NewLine;
				}
			}
		}
		ShiZhuangItem.ProcessStr(properties_left);
		ShiZhuangItem.ProcessStr(properties_right);
	}

	public static string ProcessStr(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return string.Empty;
		}
		if (str.Length > 0 && str.Substring(str.Length - 1) == "\n")
		{
			str = str.Substring(0, str.Length - 1);
		}
		return str;
	}

	private const string fontColor = "0EFF04";

	private const string netImagePath = "NetImages/GameRes/Images/ChengHao/";

	private const string imagePrefix = "title_";

	private const string imageSuffix = ".png";

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject equipState;

	public ShowNetImage roleTitleImage;

	public TextBlock properties_left;

	public TextBlock properties_right;

	public TextBlock timeLimit;

	public GButton equipBtn;

	private GoodsData goodData;

	private DateTime endTime;

	private bool _equipState;
}
