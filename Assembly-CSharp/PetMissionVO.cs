using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class PetMissionVO
{
	public void CopyForm(XElement xle)
	{
		if (xle != null)
		{
			this.ID = Global.GetXElementAttributeInt(xle, "ID");
			this.Name = Global.GetXElementAttributeStr(xle, "Name");
			this.Type = Global.GetXElementAttributeInt(xle, "Type");
			this.Quality = Global.GetXElementAttributeInt(xle, "Quality");
			this.RateInterval = Global.GetXElementAttributeStr(xle, "RateInterval");
			this.SuccessRate = Global.GetXElementAttributeFloat(xle, "SuccessRate");
			this.PetLevelStep = Global.GetXElementAttributeInt(xle, "PetLevelStep");
			this.PetLevelStepRate = Global.GetXElementAttributeFloat(xle, "PetLevelStepRate");
			this.ExcellentStep = Global.GetXElementAttributeInt(xle, "ExcellentStep");
			this.ExcellentStepRate = Global.GetXElementAttributeFloat(xle, "ExcellentStepRate");
			this.SpecialPet = Global.GetXElementAttributeInt(xle, "SpecialPet");
			this.SpecialPetRate = Global.GetXElementAttributeInt(xle, "SpecialPetRate");
			this.Time = Global.GetXElementAttributeFloat(xle, "Time");
			this.NpcId = Global.GetXElementAttributeInt(xle, "NpcId");
			this.CrystalNum = Global.GetXElementAttributeInt(xle, "CrystalNum");
			this.SignNum = Global.GetXElementAttributeInt(xle, "SignNum");
			this.Activator = Global.GetXElementAttributeStr(xle, "Activator");
		}
	}

	public int[] ActivatorArray
	{
		get
		{
			if (!string.IsNullOrEmpty(this.Activator))
			{
				string[] array = this.Activator.Split(new char[]
				{
					','
				});
				if (0 < array.Length)
				{
					int[] array2 = new int[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = array[i].SafeToInt32(0);
					}
					return array2;
				}
			}
			return null;
		}
	}

	public int[] RateIntervalArray
	{
		get
		{
			if (!string.IsNullOrEmpty(this.RateInterval))
			{
				string[] array = this.RateInterval.Split(new char[]
				{
					','
				});
				if (0 < array.Length)
				{
					int[] array2 = new int[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = array[i].SafeToInt32(0);
					}
					return array2;
				}
			}
			return null;
		}
	}

	public string TitleColorString
	{
		get
		{
			string text = "fac60d";
			if (this.Quality == 1)
			{
				text = "fdf7dd";
			}
			else if (this.Quality == 2)
			{
				text = "17e43e";
			}
			else if (this.Quality == 3)
			{
				text = "3681f3";
			}
			else if (this.Quality == 4)
			{
				text = "b266ff";
			}
			return Global.GetColorStringForNGUIText(new object[]
			{
				text,
				this.Name
			});
		}
	}

	public int ID;

	public string Name;

	public int Type;

	public int Quality;

	public string RateInterval;

	public float SuccessRate;

	public int PetLevelStep;

	public float PetLevelStepRate;

	public int ExcellentStep;

	public float ExcellentStepRate;

	public int SpecialPet;

	public int SpecialPetRate;

	public float Time;

	public int NpcId;

	public int CrystalNum;

	public int SignNum;

	public string Activator;
}
