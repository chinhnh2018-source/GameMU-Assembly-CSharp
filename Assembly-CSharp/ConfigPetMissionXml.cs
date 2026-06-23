using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigPetMissionXml
{
	public ConfigPetMissionXml(int TaskID)
	{
		XElement gameResXml = Global.GetGameResXml("Config/PetMission.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "PetMission");
			byte b = 0;
			while ((int)b < xelementList.Count)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[(int)b], "ID");
				if (xelementAttributeInt == TaskID)
				{
					this.mPetMissionVO = new PetMissionVO();
					this.mPetMissionVO.CopyForm(xelementList[(int)b]);
					break;
				}
				b += 1;
			}
		}
	}

	public PetMissionVO GetPetMissionVOByID(int ID)
	{
		return null;
	}

	public PetMissionVO GetPetMissionVO()
	{
		return this.mPetMissionVO;
	}

	public string[] GetAwardStrAray(bool bSuccess)
	{
		byte b = 0;
		string activator = this.GetPetMissionVO().Activator;
		if (!string.IsNullOrEmpty(activator))
		{
			string[] array = activator.Split(new char[]
			{
				'|'
			});
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 2 && 0 < array2[1].SafeToInt32(0))
					{
						b += 1;
					}
				}
			}
		}
		if (0 < this.GetPetMissionVO().CrystalNum)
		{
			b += 1;
		}
		if (0 < this.GetPetMissionVO().SignNum)
		{
			b += 1;
		}
		List<string> list = new List<string>();
		if (!string.IsNullOrEmpty(activator))
		{
			string[] array3 = activator.Split(new char[]
			{
				'|'
			});
			if (array3 != null)
			{
				for (int j = 0; j < array3.Length; j++)
				{
					string[] array4 = array3[j].Split(new char[]
					{
						','
					});
					if (array4.Length == 2)
					{
						float num = (float)array4[1].SafeToInt32(0);
						if (0f < num)
						{
							if (!bSuccess)
							{
								num *= (float)ConfigSystemParam.GetSystemParamIntByName("FailAwardRate") / 100f;
							}
							int num2 = Mathf.FloorToInt(num);
							if (num2 > 0)
							{
								list.Add(string.Format("{0},{1},0,0,0,0,0", array4[0].SafeToInt32(0) + 8039 - 1, num2));
							}
						}
					}
				}
			}
			string[] array5 = activator.Split(new char[]
			{
				','
			});
		}
		if (0 < this.GetPetMissionVO().CrystalNum)
		{
			float num3 = (float)this.GetPetMissionVO().CrystalNum;
			if (!bSuccess)
			{
				num3 *= (float)ConfigSystemParam.GetSystemParamIntByName("FailAwardRate") / 100f;
			}
			int num4 = Mathf.FloorToInt(num3);
			if (num4 > 0)
			{
				list.Add(string.Format("{0},{1},0,0,0,0,0", 8038, num4));
			}
		}
		if (0 < this.GetPetMissionVO().SignNum)
		{
			float num5 = (float)this.GetPetMissionVO().SignNum;
			if (!bSuccess)
			{
				num5 *= (float)ConfigSystemParam.GetSystemParamIntByName("FailAwardRate") / 100f;
			}
			int num6 = Mathf.FloorToInt(num5);
			if (num6 > 0)
			{
				list.Add(string.Format("{0},{1},0,0,0,0,0", 8036, num6));
			}
		}
		return list.ToArray();
	}

	private PetMissionVO mPetMissionVO;
}
