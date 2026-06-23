using System;
using System.Text;
using GameDBServer.Data;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	public class ZhanMengShiJianCmdProcessor : ICmdProcessor
	{
		private ZhanMengShiJianCmdProcessor()
		{
		}

		public static ZhanMengShiJianCmdProcessor getInstance()
		{
			return ZhanMengShiJianCmdProcessor.instance;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
			string[] array = @string.Split(new char[]
			{
				':'
			});
			ZhanMengShiJianData zhanMengShiJianData = new ZhanMengShiJianData();
			zhanMengShiJianData.BHID = Convert.ToInt32(array[0]);
			zhanMengShiJianData.RoleName = Convert.ToString(array[1]);
			zhanMengShiJianData.ShiJianType = Convert.ToInt32(array[2]);
			zhanMengShiJianData.SubValue1 = Convert.ToInt32(array[3]);
			zhanMengShiJianData.SubValue2 = Convert.ToInt32(array[4]);
			zhanMengShiJianData.SubValue3 = Convert.ToInt32(array[5]);
			zhanMengShiJianData.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			if (zhanMengShiJianData.ShiJianType == ZhanMengShiJianConstants.ChangeZhiWu)
			{
				string roleName;
				string text;
				Global.GetRoleNameAndUserID(DBManager.getInstance(), zhanMengShiJianData.SubValue3, out roleName, out text);
				zhanMengShiJianData.RoleName = roleName;
			}
			ZhanMengShiJianManager.getInstance().onAddZhanMengShiJian(zhanMengShiJianData);
			byte[] cmdData = DataHelper.ObjectToBytes<string>(string.Format("{0}", 1));
			client.sendCmd<byte[]>(10138, cmdData);
		}

		private static ZhanMengShiJianCmdProcessor instance = new ZhanMengShiJianCmdProcessor();
	}
}
