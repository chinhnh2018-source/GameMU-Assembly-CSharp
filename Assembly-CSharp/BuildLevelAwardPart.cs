using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Tmsk.Xml;
using UnityEngine;

public class BuildLevelAwardPart : UserControl
{
	public bool HaveAwardCanGet()
	{
		for (int i = 0; i < this.AwardPropList.Length; i++)
		{
			if (this.AwardPropList[i].GiftGainState == BuildLevelAwardItem.BuildLevelAwardState.CanGain)
			{
				return true;
			}
		}
		return false;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this._TitleLabel)
		{
			this._TitleLabel.text = BuildFintColor.Yellow + Global.GetLang("建筑等级奖励") + BuildFintColor.End;
		}
		this._Mask.alpha = 0.7f;
		if (null != this._ListBox)
		{
			this._ItemCollection = this._ListBox.ItemsSource;
		}
		base.StartCoroutine<bool>(this.InitData());
	}

	protected IEnumerator InitData()
	{
		byte counter = 0;
		XElement xml = Global.GetGameResXml("Config/Manor/BuildLevelAward.xml");
		if (this._ListBox.transform.localPosition.x != -228f)
		{
			this._ListBox.transform.localPosition = new Vector3(-235f, 158f, -0.1f);
		}
		if (xml != null)
		{
			List<XElement> argsList = Global.GetXElementList(xml, "BuildLevelAward");
			this.AwardPropList = new BuildLevelAwardItem[argsList.Count];
			for (int i = 0; i < argsList.Count; i++)
			{
				int tiaojian = Global.GetXElementAttributeInt(argsList[i], "AllLevel");
				int id = Global.GetXElementAttributeInt(argsList[i], "ID");
				BuildLevelAwardItem item = U3DUtils.NEW<BuildLevelAwardItem>();
				item.transform.localPosition = new Vector3(0f, 0f, -0.1f);
				item.AwardId = id;
				item.TiaoJianLev = tiaojian;
				this._ItemCollection.Add(item);
				this.AwardPropList[i] = item;
				this.AwardPropList[i].GiftGainState = BuildLevelAwardItem.BuildLevelAwardState.CanNotGain;
				item.LoadGoodsList(this.GetGoodsStringArray(Global.GetXElementAttributeStrArray(argsList[i], "Award", "*", ',')).ToArray(), tiaojian.ToString(), i, this._DraggablePanel);
				if (null != this._ScrollBar)
				{
					this._ScrollBar.scrollValue = 0f;
				}
				if (i == argsList.Count - 1)
				{
					GameInstance.Game.SendGetBuildLevelAwardState();
				}
				byte b;
				counter = (b = counter) + 1;
				if (b % 6 == 0)
				{
					yield return null;
				}
			}
		}
		yield break;
	}

	public void UpdataAwardState(List<int> dataList, int AllBuildLev)
	{
		byte b = 0;
		while ((int)b < this.AwardPropList.Length)
		{
			if (AllBuildLev < this.AwardPropList[(int)b].TiaoJianLev)
			{
				this.AwardPropList[(int)b].GiftGainState = BuildLevelAwardItem.BuildLevelAwardState.CanNotGain;
			}
			else if (AllBuildLev >= this.AwardPropList[(int)b].TiaoJianLev)
			{
				this.AwardPropList[(int)b].GiftGainState = BuildLevelAwardItem.BuildLevelAwardState.CanGain;
			}
			b += 1;
		}
		byte b2 = 0;
		IL_B7:
		while ((int)b2 < dataList.Count)
		{
			for (int i = 0; i < this.AwardPropList.Length; i++)
			{
				if (dataList[(int)b2] == this.AwardPropList[i].AwardId)
				{
					this.AwardPropList[i].GiftGainState = BuildLevelAwardItem.BuildLevelAwardState.Gained;
					IL_B2:
					b2 += 1;
					goto IL_B7;
				}
			}
			goto IL_B2;
		}
	}

	public void UpDateGetAwardState(int AwardId)
	{
		for (int i = 0; i < this.AwardPropList.Length; i++)
		{
			if (AwardId == this.AwardPropList[i].AwardId)
			{
				this.AwardPropList[i].GiftGainState = BuildLevelAwardItem.BuildLevelAwardState.Gained;
				return;
			}
		}
	}

	private List<string> GetGoodsStringArray(string[] str)
	{
		List<string> list = new List<string>();
		byte b = 0;
		while ((int)b < str.Length)
		{
			if (str[(int)b].Length > 1)
			{
				string text = string.Empty;
				byte b2 = 0;
				while ((int)b2 < str[(int)b].Length)
				{
					if (str[(int)b].get_Chars((int)b2).ToString() == "\0")
					{
						break;
					}
					if (str[(int)b].get_Chars((int)b2).ToString() == "|")
					{
						list.Add(text);
						text = string.Empty;
					}
					else
					{
						text += str[(int)b].get_Chars((int)b2);
					}
					b2 += 1;
				}
				list.Add(text);
			}
			else
			{
				list.Add(str[(int)b]);
			}
			b += 1;
		}
		return list;
	}

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

	public UILabel _TitleLabel;

	public UISprite _Mask;

	public GButton _BtnClose;

	public ListBox _ListBox;

	public UIScrollBar _ScrollBar;

	public UIDraggablePanel _DraggablePanel;

	private BuildLevelAwardItem[] AwardPropList;

	private ObservableCollection _ItemCollection;
}
