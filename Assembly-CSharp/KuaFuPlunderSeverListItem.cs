using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class KuaFuPlunderSeverListItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
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
			BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
			boxCollider.size = new Vector3(910f, 40f, 0f);
			UIEventListener.Get(base.gameObject).onPress = delegate(GameObject g, bool s)
			{
				if (s)
				{
					this.BSlelect = s;
					if (this.Hander != null)
					{
						this.Hander(this, new DPSelectedItemEventArgs
						{
							ID = base.name.SafeToInt32(0)
						});
					}
				}
			};
			this.BSlelect = false;
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public void SetInf(int haveOffer, int SeverID, int state, int Round, int NowResource, bool Relation, bool Conquer)
	{
		this.mServerID = SeverID;
		if (haveOffer == 0)
		{
			this.mSignSp.gameObject.SetActive(false);
		}
		else
		{
			this.mSignSp.gameObject.SetActive(true);
			this.mSignSp.spriteName = ((haveOffer != 1) ? "YiHuoDeZiGe" : "YiChuJia");
			this.mSignSp.Update();
		}
		ZtBuffServerInfo ztBuffServerInfo = null;
		if (Global.GetNowServerIsZhuTiFu(SeverID, out ztBuffServerInfo))
		{
			this.mSeverNameLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				ztBuffServerInfo.strServerName
			});
		}
		else
		{
			this.mSeverNameLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				"S." + SeverID
			});
		}
		if (state == 0)
		{
			this.mResultLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				this.GetRoundString(Round) + Global.GetLang("尚未开始")
			});
		}
		else if (state == 1)
		{
			this.mResultLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				this.GetRoundString(Round) + Global.GetLang("正在竞价")
			});
		}
		else
		{
			this.mResultLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("已公布")
			});
		}
		this.mNowResourceLabel.text = NowResource.ToString();
		bool flag = false;
		if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData != null && SeverID == KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData.ServerID)
		{
			flag = true;
		}
		if (flag)
		{
			this.mRelationLabel.text = Global.GetLang("本服");
			this.mConquerLabel.text = Global.GetLang("本服");
		}
		else
		{
			this.mRelationLabel.text = (Relation ? Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("世仇")
			}) : Global.GetLang("无"));
			this.mConquerLabel.text = (Conquer ? Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("已征服")
			}) : Global.GetLang("尚未征服"));
		}
	}

	private string GetRoundString(int round)
	{
		if (round == 1)
		{
			return Global.GetLang("第一轮");
		}
		if (round == 2)
		{
			return Global.GetLang("第二轮");
		}
		if (round == 3)
		{
			return Global.GetLang("第三轮");
		}
		if (round == 4)
		{
			return Global.GetLang("第四轮");
		}
		return Global.GetLang("尚未开始");
	}

	public int ServerID
	{
		get
		{
			return this.mServerID;
		}
	}

	public bool BSlelect
	{
		get
		{
			return this.mSelectEffect.activeSelf;
		}
		set
		{
			this.mSelectEffect.SetActive(value);
		}
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			if (null == base.GetComponent<UIDragPanelContents>())
			{
				base.gameObject.AddComponent<UIDragPanelContents>();
			}
			base.GetComponent<UIDragPanelContents>().draggablePanel = value;
			if (null != base.GetComponent<UIPanel>())
			{
				Object.Destroy(base.GetComponent<UIPanel>());
			}
		}
	}

	[SerializeField]
	private GameObject mSelectEffect;

	[SerializeField]
	private UISprite mSignSp;

	[SerializeField]
	private UILabel mSeverNameLabel;

	[SerializeField]
	private UILabel mResultLabel;

	[SerializeField]
	private UILabel mNowResourceLabel;

	[SerializeField]
	private UILabel mRelationLabel;

	[SerializeField]
	private UILabel mConquerLabel;

	[SerializeField]
	private int mServerID;

	public DPSelectedItemEventHandler Hander;
}
