using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class SkillImageMgr
{
	public static void GetSkill32x32ImageFromIconCode(string iconCode, GIcon icon)
	{
		try
		{
			SkillImageMgr.DownloadNetImage(StringUtil.substitute("NetImages/GameRes/Images/Skill/{0}.png", new object[]
			{
				iconCode
			}), icon);
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	public static void GetSkill32x32ImageFromURL(string url, GIcon icon)
	{
		try
		{
			SkillImageMgr.DownloadNetImage(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				url
			}), icon);
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	public static void DownLoaderComplete1(object sender, DownloadEventArgs e)
	{
		Downloader downloader = sender as Downloader;
		if (e.type != DownloadEventArgs.COMPLETE)
		{
			GError.AddErrMsg(StringUtil.substitute(Global.GetLang("从网络下载技能图片失败:{0}"), new object[]
			{
				Global.GetErrorMsg(e)
			}));
		}
		else
		{
			Super.AddNetImageStream(downloader.Args, new BitmapData(0.0, 0.0, true, uint.MaxValue)
			{
				TextureData = (Texture)e.target
			});
			SkillImageMgr.GetImageFromCaching(downloader.Args, null);
		}
		SkillImageMgr.WaitingDownloaderDict.Remove(downloader.Args);
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
				bitmapData.copyPixels(netImageStream, netImageStream.rect1, new Point(0, 0), 0);
				icon.BodySource = new ImageBrush(Super.ConvertBitmapToGrayBitmap(bitmapData, icon.BodyURL.ToGrayBitmap));
			}
			else
			{
				icon.BodySource = new ImageBrush(netImageStream);
			}
		}
		else
		{
			List<GIcon> list = null;
			if (SkillImageMgr.WaitingDownloaderDict.ContainsKey(key))
			{
				SkillImageMgr.WaitingDownloaderDict.TryGetValue(key, ref list);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].BodyURL != null)
					{
						BitmapData bitmapData2 = new BitmapData(netImageStream.width, netImageStream.height, netImageStream.transparent, uint.MaxValue);
						bitmapData2.copyPixels(netImageStream, netImageStream.rect1, new Point(0, 0), 0);
						list[i].BodySource = new ImageBrush(Super.ConvertBitmapToGrayBitmap(bitmapData2, list[i].BodyURL.ToGrayBitmap));
					}
					else
					{
						list[i].BodySource = new ImageBrush(netImageStream);
					}
				}
			}
		}
		return true;
	}

	public static void DownloadNetImage(string value, GIcon icon)
	{
		if (SkillImageMgr.GetImageFromCaching(value, icon))
		{
			return;
		}
		icon.BodySource = new ImageBrush(Global.GetGameResImage(StringUtil.substitute("Images/Plate/default.png", new object[0])));
		List<GIcon> list = null;
		if (SkillImageMgr.WaitingDownloaderDict.ContainsKey(value))
		{
			SkillImageMgr.WaitingDownloaderDict.TryGetValue(value, ref list);
			list.Add(icon);
			return;
		}
		list = new List<GIcon>();
		list.Add(icon);
		SkillImageMgr.WaitingDownloaderDict[value] = list;
		new Downloader(null)
		{
			Args = value,
			Completed = new DownloaderEventHander(SkillImageMgr.DownLoaderComplete1)
		}.GetResourceByVer(Global.WebPath(value), Global.ResSwfVer, false);
	}

	private static Dictionary<string, List<GIcon>> WaitingDownloaderDict = new Dictionary<string, List<GIcon>>();
}
