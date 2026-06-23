using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class BuildTaskPart : UserControl
{
	public BuildState MBuildState
	{
		get
		{
			return this.mBuildState;
		}
		set
		{
			this.mBuildState = value;
			if (null != this.TaskIconView)
			{
				BoxCollider[] componentsInChildren = this.TaskIconView.transform.GetComponentsInChildren<BoxCollider>();
				if (componentsInChildren != null)
				{
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						if (null != componentsInChildren[i])
						{
							componentsInChildren[i].enabled = (this.MBuildState == BuildState.KongXian);
						}
					}
				}
			}
		}
	}

	private Vector3 GetNetImageSize(byte Index)
	{
		switch (Index)
		{
		case 0:
		case 1:
		case 2:
		case 3:
			return new Vector3(130f, 192f, 1f);
		case 4:
			return new Vector3(180f, 216f, 1f);
		case 5:
			return new Vector3(99f, 80f, 1f);
		case 6:
			return new Vector3(106f, 227f, 1f);
		default:
			return Vector3.zero;
		}
	}

	private string GetBuildAwardSignIconSpriteName(int index)
	{
		switch (index)
		{
		case 0:
			return "moJing";
		case 1:
			return "xingHun";
		case 2:
			return "chengJiu";
		case 3:
			return "shengWang";
		case 4:
			return string.Empty;
		case 5:
			return string.Empty;
		case 6:
			return string.Empty;
		default:
			return string.Empty;
		}
	}

	private string GetBuildTitleSionIocnSpriteName(int index)
	{
		switch (index)
		{
		case 0:
			return string.Empty;
		case 1:
			return "Build_ZhanShenTai";
		case 2:
			return "Build_MoFaZhiTa";
		case 3:
			return "Build_ZhanXingTai";
		case 4:
			return "Build_ShiLianZhiTa";
		case 5:
			return string.Empty;
		case 6:
			return string.Empty;
		default:
			return string.Empty;
		}
	}

	private string GetBuildTeXiaoPath(int index)
	{
		switch (index)
		{
		case 0:
			return "zhanshenta/ZhanShenTa_0{0}";
		case 1:
			return "mofata/MoFaZhiTa_0{0}";
		case 2:
			return "zhanxingta/ZhanXingTai_0{0}";
		case 3:
			return "shilianzhimen/ShiLianZhiMen_0{0}";
		case 4:
			return "shilianzhimen/ShiLianZhiMen_0{0}";
		case 5:
			return "shilianzhimen/ShiLianZhiMen_0{0}";
		case 6:
			return "shilianzhimen/ShiLianZhiMen_0{0}";
		default:
			return string.Empty;
		}
	}

	protected override void InitializeComponent()
	{
		this.DoTaskNeedGoodsID = 5300;
		base.InitializeComponent();
		this.InitText();
		this.InitHander();
		this.InitTexture();
	}

	private void InitText()
	{
		this.AwardLabel[0].text = string.Empty;
		this.AwardLabel[1].text = string.Empty;
		this.AwardLabel[2].text = string.Empty;
		this.AwardLabel[4].text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("完成奖励")
		});
		this.AwardLabel[3].text = string.Empty;
		this.BtnZhiXing.Label.text = Global.GetLang("启动");
		this.WorkLifeTimeLabel.text = string.Empty;
	}

	private void InitHander()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
		this.mItemCollection = this.TaskIconView.ItemsSource;
		this.TaskIconView.SelectionChanged = new MouseLeftButtonUpEventHandler(this.IconClickCallBask);
		this.BtnZhiXing.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.BtnZhiXing.Label.text == Global.GetLang("启动"))
			{
				if (this.MBuildState == BuildState.KongXian)
				{
					int num = 42 + this.mConfigbuildTaskXml.GetBuildTaskXmlDataList(this.mBuildID).FindIndex((BuildtaskInF g) => g.TaskID == this.mSelectTaskID);
					if (0 < Global.GetRoleCommonUseParamsValue(num))
					{
						Super.ShowNetWaiting(null);
						GameInstance.Game.BuildSendZhiXing(this.mBuildID, this.mSelectTaskID);
					}
					else
					{
						Super.HintMainText(Global.GetGoodsNameByID(this.DoTaskNeedGoodsID + num - 42, false) + Global.GetLang("不足"), 10, 3);
					}
				}
				else if (this.MBuildState == BuildState.GongZuo)
				{
					Super.HintMainText(Global.GetLang("任务启动中"), 10, 3);
				}
				else if (this.MBuildState == BuildState.WanCheng)
				{
					Super.HintMainText(Global.GetLang("请先领取奖励"), 10, 3);
				}
			}
			else if (this.MBuildState == BuildState.WanCheng)
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendGetBuildAward(this.mBuildID);
			}
			else if (this.MBuildState == BuildState.GongZuo)
			{
				Super.HintMainText(Global.GetLang("任务未完成"), 10, 3);
			}
			else if (this.MBuildState == BuildState.KongXian)
			{
				Super.HintMainText(Global.GetLang("找不到对应的建筑任务"), 10, 3);
			}
		};
	}

	private void InitTexture()
	{
		this.BakImage.URL = "NetImages/GameRes/Images/Build/BuildTask/BuildTaskBak0.png";
		byte b = 0;
		while ((int)b < this.AwardBgSp.Length)
		{
			this.AwardBgSp[(int)b].alpha = 0.5f;
			b += 1;
		}
	}

	public void InitXml()
	{
		this.mConfigBuildXml = new ConfigBuildXml();
		this.mConfigbuildTaskXml = new BuildTaskPart.ConfigbuildTaskXml(this.mBuildID);
		this.xmlBuildTask = Global.GetGameResXml("Config/Manor/BuildTask.xml");
	}

	public void InitTask(BuildData data, int DoingTaskID, string SignURL, int BuildMaxLev)
	{
		this.mBuildData = data;
		this.MBuildID = data.BuildID;
		this.mDoingTaskID = DoingTaskID;
		BuildtaskInF buildTaskXmlData = this.mConfigbuildTaskXml.GetBuildTaskXmlData(this.mBuildID, DoingTaskID);
		if (buildTaskXmlData != null)
		{
			this.BuildTaskNeedTime_S = (int)(buildTaskXmlData.TaskTime * 60U);
		}
		this.BuildTextureURL = SignURL;
		this.TitleSprite.spriteName = StringUtil.substitute("{0}", new object[]
		{
			this.GetBuildTitleSionIocnSpriteName(this.mBuildID)
		});
		XElement gameResXml = Global.GetGameResXml("Config/Manor/BuildLevel.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "BuildTask");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (Global.GetXElementAttributeInt(xelement, "BuildID") == this.mBuildID && Global.GetXElementAttributeInt(xelement, "Level") == data.Lev)
				{
					this.Build_Award.AwardExp = (uint)Global.GetXElementAttributeInt(xelement, "Exp");
					this.Build_Award.AwardMoney = (uint)Global.GetXElementAttributeInt(xelement, "Money");
					this.Build_Award.AwardMoJing = Global.GetXElementAttributeFloat(xelement, "MoJing");
					this.Build_Award.AwardXingHun = Global.GetXElementAttributeFloat(xelement, "XingHun");
					this.Build_Award.AwardChengJiu = Global.GetXElementAttributeFloat(xelement, "ChengJiu");
					this.Build_Award.AwardShengWang = Global.GetXElementAttributeFloat(xelement, "ShengWang");
					this.Build_Award.AwardYuanSu = Global.GetXElementAttributeFloat(xelement, "YuanSu");
					this.Build_Award.AwardYingGuang = (uint)Global.GetXElementAttributeInt(xelement, "YingGuang");
					this.Build_Award.LevelNeedEXP = (uint)Global.GetXElementAttributeInt(xelement, "UpNeedExp");
					this.Build_Award.Lev = Global.GetXElementAttributeInt(xelement, "Level");
					break;
				}
			}
		}
		this.LevelLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("Lv  ") + data.Lev.ToString()
		});
		this.LevelLabel.supportEncoding = true;
		float num = (float)data.Exp * 1f / this.Build_Award.LevelNeedEXP * 1f;
		bool flag = data.Lev >= BuildMaxLev;
		if (flag)
		{
			NGUITools.SetActive(this.ExpLabel.gameObject, false);
			this.SetNextLevelAward(Global.GetLang("建筑满级，不再获得经验"));
			this.ExpLabel.text = Global.GetLang("建筑满级，不再获得经验");
			this.EXPBar.sliderValue = 0.99f;
		}
		else
		{
			this.SetNextLevelAward(string.Format(Global.GetLang("LV{0}建筑造型提升至{1}阶"), this.mConfigBuildXml.GetBuildLevelArrayByBuildID(this.mBuildID)[this.GetNextOrder(data.Lev)], this.GetNextOrder_Order(data.Lev)));
			this.ExpLabel.text = Global.GetLang(data.Exp.ToString() + "/" + this.Build_Award.LevelNeedEXP.ToString());
			if (1f >= num && 0f <= num)
			{
				this.EXPBar.sliderValue = num;
			}
		}
		int nextLevNeedExp = (int)(this.Build_Award.LevelNeedEXP - (uint)data.Exp);
		base.StartCoroutine<bool>(this.InitTaskItem(this.mBuildID, data.Lev, nextLevNeedExp, BuildMaxLev));
	}

	private GGoodIcon GetGoodIcon(GoodsData data)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			ggoodIcon.ItemObject = data;
			ggoodIcon.ItemCode = goodsXmlNodeByID.ID;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			Super.InitGoodsGIcon(ggoodIcon, data, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
		}
		return ggoodIcon;
	}

	private string GetNengLiangGoodsName(int Index)
	{
		switch (Index)
		{
		case 0:
			return Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("小型能量核心")
			});
		case 1:
			return Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("中型能量核心")
			});
		case 2:
			return Global.GetColorStringForNGUIText(new object[]
			{
				"3681f3",
				Global.GetLang("大型能量核心")
			});
		case 3:
			return Global.GetColorStringForNGUIText(new object[]
			{
				"b266ff",
				Global.GetLang("超级能量核心")
			});
		default:
			return string.Empty;
		}
	}

	private GameObject LoadTeXiao()
	{
		this.mConfigBuildXml.GetBuildPicArrayByBuildID(this.mBuildID);
		int[] buildLevelArrayByBuildID = this.mConfigBuildXml.GetBuildLevelArrayByBuildID(this.mBuildID);
		string[] buildPicArrayByBuildID = this.mConfigBuildXml.GetBuildPicArrayByBuildID(this.mBuildID);
		int num = 0;
		if (buildLevelArrayByBuildID.Length == 1)
		{
			num = 0;
		}
		else if ((buildLevelArrayByBuildID[0] <= this.mBuildData.Lev && buildLevelArrayByBuildID[1] > this.mBuildData.Lev) || this.mBuildData.Lev == 0)
		{
			num = 0;
		}
		else if (buildLevelArrayByBuildID[1] <= this.mBuildData.Lev && buildLevelArrayByBuildID[2] > this.mBuildData.Lev)
		{
			num = 1;
		}
		else if (buildLevelArrayByBuildID[2] <= this.mBuildData.Lev && buildLevelArrayByBuildID[3] > this.mBuildData.Lev)
		{
			num = 2;
		}
		else if (buildLevelArrayByBuildID[3] <= this.mBuildData.Lev)
		{
			num = 3;
		}
		string text = "UITeXiao/Perfabs/lingdi/" + string.Format(this.GetBuildTeXiaoPath(BuildMainPart.BuildIDToArrayId(this.mBuildID)), num + 1);
		Object @object = Resources.Load(text);
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			if (0 < this.BuildNetImageRoot.childCount)
			{
				Transform child = this.BuildNetImageRoot.GetChild(0);
				if (null != child)
				{
					Object.Destroy(child.gameObject);
				}
			}
			gameObject.transform.SetParent(this.BuildNetImageRoot, false);
			this.BuildNetImageRoot.localScale = new Vector3(1f, 1f, 1f);
			gameObject.layer = LayerMask.NameToLayer("MUUI");
			return gameObject;
		}
		return null;
	}

	private IEnumerator InitTaskItem(int Title, int BuildLevel, int NextLevNeedExp, int Maxlev)
	{
		List<BuildtaskInF> TaskList = this.mConfigbuildTaskXml.GetBuildTaskXmlDataList(this.mBuildID);
		int count = TaskList.Count;
		if (0 < count)
		{
			byte i = 0;
			while ((int)i < count)
			{
				BuildtaskInF dataItem = TaskList[(int)i];
				if (dataItem != null)
				{
					GoodsData gd = null;
					GoodVO vo = ConfigGoods.GetGoodsXmlNodeByID(this.DoTaskNeedGoodsID + (int)i);
					if (vo != null)
					{
						gd = Global.GetEmptyGoodsData(vo.ID, 0, vo.ItemQuality, 0, 1, 0, 0, 0, 0);
					}
					else
					{
						gd = Global.GetEmptyGoodsData(this.DoTaskNeedGoodsID + (int)i, 0, (int)(4 - i), 0, 1, 0, 0, 0, 0);
					}
					GGoodIcon item = null;
					GameObject itemObj = this.mItemCollection.GetAt((int)i);
					if (null != itemObj)
					{
						item = itemObj.GetComponent<GGoodIcon>();
						if (null == item)
						{
							Object.DestroyImmediate(item.gameObject);
							item = null;
						}
					}
					if (null == item)
					{
						item = this.GetGoodIcon(gd);
						this.mItemCollection.Add(item);
					}
					item.transform.name = i.ToString();
					int value = Global.GetRoleCommonUseParamsValue((int)(42 + i));
					item.SecondText.Label.supportEncoding = true;
					this.InitGoodIcon(item, (int)i);
					if (0 >= value)
					{
						item.SecondText.Text = Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							value
						});
					}
					else
					{
						item.SecondText.Text = Global.GetColorStringForNGUIText(new object[]
						{
							"ffffff",
							value
						});
					}
					item.ContentText.Label.supportEncoding = true;
					item.ContentText.Label.pivot = 4;
					item.ContentText.Text = Global.GetGoodsNameByID(gd.GoodsID, true);
					item.ContentText.transform.localScale = Vector3.one * 18f;
					item.BackgroundSprite1.transform.localPosition -= new Vector3(0f, 0f, 0.05f);
					Vector3 pos = item.ContentText.transform.localPosition;
					pos.y = -55f;
					pos.x = 0f;
					item.ContentText.transform.localPosition = pos;
				}
				yield return null;
				i += 1;
			}
		}
		yield return new WaitForEndOfFrame();
		this.DevelopTime = this.mBuildData.DevelopTime;
		if (0 < this.mDoingTaskID)
		{
			if (this.mConfigbuildTaskXml.IsOldTask(this.mBuildID, this.mDoingTaskID))
			{
				this.TaskIconView.SelectedIndex = -1;
				NGUITools.SetActive(this.SeleceEffectTrans.gameObject, false);
				this.RefreshTaskInf(this.mDoingTaskID);
			}
			else
			{
				int index = TaskList.FindIndex((BuildtaskInF e) => e.TaskID == this.mDoingTaskID);
				this.TaskIconView.SelectedIndex = index;
			}
		}
		else
		{
			List<TaskRankInF> RoleParamsValueList = new List<TaskRankInF>();
			for (byte j = 0; j < 4; j += 1)
			{
				RoleParamsValueList.Add(new TaskRankInF
				{
					priority = (int)j,
					RoleParamValue = Global.GetRoleCommonUseParamsValue((int)(42 + j))
				});
			}
			RoleParamsValueList.Sort((TaskRankInF s, TaskRankInF e) => e.priority - s.priority);
			int index2 = RoleParamsValueList.FindIndex((TaskRankInF e) => 0 < e.RoleParamValue);
			this.TaskIconView.SelectedIndex = RoleParamsValueList[(0 >= index2) ? 0 : index2].priority;
		}
		this.RefreshBtnState();
		yield break;
	}

	private void InitGoodIcon(GGoodIcon icon, int index)
	{
		if (index == 0)
		{
			icon.TeXiao.gameObject.SetActive(false);
		}
		else if (index == 1)
		{
			icon.TeXiao.gameObject.SetActive(false);
		}
		else if (index == 2)
		{
			icon.TeXiao.gameObject.SetActive(false);
		}
		else if (index == 3 && icon.TeXiao._Sprite != null)
		{
			icon.TeXiao._Sprite.atlas = Super.GetZhuoYueFlowLightAtlas();
			icon.TeXiao._Sprite.transform.localScale = new Vector3((float)icon.Width, (float)icon.Height);
			icon.TeXiao.gameObject.SetActive(true);
		}
	}

	private void IconClickCallBask(object sender, MouseEvent e)
	{
		int num = 0;
		GameObject gameObject = this.TaskIconView.SelectedItem;
		if (null != gameObject)
		{
			num = gameObject.ToString().SafeToInt32(0);
			if (null != gameObject)
			{
				Vector3 localPosition;
				localPosition..ctor(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, this.SeleceEffectTrans.localPosition.z);
				this.SeleceEffectTrans.localPosition = localPosition;
				NGUITools.SetActive(this.SeleceEffectTrans.gameObject, true);
			}
		}
		else
		{
			gameObject = this.mItemCollection.GetAt(num);
			if (null != gameObject)
			{
				Vector3 localPosition2;
				localPosition2..ctor(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, this.SeleceEffectTrans.localPosition.z);
				this.SeleceEffectTrans.localPosition = localPosition2;
				NGUITools.SetActive(this.SeleceEffectTrans.gameObject, true);
			}
		}
		this.RefreshTaskInf(num);
	}

	private void RefreshTaskInf(int id)
	{
		Transform transform = null;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.DoTaskNeedGoodsID + id);
		GoodsData emptyGoodsData;
		if (goodsXmlNodeByID != null)
		{
			emptyGoodsData = Global.GetEmptyGoodsData(goodsXmlNodeByID.ID, 0, goodsXmlNodeByID.ItemQuality, 0, 1, 0, 0, 0, 0);
		}
		else
		{
			emptyGoodsData = Global.GetEmptyGoodsData(this.DoTaskNeedGoodsID + id, 0, 4 - id, 0, 1, 0, 0, 0, 0);
		}
		if (0 < this.GoodIconRootTrans.childCount)
		{
			transform = this.GoodIconRootTrans.GetChild(0);
		}
		GGoodIcon ggoodIcon;
		if (null != transform)
		{
			ggoodIcon = transform.GetComponent<GGoodIcon>();
			if (null == ggoodIcon)
			{
				Object.Destroy(transform.gameObject);
			}
		}
		else
		{
			ggoodIcon = this.GetGoodIcon(emptyGoodsData);
		}
		ggoodIcon.transform.SetParent(this.GoodIconRootTrans, false);
		int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(42 + id);
		ggoodIcon.SecondText.Label.supportEncoding = true;
		if (0 >= roleCommonUseParamsValue)
		{
			ggoodIcon.SecondText.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				roleCommonUseParamsValue
			});
		}
		else
		{
			ggoodIcon.SecondText.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"ffffff",
				roleCommonUseParamsValue
			});
		}
		this.InitGoodIcon(ggoodIcon, id);
		ggoodIcon.BackSpriteName1 = string.Empty;
		Super.InitGoodsGIcon(ggoodIcon, emptyGoodsData, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
		ggoodIcon.ContentText.Label.supportEncoding = true;
		ggoodIcon.ContentText.Label.pivot = 4;
		ggoodIcon.ContentText.Text = Global.GetGoodsNameByID(emptyGoodsData.GoodsID, true);
		ggoodIcon.ContentText.transform.localScale = Vector3.one * 20f;
		Vector3 localPosition = ggoodIcon.ContentText.transform.localPosition;
		localPosition.y = 63f;
		localPosition.x = 0f;
		ggoodIcon.ContentText.transform.localPosition = localPosition;
		BuildtaskInF buildTaskXmlDataByIndex = this.mConfigbuildTaskXml.GetBuildTaskXmlDataByIndex(this.mBuildID, id);
		this.AwardLabel[3].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			this.GetTimeStr(buildTaskXmlDataByIndex.TaskTime)
		});
		BuildAwardInF awardNum = this.GetAwardNum(buildTaskXmlDataByIndex);
		this.AwardSp[0].spriteName = "BuildTaskExp";
		this.AwardLabel[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			(float)awardNum.Exp * buildTaskXmlDataByIndex.ExpNum * buildTaskXmlDataByIndex.TaskTime
		});
		this.AwardSp[1].spriteName = "TaskGold";
		this.AwardLabel[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			(float)awardNum.Money * (buildTaskXmlDataByIndex.Sum - buildTaskXmlDataByIndex.ExpNum) * buildTaskXmlDataByIndex.TaskTime
		});
		this.AwardSp[2].spriteName = this.GetBuildAwardSignIconSpriteName(awardNum.AwardType);
		this.AwardLabel[2].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			awardNum.BuildAward_A * (buildTaskXmlDataByIndex.Sum - buildTaskXmlDataByIndex.ExpNum) * buildTaskXmlDataByIndex.TaskTime
		});
		this.mSelectTaskID = buildTaskXmlDataByIndex.TaskID;
	}

	private string TimeToString(int second)
	{
		int num = second % 60;
		int num2 = (second - num) / 3600;
		int number = (second - num) / 60 - num2 * 60;
		return string.Concat(new string[]
		{
			this.GetTwoOrderNumber(num2),
			":",
			this.GetTwoOrderNumber(number),
			":",
			this.GetTwoOrderNumber(num)
		});
	}

	private string GetTwoOrderNumber(int number)
	{
		if (10 > number)
		{
			return "0" + number.ToString();
		}
		return number.ToString();
	}

	private string GetTimeStr(uint time)
	{
		int num = this.MyDoubleToInt(time / 60.0 / 24.0);
		int num2 = this.MyDoubleToInt(time / 60.0);
		int num3 = (int)(time % 60U);
		if (num > 0)
		{
			return string.Format("{0}{1}{2}", num.ToString() + Global.GetLang("天"), (num2 <= 0) ? string.Empty : (num2.ToString() + Global.GetLang("小时")), (num3 <= 0) ? string.Empty : (num3.ToString() + Global.GetLang("分钟")));
		}
		if (num2 > 0)
		{
			return string.Format("{0}{1}", num2.ToString() + Global.GetLang("小时"), (num3 <= 0) ? string.Empty : (num3.ToString() + Global.GetLang("分钟")));
		}
		return string.Format("{0}", (num3 <= 0) ? string.Empty : (num3.ToString() + Global.GetLang("分钟")));
	}

	private int MyDoubleToInt(double value)
	{
		string text = value.ToString();
		string[] array = text.Split(new char[]
		{
			'.'
		});
		if (array.Length < 1)
		{
			return Mathf.FloorToInt((float)value);
		}
		return int.Parse(array[0]);
	}

	private void DestoryShelf()
	{
		if (null != this)
		{
			GameObject gameObject = base.gameObject.transform.parent.parent.gameObject;
			Object.Destroy(this);
			if (null != gameObject)
			{
				Object.Destroy(gameObject);
			}
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		this.mConfigbuildTaskXml = null;
		this.TaskId.Clear();
	}

	private int GetTaskID(int i)
	{
		if (i < this.TaskId.Count)
		{
			return this.TaskId[i];
		}
		return 0;
	}

	public void SetNextLevelAward(string Content)
	{
		this.BuileLevelContent.text = "{e3b36c}" + Content + "{-}";
	}

	private BuildAwardInF GetAwardNum(BuildtaskInF TaskitemInf)
	{
		BuildAwardInF result = default(BuildAwardInF);
		result.Exp = (int)this.Build_Award.AwardExp;
		result.Money = (int)this.Build_Award.AwardMoney;
		if (this.Build_Award.AwardChengJiu != 0f)
		{
			result.BuildAward_A = this.Build_Award.AwardChengJiu;
			result.AwardType = 2;
		}
		else if (this.Build_Award.AwardMoJing != 0f)
		{
			result.BuildAward_A = this.Build_Award.AwardMoJing;
			result.AwardType = 0;
		}
		else if (this.Build_Award.AwardXingHun != 0f)
		{
			result.BuildAward_A = this.Build_Award.AwardXingHun;
			result.AwardType = 1;
		}
		else if (this.Build_Award.AwardShengWang != 0f)
		{
			result.BuildAward_A = this.Build_Award.AwardShengWang;
			result.AwardType = 3;
		}
		else if (this.Build_Award.AwardYuanSu != 0f)
		{
			result.BuildAward_A = this.Build_Award.AwardYuanSu;
			result.AwardType = 4;
		}
		else if (this.Build_Award.AwardYingGuang != 0U)
		{
			result.BuildAward_A = this.Build_Award.AwardYingGuang;
			result.AwardType = 5;
		}
		return result;
	}

	public override void Update()
	{
		base.Update();
		if (this.m_HaveClickShuaxinBtn)
		{
			this.m_ShuaXinbtnCd += Time.deltaTime;
			if (1f <= this.m_ShuaXinbtnCd)
			{
				this.m_ShuaXinbtnCd = 0f;
				this.m_HaveClickShuaxinBtn = false;
			}
		}
		if (this.MBuildState == BuildState.GongZuo)
		{
			this.BuildRemainingTime_S += Time.deltaTime;
			if (this.BuildRemainingTime_S >= (float)this.BuildTaskNeedTime_S)
			{
				this.MBuildState = BuildState.WanCheng;
				NGUITools.SetActive(this.BtnZhiXing.gameObject, true);
				this.BtnZhiXing.normalSprite = "ButGetAward0 ";
			}
			this.RefreshBtnState();
		}
	}

	private void RefreshBtnState()
	{
		if (this.MBuildState == BuildState.GongZuo)
		{
			NGUITools.SetActive(this.BtnZhiXing.gameObject, false);
			this.WorkLifeTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				this.TimeToString(this.BuildTaskNeedTime_S - (int)this.BuildRemainingTime_S) + Global.GetLang("后完成")
			});
		}
		else if (this.MBuildState == BuildState.WanCheng)
		{
			NGUITools.SetActive(this.BtnZhiXing.gameObject, true);
			this.BtnZhiXing.normalSprite = "ButGetAward0";
			this.BtnZhiXing.hoverSprite = "ButGetAward1";
			this.BtnZhiXing.pressedSprite = "ButGetAward1";
			this.BtnZhiXing.disabledSprite = "ButGetAward1";
			this.BtnZhiXing.Refresh();
			this.BtnZhiXing.Label.text = Global.GetLang("领取");
			this.WorkLifeTimeLabel.text = string.Empty;
		}
		else
		{
			this.BtnZhiXing.Label.text = Global.GetLang("启动");
			NGUITools.SetActive(this.BtnZhiXing.gameObject, true);
			this.BtnZhiXing.normalSprite = "TaskShuaxiaBtn0";
			this.BtnZhiXing.hoverSprite = "TaskShuaxiaBtn1";
			this.BtnZhiXing.pressedSprite = "TaskShuaxiaBtn1";
			this.BtnZhiXing.disabledSprite = "TaskShuaxiaBtn1";
			this.BtnZhiXing.Refresh();
		}
	}

	private void MyDataToString(int[] inputData, out string outData)
	{
		outData = string.Empty;
		for (int i = 0; i < inputData.Length; i++)
		{
			outData = outData + inputData[i].ToString() + "|";
		}
	}

	private int GetNextOrder(int Lev)
	{
		int[] buildLevelArrayByBuildID = this.mConfigBuildXml.GetBuildLevelArrayByBuildID(this.mBuildID);
		if ((buildLevelArrayByBuildID[0] <= Lev && buildLevelArrayByBuildID[1] > Lev) || Lev == 0)
		{
			return 1;
		}
		if (buildLevelArrayByBuildID[1] <= Lev && buildLevelArrayByBuildID[2] > Lev)
		{
			return 2;
		}
		return 3;
	}

	private int GetNextOrder_Order(int Lev)
	{
		int[] buildLevelArrayByBuildID = this.mConfigBuildXml.GetBuildLevelArrayByBuildID(this.mBuildID);
		if ((buildLevelArrayByBuildID[0] <= Lev && buildLevelArrayByBuildID[1] > Lev) || Lev == 0)
		{
			return 2;
		}
		if (buildLevelArrayByBuildID[1] <= Lev && buildLevelArrayByBuildID[2] > Lev)
		{
			return 3;
		}
		return 4;
	}

	private int[] StringToDateTime(string timeStr)
	{
		int[] array = new int[6];
		string text = string.Empty;
		byte b = 0;
		byte b2 = 0;
		while ((int)b2 <= timeStr.Length)
		{
			if (timeStr.Length == (int)b2)
			{
				int.TryParse(text, ref array[(int)b]);
			}
			else if (timeStr.get_Chars((int)b2).ToString() != " " && timeStr.get_Chars((int)b2).ToString() != "-" && timeStr.get_Chars((int)b2).ToString() != ":")
			{
				text += timeStr.get_Chars((int)b2).ToString();
			}
			else
			{
				string text2 = text;
				int[] array2 = array;
				byte b3 = b;
				b = b3 + 1;
				int.TryParse(text2, ref array2[(int)b3]);
				text = string.Empty;
			}
			b2 += 1;
		}
		return array;
	}

	private long GetReRemainingTime()
	{
		if (this.mBuildDevelopTime != null && this.mBuildDevelopTime.mHaveTime)
		{
			DateTime correctDateTime = TimeManager.GetCorrectDateTime();
			int num = correctDateTime.Hour - this.mBuildDevelopTime.mTimeArray[3];
			int num2 = correctDateTime.Minute - this.mBuildDevelopTime.mTimeArray[4];
			int num3 = correctDateTime.Second - this.mBuildDevelopTime.mTimeArray[5];
			if (0 > num)
			{
				num += 24;
			}
			return (long)(num * 60 * 60 + num2 * 60 + num3);
		}
		return 0L;
	}

	public string DevelopTime
	{
		get
		{
			return this.mBuildData.DevelopTime;
		}
		set
		{
			this.mBuildData.DevelopTime = value;
			DateTime correctDateTime = TimeManager.GetCorrectDateTime();
			int[] array = this.StringToDateTime(this.mBuildData.DevelopTime);
			if ("0000-00-00 00:00:00" != this.mBuildData.DevelopTime)
			{
				this.mBuildDevelopTime.mHaveTime = true;
				byte b = 0;
				while ((int)b < array.Length)
				{
					this.mBuildDevelopTime.mTimeArray[(int)b] = array[(int)b];
					b += 1;
				}
			}
			else
			{
				this.mBuildDevelopTime.mHaveTime = false;
			}
			if (1 <= correctDateTime.Year - array[0] || 1 <= correctDateTime.Month - array[1] || 1 < correctDateTime.Day - array[2])
			{
				int num = array[0];
				DateTime minValue = DateTime.MinValue;
				if (num <= minValue.Year)
				{
					this.MBuildState = BuildState.KongXian;
				}
				else
				{
					this.MBuildState = BuildState.WanCheng;
				}
			}
			else
			{
				this.BuildRemainingTime_S = (float)this.GetReRemainingTime();
				if (0f <= this.BuildRemainingTime_S - (float)this.BuildTaskNeedTime_S && this.BuildTaskNeedTime_S != 0)
				{
					this.MBuildState = BuildState.WanCheng;
				}
				else if (0f > this.BuildRemainingTime_S - (float)this.BuildTaskNeedTime_S && this.BuildTaskNeedTime_S != 0)
				{
					this.MBuildState = BuildState.GongZuo;
				}
				else
				{
					this.MBuildState = BuildState.KongXian;
				}
			}
		}
	}

	public BuildTaskPart.BuildTaskZhiXingDate ZhingXingData
	{
		get
		{
			return this.m_ZhingXingData;
		}
	}

	public int MBuildID
	{
		get
		{
			return this.mBuildID;
		}
		set
		{
			this.mBuildID = value;
		}
	}

	public string BuildTextureURL
	{
		set
		{
			GameObject gameObject = this.LoadTeXiao();
			if (null != gameObject)
			{
				ShowNetImage TempBgShowNetImage = gameObject.GetComponentInChildren<ShowNetImage>();
				if (null != TempBgShowNetImage)
				{
					TempBgShowNetImage.URL = value;
					TempBgShowNetImage.ImageDownloaded = delegate(object o)
					{
						if (null != TempBgShowNetImage.Texture && null != TempBgShowNetImage.Texture.mainTexture)
						{
							TempBgShowNetImage.transform.localScale = new Vector3((float)TempBgShowNetImage.ItsSizeWidth, (float)TempBgShowNetImage.ItsSizeHeight, 1f);
						}
					};
				}
			}
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private Transform BuildNetImageRoot;

	[SerializeField]
	private ShowNetImage BakImage;

	[SerializeField]
	private GButton BtnClose;

	[SerializeField]
	private UISprite TitleSprite;

	[SerializeField]
	private UILabel LevelLabel;

	[SerializeField]
	private GImgProgressBar EXPBar;

	[SerializeField]
	private UILabel ExpLabel;

	[SerializeField]
	private UILabel BuileLevelContent;

	[SerializeField]
	private Transform GoodIconRootTrans;

	[SerializeField]
	private ListBox TaskIconView;

	private ObservableCollection mItemCollection;

	[SerializeField]
	private Transform SeleceEffectTrans;

	[SerializeField]
	private UISprite[] AwardSp;

	[SerializeField]
	private UISprite[] AwardBgSp;

	[SerializeField]
	private UILabel[] AwardLabel;

	[SerializeField]
	private GButton BtnZhiXing;

	[SerializeField]
	private UILabel WorkLifeTimeLabel;

	private int mBuildID;

	private ConfigBuildXml mConfigBuildXml;

	private List<int> mHaveTaskBuildID = new List<int>();

	private List<int> TaskId = new List<int>();

	private List<BuildTaskitem> TaskItemArray = new List<BuildTaskitem>();

	private BuildAward Build_Award = new BuildAward();

	private BuildTaskPart.BuildTaskZhiXingDate m_ZhingXingData = new BuildTaskPart.BuildTaskZhiXingDate();

	private bool m_HaveClickShuaxinBtn;

	private float m_ShuaXinbtnCd;

	private BuildTaskPart.ConfigbuildTaskXml mConfigbuildTaskXml;

	private int mSelectTaskID;

	private BuildData mBuildData;

	private BuildTimeData mBuildDevelopTime = new BuildTimeData();

	private BuildState mBuildState;

	private float BuildRemainingTime_S;

	private int BuildTaskNeedTime_S;

	private XElement xmlBuildTask;

	private int mDoingTaskID;

	private int DoTaskNeedGoodsID;

	public class BuildTaskZhiXingDate
	{
		public BuildTaskZhiXingDate()
		{
			this.m_HaveZhiXingData = false;
			this.m_TaskId = -1;
			this.buildId = -1;
		}

		public bool m_HaveZhiXingData;

		public int m_TaskId;

		public int buildId;
	}

	private class ConfigbuildTaskXml
	{
		public ConfigbuildTaskXml(int BuildID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/Manor/BuildTask.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "BuildTask");
				int num = xelementList.Count;
				if (16 < num)
				{
					num = 16;
				}
				for (int i = 0; i < num; i++)
				{
					XElement xelement = xelementList[i];
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "BuildID");
					if (BuildID == xelementAttributeInt)
					{
						BuildtaskInF buildtaskInF = new BuildtaskInF();
						buildtaskInF.BuildID = xelementAttributeInt;
						buildtaskInF.TaskID = Global.GetXElementAttributeInt(xelement, "ID");
						buildtaskInF.TaskName = Global.GetXElementAttributeStr(xelement, "Name");
						buildtaskInF.Quality = Global.GetXElementAttributeFloat(xelement, "Quality");
						buildtaskInF.Sum = Global.GetXElementAttributeFloat(xelement, "Sum");
						buildtaskInF.ExpNum = Global.GetXElementAttributeFloat(xelement, "ExpNum");
						buildtaskInF.TaskTime = (uint)Global.GetXElementAttributeInt(xelement, "Time");
						if (this.TaskInF.ContainsKey(buildtaskInF.BuildID))
						{
							this.TaskInF[buildtaskInF.BuildID].Add(buildtaskInF);
						}
						else
						{
							List<BuildtaskInF> list = new List<BuildtaskInF>();
							list.Add(buildtaskInF);
							this.TaskInF.Add(buildtaskInF.BuildID, list);
						}
					}
					else if (!this.TaskInF.ContainsKey(xelementAttributeInt))
					{
						this.TaskInF.Add(xelementAttributeInt, null);
					}
				}
			}
		}

		public BuildtaskInF GetBuildTaskXmlDataByIndex(int BuildID, int index)
		{
			List<BuildtaskInF> buildTaskXmlDataList = this.GetBuildTaskXmlDataList(BuildID);
			if (buildTaskXmlDataList != null && index < buildTaskXmlDataList.Count)
			{
				return buildTaskXmlDataList[index];
			}
			return null;
		}

		public BuildtaskInF GetBuildTaskXmlData(int BuildID, int TaskID)
		{
			List<BuildtaskInF> buildTaskXmlDataList = this.GetBuildTaskXmlDataList(BuildID);
			if (buildTaskXmlDataList != null)
			{
				for (int i = 0; i < buildTaskXmlDataList.Count; i++)
				{
					if (TaskID == buildTaskXmlDataList[i].TaskID)
					{
						return buildTaskXmlDataList[i];
					}
				}
			}
			XElement gameResXml = Global.GetGameResXml("Config/Manor/BuildTask.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "BuildTask");
				for (int j = 0; j < xelementList.Count; j++)
				{
					XElement xelement = xelementList[j];
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "BuildID");
					if (BuildID == xelementAttributeInt && TaskID == Global.GetXElementAttributeInt(xelement, "ID"))
					{
						return new BuildtaskInF
						{
							BuildID = xelementAttributeInt,
							TaskID = Global.GetXElementAttributeInt(xelement, "ID"),
							TaskName = Global.GetXElementAttributeStr(xelement, "Name"),
							Quality = Global.GetXElementAttributeFloat(xelement, "Quality"),
							Sum = Global.GetXElementAttributeFloat(xelement, "Sum"),
							ExpNum = Global.GetXElementAttributeFloat(xelement, "ExpNum"),
							TaskTime = (uint)Global.GetXElementAttributeInt(xelement, "Time")
						};
					}
				}
			}
			return null;
		}

		public List<BuildtaskInF> GetBuildTaskXmlDataList(int BuildID)
		{
			if (this.TaskInF.ContainsKey(BuildID))
			{
				return this.TaskInF[BuildID];
			}
			return null;
		}

		public bool IsOldTask(int BuildID, int TaskID)
		{
			List<BuildtaskInF> buildTaskXmlDataList = this.GetBuildTaskXmlDataList(BuildID);
			if (buildTaskXmlDataList != null)
			{
				for (int i = 0; i < buildTaskXmlDataList.Count; i++)
				{
					if (TaskID == buildTaskXmlDataList[i].TaskID)
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool BuildHaveTask(int BuildID)
		{
			return this.TaskInF.ContainsKey(BuildID);
		}

		private Dictionary<int, List<BuildtaskInF>> TaskInF = new Dictionary<int, List<BuildtaskInF>>();
	}
}
