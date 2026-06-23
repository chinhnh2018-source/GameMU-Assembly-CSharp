using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UniLua;

internal class QuestionVerify
{
	private QuestionVerify()
	{
	}

	public static QuestionVerify Instance()
	{
		if (QuestionVerify._Instance == null)
		{
			object mutex = QuestionVerify.Mutex;
			lock (mutex)
			{
				if (QuestionVerify._Instance == null)
				{
					QuestionVerify._Instance = new QuestionVerify();
					QuestionVerify._Instance.init();
				}
			}
		}
		return QuestionVerify._Instance;
	}

	private void init()
	{
		this._L = LuaAPI.NewState();
		this._L.L_OpenLibs();
		CSharpFunctionDelegate csharpFunctionDelegate = new CSharpFunctionDelegate(QuestionVerify.CalcRandom);
		this._luaFuncs.Add(csharpFunctionDelegate);
		CSharpFunctionDelegate csharpFunctionDelegate2 = new CSharpFunctionDelegate(QuestionVerify.MyHash1);
		this._luaFuncs.Add(csharpFunctionDelegate2);
		CSharpFunctionDelegate csharpFunctionDelegate3 = new CSharpFunctionDelegate(QuestionVerify.MyHash2);
		this._luaFuncs.Add(csharpFunctionDelegate3);
		CSharpFunctionDelegate csharpFunctionDelegate4 = new CSharpFunctionDelegate(QuestionVerify.MyHash3);
		this._luaFuncs.Add(csharpFunctionDelegate4);
		this._L.PushCSharpFunction(csharpFunctionDelegate);
		this._L.SetGlobal("CalcRandom");
		this._L.PushCSharpFunction(csharpFunctionDelegate2);
		this._L.SetGlobal("MyHash1");
		this._L.PushCSharpFunction(csharpFunctionDelegate3);
		this._L.SetGlobal("MyHash2");
		this._L.PushCSharpFunction(csharpFunctionDelegate4);
		this._L.SetGlobal("MyHash3");
	}

	public QuestionVerifyRsp Resolve(QuestionVerifyReq req)
	{
		QuestionVerifyRsp questionVerifyRsp = null;
		int top = this._L.GetTop();
		try
		{
			if (this._L.L_DoString(req.Question) == null && this._L.IsFunction(-1))
			{
				int num = 0;
				int num2 = 0;
				while (req.ArgumentTypes != null && num2 < req.ArgumentTypes.Count)
				{
					this.PushArgument((QuestionVerify.QuestionArgType)req.ArgumentTypes[num2], req);
					num++;
					num2++;
				}
				this._L.Call(num, 1);
				questionVerifyRsp = new QuestionVerifyRsp
				{
					Id = req.Id
				};
				questionVerifyRsp.Answer = this._L.ToUnsigned(-1);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogError<string>(new string[]
			{
				"QuestionVerify.Resolve Exception: {0}",
				ex.Message
			});
		}
		finally
		{
			this._L.SetTop(top);
		}
		return questionVerifyRsp;
	}

	private bool PushArgument(QuestionVerify.QuestionArgType argType, QuestionVerifyReq req)
	{
		if (argType == QuestionVerify.QuestionArgType.Undefine)
		{
			return false;
		}
		if (argType == QuestionVerify.QuestionArgType.ClientPort)
		{
			return false;
		}
		if (argType == QuestionVerify.QuestionArgType.ClientRoleId)
		{
			this._L.PushUnsigned((uint)Global.Data.RoleID);
			return true;
		}
		if (argType == QuestionVerify.QuestionArgType.ClientZoneId)
		{
			this._L.PushUnsigned((uint)Global.Data.roleData.ZoneID);
			return true;
		}
		if (argType == QuestionVerify.QuestionArgType.ClientServerId)
		{
			this._L.PushUnsigned((uint)Global.Data.GameServerID);
			return true;
		}
		if (argType == QuestionVerify.QuestionArgType.QuestionLength)
		{
			this._L.PushUnsigned((uint)req.Question.Length);
			return true;
		}
		if (argType == QuestionVerify.QuestionArgType.QuestionSelf)
		{
			this._L.PushString(req.Question);
			return true;
		}
		return false;
	}

	private static int CalcRandom(ILuaState L)
	{
		uint num = L.L_CheckUnsigned(1);
		uint num2 = L.L_CheckUnsigned(2);
		L.PushUnsigned(num * 322U + num2);
		return 1;
	}

	private static int MySum(ILuaState L)
	{
		uint num = L.L_CheckUnsigned(1);
		uint num2 = L.L_CheckUnsigned(2);
		L.PushUnsigned(num + num2);
		return 1;
	}

	private static int MyAdd(ILuaState L)
	{
		uint num = L.L_CheckUnsigned(1);
		uint num2 = L.L_CheckUnsigned(2);
		L.PushUnsigned(num + num2);
		return 1;
	}

	public static int MyHash1(ILuaState L)
	{
		string text = L.L_CheckString(1);
		uint num = 326126U;
		byte[] bytes = new UTF8Encoding().GetBytes(text);
		for (int i = 0; i < bytes.Length; i++)
		{
			num *= 8460239U;
			num ^= (uint)bytes[i];
		}
		L.PushUnsigned(num);
		return 1;
	}

	public static int MyHash2(ILuaState L)
	{
		string text = L.L_CheckString(1);
		byte[] bytes = new UTF8Encoding().GetBytes(text);
		uint num = 16789619U;
		uint num2 = 8461461U;
		for (int i = 0; i < bytes.Length; i++)
		{
			num2 = (num2 * num ^ (uint)bytes[i]);
		}
		num2 += num2 << 13;
		num2 ^= num2 >> 7;
		num2 += num2 << 3;
		num2 ^= num2 >> 17;
		num2 += num2 << 5;
		L.PushUnsigned(num2);
		return 1;
	}

	public static int MyHash3(ILuaState L)
	{
		string text = L.L_CheckString(1);
		byte[] bytes = new UTF8Encoding().GetBytes(text);
		uint num = 1315111U;
		for (int i = 0; i < bytes.Length; i++)
		{
			num = (num << 4 ^ num >> 27 ^ (uint)bytes[i]);
		}
		L.PushUnsigned(num);
		return 1;
	}

	private static QuestionVerify _Instance = null;

	private static object Mutex = new object();

	private ILuaState _L;

	private List<CSharpFunctionDelegate> _luaFuncs = new List<CSharpFunctionDelegate>();

	private enum QuestionArgType
	{
		Undefine,
		ClientRoleId,
		ClientPort,
		ClientZoneId,
		ClientServerId,
		QuestionLength,
		QuestionSelf
	}
}
