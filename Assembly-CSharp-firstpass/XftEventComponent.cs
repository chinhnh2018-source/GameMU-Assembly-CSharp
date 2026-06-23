using System;
using UnityEngine;
using Xft;

public class XftEventComponent : MonoBehaviour
{
	public void Initialize()
	{
		switch (this.Type)
		{
		case XEventType.CameraShake:
			this.m_eventHandler = new CameraShakeEvent(this);
			break;
		case XEventType.Sound:
			this.m_eventHandler = new SoundEvent(this);
			break;
		case XEventType.Light:
			this.m_eventHandler = new LightEvent(this);
			break;
		case XEventType.CameraEffect:
			if (this.CameraEffectType == CameraEffectEvent.EType.ColorInverse)
			{
				this.m_eventHandler = new ColorInverseEvent(this);
			}
			else if (this.CameraEffectType == CameraEffectEvent.EType.Glow)
			{
				this.m_eventHandler = new GlowEvent(this);
			}
			else if (this.CameraEffectType == CameraEffectEvent.EType.GlowPerObj)
			{
				this.m_eventHandler = new GlowPerObjEvent(this);
			}
			else if (this.CameraEffectType == CameraEffectEvent.EType.RadialBlur)
			{
				this.m_eventHandler = new RadialBlurEvent(this);
			}
			else if (this.CameraEffectType == CameraEffectEvent.EType.RadialBlurMask)
			{
				this.m_eventHandler = new RadialBlurTexAddEvent(this);
			}
			else if (this.CameraEffectType == CameraEffectEvent.EType.Glitch)
			{
				this.m_eventHandler = new GlitchEvent(this);
			}
			break;
		case XEventType.TimeScale:
			this.m_eventHandler = new TimeScaleEvent(this);
			break;
		default:
			Debug.LogWarning("invalid event type!");
			break;
		}
		this.m_eventHandler.Initialize();
		this.m_elapsedTime = 0f;
		this.m_finished = false;
	}

	public void ResetCustom()
	{
		if (this.m_eventHandler != null)
		{
			this.m_eventHandler.Reset();
		}
		this.m_elapsedTime = 0f;
		this.m_finished = false;
	}

	public void UpdateCustom(float deltaTime)
	{
		if (this.m_finished)
		{
			return;
		}
		if (this.m_eventHandler != null)
		{
			this.m_elapsedTime += deltaTime;
			if (!this.m_eventHandler.CanUpdate && this.m_elapsedTime >= this.StartTime && this.StartTime >= 0f)
			{
				this.m_eventHandler.OnBegin();
			}
			if (this.m_eventHandler.CanUpdate)
			{
				this.m_eventHandler.Update(deltaTime);
			}
			if (this.m_eventHandler.CanUpdate && this.m_elapsedTime > this.EndTime && this.EndTime > 0f)
			{
				this.ResetCustom();
				this.m_finished = true;
			}
		}
	}

	public XftEventType EventType;

	public XEventType Type;

	public float StartTime;

	public float EndTime = -1f;

	public CameraEffectEvent.EType CameraEffectType = CameraEffectEvent.EType.Glow;

	public Shader RadialBlurShader;

	public Transform RadialBlurObj;

	public float RBSampleDist = 0.3f;

	public MAGTYPE RBStrengthType;

	public float RBSampleStrength = 1f;

	public AnimationCurve RBSampleStrengthCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	public Shader RadialBlurTexAddShader;

	public Texture2D RadialBlurMask;

	public float RBMaskSampleDist = 3f;

	public MAGTYPE RBMaskStrengthType;

	public float RBMaskSampleStrength = 5f;

	public AnimationCurve RBMaskSampleStrengthCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	public Shader GlowCompositeShader;

	public Shader GlowBlurShader;

	public Shader GlowDownSampleShader;

	public float GlowIntensity = 1.5f;

	public int GlowBlurIterations = 3;

	public float GlowBlurSpread = 0.7f;

	public Color GlowColorStart = new Color(0f, 0.02745098f, 0.819607854f, 0.4392157f);

	public Color GlowColorEnd = new Color(0.298039228f, 0.5882353f, 1f, 1f);

	public COLOR_GRADUAL_TYPE GlowColorGradualType;

	public float GlowColorGradualTime = 2f;

	public AnimationCurve ColorCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	public Shader GlowPerObjReplacementShader;

	public Shader GlowPerObjBlendShader;

	public Shader ColorInverseShader;

	public AnimationCurve CIStrengthCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	public Shader GlitchShader;

	public Texture2D GlitchMask;

	public float MinAmp;

	public float MaxAmp = 0.05f;

	public float MinRand = 0.05f;

	public float MaxRand = 0.85f;

	public int WaveLen = 10;

	public AudioClip Clip;

	public float Volume = 1f;

	public float Pitch = 1f;

	public Vector3 PositionForce = new Vector3(0f, 6f, 0f);

	public Vector3 RotationForce = Vector3.zero;

	public float PositionStifness = 0.3f;

	public float PositionDamping = 0.1f;

	public float RotationStiffness = 0.1f;

	public float RotationDamping = 0.25f;

	public bool UseEarthQuake;

	public float EarthQuakeMagnitude = 2f;

	public MAGTYPE EarthQuakeMagTye;

	public AnimationCurve EarthQuakeMagCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	public float EarthQuakeTime = 2f;

	public float EarthQuakeCameraRollFactor = 0.1f;

	public Light LightComp;

	public float LightIntensity = 1f;

	public MAGTYPE LightIntensityType;

	public AnimationCurve LightIntensityCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	public float LightRange = 10f;

	public MAGTYPE LightRangeType;

	public AnimationCurve LightRangeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 20f)
	});

	public float TimeScale = 1f;

	public float TimeScaleDuration = 1f;

	protected XftEvent m_eventHandler;

	protected float m_elapsedTime;

	protected bool m_finished;
}
