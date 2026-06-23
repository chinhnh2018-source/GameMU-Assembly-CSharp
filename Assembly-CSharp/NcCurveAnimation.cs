using System;
using System.Collections.Generic;
using UnityEngine;

public class NcCurveAnimation : NcEffectAniBehaviour
{
	public override int GetAnimationState()
	{
		if (!base.enabled || !NcEffectBehaviour.IsActive(base.gameObject))
		{
			return -1;
		}
		if (0f < this.m_fDurationTime && (this.m_fStartTime == 0f || !base.IsEndAnimation()))
		{
			return 1;
		}
		return 0;
	}

	public override void ResetAnimation()
	{
		base.ResetAnimation();
		this.InitAnimation();
		this.UpdateAnimation(0f);
	}

	public float GetRepeatedRate()
	{
		return this.m_fElapsedRate;
	}

	private void Awake()
	{
	}

	private void Start()
	{
		this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
		this.InitAnimation();
		if (0f < this.m_fDelayTime)
		{
			if (base.GetComponent<Renderer>())
			{
				base.GetComponent<Renderer>().enabled = false;
			}
			return;
		}
		base.InitAnimationTimer();
		this.UpdateAnimation(0f);
	}

	private void LateUpdate()
	{
		if (this.m_fStartTime == 0f)
		{
			return;
		}
		if (this.m_fDelayTime != 0f)
		{
			if (NcEffectBehaviour.GetEngineTime() < this.m_fStartTime + this.m_fDelayTime)
			{
				return;
			}
			this.m_fDelayTime = 0f;
			base.InitAnimationTimer();
			if (base.GetComponent<Renderer>())
			{
				base.GetComponent<Renderer>().enabled = true;
			}
		}
		float time = this.m_Timer.GetTime();
		float fElapsedRate = time;
		if (this.m_fDurationTime != 0f)
		{
			fElapsedRate = time / this.m_fDurationTime;
		}
		this.UpdateAnimation(fElapsedRate);
	}

	private void InitAnimation()
	{
		this.m_fElapsedRate = 0f;
		this.m_Transform = base.transform;
		foreach (NcCurveAnimation.NcInfoCurve ncInfoCurve in this.m_CurveInfoList)
		{
			if (ncInfoCurve.m_bEnabled)
			{
				switch (ncInfoCurve.m_ApplyType)
				{
				case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION:
					ncInfoCurve.m_OriginalValue = Vector4.zero;
					ncInfoCurve.m_BeforeValue = ncInfoCurve.m_OriginalValue;
					break;
				case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.ROTATION:
					ncInfoCurve.m_OriginalValue = Vector4.zero;
					ncInfoCurve.m_BeforeValue = ncInfoCurve.m_OriginalValue;
					break;
				case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE:
					ncInfoCurve.m_OriginalValue = this.m_Transform.localScale;
					ncInfoCurve.m_BeforeValue = ncInfoCurve.m_OriginalValue;
					break;
				case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.COLOR:
					if (ncInfoCurve.m_bRecursively)
					{
						this.m_ChildRenderers = base.transform.GetComponentsInChildren<Renderer>(true);
						this.m_ChildColorNames = new string[this.m_ChildRenderers.Length];
						ncInfoCurve.m_ChildOriginalColorValues = new Vector4[this.m_ChildRenderers.Length];
						ncInfoCurve.m_ChildBeforeColorValues = new Vector4[this.m_ChildRenderers.Length];
						for (int i = 0; i < this.m_ChildRenderers.Length; i++)
						{
							Renderer renderer = this.m_ChildRenderers[i];
							this.m_ChildColorNames[i] = NcCurveAnimation.Ng_GetMaterialColorName(renderer.material);
							if (this.m_ChildColorNames[i] != null)
							{
								ncInfoCurve.m_ChildOriginalColorValues[i] = renderer.material.GetColor(this.m_ChildColorNames[i]);
							}
							ncInfoCurve.m_ChildBeforeColorValues[i] = Vector4.zero;
						}
					}
					else if (base.GetComponent<Renderer>() != null)
					{
						this.m_ColorName = NcCurveAnimation.Ng_GetMaterialColorName(base.GetComponent<Renderer>().sharedMaterial);
						if (this.m_ColorName != null)
						{
							ncInfoCurve.m_OriginalValue = base.GetComponent<Renderer>().sharedMaterial.GetColor(this.m_ColorName);
						}
						ncInfoCurve.m_BeforeValue = Vector4.zero;
					}
					break;
				case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV:
					if (this.m_NcUvAnimation == null)
					{
						this.m_NcUvAnimation = base.GetComponent<NcUvAnimation>();
					}
					if (this.m_NcUvAnimation)
					{
						ncInfoCurve.m_OriginalValue = new Vector4(this.m_NcUvAnimation.m_fScrollSpeedX, this.m_NcUvAnimation.m_fScrollSpeedY, 0f, 0f);
					}
					ncInfoCurve.m_BeforeValue = ncInfoCurve.m_OriginalValue;
					break;
				}
			}
		}
	}

