using System;
using System.Collections.Generic;
using System.Text;
using Tmsk.Xml;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public static class MaterialManager
	{
		public static Color ParseStringColor(string textColor)
		{
			try
			{
				if (string.IsNullOrEmpty(textColor))
				{
					return new Color(1f, 1f, 1f);
				}
				string[] array = textColor.Split(new char[]
				{
					','
				});
				if (array.Length != 3)
				{
					return new Color(1f, 1f, 1f);
				}
				return new Color((float)Global.SafeConvertToInt32(array[0]) / 255f, (float)Global.SafeConvertToInt32(array[1]) / 255f, (float)Global.SafeConvertToInt32(array[2]) / 255f);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return new Color(1f, 1f, 1f);
		}

		private static MaterialCachingItem GetMaterialCachingItem(int shaderID)
		{
			MaterialCachingItem materialCachingItem = null;
			if (MaterialManager.MaterialCachingItemDict.TryGetValue(shaderID, ref materialCachingItem))
			{
				return materialCachingItem;
			}
			XElement shaderXmlNodeByID = Global.GetShaderXmlNodeByID(shaderID, false);
			if (shaderXmlNodeByID == null)
			{
				return null;
			}
			materialCachingItem = new MaterialCachingItem();
			materialCachingItem.Material = Global.GetXElementAttributeStr(shaderXmlNodeByID, "Material");
			materialCachingItem.Layer0 = (Global.GetXElementAttributeStr(shaderXmlNodeByID, "Layer0") == "true");
			materialCachingItem.Layer1 = (Global.GetXElementAttributeStr(shaderXmlNodeByID, "Layer1") == "true");
			materialCachingItem.Layer2 = (Global.GetXElementAttributeStr(shaderXmlNodeByID, "Layer2") == "true");
			materialCachingItem.Layer3 = (Global.GetXElementAttributeStr(shaderXmlNodeByID, "Layer3") == "true");
			materialCachingItem._Specular = MaterialManager.ParseStringColor(Global.GetXElementAttributeStr(shaderXmlNodeByID, "_Specular"));
			materialCachingItem._RimPow = (float)Global.GetXElementAttributeDouble(shaderXmlNodeByID, "_RimPow");
			materialCachingItem._RimPow3 = (float)Global.GetXElementAttributeDouble(shaderXmlNodeByID, "_RimPow3");
			materialCachingItem._LightColor = MaterialManager.ParseStringColor(Global.GetXElementAttributeStr(shaderXmlNodeByID, "_LightColor"));
			materialCachingItem._LightColor1 = MaterialManager.ParseStringColor(Global.GetXElementAttributeStr(shaderXmlNodeByID, "_LightColor1"));
			materialCachingItem._LightColor2 = MaterialManager.ParseStringColor(Global.GetXElementAttributeStr(shaderXmlNodeByID, "_LightColor2"));
			materialCachingItem._LightColor3 = MaterialManager.ParseStringColor(Global.GetXElementAttributeStr(shaderXmlNodeByID, "_LightColor3"));
			materialCachingItem._dirX3 = (float)Global.GetXElementAttributeDouble(shaderXmlNodeByID, "_dirX3");
			materialCachingItem._dirY3 = (float)Global.GetXElementAttributeDouble(shaderXmlNodeByID, "_dirY3");
			MaterialManager.MaterialCachingItemDict.Add(shaderID, materialCachingItem);
			return materialCachingItem;
		}

		private static MaterialCachingItemRefl GetMaterialCachingItemRefl(int shaderID)
		{
			MaterialCachingItemRefl materialCachingItemRefl = null;
			if (MaterialManager.MaterialCachingItemReflDict.TryGetValue(shaderID, ref materialCachingItemRefl))
			{
				return materialCachingItemRefl;
			}
			XElement shaderXmlNodeByID = Global.GetShaderXmlNodeByID(shaderID, false);
			if (shaderXmlNodeByID == null)
			{
				return null;
			}
			materialCachingItemRefl = new MaterialCachingItemRefl();
			materialCachingItemRefl.Material = Global.GetXElementAttributeStr(shaderXmlNodeByID, "Material");
			materialCachingItemRefl.Layer0 = (Global.GetXElementAttributeStr(shaderXmlNodeByID, "Layer0") == "true");
			materialCachingItemRefl.Layer1 = (Global.GetXElementAttributeStr(shaderXmlNodeByID, "Layer1") == "true");
			materialCachingItemRefl.Layer2 = (Global.GetXElementAttributeStr(shaderXmlNodeByID, "Layer2") == "true");
			materialCachingItemRefl.MainColor = MaterialManager.ParseStringColor(Global.GetXElementAttributeStr(shaderXmlNodeByID, "MainColor"));
			materialCachingItemRefl.ReflectionColor = MaterialManager.ParseStringColor(Global.GetXElementAttributeStr(shaderXmlNodeByID, "ReflectionColor"));
			materialCachingItemRefl.RotateAngleSpeed = (float)Global.GetXElementAttributeDouble(shaderXmlNodeByID, "RotateAngleSpeed");
			materialCachingItemRefl.SpecularPower = (float)Global.GetXElementAttributeDouble(shaderXmlNodeByID, "SpecularPower");
			materialCachingItemRefl.SpecLightColor1 = MaterialManager.ParseStringColor(Global.GetXElementAttributeStr(shaderXmlNodeByID, "SpecLightColor1"));
			materialCachingItemRefl.TimeScaleForAnimation1 = (float)Global.GetXElementAttributeDouble(shaderXmlNodeByID, "TimeScaleForAnimation1");
			materialCachingItemRefl.LightScale = (float)Global.GetXElementAttributeDouble(shaderXmlNodeByID, "LightScale");
			materialCachingItemRefl.SpecLightColor2 = MaterialManager.ParseStringColor(Global.GetXElementAttributeStr(shaderXmlNodeByID, "SpecLightColor2"));
			materialCachingItemRefl.TimeScaleForAnimation2 = (float)Global.GetXElementAttributeDouble(shaderXmlNodeByID, "TimeScaleForAnimation2");
			MaterialManager.MaterialCachingItemReflDict.Add(shaderID, materialCachingItemRefl);
			return materialCachingItemRefl;
		}

		public static Material GetMaterialByShaderID(int shaderID, bool isAlpha)
		{
			if (shaderID <= 0)
			{
				return null;
			}
			MaterialCachingItem materialCachingItem = MaterialManager.GetMaterialCachingItem(shaderID);
			if (materialCachingItem == null)
			{
				return null;
			}
			string text = string.Format("{0}_{1}", materialCachingItem.Material, (!isAlpha) ? "diff" : "alpha");
			text = string.Format("Materials/{0}", text);
			Material material = null;
			if (!MaterialManager.MaterialCachingDict.TryGetValue(text, ref material))
			{
				material = (Resources.Load(text, typeof(Material)) as Material);
				MaterialManager.MaterialCachingDict.Add(text, material);
			}
			if (null == material)
			{
				return null;
			}
			Material material2 = SpawnManager.Instantiate(material) as Material;
			if (!materialCachingItem.Layer0)
			{
				material2.SetTexture("_Light", null);
			}
			if (!materialCachingItem.Layer1)
			{
				material2.SetTexture("_Light1", null);
			}
			if (!materialCachingItem.Layer2)
			{
				material2.SetTexture("_Light2", null);
			}
			if (!materialCachingItem.Layer3)
			{
				material2.SetTexture("_Light3", null);
			}
			material2.SetColor("_Specular", materialCachingItem._Specular);
			material2.SetFloat("_RimPow", materialCachingItem._RimPow);
			material2.SetFloat("_RimPow3", materialCachingItem._RimPow3);
			material2.SetColor("_LightColor", materialCachingItem._LightColor);
			material2.SetColor("_LightColor1", materialCachingItem._LightColor1);
			material2.SetColor("_LightColor2", materialCachingItem._LightColor2);
			material2.SetColor("_LightColor3", materialCachingItem._LightColor3);
			material2.SetFloat("_dirX3", materialCachingItem._dirX3);
			material2.SetFloat("_dirY3", materialCachingItem._dirY3);
			return material2;
		}

		public static Material GetMaterialReflByShaderID(int shaderID, bool isAlpha)
		{
			if (shaderID <= 0)
			{
				return null;
			}
			MaterialCachingItemRefl materialCachingItemRefl = MaterialManager.GetMaterialCachingItemRefl(shaderID);
			if (materialCachingItemRefl == null)
			{
				return null;
			}
			string text = string.Format("{0}_{1}", materialCachingItemRefl.Material, (!isAlpha) ? "diff" : "alpha");
			text = string.Format("Materials/{0}", text);
			Material material = null;
			if (!MaterialManager.MaterialCachingReflDict.TryGetValue(text, ref material))
			{
				material = (Resources.Load(text, typeof(Material)) as Material);
				MaterialManager.MaterialCachingReflDict.Add(text, material);
			}
			if (null == material)
			{
				return null;
			}
			Material material2 = SpawnManager.Instantiate(material) as Material;
			if (2000 > shaderID && 1000 <= shaderID)
			{
				material2.SetColor("_RimColor", materialCachingItemRefl.MainColor);
				material2.SetColor("_InnerColor", materialCachingItemRefl.ReflectionColor);
				return material2;
			}
			if (materialCachingItemRefl.Layer0)
			{
				if (isAlpha)
				{
					material2.shader = Shader.Find("MuCharacter/Base-Alpha");
				}
				else
				{
					material2.shader = Shader.Find("MuCharacter/Base");
				}
			}
			if (materialCachingItemRefl.Layer1 && !materialCachingItemRefl.Layer2)
			{
				if (isAlpha)
				{
					material2.shader = Shader.Find("MuCharacter/Specular1-Alpha");
				}
				else
				{
					material2.shader = Shader.Find("MuCharacter/Specular1");
				}
			}
			else if (!materialCachingItemRefl.Layer1 && materialCachingItemRefl.Layer2)
			{
				if (isAlpha)
				{
					material2.shader = Shader.Find("MuCharacter/Specular2-Alpha");
				}
				else
				{
					material2.shader = Shader.Find("MuCharacter/Specular2");
				}
			}
			else if (materialCachingItemRefl.Layer1 && materialCachingItemRefl.Layer2)
			{
				if (isAlpha)
				{
					material2.shader = Shader.Find("MuCharacter/Specular1N2-Alpha");
				}
				else
				{
					material2.shader = Shader.Find("MuCharacter/Specular1N2");
				}
			}
			if (materialCachingItemRefl.Layer0)
			{
				material2.SetColor("_Color", materialCachingItemRefl.MainColor);
				material2.SetColor("_ReflectColor", materialCachingItemRefl.ReflectionColor);
			}
			else
			{
				material2.SetColor("_ReflectColor", Color.black);
			}
			if (materialCachingItemRefl.Layer1)
			{
				material2.SetFloat("_SpecPow", materialCachingItemRefl.SpecularPower);
				material2.SetColor("_SpecColor1", materialCachingItemRefl.SpecLightColor1);
				material2.SetFloat("_TimeScale1", materialCachingItemRefl.TimeScaleForAnimation1);
				Texture texture = Resources.Load("MUTexture/SpecColorLayer1") as Texture;
				material2.SetTexture("_ViewDirTex1", texture);
			}
			if (materialCachingItemRefl.Layer2)
			{
				material2.SetFloat("_LightScale", materialCachingItemRefl.LightScale);
				material2.SetColor("_SpecColor2", materialCachingItemRefl.SpecLightColor2);
				material2.SetFloat("_TimeScale2", materialCachingItemRefl.TimeScaleForAnimation2);
				Texture texture2 = Resources.Load("MUTexture/SpecColorLayer2") as Texture;
				material2.SetTexture("_ViewDirTex2", texture2);
			}
			return material2;
		}

		public static Material GetFashionMaterialReflByShaderID(Material origin, int shaderID, bool isAlpha)
		{
			XElement shaderXmlNodeByID = Global.GetShaderXmlNodeByID(shaderID, true);
			if (shaderXmlNodeByID == null)
			{
				return null;
			}
			if (null == MaterialManager.ViewSpecTex1)
			{
				MaterialManager.ViewSpecTex1 = (Resources.Load("MUTexture/SpecColorLayer1") as Texture2D);
			}
			if (null == MaterialManager.ReflCubemap)
			{
				MaterialManager.ReflCubemap = (Resources.Load("MUTexture/MUCharacter") as Cubemap);
			}
			MaterialConfig materialConfig = new MaterialConfig();
			materialConfig.BuildFrom(shaderXmlNodeByID);
			string text = new StringBuilder(origin.mainTexture.name).Append(shaderID).ToString();
			text = string.Format("Materials/{0}", text);
			Material material = null;
			if (!MaterialManager.MaterialCachingReflDict.TryGetValue(text, ref material))
			{
				material = new Material(origin);
				material.shader = Shader.Find("Artist/PlayerCharacter");
				MaterialManager.SetMaterial(material, materialConfig);
				MaterialManager.MaterialCachingReflDict.Add(text, material);
			}
			if (null == material)
			{
				return null;
			}
			return SpawnManager.Instantiate(material) as Material;
		}

		private static void SetMaterial(Material m, MaterialConfig config)
		{
			if (config.reflStrength > 0f)
			{
				m.SetFloat("_ReflectStrength", config.reflStrength);
				m.SetTexture("_Cube", MaterialManager.ReflCubemap);
				if (config.specPow1 > 0f)
				{
					m.SetFloat("_SpecPow", config.specPow1);
					m.SetFloat("_TimeScale1", config.timeScale1);
					m.SetTexture("_ViewDirTex1", MaterialManager.ViewSpecTex1);
					if (!string.IsNullOrEmpty(config._MaskTex))
					{
						Texture2D texture2D = Resources.Load(string.Format("MUTexture/{0}", config._MaskTex)) as Texture2D;
						if (null != texture2D)
						{
							m.SetTexture("_MaskTex", texture2D);
						}
					}
					m.EnableKeyword(MaterialManager.Spec1Keyword);
					m.DisableKeyword(MaterialManager.RelfKeyword);
				}
				else
				{
					m.DisableKeyword(MaterialManager.Spec1Keyword);
					m.EnableKeyword(MaterialManager.RelfKeyword);
				}
			}
			else if (config.rimPow1 > 0f)
			{
				m.SetColor("_RimColor", config.RimColor1);
				m.SetFloat("_RimPower", config.rimPow1);
				if (config.rimPow2 > 0f)
				{
					m.SetColor("_InnerColor", config.RimColor2);
					m.SetFloat("_InnerStrength", config.rimPow2);
					m.DisableKeyword("_SINGLE_RIM");
					m.EnableKeyword("_DUAL_RIM");
				}
				else
				{
					m.DisableKeyword("_DUAL_RIM");
					m.EnableKeyword("_SINGLE_RIM");
				}
			}
		}

		private static readonly string RelfKeyword = "_REF_SPEC";

		private static readonly string Spec1Keyword = "_REF_SPEC1";

		private static Texture2D ViewSpecTex1;

		private static Cubemap ReflCubemap;

		private static Dictionary<int, MaterialCachingItem> MaterialCachingItemDict = new Dictionary<int, MaterialCachingItem>();

		private static Dictionary<int, MaterialCachingItemRefl> MaterialCachingItemReflDict = new Dictionary<int, MaterialCachingItemRefl>();

		private static Dictionary<string, Material> MaterialCachingDict = new Dictionary<string, Material>();

		private static Dictionary<string, Material> MaterialCachingReflDict = new Dictionary<string, Material>();
	}
}
