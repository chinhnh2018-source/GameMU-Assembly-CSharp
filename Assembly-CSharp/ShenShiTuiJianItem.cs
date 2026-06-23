using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ShenShiTuiJianItem : UserControl
{
	public new string Name
	{
		get
		{
			return this.m_name;
		}
	}

	public string Des
	{
		get
		{
			return this.m_des;
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickItem);
	}

	public void InitInfo(int type, int id)
	{
		this.m_type = type;
		this.m_id = id;
		if (type == 1)
		{
			FuWen fuWen = null;
			if (ShenShiPart.GetDicFuWen().TryGetValue(this.m_id, ref fuWen))
			{
				this.imgIcon.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png.qj", fuWen.GoodsID);
				this.lblName.text = fuWen.Name;
				this.m_name = fuWen.Name;
				Dictionary<int, double> dictionary = new Dictionary<int, double>();
				dictionary = ConfigGoods.GetDicEquipPropsByGoodsId(fuWen.GoodsID);
				Dictionary<int, double>.Enumerator enumerator = dictionary.GetEnumerator();
				string text = string.Empty;
				while (enumerator.MoveNext())
				{
					Dictionary<int, ExtPropIndexess> dicExtPropIndexes = ShenShiPartFuWenXiangQian.GetDicExtPropIndexes();
					KeyValuePair<int, double> keyValuePair = enumerator.Current;
					string text2;
					if (dicExtPropIndexes[keyValuePair.Key].Percent == 0)
					{
						KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
						text2 = keyValuePair2.Value.ToString();
					}
					else
					{
						string text3 = "{0}%";
						KeyValuePair<int, double> keyValuePair3 = enumerator.Current;
						text2 = string.Format(text3, keyValuePair3.Value * 100.0);
					}
					string text4 = text;
					string text5 = "{0} + {1}\r\n";
					Dictionary<int, ExtPropIndexess> dicExtPropIndexes2 = ShenShiPartFuWenXiangQian.GetDicExtPropIndexes();
					KeyValuePair<int, double> keyValuePair4 = enumerator.Current;
					text = text4 + string.Format(text5, dicExtPropIndexes2[keyValuePair4.Key].Description, text2);
				}
				string text6 = null;
				int num = 0;
				int blue = fuWen.Blue;
				int red = fuWen.Red;
				int green = fuWen.Green;
				if (blue != 0)
				{
					text6 = Global.GetLang("守序");
					num = blue;
				}
				else if (red != 0)
				{
					text6 = Global.GetLang("混乱");
					num = red;
				}
				else if (green != 0)
				{
					text6 = Global.GetLang("平衡");
					num = green;
				}
				string text7 = string.Format(Global.GetLang("{0}神识 + {1}"), text6, num);
				this.m_des = string.Format(Global.GetLang("装备可获得：\n\r{0}{1}"), text, text7);
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"符文ID " + id + Global.GetLang("的配置文件不存在")
				});
				this.imgIcon.URL = string.Format(string.Empty, new object[0]);
				this.lblName.text = string.Empty;
				this.m_name = string.Empty;
				this.m_des = string.Empty;
			}
		}
		else if (type == 2)
		{
			FuWenGod fuWenGodByGoodsId = this.GetFuWenGodByGoodsId(id);
			if (fuWenGodByGoodsId != null)
			{
				this.imgIcon.URL = string.Format("NetImages/GameRes/Images/FuWenGods/God_{0}.png.qj", fuWenGodByGoodsId.GodID);
				this.lblName.text = fuWenGodByGoodsId.Name;
				this.m_name = fuWenGodByGoodsId.Name;
				this.m_des = Global.GetLang("装备可获得：") + fuWenGodByGoodsId.Intro;
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"神识ID " + id + Global.GetLang("的配置文件不存在")
				});
				this.imgIcon.URL = string.Format(string.Empty, new object[0]);
				this.lblName.text = string.Empty;
				this.m_name = string.Empty;
				this.m_des = string.Empty;
			}
		}
	}

	protected void OnClickItem(GameObject go)
	{
		this.OnClickTuiJianItem.Invoke(this);
	}

	public FuWenGod GetFuWenGodByGoodsId(int godId)
	{
		foreach (KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair in ShenShiPart.GetDicFuWenGod())
		{
			foreach (KeyValuePair<int, FuWenGod> keyValuePair2 in keyValuePair.Value)
			{
				if (keyValuePair2.Value.GodID == godId)
				{
					Dictionary<int, FuWenGod>.Enumerator enumerator2;
					KeyValuePair<int, FuWenGod> keyValuePair3 = enumerator2.Current;
					return keyValuePair3.Value;
				}
			}
		}
		return null;
	}

	public UILabel lblName;

	public ShowNetImage imgIcon;

	public Action<ShenShiTuiJianItem> OnClickTuiJianItem;

	private int m_type;

	private int m_id;

	private string m_name;

	private string m_des;
}
