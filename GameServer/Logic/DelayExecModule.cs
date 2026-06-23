using System;
using System.Collections;
using System.Threading;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class DelayExecModule
	{
		public void SetDelayExecProc(params DelayExecProcIds[] procIds)
		{
			if (procIds != null && procIds.Length != 0)
			{
				lock (this.mutex)
				{
					foreach (DelayExecProcIds index in procIds)
					{
						this.DelayExecPorcsBits.Set(index, true);
					}
				}
			}
		}

		public void ExecDelayProcs(GameClient client)
		{
			bool flag2;
			bool flag3;
			bool flag4;
			bool flag5;
			lock (this.mutex)
			{
				flag2 = this.DelayExecPorcsBits.Get(0);
				flag3 = this.DelayExecPorcsBits.Get(1);
				flag4 = this.DelayExecPorcsBits.Get(2);
				flag5 = this.DelayExecPorcsBits.Get(3);
				this.DelayExecPorcsBits.SetAll(false);
			}
			if (flag2)
			{
				this.RecalcProps(client);
			}
			if (flag3)
			{
				this.UpdateOtherProps(client);
			}
			if (flag4 || flag2)
			{
				this.NotifyRefreshProps(client);
			}
			if (flag5)
			{
				GameManager.GoodsPackMgr.ProcessClickGoodsPackWhenMovingToOtherGrid(client, 1);
			}
		}

		private void RecalcProps(GameClient client)
		{
			Global.RefreshEquipProp(client);
		}

		private void UpdateOtherProps(GameClient client)
		{
			Global.UpdateHorseDataProps(client, true);
			Global.UpdateJingMaiListProps(client, true);
		}

		private void NotifyRefreshProps(GameClient client)
		{
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
		}

		private static void ProcessClickGoodsPack(GameClient client)
		{
			GameManager.GoodsPackMgr.ProcessClickGoodsPackWhenMovingToOtherGrid(client, 1);
		}

		public static void DelayClose(GameClient client, string msg, string log, int minutes)
		{
			BanManager.BanRoleName(Global.FormatRoleName(client, client.ClientData.RoleName), minutes, 1);
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
			ThreadPool.QueueUserWorkItem(delegate(object x)
			{
				Thread.Sleep(Global.GetRandomNumber(900, 5500));
				Global.ForceCloseClient(client, msg, true);
			});
		}

		private object mutex = new object();

		public BitArray DelayExecPorcsBits = new BitArray(4);
	}
}
