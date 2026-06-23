using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class GoodsImageMgr
{
	public static void GetGoods32x32ImageFromIconCode(string iconCode, GIcon icon)
	{
		try
		{
			GoodsImageMgr.DownloadNetImage(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				iconCode
			}), icon);
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	public static void GetGoods32x32ImageFromURL(string url, GIcon icon)
	{
		try
		{
			GoodsImageMgr.DownloadNetImage(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				url
			}), icon);
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	public static void GetSales48x48ImageFromURL(string url, GIcon icon)
	{
		try
		{
			GoodsImageMgr.DownloadNetImage(StringUtil.substitute("{0}", new object[]
			{
				url
			}), icon);
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	public static void DownLoaderComplete1(object sender, DownloadEventArgs _e)
	{
		Downloader downloader = sender as Downloader;
		if (_e.type != DownloadEventArgs.COMPLETE)
		{
			GError.AddErrMsg(StringUtil.substitute(Global.GetLang("从网络下载物品图片失败:{0}"), new object[]
			{
				Global.GetErrorMsg(_e)
			}));
		}
		else
		{
			Super.AddNetImageStream(downloader.Args, new BitmapData(0.0, 0.0, true, uint.MaxValue)
			{
				TextureData = (Texture)_e.target
			});
			GoodsImageMgr.GetImageFromCaching(downloader.Args, null);
		}
		GoodsImageMgr.WaitingDownloaderDict.Remove(downloader.Args);
		downloader.Completed = null;
	}

	public static bool GetImageFromCaching(string key, GIcon icon)
	{
		BitmapData netImageStream = Super.GetNetImageStream(key);
		if (netImageStream == null)
		{
			return false;
		}
		if (null != icon)
		{
			if (icon.BodyURL != null)
			{
				BitmapData bitmapData = new BitmapData(netImageStream.width, netImageStream.height, netImageStream.transparent, uint.MaxValue);
				bitmapData.copyPixels(netImageStream, netImageStream.rect, new Point(0, 0), 0);
				icon.BodySource = new ImageBrush(Super.ConvertBitmapToGrayBitmap(bitmapData, icon.BodyURL.ToGrayBitmap));
			}
			else
			{
				icon.BodySource = new ImageBrush(netImageStream);
			}
		}
		else if (GoodsImageMgr.WaitingDownloaderDict.ContainsKey(key))
		{
			List<GIcon> value = GoodsImageMgr.WaitingDownloaderDict.GetValue(key);
			for (int i = 0; i < value.Count; i++)
			{
				if (value[i].BodyURL != null)
				{
					BitmapData bitmapData2 = new BitmapData(netImageStream.width, netImageStream.height, netImageStream.transparent, uint.MaxValue);
					bitmapData2.copyPixels(netImageStream, netImageStream.rect, new Point(0, 0), 0);
					value[i].BodySource = new ImageBrush(Super.ConvertBitmapToGrayBitmap(bitmapData2, value[i].BodyURL.ToGrayBitmap));
				}
				else
				{
					value[i].BodySource = new ImageBrush(netImageStream);
				}
			}
		}
		return true;
	}

	public static void DownloadNetImage(string value, GIcon icon)
	{
		if (GoodsImageMgr.GetImageFromCaching(value, icon))
		{
			return;
		}
		icon.BodySource = new ImageBrush(Global.GetGameResImage(StringUtil.substitute("Images/Plate/default.png", new object[0])));
		List<GIcon> list;
		if (GoodsImageMgr.WaitingDownloaderDict.ContainsKey(value))
		{
			list = GoodsImageMgr.WaitingDownloaderDict.GetValue(value);
			list.Add(icon);
			return;
		}
		list = new List<GIcon>();
		list.Add(icon);
		GoodsImageMgr.WaitingDownloaderDict[value] = list;
		new Downloader(null)
		{
			Args = value,
			Completed = new DownloaderEventHander(GoodsImageMgr.DownLoaderComplete1)
		}.GetResourceByVer(Global.WebPath(value), Global.ResSwfVer, false);
	}

	private static Dictionary<string, List<GIcon>> WaitingDownloaderDict = new Dictionary<string, List<GIcon>>();
}
