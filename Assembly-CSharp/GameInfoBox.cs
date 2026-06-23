using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class GameInfoBox : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.BakPanel);
		this.BakPanel.Visibility = false;
		this.BakPanel.Visibility = false;
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.BakPanel.Width = (double)width;
		this.BakPanel.Height = (double)height;
		this.BakPanel.Visibility = true;
		this.BakPanel.Background = new SolidColorBrush(ColorSL.FromArgb(255, 0, 0, 0));
		this.BakPanel.BackgroundAlpha = 0.6;
		this.ListViewer = new GScrollView(width, (int)this.Height, 0);
		this.Container.Children.Add(this.ListViewer);
		this.ListViewer.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		Canvas.SetLeft(this.ListViewer, 0);
		Canvas.SetTop(this.ListViewer, 0);
		this.ListPanel.Width = this.ListViewer.Width - 10.0;
		this.ListPanel.Height = this.ListViewer.Height;
		this.ListViewer.Viewer = this.ListPanel;
	}

	public void InitPartData()
	{
	}

	public Rect GetBoundsRect()
	{
		Rect result = default(Rect);
		result.x = 0f;
		result.y = 0f;
		result.width = (float)this.Width;
		result.height = (float)this.Height;
		return result;
	}

	public static void InitTextBlockExQueque()
	{
		for (int i = 0; i < 50; i++)
		{
			GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
			gtextBlockOutLine.TextFontWrapping = TextWrapping.Wrap;
			GameInfoBox.TextBlockQueue.Enqueue(gtextBlockOutLine);
		}
	}

	private static GTextBlockOutLine PopupTextBlockEx(GameInfoTextItem gameInfoTextItem, int width)
	{
		GTextBlockOutLine gtextBlockOutLine;
		if (GameInfoBox.TextBlockQueue.Count <= 0)
		{
			gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
			gtextBlockOutLine.TextFontWrapping = TextWrapping.Wrap;
		}
		else
		{
			gtextBlockOutLine = GameInfoBox.TextBlockQueue.Dequeue();
		}
		gtextBlockOutLine.TextColor = new SolidColorBrush(Super.GetGameInfoTextItemColor(gameInfoTextItem));
		gtextBlockOutLine.Width = (double)(width - 8);
		gtextBlockOutLine.Height = double.NaN;
		gtextBlockOutLine.Text = gameInfoTextItem.TextMsg;
		return gtextBlockOutLine;
	}

	private static void PushTextBlockEx(GTextBlockOutLine textBlock)
	{
		if (null == textBlock)
		{
			return;
		}
		textBlock.Text = string.Empty;
		GameInfoBox.TextBlockQueue.Enqueue(textBlock);
	}

	public void AddHintText(GameInfoTextItem gameInfoTextItem)
	{
	}

	private void ListViewer_MouseWheel(object sender, MouseEvent e)
	{
	}

	private void ListViewer_MouseMove(object sender, MouseEvent e)
	{
	}

	private Canvas BakPanel = new Canvas();

	private GScrollView ListViewer;

	private Canvas ListPanel = new Canvas();

	private static Queue<GTextBlockOutLine> TextBlockQueue = new Queue<GTextBlockOutLine>();
}
