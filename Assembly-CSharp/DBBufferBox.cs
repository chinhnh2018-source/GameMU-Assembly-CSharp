using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using UnityEngine;

public class DBBufferBox : UserControl
{
	public ObservableCollection boxIcons
	{
		get
		{
			return this._boxIcons.ItemsSource;
		}
	}

	protected void OnEnable()
	{
		base.StartCoroutine<bool>(this.TimerProc());
	}

	private IEnumerator TimerProc()
	{
		for (;;)
		{
			yield return new WaitForSeconds(1f);
			this.RefreshDataList();
		}
		yield break;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._Close.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			Object.Destroy(base.gameObject);
		};
		this.IconIDs = ConfigSystemParam.GetSystemParamStringArrayByName("BufferItemsGoodsIDs", ',');
		base.StartCoroutine<bool>(this.TimerProc());
	}

	private string GetIconID(int index)
	{
		if (this.IconIDs == null)
		{
			return "0";
		}
		if (index < 0 || index >= this.IconIDs.Length)
		{
			return "0";
		}
		return this.IconIDs[index];
	}

	public void RefreshDataList()
	{
		if (Global.Data.roleData != null && Global.Data.roleData.BufferDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.BufferDataList.Count; i++)
			{
				this.RefreshData(Global.Data.roleData.BufferDataList[i]);
			}
		}
		this.CheckBufferDataList();
	}

	public void RefreshData(BufferData bufferData)
	{
		if (Global.IsDummyBuffer(bufferData.BufferID))
		{
			return;
		}
		int num = -1;
		int num2 = 0;
		int num3 = -1;
		int count = this.boxIcons.Count;
		int num4 = (count >= 1) ? ((count - 1) / 9 + 1) : 1;
		for (int i = 0; i < count; i++)
		{
			GStillIcon gstillIcon = U3DUtils.AS<GStillIcon>(this.boxIcons[i]);
			if ((gstillIcon.Tag as BufferData).BufferID == bufferData.BufferID)
			{
				gstillIcon.StillImg.ShowImage(DBBufferBox.GetBufferIconString(bufferData, ref num2, ref num3));
				num = i;
				this.UpdateCoolDown(gstillIcon, bufferData);
				break;
			}
		}
		if (Global.IsBufferDataOver(bufferData, Global.GetCorrectLocalTime(), false))
		{
			if (num >= 0)
			{
				this.boxIcons.RemoveAt(num);
			}
			return;
		}
		if (num < 0)
		{
			this.AddIcon(bufferData);
		}
	}

	public static string GetBufferIconString(BufferData bufferData, ref int type, ref int id)
	{
		int num;
		if (bufferData.BufferID == 31)
		{
			id = (int)bufferData.BufferVal + 2000000;
			num = Global.GetGoodsIconCodeByID(id);
			return Global.GetGoodsIconString(num);
		}
		if (bufferData.BufferID == 10005)
		{
			id = (int)bufferData.BufferVal;
			num = Global.GetGoodsIconCodeByID(id);
			return Global.GetGoodsIconString(num);
		}
		if (bufferData.BufferID == 10006)
		{
			id = (int)bufferData.BufferVal;
			num = Global.GetGoodsIconCodeByID(id);
			return Global.GetGoodsIconString(num);
		}
		if (bufferData.BufferID == 87)
		{
			id = (int)bufferData.BufferVal;
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("JunXianBufferGoodsIDs", ',');
			if (systemParamIntArrayByName != null && id < systemParamIntArrayByName.Length)
			{
				num = Global.GetGoodsIconCodeByID(systemParamIntArrayByName[id]);
			}
			else
			{
				num = -1;
			}
			return Global.GetGoodsIconString(num);
		}
		if (bufferData.BufferID >= 52 && bufferData.BufferID <= 59)
		{
			id = (int)bufferData.BufferVal;
			num = Global.GetGoodsIconCodeByID(id);
			return Global.GetGoodsIconString(num);
		}
		if (20000 <= bufferData.BufferID && bufferData.BufferID <= 29999)
		{
			id = Global.GetSpecialTitleBuffGoodsID(bufferData.BufferID);
			num = Global.GetGoodsIconCodeByID(id);
			return Global.GetGoodsIconString(num);
		}
		if ((bufferData.BufferID >= 60 && bufferData.BufferID <= 63) || bufferData.BufferID == 71)
		{
			type = 1;
			int bufferID = bufferData.BufferID;
			switch (bufferID)
			{
			case 60:
				num = ConfigMagicInfos.GetSkillIconIDByID(103);
				id = 101;
				break;
			case 61:
				num = ConfigMagicInfos.GetSkillIconIDByID(205);
				id = 102;
				break;
			case 62:
				num = ConfigMagicInfos.GetSkillIconIDByID(303);
				id = 103;
				break;
			case 63:
				num = ConfigMagicInfos.GetSkillIconIDByID(304);
				id = 104;
				break;
			default:
				if (bufferID != 71)
				{
					num = ConfigMagicInfos.GetSkillIconIDByID(100);
					id = 100;
				}
				else
				{
					num = ConfigMagicInfos.GetSkillIconIDByID(304);
					id = 103;
				}
				break;
			}
			return Global.GetSkillBuffIconString(num);
		}
		if (bufferData.BufferID >= 72 && bufferData.BufferID <= 84)
		{
			type = 1;
			id = (int)(bufferData.BufferVal >> 32);
			return Global.GetSkillBuffIconString(id);
		}
		if (bufferData.BufferID == 102)
		{
			type = 1;
			id = (int)(bufferData.BufferVal >> 32);
			return Global.GetSkillBuffIconString(id);
		}
		if (bufferData.BufferID >= 64 && bufferData.BufferID <= 70)
		{
			id = (int)bufferData.BufferVal;
			num = Global.GetGoodsIconCodeByID(id);
			return Global.GetGoodsIconString(num);
		}
		if (bufferData.BufferID >= 88 && bufferData.BufferID <= 91)
		{
			id = AdvanceBufferPropsMgr.GetGoodsID((BufferItemTypes)bufferData.BufferID, (int)bufferData.BufferVal);
			num = Global.GetGoodsIconCodeByID(id);
			return Global.GetGoodsIconString(num);
		}
		if (bufferData.BufferID == 2080010 || bufferData.BufferID == 2080011)
		{
			id = bufferData.BufferID;
			num = Global.GetGoodsIconCodeByID(id);
			return Global.GetGoodsIconString(num);
		}
		if (bufferData.BufferID == 2080002)
		{
			id = (int)bufferData.BufferVal;
			num = Global.GetGoodsIconCodeByID(id);
			return Global.GetGoodsIconString(num);
		}
		if (bufferData.BufferID < 85 || bufferData.BufferID > 86)
		{
			if (bufferData.BufferID >= 31 && bufferData.BufferID <= 33)
			{
				id = Global.GetBufferGoodsID(bufferData.BufferID, (int)bufferData.BufferVal);
				num = Global.GetGoodsIconCodeByID(id);
			}
			else if (bufferData.BufferID == 28)
			{
				id = (int)bufferData.BufferVal;
				num = Global.GetGoodsIconCodeByID(id);
			}
			else if (bufferData.BufferID == 30)
			{
				num = 36002;
				id = num;
			}
			else if (bufferData.BufferID == 34)
			{
				num = 31021;
				id = num;
			}
			else if (bufferData.BufferID == 13)
			{
				num = Global.GetGoodsIconCodeByID(Global.GetVipBindGoodsID());
			}
			else if (bufferData.BufferID == 92)
			{
				id = (int)bufferData.BufferVal;
				num = Global.GetGoodsIconCodeByID(id);
			}
			else if (bufferData.BufferID == 8999)
			{
				id = (int)bufferData.BufferVal;
				num = Global.GetGoodsIconCodeByID(id);
			}
			else if (bufferData.BufferID == 9000 || bufferData.BufferID == 9001 || bufferData.BufferID == 9002)
			{
				id = (int)bufferData.BufferVal;
				num = Global.GetGoodsIconCodeByID(id);
			}
			else if (bufferData.BufferID >= 9012 && bufferData.BufferID <= 9017)
			{
				id = (int)bufferData.BufferVal;
				num = Global.GetGoodsIconCodeByID(id);
			}
			else if (bufferData.BufferID >= 2000853 && bufferData.BufferID <= 2000857)
			{
				id = (int)bufferData.BufferVal;
				num = Global.GetGoodsIconCodeByID(id);
			}
			else if (bufferData.BufferID == 121)
			{
				id = (int)bufferData.BufferVal;
				num = Global.GetGoodsIconCodeByID(id);
			}
			else if (bufferData.BufferID == 2090001)
			{
				int[] systemParamIntArrayByName2 = ConfigSystemParam.GetSystemParamIntArrayByName("EscapeBuffID", ',');
				id = systemParamIntArrayByName2[0];
				num = Global.GetGoodsIconCodeByID(id);
			}
			else if (bufferData.BufferID == 2090002)
			{
				int[] systemParamIntArrayByName3 = ConfigSystemParam.GetSystemParamIntArrayByName("EscapeBuffID", ',');
				id = systemParamIntArrayByName3[1];
				num = Global.GetGoodsIconCodeByID(id);
			}
			else
			{
				int bufferID = bufferData.BufferID;
				switch (bufferID)
				{
				case 16:
				case 49:
				case 50:
				case 51:
					id = (int)bufferData.BufferVal;
					num = Global.GetGoodsIconCodeByID(id);
					goto IL_866;
				default:
					switch (bufferID)
					{
					case 99:
					{
						string systemParamByName = ConfigSystemParam.GetSystemParamByName("WorldLevelGoodsIDs", true);
						id = systemParamByName.SafeToInt32(0);
						type = bufferData.BufferType;
						num = Global.GetGoodsIconCodeByID(id);
						goto IL_866;
					}
					case 100:
						id = ConfigSystemParam.GetSystemParamIntArrayByName("PingBanBuff", ',')[0];
						type = bufferData.BufferType;
						num = Global.GetGoodsIconCodeByID(id);
						goto IL_866;
					default:
						if (bufferID == 122)
						{
							id = (int)(bufferData.BufferVal >> 32);
							num = Global.GetGoodsIconCodeByID(id);
							goto IL_866;
						}
						if (bufferID == 123)
						{
							int num2 = (int)(bufferData.BufferVal & (long)((ulong)-1));
							double num3 = (double)(bufferData.BufferVal - (long)num2) / Math.Pow(2.0, 32.0);
							num = Global.GetGoodsIconCodeByID((int)num3);
							goto IL_866;
						}
						if (bufferID != 1)
						{
							if (bufferID != 115)
							{
								id = (int)bufferData.BufferVal;
								num = Global.GetGoodsIconCodeByID(id);
								goto IL_866;
							}
							id = ConfigSystemParam.GetSystemParamIntArrayByName("ManorBuffID", ',')[0];
							type = bufferData.BufferType;
							num = Global.GetGoodsIconCodeByID(id);
							goto IL_866;
						}
						break;
					case 104:
					{
						id = 0;
						Dictionary<int, int[]> systemParamIntDictByName = ConfigSystemParam.GetSystemParamIntDictByName("BuffMagic", '|', ',');
						if (systemParamIntDictByName.Count > 0)
						{
							id = systemParamIntDictByName[5][0];
						}
						type = bufferData.BufferType;
						return Global.GetSkillBuffIconString(id);
					}
					}
					break;
				case 18:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
				case 29:
				case 36:
				case 39:
				case 47:
					break;
				case 46:
				{
					int num4 = (int)(bufferData.BufferVal & (long)((ulong)-1));
					double num5 = (double)(bufferData.BufferVal - (long)num4) / Math.Pow(2.0, 32.0);
					num = Global.GetGoodsIconCodeByID((int)num5);
					goto IL_866;
				}
				case 48:
				{
					int num6 = (int)(bufferData.BufferVal & (long)((ulong)-1));
					double num7 = (double)(bufferData.BufferVal - (long)num6) / Math.Pow(2.0, 32.0);
					id = (int)num7;
					num = Global.GetGoodsIconCodeByID((int)num7);
					goto IL_866;
				}
				}
				num = Global.GetGoodsIconCodeByID((int)bufferData.BufferVal);
			}
			IL_866:
			return Global.GetGoodsIconString(num);
		}
		int[] systemParamIntArrayByName4 = ConfigSystemParam.GetSystemParamIntArrayByName("AngelTempleGoldBuffGoodsID", ',');
		if (bufferData.BufferID == 85)
		{
			num = Global.GetGoodsIconCodeByID(systemParamIntArrayByName4[0]);
			return Global.GetGoodsIconString(num);
		}
		num = Global.GetGoodsIconCodeByID(systemParamIntArrayByName4[1]);
		return Global.GetGoodsIconString(num);
	}

	private void AddIcon(BufferData bufferData)
	{
		int type = 0;
		int goodsID = -1;
		string bufferIconString = DBBufferBox.GetBufferIconString(bufferData, ref type, ref goodsID);
		GStillIcon gstillIcon = U3DUtils.NEW<GStillIcon>("DBBufferBoxItem");
		gstillIcon.ShowText = true;
		gstillIcon.ShowCDMask = false;
		gstillIcon.Width = 48.0;
		gstillIcon.Height = 48.0;
		gstillIcon.BodyURL = bufferIconString;
		gstillIcon.IconType = GStillIcon.GSillIconType.Buff;
		gstillIcon.Tag = bufferData;
		gstillIcon.type = type;
		gstillIcon.GoodsID = goodsID;
		gstillIcon.TipType = 5;
		gstillIcon.StillIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(gstillIcon.NormalBuffTipsHandler);
		this.boxIcons.Add(gstillIcon);
		this.boxIcons.DelayUpdate();
		this.UpdateCoolDown(gstillIcon, bufferData);
	}

	private void UpdateCoolDown(GStillIcon dbBufferBoxItem, BufferData bufferData)
	{
		if (bufferData.BufferID == 116)
		{
			return;
		}
		if (Global.IsInDaTaoSha())
		{
			if (bufferData.BufferID == 2090002)
			{
				dbBufferBoxItem.Percent = 1f;
				dbBufferBoxItem.SkillText = "X" + bufferData.BufferVal;
			}
			else if (bufferData.BufferID == 2090001)
			{
				dbBufferBoxItem.Percent = 1f;
				dbBufferBoxItem.SkillText = "X" + bufferData.BufferVal;
			}
			return;
		}
		if (bufferData.StartTime == 0L || bufferData.BufferSecs <= 0)
		{
			dbBufferBoxItem.Percent = 1f;
			return;
		}
		if (bufferData.BufferID == 4 || bufferData.BufferID == 5 || bufferData.BufferID == 10 || bufferData.BufferID == 28)
		{
			dbBufferBoxItem.Percent = (float)(bufferData.BufferVal / bufferData.StartTime);
		}
		else if (bufferData.BufferID != 34)
		{
			if (bufferData.BufferID == 115)
			{
				dbBufferBoxItem.Percent = 1f;
				dbBufferBoxItem.SkillText = string.Empty;
			}
			else if (bufferData.BufferType <= 0 && bufferData.BufferID != 28 && bufferData.BufferID != 33 && bufferData.BufferID != 31)
			{
				long correctLocalTime = Global.GetCorrectLocalTime();
				long num = (long)bufferData.BufferSecs * 1000L;
				long num2 = Math.Max(correctLocalTime - bufferData.StartTime, 0L);
				if (num2 < num)
				{
					dbBufferBoxItem.Percent = 1f;
					dbBufferBoxItem.MyStart(num, false, 1000, bufferData.StartTime, true, true);
				}
			}
			else if (bufferData.BufferID >= 52)
			{
				long correctLocalTime2 = Global.GetCorrectLocalTime();
				long num3 = (long)bufferData.BufferSecs * 1000L;
				long num4 = Math.Max(correctLocalTime2 - bufferData.StartTime, 0L);
				if (num4 < num3)
				{
					dbBufferBoxItem.Percent = 1f;
					dbBufferBoxItem.MyStart(num3, false, 1000, bufferData.StartTime, true, true);
				}
			}
			else if (bufferData.BufferID == 31)
			{
				dbBufferBoxItem.Percent = 1f;
			}
			else if (bufferData.BufferID == 49)
			{
				dbBufferBoxItem.Percent = 1f;
			}
			else if (bufferData.BufferID == 2090002)
			{
				dbBufferBoxItem.Percent = 1f;
				dbBufferBoxItem.SkillText = "X" + bufferData.BufferVal;
			}
			else if (bufferData.BufferID == 2090001)
			{
				dbBufferBoxItem.Percent = 1f;
				dbBufferBoxItem.SkillText = "X" + bufferData.BufferVal;
			}
			else
			{
				long correctLocalTime3 = Global.GetCorrectLocalTime();
				long num5 = (long)bufferData.BufferSecs * 1000L;
				long num6 = Math.Max(correctLocalTime3 - bufferData.StartTime, 0L);
				if (num6 < num5)
				{
					dbBufferBoxItem.Percent = 1f;
					dbBufferBoxItem.MyStart(num5, false, 1000, bufferData.StartTime, true, true);
				}
				else
				{
					dbBufferBoxItem.Percent = 0f;
				}
				dbBufferBoxItem.SkillText = ((BufferItemTypes)bufferData.BufferID).ToString();
			}
		}
	}

	private void UpdateCoolDown(DBBufferBoxItem dbBufferBoxItem, BufferData bufferData)
	{
		if (bufferData.BufferID == 4 || bufferData.BufferID == 5 || bufferData.BufferID == 10)
		{
			dbBufferBoxItem.CoolDown.ManualSet((double)((int)bufferData.StartTime), (double)bufferData.BufferVal, false);
		}
		else if (bufferData.BufferID == 28)
		{
			dbBufferBoxItem.CoolDown.ManualSet((double)((int)bufferData.StartTime), (double)bufferData.BufferVal, false);
		}
		else if (bufferData.BufferID != 34)
		{
			if (bufferData.BufferID != 2090001 && bufferData.BufferID != 2090002)
			{
				if (bufferData.BufferType <= 0 && bufferData.BufferID != 28 && bufferData.BufferID != 33 && bufferData.BufferID != 31)
				{
					long correctLocalTime = Global.GetCorrectLocalTime();
					long num = (long)bufferData.BufferSecs * 1000L;
					long num2 = correctLocalTime - bufferData.StartTime;
					if (num2 < num)
					{
						dbBufferBoxItem.CoolDown.MyStart(num, false, 3000, bufferData.StartTime, true, true);
					}
				}
				else
				{
					dbBufferBoxItem.CoolDown.ManualSet(1.0, 1.0, false);
				}
			}
		}
	}

	public void CheckBufferDataList()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		for (int i = 0; i < this.boxIcons.Count; i++)
		{
			GStillIcon gstillIcon = U3DUtils.AS<GStillIcon>(this.boxIcons[i]);
			BufferData bufferData = gstillIcon.Tag as BufferData;
			if (bufferData != null && Global.IsBufferDataOver(bufferData, correctLocalTime, false))
			{
				this.boxIcons.RemoveAtNoUpdate(i);
			}
		}
		this.boxIcons.DelayUpdate();
		if (this.boxIcons.Count <= 0)
		{
			this.Visibility = false;
		}
		else
		{
			int num = (this.boxIcons.Count - 1) / 4 + 1;
			this._Bak.transform.localScale = new Vector3(320f, (float)(4 + 68 * num), 0f);
		}
	}

	public void RepositionIcons()
	{
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < this.boxIcons.Count; i++)
		{
			DBBufferBoxItem obj = U3DUtils.AS<DBBufferBoxItem>(this.boxIcons[i]);
			if (num >= 170.0)
			{
				num = 0.0;
				num2 += 34.0;
			}
			Canvas.SetLeft(obj, num);
			num += 34.0;
			Canvas.SetTop(obj, num2);
		}
		this.Container.Width = num + 34.0;
		this.Container.Height = num2 + 34.0;
	}

	public static DBBufferBox GGetInstance()
	{
		if (null == DBBufferBox._Instance)
		{
			DBBufferBox._Instance = U3DUtils.NEW<DBBufferBox>();
		}
		return DBBufferBox._Instance;
	}

	public static void GClose()
	{
		if (null != DBBufferBox._Instance)
		{
			Object.Destroy(DBBufferBox._Instance.gameObject);
			DBBufferBox._Instance = null;
		}
	}

	public static DBBufferBox GShow()
	{
		if (null == DBBufferBox._Instance)
		{
			DBBufferBox.GGetInstance();
		}
		if (null != DBBufferBox._Instance)
		{
			DBBufferBox._Instance.gameObject.SetActive(true);
		}
		return DBBufferBox._Instance;
	}

	public static void GHide()
	{
		if (null != DBBufferBox._Instance)
		{
			DBBufferBox._Instance.gameObject.SetActive(false);
		}
	}

	public DBBufferBox Show()
	{
		if (null != DBBufferBox._Instance)
		{
			this.boxIcons.Clear();
			base.gameObject.SetActive(true);
		}
		return this;
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public static DBBufferBox _Instance;

	public UISprite _Bak;

	public UISprite _Grid;

	public GButton _Close;

	private string[] IconIDs;

	[SerializeField]
	protected ListBox _boxIcons;
}