	private void UpdateAnimation(float fElapsedRate)
	{
		this.m_fElapsedRate = fElapsedRate;
		foreach (NcCurveAnimation.NcInfoCurve ncInfoCurve in this.m_CurveInfoList)
		{
			if (ncInfoCurve.m_bEnabled)
			{
				float num = ncInfoCurve.m_AniCurve.Evaluate(this.m_fElapsedRate);
				if (ncInfoCurve.m_ApplyType != NcCurveAnimation.NcInfoCurve.APPLY_TYPE.COLOR)
				{
					num *= ncInfoCurve.m_fValueScale;
				}
				switch (ncInfoCurve.m_ApplyType)
				{
				case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION:
					if (ncInfoCurve.m_bApplyOption[3])
					{
						this.m_Transform.position += new Vector3(this.GetNextValue(ncInfoCurve, 0, num), this.GetNextValue(ncInfoCurve, 1, num), this.GetNextValue(ncInfoCurve, 2, num));
					}
					else
					{
						this.m_Transform.localPosition += new Vector3(this.GetNextValue(ncInfoCurve, 0, num), this.GetNextValue(ncInfoCurve, 1, num), this.GetNextValue(ncInfoCurve, 2, num));
					}
					break;
				case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.ROTATION:
					if (ncInfoCurve.m_bApplyOption[3])
					{
						this.m_Transform.rotation *= Quaternion.Euler(this.GetNextValue(ncInfoCurve, 0, num), this.GetNextValue(ncInfoCurve, 1, num), this.GetNextValue(ncInfoCurve, 2, num));
					}
					else
					{
						this.m_Transform.localRotation *= Quaternion.Euler(this.GetNextValue(ncInfoCurve, 0, num), this.GetNextValue(ncInfoCurve, 1, num), this.GetNextValue(ncInfoCurve, 2, num));
					}
					break;
				case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE:
					this.m_Transform.localScale += new Vector3(this.GetNextScale(ncInfoCurve, 0, num), this.GetNextScale(ncInfoCurve, 1, num), this.GetNextScale(ncInfoCurve, 2, num));
					break;
				case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.COLOR:
					if (ncInfoCurve.m_bRecursively)
					{
						if (this.m_ChildColorNames != null && this.m_ChildColorNames.Length >= 0)
						{
							for (int i = 0; i < this.m_ChildColorNames.Length; i++)
							{
								if (this.m_ChildColorNames[i] != null && this.m_ChildRenderers[i] != null)
								{
									this.SetChildMaterialColor(ncInfoCurve, num, i);
								}
							}
						}
					}
					else if (base.GetComponent<Renderer>() != null && this.m_ColorName != null)
					{
						if (this.m_MainMaterial == null)
						{
							this.m_MainMaterial = base.GetComponent<Renderer>().material;
						}
						Color color = ncInfoCurve.m_ToColor - ncInfoCurve.m_OriginalValue;
						Color color2 = this.m_MainMaterial.GetColor(this.m_ColorName);
						for (int j = 0; j < 4; j++)
						{
							ref Color ptr = ref color2;
							int num3;
							int num2 = num3 = j;
							float num4 = ptr[num3];
							color2[num2] = num4 + this.GetNextValue(ncInfoCurve, j, color[j] * num);
						}
						this.m_MainMaterial.SetColor(this.m_ColorName, color2);
					}
					break;
				case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV:
					if (this.m_NcUvAnimation)
					{
						this.m_NcUvAnimation.m_fScrollSpeedX += this.GetNextValue(ncInfoCurve, 0, num);
						this.m_NcUvAnimation.m_fScrollSpeedY += this.GetNextValue(ncInfoCurve, 1, num);
					}
					break;
				}
			}
		}
		if (this.m_fDurationTime != 0f && 1f < this.m_fElapsedRate)
		{
			if (!base.IsEndAnimation())
			{
				base.OnEndAnimation();
			}
			if (this.m_bAutoDestruct)
			{
				Object.DestroyObject(base.gameObject);
			}
		}
	}

