using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class InteractionPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		UIEventListener.Get(this.SpriteBak.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnBackClickHandler);
		this.LoadSocialActConfig();
	}

	public void OnBackClickHandler(GameObject ob)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1
			});
		}
	}

	private void LoadSocialActConfig()
	{
		XElement gameResXml = Global.GetGameResXml("Config/SocialAct.xml");
		if (gameResXml == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"Config/SocialAct.xml is null"
			});
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ACT");
		this.InitInteractionSymbolControl(xelementList);
	}

	private void InitInteractionSymbolControl(List<XElement> actFileList)
	{
		if (this.m_symbolList == null || this.m_symbolList.Count == 0)
		{
			this.m_symbolList.Clear();
			this.m_symbolTable.Clear();
			foreach (XElement xmlItem in actFileList)
			{
				InteractionItem interactionItem = U3DUtils.NEW<InteractionItem>();
				U3DUtils.AddChild(this.m_symbolTable.gameObject, interactionItem.gameObject, false);
				interactionItem.RefreshItem(xmlItem);
				this.m_symbolList.Add(interactionItem);
				interactionItem.transform.localPosition = new Vector3(0f, 0f, -1f);
				interactionItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.IDType == 1)
					{
						string title = e.Title;
						if (this.DPSelectedItem != null)
						{
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								IDType = 2,
								Title = title
							});
						}
					}
				};
			}
		}
	}

	private List<InteractionItem> m_symbolList = new List<InteractionItem>();

	public UITable m_symbolTable;

	public UISprite SpriteBak;

	public DPSelectedItemEventHandler DPSelectedItem;
}
