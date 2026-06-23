using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic.ProtoCheck
{
	public class ProtoChecker : SingletonTemplate<ProtoChecker>
	{
		private ProtoChecker()
		{
			this.RegisterCheck<SpriteActionData>(new ICheckerWrapper<SpriteActionData>.CheckerCallback(CheckConcrete.Checker_SpriteActionData));
			this.RegisterCheck<SpriteMagicCodeData>(new ICheckerWrapper<SpriteMagicCodeData>.CheckerCallback(CheckConcrete.Checker_SpriteMagicCodeData));
			this.RegisterCheck<SpriteMoveData>(new ICheckerWrapper<SpriteMoveData>.CheckerCallback(CheckConcrete.Checker_SpriteMoveData));
			this.RegisterCheck<SpritePositionData>(new ICheckerWrapper<SpritePositionData>.CheckerCallback(CheckConcrete.Checker_SpritePositionData));
			this.RegisterCheck<SpriteAttackData>(new ICheckerWrapper<SpriteAttackData>.CheckerCallback(CheckConcrete.Checker_SpriteAttackData));
			this.RegisterCheck<CS_SprUseGoods>(new ICheckerWrapper<CS_SprUseGoods>.CheckerCallback(CheckConcrete.Checker_CS_SprUseGoods));
			this.RegisterCheck<CS_QueryFuBen>(new ICheckerWrapper<CS_QueryFuBen>.CheckerCallback(CheckConcrete.Checker_CS_QueryFuBen));
			this.RegisterCheck<CS_ClickOn>(new ICheckerWrapper<CS_ClickOn>.CheckerCallback(CheckConcrete.Checker_CS_ClickOn));
			this.RegisterCheck<SCClientHeart>(new ICheckerWrapper<SCClientHeart>.CheckerCallback(CheckConcrete.Checker_SCClientHeart));
			this.RegisterCheck<SCFindMonster>(new ICheckerWrapper<SCFindMonster>.CheckerCallback(CheckConcrete.Checker_SCFindMonster));
			this.RegisterCheck<SCMoveEnd>(new ICheckerWrapper<SCMoveEnd>.CheckerCallback(CheckConcrete.Checker_SCMoveEnd));
			this.RegisterCheck<CSPropAddPoint>(new ICheckerWrapper<CSPropAddPoint>.CheckerCallback(CheckConcrete.Checker_CSPropAddPoint));
			this.RegisterCheck<SCMapChange>(new ICheckerWrapper<SCMapChange>.CheckerCallback(CheckConcrete.Checker_SCMapChange));
		}

		private void RegisterCheck<T>(ICheckerWrapper<T>.CheckerCallback cb) where T : class
		{
			this.checkerDic[typeof(T).FullName] = new ICheckerWrapper<T>(cb);
		}

		public bool EnableCheck
		{
			get
			{
				return this._enableCheck;
			}
		}

		public void SetEnableCheck(bool bOpen)
		{
			this._enableCheck = bOpen;
		}

		public bool Check<T>(byte[] data, int start, int count, Socket socket) where T : class, IProtoBuffData, new()
		{
			bool flag = this.CheckImpl<T>(data, start, count, socket);
			if (!flag)
			{
				if (data == null)
				{
					LogManager.WriteLog(1000, typeof(T).FullName + ", 反序列化的data为null", null, true);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < data.Length; i++)
					{
						stringBuilder.Append((int)data[i]).Append(' ');
					}
					LogManager.WriteLog(1000, string.Concat(new object[]
					{
						typeof(T).FullName,
						" 反序列化失败data=",
						stringBuilder.ToString(),
						" ,start=",
						start,
						" ,count=",
						count
					}), null, true);
					LogManager.WriteLog(1000, typeof(T).FullName + " 反序列化失败, 尝试检测是否是字符串类型 " + new UTF8Encoding().GetString(data, start, count), null, true);
				}
			}
			return flag;
		}

		private bool CheckImpl<T>(byte[] data, int start, int count, Socket socket) where T : class, IProtoBuffData, new()
		{
			bool result;
			if (!this.EnableCheck)
			{
				result = true;
			}
			else
			{
				ICheckerBase checkerBase = null;
				if (!this.checkerDic.TryGetValue(typeof(T).FullName, out checkerBase))
				{
					result = true;
				}
				else
				{
					T t = default(T);
					T t2 = default(T);
					try
					{
						t = DataHelper.BytesToObject<T>(data, 0, count);
					}
					catch (Exception)
					{
					}
					try
					{
						t2 = DataHelper.BytesToObject2<T>(data, 0, count, socket);
					}
					catch (Exception)
					{
					}
					if (t == null && t2 != null)
					{
						LogManager.WriteLog(1000, typeof(T).FullName + "， protobuf.net 解析数据为null，但是新解析方式不为null", null, true);
						result = false;
					}
					else if (t != null && t2 == null)
					{
						LogManager.WriteLog(1000, typeof(T).FullName + "， protobuf.net 解析数据不为null，但是新解析方式为null", null, true);
						result = false;
					}
					else if (t == null && t2 == null)
					{
						LogManager.WriteLog(1000, typeof(T).FullName + "， protobuf.net 解析数据为null，新解析方式为null", null, true);
						result = false;
					}
					else if (!checkerBase.Check(t, t2))
					{
						LogManager.WriteLog(1000, typeof(T).FullName + "， protobuf.net 解析数据不为null，新解析方式不为null，但是解析出来的数据不一致", null, true);
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}

		private Dictionary<string, ICheckerBase> checkerDic = new Dictionary<string, ICheckerBase>();

		private bool _enableCheck = false;
	}
}
