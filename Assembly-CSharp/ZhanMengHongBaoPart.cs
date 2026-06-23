using System;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ZhanMengHongBaoPart : UserControl
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
		if (this.mBtnClose != null)
		{
			this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.CloseHandler != null)
				{
					this.CloseHandler(null, new DPSelectedItemEventArgs
					{
						IDType = 0
					});
				}
			};
		}
		if (null != this.mMyHongBao)
		{
			this.mMyHongBao.MyHongBaoTabTypeHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.refreshTime_cfg = this.fixedTime_cfg;
				this.mCurrentMyHongBaoType = (EMyHongBaoType)e.Type;
			};
		}
	}

	private new void Update()
	{
		this.CountDown();
	}

	private void CountDown()
	{
		if (this.mCurrentMyHongBaoType == EMyHongBaoType.None)
		{
			return;
		}
		if (this.mLastMyHongBaoType != EMyHongBaoType.None && this.mLastMyHongBaoType != this.mCurrentMyHongBaoType)
		{
			this.refreshTime_cfg = this.fixedTime_cfg;
			this.mLastMyHongBaoType = this.mCurrentMyHongBaoType;
		}
		this.refreshTime_cfg -= Time.deltaTime;
		if (this.refreshTime_cfg <= 0f)
		{
			switch (this.mCurrentMyHongBaoType)
			{
			case EMyHongBaoType.MyQiang:
				GameInstance.Game.SendMyHongBaoRequest(0);
				break;
			case EMyHongBaoType.MyFa:
				GameInstance.Game.SendMyHongBaoRequest(1);
				break;
			case EMyHongBaoType.ZongLan:
				GameInstance.Game.SendMyHongBaoRequest(2);
				break;
			}
			this.mLastMyHongBaoType = this.mCurrentMyHongBaoType;
			this.refreshTime_cfg = this.fixedTime_cfg;
		}
	}

	private void InitValue()
	{
		int num = ConvertExt.SafeConvertToInt32(ConfigSystemParam.GetSystemParamByName("RedPacketsRequest", true));
		this.refreshTime_cfg = (float)num;
		this.fixedTime_cfg = (float)num;
	}

	public void SetIsFirstOpenHongBaoWindow(bool isOpen = false)
	{
		this.mMyHongBao.mIsFirstOpenWindow = isOpen;
	}

	public void RefreshMyHongBaoStatus(int hongBaoId)
	{
		if (this.mMyHongBao != null)
		{
			if (this.mMyHongBao.mMyCurrentHongBaoType == EMyHongBaoType.MyQiang)
			{
				this.mMyHongBao.MyHongBaoSort(hongBaoId);
			}
			if (this.mMyHongBao.mMyCurrentHongBaoType == EMyHongBaoType.MyFa)
			{
				this.mMyHongBao.MyFaHongBaoSort(hongBaoId);
			}
			if (this.mMyHongBao.mMyCurrentHongBaoType == EMyHongBaoType.ZongLan)
			{
				this.mMyHongBao.MyZongLanHongBaoSort(hongBaoId);
			}
		}
	}

	public void RefreshHongBaoRecordPart()
	{
		this.mFaAndRankHongBao.InitUIDataByServerData();
	}

	public void NotifyFaHongBaoResult(int result)
	{
		if (null != this.mFaAndRankHongBao && null != this.mFaAndRankHongBao.mZMFaHongBao)
		{
			this.mFaAndRankHongBao.mZMFaHongBao.NotifyFaHongBaoResult(result);
			if (result >= 0 && this.mMyHongBao != null)
			{
				if (this.mMyHongBao.mMyCurrentHongBaoType == EMyHongBaoType.MyQiang)
				{
					this.mMyHongBao.MyHongBaoSort(0);
				}
				if (this.mMyHongBao.mMyCurrentHongBaoType == EMyHongBaoType.MyFa)
				{
					this.mMyHongBao.MyFaHongBaoSort(0);
				}
				if (this.mMyHongBao.mMyCurrentHongBaoType == EMyHongBaoType.ZongLan)
				{
					this.mMyHongBao.MyZongLanHongBaoSort(0);
				}
			}
		}
	}

	public void NotifyHongBaoRankResult(HongBaoRankData data, bool isRefresh)
	{
		if (null != this.mFaAndRankHongBao)
		{
			this.mFaAndRankHongBao.InitHongBaoRankDataByServerData(data, isRefresh);
		}
	}

	public void NotifyMyHongBaoResult(MyHongBaoData data, bool isRefresh)
	{
		if (null != this.mMyHongBao)
		{
			this.mMyHongBao.InitUIDataByServerData(data, isRefresh);
		}
	}

	protected override void OnDestroy()
	{
		this.CloseHandler = null;
		this.mBtnClose = null;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public ZhanMengFaAndRankHongBaoPart mFaAndRankHongBao;

	public ZhanMengMyHongBaoPart mMyHongBao;

	private EMyHongBaoType mCurrentMyHongBaoType;

	private EMyHongBaoType mLastMyHongBaoType;

	private float refreshTime_cfg = 5f;

	private float fixedTime_cfg = 5f;
}
