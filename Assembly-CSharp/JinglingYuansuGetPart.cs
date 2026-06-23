using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class JinglingYuansuGetPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BtnSubmits[0].Text = Global.GetLang("提炼1次");
		this.BtnSubmits[1].Text = Global.GetLang("提炼10次");
		for (int i = 1; i <= 5; i++)
		{
			this.txtDang[i - 1].text = string.Format(Global.GetLang("{0}档"), i);
		}
		this.GCheckBoxPlayAnim._Lable.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("不播放动画")
		});
		this.GCheckBoxPlayAnim._Lable.lineWidth = 100;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitTypeList();
		this.InitProgress();
		this.BtnSubmits[0].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.IsEnabledTilian(this.CurrentProgress, 1, false))
			{
				this.StartTilian(1, false);
			}
		};
		this.BtnSubmits[1].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.IsEnabledTilian(this.CurrentProgress, 10, false))
			{
				this.StartTilian(10, false);
			}
		};
		UIEventListener.Get(this.BtnZhaohuan.gameObject).onClick = delegate(GameObject s)
		{
			if (this.IsEnabledTilian(this.CurrentProgress, 1, true))
			{
				this.StartTilian(1, true);
			}
		};
		this.GCheckBoxPlayAnim.Check = false;
	}

	private void OnEnable()
	{
		this.IsDoing = true;
		this.popCount = 0;
		Super.ActiveGameObject(this.Get10Anim, false);
		this.QueryGetYuansuInfo();
	}

	private void OnDisable()
	{
		this.RemoveIcons();
	}

	protected override void OnDestroy()
	{
		this.SetGetBtnState(true);
		base.OnDestroy();
	}

	private int popCount
	{
		get
		{
			return this._popCount;
		}
		set
		{
			this._popCount = value;
			if (null != this.mask)
			{
				this.mask.SetActive(this._popCount > 0);
			}
		}
	}

	private void InitTypeList()
	{
		if (this.RefineTypeDict != null)
		{
			return;
		}
		this.RefineTypeDict = new Dictionary<int, RefineTypeXmlData>();
		this.RefineTypeDict.Clear();
		XElement gameResXml = Global.GetGameResXml("Config/RefineType.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			RefineTypeXmlData refineTypeXmlData = new RefineTypeXmlData();
			refineTypeXmlData.ID = Global.GetXElementAttributeInt(xelement, "ID");
			refineTypeXmlData.RefineCost = Global.GetXElementAttributeInt(xelement, "RefineCost");
			refineTypeXmlData.ZuanShiCost = Global.GetXElementAttributeInt(xelement, "ZuanShiCost");
			refineTypeXmlData.RefineLevel = Global.GetXElementAttributeInt(xelement, "RefineLevel");
			if (!this.RefineTypeDict.ContainsKey(refineTypeXmlData.ID))
			{
				this.RefineTypeDict.Add(refineTypeXmlData.ID, refineTypeXmlData);
			}
			if (refineTypeXmlData.ID == 6)
			{
				this.UseDiamondNum = refineTypeXmlData.ZuanShiCost;
			}
		}
	}

	private void InitProgress()
	{
		this.CurrentProgress = 1;
		this.SetProgressResult(this.CurrentProgress, true);
		if (this.RefineTypeDict == null)
		{
			return;
		}
		if (this.TextMoneys != null && this.TextMoneys.Length == 5)
		{
			RefineTypeXmlData refineTypeXmlData = null;
			for (int i = 0; i < this.TextMoneys.Length; i++)
			{
				if (this.RefineTypeDict.TryGetValue(i + 1, ref refineTypeXmlData))
				{
					this.TextMoneys[i].Text = refineTypeXmlData.RefineCost.ToString();
				}
			}
		}
	}

	private void SetProgress(int level)
	{
		if (this.NetImages == null)
		{
			return;
		}
		if (level == this.CurrentProgress)
		{
			return;
		}
		if (level < 1 || level > this.NetImages.Length)
		{
			return;
		}
		this.SetProgressResult(level, false);
		this.CurrentProgress = level;
	}

	private void SetProgressResult(int level, bool isInit = false)
	{
		int num = level - 1;
		int num2 = this.CurrentProgress - 1;
		if (!isInit)
		{
			for (int i = 0; i < this.NetImages.Length; i++)
			{
				if (num >= i)
				{
					this.Anims[i].gameObject.SetActive(true);
					this.NetImages[i].gameObject.SetActive(false);
					this.NetImages[i].ToGrayBitmap = false;
				}
				else
				{
					this.Anims[i].gameObject.SetActive(false);
					this.NetImages[i].gameObject.SetActive(true);
					this.NetImages[i].ToGrayBitmap = true;
				}
			}
		}
		else
		{
			for (int j = 0; j < this.NetImages.Length; j++)
			{
				if (num == j)
				{
					this.NetImages[j].ToGrayBitmap = false;
					this.NetImages[j].gameObject.SetActive(false);
					this.Anims[j].gameObject.SetActive(true);
				}
				else
				{
					this.NetImages[j].ToGrayBitmap = true;
					this.NetImages[j].gameObject.SetActive(true);
					this.Anims[j].gameObject.SetActive(false);
				}
			}
		}
	}

	private void RefreshAddGoodIcons(string goodsStr)
	{
		this.RemoveIcons();
		this.IsStopped = false;
		string text = StringUtil.trim(goodsStr);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		List<GoodsData> list = new List<GoodsData>();
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 2)
			{
				int goodsID = Convert.ToInt32(array2[0]);
				int level = Convert.ToInt32(array2[1]);
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(goodsID, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				Global.SetGoodsDataYuansuProps(dummyGoodsDataMu, level, 0);
				list.Add(dummyGoodsDataMu);
			}
		}
		base.StartCoroutine("AddGoodListIcon", list);
	}

	private IEnumerator AddGoodListIcon(List<GoodsData> goodsList)
	{
		this.IsStopped = false;
		this.popCount = goodsList.Count;
		int goodsCount = goodsList.Count;
		if (goodsCount == 1)
		{
			this.AddGoodIcon(goodsList[0], new Vector3(0f, 0f, 0f));
		}
		else
		{
			int beginX = -180;
			float interval = 90f;
			float realY = 30f;
			float realX = 0f;
			for (int i = 0; i < goodsCount; i++)
			{
				if (i >= 5)
				{
					realY = -50f;
					realX = (float)beginX + interval * (float)(i - 5);
				}
				else
				{
					realX = (float)beginX + interval * (float)i;
				}
				if (this.IsStopped)
				{
					yield break;
				}
				this.AddGoodIcon(goodsList[i], new Vector3(realX, realY, 0f));
				yield return null;
			}
		}
		yield break;
	}

	private void AddGoodIcon(GoodsData gd, Vector3 localPos)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		GameObject flashPrefab = this.GetFlashPrefab();
		flashPrefab.gameObject.transform.localPosition = localPos;
		U3DUtils.AddChild(this.IconContainer, flashPrefab, true);
		this.AnimList.Add(flashPrefab);
		GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
		icon.Width = 78.0;
		icon.Height = 78.0;
		icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		icon.TipType = 1;
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.BoxTypes = 0;
		icon.TextSize = 16;
		icon.TextShadowColor = 4278190080U;
		icon.Tag = gd.ExcellenceInfo;
		icon.BackSpriteName0 = "bagGrid4_bak";
		Super.InitYuansuGoodsGIcon(icon, gd);
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, gd);
		};
		U3DUtils.AddChild(this.IconContainer, icon.gameObject, true);
		icon.transform.localPosition = localPos;
		icon.transform.localScale = new Vector3(0f, 0f, 0f);
		iTween.ScaleTo(icon.gameObject, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(1f, 1f, 1f),
			"time",
			0.3f,
			"easeType",
			18,
			"oncomplete",
			"OnTweenComplete",
			"oncompletetarget",
			base.gameObject
		}));
		this.IconList.Add(icon);
	}

	private bool IsEnabledTilian(int index, int bagCount, bool isDiamond = false)
	{
		if (!this.IsDoing)
		{
			return false;
		}
		if (Global.GetYuansuBagEmptyGridCount() < bagCount)
		{
			Super.HintMainText(string.Format(Global.GetLang("至少需要{0}个格子"), bagCount), 10, 3);
			return false;
		}
		if (!isDiamond)
		{
			RefineTypeXmlData refineTypeXmlData = null;
			if (this.RefineTypeDict.TryGetValue(index, ref refineTypeXmlData))
			{
				int refineCost = refineTypeXmlData.RefineCost;
				int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.YuansuFenmo);
				if (roleCommonUseParamsValue < refineCost)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedYuansuFenmo, null, string.Empty, Global.GetLang("元素粉末"));
					return false;
				}
			}
		}
		else if (Global.Data.roleData.UserMoney < this.UseDiamondNum)
		{
			Super.HintMainText(Global.GetLang("钻石不足!"), 10, 3);
			return false;
		}
		return true;
	}

	private void SetBtnText(int index, bool isInit)
	{
	}

	public void OnTweenComplete()
	{
		base.StartCoroutine<bool>(this.StartTweenComplete());
	}

	public void OnTweenComplete2()
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1
			});
			this.popCount--;
			this.IsDoing = true;
		}
	}

	public IEnumerator StartTweenComplete()
	{
		yield return new WaitForSeconds(0.5f);
		int count = this.IconList.Count;
		if (count > 0)
		{
			GGoodIcon icon = null;
			float disposeTime = 0.5f;
			Vector3 disposePos = new Vector3(463f, 32f, 0f);
			for (int i = 0; i < count; i++)
			{
				icon = this.IconList[i];
				iTween.MoveTo(icon.gameObject, iTween.Hash(new object[]
				{
					"position",
					disposePos,
					"time",
					disposeTime,
					"islocal",
					true,
					"oncomplete",
					"OnTweenComplete2",
					"oncompletetarget",
					base.gameObject
				}));
				iTween.ScaleTo(icon.gameObject, Vector3.zero, disposeTime);
			}
		}
		yield break;
	}

	private void RemoveIcons()
	{
		this.IsStopped = true;
		base.StopCoroutine("AddGoodListIcon");
		iTween.Stop();
		int count = this.IconList.Count;
		for (int i = count - 1; i >= 0; i--)
		{
			NGUITools.Destroy(this.IconList[i].gameObject);
			NGUITools.Destroy(this.AnimList[i].gameObject);
		}
		this.IconList.Clear();
		this.AnimList.Clear();
	}

	public GameObject GetFlashPrefab()
	{
		if (this.FlashPrefab == null)
		{
			this.FlashPrefab = (Resources.Load(string.Format("UITeXiao/UI_yuansuzhixin/Yuansu_tilian_shan/yuansu_tilian_shan", new object[0])) as GameObject);
		}
		return SpawnManager.Instantiate(this.FlashPrefab) as GameObject;
	}

	public GameObject GetFlyPrefab()
	{
		if (this.FlyPrefab == null)
		{
			this.FlyPrefab = (Resources.Load(string.Format("UITeXiao/UI_yuansuzhixin/Yuansu_tilianwei/yuansu_tilianwei", new object[0])) as GameObject);
		}
		return SpawnManager.Instantiate(this.FlyPrefab) as GameObject;
	}

	public void SetGet10Anim()
	{
		if (null == this.Get10Anim)
		{
			return;
		}
	}

	private void StartTilian(int count, bool isDiamond = false)
	{
		if (isDiamond)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			string message = string.Format(Global.GetLang("花费{0}钻石提升至4阶\n\n概率获取一个【元素精华】或【元素碎片】"), this.UseDiamondNum);
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					this.SetGetBtnState(false);
					this.StartCoroutine<bool>(this.ExeTilian(count, true));
				}
			}, buttons);
			return;
		}
		this.SetGetBtnState(false);
		base.StartCoroutine<bool>(this.ExeTilian(count, false));
	}

	public void SetGetBtnState(bool isEnable)
	{
		try
		{
			PlayZone.GlobalPlayZone.JingLingPart.jinglingYuansuPart.BtnGet.isEnabled = isEnable;
		}
		catch (Exception ex)
		{
		}
	}

	private IEnumerator ExeTilian(int count, bool isDiamond = false)
	{
		this.IsDoing = false;
		this.mCount = count;
		if (count == 10)
		{
			if (!this.GCheckBoxPlayAnim.Check && count == 10 && !isDiamond)
			{
				Super.PlayAnim(this.Get10Anim);
				yield return new WaitForSeconds(1f);
			}
		}
		else if (count == 10 && !isDiamond)
		{
			Super.PlayAnim(this.Get10Anim);
			yield return new WaitForSeconds(1f);
		}
		this.ShowModalDialog();
		Global.Data.IsDoingYuanSuTilian = true;
		GameInstance.Game.SpriteStartTilian(count, Convert.ToInt32(isDiamond));
		yield break;
	}

	public void NotifyTilianResult(int result, int money, int level, string goodsStr)
	{
		this.SetGetBtnState(true);
		this.CloseModalDialog();
		if (result == 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("提炼成功"), new object[0]), 0, -1, -1, 0);
			this.SetProgress(level);
			if (this.mCount == 10)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 2
				});
				if (!this.GCheckBoxPlayAnim.Check)
				{
					this.RefreshAddGoodIcons(goodsStr);
				}
				else
				{
					string text = StringUtil.trim(goodsStr);
					if (string.IsNullOrEmpty(text))
					{
						return;
					}
					string[] array = text.Split(new char[]
					{
						'|'
					});
					if (array.Length <= 0)
					{
						return;
					}
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						if (array2.Length == 2)
						{
							int id = Convert.ToInt32(array2[0]);
							Super.HintMainText(Global.GetGoodsNameByID(id, true) + " * 1", 10, 3);
							this.OnTweenComplete2();
						}
					}
				}
			}
			else
			{
				this.RefreshAddGoodIcons(goodsStr);
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 2
				});
			}
		}
		else if (result == 11)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedYuansuFenmo, null, string.Empty, Global.GetLang("元素粉末"));
			this.SetProgress(level);
			this.RefreshAddGoodIcons(goodsStr);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 2
			});
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("提炼时发生错误:{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
	}

	public void QueryGetYuansuInfo()
	{
		this.ShowModalDialog();
		GameInstance.Game.SpriteQueryGetYuanInfo();
	}

	public void NotifyGetYuansuInfoResult(int level)
	{
		this.CloseModalDialog();
		level = Math.Max(level, 1);
		this.SetProgress(level);
	}

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject BtnZhaohuan;

	public GameObject[] Anims;

	public GameObject Get10Anim;

	public ShowNetImage[] NetImages;

	public TextBlock[] TextMoneys;

	public GButton[] BtnSubmits;

	public GameObject IconContainer;

	public GCheckBox GCheckBoxPlayAnim;

	private int CurrentProgress = 1;

	private Dictionary<int, RefineTypeXmlData> RefineTypeDict;

	private List<GGoodIcon> IconList = new List<GGoodIcon>();

	private List<GameObject> AnimList = new List<GameObject>();

	private bool IsStopped;

	private bool IsDoing = true;

	private int UseDiamondNum;

	public TextBlock[] txtDang;

	public new GameObject mask;

	private int _popCount;

	private GameObject FlashPrefab;

	private GameObject FlyPrefab;

	private int mCount;
}