	private void SetChildMaterialColor(NcCurveAnimation.NcInfoCurve curveInfo, float fValue, int arrayIndex)
	{
		Color color = curveInfo.m_ToColor - curveInfo.m_ChildOriginalColorValues[arrayIndex];
		Color color2 = this.m_ChildRenderers[arrayIndex].material.GetColor(this.m_ChildColorNames[arrayIndex]);
		for (int i = 0; i < 4; i++)
		{
			ref Color ptr = ref color2;
			int num2;
			int num = num2 = i;
			float num3 = ptr[num2];
			color2[num] = num3 + this.GetChildNextColorValue(curveInfo, i, color[i] * fValue, arrayIndex);
		}
		this.m_ChildRenderers[arrayIndex].material.SetColor(this.m_ChildColorNames[arrayIndex], color2);
	}

	private float GetChildNextColorValue(NcCurveAnimation.NcInfoCurve curveInfo, int nIndex, float fValue, int arrayIndex)
	{
		if (curveInfo.m_bApplyOption[nIndex])
		{
			float result = fValue - curveInfo.m_ChildBeforeColorValues[arrayIndex][nIndex];
			curveInfo.m_ChildBeforeColorValues[arrayIndex][nIndex] = fValue;
			return result;
		}
		return 0f;
	}

	private float GetNextValue(NcCurveAnimation.NcInfoCurve curveInfo, int nIndex, float fValue)
	{
		if (curveInfo.m_bApplyOption[nIndex])
		{
			float result = fValue - curveInfo.m_BeforeValue[nIndex];
			curveInfo.m_BeforeValue[nIndex] = fValue;
			return result;
		}
		return 0f;
	}

	private float GetNextScale(NcCurveAnimation.NcInfoCurve curveInfo, int nIndex, float fValue)
	{
		if (curveInfo.m_bApplyOption[nIndex])
		{
			float num = curveInfo.m_OriginalValue[nIndex] * (1f + fValue);
			float result = num - curveInfo.m_BeforeValue[nIndex];
			curveInfo.m_BeforeValue[nIndex] = num;
			return result;
		}
		return 0f;
	}

	public float GetElapsedRate()
	{
		return this.m_fElapsedRate;
	}

	public void CopyTo(NcCurveAnimation target, bool bCurveOnly)
	{
		target.m_CurveInfoList = new List<NcCurveAnimation.NcInfoCurve>();
		foreach (NcCurveAnimation.NcInfoCurve ncInfoCurve in this.m_CurveInfoList)
		{
			target.m_CurveInfoList.Add(ncInfoCurve.GetClone());
		}
		if (!bCurveOnly)
		{
			target.m_fDelayTime = this.m_fDelayTime;
			target.m_fDurationTime = this.m_fDurationTime;
		}
	}

	public void AppendTo(NcCurveAnimation target, bool bCurveOnly)
	{
		if (target.m_CurveInfoList == null)
		{
			target.m_CurveInfoList = new List<NcCurveAnimation.NcInfoCurve>();
		}
		foreach (NcCurveAnimation.NcInfoCurve ncInfoCurve in this.m_CurveInfoList)
		{
			target.m_CurveInfoList.Add(ncInfoCurve.GetClone());
		}
		if (!bCurveOnly)
		{
			target.m_fDelayTime = this.m_fDelayTime;
			target.m_fDurationTime = this.m_fDurationTime;
		}
	}

	public NcCurveAnimation.NcInfoCurve GetCurveInfo(int nIndex)
	{
		if (this.m_CurveInfoList == null || nIndex < 0 || this.m_CurveInfoList.Count <= nIndex)
		{
			return null;
		}
		return this.m_CurveInfoList[nIndex];
	}

	public NcCurveAnimation.NcInfoCurve GetCurveInfo(string curveName)
	{
		if (this.m_CurveInfoList == null)
		{
			return null;
		}
		foreach (NcCurveAnimation.NcInfoCurve ncInfoCurve in this.m_CurveInfoList)
		{
			if (ncInfoCurve.m_CurveName == curveName)
			{
				return ncInfoCurve;
			}
		}
		return null;
	}

