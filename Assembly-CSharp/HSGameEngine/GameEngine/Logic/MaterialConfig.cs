using System;
using Tmsk.Xml;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public class MaterialConfig
	{
		public Color RimColor1
		{
			get
			{
				return this.mRimColor1;
			}
		}

		public Color RimColor2
		{
			get
			{
				return this.mRimColor2;
			}
		}

		public void ResolveColor()
		{
			this.mRimColor1 = new Color32((byte)this.rimColor1[0], (byte)this.rimColor1[1], (byte)this.rimColor1[2], 0);
			this.mRimColor2 = new Color32((byte)this.rimColor2[0], (byte)this.rimColor2[1], (byte)this.rimColor2[2], 0);
		}

		public void BuildFrom(XElement ele)
		{
			this.ID = Global.GetXElementAttributeInt(ele, "ID");
			this.reflStrength = Global.GetXElementAttributeFloat(ele, "reflStrength");
			this.timeScale1 = Global.GetXElementAttributeFloat(ele, "timeScale1");
			this.specPow1 = Global.GetXElementAttributeFloat(ele, "specPow1");
			this.rimPow1 = Global.GetXElementAttributeFloat(ele, "rimPow1");
			int[] xelementAttributeIntArray = Global.GetXElementAttributeIntArray(ele, "rimColor1", "*");
			if (xelementAttributeIntArray != null && xelementAttributeIntArray.Length == 3)
			{
				this.rimColor1 = xelementAttributeIntArray;
			}
			this.rimPow2 = Global.GetXElementAttributeFloat(ele, "rimPow2");
			int[] xelementAttributeIntArray2 = Global.GetXElementAttributeIntArray(ele, "rimColor2", "*");
			if (xelementAttributeIntArray2 != null && xelementAttributeIntArray2.Length == 3)
			{
				this.rimColor2 = xelementAttributeIntArray2;
			}
			this._MaskTex = Global.GetXElementAttributeStr(ele, "_MaskTex");
		}

		public int ID;

		public float reflStrength;

		public float specPow1;

		public float timeScale1;

		public int[] rimColor1 = new int[3];

		public float rimPow1;

		public int[] rimColor2 = new int[3];

		public float rimPow2;

		private Color mRimColor1;

		private Color mRimColor2;

		public string _MaskTex;
	}
}
