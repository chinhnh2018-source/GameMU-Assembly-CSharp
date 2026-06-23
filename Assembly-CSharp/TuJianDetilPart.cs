using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class TuJianDetilPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.SubmitBtn.Text = Global.GetLang("一键提交");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.DetilCollection = this.DetilList.ItemsSource;
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartSubmit();
		};
	}

	public void InitPartData(int typeID, string propsStr, bool isCanSubmit)
	{
		this._typeID = typeID;
		this.TypeItem.strTypeID = typeID.ToString();
		this.TextProps.Text = this.GetFormatPropsStr(propsStr, 1);
		foreach (TujianXmlData tujianXmlData in TuJianPart.TujiaXmlDataDict.Values)
		{
			if (tujianXmlData.TypeID == typeID)
			{
				TuJianDetailItem tuJianDetailItem = U3DUtils.NEW<TuJianDetailItem>();
				string[] array = tujianXmlData.NeedGoods.Split(new char[]
				{
					','
				});
				if (array != null)
				{
					tuJianDetailItem.MonsterID = tujianXmlData.MonsterID;
					tuJianDetailItem.TujianID = tujianXmlData.ID;
					tuJianDetailItem.MaxNum = array[1].SafeToInt32(0);
					tuJianDetailItem.GoodsID = array[0].SafeToInt32(0);
					tuJianDetailItem.TxtProps.Text = this.GetFormatPropsStr(tujianXmlData.Props, 3);
					tuJianDetailItem.TxtName2.Text = tujianXmlData.Name;
				}
				UIHelper.AddGoodsIcon(tuJianDetailItem.Goods, Global.GetDummyGoodsDataMu(tuJianDetailItem.GoodsID, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00"), null, false);
				tuJianDetailItem.DPSelectedItem = delegate(object o, DPSelectedItemEventArgs e)
				{
					if (e.ID > 0)
					{
						int id = e.ID;
						MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(id);
						int npcorMonsterMapCodeByID = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID.MapCode);
						int num = 2;
						if (Global.IsGoToKuaFuMap(npcorMonsterMapCodeByID))
						{
							PlayZone.GlobalPlayZone.OpenKuafuMapView(num, -1, id, npcorMonsterMapCodeByID, -1, -1, false, 0, 0, false, false);
						}
						else
						{
							Super.AutoFindRoad(npcorMonsterMapCodeByID, num, id, 1);
						}
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = 1
						});
					}
				};
				this.DetilCollection.AddNoUpdate(tuJianDetailItem);
			}
		}
		this.DetilCollection.DelayUpdate();
		this.RefreshDetilList(isCanSubmit);
	}

	private void RefreshDetilList(bool isCanSubmit = false)
	{
		int num = 0;
		int activedTujianTypeNumByTypeID = TuJianPart.GetActivedTujianTypeNumByTypeID(this._typeID, out num);
		this.TypeItem.m_LblInfo.text = string.Format("{0}/{1}", activedTujianTypeNumByTypeID, num);
		this.IsAllActived = (activedTujianTypeNumByTypeID == num);
		for (int i = 0; i < this.DetilCollection.Length; i++)
		{
			TuJianDetailItem tuJianDetailItem = U3DUtils.AS<TuJianDetailItem>(this.DetilCollection[i]);
			int tiJiaoTuJianNum = Global.GetTiJiaoTuJianNum(tuJianDetailItem.TujianID);
			tuJianDetailItem.IsActived = (tiJiaoTuJianNum >= tuJianDetailItem.MaxNum);
			tuJianDetailItem.TxtName.Text = string.Format("{0}: {1}/{2}", Global.GetGoodsNameByID(tuJianDetailItem.GoodsID, false), tiJiaoTuJianNum, tuJianDetailItem.MaxNum);
		}
		this.Anim.gameObject.SetActive(isCanSubmit);
	}

	private string GetFormatPropsStr(string propsStr, int maxPerLine = 1)
	{
		string text = string.Empty;
		if (string.IsNullOrEmpty(propsStr))
		{
			return text;
		}
		string[] array = propsStr.Split(new char[]
		{
			'|'
		});
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2 != null)
				{
					int num = ExtPropIndexes.ExtPropIndexNames.IndexOf(array2[0].ToLower());
					text += Global.GetColorStringForNGUIText(new object[]
					{
						"E5BB6F",
						string.Format("{0}: ", Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[num])),
						"F5E3BB",
						string.Format("{0}", array2[1])
					});
					if ((i + 1) % maxPerLine == 0)
					{
						text += "\n";
					}
					else
					{
						text += "      ";
					}
				}
			}
		}
		return text;
	}

	private string ProcessStr(string str)
	{
		if (str.Length > 0 && str.Substring(str.Length - 1) == ",")
		{
			str = str.Substring(0, str.Length - 1);
		}
		return str;
	}

	private void StartSubmit()
	{
		if (this.IsAllActived)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("当前地图图鉴已全部激活"), 0, -1, -1, 0);
			return;
		}
		TuJianPart.InitTujianListInBag();
		if (TuJianPart.TujianListInBagDict == null || TuJianPart.TujianListInBagDict.Keys.Count <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("背包中没有可以提交的图鉴"), 0, -1, -1, 0);
			return;
		}
		string text = string.Empty;
		foreach (int num in TuJianPart.TujianListInBagDict.Keys)
		{
			if (TuJianPart.TujianListInBagDict[num] == this._typeID)
			{
				text += num;
				text += ",";
			}
		}
		text = this.ProcessStr(text);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		GameInstance.Game.SpriteReferPictureJudgeCmd(this.ProcessStr(text));
	}

	public void NotifySubmitResult(Dictionary<int, int> dict)
	{
		if (dict != null && dict.Count > 0)
		{
			foreach (int num in dict.Keys)
			{
				TujianXmlData tujianXmlData = null;
				if (TuJianPart.TujiaXmlDataDict.TryGetValue(num, ref tujianXmlData))
				{
					string[] array = tujianXmlData.NeedGoods.Split(new char[]
					{
						','
					});
					if (array != null)
					{
						int id = array[0].SafeToInt32(0);
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("{0}x{1}提交成功"), new object[]
						{
							Global.GetGoodsNameByID(id, false),
							dict[num]
						}), 0, -1, -1, 0);
					}
				}
			}
		}
		this.RefreshDetilList(false);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox DetilList;

	private ObservableCollection DetilCollection;

	public GButton CloseBtn;

	public GButton SubmitBtn;

	public TuJianTypeItem TypeItem;

	public TextBlock TextProps;

	public GameObject Anim;

	private int _typeID;

	private bool IsAllActived;
}
