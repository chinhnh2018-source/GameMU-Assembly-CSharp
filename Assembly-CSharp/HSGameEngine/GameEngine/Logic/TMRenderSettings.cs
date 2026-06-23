using System;
using Tmsk.Xml;

namespace HSGameEngine.GameEngine.Logic
{
	public class TMRenderSettings
	{
		public TMRenderSettings()
		{
			bool[] array = new bool[4];
			array[0] = true;
			array[1] = true;
			this.ParticleEnabledOnLoading = array;
			this.ShaderLOD = new int[]
			{
				300,
				200,
				200,
				150
			};
			int[] array2 = new int[4];
			array2[0] = 1000;
			array2[1] = 30;
			array2[2] = 10;
			this.MaxVisiblePlayer = array2;
			int[] array3 = new int[4];
			array3[0] = 1000;
			array3[1] = 45;
			array3[2] = 15;
			this.MaxVisiblePet = array3;
			int[] array4 = new int[4];
			array4[0] = 1000;
			array4[1] = 45;
			array4[2] = 15;
			this.MaxVisibleWing = array4;
			int[] array5 = new int[4];
			array5[0] = 1000;
			array5[1] = 45;
			array5[2] = 15;
			this.MaxVisibleHunQi = array5;
			int[] array6 = new int[4];
			array6[0] = 1000;
			array6[1] = 45;
			array6[2] = 15;
			this.MaxVisibleWeaponEff = array6;
			int[] array7 = new int[4];
			array7[0] = 1000;
			array7[1] = 10;
			array7[2] = 5;
			this.MaxVisibleSkillEffect = array7;
			this.MaxScreenHight = new int[]
			{
				1080,
				960,
				720,
				540
			};
			this.ScreenScale = new float[]
			{
				1f,
				1f,
				1f,
				0.7f
			};
			bool[] array8 = new bool[4];
			array8[0] = true;
			array8[1] = true;
			array8[2] = true;
			this.ShadowEnable = array8;
			bool[] array9 = new bool[4];
			array9[0] = true;
			array9[1] = true;
			this.ParticleEnable = array9;
			bool[] array10 = new bool[4];
			array10[0] = true;
			array10[1] = true;
			this.PostEffectEnable = array10;
			bool[] array11 = new bool[4];
			array11[0] = true;
			this.WaterDistortEnable = array11;
			bool[] array12 = new bool[4];
			array12[0] = true;
			this.CameraBloomEnable = array12;
			base..ctor();
		}

		private void GetAttributeBoolArray(XElement ele, string attribute, ref bool[] setting)
		{
			int[] xelementAttributeIntArray = Global.GetXElementAttributeIntArray(ele, attribute, "*");
			setting = new bool[xelementAttributeIntArray.Length];
			for (int i = 0; i < xelementAttributeIntArray.Length; i++)
			{
				setting[i] = (xelementAttributeIntArray[i] > 0);
			}
		}

		public bool LoadFrom(XElement ele)
		{
			this.ShaderLODOnLoading = Global.GetXElementAttributeIntArray(ele, "ShaderOnLoad", "*");
			this.GetAttributeBoolArray(ele, "ParticleOnLoad", ref this.ParticleEnabledOnLoading);
			this.GetAttributeBoolArray(ele, "Shadow", ref this.ShadowEnable);
			this.ShaderLOD = Global.GetXElementAttributeIntArray(ele, "ShaderLOD", "*");
			this.MaxVisiblePlayer = Global.GetXElementAttributeIntArray(ele, "VisiblePlayer", "*");
			for (int i = 0; i < this.MaxVisiblePlayer.Length; i++)
			{
				this.MaxVisibleWing[i] = this.MaxVisiblePlayer[i] / 2;
				this.MaxVisibleWeaponEff[i] = this.MaxVisiblePlayer[i] / 2;
			}
			this.MaxVisibleSkillEffect = Global.GetXElementAttributeIntArray(ele, "MaxSkillCount", "*");
			this.MaxScreenHight = Global.GetXElementAttributeIntArray(ele, "MaxRes", "*");
			int[] xelementAttributeIntArray = Global.GetXElementAttributeIntArray(ele, "ScreenScale", "*");
			this.ScreenScale = new float[4];
			for (int j = 0; j < xelementAttributeIntArray.Length; j++)
			{
				this.ScreenScale[j] = (float)xelementAttributeIntArray[j] * 0.01f;
			}
			this.GetAttributeBoolArray(ele, "Particle", ref this.ParticleEnable);
			this.GetAttributeBoolArray(ele, "PostEffect", ref this.PostEffectEnable);
			this.GetAttributeBoolArray(ele, "WaterDistort", ref this.WaterDistortEnable);
			this.GetAttributeBoolArray(ele, "CameraBloom", ref this.CameraBloomEnable);
			return true;
		}

		public int[] ShaderLODOnLoading = new int[]
		{
			300,
			300,
			200,
			200
		};

		public bool[] ParticleEnabledOnLoading;

		public int[] ShaderLOD;

		public int[] MaxVisiblePlayer;

		public int[] MaxVisiblePet;

		public int[] MaxVisibleWing;

		public int[] MaxVisibleHunQi;

		public int[] MaxVisibleWeaponEff;

		public int[] MaxVisibleSkillEffect;

		public int[] MaxScreenHight;

		public float[] ScreenScale;

		public bool[] ShadowEnable;

		public bool[] ParticleEnable;

		public bool[] PostEffectEnable;

		public bool[] WaterDistortEnable;

		public bool[] CameraBloomEnable;
	}
}
