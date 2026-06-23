using System;
using UnityEngine;
using Xft;

public class ExampleManager : MonoBehaviour
{
	private void Update()
	{
		if (this.ShowLightBeams)
		{
			this.XLightBeams.ActiveNoInterrupt();
		}
		else
		{
			this.XLightBeams.DeActive();
		}
		if (this.ShowRaining)
		{
			this.XRaining.ActiveNoInterrupt();
		}
		else
		{
			this.XRaining.DeActive();
		}
		if (this.ShowVolumeFog)
		{
			XffectComponent.SetActive(this.XVolumeFog.gameObject, true);
		}
		else
		{
			XffectComponent.SetActive(this.XVolumeFog.gameObject, false);
		}
		if (this.ShowWaterfall)
		{
			this.XWaterfall.ActiveNoInterrupt();
		}
		else
		{
			this.XWaterfall.DeActive();
		}
		if (this.ShowPortalCone)
		{
			this.XPortalCone.ActiveNoInterrupt();
		}
		else
		{
			this.XPortalCone.DeActive();
		}
		if (this.ShowTadpoleGate)
		{
			this.XTadpoleGate.ActiveNoInterrupt();
		}
		else
		{
			this.XTadpoleGate.DeActive();
		}
		if (this.ShowCrystalEnergy)
		{
			this.XCrystalEnergy.ActiveNoInterrupt();
		}
		else
		{
			this.XCrystalEnergy.DeActive();
		}
		if (this.ShowSurroundSoul)
		{
			this.XSurroundSoul.ActiveNoInterrupt();
		}
		else
		{
			this.XSurroundSoul.DeActive();
		}
		if (this.ShowPhantomSwordSlash)
		{
			this.XPhantomSwordSlash.ActiveNoInterrupt();
		}
		else
		{
			this.XPhantomSwordSlash.DeActive();
		}
		if (this.ShowPhantomSword)
		{
			this.XPhantomSword.ActiveNoInterrupt();
		}
		else
		{
			this.XPhantomSword.DeActive();
		}
		if (this.ShowTransformSpell)
		{
			this.XTransformSpell.ActiveNoInterrupt();
		}
		else
		{
			this.XTransformSpell.DeActive();
		}
		if (this.ShowSpreadSlash)
		{
			this.XSpreadSlash.ActiveNoInterrupt();
		}
		else
		{
			this.XSpreadSlash.DeActive();
		}
		if (this.ShowPinkSoul)
		{
			this.XPinkSoul.ActiveNoInterrupt();
		}
		else
		{
			this.XPinkSoul.DeActive();
		}
		if (this.ShowSakura)
		{
			this.XSakura.ActiveNoInterrupt();
		}
		else
		{
			this.XSakura.DeActive();
		}
		if (this.ShowIceImpact)
		{
			this.XIceImpact.ActiveNoInterrupt();
		}
		else
		{
			this.XIceImpact.DeActive();
		}
		if (this.ShowWindowLight)
		{
			this.XWindowLight.ActiveNoInterrupt();
		}
		else
		{
			this.XWindowLight.DeActive();
		}
		if (this.ShowVolumetricLight1)
		{
			this.XVolumetricLight1.ActiveNoInterrupt();
		}
		else
		{
			this.XVolumetricLight1.DeActive();
		}
		if (this.ShowVolumetricLight2)
		{
			this.XVolumetricLight2.ActiveNoInterrupt();
		}
		else
		{
			this.XVolumetricLight2.DeActive();
		}
		if (this.ShowSuckBlood)
		{
			this.XSuckBlood.ActiveNoInterrupt();
		}
		else
		{
			this.XSuckBlood.DeActive();
		}
		if (this.ShowCyclone2)
		{
			this.Cyclone2.ActiveNoInterrupt();
		}
		else
		{
			this.Cyclone2.DeActive();
		}
		if (this.ShowCyclone3)
		{
			this.Cyclone3.ActiveNoInterrupt();
		}
		else
		{
			this.Cyclone3.DeActive();
		}
		if (this.ShowDevilTrigger)
		{
			this.DevilTrigger.ActiveNoInterrupt();
		}
		else
		{
			this.DevilTrigger.DeActive();
		}
		if (this.ShowMeshFogVolume)
		{
			this.MeshFogVolume.ActiveNoInterrupt();
		}
		else
		{
			this.MeshFogVolume.DeActive();
		}
		if (this.ShowGlowTrails)
		{
			this.GlowTrails.ActiveNoInterrupt();
		}
		else
		{
			this.GlowTrails.DeActive();
		}
	}

