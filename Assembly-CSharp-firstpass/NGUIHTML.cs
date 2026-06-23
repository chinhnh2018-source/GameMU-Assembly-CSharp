using System;
using System.Collections;
using HTMLEngine;
using HTMLEngine.Core;
using UnityEngine;

[AddComponentMenu("HTMLEngine/NGUIHTML")]
public class NGUIHTML : MonoBehaviour
{
	public string html
	{
		get
		{
			return this._html;
		}
		set
		{
			this._html = value;
			this.changed = true;
		}
	}

	public int OffSetX
	{
		get
		{
			return this.mOffSetX;
		}
		set
		{
			this.mOffSetX = value;
		}
	}

	public int OffSetY
	{
		set
		{
			this.mOffSetY = value;
		}
	}

	private void Start()
	{
		this.compiler = HtEngine.GetCompiler();
	}

	private void Update()
	{
		if (this.changed && this.compiler != null)
		{
			this.compiler.Compile(this.html, (this.maxLineWidth <= 0) ? Screen.width : this.maxLineWidth);
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				Object.Destroy(transform.gameObject);
			}
			this.compiler.Offset(this.mOffSetX, this.mOffSetY);
			this.compiler.Draw(Time.deltaTime, base.transform);
			this.changed = false;
			if (this.autoScroll != NGUIHTML.AutoScrollType.MANUAL)
			{
				base.StartCoroutine(this.updateAutoScroll());
			}
		}
	}

	private void OnDestroy()
	{
		if (this.compiler != null)
		{
			this.compiler.Dispose();
			this.compiler = null;
		}
	}

	private IEnumerator updateAutoScroll()
	{
		yield return new WaitForEndOfFrame();
		UIDraggablePanel uiDraggablePanel = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
		if (uiDraggablePanel != null)
		{
			NGUIHTML.AutoScrollType autoScrollType = this.autoScroll;
			if (autoScrollType != NGUIHTML.AutoScrollType.AUTO_TOP)
			{
				if (autoScrollType == NGUIHTML.AutoScrollType.AUTO_BOTTOM)
				{
					uiDraggablePanel.relativePositionOnReset = new Vector2(0f, 1f);
				}
			}
			else
			{
				uiDraggablePanel.relativePositionOnReset = Vector2.zero;
			}
			uiDraggablePanel.ResetPosition();
		}
		yield break;
	}

	public static string GetPureText(string htmlText)
	{
		HtEngine.GetCompiler();
		Reader reader = new Reader();
		string text = string.Empty;
		reader.SetSource(htmlText);
		using (HtmlChunkCollection htmlChunkCollection = OP<HtmlChunkCollection>.Acquire())
		{
			htmlChunkCollection.Read(reader);
			foreach (HtmlChunk htmlChunk in htmlChunkCollection)
			{
				HtmlChunkWord htmlChunkWord = htmlChunk as HtmlChunkWord;
				if (htmlChunkWord != null)
				{
					text += htmlChunkWord.Text;
				}
			}
		}
		return text;
	}

	public static int GetLineCount(string htmlText, int maxLineWidth)
	{
		HtCompiler htCompiler = HtEngine.GetCompiler();
		htCompiler.Compile(htmlText, (maxLineWidth <= 0) ? Screen.width : maxLineWidth);
		return htCompiler.GetLineCount();
	}

	public string _html = string.Empty;

	public int maxLineWidth;

	public NGUIHTML.AutoScrollType autoScroll;

	private bool changed;

	private HtCompiler compiler;

	public bool IsVoiceInfo;

	private int mOffSetX;

	private int mOffSetY;

	public enum AutoScrollType
	{
		MANUAL,
		AUTO_TOP,
		AUTO_BOTTOM
	}
}
