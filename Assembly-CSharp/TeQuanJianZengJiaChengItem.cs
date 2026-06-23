using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TeQuanJianZengJiaChengItem : UserControl
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
			this._BtnGo.transform.FindChild("UILabel").GetComponent<UILabel>().text = Global.GetLang("立即前往");
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
			UIEventListener.Get(this._BtnGo.gameObject).onClick = delegate(GameObject g)
			{
				if (this.mTeQuanBuffVO != null)
				{
					switch (this.mTeQuanBuffVO.HuoDongLeiXing)
					{
					case 3:
						PlayZone.GlobalPlayZone.ShowQiFuWindow(false);
						break;
					case 4:
						PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
						{
							ID = 107
						});
						break;
					case 5:
						PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
						{
							ID = 106
						});
						break;
					case 6:
						PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
						{
							ID = 105
						});
						break;
					case 7:
						PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
						{
							ID = 1000
						});
						break;
					case 9:
					case 10:
						PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
						{
							ID = 1320
						});
						break;
					case 12:
						(Super.GData.GlobalPlayZone as PlayZone).ShowSyntesizeWindow(false, 201);
						break;
					}
				}
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0,
						ID = this.mID
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

	private bool GetBtnActive()
	{
		switch (this.mTeQuanBuffVO.HuoDongLeiXing)
		{
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
			return true;
		case 9:
		case 10:
			return true;
		case 12:
			return true;
		}
		return false;
	}

	private string GetContent(TeQuanBuffVO vo)
	{
		string result = string.Empty;
		MUDebug.Log<int>(new int[]
		{
			vo.HuoDongLeiXing
		});
		switch (vo.HuoDongLeiXing)
		{
		case 1:
			result = Global.GetLang("击杀怪物经验提高") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"+" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("倍");
			break;
		case 3:
			result = Global.GetLang("祈福积分") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"+" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("倍");
			break;
		case 4:
			result = Global.GetLang("天使神殿活动奖励") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"X" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("倍");
			break;
		case 5:
			result = Global.GetLang("阵营战活动奖励") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"X" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("倍");
			break;
		case 6:
			result = Global.GetLang("PK之王活动奖励") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"X" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("倍");
			break;
		case 7:
			result = Global.GetLang("古战场每日进入时间") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"+" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("分钟");
			break;
		case 8:
			result = Global.GetLang("组队副本通关奖励") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"X" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("倍");
			break;
		case 9:
			result = Global.GetLang("转换次数") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"+" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("次");
			break;
		case 10:
			result = Global.GetLang("转换获得奖励") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"X" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("倍");
			break;
		case 11:
			result = Global.GetLang("雕像膜拜") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"+" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("倍");
			break;
		case 12:
			result = Global.GetLang("合成果实成功率增加") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"+" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("倍");
			break;
		case 13:
			result = Global.GetLang("要塞Boss奖励") + Global.GetColorStringForNGUIText(new object[]
			{
				"ff9966",
				"X" + vo.KaiQiBeiShu.ToString("f1")
			}) + Global.GetLang("倍");
			break;
		}
		return result;
	}

	public void SetData(TeQuanBuffVO vo)
	{
		if (vo != null)
		{
			this.mTeQuanBuffVO = vo;
			this._Label.text = this.GetContent(vo);
			this.mID = vo.ID;
			this._BtnGo.gameObject.SetActive(this.GetBtnActive());
			if (string.IsNullOrEmpty(this._Label.text))
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	public int ID
	{
		get
		{
			return this.mID;
		}
		set
		{
			this.mID = value;
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private UILabel _Label;

	[SerializeField]
	private UIButton _BtnGo;

	private int mID;

	private TeQuanBuffVO mTeQuanBuffVO;
}