	private void LateUpdate()
	{
		if (!XffectComponent.IsActive(this.XCollisionTest1.gameObject) && !XffectComponent.IsActive(this.XCollisionTest2.gameObject))
		{
			if (this.Colliders.gameObject)
			{
				foreach (object obj in this.Colliders)
				{
					Transform transform = (Transform)obj;
					XffectComponent.SetActive(transform.gameObject, false);
				}
			}
			XffectComponent.SetActive(this.Colliders.gameObject, false);
		}
		this.m_checkTime += Time.deltaTime;
		if (this.m_checkTime > 5f)
		{
			XffectComponent.SetActive(this.BackgroundWallBottom.gameObject, false);
		}
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(150f, 0f, 350f, 25f), "left button to rotate, middle button to pan, wheel to zoom.");
		GUI.Label(new Rect(200f, 18f, 200f, 25f), "xffect pro version 3.0.0");
		this.ScrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, 140f, 600f), this.ScrollPosition, new Rect(0f, 0f, 140f, 800f));
		this.ShowLightBeams = GUI.Toggle(new Rect(10f, 0f, 80f, 20f), this.ShowLightBeams, "LightBeams");
		this.ShowRaining = GUI.Toggle(new Rect(10f, 20f, 80f, 20f), this.ShowRaining, "Raining");
		this.ShowVolumeFog = GUI.Toggle(new Rect(10f, 40f, 80f, 20f), this.ShowVolumeFog, "VolumeFog");
		this.ShowWaterfall = GUI.Toggle(new Rect(10f, 60f, 80f, 20f), this.ShowWaterfall, "Waterfall");
		this.ShowPortalCone = GUI.Toggle(new Rect(10f, 100f, 80f, 20f), this.ShowPortalCone, "PortalCone");
		this.ShowTadpoleGate = GUI.Toggle(new Rect(10f, 120f, 80f, 20f), this.ShowTadpoleGate, "TadpoleGate");
		this.ShowCrystalEnergy = GUI.Toggle(new Rect(10f, 140f, 120f, 20f), this.ShowCrystalEnergy, "CrystalEnergy");
		this.ShowSurroundSoul = GUI.Toggle(new Rect(10f, 160f, 120f, 20f), this.ShowSurroundSoul, "SwingAround");
		if (GUI.Button(new Rect(10f, 80f, 120f, 20f), "RadialEnergy"))
		{
			XffectComponent.SetActive(this.BackgroundWallBottom.gameObject, true);
			this.m_checkTime = 0f;
			this.XRadialEnergy.ActiveNoInterrupt();
		}
		if (GUI.Button(new Rect(10f, 190f, 80f, 20f), "collision1"))
		{
			this.XCollisionTest1.ActiveNoInterrupt();
			foreach (object obj in this.Colliders)
			{
				Transform transform = (Transform)obj;
				XffectComponent.SetActive(transform.gameObject, true);
			}
			XffectComponent.SetActive(this.Colliders.gameObject, true);
		}
		if (GUI.Button(new Rect(10f, 210f, 80f, 20f), "collision2"))
		{
			this.XCollisionTest2.ActiveNoInterrupt();
			this.XCollisionTest2.SetCollisionGoalPos(this.CollisionGoal);
			foreach (object obj2 in this.Colliders)
			{
				Transform transform2 = (Transform)obj2;
				XffectComponent.SetActive(transform2.gameObject, true);
			}
			XffectComponent.SetActive(this.Colliders.gameObject, true);
		}
		if (GUI.Button(new Rect(10f, 240f, 80f, 20f), "missile1"))
		{
			this.XMissile1.ActiveNoInterrupt();
			this.XMissile1.transform.position = new Vector3(0f, 0f, 50f);
			SimpleMissile component = this.XMissile1.transform.GetComponent<SimpleMissile>();
			component.Reset();
		}
		if (GUI.Button(new Rect(10f, 260f, 80f, 20f), "missile2"))
		{
			this.XMissile2.ActiveNoInterrupt();
			this.XMissile2.transform.position = new Vector3(0f, 0f, 40f);
			SimpleMissile component2 = this.XMissile2.transform.GetComponent<SimpleMissile>();
			component2.Reset();
		}
		if (GUI.Button(new Rect(10f, 280f, 80f, 20f), "missile3"))
		{
			this.XMissile3.ActiveNoInterrupt();
			this.XMissile3.transform.position = new Vector3(0f, 0f, 40f);
			SimpleMissile component3 = this.XMissile3.transform.GetComponent<SimpleMissile>();
			component3.Reset();
		}
		if (GUI.Button(new Rect(10f, 300f, 80f, 20f), "missile4"))
		{
			this.XMissile4.transform.position = new Vector3(0f, 0f, 40f);
			this.XMissile4.ActiveNoInterrupt();
			SimpleMissile component4 = this.XMissile4.transform.GetComponent<SimpleMissile>();
			component4.Reset();
		}
		if (GUI.Button(new Rect(10f, 330f, 80f, 20f), "explosion1"))
		{
			XffectComponent.SetActive(this.BackgroundWallBottom.gameObject, true);
			this.m_checkTime = 0f;
			this.XExplode1.ActiveNoInterrupt();
		}
		if (GUI.Button(new Rect(10f, 350f, 80f, 20f), "explosion2"))
		{
			this.XExplode2.ActiveNoInterrupt();
		}
		if (GUI.Button(new Rect(10f, 370f, 80f, 20f), "explosion3"))
		{
			this.XExplode3.ActiveNoInterrupt();
		}
		if (GUI.Toggle(new Rect(10f, 390f, 120f, 20f), this.ShowPhantomSwordSlash, "PhantomSlash"))
		{
			this.ShowPhantomSwordSlash = true;
			this.m_checkTime = 0f;
			XffectComponent.SetActive(this.BackgroundWallBottom.gameObject, true);
		}
		else
		{
			this.ShowPhantomSwordSlash = false;
		}
		this.ShowPhantomSword = GUI.Toggle(new Rect(10f, 410f, 120f, 20f), this.ShowPhantomSword, "PhantomSword");
		this.ShowTransformSpell = GUI.Toggle(new Rect(10f, 430f, 120f, 20f), this.ShowTransformSpell, "TransformSpell");
		if (GUI.Button(new Rect(10f, 450f, 100f, 20f), "BombAffector"))
		{
			this.XBombAffector.ActiveNoInterrupt();
		}
		this.ShowSpreadSlash = GUI.Toggle(new Rect(10f, 470f, 120f, 20f), this.ShowSpreadSlash, "SpreadSlash");
		this.ShowPinkSoul = GUI.Toggle(new Rect(10f, 490f, 120f, 20f), this.ShowPinkSoul, "PinkSoul");
		this.ShowSakura = GUI.Toggle(new Rect(10f, 510f, 120f, 20f), this.ShowSakura, "Sakura");
		if (GUI.Button(new Rect(10f, 530f, 100f, 20f), "LevelUp"))
		{
			this.XLevelUp.ActiveNoInterrupt();
		}
		this.ShowIceImpact = GUI.Toggle(new Rect(10f, 550f, 120f, 20f), this.ShowIceImpact, "IceImpact");
		if (GUI.Button(new Rect(10f, 570f, 80f, 20f), "Dissolve"))
		{
			XffectComponent.SetActive(this.BackgroundWallBottom.gameObject, true);
			this.m_checkTime = 0f;
			this.XDissolve.ActiveNoInterrupt();
		}
		this.ShowWindowLight = GUI.Toggle(new Rect(10f, 590f, 120f, 20f), this.ShowWindowLight, "WindowLight");
		this.ShowVolumetricLight1 = GUI.Toggle(new Rect(10f, 610f, 120f, 20f), this.ShowVolumetricLight1, "VolumetricLight1");
		this.ShowVolumetricLight2 = GUI.Toggle(new Rect(10f, 630f, 120f, 20f), this.ShowVolumetricLight2, "VolumetricLight2");
		this.ShowSuckBlood = GUI.Toggle(new Rect(10f, 650f, 120f, 20f), this.ShowSuckBlood, "SuckBlood");
		if (GUI.Button(new Rect(10f, 670f, 120f, 20f), "Cyclone1"))
		{
			this.Cyclone1.ActiveNoInterrupt();
		}
		this.ShowCyclone2 = GUI.Toggle(new Rect(10f, 690f, 120f, 20f), this.ShowCyclone2, "Cyclone2");
		this.ShowCyclone3 = GUI.Toggle(new Rect(10f, 710f, 120f, 20f), this.ShowCyclone3, "Cyclone3");
		this.ShowDevilTrigger = GUI.Toggle(new Rect(10f, 730f, 120f, 20f), this.ShowDevilTrigger, "Devil Trigger");
		this.ShowMeshFogVolume = GUI.Toggle(new Rect(10f, 750f, 120f, 20f), this.ShowMeshFogVolume, "MeshFogVolume");
		this.ShowGlowTrails = GUI.Toggle(new Rect(10f, 770f, 120f, 20f), this.ShowGlowTrails, "GlowTrails");
		GUI.EndScrollView();
	}

	private void OnSpreadHit(CollisionParam param)
	{
		this.EffectCache.ReleaseEffect("exp_small", param.CollidePos);
	}

	private void OnConcentrateHit(CollisionParam param)
	{
		this.EffectCache.ReleaseEffect("exp_small", param.CollidePos);
	}

	public XffectCache EffectCache;

	public CompositeXffect XLightBeams;

	protected bool ShowLightBeams;

	public XffectComponent XRaining;

	protected bool ShowRaining;

	public XftVolumeFogObject XVolumeFog;

	protected bool ShowVolumeFog;

	public XffectComponent XWaterfall;

	protected bool ShowWaterfall;

	public XffectComponent XPortalCone;

	protected bool ShowPortalCone;

	public XffectComponent XTadpoleGate;

	protected bool ShowTadpoleGate;

	public CompositeXffect XCrystalEnergy;

	protected bool ShowCrystalEnergy;

	public XffectComponent XSurroundSoul;

	protected bool ShowSurroundSoul;

	public XffectComponent XPhantomSwordSlash;

	protected bool ShowPhantomSwordSlash;

	public XffectComponent XPhantomSword;

	protected bool ShowPhantomSword;

	public XffectComponent XTransformSpell;

	protected bool ShowTransformSpell;

	public XffectComponent XSpreadSlash;

	protected bool ShowSpreadSlash;

	public XffectComponent XPinkSoul;

	protected bool ShowPinkSoul;

	public XffectComponent XSakura;

	protected bool ShowSakura;

	public CompositeXffect XWindowLight;

	protected bool ShowWindowLight;

	public Transform Colliders;

	public Transform CollisionGoal;

	public XffectComponent XCollisionTest1;

	public XffectComponent XCollisionTest2;

	public XffectComponent XMissile1;

	public XffectComponent XMissile2;

	public XffectComponent XMissile3;

	public XffectComponent XMissile4;

	public XffectComponent XExplode1;

	public XffectComponent XExplode2;

	public CompositeXffect XExplode3;

	public Transform BackgroundWallBottom;

	public XffectComponent XBombAffector;

	public XffectComponent XLevelUp;

	public XffectComponent XIceImpact;

	protected bool ShowIceImpact;

	public CompositeXffect XDissolve;

	public XffectComponent XRadialEnergy;

	public XffectComponent XVolumetricLight1;

	protected bool ShowVolumetricLight1;

	public XffectComponent XVolumetricLight2;

	public CompositeXffect XSuckBlood;

	protected bool ShowSuckBlood;

	protected bool ShowVolumetricLight2;

	public XffectComponent Cyclone1;

	public XffectComponent Cyclone2;

	public XffectComponent Cyclone3;

	public XffectComponent DevilTrigger;

	public XffectComponent MeshFogVolume;

	public XffectComponent GlowTrails;

	protected bool ShowCyclone2;

	protected bool ShowCyclone3;

	protected bool ShowDevilTrigger;

	protected bool ShowMeshFogVolume;

	protected bool ShowGlowTrails;

	protected Vector2 ScrollPosition = Vector2.zero;

	protected float m_checkTime;
}
