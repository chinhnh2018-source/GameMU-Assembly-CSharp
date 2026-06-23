using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class SpecPlatFuLiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static SpecPlatFuLiManager getInstance()
		{
			return SpecPlatFuLiManager.instance;
		}

		public bool initialize()
		{
			this.evQueryHandlerDict[SpecialPlatformType.UC.ToString()] = new SpecPlatFuLiManager.SpecHandler(this._Query_Handle_UC);
			this.evExcuteHandlerDict[SpecialPlatformType.UC.ToString()] = new SpecPlatFuLiManager.SpecHandler(this._Excute_Handle_UC);
			return this.InitConfig();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(692, 3, 3, SpecPlatFuLiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(693, 3, 3, SpecPlatFuLiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 692:
				result = this.ProcessQueryCmd(client, nID, bytes, cmdParams);
				break;
			case 693:
				result = this.ProcessExcuteCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		public bool ProcessQueryCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (client == null || client.ClientSocket.IsKuaFuLogin)
				{
					return true;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				string text = cmdParams[1];
				string text2 = cmdParams[2];
				string arg = "0";
				SpecPlatFuLiManager.SpecHandler specHandler;
				if (this.evQueryHandlerDict.TryGetValue(text.ToUpper(), out specHandler))
				{
					arg = specHandler(client, text2);
				}
				client.ClientData.platType = text;
				client.ClientData.launch = text2;
				client.sendCmd(nID, string.Format("{0}", arg), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessExcuteCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (client == null || client.ClientSocket.IsKuaFuLogin)
				{
					return true;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				string text = cmdParams[1];
				string text2 = cmdParams[2];
				string arg = "";
				SpecPlatFuLiManager.SpecHandler specHandler;
				if (this.evExcuteHandlerDict.TryGetValue(text.ToUpper(), out specHandler))
				{
					arg = specHandler(client, text2);
				}
				client.sendCmd(nID, string.Format("{0}:{1}", arg, text2), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public void OnNewDay(GameClient client)
		{
			if (client != null && !client.ClientSocket.IsKuaFuLogin)
			{
				if (string.Compare(client.ClientData.platType, SpecialPlatformType.UC.ToString(), true) == 0)
				{
					client.sendCmd(692, string.Format("{0}", 0), false);
				}
			}
		}

		private string _Query_Handle_UC(GameClient client, string param)
		{
			string text = "0";
			string result;
			if (!this.AliGiftsSwitch)
			{
				result = text;
			}
			else
			{
				int num = Global.SafeConvertToInt32(param);
				DateTime dateTime = TimeUtil.NowDateTime();
				DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
				DateTime dateTime3 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
				string arg = dateTime2.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string arg2 = dateTime3.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string keyStr = string.Format("fuli_{0}_{1}_{2}", arg, arg2, SpecialPlatformType.UC.ToString());
				string keyStr2 = string.Format("login_{0}_{1}_{2}", arg, arg2, SpecialPlatformType.UC.ToString());
				string[] array = Global.QeuryUserActivityInfo(client, keyStr, 1000, "0");
				if (array == null || array.Length == 0)
				{
					result = text;
				}
				else
				{
					int num2 = Global.SafeConvertToInt32(array[3]);
					if (num2 > 0)
					{
						result = text;
					}
					else
					{
						if (num <= 0)
						{
							array = Global.QeuryUserActivityInfo(client, keyStr2, 1000, "0");
							if (array == null || array.Length == 0)
							{
								return text;
							}
							num = Global.SafeConvertToInt32(array[3]);
							if (num <= 0)
							{
								return text;
							}
						}
						else
						{
							Global.UpdateUserActivityInfo(client, keyStr2, 1000, (long)num, dateTime.ToString("yyyy-MM-dd HH$mm$ss"));
						}
						text = "1";
						result = text;
					}
				}
			}
			return result;
		}

		private string _Excute_Handle_UC(GameClient client, string param)
		{
			int num = 0;
			PlatFuLiBaseData platFuLiBaseData;
			lock (this.ConfigMutex)
			{
				this.PlatFuLiBaseDataDict.TryGetValue(SpecialPlatformType.UC.ToString(), out platFuLiBaseData);
			}
			string result;
			if (platFuLiBaseData == null || !this.AliGiftsSwitch)
			{
				num = -3;
				result = string.Format("{0}", num);
			}
			else if (!Global.CanAddGoodsDataList(client, ((UCPlatFuLiData)platFuLiBaseData).myAwardItem.GoodsDataList))
			{
				num = -100;
				result = string.Format("{0}", num);
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
				DateTime dateTime3 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
				string arg = dateTime2.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string arg2 = dateTime3.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string keyStr = string.Format("fuli_{0}_{1}_{2}", arg, arg2, SpecialPlatformType.UC.ToString());
				string[] array = Global.QeuryUserActivityInfo(client, keyStr, 1000, "0");
				if (array == null || array.Length == 0)
				{
					num = -15;
					result = string.Format("{0}", num);
				}
				else
				{
					int num2 = Global.SafeConvertToInt32(array[3]);
					if (num2 > 0)
					{
						num = -200;
						result = string.Format("{0}", num);
					}
					else
					{
						HuodongCachingMgr.GiveAward(client, platFuLiBaseData.myAwardItem, "UC平台福利");
						num2 = 1;
						Global.UpdateUserActivityInfo(client, keyStr, 1000, (long)num2, dateTime.ToString("yyyy-MM-dd HH$mm$ss"));
						result = string.Format("{0}", num);
					}
				}
			}
			return result;
		}

		public bool InitConfig()
		{
			lock (this.ConfigMutex)
			{
				UCPlatFuLiData ucplatFuLiData = new UCPlatFuLiData();
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("AliGifts");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array = paramValueByName.Split(new char[]
					{
						'|'
					});
					if (array.Length > 0)
					{
						ucplatFuLiData.myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "UC平台福利奖励");
					}
				}
				this.PlatFuLiBaseDataDict[SpecialPlatformType.UC.ToString()] = ucplatFuLiData;
				this.AliGiftsSwitch = (GameManager.systemParamsList.GetParamValueIntByName("AliGiftsSwitch", 1) > 0L);
			}
			return true;
		}

		private object ConfigMutex = new object();

		private Dictionary<string, SpecPlatFuLiManager.SpecHandler> evQueryHandlerDict = new Dictionary<string, SpecPlatFuLiManager.SpecHandler>();

		private Dictionary<string, SpecPlatFuLiManager.SpecHandler> evExcuteHandlerDict = new Dictionary<string, SpecPlatFuLiManager.SpecHandler>();

		private Dictionary<string, PlatFuLiBaseData> PlatFuLiBaseDataDict = new Dictionary<string, PlatFuLiBaseData>();

		private bool AliGiftsSwitch = true;

		private static SpecPlatFuLiManager instance = new SpecPlatFuLiManager();

		public delegate string SpecHandler(GameClient client, string param);
	}
}
