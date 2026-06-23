using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ActivityCommonSaiShipPinPart : UserControl
{
	private OrnamentXmlConfig OrnamentXmlConfig
	{
		get
		{
			if (this.mOrnamentXmlConfig == null)
			{
				this.mOrnamentXmlConfig = new OrnamentXmlConfig();
			}
			return this.mOrnamentXmlConfig;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mBak.gameObject.SetActive(false);
		this.mBtnGoShiPinPart.gameObject.SetActive(false);
		this.mBtnClose.gameObject.SetActive(false);
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitData()
	{
		ActivityCommonSaiShipPinPart.SpecialOrnamentData specialOrnamentData = new ActivityCommonSaiShipPinPart.SpecialOrnamentData();
		specialOrnamentData.SpecialOrnament = ActivityCommonSaiShipPinPart.SpecialOrnament.ZhanMengLianSai;
		specialOrnamentData.GoalTypeList.Add(6);
		specialOrnamentData.GoalTypeList.Add(7);
		specialOrnamentData.GoalTypeList.Add(8);
		specialOrnamentData.GoalTypeList.Add(9);
		this.DicData.Add((int)specialOrnamentData.SpecialOrnament, specialOrnamentData);
		ActivityCommonSaiShipPinPart.SpecialOrnamentData specialOrnamentData2 = new ActivityCommonSaiShipPinPart.SpecialOrnamentData();
		specialOrnamentData2.SpecialOrnament = ActivityCommonSaiShipPinPart.SpecialOrnament.KuFuPlunder;
		specialOrnamentData2.GoalTypeList.Add(15);
		specialOrnamentData2.GoalTypeList.Add(16);
		this.DicData.Add((int)specialOrnamentData2.SpecialOrnament, specialOrnamentData2);
		ActivityCommonSaiShipPinPart.SpecialOrnamentData specialOrnamentData3 = new ActivityCommonSaiShipPinPart.SpecialOrnamentData();
		specialOrnamentData3.SpecialOrnament = ActivityCommonSaiShipPinPart.SpecialOrnament.WangZheZhanChang;
		specialOrnamentData3.GoalTypeList.Add(2);
		specialOrnamentData3.GoalTypeList.Add(10);
		this.DicData.Add((int)specialOrnamentData3.SpecialOrnament, specialOrnamentData3);
		ActivityCommonSaiShipPinPart.SpecialOrnamentData specialOrnamentData4 = new ActivityCommonSaiShipPinPart.SpecialOrnamentData();
		specialOrnamentData4.SpecialOrnament = ActivityCommonSaiShipPinPart.SpecialOrnament.YongZheZhanChang;
		specialOrnamentData4.GoalTypeList.Add(3);
		specialOrnamentData4.GoalTypeList.Add(11);
		this.DicData.Add((int)specialOrnamentData4.SpecialOrnament, specialOrnamentData4);
		ActivityCommonSaiShipPinPart.SpecialOrnamentData specialOrnamentData5 = new ActivityCommonSaiShipPinPart.SpecialOrnamentData();
		specialOrnamentData5.SpecialOrnament = ActivityCommonSaiShipPinPart.SpecialOrnament.KuaFuTianTi;
		specialOrnamentData5.GoalTypeList.Add(12);
		specialOrnamentData5.GoalTypeList.Add(13);
		this.DicData.Add((int)specialOrnamentData5.SpecialOrnament, specialOrnamentData5);
		ActivityCommonSaiShipPinPart.SpecialOrnamentData specialOrnamentData6 = new ActivityCommonSaiShipPinPart.SpecialOrnamentData();
		specialOrnamentData6.SpecialOrnament = ActivityCommonSaiShipPinPart.SpecialOrnament.QingLvJingJiChang;
		specialOrnamentData6.GoalTypeList.Add(14);
		this.DicData.Add((int)specialOrnamentData6.SpecialOrnament, specialOrnamentData6);
		ActivityCommonSaiShipPinPart.SpecialOrnamentData specialOrnamentData7 = new ActivityCommonSaiShipPinPart.SpecialOrnamentData();
		specialOrnamentData7.SpecialOrnament = ActivityCommonSaiShipPinPart.SpecialOrnament.DaTaoSha;
		specialOrnamentData7.GoalTypeList.Add(17);
		this.DicData.Add((int)specialOrnamentData7.SpecialOrnament, specialOrnamentData7);
	}

	private void InitPrefabText()
	{
		try
		{
			this.mBtnGoShiPinPart.Label.text = Global.GetLang("饰品详情");
			this.mTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("专属饰品")
			});
			this.mBtnGoShiPinPart.transform.localPosition = new Vector3(this.mBtnGoShiPinPart.transform.localPosition.x, this.mBtnGoShiPinPart.transform.localPosition.y, -0.001f);
			Transform transform = base.transform.FindChild("bak");
			if (null != transform)
			{
				transform.gameObject.AddComponent<BoxCollider>();
			}
		}
		catch (Exception ex)
		{
		}
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		try
		{
			this.mObservableCollection = this.mListBox.ItemsSource;
			this.mBtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
			this.mBtnGoShiPinPart.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1491,
					Index = 1
				});
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						Type = 1,
						ID = 13
					});
				}
			};
		}
		catch (Exception ex)
		{
		}
	}

	public void RefreshInf(ActivityCommonSaiShipPinPart.SpecialOrnament specialOrnament)
	{
		if (0 >= this.DicData.Count)
		{
			this.InitData();
		}
		if (this.DicData.ContainsKey((int)specialOrnament))
		{
			this.mOrnamentXmlDataLst = this.OrnamentXmlConfig.GetOrnamentXmlDataLst(this.DicData[(int)specialOrnament].GoalTypeList);
			if (this.mOrnamentXmlDataLst != null && 0 < this.mOrnamentXmlDataLst.Count)
			{
				GameInstance.Game.SendGetShiPinData();
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					Global.GetLang("联赛饰品配置有误11")
				});
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				Global.GetLang("联赛饰品配置有误")
			});
		}
	}

	public void NoticeGetShiPinDataCallBack(Dictionary<int, OrnamentData> data)
	{
		if (this.mOrnamentXmlDataLst != null)
		{
			int count = this.mOrnamentXmlDataLst.Count;
			for (int i = 0; i < count; i++)
			{
				OrnamentXmlData ornamentXmlData = this.mOrnamentXmlDataLst[i];
				ZhanMengLianSaiShipPinItem zhanMengLianSaiShipPinItem = U3DUtils.NEW<ZhanMengLianSaiShipPinItem>();
				OrnamentData ornamentData = null;
				foreach (KeyValuePair<int, OrnamentData> keyValuePair in data)
				{
					if (keyValuePair.Value.ID == this.mOrnamentXmlDataLst[i].GoodsID)
					{
						Dictionary<int, OrnamentData>.Enumerator enumerator;
						KeyValuePair<int, OrnamentData> keyValuePair2 = enumerator.Current;
						ornamentData = keyValuePair2.Value;
						break;
					}
				}
				string description = string.Empty;
				int num = (0 > ornamentXmlData.GoalNum) ? 0 : ornamentXmlData.GoalNum;
				int num2 = 0;
				if (ornamentData != null)
				{
					num2 = ((ornamentData.Param1 > num) ? num : ornamentData.Param1);
				}
				description = string.Format("{0}{1}", this.mOrnamentXmlDataLst[i].Description, string.Format("({0}/{1})", num2, num));
				zhanMengLianSaiShipPinItem.RefreshInf(this.mOrnamentXmlDataLst[i].GoodsID, this.mOrnamentXmlDataLst[i].Name, description);
				this.mObservableCollection.AddNoUpdate(zhanMengLianSaiShipPinItem);
				if (4 < count)
				{
					this.AddDrag(zhanMengLianSaiShipPinItem.gameObject, this.mDragPanelView);
				}
			}
			this.mListBox.repositionNow = true;
			if (4 > count)
			{
				Vector3 localPosition = this.mBtnClose.transform.localPosition;
				localPosition.y -= (float)((4 - count) * 40);
				this.mBtnClose.transform.localPosition = localPosition;
				localPosition = this.mTitleLabel.transform.localPosition;
				localPosition.y -= (float)((4 - count) * 40);
				this.mTitleLabel.transform.localPosition = localPosition;
				localPosition = this.mBtnGoShiPinPart.transform.localPosition;
				localPosition.y += (float)((4 - count) * 40);
				this.mBtnGoShiPinPart.transform.localPosition = localPosition;
				Vector3 localScale = this.mBak.localScale;
				localScale.y -= (float)((4 - count) * 80);
				this.mBak.localScale = localScale;
				localPosition = this.mListBox.transform.localPosition;
				localPosition.y -= (float)((4 - count) * 40);
				this.mListBox.transform.localPosition = localPosition;
			}
			this.mBak.gameObject.SetActive(true);
			this.mBtnGoShiPinPart.gameObject.SetActive(true);
			this.mBtnClose.gameObject.SetActive(true);
		}
	}

	private void AddDrag(GameObject obj, UIDraggablePanel dp)
	{
		if (null != obj && null != dp)
		{
			BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
			if (null == boxCollider)
			{
				boxCollider = obj.AddComponent<BoxCollider>();
			}
			Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(obj.transform);
			boxCollider.size = new Vector3(428f, 82f, 0f);
			UIDragPanelContents uidragPanelContents = obj.GetComponent<UIDragPanelContents>();
			if (null == uidragPanelContents)
			{
				uidragPanelContents = obj.AddComponent<UIDragPanelContents>();
			}
			uidragPanelContents.draggablePanel = dp;
			UIPanel component = obj.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	[SerializeField]
	private Transform mBak;

	[SerializeField]
	private GButton mBtnGoShiPinPart;

	[SerializeField]
	private UIDraggablePanel mDragPanelView;

	[SerializeField]
	private ListBox mListBox;

	[SerializeField]
	private GButton mBtnClose;

	[SerializeField]
	private UILabel mTitleLabel;

	private OrnamentXmlConfig mOrnamentXmlConfig;

	private ObservableCollection mObservableCollection;

	private Dictionary<int, ActivityCommonSaiShipPinPart.SpecialOrnamentData> DicData = new Dictionary<int, ActivityCommonSaiShipPinPart.SpecialOrnamentData>();

	private List<OrnamentXmlData> mOrnamentXmlDataLst;

	public DPSelectedItemEventHandler Hander;

	private class SpecialOrnamentData
	{
		public ActivityCommonSaiShipPinPart.SpecialOrnament SpecialOrnament;

		public List<int> GoalTypeList = new List<int>();
	}

	public enum SpecialOrnament
	{
		ZhanMengLianSai,
		KuFuPlunder,
		WangZheZhanChang,
		YongZheZhanChang,
		KuaFuTianTi,
		QingLvJingJiChang,
		DaTaoSha
	}
}
