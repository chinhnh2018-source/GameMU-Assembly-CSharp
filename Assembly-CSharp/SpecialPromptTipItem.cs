using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class SpecialPromptTipItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		base.transform.localPosition = new Vector3(600f, 50f, 0f);
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
			UIEventListener.Get(base.gameObject).onDrag = delegate(GameObject g, Vector2 v)
			{
				if (20f <= v.x)
				{
					TweenPosition.Begin(base.gameObject, 0.1f, new Vector3(545f, 40f, 0f));
				}
				else if (-20f >= v.x)
				{
					TweenPosition.Begin(base.gameObject, 0.1f, new Vector3(360f, 40f, 0f));
				}
			};
			UIEventListener.Get(base.gameObject).onClick = delegate(GameObject g)
			{
				if (Mathf.Approximately(base.transform.localPosition.x, 545f))
				{
					TweenPosition.Begin(base.gameObject, 0.1f, new Vector3(360f, 40f, 0f));
				}
				else
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = this.mSpecialPromptVO.List
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

	private string ParseDateTimeToChanese(int seconds)
	{
		int[] array = new int[]
		{
			default(int),
			default(int),
			default(int),
			seconds % 60
		};
		array[2] = (seconds - array[3]) / 60 % 60;
		array[1] = (seconds - array[2] * 60 - array[3]) / 3600 % 24;
		array[0] = (seconds - array[1] * 3600 - array[2] * 60 - array[3]) / 86400;
		string text = string.Empty;
		byte b = 0;
		string[] array2 = new string[]
		{
			Global.GetLang("天"),
			Global.GetLang("小时"),
			Global.GetLang("分钟"),
			Global.GetLang("秒")
		};
		byte b2 = 0;
		while ((int)b2 < array.Length)
		{
			if (array[(int)b2] > 0)
			{
				text = text + array[(int)b2].ToString() + array2[(int)b2];
				b = 1;
			}
			else if (b == 1)
			{
				text = text + array[(int)b2].ToString() + array2[(int)b2];
			}
			b2 += 1;
		}
		return text;
	}

	public void UpDate()
	{
		if (this.mSpecialPromptVO != null)
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			this.mInfLabel.text = this.mSpecialPromptVO.Intro;
			DateTime dateTime;
			dateTime..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, this.mSpecialPromptVO.CountDownTime.time1[0], this.mSpecialPromptVO.CountDownTime.time1[1], this.mSpecialPromptVO.CountDownTime.time1[2]);
			if (dateTime <= correctDateTime)
			{
				this.HideItem();
			}
			else
			{
				UILabel uilabel = this.mInfLabel;
				uilabel.text += Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetTimeStrBySecEx((dateTime - correctDateTime).TotalSeconds, true, -1)
				});
			}
		}
	}

	public void RefreshInf(SpecialPromptVO vo)
	{
		if (vo != null)
		{
			this.mSpecialPromptVO = vo;
			if (vo.ThisTimesIsInActivity && this.m_ShowItem == 0)
			{
				this.mIconImage.URL = "NetImages/GameRes/Images/Hybrid/" + vo.Icon;
				this.mInfLabel.text = vo.Intro;
				TweenPosition.Begin(base.gameObject, 0.1f, new Vector3(360f, 40f, 0f));
				this.m_ShowItem = 1;
			}
		}
	}

	public void HideItem()
	{
		TweenPosition.Begin(base.gameObject, 0.01f, new Vector3(600f, 50f, 0f));
		this.m_ShowItem = 0;
	}

	public int SpecialPromptID
	{
		get
		{
			if (this.mSpecialPromptVO == null)
			{
				return -1;
			}
			return this.mSpecialPromptVO.ID;
		}
	}

	public bool ShowItem
	{
		get
		{
			return this.m_ShowItem == 1;
		}
	}

	[SerializeField]
	private ShowNetImage mIconImage;

	[SerializeField]
	private UILabel mInfLabel;

	private SpecialPromptVO mSpecialPromptVO;

	private byte m_ShowItem;
}
