using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class CommonZhanBaoPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.mListBox.Items;
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
	}

	private GameObject GetItem(ZhanDuiZhengBaPkLogData data)
	{
		if (data == null)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.zhanBaoItem);
		gameObject.SetActive(true);
		TextBlock component = gameObject.transform.FindChild("Label").GetComponent<TextBlock>();
		if (component != null)
		{
			string text = string.Empty;
			int pkResult = data.PkResult;
			if (pkResult != 1)
			{
				if (pkResult == 2)
				{
					text = Global.GetString(new object[]
					{
						data.ZhanDuiName1,
						this.GetFailureDes(data.Day, data.ZhanDuiName2)
					});
				}
			}
			else
			{
				text = Global.GetString(new object[]
				{
					data.ZhanDuiName1,
					this.GetSuccessDes(data.Day, data.UpGrade, data.ZhanDuiName2)
				});
			}
			component.Text = text;
			if (Global.Data.roleData != null && (Global.Data.roleData.ZhanDuiID == data.ZhanDuiID1 || Global.Data.roleData.ZhanDuiID == data.ZhanDuiID2))
			{
				component.Label.color = Color.yellow;
			}
		}
		return gameObject;
	}

	private string GetSuccessDes(int grade, bool isSuccess, string teanName)
	{
		if (grade == 1)
		{
			return (!isSuccess) ? (Global.GetLang("战队在64进32的比赛中战胜") + teanName) : (Global.GetLang("战队在64进32的比赛中战胜") + teanName + Global.GetLang("，成功晋级32强"));
		}
		if (grade == 2)
		{
			return (!isSuccess) ? (Global.GetLang("战队在32进16的比赛中战胜") + teanName) : (Global.GetLang("战队在32进16的比赛中战胜") + teanName + Global.GetLang("，成功晋级16强"));
		}
		if (grade == 3)
		{
			return (!isSuccess) ? (Global.GetLang("战队在16进8的比赛中战胜") + teanName) : (Global.GetLang("战队在16进8的比赛中战胜") + teanName + Global.GetLang("，成功晋级8强"));
		}
		if (grade == 4)
		{
			return (!isSuccess) ? (Global.GetLang("战队在8进4的比赛中战胜") + teanName) : (Global.GetLang("战队在8进4的比赛中战胜") + teanName + Global.GetLang("，成功晋级4强"));
		}
		if (grade == 5)
		{
			return (!isSuccess) ? (Global.GetLang("战队在4进半决赛的比赛中战胜") + teanName) : (Global.GetLang("战队在4进半决赛的比赛中战胜") + teanName + Global.GetLang("，成功晋级半决赛"));
		}
		if (grade == 6)
		{
			return (!isSuccess) ? (Global.GetLang("战队在决赛中战胜") + teanName) : (Global.GetLang("战队在决赛中战胜") + teanName + Global.GetLang("，成功夺得荣耀之王"));
		}
		if (grade == 7)
		{
			return null;
		}
		return null;
	}

	private string GetFailureDes(int grade, string teanName)
	{
		if (grade == 1)
		{
			return Global.GetLang("战队在64进32的比赛中败给") + teanName;
		}
		if (grade == 2)
		{
			return Global.GetLang("战队在32进16的比赛中败给") + teanName;
		}
		if (grade == 3)
		{
			return Global.GetLang("战队在16进8的比赛中败给") + teanName;
		}
		if (grade == 4)
		{
			return Global.GetLang("战队在8进4的比赛中败给") + teanName;
		}
		if (grade == 5)
		{
			return Global.GetLang("战队在4进半决赛的比赛中败给") + teanName;
		}
		if (grade == 6)
		{
			return Global.GetLang("战队在决赛中败给") + teanName;
		}
		if (grade == 7)
		{
			return null;
		}
		return null;
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	public void InitValue(List<ZhanDuiZhengBaPkLogData> dataList)
	{
		if (dataList != null && dataList.Count > 0)
		{
			dataList.Reverse();
		}
		for (int i = 0; i < dataList.Count; i++)
		{
			this.ItemCollection.Add(this.GetItem(dataList[i]));
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public GButton BtnClose;

	public GameObject zhanBaoItem;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	private string green = "00FF00";

	private string red = "FF0000";

	private string normal = "DEC69C";

	private string yellow = "F9F702";
}
