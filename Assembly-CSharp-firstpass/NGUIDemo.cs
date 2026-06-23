using System;
using System.Collections.Generic;
using HTMLEngine;
using HTMLEngine.NGUI;
using UnityEngine;

public class NGUIDemo : MonoBehaviour
{
	public void Awake()
	{
		Debug.Log("Initializing Demo");
		HtEngine.RegisterDevice(new NGUIDevice());
		HtEngine.LinkHoverColor = HtColor.Parse("#FF4444");
		HtEngine.LinkPressedFactor = 0.5f;
		HtEngine.LinkFunctionName = "onLinkClicked";
		this.html = base.GetComponent<NGUIHTML>();
		this.html.html = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<p id='introduction' align=center>This is based on Profixy's HTMLEngine-Mini.<br>URL:&nbsp;<u>http://html-engine-mini.googlecode.com/</u><br></p>\r\n<br><p><i>Here is no any &lt;html&gt;, &lt;body&gt; etc tags. Only small subset of tags supported yet:</i></p>\r\n<br>\r\n<p><code>- &lt;<font color=yellow>p</font>&nbsp;[id='...'] [align=(left | right | center | justify)]<br>[valign=(top | middle | bottom)]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>spin</font>&nbsp;[id='...'] [width=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>img</font>&nbsp;src='...' [id='...'] [fps=...] [width=...] [height=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>font</font>&nbsp;[face=...] [size=...] [color=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>effect</font>&nbsp;name=... [amount=...] [color=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>br</font>&gt; &lt;<font color=yellow>b</font>&gt; &lt;<font color=yellow>i</font>&gt; &lt;<font color=yellow>u</font>&gt; &lt;<font color=yellow>s</font>&gt; &lt;<font color=yellow>code</font>&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>a</font>&nbsp;href='...'&gt;</code></p>\r\n<br>\r\n<p><i>Also this demo contains some internal resources (fonts and images) than can be used.</i></p>\r\n<br>\r\n<p><b>Available fonts:&nbsp;</b>'default16', 'default16b', 'default16bi', 'default16i', 'title24'</p>\r\n<p><b>Available atleses:&nbsp;</b>'smiles', 'logos', 'faces'</p>\r\n";
	}

