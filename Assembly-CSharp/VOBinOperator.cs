using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class VOBinOperator
{
	public static VOBinOperator Instance(Type type)
	{
		if (VOBinOperator.opDic.ContainsKey(type))
		{
			return VOBinOperator.opDic[type];
		}
		VOBinOperator vobinOperator = new VOBinOperator();
		VOBinOperator.opDic.Add(type, vobinOperator);
		vobinOperator.op = vobinOperator;
		return vobinOperator;
	}

	public void SetBuffer(byte[] blst)
	{
		this.buffer = blst;
	}

	public void ClearBuffer()
	{
		this.buffer = null;
	}

	public static List<byte> GoodsVOToBinLst(object vo)
	{
		if (vo.GetType() == typeof(GoodVO))
		{
			VOBinOperator.preGoodVO = VOBinOperator.curGoodVO;
			VOBinOperator.curGoodVO = (vo as GoodVO);
			return (vo as GoodVO).ToByteLst(VOBinOperator.preGoodVO, VOBinOperator.curGoodVO);
		}
		if (vo.GetType() == typeof(MonsterVO))
		{
			VOBinOperator.preMonsterVO = VOBinOperator.curMonsterVO;
			VOBinOperator.curMonsterVO = (vo as MonsterVO);
			return (vo as MonsterVO).ToByteLst(VOBinOperator.preMonsterVO, VOBinOperator.curMonsterVO);
		}
		if (vo.GetType() == typeof(NPCInfoVO))
		{
			VOBinOperator.preNPCInfoVO = VOBinOperator.curNPCInfoVO;
			VOBinOperator.curNPCInfoVO = (vo as NPCInfoVO);
			return (vo as NPCInfoVO).ToByteLst(VOBinOperator.preNPCInfoVO, VOBinOperator.curNPCInfoVO);
		}
		if (vo.GetType() == typeof(TaskVO))
		{
			VOBinOperator.preTaskVO = VOBinOperator.curTaskVO;
			VOBinOperator.curTaskVO = (vo as TaskVO);
			return (vo as TaskVO).ToByteLst(VOBinOperator.preTaskVO, VOBinOperator.curTaskVO);
		}
		return null;
	}

	public static void Clear()
	{
		VOBinOperator.preGoodVO = null;
		VOBinOperator.curGoodVO = null;
		VOBinOperator.preMonsterVO = null;
		VOBinOperator.curMonsterVO = null;
		VOBinOperator.preNPCInfoVO = null;
		VOBinOperator.curNPCInfoVO = null;
		VOBinOperator.preTaskVO = null;
		VOBinOperator.curTaskVO = null;
	}

	public static object RedefineType(object obj)
	{
		return obj;
	}

	public static void VOMemberToBinLst(List<byte> blst, int nMemberIndex, object preValue, object curValue)
	{
		if (curValue.GetType().Equals(typeof(int)))
		{
			bool isSameWithPre = preValue != null && (int)preValue == (int)curValue;
			VOBinOperator.VOMemberToBinLst(blst, nMemberIndex, true, isSameWithPre, (int)curValue, string.Empty);
		}
		else if (curValue.GetType().Equals(typeof(string)))
		{
			bool isSameWithPre = preValue != null && (string)preValue == (string)curValue;
			VOBinOperator.VOMemberToBinLst(blst, nMemberIndex, false, isSameWithPre, 0, (string)curValue);
		}
		else
		{
			bool isSameWithPre = preValue != null && preValue.ToString() == curValue.ToString();
			VOBinOperator.VOMemberToBinLst(blst, nMemberIndex, false, isSameWithPre, 0, curValue.ToString());
		}
	}

	public static void VOMemberToBinLst(List<byte> blst, int nMemberIndex, bool isInt, bool isSameWithPre = false, int nValue = 0, string sValue = "")
	{
		if (isSameWithPre)
		{
			blst.Add(240);
			blst.Add((byte)nMemberIndex);
			return;
		}
		if (isInt)
		{
			if (nValue == -1)
			{
				blst.Add(byte.MaxValue);
				blst.Add((byte)nMemberIndex);
			}
			else if (nValue == 0)
			{
				blst.Add(0);
				blst.Add((byte)nMemberIndex);
			}
			else
			{
				blst.Add(1);
				blst.Add((byte)nMemberIndex);
				byte b = (byte)(((long)nValue & (long)((ulong)-16777216)) >> 24);
				byte b2 = (byte)((nValue & 16711680) >> 16);
				byte b3 = (byte)((nValue & 65280) >> 8);
				byte b4 = (byte)(nValue & 255);
				blst.Add(b);
				blst.Add(b2);
				blst.Add(b3);
				blst.Add(b4);
			}
		}
		else if (string.IsNullOrEmpty(sValue))
		{
			blst.Add(16);
			blst.Add((byte)nMemberIndex);
		}
		else
		{
			blst.Add(17);
			blst.Add((byte)nMemberIndex);
			byte[] bytes = Encoding.UTF8.GetBytes(sValue);
			byte b5 = (byte)((bytes.Length & 16711680) >> 16);
			byte b6 = (byte)((bytes.Length & 65280) >> 8);
			byte b7 = (byte)(bytes.Length & 255);
			blst.Add(b5);
			blst.Add(b6);
			blst.Add(b7);
			for (int i = 0; i < bytes.Length; i++)
			{
				blst.Add(bytes[i]);
			}
		}
	}

	private int _GetBufferInt(int i)
	{
		int result = 0;
		if (this.buffer[i] == 255)
		{
			result = -1;
		}
		else if (this.buffer[i] == 0)
		{
			result = 0;
		}
		else if (this.buffer[i] == 1)
		{
			result = ((int)this.buffer[i + 2] << 24 | (int)this.buffer[i + 3] << 16 | (int)this.buffer[i + 4] << 8 | (int)this.buffer[i + 5]);
		}
		return result;
	}

	private string _GetBufferStr(int i)
	{
		string result = string.Empty;
		if (this.buffer[i] != 16)
		{
			if (this.buffer[i] == 17)
			{
				int num = (int)this.buffer[i + 2] << 16 | (int)this.buffer[i + 3] << 8 | (int)this.buffer[i + 4];
				byte[] array = new byte[num];
				Array.ConstrainedCopy(this.buffer, i + 5, array, 0, num);
				result = Encoding.UTF8.GetString(array);
			}
		}
		return result;
	}

	private byte[] _GetBufferStrBytes(int i)
	{
		if (this.buffer[i] == 16)
		{
			return null;
		}
		if (this.buffer[i] == 17)
		{
			int num = (int)this.buffer[i + 2] << 16 | (int)this.buffer[i + 3] << 8 | (int)this.buffer[i + 4];
			byte[] array = new byte[num];
			Array.ConstrainedCopy(this.buffer, i + 5, array, 0, num);
			return array;
		}
		return null;
	}

	private void SetMemberIntValueByTrd(int bufIndex, bool isInt = true, bool isSame = false)
	{
		byte b = this.buffer[bufIndex + 1];
		if (isSame)
		{
			VOBinOperator.curGoodVOPairs.PairValueLst[(int)b] = VOBinOperator.preGoodVOPairs.PairValueLst[(int)b];
			return;
		}
		if (isInt)
		{
			VOBinOperator.curGoodVOPairs.PairValueLst[(int)b].nValue = this._GetBufferInt(bufIndex);
		}
		else
		{
			VOBinOperator.curGoodVOPairs.PairValueLst[(int)b].sValue = this._GetBufferStr(bufIndex);
		}
		VOBinOperator.preGoodVOPairs.PairValueLst[(int)b] = VOBinOperator.curGoodVOPairs.PairValueLst[(int)b];
	}

	public void ParseBinToVOofGoodVO_ByTrdPairs()
	{
		VOBinOperator.preGoodVO = null;
		VOBinOperator.curGoodVO = null;
		VOBinOperator.preGoodVOPairs = new TrdGoodVOPairs(GoodVO.PropertyIndexDict);
		VOBinOperator.curGoodVOPairs = new TrdGoodVOPairs(GoodVO.PropertyIndexDict);
		Dictionary<int, GoodVO> goodsXmlNodeDict = ConfigGoods.GoodsXmlNodeDict;
		lock (goodsXmlNodeDict)
		{
			VOBinOperator.curGoodVO = new GoodVO();
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.buffer.Length)
			{
				byte b = this.buffer[i];
				if (this.buffer[i] == 240)
				{
					this.SetMemberIntValueByTrd(i, true, true);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 255)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 0)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 1)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 6;
					num2++;
				}
				else if (this.buffer[i] == 16)
				{
					this.SetMemberIntValueByTrd(i, false, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 17)
				{
					this.SetMemberIntValueByTrd(i, false, false);
					int num3 = (int)this.buffer[i + 2] << 16 | (int)this.buffer[i + 3] << 8 | (int)this.buffer[i + 4];
					i += 5 + num3;
					num2++;
				}
				else if (this.buffer[i] == 32)
				{
					VOBinOperator.curGoodVO.CopyFrom(VOBinOperator.curGoodVOPairs);
					ConfigGoods.GoodsXmlNodeDict[VOBinOperator.curGoodVO.ID] = VOBinOperator.curGoodVO;
					VOBinOperator.preGoodVO = VOBinOperator.curGoodVO;
					VOBinOperator.curGoodVO = new GoodVO();
					i++;
					num++;
					num2 = 0;
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"ERROR"
					});
				}
			}
		}
		this.ClearBuffer();
	}

	public void ParseBinToVOofMonsterVO_ByTrdPairs()
	{
		VOBinOperator.preMonsterVO = null;
		VOBinOperator.curMonsterVO = null;
		VOBinOperator.preGoodVOPairs = new TrdGoodVOPairs(MonsterVO.PropertyIndexDict);
		VOBinOperator.curGoodVOPairs = new TrdGoodVOPairs(MonsterVO.PropertyIndexDict);
		VOBinOperator.curMonsterVO = new MonsterVO();
		Dictionary<int, MonsterVO> monsterXmlNode = ConfigMonsters.MonsterXmlNode;
		Dictionary<int, MonsterVO> dictionary = monsterXmlNode;
		lock (dictionary)
		{
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.buffer.Length)
			{
				byte b = this.buffer[i];
				if (this.buffer[i] == 240)
				{
					this.SetMemberIntValueByTrd(i, true, true);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 255)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 0)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 1)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 6;
					num2++;
				}
				else if (this.buffer[i] == 16)
				{
					this.SetMemberIntValueByTrd(i, false, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 17)
				{
					this.SetMemberIntValueByTrd(i, false, false);
					int num3 = (int)this.buffer[i + 2] << 16 | (int)this.buffer[i + 3] << 8 | (int)this.buffer[i + 4];
					i += 5 + num3;
					num2++;
				}
				else if (this.buffer[i] == 32)
				{
					VOBinOperator.curMonsterVO.CopyFrom(VOBinOperator.curGoodVOPairs);
					monsterXmlNode[VOBinOperator.curMonsterVO.ID] = VOBinOperator.curMonsterVO;
					VOBinOperator.preMonsterVO = VOBinOperator.curMonsterVO;
					VOBinOperator.curMonsterVO = new MonsterVO();
					i++;
					num++;
					num2 = 0;
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"ERROR"
					});
				}
			}
		}
		this.ClearBuffer();
	}

	public void ParseBinToVOofNPCInfoVO_ByTrdPairs()
	{
		VOBinOperator.preNPCInfoVO = null;
		VOBinOperator.curNPCInfoVO = null;
		VOBinOperator.preGoodVOPairs = new TrdGoodVOPairs(NPCInfoVO.PropertyIndexDict);
		VOBinOperator.curGoodVOPairs = new TrdGoodVOPairs(NPCInfoVO.PropertyIndexDict);
		VOBinOperator.curNPCInfoVO = new NPCInfoVO();
		Dictionary<int, NPCInfoVO> npcvodict = ConfigNPCs.NPCVODict;
		Dictionary<int, NPCInfoVO> dictionary = npcvodict;
		lock (dictionary)
		{
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.buffer.Length)
			{
				byte b = this.buffer[i];
				if (this.buffer[i] == 240)
				{
					this.SetMemberIntValueByTrd(i, true, true);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 255)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 0)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 1)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 6;
					num2++;
				}
				else if (this.buffer[i] == 16)
				{
					this.SetMemberIntValueByTrd(i, false, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 17)
				{
					this.SetMemberIntValueByTrd(i, false, false);
					int num3 = (int)this.buffer[i + 2] << 16 | (int)this.buffer[i + 3] << 8 | (int)this.buffer[i + 4];
					i += 5 + num3;
					num2++;
				}
				else if (this.buffer[i] == 32)
				{
					VOBinOperator.curNPCInfoVO.CopyFrom(VOBinOperator.curGoodVOPairs);
					npcvodict[VOBinOperator.curNPCInfoVO.ID] = VOBinOperator.curNPCInfoVO;
					VOBinOperator.preNPCInfoVO = VOBinOperator.curNPCInfoVO;
					VOBinOperator.curNPCInfoVO = new NPCInfoVO();
					i++;
					num++;
					num2 = 0;
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"ERROR"
					});
				}
			}
		}
		this.ClearBuffer();
	}

	public void ParseBinToVOofTaskVO_ByTrdPairs()
	{
		VOBinOperator.preTaskVO = null;
		VOBinOperator.curTaskVO = null;
		VOBinOperator.preGoodVOPairs = new TrdGoodVOPairs(TaskVO.PropertyIndexDict);
		VOBinOperator.curGoodVOPairs = new TrdGoodVOPairs(TaskVO.PropertyIndexDict);
		VOBinOperator.curTaskVO = new TaskVO();
		Dictionary<int, TaskVO> taskXmlNodeDict = ConfigTasks.TaskXmlNodeDict;
		Dictionary<int, TaskVO> dictionary = taskXmlNodeDict;
		lock (dictionary)
		{
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.buffer.Length)
			{
				byte b = this.buffer[i];
				if (this.buffer[i] == 240)
				{
					this.SetMemberIntValueByTrd(i, true, true);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 255)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 0)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 1)
				{
					this.SetMemberIntValueByTrd(i, true, false);
					i += 6;
					num2++;
				}
				else if (this.buffer[i] == 16)
				{
					this.SetMemberIntValueByTrd(i, false, false);
					i += 2;
					num2++;
				}
				else if (this.buffer[i] == 17)
				{
					this.SetMemberIntValueByTrd(i, false, false);
					int num3 = (int)this.buffer[i + 2] << 16 | (int)this.buffer[i + 3] << 8 | (int)this.buffer[i + 4];
					i += 5 + num3;
					num2++;
				}
				else if (this.buffer[i] == 32)
				{
					VOBinOperator.curTaskVO.CopyFrom(VOBinOperator.curGoodVOPairs);
					taskXmlNodeDict[VOBinOperator.curTaskVO.ID] = VOBinOperator.curTaskVO;
					VOBinOperator.preTaskVO = VOBinOperator.curTaskVO;
					VOBinOperator.curTaskVO = new TaskVO();
					i++;
					num++;
					num2 = 0;
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"ERROR"
					});
				}
			}
		}
		this.ClearBuffer();
	}

	public static void TestXMLDicWithBinDic()
	{
	}

	public static void ParaseGoodXMLToBin()
	{
		VOBinOperator.Clear();
		VOBinOperator.XMLToBin<GoodVO>(new GoodVO(), false, 10);
		VOBinOperator.XMLToBin<MonsterVO>(new MonsterVO(), false, 10);
		VOBinOperator.XMLToBin<NPCInfoVO>(new NPCInfoVO(), false, 10);
		VOBinOperator.XMLToBin<TaskVO>(new TaskVO(), false, 10);
	}

	public static object XMLToBin<VO>(VO vo, bool retDicTrueOrBlst, int ConfigType = 10)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string newroot = string.Empty;
		if (vo.GetType() == typeof(GoodVO))
		{
			newroot = "Item";
			text = Application.dataPath + string.Format("/UIResources/Config{0}/GameRes/Config/Goods.Xml", ConfigType);
			text2 = Application.dataPath + string.Format("/UIResources/Config{0}/GameRes/Config/GoodsBin.txt", ConfigType);
		}
		else if (vo.GetType() == typeof(MonsterVO))
		{
			newroot = "Monster";
			text = Application.dataPath + string.Format("/UIResources/Config{0}/GameRes/Config/Monsters.Xml", ConfigType);
			text2 = Application.dataPath + string.Format("/UIResources/Config{0}/GameRes/Config/MonstersBin.txt", ConfigType);
		}
		else if (vo.GetType() == typeof(NPCInfoVO))
		{
			newroot = "NPC";
			text = Application.dataPath + string.Format("/UIResources/Config{0}/GameRes/Config/npcs.Xml", ConfigType);
			text2 = Application.dataPath + string.Format("/UIResources/Config{0}/GameRes/Config/npcsBin.txt", ConfigType);
		}
		else if (vo.GetType() == typeof(TaskVO))
		{
			newroot = "Task";
			text = Application.dataPath + string.Format("/UIResources/Config{0}/ServerRes/1/IsolateRes/Config/SystemTasks.Xml", ConfigType);
			text2 = Application.dataPath + string.Format("/UIResources/Config{0}/ServerRes/1/IsolateRes/Config/SystemTasksBin.txt", ConfigType);
		}
		XElement xelement = XElement.Load(text);
		if (xelement == null)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("解析: {0} 失败"), text));
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, newroot);
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("解析: {0} 失败"), text));
			return null;
		}
		List<List<byte>> list = new List<List<byte>>();
		if (vo.GetType() == typeof(GoodVO))
		{
			Dictionary<int, GoodVO> dictionary = new Dictionary<int, GoodVO>();
			Dictionary<int, GoodVO> dictionary2 = dictionary;
			lock (dictionary2)
			{
				int count = xelementList.Count;
				for (int i = 0; i < count; i++)
				{
					GoodVO goodVO = new GoodVO();
					goodVO.CopyFrom(xelementList[i]);
					dictionary[goodVO.ID] = goodVO;
					if (!retDicTrueOrBlst)
					{
						List<byte> list2 = VOBinOperator.GoodsVOToBinLst(goodVO);
						list.Add(list2);
					}
				}
			}
			if (retDicTrueOrBlst)
			{
				return dictionary as Dictionary<int, VO>;
			}
		}
		else if (vo.GetType() == typeof(MonsterVO))
		{
			Dictionary<int, MonsterVO> dictionary3 = new Dictionary<int, MonsterVO>();
			Dictionary<int, MonsterVO> dictionary4 = dictionary3;
			lock (dictionary4)
			{
				int count2 = xelementList.Count;
				for (int j = 0; j < count2; j++)
				{
					MonsterVO monsterVO = new MonsterVO();
					monsterVO.CopyFrom(xelementList[j]);
					dictionary3[monsterVO.ID] = monsterVO;
					if (!retDicTrueOrBlst)
					{
						List<byte> list3 = VOBinOperator.GoodsVOToBinLst(monsterVO);
						list.Add(list3);
					}
				}
			}
			if (retDicTrueOrBlst)
			{
				return dictionary3 as Dictionary<int, VO>;
			}
		}
		else if (vo.GetType() == typeof(NPCInfoVO))
		{
			Dictionary<int, NPCInfoVO> dictionary5 = new Dictionary<int, NPCInfoVO>();
			Dictionary<int, NPCInfoVO> dictionary6 = dictionary5;
			lock (dictionary6)
			{
				int count3 = xelementList.Count;
				for (int k = 0; k < count3; k++)
				{
					NPCInfoVO npcinfoVO = new NPCInfoVO();
					npcinfoVO.CopyFrom(xelementList[k]);
					dictionary5[npcinfoVO.ID] = npcinfoVO;
					if (!retDicTrueOrBlst)
					{
						List<byte> list4 = VOBinOperator.GoodsVOToBinLst(npcinfoVO);
						list.Add(list4);
					}
				}
			}
			if (retDicTrueOrBlst)
			{
				return dictionary5 as Dictionary<int, VO>;
			}
		}
		else if (vo.GetType() == typeof(TaskVO))
		{
			Dictionary<int, TaskVO> dictionary7 = new Dictionary<int, TaskVO>();
			Dictionary<int, TaskZhangJieVO> dictionary8 = new Dictionary<int, TaskZhangJieVO>();
			VOBinOperator.PreCacheTaskZhangJieXmlNodes(dictionary8, ConfigType);
			int l = 0;
			TaskZhangJieVO taskZhangJieVO = null;
			foreach (KeyValuePair<int, TaskZhangJieVO> keyValuePair in dictionary8)
			{
				int key = keyValuePair.Key;
				taskZhangJieVO = keyValuePair.Value;
				while (l < xelementList.Count)
				{
					TaskVO taskVO = new TaskVO();
					taskVO.CopyFrom(xelementList[l]);
					dictionary7[taskVO.ID] = taskVO;
					if (taskVO.TaskClass == 0)
					{
						if (taskVO.ID > taskZhangJieVO.EndTaskID)
						{
							break;
						}
						taskVO.TaskZhangJieID = key;
						taskVO.TaskIndexOfZhangJie = taskZhangJieVO.TaskCount;
						taskZhangJieVO.TaskCount++;
					}
					l++;
				}
			}
			while (l < xelementList.Count)
			{
				TaskVO taskVO = new TaskVO();
				taskVO.CopyFrom(xelementList[l]);
				if (taskVO.TaskClass == 0 && taskZhangJieVO != null)
				{
					taskVO.TaskIndexOfZhangJie = taskZhangJieVO.TaskCount;
					taskVO.TaskZhangJieID = taskZhangJieVO.ID;
				}
				dictionary7[taskVO.ID] = taskVO;
				l++;
			}
			if (retDicTrueOrBlst)
			{
				return dictionary7 as Dictionary<int, VO>;
			}
			foreach (TaskVO vo2 in dictionary7.Values)
			{
				List<byte> list5 = VOBinOperator.GoodsVOToBinLst(vo2);
				list.Add(list5);
			}
		}
		if (File.Exists(text))
		{
		}
		if (File.Exists(text2))
		{
			File.Delete(text2);
		}
		FileStream fileStream = new FileStream(text2, 2);
		for (int m = 0; m < list.Count; m++)
		{
			List<byte> list6 = list[m];
			for (int n = 0; n < list6.Count; n++)
			{
				byte b = list6[n];
				fileStream.WriteByte(b);
			}
		}
		fileStream.Close();
		return null;
	}

	private protected static int MinZhangJieID { protected get; private set; }

	private protected static int MaxZhangJieID { protected get; private set; }

	private static void PreCacheTaskZhangJieXmlNodes(Dictionary<int, TaskZhangJieVO> TaskZhangJieXmlNodeDict, int ConfigType)
	{
		TaskZhangJieXmlNodeDict.Clear();
		string text = Application.dataPath + string.Format("/UIResources/Config{0}/ServerRes/1/IsolateRes/Config/TaskZhangJie.Xml", ConfigType);
		XElement xelement = XElement.Load(text);
		if (xelement == null)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("解析: {0} 失败"), text));
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "ZhangJie");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("解析: {0} 失败"), text));
			return;
		}
		for (int i = 0; i < xelementList.Count; i++)
		{
			TaskZhangJieVO taskZhangJieVO = new TaskZhangJieVO();
			taskZhangJieVO.CopyFrom(xelementList[i]);
			TaskZhangJieXmlNodeDict[taskZhangJieVO.ID] = taskZhangJieVO;
			if (VOBinOperator.MinZhangJieID == 0)
			{
				VOBinOperator.MinZhangJieID = taskZhangJieVO.ID;
			}
			if (VOBinOperator.MaxZhangJieID < taskZhangJieVO.ID)
			{
				VOBinOperator.MaxZhangJieID = taskZhangJieVO.ID;
			}
		}
	}

	private static string _PackArrayString(string[] strArr)
	{
		if (strArr == null || strArr.Length <= 0)
		{
			return string.Empty;
		}
		char c = '\u0019';
		string text = ((char)strArr.Length).ToString() + c.ToString();
		for (int i = 0; i < strArr.Length; i++)
		{
			if (!ConvertExt.SafeConvertToDouble(strArr[i]).Equals(0.0))
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					((char)(i + 26)).ToString(),
					c.ToString(),
					strArr[i],
					c.ToString()
				});
			}
		}
		text = text.Substring(0, text.Length - 1);
		return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(text));
	}

	public static string PackArray<T>(T[] Arr)
	{
		string[] array = new string[Arr.Length];
		for (int i = 0; i < Arr.Length; i++)
		{
			array[i] = Arr[i].ToString();
		}
		return VOBinOperator._PackArrayString(array);
	}

	public static string[] UnPackArrayString(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			MUDebug.LogError<string>(new string[]
			{
				"UnPackArrayDouble Error! Owesome Error!"
			});
			return new string[0];
		}
		char c = '\u0019';
		string[] array = str.Split(new char[]
		{
			c
		});
		if (array.Length > 1)
		{
			int num = (int)array[0].charAt(0);
			string[] array2 = new string[num];
			for (int i = 2; i < array.Length; i += 2)
			{
				int num2 = (int)(array[i - 1].charAt(0) - '\u001a');
				array2[num2] = array[i];
			}
			return array2;
		}
		if (array.Length == 1)
		{
			int num3 = (int)array[0].charAt(0);
			return new string[num3];
		}
		MUDebug.LogError<string>(new string[]
		{
			"UnPackArrayDouble Error! Owesome Error!"
		});
		return new string[0];
	}

	public static double[] UnPackArrayDouble(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			MUDebug.LogError<string>(new string[]
			{
				"UnPackArrayDouble Error! Owesome Error!"
			});
			return new double[0];
		}
		char c = '\u0019';
		string[] array = str.Split(new char[]
		{
			c
		});
		if (array.Length > 1)
		{
			int num = (int)array[0].charAt(0);
			double[] array2 = new double[num];
			for (int i = 2; i < array.Length; i += 2)
			{
				int num2 = (int)(array[i - 1].charAt(0) - '\u001a');
				array2[num2] = ConvertExt.SafeConvertToDouble(array[i]);
			}
			return array2;
		}
		if (array.Length == 1)
		{
			int num3 = (int)array[0].charAt(0);
			return new double[num3];
		}
		MUDebug.LogError<string>(new string[]
		{
			"UnPackArrayDouble Error! Owesome Error!"
		});
		return new double[0];
	}

	private const bool USING_ANSI3 = false;

	private const char packc = '\u0019';

	private static Dictionary<Type, VOBinOperator> opDic = new Dictionary<Type, VOBinOperator>();

	private VOBinOperator op;

	private byte[] buffer;

	private static GoodVO preGoodVO = null;

	private static GoodVO curGoodVO = null;

	private static MonsterVO preMonsterVO = null;

	private static MonsterVO curMonsterVO = null;

	private static NPCInfoVO preNPCInfoVO = null;

	private static NPCInfoVO curNPCInfoVO = null;

	private static TaskVO preTaskVO = null;

	private static TaskVO curTaskVO = null;

	private static TrdGoodVOPairs preGoodVOPairs;

	private static TrdGoodVOPairs curGoodVOPairs;
}
