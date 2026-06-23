using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class KuafuPlunderRulePart : UserControl
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
			this.mTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("规则详情")
			});
			this.mInfLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("点击查看规则")
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
			this.mPluderRuleImage.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/PeiTu_TaoFaGuiZe.jpg";
			this.mPluderRuleImage.ImageDownloaded = delegate(object g)
			{
				this.mPluderRuleImage.transform.localScale = new Vector3((float)this.mPluderRuleImage.ItsSizeWidth, (float)this.mPluderRuleImage.ItsSizeHeight, 0f);
			};
			this.mBattlegroundImage.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/PeiTu_ZhanChangGuiZe.jpg";
			this.mBattlegroundImage.ImageDownloaded = delegate(object g)
			{
				this.mBattlegroundImage.transform.localScale = new Vector3((float)this.mBattlegroundImage.ItsSizeWidth, (float)this.mBattlegroundImage.ItsSizeHeight, 0f);
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
			UIEventListener.Get(this.mPluderRuleImage.gameObject).onClick = delegate(GameObject g)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						ID = 1,
						Type = 0
					});
				}
			};
			UIEventListener.Get(this.mBattlegroundImage.gameObject).onClick = delegate(GameObject g)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						ID = 2,
						Type = 0
					});
				}
			};
			this.mColseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						ID = 0,
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

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private ShowNetImage mPluderRuleImage;

	[SerializeField]
	private ShowNetImage mBattlegroundImage;

	[SerializeField]
	private UILabel mTitleLabel;

	[SerializeField]
	private UILabel mInfLabel;

	[SerializeField]
	private GButton mColseBtn;
}