	public void FixedUpdate()
	{
		if (this.updateTime)
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.name == "time")
				{
					UILabel component = transform.GetComponent<UILabel>();
					if (component != null)
					{
						DateTime now = DateTime.Now;
						component.text = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", new object[]
						{
							now.Hour,
							now.Minute,
							now.Second,
							now.Millisecond
						});
					}
				}
			}
		}
	}

	internal void onBtnDemoClicked(GameObject senderGo)
	{
		this.updateTime = false;
		string name = senderGo.name;
		if (name != null)
		{
			if (NGUIDemo.<>f__switch$map0 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(7);
				dictionary.Add("BtnDemo1", 0);
				dictionary.Add("BtnDemo2", 1);
				dictionary.Add("BtnDemo3", 2);
				dictionary.Add("BtnDemo4", 3);
				dictionary.Add("BtnDemo5", 4);
				dictionary.Add("BtnDemo6", 5);
				dictionary.Add("BtnDemo7", 6);
				NGUIDemo.<>f__switch$map0 = dictionary;
			}
			int num;
			if (NGUIDemo.<>f__switch$map0.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					this.html.html = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<p id='introduction' align=center>This is based on Profixy's HTMLEngine-Mini.<br>URL:&nbsp;<u>http://html-engine-mini.googlecode.com/</u><br></p>\r\n<br><p><i>Here is no any &lt;html&gt;, &lt;body&gt; etc tags. Only small subset of tags supported yet:</i></p>\r\n<br>\r\n<p><code>- &lt;<font color=yellow>p</font>&nbsp;[id='...'] [align=(left | right | center | justify)]<br>[valign=(top | middle | bottom)]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>spin</font>&nbsp;[id='...'] [width=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>img</font>&nbsp;src='...' [id='...'] [fps=...] [width=...] [height=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>font</font>&nbsp;[face=...] [size=...] [color=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>effect</font>&nbsp;name=... [amount=...] [color=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>br</font>&gt; &lt;<font color=yellow>b</font>&gt; &lt;<font color=yellow>i</font>&gt; &lt;<font color=yellow>u</font>&gt; &lt;<font color=yellow>s</font>&gt; &lt;<font color=yellow>code</font>&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>a</font>&nbsp;href='...'&gt;</code></p>\r\n<br>\r\n<p><i>Also this demo contains some internal resources (fonts and images) than can be used.</i></p>\r\n<br>\r\n<p><b>Available fonts:&nbsp;</b>'default16', 'default16b', 'default16bi', 'default16i', 'title24'</p>\r\n<p><b>Available atleses:&nbsp;</b>'smiles', 'logos', 'faces'</p>\r\n";
					break;
				case 1:
					this.html.html = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<p align=left>Without effect:</p>\r\n<p align=center>Normal text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></p>\r\n<p align=center><b>Bold text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></b></p>\r\n<p align=center><i>Italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></p>\r\n<p align=center><b><i>Bold and italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></b></p>\r\n<p align=left>Shadow effect:</p>\r\n<effect name=shadow color=black>\r\n<p align=center>Normal text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></p>\r\n<p align=center><b>Bold text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></b></p>\r\n<p align=center><i>Italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></p>\r\n<p align=center><b><i>Bold and italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></b></p>\r\n</effect>\r\n<p align=left>Outline effect:</p>\r\n<effect name=outline color=black>\r\n<p align=center>Normal text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></p>\r\n<p align=center><b>Bold text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></b></p>\r\n<p align=center><i>Italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></p>\r\n<p align=center><b><i>Bold and italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></b></p>\r\n</effect>\r\n";
					break;
				case 2:
					this.html.html = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<font size=24>\r\n<br><spin id='outlined' align=center><effect name=outline color=#FFFFFF80><font color=black>Outlined text</font></effect></spin>\r\n<p align=center><effect name=outline color=yellow><font color=black>Outlined yellow text</font></effect></p>\r\n<p align=center><effect name=outline color=#FFFFFF80 amount=2>Some stuppid effect i got</effect></p>\r\n<p align=center><effect name=shadow>Default shadowed text</effect></p>\r\n<p align=center><effect name=shadow color=black>Strong-shadowed text</effect></p>\r\n<p align=center><effect name=shadow color=#FFFFFF80 amount=2>Some shadowed text</effect></p>\r\n</font>\r\n    ";
					break;
				case 3:
					this.html.html = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<br><p align=justify>Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text.</p>\r\n<br><p align=center><font color=gray>Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text.</font></p>\r\n<br><p align=right>Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text.</p>\r\n<br><p align=left><font color=gray>Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text.</font></p>\r\n        ";
					break;
				case 4:
					this.html.html = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<br><p align=center valign=top>Picture <img src='smiles/sad'> with &lt;p valign=top&gt;</p>\r\n<br><p align=center valign=middle>Picture <img src='smiles/smile'> with &lt;p valign=middle&gt; much better than others in this case <img src='smiles/cool'></p>\r\n<br><p align=center valign=bottom>Picture <img src='smiles/sad'> with &lt;p valign=bottom&gt;</p>\r\n<br><p align=center valign=bottom>Picture <img src='faces/power_' fps=10 id='anim'> with &lt;img fps=10&gt;</p>\r\n<br><p align=justify valign=bottom><img src='logos/unity'> is a feature rich, fully integrated development engine for the creation of interactive 3D content. It provides complete, out-of-the-box functionality to assemble high-quality, high-performing content and publish to multiple platforms.</p>\r\n<br><p align=center><img src='logos/unity2'></p>\r\n        ";
					break;
				case 5:
					this.html.html = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<br><p align=center>Now we try to make something dynamic inside text markup...</p>\r\n<br><p align=center valign=middle>Due to performance we can not parse text every frame <img src='smiles/sad'>, but we can reserve some place to draw things inside compiled html! <img src='smiles/cool'></p>\r\n<br><p align=center>It's possible with img tag. Look at source.</p>\r\n<br><p align=center><img src='#time'></p>\r\n<br><p align=center>With same technique we can render animated pictures and even results from render targets.</p>\r\n";
					this.updateTime = true;
					break;
				case 6:
					this.html.html = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<br><p align=center>Links support</p>\r\n<br><p align=left valign=middle>1)&nbsp;<a href='plaintextlink' id='simple'>Simple plain text link.</a></p>\r\n<br><p align=left valign=middle>2)&nbsp;<a href='textandimage'>Simple text and <img src='smiles/smile'> image link.</a></p>\r\n<br><p align=left valign=middle>3)&nbsp;<a href='biglink1'>Multiline link <img src='smiles/smile'>.</a>&nbsp;<a href='biglink2'>Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>.</a></p>\r\n<br><br><p align=center>Try to click around and see to left-bottom corner for results</p>\r\n<br><br><p align=center>At last we have some basic stuff to interact with player</p>\r\n";
					break;
				}
			}
		}
	}

	internal void onLinkClicked(GameObject senderGo)
	{
		NGUILinkText component = senderGo.GetComponent<NGUILinkText>();
		if (component != null)
		{
			Debug.Log(component.linkText);
			if (this.lastLinkText != null)
			{
				this.lastLinkText.text = "Last Link Text: [FFFF00]" + component.linkText + "[-]";
			}
		}
	}

	private const string demo0 = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<p id='introduction' align=center>This is based on Profixy's HTMLEngine-Mini.<br>URL:&nbsp;<u>http://html-engine-mini.googlecode.com/</u><br></p>\r\n<br><p><i>Here is no any &lt;html&gt;, &lt;body&gt; etc tags. Only small subset of tags supported yet:</i></p>\r\n<br>\r\n<p><code>- &lt;<font color=yellow>p</font>&nbsp;[id='...'] [align=(left | right | center | justify)]<br>[valign=(top | middle | bottom)]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>spin</font>&nbsp;[id='...'] [width=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>img</font>&nbsp;src='...' [id='...'] [fps=...] [width=...] [height=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>font</font>&nbsp;[face=...] [size=...] [color=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>effect</font>&nbsp;name=... [amount=...] [color=...]&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>br</font>&gt; &lt;<font color=yellow>b</font>&gt; &lt;<font color=yellow>i</font>&gt; &lt;<font color=yellow>u</font>&gt; &lt;<font color=yellow>s</font>&gt; &lt;<font color=yellow>code</font>&gt;</code></p>\r\n<p><code>- &lt;<font color=yellow>a</font>&nbsp;href='...'&gt;</code></p>\r\n<br>\r\n<p><i>Also this demo contains some internal resources (fonts and images) than can be used.</i></p>\r\n<br>\r\n<p><b>Available fonts:&nbsp;</b>'default16', 'default16b', 'default16bi', 'default16i', 'title24'</p>\r\n<p><b>Available atleses:&nbsp;</b>'smiles', 'logos', 'faces'</p>\r\n";

	private const string demo1 = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<p align=left>Without effect:</p>\r\n<p align=center>Normal text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></p>\r\n<p align=center><b>Bold text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></b></p>\r\n<p align=center><i>Italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></p>\r\n<p align=center><b><i>Bold and italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></b></p>\r\n<p align=left>Shadow effect:</p>\r\n<effect name=shadow color=black>\r\n<p align=center>Normal text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></p>\r\n<p align=center><b>Bold text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></b></p>\r\n<p align=center><i>Italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></p>\r\n<p align=center><b><i>Bold and italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></b></p>\r\n</effect>\r\n<p align=left>Outline effect:</p>\r\n<effect name=outline color=black>\r\n<p align=center>Normal text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></p>\r\n<p align=center><b>Bold text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></b></p>\r\n<p align=center><i>Italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></p>\r\n<p align=center><b><i>Bold and italic text&nbsp;<u>underlined</u>&nbsp;<s>striked</s></i></b></p>\r\n</effect>\r\n";

	private const string demo2 = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<font size=24>\r\n<br><spin id='outlined' align=center><effect name=outline color=#FFFFFF80><font color=black>Outlined text</font></effect></spin>\r\n<p align=center><effect name=outline color=yellow><font color=black>Outlined yellow text</font></effect></p>\r\n<p align=center><effect name=outline color=#FFFFFF80 amount=2>Some stuppid effect i got</effect></p>\r\n<p align=center><effect name=shadow>Default shadowed text</effect></p>\r\n<p align=center><effect name=shadow color=black>Strong-shadowed text</effect></p>\r\n<p align=center><effect name=shadow color=#FFFFFF80 amount=2>Some shadowed text</effect></p>\r\n</font>\r\n    ";

	private const string demo3 = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<br><p align=justify>Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text. Justify aligned text.</p>\r\n<br><p align=center><font color=gray>Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text. Centered text.</font></p>\r\n<br><p align=right>Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text. Right aligned text.</p>\r\n<br><p align=left><font color=gray>Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text. Left aligned text.</font></p>\r\n        ";

	private const string demo4 = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<br><p align=center valign=top>Picture <img src='smiles/sad'> with &lt;p valign=top&gt;</p>\r\n<br><p align=center valign=middle>Picture <img src='smiles/smile'> with &lt;p valign=middle&gt; much better than others in this case <img src='smiles/cool'></p>\r\n<br><p align=center valign=bottom>Picture <img src='smiles/sad'> with &lt;p valign=bottom&gt;</p>\r\n<br><p align=center valign=bottom>Picture <img src='faces/power_' fps=10 id='anim'> with &lt;img fps=10&gt;</p>\r\n<br><p align=justify valign=bottom><img src='logos/unity'> is a feature rich, fully integrated development engine for the creation of interactive 3D content. It provides complete, out-of-the-box functionality to assemble high-quality, high-performing content and publish to multiple platforms.</p>\r\n<br><p align=center><img src='logos/unity2'></p>\r\n        ";

	private const string demo5 = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<br><p align=center>Now we try to make something dynamic inside text markup...</p>\r\n<br><p align=center valign=middle>Due to performance we can not parse text every frame <img src='smiles/sad'>, but we can reserve some place to draw things inside compiled html! <img src='smiles/cool'></p>\r\n<br><p align=center>It's possible with img tag. Look at source.</p>\r\n<br><p align=center><img src='#time'></p>\r\n<br><p align=center>With same technique we can render animated pictures and even results from render targets.</p>\r\n";

	private const string demo6 = "<p align=center><font face=title size=24><font color=yellow>HTMLEngine</font>&nbsp;for&nbsp;<font color=lime>Unity3D.GUI</font>&nbsp;and&nbsp;<font color=lime>NGUI</font></font></p>\r\n<br>\r\n<br><p align=center>Links support</p>\r\n<br><p align=left valign=middle>1)&nbsp;<a href='plaintextlink' id='simple'>Simple plain text link.</a></p>\r\n<br><p align=left valign=middle>2)&nbsp;<a href='textandimage'>Simple text and <img src='smiles/smile'> image link.</a></p>\r\n<br><p align=left valign=middle>3)&nbsp;<a href='biglink1'>Multiline link <img src='smiles/smile'>.</a>&nbsp;<a href='biglink2'>Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>. Multiline link <img src='smiles/smile'>.</a></p>\r\n<br><br><p align=center>Try to click around and see to left-bottom corner for results</p>\r\n<br><br><p align=center>At last we have some basic stuff to interact with player</p>\r\n";

	public UILabel lastLinkText;

	public UIScrollBar scrollBar;

	private NGUIHTML html;

	private bool updateTime;
}