	public NcCurveAnimation.NcInfoCurve SetCurveInfo(int nIndex, NcCurveAnimation.NcInfoCurve newInfo)
	{
		if (this.m_CurveInfoList == null || nIndex < 0 || this.m_CurveInfoList.Count <= nIndex)
		{
			return null;
		}
		NcCurveAnimation.NcInfoCurve result = this.m_CurveInfoList[nIndex];
		this.m_CurveInfoList[nIndex] = newInfo;
		return result;
	}

	public int AddCurveInfo()
	{
		NcCurveAnimation.NcInfoCurve ncInfoCurve = new NcCurveAnimation.NcInfoCurve();
		ncInfoCurve.m_AniCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 0f);
		ncInfoCurve.m_AniCurve.AddKey(0.5f, 0.5f);
		if (this.m_CurveInfoList == null)
		{
			this.m_CurveInfoList = new List<NcCurveAnimation.NcInfoCurve>();
		}
		this.m_CurveInfoList.Add(ncInfoCurve);
		return this.m_CurveInfoList.Count - 1;
	}

	public int AddCurveInfo(NcCurveAnimation.NcInfoCurve addCurveInfo)
	{
		if (this.m_CurveInfoList == null)
		{
			this.m_CurveInfoList = new List<NcCurveAnimation.NcInfoCurve>();
		}
		this.m_CurveInfoList.Add(addCurveInfo.GetClone());
		return this.m_CurveInfoList.Count - 1;
	}

	public void DeleteCurveInfo(int nIndex)
	{
		if (this.m_CurveInfoList == null || nIndex < 0 || this.m_CurveInfoList.Count <= nIndex)
		{
			return;
		}
		this.m_CurveInfoList.Remove(this.m_CurveInfoList[nIndex]);
	}

	public void ClearAllCurveInfo()
	{
		if (this.m_CurveInfoList == null)
		{
			return;
		}
		this.m_CurveInfoList.Clear();
	}

	public int GetCurveInfoCount()
	{
		if (this.m_CurveInfoList == null)
		{
			return 0;
		}
		return this.m_CurveInfoList.Count;
	}

	public void SortCurveInfo()
	{
		if (this.m_CurveInfoList == null)
		{
			return;
		}
		this.m_CurveInfoList.Sort(new NcCurveAnimation.NcComparerCurve());
		foreach (NcCurveAnimation.NcInfoCurve ncInfoCurve in this.m_CurveInfoList)
		{
			ncInfoCurve.m_nSortGroup = NcCurveAnimation.NcComparerCurve.GetSortGroup(ncInfoCurve);
		}
	}

	public bool CheckInvalidOption()
	{
		bool result = false;
		for (int i = 0; i < this.m_CurveInfoList.Count; i++)
		{
			if (this.CheckInvalidOption(i))
			{
				result = true;
			}
		}
		return result;
	}

	public bool CheckInvalidOption(int nSrcIndex)
	{
		NcCurveAnimation.NcInfoCurve curveInfo = this.GetCurveInfo(nSrcIndex);
		return curveInfo != null && (curveInfo.m_ApplyType != NcCurveAnimation.NcInfoCurve.APPLY_TYPE.COLOR && curveInfo.m_ApplyType != NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE && curveInfo.m_ApplyType != NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV) && false;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDelayTime /= fSpeedRate;
		this.m_fDurationTime /= fSpeedRate;
	}

	public static string Ng_GetMaterialColorName(Material mat)
	{
		string[] array = new string[]
		{
			"_Color",
			"_TintColor",
			"_EmisColor"
		};
		if (mat != null)
		{
			foreach (string text in array)
			{
				if (mat.HasProperty(text))
				{
					return text;
				}
			}
		}
		return null;
	}

	[SerializeField]
	public List<NcCurveAnimation.NcInfoCurve> m_CurveInfoList;

	public float m_fDelayTime;

	public float m_fDurationTime = 0.6f;

	public bool m_bAutoDestruct = true;

	protected float m_fStartTime;

	protected float m_fElapsedRate;

	protected Transform m_Transform;

	protected string m_ColorName;

	protected Material m_MainMaterial;

	protected string[] m_ChildColorNames;

	protected Renderer[] m_ChildRenderers;

	protected NcUvAnimation m_NcUvAnimation;

	private class NcComparerCurve : IComparer<NcCurveAnimation.NcInfoCurve>
	{
		public int Compare(NcCurveAnimation.NcInfoCurve a, NcCurveAnimation.NcInfoCurve b)
		{
			float num = a.m_AniCurve.Evaluate(NcCurveAnimation.NcComparerCurve.m_fEqualRange / NcCurveAnimation.NcComparerCurve.m_fHDiv) - b.m_AniCurve.Evaluate(NcCurveAnimation.NcComparerCurve.m_fEqualRange / NcCurveAnimation.NcComparerCurve.m_fHDiv);
			if (Mathf.Abs(num) < NcCurveAnimation.NcComparerCurve.m_fEqualRange)
			{
				num = b.m_AniCurve.Evaluate(1f - NcCurveAnimation.NcComparerCurve.m_fEqualRange / NcCurveAnimation.NcComparerCurve.m_fHDiv) - a.m_AniCurve.Evaluate(1f - NcCurveAnimation.NcComparerCurve.m_fEqualRange / NcCurveAnimation.NcComparerCurve.m_fHDiv);
				if (Mathf.Abs(num) < NcCurveAnimation.NcComparerCurve.m_fEqualRange)
				{
					return 0;
				}
			}
			return (int)(num * 1000f);
		}

		public static int GetSortGroup(NcCurveAnimation.NcInfoCurve info)
		{
			float num = info.m_AniCurve.Evaluate(NcCurveAnimation.NcComparerCurve.m_fEqualRange / NcCurveAnimation.NcComparerCurve.m_fHDiv);
			if (num < -NcCurveAnimation.NcComparerCurve.m_fEqualRange)
			{
				return 1;
			}
			if (NcCurveAnimation.NcComparerCurve.m_fEqualRange < num)
			{
				return 3;
			}
			return 2;
		}

		protected static float m_fEqualRange = 0.03f;

		protected static float m_fHDiv = 5f;
	}

	[Serializable]
	public class NcInfoCurve
	{
		public NcInfoCurve()
		{
			bool[] array = new bool[4];
			array[1] = true;
			this.m_bApplyOption = array;
			this.m_fValueScale = 1f;
			this.m_ToColor = Color.white;
			base..ctor();
		}

		public bool IsEnabled()
		{
			return this.m_bEnabled;
		}

		public void SetEnabled(bool bEnable)
		{
			this.m_bEnabled = bEnable;
		}

		public string GetCurveName()
		{
			return this.m_CurveName;
		}

		public NcCurveAnimation.NcInfoCurve GetClone()
		{
			NcCurveAnimation.NcInfoCurve ncInfoCurve = new NcCurveAnimation.NcInfoCurve();
			ncInfoCurve.m_AniCurve = new AnimationCurve(this.m_AniCurve.keys);
			ncInfoCurve.m_AniCurve.postWrapMode = this.m_AniCurve.postWrapMode;
			ncInfoCurve.m_AniCurve.preWrapMode = this.m_AniCurve.preWrapMode;
			ncInfoCurve.m_bEnabled = this.m_bEnabled;
			ncInfoCurve.m_CurveName = this.m_CurveName;
			ncInfoCurve.m_ApplyType = this.m_ApplyType;
			Array.Copy(this.m_bApplyOption, ncInfoCurve.m_bApplyOption, this.m_bApplyOption.Length);
			ncInfoCurve.m_fValueScale = this.m_fValueScale;
			ncInfoCurve.m_bRecursively = this.m_bRecursively;
			ncInfoCurve.m_ToColor = this.m_ToColor;
			ncInfoCurve.m_nTag = this.m_nTag;
			ncInfoCurve.m_nSortGroup = this.m_nSortGroup;
			return ncInfoCurve;
		}

		public void CopyTo(NcCurveAnimation.NcInfoCurve target)
		{
			target.m_AniCurve = new AnimationCurve(this.m_AniCurve.keys);
			target.m_AniCurve.postWrapMode = this.m_AniCurve.postWrapMode;
			target.m_AniCurve.preWrapMode = this.m_AniCurve.preWrapMode;
			target.m_bEnabled = this.m_bEnabled;
			target.m_ApplyType = this.m_ApplyType;
			Array.Copy(this.m_bApplyOption, target.m_bApplyOption, this.m_bApplyOption.Length);
			target.m_fValueScale = this.m_fValueScale;
			target.m_bRecursively = this.m_bRecursively;
			target.m_ToColor = this.m_ToColor;
			target.m_nTag = this.m_nTag;
			target.m_nSortGroup = this.m_nSortGroup;
		}

		public int GetValueCount()
		{
			switch (this.m_ApplyType)
			{
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION:
				return 4;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.ROTATION:
				return 4;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE:
				return 3;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.COLOR:
				return 4;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV:
				return 2;
			}
			return 0;
		}

		public string GetValueName(int nIndex)
		{
			string[] array;
			switch (this.m_ApplyType)
			{
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION:
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.ROTATION:
				array = new string[]
				{
					"X",
					"Y",
					"Z",
					"World"
				};
				goto IL_106;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE:
				array = new string[]
				{
					"X",
					"Y",
					"Z",
					string.Empty
				};
				goto IL_106;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.COLOR:
				array = new string[]
				{
					"R",
					"G",
					"B",
					"A"
				};
				goto IL_106;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV:
				array = new string[]
				{
					"X",
					"Y",
					string.Empty,
					string.Empty
				};
				goto IL_106;
			}
			array = new string[]
			{
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty
			};
			IL_106:
			return array[nIndex];
		}

		public void SetDefaultValueScale()
		{
			switch (this.m_ApplyType)
			{
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION:
				this.m_fValueScale = 1f;
				break;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.ROTATION:
				this.m_fValueScale = 360f;
				break;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE:
				this.m_fValueScale = 1f;
				break;
			case NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV:
				this.m_fValueScale = 10f;
				break;
			}
		}

		public Rect GetFixedDrawRange()
		{
			return new Rect(-0.2f, -1.2f, 1.4f, 2.4f);
		}

		public Rect GetVariableDrawRange()
		{
			Rect result = default(Rect);
			for (int i = 0; i < this.m_AniCurve.keys.Length; i++)
			{
				result.yMin = Mathf.Min(result.yMin, this.m_AniCurve[i].value);
				result.yMax = Mathf.Max(result.yMax, this.m_AniCurve[i].value);
			}
			int num = 20;
			for (int j = 0; j < num; j++)
			{
				float num2 = this.m_AniCurve.Evaluate((float)j / (float)num);
				result.yMin = Mathf.Min(result.yMin, num2);
				result.yMax = Mathf.Max(result.yMax, num2);
			}
			result.xMin = 0f;
			result.xMax = 1f;
			result.xMin -= result.width * 0.2f;
			result.xMax += result.width * 0.2f;
			result.yMin -= result.height * 0.2f;
			result.yMax += result.height * 0.2f;
			return result;
		}

		public Rect GetEditRange()
		{
			return new Rect(0f, -1f, 1f, 2f);
		}

		public void NormalizeCurveTime()
		{
			int i = 0;
			while (i < this.m_AniCurve.keys.Length)
			{
				Keyframe keyframe = this.m_AniCurve[i];
				float num = Mathf.Max(0f, keyframe.time);
				float num2 = Mathf.Min(1f, Mathf.Max(num, keyframe.time));
				if (num2 != keyframe.time)
				{
					Keyframe keyframe2;
					keyframe2..ctor(num2, keyframe.value, keyframe.inTangent, keyframe.outTangent);
					this.m_AniCurve.RemoveKey(i);
					i = 0;
					this.m_AniCurve.AddKey(keyframe2);
				}
				else
				{
					i++;
				}
			}
		}

		protected const float m_fOverDraw = 0.2f;

		public bool m_bEnabled = true;

		public string m_CurveName = string.Empty;

		public AnimationCurve m_AniCurve = new AnimationCurve();

		public static string[] m_TypeName = new string[]
		{
			"None",
			"Position",
			"Rotation",
			"Scale",
			"Color",
			"TextureUV"
		};

		public NcCurveAnimation.NcInfoCurve.APPLY_TYPE m_ApplyType = NcCurveAnimation.NcInfoCurve.APPLY_TYPE.POSITION;

		public bool[] m_bApplyOption;

		public bool m_bRecursively;

		public float m_fValueScale;

		public Vector4 m_ToColor;

		public int m_nTag;

		public int m_nSortGroup;

		public Vector4 m_OriginalValue;

		public Vector4 m_BeforeValue;

		public Vector4[] m_ChildOriginalColorValues;

		public Vector4[] m_ChildBeforeColorValues;

		public enum APPLY_TYPE
		{
			NONE,
			POSITION,
			ROTATION,
			SCALE,
			COLOR,
			TEXTUREUV
		}
	}
}
