using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class AlchemyPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.AttrTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("提升属性")
		});
		for (int i = 0; i < this.AttributeName.Length; i++)
		{
			this.AttributeName[i].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("{0}："), this.AttrName[i])
			});
			this.AttributeNum[i].text = "0";
			this.AttributeNum[i].transform.localPosition = new Vector3(95f, 0f, 0f);
		}
		this.UseOneTimeElementNum.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			ConfigSystemParam.GetSystemParamIntArrayByName("AlchemyLevelUp", ',')[0]
		});
		this.UseTenTimeElementNum.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			(ConfigSystemParam.GetSystemParamIntArrayByName("AlchemyLevelUp", ',')[0] * 10).ToString()
		});
		this.BtnAlchemyOneTime.Label.text = Global.GetLang("炼金1次");
		this.BtnAlchemyTenTime.Label.text = Global.GetLang("炼金10次");
		this.SetBtnState();
		this.ElementNum.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			this.elementCount
		});
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnAddElement.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenAlchemyAddWindow();
		};
		this.BtnAlchemyOneTime.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DicLianJinTen.Clear();
			GameInstance.Game.SendAlchemyExcute(0);
			Super.ShowNetWaiting(null);
		};
		this.BtnAlchemyTenTime.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DicLianJinTen.Clear();
			GameInstance.Game.SendAlchemyExcute(1);
			Super.ShowNetWaiting(null);
		};
		UIEventListener.Get(this.AttributeNameRight[0].GetComponentInChildren<UISprite>().gameObject).onPress = delegate(GameObject sb, bool state)
		{
			this.TiShiAttrName(state, 0);
		};
		UIEventListener.Get(this.AttributeNameRight[1].GetComponentInChildren<UISprite>().gameObject).onPress = delegate(GameObject sb, bool state)
		{
			this.TiShiAttrName(state, 1);
		};
		UIEventListener.Get(this.AttributeNameRight[2].GetComponentInChildren<UISprite>().gameObject).onPress = delegate(GameObject sb, bool state)
		{
			this.TiShiAttrName(state, 2);
		};
		UIEventListener.Get(this.AttributeNameRight[3].GetComponentInChildren<UISprite>().gameObject).onPress = delegate(GameObject sb, bool state)
		{
			this.TiShiAttrName(state, 3);
		};
		UIEventListener.Get(this.AttributeNameRight[4].GetComponentInChildren<UISprite>().gameObject).onPress = delegate(GameObject sb, bool state)
		{
			this.TiShiAttrName(state, 4);
		};
		UIEventListener.Get(this.AttributeNameRight[5].GetComponentInChildren<UISprite>().gameObject).onPress = delegate(GameObject sb, bool state)
		{
			this.TiShiAttrName(state, 5);
		};
		UIEventListener.Get(this.AttributeNameRight[6].GetComponentInChildren<UISprite>().gameObject).onPress = delegate(GameObject sb, bool state)
		{
			this.TiShiAttrName(state, 6);
		};
		UIEventListener.Get(this.AttributeNameRight[7].GetComponentInChildren<UISprite>().gameObject).onPress = delegate(GameObject sb, bool state)
		{
			this.TiShiAttrName(state, 7);
		};
		UIEventListener.Get(this.AttributeNameRight[8].GetComponentInChildren<UISprite>().gameObject).onPress = delegate(GameObject sb, bool state)
		{
			this.TiShiAttrName(state, 8);
		};
		GameInstance.Game.SendAlchemyData();
		Super.ShowNetWaiting(null);
	}

	private void TiShiAttrName(bool state, int index)
	{
		if (state)
		{
			this.TiShiAttrName(index);
		}
		else
		{
			this.CloseAlchemyAttrWindow();
		}
	}

	private void TiShiAttrName(int index)
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("AlchemyProperty", ',');
		string name = string.Format("{0} +{1}", this.AttrName[index], systemParamIntArrayByName[index]);
		this.OpenAlchemyAttrWindow(name);
	}

	public void InitInfoToServerData(AlchemyData data)
	{
		if (data == null)
		{
			return;
		}
		this.ElementNum.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			data.Element
		});
		this.elementCount = data.Element;
		this.SetBtnState();
		if (data.AlchemyValue != null && data.AlchemyValue.Count > 0)
		{
			int i = 0;
			int num = this.AttributeNum.Length;
			while (i < num)
			{
				if (data.AlchemyValue.ContainsKey(i))
				{
					int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("AlchemyProperty", ',');
					this.AttributeNum[i].text = Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						systemParamIntArrayByName[i] * data.AlchemyValue[i]
					});
				}
				else
				{
					this.AttributeNum[i].text = Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						0
					});
				}
				i++;
			}
		}
	}

	public void ReElementNum(int elementNum)
	{
		this.ElementNum.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			elementNum
		});
		this.elementCount = elementNum;
		this.SetBtnState();
	}

	public void InitAddInfoToServerData(int type)
	{
		if (this.alchemyAddPart)
		{
			this.alchemyAddPart.InitAttr(type);
		}
	}

	private void SetBtnState()
	{
		if (this.elementCount >= ConfigSystemParam.GetSystemParamIntArrayByName("AlchemyLevelUp", ',')[0])
		{
			this.BtnAlchemyOneTime.isEnabled = true;
		}
		else
		{
			this.BtnAlchemyOneTime.isEnabled = false;
		}
		if (this.elementCount >= ConfigSystemParam.GetSystemParamIntArrayByName("AlchemyLevelUp", ',')[0] * 10)
		{
			this.BtnAlchemyTenTime.isEnabled = true;
		}
		else
		{
			this.BtnAlchemyTenTime.isEnabled = false;
		}
	}

	private GameObject LoadTeXiao(string path, Transform parent = null)
	{
		Object @object = Resources.Load(path);
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.SetParent(parent, false);
			U3DUtils.ResetLayer(gameObject, "MUUI");
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			return gameObject;
		}
		return null;
	}

	public void InitLianJinInfoToServerData(int element, List<string> alchemyProp)
	{
		this.AlchemyProp = alchemyProp;
		this.ReElementNum(element);
		this.OpenZuDangWindow();
		base.StopCoroutine(this.LianJinData());
		TTMonoBehaviour.Coroutine<bool> coroutine = base.StartCoroutine<bool>(this.LianJinData());
	}

	private IEnumerator LianJinData()
	{
		int a = 0;
		int len = this.AttributeNameRightTeXiao.Length;
		while (a < len)
		{
			this.DestoryTeXiao(this.AttributeNameRightTeXiao[a]);
			this.DestoryTeXiao(this.AttributeNameRight[a]);
			a++;
		}
		int b = 0;
		int len2 = this.AttributeNum.Length;
		while (b < len2)
		{
			this.DestoryTeXiao(this.LabTeXiao[b].gameObject);
			b++;
		}
		this.NengLiang.SetActive(false);
		this.NengLiang.SetActive(true);
		yield return new WaitForSeconds(2.7f);
		int lens = this.AlchemyProp.Count;
		if (lens > 1)
		{
			this.InitInfoToServerData(Global.Data.MyAlchemyData);
			for (int i = 0; i < lens; i++)
			{
				this.DestoryTeXiao(this.AttributeNameRightTeXiao[int.Parse(this.AlchemyProp[i])]);
				this.DestoryTeXiao(this.AttributeNameRight[int.Parse(this.AlchemyProp[i])]);
				GameObject go = this.LoadTeXiao(string.Format("{0}{1}", this.path, "lianjin_jihuo"), this.AttributeNameRight[int.Parse(this.AlchemyProp[i])].transform);
				go.transform.localPosition = new Vector3(0f, 0f, -5f);
				GameObject go2 = this.LoadTeXiao(string.Format("{0}{1}", this.path, "lianjin_xuanzhong"), this.AttributeNameRight[int.Parse(this.AlchemyProp[i])].transform);
				go2.transform.localPosition = new Vector3(0f, 0f, -5f);
				this.InitLianJinInfoToTen(int.Parse(this.AlchemyProp[i]));
				yield return new WaitForSeconds(0.1f);
			}
			for (int j = 0; j < lens; j++)
			{
				this.AddAlchemyValue(int.Parse(this.AlchemyProp[j]));
			}
		}
		else
		{
			for (int k = 0; k < lens; k++)
			{
				GameObject go3 = this.LoadTeXiao(string.Format("{0}{1}", this.path, "lianjin_jihuo"), this.AttributeNameRight[int.Parse(this.AlchemyProp[k])].transform);
				go3.transform.localPosition = new Vector3(0f, 0f, -5f);
				GameObject go4 = this.LoadTeXiao(string.Format("{0}{1}", this.path, "lianjin_xuanzhong"), this.AttributeNameRight[int.Parse(this.AlchemyProp[k])].transform);
				go4.transform.localPosition = new Vector3(0f, 0f, -5f);
				this.InitInfoToServerData(Global.Data.MyAlchemyData);
				this.InitLianJinInfo(int.Parse(this.AlchemyProp[k]));
				yield return new WaitForSeconds(0.1f);
			}
		}
		this.CloseZuDangWindow();
		yield break;
	}

	private void DestoryTeXiao(GameObject parent)
	{
		int i = 0;
		int childCount = parent.transform.childCount;
		while (i < childCount)
		{
			Transform transform = parent.transform.Find("lianjin_xuanzhong(Clone)");
			if (transform)
			{
				Object.Destroy(transform.gameObject);
			}
			Transform transform2 = parent.transform.Find("lianjin_jihuo(Clone)");
			if (transform2)
			{
				Object.Destroy(transform2.gameObject);
			}
			Transform transform3 = parent.transform.Find("lianjin_shuzishuaxin(Clone)");
			if (transform3)
			{
				Object.Destroy(transform3.gameObject);
			}
			i++;
		}
	}

	public void SetStarsPosition(Vector3[] positions, GameObject tagert)
	{
		tagert.transform.localPosition = positions[1];
		tagert.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.GetAngel(positions[1], positions[0])));
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
		return num + 90f;
	}

	private void InitLianJinInfo(int AlchemyProp)
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("AlchemyProperty", ',');
		this.AttributeNum[AlchemyProp].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			this.AttributeNum[AlchemyProp].text.ToString(),
			"17e43e",
			string.Format("   +{0}", systemParamIntArrayByName[AlchemyProp])
		});
		this.AddAlchemyValue(AlchemyProp);
		GameObject gameObject = this.LoadTeXiao(string.Format("{0}{1}", this.path, "lianjin_shuzishuaxin"), this.LabTeXiao[AlchemyProp].transform);
		gameObject.transform.localPosition = new Vector3(60f, 0f, 0f);
	}

	private void AddAlchemyValue(int AlchemyProp)
	{
		if (Global.Data.MyAlchemyData.AlchemyValue.ContainsKey(AlchemyProp))
		{
			Dictionary<int, int> alchemyValue;
			Dictionary<int, int> dictionary = alchemyValue = Global.Data.MyAlchemyData.AlchemyValue;
			int num = alchemyValue[AlchemyProp];
			dictionary[AlchemyProp] = num + 1;
		}
		else
		{
			Global.Data.MyAlchemyData.AlchemyValue.Add(AlchemyProp, 1);
		}
	}

	private void InitLianJinInfoToTen(int AlchemyProp)
	{
		if (this.DicLianJinTen.ContainsKey(AlchemyProp))
		{
			Dictionary<int, int> dicLianJinTen;
			Dictionary<int, int> dictionary = dicLianJinTen = this.DicLianJinTen;
			int num = dicLianJinTen[AlchemyProp];
			dictionary[AlchemyProp] = num + 1;
		}
		else
		{
			this.DicLianJinTen.Add(AlchemyProp, 1);
		}
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("AlchemyProperty", ',');
		if (Global.Data.MyAlchemyData != null && Global.Data.MyAlchemyData.AlchemyValue.ContainsKey(AlchemyProp))
		{
			this.AttributeNum[AlchemyProp].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.Data.MyAlchemyData.AlchemyValue[AlchemyProp] * systemParamIntArrayByName[AlchemyProp],
				"17e43e",
				string.Format("   +{0}", systemParamIntArrayByName[AlchemyProp] * this.DicLianJinTen[AlchemyProp])
			});
		}
		else
		{
			this.AttributeNum[AlchemyProp].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				0,
				"17e43e",
				string.Format("   +{0}", systemParamIntArrayByName[AlchemyProp] * this.DicLianJinTen[AlchemyProp])
			});
		}
		GameObject gameObject = this.LoadTeXiao(string.Format("{0}{1}", this.path, "lianjin_shuzishuaxin"), this.LabTeXiao[AlchemyProp].transform);
		gameObject.transform.localPosition = new Vector3(60f, 0f, 0f);
	}

	private void OpenZuDangWindow()
	{
		if (this.ZuDangWindow == null)
		{
			this.ZuDangWindow = U3DUtils.NEW<GChildWindow>();
			this.ZuDangWindow.IsShowModal = true;
			this.ZuDangWindow.ModalType = ChildWindowModalType.TransBak;
			Super.InitChildWindow(this.ZuDangWindow, Global.GetLang("灌注界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.ZuDangWindow);
			this.ZuDangWindow.ChildWindowClose = delegate(object s, EventArgs e)
			{
				this.CloseZuDangWindow();
				return true;
			};
		}
	}

	private void CloseZuDangWindow()
	{
		if (null != this.ZuDangWindow)
		{
			Super.CloseChildWindow(base.Children, this.ZuDangWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.ZuDangWindow, true);
			this.ZuDangWindow = null;
		}
	}

	private void OpenAlchemyAddWindow()
	{
		if (this.AlchemyAddWindow == null)
		{
			this.AlchemyAddWindow = U3DUtils.NEW<GChildWindow>();
			this.AlchemyAddWindow.IsShowModal = true;
			this.AlchemyAddWindow.ModalType = ChildWindowModalType.TransBak;
			Super.InitChildWindow(this.AlchemyAddWindow, Global.GetLang("灌注界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.AlchemyAddWindow);
			this.AlchemyAddWindow.ChildWindowClose = delegate(object s, EventArgs e)
			{
				this.CloseAlchemyAddWindow();
				return true;
			};
		}
		if (this.alchemyAddPart == null)
		{
			this.alchemyAddPart = U3DUtils.NEW<AlchemyAddPart>();
			this.alchemyAddPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseAlchemyAddWindow();
			};
		}
		this.AlchemyAddWindow.SetContent(this.AlchemyAddWindow.BodyPresenter, this.alchemyAddPart, 0.0, 0.0, true);
	}

	private void CloseAlchemyAddWindow()
	{
		if (null != this.alchemyAddPart)
		{
			this.alchemyAddPart.transform.parent = null;
			Object.Destroy(this.alchemyAddPart.gameObject);
			this.alchemyAddPart = null;
		}
		if (null != this.AlchemyAddWindow)
		{
			Super.CloseChildWindow(base.Children, this.AlchemyAddWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.AlchemyAddWindow, true);
			this.AlchemyAddWindow = null;
		}
	}

	private void OpenAlchemyAttrWindow(string name)
	{
		if (this.AlchemyAttrWindow == null)
		{
			this.AlchemyAttrWindow = U3DUtils.NEW<GChildWindow>();
			this.AlchemyAttrWindow.IsShowModal = true;
			this.AlchemyAttrWindow.ModalType = ChildWindowModalType.TransBak;
			Super.InitChildWindow(this.AlchemyAttrWindow, Global.GetLang("界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.AlchemyAttrWindow);
			this.AlchemyAttrWindow.ChildWindowClose = delegate(object s, EventArgs e)
			{
				this.CloseAlchemyAttrWindow();
				return true;
			};
		}
		if (this.AlchemyAttr == null)
		{
			this.AlchemyAttr = U3DUtils.NEW<AlchemyPartAttr>();
		}
		if (this.AlchemyAttr)
		{
			this.AlchemyAttr.Name = name;
		}
		this.AlchemyAttrWindow.SetContent(this.AlchemyAttrWindow.BodyPresenter, this.AlchemyAttr, 0.0, 0.0, true);
		this.AlchemyAttrWindow.Body.transform.localPosition = new Vector3(180f, 0f, 0f);
	}

	private void CloseAlchemyAttrWindow()
	{
		if (null != this.AlchemyAttr)
		{
			this.AlchemyAttr.transform.parent = null;
			Object.Destroy(this.AlchemyAttr.gameObject);
			this.AlchemyAttr = null;
		}
		if (null != this.AlchemyAttrWindow)
		{
			Super.CloseChildWindow(base.Children, this.AlchemyAttrWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.AlchemyAttrWindow, true);
			this.AlchemyAttrWindow = null;
		}
	}

	public static Dictionary<int, CurrencyConversion> GetDicCurrencyConversion()
	{
		if (AlchemyPart.dicCurrencyConversion.Count > 0)
		{
			return AlchemyPart.dicCurrencyConversion;
		}
		XElement gameResXml = Global.GetGameResXml("Config/CurrencyConversion.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "CurrencyConversion");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			CurrencyConversion currencyConversion = new CurrencyConversion();
			currencyConversion.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			currencyConversion.Type = Global.GetXElementAttributeInt(xelementList[i], "Type");
			currencyConversion.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			currencyConversion.Unit = Global.GetXElementAttributeInt(xelementList[i], "Unit");
			currencyConversion.Element = Global.GetXElementAttributeInt(xelementList[i], "Element");
			currencyConversion.Limit = Global.GetXElementAttributeInt(xelementList[i], "Limit");
			if (!AlchemyPart.dicCurrencyConversion.ContainsKey(currencyConversion.Type))
			{
				AlchemyPart.dicCurrencyConversion.Add(currencyConversion.Type, currencyConversion);
			}
			i++;
		}
		return AlchemyPart.dicCurrencyConversion;
	}

	public static void ClearXMLData()
	{
		if (AlchemyPart.dicCurrencyConversion.Count > 0)
		{
			AlchemyPart.dicCurrencyConversion.Clear();
		}
	}

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel AttrTitle;

	public UILabel[] AttributeName;

	public UILabel[] AttributeNum;

	public GButton BtnClose;

	public GButton BtnAddElement;

	public GButton BtnAlchemyOneTime;

	public GButton BtnAlchemyTenTime;

	public UILabel ElementNum;

	public UILabel UseOneTimeElementNum;

	public UILabel UseTenTimeElementNum;

	public GameObject[] AttributeNameRight;

	public GameObject[] AttributeNameRightTeXiao;

	public GameObject NengLiang;

	public GameObject[] LabTeXiao;

	private int elementCount;

	private string[] AttrName = new string[]
	{
		Global.GetLang("生命上限"),
		Global.GetLang("攻  击  力"),
		Global.GetLang("防  御  力"),
		Global.GetLang("命      中"),
		Global.GetLang("闪      避"),
		Global.GetLang("附加伤害"),
		Global.GetLang("抵挡伤害"),
		Global.GetLang("击中恢复"),
		Global.GetLang("反弹伤害")
	};

	private List<string> AlchemyProp = new List<string>();

	private string path = "UITeXiao/Perfabs/lianjin/";

	private Dictionary<int, int> DicLianJinTen = new Dictionary<int, int>();

	protected GChildWindow ZuDangWindow;

	protected GChildWindow AlchemyAddWindow;

	protected AlchemyAddPart alchemyAddPart;

	protected GChildWindow AlchemyAttrWindow;

	protected AlchemyPartAttr AlchemyAttr;

	private static Dictionary<int, CurrencyConversion> dicCurrencyConversion = new Dictionary<int, CurrencyConversion>();
}
