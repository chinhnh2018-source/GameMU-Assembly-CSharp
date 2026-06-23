using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class TianFuSYDS : UserControl
{
	protected override void InitializeComponent()
	{
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(base.gameObject, new DPSelectedItemEventArgs
			{
				ID = -1
			});
			this.IsTianChong = false;
			base.StopCoroutine("TweenPercent_Tick");
			base.gameObject.SetActive(false);
		};
		this.ZhuRu_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsTianChong)
			{
				GameInstance.Game.TianFuZhuRuJingYan();
			}
		};
		this.InitStr();
	}

	private void InitStr()
	{
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("天赋点数")
		});
		this.ZhuRu_Btn.Text = Global.GetLang("注入");
		this.SYDS_explain.text = string.Empty;
		this.SYDS_explain.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ff0d00",
			Global.GetLang("注入经验将消耗人物所有EXP")
		});
	}

	private new void Start()
	{
	}

	public void Suc_TianChongPoint()
	{
		this.DPSelectedItem(base.gameObject, new DPSelectedItemEventArgs
		{
			ID = -1
		});
		this.Init();
	}

	public void Init()
	{
		this.IsTianChong = false;
		this.InitBar();
		this.info();
	}

	public void InitBar()
	{
		this.bar.BodyHeight = 12.0;
		this.bar.BodyWidth = 368.0;
		this.bar.Percent = 0.0;
		this.BarBack.gameObject.transform.localScale = new Vector3(0f, 12f, 0f);
	}

	public void UpdateStrExp(long NeedExp)
	{
		if (Global.Data.roleData.Experience >= NeedExp - Global.Data.roleData.MyTalentData.Exp)
		{
			this.SYDS_explain.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(Global.GetLang("还需注入经验：{0}/{1}"), NeedExp - Global.Data.roleData.MyTalentData.Exp, NeedExp - Global.Data.roleData.MyTalentData.Exp)
			});
		}
		else
		{
			this.SYDS_explain.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(Global.GetLang("还需注入经验：{0}/{1}"), Global.Data.roleData.Experience, NeedExp - Global.Data.roleData.MyTalentData.Exp)
			});
		}
	}

	private XElement getXelement(string count)
	{
		XElement gameResXml = Global.GetGameResXml("Config/TianFuDian.Xml");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "TianFuDian", "TianFuDian", count);
		return xelementList[0];
	}

	private void info()
	{
		if (Global.Data.roleData.MyTalentData == null)
		{
			return;
		}
		int num = Global.Data.roleData.MyTalentData.TotalCount + 1;
		string text = string.Empty;
		if (Global.Data.roleData.MyTalentData.TotalCount >= int.Parse(ConfigSystemParam.GetSystemParamByName("MaxTianFu", true)))
		{
			this.IconSp.gameObject.SetActive(false);
			this.TiaoJianLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0d00",
				Global.GetLang("天赋点数已满")
			});
			this.TiaoJianLabel.gameObject.SetActive(true);
			this.InitBar();
			this.ZhuRu_Btn.isEnabled = false;
			return;
		}
		XElement xelement = this.getXelement(num.ToString());
		int num2 = int.Parse(Global.GetXElementAttributeStr(xelement, "TianFuDian"));
		text = Global.GetXElementAttributeStr(xelement, "NeedLevel");
		long num3 = long.Parse(Global.GetXElementAttributeStr(xelement, "NeedExp"));
		this.UpdateStrExp(num3);
		string[] array = text.Split(new char[]
		{
			','
		});
		int num4 = int.Parse(array[0]) * 100 + int.Parse(array[1]);
		int num5 = Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level;
		if (num5 >= num4)
		{
			double num6 = (double)Global.Data.roleData.MyTalentData.Exp / (double)num3;
			double num7 = (double)(num3 - Global.Data.roleData.MyTalentData.Exp);
			double num8 = (double)Global.Data.roleData.Experience / num7;
			if (num8 >= 1.0)
			{
				this.SetJinDuTiao(num6, 368.0);
			}
			else
			{
				double num9 = 368.0 * num6;
				double num10 = (368.0 - num9) * num8;
				this.SetJinDuTiao(num6, num9 + num10);
			}
			this.IconSp.gameObject.SetActive(true);
			this.TiaoJianLabel.gameObject.SetActive(false);
			this.ZhuRu_Btn.isEnabled = true;
		}
		else
		{
			this.IconSp.gameObject.SetActive(false);
			this.TiaoJianLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0d00",
				string.Format(Global.GetLang("角色等级:{0}转{1}级"), int.Parse(array[0]), int.Parse(array[1]))
			});
			this.TiaoJianLabel.gameObject.SetActive(true);
			this.InitBar();
			this.ZhuRu_Btn.isEnabled = false;
		}
	}

	public void SetJinDuTiao(double curr, double max)
	{
		this.bar.Percent = curr;
		if (max > 368.0)
		{
			max = 368.0;
		}
		this.BarBack.gameObject.transform.localScale += new Vector3((float)max, 0f, 0f);
	}

	public void SetJinDuTiaoTianChong(int type)
	{
		XElement gameResXml = Global.GetGameResXml("Config/TianFuDian.Xml");
		int num = Global.Data.roleData.MyTalentData.TotalCount + 1;
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "TianFuDian", "TianFuDian", num.ToString());
		XElement xelement = xelementList[0];
		long num2 = long.Parse(Global.GetXElementAttributeStr(xelement, "NeedExp"));
		double num3 = (double)(num2 - Global.Data.roleData.MyTalentData.Exp);
		double num4 = (double)Global.Data.roleData.Experience / (double)num2;
		double endPersent = (double)Global.Data.roleData.MyTalentData.Exp / (double)num2;
		this.UpdateStrExp(num2);
		if (type == 2)
		{
			this.IsTianChong = true;
			this.TotalTicks = 800.0;
			this.StartPersent = this.bar.Percent;
			this.EndPersent = 1.0;
			this.LastTicks = (double)Global.GetCorrectLocalTime();
			this.ElapsedNumTicks = 0.0;
			base.StartCoroutine("TweenPercent_Tick");
		}
		else
		{
			this.bar.TweenPercent(this.bar.Percent, endPersent, 0.8);
		}
	}

	private IEnumerator TweenPercent_Tick()
	{
		for (;;)
		{
			double ticks = (double)Global.GetCorrectLocalTime();
			double subTicks = ticks - this.LastTicks;
			this.LastTicks = ticks;
			this.ElapsedNumTicks += subTicks;
			double persent = this.ElapsedNumTicks / this.TotalTicks;
			this.SetTweenPersent(persent);
			if (persent >= 1.0)
			{
				base.StopCoroutine("TweenPercent_Tick");
				this.bar.Percent = Math.Min(this.EndPersent, 1.0);
				if (this.JiaDianTeXiao.activeSelf)
				{
					this.JiaDianTeXiao.SetActive(false);
				}
				this.JiaDianTeXiao.SetActive(true);
				this.Suc_TianChongPoint();
			}
			yield return new WaitForSeconds(0.01f);
		}
		yield break;
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

	private void SetTweenPersent(double persent)
	{
		double num = (this.EndPersent - this.StartPersent) * persent + this.StartPersent;
		this.bar.Percent = (double)((float)num);
	}

	private void updateBar()
	{
		this.bar.Percent += 0.0040000001899898052;
		if (this.bar.Percent >= 1.0)
		{
			this.bar.Percent = 1.0;
			this.Suc_TianChongPoint();
			base.CancelInvoke("updateBar");
		}
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

	private bool IsTianChong;

	public Animation LianluAnim;

	public GameObject JiaDianTeXiao;

	private double TotalTicks = 1000.0;

	private double LastTicks;

	private double ElapsedNumTicks;

	private double StartPersent;

	private double EndPersent;
}
