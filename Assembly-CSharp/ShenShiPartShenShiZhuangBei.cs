using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenShiPartShenShiZhuangBei : UserControl
{
	public int FuWenTabID
	{
		get
		{
			return this.fuwenTabId;
		}
		set
		{
			this.fuwenTabId = value;
			this.AddShenShiListItem();
			ShenShiPartShenShiItem shenShiPartShenShiItem = U3DUtils.AS<ShenShiPartShenShiItem>(this.mOBCList[0]);
			this.InitItemAttr(shenShiPartShenShiItem.Type, shenShiPartShenShiItem.Lev, shenShiPartShenShiItem.IsZhuangBei, shenShiPartShenShiItem.IsJiHuo);
			this.ShenShiItems.SelectedIndex = 0;
			shenShiPartShenShiItem.isSelect = true;
			this.goodId = shenShiPartShenShiItem.Type * 100 + shenShiPartShenShiItem.Lev;
			this.AddShenShiItem();
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.BG.URL = "NetImages/GameRes/Images/shenshiTexture/zhuangbeishenshiditu.jpg.qj";
		this.mOBCList = this.ShenShiItems.ItemsSource;
		this.ShenShiItems.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ListBoxSelectChange);
		this.BtnFuWenTuiJian.Label.text = Global.GetLang("符文推荐");
		this.SkillIcon.StillIcon.MouseLeftButtonUp = delegate(object go, MouseEvent state)
		{
			this.AddSkillServerToClicerData();
		};
		this.BtnSure.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.GetShenShiModEquip(this.FuWenTabID, this.goodId, 1);
			Super.ShowNetWaiting(null);
		};
		this.BtnFuWenTuiJian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenFuWenTuiJian();
		};
	}

	public void AddShenShiServerToClicerData(int goodid, int equip)
	{
		this.AddShenShiItem();
		int num = goodid / 100;
		int num2 = goodid % 100;
		int i = 0;
		int count = this.mOBCList.Count;
		while (i < count)
		{
			ShenShiPartShenShiItem shenShiPartShenShiItem = U3DUtils.AS<ShenShiPartShenShiItem>(this.mOBCList[i]);
			if (num == shenShiPartShenShiItem.Type && num2 == shenShiPartShenShiItem.Lev)
			{
				if (equip > 0)
				{
					shenShiPartShenShiItem.IsZhuangBei = true;
				}
				else
				{
					shenShiPartShenShiItem.IsZhuangBei = false;
				}
			}
			i++;
		}
		ShenShiPartShenShiItem shenShiPartShenShiItem2 = U3DUtils.AS<ShenShiPartShenShiItem>(this.ShenShiItems.SelectedItem);
		if (shenShiPartShenShiItem2.IsZhuangBei)
		{
			this.mioashu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("已装备")
			});
			this.BtnSure.gameObject.SetActive(false);
		}
		else if (!shenShiPartShenShiItem2.IsZhuangBei && shenShiPartShenShiItem2.IsJiHuo)
		{
			this.BtnSure.gameObject.SetActive(true);
			this.BtnSure.Label.text = Global.GetLang("佩戴");
			this.mioashu.text = string.Empty;
		}
		else if (!shenShiPartShenShiItem2.IsZhuangBei && shenShiPartShenShiItem2.IsJiHuo)
		{
			this.BtnSure.gameObject.SetActive(false);
			this.mioashu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("需要选择技能")
			});
		}
		else if (!shenShiPartShenShiItem2.IsJiHuo)
		{
			this.BtnSure.gameObject.SetActive(false);
			this.mioashu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("未获得")
			});
		}
	}

	public void AddSkillServerToClicerData()
	{
		this.ShowSkillSelectPart(0);
	}

	public void ReInitSkillImage(int fuwentabID, int sid)
	{
		if (fuwentabID < Global.Data.MyFuWenTabData.Count)
		{
			Global.Data.MyFuWenTabData[fuwentabID].SkillEquip = sid;
			this.SkillIcon.StillImg.URL = string.Format("NetImages/GameRes/Images/Skill/{0}.png.qj", sid);
		}
		ShenShiPartShenShiItem shenShiPartShenShiItem = U3DUtils.AS<ShenShiPartShenShiItem>(this.ShenShiItems.SelectedItem);
		this.InitItemAttr(shenShiPartShenShiItem.Type, shenShiPartShenShiItem.Lev, shenShiPartShenShiItem.IsZhuangBei, shenShiPartShenShiItem.IsJiHuo);
	}

	public void ShowSkillSelectPart(int index = 0)
	{
		int skillID = this.SkillID;
		GChildWindow window = U3DUtils.NEW<GChildWindow>();
		Super.InitChildWindow(window, "Skill Select");
		this.Container.Children.Add(window);
		SkillSelectPart skillSelectPart = SkillSelectPart.GShow();
		window.SetContent(window.BodyPresenter, skillSelectPart, 0.0, 0.0, true);
		skillSelectPart.InitPartData(index, skillID, delegate(object s, DPSelectedItemEventArgs e)
		{
			Super.CloseChildWindow(this, window);
			int num = -1;
			if (e != null)
			{
				num = e.ID;
				int index2 = e.Index;
			}
			if (num != 0)
			{
				GameInstance.Game.GetSkillModEquip(this.FuWenTabID, num);
				Super.ShowNetWaiting(null);
			}
		}, true);
	}

	private void ComputeFuWenAttr(int tabID, out int blue, out int red, out int green)
	{
		blue = 0;
		red = 0;
		green = 0;
		if (Global.Data.MyFuWenTabData != null && Global.Data.MyFuWenTabData.Count > 0 && this.FuWenTabID < Global.Data.MyFuWenTabData.Count)
		{
			int i = 0;
			int count = Global.Data.MyFuWenTabData[this.FuWenTabID].FuWenEquipList.Count;
			while (i < count)
			{
				int num = Global.Data.MyFuWenTabData[this.FuWenTabID].FuWenEquipList[i];
				if (ShenShiPart.GetDicFuWen().ContainsKey(num))
				{
					blue += ShenShiPart.GetDicFuWen()[num].Blue;
					red += ShenShiPart.GetDicFuWen()[num].Red;
					green += ShenShiPart.GetDicFuWen()[num].Green;
				}
				i++;
			}
		}
		this.shouxu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"3681f3",
			string.Format(Global.GetLang("守序：{0}"), blue)
		});
		this.hunluan.text = Global.GetColorStringForNGUIText(new object[]
		{
			"FF0000",
			string.Format(Global.GetLang("混乱：{0}"), red)
		});
		this.pingheng.text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			string.Format(Global.GetLang("平衡：{0}"), green)
		});
	}

	private Dictionary<int, Dictionary<int, FuWenGod>> GetdicFuWenGod()
	{
		if (this.dicFuWenGodList != null && this.dicFuWenGodList.Count > 0)
		{
			this.dicFuWenGodList.Clear();
		}
		this.ComputeFuWenAttr(this.FuWenTabID, out this.blue, out this.red, out this.green);
		this.GetdicFuWenGodList(this.blue, this.red, this.green);
		foreach (KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair in ShenShiPart.GetDicFuWenGod())
		{
			Dictionary<int, FuWenGod>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				Dictionary<int, Dictionary<int, FuWenGod>> dictionary = this.dicFuWenGodList;
				Dictionary<int, Dictionary<int, FuWenGod>>.Enumerator enumerator;
				KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair2 = enumerator.Current;
				if (!dictionary.ContainsKey(keyValuePair2.Key))
				{
					KeyValuePair<int, FuWenGod> keyValuePair3 = enumerator2.Current;
					if (keyValuePair3.Value.Level == 1)
					{
						KeyValuePair<int, FuWenGod> keyValuePair4 = enumerator2.Current;
						keyValuePair4.Value.isJiHuo = false;
						KeyValuePair<int, FuWenGod> keyValuePair5 = enumerator2.Current;
						keyValuePair5.Value.isZhuangBei = false;
						Dictionary<int, FuWenGod> dictionary2 = new Dictionary<int, FuWenGod>();
						Dictionary<int, FuWenGod> dictionary3 = dictionary2;
						KeyValuePair<int, FuWenGod> keyValuePair6 = enumerator2.Current;
						int key = keyValuePair6.Key;
						KeyValuePair<int, FuWenGod> keyValuePair7 = enumerator2.Current;
						dictionary3.Add(key, keyValuePair7.Value);
						Dictionary<int, Dictionary<int, FuWenGod>> dictionary4 = this.dicFuWenGodList;
						KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair8 = enumerator.Current;
						dictionary4.Add(keyValuePair8.Key, dictionary2);
					}
				}
			}
		}
		return this.dicFuWenGodList;
	}

	private void GetdicFuWenGodList(int blue, int red, int green)
	{
		foreach (KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair in ShenShiPart.GetDicFuWenGod())
		{
			foreach (KeyValuePair<int, FuWenGod> keyValuePair2 in keyValuePair.Value)
			{
				if (blue >= keyValuePair2.Value.NeedBlue)
				{
					Dictionary<int, FuWenGod>.Enumerator enumerator2;
					KeyValuePair<int, FuWenGod> keyValuePair3 = enumerator2.Current;
					if (red >= keyValuePair3.Value.NeedRed)
					{
						KeyValuePair<int, FuWenGod> keyValuePair4 = enumerator2.Current;
						if (green >= keyValuePair4.Value.NeedGreen)
						{
							KeyValuePair<int, FuWenGod> keyValuePair5 = enumerator2.Current;
							keyValuePair5.Value.isZhuangBei = false;
							Dictionary<int, Dictionary<int, FuWenGod>>.Enumerator enumerator;
							if (Global.Data.MyFuWenTabData != null && this.FuWenTabID < Global.Data.MyFuWenTabData.Count && Global.Data.MyFuWenTabData[this.FuWenTabID].ShenShiActiveList != null)
							{
								int i = 0;
								int count = Global.Data.MyFuWenTabData[this.FuWenTabID].ShenShiActiveList.Count;
								while (i < count)
								{
									int num = Global.Data.MyFuWenTabData[this.FuWenTabID].ShenShiActiveList[i] / 100;
									int num2 = num;
									KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair6 = enumerator.Current;
									if (num2 == keyValuePair6.Key)
									{
										KeyValuePair<int, FuWenGod> keyValuePair7 = enumerator2.Current;
										keyValuePair7.Value.isZhuangBei = true;
									}
									i++;
								}
							}
							KeyValuePair<int, FuWenGod> keyValuePair8 = enumerator2.Current;
							keyValuePair8.Value.isJiHuo = true;
							Dictionary<int, Dictionary<int, FuWenGod>> dictionary = this.dicFuWenGodList;
							KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair9 = enumerator.Current;
							if (dictionary.ContainsKey(keyValuePair9.Key))
							{
								Dictionary<int, Dictionary<int, FuWenGod>> dictionary2 = this.dicFuWenGodList;
								KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair10 = enumerator.Current;
								dictionary2[keyValuePair10.Key].Clear();
								Dictionary<int, Dictionary<int, FuWenGod>> dictionary3 = this.dicFuWenGodList;
								KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair11 = enumerator.Current;
								Dictionary<int, FuWenGod> dictionary4 = dictionary3[keyValuePair11.Key];
								KeyValuePair<int, FuWenGod> keyValuePair12 = enumerator2.Current;
								int key = keyValuePair12.Key;
								KeyValuePair<int, FuWenGod> keyValuePair13 = enumerator2.Current;
								dictionary4.Add(key, keyValuePair13.Value);
							}
							else
							{
								Dictionary<int, FuWenGod> dictionary5 = new Dictionary<int, FuWenGod>();
								Dictionary<int, FuWenGod> dictionary6 = dictionary5;
								KeyValuePair<int, FuWenGod> keyValuePair14 = enumerator2.Current;
								int key2 = keyValuePair14.Key;
								KeyValuePair<int, FuWenGod> keyValuePair15 = enumerator2.Current;
								dictionary6.Add(key2, keyValuePair15.Value);
								Dictionary<int, Dictionary<int, FuWenGod>> dictionary7 = this.dicFuWenGodList;
								KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair16 = enumerator.Current;
								dictionary7.Add(keyValuePair16.Key, dictionary5);
							}
						}
					}
				}
			}
		}
	}

	private void AddShenShiListItem()
	{
		foreach (KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair in this.GetdicFuWenGod())
		{
			Dictionary<int, FuWenGod>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				ShenShiPartShenShiItem shenShiPartShenShiItem = U3DUtils.NEW<ShenShiPartShenShiItem>();
				ShenShiPartShenShiItem shenShiPartShenShiItem2 = shenShiPartShenShiItem;
				KeyValuePair<int, FuWenGod> keyValuePair2 = enumerator2.Current;
				shenShiPartShenShiItem2.goodID = keyValuePair2.Value.GodID;
				shenShiPartShenShiItem.isSelect = false;
				shenShiPartShenShiItem.openBoxC0llider = true;
				shenShiPartShenShiItem.openBtnClose = true;
				ShenShiPartShenShiItem shenShiPartShenShiItem3 = shenShiPartShenShiItem;
				Dictionary<int, Dictionary<int, FuWenGod>>.Enumerator enumerator;
				KeyValuePair<int, Dictionary<int, FuWenGod>> keyValuePair3 = enumerator.Current;
				shenShiPartShenShiItem3.Type = keyValuePair3.Key;
				ShenShiPartShenShiItem shenShiPartShenShiItem4 = shenShiPartShenShiItem;
				KeyValuePair<int, FuWenGod> keyValuePair4 = enumerator2.Current;
				shenShiPartShenShiItem4.Lev = keyValuePair4.Value.Level;
				ShenShiPartShenShiItem shenShiPartShenShiItem5 = shenShiPartShenShiItem;
				KeyValuePair<int, FuWenGod> keyValuePair5 = enumerator2.Current;
				shenShiPartShenShiItem5.IsZhuangBei = keyValuePair5.Value.isZhuangBei;
				ShenShiPartShenShiItem shenShiPartShenShiItem6 = shenShiPartShenShiItem;
				KeyValuePair<int, FuWenGod> keyValuePair6 = enumerator2.Current;
				shenShiPartShenShiItem6.IsJiHuo = keyValuePair6.Value.isJiHuo;
				this.mOBCList.AddNoUpdate(shenShiPartShenShiItem);
			}
		}
		if (Global.Data.MyFuWenTabData != null && Global.Data.MyFuWenTabData.Count > 0 && this.FuWenTabID < Global.Data.MyFuWenTabData.Count)
		{
			this.SkillIcon.StillImg.URL = string.Format("NetImages/GameRes/Images/Skill/{0}.png.qj", Global.Data.MyFuWenTabData[this.FuWenTabID].SkillEquip);
		}
	}

	private void InitItemAttr(int type, int lev, bool isZhuangBei, bool isJiHuo)
	{
		this.ComputeFuWenAttr(this.FuWenTabID, out this.blue, out this.red, out this.green);
		this.shenshiName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			ShenShiPart.GetDicFuWenGod()[type][lev].Name
		});
		string text = "FF0000";
		string text2 = "FF0000";
		string text3 = "FF0000";
		if (this.blue >= ShenShiPart.GetDicFuWenGod()[type][lev].NeedBlue)
		{
			text = "17e43e";
		}
		if (this.red >= ShenShiPart.GetDicFuWenGod()[type][lev].NeedRed)
		{
			text2 = "17e43e";
		}
		if (this.green >= ShenShiPart.GetDicFuWenGod()[type][lev].NeedGreen)
		{
			text3 = "17e43e";
		}
		this.shenshiAttr.text = string.Concat(new string[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("需要神识")
			}),
			"\r\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("守序："),
				text,
				string.Format("{0}", ShenShiPart.GetDicFuWenGod()[type][lev].NeedBlue)
			}),
			"\r\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("混乱："),
				text2,
				string.Format("{0}", ShenShiPart.GetDicFuWenGod()[type][lev].NeedRed)
			}),
			"\r\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("平衡："),
				text3,
				string.Format("{0}", ShenShiPart.GetDicFuWenGod()[type][lev].NeedGreen)
			})
		});
		this.shenshiXiaoGuo.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("效果："),
			"dac7ae",
			ShenShiPart.GetDicFuWenGod()[type][lev].Intro
		});
		if (isZhuangBei)
		{
			this.mioashu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("已装备")
			});
			this.BtnSure.gameObject.SetActive(false);
		}
		else if (!isZhuangBei && isJiHuo && this.FuWenTabID < Global.Data.MyFuWenTabData.Count && Global.Data.MyFuWenTabData[this.FuWenTabID].SkillEquip > 0)
		{
			this.BtnSure.gameObject.SetActive(true);
			this.BtnSure.Label.text = Global.GetLang("佩戴");
			this.mioashu.text = string.Empty;
		}
		else if (!isZhuangBei && isJiHuo && this.FuWenTabID < Global.Data.MyFuWenTabData.Count && Global.Data.MyFuWenTabData[this.FuWenTabID].SkillEquip == 0)
		{
			this.BtnSure.gameObject.SetActive(false);
			this.mioashu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("需要选择技能")
			});
		}
		else if (!isJiHuo)
		{
			this.BtnSure.gameObject.SetActive(false);
			this.mioashu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("未获得")
			});
		}
	}

	public void AddShenShiItem()
	{
		if (this.shenshiItem1.transform.childCount > 0)
		{
			int i = 0;
			int childCount = this.shenshiItem1.transform.childCount;
			while (i < childCount)
			{
				Object.Destroy(this.shenshiItem1.transform.GetChild(i).gameObject);
				i++;
			}
			this.TeXiao1.SetActive(false);
		}
		if (this.shenshiItem2.transform.childCount > 0)
		{
			int j = 0;
			int childCount2 = this.shenshiItem2.transform.childCount;
			while (j < childCount2)
			{
				Object.Destroy(this.shenshiItem2.transform.GetChild(j).gameObject);
				j++;
			}
			this.TeXiao2.SetActive(false);
		}
		if (this.shenshiItem3.transform.childCount > 0)
		{
			int k = 0;
			int childCount3 = this.shenshiItem3.transform.childCount;
			while (k < childCount3)
			{
				Object.Destroy(this.shenshiItem3.transform.GetChild(k).gameObject);
				k++;
			}
			this.TeXiao3.SetActive(false);
		}
		if (Global.Data.MyFuWenTabData == null)
		{
			return;
		}
		if (this.FuWenTabID >= Global.Data.MyFuWenTabData.Count)
		{
			return;
		}
		if (Global.Data.MyFuWenTabData[this.FuWenTabID].ShenShiActiveList == null)
		{
			return;
		}
		int count = Global.Data.MyFuWenTabData[this.FuWenTabID].ShenShiActiveList.Count;
		if (count <= 0)
		{
			return;
		}
		Transform transform = null;
		int l = 0;
		int num = count;
		while (l < num)
		{
			if (l == 0)
			{
				transform = this.shenshiItem1.transform;
				this.TeXiao1.SetActive(true);
			}
			else if (l == 1)
			{
				transform = this.shenshiItem2.transform;
				this.TeXiao2.SetActive(true);
			}
			else if (l == 2)
			{
				transform = this.shenshiItem3.transform;
				this.TeXiao3.SetActive(true);
			}
			ShenShiPartShenShiItem shenShiPartShenShiItem = U3DUtils.NEW<ShenShiPartShenShiItem>();
			int num2 = Global.Data.MyFuWenTabData[this.FuWenTabID].ShenShiActiveList[l] / 100;
			int num3 = Global.Data.MyFuWenTabData[this.FuWenTabID].ShenShiActiveList[l] % 100;
			int goodID = 0;
			if (ShenShiPart.GetDicFuWenGod().ContainsKey(num2) && ShenShiPart.GetDicFuWenGod()[num2].ContainsKey(num3))
			{
				goodID = ShenShiPart.GetDicFuWenGod()[num2][num3].GodID;
			}
			shenShiPartShenShiItem.goodID = goodID;
			shenShiPartShenShiItem.Type = num2;
			shenShiPartShenShiItem.Lev = num3;
			shenShiPartShenShiItem.isSelect = false;
			shenShiPartShenShiItem.openBoxC0llider = true;
			shenShiPartShenShiItem.openBtnClose = false;
			shenShiPartShenShiItem.Index = l;
			shenShiPartShenShiItem.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				GameInstance.Game.GetShenShiModEquip(this.FuWenTabID, e.ID, 0);
				Super.ShowNetWaiting(null);
				if (e.Index == 0)
				{
					int m = 0;
					int childCount4 = this.shenshiItem1.transform.childCount;
					while (m < childCount4)
					{
						Object.Destroy(this.shenshiItem1.transform.GetChild(m).gameObject);
						m++;
					}
					this.TeXiao1.SetActive(false);
				}
				else if (e.Index == 1)
				{
					int n = 0;
					int childCount5 = this.shenshiItem2.transform.childCount;
					while (n < childCount5)
					{
						Object.Destroy(this.shenshiItem2.transform.GetChild(n).gameObject);
						n++;
					}
					this.TeXiao2.SetActive(false);
				}
				else if (e.Index == 2)
				{
					int num4 = 0;
					int childCount6 = this.shenshiItem3.transform.childCount;
					while (num4 < childCount6)
					{
						Object.Destroy(this.shenshiItem3.transform.GetChild(num4).gameObject);
						num4++;
					}
					this.TeXiao3.SetActive(false);
				}
			};
			shenShiPartShenShiItem.Handler = delegate(object s1, DPSelectedItemEventArgs e1)
			{
				int m = 0;
				int count2 = this.mOBCList.Count;
				while (m < count2)
				{
					ShenShiPartShenShiItem shenShiPartShenShiItem2 = U3DUtils.AS<ShenShiPartShenShiItem>(this.mOBCList[m]);
					if (shenShiPartShenShiItem2.Type == e1.Type)
					{
						this.InitItemAttr(shenShiPartShenShiItem2.Type, shenShiPartShenShiItem2.Lev, shenShiPartShenShiItem2.IsZhuangBei, shenShiPartShenShiItem2.IsJiHuo);
						this.ShenShiItems.SelectedIndex = m;
						GameObject selectedItem = this.ShenShiItems.SelectedItem;
						if (null != selectedItem)
						{
							ShenShiPartShenShiItem component = selectedItem.GetComponent<ShenShiPartShenShiItem>();
							if (null != component)
							{
								component.isSelect = true;
							}
						}
						GameObject lastSelectedItem = this.ShenShiItems.LastSelectedItem;
						if (null != lastSelectedItem && lastSelectedItem != selectedItem)
						{
							ShenShiPartShenShiItem component2 = lastSelectedItem.GetComponent<ShenShiPartShenShiItem>();
							if (null != component2)
							{
								component2.isSelect = false;
							}
						}
					}
					m++;
				}
			};
			shenShiPartShenShiItem.transform.SetParent(transform, false);
			l++;
		}
	}

	private void ListBoxSelectChange(object sender, MouseEvent e)
	{
		ShenShiPartShenShiItem shenShiPartShenShiItem = U3DUtils.AS<ShenShiPartShenShiItem>(this.ShenShiItems.SelectedItem);
		this.InitItemAttr(shenShiPartShenShiItem.Type, shenShiPartShenShiItem.Lev, shenShiPartShenShiItem.IsZhuangBei, shenShiPartShenShiItem.IsJiHuo);
		this.goodId = shenShiPartShenShiItem.Type * 100 + shenShiPartShenShiItem.Lev;
		GameObject selectedItem = this.ShenShiItems.SelectedItem;
		if (null != selectedItem)
		{
			ShenShiPartShenShiItem component = selectedItem.GetComponent<ShenShiPartShenShiItem>();
			if (null != component)
			{
				component.isSelect = true;
			}
		}
		GameObject lastSelectedItem = this.ShenShiItems.LastSelectedItem;
		if (null != lastSelectedItem && lastSelectedItem != selectedItem)
		{
			ShenShiPartShenShiItem component2 = lastSelectedItem.GetComponent<ShenShiPartShenShiItem>();
			if (null != component2)
			{
				component2.isSelect = false;
			}
		}
	}

	private void OpenFuWenTuiJian()
	{
		if (this.m_tuiJianWindow == null)
		{
			this.m_tuiJianWindow = U3DUtils.NEW<GChildWindow>();
			this.m_tuiJianWindow.IsShowModal = true;
			this.m_tuiJianWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_tuiJianWindow, Global.GetLang("推荐"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_tuiJianWindow);
		}
		if (this.m_tuiJianPart == null)
		{
			this.m_tuiJianPart = U3DUtils.NEW<ShenShiPartTuiJian>();
			this.m_tuiJianPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHuiShouWindow();
			};
		}
		this.m_tuiJianWindow.SetContent(this.m_tuiJianWindow.BodyPresenter, this.m_tuiJianPart, 0.0, 0.0, true);
	}

	private void CloseHuiShouWindow()
	{
		if (null != this.m_tuiJianPart)
		{
			this.m_tuiJianPart.transform.parent = null;
			Object.Destroy(this.m_tuiJianPart.gameObject);
			this.m_tuiJianPart = null;
		}
		if (null != this.m_tuiJianWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_tuiJianWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_tuiJianWindow, true);
			this.m_tuiJianWindow = null;
		}
	}

	public GButton BtnAddJiNeng;

	public GButton BtnSure;

	public ListBox ShenShiItems;

	public UILabel shouxu;

	public UILabel hunluan;

	public UILabel pingheng;

	public UILabel shenshiName;

	public UILabel shenshiAttr;

	public UILabel shenshiXiaoGuo;

	public UILabel mioashu;

	public GameObject shenshiItem1;

	public GameObject shenshiItem2;

	public GameObject shenshiItem3;

	public GameObject TeXiao1;

	public GameObject TeXiao2;

	public GameObject TeXiao3;

	public GSkillIcon SkillIcon;

	public ShowNetImage BG;

	public GButton BtnFuWenTuiJian;

	private int fuwenTabId;

	public int blue;

	public int red;

	public int green;

	public int SkillID;

	private int goodId;

	private ObservableCollection mOBCList;

	private Dictionary<int, Dictionary<int, FuWenGod>> dicFuWenGodList = new Dictionary<int, Dictionary<int, FuWenGod>>();

	protected GChildWindow m_tuiJianWindow;

	protected ShenShiPartTuiJian m_tuiJianPart;
}
