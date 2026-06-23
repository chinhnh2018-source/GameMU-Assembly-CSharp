using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class SpiritTrackPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.shenjidianLab.text = string.Format(Global.GetLang("神迹点数：{0}"), Global.Data.roleData.RoleCommonUseIntPamams[46]);
		this.BG.URL = "NetImages/GameRes/Images/shenjiTex/shenjiBG.jpg.qj";
		this.WaiKuangBG1.URL = "NetImages/GameRes/Images/shenjiTex/shenjiwaikuang.png.qj";
		this.BtnChongZhi.Text = Global.GetLang("重置点数");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.UpdateAttr();
		this.InitTextInPrefabs();
		this.InitSpiritItems();
		this.LoadBeiJingTeXiao();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedClose(this, new DPSelectedItemEventArgs());
		};
		this.BtnAdd.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowShenJiJiFenZhuRuWindow();
		};
		this.BtnChongZhi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowShenJiDianCZWindow();
		};
	}

	public void UpdateShenJiDian()
	{
		this.shenjidianLab.text = string.Format(Global.GetLang("神迹点数：{0}"), Global.Data.roleData.RoleCommonUseIntPamams[46]);
		this.UpdateAttr();
	}

	public void SetStarsPosition(Vector3[] positions, GameObject shenji, GameObject xianlu)
	{
		shenji.transform.localPosition = positions[0];
		xianlu.transform.localPosition = positions[1];
		xianlu.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.GetAngel(positions[1], positions[0])));
		xianlu.transform.localScale = new Vector3(12f, Vector3.Distance(positions[0], positions[1]), 0f);
	}

	private float GetAngel(Vector3 center, Vector3 point)
	{
		float num = Mathf.Asin((point.y - center.y) / Vector3.Distance(center, point));
		if (point.x - center.x < 0f)
		{
			if (point.x - center.x < 0f && point.y - center.y >= 0f)
			{
				num = 3.14159274f - num;
			}
			else
			{
				num = -num - 3.14159274f;
			}
		}
		num = num / 3.14159274f * 180f;
		return num - 90f;
	}

	public void InitSpiritItems()
	{
		base.StartCoroutine<bool>(this.InitItems());
	}

	public void ReItems()
	{
		if (Global.Data.roleData.ShenJiDict == null)
		{
			return;
		}
		Dictionary<int, ShenJiFuWenData>.Enumerator enumerator = Global.Data.roleData.ShenJiDict.GetEnumerator();
		while (enumerator.MoveNext())
		{
			for (int i = 0; i < this.spiritItems.Count; i++)
			{
				int id = this.spiritItems[i].ID;
				KeyValuePair<int, ShenJiFuWenData> keyValuePair = enumerator.Current;
				if (id == keyValuePair.Key)
				{
					SpiritTrackItemPart spiritTrackItemPart = this.spiritItems[i];
					KeyValuePair<int, ShenJiFuWenData> keyValuePair2 = enumerator.Current;
					spiritTrackItemPart.Level = keyValuePair2.Value.Level;
					if (this.spiritItems[i].Level != 0)
					{
						UITexture component = this.spiritItems[i].TuBiao.GetComponent<UITexture>();
						component.shader = Shader.Find("Unlit/Transparent Colored");
						component.enabled = false;
						component.enabled = true;
					}
				}
			}
		}
		if (this.loadId == 1)
		{
			this.LoadJiHuoTeXiao(this.loadIndex);
		}
		else if (this.loadId == 2)
		{
			this.LoadShengJiTeXiao(this.loadIndex);
		}
		base.CancelInvoke("ReKeJiHuoItemXianLu");
		base.InvokeRepeating("ReKeJiHuoItemXianLu", 1f, 1f);
	}

	private void ReKeJiHuoItemXianLu()
	{
		for (int i = 0; i < this.spiritItems.Count; i++)
		{
			if (this.spiritItems[i].Level != 0 || this.spiritItems[i].Perv == -1 || this.spiritDic[this.spiritItems[i].Perv].Level >= SpiritTrackPart.GetDicShenJiFuWen()[this.spiritItems[i].ID].PrevLevel)
			{
				this.spiritItems[i].BG.spriteName = "dizuo_yes";
				this.xianluTeXiao[i].GetComponent<UISprite>().spriteName = "shenjilianxian_right";
			}
			else
			{
				this.spiritItems[i].BG.spriteName = "dizuo_no";
				this.xianluTeXiao[i].GetComponent<UISprite>().spriteName = "shenjilianxian_gry";
			}
		}
	}

	private void ShuaXinItem()
	{
		for (int i = 0; i < this.spiritItems.Count; i++)
		{
			if (this.spiritItems[i].Level != 0 || this.spiritItems[i].Perv == -1 || (this.spiritDic.ContainsKey(this.spiritItems[i].Perv) && this.spiritDic[this.spiritItems[i].Perv].Level >= SpiritTrackPart.GetDicShenJiFuWen()[this.spiritItems[i].ID].PrevLevel))
			{
				this.spiritItems[i].BG.spriteName = "dizuo_yes";
				this.xianluTeXiao[i].GetComponent<UISprite>().spriteName = "shenjilianxian_right";
				if (this.spiritItems[i].Level != 0)
				{
					string path = "UITeXiao/Perfabs/jinglingshenji/shenji_liuguang";
					Transform transform = this.xianluTeXiao[i];
					transform.localScale = new Vector3(1f, 1f, 1f);
					GameObject gameObject = this.LoadTeXiao(path, transform, true, false);
					this.liuguangTeXiao.Add(gameObject);
					string path2 = "UITeXiao/Perfabs/jinglingshenji/shenji_shiyongzhong";
					Transform transform2 = this.spiritItems[i].transform;
					this.LoadTeXiao(path2, transform2, false, true);
				}
			}
			else
			{
				this.spiritItems[i].BG.spriteName = "dizuo_no";
				this.xianluTeXiao[i].GetComponent<UISprite>().spriteName = "shenjilianxian_gry";
			}
		}
	}

	public void UpdateAttr()
	{
		this.num = Global.Data.roleData.RoleCommonUseIntPamams[46];
	}

	private IEnumerator InitItems()
	{
		Dictionary<int, ShenJiFuWen>.Enumerator shenjiDic = SpiritTrackPart.GetDicShenJiFuWen().GetEnumerator();
		int i = 0;
		while (shenjiDic.MoveNext())
		{
			Dictionary<int, ShenJiFuWen> dictionary = SpiritTrackPart.GetDicShenJiFuWen();
			KeyValuePair<int, ShenJiFuWen> keyValuePair = shenjiDic.Current;
			if (dictionary.ContainsKey(keyValuePair.Key))
			{
				SpiritTrackItemPart item = U3DUtils.NEW<SpiritTrackItemPart>();
				SpiritTrackLianXianPart lianxian = U3DUtils.NEW<SpiritTrackLianXianPart>();
				SpiritTrackItemPart spiritTrackItemPart = item;
				KeyValuePair<int, ShenJiFuWen> keyValuePair2 = shenjiDic.Current;
				spiritTrackItemPart.ID = keyValuePair2.Value.ID;
				if (Global.Data.roleData.ShenJiDict == null)
				{
					goto IL_123;
				}
				Dictionary<int, ShenJiFuWenData> shenJiDict = Global.Data.roleData.ShenJiDict;
				KeyValuePair<int, ShenJiFuWen> keyValuePair3 = shenjiDic.Current;
				if (!shenJiDict.ContainsKey(keyValuePair3.Value.ID))
				{
					goto IL_123;
				}
				SpiritTrackItemPart spiritTrackItemPart2 = item;
				Dictionary<int, ShenJiFuWenData> shenJiDict2 = Global.Data.roleData.ShenJiDict;
				KeyValuePair<int, ShenJiFuWen> keyValuePair4 = shenjiDic.Current;
				spiritTrackItemPart2.Level = shenJiDict2[keyValuePair4.Value.ID].Level;
				IL_12F:
				item.Index = i;
				SpiritTrackItemPart spiritTrackItemPart3 = item;
				Dictionary<int, ShenJiFuWen> dictionary2 = SpiritTrackPart.GetDicShenJiFuWen();
				KeyValuePair<int, ShenJiFuWen> keyValuePair5 = shenjiDic.Current;
				spiritTrackItemPart3.Perv = dictionary2[keyValuePair5.Value.ID].Prev;
				SpiritTrackItemPart spiritTrackItemPart4 = item;
				Dictionary<int, ShenJiFuWen> dictionary3 = SpiritTrackPart.GetDicShenJiFuWen();
				KeyValuePair<int, ShenJiFuWen> keyValuePair6 = shenjiDic.Current;
				spiritTrackItemPart4.Size = dictionary3[keyValuePair6.Value.ID].Size;
				Transform transform = item.transform;
				Dictionary<int, ShenJiFuWen> dictionary4 = SpiritTrackPart.GetDicShenJiFuWen();
				KeyValuePair<int, ShenJiFuWen> keyValuePair7 = shenjiDic.Current;
				float size = dictionary4[keyValuePair7.Value.ID].Size;
				Dictionary<int, ShenJiFuWen> dictionary5 = SpiritTrackPart.GetDicShenJiFuWen();
				KeyValuePair<int, ShenJiFuWen> keyValuePair8 = shenjiDic.Current;
				float size2 = dictionary5[keyValuePair8.Value.ID].Size;
				Dictionary<int, ShenJiFuWen> dictionary6 = SpiritTrackPart.GetDicShenJiFuWen();
				KeyValuePair<int, ShenJiFuWen> keyValuePair9 = shenjiDic.Current;
				transform.localScale = new Vector3(size, size2, dictionary6[keyValuePair9.Value.ID].Size);
				item.transform.SetParent(this.spiritItemParent, false);
				lianxian.transform.SetParent(this.xianluParent, false);
				SpiritTrackItemPart spiritTrackItemPart5 = item;
				KeyValuePair<int, ShenJiFuWen> keyValuePair10 = shenjiDic.Current;
				spiritTrackItemPart5.Icon = keyValuePair10.Value.Icon;
				if (item.Level != 0)
				{
					UITexture bgTexture = item.TuBiao.GetComponent<UITexture>();
					bgTexture.shader = Shader.Find("Unlit/Transparent Colored");
				}
				else
				{
					UITexture bgTexture2 = item.TuBiao.GetComponent<UITexture>();
					bgTexture2.shader = Shader.Find("Unlit/Gray");
				}
				KeyValuePair<int, ShenJiFuWen> keyValuePair11 = shenjiDic.Current;
				float x = keyValuePair11.Value.X;
				KeyValuePair<int, ShenJiFuWen> keyValuePair12 = shenjiDic.Current;
				Vector3 point = new Vector3(x, keyValuePair12.Value.Y, -1f);
				Vector3 perpoint = new Vector3(0f, 0f, 0f);
				Dictionary<int, ShenJiFuWen> dictionary7 = SpiritTrackPart.GetDicShenJiFuWen();
				KeyValuePair<int, ShenJiFuWen> keyValuePair13 = shenjiDic.Current;
				if (dictionary7.ContainsKey(keyValuePair13.Value.Prev))
				{
					Dictionary<int, ShenJiFuWen> dictionary8 = SpiritTrackPart.GetDicShenJiFuWen();
					KeyValuePair<int, ShenJiFuWen> keyValuePair14 = shenjiDic.Current;
					float x2 = dictionary8[keyValuePair14.Value.Prev].X;
					Dictionary<int, ShenJiFuWen> dictionary9 = SpiritTrackPart.GetDicShenJiFuWen();
					KeyValuePair<int, ShenJiFuWen> keyValuePair15 = shenjiDic.Current;
					perpoint = new Vector3(x2, dictionary9[keyValuePair15.Value.Prev].Y, -1f);
				}
				this.SetStarsPosition(new Vector3[]
				{
					point,
					perpoint
				}, item.gameObject, lianxian.gameObject);
				this.spiritItems.Add(item);
				this.xianluTeXiao.Add(lianxian.gameObject.transform);
				this.spiritDic.Add(item.ID, item);
				item.ItemClick = new DPSelectedItemEventHandler(this.ShenJiItemClick);
				goto IL_4B7;
				IL_123:
				item.Level = 0;
				goto IL_12F;
			}
			IL_4B7:
			i++;
			if (i % 10 == 0)
			{
				yield return -1;
			}
		}
		yield return null;
		this.ShuaXinItem();
		yield break;
	}

	public void DestoryLiuDongTeXiao()
	{
		for (int i = 0; i < this.liuguangTeXiao.Count; i++)
		{
			if (this.liuguangTeXiao[i] != null)
			{
				NGUITools.Destroy(this.liuguangTeXiao[i].gameObject);
			}
		}
		for (int j = 0; j < this.spiritItems.Count; j++)
		{
			if (this.spiritItems[j].gameObject != null)
			{
				NGUITools.Destroy(this.spiritItems[j].gameObject);
			}
		}
		for (int k = 0; k < this.xianluTeXiao.Count; k++)
		{
			if (this.xianluTeXiao[k].gameObject != null)
			{
				NGUITools.Destroy(this.xianluTeXiao[k].gameObject);
			}
		}
	}

	public void ShowShenJiJiFenZhuRuWindow()
	{
		if (this.ShenJiJiFenZhuRuWindow == null)
		{
			this.ShenJiJiFenZhuRuWindow = U3DUtils.NEW<GChildWindow>();
			this.ShenJiJiFenZhuRuWindow.IsShowModal = true;
			this.ShenJiJiFenZhuRuWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.ShenJiJiFenZhuRuWindow, Global.GetLang("精灵神迹属性界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.ShenJiJiFenZhuRuWindow);
		}
		if (this.ShenJiJiFenZhuRuPart == null)
		{
			this.ShenJiJiFenZhuRuPart = U3DUtils.NEW<ShenJiJiFenZhuRu>();
			this.ShenJiJiFenZhuRuPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseShenJiJiFenZhuRuWindow();
			};
		}
		this.ShenJiJiFenZhuRuWindow.SetContent(this.ShenJiJiFenZhuRuWindow.BodyPresenter, this.ShenJiJiFenZhuRuPart, 0.0, 0.0, true);
	}

	public void CloseShenJiJiFenZhuRuWindow()
	{
		if (null != this.ShenJiJiFenZhuRuPart)
		{
			this.ShenJiJiFenZhuRuPart.transform.parent = null;
			Object.Destroy(this.ShenJiJiFenZhuRuPart.gameObject);
			this.ShenJiJiFenZhuRuPart = null;
		}
		if (null != this.ShenJiJiFenZhuRuWindow)
		{
			Super.CloseChildWindow(base.Children, this.ShenJiJiFenZhuRuWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.ShenJiJiFenZhuRuWindow, true);
			this.ShenJiJiFenZhuRuWindow = null;
		}
	}

	public void ShowShenJiDianCZWindow()
	{
		if (this.ShenJiDianCZWindow == null)
		{
			this.ShenJiDianCZWindow = U3DUtils.NEW<GChildWindow>();
			this.ShenJiDianCZWindow.IsShowModal = true;
			this.ShenJiDianCZWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.ShenJiDianCZWindow, Global.GetLang("精灵神迹属性界面00"));
			Super.GData.GlobalPlayZone.Children.Add(this.ShenJiDianCZWindow);
			this.ShenJiDianCZWindow.ChildWindowClose = delegate(object s, EventArgs e)
			{
				this.CloseShenJiDianCZWindow();
				return true;
			};
		}
		if (this.ShenJiDianCZ == null)
		{
			this.ShenJiDianCZ = U3DUtils.NEW<SpiritTrackXiDian>();
			this.ShenJiDianCZ.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.CloseShenJiDianCZWindow();
			};
			this.ShenJiDianCZ.InitStr();
			this.ShenJiDianCZ.initIcon();
		}
		this.ShenJiDianCZWindow.SetContent(this.ShenJiDianCZWindow.BodyPresenter, this.ShenJiDianCZ, 0.0, 0.0, true);
	}

	public void CloseShenJiDianCZWindow()
	{
		if (null != this.ShenJiDianCZ)
		{
			this.ShenJiDianCZ.transform.parent = null;
			Object.Destroy(this.ShenJiDianCZ.gameObject);
			this.ShenJiDianCZ = null;
		}
		if (null != this.ShenJiDianCZWindow)
		{
			Super.CloseChildWindow(base.Children, this.ShenJiDianCZWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.ShenJiDianCZWindow, true);
			this.ShenJiDianCZWindow = null;
		}
	}

	public void ShowSpiritTrackItemAttrPartWindow()
	{
		if (this.SpiritTrackItemAttrWindow == null)
		{
			this.SpiritTrackItemAttrWindow = U3DUtils.NEW<GChildWindow>();
			this.SpiritTrackItemAttrWindow.IsShowModal = true;
			this.SpiritTrackItemAttrWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.SpiritTrackItemAttrWindow, Global.GetLang("精灵神迹属性界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.SpiritTrackItemAttrWindow);
		}
		if (this.spiritTrackItemAttrPart == null)
		{
			this.spiritTrackItemAttrPart = U3DUtils.NEW<SpiritTrackItemAttrPart>();
			this.spiritTrackItemAttrPart.DPSelectedClose = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseSpiritTrackItemAttrPartWindow();
			};
		}
		this.SpiritTrackItemAttrWindow.SetContent(this.SpiritTrackItemAttrWindow.BodyPresenter, this.spiritTrackItemAttrPart, 0.0, 0.0, true);
	}

	public void CloseSpiritTrackItemAttrPartWindow()
	{
		if (null != this.spiritTrackItemAttrPart)
		{
			this.spiritTrackItemAttrPart.transform.parent = null;
			Object.Destroy(this.spiritTrackItemAttrPart.gameObject);
			this.spiritTrackItemAttrPart = null;
		}
		if (null != this.SpiritTrackItemAttrWindow)
		{
			Super.CloseChildWindow(base.Children, this.SpiritTrackItemAttrWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.SpiritTrackItemAttrWindow, true);
			this.SpiritTrackItemAttrWindow = null;
		}
	}

	private string StringAttr(string attr, out int Percent)
	{
		Percent = 0;
		XElement gameResXml = Global.GetGameResXml("Config/ExtPropIndexes.xml");
		if (gameResXml == null)
		{
			return string.Empty;
		}
		string[] array = attr.Split(new char[]
		{
			','
		});
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ExtPropIndexes");
		int i = 0;
		while (i < xelementList.Count)
		{
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[i], "Word");
			Percent = Global.GetXElementAttributeInt(xelementList[i], "Percent");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelementList[i], "Description");
			if (array[0].Equals(xelementAttributeStr))
			{
				if (Percent == 0)
				{
					return string.Format("{0}: + {1}", xelementAttributeStr2, array[1]);
				}
				return string.Format("{0}: + {1}%", xelementAttributeStr2, double.Parse(array[1]) * 100.0);
			}
			else
			{
				i++;
			}
		}
		return string.Empty;
	}

	private void LoadShiYongZhong(int index)
	{
		string path = "UITeXiao/Perfabs/jinglingshenji/shenji_shiyongzhong";
		Transform transform = this.spiritItems[index].transform;
		this.LoadTeXiao(path, transform, false, true);
	}

	private void LoadJiHuoTeXiao(int index)
	{
		this.indexTeXiao = index;
		base.StartCoroutine<bool>(this.LoadJiHuo());
	}

	private IEnumerator LoadJiHuo()
	{
		string liudongpath = "UITeXiao/Perfabs/jinglingshenji/shenji_liuguang";
		Transform liudongparent = this.xianluTeXiao[this.indexTeXiao];
		liudongparent.localScale = new Vector3(1f, 1f, 1f);
		GameObject go = this.LoadTeXiao(liudongpath, liudongparent, true, false);
		this.liuguangTeXiao.Add(go);
		yield return new WaitForSeconds(0.15f);
		string jihuopath = "UITeXiao/Perfabs/jinglingshenji/shenji_jihuo";
		Transform jihuoparent = this.spiritItems[this.indexTeXiao].transform;
		this.LoadTeXiao(jihuopath, jihuoparent, false, false);
		yield return new WaitForSeconds(0.25f);
		this.LoadShengJiTeXiao(this.indexTeXiao);
		yield return new WaitForSeconds(0.25f);
		this.LoadShiYongZhong(this.indexTeXiao);
		yield break;
	}

	private void LoadShengJiTeXiao(int index)
	{
		string path = "UITeXiao/Perfabs/jinglingshenji/ShenJi_jiesuo";
		Transform transform = this.spiritItems[index].transform;
		this.LoadTeXiao(path, transform, false, false);
	}

	private void LoadBeiJingTeXiao()
	{
		string path = "UITeXiao/Perfabs/jinglingshenji/shenji_liuxing_qiu";
		string path2 = "UITeXiao/Perfabs/jinglingshenji/zhen";
		Transform beijingZhongxindian = this.BeijingZhongxindian;
		Transform beiJingQiu = this.BeiJingQiu;
		this.LoadTeXiao(path, beiJingQiu, false, false);
		this.LoadTeXiao(path2, beijingZhongxindian, false, false);
		string path3 = "UITeXiao/Perfabs/jinglingshenji/shenji_liudong_big";
		Transform parent = this.zhongxindian;
		this.LoadTeXiao(path3, parent, false, false);
		string path4 = "UITeXiao/Perfabs/jinglingshenji/shenji_liuxing";
		Transform parent2 = this.zhongxinliuxing;
		this.LoadTeXiao(path4, parent2, false, false);
	}

	private GameObject LoadTeXiao(string path, Transform parent = null, bool xianlu = false, bool itemshiyong = false)
	{
		Object @object = Resources.Load(path);
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			if (xianlu)
			{
				gameObject.transform.localScale = new Vector3(1.7f, 1f, 1f);
			}
			else
			{
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			gameObject.transform.SetParent(parent, false);
			U3DUtils.ResetLayer(gameObject, "MUUI");
			if (itemshiyong)
			{
				gameObject.transform.localPosition = new Vector3(0f, 0f, -11f);
			}
			return gameObject;
		}
		return null;
	}

	public void ShenJiItemClick(object s, DPSelectedItemEventArgs e)
	{
		this.ShowSpiritTrackItemAttrPartWindow();
		this.spiritTrackItemAttrPart.ID = e.ID;
		this.spiritTrackItemAttrPart.Index = e.Index;
		this.spiritTrackItemAttrPart.DPSelectedClose = delegate(object s1, DPSelectedItemEventArgs e1)
		{
			this.CloseSpiritTrackItemAttrPartWindow();
		};
		this.spiritTrackItemAttrPart.DPSelected = delegate(object s2, DPSelectedItemEventArgs e2)
		{
			if (e2.ID == 1)
			{
				this.loadId = e2.ID;
				this.loadIndex = e2.Index;
			}
			else if (e2.ID == 2)
			{
				this.loadId = e2.ID;
				this.loadIndex = e2.Index;
			}
		};
		if (e.Level == 0)
		{
			if (e.Type == -1 || this.spiritDic[e.Type].Level >= SpiritTrackPart.GetDicShenJiFuWen()[e.ID].PrevLevel)
			{
				bool canJiHuo = false;
				string name = SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Name;
				int num = 0;
				string attr = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect1.Split(new char[]
				{
					'|'
				})[0], out num);
				string attr2 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect1.Split(new char[]
				{
					'|'
				})[1], out num);
				string miaoshu = string.Format(Global.GetLang("需要{0}点神迹点"), SpiritTrackPart.GetDicShenJiFuWen()[e.ID].UpNeed);
				if (this.num >= SpiritTrackPart.GetDicShenJiFuWen()[e.ID].UpNeed)
				{
					canJiHuo = true;
				}
				this.spiritTrackItemAttrPart.JiHuoState(name, attr, attr2, miaoshu, canJiHuo);
			}
			else
			{
				string name2 = SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Name;
				int num2 = 0;
				string attr3 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect1.Split(new char[]
				{
					'|'
				})[0], out num2);
				string attr4 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect1.Split(new char[]
				{
					'|'
				})[1], out num2);
				string lang = Global.GetLang("需激活前置技能");
				this.spiritTrackItemAttrPart.JiHuoQianZhiState(name2, attr3, attr4, lang);
			}
		}
		else if (e.Level == SpiritTrackPart.GetDicShenJiFuWen()[e.ID].MaxLevel)
		{
			string name3 = SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Name;
			string attr5 = null;
			string attr6 = null;
			int num3 = 0;
			if (SpiritTrackPart.GetDicShenJiFuWen()[e.ID].MaxLevel == 1)
			{
				attr5 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect1.Split(new char[]
				{
					'|'
				})[0], out num3);
				attr6 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect1.Split(new char[]
				{
					'|'
				})[1], out num3);
			}
			else if (SpiritTrackPart.GetDicShenJiFuWen()[e.ID].MaxLevel == 2)
			{
				attr5 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect2.Split(new char[]
				{
					'|'
				})[0], out num3);
				attr6 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect2.Split(new char[]
				{
					'|'
				})[1], out num3);
			}
			else if (SpiritTrackPart.GetDicShenJiFuWen()[e.ID].MaxLevel == 3)
			{
				attr5 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect3.Split(new char[]
				{
					'|'
				})[0], out num3);
				attr6 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect3.Split(new char[]
				{
					'|'
				})[1], out num3);
			}
			else if (SpiritTrackPart.GetDicShenJiFuWen()[e.ID].MaxLevel == 4)
			{
				attr5 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect4.Split(new char[]
				{
					'|'
				})[0], out num3);
				attr6 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect4.Split(new char[]
				{
					'|'
				})[1], out num3);
			}
			else if (SpiritTrackPart.GetDicShenJiFuWen()[e.ID].MaxLevel == 5)
			{
				attr5 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect5.Split(new char[]
				{
					'|'
				})[0], out num3);
				attr6 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect5.Split(new char[]
				{
					'|'
				})[1], out num3);
			}
			this.spiritTrackItemAttrPart.ManJiState(name3, attr5, attr6);
		}
		else
		{
			string name4 = SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Name;
			string attr7 = string.Empty;
			string attr8 = string.Empty;
			string empty = string.Empty;
			string empty2 = string.Empty;
			int percent = 0;
			int percent2 = 0;
			bool canjihuo = false;
			if (this.num >= SpiritTrackPart.GetDicShenJiFuWen()[e.ID].UpNeed)
			{
				canjihuo = true;
			}
			if (e.Level == 1)
			{
				attr7 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect1.Split(new char[]
				{
					'|'
				})[0], out percent);
				attr8 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect1.Split(new char[]
				{
					'|'
				})[1], out percent2);
				this.GetAttrAdd(e.ID, e.Level, percent, out empty);
				this.GetAttrAdd(e.ID, e.Level, percent2, out empty2);
			}
			else if (e.Level == 2)
			{
				attr7 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect2.Split(new char[]
				{
					'|'
				})[0], out percent);
				attr8 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect2.Split(new char[]
				{
					'|'
				})[1], out percent2);
				this.GetAttrAdd(e.ID, e.Level, percent, out empty);
				this.GetAttrAdd(e.ID, e.Level, percent2, out empty2);
			}
			else if (e.Level == 3)
			{
				attr7 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect3.Split(new char[]
				{
					'|'
				})[0], out percent);
				attr8 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect3.Split(new char[]
				{
					'|'
				})[1], out percent2);
				this.GetAttrAdd(e.ID, e.Level, percent, out empty);
				this.GetAttrAdd(e.ID, e.Level, percent2, out empty2);
			}
			else if (e.Level == 4)
			{
				attr7 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect4.Split(new char[]
				{
					'|'
				})[0], out percent);
				attr8 = this.StringAttr(SpiritTrackPart.GetDicShenJiFuWen()[e.ID].Effect4.Split(new char[]
				{
					'|'
				})[1], out percent2);
				this.GetAttrAdd(e.ID, e.Level, percent, out empty);
				this.GetAttrAdd(e.ID, e.Level, percent2, out empty2);
			}
			string miaoshu2 = string.Format(Global.GetLang("需要{0}点神迹点"), SpiritTrackPart.GetDicShenJiFuWen()[e.ID].UpNeed);
			this.spiritTrackItemAttrPart.ShengJiState(name4, attr7, attr8, empty, empty2, miaoshu2, canjihuo);
		}
	}

	private void GetAttrAdd(int ID, int level, int Percent, out string attradd)
	{
		attradd = string.Empty;
		int num = 0;
		double num2 = 0.0;
		if (Percent == 0)
		{
			if (level == 1)
			{
				num = SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect2.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1].SafeToInt32(0) - SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect1.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1].SafeToInt32(0);
			}
			else if (level == 2)
			{
				num = SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect3.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1].SafeToInt32(0) - SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect2.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1].SafeToInt32(0);
			}
			else if (level == 3)
			{
				num = SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect4.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1].SafeToInt32(0) - SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect3.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1].SafeToInt32(0);
			}
			else if (level == 4)
			{
				num = SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect5.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1].SafeToInt32(0) - SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect4.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1].SafeToInt32(0);
			}
			attradd = num.ToString();
		}
		else
		{
			if (level == 1)
			{
				num2 = double.Parse(SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect2.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1]) * 100.0 - double.Parse(SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect1.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1]) * 100.0;
			}
			else if (level == 2)
			{
				num2 = double.Parse(SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect3.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1]) * 100.0 - double.Parse(SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect2.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1]) * 100.0;
			}
			else if (level == 3)
			{
				num2 = double.Parse(SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect4.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1]) * 100.0 - double.Parse(SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect3.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1]) * 100.0;
			}
			else if (level == 4)
			{
				num2 = double.Parse(SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect5.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1]) * 100.0 - double.Parse(SpiritTrackPart.GetDicShenJiFuWen()[ID].Effect4.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[1]) * 100.0;
			}
			attradd = string.Format("{0}%", num2);
		}
	}

	public void ChongZhiWeiZhi()
	{
		this.springPanel.target = new Vector3(0f, 0f, 0f);
		this.springPanel.enabled = true;
	}

	public void PlayAnimation()
	{
		this.SuccessAnim.gameObject.SetActive(true);
		this.PlayStart(this.SuccessAnim, new ActiveAnimation.OnFinished(this.PlayFinished));
	}

	private void PlayStart(Animation anim, ActiveAnimation.OnFinished onFinished)
	{
		ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, 1);
		if (activeAnimation == null)
		{
			return;
		}
		activeAnimation.onFinished = onFinished;
	}

	private void PlayFinished(ActiveAnimation anim)
	{
		anim.gameObject.SetActive(false);
	}

	public static Dictionary<int, ShenJiFuWen> GetDicShenJiFuWen()
	{
		if (SpiritTrackPart.dicShenJiFuWen.Count > 0)
		{
			return SpiritTrackPart.dicShenJiFuWen;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ShenJiFuWen.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShenJiFuWen");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ShenJiFuWen shenJiFuWen = new ShenJiFuWen();
			shenJiFuWen.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			shenJiFuWen.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			shenJiFuWen.Icon = Global.GetXElementAttributeStr(xelementList[i], "Icon");
			shenJiFuWen.Prev = Global.GetXElementAttributeInt(xelementList[i], "Prev");
			shenJiFuWen.PrevLevel = Global.GetXElementAttributeInt(xelementList[i], "PrevLevel");
			shenJiFuWen.MaxLevel = Global.GetXElementAttributeInt(xelementList[i], "MaxLevel");
			shenJiFuWen.UpNeed = Global.GetXElementAttributeInt(xelementList[i], "UpNeed");
			shenJiFuWen.X = Global.GetXElementAttributeFloat(xelementList[i], "X");
			shenJiFuWen.Y = Global.GetXElementAttributeFloat(xelementList[i], "Y");
			shenJiFuWen.Size = Global.GetXElementAttributeFloat(xelementList[i], "Size");
			shenJiFuWen.Effect1 = Global.GetXElementAttributeStr(xelementList[i], "Effect1");
			shenJiFuWen.Effect2 = Global.GetXElementAttributeStr(xelementList[i], "Effect2");
			shenJiFuWen.Effect3 = Global.GetXElementAttributeStr(xelementList[i], "Effect3");
			shenJiFuWen.Effect4 = Global.GetXElementAttributeStr(xelementList[i], "Effect4");
			shenJiFuWen.Effect5 = Global.GetXElementAttributeStr(xelementList[i], "Effect5");
			if (!SpiritTrackPart.dicShenJiFuWen.ContainsKey(shenJiFuWen.ID))
			{
				SpiritTrackPart.dicShenJiFuWen.Add(shenJiFuWen.ID, shenJiFuWen);
			}
			i++;
		}
		return SpiritTrackPart.dicShenJiFuWen;
	}

	public static void ClearXMLData()
	{
		if (0 < SpiritTrackPart.dicShenJiFuWen.Count)
		{
			SpiritTrackPart.dicShenJiFuWen.Clear();
		}
	}

	public List<SpiritTrackItemPart> spiritItems;

	public Dictionary<int, SpiritTrackItemPart> spiritDic = new Dictionary<int, SpiritTrackItemPart>();

	public List<Transform> xianluTeXiao;

	public List<GameObject> liuguangTeXiao;

	public Transform zhongxindian;

	public Transform BeijingZhongxindian;

	public Transform BeiJingQiu;

	public Transform spiritItemParent;

	public Transform xianluParent;

	public Transform zhongxinliuxing;

	public UILabel shenjidianLab;

	public ShowNetImage BG;

	public ShowNetImage WaiKuangBG1;

	public GButton BtnClose;

	public GButton BtnAdd;

	public GButton BtnChongZhi;

	public DPSelectedItemEventHandler DPSelectedClose;

	public Animation SuccessAnim;

	public SpringPanel springPanel;

	private int num;

	private int loadIndex;

	private int loadId;

	protected GChildWindow ShenJiJiFenZhuRuWindow;

	public ShenJiJiFenZhuRu ShenJiJiFenZhuRuPart;

	protected GChildWindow ShenJiDianCZWindow;

	public SpiritTrackXiDian ShenJiDianCZ;

	protected GChildWindow SpiritTrackItemAttrWindow;

	public SpiritTrackItemAttrPart spiritTrackItemAttrPart;

	private int indexTeXiao;

	private static Dictionary<int, ShenJiFuWen> dicShenJiFuWen = new Dictionary<int, ShenJiFuWen>();
}
