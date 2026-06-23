using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class RideZuHeSowPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		base.StartCoroutine<bool>(this.InitView());
	}

	public override void Destroy()
	{
		base.Destroy();
		IConfigbase<ConfigRidePet>.Instance.ClearHorseSuitData();
	}

	private IEnumerator InitView()
	{
		ObservableCollection mObservableCollection = this.mListBox.ItemsSource;
		Super.ShowNetWaiting(null);
		Dictionary<int, HorseSuit>.Enumerator map = IConfigbase<ConfigRidePet>.Instance.GetHorseSuitEnumerator();
		yield return null;
		while (map.MoveNext())
		{
			List<int> GoodsList = new List<int>();
			RidePetZuHeItem item = U3DUtils.NEW<RidePetZuHeItem>();
			KeyValuePair<int, HorseSuit> keyValuePair = map.Current;
			string[] goodsStr = keyValuePair.Value.HorseID.Split(new char[]
			{
				','
			});
			bool Active = false;
			bool MaxJiHuoBool = true;
			if (goodsStr != null && 0 < goodsStr.Length)
			{
				Active = true;
				for (int i = 0; i < goodsStr.Length; i++)
				{
					int goodsId = goodsStr[i].SafeToInt32(0);
					if (Global.Data.roleData.MountEquipList == null)
					{
						Active = false;
						MaxJiHuoBool = false;
					}
					else if (Global.Data.roleData.MountEquipList.Find((GoodsData e) => e.GoodsID == goodsId) == null)
					{
						Active = false;
						MaxJiHuoBool = false;
					}
					GoodsList.Add(goodsId);
				}
			}
			string[] strAttArray = new string[GoodsList.Count];
			KeyValuePair<int, HorseSuit> keyValuePair2 = map.Current;
			string[] strArray = keyValuePair2.Value.HorseSuitProps.Split(new char[]
			{
				'|'
			});
			if (strArray != null)
			{
				for (int j = 0; j < strArray.Length; j++)
				{
					string[] str = strArray[j].Split(new char[]
					{
						','
					});
					if (str.Length == 2 && j < strAttArray.Length)
					{
						if (ConfigExtPropIndexes.GetPercentByWord(str[0]))
						{
							double Att = double.Parse(str[1]) * 100.0;
							strAttArray[j] = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(str[0], false) + "+" + Att.ToString("f1") + "%";
						}
						else
						{
							strAttArray[j] = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(str[0], false) + "+" + str[1];
						}
					}
				}
			}
			MUDebug.Log<bool>(new bool[]
			{
				Active
			});
			item.SetInf(GoodsList, strAttArray, !Active);
			if (MaxJiHuoBool)
			{
				mObservableCollection.Insert(0, item);
			}
			else
			{
				mObservableCollection.Add(item);
			}
			item.DragPanel = this.mDraggablePanel;
			yield return null;
		}
		Super.HideNetWaiting();
		yield break;
	}

	private void InitPrefabText()
	{
		try
		{
			this.mTitlelabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑组合")
			});
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			this.mCloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.hander != null)
				{
					this.hander(this, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public DPSelectedItemEventHandler hander;

	[SerializeField]
	private GButton mCloseBtn;

	[SerializeField]
	private ListBox mListBox;

	[SerializeField]
	private UIDraggablePanel mDraggablePanel;

	[SerializeField]
	private UILabel mTitlelabel;
}
