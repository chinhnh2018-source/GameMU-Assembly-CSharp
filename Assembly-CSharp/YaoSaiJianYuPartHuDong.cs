using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class YaoSaiJianYuPartHuDong : UserControl
{
	public int roleID
	{
		set
		{
			this.AddItem(value, this.Level, this.ChangeLevel);
		}
	}

	private void InitTextInPrefabs()
	{
		string text = ConfigSystemParam.GetSystemParamIntArrayByName("ManorCommandAward", ',')[2].ToString();
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("派遣俘虏")
		});
		this.MiaoShu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format(Global.GetLang("俘虏劳动后需要休息{0}分钟"), text)
		});
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ManorWorkAward", '|');
		if (systemParamStringArrayByName != null && systemParamStringArrayByName.Length > 0 && systemParamStringArrayByName[0].Split(new char[]
		{
			','
		}) != null && systemParamStringArrayByName[0].Split(new char[]
		{
			','
		}).Length > 0)
		{
			int id = systemParamStringArrayByName[0].Split(new char[]
			{
				','
			})[0].SafeToInt32(0);
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
			this.MiaoShu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("劳动有一定几率获得【") + goodsXmlNodeByID.Title + string.Format(Global.GetLang("】，俘虏劳动后需要休息{0}分钟"), text)
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.OBCItem = this.Items.ItemsSource;
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public void AddItem(int roleID, int level, int changeLevel)
	{
		foreach (KeyValuePair<int, ManorCommandXml> keyValuePair in YaoSaiJianYuPart.GetDicManorCommand())
		{
			string[] array = keyValuePair.Value.Award.Split(new char[]
			{
				'|'
			});
			int num = (int)(Mathf.Pow((float)(Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level + changeLevel * 100 + level), 1.6f) * float.Parse(array[1]));
			if (num > 100)
			{
				num -= num % 100;
			}
			YaoSaiJianYuPartHuDongItem yaoSaiJianYuPartHuDongItem = U3DUtils.NEW<YaoSaiJianYuPartHuDongItem>();
			yaoSaiJianYuPartHuDongItem.RoleID = roleID;
			YaoSaiJianYuPartHuDongItem yaoSaiJianYuPartHuDongItem2 = yaoSaiJianYuPartHuDongItem;
			Dictionary<int, ManorCommandXml>.Enumerator enumerator;
			KeyValuePair<int, ManorCommandXml> keyValuePair2 = enumerator.Current;
			yaoSaiJianYuPartHuDongItem2.ID = keyValuePair2.Value.ID;
			yaoSaiJianYuPartHuDongItem.Type = array[0].SafeToInt32(0);
			yaoSaiJianYuPartHuDongItem.JiangliCount = num;
			yaoSaiJianYuPartHuDongItem.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHandler(this, new DPSelectedItemEventArgs());
			};
			this.OBCItem.AddNoUpdate(yaoSaiJianYuPartHuDongItem);
		}
	}

	public GButton BtnClose;

	public UILabel Title;

	public UILabel MiaoShu;

	public ListBox Items;

	public DPSelectedItemEventHandler CloseHandler;

	public int Level;

	public int ChangeLevel;

	private ObservableCollection OBCItem;
}
