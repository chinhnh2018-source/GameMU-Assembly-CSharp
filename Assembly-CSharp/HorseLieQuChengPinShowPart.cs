using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class HorseLieQuChengPinShowPart : UserControl
{
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (0 < this.mHorseResLoaderList.Count)
		{
			for (int i = 0; i < this.mHorseResLoaderList.Count; i++)
			{
				if (this.mHorseResLoaderList[i] != null)
				{
					this.mHorseResLoaderList[i].Stop();
				}
			}
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		base.StartCoroutine<bool>(this.Cd());
		this.mHorseResLoaderList.Clear();
	}

	private void InitTeXiao()
	{
		GameObject gameObject = Global.LoadTeXiaoObj("UITeXiao/Perfabs/zuoqi/zuoqi_zhanshi_effect", this._TeXiaoRoot.transform);
		if (null != gameObject)
		{
		}
	}

	private void InitPrefabText()
	{
		try
		{
			this._ClickLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("点击任意位置关闭界面")
			});
			this.m_LabNameTitle.text = string.Empty;
			this.m_LabZhuoYueContent.text = string.Empty;
			this.m_LabName.text = string.Empty;
			this.m_LabZhuoYueTitle.text = string.Empty;
			this.m_BtnQueDing.Text = Global.GetLang("确定");
			this.m_LabZhuoYueTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑卓越属性")
			});
			this.m_LabZhuoYueContent.pivot = 2;
			this.m_LabZhuoYueContent.transform.localPosition = new Vector3(75f, 41f, -1f);
			this.m_LabZhuoYueContent.lineWidth = 250;
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
			this.BgImage.URL = "NetImages/GameRes/Images/RidePet/HuoDeDi.jpg";
			this.BgImage.ImageDownloaded = delegate(object g)
			{
				this.BgImage.transform.localScale = new Vector3((float)this.BgImage.ItsSizeWidth, (float)this.BgImage.ItsSizeHeight, 0f);
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

	private void InitHandler()
	{
		try
		{
			this.m_BtnRuKu.gameObject.SetActive(false);
			this.m_BtnQueDing.transform.localPosition = new Vector3(0f, -200f, -0.1f);
			this.m_BtnQueDing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.Hander != null)
				{
					this.Hander(s, new DPSelectedItemEventArgs
					{
						Type = 0,
						ID = 2
					});
				}
			};
			UIEventListener.Get(this.BgImage.gameObject).onClick = delegate(GameObject s)
			{
				if (this.CanCkick && this.Hander != null)
				{
					this.Hander(s, new DPSelectedItemEventArgs
					{
						Type = 0,
						ID = 2
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

	private IEnumerator Cd()
	{
		yield return new WaitForSeconds(1f);
		this.CanCkick = true;
		yield break;
	}

	private void AddZuoYue(int ID)
	{
		for (int i = 0; i < this.mUpSpList.Count; i++)
		{
			Object.DestroyObject(this.mUpSpList[i].gameObject);
		}
		this.mUpSpList.Clear();
		string text = string.Empty;
		GoodsData roleHorseGoodsDataInHorseCangKuByDbId = Global.GetRoleHorseGoodsDataInHorseCangKuByDbId(Global.Data.RoleID, ID, 0);
		if (roleHorseGoodsDataInHorseCangKuByDbId != null)
		{
			this.m_LabName.text = Global.GetGoodsNameByID(roleHorseGoodsDataInHorseCangKuByDbId.GoodsID, true);
			List<HorseLieQuChengPinShowPart.PropData> list = new List<HorseLieQuChengPinShowPart.PropData>();
			this.m_LabZhuoYueTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑卓越属性")
			});
			if (roleHorseGoodsDataInHorseCangKuByDbId.WashProps != null && 0 < roleHorseGoodsDataInHorseCangKuByDbId.WashProps.Count)
			{
				for (int j = 0; j < roleHorseGoodsDataInHorseCangKuByDbId.WashProps.Count; j += 2)
				{
					if (j < roleHorseGoodsDataInHorseCangKuByDbId.WashProps.Count - 1)
					{
						list.Add(new HorseLieQuChengPinShowPart.PropData
						{
							Id = roleHorseGoodsDataInHorseCangKuByDbId.WashProps[j],
							Value = (double)((float)roleHorseGoodsDataInHorseCangKuByDbId.WashProps[j + 1] / 1000f)
						});
					}
				}
			}
			else
			{
				this.m_LabZhuoYueTitle.text = string.Empty;
			}
			Dictionary<string, int> dictionary = Global.MaxZhuoYurZuoQi(roleHorseGoodsDataInHorseCangKuByDbId.GoodsID);
			for (int k = 0; k < list.Count; k++)
			{
				if (ConfigExtPropIndexes.GetPercentByID(list[k].Id))
				{
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(list[k].Id, true)
						}),
						"      ",
						Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							(list[k].Value * 100.0).ToString("f1") + "%"
						}),
						Environment.NewLine
					});
				}
				else
				{
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(list[k].Id, true)
						}),
						"      ",
						Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							list[k].Value.ToString("f0")
						}),
						Environment.NewLine
					});
				}
				if (ConfigExtPropIndexes.GetExtPropIndexesVoByID(list[k].Id) != null)
				{
					string word = ConfigExtPropIndexes.GetExtPropIndexesVoByID(list[k].Id).Word;
					if (dictionary.ContainsKey(word) && (double)dictionary[word] <= list[k].Value * 1000.0)
					{
						GameObject gameObject = Object.Instantiate<GameObject>(this.mUpSp.gameObject);
						gameObject.transform.SetParent(this.m_LabZhuoYueContent.transform.parent.transform, false);
						this.mUpSpList.Add(gameObject);
						gameObject.transform.localPosition = new Vector3(100f, this.m_LabZhuoYueContent.transform.localPosition.y - 5f - (float)(21 * k), -1f);
						gameObject.gameObject.SetActive(true);
					}
				}
			}
		}
		this.m_LabZhuoYueContent.text = text;
	}

	private IEnumerator ShowModal(GoodsData Goodsdata)
	{
		yield return new WaitForSeconds(0.27f);
		if (this.HaveActiviteGoodsList != null)
		{
			if (-1 < this.HaveActiviteGoodsList.FindIndex((int e) => e == Goodsdata.GoodsID))
			{
				this.m_LabNameTitle.text = string.Empty;
			}
			else
			{
				this.m_LabNameTitle.text = Global.GetColorStringForNGUIText(new object[]
				{
					"A49868",
					Global.GetLang("激活坐骑图鉴")
				});
			}
		}
		yield return null;
		this.AddZuoYue(Goodsdata.Id);
		if (IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(Goodsdata.GoodsID) != null)
		{
			this.m_GameModal3D.gameObject.SetActive(true);
			HorseResLoader loader = UIHelper.LoadHorseRes(this.m_GameModal3D, Goodsdata.GoodsID, Goodsdata.Forge_level + 1, Quaternion.Euler(0f, 125f, 0f), new Vector3(110f, 110f, 110f), delegate(GameObject g)
			{
				if (this.m_GameModal3D.ChildGameObjectList != null && 1 < this.m_GameModal3D.ChildGameObjectList.Count)
				{
					for (int i = this.m_GameModal3D.ChildGameObjectList.Count - 1; i > 0; i--)
					{
						if (null != this.m_GameModal3D.ChildGameObjectList[i])
						{
							Object.Destroy(this.m_GameModal3D.ChildGameObjectList[i]);
							this.m_GameModal3D.ChildGameObjectList.RemoveAt(this.m_GameModal3D.ChildGameObjectList.Count - 1);
						}
					}
					this.m_GameModal3D._Target = this.m_GameModal3D.ChildGameObjectList[0];
				}
			});
			this.mHorseResLoaderList.Add(loader);
		}
		yield break;
	}

	public void Show3DModal(GoodsData Goodsdata)
	{
		this.m_GameModal3D.gameObject.SetActive(false);
		base.StartCoroutine<bool>(this.ShowModal(Goodsdata));
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private UILabel m_LabZhuoYueTitle;

	[SerializeField]
	private UILabel m_LabZhuoYueContent;

	[SerializeField]
	private UILabel m_LabName;

	[SerializeField]
	private UILabel m_LabNameTitle;

	[SerializeField]
	private GButton m_BtnRuKu;

	[SerializeField]
	private GButton m_BtnQueDing;

	[SerializeField]
	private Modal3DShow m_GameModal3D;

	[SerializeField]
	private ShowNetImage BgImage;

	[SerializeField]
	private GameObject _TeXiaoRoot;

	[SerializeField]
	private UILabel _ClickLabel;

	[SerializeField]
	private UISprite mUpSp;

	private List<GameObject> mUpSpList = new List<GameObject>();

	private bool CanCkick;

	private List<HorseResLoader> mHorseResLoaderList = new List<HorseResLoader>();

	public List<int> HaveActiviteGoodsList;

	private struct PropData
	{
		public int Id;

		public double Value;
	}
}
