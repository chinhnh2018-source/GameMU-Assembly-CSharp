using System;
using System.Text;
using GameDBServer.Logic.BaiTan;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	public class BaiTanLogAddCmdProcessor : ICmdProcessor
	{
		private BaiTanLogAddCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(10150, this);
		}

		public static BaiTanLogAddCmdProcessor getInstance()
		{
			return BaiTanLogAddCmdProcessor.instance;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(cmdParams, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd(30767, "0");
				return;
			}
			string[] array = text.Split(new char[]
			{
				':'
			});
			if (array.Length != 13)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int rid = Convert.ToInt32(array[0]);
				int otherRoleID = Convert.ToInt32(array[1]);
				string otherRName = array[2];
				int goodsID = Convert.ToInt32(array[3]);
				int goodsNum = Convert.ToInt32(array[4]);
				int forgeLevel = Convert.ToInt32(array[5]);
				int totalPrice = Convert.ToInt32(array[6]);
				int leftYuanBao = Convert.ToInt32(array[7]);
				int yinLiang = Convert.ToInt32(array[8]);
				int leftYinLiang = Convert.ToInt32(array[9]);
				int tax = Convert.ToInt32(array[10]);
				int excellenceinfo = Convert.ToInt32(array[11]);
				string washprops = array[12];
				BaiTanLogItemData baiTanLogItemData = new BaiTanLogItemData();
				baiTanLogItemData.rid = rid;
				baiTanLogItemData.OtherRoleID = otherRoleID;
				baiTanLogItemData.OtherRName = otherRName;
				baiTanLogItemData.GoodsID = goodsID;
				baiTanLogItemData.GoodsNum = goodsNum;
				baiTanLogItemData.ForgeLevel = forgeLevel;
				baiTanLogItemData.TotalPrice = totalPrice;
				baiTanLogItemData.LeftYuanBao = leftYuanBao;
				baiTanLogItemData.YinLiang = yinLiang;
				baiTanLogItemData.LeftYinLiang = leftYinLiang;
				baiTanLogItemData.BuyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				baiTanLogItemData.Tax = tax;
				baiTanLogItemData.Excellenceinfo = excellenceinfo;
				baiTanLogItemData.Washprops = washprops;
				BaiTanManager.getInstance().onAddBaiTanLog(baiTanLogItemData);
				client.sendCmd<string>(10150, string.Format("{0}", 0));
			}
		}

		private static BaiTanLogAddCmdProcessor instance = new BaiTanLogAddCmdProcessor();
	}
}
