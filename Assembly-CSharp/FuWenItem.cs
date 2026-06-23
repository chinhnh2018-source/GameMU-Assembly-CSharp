using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class FuWenItem : MonoBehaviour
{
	private void Awake()
	{
		this.Gbtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = this.value
			});
		};
	}

	private void Start()
	{
	}

	public void setFuWenProgress(float value, bool isok, bool Pressed = false)
	{
		this.isOK = isok;
		if (isok)
		{
			this.Gbtn.isEnabled = true;
			this.Gbtn.normalSprite = "1_hover";
			this.Gbtn.Refresh();
			this.btnSp.gameObject.SetActive(false);
			if (Pressed)
			{
				this.Gbtn.normalSprite = "1_hover";
				this.Gbtn.Refresh();
				this.choise.SetActive(true);
				if (this.nomal.activeSelf)
				{
					this.nomal.SetActive(false);
				}
			}
			else
			{
				if (this.choise.activeSelf)
				{
					this.choise.SetActive(false);
				}
				this.nomal.SetActive(true);
			}
		}
		else
		{
			this.Gbtn.normalSprite = "1_normal";
			this.Gbtn.Refresh();
		}
	}

	public UISprite btnSp;

	public GameObject nomal;

	public GameObject choise;

	public UILabel barLabel;

	public GButton Gbtn;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int value = 1;

	public bool isOK;
}
