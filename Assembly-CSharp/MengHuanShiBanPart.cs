using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class MengHuanShiBanPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitOnCkick();
		this.InitText();
		this.RefreshHuoBi();
		GameInstance.Game.SendMengHuanStoneData();
		GameInstance.Game.SendMengHuanStoneLogs();
	}

	private void InitText()
	{
		this.labContent.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("每次闯荡不会获得已获得的物品")
		});
		this.btnOne.Text = Global.GetLang("闯荡1次");
		this.btnTen.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("闯荡9次")
		});
		this._btnGoodsConfirm.Text = Global.GetLang("确定");
		this._btnJiHuoConfirm.Label.text = Global.GetLang("确定");
		this._labGoodsTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("恭喜获得")
		});
		this.labZuanShi.pivot = 3;
		this.labZuanShi.transform.localPosition = new Vector3(40f, this.labZuanShi.transform.localPosition.y, this.labZuanShi.transform.localPosition.z);
	}

	private void InitOnCkick()
	{
		this._btnJiHuoClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this._panelJiHuo.gameObject.SetActive(false);
		};
		this._btnJiHuoConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this._panelJiHuo.gameObject.SetActive(false);
		};
		this._btnGoodsClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this._panelGoods.gameObject.SetActive(false);
		};
		this._btnGoodsConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this._panelGoods.gameObject.SetActive(false);
		};
		this.btnOne.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this._endTime.Ticks / 10000L < Global.GetCorrectLocalTime())
			{
				Super.HintMainText(Global.GetLang("活动已结束，无法进行相应操作，请等待下次活动开启"), 10, 3);
				return;
			}
			if (this._boolChouJiangZhong)
			{
				return;
			}
			if (this._DicData.ContainsKey((int)this._BtnTyp) && this._DicData[(int)this._BtnTyp].State == 1)
			{
				Super.HintMainText(Global.GetLang("很抱歉石板未激活，无法进行相关操作"), 10, 3);
				return;
			}
			if (Global.Data.roleData != null && Global.Data.roleData.GoodsDataList != null && Global.GetBaoGuoSpaceCount() <= 0)
			{
				Super.HintMainText(Global.GetLang("背包栏位不足，无法进行相应操作，请清理背包"), 10, 3);
				return;
			}
			this._ChoujiangType = MengHuanShiBanPart.ChouJiangType.one;
			this.SetChouJiang();
		};
		this.btnTen.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this._endTime.Ticks / 10000L < Global.GetCorrectLocalTime())
			{
				Super.HintMainText(Global.GetLang("活动已结束，无法进行相应操作，请等待下次活动开启"), 10, 3);
				return;
			}
			if (this._boolChouJiangZhong)
			{
				return;
			}
			if (this._DicData.ContainsKey((int)this._BtnTyp) && this._DicData[(int)this._BtnTyp].State == 1)
			{
				Super.HintMainText(Global.GetLang("很抱歉石板未激活，无法进行相关操作"), 10, 3);
				return;
			}
			if (Global.Data.roleData != null && Global.Data.roleData.GoodsDataList != null && Global.GetBaoGuoSpaceCount() < this._Data.LivTime)
			{
				Super.HintMainText(Global.GetLang("背包栏位不足，无法进行相应操作，请清理背包"), 10, 3);
				return;
			}
			this._ChoujiangType = MengHuanShiBanPart.ChouJiangType.more;
			this.listJiangPing.Clear();
			this.SetChouJiang();
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.dpsClose != null)
			{
				this.dpsClose(this, new DPSelectedItemEventArgs());
			}
		};
		this.btnOpenHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenHelpWindow("Config/HuanMengSlateIntro.xml");
		};
		this.BtnJin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RefreshBtn(StoneType.Jin);
		};
		this.BtnYin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RefreshBtn(StoneType.Yin);
		};
		this.BtnTong.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RefreshBtn(StoneType.Tong);
		};
		this.BtnJin.TagIndex = 3;
		this.BtnYin.TagIndex = 2;
		this.BtnTong.TagIndex = 1;
		this._ListBtn.Add(this.BtnJin);
		this._ListBtn.Add(this.BtnYin);
		this._ListBtn.Add(this.BtnTong);
		this._panelJiHuo.gameObject.SetActive(false);
	}

	private void SetChouJiang()
	{
		int num = 0;
		if (this._ChoujiangType == MengHuanShiBanPart.ChouJiangType.one)
		{
			num = this.labOne.text.SafeToInt32(0);
		}
		else if (this._ChoujiangType == MengHuanShiBanPart.ChouJiangType.more)
		{
			num = this.labTen.text.SafeToInt32(0);
		}
		if (Global.GetRoleOwnNumByMoneyType(163) < num)
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = num - Global.GetRoleOwnNumByMoneyType(163);
			string lang = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang(string.Format(lang, IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi))
			}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					GameInstance.Game.SendBoCaiDaiBi(IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi, 2);
				}
				return true;
			};
			return;
		}
		base.StartCoroutine<bool>(this.StarChouJiangZhuanQuan());
	}

	private void RefreshBtn(StoneType type)
	{
		if (this._BtnTyp == type)
		{
			return;
		}
		if (this._boolChouJiangZhong)
		{
			return;
		}
		this._BtnTyp = type;
		for (int i = 0; i < this._ListBtn.Count; i++)
		{
			if (this._ListBtn[i].TagIndex == (int)type)
			{
				this._ListBtn[i].BtnState.gameObject.SetActive(true);
				this._GameTeXiaoBtnSelect.gameObject.transform.localPosition = new Vector3(0f, this._ListBtn[i].transform.localPosition.y, -3f);
				if (!this._GameTeXiaoBtnSelect.gameObject.activeSelf)
				{
					this._GameTeXiaoBtnSelect.gameObject.SetActive(true);
				}
			}
			else
			{
				this._ListBtn[i].BtnState.gameObject.SetActive(false);
			}
		}
		base.StopAllCoroutines();
		base.StartCoroutine<bool>(this.StartTween());
		this.RefrshMain(type);
	}

	private void RefrshMain(StoneType type)
	{
		if (!this._DicData.ContainsKey((int)type))
		{
			return;
		}
		this._xmlData = IConfigbase<ConfigMengHuanSlateRewardPool>.Instance.GetHuanMengSlateRewardPoolVODataById((int)type);
		if (this._xmlData == null)
		{
			return;
		}
		this._Data = this._DicData[(int)type];
		if (this._DicData.ContainsKey(2) && this._DicData[2].ActCount > 0)
		{
			this._labJiHuoYinNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this._DicData[2].ActCount.ToString()
			});
		}
		else
		{
			this._labJiHuoYinNum.text = string.Empty;
		}
		if (this._DicData.ContainsKey(3) && this._DicData[3].ActCount > 0)
		{
			this._labJiHuoJinNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this._DicData[3].ActCount.ToString()
			});
		}
		else
		{
			this._labJiHuoJinNum.text = string.Empty;
		}
		int num = this.XiaoHaoNum(this._Data.LivTime, this._xmlData);
		this.labOne.text = num.ToString();
		this.btnTen.Text = Global.GetLang(string.Format(Global.GetLang("闯荡{0}次"), this._Data.LivTime));
		int num2 = 0;
		string[] array = this._xmlData.NeedLuckStar.Split(new char[]
		{
			','
		});
		for (int i = 9 - this._Data.LivTime; i < array.Length; i++)
		{
			num2 += array[i].SafeToInt32(0);
		}
		this.labTen.text = num2.ToString();
		this.spJinDuQuan.fillAmount = (float)this._Data.CurrCount / (float)this._xmlData.ActivatedNum;
	}

	private int XiaoHaoNum(int LivTime, HuanMengSlateRewardPoolVO vo)
	{
		string[] array = vo.NeedLuckStar.Split(new char[]
		{
			','
		});
		return array[9 - LivTime].SafeToInt32(0);
	}

	public void AddLogItem(DreamStoneLogInfo data)
	{
		this.listLog.Insert(0, data);
		this.RefreshLog();
	}

	public void AddLog(DreamStoneS2C<List<DreamStoneLogInfo>> data)
	{
		if (this.listLog != null && data != null && data.V != null)
		{
			for (int i = 0; i < data.V.Count; i++)
			{
				this.listLog.Add(data.V[i]);
			}
		}
		this.RefreshLog();
	}

	private void RefreshLog()
	{
		if (this.listLog.Count > 15)
		{
			this.listLog.RemoveAt(this.listLog.Count - 1);
		}
		MUDebug.Log<string>(new string[]
		{
			"listLog.Count" + this.listLog.Count
		});
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < this.listLog.Count; i++)
		{
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				this.listLog[i].RoleName
			});
			if (this.listLog[i].Type == 1)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.listLog[i].GoodsID);
				string colorStringForNGUIText2 = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					goodsXmlNodeByID.Title
				});
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("系统：恭喜{0}获得天使祝福成功获得{1}"), colorStringForNGUIText, colorStringForNGUIText2)
				}));
			}
			else if (this.listLog[i].Type == 2)
			{
				string colorStringForNGUIText3 = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					this.listLog[i].StoneName
				});
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("系统：恭喜{0}获得天使祝福成功激活{1}"), colorStringForNGUIText, colorStringForNGUIText3)
				}));
			}
			stringBuilder.Append(Environment.NewLine);
		}
		this.labText.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			stringBuilder.ToString()
		});
	}

	public void InitReturn(DSTableInfo dicdata)
	{
		string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			dicdata.Start.ToString("yyyy.MM.dd")
		});
		string colorStringForNGUIText2 = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			dicdata.End.ToString("yyyy.MM.dd")
		});
		this._endTime = dicdata.End;
		this.labTime.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format(Global.GetLang("活动持续时间：{0}——{1}"), colorStringForNGUIText, colorStringForNGUIText2)
		}));
		this._DicData = dicdata.DStoneData;
		this.RefreshBtnsNormal();
		this.RefreshBtn(StoneType.Tong);
	}

	private void RefreshBtnsNormal()
	{
		if (this._DicData.ContainsKey(3))
		{
			if (this._DicData[3].State == 0)
			{
				this.BtnJin.normalSprite = "Anniu_03";
				this.BtnJin.hoverSprite = "Anniu_03";
				this.BtnJin.pressedSprite = "Anniu_03";
				this.BtnJin.target.spriteName = "Anniu_03";
				this.spJinTitle.color = Color.white;
			}
			else
			{
				this.BtnJin.normalSprite = "Anniu_03_Hui";
				this.BtnJin.hoverSprite = "Anniu_03_Hui";
				this.BtnJin.pressedSprite = "Anniu_03_Hui";
				this.BtnJin.target.spriteName = "Anniu_03_Hui";
				this.spJinTitle.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			}
		}
		if (this._DicData.ContainsKey(2))
		{
			if (this._DicData[2].State == 0)
			{
				this.BtnYin.normalSprite = "Anniu_02";
				this.BtnYin.hoverSprite = "Anniu_02";
				this.BtnYin.pressedSprite = "Anniu_02";
				this.BtnYin.target.spriteName = "Anniu_02";
				this.spYinTitle.color = Color.white;
			}
			else
			{
				this.BtnYin.normalSprite = "Anniu_02_Hui";
				this.BtnYin.hoverSprite = "Anniu_02_Hui";
				this.BtnYin.pressedSprite = "Anniu_02_Hui";
				this.BtnYin.target.spriteName = "Anniu_02_Hui";
				this.spYinTitle.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			}
		}
		if (this._DicData.ContainsKey(1))
		{
			if (this._DicData[1].State == 0)
			{
				this.BtnTong.normalSprite = "Anniu_01";
				this.BtnTong.hoverSprite = "Anniu_01";
				this.BtnTong.pressedSprite = "Anniu_01";
				this.BtnTong.target.spriteName = "Anniu_01";
				this.spTongTitle.color = Color.white;
			}
			else
			{
				this.BtnTong.normalSprite = "Anniu_01_Hui";
				this.BtnTong.hoverSprite = "Anniu_01_Hui";
				this.BtnTong.pressedSprite = "Anniu_01_Hui";
				this.BtnTong.target.spriteName = "Anniu_01_Hui";
				this.spTongTitle.color = new Color(0.5f, 0.5f, 0.5f, 1f);
			}
		}
	}

	public void RetChouJiang(DreamStoneS2C<LuckyDrawRes> retData)
	{
		this.RefreshHuoBi();
		if (retData.Result != 1)
		{
			Super.HintMainText(IConfigbase<ConfigMengHuanSlateRewardPool>.Instance.GetError(retData.Result), 10, 3);
			this.choujiangFlag = false;
			this._boolChouJiangZhong = false;
			return;
		}
		MUDebug.Log<string>(new string[]
		{
			"_Data.LivTime " + retData.V.PosDict[(int)this._BtnTyp].LivTime
		});
		this.nextType = this._BtnTyp;
		this.nextType = this.RfreshNextBtn(retData);
		if (this._ChoujiangType == MengHuanShiBanPart.ChouJiangType.more)
		{
			GGoodIcon[] componentsInChildren = this._slYuanQuan.GetComponentsInChildren<GGoodIcon>();
			GoodsData goodsData = componentsInChildren[retData.V.PosInfo.Site - 1].ItemObject as GoodsData;
			this.listJiangPing.Add(goodsData);
			if (retData.V.PosDict[(int)this._BtnTyp].LivTime <= 0 || retData.V.PosDict[(int)this._BtnTyp].LivTime >= 9)
			{
				base.StartCoroutine<bool>(this.StarChouJiangGoods(retData.V.PosInfo.Site, true, false, true));
			}
			else if (this.JiHuoBool(retData.V.PosDict))
			{
				this.SetJiangPing(retData.V.PosInfo, true, true);
			}
			else
			{
				if (goodsData != null)
				{
					componentsInChildren[retData.V.PosInfo.Site - 1].GoodImg.ToGrayBitmap = true;
				}
				GameInstance.Game.SendMengHuanStoneChouJiangOne(this._DicData[(int)this._BtnTyp].Type);
			}
		}
		else if (this._ChoujiangType == MengHuanShiBanPart.ChouJiangType.one)
		{
			if (retData.V.PosDict[(int)this._BtnTyp].LivTime <= 0 || retData.V.PosDict[(int)this._BtnTyp].LivTime >= 9)
			{
				if (this.nextType == this._BtnTyp)
				{
					base.StartCoroutine<bool>(this.StarChouJiangGoods(retData.V.PosInfo.Site, true, false, false));
				}
				else
				{
					base.StartCoroutine<bool>(this.StarChouJiangGoods(retData.V.PosInfo.Site, false, false, false));
				}
			}
			else if (this.JiHuoBool(retData.V.PosDict))
			{
				this.SetJiangPing(retData.V.PosInfo, true, false);
			}
			else
			{
				this.SetJiangPing(retData.V.PosInfo, false, false);
			}
		}
		this._DicData = retData.V.PosDict;
	}

	public void RefreshHuoBi()
	{
		this.labZuanShi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dfd7dd",
			Global.Data.roleData.UserMoney.ToString()
		});
		this.labXingYunZhiXing.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dfd7dd",
			Global.GetRoleOwnNumByMoneyType(163)
		});
	}

	private void AddGoddsJiangPing(Dictionary<int, StonePosInfo> infoDic)
	{
		if (infoDic.Count != 9)
		{
			return;
		}
		this._slYuanQuan.Clear();
		float num = 0f;
		Dictionary<int, StonePosInfo>.Enumerator enumerator = infoDic.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameObject gameObject = U3DUtils.Clone(this._slYuanQuan.gameObject, this._GameYuanQuan);
			Vector3 localPosition = default(Vector3);
			float num2 = num / 180f * 3.14159274f;
			localPosition.x = (float)this.posGoods * Mathf.Sin(num2);
			localPosition.y = (float)this.posGoods * Mathf.Cos(num2);
			localPosition.z = -1f;
			KeyValuePair<int, StonePosInfo> keyValuePair = enumerator.Current;
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(keyValuePair.Value.GoodsID);
			GoodsData goodsData = dummyGoodsData;
			KeyValuePair<int, StonePosInfo> keyValuePair2 = enumerator.Current;
			goodsData.GCount = keyValuePair2.Value.Num;
			GoodsData goodsData2 = dummyGoodsData;
			KeyValuePair<int, StonePosInfo> keyValuePair3 = enumerator.Current;
			goodsData2.Binding = keyValuePair3.Value.Bind;
			GGoodIcon ggoodIcon = Global.AddGoodsIcon(dummyGoodsData, this._slYuanQuan.gameObject, false, GoodsOwnerTypes.SysGifts);
			ggoodIcon.SetBackHide(false);
			ggoodIcon.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			num += (float)(360 / infoDic.Count);
			ggoodIcon.transform.localPosition = localPosition;
			localPosition.z += 0.3f;
			gameObject.transform.localPosition = localPosition;
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, -(num / 360f));
			KeyValuePair<int, StonePosInfo> keyValuePair4 = enumerator.Current;
			if (keyValuePair4.Value.State == 1)
			{
				ggoodIcon.GoodImg.Texture.shader = Shader.Find("Unlit/Gray");
			}
		}
	}

	private StoneType RfreshNextBtn(DreamStoneS2C<LuckyDrawRes> retData)
	{
		if (retData.V.PosDict[(int)this._BtnTyp].State == 0)
		{
			return this._BtnTyp;
		}
		if (retData.V.PosDict[3].State == 0)
		{
			return StoneType.Jin;
		}
		if (retData.V.PosDict[2].State == 0)
		{
			return StoneType.Yin;
		}
		return StoneType.Tong;
	}

	private bool JiHuoBool(Dictionary<int, StoneData> retData)
	{
		bool result = false;
		if (this._BtnTyp == StoneType.Tong)
		{
			if (retData[2].ActCount > this._DicData[2].ActCount)
			{
				result = true;
			}
		}
		else if (this._BtnTyp == StoneType.Yin && retData[3].ActCount > this._DicData[3].ActCount)
		{
			result = true;
		}
		return result;
	}

	private void SetJiangPing(StonePosInfo info, bool shiBan, bool GoodsFlag)
	{
		GGoodIcon[] componentsInChildren = this._slYuanQuan.GetComponentsInChildren<GGoodIcon>();
		if (componentsInChildren != null)
		{
			GoodsData goodsData = componentsInChildren[info.Site - 1].ItemObject as GoodsData;
			if (goodsData != null)
			{
				MUDebug.Log<string>(new string[]
				{
					"Site" + info.Site
				});
				base.StartCoroutine<bool>(this.StarChouJiangGoods(info.Site, false, shiBan, GoodsFlag));
			}
		}
	}

	private GameObject LoadTeXiao(string path, Transform parent)
	{
		return Global.LoadTeXiaoObj(path, parent);
	}

	private IEnumerator StartTween()
	{
		this._boolChouJiangZhong = true;
		this._panelZone.gameObject.SetActive(true);
		float duration = 0f;
		TweenScale[] scales = this._panelZone.GetComponentsInChildren<TweenScale>();
		if (scales != null)
		{
			for (int i = 0; i < scales.Length; i++)
			{
				scales[i].Reset();
				scales[i].Play(true);
				duration = Mathf.Max(duration, scales[i].duration + scales[i].delay);
			}
		}
		TweenPosition[] pos = this._panelZone.GetComponentsInChildren<TweenPosition>();
		if (pos != null)
		{
			for (int j = 0; j < pos.Length; j++)
			{
				pos[j].Reset();
				pos[j].Play(true);
				duration = Mathf.Max(duration, pos[j].duration + pos[j].delay);
			}
		}
		TweenAlpha alpha = this.spJiangPinZhen.GetComponent<TweenAlpha>();
		if (alpha != null)
		{
			alpha.duration = duration;
			alpha.Reset();
			alpha.Play(true);
		}
		yield return new WaitForSeconds(duration / 2f);
		this.spJiangPinZhen.transform.localEulerAngles = Vector3.zero;
		this._GameTeXiaoXuanZhong.gameObject.SetActive(false);
		this.AddGoddsJiangPing(this._Data.PosInfo);
		yield return new WaitForSeconds(duration / 2f);
		this._panelZone.gameObject.SetActive(false);
		this._boolChouJiangZhong = false;
		yield break;
	}

	private IEnumerator StarChouJiangZhuanQuan()
	{
		this._GameTeXiaoXuanZhong.gameObject.SetActive(false);
		this._boolChouJiangZhong = true;
		this.choujiangFlag = true;
		float chouJiangTime = 1f;
		float nextTime = 0.02f;
		Vector3 vecRation = this.spJiangPinZhen.transform.localEulerAngles;
		bool faSong = true;
		while (this.choujiangFlag)
		{
			chouJiangTime -= nextTime;
			vecRation.z -= this.chouJiangMaxcurr;
			if (vecRation.z < 0f)
			{
				vecRation.z += 360f;
			}
			this.spJiangPinZhen.transform.localEulerAngles = vecRation;
			yield return new WaitForSeconds(nextTime);
			if (chouJiangTime < 0f && faSong)
			{
				GameInstance.Game.SendMengHuanStoneChouJiangOne(this._DicData[(int)this._BtnTyp].Type);
				faSong = false;
			}
		}
		MUDebug.Log<string>(new string[]
		{
			chouJiangTime + string.Empty
		});
		yield break;
	}

	private IEnumerator StarChouJiangGoods(int site, bool zhuanPanteXiao, bool shiBanTeXiao, bool GoosTanChuang)
	{
		float jiangPingVec = (float)(360 - (site - 1) * 40);
		this.choujiangFlag = false;
		Vector3 vecRation = this.spJiangPinZhen.transform.localEulerAngles;
		MUDebug.Log(vecRation);
		float maxS = 0f;
		if (jiangPingVec < vecRation.z)
		{
			maxS = jiangPingVec + 360f - vecRation.z;
		}
		else
		{
			maxS = jiangPingVec - vecRation.z;
		}
		float maxCurr = this.chouJiangMaxcurr;
		if (maxS > 240f)
		{
			maxCurr = this.chouJiangMaxcurr * 2f;
		}
		float maxTime = 0.2f;
		float nextTime = 0.01f;
		float currPi = 0.05f;
		float curr = this.chouJiangMaxcurr;
		currPi = (jiangPingVec - curr * maxTime) * 2f / (maxTime * maxTime);
		currPi = -currPi / 10000f * 9f;
		while (maxTime >= 0.1f || Mathf.Abs(vecRation.z - jiangPingVec) > this.currZone)
		{
			maxTime -= nextTime;
			curr += currPi;
			if (curr <= this.currZone)
			{
				curr = this.currZone;
			}
			vecRation.z -= curr;
			if (vecRation.z < 0f)
			{
				vecRation.z += 360f;
			}
			this.spJiangPinZhen.transform.localEulerAngles = vecRation;
			yield return new WaitForSeconds(nextTime);
		}
		vecRation.z = jiangPingVec;
		this.spJiangPinZhen.transform.localEulerAngles = vecRation;
		this._GameTeXiaoShan.gameObject.SetActive(false);
		this._GameTeXiaoShan.gameObject.SetActive(true);
		this._GameTeXiaoXuanZhong.gameObject.SetActive(true);
		GGoodIcon[] icon = this._slYuanQuan.GetComponentsInChildren<GGoodIcon>();
		if (icon != null)
		{
			icon[site - 1].GoodImg.ToGrayBitmap = true;
		}
		yield return new WaitForSeconds(0.8f);
		if (shiBanTeXiao)
		{
			GameObject shanGuang = this.LoadTeXiao("UITeXiao/Perfabs/menghuanshiban/UI_MHSB_man", this._panelChouJiang.transform);
			shanGuang.transform.localPosition = Vector3.back;
			if (shanGuang != null)
			{
				shanGuang.AddComponent<DelayDestroy>().delayTime = 0.8f;
			}
			TweenScale cale = this._GameTeXiaoJiHuoFeiRu.GetComponent<TweenScale>();
			TweenPosition posi = this._GameTeXiaoJiHuoFeiRu.GetComponent<TweenPosition>();
			this._GameTeXiaoJiHuoFeiRu.transform.localPosition = posi.from;
			yield return new WaitForSeconds(0.8f);
			this._GameTeXiaoJiHuoFeiRu.gameObject.SetActive(true);
			float tweenTime = 0f;
			if (cale != null)
			{
				cale.Reset();
				cale.Play(true);
				tweenTime = Mathf.Max(tweenTime, cale.duration + cale.delay);
			}
			if (posi != null)
			{
				if (this._BtnTyp == StoneType.Tong)
				{
					posi.to = this.vecFeiRu[0];
				}
				else if (this._BtnTyp == StoneType.Yin)
				{
					posi.to = this.vecFeiRu[1];
				}
				posi.Reset();
				posi.Play(true);
				tweenTime = Mathf.Max(tweenTime, posi.duration + posi.delay);
			}
			yield return new WaitForSeconds(tweenTime + 0.1f);
			this._GameTeXiaoJiHuoFeiRu.gameObject.SetActive(false);
			this.OpenJiHuo();
		}
		if (GoosTanChuang)
		{
			this.OpenGoods();
		}
		this._boolChouJiangZhong = false;
		if (this.nextType == this._BtnTyp)
		{
			this.RefrshMain(this.nextType);
		}
		else
		{
			this.RefreshBtn(this.nextType);
		}
		if (zhuanPanteXiao)
		{
			this.AddGoddsJiangPing(this._Data.PosInfo);
			if (this.nextType == this._BtnTyp)
			{
				base.StartCoroutine<bool>(this.StartTween());
			}
		}
		this.RefreshBtnsNormal();
		yield break;
	}

	private void OpenJiHuo()
	{
		string chineseText = string.Empty;
		if (this._BtnTyp == StoneType.Tong)
		{
			chineseText = Global.GetLang("恭喜激活银石板");
		}
		else if (this._BtnTyp == StoneType.Yin)
		{
			chineseText = Global.GetLang("恭喜激活金石板");
		}
		this._panelJiHuo.gameObject.SetActive(true);
		this._labJiHuoTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("恭喜激活")
		});
		this._labJiHuoContent.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang(chineseText)
		});
	}

	private void OpenGoods()
	{
		this._panelGoods.gameObject.SetActive(true);
		this._labGoodsTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("恭喜获得")
		});
		if (this.listJiangPing == null)
		{
			return;
		}
		if (this.obserGoods == null)
		{
			this.obserGoods = this._listBoxGoods.ItemsSource;
		}
		this.obserGoods.Clear();
		for (int i = 0; i < this.listJiangPing.Count; i++)
		{
			GGoodIcon component = Global.AddGoodsIcon(this.listJiangPing[i], this._listBoxGoods.gameObject, false, GoodsOwnerTypes.SysGifts);
			this.obserGoods.AddNoUpdate(component);
		}
	}

	private void OpenHelpWindow(string path = "Config/HuanMengSlateIntro.xml")
	{
		if (this.m_helpWindow == null)
		{
			this.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			this.m_helpWindow.IsShowModal = true;
			this.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_helpWindow, Global.GetLang("NewCommonHelpWindow"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_helpWindow);
		}
		if (this.m_helpPart == null)
		{
			this.m_helpPart = U3DUtils.NEW<NewCommonHelpWindow>();
			this.m_helpPart.mChildTransform.localPosition = new Vector3(100f, 0f, 0f);
			this.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHelpWindow();
			};
		}
		this.m_helpWindow.SetContent(this.m_helpWindow.BodyPresenter, this.m_helpPart, 0.0, 0.0, true);
		ChangeableRulePart.RuleXml ruleXml = null;
		if (ruleXml == null)
		{
			XElement gameResXml = Global.GetGameResXml(path);
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					string.Format("加载{0}出现错误", path)
				});
			}
			ruleXml = new ChangeableRulePart.RuleXml(gameResXml);
		}
		this.m_helpPart.SetHelpInfo(ruleXml.list, false);
	}

	private void CloseHelpWindow()
	{
		if (null != this.m_helpPart)
		{
			this.m_helpPart.transform.parent = null;
			Object.Destroy(this.m_helpPart.gameObject);
			this.m_helpPart = null;
		}
		if (null != this.m_helpWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_helpWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_helpWindow, true);
			this.m_helpWindow = null;
		}
	}

	protected override void OnDestroy()
	{
		base.StopAllCoroutines();
	}

	public UISprite spJinTitle;

	public UISprite spYinTitle;

	public UISprite spTongTitle;

	public UISprite spJinDuQuan;

	public GameObject spJiangPinZhen;

	public UILabel labOne;

	public UILabel labTen;

	public UILabel labContent;

	public UILabel labTime;

	public UILabel labZuanShi;

	public UILabel labXingYunZhiXing;

	public UILabel labText;

	public GButton btnClose;

	public GButton BtnTong;

	public GButton BtnYin;

	public GButton BtnJin;

	public GButton btnOne;

	public GButton btnTen;

	public GButton btnOpenHelp;

	public UIPanel _panelChouJiang;

	public UIPanel _panelZone;

	public SpriteSL _slYuanQuan;

	public GameObject _GameYuanQuan;

	public UILabel _labJiHuoYinNum;

	public UILabel _labJiHuoJinNum;

	public GameObject _GameTeXiaoXuanZhong;

	public GameObject _GameTeXiaoShan;

	public GameObject _GameTeXiaoBtnSelect;

	public GameObject _GameTeXiaoJiHuoFeiRu;

	public UIPanel _panelJiHuo;

	public UILabel _labJiHuoTitle;

	public UILabel _labJiHuoContent;

	public GButton _btnJiHuoClose;

	public GButton _btnJiHuoConfirm;

	public UIPanel _panelGoods;

	public UILabel _labGoodsTitle;

	public ListBox _listBoxGoods;

	public GButton _btnGoodsClose;

	public GButton _btnGoodsConfirm;

	public DPSelectedItemEventHandler dpsClose;

	public int posGoods = 124;

	private StoneData _Data;

	private List<GButton> _ListBtn = new List<GButton>();

	private StoneType _BtnTyp;

	private Dictionary<int, StoneData> _DicData = new Dictionary<int, StoneData>();

	private List<GoodsData> listJiangPing = new List<GoodsData>();

	private MengHuanShiBanPart.ChouJiangType _ChoujiangType;

	private HuanMengSlateRewardPoolVO _xmlData = new HuanMengSlateRewardPoolVO();

	public bool _boolChouJiangZhong;

	private DateTime _endTime;

	private List<DreamStoneLogInfo> listLog = new List<DreamStoneLogInfo>();

	private StoneType nextType = StoneType.Tong;

	public Material material;

	private bool choujiangFlag = true;

	private float chouJiangMaxcurr = 40f;

	private float currZone = 20f;

	private Vector3[] vecFeiRu = new Vector3[]
	{
		new Vector3(-77f, 86f, -24f),
		new Vector3(-77f, 185f, -24f)
	};

	private ObservableCollection obserGoods;

	protected GChildWindow m_helpWindow;

	protected NewCommonHelpWindow m_helpPart;

	private enum ChouJiangType
	{
		none,
		one,
		more
	}
}
