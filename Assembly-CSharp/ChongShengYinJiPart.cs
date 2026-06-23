using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChongShengYinJiPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.IsChangeYiJi)
			{
				this.IsChangeYiJi = false;
				this.InitValue();
				return;
			}
			if (this.CloseHandler != null)
			{
				this.CloseSelectPart();
				this.CloseShowPart();
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
	}

	private void InitValue()
	{
		this.mIsFirstOpen = true;
		GameInstance.Game.RequestYinJiInfo();
	}

	public void RespondMainInfo(RebornStampData data)
	{
		if (this.mIsFirstOpen && !this.mIsResetYinJi)
		{
			this.mIsFirstOpen = false;
			if (data != null && data.StampInfo != null && data.StampInfo.Count > 0)
			{
				this.CloseSelectPart();
				this.OpenShowPart(data);
			}
			else
			{
				this.OpenSlecetPart(data);
			}
		}
		else if (this.mIsResetYinJi)
		{
			this.mIsResetYinJi = false;
			this.OpenSlecetPart(data);
		}
		else
		{
			this.CloseSelectPart();
			this.OpenShowPart(data);
		}
	}

	public void RespondRestAll(int result)
	{
		if (this.mChongShengYinJiSelectPart != null)
		{
			if (result == 1)
			{
				this.IsChangeYiJi = false;
			}
			this.mChongShengYinJiSelectPart.RespondResetAll(result);
		}
	}

	public void RespondLevelUp(string[] fields)
	{
		if (this.mChongShengYinJiShowPart != null)
		{
			this.mChongShengYinJiShowPart.RespondLevelUp(fields);
		}
	}

	public void RespondChoose(int ret)
	{
		if (ret == 1)
		{
		}
		if (ret == 6)
		{
			GameInstance.Game.RequestYinJiInfo();
		}
		else
		{
			Super.HintMainText(Global.GetLang(ChongShengYinJiData.GetErrorMsg(ret)), 10, 3);
		}
	}

	private void OpenSlecetPart(RebornStampData data)
	{
		if (this.mChongShengYinJiSelectPart == null)
		{
			this.mChongShengYinJiSelectPart = U3DUtils.NEW<ChongShengYinJiSelectPart>();
			this.mChongShengYinJiSelectPart.transform.SetParent(this.mParentTrsfm);
			this.mChongShengYinJiSelectPart.transform.localPosition = this.mParentTrsfm.localPosition;
			this.mChongShengYinJiSelectPart.transform.localRotation = Quaternion.identity;
			this.mChongShengYinJiSelectPart.transform.localScale = Vector3.one;
			this.mChongShengYinJiSelectPart.ChangeHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseSelectPart();
			};
		}
		this.mChongShengYinJiSelectPart.InitValue(data);
		NGUITools.SetActive(this.mBottom, false);
	}

	private void CloseSelectPart()
	{
		if (this.mChongShengYinJiSelectPart != null)
		{
			this.mChongShengYinJiSelectPart.DestroySelf();
			this.mChongShengYinJiSelectPart = null;
		}
	}

	private void OpenShowPart(RebornStampData data)
	{
		if (this.mChongShengYinJiShowPart == null)
		{
			this.mChongShengYinJiShowPart = U3DUtils.NEW<ChongShengYinJiShowPart>();
			this.mChongShengYinJiShowPart.transform.SetParent(this.mParentTrsfm);
			this.mChongShengYinJiShowPart.transform.localPosition = this.mParentTrsfm.localPosition;
			this.mChongShengYinJiShowPart.transform.localRotation = Quaternion.identity;
			this.mChongShengYinJiShowPart.transform.localScale = Vector3.one;
			this.mChongShengYinJiShowPart.ChangeHandler = new DPSelectedItemEventHandler(this.OnChangeHandler);
		}
		this.mChongShengYinJiShowPart.InitValue(data);
		NGUITools.SetActive(this.mBottom, true);
	}

	private void CloseShowPart()
	{
		if (this.mChongShengYinJiShowPart != null)
		{
			this.mChongShengYinJiShowPart.DestroySelf();
			this.mChongShengYinJiShowPart = null;
		}
	}

	private void OnChangeHandler(object sender, DPSelectedItemEventArgs args)
	{
		this.CloseShowPart();
		this.mIsResetYinJi = true;
		this.IsChangeYiJi = true;
		this.InitValue();
	}

	protected override void OnDestroy()
	{
		ChongShengYinJiData.ClearAll();
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public TextBlock mLblTitle;

	public GameObject mBottom;

	public Transform mParentTrsfm;

	private EChongShengType mCurrentType;

	private bool mIsFirstOpen;

	private bool mIsResetYinJi;

	private ChongShengYinJiSelectPart mChongShengYinJiSelectPart;

	private ChongShengYinJiShowPart mChongShengYinJiShowPart;

	private bool IsChangeYiJi;
}
