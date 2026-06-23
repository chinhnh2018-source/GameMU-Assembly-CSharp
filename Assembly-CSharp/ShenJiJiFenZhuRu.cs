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

public class ShenJiJiFenZhuRu : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("神迹点数")
		});
		this.ZhuRu_Btn.Text = Global.GetLang("注入");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitShenJiDian(1);
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		};
		this.ZhuRu_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.nowTime = Global.GetCorrectDateTime().Ticks;
			if (this.nowTime - this.lastTime < 10000000L)
			{
				Super.HintMainText(Global.GetLang("请不要过于频繁操作！"), 10, 3);
				return;
			}
			this.lastTime = this.nowTime;
			if (this.lv[0].SafeToInt32(0) == 100 && this.lv[1].SafeToInt32(0) == 100)
			{
				Super.HintMainText(Global.GetLang("神迹点数已满"), 10, 3);
				return;
			}
			if (this.canZhuRu)
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.ShenJiZhuRu();
			}
			else
			{
				Super.HintMainText(Global.GetLang("等级不足"), 10, 3);
			}
		};
	}

	public void InitBar()
	{
		this.bar.BodyHeight = 12.0;
		this.bar.BodyWidth = 368.0;
		this.bar.Percent = 0.0;
		this.BarBack.gameObject.transform.localScale = new Vector3(0f, 12f, 0f);
	}

	public void UpServerToClient()
	{
		base.StartCoroutine<bool>(this.ServerToClientUpDate());
	}

	private IEnumerator ServerToClientUpDate()
	{
		double start = (double)this.OldyizhuruShenJiJiFen / (double)this.oldneedShenJiJiFen;
		double end = (double)((this.oldmax / 389f < 1f) ? (this.oldmax / 389f) : 1f);
		this.bar.TweenPercent(start, end, 0.8);
		yield return new WaitForSeconds(0.8f);
		this.UpDateShenJiDian(2);
		long needJiFen = 0L;
		long xianyouJiFen = 0L;
		if (this.needShenJiJiFen - this.yizhuruShenJiJiFen < 0L)
		{
			needJiFen = 0L;
		}
		else
		{
			needJiFen = this.needShenJiJiFen - this.yizhuruShenJiJiFen;
		}
		if (this.xianyouShenJiJiFen > needJiFen)
		{
			xianyouJiFen = needJiFen;
		}
		else
		{
			xianyouJiFen = this.xianyouShenJiJiFen;
		}
		this.SYDS_explain.text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			string.Format(Global.GetLang("还需注入神迹积分: {0}/{1}"), xianyouJiFen, needJiFen)
		});
		yield break;
	}

	public void UpDateShenJiDian(int type)
	{
		this.xianyouShenJiDian = Global.Data.roleData.RoleCommonUseIntPamams[46];
		this.xianyouShenJiJiFen = (long)Global.Data.roleData.RoleCommonUseIntPamams[47];
		this.yizhuruShenJiJiFen = (long)Global.Data.roleData.RoleCommonUseIntPamams[48];
		this.XianYouJiFenLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			string.Format(Global.GetLang("现有神迹积分: {0}"), this.xianyouShenJiJiFen)
		});
		this.XianYouDianShuLab.text = string.Format(Global.GetLang("神迹点数：{0}"), this.xianyouShenJiDian);
		if (type == 2 && this.num < Global.Data.roleData.RoleCommonUseIntPamams[46])
		{
			string path = "UITeXiao/Perfabs/jinglingshenji/shenji_dian_huode";
			Transform parent = this.shenjidian;
			this.LoadTeXiao(path, parent, false, false);
		}
		this.num = this.xianyouShenJiDian;
		this.totalShenJiDian = 0;
		if (Global.Data.roleData.ShenJiDict != null && Global.Data.roleData.ShenJiDict.Count != 0)
		{
			Dictionary<int, ShenJiFuWenData>.Enumerator enumerator = Global.Data.roleData.ShenJiDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Dictionary<int, ShenJiFuWen> dicShenJiFuWen = SpiritTrackPart.GetDicShenJiFuWen();
				KeyValuePair<int, ShenJiFuWenData> keyValuePair = enumerator.Current;
				if (dicShenJiFuWen.ContainsKey(keyValuePair.Value.ID))
				{
					int num = this.totalShenJiDian;
					Dictionary<int, ShenJiFuWen> dicShenJiFuWen2 = SpiritTrackPart.GetDicShenJiFuWen();
					KeyValuePair<int, ShenJiFuWenData> keyValuePair2 = enumerator.Current;
					int upNeed = dicShenJiFuWen2[keyValuePair2.Value.ID].UpNeed;
					KeyValuePair<int, ShenJiFuWenData> keyValuePair3 = enumerator.Current;
					this.totalShenJiDian = num + upNeed * keyValuePair3.Value.Level;
				}
			}
		}
		this.totalShenJiDian += this.xianyouShenJiDian;
		if (ShenJiJiFenZhuRu.GetDicShenJiDian().ContainsKey(this.totalShenJiDian + 1))
		{
			int num2 = Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level;
			this.lv = ShenJiJiFenZhuRu.GetDicShenJiDian()[this.totalShenJiDian + 1].NeedLevel.Split(new char[]
			{
				'|'
			});
			if (this.lv[0].SafeToInt32(0) * 100 + this.lv[1].SafeToInt32(0) <= num2)
			{
				this.canZhuRu = true;
			}
			else
			{
				if (this.lv[0].SafeToInt32(0) == 100 && this.lv[1].SafeToInt32(0) == 100)
				{
					this.TiaoJianLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"FF0000",
						Global.GetLang("神迹点数已满")
					});
				}
				else
				{
					this.TiaoJianLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"FF0000",
						string.Format(Global.GetLang("角色等级:{0}转{1}级"), this.lv[0], this.lv[1])
					});
				}
				this.IconSp.gameObject.SetActive(false);
				this.ZhuRu_Btn.isEnabled = false;
			}
			this.needShenJiJiFen = ShenJiJiFenZhuRu.GetDicShenJiDian()[this.totalShenJiDian + 1].NeedShenJi;
		}
		float num3 = (float)(this.xianyouShenJiJiFen + this.yizhuruShenJiJiFen) / (float)this.needShenJiJiFen * 389f;
		if (num3 > 389f)
		{
			num3 = 389f;
		}
		double percent = (double)this.yizhuruShenJiJiFen / (double)this.needShenJiJiFen;
		this.bar.Percent = percent;
		if (type == 1)
		{
		}
		this.BarBack.gameObject.transform.localScale = new Vector3(num3, 12f, 0f);
		this.OldyizhuruShenJiJiFen = this.yizhuruShenJiJiFen;
		this.oldmax = num3;
		this.oldneedShenJiJiFen = this.needShenJiJiFen;
	}

	public void InitShenJiDian(int type)
	{
		this.InitBar();
		this.UpDateShenJiDian(type);
		long num;
		if (this.needShenJiJiFen - this.yizhuruShenJiJiFen < 0L)
		{
			num = 0L;
		}
		else
		{
			num = this.needShenJiJiFen - this.yizhuruShenJiJiFen;
		}
		long num2;
		if (this.xianyouShenJiJiFen > num)
		{
			num2 = num;
		}
		else
		{
			num2 = this.xianyouShenJiJiFen;
		}
		this.SYDS_explain.text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			string.Format(Global.GetLang("还需注入神迹积分: {0}/{1}"), num2, num)
		});
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

	public static void ClearXMLData()
	{
		if (ShenJiJiFenZhuRu.dicShenJiDian.Count > 0)
		{
			ShenJiJiFenZhuRu.dicShenJiDian.Clear();
		}
	}

	public static Dictionary<int, ShenJiDian> GetDicShenJiDian()
	{
		if (ShenJiJiFenZhuRu.dicShenJiDian.Count > 0)
		{
			return ShenJiJiFenZhuRu.dicShenJiDian;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ShenJiDian.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShenJiDian");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ShenJiDian shenJiDian = new ShenJiDian();
			shenJiDian.ShenJiDiannum = Global.GetXElementAttributeInt(xelementList[i], "ShenJiDian");
			shenJiDian.NeedShenJi = Global.GetXElementAttributeLong(xelementList[i], "NeedShenJi");
			shenJiDian.NeedLevel = Global.GetXElementAttributeStr(xelementList[i], "NeedLevel");
			if (!ShenJiJiFenZhuRu.dicShenJiDian.ContainsKey(shenJiDian.ShenJiDiannum))
			{
				ShenJiJiFenZhuRu.dicShenJiDian.Add(shenJiDian.ShenJiDiannum, shenJiDian);
			}
			i++;
		}
		return ShenJiJiFenZhuRu.dicShenJiDian;
	}

	public UISprite BarBack;

	public GButton Close;

	public GImgProgressBar bar;

	public UILabel Title;

	public UILabel SYDS_explain;

	public GButton ZhuRu_Btn;

	public UILabel TiaoJianLabel;

	public UISprite IconSp;

	public DPSelectedItemEventHandler DPSelectedItem;

	public Animation LianluAnim;

	public GameObject JiaDianTeXiao;

	public int xianyouShenJiDian;

	public long xianyouShenJiJiFen;

	public long yizhuruShenJiJiFen;

	public long needShenJiJiFen;

	public int totalShenJiDian;

	public long OldyizhuruShenJiJiFen;

	public float oldmax;

	public long oldneedShenJiJiFen;

	public UILabel XianYouJiFenLab;

	public UILabel XianYouDianShuLab;

	public Transform shenjidian;

	private int num;

	private bool canZhuRu;

	private string[] lv;

	private long nowTime;

	private long lastTime;

	private static Dictionary<int, ShenJiDian> dicShenJiDian = new Dictionary<int, ShenJiDian>();
}
